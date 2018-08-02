Imports Microsoft.VisualBasic
Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Conpetition Question Confirmation
'
'       Date                        Sept 2009
'
'       Author                      Craig Mcloughlin 
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base   
'                                    
'
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class PagesLogin_competitionQuestionsConfirmation
    Inherits TalentBase01

    Private wfr As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Try

            With wfr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = ProfileHelper.GetPageName
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "competitionQuestionsConfirmation.aspx"
            End With

            Dim def As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
            Dim defaults As New Talent.eCommerce.ECommerceModuleDefaults
            def = defaults.GetDefaults

            PageHeaderTextLabel.Text = wfr.Content("PageHeaderText", _languageCode, True)
            ConfirmationTextLabel.Text = wfr.Content("CompetitionConfirmationText", _languageCode, True)

        Catch ex As Exception

        End Try

    End Sub
End Class
