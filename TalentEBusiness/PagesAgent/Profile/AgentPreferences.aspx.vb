
Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities

Partial Class PagesAgent_Profile_AgentPreferences
    Inherits TalentBase01

    Protected Sub Page_Init1(sender As Object, e As EventArgs) Handles Me.Init
        uscAgentPreferences.AgentName = AgentProfile.Name
        If Not Session("NewAgent") Is Nothing Then
            uscAgentPreferences.AgentName = Session("NewAgent")
        End If
    End Sub
End Class
