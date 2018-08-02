Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with PurchaseOrder Requests
'
'       Date                        Feb 2007
'
'       Author                      *** THIS IS WORK-IN-PROGRESS *** 
'
'       � CS Group 2006             All rights reserved.
'       Error Number Code base      TACDBPO- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBPurchaseOrder
    Inherits DBAccess

    Private _dep As New DEPurchaseOrder
    Private _dtHead As New DataTable("Header")
    Private _dtDetails As New DataTable("Details")
    Private _dtComments As New DataTable("Comments")

    Private _parmTRAN As String
    Private _pOrders As String = String.Empty

    Public Property Dep() As DEPurchaseOrder
        Get
            Return _dep
        End Get
        Set(ByVal value As DEPurchaseOrder)
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
    Public Property dtHead() As DataTable
        Get
            Return _dtHead
        End Get
        Set(ByVal value As DataTable)
            _dtHead = value
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
    Protected Overrides Function AccessDataBaseSystem21() As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------
        '   Create command object
        '
        Dim cmdSELECT As iDB2Command = Nothing

        ' Const strHEADER As String = "CALL WESTCOAST/POEXT(@PARAM1, @PARAM2)"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                  "/POEXT(@PARAM1, @PARAM2)"
        Dim parmInput, Paramoutput As iDB2Parameter
        Dim PARMOUT As String = String.Empty

        Try
            If Not err.HasError Then
                cmdSELECT = New iDB2Command(strHEADER, conSystem21)
                parmInput = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                ' Pass in blank Delivery Seq to extract all invoices for the company
                parmInput.Value = Utilities.FixStringLength(Settings.AccountNo3, 2) & _
                                    Utilities.FixStringLength(Settings.AccountNo5, 8)
                parmInput.Direction = ParameterDirection.Input
                Paramoutput = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                Paramoutput.Value = String.Empty
                Paramoutput.Direction = ParameterDirection.InputOutput
                cmdSELECT.ExecuteNonQuery()
                PARMOUT = cmdSELECT.Parameters(Param2).Value.ToString
                err = ReadPurchaseOrderSystem21()
            End If
        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPO-02"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Public Function ReadPurchaseOrder(Optional ByVal opendb As Boolean = False) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------
        Select Case Settings.DestinationDatabase
            Case Is = SYSTEM21
                If opendb Then err = System21Open()
                If Not err.HasError Then err = ReadPurchaseOrderSystem21()
                If opendb Then System21Close()
        End Select
        Return err
    End Function
    Private Function ReadPurchaseOrderSystem21() As ErrorObj
        Dim err As New ErrorObj
        '---------------------------------------------------------------------------
        ResultDataSet = New DataSet
        ResultDataSet.Tables.Add(dtHead)
        ResultDataSet.Tables.Add(dtDetails)
        ResultDataSet.Tables.Add(dtComments)
        AddColumnsToDataTables()
        '---------------------------------------------------------------------------
        Try
            Dim cmdSelectHeader As iDB2Command = Nothing
            Dim dtrHead As iDB2DataReader = Nothing
            Dim dHead As DataRow = Nothing
            Dim dRow As DataRow = Nothing
            '-----------------------------------------------------------------------
            '   Read purchase order header
            '   Only read purchase order headers for supplier, then read transaction lines
            '   for each oder header and put to datasets'
            '
            '   Only put out header record if detail lines exist
            '-----------------------------------------------------------------------
            Const sqlSelectHeader As String = "SELECT * FROM XMPOP100   " & _
                                                " WHERE XP1SNO <> 'Y'    " & _
                                                " AND XP1VND = @PARAM1   " & _
                                                " ORDER BY XP1ORD        "
            cmdSelectHeader = New iDB2Command(sqlSelectHeader, conSystem21)
            cmdSelectHeader.Parameters.Add(Param1, iDB2DbType.iDB2VarChar, 8).Value = Settings.AccountNo5
            dtrHead = cmdSelectHeader.ExecuteReader()
            With dtrHead
                If Not .HasRows Then
                    Const strError15 As String = "No data to display"
                    With err
                        .ErrorMessage = Settings.AccountNo1
                        .ErrorNumber = "TACDBPO15"
                        .ErrorStatus = strError15
                        .HasError = True
                    End With
                End If
            End With
            '-----------------------------------------------------------------------
            Dim PurchaseOrderNumber As String = String.Empty
            Dim NumberOfLinesOnOrder As Integer = 0
            Dim NumberOfOrders As Integer = 0
            '
            While dtrHead.Read
                With dtrHead
                    '
                    dHead = Nothing
                    dHead = dtHead.NewRow()
                    NumberOfOrders += 1

                    dHead("LOBCode") = .GetString(.GetOrdinal("XP1SYS")).Trim
                    dHead("DataStructureName") = .GetString(.GetOrdinal("XP1ORT")).Trim
                    PurchaseOrderNumber = .GetString(.GetOrdinal("XP1ORD")).Trim()
                    dHead("PurchaseOrderNumber") = PurchaseOrderNumber
                    dHead("VendorNumber") = .GetString(.GetOrdinal("XP1VND")).Trim
                    dHead("VendorType") = .GetString(.GetOrdinal("XP1CVT")).Trim
                    dHead("SendThisSession") = .GetString(.GetOrdinal("XP1SND")).Trim
                    dHead("SentOK") = .GetString(.GetOrdinal("XP1SNO")).Trim
                    dHead("XMLDocumentName") = .GetString(.GetOrdinal("XP1FIL")).Trim
                    dHead("ReleaseNumber") = .Item("XP1REL")
                    dHead("OrderStatus") = .GetString(.GetOrdinal("XP1STS")).Trim
                    dHead("BuyerID") = .GetString(.GetOrdinal("XP1BUY")).Trim
                    dHead("WarehouseID") = .GetString(.GetOrdinal("XP1WHS")).Trim
                    dHead("ShipToNumber") = .GetString(.GetOrdinal("XP1SHP")).Trim
                    dHead("CarrierCode") = .GetString(.GetOrdinal("XP1CRR")).Trim
                    Dim dt1 As String = Utilities.ISeriesDate(.GetString(.GetOrdinal("XP1ODT")))
                    dHead("OrderDateTime") = Date.Parse(dt1)
                    Dim dt2 As String = Utilities.ISeriesDate(.GetString(.GetOrdinal("XP1EDT")))
                    dHead("ExpectedDate") = Date.Parse(dt2)
                    Dim dt3 As String = Utilities.ISeriesDate(.GetString(.GetOrdinal("XP1RDT")))
                    dHead("RequestedReceiptDate") = Date.Parse(dt3)
                    NumberOfLinesOnOrder = .Item("XP1LNS")
                    dHead("NumberOfLinesOnOrder") = NumberOfLinesOnOrder
                    dHead("TotalQuantity") = .Item("XP1QUA")
                    dHead("TotalEstimatedCost") = .Item("XP1CST")
                    dHead("TaxTotal") = .Item("XP1TAX")
                    dHead("DiscountPercent") = .Item("XP1DSN")
                    dHead("DiscountAmount") = .Item("XP1DAM")
                    dHead("TransactionCurrencyCode") = .GetString(.GetOrdinal("XP1TCR")).Trim
                    dHead("BaseCurrencyCode") = .GetString(.GetOrdinal("XP1BCR")).Trim
                    dHead("TransactionExchangeRate") = .Item("XP1TER")
                    dHead("MultyDivideFlag") = .GetString(.GetOrdinal("XP1MDF")).Trim
                    dHead("InvoiceNumber") = .GetString(.GetOrdinal("XP1INV")).Trim
                    dHead("SpecialInstructions") = .GetString(.GetOrdinal("XP1SPC")).Trim
                    dHead("UDF01") = .GetString(.GetOrdinal("XP1F01")).Trim
                    dHead("UDF02") = .GetString(.GetOrdinal("XP1F02")).Trim
                    dHead("UDF03") = .GetString(.GetOrdinal("XP1F03")).Trim
                    dHead("UDF04") = .GetString(.GetOrdinal("XP1F04")).Trim
                    dHead("UDF05") = .GetString(.GetOrdinal("XP1F05")).Trim
                    dHead("UDF06") = .GetString(.GetOrdinal("XP1F06")).Trim
                    dHead("UDF07") = .GetString(.GetOrdinal("XP1F07")).Trim
                    dHead("UDF08") = .GetString(.GetOrdinal("XP1F08")).Trim
                    dHead("UDF09") = .GetString(.GetOrdinal("XP1F09")).Trim
                    dHead("UDF10") = .GetString(.GetOrdinal("XP1F10")).Trim
                End With
                '-----------------------------------------------------------------------
                '   Read delivery address for PO
                ' 
                Dim cmdDeliveryAddress As iDB2Command = Nothing
                Dim dtrDeliveryAddress As iDB2DataReader = Nothing
                Const sqlDeliveryAddress As String = "SELECT * FROM PMP45 WHERE CONO45 = @PARAM1 AND ORDN45 = @PARAM2"
                cmdDeliveryAddress = New iDB2Command(sqlDeliveryAddress, conSystem21)
                cmdDeliveryAddress.Parameters.Add(Param1, iDB2DbType.iDB2VarChar, 2).Value = Settings.AccountNo3
                cmdDeliveryAddress.Parameters.Add(Param2, iDB2DbType.iDB2VarChar, 7).Value = PurchaseOrderNumber
                Try
                    dtrDeliveryAddress = cmdDeliveryAddress.ExecuteReader()
                    If dtrDeliveryAddress.HasRows Then
                        With dtrDeliveryAddress
                            .Read()
                            dHead("DeliveryName") = .GetString(.GetOrdinal("ONAM45")).Trim
                            dHead("DeliveryAddress1") = .GetString(.GetOrdinal("OAD145")).Trim
                            dHead("DeliveryAddress2") = .GetString(.GetOrdinal("OAD245")).Trim
                            dHead("DeliveryAddress3") = .GetString(.GetOrdinal("OAD345")).Trim
                            dHead("DeliveryAddress4") = .GetString(.GetOrdinal("OAD445")).Trim
                            dHead("DeliveryAddress5") = .GetString(.GetOrdinal("OAD545")).Trim
                            dHead("DeliveryPostcode") = .GetString(.GetOrdinal("OPST45")).Trim
                        End With
                    End If
                Catch ex As Exception

                End Try

                '-----------------------------------------------------------------------
                '   Read purchase order detail lines
                ' 
                Dim cmdSelectItem As iDB2Command = Nothing
                Dim dtrItem As iDB2DataReader = Nothing

                Const sqlSelectItem As String = "SELECT * FROM XMPOP300 WHERE XP3ORD = @PARAM1 AND XP3VND = @PARAM2 ORDER BY XP3LIN"
                cmdSelectItem = New iDB2Command(sqlSelectItem, conSystem21)
                cmdSelectItem.Parameters.Add(Param1, iDB2DbType.iDB2VarChar, 15).Value = PurchaseOrderNumber
                cmdSelectItem.Parameters.Add(Param2, iDB2DbType.iDB2VarChar, 8).Value = Settings.AccountNo5
                dtrItem = cmdSelectItem.ExecuteReader()
                '-----------------------------------------------------------------------
                With dtrItem
                    While .Read
                        '------------------------------------------------------
                        dRow = Nothing
                        dRow = dtDetails.NewRow()
                        dRow("PurchaseOrderNumber") = .GetString(.GetOrdinal("XP3ORD")).Trim
                        dRow("VendorNumber") = .GetString(.GetOrdinal("XP3VND")).Trim
                        dRow("ReleaseNumber") = .Item("XP3REL")
                        dRow("LineNumber") = .Item("XP3LIN")
                        dRow("LineActionCode") = .GetString(.GetOrdinal("XP3LAC")).Trim
                        dRow("Line_OrderStatus") = .GetString(.GetOrdinal("XP3STS")).Trim
                        dRow("Line_BuyerID") = .GetString(.GetOrdinal("XP3BUY")).Trim
                        dRow("Line_WarehouseID") = .GetString(.GetOrdinal("XP3WHS")).Trim
                        dRow("ProductNumber") = .GetString(.GetOrdinal("XP3PRD")).Trim
                        dRow("ProductDescription") = .GetString(.GetOrdinal("XP3PDC")).Trim
                        dRow("UserProductNumber") = .GetString(.GetOrdinal("XP3UPR")).Trim
                        dRow("UserProductDescription") = .GetString(.GetOrdinal("XP3UDC")).Trim
                        dRow("MeasureCode") = .GetString(.GetOrdinal("XP3UTM")).Trim
                        Dim dt4 As String = Utilities.ISeriesDate(.GetString(.GetOrdinal("XP3DDT")))
                        dRow("DeliveryDate") = Date.Parse(dt4)
                        dRow("QuantityOrdered") = .Item("XP3OQU")
                        dRow("QuantityReceived") = .Item("XP3RQU")
                        dRow("QuantityOutstanding") = .Item("XP3OSQ")
                        dRow("EstimatedCost") = .Item("XP3ECS")
                        dRow("ActualSellingPrice") = .Item("XP3ASP")
                        dRow("SpecialSellingPrice") = .Item("XP3SSP")
                        dRow("Line_DiscountAmount") = .Item("XP3DAM")
                        dRow("Line_DiscountPercent") = .Item("XP3DSN")
                        dRow("LineInstructions") = .GetString(.GetOrdinal("XP3SPC")).Trim
                        dtDetails.Rows.Add(dRow)
                        '
                    End While
                    .Close()
                End With

                '-----------------------------------------------------------------------
                '   Read any comment lines - Internal
                ' 
                Dim cmdSelectComment As iDB2Command = Nothing
                Dim dtrComment As iDB2DataReader = Nothing

                Const sqlSelectComment As String = "SELECT * FROM PMFRETXT WHERE " & _
                                                    "CONO11 = @CONO11 AND " & _
                                                    "VNDR11 = @VNDR11 AND " & _
                                                    "XTYP11 = @XTYP11 AND " & _
                                                    "USGE11 = @USGE11 AND " & _
                                                    "XREF11 = @XREF11 "
                cmdSelectComment = New iDB2Command(sqlSelectComment, conSystem21)
                cmdSelectComment.Parameters.Add("@CONO11", iDB2DbType.iDB2VarChar, 2).Value = Settings.AccountNo3
                cmdSelectComment.Parameters.Add("@VNDR11", iDB2DbType.iDB2VarChar, 8).Value = Settings.AccountNo5
                cmdSelectComment.Parameters.Add("@XTYP11", iDB2DbType.iDB2VarChar, 1).Value = "O"
                cmdSelectComment.Parameters.Add("@USGE11", iDB2DbType.iDB2VarChar, 1).Value = " "
                cmdSelectComment.Parameters.Add("@XREF11", iDB2DbType.iDB2VarChar, 20).Value = PurchaseOrderNumber

                dtrComment = cmdSelectComment.ExecuteReader()
                '-----------------------------------------------------------------------
                With dtrComment
                    While .Read
                        '------------------------------------------------------
                        dRow = Nothing
                        dRow = dtComments.NewRow()
                        dRow("PurchaseOrderNumber") = PurchaseOrderNumber
                        dRow("VendorNumber") = Settings.AccountNo5
                        dRow("CommentType") = "INTERNAL"
                        dRow("LineNumber") = .Item("TLNO11")
                        dRow("Text") = .GetString(.GetOrdinal("TLIN11")).Trim
                        dtComments.Rows.Add(dRow)
                        '
                    End While
                    .Close()
                End With
                '-----------------------------------------------------------------------
                '   Read any comment lines - External
                ' 
                Dim cmdSelectCommentEx As iDB2Command = Nothing
                Dim dtrCommentEx As iDB2DataReader = Nothing

                Const sqlSelectCommentEx As String = "SELECT * FROM PMFRETXT WHERE " & _
                                                    "CONO11 = @CONO11 AND " & _
                                                    "VNDR11 = @VNDR11 AND " & _
                                                    "XTYP11 = @XTYP11 AND " & _
                                                    "USGE11 = @USGE11 AND " & _
                                                    "XREF11 = @XREF11 "
                cmdSelectCommentEx = New iDB2Command(sqlSelectCommentEx, conSystem21)
                cmdSelectCommentEx.Parameters.Add("@CONO11", iDB2DbType.iDB2VarChar, 2).Value = Settings.AccountNo3
                cmdSelectCommentEx.Parameters.Add("@VNDR11", iDB2DbType.iDB2VarChar, 8).Value = Settings.AccountNo5
                cmdSelectCommentEx.Parameters.Add("@XTYP11", iDB2DbType.iDB2VarChar, 1).Value = "O"
                cmdSelectCommentEx.Parameters.Add("@USGE11", iDB2DbType.iDB2VarChar, 1).Value = "O"
                cmdSelectCommentEx.Parameters.Add("@XREF11", iDB2DbType.iDB2VarChar, 20).Value = PurchaseOrderNumber

                dtrCommentEx = cmdSelectCommentEx.ExecuteReader()
                '-----------------------------------------------------------------------
                With dtrCommentEx
                    While .Read
                        '------------------------------------------------------
                        dRow = Nothing
                        dRow = dtComments.NewRow()
                        dRow("PurchaseOrderNumber") = PurchaseOrderNumber
                        dRow("VendorNumber") = Settings.AccountNo5
                        dRow("CommentType") = "EXTERNAL"
                        dRow("LineNumber") = .Item("TLNO11")
                        dRow("Text") = .GetString(.GetOrdinal("TLIN11")).Trim
                        dtComments.Rows.Add(dRow)
                        '
                    End While
                    .Close()
                End With
                '--------------------------------------------------------------------
                '   Only put out header record if detail lines exist
                '
                If NumberOfLinesOnOrder > 0 Then dtHead.Rows.Add(dHead)
                '
                err = UpdatePurchaseOrderSystem21(PurchaseOrderNumber)
                '
                '--------------------------------------------------------------------
                '   Or else it gets to big and times out
                '
                If NumberOfOrders > 20 Then Exit While
                '
            End While
            dtrHead.Close()
        Catch ex As Exception
            Const strError As String = "Error during Read Purchase Order System21"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPO-03"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Private Function UpdatePurchaseOrderSystem21(ByVal PurchaseOrderNumber As String) As ErrorObj
        Dim err As New ErrorObj
        '---------------------------------------------------------------------------
        Try
            Dim cmdSelect As iDB2Command = Nothing
            Dim sqlSelect As String = "UPDATE XMPOP100 SET XP1SNO = 'Y' WHERE XP1ORD = '" & PurchaseOrderNumber & "'" & _
                                        " AND XP1VND = '" & Settings.AccountNo5.Trim & "'"
            cmdSelect = New iDB2Command(sqlSelect, conSystem21)
            cmdSelect.ExecuteNonQuery()
            cmdSelect = Nothing
        Catch ex As Exception
            Const strError As String = "Error during Update Purchase Order System21"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPO-04"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Private Sub AddColumnsToDataTables()
        '---------------------------------------------------------------------------
        With dtHead.Columns
            .Add("PurchaseOrderNumber", GetType(String))
            .Add("LOBCode", GetType(String))
            .Add("DataStructureName", GetType(String))
            .Add("VendorNumber", GetType(String))
            .Add("VendorType", GetType(String))
            .Add("SendThisSession", GetType(String))
            .Add("SentOK", GetType(String))
            .Add("XMLDocumentName", GetType(String))
            .Add("ReleaseNumber", GetType(Integer))
            .Add("OrderStatus", GetType(String))
            .Add("BuyerID", GetType(String))
            .Add("WarehouseID", GetType(String))
            .Add("ShipToNumber", GetType(String))
            .Add("CarrierCode", GetType(String))
            .Add("OrderDateTime", GetType(Date))
            .Add("ExpectedDate", GetType(Date))
            .Add("RequestedReceiptDate", GetType(Date))
            .Add("NumberOfLinesOnOrder", GetType(Integer))
            .Add("TotalQuantity", GetType(Decimal))
            .Add("TotalEstimatedCost", GetType(Decimal))
            .Add("TaxTotal", GetType(Decimal))
            .Add("DiscountPercent", GetType(Decimal))
            .Add("DiscountAmount", GetType(Decimal))
            .Add("TransactionCurrencyCode", GetType(String))
            .Add("BaseCurrencyCode", GetType(String))
            .Add("TransactionExchangeRate", GetType(Decimal))
            .Add("MultyDivideFlag", GetType(String))
            .Add("InvoiceNumber", GetType(String))
            .Add("SpecialInstructions", GetType(String))
            .Add("UDF01", GetType(String))
            .Add("UDF02", GetType(String))
            .Add("UDF03", GetType(String))
            .Add("UDF04", GetType(String))
            .Add("UDF05", GetType(String))
            .Add("UDF06", GetType(String))
            .Add("UDF07", GetType(String))
            .Add("UDF08", GetType(String))
            .Add("UDF09", GetType(String))
            .Add("UDF10", GetType(String))
            .Add("DeliveryName", GetType(String))
            .Add("DeliveryAddress1", GetType(String))
            .Add("DeliveryAddress2", GetType(String))
            .Add("DeliveryAddress3", GetType(String))
            .Add("DeliveryAddress4", GetType(String))
            .Add("DeliveryAddress5", GetType(String))
            .Add("DeliveryPostcode", GetType(String))

        End With
        '---------------------------------------------------------------------------
        With dtDetails.Columns
            .Add("PurchaseOrderNumber", GetType(String))
            .Add("VendorNumber", GetType(String))
            .Add("ReleaseNumber", GetType(Integer))
            .Add("LineNumber", GetType(Integer))
            .Add("LineActionCode", GetType(String))
            .Add("Line_OrderStatus", GetType(String))
            .Add("Line_BuyerID", GetType(String))
            .Add("Line_WarehouseID", GetType(String))
            .Add("ProductNumber", GetType(String))
            .Add("ProductDescription", GetType(String))
            .Add("UserProductNumber", GetType(String))
            .Add("UserProductDescription", GetType(String))
            .Add("MeasureCode", GetType(String))
            .Add("DeliveryDate", GetType(Date))
            .Add("QuantityOrdered", GetType(Decimal))
            .Add("QuantityReceived", GetType(Decimal))
            .Add("QuantityOutstanding", GetType(Decimal))
            .Add("EstimatedCost", GetType(Decimal))
            .Add("ActualSellingPrice", GetType(Decimal))
            .Add("SpecialSellingPrice", GetType(Decimal))
            .Add("Line_DiscountAmount", GetType(Decimal))
            .Add("Line_DiscountPercent", GetType(Decimal))
            .Add("LineInstructions", GetType(String))
        End With
        With dtcomments.Columns
            .Add("PurchaseOrderNumber", GetType(String))
            .Add("VendorNumber", GetType(String))
            .Add("CommentType", GetType(String))
            .Add("LineNumber", GetType(Integer))
            .Add("Text", GetType(String))
        End With
        '---------------------------------------------------------------------------
    End Sub
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
            Const strError As String = "Error during Data Entity UnPack"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPO-09"
                .HasError = True
            End With
        End Try
        Return err
    End Function

End Class
