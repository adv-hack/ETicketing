Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.Sql
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Order Tracking Requests
'
'       Date                        5th Feb 2007
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved. 
'
'       Error Number Code base      TACDBOT- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBOrderTracking
    Inherits DBAccess
    '---------------------------------------------------------------------------------------------
    Private _system21CompanyPo As String = String.Empty
    Private _dep As New DEOrder
    Private DB As New DBOrder

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

    Public Function ReadOrder() As ErrorObj
        Dim err As ErrorObj = Nothing
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
                                .ItemErrorCode(iOrder) = "TACDBOT-036"
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
            Const strError As String = "Error during Read By Purchase Order Number / Branch Order Number"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBOT-032"
                .HasError = True
                Return err
            End With
        End Try
        Return err
    End Function

End Class
