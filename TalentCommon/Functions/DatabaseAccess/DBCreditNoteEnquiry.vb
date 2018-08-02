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
#Region "DBCreditNoteEnquiryHeader"


<Serializable()> _
Public Class DBCreditNoteEnquiryHeader
    Inherits DBAccess

    Private _dep As New DECreditNote
    Private _dtHeaderResults As New DataTable


    Public Property Dep() As DECreditNote
        Get
            Return _dep
        End Get
        Set(ByVal value As DECreditNote)
            _dep = value
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
                          " AND LOGINID = @loginID "

        '  BF- Currently FINALISED is never set in Datatransfer so don't use
        ' " AND FINALISED = @finalised "

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
            '   .Add("@finalised", Data.SqlDbType.NVarChar).Value = Dep.Finalised
        End With

        Try
            cmd.Connection.Open()

            Dim dr As SqlDataReader = cmd.ExecuteReader
            '-------------------------------
            ' If no rows then try by partner
            '-------------------------------
            If Not dr.HasRows Then
                dr.Close()
                cmd.Parameters.Clear()
                cmd.CommandText = " SELECT * " & _
                                  "     FROM tbl_CreditNote_header WITH (NOLOCK)  " & _
                                  " WHERE BUSINESS_UNIT = @bu " & _
                                  " AND PARTNER = @PARTNER "
                cmd = BuildWhereClause(cmd)
                cmd.CommandText += " ORDER BY CreditNote_DATE DESC "
                With cmd.Parameters
                    .Add("@bu", Data.SqlDbType.NVarChar).Value = Dep.BusinessUnit
                    .Add("@PARTNER", Data.SqlDbType.NVarChar).Value = Settings.Partner
                End With
                dr = cmd.ExecuteReader

            End If

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
                        AndAlso Not FromDate = Date.MinValue.ToString Then

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
                                AndAlso Not ToDate = Date.MinValue.ToString Then

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

    End Sub

End Class
#End Region

#Region "CreditNoteEnquiryDetail"

