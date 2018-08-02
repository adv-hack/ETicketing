Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Dispatch Notes
'
'       Date                        7th Nov 2006
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDBDA- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'       06/08/07    /001    Ben     Return dispatches for all del seqs
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBDispatchAdvice
    Inherits DBAccess

    Private _dep As DEOrder
    Private _dtHeader As New DataTable("DtHeader")
    Private _dtDetails As New DataTable("DtDetail")
    Private _dtComments As New DataTable("DtComment")
    Private _dtProduct As New DataTable("DtProduct")

    Private _parmTRAN As String

    Public Property Dep() As DEOrder
        Get
            Return _dep
        End Get
        Set(ByVal value As DEOrder)
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
    Public Property dtProduct() As DataTable
        Get
            Return _dtProduct
        End Get
        Set(ByVal value As DataTable)
            _dtProduct = value
        End Set
    End Property

    Protected Overrides Function AccessDataBaseSystem21() As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------
        '   Create command object
        '
        Dim cmdSELECT As iDB2Command = Nothing
        Dim parmInput, Paramoutput As iDB2Parameter
        Dim PARAMOUT As String = String.Empty
        ' Const strHEADER As String = "CALL WESTCOAST/DSPADVCE(@PARAM1, @PARAM2)"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                  "/DSPADVCE(@PARAM1, @PARAM2)"
        Try
            cmdSELECT = New iDB2Command(strHEADER, conSystem21)

            parmInput = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
            parmInput.Value = Utilities.FixStringLength(Settings.AccountNo3, 2) & _
                                Utilities.FixStringLength(Settings.AccountNo1, 10)
            '  /001 Set del seq as blank for *ALL   
            '   Utilities.FixStringLength(Settings.AccountNo2, 3)
            parmInput.Direction = ParameterDirection.Input
            Paramoutput = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
            Paramoutput.Value = String.Empty
            Paramoutput.Direction = ParameterDirection.InputOutput
            cmdSELECT.ExecuteNonQuery()
            PARAMOUT = cmdSELECT.Parameters(Param2).Value.ToString
        Catch ex As Exception
            Const strError8 As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBDA-02"
                .HasError = True
                Return err
            End With
        End Try
        '----------------------------------------------------------
        ' Read the dispatches back out 
        '
        If Not err.HasError Then
            err = ReadDispatch()
            '----------------------------------------------------------------------
            ' Update the dispatches as processed 
            '
            If Not err.HasError Then _
                err = UpdateDispatch()
        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Public Function ReadDispatch(Optional ByVal opendb As Boolean = False) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------
        Select Case Settings.DestinationDatabase
            Case Is = SYSTEM21
                If opendb Then err = System21Open()
                If Not err.HasError Then err = ReadDispatchSystem21()
                If opendb Then System21Close()
            Case Is = SQL2005
        End Select
        Return err
    End Function
    Public Function UpdateDispatch(Optional ByVal opendb As Boolean = False) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------
        Select Case Settings.DestinationDatabase
            Case Is = SYSTEM21
                If opendb Then err = System21Open()
                If Not err.HasError Then err = UpdateDispatchSystem21()
                If opendb Then err = System21Close()
            Case Is = SQL2005
        End Select
        Return err
    End Function

    Private Function DataEntityUnPack() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------------
        '   put the parameter generation in its own function as it is likely to be used  
        '   several times
        '
        Dim detr As New DETransaction           ' Items
        Try
            detr = Dep.CollDETrans.Item(1)
            Const sTran As String = "SenderID = {0}, ReceiverID = {1}, CountryCode = {2}, TransactionID = {3}, ShowDetail = {4}"
            With detr
                ParmTRAN = String.Format(sTran, _
                                            .SenderID, _
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
                .ErrorNumber = "TACDBDA-13"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Private Function ReadDispatchSystem21() As ErrorObj
        Dim err As New ErrorObj

        If dtHeader.Columns.Count = 0 Then
            AddColumnsToDataTables()
        End If
        '-------------------------------------------------------------------------------
        ResultDataSet = New DataSet
        ResultDataSet.Tables.Add(dtHeader)
        ResultDataSet.Tables.Add(dtDetails)
        ResultDataSet.Tables.Add(dtComments)
        ResultDataSet.Tables.Add(dtProduct)

        Try
            Dim cmdSelectHeader As iDB2Command = Nothing
            Dim dtrReaderHeader As iDB2DataReader = Nothing
            Dim dRow As DataRow = Nothing
            Dim OrderNumber As String = String.Empty
            Dim BackOfficeOrderNumber As String = String.Empty
            Dim DispatchSequence As Decimal = 0
            '--------------------------------------------------------------------------------------
            ' /001 Return for all del seqs
            Const sqlSelectHeader As String = "SELECT * FROM XXCOP500 WHERE C5CUS = @PARAM1 AND C5PRC = 'N'"
            ' Const sqlSelectHeader As String = "SELECT * FROM XXCOP500 WHERE C5CUS = @PARAM1 AND C5SHP = @PARAM2 AND C5PRC = 'N'"
            cmdSelectHeader = New iDB2Command(sqlSelectHeader, conSystem21)

            ' Only read dispatches for partner
            cmdSelectHeader.Parameters.Add(Param1, iDB2DbType.iDB2VarChar, 10).Value = Settings.AccountNo1
            '   cmdSelectHeader.Parameters.Add(Param2, iDB2DbType.iDB2VarChar, 10).Value = Settings.AccountNo2

            dtrReaderHeader = cmdSelectHeader.ExecuteReader()
            With dtrReaderHeader
                If Not .HasRows Then
                    Const strError11 As String = "No data to display"
                    With err
                        .ErrorMessage = String.Empty
                        .ErrorNumber = "TACDBDA-15"
                        .ErrorStatus = strError11
                        .HasError = True
                    End With
                End If
                While dtrReaderHeader.Read
                    dRow = Nothing
                    dRow = dtHeader.NewRow()

                    OrderNumber = .GetString(.GetOrdinal("C5ORD"))
                    dRow("OrderNumber") = OrderNumber
                    dRow("LineNumber") = .Item("C5LIN")
                    BackOfficeOrderNumber = .GetString(.GetOrdinal("C5BOR")).Trim
                    DispatchSequence = .Item("C5SEQ")
                    dRow("BackOfficeOrderNumber") = BackOfficeOrderNumber
                    dRow("CustomerNumber") = .GetString(.GetOrdinal("C5CUS"))
                    dRow("CustomerShipTo") = .GetString(.GetOrdinal("C5SHP"))
                    dRow("DispatchNoteSequence") = .Item("C5SEQ")
                    '
                    Dim dt As String = Utilities.ISeriesDate(.GetString(.GetOrdinal("C5SDT")))
                    dRow("DispatchDate") = Date.Parse(dt)
                    dRow("TransportMode") = .GetString(.GetOrdinal("C5TRA"))
                    dRow("DispatchProcessed") = .GetString(.GetOrdinal("C5PRC"))
                    dRow("AccountNumber") = Settings.AccountNo3.Trim
                    '----------------------------------------------------------------------------------
                    Dim cmdSelectXXCOP100 As iDB2Command = Nothing
                    Dim dtrReaderXXCOP100 As iDB2DataReader = Nothing
                    Const sqlSelectXXCOP100 As String = "SELECT * FROM XXCOP100 WHERE C1ORD = @PARAM1"
                    cmdSelectXXCOP100 = New iDB2Command(sqlSelectXXCOP100, conSystem21)
                    cmdSelectXXCOP100.Parameters.Add(Param1, iDB2DbType.iDB2VarChar, 10).Value = OrderNumber
                    dtrReaderXXCOP100 = cmdSelectXXCOP100.ExecuteReader()
                    ' 
                    With dtrReaderXXCOP100
                        If .HasRows Then
                            .Read()
                            dRow("CustomerPO") = .GetString(.GetOrdinal("C1PON"))
                           
                            dRow("CustomerName") = .GetString(.GetOrdinal("C1NAM"))
                            dRow("CustomerAttention") = .GetString(.GetOrdinal("C1NAM"))
                            dRow("CustomerAddressLine1") = .GetString(.GetOrdinal("C1AD1"))
                            dRow("CustomerAddressLine2") = .GetString(.GetOrdinal("C1AD2"))
                            dRow("CustomerAddressLine3") = .GetString(.GetOrdinal("C1AD3"))
                            dRow("CustomerAddressLine4") = .GetString(.GetOrdinal("C1AD4"))
                            dRow("CustomerCity") = .GetString(.GetOrdinal("C1AD5"))
                            dRow("CustomerPostalCode") = .GetString(.GetOrdinal("C1PCD"))
                            dRow("CustomerCountryCode") = .GetString(.GetOrdinal("C1CCD"))
                            dRow("CustomerShipToSuffix") = .GetString(.GetOrdinal("C1SHP"))
                        End If
                        .Close()
                    End With
                    '----------------------------------------------------------------------------------
                    Dim cmdSelectXXCOP200 As iDB2Command = Nothing
                    Dim dtrReaderXXCOP200 As iDB2DataReader = Nothing
                    Const sqlSelectXXCOP200 As String = "SELECT * FROM XXCOP200 WHERE C2ORD = @PARAM1"
                    cmdSelectXXCOP200 = New iDB2Command(sqlSelectXXCOP200, conSystem21)
                    cmdSelectXXCOP200.Parameters.Add(Param1, iDB2DbType.iDB2VarChar, 10).Value = OrderNumber
                    dtrReaderXXCOP200 = cmdSelectXXCOP200.ExecuteReader()
                    ' -
                    With dtrReaderXXCOP200
                        If .HasRows Then
                            .Read()
                            dRow("ShipToName") = .GetString(.GetOrdinal("C2NAM"))
                            dRow("ShipToAttention") = .GetString(.GetOrdinal("C2NAM"))
                            dRow("ShipToAddressLine1") = .GetString(.GetOrdinal("C2AD1"))
                            dRow("ShipToAddressLine2") = .GetString(.GetOrdinal("C2AD2"))
                            dRow("ShipToAddressLine3") = .GetString(.GetOrdinal("C2AD3"))
                            dRow("ShipToAddressLine4") = .GetString(.GetOrdinal("C2AD4"))
                            dRow("ShipToCity") = .GetString(.GetOrdinal("C2AD5"))
                            dRow("ShipToPostalCode") = .GetString(.GetOrdinal("C2PCD"))
                            dRow("ShipToCountryCode") = .GetString(.GetOrdinal("C2CCD"))
                        End If
                        .Close()
                    End With
                    '----------------------------------------------------------------------------------
                    Dim cmdSelectOEP05 As iDB2Command = Nothing
                    Dim dtrReaderOEP05 As iDB2DataReader = Nothing
                    Const sqlSelectOEP05 As String = "SELECT * FROM COMPFILE WHERE CONO05 = @PARAM1"
                    cmdSelectOEP05 = New iDB2Command(sqlSelectOEP05, conSystem21)
                    cmdSelectOEP05.Parameters.Add(Param1, iDB2DbType.iDB2VarChar, 2).Value = Settings.AccountNo3
                    dtrReaderOEP05 = cmdSelectOEP05.ExecuteReader()
                    ' 
                    With dtrReaderOEP05
                        If .HasRows Then
                            .Read()
                           
                            dRow("ShipFromName") = .GetString(.GetOrdinal("CNAM05"))
                            dRow("ShipFromAttention") = .GetString(.GetOrdinal("CNAM05"))
                            dRow("ShipFromAddressLine1") = .GetString(.GetOrdinal("CAD105"))
                            dRow("ShipFromAddressLine2") = .GetString(.GetOrdinal("CAD205"))
                            dRow("ShipFromAddressLine3") = .GetString(.GetOrdinal("CAD305"))
                            dRow("ShipFromAddressLine4") = .GetString(.GetOrdinal("CAD405"))
                            dRow("ShipFromCity") = .GetString(.GetOrdinal("CAD505"))
                            dRow("ShipFromPostalCode") = .GetString(.GetOrdinal("CPST05"))
                            dRow("ShipFromCountryCode") = .GetString(.GetOrdinal("CAD505"))
                        End If
                        .Close()
                    End With
                    '----------------------------------------
                    ' Find No Of Packages
                    Dim cmdSelectDEAA As iDB2Command = Nothing
                    Dim dtrReaderDEAA As iDB2DataReader = Nothing
                    Const sqlSelectDEAA As String = "SELECT * FROM DEAA WHERE CONOAA = @PARAM1 AND ORDNAA = @PARAM2 AND DESNAA = @PARAM3"
                    Try
                        cmdSelectDEAA = New iDB2Command(sqlSelectDEAA, conSystem21)
                        cmdSelectDEAA.Parameters.Add(Param1, iDB2DbType.iDB2VarChar, 2).Value = Settings.AccountNo3
                        cmdSelectDEAA.Parameters.Add(Param2, iDB2DbType.iDB2VarChar, 7).Value = BackOfficeOrderNumber.Trim
                        cmdSelectDEAA.Parameters.Add(Param3, iDB2DbType.iDB2Integer, 2).Value = CInt(dRow("DispatchNoteSequence"))
                        dtrReaderDEAA = cmdSelectDEAA.ExecuteReader()
                        ' 
                        With dtrReaderDEAA
                            If .HasRows Then
                                .Read()
                                dRow("NumberOfPackages") = .Item("NPARAA").ToString
                            End If
                        End With
                    Catch ex As Exception
                        dRow("NumberOfPackages") = String.Empty
                    Finally
                        Try
                            dtrReaderDEAA.Close()
                        Catch ex As Exception
                        End Try
                    End Try

                    '----------------------------------------------------------------------------------
                    ' Add to Header Table
                    ' 
                    dtHeader.Rows.Add(dRow)
                    'err = GetDetailsetail(BackOfficeOrderNumber)
                    err = GetDetailsetail(BackOfficeOrderNumber, DispatchSequence)
                    err = ReadSerialNumbersSystem21(OrderNumber.Trim)
                End While
                .Close()
            End With
        Catch ex As Exception
        End Try
        '----------------------------------------------------------------------------------
        Return err
    End Function
    Private Function UpdateDispatchSystem21() As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------
        Dim libl As String = conSystem21.LibraryList
        Dim cmdSelect As iDB2Command = Nothing

        Const sqlSelect As String = "UPDATE XXCOP500 SET C5PRC = 'Y' WHERE C5CUS = @PARAM1 AND C5PRC = 'N'"
        'Const sqlSelect As String = "UPDATE XXCOP500 SET C5PRC = 'Y' WHERE C5CUS = @PARAM1 AND C5SHP = @PARAM2 AND C5PRC = 'N'"

        Try
            cmdSelect = New iDB2Command(sqlSelect, conSystem21)
            ' Only read dispatches for partner
            cmdSelect.Parameters.Add(Param1, iDB2DbType.iDB2VarChar, 10).Value = Settings.AccountNo1
            ' /001 - Update for all del seqs
            '  cmdSelect.Parameters.Add(Param2, iDB2DbType.iDB2VarChar, 10).Value = Settings.AccountNo2
            cmdSelect.ExecuteNonQuery()

        Catch ex As Exception
        End Try
        '----------------------------------------------------------------------------------
        Return err
    End Function
    Private Sub AddColumnsToDataTables()
        With dtHeader.Columns
            .Add("OrderNumber", GetType(String))
            .Add("LineNumber", GetType(String))
            .Add("BackOfficeOrderNumber", GetType(String))
            .Add("CustomerNumber", GetType(String))
            .Add("CustomerShipTo", GetType(String))
            .Add("DispatchNoteSequence", GetType(String))

            .Add("CustomerPO", GetType(String))
            .Add("AccountNumber", GetType(String))
            '
            .Add("DocumentDate", GetType(Date))
            .Add("DispatchDate", GetType(Date))
            .Add("DeliveryDate", GetType(Date))
            '
            .Add("CustomerName", GetType(String))
            .Add("CustomerAttention", GetType(String))
            .Add("CustomerAddressLine1", GetType(String))
            .Add("CustomerAddressLine2", GetType(String))
            .Add("CustomerAddressLine3", GetType(String))
            .Add("CustomerAddressLine4", GetType(String))
            .Add("CustomerCity", GetType(String))
            .Add("CustomerProvince", GetType(String))
            .Add("CustomerPostalCode", GetType(String))
            .Add("CustomerCountryCode", GetType(String))
            '
            .Add("ShipToName", GetType(String))
            .Add("ShipToAttention", GetType(String))
            .Add("ShipToAddressLine1", GetType(String))
            .Add("ShipToAddressLine2", GetType(String))
            .Add("ShipToAddressLine3", GetType(String))
            .Add("ShipToAddressLine4", GetType(String))
            .Add("ShipToCity", GetType(String))
            .Add("ShipToProvince", GetType(String))
            .Add("ShipToPostalCode", GetType(String))
            .Add("ShipToCountryCode", GetType(String))
            .Add("CustomerShipToSuffix", GetType(String))
            '
            .Add("ShipFromName", GetType(String))
            .Add("ShipFromAttention", GetType(String))
            .Add("ShipFromAddressLine1", GetType(String))
            .Add("ShipFromAddressLine2", GetType(String))
            .Add("ShipFromAddressLine3", GetType(String))
            .Add("ShipFromAddressLine4", GetType(String))
            .Add("ShipFromCity", GetType(String))
            .Add("ShipFromProvince", GetType(String))
            .Add("ShipFromPostalCode", GetType(String))
            .Add("ShipFromCountryCode", GetType(String))
            '
            .Add("SequenceNumber", GetType(String))
            .Add("NumberOfPackages", GetType(String))
            '
            .Add("UnitOfMeasure", GetType(String))
            .Add("Weight", GetType(String))
            '
            .Add("TrackingNumber", GetType(String))
            .Add("TransportMode", GetType(String))
            .Add("DispatchProcessed", GetType(String))
            '
        End With
        '----------------------------------------------------------------------------------
        With dtDetails.Columns
            .Add("OrderNumber", GetType(String))
            .Add("LineNumber", GetType(Integer))
            .Add("ProductCode", GetType(String))
            .Add("DispatchQuantity", GetType(Integer))
            .Add("EANCode", GetType(String))
            .Add("CustomerSKU", GetType(String))
            .Add("Description1", GetType(String))
            .Add("Description2", GetType(String))
            '
            .Add("itemDispatchSequence", GetType(String))
            .Add("processedIndicator", GetType(String))
        End With
        '----------------------------------------------------------------------------------
        With dtComments.Columns
            .Add("OrderNumber", GetType(String))
            .Add("LineNumber", GetType(Integer))
            .Add("SerialNumber", GetType(Integer))
        End With
        With dtProduct.Columns                              ' Sku Serial Number
            .Add("OrderNumber", GetType(String))
            .Add("OrderLine", GetType(String))
            .Add("PackageID", GetType(String))
            .Add("SKU", GetType(String))
            .Add("SerialNumber", GetType(String))
        End With
    End Sub

    'Private Function GetDetailsetail(ByVal backOfficeOrderNumber As String) As ErrorObj
    Private Function GetDetailsetail(ByVal backOfficeOrderNumber As String, ByVal dispatchSequence As Decimal) As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------------
        '   Read Order Detail Lines OEP55
        ' 
        Dim cmdSelect As iDB2Command = Nothing
        Dim dtrReader As iDB2DataReader = Nothing
        Dim dRow As DataRow = Nothing
        Dim param1 As String = "@PARAM1"
        Try
            '----------------------------------------------------------------------------------
            ' Read items
            '
            'Const sqlSelectItem As String = "SELECT * FROM XXCOP600 WHERE C6ORD = @PARAM1"
            Const sqlSelectItem As String = "SELECT * FROM XXCOP600 WHERE C6ORD = @PARAM1 AND C6SQN = @PARAM2"
            cmdSelect = New iDB2Command(sqlSelectItem, conSystem21)
            cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2VarChar, 20).Value = backOfficeOrderNumber
            cmdSelect.Parameters.Add("@PARAM2", iDB2DbType.iDB2Decimal).Value = dispatchSequence
            dtrReader = cmdSelect.ExecuteReader()
            '----------------------------------------------------------------------------------
            With dtrReader
                While .Read
                    dRow = Nothing
                    dRow = dtDetails.NewRow()
                    dRow("OrderNumber") = .GetString(.GetOrdinal("C6ORD")).Trim
                    dRow("LineNumber") = .Item("C6LIN")
                    dRow("ProductCode") = .GetString(.GetOrdinal("C6PCD")).Trim
                    dRow("DispatchQuantity") = .Item("C6QTY")
                    dRow("EANCode") = .GetString(.GetOrdinal("C6EAN")).Trim

                    ' Surely this isn't the right field for Customer SKU
                    dRow("CustomerSKU") = String.Empty
                    Try
                        dRow("CustomerSKU") = .GetString(.GetOrdinal("C6CSU")).Trim
                    Catch ex As Exception

                    End Try
                    'dRow("CustomerSKU") = .Item("C6NUM").ToString

                    dRow("Description1") = .GetString(.GetOrdinal("C6DSC")).Trim
                    dRow("Description2") = .GetString(.GetOrdinal("C6DS1")).Trim
                    dRow("itemDispatchSequence") = .Item("C6SQN").ToString
                    dRow("processedIndicator") = .GetString(.GetOrdinal("C6PCD")).Trim
                    dtDetails.Rows.Add(dRow)
                End While
                .Close()
            End With
        Catch ex As Exception
            Const strError As String = "Failed to Read Detail Lines "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBDA-75"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Private Function ReadSerialNumbersSystem21(ByVal orderNumber As String) As ErrorObj
        Dim err As New ErrorObj

        Dim cmdSelect As iDB2Command = Nothing
        Dim dtrReader As iDB2DataReader = Nothing
        Dim dRow As DataRow = Nothing
        Dim param1 As String = "@PARAM1"
        Dim param2 As String = "@PARAM2"
        Dim fieldOrdinal(7) As String

        Try
            Const sqlSelect5 As String = "SELECT * FROM DEAM WHERE " & _
                                         " CONOAM = @PARAM1 AND " & _
                                         " ORDNAM = @PARAM2  "

            cmdSelect = New iDB2Command(sqlSelect5, conSystem21)
            Select Case Settings.DatabaseType1
                Case Is = T65535
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = Settings.AccountNo3
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 7).Value = orderNumber
                Case Is = "285"
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = Settings.AccountNo3
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = orderNumber
                Case Else
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = Settings.AccountNo3
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = orderNumber
            End Select
            '-----------------------------------------------------------------------------
            dtrReader = cmdSelect.ExecuteReader()
            If dtrReader.HasRows Then
                With dtrReader

                    While dtrReader.Read

                        dRow = Nothing
                        dRow = dtProduct.NewRow()
                        dRow("OrderNumber") = orderNumber.Trim
                        dRow("OrderLine") = .GetString(.GetOrdinal("ORDLAM")).Trim
                        dRow("PackageID") = .GetString(.GetOrdinal("DESNAM")).Trim
                        dRow("SKU") = .GetString(.GetOrdinal("ITEMAM")).Trim
                        dRow("SerialNumber") = .GetString(.GetOrdinal("SENOAM")).Trim
                        dtProduct.Rows.Add(dRow)
                    End While
                End With

            End If

            '-------------------------------------------------------------------------

        Catch ex As Exception
            ' Don't return an error..

            'Const strError As String = "Failed to Read Serial Numbers "
            'With err
            '    .ErrorMessage = ex.Message
            '    .ErrorStatus = strError
            '    .ErrorNumber = "TACDBOR79"
            '    .HasError = True
            'End With
        End Try
        Return err
    End Function

End Class
