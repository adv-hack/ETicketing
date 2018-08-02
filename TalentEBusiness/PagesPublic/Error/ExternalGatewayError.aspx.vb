Imports Talent.eCommerce
Partial Class PagesPublic_Error_ExternalGatewayError
    Inherits TalentBase01

    Private wfr As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
            .KeyCode = "ExternalGatewayError.aspx"
        End With

        If Not String.IsNullOrWhiteSpace(Request.QueryString("gatewayname")) Then
            If Request.QueryString("gatewayname").ToUpper() = "PAYPAL" Then
                PayPalGatewayError()
            End If
        End If

    End Sub

    Private Sub PayPalGatewayError()
        plhExternalGatewayError.Visible = True
        ltlExtGatewayErrMess.Text = Utilities.CheckForDBNull_String(wfr.Content("PPErrorMessage", _languageCode, True))
    End Sub
End Class