<Serializable()> _
Public Class DBCreditNoteEnquiryDetail
    Inherits DBAccess

    Private _dep As New DECreditNote
    Private _der As New DEInvoiceStatus

    Private _dtDetails As New DataTable

    Private _parmTRAN As String

    Public Property Dep() As DECreditNote
        Get
            Return _dep
        End Get
        Set(ByVal value As DECreditNote)
            _dep = value
        End Set
    End Property
    Public Property Der() As DEInvoiceStatus
        Get
            Return _der
        End Get
        Set(ByVal value As DEInvoiceStatus)
            _der = value
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

    Public Function ReadCreditNote(Optional ByVal opendb As Boolean = False) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------
        Select Case Settings.DestinationDatabase
            Case Is = SYSTEM21
                If opendb Then err = System21Open()
                If Not err.HasError Then err = ReadCreditNoteSystem21()
                If opendb Then System21Close()
            Case Is = CHORUS
                If opendb Then err = ChorusOpen()
                If Not err.HasError Then err = ReadCreditNoteChorus()
                If opendb Then ChorusClose()

        End Select
        Return err
    End Function

    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Dim err As ErrorObj
        err = ReadCreditNote()
        Return err
    End Function

    Protected Overrides Function AccessDataBaseSystem21() As ErrorObj

        Dim err As ErrorObj
        '----------------------
        ' Read the Credit Notes
        '----------------------
        err = ReadCreditNote()

        Return err
    End Function
    Protected Overrides Function AccessDataBaseChorus() As ErrorObj

        Dim err As ErrorObj
        '----------------------
        ' Read the Credit Notes
        '----------------------
        err = ReadCreditNote()

        Return err
    End Function


    Private Function ReadCreditNoteSystem21() As ErrorObj
        Dim err As New ErrorObj
        Dim intPosition As Integer = 0
        '---------------------------------------------------------------------------
        dtDetails = New DataTable("CreditNoteEnquiryDetail")
        ResultDataSet = New DataSet
        ResultDataSet.Tables.Add(dtDetails)

        If dtDetails.Columns.Count = 0 Then
            AddColumnsToDataTables()
        End If

        Dim txs As String = String.Empty
        Dim moreLines As Boolean = True

        '---------------------------------------------------------------------------
        Try
            '-------------------------------------------------------------------------------
            '   Read Credit Noteitem
            ' 
            Dim parmInput, Paramoutput As iDB2Parameter
            Dim PARMOUT As String = String.Empty
            '--------------------------------------------------------------------------
            ' Retrieve Credit note - note for Sys 21 this is done with the same routine
            ' as for Invoices
            '--------------------------------------------------------------------------
            Dim sqlCallInvEnq As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                              "/INVCEENQ(@PARAM1, @PARAM2)"
            Dim cmdSELECT As iDB2Command = Nothing

            While moreLines

                cmdSELECT = New iDB2Command(sqlCallInvEnq, conSystem21)
                parmInput = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                parmInput.Value = Utilities.FixStringLength(Settings.AccountNo3, 2) & _
                                    Utilities.FixStringLength(Settings.AccountNo1, 8) & _
                                    Utilities.FixStringLength(Dep.CreditNoteNumber, 7) & _
                                    Utilities.FixStringLength(txs, 24)

                parmInput.Direction = ParameterDirection.Input
                Paramoutput = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 2048)
                Paramoutput.Value = String.Empty
                Paramoutput.Direction = ParameterDirection.InputOutput
                cmdSELECT.ExecuteNonQuery()
                PARMOUT = cmdSELECT.Parameters(Param2).Value.ToString

                If PARMOUT.Substring(2047, 1) <> "Y" Then
                    '--------------------------------------------------------------------
                    ' Build the response DataSet
                    '
                    '--------------------------------------------------------------------
                    '          1         2         3         4         5         6         7         8         9
                    '01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890 
                    '001DI813953       My Item Description                 00000000005EA000000000000299000000000000029000000000000001
                    '1231234567890123451234567890123456789012345678901234561234567890112123456789012345123456789012345
                    '--------------------------------------------------------------------
                    '
                    Dim iCounter As Integer
                    Dim iPosition As Integer = 0
                    Dim sWork As String = (PARMOUT.Substring(709, 1200))
                    Dim dr As DataRow = Nothing
                    Dim strQty As String = String.Empty
                    Dim strInvNet As String = String.Empty
                    Dim strInvVat As String = String.Empty
                    Dim strProdPrice As String = String.Empty
                    '-------------------------
                    ' Build results - Note Result Data set is same as for 
                    For iCounter = 0 To 9
                        iPosition = iCounter * 120
                        If sWork.Substring(iPosition, 30).Trim > String.Empty Then
                            dr = Nothing
                            dr = dtDetails.NewRow()
                            dr("CreditNoteNumber") = Dep.CreditNoteNumber
                            dr("LineNumber") = CInt(sWork.Substring(iPosition, 3))
                            If sWork.Substring(iPosition + 54, 11).Trim <> String.Empty Then
                                ' Convert QTY (As/400 type 11,3)
                                strQty = sWork.Substring(iPosition + 54, 11)
                                strQty = strQty.Insert(8, ".")
                                dr("QuantityInvoiced") = CDec(strQty)
                            Else
                                dr("QuantityInvoiced") = 0
                            End If
                            dr("UnitOfMeasure") = String.Empty

                            If sWork.Substring(iPosition + 82, 15).Trim <> String.Empty Then
                                ' Convert Net amount (As/400 type 15,5)
                                strInvNet = sWork.Substring(iPosition + 82, 15)
                                strInvNet = strInvNet.Insert(10, ".")
                                dr("InvoiceLineNetAmount") = CDec(strInvNet)
                            Else
                                dr("InvoiceLineNetAmount") = 0
                            End If
                            If sWork.Substring(iPosition + 97, 15).Trim <> String.Empty Then
                                ' Convert VAT amount (As/400 type 15,5)
                                strInvVat = sWork.Substring(iPosition + 97, 15)
                                strInvVat = strInvVat.Insert(10, ".")
                                dr("InvoiceLineVatAmount") = CDec(strInvVat)
                            Else
                                dr("InvoiceLineVatAmount") = 0
                            End If

                            dr("ProductCode") = sWork.Substring(iPosition + 3, 15)

                            If sWork.Substring(iPosition + 67, 15).Trim <> String.Empty Then
                                ' Convert PRICE amount (As/400 type 15,5)
                                strProdPrice = sWork.Substring(iPosition + 67, 15)
                                strProdPrice = strProdPrice.Insert(10, ".")
                                dr("ProductPrice") = CDec(strProdPrice)
                            Else
                                dr("ProductPrice") = 0
                            End If

                            dr("CurrencyCode") = String.Empty
                            dr("CustomerSKU") = String.Empty
                            dr("EANCode") = String.Empty
                            dr("Description1") = sWork.Substring(iPosition + 18, 36)
                            dr("Description2") = String.Empty
                            dtDetails.Rows.Add(dr)
                            '--------------------------------------------------------------

                        Else
                            Exit For
                        End If
                    Next
                    ' Check EOF flag
                    If PARMOUT.Substring(2042, 1) = "1" Then
                        moreLines = False
                    Else
                        ' Set pointer
                        txs = PARMOUT.Substring(2016, 24)
                    End If

                Else
                    Dim strError As String = "Invalid Credit Note number - " & PARMOUT.Substring(2043, 4) & "-" & _
                                            Descriptions.GetDescription(Settings.FrontEndConnectionString, "ENG", "ERRORCODE", PARMOUT.Substring(2043, 4))
                    With err
                        .ErrorMessage = ""
                        .ErrorStatus = strError
                        .ErrorNumber = "TTPRSIN-15"
                        .HasError = True
                        moreLines = False
                    End With

                End If

            End While
            intPosition += 1

        Catch ex As Exception
            Dim strError As String = "Failed to read invoice. Position " & intPosition.ToString & ex.Message
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TTPRSIN-13"
                .HasError = True
            End With
        End Try
        '---------------------------------------------------------------------------
        Return err
    End Function
    Private Function ReadCreditNoteChorus() As ErrorObj
        Dim err As New ErrorObj
        Dim intPosition As Integer = 0
        '---------------------------------------------------------------------------
        dtDetails = New DataTable("CreditNoteEnquiryDetail")
        ResultDataSet = New DataSet
        ResultDataSet.Tables.Add(dtDetails)

        If dtDetails.Columns.Count = 0 Then
            AddColumnsToDataTables()
        End If

        Dim txs As String = String.Empty
        Dim moreLines As Boolean = True

        '---------------------------------------------------------------------------
        Try
            '-------------------------------------------------------------------------------
            '   Read Credit Noteitem
            ' 
            Dim parmInput, Paramoutput As iDB2Parameter
            Dim PARMOUT As String = String.Empty
            '--------------------------------------------------------------------------
            ' Retrieve Credit note - note for Sys 21 this is done with the same routine
            ' as for Invoices
            '--------------------------------------------------------------------------
            Dim sqlCallInvEnq As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                              "/CRDNOENQ(@PARAM1, @PARAM2)"
            Dim cmdSELECT As iDB2Command = Nothing

            While moreLines

                cmdSELECT = New iDB2Command(sqlCallInvEnq, conChorus)
                parmInput = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                parmInput.Value = Utilities.FixStringLength(Settings.AccountNo3, 2) & _
                                    Utilities.FixStringLength(Settings.AccountNo1, 15) & _
                                    Utilities.FixStringLength(Settings.AccountNo2, 15) & _
                                    Utilities.FixStringLength(Dep.CreditNoteNumber, 7) & _
                                    Utilities.FixStringLength(txs, 24)

                parmInput.Direction = ParameterDirection.Input
                Paramoutput = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 2048)
                Paramoutput.Value = String.Empty
                Paramoutput.Direction = ParameterDirection.InputOutput
                cmdSELECT.ExecuteNonQuery()
                PARMOUT = cmdSELECT.Parameters(Param2).Value.ToString

                If PARMOUT.Substring(2047, 1) <> "Y" Then
                    '--------------------------------------------------------------------
                    ' Build the response DataSet
                    '
                    '--------------------------------------------------------------------
                    '          1         2         3         4         5         6         7         8         9
                    '01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890 
                    '001DI813953       My Item Description                 00000000005EA000000000000299000000000000029000000000000001
                    '1231234567890123451234567890123456789012345678901234561234567890112123456789012345123456789012345
                    '--------------------------------------------------------------------
                    '
                    Dim iCounter As Integer
                    Dim iPosition As Integer = 0
                    Dim sWork As String = (PARMOUT.Substring(716, 1200))
                    Dim dr As DataRow = Nothing
                    Dim strQty As String = String.Empty
                    Dim strInvNet As String = String.Empty
                    Dim strInvVat As String = String.Empty
                    Dim strProdPrice As String = String.Empty
                    '-------------------------
                    ' Build results - Note Result Data set is same as for 
                    For iCounter = 0 To 9
                        iPosition = iCounter * 120
                        If sWork.Substring(iPosition, 30).Trim > String.Empty Then
                            dr = Nothing
                            dr = dtDetails.NewRow()
                            dr("CreditNoteNumber") = Dep.CreditNoteNumber
                            dr("LineNumber") = CInt(sWork.Substring(iPosition, 3))
                            If sWork.Substring(iPosition + 54, 11).Trim <> String.Empty Then
                                ' Convert QTY (As/400 type 11,3)
                                strQty = sWork.Substring(iPosition + 54, 11)
                                strQty = strQty.Insert(8, ".")
                                dr("QuantityInvoiced") = CDec(strQty)
                            Else
                                dr("QuantityInvoiced") = 0
                            End If
                            dr("UnitOfMeasure") = String.Empty

                            If sWork.Substring(iPosition + 82, 15).Trim <> String.Empty Then
                                ' Convert Net amount (As/400 type 15,5)
                                strInvNet = sWork.Substring(iPosition + 82, 15)
                                strInvNet = strInvNet.Insert(13, ".")
                                dr("InvoiceLineNetAmount") = CDec(strInvNet)
                            Else
                                dr("InvoiceLineNetAmount") = 0
                            End If
                            If sWork.Substring(iPosition + 97, 15).Trim <> String.Empty Then
                                ' Convert VAT amount (As/400 type 15,5)
                                strInvVat = sWork.Substring(iPosition + 97, 15)
                                strInvVat = strInvVat.Insert(13, ".")
                                dr("InvoiceLineVatAmount") = CDec(strInvVat)
                            Else
                                dr("InvoiceLineVatAmount") = 0
                            End If

                            dr("ProductCode") = sWork.Substring(iPosition + 3, 15)

                            If sWork.Substring(iPosition + 67, 15).Trim <> String.Empty Then
                                ' Convert PRICE amount (As/400 type 15,5)
                                strProdPrice = sWork.Substring(iPosition + 67, 15)
                                strProdPrice = strProdPrice.Insert(10, ".")
                                dr("ProductPrice") = CDec(strProdPrice)
                            Else
                                dr("ProductPrice") = 0
                            End If

                            dr("CurrencyCode") = String.Empty
                            dr("CustomerSKU") = String.Empty
                            dr("EANCode") = String.Empty
                            dr("Description1") = sWork.Substring(iPosition + 18, 36)
                            dr("Description2") = String.Empty
                            dtDetails.Rows.Add(dr)
                            '--------------------------------------------------------------

                        Else
                            Exit For
                        End If
                    Next
                    ' Check EOF flag
                    If PARMOUT.Substring(2042, 1) = "1" Then
                        moreLines = False
                    Else
                        ' Set pointer
                        txs = PARMOUT.Substring(2016, 24)
                    End If

                Else
                    Dim strError As String = "Invalid Credit Note number - " & PARMOUT.Substring(2043, 4) & "-" & _
                                            Descriptions.GetDescription(Settings.FrontEndConnectionString, "ENG", "ERRORCODE", PARMOUT.Substring(2043, 4))
                    With err
                        .ErrorMessage = ""
                        .ErrorStatus = strError
                        .ErrorNumber = "TTPRSIN-30"
                        .HasError = True
                        moreLines = False
                    End With

                End If

            End While
            intPosition += 1

        Catch ex As Exception
            Dim strError As String = "Failed to read invoice. Position " & intPosition.ToString & ex.Message
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TTPRSIN-31"
                .HasError = True
            End With
        End Try
        '---------------------------------------------------------------------------
        Return err
    End Function

    Private Sub AddColumnsToDataTables()
        '---------------------------------------------------------------------------
        With dtDetails.Columns
            .Add("CreditNoteNumber", GetType(String))
            .Add("LineNumber", GetType(Integer))
            .Add("QuantityInvoiced", GetType(Decimal))
            .Add("UnitOfMeasure", GetType(String))
            .Add("InvoiceLineNetAmount", GetType(Decimal))
            .Add("InvoiceLineVatAmount", GetType(Decimal))
            .Add("ProductCode", GetType(String))
            .Add("ProductPrice", GetType(Decimal))
            .Add("CurrencyCode", GetType(String))
            .Add("CustomerSKU", GetType(String))
            .Add("EANCode", GetType(String))
            .Add("ManufacturerSKU", GetType(String))
            .Add("Description1", GetType(String))
            .Add("Description2", GetType(String))
        End With
        '---------------------------------------------------------------------------
    End Sub

End Class
#End Region
