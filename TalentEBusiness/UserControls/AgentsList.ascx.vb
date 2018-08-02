Imports System.Data
Imports Talent.eCommerce
Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities
Partial Class UserControls_AgentsList
    Inherits ControlBase

#Region "Class Level Fields"

    Private _languageCode As String = String.Empty
    Private _errMsg As TalentErrorMessages
    Private _errMessage As TalentErrorMessages = Nothing
    Private _ucr As UserControlResource = Nothing

#End Region

#Region "Constants"
    Const KEYCODE As String = "AgentsList.ascx"
#End Region
    Public Sub LoadAgents()
        Dim talAgent As New TalentAgent
        Dim err As New ErrorObj
        Dim agents As New DataTable
        Dim agentDataEntity As New DEAgent

        talAgent.Settings = TEBUtilities.GetSettingsObject()
        agentDataEntity.Source = GlobalConstants.SOURCE
        talAgent.AgentDataEntity = agentDataEntity
        err = talAgent.RetrieveAllAgents()

        If Not err.HasError AndAlso talAgent.ResultDataSet IsNot Nothing AndAlso talAgent.ResultDataSet.Tables("ErrorStatus").Rows.Count > 0 Then
            If talAgent.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                'Dim talentErrorMsg As TalentErrorMessage = _errMessage.GetErrorMessage(talAgent.ResultDataSet.Tables("ErrorStatus").Rows(0)("ReturnCode"))
            Else
                agents = talAgent.ResultDataSet.Tables("AgentUsers")
                ddlAgents.DataSource = agents
                ddlAgents.DataTextField = "USERNAME"
                ddlAgents.DataValueField = "USERCODE"
                ddlAgents.DataBind()
                ddlAgents.Items.Add(New ListItem(_ucr.Content("selectAgentText", _languageCode, True), "none"))
                ddlAgents.SelectedValue = AgentProfile.Name
            End If
        End If
    End Sub

    Private Sub UserControls_Agents_Init(sender As Object, e As EventArgs) Handles Me.Init
        _ucr = New UserControlResource
        _languageCode = TCUtilities.GetDefaultLanguage()
        With _ucr
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = KEYCODE
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
        End With
    End Sub
    Private Sub UserControls_Agents_Load(sender As Object, e As EventArgs) Handles Me.Load
        lblAgent.Text = _ucr.Content("lblAgent", _languageCode, False)
    End Sub
End Class
