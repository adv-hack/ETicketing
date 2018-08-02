Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.Sql
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Order Status Requests
'
'       Date                        5th Feb 2007
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved. 
'
'       Error Number Code base      TACDBOS- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBOrderStatus
    Inherits DBAccess
    '---------------------------------------------------------------------------------------------
    Private _system21CompanyPo As String = String.Empty
    Private _dep As New DEOrder
    Private DB As New DBOrder
    Private _dtHeader As New DataTable

    Public Property System21CompanyPo() As String
        Get
            Return _system21CompanyPo
        End Get
        Set(ByVal value As String)
            _system21CompanyPo = value
        End Set
    End Property
    Public Property Dep() As DEOrder
        Get
            Return _dep
        End Get
        Set(ByVal value As DEOrder)
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
    Private Sub DtHeader_Columns()
        '---------------------------------------------------
        '   dtHeader            
        '   
        With DB.DtHeader.Columns
            '   <OrderNumbers>
            .Add("BranchOrderNumber", GetType(String))
            .Add("CarrierCode", GetType(String))
            .Add("CarrierCodeDescription", GetType(String))
            .Add("ContractNumber", GetType(Decimal))
            .Add("CustomerPO", GetType(String))
            .Add("DistributionWeight", GetType(String))
            .Add("FreightRate", GetType(Decimal))
            .Add("OrderNumber", GetType(String))
            '   <Address Type="ShipTo">
            .Add("ShipToAttention", GetType(String))
            .Add("ShipToName", GetType(String))
            .Add("ShipToAddress1", GetType(String))
            .Add("ShipToAddress2", GetType(String))
            .Add("ShipToAddress3", GetType(String))
            .Add("ShipToAddress4", GetType(String))
            .Add("ShipToCity", GetType(String))
            .Add("ShipToProvince", GetType(String))
            .Add("ShipToPostalCode", GetType(String))
            .Add("ShipToSuffix", GetType(String))
            .Add("ShipToCountry", GetType(String))
            '   <Address Type="BillTo">
            .Add("BillToAttention", GetType(String))
            .Add("BillToName", GetType(String))
            .Add("BillToAddress1", GetType(String))
            .Add("BillToAddress2", GetType(String))
            .Add("BillToAddress3", GetType(String))
            .Add("BillToAddress4", GetType(String))
            .Add("BillToCity", GetType(String))
            .Add("BillToProvince", GetType(String))
            .Add("BillToPostalCode", GetType(String))
            .Add("BillToSuffix", GetType(String))
            .Add("BillToCountry", GetType(String))
            .Add("ThirdPartyFreight", GetType(String))
            '   <OrderInformation>
            .Add("BackOrderStatus", GetType(String))
            .Add("ConfigFlag", GetType(String))
            .Add("ConfigTimeStamp", GetType(String))
            .Add("CreditCardSW", GetType(String))
            .Add("CreditMemoReasonCode", GetType(String))
            .Add("EndUserPO", GetType(String))
            .Add("EntryMethod", GetType(String))
            .Add("FrtOutCode", GetType(String))
            .Add("FulfillmentFlag", GetType(String))
            .Add("GovEndUserType", GetType(String))
            .Add("HoldReason", GetType(String))
            .Add("InvoiceDate", GetType(Date))
            .Add("NumberOfCartons", GetType(String))
            .Add("OECarrier", GetType(String))
            .Add("OrderWeight", GetType(String))
            .Add("OrderEntryDate", GetType(Date))
            .Add("OrderType", GetType(String))
            .Add("PromiseDate", GetType(Date))
            .Add("ProNbrSW", GetType(String))
            .Add("ProNbr", GetType(String))
            .Add("ResellerNBR", GetType(String))
            .Add("RMACode", GetType(String))
            .Add("SelSrcAcctnoHdr", GetType(String))
            .Add("SelSrcSlsHdr", GetType(String))
            .Add("ShipComplete", GetType(String))
            .Add("ShippableSW", GetType(String))
            .Add("SplitBillToSwitch", GetType(String))
            .Add("SplitFromOrderNumber", GetType(String))
            .Add("TermsCode", GetType(String))
            .Add("TermID", GetType(String))
            '   <OrderTotals>
            .Add("SalesTotal", GetType(String))
            .Add("FreightTotal", GetType(String))
            .Add("TaxTotal", GetType(String))
            .Add("SalePlusTax", GetType(String))
            .Add("GrandTotal", GetType(String))
            .Add("CODAmount", GetType(String))
            .Add("DiscountAmount", GetType(String))
            .Add("CurrencyCode", GetType(String))
            .Add("CompanyCurrency", GetType(String))
            .Add("CurrencyRate", GetType(String))
            '   </OrderTotals>
            .Add("OrderStatus", GetType(String))
            .Add("ShippedDate", GetType(Date))
            .Add("ShipFromBranch", GetType(String))
            .Add("ShipFromBranchNumber", GetType(String))

            .Add("CartonCount", GetType(String))
            .Add("TotalWeight", GetType(String))
            ''End If
            '-------------------------------------------------
        End With
        '
    End Sub

    Public Function ReadOrder() As ErrorObj
        Dim err As ErrorObj = Nothing
        ' If DB.DtHeader.Columns.Count = 0 Then DB.dtDtHeader_Columns()
        'If DtDetail.Columns.Count = 0 Then DtDetail_Columns()
        'If DtText.Columns.Count = 0 Then DtText_Columns()
        'If DtCarrier.Columns.Count = 0 Then DtCarrier_Columns()
        'If DtPackage.Columns.Count = 0 Then DtPackage_Columns()
        'If DtProduct.Columns.Count = 0 Then DtProduct_Columns()

        '-----------------------------------------------------------------------------
        '   by customer purchase order number
        '
        Const sqlSelect1 As String = "SELECT ORDN40 FROM OEP40 " & _
                                          " WHERE CONO40 = @PARAM1 AND CUSO40 = @PARAM2 AND " & _
                                           " CUSN40 = @CUSN40 " & _
                                          " ORDER BY ORDN40 "
        '-----------------------------------------------------------------------------
        '   by Branch order number
        '
        Const sqlSelect2 As String = "SELECT ORDN40 FROM OEP40 " & _
                                          " WHERE CONO40 = @PARAM1 AND ORDN40 = @PARAM2 AND " & _
                                           " CUSN40 = @CUSN40 " & _
                                          " ORDER BY ORDN40 "
        '-----------------------------------------------------------------------------
        '   by both
        '
        Const sqlSelect3 As String = "SELECT ORDN40 FROM OEP40 " & _
                                          " WHERE CONO40 = @PARAM1 AND CUSO40 = @PARAM2 AND " & _
                                           " ORDN40 = @ORDN40 AND CUSN40 = @CUSN40 " & _
                                          " ORDER BY ORDN40 "
        Dim cmdSelect As iDB2Command = Nothing
        Dim dtrReader As iDB2DataReader = Nothing
        Dim param1 As String = "@PARAM1"
        Dim param2 As String = "@PARAM2"
        Dim iOrder As Integer = 0
        '----------------------------------------------------------------------------------------
        Try
            err = System21Open()
            Dim dRow As DataRow
            Dim deoh As New DeOrderHeader                       ' Items  as Collection
            Dim OrderNumber As String = String.Empty
            With DB
                .System21CompanyNo = Settings.AccountNo3
                Settings.Authorised = True                      ' to prevent ReadOrderSystem21 being abused
                '-----------------------------------------------------------------------------
                If .ResultDataSet Is Nothing Then
                    .ResultDataSet = New DataSet
                    .ResultDataSet.Tables.Add(.DtHeader)        ' 0
                    '.ResultDataSet.Tables.Add(DtHeader)        ' 0
                    .ResultDataSet.Tables.Add(.DtDetail)        ' 1
                    .ResultDataSet.Tables.Add(.DtText)          ' 2
                    .ResultDataSet.Tables.Add(.DtCarrier)       ' 3
                    .ResultDataSet.Tables.Add(.DtPackage)       ' 4
                    .ResultDataSet.Tables.Add(.DtProduct)       ' 5
                    '
                    .ResultDataSet.Tables.Item(0).TableName = "DtHeader"
                    .ResultDataSet.Tables.Item(1).TableName = "DtDetail"
                    .ResultDataSet.Tables.Item(2).TableName = "DtText"
                    .ResultDataSet.Tables.Item(3).TableName = "DtCarrier"
                    .ResultDataSet.Tables.Item(4).TableName = "DtPackage"
                    .ResultDataSet.Tables.Item(5).TableName = "DtProduct"
                End If
                '-----------------------------------------------------------------------------
                Dim system21OrderNo As String
                For iOrder = 1 To Dep.CollDEOrders.Count
                    deoh = Dep.CollDEOrders.Item(iOrder)
                    .System21OrderNo = String.Empty
                    System21CompanyPo = deoh.CustomerPO
                    system21OrderNo = deoh.BranchOrderNumber
                    '-------------------------------------------------------------------------
                    If System21CompanyPo <> String.Empty And system21OrderNo = String.Empty Then
                        '-----------------------------------------------------------------------------
                        '   by customer purchase order number
                        '
                        cmdSelect = New iDB2Command(sqlSelect1, conSystem21)
                        Select Case Settings.DatabaseType1
                            Case Is = T65535
                                cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = .System21CompanyNo
                                cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 20).Value = System21CompanyPo
                                cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2CharBitData, 8).Value = Settings.AccountNo1.Trim
                            Case Is = T285
                                cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = .System21CompanyNo
                                cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 20).Value = System21CompanyPo
                                cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2Char, 8).Value = Settings.AccountNo1.Trim
                            Case Else
                                cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = .System21CompanyNo
                                cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 20).Value = System21CompanyPo
                                cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2Char, 8).Value = Settings.AccountNo1.Trim
                        End Select
                    ElseIf System21CompanyPo = String.Empty And system21OrderNo <> String.Empty Then
                        '-----------------------------------------------------------------------------
                        '   by Branch order number
                        '
                        cmdSelect = New iDB2Command(sqlSelect2, conSystem21)
                        Select Case Settings.DatabaseType1
                            Case Is = T65535
                                cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = .System21CompanyNo
                                cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 7).Value = system21OrderNo
                                cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2CharBitData, 8).Value = Settings.AccountNo1.Trim
                            Case Is = T285
                                cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = .System21CompanyNo
                                cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = system21OrderNo
                                cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2Char, 8).Value = Settings.AccountNo1.Trim
                            Case Else
                                cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = .System21CompanyNo
                                cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = system21OrderNo
                                cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2Char, 8).Value = Settings.AccountNo1.Trim
                        End Select
                    Else
                        '-----------------------------------------------------------------------------
                        '   by both
                        '
                        cmdSelect = New iDB2Command(sqlSelect3, conSystem21)
                        Select Case Settings.DatabaseType1
                            Case Is = T65535
                                cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = .System21CompanyNo
                                cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 20).Value = System21CompanyPo
                                cmdSelect.Parameters.Add("@ORDN40", iDB2DbType.iDB2CharBitData, 7).Value = system21OrderNo
                                cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2CharBitData, 8).Value = Settings.AccountNo1.Trim
                            Case Is = T285
                                cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = .System21CompanyNo
                                cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 20).Value = System21CompanyPo
                                cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = system21OrderNo
                                cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2Char, 8).Value = Settings.AccountNo1.Trim
                            Case Else
                                cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = .System21CompanyNo
                                cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 20).Value = System21CompanyPo
                                cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = system21OrderNo
                                cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2Char, 8).Value = Settings.AccountNo1.Trim
                        End Select
                    End If
                    '-------------------------------------------------------------------------
                    dtrReader = cmdSelect.ExecuteReader()
                    If Not err.HasError Then
                        .Settings = Settings
                        .Dep = Dep
                        If dtrReader.HasRows Then
                            While dtrReader.Read
                                OrderNumber = dtrReader.GetString(dtrReader.GetOrdinal("ORDN40"))
                                .System21OrderNo = OrderNumber
                                err = .ReadOrder(True)
                            End While
                        Else
                            dRow = Nothing
                            dRow = DB.DtHeader.NewRow()
                            dRow("BranchOrderNumber") = "Not Found"
                            dRow("CustomerPO") = System21CompanyPo
                            DB.DtHeader.Rows.Add(dRow)              ' Add to Header Table
                            '
                            With err
                                .ItemErrorMessage(iOrder) = String.Empty
                                .ItemErrorCode(iOrder) = "TACDBOR-036"
                                .ItemErrorStatus(iOrder) = "Error Purchase Order number does not exist on System21- " & System21CompanyPo
                            End With

                        End If
                    End If
                    '-------------------------------------------------------------------------
                Next iOrder
                ResultDataSet = DB.ResultDataSet
            End With
            System21Close()
            '
        Catch ex As Exception
            Const strError As String = "Error during Read By Purchase Order Number"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBOR-032"
                .HasError = True
                Return err
            End With
        End Try
        Return err
    End Function


End Class
