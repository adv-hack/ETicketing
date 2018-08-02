Imports System.Collections.Generic
Imports System.Data
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Threading.Tasks
Imports System.Net.Http.Formatting
Imports System.Linq
Imports TalentEBusiness.SystemDefaults

Partial Class SystemDefaults
    Inherits SystemDefaultsBasePage

    Const API_SystemDefaults As String = "SystemDefaultsValues"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim returnValue As String = String.Empty
        Try
            If (Not IsPostBack) Then
                returnValue = GetValues(API_SystemDefaults)
                ' check to see if there is "returnURL" retrieved in the HTML response. if yes then redirect to there 
                If returnValue.IndexOf("returnURL=") > -1 Then
                    Response.Redirect(returnValue.Replace("returnURL=", ""))
                Else
                    container.InnerHtml = returnValue
                End If
            Else
                btnSubmit_Click(sender, e)
            End If
        Catch ae As AggregateException
            HandleException(ae)
        End Try
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Dim form = Request.Form
        Dim keys As String() = form.AllKeys
        Dim data As List(Of DefaultValues) = New List(Of DefaultValues)
        Dim metaDataKey As String
        Dim currValueKey As String
        Dim currentValue As String
        Dim updatedValue As String
        Dim returnValue As String = String.Empty

        For Each key As String In keys
            metaDataKey = "hf_" & key
            If keys.Contains(metaDataKey) Then
                currValueKey = "hf_" & key & "_v"
                updatedValue = form(key)
                currentValue = form(currValueKey)
                data.Add(New DefaultValues With {.MetaData = form(metaDataKey), .CurrentValue = currentValue, .UpdatedValue = If(updatedValue = "true,false", "true", updatedValue)})
            End If
        Next

        returnValue = PostValues(API_SystemDefaults, data.ToArray())

        ' check to see if there is "returnURL" retrieved in the HTML response. if yes then redirect to there 
        If returnValue.IndexOf("returnURL=") > -1 Then
            Response.Redirect(returnValue.Replace("returnURL=", ""))
        Else
            container.InnerHtml = returnValue
        End If

    End Sub

    Protected Overrides Sub ShowErrorMessage(ByVal msg As String)
        blErrorMessages.Items.Add(msg)
        plhErrorList.Visible = True
    End Sub

End Class
