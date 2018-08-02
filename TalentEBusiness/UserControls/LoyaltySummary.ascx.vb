'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Loyalty Summary
'
'       Date                        Jan 2008
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      UCLOYS- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_Loyalty
    Inherits ControlBase

    Dim ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    'Protected Sub BasketLinkButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BasketLinkButton.Click
    '   Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
    'End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "LoyaltySummary.ascx"
        End With
    End Sub

    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)

        Dim obj As Object = sender
        Select Case obj.ID.ToString
            Case Is = "TotalLabel"
                obj.Text = ucr.Content("TotalLabelText", _languageCode, True)
            Case Is = "Total"
                If Profile.IsAnonymous Then
                    obj.Text = "login"
                Else
                    obj.Text = Profile.User.Details.User_Number
                End If
            Case Is = "LoyaltyHeaderLabel"
                obj.Text = "<h2 class='loyalty'>" & ucr.Content("LoyaltyHeaderLabelText", _languageCode, True) & "</h2>"
        End Select
    End Sub


End Class
