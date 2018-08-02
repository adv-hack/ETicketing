
Partial Class DatabaseSearch
	Inherits SystemDefaultsBasePage

    Const API_DatabaseSearch As String = "DatabaseSearch/"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
			If (Not IsPostBack) Then
				container.InnerHtml = GetValues(API_DatabaseSearch)
			End If
        Catch ae As AggregateException
            HandleException(ae)
        End Try
    End Sub

    Protected Overrides Sub ShowErrorMessage(ByVal msg As String)
        blErrorMessages.Items.Add(msg)
        plhErrorList.Visible = True
    End Sub

End Class
