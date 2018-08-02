Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Text
Imports IBM.Data.DB2.iSeries
Imports Talent.Common
Imports Talent.Common.Utilities

<Serializable()> _
Public Class DBFeeds
    Inherits DBAccess

    Private Const PRODUCT_FEEDS As String = "ProductFeeds"

    Public Property FeedsEntity() As DEFeeds

#Region "TALENTTKT"

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj

        Dim err As New ErrorObj

        Select Case _settings.ModuleName
            Case Is = PRODUCT_FEEDS : err = AccessDatabaseWS066R()
        End Select

        Return err

    End Function

#End Region

    Private Function AccessDatabaseWS066R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim sLastRecord As String = "00000"
        Dim sRecordTotal As String = "00000"
        Dim bMoreRecords As Boolean = True
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable
        DtStatusResults.TableName = GlobalConstants.STATUS_RESULTS_TABLE_NAME
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the product list data table
        Dim DtFeedsProductResults As New DataTable
        DtFeedsProductResults.TableName = "FeedsProduct"
        ResultDataSet.Tables.Add(DtFeedsProductResults)
        FeedsDataTable(DtFeedsProductResults)

        Try

            'Loop until no more products available
            Do While bMoreRecords = True

                'Call WS016R
                PARAMOUT = CallWS066R(sRecordTotal, sLastRecord)

                'Set the response data on the first call to WS016R
                If sLastRecord = "00000" Then
                    dRow = Nothing
                    dRow = DtStatusResults.NewRow
                    If PARAMOUT.Substring(30075, 1) = "E" Or PARAMOUT.Substring(30073, 2).Trim <> "" Then
                        dRow("ErrorOccurred") = "E"
                        dRow("ReturnCode") = PARAMOUT.Substring(30073, 2)
                        bMoreRecords = False
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                    End If
                    DtStatusResults.Rows.Add(dRow)
                End If

                'No errors 
                If PARAMOUT.Substring(30075, 1) <> "E" And PARAMOUT.Substring(30073, 2).Trim = "" Then

                    'Extract the data from the parameter
                    Dim iPosition As Integer = 0
                    Dim iCounter As Integer = 1
                    Do While iCounter <= 75

                        ' Has a product been returned
                        If PARAMOUT.Substring(iPosition, 6).Trim = "" Then
                            Exit Do
                        Else

                            'Create a new row
                            dRow = Nothing
                            dRow = DtFeedsProductResults.NewRow
                            dRow("ProductCode") = PARAMOUT.Substring(iPosition, 6).Trim
                            dRow("ProductDescription") = PARAMOUT.Substring(iPosition + 6, 40).Trim
                            dRow("ProductMDTE08") = PARAMOUT.Substring(iPosition + 225, 7).Trim
                            dRow("ProductDateYear") = GetFormattedProductDate(Utilities.CheckForDBNull_String(PARAMOUT.Substring(iPosition + 225, 7).Trim))
                            If Not convertToBool(PARAMOUT.Substring(iPosition + 252, 1)) Then
                                Dim productDate As Date = GetFormattedProductDate(Utilities.CheckForDBNull_String(PARAMOUT.Substring(iPosition + 225, 7).Trim))
                                dRow("ProductDate") = productDate.ToString("ddd dd MMM")
                                dRow("ProductYear") = productDate.ToString("yyyy")
                            Else
                                dRow("ProductDate") = String.Empty
                                dRow("ProductYear") = String.Empty
                            End If
                            If Not convertToBool(PARAMOUT.Substring(iPosition + 253, 1)) Then
                                dRow("ProductTime") = PARAMOUT.Substring(iPosition + 69, 7)
                            Else
                                dRow("ProductTime") = String.Empty
                            End If
                            dRow("ProductType") = PARAMOUT.Substring(iPosition + 125, 1).Trim
                            dRow("ProductOppositionCode") = PARAMOUT.Substring(iPosition + 160, 4)
                            dRow("ProductStadium") = PARAMOUT.Substring(iPosition + 205, 2).Trim
                            dRow("CampaignCode") = PARAMOUT.Substring(iPosition + 207, 2)
                            dRow("ProductSponsorCode") = PARAMOUT.Substring(iPosition + 209, 4)
                            dRow("UseVisualSeatLevelSelection") = PARAMOUT.Substring(iPosition + 213, 1)
                            dRow("ProductCompetitionCode") = PARAMOUT.Substring(iPosition + 214, 4)
                            dRow("ProductHomeAsAway") = PARAMOUT.Substring(iPosition + 220, 1).Trim
                            dRow("ProductSubType") = PARAMOUT.Substring(iPosition + 221, 4).Trim
                            dRow("ProductCompetitionDesc") = PARAMOUT.Substring(iPosition + 232, 20).Trim
                            dRow("HideDate") = convertToBool(PARAMOUT.Substring(iPosition + 252, 1))
                            dRow("HideTime") = convertToBool(PARAMOUT.Substring(iPosition + 253, 1))
                            dRow("RestrictGraphical") = convertToBool(PARAMOUT.Substring(iPosition + 254, 1))
                            dRow("Location") = PARAMOUT.Substring(iPosition + 255, 40).Trim
                            dRow("LocationId") = PARAMOUT.Substring(iPosition + 311, 15).Trim
                            dRow("ExcludeFromWebpage") = convertToBool(PARAMOUT.Substring(iPosition + 295, 1))
                            If String.IsNullOrWhiteSpace(PARAMOUT.Substring(iPosition + 296, 15)) Then
                                dRow("CategoryId") = 0
                            Else
                                dRow("CategoryId") = PARAMOUT.Substring(iPosition + 296, 15)
                            End If

                            DtFeedsProductResults.Rows.Add(dRow)
                            'Increment
                            iPosition = iPosition + 400
                            iCounter = iCounter + 1
                        End If
                    Loop

                    'Extract the footer information
                    sLastRecord = PARAMOUT.Substring(30067, 5)
                    sRecordTotal = PARAMOUT.Substring(30062, 5)
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
                .ErrorNumber = "TACDBF-WS066R"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function CallWS066R(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS066R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1,@PARAM2,@PARAM3)"
        Dim parmIO, parmIO2, parmIO3 As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter (parameter 1 not used now - replaced with parameter 3)
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 3072)
        parmIO.Value = Utilities.FixStringLength("", 3072)
        parmIO.Direction = ParameterDirection.InputOutput

        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 5120)
        parmIO2.Value = WS066Parm2()
        parmIO2.Direction = ParameterDirection.InputOutput

        parmIO3 = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 30076)
        parmIO3.Value = WS066Parm(sRecordTotal, sLastRecord)
        parmIO3.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS066R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param3).Value.ToString

        TalentCommonLog("CallWS066R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS066Parm(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String

        Dim myString As String
        Dim stadium0 As String = String.Empty
        Dim stadium1 As String = String.Empty
        Dim stadium2 As String = String.Empty
        Dim stadium3 As String = String.Empty
        Dim stadium4 As String = String.Empty
        Dim stadium5 As String = String.Empty

        'Construct the parameter
        myString = Utilities.FixStringLength("", 30038) & _
                 Utilities.FixStringLength(stadium0, 2) & _
                 Utilities.FixStringLength(stadium1, 2) & _
                 Utilities.FixStringLength(stadium2, 2) & _
                 Utilities.FixStringLength(stadium3, 2) & _
                 Utilities.FixStringLength(stadium4, 2) & _
                 Utilities.FixStringLength(stadium5, 2) & _
                 Utilities.FixStringLength("", 12) & _
                 Utilities.FixStringLength(sRecordTotal, 5) & _
                 Utilities.FixStringLength(sLastRecord, 5) & _
                 Utilities.FixStringLength("", 4)

        Return myString

    End Function

    Private Function WS066Parm2() As String
        Dim myString As String
        myString = Utilities.FixStringLength("", 30) & _
            Utilities.FixStringLength(Utilities.ConvertToYN(Utilities.CheckForDBNull_Boolean_DefaultTrue(FeedsEntity.Online_Products_Only)), 1) & _
            Utilities.FixStringLength("", 5089)

        Return myString

    End Function

    Private Sub FeedsDataTable(ByRef dtFeedsProductList As DataTable)

        With dtFeedsProductList.Columns
            .Add("ProductCode", GetType(String))
            .Add("ProductDescription", GetType(String))
            .Add("ProductMDTE08", GetType(String))
            .Add("ProductDateYear", GetType(Date))
            .Add("ProductDate", GetType(String))
            .Add("ProductYear", GetType(String))
            .Add("ProductTime", GetType(String))
            .Add("ProductType", GetType(String))
            .Add("ProductOppositionCode", GetType(String))
            .Add("ProductStadium", GetType(String))
            .Add("CampaignCode", GetType(String))
            .Add("ProductSponsorCode", GetType(String))
            .Add("UseVisualSeatLevelSelection", GetType(String))
            .Add("ProductCompetitionCode", GetType(String))
            .Add("ProductHomeAsAway", GetType(String))
            .Add("ProductSubType", GetType(String))
            .Add("ProductCompetitionDesc", GetType(String))
            .Add("HideDate", GetType(Boolean))
            .Add("HideTime", GetType(Boolean))
            .Add("RestrictGraphical", GetType(Boolean))
            .Add("CategoryId", GetType(Integer))
            .Add("LocationId", GetType(String))
            .Add("Location", GetType(String))
            .Add("ExcludeFromWebpage", GetType(Boolean))
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
