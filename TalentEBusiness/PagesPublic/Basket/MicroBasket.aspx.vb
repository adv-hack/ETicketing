Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class PagesPublic_Basket_MicroBasket
    Inherits TalentBase01

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim wfrPage As New WebFormResource
        With wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "MicroBasket.aspx"
        End With
        If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(wfrPage.Attribute("ShowPersonalisation")) Then
            plhPersonalisation.Visible = True
            Dim personalisationContent As String = String.Empty
            If Profile.IsAnonymous Then
                personalisationContent = wfrPage.Content("AnonymousContent", TCUtilities.GetDefaultLanguage(), True)
            Else
                personalisationContent = wfrPage.Content("PersonalisationContent", TCUtilities.GetDefaultLanguage(), True)
                personalisationContent = personalisationContent.Replace("<<CUSTOMER_TITLE>>", Profile.User.Details.Title)
                personalisationContent = personalisationContent.Replace("<<CUSTOMER_FORENAME>>", Profile.User.Details.Forename)
                personalisationContent = personalisationContent.Replace("<<CUSTOMER_SURNAME>>", Profile.User.Details.Surname)
            End If
            ltlPersonalisation.Text = personalisationContent
        Else
            plhPersonalisation.Visible = False
        End If
    End Sub

End Class