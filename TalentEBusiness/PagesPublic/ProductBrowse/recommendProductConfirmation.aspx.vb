Imports Microsoft.VisualBasic
Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Pages Public - Recommend Product Confirmation
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      PPREPRCO- 
' 
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class PagesPublic_recommendProductConfirmation
    Inherits TalentBase01

    Private wfr As New Talent.Common.WebFormResource
    Private ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Not Page.IsPostBack Then
            With wfr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = ProfileHelper.GetPageName
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "RecommendProductConfirmation.ascx"

                MessageText.Text = .Content("MessageText", _languageCode, True)
                ' HyperLink1.NavigateUrl = .Content("HyperLink1NavigateUrl", _L.CID, True)
                ' HyperLink1.Text = .Content("HyperLink1NavigateUrlText", _L.CID, True)

            End With
        End If
    End Sub

    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "RecommendProductConfirmation.ascx"
            '
            Select Case sender.ID.ToString
                Case Is = "HomeLink"
                    CType(sender, HyperLink).Text = .Content("HomeLinkText", _languageCode, True)
                    CType(sender, HyperLink).NavigateUrl = .Content("HomeLinkUrl", _languageCode, True)
            End Select
        End With
    End Sub

End Class
