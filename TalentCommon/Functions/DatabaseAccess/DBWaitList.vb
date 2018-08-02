Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports Talent.Common
Imports Talent.Common.Utilities

<Serializable()> _
Public Class DBWaitList
    Inherits DBAccess

    Private _de As DEWaitList
    Public Property DE() As DEWaitList
        Get
            Return _de
        End Get
        Set(ByVal value As DEWaitList)
            _de = value
        End Set
    End Property

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj

        Select Case _settings.ModuleName
            Case "RetrieveCustomerWaitListHistory"
                err = AccessDatabase_MD196R()
            Case "WithdrawCustomerWaitListRequest"
                err = AccessDatabase_MD199R()
            Case "AddCustomerWaitListRequest"
                err = AccessDatabase_MD199R()

        End Select

        Return err
    End Function

   

    Protected Function AccessDatabase_MD196R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("DtStatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the product  Details data table
        Dim DtWaitListHeaderResults As New DataTable("DtWaitListHeaderResults")
        ResultDataSet.Tables.Add(DtWaitListHeaderResults)
        With DtWaitListHeaderResults.Columns
            .Add("UpdatedDate", GetType(String))
            .Add("UpdatedBy_CustomerNo", GetType(String))
            .Add("UpdatedBy_FirstName", GetType(String))
            .Add("UpdatedBy_Surname", GetType(String))
            .Add("AddedDate", GetType(String))
            .Add("AddedBy_CustomerNo", GetType(String))
            .Add("AddedBy_FirstName", GetType(String))
            .Add("AddedBy_Surname", GetType(String))
            .Add("PreferredStand1", GetType(String))
            .Add("PreferredArea1", GetType(String))
            .Add("WaitListID", GetType(String))
            .Add("NoOfLinkedRequests", GetType(String))
            .Add("WaitListRequests", GetType(String))
            .Add("Status", GetType(String))
            .Add("PreferredStand2", GetType(String))
            .Add("PreferredArea2", GetType(String))
            .Add("PreferredStand3", GetType(String))
            .Add("PreferredArea3", GetType(String))
        End With

        Dim DtWaitListDetailResults As New DataTable("DtWaitListDetailResults")
        ResultDataSet.Tables.Add(DtWaitListDetailResults)
        With DtWaitListDetailResults.Columns
            .Add("CustomerNumber", GetType(String))
            .Add("FirstName", GetType(String))
            .Add("Surname", GetType(String))
            .Add("Status", GetType(String))
            .Add("Stand", GetType(String))
            .Add("Area", GetType(String))
        End With

        Try
            PARAMOUT = CallMD196R()

            'Set the response data
            Dim sRow As DataRow
            sRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(1023, 1) = "E" Or PARAMOUT.Substring(1021, 2).Trim <> "" Then
                sRow("ErrorOccurred") = "E"
                sRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
            Else
                sRow("ErrorOccurred") = ""
                sRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(sRow)

            'No errors 
            If PARAMOUT.Substring(1023, 1) <> "E" And PARAMOUT.Substring(1021, 2).Trim = "" Then

                Dim hRow As DataRow
                hRow = DtWaitListHeaderResults.NewRow

                hRow("UpdatedDate") = PARAMOUT.Substring(0, 7)
                hRow("UpdatedBy_CustomerNo") = PARAMOUT.Substring(7, 12)
                hRow("UpdatedBy_FirstName") = PARAMOUT.Substring(19, 20)
                hRow("UpdatedBy_Surname") = PARAMOUT.Substring(39, 30)
                hRow("AddedDate") = PARAMOUT.Substring(69, 7)
                hRow("AddedBy_CustomerNo") = PARAMOUT.Substring(76, 12)
                hRow("AddedBy_FirstName") = PARAMOUT.Substring(88, 20)
                hRow("AddedBy_Surname") = PARAMOUT.Substring(108, 30)
                hRow("PreferredStand1") = PARAMOUT.Substring(138, 3)
                hRow("PreferredArea1") = PARAMOUT.Substring(141, 4)
                hRow("WaitListID") = PARAMOUT.Substring(145, 13)
                hRow("NoOfLinkedRequests") = PARAMOUT.Substring(158, 1)
                hRow("WaitListRequests") = PARAMOUT.Substring(159, 1)
                hRow("Status") = PARAMOUT.Substring(160, 1)
                hRow("PreferredStand2") = PARAMOUT.Substring(786, 3)
                hRow("PreferredArea2") = PARAMOUT.Substring(789, 4)
                hRow("PreferredStand3") = PARAMOUT.Substring(793, 3)
                hRow("PreferredArea3") = PARAMOUT.Substring(796, 4)

                DtWaitListHeaderResults.Rows.Add(hRow)

                Dim iStart As Integer = 161
                Dim iInc As Integer = 79
                Dim iCounter As Integer = 1
                Do While iCounter <= 8

                    ' Has a record been returned
                    If PARAMOUT.Substring(iStart, 1).Trim = "" Then
                        Exit Do
                    Else

                        Dim dRow As DataRow = DtWaitListDetailResults.NewRow

                        dRow("CustomerNumber") = PARAMOUT.Substring(iStart, 12)
                        dRow("FirstName") = PARAMOUT.Substring(iStart + 12, 20)
                        dRow("Surname") = PARAMOUT.Substring(iStart + 32, 30)
                        dRow("Status") = PARAMOUT.Substring(iStart + 62, 1)
                        dRow("Stand") = PARAMOUT.Substring(iStart + 63, 3)
                        dRow("Area") = PARAMOUT.Substring(iStart + 66, 4)
                        'Dim remainder As String = PARAMOUT.Substring(iStart + 70, 8)

                        DtWaitListDetailResults.Rows.Add(dRow)

                        'Increment
                        iStart += iInc
                        iCounter += 1

                    End If
                Loop
            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-MD196R"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function CallMD196R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "MD196R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = MD196Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallMD196R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallMD196R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function
    Private Function MD196Parm() As String

        Dim myString As String

        myString = Utilities.FixStringLength("", 1008) & _
                 Utilities.PadLeadingZeros(DE.CustomerNumber, 12) & _
                 Utilities.FixStringLength(DE.Src, 1) & _
                 Utilities.FixStringLength("", 3)

        Return myString

    End Function


    Protected Function AccessDatabase_MD199R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try
            PARAMOUT = CallMD199R()

            'Set the response data
            Dim sRow As DataRow
            sRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(1023, 1) = "E" Or PARAMOUT.Substring(1021, 2).Trim <> "" Then
                sRow("ErrorOccurred") = "E"
                sRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
            Else
                sRow("ErrorOccurred") = ""
                sRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(sRow)

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPD-MD199R"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function CallMD199R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "MD199R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = MD199Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallMD199R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallMD199R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function
    Private Function MD199Parm() As String

        Dim myString As String = ""

        Select Case DE.RequestType
            Case DEWaitList.WaitListType.Add
                myString = Utilities.FixStringLength("", 13) & _
                                Utilities.PadLeadingZeros("N", 1) & _
                                Utilities.FixStringLength(DE.CurrentSeasonTicketProduct, 6) & _
                                Utilities.FixStringLength(DE.PreferredStand1, 3) & _
                                Utilities.FixStringLength(DE.PreferredArea1, 4) & _
                                Utilities.FixStringLength(DE.CustomerPhoneNo, 20) & _
                                Utilities.FixStringLength(DE.CustomerEmailAddress, 80) & _
                                Utilities.FixStringLength(DE.Comment1, 50) & _
                                Utilities.FixStringLength(DE.Comment2, 50) & _
                                Utilities.FixStringLength(DE.Quantity.ToString, 1)

                Dim requestsStr As String = ""
                For Each str As String In DE.WaitListRequests
                    requestsStr += Utilities.PadLeadingZeros(str, 12) & _
                                    Utilities.FixStringLength("", 16)
                Next

                myString += Utilities.FixStringLength(requestsStr, 224) & _
                            Utilities.FixStringLength(DE.PreferredStand2, 3) & _
                            Utilities.FixStringLength(DE.PreferredArea2, 4) & _
                            Utilities.FixStringLength(DE.PreferredStand3, 3) & _
                            Utilities.FixStringLength(DE.PreferredArea3, 4) & _
                            Utilities.FixStringLength(DE.TalentUser, 8) & _
                            Utilities.PadLeadingZeros("", 13)

                If DE.CheckPendingRequests Then
                    myString += "Y"
                Else
                    myString += "N"
                End If

                myString += Utilities.FixStringLength(DE.ActionComment1, 60) & _
                            Utilities.FixStringLength(DE.ActionComment2, 60) & _
                            Utilities.FixStringLength(DE.ActionComment3, 60) & _
                            Utilities.FixStringLength(DE.ActionComment4, 60) & _
                            Utilities.FixStringLength(DE.ActionComment5, 60) & _
                            Utilities.FixStringLength(DE.ActionComment6, 60) & _
                            Utilities.FixStringLength("", 160) & _
                            Utilities.FixStringLength(DE.CustomerNumber, 12) & _
                            Utilities.FixStringLength(DE.Src, 1) & _
                            Utilities.FixStringLength("", 3)

            Case DEWaitList.WaitListType.Withdraw
                myString = Utilities.PadLeadingZeros(DE.WaitListID, 13) & _
                                Utilities.PadLeadingZeros("W", 1) & _
                                Utilities.FixStringLength("", 994) & _
                                Utilities.FixStringLength(DE.CustomerNumber, 12) & _
                                Utilities.FixStringLength(DE.Src, 1) & _
                                Utilities.FixStringLength("", 3)

        End Select


        Return myString

    End Function



End Class
