''' <summary>
''' This class is used for functionality that is shared between a WS060R call in DBPayment and WS607R call in DBCustomer.
''' The data table will contain the same data despite being called through either WS060R (when using the "Save My Card" page
''' or when logging on doing a WS607R call.
''' </summary>
''' <remarks></remarks>

<Serializable()> _
Public Class DBSaveCardFunctions

    Public Sub CreateCardDetailsTable(ByRef ResultDataSet As DataSet)
        Dim dtCardDetails As New DataTable("CardDetails")
        ResultDataSet.Tables.Add(dtCardDetails)
        With dtCardDetails.Columns
            .Add("UniqueCardId", GetType(String))
            .Add("LastFourCardDigits", GetType(String))
            .Add("CardNumber", GetType(String))
            .Add("ExpiryDate", GetType(String))
            .Add("StartDate", GetType(String))
            .Add("IssueNumber", GetType(String))
            .Add("CardType", GetType(String))
            .Add("VGTokenID", GetType(String))
            .Add("VGProcessingDB", GetType(String))
            .Add("VGTokenDate", GetType(String))
            .Add("DefaultCard", GetType(String))
        End With
    End Sub

    Public Sub PopulateCardDetails(ByRef ResultDataSet As DataSet, ByVal PARAMOUT As String, Optional ByVal startPosition As Integer = 0)
        Dim dRow As DataRow = Nothing
        Do While PARAMOUT.Substring(startPosition, 1).Trim <> String.Empty
            dRow = Nothing
            dRow = ResultDataSet.Tables("CardDetails").NewRow
            dRow("LastFourCardDigits") = PARAMOUT.Substring(startPosition, 4).Trim
            dRow("UniqueCardId") = PARAMOUT.Substring(startPosition + 4, 13).Trim
            dRow("CardNumber") = PARAMOUT.Substring(startPosition + 17, 20).Trim
            dRow("ExpiryDate") = PARAMOUT.Substring(startPosition + 38, 4).Trim
            dRow("StartDate") = PARAMOUT.Substring(startPosition + 42, 4).Trim
            dRow("IssueNumber") = PARAMOUT.Substring(startPosition + 46, 2).Trim
            dRow("CardType") = PARAMOUT.Substring(startPosition + 48, 30).Trim.ToUpper
            dRow("VGTokenID") = PARAMOUT.Substring(startPosition + 78, 18).Trim
            dRow("VGProcessingDB") = PARAMOUT.Substring(startPosition + 96, 15).Trim
            dRow("VGTokenDate") = PARAMOUT.Substring(startPosition + 111, 7).Trim
            dRow("DefaultCard") = PARAMOUT.Substring(startPosition + 118, 1).Trim
            ResultDataSet.Tables("CardDetails").Rows.Add(dRow)
            startPosition += 119
        Loop
    End Sub

End Class