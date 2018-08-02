Imports Microsoft.VisualBasic
Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Pages Login - Contact Us
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      PLCONUS- 
'                                    
'       User Controls
'           contactUsDetails(5)
'
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class PagesPublic_catalogueRequestConfirmation
    Inherits TalentBase01
    Private wfr As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Request.QueryString("CatalogueRequest") IsNot Nothing _
            AndAlso Request.QueryString("CatalogueRequest") = "failed" Then
            DisplayFailedMessage()
        End If
    End Sub
    Private Sub DisplayFailedMessage()
        HTMLInclude1.Visible = False
        ltlFailedMessage.Visible = True
        With wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "catalogueRequestConfirmation.aspx"
            ltlFailedMessage.Text = .Content("FailedMessage", _languageCode, True)
        End With
    End Sub
End Class
