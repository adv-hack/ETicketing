Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Order Change Requests
'
'       Date                        7th Nov 2006
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDBOC- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBOrderChange
    Inherits DBAccess
    '---------------------------------------------------------------------------------------------
    Private _dep As New DEOrder

    '---------------------------------------------------------------------------------------------
    Friend ord As DEOrderReponse         ' otherwise we hold previous order info

    Private _parmTRAN As String
    Private _parmORDER(10) As String
    Private _parmCUSTOMERPO(10) As String
    Private _parmHEAD(10) As String
    Private _parmADDR(10, 10) As String
    Private _parmADD(10, 100) As String
    Private _parmCHANGE(10, 100) As String
    Private _parmDELETE(10, 100) As String
    Private _parmCOMM1(10, 100) As String
    Private _parmCOMM2(10, 100) As String
    Private _parmCHANGESHIP(10) As String

    Private _dtChangeShip As New DataTable
    Private _dtAddLines As New DataTable
    Private _dtChangeLines As New DataTable
    Private _dtDeleteLines As New DataTable
    Private _dtAddComment As New DataTable
    Private _dtHeaderErrors As New DataTable

    Private Const AmendTicketingOrder As String = "AmendTicketingOrder"

    Public Property DeAmendTicketingOrder As DEAmendTicketingOrder


    Protected Overrides Function AccessDataBaseSystem21() As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------
        Dim cmdSELECT As iDB2Command = Nothing
        Dim parmCOMM, parmOUTPUT, parmINPUT As iDB2Parameter
        Dim iOrder As Integer = 0
        Dim iItem As Integer = 0
        Dim db As New DBOrderDetail
        AddChangeOrderColumns()
        err = DataEntityUnPackSystem21()
        Dim changeShipRow, addLinesRow, changeLinesRow, deleteLinesRow, addCommentRow As DataRow
        Dim dep As New DEOrder
        Dim deoh As DeOrderHeader
        Dim parmOutString As String = String.Empty
        '
        Dim strCHANGESHIP As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                  "/AMENDSHIP(@PARAM1, @PARAM2)"
        If Not err.HasError Then
            Try
                For iOrder = 1 To 10
                    '----------------------------------------
                    ' Only process order changes not in error
                    '----------------------------------------
                    If err.ItemErrorMessage(iOrder) = String.Empty Then
                        '---------------------------------------------
                        ' Call stored procedure to change ship address
                        '---------------------------------------------
                        If Not ParmCHANGESHIP(iOrder) Is Nothing AndAlso ParmCHANGESHIP(iOrder).Length > 0 Then
                            cmdSELECT = New iDB2Command(strCHANGESHIP, conSystem21)
                            '------------------------------------------------------------------------------  
                            parmINPUT = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                            parmINPUT.Value = ParmCHANGESHIP(iOrder)
                            parmINPUT.Direction = ParameterDirection.Input
                            '------------------------------------------------------------------------------
                            parmOUTPUT = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 1024)
                            parmOUTPUT.Value = "Outgoing"
                            parmOUTPUT.Direction = ParameterDirection.Output
                            cmdSELECT.ExecuteNonQuery()
                            parmOutString = parmOUTPUT.Value.ToString
                            changeShipRow = DtChangeShip.NewRow
                            ' Check for error
                            changeShipRow("OrderNo") = parmOutString.Substring(2, 15).Trim
                            If parmOUTPUT.Value.ToString.Substring(1023, 1) = "Y" Then
                                changeShipRow("ErrorCode") = parmOUTPUT.Value.ToString.Substring(1019, 4).Trim
                                changeShipRow("ErrorMessage") = Descriptions.GetDescription(Settings.FrontEndConnectionString, _
                                                                "ENG", "ERRORCODE", parmOutString.Substring(1019, 4)).Trim
                            Else
                                changeShipRow("ErrorCode") = String.Empty
                                changeShipRow("ErrorMessage") = String.Empty
                            End If
                            DtChangeShip.Rows.Add(changeShipRow)

                        End If
                        '-----------------------------------------
                        ' Call stored procedure to add order lines
                        '-----------------------------------------
                        Dim strADDLINE As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                                    "/ADDLINE(@PARAM1, @PARAM2)"
                        For iItem = 1 To 100
                            If Not ParmADD(iOrder, iItem) Is Nothing AndAlso ParmADD(iOrder, iItem).Length > 0 Then
                                cmdSELECT = New iDB2Command(strADDLINE, conSystem21)
                                '------------------------------------------------------------------------------  
                                parmINPUT = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                                parmINPUT.Value = ParmADD(iOrder, iItem)
                                parmINPUT.Direction = ParameterDirection.Input
                                '------------------------------------------------------------------------------
                                parmOUTPUT = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 1024)
                                parmOUTPUT.Value = "Outgoing"
                                parmOUTPUT.Direction = ParameterDirection.Output
                                cmdSELECT.ExecuteNonQuery()

                                parmOutString = parmOUTPUT.Value.ToString
                                addLinesRow = DtAddLines.NewRow
                                ' Check for error
                                addLinesRow("OrderNo") = parmOutString.Substring(2, 15).Trim
                                addLinesRow("SKU") = parmOutString.Substring(17, 15).Trim
                                addLinesRow("Quantity") = parmOutString.Substring(47, 9).Trim

                                addLinesRow("LineNo") = parmOutString.Substring(96, 3).Trim

                                If parmOUTPUT.Value.ToString.Substring(1023, 1) = "Y" Then
                                    addLinesRow("ErrorCode") = parmOutString.Substring(1019, 4).Trim
                                    addLinesRow("ErrorMessage") = Descriptions.GetDescription(Settings.FrontEndConnectionString, _
                                                            "ENG", "ERRORCODE", parmOutString.Substring(1019, 4)).Trim
                                    addLinesRow("Price") = 0
                                Else
                                    addLinesRow("ErrorCode") = String.Empty
                                    addLinesRow("ErrorMessage") = String.Empty
                                    Try
                                        addLinesRow("Price") = (CDec(parmOutString.Substring(81, 15).Trim / 100000)).ToString
                                    Catch
                                    End Try
                                End If

                                DtAddLines.Rows.Add(addLinesRow)
                            Else
                                Exit For
                            End If
                        Next iItem
                        '-----------------------------------------------------------------------------------
                        ' 
                        Dim strCHANGELINE As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                                      "/AMENDLINE(@PARAM1, @PARAM2)"
                        For iItem = 1 To 100
                            If Not ParmCHANGE(iOrder, iItem) Is Nothing AndAlso ParmCHANGE(iOrder, iItem).Length > 0 Then
                                cmdSELECT = New iDB2Command(strCHANGELINE, conSystem21)
                                '------------------------------------------------------------------------------  
                                parmINPUT = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                                parmINPUT.Value = ParmCHANGE(iOrder, iItem)
                                parmINPUT.Direction = ParameterDirection.Input
                                '------------------------------------------------------------------------------
                                parmOUTPUT = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 1024)
                                parmOUTPUT.Value = "Outgoing"
                                parmOUTPUT.Direction = ParameterDirection.Output
                                cmdSELECT.ExecuteNonQuery()

                                parmOutString = parmOUTPUT.Value.ToString
                                changeLinesRow = DtChangeLines.NewRow
                                ' Check for error
                                changeLinesRow("OrderNo") = parmOutString.Substring(2, 15).Trim
                                changeLinesRow("SKU") = parmOutString.Substring(986, 15).Trim
                                If changeLinesRow("SKU") = String.Empty Then
                                    changeLinesRow("SKU") = parmOutString.Substring(22, 15).Trim
                                End If
                                changeLinesRow("Quantity") = parmOutString.Substring(52, 9).Trim
                                changeLinesRow("OldLineNo") = parmOutString.Substring(17, 3).Trim

                                If parmOUTPUT.Value.ToString.Substring(1023, 1) = "Y" Then
                                    changeLinesRow("ErrorCode") = parmOutString.Substring(1019, 4).Trim
                                    changeLinesRow("ErrorMessage") = Descriptions.GetDescription(Settings.FrontEndConnectionString, _
                                                "ENG", "ERRORCODE", parmOutString.Substring(1019, 4)).Trim
                                    changeLinesRow("Price") = String.Empty
                                    changeLinesRow("LineNo") = String.Empty
                                Else
                                    changeLinesRow("ErrorCode") = String.Empty
                                    changeLinesRow("ErrorMessage") = String.Empty
                                    Try
                                        changeLinesRow("Price") = (CDec(parmOutString.Substring(86, 15).Trim) / 100000).ToString
                                    Catch ex As Exception
                                    End Try

                                    changeLinesRow("LineNo") = parmOutString.Substring(101, 3)

                                End If
                                DtChangeLines.Rows.Add(changeLinesRow)
                            Else
                                Exit For
                            End If
                        Next iItem
                        '-----------------------------------------------------------------------------------
                        ' 
                        Dim strDELETELINE As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                                      "/DELETELINE(@PARAM1, @PARAM2)"
                        For iItem = 1 To 100
                            If Not ParmDELETE(iOrder, iItem) Is Nothing AndAlso ParmDELETE(iOrder, iItem).Length > 0 Then
                                cmdSELECT = New iDB2Command(strDELETELINE, conSystem21)
                                '------------------------------------------------------------------------------  
                                parmINPUT = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                                parmINPUT.Value = ParmDELETE(iOrder, iItem)
                                parmINPUT.Direction = ParameterDirection.Input
                                '------------------------------------------------------------------------------
                                parmOUTPUT = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 1024)

                                parmOUTPUT.Direction = ParameterDirection.Output
                                parmOUTPUT.Value = "Outgoing"
                                cmdSELECT.ExecuteNonQuery()

                                parmOutString = parmOUTPUT.Value.ToString
                                deleteLinesRow = DtDeleteLines.NewRow
                                ' Check for error
                                deleteLinesRow("OrderNo") = parmOutString.Substring(2, 15).Trim
                                deleteLinesRow("LineNo") = parmOutString.Substring(17, 3).Trim
                                deleteLinesRow("CancelCode") = parmOutString.Substring(20, 2).Trim

                                If parmOUTPUT.Value.ToString.Substring(1023, 1) = "Y" Then
                                    deleteLinesRow("ErrorCode") = parmOutString.Substring(1019, 4).Trim
                                    deleteLinesRow("ErrorMessage") = Descriptions.GetDescription(Settings.FrontEndConnectionString, _
                                                                "ENG", "ERRORCODE", parmOutString.Substring(1019, 4)).Trim
                                Else
                                    deleteLinesRow("ErrorCode") = String.Empty
                                    deleteLinesRow("ErrorMessage") = String.Empty
                                End If
                                DtDeleteLines.Rows.Add(deleteLinesRow)
                            Else
                                Exit For
                            End If
                        Next iItem
                        '-----------------------------------------------------------------------------------
                        ' 
                        Dim strCOMMENT As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                                   "/ADDTEXT(@PARAM1, @PARAM2, @PARAM3)"
                        For iItem = 1 To 100
                            If Not ParmCOMM2(iOrder, iItem) Is Nothing AndAlso ParmCOMM2(iOrder, iItem).Length > 0 Then
                                cmdSELECT = New iDB2Command(strCOMMENT, conSystem21)
                                '------------------------------------------------------------------------------  
                                parmINPUT = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                                parmINPUT.Value = ParmCOMM1(iOrder, iItem)
                                parmINPUT.Direction = ParameterDirection.Input
                                '------------------------------------------------------------------------------
                                parmCOMM = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                                parmCOMM.Value = ParmCOMM2(iOrder, iItem)
                                parmCOMM.Direction = ParameterDirection.Input
                                '------------------------------------------------------------------------------
                                parmOUTPUT = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 1024)
                                parmOUTPUT.Value = "Outgoing"
                                parmOUTPUT.Direction = ParameterDirection.Output
                                cmdSELECT.ExecuteNonQuery()

                                parmOutString = parmOUTPUT.Value.ToString
                                addCommentRow = DtAddComment.NewRow
                                ' Check for error
                                addCommentRow("OrderNo") = parmOutString.Substring(2, 15).Trim
                                addCommentRow("Comment") = parmCOMM.Value.ToString.Trim

                                If parmOUTPUT.Value.ToString.Substring(1023, 1) = "Y" Then
                                    addCommentRow("ErrorCode") = parmOutString.Substring(1019, 4).Trim
                                    addCommentRow("ErrorMessage") = Descriptions.GetDescription(Settings.FrontEndConnectionString, _
                                                                "ENG", "ERRORCODE", parmOutString.Substring(1019, 4)).Trim
                                Else
                                    addCommentRow("ErrorCode") = String.Empty
                                    addCommentRow("ErrorMessage") = String.Empty
                                End If
                                DtAddComment.Rows.Add(addCommentRow)
                            Else
                                Exit For
                            End If
                        Next iItem
                        '
                        If Not ParmORDER(iOrder) Is Nothing AndAlso ParmORDER(iOrder) <> String.Empty Then
                            deoh = New DeOrderHeader
                            deoh.BranchOrderNumber = ParmORDER(iOrder)
                            dep.CollDEOrders.Add(deoh)
                        End If

                    Else
                        '--------------------------------------------------------
                        ' Error such as Order not found - Just write header error
                        '--------------------------------------------------------
                        'changeShipRow = DtChangeShip.NewRow
                        '' Check for error
                        'changeShipRow("OrderNo") = ParmORDER(iOrder)
                        'changeShipRow("CustomerPO") = ParmCUSTOMERPO(iOrder)
                        'changeShipRow("ErrorCode") = "D035"
                        'changeShipRow("ErrorMessage") = Descriptions.GetDescription(Settings.FrontEndConnectionString, _
                        '                                "ENG", "ERRORCODE", "D035")
                        'DtChangeShip.Rows.Add(changeShipRow)
                    End If
                    '
                Next iOrder

                With db
                    .Settings = Settings
                    '    .ResultDataSet = ResultDataSet
                    .Dep = dep
                    err = .ReadOrder()
                    ResultDataSet = .ResultDataSet
                    ResultDataSet.Tables.Add(DtChangeShip)
                    ResultDataSet.Tables.Add(DtAddLines)
                    ResultDataSet.Tables.Add(DtChangeLines)
                    ResultDataSet.Tables.Add(DtDeleteLines)
                    ResultDataSet.Tables.Add(DtAddComment)
                    ResultDataSet.Tables.Add(DtHeaderErrors)

                End With
                '------------------------------------------------------------
            Catch ex As Exception
                Const strError8 As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8
                    .ErrorNumber = "TACDBOC-02"
                    .HasError = True
                End With
            End Try
        End If
        '--------------------------------------------------------------------
        Return err
    End Function
    '--------------------------------------
    ' Unpack data structure for Sys21 calls
    '--------------------------------------
    Private Function DataEntityUnPackSystem21() As ErrorObj
        Const ModuleName As String = "DataEntityUnPack"
        Dim err As New ErrorObj
        Dim detr As New DETransaction           ' Items
        Dim deos As New DeOrders                ' DeOrderHeader, DEOrderInfo  as collections
        Dim deoh As New DeOrderHeader           ' Items and DEAddress as Collection
        Dim dead As New DeAddress               ' Items
        '
        Dim deoi As New DEOrderInfo             ' DEProductLines, DECommentLines  as collections
        Dim depr As DeProductLines              ' Items
        Dim decl As DeCommentLines              ' Items

        Dim iOrder As Integer = 0
        Dim iAddress As Integer = 0
        Dim iItems As Integer = 0
        Dim iComments As Integer = 0
        Dim dr As DataRow
        Try
            With Dep
                detr = .CollDETrans.Item(1)
                '--------------------------------------------------------------------------
                For iOrder = 1 To .CollDEOrders.Count

                    deos = Dep.CollDEOrders.Item(iOrder)
                    deoh = deos.DEOrderHeader

                    Dim tempErr As New ErrorObj
                    '--------------------------------------------------
                    ' Check order exists and get order no if not passed
                    '--------------------------------------------------
                    tempErr = ReadOrder(deoh)
                    If tempErr.HasError Then
                        err.ItemErrorCode(iOrder) = tempErr.ErrorNumber
                        err.ItemErrorMessage(iOrder) = tempErr.ErrorMessage
                        err.ItemErrorStatus(iOrder) = tempErr.ErrorStatus
                        dr = DtHeaderErrors.NewRow
                        dr("OrderNo") = deoh.BranchOrderNumber
                        dr("CustomerPO") = deoh.CustomerPO
                        dr("ErrorCode") = "D035"
                        dr("ErrorMessage") = Descriptions.GetDescription(Settings.FrontEndConnectionString, _
                                                        "ENG", "ERRORCODE", "D035")
                        DtHeaderErrors.Rows.Add(dr)
                    Else
                        '------------------------------------------
                        ' If order is complete/cancelled then error
                        '------------------------------------------
                        If deoh.Status = "C" OrElse deoh.Status = "X" Then
                            err.ItemErrorCode(iOrder) = "TACDBOC-037"
                            err.ItemErrorMessage(iOrder) = "Order is complete"
                            err.ItemErrorStatus(iOrder) = "Error Order is complete or cancelled in System21- " & deoh.BranchOrderNumber

                            dr = DtHeaderErrors.NewRow
                            dr("OrderNo") = deoh.BranchOrderNumber
                            dr("CustomerPO") = deoh.CustomerPO
                            dr("ErrorCode") = "D035b"
                            dr("ErrorMessage") = "Error Order is complete is System21- " & deoh.BranchOrderNumber
                            DtHeaderErrors.Rows.Add(dr)
                        End If

                    End If


                    With deoh
                        ParmORDER(iOrder) = .BranchOrderNumber
                        ParmCUSTOMERPO(iOrder) = .CustomerPO
                        ParmCHANGESHIP(iOrder) = BuildParmCHANGESHIP(deoh)
                    End With
                    '--------------------------------------------------------------------------
                    deoi = deos.DEOrderInfo
                    Dim iItemsAdd, iItemsChange, iItemsDelete As Integer
                    iItemsAdd = 1
                    iItemsChange = 1
                    iItemsDelete = 1

                    With deoi
                        For iItems = 1 To .CollDEProductLines.Count
                            depr = .CollDEProductLines.Item(iItems)
                            With depr
                                Select Case .Category
                                    Case Is = "AddLine"
                                        ParmADD(iOrder, iItemsAdd) = BuildParmADDLINE(deoh, depr)
                                        iItemsAdd += 1
                                    Case Is = "ChangeLine"
                                        ParmCHANGE(iOrder, iItemsChange) = BuildParmCHANGELINE(deoh, depr)
                                        iItemsChange += 1
                                    Case Is = "DeleteLine"
                                        ParmDELETE(iOrder, iItemsDelete) = BuildParmDELETELINE(deoh, depr)
                                        iItemsDelete += 1
                                End Select
                            End With
                            '--------------------------------------------
                            ' Check if there was an earlier pricing error
                            '--------------------------------------------
                            If depr.LineError Then
                                err.ItemErrorMessage(iOrder) = depr.SKU & " : " & depr.LineErrorMessage
                                err.ItemErrorCode(iOrder) = "TACDBOC-19"
                                err.ItemErrorStatus(iOrder) = depr.SKU & " : " & depr.LineErrorMessage

                                dr = DtHeaderErrors.NewRow
                                dr("OrderNo") = deoh.BranchOrderNumber
                                dr("CustomerPO") = deoh.CustomerPO
                                dr("ErrorCode") = "TACDBOC-19"
                                dr("ErrorMessage") = depr.SKU & " : " & depr.LineErrorMessage
                                DtHeaderErrors.Rows.Add(dr)
                            End If
                        Next
                        '-------------------------------------------------------------
                        For iComments = 1 To .CollDECommentLines.Count
                            decl = .CollDECommentLines.Item(iComments)
                            With decl
                                ParmCOMM1(iOrder, iComments) = Utilities.FixStringLength(Settings.AccountNo3, 2) & _
                                                                Utilities.FixStringLength(deoh.BranchOrderNumber, 15)
                                ParmCOMM2(iOrder, iComments) = .CommentText
                            End With
                        Next iComments
                    End With
                    '-------------------------------------------------------------
                Next iOrder
            End With
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = ModuleName & " Error"
                .ErrorNumber = "TACDBOC-99"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Public Function ReadOrder(ByRef deoh As DeOrderHeader) As ErrorObj
        Dim err As ErrorObj = Nothing
        '-----------------------------------------------------------------------------
        '   by customer purchase order number
        '
        Const sqlSelect1 As String = "SELECT ORDN40,STAT40 FROM OEP40 " & _
                                          " WHERE CONO40 = @PARAM1 AND CUSO40 = @PARAM2 AND " & _
                                           " CUSN40 = @CUSN40 " & _
                                          " ORDER BY ORDN40 "
        '-----------------------------------------------------------------------------
        '   by Branch order number
        '
        Const sqlSelect2 As String = "SELECT ORDN40, STAT40 FROM OEP40 " & _
                                          " WHERE CONO40 = @PARAM1 AND ORDN40 = @PARAM2 AND " & _
                                           " CUSN40 = @CUSN40 " & _
                                          " ORDER BY ORDN40 "
        '-----------------------------------------------------------------------------
        '   by both
        '
        Const sqlSelect3 As String = "SELECT ORDN40, STAT40 FROM OEP40 " & _
                                          " WHERE CONO40 = @PARAM1 AND CUSO40 = @PARAM2 AND " & _
                                           " ORDN40 = @ORDN40 AND CUSN40 = @CUSN40 " & _
                                          " ORDER BY ORDN40 "
        Dim cmdSelect As iDB2Command = Nothing
        Dim dtrReader As iDB2DataReader = Nothing
        Dim param1 As String = "@PARAM1"
        Dim param2 As String = "@PARAM2"

        '----------------------------------------------------------------------------------------
        Try
            err = System21Open()
            Dim OrderNumber As String = String.Empty
            Settings.Authorised = True                      ' to prevent ReadOrderSystem21 being abused
            '-----------------------------------------------------------------------------
            Dim system21OrderNo As String
            Dim system21CompanyPO As String

            system21CompanyPO = deoh.CustomerPO
            system21OrderNo = deoh.BranchOrderNumber
            '-------------------------------------------------------------------------
            If system21CompanyPO <> String.Empty And system21OrderNo = String.Empty Then
                '-----------------------------------------------------------------------------
                '   by customer purchase order number
                '
                cmdSelect = New iDB2Command(sqlSelect1, conSystem21)
                Select Case Settings.DatabaseType1
                    Case Is = T65535
                        cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = Settings.AccountNo3
                        cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 20).Value = system21CompanyPO
                        cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2CharBitData, 8).Value = Settings.AccountNo1.Trim
                    Case Is = T285
                        cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = Settings.AccountNo3
                        cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 20).Value = system21CompanyPO
                        cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2Char, 8).Value = Settings.AccountNo1.Trim
                    Case Else
                        cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = Settings.AccountNo3
                        cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 20).Value = system21CompanyPO
                        cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2Char, 8).Value = Settings.AccountNo1.Trim
                End Select
            ElseIf system21CompanyPO = String.Empty And system21OrderNo <> String.Empty Then
                '-----------------------------------------------------------------------------
                '   by Branch order number
                '
                cmdSelect = New iDB2Command(sqlSelect2, conSystem21)
                Select Case Settings.DatabaseType1
                    Case Is = T65535
                        cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = Settings.AccountNo3
                        cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 7).Value = system21OrderNo
                        cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2CharBitData, 8).Value = Settings.AccountNo1.Trim
                    Case Is = T285
                        cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = Settings.AccountNo3
                        cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = system21OrderNo
                        cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2Char, 8).Value = Settings.AccountNo1.Trim
                    Case Else
                        cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = Settings.AccountNo3
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
                        cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = Settings.AccountNo3
                        cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 20).Value = system21CompanyPO
                        cmdSelect.Parameters.Add("@ORDN40", iDB2DbType.iDB2CharBitData, 7).Value = system21OrderNo
                        cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2CharBitData, 8).Value = Settings.AccountNo1.Trim
                    Case Is = T285
                        cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = Settings.AccountNo3
                        cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 20).Value = system21CompanyPO
                        cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = system21OrderNo
                        cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2Char, 8).Value = Settings.AccountNo1.Trim
                    Case Else
                        cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = Settings.AccountNo3
                        cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 20).Value = system21CompanyPO
                        cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = system21OrderNo
                        cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2Char, 8).Value = Settings.AccountNo1.Trim
                End Select
            End If

            '-------------------------------------------------------------------------
            dtrReader = cmdSelect.ExecuteReader()

            If dtrReader.HasRows Then
                While dtrReader.Read
                    deoh.BranchOrderNumber = dtrReader.GetString(dtrReader.GetOrdinal("ORDN40"))
                    deoh.Status = dtrReader.GetString(dtrReader.GetOrdinal("STAT40"))
                End While
            Else
                With err
                    .ErrorMessage = "Order not found"
                    .ErrorNumber = "TACDBOC-036"
                    .ErrorStatus = "Error Purchase Order number does not exist on System21- " & system21CompanyPO
                    .HasError = True
                End With

            End If
            '-------------------------------------------------------------------------
            ' Keep open until after stored procedure calls
            ''     System21Close()
            '
        Catch ex As Exception
            Const strError As String = "Error during Read By Purchase Order Number / Branch Order Number"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBOC-032"
                .HasError = True
                Return err
            End With
        End Try
        Return err
    End Function
    '------------------------------------------
    ' Build parm for change of shipping address
    '------------------------------------------
    Private Function BuildParmCHANGESHIP(ByVal deoh As DeOrderHeader) As String
        Dim shipToAddress As New DeAddress
        Dim shipAddressFound As Boolean = False
        Dim strParmCHANGESHIP As String = String.Empty
        '--------------------------------
        ' Extension fields - header level
        '--------------------------------
        Dim parmExtReference1 As String = Utilities.FixStringLength(deoh.ExtensionReference1, 30)
        Dim parmExtReference2 As String = Utilities.FixStringLength(deoh.ExtensionReference2, 30)
        Dim parmExtReference3 As String = Utilities.FixStringLength(deoh.ExtensionReference3, 30)
        Dim parmExtReference4 As String = Utilities.FixStringLength(deoh.ExtensionReference4, 30)
        Dim parmExtFixedPrice1 As String = Utilities.FixStringLength(deoh.ExtensionFixedPrice1, 16)
        Dim parmExtFixedPrice2 As String = Utilities.FixStringLength(deoh.ExtensionFixedPrice2, 16)
        Dim parmExtFixedPrice3 As String = Utilities.FixStringLength(deoh.ExtensionFixedPrice3, 16)
        Dim parmExtFixedPrice4 As String = Utilities.FixStringLength(deoh.ExtensionFixedPrice4, 16)
        Dim parmExtDealID1 As String = Utilities.FixStringLength(deoh.ExtensionDealID1, 15)
        Dim parmExtDealID2 As String = Utilities.FixStringLength(deoh.ExtensionDealID2, 15)
        Dim parmExtDealID3 As String = Utilities.FixStringLength(deoh.ExtensionDealID3, 15)
        Dim parmExtDealID4 As String = Utilities.FixStringLength(deoh.ExtensionDealID4, 15)
        Dim parmExtDealID5 As String = Utilities.FixStringLength(deoh.ExtensionDealID5, 15)
        Dim parmExtDealID6 As String = Utilities.FixStringLength(deoh.ExtensionDealID6, 15)
        Dim parmExtDealID7 As String = Utilities.FixStringLength(deoh.ExtensionDealID7, 15)
        Dim parmExtDealID8 As String = Utilities.FixStringLength(deoh.ExtensionDealID8, 15)
        Dim parmExtFlag1 As String = Utilities.FixStringLength(deoh.ExtensionFlag1, 1)
        Dim parmExtFlag2 As String = Utilities.FixStringLength(deoh.ExtensionFlag2, 1)
        Dim parmExtFlag3 As String = Utilities.FixStringLength(deoh.ExtensionFlag3, 1)
        Dim parmExtFlag4 As String = Utilities.FixStringLength(deoh.ExtensionFlag4, 1)
        Dim parmExtFlag5 As String = Utilities.FixStringLength(deoh.ExtensionFlag5, 1)
        Dim parmExtFlag6 As String = Utilities.FixStringLength(deoh.ExtensionFlag6, 1)
        Dim parmExtFlag7 As String = Utilities.FixStringLength(deoh.ExtensionFlag7, 1)
        Dim parmExtStatus As String = Utilities.FixStringLength(deoh.ExtensionStatus, 1)


        For Each address As DeAddress In deoh.CollDEAddress
            If address.Category = "ShipTo" Then
                shipToAddress = address
                shipAddressFound = True
                Exit For
            End If
        Next

        Dim sb As New StringBuilder
        '   If shipAddressFound Then

        Dim parmCompanyNo As String = Utilities.FixStringLength(Settings.AccountNo3, 2)
        Dim parmOrderNo As String = Utilities.FixStringLength(deoh.BranchOrderNumber, 15)
        Dim parmShipToAttention As String = Utilities.FixStringLength(shipToAddress.Attention, 15)
        Dim parmAddress1 As String = Utilities.FixStringLength(shipToAddress.Line1, 35)
        Dim parmAddress2 As String = Utilities.FixStringLength(shipToAddress.Line2, 35)
        Dim parmAddress3 As String = Utilities.FixStringLength(shipToAddress.Line3, 35)
        Dim parmCity As String = Utilities.FixStringLength(shipToAddress.City, 25)
        Dim parmProvince As String = Utilities.FixStringLength(shipToAddress.Province, 25)
        Dim parmPostCode As String = Utilities.FixStringLength(shipToAddress.PostalCode, 15)
        With sb
            .Append(parmCompanyNo).Append(parmOrderNo).Append(parmShipToAttention).Append(parmAddress1)
            .Append(parmAddress2).Append(parmAddress3).Append(parmCity).Append(parmProvince).Append(parmPostCode)
            .Append(parmExtReference1).Append(parmExtReference2).Append(parmExtReference3).Append(parmExtReference4)
            .Append(parmExtFixedPrice1).Append(parmExtFixedPrice2).Append(parmExtFixedPrice3).Append(parmExtFixedPrice4)
            .Append(parmExtDealID1).Append(parmExtDealID2).Append(parmExtDealID3).Append(parmExtDealID4)
            .Append(parmExtDealID5).Append(parmExtDealID6).Append(parmExtDealID7).Append(parmExtDealID8)
            .Append(parmExtFlag1).Append(parmExtFlag2).Append(parmExtFlag3).Append(parmExtFlag4)
            .Append(parmExtFlag5).Append(parmExtFlag6).Append(parmExtFlag7).Append(parmExtStatus)

        End With

        '    End If
        strParmCHANGESHIP = sb.ToString
        Return strParmCHANGESHIP
    End Function
    '------------------------------------
    ' Build parm for adding a detail line
    '------------------------------------
    Private Function BuildParmADDLINE(ByVal deoh As DeOrderHeader, ByVal depr As DeProductLines) As String
        Dim sb As New StringBuilder
        Dim strParmADDLINE As String = String.Empty
        Dim parmCompanyNo As String = Utilities.FixStringLength(Settings.AccountNo3, 2)
        Dim parmOrderNo As String = Utilities.FixStringLength(deoh.BranchOrderNumber, 15)
        Dim parmSKU As String = Utilities.FixStringLength(depr.SKU, 15)
        Dim parmAltSKU As String = Utilities.FixStringLength(depr.AlternateSKU, 15)
        Dim parmQuantity As String = Utilities.FixStringLength(depr.Quantity, 9)
        Dim parmPrice As String = Utilities.FixStringLength(depr.FixedPrice, 10)
        '------------------------------
        ' Extension fields - line level
        '------------------------------
        Dim parmExtReference1 As String = Utilities.FixStringLength(depr.ExtensionReference1, 30)
        Dim parmExtReference2 As String = Utilities.FixStringLength(depr.ExtensionReference2, 30)
        Dim parmExtReference3 As String = Utilities.FixStringLength(depr.ExtensionReference3, 30)
        Dim parmExtReference4 As String = Utilities.FixStringLength(depr.ExtensionReference4, 30)
        Dim parmExtReference5 As String = Utilities.FixStringLength(depr.ExtensionReference5, 30)
        Dim parmExtReference6 As String = Utilities.FixStringLength(depr.ExtensionReference6, 30)
        Dim parmExtReference7 As String = Utilities.FixStringLength(depr.ExtensionReference7, 30)
        Dim parmExtReference8 As String = Utilities.FixStringLength(depr.ExtensionReference8, 30)
        Dim parmExtFlag1 As String = Utilities.FixStringLength(depr.ExtensionFlag1, 1)
        Dim parmExtFlag2 As String = Utilities.FixStringLength(depr.ExtensionFlag2, 1)
        Dim parmExtFlag3 As String = Utilities.FixStringLength(depr.ExtensionFlag3, 1)
        Dim parmExtFlag4 As String = Utilities.FixStringLength(depr.ExtensionFlag4, 1)
        Dim parmExtFlag5 As String = Utilities.FixStringLength(depr.ExtensionFlag5, 1)
        Dim parmExtFlag6 As String = Utilities.FixStringLength(depr.ExtensionFlag6, 1)
        Dim parmExtFlag7 As String = Utilities.FixStringLength(depr.ExtensionFlag7, 1)
        Dim parmExtFlag8 As String = Utilities.FixStringLength(depr.ExtensionFlag8, 1)
        Dim parmExtFlag9 As String = Utilities.FixStringLength(depr.ExtensionFlag9, 1)
        Dim parmExtFlag0 As String = Utilities.FixStringLength(depr.ExtensionFlag0, 1)
        Dim parmExtField1 As String = Utilities.FixStringLength(depr.ExtensionField1, 10)
        Dim parmExtField2 As String = Utilities.FixStringLength(depr.ExtensionField2, 10)
        Dim parmExtField3 As String = Utilities.FixStringLength(depr.ExtensionField3, 10)
        Dim parmExtField4 As String = Utilities.FixStringLength(depr.ExtensionField4, 10)
        Dim parmExtFixedPrice1 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice1, 16)
        Dim parmExtFixedPrice2 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice2, 16)
        Dim parmExtFixedPrice3 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice3, 16)
        Dim parmExtFixedPrice4 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice4, 16)
        Dim parmExtFixedPrice5 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice5, 16)
        Dim parmExtFixedPrice6 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice6, 16)
        Dim parmExtFixedPrice7 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice7, 16)
        Dim parmExtFixedPrice8 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice8, 16)
        Dim parmExtDealID1 As String = Utilities.FixStringLength(depr.ExtensionDealID1, 15)
        Dim parmExtDealID2 As String = Utilities.FixStringLength(depr.ExtensionDealID2, 15)
        Dim parmExtDealID3 As String = Utilities.FixStringLength(depr.ExtensionDealID3, 15)
        Dim parmExtDealID4 As String = Utilities.FixStringLength(depr.ExtensionDealID4, 15)
        Dim parmExtDealID5 As String = Utilities.FixStringLength(depr.ExtensionDealID5, 15)
        Dim parmExtDealID6 As String = Utilities.FixStringLength(depr.ExtensionDealID6, 15)
        Dim parmExtDealID7 As String = Utilities.FixStringLength(depr.ExtensionDealID7, 15)
        Dim parmExtDealID8 As String = Utilities.FixStringLength(depr.ExtensionDealID8, 15)
        Dim parmExtStatus As String = Utilities.FixStringLength(depr.ExtensionStatus, 1)

        With sb
            .Append(parmCompanyNo).Append(parmOrderNo).Append(parmSKU).Append(parmAltSKU)
            .Append(parmQuantity).Append(parmPrice)
            .Append(parmExtReference1).Append(parmExtReference2).Append(parmExtReference3).Append(parmExtReference4)
            .Append(parmExtReference5).Append(parmExtReference6).Append(parmExtReference7).Append(parmExtReference8)
            .Append(parmExtFlag1).Append(parmExtFlag2).Append(parmExtFlag3).Append(parmExtFlag4).Append(parmExtFlag5)
            .Append(parmExtFlag6).Append(parmExtFlag7).Append(parmExtFlag8).Append(parmExtFlag9).Append(parmExtFlag0)
            .Append(parmExtField1).Append(parmExtField2).Append(parmExtField3).Append(parmExtField4)
            .Append(parmExtFixedPrice1).Append(parmExtFixedPrice2).Append(parmExtFixedPrice3).Append(parmExtFixedPrice4)
            .Append(parmExtFixedPrice5).Append(parmExtFixedPrice6).Append(parmExtFixedPrice7).Append(parmExtFixedPrice8)
            .Append(parmExtDealID1).Append(parmExtDealID2).Append(parmExtDealID3).Append(parmExtDealID4)
            .Append(parmExtDealID5).Append(parmExtDealID6).Append(parmExtDealID7).Append(parmExtDealID8)
            .Append(parmExtStatus)
        End With
        strParmADDLINE = sb.ToString

        Return strParmADDLINE
    End Function
    '--------------------------------------
    ' Build parm for changing a detail line
    '--------------------------------------
    Private Function BuildParmCHANGELINE(ByVal deoh As DeOrderHeader, ByVal depr As DeProductLines) As String
        Dim sb As New StringBuilder
        Dim strParmCHANGELINE As String = String.Empty
        Dim parmCompanyNo As String = Utilities.FixStringLength(Settings.AccountNo3, 2)
        Dim parmOrderNo As String = Utilities.FixStringLength(deoh.BranchOrderNumber, 15)
        Dim parmLineNo As String = Utilities.FixStringLength(depr.WestCoastLineNumber, 3)
        Dim parmCancelCode As String = Utilities.FixStringLength(depr.CancellationCode, 2)
        Dim parmSKU As String = Utilities.FixStringLength(depr.SKU, 15)
        Dim parmAltSKU As String = Utilities.FixStringLength(depr.AlternateSKU, 15)
        Dim parmQuantity As String = Utilities.FixStringLength(depr.Quantity, 9)
        Dim parmPrice As String = Utilities.FixStringLength(depr.FixedPrice, 10)
        '------------------------------
        ' Extension fields - line level
        '------------------------------
        Dim parmExtReference1 As String = Utilities.FixStringLength(depr.ExtensionReference1, 30)
        Dim parmExtReference2 As String = Utilities.FixStringLength(depr.ExtensionReference2, 30)
        Dim parmExtReference3 As String = Utilities.FixStringLength(depr.ExtensionReference3, 30)
        Dim parmExtReference4 As String = Utilities.FixStringLength(depr.ExtensionReference4, 30)
        Dim parmExtReference5 As String = Utilities.FixStringLength(depr.ExtensionReference5, 30)
        Dim parmExtReference6 As String = Utilities.FixStringLength(depr.ExtensionReference6, 30)
        Dim parmExtReference7 As String = Utilities.FixStringLength(depr.ExtensionReference7, 30)
        Dim parmExtReference8 As String = Utilities.FixStringLength(depr.ExtensionReference8, 30)
        Dim parmExtFlag1 As String = Utilities.FixStringLength(depr.ExtensionFlag1, 1)
        Dim parmExtFlag2 As String = Utilities.FixStringLength(depr.ExtensionFlag2, 1)
        Dim parmExtFlag3 As String = Utilities.FixStringLength(depr.ExtensionFlag3, 1)
        Dim parmExtFlag4 As String = Utilities.FixStringLength(depr.ExtensionFlag4, 1)
        Dim parmExtFlag5 As String = Utilities.FixStringLength(depr.ExtensionFlag5, 1)
        Dim parmExtFlag6 As String = Utilities.FixStringLength(depr.ExtensionFlag6, 1)
        Dim parmExtFlag7 As String = Utilities.FixStringLength(depr.ExtensionFlag7, 1)
        Dim parmExtFlag8 As String = Utilities.FixStringLength(depr.ExtensionFlag8, 1)
        Dim parmExtFlag9 As String = Utilities.FixStringLength(depr.ExtensionFlag9, 1)
        Dim parmExtFlag0 As String = Utilities.FixStringLength(depr.ExtensionFlag0, 1)
        Dim parmExtField1 As String = Utilities.FixStringLength(depr.ExtensionField1, 10)
        Dim parmExtField2 As String = Utilities.FixStringLength(depr.ExtensionField2, 10)
        Dim parmExtField3 As String = Utilities.FixStringLength(depr.ExtensionField3, 10)
        Dim parmExtField4 As String = Utilities.FixStringLength(depr.ExtensionField4, 10)
        Dim parmExtFixedPrice1 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice1, 16)
        Dim parmExtFixedPrice2 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice2, 16)
        Dim parmExtFixedPrice3 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice3, 16)
        Dim parmExtFixedPrice4 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice4, 16)
        Dim parmExtFixedPrice5 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice5, 16)
        Dim parmExtFixedPrice6 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice6, 16)
        Dim parmExtFixedPrice7 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice7, 16)
        Dim parmExtFixedPrice8 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice8, 16)
        Dim parmExtDealID1 As String = Utilities.FixStringLength(depr.ExtensionDealID1, 15)
        Dim parmExtDealID2 As String = Utilities.FixStringLength(depr.ExtensionDealID2, 15)
        Dim parmExtDealID3 As String = Utilities.FixStringLength(depr.ExtensionDealID3, 15)
        Dim parmExtDealID4 As String = Utilities.FixStringLength(depr.ExtensionDealID4, 15)
        Dim parmExtDealID5 As String = Utilities.FixStringLength(depr.ExtensionDealID5, 15)
        Dim parmExtDealID6 As String = Utilities.FixStringLength(depr.ExtensionDealID6, 15)
        Dim parmExtDealID7 As String = Utilities.FixStringLength(depr.ExtensionDealID7, 15)
        Dim parmExtDealID8 As String = Utilities.FixStringLength(depr.ExtensionDealID8, 15)
        Dim parmExtStatus As String = Utilities.FixStringLength(depr.ExtensionStatus, 1)


        With sb
            .Append(parmCompanyNo).Append(parmOrderNo).Append(parmLineNo).Append(parmCancelCode)
            .Append(parmSKU).Append(parmAltSKU).Append(parmQuantity).Append(parmPrice)
            .Append(parmExtReference1).Append(parmExtReference2).Append(parmExtReference3).Append(parmExtReference4)
            .Append(parmExtReference5).Append(parmExtReference6).Append(parmExtReference7).Append(parmExtReference8)
            .Append(parmExtFlag1).Append(parmExtFlag2).Append(parmExtFlag3).Append(parmExtFlag4).Append(parmExtFlag5)
            .Append(parmExtFlag6).Append(parmExtFlag7).Append(parmExtFlag8).Append(parmExtFlag9).Append(parmExtFlag0)
            .Append(parmExtField1).Append(parmExtField2).Append(parmExtField3).Append(parmExtField4)
            .Append(parmExtFixedPrice1).Append(parmExtFixedPrice2).Append(parmExtFixedPrice3).Append(parmExtFixedPrice4)
            .Append(parmExtFixedPrice5).Append(parmExtFixedPrice6).Append(parmExtFixedPrice7).Append(parmExtFixedPrice8)
            .Append(parmExtDealID1).Append(parmExtDealID2).Append(parmExtDealID3).Append(parmExtDealID4)
            .Append(parmExtDealID5).Append(parmExtDealID6).Append(parmExtDealID7).Append(parmExtDealID8)
            .Append(parmExtStatus)
        End With

        strParmCHANGELINE = sb.ToString

        Return strParmCHANGELINE
    End Function
    '---------------------------------------
    ' Build parm for deleting a product line
    '---------------------------------------
    Private Function BuildParmDELETELINE(ByVal deoh As DeOrderHeader, ByVal depr As DeProductLines) As String
        Dim sb As New StringBuilder
        Dim strParmDELETELINE As String = String.Empty
        Dim parmCompanyNo As String = Utilities.FixStringLength(Settings.AccountNo3, 2)
        Dim parmOrderNo As String = Utilities.FixStringLength(deoh.BranchOrderNumber, 15)
        Dim parmLineNo As String = Utilities.FixStringLength(depr.WestCoastLineNumber, 3)
        Dim parmCancelCode As String = Utilities.FixStringLength(depr.CancellationCode, 2)

        With sb
            .Append(parmCompanyNo).Append(parmOrderNo).Append(parmLineNo).Append(parmCancelCode)
        End With
        strParmDELETELINE = sb.ToString

        Return strParmDELETELINE
    End Function

    Private Function DataEntityUnPack() As ErrorObj
        Const ModuleName As String = "DataEntityUnPack"
        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        '   put the parameter generation in its own function as it is likely to be used  
        '   several times
        '
        Dim detr As New DETransaction           ' Items
        Dim deos As New DeOrders                ' DeOrderHeader, DEOrderInfo  as collections
        '-------------------------------------------------------------------------------------
        Dim deoh As New DeOrderHeader           ' Items and DEAddress as Collection
        Dim dead As New DeAddress               ' Items
        '
        Dim deoi As New DEOrderInfo             ' DEProductLines, DECommentLines  as collections
        Dim depr As DeProductLines              ' Items
        Dim decl As DeCommentLines              ' Items

        Dim iOrder As Integer = 0
        Dim iAddress As Integer = 0
        Dim iItems As Integer = 0
        Dim iComments As Integer = 0
        Try
            With Dep
                '
                detr = .CollDETrans.Item(1)
                Const sTran As String = "SenderID = {0}, ReceiverID = {1}, CountryCode = {2}" & _
                                            ", TransactionID = {3}, ShowDetail = {4}"
                With detr
                    ParmTRAN = String.Format(sTran, _
                                                .SenderID, _
                                                .ReceiverID, _
                                                .CountryCode, _
                                                .TransactionID, _
                                                .ShowDetail)
                End With
                '--------------------------------------------------------------------------
                For iOrder = 1 To .CollDEOrders.Count
                    Const sHead As String = "BranchOrderNumber = {0}, OrderSuffix = {1}" & _
                                            ", CustomerPO = {2}, OrderActionCode = {3}" & _
                                            ", NewCarrierCode = {4}, NewCustomerPO = {5}" & _
                                            ", NewEndUserPO = {6}, NewBillToSuffix = {7}" & _
                                            ", NewShipToSuffix = {8}"
                    Const sADDR As String = "Category = {0}" & _
                                            ", Contact = {1}, Line1 = {2}" & _
                                            ", Line2 = {3}, Line3 = {4}" & _
                                            ", City = {5}, Province = {6}" & _
                                            ", PostalCode = {7}"
                    '----------------------------------------------------------------------
                    deos = Dep.CollDEOrders.Item(iOrder)
                    deoh = deos.DEOrderHeader
                    '
                    With deoh
                        ParmORDER(iOrder) = .CustomerPO
                        ParmHEAD(iOrder) = String.Format(sHead, _
                                                .BranchOrderNumber, _
                                                .OrderSuffix, _
                                                .CustomerPO, _
                                                .OrderActionCode, _
                                                .CarrierCode, _
                                                .NewCustomerPO, _
                                                .EndUserPO, _
                                                .BillToSuffix, _
                                                .ShipToSuffix)
                        '---------------------------------------------------------------------
                        For iAddress = 1 To .CollDEAddress.Count
                            dead = .CollDEAddress.Item(iAddress)
                            With dead
                                ParmADDR(iOrder, iAddress) = String.Format(sADDR, _
                                               .Category, _
                                               .ContactName, _
                                               .Line1, _
                                               .Line2, _
                                               .Line3, _
                                               .City, _
                                               .Province, _
                                               .PostalCode)
                            End With
                        Next iAddress
                    End With
                    '--------------------------------------------------------------------------
                    Const sAdd As String = "SKU ={0} , Quantity = {1}" & _
                                                 ", CustomerLineNumber = {2}" & _
                                                 ", ShipFromWarehouse = {3}"
                    Const sChange As String = "WestCoastLineNumber ={0}" & _
                                                 ", SKU ={1}" & _
                                                 ", Quantity = {2}" & _
                                                 ", CustomerLineNumber = {3}" & _
                                                 ", Suffix = {4}"
                    Const sDelete As String = "WestCoastLineNumber ={0}" & _
                                                ", Suffix = {1}"
                    Const sCOMM As String = "  CommentText = {0}" & _
                                               ", CustomerLineNumber = {1}" & _
                                               ", Suffix = {2}"
                    '---------------------------------------------------------------------------------
                    deoi = deos.DEOrderInfo
                    With deoi
                        For iItems = 1 To .CollDEProductLines.Count
                            depr = .CollDEProductLines.Item(iItems)
                            With depr
                                Select Case .Category
                                    Case Is = "AddLine"
                                        ParmADD(iOrder, iItems) = String.Format(sAdd, _
                                                                 .SKU, _
                                                                 .Quantity, _
                                                                 .CustomerLineNumber, _
                                                                 .ShipFromWarehouse)
                                    Case Is = "ChangeLine"
                                        ParmCHANGE(iOrder, iItems) = String.Format(sChange, _
                                                                 .WestCoastLineNumber, _
                                                                 .SKU, _
                                                                 .Quantity, _
                                                                 .CustomerLineNumber, _
                                                                 .Suffix)
                                    Case Is = "DeleteLine"
                                        ParmDELETE(iOrder, iItems) = String.Format(sDelete, _
                                                                  .WestCoastLineNumber, _
                                                                  .Suffix)
                                End Select
                            End With
                        Next
                        '-------------------------------------------------------------
                        For iComments = 1 To .CollDECommentLines.Count
                            decl = .CollDECommentLines.Item(iComments)
                            With decl
                                ParmCOMM2(iOrder, iComments) = String.Format( _
                                                sCOMM, _
                                                .CommentText, _
                                                .CustomerLineNumber, _
                                                .Suffix)

                            End With
                        Next iComments
                    End With
                    '-------------------------------------------------------------
                Next iOrder
            End With
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = ModuleName & " Error"
                .ErrorNumber = "TACDBOC-99"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Sub AddChangeOrderColumns()
        With DtChangeShip.Columns
            .Add("OrderNo", GetType(String))
            .Add("CustomerPO", GetType(String))
            .Add("ErrorMessage", GetType(String))
            .Add("ErrorCode", GetType(String))
        End With
        With DtAddLines.Columns
            .Add("OrderNo", GetType(String))
            .Add("ErrorMessage", GetType(String))
            .Add("ErrorCode", GetType(String))
            .Add("SKU", GetType(String))
            .Add("Quantity", GetType(String))
            .Add("Price", GetType(String))
            .Add("LineNo", GetType(String))
        End With
        With DtChangeLines.Columns
            .Add("OrderNo", GetType(String))
            .Add("ErrorMessage", GetType(String))
            .Add("ErrorCode", GetType(String))
            .Add("SKU", GetType(String))
            .Add("Quantity", GetType(String))
            .Add("Price", GetType(String))
            .Add("LineNo", GetType(String))
            .Add("OldLineNo", GetType(String))
        End With
        With DtDeleteLines.Columns
            .Add("OrderNo", GetType(String))
            .Add("ErrorMessage", GetType(String))
            .Add("ErrorCode", GetType(String))
            .Add("LineNo", GetType(String))
            .Add("CancelCode", GetType(String))
        End With
        With DtAddComment.Columns
            .Add("OrderNo", GetType(String))
            .Add("ErrorMessage", GetType(String))
            .Add("ErrorCode", GetType(String))
            .Add("Comment", GetType(String))
        End With
        With DtHeaderErrors.Columns
            .Add("OrderNo", GetType(String))
            .Add("CustomerPO", GetType(String))
            .Add("ErrorMessage", GetType(String))
            .Add("ErrorCode", GetType(String))
            .Add("Comment", GetType(String))
        End With
    End Sub

    Public Property Dep() As DEOrder
        Get
            Return _dep
        End Get
        Set(ByVal value As DEOrder)
            _dep = value
        End Set
    End Property
    '
    Private Property ParmTRAN() As String
        Get
            Return _parmTRAN
        End Get
        Set(ByVal value As String)
            _parmTRAN = value
        End Set
    End Property
    Private Property ParmORDER(ByVal order As Integer) As String
        Get
            Return _parmORDER(order)
        End Get
        Set(ByVal value As String)
            _parmORDER(order) = value
        End Set
    End Property
    Private Property ParmCUSTOMERPO(ByVal order As Integer) As String
        Get
            Return _parmCUSTOMERPO(order)
        End Get
        Set(ByVal value As String)
            _parmCUSTOMERPO(order) = value
        End Set
    End Property
    Private Property ParmHEAD(ByVal order As Integer) As String
        Get
            Return _parmHEAD(order)
        End Get
        Set(ByVal value As String)
            _parmHEAD(order) = value
        End Set
    End Property
    Private Property ParmADDR(ByVal order As Integer, ByVal index As Integer) As String
        Get
            Return _parmADDR(order, index)
        End Get
        Set(ByVal value As String)
            _parmADDR(order, index) = value
        End Set
    End Property
    Private Property ParmADD(ByVal order As Integer, ByVal index As Integer) As String
        Get
            Return _parmADD(order, index)
        End Get
        Set(ByVal value As String)
            _parmADD(order, index) = value
        End Set
    End Property
    Private Property ParmCHANGE(ByVal order As Integer, ByVal index As Integer) As String
        Get
            Return _parmCHANGE(order, index)
        End Get
        Set(ByVal value As String)
            _parmCHANGE(order, index) = value
        End Set
    End Property
    Private Property ParmDELETE(ByVal order As Integer, ByVal index As Integer) As String
        Get
            Return _parmDELETE(order, index)
        End Get
        Set(ByVal value As String)
            _parmDELETE(order, index) = value
        End Set
    End Property
    Private Property ParmCOMM1(ByVal order As Integer, ByVal index As Integer) As String
        Get
            Return _parmCOMM1(order, index)
        End Get
        Set(ByVal value As String)
            _parmCOMM1(order, index) = value
        End Set
    End Property
    Private Property ParmCOMM2(ByVal order As Integer, ByVal index As Integer) As String
        Get
            Return _parmCOMM2(order, index)
        End Get
        Set(ByVal value As String)
            _parmCOMM2(order, index) = value
        End Set
    End Property
    Private Property ParmCHANGESHIP(ByVal order As Integer) As String
        Get
            Return _parmCHANGESHIP(order)
        End Get
        Set(ByVal value As String)
            _parmCHANGESHIP(order) = value
        End Set
    End Property

    Public Property DtChangeShip() As DataTable
        Get
            Return _dtChangeShip
        End Get
        Set(ByVal value As DataTable)
            _dtChangeShip = value
        End Set
    End Property
    Public Property DtAddLines() As DataTable
        Get
            Return _dtAddLines
        End Get
        Set(ByVal value As DataTable)
            _dtAddLines = value
        End Set
    End Property
    Public Property DtChangeLines() As DataTable
        Get
            Return _dtChangeLines
        End Get
        Set(ByVal value As DataTable)
            _dtChangeLines = value
        End Set
    End Property
    Public Property DtDeleteLines() As DataTable
        Get
            Return _dtDeleteLines
        End Get
        Set(ByVal value As DataTable)
            _dtDeleteLines = value
        End Set
    End Property
    Public Property DtAddComment() As DataTable
        Get
            Return _dtAddComment
        End Get
        Set(ByVal value As DataTable)
            _dtAddComment = value
        End Set
    End Property
    Public Property DtHeaderErrors() As DataTable
        Get
            Return _dtHeaderErrors
        End Get
        Set(ByVal value As DataTable)
            _dtHeaderErrors = value
        End Set
    End Property

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj

        Dim err As New ErrorObj

        

        Select Case _settings.ModuleName
            Case Is = AmendTicketingOrder : err = AccessDatabaseWS141R()

        End Select

        Return err

    End Function

