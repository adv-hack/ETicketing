
<Serializable()> _
Public Class DEAmendTicketingItems

    Public Property CustomerSelection() As String
    Public Property SessionID() As String
    Public Property CustomerNo() As String
    Public Property CollAmendItems() As New Collection
    Public Property ErrorCode() As String
    Public Property Src() As String
    Public Property ByPassPreReqCheck() As String
    Public Property OverrideBasketErrorCode() As String
    Public Property ProductCodeInError() As String
    Public Property StandCodeInError() As String

    Public Function LogString() As String
        Dim sb As New System.Text.StringBuilder
        Dim Detbi As DETicketingBasketItem
        With sb
            .Append(SessionID & ",")
            .Append(CustomerNo & ",")
            For Each Detbi In CollAmendItems
                .Append(Detbi.LogString & ",")
            Next
            .Append(ErrorCode & ",")
            .Append(Src & ",")
        End With

        Return sb.ToString.Trim
    End Function

End Class
