Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text
Imports Talent.Common.Utilities
'--------------------------------------------------------------------------------------------------

'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Price Requests
'
'       Date                        Nov 2006
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDBPR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBPricing
    Inherits DBAccess

#Region "Class Level Fields"
    Private _dep As New DePNA
    Private _depnarequest As New DEPNARequest
    ' Allow a million items (complete PNA request may need them)
    Private _parmITEM(1000000) As String
    Private _de As New DEProductDetails
    Private Const PRICECODEDETAILS As String = "PriceCodeDetails"
    Private _dicPriceCodeDetails As Generic.Dictionary(Of String, DEPriceCode) = Nothing
#End Region

#Region "Inner Class"

    Private Class savedPrice
        Public price As Decimal
        Public currencyCode As String

        Public Sub New(ByVal price As Decimal, _
                       ByVal currencyCode As String)
            Me.price = price
            Me.currencyCode = currencyCode

        End Sub
    End Class
#End Region

#Region "Properties"

    Public ReadOnly Property PriceCodeDesc() As Generic.Dictionary(Of String, DEPriceCode)
        Get
            Return _dicPriceCodeDetails
        End Get
    End Property

    Public Property De() As DEProductDetails
        Get
            Return _de
        End Get
        Set(ByVal value As DEProductDetails)
            _de = value
        End Set
    End Property

    Public Property dep() As DePNA
        Get
            Return _dep
        End Get
        Set(ByVal value As DePNA)
            _dep = value
        End Set
    End Property
    Public Property Depnarequest() As DEPNARequest
        Get
            Return _depnarequest
        End Get
        Set(ByVal value As DEPNARequest)
            _depnarequest = value
        End Set
    End Property

    Private Property ParmITEM(ByVal index As Integer) As String
        Get
            Return _parmITEM(index)
        End Get
        Set(ByVal value As String)
            _parmITEM(index) = value
        End Set
    End Property
#End Region

