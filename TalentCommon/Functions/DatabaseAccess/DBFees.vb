Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports Talent.Common.Utilities
<Serializable()> _
Public Class DBFees
    Inherits DBAccess

    Private Const FEES_LIST As String = "FeesList"
    Private Const CLASSNAME As String = "DBFEES"

#Region "Protected Functions"

    ''' <summary>
    ''' Determine the module we are calling for this class and return the error object
    ''' </summary>
    ''' <returns>An error object</returns>
    ''' <remarks></remarks>
    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj

        Select Case Settings.ModuleName
            Case Is = FEES_LIST : err = AccessDatabaseWS175R()
        End Select

        Return err
    End Function

#End Region

#Region "Private Functions"

    Private Function AccessDatabaseWS175R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim sLastRecord As String = "0"
        Dim sRecordTotal As String = "0"
        Dim bMoreRecords As Boolean = True
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUT2 As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable
        DtStatusResults.TableName = "ResultStatus"
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim dtFees As New DataTable
        dtFees.TableName = "FeesList"
        ResultDataSet.Tables.Add(dtFees)
        CreateDtFeesColumns(dtFees)

        Try

            'Loop until no more fee products available
            Do While bMoreRecords = True

                'Call WS175R
                PARAMOUT = CallWS175R(sRecordTotal, sLastRecord, PARAMOUT2)

                'Set the response data on the first call to WS175R
                If sLastRecord = "0" Then
                    dRow = Nothing
                    dRow = DtStatusResults.NewRow
                    If PARAMOUT.Substring(15359, 1) = "E" Or PARAMOUT.Substring(15357, 2).Trim <> "" Then
                        dRow("ErrorOccurred") = "E"
                        dRow("ReturnCode") = PARAMOUT.Substring(15357, 2)
                        bMoreRecords = False
                        TalentLogger.Logging(CLASSNAME, "AccessDatabaseWS175R", PARAMOUT.Substring(15357, 2), "WS175R-ERRCODE", LogTypeConstants.TCBMFEESLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                    End If
                    DtStatusResults.Rows.Add(dRow)

                    'set the product excluded from fees on the first call to WS175R
                    Dim dtFeesExcludedProducts As New DataTable
                    dtFeesExcludedProducts.TableName = "FeesExcludedProducts"
                    ResultDataSet.Tables.Add(dtFeesExcludedProducts)
                    CreateDtFeesExcludedProducts(dtFeesExcludedProducts)
                    PopulateFeesExcludedProduct(PARAMOUT2, dtFeesExcludedProducts)

                    'set the websales profile department
                    Dim dtWebSalesDepartment As New DataTable
                    dtWebSalesDepartment.TableName = "WebSalesDepartment"
                    ResultDataSet.Tables.Add(dtWebSalesDepartment)
                    dtWebSalesDepartment.Columns.Add("WebSalesDepartment", GetType(String))
                    dRow = Nothing
                    dRow = dtWebSalesDepartment.NewRow
                    dRow("WebSalesDepartment") = PARAMOUT.Substring(15335, 10).Trim
                    dtWebSalesDepartment.Rows.Add(dRow)

                End If

                'No errors 
                If PARAMOUT.Substring(15359, 1) <> "E" And PARAMOUT.Substring(15357, 2).Trim = "" Then

                    'Extract the data from the parameter
                    Dim iPosition As Integer = 0
                    Dim iCounter As Integer = 1
                    Dim ApplyFeeTo As String = String.Empty
                    Do While iCounter <= 100

                        ' Has a fee code been returned
                        If PARAMOUT.Substring(iPosition, 6).Trim = "" Then
                            Exit Do
                        Else
                            'Create a new row
                            dRow = Nothing
                            dRow = dtFees.NewRow
                            dRow("FeeCode") = PARAMOUT.Substring(iPosition, 6).Trim
                            dRow("FeeDescription") = PARAMOUT.Substring(iPosition + 6, 35).Trim
                            dRow("FeeType") = PARAMOUT.Substring(iPosition + 41, 1).Trim
                            dRow("FeeCategory") = PARAMOUT.Substring(iPosition + 42, 10).Trim
                            dRow("FeeValue") = Utilities.FormatPrice(PARAMOUT.Substring(iPosition + 52, 9))
                            dRow("ApplyWebSales") = False
                            If PARAMOUT.Substring(iPosition + 61, 1).Trim = "Y" Then
                                dRow("ApplyWebSales") = True
                            End If
                            dRow("IsNegativeFee") = False
                            If PARAMOUT.Substring(iPosition + 62, 1).Trim = "Y" Then
                                dRow("IsNegativeFee") = True
                            End If
                            dRow("ProductType") = PARAMOUT.Substring(iPosition + 63, 1).Trim
                            dRow("ProductCode") = PARAMOUT.Substring(iPosition + 64, 7).Trim
                            dRow("FeeDepartment") = PARAMOUT.Substring(iPosition + 71, 10).Trim
                            dRow("CardType") = PARAMOUT.Substring(iPosition + 81, 30).Trim.ToUpper
                            dRow("IsSystemFee") = False
                            If PARAMOUT.Substring(iPosition + 111, 1).Trim = "Y" Then
                                dRow("IsSystemFee") = True
                            End If
                            dRow("ChargeFee") = Utilities.convertToBool(PARAMOUT.Substring(iPosition + 112, 1).Trim)
                            dRow("FeeFunction") = PARAMOUT.Substring(iPosition + 113, 10).Trim
                            ApplyFeeTo = PARAMOUT.Substring(iPosition + 123, 1).Trim
                            If String.IsNullOrWhiteSpace(ApplyFeeTo) Then
                                ApplyFeeTo = GlobalConstants.FEEAPPLYTO_TICKETING
                            End If
                            dRow("ApplyFeeTo") = ApplyFeeTo
                            dRow("ChargeType") = PARAMOUT.Substring(iPosition + 124, 1).Trim
                            dRow("GeographicalZone") = PARAMOUT.Substring(iPosition + 125, 10).Trim
                            dtFees.Rows.Add(dRow)
                            'Increment
                            iPosition = iPosition + 150
                            iCounter = iCounter + 1
                        End If
                    Loop

                    'Extract the footer information
                    sRecordTotal = PARAMOUT.Substring(15345, 6)
                    sLastRecord = PARAMOUT.Substring(15352, 4)
                    If CInt(sLastRecord) >= CInt(sRecordTotal) Then
                        bMoreRecords = False
                    End If
                Else
                    bMoreRecords = False
                End If
            Loop

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBF-WS175R"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "AccessDatabaseWS175R", strError, err, ex, LogTypeConstants.TCBMFEESLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Private Function CallWS175R(ByVal sRecordTotal As String, ByVal sLastRecord As String, ByRef PARAMOUT2 As String) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS175R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1, @PARAM2)"
        Dim parmIO As iDB2Parameter
        Dim parmIO2 As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 15360)
        parmIO.Value = WS175Parm(sRecordTotal, sLastRecord)
        parmIO.Direction = ParameterDirection.InputOutput

        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 5120)
        parmIO2.Value = Utilities.FixStringLength("", 5120)
        parmIO2.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS175R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value & ", parmIO2.Value=" & parmIO2.Value)

        cmdSELECT.ExecuteNonQuery()

        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString

        TalentCommonLog("CallWS175R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS175Parm(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String

        Dim parmString As String

        'Construct the parameter
        parmString = Utilities.FixStringLength("", 15345) & _
                 Utilities.PadLeadingZeros(sRecordTotal, 6) & _
                 Utilities.FixStringLength("", 1) & _
                 Utilities.PadLeadingZeros(sLastRecord, 4) & _
                 Utilities.FixStringLength(Settings.OriginatingSourceCode, 1) & _
                 Utilities.FixStringLength("", 2) & _
                 Utilities.FixStringLength("", 1)

        Return parmString

    End Function

    Private Sub PopulateFeesExcludedProduct(ByVal ParamOut2 As String, ByRef FeesExcludedProducts As DataTable)
        Dim iPosition As Integer = 0
        Dim iCounter As Integer = 1
        Dim dRow As DataRow = Nothing
        Do While iCounter <= 100
            If ParamOut2.Substring(iPosition, 6).Trim = "" Then
                Exit Do
            Else
                dRow = Nothing
                dRow = FeesExcludedProducts.NewRow
                dRow("ProductCode") = ParamOut2.Substring(iPosition, 6).Trim
                FeesExcludedProducts.Rows.Add(dRow)
                iPosition = iPosition + 6
                iCounter = iCounter + 1
            End If
        Loop
    End Sub

    Private Sub CreateDtFeesColumns(ByRef dtFees As DataTable)

        With dtFees.Columns
            .Add("FeeCode", GetType(String))
            .Add("FeeDescription", GetType(String))
            .Add("FeeType", GetType(String))
            .Add("FeeCategory", GetType(String))
            .Add("FeeValue", GetType(Decimal))
            .Add("ApplyWebSales", GetType(Boolean))
            .Add("IsNegativeFee", GetType(Boolean))
            .Add("ProductType", GetType(String))
            .Add("ProductCode", GetType(String))
            .Add("FeeDepartment", GetType(String))
            .Add("CardType", GetType(String))
            .Add("IsSystemFee", GetType(Boolean))
            .Add("ChargeFee", GetType(Boolean))
            .Add("FeeFunction", GetType(String))
            .Add("ApplyFeeTo", GetType(String))
            .Add("ChargeType", GetType(String))
            .Add("GeographicalZone", GetType(String))
        End With

    End Sub

    Private Sub CreateDtFeesExcludedProducts(ByRef FeesExcludedProducts As DataTable)

        With FeesExcludedProducts.Columns
            .Add("ProductCode", GetType(String))
        End With

    End Sub

#End Region



End Class
