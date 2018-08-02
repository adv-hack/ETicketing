Imports System.IO
Imports TalentSystemDefaults.DataEntities
Imports System.Data.SqlClient
Imports TalentSystemDefaults.DataAccess.ConfigObjects
Imports TalentSystemDefaults


Partial Class UpdateConfiguration
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        plhNewConfigId.Visible = False
        lblErrorMessage.Text = String.Empty

        Dim configId As String = txtConfigId.Text
        If (String.IsNullOrEmpty(configId)) Then
            lblErrorMessage.Text = "Enter configuration ID"
        Else
            Dim settings As DESettings = GetDESettings()
            Dim configDetail As New tbl_config_detail(settings)
            Dim newConfigId As String = configDetail.UpdateConfigId(configId)
            If (Not String.IsNullOrEmpty(newConfigId)) Then
                lblNewConfigId.Text = newConfigId
                plhNewConfigId.Visible = True
            Else
                lblErrorMessage.Text = "Configuration ID not found"
            End If
        End If
    End Sub

    Private Function GetDESettings() As DESettings
        Dim settings As New DESettings
        With settings
            .BusinessUnit = ConfigurationManager.AppSettings("DefaultBusinessUnit")
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .DestinationDatabase = DestinationDatabase.SQL2005.ToString
        End With

        Return settings
    End Function
End Class