#Region "Protected Methods"

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj

        Dim err As New ErrorObj

        Select Case _settings.ModuleName
            Case Is = PRICECODEDETAILS : err = AccessDatabaseWS132R()
        End Select

        Return err

    End Function

    Protected Overrides Function ValidateAgainstDatabaseChorus() As ErrorObj
        'Dim err As New ErrorObj


        '' Build the response DataSet

        ResultDataSet = New DataSet

        'Dim dt As New DataTable("ChorusProductPrices")
        'With dt.Columns
        '    .Add("ProductCode", GetType(String))
        '    .Add("GrossPrice", GetType(Decimal))
        '    .Add("NetPrice", GetType(Decimal))
        '    .Add("TaxPrice", GetType(Decimal))
        '    .Add("Quantity", GetType(Decimal))
        'End With

        'dt.Rows.Add("123456", 100.0, 85.0, 15.0, 1)
        'ResultDataSet.Tables.Add(dt)

        'Return err


        '---------------------------------------------------------------------------
        '   AccessDataBaseSystem21: Write the order to Chorus via Stored Procedures
        '
        Dim err As New ErrorObj
        Dim sProcError As String = String.Empty
        '---------------------------------------------------------------------------
        '   Open up collection and deal with by putting to stored procedures parameters 
        '
        Dim collDates As New Collection
        collDates.Add(Date.Now)
        If Not err.HasError Then
            '------------------------------------------------------------------
            ' Setup Calls to As400
            ' 
            Dim strPRCEBSK As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                                "/PRCEBSK(@PARAM1, @PARAM2)"

            Dim cmdPRCEBSK As iDB2Command = Nothing

            Dim iItems As Integer = 0
            Dim iOrder As Integer = 0
            Dim iComments As Integer = 0

            Dim parmInput, parmOutput As iDB2Parameter
            Dim PARMOUT As String = String.Empty

            '----------------------------------------------------------------
            '   Loop through orders in transaction
            '
            Try
                '
                cmdPRCEBSK = New iDB2Command(strPRCEBSK, conChorus)

                parmInput = cmdPRCEBSK.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                parmInput.Value = ParmPRCEBSK()
                parmInput.Direction = ParameterDirection.Input

                parmOutput = cmdPRCEBSK.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                parmOutput.Value = String.Empty
                parmOutput.Direction = ParameterDirection.InputOutput
                cmdPRCEBSK.ExecuteNonQuery()

                PARMOUT = cmdPRCEBSK.Parameters(Param2).Value.ToString

                If PARMOUT.Substring(1023, 1) = "Y" Then
                    With err
                        .ItemErrorMessage(iOrder) = String.Empty
                        .ItemErrorCode(iOrder) = "TACDBPR-100"
                        .ItemErrorStatus(iOrder) = "Error retrieving Chorus Price - " & _
                                            PARMOUT.Substring(1019, 4) & "-" & _
                                            Descriptions.GetDescription(Settings.FrontEndConnectionString, _
                                            "ENG", "ERRORCODE", PARMOUT.Substring(1019, 4))
                    End With

                Else

                    Dim gross As String = PARMOUT.Substring(0, 11), _
                        net As String = PARMOUT.Substring(11, 11), _
                        tax As String = PARMOUT.Substring(22, 11)


                    gross = gross.Trim
                    If gross.Length > 2 Then
                        gross = gross.Insert(gross.Length - 2, ".")
                    Else
                        gross = "0." & Utilities.PadLeadingZeros(gross, 2)
                    End If

                    net = net.Trim
                    If net.Length > 2 Then
                        net = net.Insert(net.Length - 2, ".")
                    Else
                        net = "0." & Utilities.PadLeadingZeros(net, 2)
                    End If

                    tax = tax.Trim
                    If tax.Length > 2 Then
                        tax = tax.Insert(tax.Length - 2, ".")
                    Else
                        tax = "0." & Utilities.PadLeadingZeros(tax, 2)
                    End If


                    Dim dt As New DataTable("ChorusProductPrices")
                    With dt.Columns
                        .Add("ProductCode", GetType(String))
                        .Add("GrossPrice", GetType(Decimal))
                        .Add("NetPrice", GetType(Decimal))
                        .Add("TaxPrice", GetType(Decimal))
                        .Add("Quantity", GetType(Decimal))
                    End With

                    Dim dr As DataRow = dt.NewRow
                    Try
                        dr("ProductCode") = dep.WebProductCode
                        dr("GrossPrice") = CDec(gross)
                        dr("NetPrice") = CDec(net)
                        dr("TaxPrice") = CDec(tax)
                        dr("Quantity") = CDec(dep.Quantity.ToString("#0.00").Split(".")(0))
                    Catch ex As Exception
                    End Try

                    dt.Rows.Add(dr)
                    ResultDataSet.Tables.Add(dt)
                End If


            Catch ex As Exception
                Const strError3 As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError3
                    .ErrorNumber = "TACDBOR-53"
                    .HasError = True
                    Return err
                End With
            End Try

        End If
        Return err

    End Function

    Protected Overrides Function ValidateAgainstDatabaseSystem21() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        '   Execute
        '
        Dim cmdSelect As iDB2Command = Nothing
        Dim dtrProducts As SqlDataReader = Nothing
        Dim parmInput As iDB2Parameter
        Dim parmOutput As iDB2Parameter
        Dim parmIn As String = dep.SKU() & dep.Quantity.ToString
        Dim Outgoing As String = "Outgoing"
        '
        ' Const SQLString As String = "CALL WESTCOAST/PRICEENQ( @PARAM1, @PARAM2) "
        Dim SQLString As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                  "/PRICEENQ( @PARAM1, @PARAM2) "
        If Not err.HasError Then
            Try
                cmdSelect = New iDB2Command(SQLString, conSystem21)
                parmInput = cmdSelect.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                parmIn = String.Format("{0,-15}{1,-25}{2,-11}", dep.SKU(), dep.Quantity(), Utilities.FixStringLength(Settings.AccountNo1(), 8) & Settings.AccountNo2)
                parmInput.Value = parmIn
                parmInput.Direction = ParameterDirection.Input

                parmOutput = cmdSelect.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                parmOutput.Value = Outgoing
                parmOutput.Direction = ParameterDirection.InputOutput

                cmdSelect.ExecuteNonQuery()
                '--------------------------------------------------------------------
                Outgoing = cmdSelect.Parameters(Param2).Value.ToString
                If Outgoing.Substring(1023, 1) = "Y" Then
                    With err
                        .ErrorMessage = parmIn
                        .ErrorNumber = Outgoing.Substring(1019, 4)
                        .ErrorStatus = "Error pricing item - " & _
                                    Descriptions.GetDescription(Settings.FrontEndConnectionString, "ENG", "ERRORCODE", err.ErrorNumber)
                        .HasError = True
                    End With
                Else
                    ' Build the response DataSet
                    Dim price As Decimal = 0
                    price = CType(Outgoing.Substring(0, 15), Decimal)
                    Dim dt As New DataTable
                    dt.Columns.Add("Item")
                    dt.Columns.Add("Price")
                    dt.Rows.Add(dep.SKU, price)
                    ResultDataSet.Tables.Add(dt)
                End If
                cmdSelect.Dispose()
                '--------------------------------------------------------------------
            Catch ex As Exception
                Const strError8 As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8
                    .ErrorNumber = "TACDBPR-08"
                    .HasError = True
                End With
            End Try
        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Overrides Function ValidateAgainstDatabaseURL() As ErrorObj
        Dim err As New ErrorObj
        '    err = DataEntityUnPackURL()

        Dim dt As New DataTable
        With dt.Columns
            .Add("ProductNumber", GetType(String))
            .Add("WareHouse", GetType(String))
            .Add("QuantityRequested", GetType(Double))
            .Add("QuantityAvailable", GetType(Double))
            .Add("QuantityOnOrder", GetType(Double))
            .Add("Price", GetType(Decimal))
            .Add("AltPartNo", GetType(String))
            .Add("CurrencyCode", GetType(String))
            .Add("Description", GetType(String))
        End With
        Dim iItems As Integer = 0
        Dim dr As DataRow = Nothing
        ResultDataSet = New DataSet
        Dim talAvailDep As New DEProductAlert

        ' Definite number of products to batch up in 1 call
        Dim numberToBatch As Integer = 100
        Dim url As String = Depnarequest.PriceUrl
        url = Replace(url, "@param1", System.Web.HttpUtility.UrlEncode(Settings.AccountNo1.Trim))
        Dim callUrl As String = url
        Dim sWork As String = String.Empty
        Dim sCurrency As String = String.Empty
        Dim sOk As String = String.Empty
        Dim saWork() As String = Nothing
        Dim saResults() As String = Nothing
        Dim resultCount As Integer = 0
        Dim totalProducts As Integer = Depnarequest.CollDEAlerts.Count
        Dim totalProductCount As Integer = 0
        Dim batchproductCount As Integer = 0
        Dim totalResultCount As Integer = 0
        Dim arrProducts(Depnarequest.CollDEAlerts.Count) As DEAlerts

        Dim htPrices As New Hashtable
        '--------------------------------------------------------------------
        '   Execute
        '
        If Not err.HasError Then
            Try
                For Each product As DEAlerts In Depnarequest.CollDEAlerts
                    '------------
                    ' Build array
                    '------------
                    arrProducts(totalProductCount) = product
                    '---------------------------------------------------------------
                    ' Check we've not already got the price...if we have just add it
                    '---------------------------------------------------------------
                    ' ToDo if still slow:
                    ' - Build a hash of Product/Customer and price
                    ' - If a product is already in the current batch then build a collection
                    '   of warehouse for that product and add multiple records at the end

                    '---------------------------------------------------------------------------------------
                    ' Loop through next set of products to build URL (products seperate by ';')
                    ' function=getpricedirect&web=1&custcode=zzc000&prodcode=51645ae;51645ae#231;EU307ET#ABU
                    '---------------------------------------------------------------------------------------
                    If callUrl.EndsWith(";") OrElse callUrl.EndsWith("=") Then
                        callUrl &= System.Web.HttpUtility.UrlEncode(product.ProductCode.Trim)
                    Else
                        callUrl &= ";" & System.Web.HttpUtility.UrlEncode(product.ProductCode.Trim)
                    End If
                    batchproductCount += 1
                    totalProductCount += 1
                    If batchproductCount = numberToBatch OrElse totalProductCount = totalProducts Then
                        '------------------
                        ' Ready to call URL
                        '------------------
                        Try
                            Dim strReply As String = String.Empty
                            ' if working locally can't see url, so dummy results..
                            If Environment.MachineName = "NABF02" OrElse Environment.MachineName = "NWBF01" Then
                                strReply = "OK,SSP,zzc000,GBP" & Environment.NewLine
                                For a As Integer = 1 To batchproductCount
                                    If arrProducts(a - 1).ProductCode.Trim = "BH770058" Then
                                        strReply &= "FAIL"
                                    Else
                                        strReply &= arrProducts(a - 1).ProductCode.Trim & _
                                                                                    ",OK,CA1,0.01,18.8,19.79,19.03" & _
                                                                                    Environment.NewLine()
                                    End If

                                Next
                                ' Remove last 'New Line'
                                strReply = strReply.TrimEnd(Environment.NewLine.ToCharArray)

                            Else
                                Dim httpRequest As Net.WebRequest = Net.WebRequest.Create(callUrl)
                                Dim httpResponse As Net.WebResponse = httpRequest.GetResponse()
                                Dim stream As IO.Stream = httpResponse.GetResponseStream()
                                Dim reader As New IO.StreamReader(stream)
                                strReply = reader.ReadToEnd()
                            End If
                            '---------------------------------------------------------------------------------------
                            ' Process results in the format:
                            ' OK,001,zzc000,1,1 ;51645ae,OK,19.99,12.19,12.80,V ;51645ae#231,OK,18.14,12.79,13.43,V ;EU307ET#ABU,OK,354.00,268.60,273.97,V 
                            ' (Header)           (First Prod)                    (Second Prod).....
                            '---------------------------------------------------------------------------------------
                            'saResults = strReply.Split(";")
                            saResults = strReply.Split(Environment.NewLine())
                            ' Get currency from Header
                            Dim header() As String = saResults(0).Split(",")
                            sCurrency = header(3)

                            ' Skip first entry as it's the header
                            resultCount = 1
                            Do While resultCount < saResults.GetLength(0)
                                saWork = saResults(resultCount).Split(",")
                                ' Customer price is now 7th item
                                'sWork = saWork(4)
                                sWork = saWork(6)
                                sOk = saWork(1)
                                '------------------------------------------------------------------------
                                ' Only create row if 'OK' - Otherwise product is unavailable for customer
                                '------------------------------------------------------------------------
                                Dim resultProd As DEAlerts = arrProducts(totalResultCount)
                                If sOk = "OK" Then
                                    dr = dt.NewRow()
                                    dr("ProductNumber") = saWork(0)
                                    dr("WareHouse") = resultProd.BranchID
                                    dr("QuantityAvailable") = resultProd.AvailabilQty
                                    dr("QuantityRequested") = resultProd.Quantity
                                    If resultProd.OnOrder = "" Then
                                        dr("QuantityOnOrder") = 0
                                    Else
                                        dr("QuantityOnOrder") = CType(resultProd.OnOrder, Double)
                                    End If
                                    dr("Price") = CType(sWork, Decimal)
                                    dr("AltPartNo") = resultProd.ManufacturerPartNumber
                                    dr("CurrencyCode") = sCurrency
                                    dr("Description") = resultProd.Description
                                    dt.Rows.Add(dr)
                                End If
                                resultCount += 1
                                totalResultCount += 1
                            Loop

                        Catch ex As Exception
                            'Const strError8 As String = "Error during Price URL access"
                            'With err
                            '    .ErrorMessage = ex.Message
                            '    .ErrorStatus = strError8
                            '    .ErrorNumber = "TACDBPR-14"
                            '    .HasError = True
                            'End With

                        End Try
                        '---------------------
                        ' Reset for next batch
                        '---------------------
                        batchproductCount = 0
                        callUrl = url

                    End If

                Next
                ResultDataSet.Tables.Add(dt)
                '--------------------------------------------------------------------
            Catch ex As Exception
                Const strError8 As String = "Error during Price URL access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8
                    .ErrorNumber = "TACDBPR-13"
                    .HasError = True
                End With
            End Try
        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Overrides Function ValidateAgainstDatabaseSql2005() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        Dim cmdSelect As SqlCommand = Nothing
        Dim dtrProducts As SqlDataReader = Nothing
        If Not err.HasError Then
            Try
                Const SQLString As String = "SELECT * FROM tbl_product WITH (NOLOCK)  WHERE PRODUCT_CODE = @PARAM1"
                cmdSelect = New SqlCommand(SQLString, conSql2005)
                With cmdSelect
                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char, 50))
                    .Parameters(Param1).Value = dep.SKU.Trim
                    dtrProducts = .ExecuteReader()
                End With

                If Not dtrProducts.HasRows Then
                    With err
                        .ErrorMessage = dep.SKU.Trim
                        .ErrorStatus = "Product " & dep.SKU & " not found"
                        .ErrorNumber = "TACDBPR-11"
                        .HasError = True
                    End With
                    dtrProducts.Close()
                Else
                    '   Load the Datareader into the DataSet to return
                    Dim dt As New DataTable
                    dt.Load(dtrProducts)
                    ResultDataSet.Tables.Add(dt)
                    dtrProducts.Close()
                End If
                '--------------------------------------------------------------------
            Catch ex As Exception
                Const strError8 As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8
                    .ErrorNumber = "TACDBPR-12"
                    .HasError = True
                End With
            End Try
        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Function ParmPRCEBSK() As String
        ParmPRCEBSK = Utilities.FixStringLength( _
                            Utilities.FixStringLength( _
                                Utilities.FixStringLength(dep.AccountNumber1, 6) & _
                                Utilities.FixStringLength(dep.AccountNumber2, 3) _
                            , 15) & _
                            Utilities.FixStringLength(dep.WebProductCode, 50) & _
                            Utilities.PadLeadingZeros(dep.Quantity.ToString("#0.00").Split(".")(0), 5) _
                        , 1024)
        Return ParmPRCEBSK

    End Function

