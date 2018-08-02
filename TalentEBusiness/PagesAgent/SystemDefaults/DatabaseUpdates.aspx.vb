Imports System.Net.Http
Imports System.Net.Http.Formatting
Imports System.Collections.Generic

Partial Class DatabaseUpdates
	Inherits SystemDefaultsBasePage

    Const API_SystemDefaults_Database As String = "SystemDefaultsDatabase"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim content As String = String.Empty

        Try
			If Not IsPostBack Then
				content = GetValues(API_SystemDefaults_Database)
			Else
				Dim values As New Dictionary(Of String, String)
				For Each key As String In Request.Form.AllKeys
					If Not key = "__VIEWSTATE" Then
						values.Add(key, Request.Form(key))
					End If
				Next
				content = PostValues(API_SystemDefaults_Database, values)
			End If

            If (content <> String.Empty) Then
                container.InnerHtml = content
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
