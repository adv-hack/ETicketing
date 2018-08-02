Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Invoice Requests
'
'       Date                        Nov 2006
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'       
'       Error Number Code base      TACDBIN- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'       13/02/07    /001     Ben    Catch error if fail to read
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBInvoicing
    Inherits DBAccess

    Private _dep As New DEInvoice
    Private _der As New DEInvoiceStatus

    Private _dtHeader As New DataTable
    Private _dtDetails As New DataTable
    Private _dtComments As New DataTable

    Private _parmTRAN As String

    Public Property Dep() As DEInvoice
        Get
            Return _dep
        End Get
        Set(ByVal value As DEInvoice)
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

    Public Property dtHeader() As DataTable
        Get
            Return _dtHeader
        End Get
        Set(ByVal value As DataTable)
            _dtHeader = value
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
    Public Property dtComments() As DataTable
        Get
            Return _dtComments
        End Get
        Set(ByVal value As DataTable)
            _dtComments = value
        End Set
    End Property

    Public Function ReadInvoice(Optional ByVal opendb As Boolean = False) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------
        Select Case Settings.DestinationDatabase
            Case Is = SYSTEM21
                If opendb Then err = System21Open()
                If Not err.HasError Then err = ReadInvoiceSystem21()
                If opendb Then System21Close()
            Case Is = SQL2005
                ReadInvoiceSQL2005()
        End Select
        Return err
    End Function

    Protected Function ReadInvoiceSQL2005() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        ResultDataSet.Tables.Add(dtHeader)
        ResultDataSet.Tables.Add(dtDetails)
        ResultDataSet.Tables.Add(dtComments)

        Dim dRow As DataRow

        dRow = Nothing
        dRow = dtDetails.NewRow()
        dRow("InvoiceNumber") = "11111"
        dRow("LineNumber") = "1"
        dRow("QuantityInvoiced") = 3
        dRow("UnitOfMeasure") = ""
        dRow("InvoiceLineNetAmount") = 29.99
        dRow("InvoiceLineVATAmount") = 2.99
        dRow("ProductCode") = "PRD111222"
        dRow("ProductPrice") = 32.98
        dRow("CurrencyCode") = "GBP"
        dRow("CustomerSKU") = ""
        dRow("EANCode") = ""
        dRow("Description1") = "Test Product 1"
        dRow("Description2") = "Test Product 1"
        dtDetails.Rows.Add(dRow)

        '--------------------------------------------------------------------
        ' Add to Header Table Only  if:- Total qty of item  lines is not zero
        '

        dRow = Nothing
        dRow = dtHeader.NewRow()
        dRow("OrderNumber") = "ORD1111"
        dRow("CustomerCompanyCode") = "Company1"
        dRow("CustomerNumber") = "11111"
        dRow("InvoiceNumber") = "11111"
        dRow("InvoiceDateTime") = Now
        dRow("InvoiceAmount") = 32.98
        dRow("VatAmount") = 2.99
        dRow("ChargesAmount") = 32.98
        dRow("InvoiceProcessed") = "False"
        dRow("CurrencyCode") = "GBP"
        dRow("CustomerPO") = "11111"
        dRow("OriginalOrderNo") = "ORD1111"
        dRow("OriginalOrderDate") = Now
        dRow("DispatchSequence") = "123"
        dRow("AccountNumber") = "2222"
        dRow("CustomerName") = "Fred"
        dRow("CustomerAttention") = ""
        dRow("CustomerAddress1") = "Address 1"
        dRow("CustomerAddress2") = "Address 2"
        dRow("CustomerAddress3") = "Address 3"
        dRow("CustomerAddress4") = "Address 4"
        dRow("CustomerAddress5") = "Address 5"
        dRow("CustomerAddress6") = "Address 6"
        dRow("CustomerAddress7") = "Address 7"
        dRow("ShipToName") = "Fred"
        dRow("ShipToAttention") = ""
        dRow("ShipToAddress1") = "Address 1"
        dRow("ShipToAddress2") = "Address 2"
        dRow("ShipToAddress3") = "Address 3"
        dRow("ShipToAddress4") = "Address 4"
        dRow("ShipToAddress5") = "Address 5"
        dRow("ShipToAddress6") = "Address 6"
        dRow("ShipToAddress7") = "Address 7"
        dRow("ShipFromName") = "Fred"
        dRow("ShipFromAttention") = ""
        dRow("ShipFromAddress1") = "Address 1"
        dRow("ShipFromAddress2") = "Address 2"
        dRow("ShipFromAddress3") = "Address 3"
        dRow("ShipFromAddress4") = "Address 4"
        dRow("ShipFromAddress5") = "Address 5"
        dRow("ShipFromAddress6") = "Address 6"
        dRow("ShipFromAddress7") = "Address 7"
        dRow("VATNumber") = "12345"
        dRow("PaymentTermsType") = "Address 1"
        dRow("PaymentTermsPeriod") = 5
        dRow("PaymentTermsDays") = 5
        dtHeader.Rows.Add(dRow)

        '   Read order text
        ' 
        dRow = Nothing
        dRow = dtComments.NewRow()
        dRow("InvoiceNumber") = "11111"
        dRow("Text") = "Text"
        dRow("TextLineNumber") = 1
        dtComments.Rows.Add(dRow)

        Return err

    End Function
    Public Function UpdateInvoice(Optional ByVal opendb As Boolean = False) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------
        Select Case Settings.DestinationDatabase
            Case Is = SYSTEM21
                If opendb Then err = System21Open()
                If Not err.HasError Then err = UpdateInvoiceSystem21()
                If opendb Then err = System21Close()
        End Select
        Return err
    End Function

    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Dim err As New ErrorObj

        Return err
    End Function

    Protected Overrides Function AccessDataBaseSystem21() As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------
        '   Create command object
        '
        Dim cmdSELECT As iDB2Command = Nothing
        ' Const strHEADER As String = "CALL WESTCOAST/INVCEREQ(@PARAM1, @PARAM2)"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                  "/INVCEREQ(@PARAM1, @PARAM2)"
        Dim parmInput, Paramoutput As iDB2Parameter
        Dim PARMOUT As String = String.Empty

        If Not err.HasError Then
            Try
                cmdSELECT = New iDB2Command(strHEADER, conSystem21)
                parmInput = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                ' Pass in blank Delivery Seq to extract all invoices for the company
                parmInput.Value = Utilities.FixStringLength(Settings.AccountNo3, 2) & _
                                    Utilities.FixStringLength(Settings.AccountNo1, 10) & _
                                    "   " & "IN"
                parmInput.Direction = ParameterDirection.Input
                Paramoutput = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                Paramoutput.Value = String.Empty
                Paramoutput.Direction = ParameterDirection.InputOutput
                cmdSELECT.ExecuteNonQuery()
                PARMOUT = cmdSELECT.Parameters(Param2).Value.ToString
            Catch ex As Exception
                Const strError2 As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError2
                    .ErrorNumber = "TACDBIN02"
                    .HasError = True
                End With
            End Try

            If Not err.HasError Then
                '------------------------------------------------------------
                ' Read the invoices back out 
                ' 
                err = ReadInvoice()
                '------------------------------------------------------------
                ' Update the invoices as processed 
                ' 
                If Not err.HasError Then err = UpdateInvoice()
            End If
        End If
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
            '---------------------------------------------------------------------------------------------
            '   <TransactionHeader>
            '       <SenderID>123456789</SenderID>
            '       <ReceiverID>987654321</ReceiverID>
            '       <CountryCode>UK</CountryCode>
            '       <LoginID>UK3833HHD</LoginID>
            '       <Password>Re887Jky52</Password>
            '       <Company>CSG</Company>
            '       <TransactionID>54321</TransactionID>
            '   </TransactionHeader>
            With detr
                ParmTRAN = String.Format("SenderID = {0}, ReceiverID = {1}, CountryCode = {2}" & _
                                            ", TransactionID = {3}, ShowDetail = {4}", _
                                            .SenderID, _
                                            .ReceiverID, _
                                            .CountryCode, _
                                            .TransactionID, _
                                            .ShowDetail)
            End With
        Catch ex As Exception
            Const strError13 As String = "Could not Unpack Data Entity "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError13
                .ErrorNumber = "TACDBIN13"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Private Function ReadInvoiceSystem21() As ErrorObj
        Dim err As New ErrorObj
        Dim intPosition As Integer = 0
        '---------------------------------------------------------------------------
        ResultDataSet = New DataSet
        ResultDataSet.Tables.Add(dtHeader)
        ResultDataSet.Tables.Add(dtDetails)
        ResultDataSet.Tables.Add(dtComments)

        If dtHeader.Columns.Count = 0 Then
            AddColumnsToDataTables()
        End If

        '---------------------------------------------------------------------------
        Try
            Dim cmdSelectHeader As iDB2Command = Nothing
            Dim dtrReaderHeader As iDB2DataReader = Nothing
            Dim dRow As DataRow = Nothing
            Dim totalItemQuantity As Integer = 0

            '-----------------------------------------------------------------------
            ' Read invoice header
            '
            Dim orderNumber As String = String.Empty
            Dim orderDate As Date
            Dim customerCompanyCode As String = String.Empty
            Dim customerNumber As String = String.Empty
            Dim invoiceNumber As String = String.Empty
            Dim invoiceDateTime As Date = Date.MinValue
            Dim invoiceAmount As Decimal = 0
            Dim vatAmount As Decimal = 0
            Dim invoiceProcessed As String = String.Empty
            Dim currencyCode As String = String.Empty
            Dim customerPO As String = String.Empty
            Dim backOfficeOrderNo As String = String.Empty
            Dim accountNumber As String = String.Empty
            Dim customerName As String = String.Empty
            Dim customerAttention As String = String.Empty
            Dim customerAddress1 As String = String.Empty
            Dim customerAddress2 As String = String.Empty
            Dim customerAddress3 As String = String.Empty
            Dim customerAddress4 As String = String.Empty
            Dim customerAddress5 As String = String.Empty
            Dim customerAddress6 As String = String.Empty
            Dim customerAddress7 As String = String.Empty
            Dim shipToName As String = String.Empty
            Dim shipToAttention As String = String.Empty
            Dim shipToAddress1 As String = String.Empty
            Dim shipToAddress2 As String = String.Empty
            Dim shipToAddress3 As String = String.Empty
            Dim shipToAddress4 As String = String.Empty
            Dim shipToAddress5 As String = String.Empty
            Dim shipToAddress6 As String = String.Empty
            Dim shipToAddress7 As String = String.Empty
            Dim shipFromName As String = String.Empty
            Dim shipFromAttention As String = String.Empty
            Dim shipFromAddress1 As String = String.Empty
            Dim shipFromAddress2 As String = String.Empty
            Dim shipFromAddress3 As String = String.Empty
            Dim shipFromAddress4 As String = String.Empty
            Dim shipFromAddress5 As String = String.Empty
            Dim shipFromAddress6 As String = String.Empty
            Dim shipFromAddress7 As String = String.Empty
            Dim vatNumber As String = String.Empty
            Dim paymentTermsType As String = String.Empty
            Dim paymentTermsPeriod As Integer = 0
            Dim paymentTermsDays As Integer = 0
            Dim totalCharges As Decimal = 0
            Dim dispatchSequence As Integer = 0

            Dim param1 As String = "@PARAM1"
            Dim param2 As String = "@PARAM2"
            '-----------------------------------------------------------------------------------
            '   Only read invoices for partner
            '
            Const sqlSelectHeader As String = "SELECT * FROM XMIVP100 WHERE I1CUS =@PARAM1 AND I1PRC = 'N'"
            cmdSelectHeader = New iDB2Command(sqlSelectHeader, conSystem21)
            cmdSelectHeader.Parameters.Add(param1, iDB2DbType.iDB2VarChar, 10).Value = Settings.AccountNo1
            dtrReaderHeader = cmdSelectHeader.ExecuteReader()
            With dtrReaderHeader
                If Not .HasRows Then
                    Const strError15 As String = "No data to display"
                    With err
                        .ErrorMessage = Settings.AccountNo1
                        .ErrorNumber = "TACDBIN15"
                        .ErrorStatus = strError15
                        .HasError = True
                    End With
                End If
            End With

            intPosition += 1
            '-----------------------------------------------------------------------------------
            While dtrReaderHeader.Read
                With dtrReaderHeader
                    totalItemQuantity = 0
                    '
                    orderNumber = .GetString(.GetOrdinal("I1ORD"))
                    customerCompanyCode = .GetString(.GetOrdinal("I1SHP"))
                    customerNumber = .GetString(.GetOrdinal("I1CUS"))
                    invoiceNumber = .GetString(.GetOrdinal("I1INV"))
                    '---------------------------------------------------------------------------
                    Dim dt As String = Utilities.ISeriesDate(.GetString(.GetOrdinal("I1IDT")))
                    Dim tm As String = Utilities.ISeriesTime(.GetString(.GetOrdinal("I1ITM")))
                    invoiceDateTime = Date.Parse(dt & " " & tm)
                    '---------------------------------------------------------------------------
                    invoiceAmount = .Item("I1IAM")
                    vatAmount = .Item("I1VAM")
                    invoiceProcessed = .GetString(.GetOrdinal("I1PRC"))
                    currencyCode = .GetString(.GetOrdinal("I1CUR"))
                    accountNumber = Settings.AccountNo3.Trim
                End With

                intPosition += 1
                '-------------------------------------------------------------------------------
                Dim cmdSelectXXCOP100 As iDB2Command = Nothing
                Dim dtrReaderXXCOP100 As iDB2DataReader = Nothing
                Const sqlSelectXXCOP100 As String = "SELECT * FROM XXCOP100 WHERE C1ORD = @PARAM1"
                cmdSelectXXCOP100 = New iDB2Command(sqlSelectXXCOP100, conSystem21)
                cmdSelectXXCOP100.Parameters.Add(param1, iDB2DbType.iDB2VarChar, 10).Value = orderNumber
                dtrReaderXXCOP100 = cmdSelectXXCOP100.ExecuteReader()
                intPosition += 1
                ' 
                With dtrReaderXXCOP100
                    If .HasRows Then
                        .Read()
                        customerPO = .GetString(.GetOrdinal("C1PON"))
                        Dim oD As String = Utilities.ISeriesDate(.GetString(.GetOrdinal("C1ODT")))
                        orderDate = Date.Parse(oD)
                        'shipFromName = .GetString(.GetOrdinal("C1NAM"))
                        'shipFromAddress1 = .GetString(.GetOrdinal("C1AD1"))
                        'shipFromAddress2 = .GetString(.GetOrdinal("C1AD2"))
                        'shipFromAddress3 = .GetString(.GetOrdinal("C1AD3"))
                        'shipFromAddress4 = .GetString(.GetOrdinal("C1AD4"))
                        'shipFromAddress5 = .GetString(.GetOrdinal("C1AD5"))
                        'shipFromAddress6 = .GetString(.GetOrdinal("C1PCD"))
                        'shipFromAddress7 = .GetString(.GetOrdinal("C1CCD"))
                        customerName = .GetString(.GetOrdinal("C1NAM"))
                        customerAttention = .GetString(.GetOrdinal("C1AD1"))
                        customerAddress2 = .GetString(.GetOrdinal("C1AD2"))
                        customerAddress3 = .GetString(.GetOrdinal("C1AD3"))
                        customerAddress4 = .GetString(.GetOrdinal("C1AD4"))
                        customerAddress5 = .GetString(.GetOrdinal("C1AD5"))
                        customerAddress6 = .GetString(.GetOrdinal("C1PCD"))
                        backOfficeOrderNo = .GetString(.GetOrdinal("C1BOR"))
                    End If
                    .Close()
                End With
                '-------------------------------------------------------------------------------
                Dim cmdSelectXXCOP200 As iDB2Command = Nothing
                Dim dtrReaderXXCOP200 As iDB2DataReader = Nothing
                Const sqlSelectXXCOP200 As String = "SELECT * FROM XXCOP200 WHERE C2ORD = @PARAM1"
                cmdSelectXXCOP200 = New iDB2Command(sqlSelectXXCOP200, conSystem21)
                cmdSelectXXCOP200.Parameters.Add(param1, iDB2DbType.iDB2VarChar, 10).Value = orderNumber
                dtrReaderXXCOP200 = cmdSelectXXCOP200.ExecuteReader()
                '-------------------------------------------------------------------------------
                With dtrReaderXXCOP200
                    If .HasRows Then
                        .Read()
                        shipToName = .GetString(.GetOrdinal("C2NAM"))
                        shipToAddress1 = .GetString(.GetOrdinal("C2AD1"))
                        shipToAddress2 = .GetString(.GetOrdinal("C2AD2"))
                        shipToAddress3 = .GetString(.GetOrdinal("C2AD3"))
                        shipToAddress4 = .GetString(.GetOrdinal("C2AD4"))
                        shipToAddress5 = .GetString(.GetOrdinal("C2AD5"))
                        shipToAddress6 = .GetString(.GetOrdinal("C2PCD"))
                        shipToAddress7 = .GetString(.GetOrdinal("C2CCD"))
                    End If
                    .Close()
                End With
                intPosition += 1
                '-------------------------------------------------------------------------------
                Dim cmdSelectOEP05 As iDB2Command = Nothing
                Dim dtrReaderOEP05 As iDB2DataReader = Nothing
                Const sqlSelectOEP05 As String = "SELECT * FROM COMPFILE WHERE CONO05 = @PARAM1"
                cmdSelectOEP05 = New iDB2Command(sqlSelectOEP05, conSystem21)
                cmdSelectOEP05.Parameters.Add(param1, iDB2DbType.iDB2VarChar, 2).Value = Settings.AccountNo3
                dtrReaderOEP05 = cmdSelectOEP05.ExecuteReader()
                intPosition += 1
                '-------------------------------------------------------------------------------
                With dtrReaderOEP05
                    If .HasRows Then
                        .Read()
                        'customerName = .GetString(.GetOrdinal("CNAM05"))
                        'customerAttention = .GetString(.GetOrdinal("CAD105"))
                        'customerAddress2 = .GetString(.GetOrdinal("CAD205"))
                        'customerAddress3 = .GetString(.GetOrdinal("CAD305"))
                        'customerAddress4 = .GetString(.GetOrdinal("CAD405"))
                        'customerAddress5 = .GetString(.GetOrdinal("CAD505"))
                        'customerAddress6 = .GetString(.GetOrdinal("CPST05"))
                        shipFromName = .GetString(.GetOrdinal("CNAM05"))
                        shipFromAddress1 = .GetString(.GetOrdinal("CAD105"))
                        shipFromAddress2 = .GetString(.GetOrdinal("CAD205"))
                        shipFromAddress3 = .GetString(.GetOrdinal("CAD305"))
                        shipFromAddress4 = .GetString(.GetOrdinal("CAD405"))
                        shipFromAddress5 = .GetString(.GetOrdinal("CAD505"))
                        shipFromAddress6 = .GetString(.GetOrdinal("CPST05"))
                        ' shipFromAddress7 = .GetString(.GetOrdinal("C1CCD"))
                    End If
                    .Close()
                End With
                intPosition += 1
                '-------------------------------------------------------------------------------
                '   Get additional details of System 21 invoice header
                ' 
                Dim cmdSelectOEP65 As iDB2Command = Nothing
                Dim dtrReaderOEP65 As iDB2DataReader = Nothing
                Const sqlSelectOEP65 As String = "SELECT * FROM OEP65 WHERE CONO65 = @PARAM1 and INVN65 = @PARAM2"
                cmdSelectOEP65 = New iDB2Command(sqlSelectOEP65, conSystem21)
                cmdSelectOEP65.Parameters.Add(param1, iDB2DbType.iDB2VarChar, 2).Value = Settings.AccountNo3
                cmdSelectOEP65.Parameters.Add(param2, iDB2DbType.iDB2VarChar, 7).Value = invoiceNumber
                dtrReaderOEP65 = cmdSelectOEP65.ExecuteReader()
                With dtrReaderOEP65
                    If .HasRows Then
                        .Read()
                        totalCharges = .GetDecimal(.GetOrdinal("INVC65"))
                        dispatchSequence = .GetInt32(.GetOrdinal("DESN65"))
                    End If
                    .Close()
                End With
                intPosition += 1
                '-------------------------------------------------------------------------------
                '   Read invoice item
                ' 
                Dim cmdSelectItem As iDB2Command = Nothing
                Dim dtrReaderItem As iDB2DataReader = Nothing

                Dim quantityInvoiced As Double = 0
                Const sqlSelectItem As String = "SELECT * FROM XMIVP200 WHERE I2INV = @PARAM1"
                cmdSelectItem = New iDB2Command(sqlSelectItem, conSystem21)
                cmdSelectItem.Parameters.Add(param1, iDB2DbType.iDB2VarChar, 20).Value = invoiceNumber
                dtrReaderItem = cmdSelectItem.ExecuteReader()
                '-------------------------------------------------------------------------------
                With dtrReaderItem
                    While .Read
                        quantityInvoiced = .Item("I2QTY")
                        totalItemQuantity += quantityInvoiced
                        '------------------------------------------------------
                        '   Add to Item Table
                        '
                        dRow = Nothing
                        dRow = dtDetails.NewRow()
                        dRow("InvoiceNumber") = .GetString(.GetOrdinal("I2INV")).Trim
                        dRow("LineNumber") = .Item("I2LIN")
                        dRow("QuantityInvoiced") = quantityInvoiced
                        dRow("UnitOfMeasure") = .GetString(.GetOrdinal("I2UOM")).Trim
                        dRow("InvoiceLineNetAmount") = .Item("I2NAM")
                        dRow("InvoiceLineVATAmount") = .Item("I2VAM")
                        dRow("ProductCode") = .GetString(.GetOrdinal("I2PRD")).Trim
                        dRow("ProductPrice") = .Item("I2PRC")
                        dRow("CurrencyCode") = .GetString(.GetOrdinal("I2CUR")).Trim
                        dRow("CustomerSKU") = .GetString(.GetOrdinal("I2CPN")).Trim
                        dRow("EANCode") = .GetString(.GetOrdinal("I2EAN")).Trim
                        dRow("Description1") = .GetString(.GetOrdinal("I2DSC")).Trim
                        dRow("Description2") = .GetString(.GetOrdinal("I2DS1")).Trim
                        dtDetails.Rows.Add(dRow)
                    End While
                    .Close()
                End With
                intPosition += 1
                '--------------------------------------------------------------------
                ' Add to Header Table Only  if:- Total qty of item  lines is not zero
                '
                If totalItemQuantity > 0 Then
                    dRow = Nothing
                    dRow = dtHeader.NewRow()
                    dRow("OrderNumber") = orderNumber.Trim
                    dRow("CustomerCompanyCode") = customerCompanyCode.Trim
                    dRow("CustomerNumber") = customerNumber.Trim
                    dRow("InvoiceNumber") = invoiceNumber.Trim
                    dRow("InvoiceDateTime") = invoiceDateTime
                    dRow("InvoiceAmount") = invoiceAmount
                    dRow("VatAmount") = vatAmount
                    dRow("ChargesAmount") = totalCharges
                    dRow("InvoiceProcessed") = invoiceProcessed.Trim
                    dRow("CurrencyCode") = currencyCode.Trim
                    dRow("CustomerPO") = customerPO.Trim
                    dRow("OriginalOrderNo") = backOfficeOrderNo.Trim
                    dRow("OriginalOrderDate") = orderDate
                    dRow("DispatchSequence") = dispatchSequence
                    dRow("AccountNumber") = accountNumber.Trim
                    dRow("CustomerName") = customerName.Trim
                    dRow("CustomerAttention") = customerAttention.Trim
                    dRow("CustomerAddress1") = customerAddress1.Trim
                    dRow("CustomerAddress2") = customerAddress2.Trim
                    dRow("CustomerAddress3") = customerAddress3.Trim
                    dRow("CustomerAddress4") = customerAddress4.Trim
                    dRow("CustomerAddress5") = customerAddress5.Trim
                    dRow("CustomerAddress6") = customerAddress6.Trim
                    dRow("CustomerAddress7") = customerAddress7.Trim
                    dRow("ShipToName") = shipToName.Trim
                    dRow("ShipToAttention") = shipToAttention.Trim
                    dRow("ShipToAddress1") = shipToAddress1.Trim
                    dRow("ShipToAddress2") = shipToAddress2.Trim
                    dRow("ShipToAddress3") = shipToAddress3.Trim
                    dRow("ShipToAddress4") = shipToAddress4.Trim
                    dRow("ShipToAddress5") = shipToAddress5.Trim
                    dRow("ShipToAddress6") = shipToAddress6.Trim
                    dRow("ShipToAddress7") = shipToAddress7.Trim
                    dRow("ShipFromName") = shipFromName.Trim
                    dRow("ShipFromAttention") = shipFromAttention.Trim
                    dRow("ShipFromAddress1") = shipFromAddress1.Trim
                    dRow("ShipFromAddress2") = shipFromAddress2.Trim
                    dRow("ShipFromAddress3") = shipFromAddress3.Trim
                    dRow("ShipFromAddress4") = shipFromAddress4.Trim
                    dRow("ShipFromAddress5") = shipFromAddress5.Trim
                    dRow("ShipFromAddress6") = shipFromAddress6.Trim
                    dRow("ShipFromAddress7") = shipFromAddress7.Trim
                    dRow("VATNumber") = vatNumber.Trim
                    dRow("PaymentTermsType") = paymentTermsType.Trim
                    dRow("PaymentTermsPeriod") = paymentTermsPeriod
                    dRow("PaymentTermsDays") = paymentTermsDays
                    dtHeader.Rows.Add(dRow)
                    intPosition += 1
                    '-------------------------------------------------------------------------------------
                    '   Read order text
                    ' 
                    Dim text As String = String.Empty
                    Dim textLineNo As Integer = 0
                    Dim cmdSelectText As iDB2Command = Nothing
                    Dim dtrReaderText As iDB2DataReader = Nothing

                    Const sqlSelectText As String = "SELECT * FROM INP40 WHERE CONO40 = @PARAM1 AND TTYP40 = 'O' AND TREF40 = @PARAM2 AND USGC40 = 'E'"

                    cmdSelectText = New iDB2Command(sqlSelectText, conSystem21)
                    Select Case Settings.DatabaseType1
                        Case Is = T65535
                            cmdSelectText.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = Settings.AccountNo3
                            cmdSelectText.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 15).Value = backOfficeOrderNo
                        Case Is = T285
                            cmdSelectText.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = Settings.AccountNo3
                            cmdSelectText.Parameters.Add(param2, iDB2DbType.iDB2Char, 15).Value = backOfficeOrderNo
                        Case Else
                            cmdSelectText.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = Settings.AccountNo3
                            cmdSelectText.Parameters.Add(param2, iDB2DbType.iDB2Char, 15).Value = backOfficeOrderNo
                    End Select
                    dtrReaderText = cmdSelectText.ExecuteReader()
                    intPosition += 1
                    With dtrReaderText
                        While .Read
                            text = .GetString(.GetOrdinal("TLIN40"))
                            textLineNo = .Item("TLNO40")
                            '---------------------------------------------------------------------------------------------
                            '   Add to Detail Table
                            '
                            dRow = Nothing
                            dRow = dtComments.NewRow()
                            dRow("InvoiceNumber") = invoiceNumber.Trim
                            dRow("Text") = text
                            dRow("TextLineNumber") = textLineNo
                            dtComments.Rows.Add(dRow)
                        End While
                        .Close()
                    End With
                    intPosition += 1
                End If
            End While
            dtrReaderHeader.Close()
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
    Private Function UpdateInvoiceSystem21() As ErrorObj
        Dim err As New ErrorObj
        '---------------------------------------------------------------------------
        Try
            Dim cmdSelect As iDB2Command = Nothing
            Dim param1 As String = "@PARAM1"
            Dim param2 As String = "@PARAM2"
            Const sqlSelect As String = "UPDATE XMIVP100 SET I1PRC = 'Y' WHERE I1CUS =@PARAM1 AND I1PRC = 'N'"
            cmdSelect = New iDB2Command(sqlSelect, conSystem21)
            ' Only read invoices for partner
            cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2VarChar, 10).Value = Settings.AccountNo1
            ' cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2VarChar, 10).Value = Settings.AccountNo2
            cmdSelect.ExecuteNonQuery()
        Catch ex As Exception
        End Try
        '---------------------------------------------------------------------------
        Return err
    End Function
    Private Sub AddColumnsToDataTables()
        '---------------------------------------------------------------------------
        With dtHeader.Columns
            .Add("OrderNumber", GetType(String))
            .Add("CustomerCompanyCode", GetType(String))
            .Add("CustomerNumber", GetType(String))
            .Add("InvoiceNumber", GetType(String))
            .Add("InvoiceDateTime", GetType(Date))
            .Add("InvoiceAmount", GetType(Decimal))
            .Add("VatAmount", GetType(Decimal))
            .Add("ChargesAmount", GetType(Decimal))
            .Add("InvoiceProcessed", GetType(String))
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
        '---------------------------------------------------------------------------
        With dtDetails.Columns
            .Add("InvoiceNumber", GetType(String))
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
        With dtComments.Columns
            .Add("InvoiceNumber", GetType(String))
            .Add("Text", GetType(String))
            .Add("TextLineNumber", GetType(Integer))
        End With
        '---------------------------------------------------------------------------
    End Sub

End Class

