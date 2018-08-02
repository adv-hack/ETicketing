Imports System.Data
Imports Talent.Common.DataObjects
Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class PagesAgent_Admin_AgentCopy
    Inherits TalentBase01

#Region "Constants"
    Const KEYCODE As String = "AgentCopy.aspx"
#End Region

#Region "Class Level Fields"

    Private _wfrPage As WebFormResource = Nothing
    Private _languageCode As String = String.Empty
    Private _agentCopyFailed As Boolean = False
    Private _errMessage As TalentErrorMessages = Nothing

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If AgentProfile.AgentPermissions.IsAdministrator Then
            _wfrPage = New WebFormResource
            _languageCode = TCUtilities.GetDefaultLanguage()
            With _wfrPage
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = ProfileHelper.GetPageName
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = KEYCODE
            End With
            plhAgentCopyErrorMessage.Visible = False
        Else
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        lblAgentCopy.Text = _wfrPage.Content("lblAgentCopy", _languageCode, True)
        If Not IsPostBack Then
            uscAgentList.LoadAgents()
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        If _agentCopyFailed Then
            plhAgentCopyErrorMessage.Visible = True
            agentCopyControls.Visible = True
        Else
            plhAgentCopyErrorMessage.Visible = False
        End If
    End Sub
    Protected Sub btnCopySubmit_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim err As ErrorObj
        Dim agentDataEntity As New DEAgent
        plhAgentCopyErrorMessage.Visible = False
        agentDataEntity.AgentUsername = CType(uscAgentList.FindControl("ddlAgents"), DropDownList).SelectedValue
        agentDataEntity.NewAgent = txtNewAgentID.Text.ToUpper
        agentDataEntity.NewAgentPassword = txtNewAgentPassword.Text
        agentDataEntity.NewAgentDescription = txtNewAgentDescription.Text
        Dim talAgent As New TalentAgent()
        agentDataEntity.Source = GlobalConstants.SOURCE
        Dim settings As DESettings = TEBUtilities.GetSettingsObjectForAgent()
        talAgent.AgentDataEntity = agentDataEntity
        talAgent.Settings = settings
        talAgent.Settings.DestinationDatabase = "TALENTTKT"
        _errMessage = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnit, TalentCache.GetDefaultPartner(), _wfrPage.FrontEndConnectionString)
        err = talAgent.AgentCopy()
        If err.HasError Then
            _agentCopyFailed = True
            plhAgentCopyErrorMessage.Visible = True
            ltlAgentCopyErrorMessage.Text = _wfrPage.Content("AgentCopyError", _languageCode, True).Replace("<<ERROR_MESSAGE>>", err.ErrorMessage)
        ElseIf talAgent.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
            _agentCopyFailed = True
            plhAgentCopyErrorMessage.Visible = True
            ltlAgentCopyErrorMessage.Text = _errMessage.GetErrorMessage(talAgent.ResultDataSet.Tables("ErrorStatus").Rows(0).Item("ReturnCode")).ERROR_MESSAGE
        Else
            talAgent.RetrieveAllAgentsClearCache()
            Session("AgentCopySuccess") = _wfrPage.Content("AgentCopySuccess", _languageCode, True)
            Session("NewAgent") = txtNewAgentID.Text.ToUpper
            Response.Redirect("~/PagesAgent/Profile/AgentPreferences.aspx")
        End If
    End Sub
#End Region
End Class
