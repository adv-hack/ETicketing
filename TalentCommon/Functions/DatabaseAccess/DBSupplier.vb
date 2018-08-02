Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Text
Imports IBM.Data.DB2.iSeries
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Supplier Invoices
'
'       Date                        Mar 2007
'
'       Author                      Andy White   
'
'        CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDBSI- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBSupplier
    Inherits DBAccess

    Private _dep As DESupplier
    Private _dtHeader As New DataTable
    Private _parmHEAD(20) As String
    Private _parmITEM(20, 500) As String
    Private InvoiceNumber As String = String.Empty
    Private CompanyNumber As String = String.Empty
    Private PoNumber As String = String.Empty

    Public Property Dep() As DESupplier
        Get
            Return _dep
        End Get
        Set(ByVal value As DESupplier)
            _dep = value
        End Set
    End Property
    Public Property DtHeader() As DataTable
        Get
            Return _dtHeader
        End Get
        Set(ByVal value As DataTable)
            _dtHeader = value
        End Set
    End Property

    Protected Overrides Function AccessDataBaseSystem21() As ErrorObj
        Dim err As New ErrorObj
        Select Case _settings.ModuleName
            Case Is = "WriteInvoice"
                err = AccessDataBaseSystem21_WriteInvoice()
            Case Is = "SupplierOrderAcknowledgement"
                err = AccessDataBaseSystem21_SupplierOrderAcknowledgement()
        End Select
        Return err
    End Function

    Protected Function AccessDataBaseSystem21_WriteInvoice() As ErrorObj

        '---------------------------------------------------------------------------
        '   AccessDataBaseSystem21: Write the invoice to System 21 via Stored Procedures
        '
        Dim err As New ErrorObj
        Dim sProcError As String = String.Empty
        '---------------------------------------------------------------------------
        '   Open up collection and deal with by putting to stored procedures parameters 
        '
        err = DataEntityUnPackSystem21()
        If Not err.HasError Then
            '------------------------------------------------------------------
            '   Setup Calls to As400
            ' 
            Dim SqlHead As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/RCVINVHDR(@PARAM1, @PARAM2)"
            Dim SqlItem As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/RCVINVLIN(@PARAM1, @PARAM2)"

            Dim cmdHead As iDB2Command = Nothing
            Dim cmdItem As iDB2Command = Nothing

            Dim iItems As Integer = 0
            Dim iInvoice As Integer = 0

            Dim parmInput, parmOutput As iDB2Parameter
            Dim PARMOUT As String = String.Empty

            Dim strTalentOrderNo As String = String.Empty
            '----------------------------------------------------------------
            '   Loop through Invoices in transaction
            '
            Try
                For iInvoice = 1 To 500
                    If ParmHEAD(iInvoice) > String.Empty Then
                        '----------------------------------------------------------------
                        '   Call Invoice header stored procedure to write to Talent  
                        '
                        cmdHead = New iDB2Command(SqlHead, conSystem21)
                        With cmdHead
                            parmInput = .Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                            parmInput.Value = ParmHEAD(iInvoice)
                            parmInput.Direction = ParameterDirection.Input
                            parmOutput = .Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                            parmOutput.Value = String.Empty
                            parmOutput.Direction = ParameterDirection.InputOutput
                            .ExecuteNonQuery()
                            PARMOUT = .Parameters(Param2).Value.ToString
                        End With
                        strTalentOrderNo = Utilities.FixStringLength(PARMOUT.Substring(0, 15), 15)
                        If PARMOUT.Substring(1023, 1) = "Y" Then
                            With err
                                .ItemErrorMessage(iInvoice) = PARMOUT.Substring(1020, 4)
                                .ItemErrorCode(iInvoice) = "TACDBSI-16"
                                .ItemErrorStatus(iInvoice) = "Error creating Invoice header " & PARMOUT.Substring(1019, 4)
                                .ErrorMessage = PARMOUT.Substring(1019, 4)
                                .ErrorNumber = "TACDBSI-16"
                                .ErrorStatus = "Error creating Invoice header " & PARMOUT.Substring(1019, 4) & " - " & Descriptions.GetDescription(Settings.FrontEndConnectionString, _
                                                    "ENG", "ERRORCODE", PARMOUT.Substring(1019, 4))
                                .HasError = True
                                Return err
                            End With
                        Else
                            '----------------------------------------------------------------
                            '   Loop through items in current invoice
                            ' 
                            For iItems = 1 To 500
                                If ParmITEM(iInvoice, iItems) > String.Empty Then
                                    '---------------------------------------------------------
                                    '   Call order detail stored procedure to write to Talent  
                                    ' 
                                    cmdItem = New iDB2Command(SqlItem, conSystem21)
                                    With cmdItem
                                        PARMOUT = String.Empty
                                        parmInput = .Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                                        parmInput.Value = ParmITEM(iInvoice, iItems)
                                        parmInput.Direction = ParameterDirection.Input
                                        parmOutput = .Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                                        parmOutput.Value = String.Empty
                                        parmOutput.Direction = ParameterDirection.InputOutput
                                        .ExecuteNonQuery()
                                        PARMOUT = .Parameters(Param2).Value.ToString
                                    End With
                                    If PARMOUT.Substring(1023, 1) = "Y" Then
                                        With err
                                            .ItemErrorMessage(iInvoice) = PARMOUT.Substring(1020, 4)
                                            .ItemErrorCode(iInvoice) = "TACDBSI-17"
                                            .ItemErrorStatus(iInvoice) = "Error creating Invoice item"
                                            .ErrorMessage = PARMOUT.Substring(1020, 4)
                                            .ErrorNumber = "TACDBSI-17"
                                            .ErrorStatus = "Error creating Invoice item"
                                            .HasError = True
                                            Return err
                                        End With
                                    End If
                                Else
                                    Exit For
                                End If
                            Next iItems
                        End If
                    Else
                        Exit For
                    End If
                Next iInvoice
                '---------------------------------------------------------------------------------------------
                '   Last item now written - if no error then write to System 21  
                ' 
                ' Const SqlFinish As String = "CALL WESTCOAST/RCVINVEND(@PARAM1, @PARAM2)"
                Dim SqlFinish As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                            "/RCVINVEND(@PARAM1, @PARAM2)"
                If Not (err.HasError) Then
                    Dim cmdFinish As iDB2Command = New iDB2Command(SqlFinish, conSystem21)
                    With cmdFinish
                        parmInput = .Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                        parmInput.Value = InvoiceNumber
                        parmInput.Direction = ParameterDirection.Input
                        parmOutput = .Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                        parmOutput.Value = String.Empty
                        parmOutput.Direction = ParameterDirection.InputOutput
                        .ExecuteNonQuery()
                        PARMOUT = .Parameters(Param2).Value.ToString
                    End With
                End If

            Catch ex As Exception
                Const strError3 As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError3
                    .ErrorNumber = "TACDBSI-19"
                    .HasError = True
                    Return err
                End With
            End Try
        End If
        Return err
    End Function
    Protected Function AccessDataBaseSystem21_SupplierOrderAcknowledgement() As ErrorObj

        '---------------------------------------------------------------------------
        '   AccessDataBaseSystem21: Write the invoice to System 21 via Stored Procedures
        '
        Dim err As New ErrorObj
        Dim sProcError As String = String.Empty
        '---------------------------------------------------------------------------
        '   Open up collection and deal with by putting to stored procedures parameters 
        '
        err = DataEntityUnPackSystem21()
        If Not err.HasError Then
            '------------------------------------------------------------------
            '   Setup Calls to As400
            ' 
            Dim SqlHead As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/RCVACKHDR(@PARAM1, @PARAM2)"
            Dim SqlItem As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/RCVACKLIN(@PARAM1, @PARAM2)"

            Dim cmdHead As iDB2Command = Nothing
            Dim cmdItem As iDB2Command = Nothing

            Dim iItems As Integer = 0
            Dim iCount As Integer = 0

            Dim parmInput, parmOutput As iDB2Parameter
            Dim PARMOUT As String = String.Empty
            Dim parmCompanyCode As String = String.Empty
            Dim parmSupplierCode As String = String.Empty
            Dim parmPoNumber As String = String.Empty

            Dim strTalentOrderNo As String = String.Empty
            '----------------------------------------------------------------
            '   Loop through Invoices in transaction
            '
            Try
                For iCount = 1 To 500
                    If ParmHEAD(iCount) > String.Empty Then
                        '----------------------------------------------------------------
                        '   Call Invoice header stored procedure to write to Talent  
                        '
                        cmdHead = New iDB2Command(SqlHead, conSystem21)
                        parmCompanyCode = ParmHEAD(iCount).Substring(0, 2)
                        parmSupplierCode = ParmHEAD(iCount).Substring(2, 8)
                        parmPoNumber = ParmHEAD(iCount).Substring(10, 15)
                        With cmdHead
                            parmInput = .Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                            parmInput.Value = ParmHEAD(iCount)
                            parmInput.Direction = ParameterDirection.Input
                            parmOutput = .Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                            parmOutput.Value = String.Empty
                            parmOutput.Direction = ParameterDirection.InputOutput
                            .ExecuteNonQuery()
                            PARMOUT = .Parameters(Param2).Value.ToString
                        End With
                        strTalentOrderNo = Utilities.FixStringLength(PARMOUT.Substring(0, 15), 15)
                        If PARMOUT.Substring(1023, 1) = "Y" Then
                            With err
                                .ItemErrorMessage(iCount) = PARMOUT.Substring(1020, 4)
                                .ItemErrorCode(iCount) = "TACDBSI-16"
                                .ItemErrorStatus(iCount) = "Error creating Supplier Order Ack header " & PARMOUT.Substring(1019, 4)
                                .ErrorMessage = PARMOUT.Substring(1019, 4)
                                .ErrorNumber = "TACDBSI-16"
                                .ErrorStatus = "Error creating Supplier Order Ack header " & PARMOUT.Substring(1019, 4) & " - " & Descriptions.GetDescription(Settings.FrontEndConnectionString, _
                                                    "ENG", "ERRORCODE", PARMOUT.Substring(1019, 4))
                                .HasError = True
                                Return err
                            End With
                        Else
                            '----------------------------------------------------------------
                            '   Loop through items in order ack
                            ' 
                            For iItems = 1 To 500
                                If ParmITEM(iCount, iItems) > String.Empty Then
                                    '---------------------------------------------------------
                                    '   Call order detail stored procedure to write to Talent  
                                    ' 
                                    cmdItem = New iDB2Command(SqlItem, conSystem21)
                                    With cmdItem
                                        PARMOUT = String.Empty
                                        parmInput = .Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                                        parmInput.Value = ParmITEM(iCount, iItems)
                                        parmInput.Direction = ParameterDirection.Input
                                        parmOutput = .Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                                        parmOutput.Value = String.Empty
                                        parmOutput.Direction = ParameterDirection.InputOutput
                                        .ExecuteNonQuery()
                                        PARMOUT = .Parameters(Param2).Value.ToString
                                    End With
                                    If PARMOUT.Substring(1023, 1) = "Y" Then
                                        With err
                                            .ItemErrorMessage(iCount) = PARMOUT.Substring(1020, 4)
                                            .ItemErrorCode(iCount) = "TACDBSI-17"
                                            .ItemErrorStatus(iCount) = "Error creating Invoice item"
                                            .ErrorMessage = PARMOUT.Substring(1020, 4)
                                            .ErrorNumber = "TACDBSI-17"
                                            .ErrorStatus = "Error creating Order Ack item"
                                            .HasError = True
                                            Return err
                                        End With
                                    End If
                                Else
                                    Exit For
                                End If

                            Next iItems
                            '---------------------------------------------------------------------------------------------
                            '   Last item now written - if no error then write to System 21  
                            ' 
                            Dim SqlFinish As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                                        "/RCVACKEND(@PARAM1, @PARAM2)"
                            If Not (err.HasError) Then
                                Dim cmdFinish As iDB2Command = New iDB2Command(SqlFinish, conSystem21)
                                With cmdFinish
                                    parmInput = .Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                                    parmInput.Value = parmCompanyCode & parmSupplierCode & parmPoNumber
                                    parmInput.Direction = ParameterDirection.Input
                                    parmOutput = .Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                                    parmOutput.Value = String.Empty
                                    parmOutput.Direction = ParameterDirection.InputOutput
                                    .ExecuteNonQuery()
                                    PARMOUT = .Parameters(Param2).Value.ToString
                                End With
                            End If
                        End If
                    Else
                        Exit For
                    End If
                Next iCount


            Catch ex As Exception
                Const strError3 As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError3
                    .ErrorNumber = "TACDBSI-19"
                    .HasError = True
                    Return err
                End With
            End Try
        End If
        Return err
    End Function

    Public Function ReadInvoiceStatus(Optional ByVal opendb As Boolean = False) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------
        Select Case Settings.DestinationDatabase
            Case Is = SYSTEM21
                If opendb Then err = System21Open()
                If Not err.HasError Then err = ReadInvoiceStatusSystem21()
                If opendb Then System21Close()
            Case Is = SQL2005
        End Select
        Return err
    End Function

    Private Function DataEntityUnPackSystem21() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------------
        '   put the parameter generation in its own function as it is likely to be used  
        '   several times
        '
        Dim deoh As New DESupplierInvoice
        Dim deoi As New DESupplierInvoiceLines

        Dim supplierOrderAcknowledgement As New DESupplierOrderAcknowledgement
        Dim supplierOrderAcknowledgementLines As New DESupplierOrderAcknowledgementLines

        Dim iCount As Integer = 0
        Dim iItem As Integer = 0
        Dim iItems As Integer = 0
        Dim sHeads As StringBuilder = Nothing
        Dim sItems As StringBuilder = Nothing
        '
        Try
            With Dep
                '-----------------------------------------------------------------------------------------
                For iCount = 1 To .collDEHeader.Count
                    Select Case _settings.ModuleName
                        
                        Case Is = "WriteInvoice"
                            '--------------
                            ' Write Invoice
                            '--------------
                            deoh = Dep.collDEHeader.Item(iCount)
                            sHeads = New StringBuilder
                            With deoh
                                InvoiceNumber = .InvoiceNumber
                                sHeads.Append(Utilities.FixStringLength(.VendorNumber, 10))
                                sHeads.Append(Utilities.FixStringLength(.InvoiceNumber, 15))
                                sHeads.Append(Utilities.FixStringLength(.InvoiceDate, 8))
                                sHeads.Append(Utilities.FixStringLength(.PurchaseOrderNumber, 15))
                                sHeads.Append(Utilities.FixStringLength((.InvoiceAmount.ToString).PadLeft(9, "0"), 9))
                                sHeads.Append(Utilities.FixStringLength((.VATAmount.ToString).PadLeft(9, "0"), 9))
                                sHeads.Append(Utilities.FixStringLength(.InvoicedProcessed, 1))
                                sHeads.Append(Utilities.FixStringLength(.CompanyCode, 3))
                                sHeads.Append(Utilities.FixStringLength(.CurrencyCode, 3))
                                sHeads.Append(Utilities.FixStringLength((.GrossAmount.ToString).PadLeft(9, "0"), 9))
                                sHeads.Append(Utilities.FixStringLength(.CustomerOrderReference, 20))
                                sHeads.Append(Utilities.FixStringLength(.OrderNumber, 7))
                                sHeads.Append(Utilities.FixStringLength(.DespatchDate, 8))
                                sHeads.Append(Utilities.FixStringLength(.DeliveryNoteNumber, 20))
                                sHeads.Append(Utilities.FixStringLength(.ProofOfDelivery, 10))
                                sHeads.Append(Utilities.FixStringLength(.CustomerOrderDate, 8))
                                sHeads.Append(Utilities.FixStringLength(.DeliveryName, 35))
                                sHeads.Append(Utilities.FixStringLength(.DeliveryAddress1, 35))
                                sHeads.Append(Utilities.FixStringLength(.DeliveryAddress2, 35))
                                sHeads.Append(Utilities.FixStringLength(.DeliveryAddress3, 35))
                                sHeads.Append(Utilities.FixStringLength(.DeliveryAddress4, 35))
                                sHeads.Append(Utilities.FixStringLength(.DeliveryAddress5, 35))
                                sHeads.Append(Utilities.FixStringLength(.DeliveryPostcode, 12))
                                sHeads.Append(Utilities.FixStringLength(.PaymentMethod, 3))
                                sHeads.Append(Utilities.FixStringLength((.SettlementDiscount.ToString).PadLeft(15, "0"), 15))

                                ParmHEAD(iCount) = sHeads.ToString
                            End With
                            iItem = 0
                            For iItems = 1 To Dep.collDEInfo.Count
                                deoi = Dep.collDEInfo.Item(iItems)
                                sItems = New StringBuilder
                                If deoi.InvoiceNumber.Equals(InvoiceNumber) Then
                                    With deoi
                                        sItems.Append(Utilities.FixStringLength(.InvoiceNumber, 15))
                                        sItems.Append(Utilities.FixStringLength((.InvoiceLine.ToString).PadLeft(6, "0"), 6))
                                        sItems.Append(Utilities.FixStringLength(.QuantityInvoiced, 11))
                                        sItems.Append(Utilities.FixStringLength(.UnitOfMeasure, 3))
                                        sItems.Append(Utilities.FixStringLength((.InvoiceLineNetAmount.ToString).PadLeft(9, "0"), 9))
                                        sItems.Append(Utilities.FixStringLength((.InvoiceLineVatAmount.ToString).PadLeft(9, "0"), 9))
                                        sItems.Append(Utilities.FixStringLength(.ProductCode, 15))
                                        sItems.Append(Utilities.FixStringLength(.CompanyCode, 3))
                                        sItems.Append(Utilities.FixStringLength(.VATCode, 3))
                                        sItems.Append(Utilities.FixStringLength(.LocationCode, 5))
                                        sItems.Append(Utilities.FixStringLength(.CurrencyCode, 3))
                                        sItems.Append(Utilities.FixStringLength(.Description, 35))
                                        sItems.Append(Utilities.FixStringLength((.DespatchTime.ToString).PadLeft(6, "0"), 6))
                                        sItems.Append(Utilities.FixStringLength(.VATRate, 8))
                                        sItems.Append(Utilities.FixStringLength((.UnitCostPrice.ToString).PadLeft(15, "0"), 15))

                                        iItem += 1
                                        ParmITEM(iCount, iItem) = sItems.ToString
                                    End With
                                End If
                            Next iItems

                        Case Is = "SupplierOrderAcknowledgement"
                            '-------------------------------
                            ' Supplier Order Acknowledgement
                            '-------------------------------
                            supplierOrderAcknowledgement = Dep.collDEHeader.Item(iCount)
                            sHeads = New StringBuilder
                            With supplierOrderAcknowledgement
                                PoNumber = .PoNumber
                                sHeads.Append(Utilities.FixStringLength(.CompanyCode, 2))
                                sHeads.Append(Utilities.FixStringLength(.VendorNumber, 8))
                                sHeads.Append(Utilities.FixStringLength(.PoNumber, 15))
                                sHeads.Append(Utilities.FixStringLength(.DespatchDate, 8))

                                ParmHEAD(iCount) = sHeads.ToString
                            End With
                            iItem = 0
                            For iItems = 1 To Dep.collDEInfo.Count
                                supplierOrderAcknowledgementLines = Dep.collDEInfo.Item(iItems)
                                sItems = New StringBuilder
                                If supplierOrderAcknowledgementLines.PoNumber.Equals(PoNumber) Then
                                    With supplierOrderAcknowledgementLines
                                        sItems.Append(Utilities.FixStringLength(supplierOrderAcknowledgement.CompanyCode, 2))
                                        sItems.Append(Utilities.FixStringLength(supplierOrderAcknowledgement.VendorNumber, 8))
                                        sItems.Append(Utilities.FixStringLength(PoNumber, 15))
                                        sItems.Append(Utilities.FixStringLength((.LineNumber.ToString.PadLeft(3, "0")), 3))
                                        sItems.Append(Utilities.FixStringLength(.ProductCode, 15))
                                        sItems.Append(Utilities.FixStringLength((.OrderQuantity.ToString.PadLeft(15, "0")), 15))
                                        sItems.Append(Utilities.FixStringLength(.UnitPrice, 15))
                                        sItems.Append(Utilities.FixStringLength(.DeliveryDate, 8))

                                        iItem += 1
                                        ParmITEM(iCount, iItem) = sItems.ToString
                                    End With
                                End If
                            Next iItems

                    End Select
               
                Next iCount
                '-----------------------------------------------------------------------------------------
            End With
        Catch ex As Exception
            Const strError17 As String = "Could not Unpack Data Entity "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError17
                err.ErrorNumber = "TACDBSI-17"
                err.HasError = True
            End With

        End Try
        Return err
    End Function

    Private Function ReadInvoiceStatusSystem21() As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------
        ResultDataSet = New DataSet
        ResultDataSet.Tables.Add(DtHeader)
        '
        If DtHeader.Columns.Count = 0 Then
            With DtHeader.Columns
                .Add("InvoiceNumber", GetType(String))
                .Add("InvoiceStatus", GetType(String))
            End With
        End If
        '----------------------------------------------------------------------
        Try
            Dim cmdSelect As iDB2Command = Nothing
            Dim dtr As iDB2DataReader = Nothing
            Dim dRow As DataRow = Nothing

            Const sqlSelect As String = "SELECT * FROM XXPVP100 WHERE I1CUS = @Param1 AND I1SHP =@Param2 AND I1PRC = 'N'"
            cmdSelect = New iDB2Command(sqlSelect, conSystem21)

            '--------------------------------------------------------------------------------------
            ' Only read Invoices for partner
            '
            With cmdSelect
                With .Parameters
                    .Add(Param1, iDB2DbType.iDB2VarChar).Value = Settings.AccountNo1
                    .Add(Param2, iDB2DbType.iDB2VarChar).Value = Settings.AccountNo2
                End With
                dtr = .ExecuteReader()
            End With
            '--------------------------------------------------------------------------------------
            With dtr

                If .HasRows Then
                    While .Read
                        '-------------------------------------------------------------------------------
                        dRow = Nothing
                        dRow = DtHeader.NewRow()

                        dRow("InvoiceNumber") = .GetString(.GetOrdinal("xxxxxx"))
                        dRow("InvoiceStatus") = .GetString(.GetOrdinal("xxxxxx"))

                        DtHeader.Rows.Add(dRow)

                    End While
                    dtr.Close()
                    cmdSelect = Nothing

                Else
                    Const strError11 As String = "No data to display"
                    With err
                        .ErrorMessage = String.Empty
                        .ErrorNumber = "TACDBSI-33"
                        .ErrorStatus = strError11
                        .HasError = True
                    End With
                End If
            End With

        Catch ex As Exception
            err.ErrorMessage = ex.Message
        End Try
        '--------------------------------------------------------------------------
        Return err
    End Function
    Private Function UpdateInvoiceSystem21() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        Dim libl As String = conSystem21.LibraryList
        Try
            Dim cmdSelect As iDB2Command = Nothing
            Const sqlSelect As String = "UPDATE XXPVP100 SET I1PRC = 'Y' WHERE I1CUS = @Param1 AND I1SHP =@Param2 AND I1PRC = 'N'"
            cmdSelect = New iDB2Command(sqlSelect, conSystem21)

            ' Only read Invoices for partner
            With cmdSelect.Parameters
                .Add(Param1, iDB2DbType.iDB2VarChar).Value = Settings.AccountNo1
                .Add(Param2, iDB2DbType.iDB2VarChar).Value = Settings.AccountNo2
            End With
            cmdSelect.ExecuteNonQuery()
            cmdSelect = Nothing
        Catch ex As Exception
        End Try
        '--------------------------------------------------------------------------
        Return err
    End Function

    Private Property ParmHEAD(ByVal order As Integer) As String
        Get
            Return _parmHEAD(order)
        End Get
        Set(ByVal value As String)
            _parmHEAD(order) = value
        End Set
    End Property
    Private Property ParmITEM(ByVal order As Integer, ByVal index As Integer) As String
        Get
            Return _parmITEM(order, index)
        End Get
        Set(ByVal value As String)
            _parmITEM(order, index) = value
        End Set
    End Property

End Class