#Region "AmendTicketingItems"

    Protected Function AccessDatabaseWS141R() As ErrorObj

        Dim err As New ErrorObj
        Dim PARAMOUT As String = String.Empty
        Dim dRow As DataRow = Nothing
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try

            'Call WS141R  - Call for as many items as you have
            PARAMOUT = CallWS141R()

            If PARAMOUT.Length >= 6000 Then

                dRow = Nothing
                dRow = DtStatusResults.NewRow
                If PARAMOUT.Substring(5999, 1) = "E" Or PARAMOUT.Substring(5997, 2).Trim <> "" Then
                    dRow("ErrorOccurred") = "E"
                    dRow("ReturnCode") = PARAMOUT.Substring(5997, 2)
                Else
                    dRow("ErrorOccurred") = ""
                    dRow("ReturnCode") = ""
                End If
                DtStatusResults.Rows.Add(dRow)

            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDABAS-52"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS141R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS141R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 6000)
        parmIO.Value = WS141Parm()
        parmIO.Direction = ParameterDirection.InputOutput


        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        Return PARAMOUT

    End Function

    Private Function WS141Parm() As String

        Dim myString As New StringBuilder
        Dim Seat As New DESeatDetails

        For i As Integer = 0 To 49

            If i <= DeAmendTicketingOrder.BasketItem.Count - 1 Then
                Seat.FormattedSeat = DeAmendTicketingOrder.BasketItem(i).Seat
                myString.Append(Utilities.FixStringLength(DeAmendTicketingOrder.BasketItem(i).ProductCode, 6))
                myString.Append(Utilities.FixStringLength(Seat.Stand, 3))
                myString.Append(Utilities.FixStringLength(Seat.Area, 4))
                myString.Append(Utilities.FixStringLength(Seat.Row, 4))
                myString.Append(Utilities.FixStringLength(Seat.Seat, 4))
                myString.Append(Utilities.FixStringLength(Seat.AlphaSuffix, 1))
                myString.Append(Utilities.FixStringLength(DeAmendTicketingOrder.BasketItem(i).AllocatedMemberNo, 12))
                myString.Append(Utilities.FixStringLength("", 66))
            Else
                myString.Append(Utilities.FixStringLength("", 100))
            End If

        Next

        myString.Append(Utilities.FixStringLength("", 969))
        myString.Append(Utilities.PadLeadingZeros(DeAmendTicketingOrder.PaymentReference, 15))
        myString.Append(Utilities.FixStringLength(DeAmendTicketingOrder.CustomerNo, 12))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        myString.Append(Utilities.FixStringLength("", 3))


        Return myString.ToString

    End Function

#End Region
End Class
