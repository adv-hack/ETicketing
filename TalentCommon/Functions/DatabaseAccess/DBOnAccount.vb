Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Text
Imports IBM.Data.DB2.iSeries
Imports Talent.Common
Imports Talent.Common.Utilities

<Serializable()> _
Public Class DBOnAccount
    Inherits DBAccess

    Private Const RETRIEVE_ON_ACCOUNT_DETAILS As String = "RetrieveOnAccountDetails"

#Region "TALENTTKT"

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj

        Dim err As New ErrorObj

        Select Case _settings.ModuleName
            Case Is = RETRIEVE_ON_ACCOUNT_DETAILS : err = AccessDatabaseWS067R()
        End Select

        Return err

    End Function

#End Region

    Private Function AccessDatabaseWS067R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim sLastRecord As String = "000"
        Dim sRecordTotal As String = "000"
        Dim sMore As String = " "
        Dim bMoreRecords As Boolean = True
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable
        DtStatusResults.TableName = "ResultStatus"
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the product list data table
        Dim DtOnAccountDetailsResults As New DataTable
        DtOnAccountDetailsResults.TableName = "OnAccountDetails"
        ResultDataSet.Tables.Add(DtOnAccountDetailsResults)
        OnAccountDetailsResultsDataTable(DtOnAccountDetailsResults)

        Try

            'Loop until no more products available
            Do While bMoreRecords = True

                'Call WS067R
                PARAMOUT = CallWS067R(sRecordTotal, sLastRecord)

                'Set the response data on the first call to WS057R
                If sLastRecord = "000" Then
                    dRow = Nothing
                    dRow = DtStatusResults.NewRow
                    If PARAMOUT.Substring(3071, 1) = "E" Or PARAMOUT.Substring(3069, 2).Trim <> "" Then
                        dRow("ErrorOccurred") = "E"
                        dRow("ReturnCode") = PARAMOUT.Substring(3069, 2)
                        bMoreRecords = False
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                    End If
                    DtStatusResults.Rows.Add(dRow)
                End If

                'No errors 
                If PARAMOUT.Substring(5119, 1) <> "E" And PARAMOUT.Substring(5117, 2).Trim = "" Then

                    'Extract the data from the parameter
                    Dim iPosition As Integer = 0
                    Dim iCounter As Integer = 1
                    Do While iCounter <= 120

                        ' Has anything being returned?
                        If PARAMOUT.Substring(iPosition, 6).Trim = "" Then
                            Exit Do
                        Else

                            'Create a new row
                            dRow = Nothing
                            dRow = DtOnAccountDetailsResults.NewRow
                            dRow("TransactionDate") = GetFormattedProductDate(Utilities.CheckForDBNull_String(PARAMOUT.Substring(iPosition, 7).Trim))
                            '   dRow("Amount") = Utilities.FormatCurrency(PARAMOUT.Substring(iPosition + 16, 9).Trim, currencyCode, businessUnit)
                            dRow("Sign") = PARAMOUT.Substring(iPosition + 16, 1).Trim
                            dRow("Product") = PARAMOUT.Substring(iPosition + 17, 6).Trim
                            dRow("ProductDescription") = PARAMOUT.Substring(iPosition + 23, 40).Trim
                            dRow("PaymentRef") = PARAMOUT.Substring(iPosition + 63, 15)
                            dRow("TransactionType") = PARAMOUT.Substring(iPosition + 78, 2).Trim
                            dRow("RunningBalance") = PARAMOUT.Substring(iPosition + 80, 7)
                            DtOnAccountDetailsResults.Rows.Add(dRow)

                            'Increment
                            iPosition = iPosition + 200
                            iCounter = iCounter + 1
                        End If
                    Loop

                    'Extract the footer information
                    sMore = PARAMOUT.Substring(5110, 1)
                    If sMore = "Y" Then
                        bMoreRecords = True
                    Else
                        bMoreRecords = False
                    End If
                End If

            Loop

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBOnAccount-WS067R"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function CallWS067R(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS067R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1,@PARAM2)"
        Dim parmIO, parmIO2 As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 5120)
        parmIO.Value = WS067Parm(sLastRecord)
        parmIO.Direction = ParameterDirection.InputOutput

        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 24000)
        parmIO2.Value = Utilities.FixStringLength("", 5120)
        parmIO2.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS067R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS067R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS067Parm(ByVal sLastRecord As String) As String

        Dim myString As String

        'Construct the parameter
        myString = Utilities.FixStringLength("", 3038) & _
          Utilities.FixStringLength(sLastRecord, 3) & _
          Utilities.FixStringLength("", 4)
        Return myString

    End Function

    Private Sub OnAccountDetailsResultsDataTable(ByRef dtOnAccountDetailsResults As DataTable)

        With dtOnAccountDetailsResults.Columns
            .Add("TransactionDate", GetType(String))
            .Add("Amount", GetType(String))
            .Add("Sign", GetType(Date))
            .Add("Product", GetType(String))
            .Add("ProductDescription", GetType(String))
            .Add("PaymentRef", GetType(String))
            .Add("TransactionType", GetType(String))
            .Add("RunningBalance", GetType(String))
            .Add("TotalBalance", GetType(String))
            .Add("RefundableBalance", GetType(String))
        End With

    End Sub

    ''' <summary>
    ''' Gets the formatted product date - IseriesDate is formatted to normal format
    ''' </summary>
    ''' <param name="productDate">The product date.</param><returns></returns>
    Private Function GetFormattedProductDate(ByVal productDate As String) As Date
        Dim formattedProductDate As Date
        If productDate.Trim("0").Length > 0 Then
            formattedProductDate = Utilities.ISeriesDate(productDate).ToString()
        End If
        Return formattedProductDate
    End Function

End Class
