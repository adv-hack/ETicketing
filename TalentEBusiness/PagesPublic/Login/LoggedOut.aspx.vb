
Partial Class PagesPublic_Login_LoggedOut
    Inherits TalentBase01

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If ModuleDefaults.NOISE_IN_USE AndAlso ModuleDefaults.NOISE_CLEAR_SESSION_ON_LOGOUT Then
            Dim noiseSettings As Talent.Common.DESettings = Talent.eCommerce.Utilities.GetSettingsObject
            Dim myNoise As New Talent.Common.TalentNoise(noiseSettings, _
                                                        Session.SessionID, _
                                                        Now, _
                                                        Now.AddMinutes(-ModuleDefaults.NOISE_THRESHOLD_MINUTES), _
                                                        ModuleDefaults.NOISE_MAX_SESSION_KEEP_ALIVE_MINUTES)
            myNoise.RemoveSpecificNoiseSession()
        End If
        Profile.CustomerLogout("")
    End Sub
End Class