#End Region

#Region "Private Methods"

    Private Function DataEntityUnPackURL() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------------
        '   put the parameter generation in its own function as it is likely to be used  
        '   several times
        '   PARAM1  = (product (30-text) & Wharehouse(10-text) ) by 20 thus 800 char long
        '
        Dim iItem As Integer = 0
        Dim iItems As Integer = 0
        Dim sItems As StringBuilder = Nothing
        Dim dea As New DEAlerts                 ' Items
        '
        Try
            '-----------------------------------------------------------------------------------------

            sItems = New StringBuilder
            For iItems = 1 To Depnarequest.CollDEAlerts.Count
                dea = Depnarequest.CollDEAlerts.Item(iItems)
                With dea
                    sItems.Append(Utilities.FixStringLength(.ProductCode, 30))
                    sItems.Append(Utilities.FixStringLength(Settings.AccountNo1, 8))
                    If iItems \ 20 = iItems / 20 Then
                        iItem += 1
                        ParmITEM(iItem) = sItems.ToString
                        sItems = New StringBuilder
                    End If
                End With
            Next iItems
            If (sItems.ToString).Length > 39 Then
                iItem += 1
                ParmITEM(iItem) = sItems.ToString
            End If
            '-----------------------------------------------------------------------------------------
        Catch ex As Exception
            Const strError17 As String = "Could not Unpack Data Entity "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError17
                err.ErrorNumber = "TACDBPR-17"
                err.HasError = True
            End With

        End Try
        Return err
    End Function

    Private Function AccessDatabaseWS132R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim moreRecords As Boolean = True
        Dim lastPriceCode As String = String.Empty
        Dim stadiumCode As String = String.Empty

        'Create the Status data table
        Dim dtStatusResults As New DataTable
        dtStatusResults.TableName = "Status"
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        _dicPriceCodeDetails = New Dictionary(Of String, DEPriceCode)

        Try
            'Loop until no more products available
            Do While moreRecords = True
                PARAMOUT = CallWS132R(lastPriceCode)
                dRow = Nothing
                dRow = dtStatusResults.NewRow
                If PARAMOUT.Substring(10239, 1) = "E" Or PARAMOUT.Substring(10237, 2).Trim <> "" Then
                    dRow("ErrorOccurred") = "E"
                    dRow("ReturnCode") = PARAMOUT.Substring(10237, 2)
                    moreRecords = False
                Else
                    dRow("ErrorOccurred") = ""
                    dRow("ReturnCode") = ""
                End If
                dtStatusResults.Rows.Add(dRow)

                If PARAMOUT.Substring(10239, 1) <> "E" And PARAMOUT.Substring(10237, 2).Trim = "" Then

                    'Extract the data from the parameter
                    Dim position As Integer = 0
                    Dim exitLoop As Boolean = False
                    Dim dePC As DEPriceCode
                    Do While Not exitLoop
                        ' Determine the start point for the next set of price code information
                        If PARAMOUT.Substring(position, 60).Trim = "" Then
                            exitLoop = True
                        Else
                            If Not _dicPriceCodeDetails.ContainsKey(PARAMOUT.Substring(position, 2).Trim()) Then
                                dePC = New DEPriceCode(PARAMOUT.Substring(position, 2).Trim(), PARAMOUT.Substring(position + 2, 10), PARAMOUT.Substring(position + 12, 40), convertToBool(PARAMOUT.Substring(position + 52, 1)))
                                _dicPriceCodeDetails.Add(PARAMOUT.Substring(position, 2).Trim(), dePC)
                            End If
                            'Loop again
                            position = position + 60
                        End If
                    Loop
                    'PARAMOUT.Substring(10234, 2).Trim() gives stadium code
                    lastPriceCode = PARAMOUT.Substring(10232, 2)
                    If PARAMOUT.Substring(10231, 1).Trim = "N" Then
                        moreRecords = False
                    End If
                End If
            Loop

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPR-WS132R"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function CallWS132R(ByVal lastPriceCode As String) As String
        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS132R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 10240)
        parmIO.Value = WS132RParm(lastPriceCode)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS132R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS132R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT
    End Function

    Private Function WS132RParm(ByVal lastPriceCode As String) As String
        Dim myString As String = String.Empty

        'Construct the parameter
        myString = Utilities.FixStringLength("", 10000) &
                    Utilities.FixStringLength("", 232) &
                    Utilities.FixStringLength(lastPriceCode, 2) &
                    Utilities.FixStringLength(De.StadiumCode, 2) &
                    Utilities.FixStringLength(De.Src, 1) &
                    Utilities.FixStringLength("", 3)
        Return myString
    End Function
#End Region

End Class
