Imports System.Data
Imports Talent.eCommerce
Imports Talent.Common

Partial Class UserControls_HospitalityFilter
    Inherits ControlBase

#Region "Class Level Fields"
    Private _ucr As New Talent.Common.UserControlResource
    Private _log As Talent.Common.TalentLogging = Talent.eCommerce.Utilities.TalentLogging
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "HospitalityFilter.ascx"
        End With
    End Sub

#End Region

End Class
