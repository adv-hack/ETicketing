'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Credit finance
'
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DECreditFinance
    '---------------------------------------------------------------------------------
    '
    Public Property CreditFinanceCompanyCode() As String
    Public Property CreditFinanceOptionCode() As String
    Public Property OnlyAvailablePlan() As String

    Public Function LogString() As String

        Dim sb As New System.Text.StringBuilder

        With sb
            .Append(CreditFinanceCompanyCode)
        End With

        Return sb.ToString.Trim

    End Function
End Class
