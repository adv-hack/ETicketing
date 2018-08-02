Imports Microsoft.VisualBasic
Imports Talent.eCommerce

Partial Class UserControls_CompanyLogo
    Inherits ControlBase

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        Dim ucr As New Talent.Common.UserControlResource
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
            .KeyCode = "CompanyLogo.ascx"
            .PageCode = Talent.Common.Utilities.GetAllString
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)

            Dim strImage As String = String.Empty
            If Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(.Attribute("checkLogoExists")) Then
                strImage = ImagePath.getImagePath("APPTHEME", .Attribute("companyLogo"), TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
            End If
            If strImage.Trim = String.Empty Or strImage.Trim = ModuleDefaults.MissingImagePath.ToString.Trim Then
                strImage = .Attribute("companyLogo")
            End If
            logoImage.ImageUrl = strImage


            Dim def As ECommerceModuleDefaults.DefaultValues = New ECommerceModuleDefaults().GetDefaults()
            If Profile.IsAnonymous Then
                logoHyperlink.NavigateUrl = def.ApplicationStartupPage
            Else
                logoHyperlink.NavigateUrl = def.PAGE_AFTER_LOGIN
            End If
        End With
    End Sub

End Class