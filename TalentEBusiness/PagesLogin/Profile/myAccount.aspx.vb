Imports Talent.eCommerce

Partial Class PagesLogin_myAccount
    Inherits TalentBase01

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim wfr As New Talent.Common.WebFormResource
        Dim languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
        Dim errMsg As New Talent.Common.TalentErrorMessages(languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)

        With wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "myAccount.aspx"
            bodyLabel1.Text = .Content("bodyText", languageCode, True)
        End With

        If Not String.IsNullOrEmpty(Request.QueryString("error")) AndAlso Not String.IsNullOrEmpty(Request.QueryString("status")) Then
            Select Case Request.QueryString("error").ToLower
                Case "cc"
                    errorList.Items.Add(errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, Talent.eCommerce.Utilities.GetCurrentPageName, "CC" & Request.QueryString("status")).ERROR_MESSAGE)
            End Select
        End If
        plhErrorList.Visible = (errorList.Items.Count > 0)
    End Sub
    
End Class
