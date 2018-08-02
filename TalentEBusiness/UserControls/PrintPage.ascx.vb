
Partial Class UserControls_PrintPage
    Inherits ControlBase

    Protected Function getText() As String
        Return ucr.Content("LinkText", _languageCode, True)
    End Function

    Private ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "PrintPage.ascx"
        End With
    End Sub

End Class
