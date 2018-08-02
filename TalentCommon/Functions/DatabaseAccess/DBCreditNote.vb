Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries

'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with CreditNote Requests
'
'       Date                        Nov 2006
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDBCN- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBCreditNote
    Inherits DBAccess

    Private _dep As New DECreditNote
    Private _dtHeaderResults As New DataTable
    Private _dtItemResults As New DataTable
    Private _parmTRAN As String


    Public Property Dep() As DECreditNote
        Get
            Return _dep
        End Get
        Set(ByVal value As DECreditNote)
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

    Public Property DtHeaderResults() As DataTable
        Get
            Return _dtHeaderResults
        End Get
        Set(ByVal value As DataTable)
            _dtHeaderResults = value
        End Set
    End Property
    Public Property DtItemResults() As DataTable
        Get
            Return _dtItemResults
        End Get
        Set(ByVal value As DataTable)
            _dtItemResults = value
        End Set
    End Property

    Public Function ReadCreditNote(Optional ByVal opendb As Boolean = False) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------
        Select Case Settings.DestinationDatabase
            Case Is = SYSTEM21
                If opendb Then err = System21Open()
                If Not err.HasError Then err = ReadCreditNoteSystem21()
                If opendb Then System21Close()
            Case Is = SQL2005
        End Select
        Return err
    End Function
    Public Function UpdateCreditNote(Optional ByVal opendb As Boolean = False) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------
        Select Case Settings.DestinationDatabase
            Case Is = SYSTEM21
                If opendb Then err = System21Open()
                If Not err.HasError Then err = UpdateCreditNoteSystem21()
                If opendb Then err = System21Close()
            Case Is = SQL2005
        End Select
        Return err
    End Function

    Protected Overrides Function AccessDataBaseSystem21() As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------
        '   Create command object
        '
        Dim cmdSELECT As iDB2Command = Nothing

        Dim parmInput, Paramoutput As iDB2Parameter
        Dim PARMOUT As String = String.Empty
        ' Const strHEADER As String = "CALL WESTCOAST/INVCEREQ(@PARAM1, @PARAM2)"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/INVCEREQ(@PARAM1, @PARAM2)"

        If Not err.HasError Then
            Try
                cmdSELECT = New iDB2Command(strHEADER, conSystem21)

                parmInput = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                parmInput.Value = Utilities.FixStringLength(Settings.AccountNo3, 2) & _
                                    Utilities.FixStringLength(Settings.AccountNo1, 10) & _
                                    Utilities.FixStringLength(Settings.AccountNo2, 3) & "CR"
                parmInput.Direction = ParameterDirection.Input

                Paramoutput = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                Paramoutput.Value = String.Empty
                Paramoutput.Direction = ParameterDirection.InputOutput
                cmdSELECT.ExecuteNonQuery()
                PARMOUT = cmdSELECT.Parameters(Param2).Value.ToString
                '
            Catch ex As Exception
                Const strError8 As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8
                    .ErrorNumber = "TACDBCN-02"
                    .HasError = True
                End With
            End Try
            '----------------------------------------------------------------------
            ' Read the CreditNotes back out 
            '
            If Not err.HasError Then
                err = ReadCreditNote()
                '------------------------------------------------------------------
                ' Update the CreditNotes as processed 
                '
                If Not err.HasError Then _
                    err = UpdateCreditNote()
            End If
        End If
        '------------------------------------------------------------------------------
        Return err
    End Function

    Private Function DataEntityUnPack() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------------
        '   put the parameter generation in its own function as it is likely to be used  
        '   several times
        '
        Dim detr As New DETransaction           ' Items
        Dim iItems As Integer = 0
        '
        Try
            detr = Dep.CollDETrans.Item(1)
            Const sTran As String = "SenderID = {0}, ReceiverID = {1}, CountryCode = {2}, TransactionID = {3}, ShowDetail = {4}"
            With detr
                ParmTRAN = String.Format(sTran, .SenderID, _
                                            .ReceiverID, _
                                            .CountryCode, _
                                            .TransactionID, _
                                            .ShowDetail)
            End With

        Catch ex As Exception
            Const strError1 As String = "Could not Unpack Data Entity "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError1
                .ErrorNumber = "TACDBCN-19"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Return ReadCreditNote_Sql2005()
    End Function

    Protected Function ReadCreditNote_Sql2005() As ErrorObj
        Dim err As New ErrorObj

        Dim cmd As New Data.SqlClient.SqlCommand()

        cmd.Parameters.Clear()
        cmd.Connection = New Data.SqlClient.SqlConnection(Settings.FrontEndConnectionString)
        cmd.CommandText = " SELECT * " & _
                          "     FROM tbl_CreditNote_header WITH (NOLOCK)  " & _
                          " WHERE BUSINESS_UNIT = @bu " & _
                          " AND LOGINID = @loginID " & _
                          " AND FINALISED = @finalised "

        Try 'default to true if not set
            Dim fin As Boolean = Dep.Finalised
        Catch ex As Exception
            Dep.Finalised = True
        End Try

        cmd = BuildWhereClause(cmd)
        cmd.CommandText += " ORDER BY CreditNote_DATE DESC "
        With cmd.Parameters
            .Add("@bu", Data.SqlDbType.NVarChar).Value = Dep.BusinessUnit
            .Add("@loginID", Data.SqlDbType.NVarChar).Value = Dep.Username
            .Add("@finalised", Data.SqlDbType.NVarChar).Value = Dep.Finalised
        End With

        Try
            cmd.Connection.Open()

            Dim dr As SqlDataReader = cmd.ExecuteReader
            DtHeaderResults = New DataTable("CreditNoteEnquiryHeader")
            DtHeaderResults.Rows.Clear()
            DtHeaderResults.Columns.Clear()
            AddColumnsToDataTables()

            If dr.HasRows Then

                While dr.Read
                    Dim dRow As DataRow = DtHeaderResults.NewRow
                    dRow = Nothing
                    dRow = DtHeaderResults.NewRow()
                    dRow("OrderNumber") = Utilities.CheckForDBNull_String(dr("ORDER_NO"))
                    dRow("CustomerCompanyCode") = "" 'Utilities.CheckForDBNull_String(dr(""))
                    dRow("BusinessUnit") = Utilities.CheckForDBNull_String(dr("BUSINESS_UNIT"))
                    dRow("Partner") = Utilities.CheckForDBNull_String(dr("PARTNER"))
                    dRow("PartnerNumber") = Utilities.CheckForDBNull_String(dr("PARTNER_NUMBER"))
                    dRow("LoginID") = Utilities.CheckForDBNull_String(dr("LOGINID"))
                    dRow("CustomerNumber") = Utilities.CheckForDBNull_String(dr("CUSTOMER_REF"))
                    dRow("Finalised") = Utilities.CheckForDBNull_Boolean_DefaultFalse(dr("FINALISED"))
                    dRow("CreditNoteNumber") = Utilities.CheckForDBNull_String(dr("CreditNote_NO"))
                    dRow("CreditNoteDateTime") = Utilities.CheckForDBNull_DateTime(dr("CreditNote_DATE"))
                    dRow("CreditNoteAmount") = Utilities.CheckForDBNull_Decimal(dr("CreditNote_AMOUNT"))
                    dRow("VatAmount") = Utilities.CheckForDBNull_Decimal(dr("VAT_AMOUNT"))
                    dRow("ChargesAmount") = 0 'Utilities.CheckForDBNull_String(dr(""))
                    dRow("OutstandingAmount") = Utilities.CheckForDBNull_String(dr("OUTSTANDING_AMOUNT"))
                    dRow("CreditNoteProcessed") = Utilities.CheckForDBNull_String(dr("CreditNote_STATUS"))
                    dRow("CurrencyCode") = "" 'Utilities.CheckForDBNull_String(dr(""))
                    dRow("CustomerPO") = Utilities.CheckForDBNull_String(dr("CUSTOMER_PO"))
                    dRow("OriginalOrderNo") = Utilities.CheckForDBNull_String(dr("ORIGINAL_ORDER_NO"))
                    dRow("OriginalOrderDate") = Utilities.CheckForDBNull_DateTime(dr("ORIGINAL_ORDER_DATE"))
                    dRow("DispatchSequence") = Utilities.CheckForDBNull_String(dr("DISPATCH_SEQUENCE"))
                    dRow("AccountNumber") = Utilities.CheckForDBNull_String(dr("ACCOUNT_NUMBER"))
                    dRow("CustomerName") = Utilities.CheckForDBNull_String(dr("CUSTOMER_NAME"))
                    dRow("CustomerAttention") = Utilities.CheckForDBNull_String(dr("CUSTOMER_ATTENTION"))
                    dRow("CustomerAddress1") = Utilities.CheckForDBNull_String(dr("CUSTOMER_ADDRESS_1"))
                    dRow("CustomerAddress2") = Utilities.CheckForDBNull_String(dr("CUSTOMER_ADDRESS_2"))
                    dRow("CustomerAddress3") = Utilities.CheckForDBNull_String(dr("CUSTOMER_ADDRESS_3"))
                    dRow("CustomerAddress4") = Utilities.CheckForDBNull_String(dr("CUSTOMER_ADDRESS_4"))
                    dRow("CustomerAddress5") = Utilities.CheckForDBNull_String(dr("CUSTOMER_ADDRESS_5"))
                    dRow("CustomerAddress6") = Utilities.CheckForDBNull_String(dr("CUSTOMER_ADDRESS_6"))
                    dRow("CustomerAddress7") = Utilities.CheckForDBNull_String(dr("CUSTOMER_ADDRESS_7"))
                    dRow("ShipToName") = Utilities.CheckForDBNull_String(dr("SHIPTO_NAME"))
                    dRow("ShipToAttention") = Utilities.CheckForDBNull_String(dr("SHIPTO_ATTENTION"))
                    dRow("ShipToAddress1") = Utilities.CheckForDBNull_String(dr("SHIPTO_ADDRESS_1"))
                    dRow("ShipToAddress2") = Utilities.CheckForDBNull_String(dr("SHIPTO_ADDRESS_2"))
                    dRow("ShipToAddress3") = Utilities.CheckForDBNull_String(dr("SHIPTO_ADDRESS_3"))
                    dRow("ShipToAddress4") = Utilities.CheckForDBNull_String(dr("SHIPTO_ADDRESS_4"))
                    dRow("ShipToAddress5") = Utilities.CheckForDBNull_String(dr("SHIPTO_ADDRESS_5"))
                    dRow("ShipToAddress6") = Utilities.CheckForDBNull_String(dr("SHIPTO_ADDRESS_6"))
                    dRow("ShipToAddress7") = Utilities.CheckForDBNull_String(dr("SHIPTO_ADDRESS_7"))
                    dRow("ShipFromName") = Utilities.CheckForDBNull_String(dr("SHIPFROM_NAME"))
                    dRow("ShipFromAttention") = Utilities.CheckForDBNull_String(dr("SHIPFROM_ATTENTION"))
                    dRow("ShipFromAddress1") = Utilities.CheckForDBNull_String(dr("SHIPFROM_ADDRESS_1"))
                    dRow("ShipFromAddress2") = Utilities.CheckForDBNull_String(dr("SHIPFROM_ADDRESS_2"))
                    dRow("ShipFromAddress3") = Utilities.CheckForDBNull_String(dr("SHIPFROM_ADDRESS_3"))
                    dRow("ShipFromAddress4") = Utilities.CheckForDBNull_String(dr("SHIPFROM_ADDRESS_4"))
                    dRow("ShipFromAddress5") = Utilities.CheckForDBNull_String(dr("SHIPFROM_ADDRESS_5"))
                    dRow("ShipFromAddress6") = Utilities.CheckForDBNull_String(dr("SHIPFROM_ADDRESS_6"))
                    dRow("ShipFromAddress7") = Utilities.CheckForDBNull_String(dr("SHIPFROM_ADDRESS_7"))
                    dRow("VATNumber") = Utilities.CheckForDBNull_String(dr("VAT_NUMBER"))
                    dRow("PaymentTermsType") = Utilities.CheckForDBNull_String(dr("PAYMENT_TERMS_TYPE"))
                    dRow("PaymentTermsPeriod") = Utilities.CheckForDBNull_Int(dr("PAYMENT_TERMS_PERIOD"))
                    dRow("PaymentTermsDays") = Utilities.CheckForDBNull_Int(dr("PAYMENT_TERMS_DAYS"))
                    DtHeaderResults.Rows.Add(dRow)
                End While
            End If

            dr.Close()

            ResultDataSet = New DataSet("CreditNoteEnquiry")
            ResultDataSet.Tables.Add(DtHeaderResults)

        Catch ex As Exception
            'error with DB connection/command execution
            err.HasError = True
            err.ErrorMessage = ex.Message
            err.ErrorNumber = "TACDBIN50"
        Finally
            cmd.Connection.Close()
            cmd.Connection.Dispose()
            cmd.Dispose()
        End Try

        Return err
    End Function


    Protected Function BuildWhereClause(ByVal cmd As Data.SqlClient.SqlCommand) As Data.SqlClient.SqlCommand
        Dim cnNo As String = "", _
            OrdNo As String = "", _
            ToDate As String = "", _
            FromDate As String = ""

        Try
            cnNo = Dep.CreditNoteNumber
        Catch ex As Exception
        End Try
        Try
            OrdNo = Dep.OrderNumber
        Catch ex As Exception
        End Try
        Try
            ToDate = Dep.ToDate.ToString
        Catch ex As Exception
        End Try
        Try
            FromDate = Dep.FromDate.ToString
        Catch ex As Exception
        End Try

        With cmd

            'If an CreditNote Number has been specified, add the where clause
            If Not String.IsNullOrEmpty(cnNo) Then
                .CommandText += " AND CreditNote_NO = @CreditNoteNo "
                .Parameters.Add("@CreditNoteNo", Data.SqlDbType.NVarChar).Value = cnNo
            End If

            'If an Order Number has been specified, add the where clause
            If Not String.IsNullOrEmpty(OrdNo) Then
                .CommandText += " AND ORDER_NO = @orderNo "
                .Parameters.Add("@orderNo", Data.SqlDbType.NVarChar).Value = OrdNo
            End If

            'If a From Date has been specified, add the where clause
            If Not String.IsNullOrEmpty(FromDate) _
                        AndAlso Not FromDate =  Date.MinValue.ToString Then

                Dim d As DateTime
                Try
                    d = CType(FromDate, DateTime)
                    .CommandText += " AND CreditNote_DATE >= @FromDate "
                    .Parameters.Add("@FromDate", Data.SqlDbType.DateTime).Value = d
                Catch ex As Exception
                    'Catch date error
                End Try
            End If

            'If a To Date has been specified, add the where clause
            If Not String.IsNullOrEmpty(ToDate) _
                                AndAlso Not FromDate = ToDate _
                                AndAlso Not ToDate =  Date.MinValue.ToString Then

                Dim d As DateTime
                Try
                    d = CType(ToDate, DateTime)
                    .CommandText += " AND CreditNote_DATE <= @ToDate "
                    .Parameters.Add("@ToDate", Data.SqlDbType.DateTime).Value = d
                Catch ex As Exception
                    'Catch date error
                End Try
            End If

            'If Status.SelectedIndex > 0 Then
            '    .CommandText += " AND STATUS = @status "
            '    .Parameters.Add("@status", Data.SqlDbType.NVarChar).Value = Status.Text
            'End If

        End With
        Return cmd
    End Function

    Private Function ReadCreditNoteSystem21() As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------
        ResultDataSet = New DataSet
        DtHeaderResults = New DataTable("CreditNoteEnquiryHeader")
        DtItemResults = New DataTable("CreditNoteEnquiryDetail")
        ResultDataSet.Tables.Add(DtHeaderResults)
        ResultDataSet.Tables.Add(DtItemResults)
        '----------------------------------------------------------------------
        Try

            Dim field1Ordinal, field2Ordinal, field3Ordinal, field4Ordinal, field5Ordinal As String
            Dim field6Ordinal, field7Ordinal, field8Ordinal, field9Ordinal, field10Ordinal As String
            Dim field11Ordinal, field12Ordinal, field13Ordinal, field14Ordinal, field15Ordinal As String
            Dim field16Ordinal, field17Ordinal, field18Ordinal, field19Ordinal, field20Ordinal As String
            Dim field21Ordinal, field22Ordinal, field23Ordinal, field24Ordinal, field25Ordinal As String
            Dim field26Ordinal, field27Ordinal, field28Ordinal, field29Ordinal, field30Ordinal As String
            Dim field31Ordinal, field32Ordinal, field33Ordinal, field34Ordinal, field35Ordinal As String

            field1Ordinal = String.Empty
            field2Ordinal = String.Empty
            field3Ordinal = String.Empty
            field4Ordinal = String.Empty
            field5Ordinal = String.Empty
            field6Ordinal = String.Empty
            field7Ordinal = String.Empty
            field8Ordinal = String.Empty
            field9Ordinal = String.Empty
            field10Ordinal = String.Empty
            field11Ordinal = String.Empty
            field12Ordinal = String.Empty
            field13Ordinal = String.Empty
            field14Ordinal = String.Empty
            field15Ordinal = String.Empty
            field16Ordinal = String.Empty
            field17Ordinal = String.Empty
            field18Ordinal = String.Empty
            field19Ordinal = String.Empty
            field20Ordinal = String.Empty
            field21Ordinal = String.Empty
            field22Ordinal = String.Empty
            field23Ordinal = String.Empty
            field24Ordinal = String.Empty
            field25Ordinal = String.Empty
            field26Ordinal = String.Empty
            field27Ordinal = String.Empty
            field28Ordinal = String.Empty
            field29Ordinal = String.Empty
            field30Ordinal = String.Empty
            field31Ordinal = String.Empty
            field32Ordinal = String.Empty
            field33Ordinal = String.Empty
            field34Ordinal = String.Empty
            field35Ordinal = String.Empty

            Dim field1OrdinalI, field2OrdinalI, field3OrdinalI, field4OrdinalI, field5OrdinalI As String
            Dim field6OrdinalI, field7OrdinalI, field8OrdinalI, field9OrdinalI As String

            field1OrdinalI = String.Empty
            field2OrdinalI = String.Empty
            field3OrdinalI = String.Empty
            field4OrdinalI = String.Empty
            field5OrdinalI = String.Empty
            field6OrdinalI = String.Empty
            field7OrdinalI = String.Empty
            field8OrdinalI = String.Empty
            field9OrdinalI = String.Empty

            '--------------------
            ' Read CreditNote header
            '--------------------
            Dim orderNumber As String = String.Empty
            Dim customerCompanyCode As String = String.Empty
            Dim customerNumber As String = String.Empty
            Dim CreditNoteNumber As String = String.Empty
            Dim CreditNoteDateTime As Date = Date.MinValue
            Dim CreditNoteAmount As Decimal = 0
            Dim vatAmount As Decimal = 0
            Dim CreditNoteProcessed As String = String.Empty
            Dim currencyCode As String = String.Empty
            Dim customerPO As String = String.Empty
            Dim accountNumber As String = String.Empty
            Dim customerName As String = String.Empty
            Dim customerAttention As String = String.Empty
            Dim customerAddressLine1 As String = String.Empty
            Dim customerAddressLine2 As String = String.Empty
            Dim customerAddressLine3 As String = String.Empty
            Dim customerAddressLine4 As String = String.Empty
            Dim customerAddressLine5 As String = String.Empty
            Dim customerAddressLine6 As String = String.Empty
            Dim customerAddressLine7 As String = String.Empty
            Dim shipToName As String = String.Empty
            Dim shipToAttention As String = String.Empty
            Dim shipToAddressLine1 As String = String.Empty
            Dim shipToAddressLine2 As String = String.Empty
            Dim shipToAddressLine3 As String = String.Empty
            Dim shipToAddressLine4 As String = String.Empty
            Dim shipToAddressLine5 As String = String.Empty
            Dim shipToAddressLine6 As String = String.Empty
            Dim shipToAddressLine7 As String = String.Empty
            Dim shipFromName As String = String.Empty
            Dim shipFromAttention As String = String.Empty
            Dim shipFromAddressLine1 As String = String.Empty
            Dim shipFromAddressLine2 As String = String.Empty
            Dim shipFromAddressLine3 As String = String.Empty
            Dim shipFromAddressLine4 As String = String.Empty
            Dim shipFromAddressLine5 As String = String.Empty
            Dim shipFromAddressLine6 As String = String.Empty
            Dim shipFromAddressLine7 As String = String.Empty
            Dim vatNumber As String = String.Empty
            Dim paymentTermsType As String = String.Empty
            Dim paymentTermsPeriod As Integer = 0
            Dim paymentTermsDays As Integer = 0

            Dim cmdSelectHeader As iDB2Command = Nothing
            Dim dtrReaderHeader As iDB2DataReader = Nothing
            Dim dRow As DataRow = Nothing

            Const sqlSelectHeader As String = "SELECT * FROM XMIVP100 WHERE I1CUS = @PARAM1 AND I1SHP =@PARAM2 AND I1PRC = 'N'"
            cmdSelectHeader = New iDB2Command(sqlSelectHeader, conSystem21)

            '--------------------------------------------------------------------------------------
            ' Only read CreditNotes for partner
            '
            With cmdSelectHeader
                With .Parameters
                    .Add(Param1, iDB2DbType.iDB2VarChar, 10).Value = Settings.AccountNo1
                    .Add(Param2, iDB2DbType.iDB2VarChar, 10).Value = Settings.AccountNo2
                End With
                dtrReaderHeader = .ExecuteReader()
            End With

            With dtrReaderHeader
                field1Ordinal = .GetOrdinal("I1ORD")
                field2Ordinal = .GetOrdinal("I1SHP")
                field3Ordinal = .GetOrdinal("I1CUS")
                field4Ordinal = .GetOrdinal("I1INV")
                field5Ordinal = .GetOrdinal("I1PRC")
                field6Ordinal = .GetOrdinal("I1CUR")
                field7Ordinal = .GetOrdinal("I1IDT")
            End With
            '--------------------------------------------------------------------------------------

            'If Not dtrReaderHeader.HasRows Then
            '    Const strError11 As String = "No data to display"
            '    With err
            '        .ErrorMessage = String.Empty
            '        .ErrorNumber = "TACDBCN-33"
            '        .ErrorStatus = strError11
            '        .HasError = True
            '    End With
            'End If

            While dtrReaderHeader.Read
                '-------------------------------------------------------------------------------
                With dtrReaderHeader
                    orderNumber = .GetString(field1Ordinal)
                    customerCompanyCode = .GetString(field2Ordinal)
                    customerNumber = .GetString(field3Ordinal)
                    CreditNoteNumber = .GetString(field4Ordinal)
                    'CreditNoteDateTime = ("I1IDT") & dtrReaderHeader("I1ITM")
                    Dim y As String = .GetiDB2Date(field7Ordinal).Year.ToString
                    Dim m As String = .GetiDB2Date(field7Ordinal).Month.ToString
                    Dim d As String = .GetiDB2Date(field7Ordinal).Day.ToString
                    Dim dt As String = d & "/" & m & "/" & y
                    '---------------------------------------------------------------------------
                    Dim ts As String = .Item("I1ITM").ToString
                    Dim h As String = "00"
                    Try
                        h = ts.Substring(0, 2)
                    Catch ex As Exception
                    End Try
                    '----------------------------------------------------------------------------
                    Dim mn As String = "00"
                    Try
                    Catch ex As Exception
                        mn = ts.Substring(2, 2)
                    End Try
                    '----------------------------------------------------------------------------
                    Dim s As String = "00"
                    Try
                        s = ts.Substring(4, 2)
                    Catch ex As Exception
                    End Try
                    '----------------------------------------------------------------------------
                    Dim tm As String = h & ":" & mn & ":" & s
                    CreditNoteDateTime = Date.Parse(dt & " " & tm)
                    CreditNoteAmount = .Item("I1IAM")
                    vatAmount = .Item("I1VAM")
                    CreditNoteProcessed = .GetString(field5Ordinal)
                    currencyCode = .GetString(field6Ordinal)
                    accountNumber = Settings.AccountNo3.Trim
                End With
                '-------------------------------------------------------------------------------
                Dim cmdSelectXXCOP100 As iDB2Command = Nothing
                Dim dtrReaderXXCOP100 As iDB2DataReader = Nothing
                Const sqlSelectXXCOP100 As String = "SELECT * FROM XXCOP100 WHERE C1ORD = @PARAM1"
                cmdSelectXXCOP100 = New iDB2Command(sqlSelectXXCOP100, conSystem21)
                cmdSelectXXCOP100.Parameters.Add(Param1, iDB2DbType.iDB2VarChar, 10).Value = orderNumber
                dtrReaderXXCOP100 = cmdSelectXXCOP100.ExecuteReader()
                '-------------------------------------------------------------------------------
                With dtrReaderXXCOP100
                    field8Ordinal = .GetOrdinal("C1PON")
                    field27Ordinal = .GetOrdinal("C1NAM")
                    field29Ordinal = .GetOrdinal("C1AD1")
                    field30Ordinal = .GetOrdinal("C1AD2")
                    field31Ordinal = .GetOrdinal("C1AD3")
                    field32Ordinal = .GetOrdinal("C1AD4")
                    field33Ordinal = .GetOrdinal("C1AD5")
                    field34Ordinal = .GetOrdinal("C1PCD")
                    field35Ordinal = .GetOrdinal("C1CCD")
                    If .HasRows Then
                        .Read()
                        customerPO = .GetString(field8Ordinal)
                        shipFromName = .GetString(field27Ordinal)
                        shipFromAddressLine1 = .GetString(field29Ordinal)
                        shipFromAddressLine2 = .GetString(field30Ordinal)
                        shipFromAddressLine3 = .GetString(field31Ordinal)
                        shipFromAddressLine4 = .GetString(field32Ordinal)
                        shipFromAddressLine5 = .GetString(field33Ordinal)
                        shipFromAddressLine6 = .GetString(field34Ordinal)
                        shipFromAddressLine7 = .GetString(field35Ordinal)
                    End If
                    .Close()
                End With
                '-------------------------------------------------------------------------------
                Dim cmdSelectXXCOP200 As iDB2Command = Nothing
                Dim dtrReaderXXCOP200 As iDB2DataReader = Nothing
                Const sqlSelectXXCOP200 As String = "SELECT * FROM XXCOP200 WHERE C2ORD = @PARAM1"
                cmdSelectXXCOP200 = New iDB2Command(sqlSelectXXCOP200, conSystem21)
                cmdSelectXXCOP200.Parameters.Add(Param1, iDB2DbType.iDB2VarChar, 10).Value = orderNumber
                dtrReaderXXCOP200 = cmdSelectXXCOP200.ExecuteReader()
                '-------------------------------------------------------------------------------
                With dtrReaderXXCOP200
                    field18Ordinal = .GetOrdinal("C2NAM")
                    field20Ordinal = .GetOrdinal("C2AD1")
                    field21Ordinal = .GetOrdinal("C2AD2")
                    field22Ordinal = .GetOrdinal("C2AD3")
                    field23Ordinal = .GetOrdinal("C2AD4")
                    field24Ordinal = .GetOrdinal("C2AD5")
                    field25Ordinal = .GetOrdinal("C2PCD")
                    field26Ordinal = .GetOrdinal("C2CCD")
                    If .HasRows Then
                        .Read()
                        shipToName = .GetString(field18Ordinal)
                        shipToAddressLine1 = .GetString(field20Ordinal)
                        shipToAddressLine2 = .GetString(field21Ordinal)
                        shipToAddressLine3 = .GetString(field22Ordinal)
                        shipToAddressLine4 = .GetString(field23Ordinal)
                        shipToAddressLine5 = .GetString(field24Ordinal)
                        shipToAddressLine6 = .GetString(field25Ordinal)
                        shipToAddressLine7 = .GetString(field26Ordinal)
                    End If
                    .Close()
                End With
                '-------------------------------------------------------------------------------
                Dim cmdSelectOEP05 As iDB2Command = Nothing
                Dim dtrReaderOEP05 As iDB2DataReader = Nothing
                Const sqlSelectOEP05 As String = "SELECT * FROM COMPFILE WHERE CONO05 = @PARAM1"
                cmdSelectOEP05 = New iDB2Command(sqlSelectOEP05, conSystem21)
                cmdSelectOEP05.Parameters.Add(Param1, iDB2DbType.iDB2VarChar, 2).Value = Settings.AccountNo3
                dtrReaderOEP05 = cmdSelectOEP05.ExecuteReader()
                '-------------------------------------------------------------------------------
                With dtrReaderOEP05
                    field9Ordinal = .GetOrdinal("CNAM05")
                    field11Ordinal = .GetOrdinal("CAD105")
                    field12Ordinal = .GetOrdinal("CAD205")
                    field13Ordinal = .GetOrdinal("CAD305")
                    field14Ordinal = .GetOrdinal("CAD405")
                    field15Ordinal = .GetOrdinal("CAD505")
                    field16Ordinal = .GetOrdinal("CPST05")
                    If .HasRows Then
                        .Read()
                        customerName = .GetString(field9Ordinal)
                        customerAttention = .GetString(field11Ordinal)
                        customerAddressLine2 = .GetString(field12Ordinal)
                        customerAddressLine3 = .GetString(field13Ordinal)
                        customerAddressLine4 = .GetString(field14Ordinal)
                        customerAddressLine5 = .GetString(field15Ordinal)
                        customerAddressLine6 = .GetString(field16Ordinal)
                    End If
                    .Close()
                End With
                '-------------------------------------------------------------------------------
                '   Add to Header Table
                ' 
                dRow = Nothing
                dRow = DtHeaderResults.NewRow()
                dRow("OrderNumber") = orderNumber
                dRow("CustomerCompanyCode") = customerCompanyCode
                dRow("CustomerNumber") = customerNumber
                dRow("CreditNoteNumber") = CreditNoteNumber
                dRow("CreditNoteDateTime") = CreditNoteDateTime
                dRow("CreditNoteAmount") = CreditNoteAmount
                dRow("VatAmount") = vatAmount
                dRow("CreditNoteProcessed") = CreditNoteProcessed
                dRow("CurrencyCode") = currencyCode
                dRow("CustomerPO") = customerPO
                dRow("AccountNumber") = accountNumber
                dRow("CustomerName") = customerName
                dRow("CustomerAttention") = customerAttention
                dRow("CustomerAddressLine1") = customerAddressLine1
                dRow("CustomerAddressLine2") = customerAddressLine2
                dRow("CustomerAddressLine3") = customerAddressLine3
                dRow("CustomerAddressLine4") = customerAddressLine4
                dRow("CustomerAddressLine5") = customerAddressLine5
                dRow("CustomerAddressLine6") = customerAddressLine6
                dRow("CustomerAddressLine7") = customerAddressLine7
                dRow("ShipToName") = shipToName
                dRow("ShipToAttention") = shipToAttention
                dRow("ShipToAddressLine1") = shipToAddressLine1
                dRow("ShipToAddressLine2") = shipToAddressLine2
                dRow("ShipToAddressLine3") = shipToAddressLine3
                dRow("ShipToAddressLine4") = shipToAddressLine4
                dRow("ShipToAddressLine5") = shipToAddressLine5
                dRow("ShipToAddressLine6") = shipToAddressLine6
                dRow("ShipToAddressLine7") = shipToAddressLine7
                dRow("ShipFromName") = shipFromName
                dRow("ShipFromAttention") = shipFromAttention
                dRow("ShipFromAddressLine1") = shipFromAddressLine1
                dRow("ShipFromAddressLine2") = shipFromAddressLine2
                dRow("ShipFromAddressLine3") = shipFromAddressLine3
                dRow("ShipFromAddressLine4") = shipFromAddressLine4
                dRow("ShipFromAddressLine5") = shipFromAddressLine5
                dRow("ShipFromAddressLine6") = shipFromAddressLine6
                dRow("ShipFromAddressLine7") = shipFromAddressLine7
                dRow("VATNumber") = vatNumber
                dRow("PaymentTermsType") = paymentTermsType
                dRow("PaymentTermsPeriod") = paymentTermsPeriod
                dRow("PaymentTermsDays") = paymentTermsDays
                DtHeaderResults.Rows.Add(dRow)

            End While
            dtrReaderHeader.Close()
            '-------------------------------------------------------------------------------
            '   Read CreditNote item
            ' 
            Dim cmdSelectItem As iDB2Command = Nothing
            Dim dtrReaderItem As iDB2DataReader = Nothing

            Dim itemCreditNoteNumber As String = String.Empty
            Dim lineNumber As Integer = 0
            Dim quantityCreditNote As Double = 0
            Dim unitOfMeasure As String = String.Empty
            Dim CreditNoteLineNetAmount As Decimal = 0
            Dim CreditNoteLineVatAmount As Decimal = 0
            Dim productCode As String = String.Empty
            Dim productPrice As Decimal = 0
            Dim itemCurrencyCode As String = String.Empty
            Dim EANCode As String = String.Empty
            Dim description1 As String = String.Empty
            Dim description2 As String = String.Empty

            Const sqlSelectItem As String = "SELECT * FROM XMIVP200 WHERE I2INV = @PARAM1"
            cmdSelectItem = New iDB2Command(sqlSelectItem, conSystem21)
            cmdSelectItem.Parameters.Add(Param1, iDB2DbType.iDB2VarChar, 20).Value = CreditNoteNumber
            dtrReaderItem = cmdSelectItem.ExecuteReader()
            '-------------------------------------------------------------------------------
            With dtrReaderItem
                field1OrdinalI = .GetOrdinal("I2INV")
                field2OrdinalI = .GetOrdinal("I2UOM")
                field3OrdinalI = .GetOrdinal("I2PRD")
                field4OrdinalI = .GetOrdinal("I2CUR")
                field5OrdinalI = .GetOrdinal("I2DSC")
                field6OrdinalI = .GetOrdinal("I2DS1")
                field7OrdinalI = .GetOrdinal("I2EAN")
                While .Read
                    itemCreditNoteNumber = .GetString(field1OrdinalI)
                    lineNumber = .Item("I2LIN")
                    quantityCreditNote = .Item("I2QTY")
                    unitOfMeasure = .GetString(field2OrdinalI)
                    CreditNoteLineNetAmount = .Item("I2NAM")
                    CreditNoteLineVatAmount = .Item("I2VAM")
                    productCode = .GetString(field3OrdinalI)
                    productPrice = .Item("I2PRC")
                    itemCurrencyCode = .GetString(field4OrdinalI)
                    description1 = .GetString(field5OrdinalI)
                    description2 = .GetString(field6OrdinalI)
                    EANCode = .GetString(field7OrdinalI)
                    '------------------------------------
                    ' Add to Item Table
                    '
                    dRow = Nothing
                    dRow = DtItemResults.NewRow
                    dRow("CreditNoteNumber") = itemCreditNoteNumber
                    dRow("LineNumber") = lineNumber
                    dRow("QuantityCreditNote") = quantityCreditNote
                    dRow("UnitOfMeasure") = unitOfMeasure
                    dRow("CreditNoteLineNetAmount") = CreditNoteLineNetAmount
                    dRow("CreditNoteLineVATAmount") = CreditNoteLineVatAmount
                    dRow("ProductCode") = productCode
                    dRow("ProductPrice") = productPrice
                    dRow("CurrencyCode") = itemCurrencyCode
                    dRow("EANCode") = EANCode
                    dRow("Description1") = description1
                    dRow("Description2") = description2
                    DtItemResults.Rows.Add(dRow)
                End While
                .Close()
            End With
        Catch ex As Exception
            err.ErrorMessage = ex.Message
        End Try
        '--------------------------------------------------------------------------
        Return err
    End Function
    Private Function UpdateCreditNoteSystem21() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        Dim libl As String = conSystem21.LibraryList
        Try
            Dim cmdSelect As iDB2Command = Nothing
            Const sqlSelect As String = "UPDATE XMIVP100 SET I1PRC = 'Y' WHERE I1CUS = @PARAM1 AND I1SHP =@PARAM2 AND I1PRC = 'N'"
            cmdSelect = New iDB2Command(sqlSelect, conSystem21)

            ' Only read CreditNotes for partner
            With cmdSelect.Parameters
                .Add(Param1, iDB2DbType.iDB2VarChar, 10).Value = Settings.AccountNo1
                .Add(Param2, iDB2DbType.iDB2VarChar, 10).Value = Settings.AccountNo2
            End With
            cmdSelect.ExecuteNonQuery()
        Catch ex As Exception
        End Try
        '--------------------------------------------------------------------------
        Return err
    End Function
    Private Sub AddColumnsToDataTables()
        With DtHeaderResults.Columns
            .Add("OrderNumber", GetType(String))
            .Add("CustomerCompanyCode", GetType(String))
            .Add("BusinessUnit", GetType(String))
            .Add("Partner", GetType(String))
            .Add("PartnerNumber", GetType(String))
            .Add("LoginID", GetType(String))
            .Add("CustomerNumber", GetType(String))
            .Add("CreditNoteNumber", GetType(String))
            .Add("Finalised", GetType(Boolean))
            .Add("CreditNoteDateTime", GetType(Date))
            .Add("CreditNoteAmount", GetType(Decimal))
            .Add("VatAmount", GetType(Decimal))
            .Add("ChargesAmount", GetType(Decimal))
            .Add("OutstandingAmount", GetType(Decimal))
            .Add("CreditNoteProcessed", GetType(String))
            .Add("CurrencyCode", GetType(String))
            .Add("CustomerPO", GetType(String))
            .Add("OriginalOrderNo", GetType(String))
            .Add("OriginalOrderDate", GetType(Date))
            .Add("DispatchSequence", GetType(String))
            .Add("AccountNumber", GetType(String))
            .Add("CustomerName", GetType(String))
            .Add("CustomerAttention", GetType(String))
            .Add("CustomerAddress1", GetType(String))
            .Add("CustomerAddress2", GetType(String))
            .Add("CustomerAddress3", GetType(String))
            .Add("CustomerAddress4", GetType(String))
            .Add("CustomerAddress5", GetType(String))
            .Add("CustomerAddress6", GetType(String))
            .Add("CustomerAddress7", GetType(String))
            .Add("ShipToName", GetType(String))
            .Add("ShipToAttention", GetType(String))
            .Add("ShipToAddress1", GetType(String))
            .Add("ShipToAddress2", GetType(String))
            .Add("ShipToAddress3", GetType(String))
            .Add("ShipToAddress4", GetType(String))
            .Add("ShipToAddress5", GetType(String))
            .Add("ShipToAddress6", GetType(String))
            .Add("ShipToAddress7", GetType(String))
            .Add("ShipFromName", GetType(String))
            .Add("ShipFromAttention", GetType(String))
            .Add("ShipFromAddress1", GetType(String))
            .Add("ShipFromAddress2", GetType(String))
            .Add("ShipFromAddress3", GetType(String))
            .Add("ShipFromAddress4", GetType(String))
            .Add("ShipFromAddress5", GetType(String))
            .Add("ShipFromAddress6", GetType(String))
            .Add("ShipFromAddress7", GetType(String))
            .Add("VATNumber", GetType(String))
            .Add("PaymentTermsType", GetType(String))
            .Add("PaymentTermsPeriod", GetType(Integer))
            .Add("PaymentTermsDays", GetType(Integer))
        End With


        With DtItemResults.Columns
            .Add("CreditNoteNumber", GetType(String))
            .Add("LineNumber", GetType(Integer))
            .Add("QuantityCreditNote", GetType(Double))
            .Add("UnitOfMeasure", GetType(String))
            .Add("CreditNoteLineNetAmount", GetType(Decimal))
            .Add("CreditNoteLineVatAmount", GetType(Decimal))
            .Add("ProductCode", GetType(String))
            .Add("ProductPrice", GetType(Decimal))
            .Add("CurrencyCode", GetType(String))
            .Add("EANCode", GetType(String))
            .Add("Description1", GetType(String))
            .Add("Description2", GetType(String))
        End With

    End Sub

End Class

