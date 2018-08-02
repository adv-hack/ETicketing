Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text

<Serializable()> _
Public Class DBCustomerText
    Inherits DBAccess

    Public Property DeCustomerText As New DECustomerText
    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj

          Select _settings.ModuleName
            ' This is full ticketing completion routine
            Case Is = "RetrieveTalentCustomerText" : err = CallWS204RtoRetrieve()
            Case Is = "UpdateTalentCustomerText" : err = CallWS204RtoUpdate()
        End Select

        Return err
    End Function

    ' This calls WS204R to retrieve customer text as maintained via CD020R F10 press
    ' It calls the RPG only once but passes a big enough paramater to return more text than anyone has ever entered !

    Private Function CallWS204RtoRetrieve() As Talent.Common.ErrorObj

        Dim cmdSELECT As iDB2Command = Nothing
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim sLastRecord As String = "000"
        Dim sRecordTotal As String = "000"
        Dim bMoreRecords As Boolean = True
        Dim bError As Boolean = False
        Dim dRow As DataRow = Nothing
        Dim strProgram As String = "WS204R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1, @PARAM2)"
        Dim parmIO1 As iDB2Parameter
        Dim parmIO2 As iDB2Parameter
        Dim PARAMOUT1 As String = String.Empty
        Dim PARAMOUT2 As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameters

        parmIO1 = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO1.Value = buildWS204Parm("R")
        parmIO1.Direction = ParameterDirection.InputOutput

        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 35000)
        parmIO2.Value = " "
        parmIO2.Direction = ParameterDirection.InputOutput

        ' Call WS204R
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT1 = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString

        'Create the Status data table
        Dim DtStatusResults As New DataTable("Status")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the customer text table 
        Dim DtCustomerText As New DataTable("CustomerText")
        DtCustomerText.Columns.Add("text", GetType(String))
        ResultDataSet.Tables.Add(DtCustomerText)

        'Set the response data on the first call to WS015R
        If sLastRecord = "000" Then
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT1.Substring(1023, 1) = "E" Or PARAMOUT1.Substring(1021, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT1.Substring(1021, 2)
                bMoreRecords = False
                bError = True
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)
        End If

        'No errors 
        If bError = False Then

            'Extract the data from the parameter
            Dim iPosition As Integer = 0
            Dim iCounter As Integer = 1
            Dim NumberOfLines As Integer = PARAMOUT1.Substring(1015, 5).Trim
            Do While iCounter <= NumberOfLines

                'Create a new row
                dRow = Nothing
                dRow = DtCustomerText.NewRow
                dRow("Text") = PARAMOUT2.Substring(iPosition, 70)
                DtCustomerText.Rows.Add(dRow)
                iPosition = iPosition + 70
                iCounter = iCounter + 1
            Loop


            'Catch ex As Exception
            '    ResultDataSet = Nothing
            '    Const strError As String = "Error during database access"
            '    With err
            '        .ErrorMessage = ex.Message
            '        .ErrorStatus = strError
            '        .ErrorNumber = "TACDBPD-WS015R"
            '        .HasError = True
            '    End With
            'End Try

        End If
        Return err
    End Function

    ' This calls WS204R to update customer text as maintained via CD020R F10 press
    ' It calls the RPG only once but passes a big enough paramater to return more text than anyone has ever entered !

    Private Function CallWS204RtoUpdate() As Talent.Common.ErrorObj

        Dim cmdSELECT As iDB2Command = Nothing
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim sLastRecord As String = "000"
        Dim sRecordTotal As String = "000"
        Dim bMoreRecords As Boolean = True
        Dim bError As Boolean = False
        Dim dRow As DataRow = Nothing
        Dim strProgram As String = "WS204R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1, @PARAM2)"
        Dim parmIO1 As iDB2Parameter
        Dim parmIO2 As iDB2Parameter
        Dim PARAMOUT1 As String = String.Empty
        Dim PARAMOUT2 As String = String.Empty


        Try

            'Set the connection string
            cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

            'Populate the parameters
            parmIO1 = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
            parmIO1.Value = buildWS204Parm("U")
            parmIO1.Direction = ParameterDirection.InputOutput

            parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 35000)
            parmIO2.Value = DeCustomerText.CustomerText
            parmIO2.Direction = ParameterDirection.InputOutput

            ' Call WS204R
            cmdSELECT.ExecuteNonQuery()
            PARAMOUT1 = cmdSELECT.Parameters(Param1).Value.ToString
            PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString

            'Create the Status data table
            Dim DtStatusResults As New DataTable
            ResultDataSet.Tables.Add(DtStatusResults)
            With DtStatusResults.Columns
                .Add("ErrorOccurred", GetType(String))
                .Add("ReturnCode", GetType(String))
            End With

            'Create the customer text table 
            Dim DtCustomerText As New DataTable
            DtCustomerText.Columns.Add("text", GetType(String))
            ResultDataSet.Tables.Add(DtCustomerText)

            'Set the response data on the first call to WS015R
            If sLastRecord = "000" Then
                dRow = Nothing
                dRow = DtStatusResults.NewRow
                If PARAMOUT1.Substring(1023, 1) = "E" Or PARAMOUT1.Substring(1021, 2).Trim <> "" Then
                    dRow("ErrorOccurred") = "E"
                    dRow("ReturnCode") = PARAMOUT1.Substring(1021, 2)
                    bMoreRecords = False
                    bError = True
                Else
                    dRow("ErrorOccurred") = ""
                    dRow("ReturnCode") = ""
                End If
                DtStatusResults.Rows.Add(dRow)
            End If

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "CustText-WS204R"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function buildWS204Parm(mode As String) As String
        Dim myString As String
        'Construct the parameter
        myString = Utilities.FixStringLength(DeCustomerText.Customer, 12) & _
                   Utilities.FixStringLength(mode, 1) & _
                   Utilities.FixStringLength(DeCustomerText.Agent, 10) & _
                   Utilities.FixStringLength("", 1011)
        Return myString
    End Function
End Class
