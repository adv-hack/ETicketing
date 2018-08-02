Imports System.Text
Imports IBM.Data.DB2.iSeries
Imports Talent.Common
Imports Talent.Common.Utilities

<Serializable()> _
Public Class DBPrint
    Inherits DBAccess

#Region "Constants"


    Private Const PrintTicketsByWeb As String = "PrintTicketsByWeb"
    Private Const DespatchTicketPrint As String = "DespatchTicketPrint"
#End Region

#Region "Public Properties"

    Public Property PrintEntity() As DEPrint

#End Region

#Region "Protected Functions"

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj

        Select Case _settings.ModuleName
            Case Is = PrintTicketsByWeb : err = AccessDatabaseWS096R()
            Case Is = DespatchTicketPrint : err = AccessDatabaseWS633R()
        End Select

        Return err
    End Function

#End Region

#Region "Private Functions"
    Private Function AccessDatabaseWS633R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable
        DtStatusResults.TableName = "ErrorStatus"
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With
        Try

            'Call WS633R
            PARAMOUT = CallWS633R()

            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(15999, 1) = GlobalConstants.ERRORFLAG Or PARAMOUT.Substring(15997, 2).Trim <> "" Then
                dRow("ErrorOccurred") = GlobalConstants.ERRORFLAG
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBF-WS633R"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function CallWS633R() As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/WS633R(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 16000)
        parmIO.Value = WS633Parm()
        parmIO.Direction = ParameterDirection.InputOutput


        'Execute
        TalentCommonLog("CallWS633R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        TalentCommonLog("CallWS633R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT
    End Function

    Private Function WS633Parm() As String
        Dim parm1 As New StringBuilder
        For Each reference As String In PrintEntity.PaymentReferences
            parm1.Append(Utilities.PadLeadingZeros(reference, 15))
        Next
        parm1.Append(Utilities.FixStringLength(String.Empty, (1000 - PrintEntity.PaymentReferences.Count) * 15))
        parm1.Append(Utilities.FixStringLength(String.Empty, 984))
        parm1.Append(Utilities.FixStringLength(Settings.AgentEntity.AgentUsername, 10))
        parm1.Append(Utilities.FixStringLength(String.Empty, 1))
        parm1.Append(Utilities.FixStringLength(ConvertToYN(PrintEntity.UnPrintedTickets), 1))
        parm1.Append(Utilities.FixStringLength(ConvertToYN(PrintEntity.PrintAll), 1))
        parm1.Append(Utilities.FixStringLength(String.Empty, 3))
        Return parm1.ToString()
    End Function
    Private Function AccessDatabaseWS096R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
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
        Try

            'Call WS096R
            PARAMOUT = CallWS096R()

            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(1023, 1) = GlobalConstants.ERRORFLAG Or PARAMOUT.Substring(1021, 2).Trim <> "" Then
                dRow("ErrorOccurred") = GlobalConstants.ERRORFLAG
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBF-WS096R"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function CallWS096R() As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/WS096R(@PARAM1,@PARAM2)"
        Dim parmIO, parmIO2 As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS096Parm1()
        parmIO.Direction = ParameterDirection.InputOutput

        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 5120)
        parmIO2.Value = WS096Parm2()
        parmIO2.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS096R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        TalentCommonLog("CallWS096R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT
    End Function

    Private Function WS096Parm1() As String
        Dim parm1 As New StringBuilder
        parm1.Append(Utilities.FixStringLength(String.Empty, 3))
        parm1.Append(Utilities.FixStringLength(Settings.OriginatingSource, 10))
        parm1.Append(Utilities.PadLeadingZeros(PrintEntity.PaymentReference, 15))
        parm1.Append(Utilities.PadLeadingZeros(PrintEntity.RelativeRecordNumber, 16))
        parm1.Append(Utilities.PadLeadingZeros(PrintEntity.BulkSalesID, 13))
        parm1.Append(Utilities.FixStringLength(String.Empty, 952))
        parm1.Append(Utilities.FixStringLength("Y", 1))
        parm1.Append(Utilities.FixStringLength(String.Empty, 14))
        Return parm1.ToString()
    End Function

    Private Function WS096Parm2() As String
        Dim parm2 As New StringBuilder
        parm2.Append(Utilities.FixStringLength(Utilities.ConvertToYN(PrintEntity.PrintTransaction), 1))
        parm2.Append(Utilities.FixStringLength(Utilities.ConvertToYN(PrintEntity.PrintAddress), 1))
        parm2.Append(Utilities.FixStringLength(PrintEntity.PaymentOwnerName, 30))
        parm2.Append(Utilities.FixStringLength(PrintEntity.AddressLine1, 30))
        parm2.Append(Utilities.FixStringLength(PrintEntity.AddressLine2, 30))
        parm2.Append(Utilities.FixStringLength(PrintEntity.AddressLine3, 30))
        parm2.Append(Utilities.FixStringLength(PrintEntity.AddressLine4, 30))
        parm2.Append(Utilities.FixStringLength(PrintEntity.PostCodePart1, 4))
        parm2.Append(Utilities.FixStringLength(PrintEntity.PostCodePart2, 4))
        parm2.Append(Utilities.FixStringLength(String.Empty, 4960))
        Return parm2.ToString()
    End Function

#End Region

End Class
