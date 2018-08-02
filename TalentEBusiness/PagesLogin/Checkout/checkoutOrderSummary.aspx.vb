Imports Microsoft.VisualBasic
Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Pages Login Checkout Order Summary
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      PLCOS- 
'
'       User Controls
'           orderSummary
'           basketSummary
'           deliveryDetailsSummary
'
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class PagesLogin_checkoutOrderSummary
    Inherits TalentBase01

    Private wfr As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Checkout.CheckBasketValidity Then

        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        With wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "CheckoutOrderSummary.aspx"
            If Not Page.IsPostBack Then
                InstructionsLabel.Text = .Content("InstructionsLabelText", _languageCode, True)
            End If
        End With
    End Sub

End Class
