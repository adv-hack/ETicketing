Imports Talent.Common
Imports System.Data

Partial Class PagesLogin_Checkout_AdditionalProductInformation
    Inherits TalentBase01
    Private settings As Talent.Common.DESettings
    Public wfrPage As New Talent.Common.WebFormResource
    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        settings = New Talent.Common.DESettings()
        'With settings
        '    .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        '    .DestinationDatabase = "SQL2005"
        '    .BusinessUnit = TalentCache.GetBusinessUnit
        '    .Cacheing = False
        'End With
        With wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = String.Empty
            'added for testing should be removed once Talent.common is updated
            .FrontEndConnectionString = settings.FrontEndConnectionString
            .PartnerCode = "*ALL"
            .KeyCode = "AdditionalProductInformation.aspx"
            .PageCode = "AdditionalProductInformation.aspx"
        End With
    End Sub
End Class
