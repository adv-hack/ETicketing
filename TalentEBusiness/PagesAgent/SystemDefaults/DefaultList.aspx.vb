Imports System.Collections.Generic
Imports System.Data
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Threading.Tasks
Imports System.Net.Http.Formatting
Imports System.Threading

Partial Class DefaultList
    Inherits SystemDefaultsBasePage

    Const API_SystemDefaults_List As String = "SystemDefaultsList"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If AgentProfile.IsAgent And AgentProfile.AgentPermissions.CanAccessSystemDefaults Then
            Dim content As String = String.Empty
            Dim strArray As New List(Of String)
            Dim values As New Dictionary(Of String, String)
            Try
                If Not IsPostBack Then
                    content = GetValues(API_SystemDefaults_List)
                Else
                    ' list of keys we need for update operations 
                    strArray.Add("defaultListTable_length")
                    strArray.Add("ModuleName")
                    strArray.Add("action")
                    strArray.Add("variableKey1")
                    strArray.Add("variableKey2")
                    strArray.Add("variableKey3")
                    strArray.Add("variableKey4")
                    For Each key As String In Request.Form.AllKeys
                        If strArray.Contains(key) Then
                            values.Add(key, Request.Form(key))
                        End If
                    Next
                    content = PostValues(API_SystemDefaults_List, values)
                End If

                If (content <> String.Empty) Then
                    container.InnerHtml = content
                End If
            Catch ae As AggregateException
                HandleException(ae)
            End Try
        Else
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If
    End Sub

    Protected Overrides Sub ShowErrorMessage(ByVal msg As String)
        blErrorMessages.Items.Add(msg)
        plhErrorList.Visible = True
    End Sub

End Class
