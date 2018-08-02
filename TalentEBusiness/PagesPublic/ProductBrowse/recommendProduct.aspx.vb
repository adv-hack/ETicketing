Imports Microsoft.VisualBasic
Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Pages Public - Recommend Product  
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      PPREPRO- 
' 
'       User Controls
'           recommendAProductDetails
'
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class PagesPublic_recommendProduct
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
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                .KeyCode = "RecommendProduct.aspx"

                MessageText.Text = .Content("MessageText", _languageCode, True)
            End With
        End If
    End Sub
    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "RecommendProduct.ascx"
            '
            Select Case sender.ID.ToString
                Case Is = "ProductLink"
                    CType(sender, HyperLink).Text = .Content("ProductLinkText", _languageCode, True)
                    CType(sender, HyperLink).NavigateUrl = "~/PagesPublic/ProductBrowse/Product.aspx" & Request.Url.Query
            End Select
        End With
    End Sub
End Class
