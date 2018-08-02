Imports System.Threading

Partial Class PagesAdmin_Sleep
    Inherits Base01

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not String.IsNullOrEmpty(Request("sleep")) Then
            Dim sleep As String = Request("sleep")
            Thread.Sleep(CInt(sleep))
            Response.Write("Went To Sleep For - " & sleep)
        Else
            Response.Write("No Sleep Required")
        End If

    End Sub


End Class
