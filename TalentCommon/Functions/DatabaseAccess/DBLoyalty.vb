Imports IBM.Data.DB2.iSeries
Imports Talent.Common.Utilities
Imports System.Text

<Serializable()> _
Public Class DBLoyalty
    Inherits DBAccess

#Region "Class Level Fields"

    Private Const AddLoyaltyPoints As String = "AddLoyaltyPoints"
    Private _deLoyaltyList As DELoyaltyList
    Private _itemCount As Integer = 0

#End Region

#Region "Public Properties"

    Public Property LoyaltyList As DELoyaltyList
        Get
            Return _deLoyaltyList
        End Get
        Set(ByVal value As DELoyaltyList)
            _deLoyaltyList = value
        End Set
    End Property

#End Region

#Region "TALENTTKT"

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj
        Select _settings.ModuleName
            Case Is = AddLoyaltyPoints : err = AccessDatabaseWS619R()
        End Select
        Return err
    End Function

    Private Function AccessDatabaseWS619R() As ErrorObj
        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim positionCounter As Integer = 0
        ResultDataSet = New DataSet

        createSupplyNetRequest(Settings.Partner, Settings.LoginId, Settings.SupplyNetRequestName, Settings.TransactionID, Settings.RequestCount, 0, Now, Nothing, False)

        Dim DtStatusResults As New DataTable
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("CustomerNumber", GetType(String))
            .Add("ProductCode", GetType(String))
            .Add("Date", GetType(Date))
            .Add("Time", GetType(String))
            .Add("Stand", GetType(String))
            .Add("Area", GetType(String))
            .Add("Row", GetType(String))
            .Add("Seat", GetType(String))
            .Add("Points", GetType(Integer))
            .Add("Success", GetType(Boolean))
            .Add("ReturnCode", GetType(String))
        End With

        Try
            While _deLoyaltyList.LoyaltyList.Count > _itemCount
                PARAMOUT = CallWS619R()
                positionCounter = 0
                While Not String.IsNullOrWhiteSpace(PARAMOUT.Substring(positionCounter, 1))
                    If incrementSupplyNetProgressCount(Settings.TransactionID) = Settings.RequestCount Then
                        markSupplyNetTransactionAsCompleted(Settings.TransactionID)
                    End If
                    dRow = Nothing
                    dRow = DtStatusResults.NewRow
                    dRow("CustomerNumber") = PARAMOUT.Substring(positionCounter, 12)
                    dRow("ProductCode") = PARAMOUT.Substring(positionCounter + 12, 6).Trim
                    dRow("Date") = CDate(PARAMOUT.Substring(positionCounter + 18, 10))
                    dRow("Time") = PARAMOUT.Substring(positionCounter + 28, 8)
                    dRow("Stand") = PARAMOUT.Substring(positionCounter + 36, 3).Trim
                    dRow("Area") = PARAMOUT.Substring(positionCounter + 39, 4).Trim
                    dRow("Row") = PARAMOUT.Substring(positionCounter + 43, 4).Trim
                    dRow("Seat") = PARAMOUT.Substring(positionCounter + 47, 5).Trim
                    dRow("Points") = CInt(PARAMOUT.Substring(positionCounter + 54, 5))
                    If String.IsNullOrWhiteSpace(PARAMOUT.Substring(positionCounter + 78, 2)) Then
                        dRow("Success") = True
                    Else
                        dRow("Success") = False
                    End If
                    dRow("ReturnCode") = PARAMOUT.Substring(positionCounter + 78, 2)
                    DtStatusResults.Rows.Add(dRow)
                    positionCounter += 80
                End While
            End While
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBLY-WS619R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS619R() As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS619R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 5012)
        parmIO.Value = WS619RParm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS619R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        TalentCommonLog("CallWS619R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT
    End Function

    Private Function WS619RParm() As String
        Dim myString As New StringBuilder

        ' This loop will execute until either;
        ' (a) 60 loyalty items have been added as a maximum, per call
        ' (b) all the items in the list have been added if there less than 60
        '
        ' If there are more than 60 items in the list the call will execute again
        ' but will start at the next position in the list which is passed in - itemCount
        Dim counter As Integer = 1
        While counter <= 60 And _deLoyaltyList.LoyaltyList.Count > _itemCount
            With _deLoyaltyList.LoyaltyList.Item(_itemCount)
                myString.Append(Utilities.PadLeadingZeros(.CustomerNumber, 12))
                myString.Append(Utilities.FixStringLength(.ProductCode, 6))
                myString.Append(Utilities.FixStringLength(.TheDate, 10)) 'need format
                myString.Append(Utilities.FixStringLength(.TheTime, 8)) 'need format
                myString.Append(Utilities.FixStringLength(.Stand, 3))
                myString.Append(Utilities.FixStringLength(.Area, 4))
                myString.Append(Utilities.FixStringLength(.Row, 4))
                myString.Append(Utilities.FixStringLength(.Seat, 5))
                If .Type = LoyaltyPointsType.Attendance Then
                    myString.Append("1 ")
                ElseIf .Type = LoyaltyPointsType.Adhoc Then
                    myString.Append("2 ")
                End If
                myString.Append(Utilities.PadLeadingZeros(.Points, 5))
                myString.Append(Utilities.FixStringLength("", 21)) 'fill up to position 80
                counter += 1
                _itemCount += 1
            End With
        End While

        ' Depending on the number of loyalty items added we need to pad the string up to 5003 characters.
        ' Therefore take the number of items added so far from 60 to determine the number of empty items.
        ' Then multiply it by 80 to get the exact number of characters to pad. Each item is 80 long.
        Dim padValue As Integer = 0
        counter -= 1
        padValue = 60 - counter
        padValue = padValue * 80
        myString.Append(Utilities.FixStringLength("", padValue))
        myString.Append(Utilities.FixStringLength("", 6))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSource, 1))

        Return myString.ToString
    End Function

#End Region

End Class
