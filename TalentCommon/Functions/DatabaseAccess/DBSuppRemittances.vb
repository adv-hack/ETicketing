Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Supplier Remittances Requests
'
'       Date                        18/06/07
'
'       Author                      Ben Ford (based on DBPurchaseOrder) 
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDSRA- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBSuppRemittances
    Inherits DBAccess
    Private _der As New DeRemittances

    Public Property Der() As DeRemittances
        Get
            Return _der
        End Get
        Set(ByVal value As DeRemittances)
            _der = value
        End Set
    End Property

    Private _dtHead As New DataTable
    Private _dtDetails As New DataTable

    Private _parmTRAN As String
    Private _pOrders As String = String.Empty

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
    Protected Overrides Function AccessDataBaseSystem21() As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------
        '   Create command object
        '
        Dim cmdSELECT As iDB2Command = Nothing

        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                  "/RMEXT(@PARAM1, @PARAM2)"
        Dim parmInput, Paramoutput As iDB2Parameter
        Dim PARMOUT As String = String.Empty

        Try
            If Not err.HasError Then
                cmdSELECT = New iDB2Command(strHEADER, conSystem21)
                parmInput = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                parmInput.Value = Utilities.FixStringLength(Settings.AccountNo3, 2) & _
                                    Utilities.FixStringLength(Settings.AccountNo5, 8)
                parmInput.Direction = ParameterDirection.Input
                Paramoutput = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                Paramoutput.Value = String.Empty
                Paramoutput.Direction = ParameterDirection.InputOutput
                cmdSELECT.ExecuteNonQuery()
                PARMOUT = cmdSELECT.Parameters(Param2).Value.ToString
                err = ReadRemittanceSystem21()
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

    Public Function ReadRemittance(Optional ByVal opendb As Boolean = False) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------
        Select Case Settings.DestinationDatabase
            Case Is = SYSTEM21
                If opendb Then err = System21Open()
                If Not err.HasError Then err = ReadRemittanceSystem21()
                If opendb Then System21Close()
        End Select
        Return err
    End Function
    Private Function ReadRemittanceSystem21() As ErrorObj
        Dim err As New ErrorObj
        '---------------------------------------------------------------------------
        ResultDataSet = New DataSet
        ResultDataSet.Tables.Add(dtHead)
        ResultDataSet.Tables.Add(dtDetails)
        AddColumnsToDataTables()
        '---------------------------------------------------------------------------
        Try
            Dim cmdSelectHeader As iDB2Command = Nothing
            Dim dtrHead As iDB2DataReader = Nothing
            Dim dHead As DataRow = Nothing
            Dim dRow As DataRow = Nothing
            '-----------------------------------------------------------------------
            '   Read Remittance header
            '   Only read Remittance headers for supplier, then read transaction lines
            '   for each remittance header and put to datasets
            '
            '   Only put out header record if detail lines exist
            '-----------------------------------------------------------------------
            Const sqlSelectHeader As String = "SELECT * FROM XMROP100   " & _
                                              " WHERE R1CMP = @PARAM1 AND R1SUP = @PARAM2 AND " & _
                                              "  R1PRC <> 'Y' "
            cmdSelectHeader = New iDB2Command(sqlSelectHeader, conSystem21)
            cmdSelectHeader.Parameters.Add(Param1, iDB2DbType.iDB2VarChar, 2).Value = Settings.AccountNo3
            cmdSelectHeader.Parameters.Add(Param2, iDB2DbType.iDB2VarChar, 8).Value = Settings.AccountNo5
            dtrHead = cmdSelectHeader.ExecuteReader()
            With dtrHead
                If Not .HasRows Then
                    Const strError15 As String = "No data to display"
                    With err
                        .ErrorMessage = Settings.AccountNo1
                        .ErrorNumber = "TACDSRA01"
                        .ErrorStatus = strError15
                        .HasError = True
                    End With
                End If
            End With
            '-----------------------------------------------------------------------
            Dim supplierCode As String = String.Empty
            Dim paymentRef As String = String.Empty
            Dim NumberOfLinesOnOrder As Integer = 0
            Dim NumberOfOrders As Integer = 0
            '
            While dtrHead.Read
                With dtrHead
                    '
                    dHead = Nothing
                    dHead = dtHead.NewRow()
                    NumberOfOrders += 1

                    dHead("SupplierCode") = .GetString(.GetOrdinal("R1SUP")).Trim
                    dHead("PaymentRunNo") = .GetString(.GetOrdinal("R1RUN")).Trim
                    dHead("PaymentReference") = .GetString(.GetOrdinal("R1PRF")).Trim
                    dHead("PaymentMethod") = .GetString(.GetOrdinal("R1MTH")).Trim
                    Dim dt1 As String = Utilities.ISeriesDate(.GetString(.GetOrdinal("R1DAT")))
                    dHead("DocumentDate") = Date.Parse(dt1)
                    dHead("ChequeNo") = .GetString(.GetOrdinal("R1CHQ")).Trim
                    dHead("RemittanceCurrCode") = .GetString(.GetOrdinal("R1CUR")).Trim
                    dHead("RemittanceValue") = .GetString(.GetOrdinal("R1VAL")).Trim
                    dHead("SupplierName") = .GetString(.GetOrdinal("R1SNM")).Trim
                    dHead("SupplierAddress1") = .GetString(.GetOrdinal("R1SA1")).Trim
                    dHead("SupplierAddress2") = .GetString(.GetOrdinal("R1SA2")).Trim
                    dHead("SupplierAddress3") = .GetString(.GetOrdinal("R1SA3")).Trim
                    dHead("SupplierAddress4") = .GetString(.GetOrdinal("R1SA4")).Trim
                    dHead("SupplierAddress5") = .GetString(.GetOrdinal("R1SA5")).Trim
                    dHead("SupplierPostcode") = .GetString(.GetOrdinal("R1SPC")).Trim
                    dHead("SupplierBankAccNo") = .GetString(.GetOrdinal("R1SAC")).Trim
                    dHead("SupplierBankName") = .GetString(.GetOrdinal("R1SBN")).Trim
                    dHead("RemittanceLines") = .GetString(.GetOrdinal("R1LNS")).Trim

                    supplierCode = dHead("SupplierCode")
                    paymentRef = dHead("PaymentReference")
                End With
                '-----------------------------------------------------------------------
                '   Read purchase order detail lines
                ' 
                Dim cmdSelectItem As iDB2Command = Nothing
                Dim dtrItem As iDB2DataReader = Nothing

                Const sqlSelectItem As String = "SELECT * FROM XMROP200 WHERE R2CMP = @PARAM1 AND " & _
                                                "  R2SUP = @PARAM2 AND R2PRF = @PARAM3 ORDER BY R2PRF"
                cmdSelectItem = New iDB2Command(sqlSelectItem, conSystem21)
                cmdSelectItem.Parameters.Add(Param1, iDB2DbType.iDB2VarChar, 2).Value = Settings.AccountNo3
                cmdSelectItem.Parameters.Add(Param2, iDB2DbType.iDB2VarChar, 8).Value = Settings.AccountNo5
                cmdSelectItem.Parameters.Add(Param3, iDB2DbType.iDB2VarChar, 15).Value = paymentRef
                NumberOfLinesOnOrder = 0
                dtrItem = cmdSelectItem.ExecuteReader()
                '-----------------------------------------------------------------------
                With dtrItem
                    While .Read
                        '------------------------------------------------------
                        dRow = Nothing
                        dRow = dtDetails.NewRow()
                        dRow("SupplierCode") = .GetString(.GetOrdinal("R2SUP")).Trim
                        dRow("PaymentRunNo") = .GetString(.GetOrdinal("R2RUN"))
                        dRow("PaymentReference") = .Item("R2PRF").ToString.Trim
                        dRow("RemittanceLineNo") = .Item("R2LIN")
                        dRow("MasterItemType") = .GetString(.GetOrdinal("R2MIT"))
                        dRow("LedgerItemDocumentRefe") = .GetString(.GetOrdinal("R2DRF")).Trim
                        dRow("SuppliersRef") = .GetString(.GetOrdinal("R2SRF")).Trim
                        dRow("SOPORderNo") = .GetString(.GetOrdinal("R2SOP")).Trim
                        dRow("PostingAmount") = .GetString(.GetOrdinal("R2PAP")).Trim
                        dRow("DiscountAmount") = .GetString(.GetOrdinal("R2DAP")).Trim
                        dRow("VAT") = .GetString(.GetOrdinal("R2VAP")).Trim
                        dtDetails.Rows.Add(dRow)
                        NumberOfLinesOnOrder += 1
                        '
                    End While
                    .Close()
                End With
                '--------------------------------------------------------------------
                '   Only put out header record if detail lines exist
                '
                If NumberOfLinesOnOrder > 0 Then dtHead.Rows.Add(dHead)
                err = UpdateRemittanceSystem21(supplierCode, paymentRef)
                '--------------------------------------------------------------------
                '   Or else it gets too big and times out
                '
                If NumberOfOrders > 20 Then Exit While
                '
            End While
            dtrHead.Close()
        Catch ex As Exception
            Const strError As String = "Error during Read Remittance in System21"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDSRA-03"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Private Function UpdateRemittanceSystem21(ByVal supplierCode As String, ByVal paymentRef As String) As ErrorObj
        Dim err As New ErrorObj
        '---------------------------------------------------------------------------
        Try
            Dim cmdSelect As iDB2Command = Nothing
            Dim sqlSelect As String = "UPDATE XMROP100 SET R1PRC = 'Y' WHERE R1SUP = '" & supplierCode & "'" & _
                                     " AND R1PRF = '" & paymentRef & "'"
            cmdSelect = New iDB2Command(sqlSelect, conSystem21)
            cmdSelect.ExecuteNonQuery()
            cmdSelect = Nothing
        Catch ex As Exception
            Const strError As String = "Error during Update Remittance in System21"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDSRA-04"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Private Sub AddColumnsToDataTables()
        '---------------------------------------------------------------------------
        With dtHead.Columns
            .Add("SupplierCode", GetType(String))
            .Add("PaymentRunNo", GetType(String))
            .Add("PaymentReference", GetType(String))
            .Add("PaymentMethod", GetType(String))
            .Add("DocumentDate", GetType(String))
            .Add("ChequeNo", GetType(String))
            .Add("RemittanceCurrCode", GetType(String))
            .Add("RemittanceValue", GetType(String))
            .Add("SupplierName", GetType(String))
            .Add("SupplierAddress1", GetType(String))
            .Add("SupplierAddress2", GetType(String))
            .Add("SupplierAddress3", GetType(String))
            .Add("SupplierAddress4", GetType(String))
            .Add("SupplierAddress5", GetType(String))
            .Add("SupplierPostcode", GetType(String))
            .Add("SupplierBankAccNo", GetType(String))
            .Add("SupplierBankName", GetType(String))
            .Add("RemittanceLines", GetType(String))

        End With
        '---------------------------------------------------------------------------
        With dtDetails.Columns
            .Add("SupplierCode", GetType(String))
            .Add("PaymentRunNo", GetType(String))
            .Add("PaymentReference", GetType(String))
            .Add("RemittanceLineNo", GetType(String))
            .Add("MasterItemType", GetType(String))
            .Add("LedgerItemDocumentRefe", GetType(String))
            .Add("SuppliersRef", GetType(String))
            .Add("SOPOrderNo", GetType(String))
            .Add("PostingAmount", GetType(String))
            .Add("DiscountAmount", GetType(String))
            .Add("VAT", GetType(String))

        End With
        '---------------------------------------------------------------------------
    End Sub
    Private Function DataEntityUnPack() As ErrorObj
        Dim err As New ErrorObj
        
        Return err
    End Function


End Class
