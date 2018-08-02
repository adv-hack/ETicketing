Imports System.Text
<Serializable()> _
Public Class DECompanySearch
    Public Property Company() As DECompany

    ' Parameters for Search Lists: Start position 
    Public Property Start As Integer
    ' Parameters for Search Lists: Number of records to return)
    Public Property Length As Integer

    ' Order by fields
    Public Property SortOrder As String

    ' Draw parameter is used to determine if we are on a fresh search or 
    ' just paging/reordering an existing list. We only need to perform a record count 
    ' in the former case
    Public Property Draw As String

    Public ReadOnly Property LogString() As String
        Get
            Dim logStringBuilder As New StringBuilder
            With logStringBuilder
                .Append(Company.LogString)
                .Append(Start)
                .Append(Length)
                .Append(SortOrder)
                .Append(Draw)
            End With

            Return logStringBuilder.ToString
        End Get
    End Property

End Class
