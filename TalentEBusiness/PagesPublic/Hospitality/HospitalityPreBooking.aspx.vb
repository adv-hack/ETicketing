Imports Talent.Common
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class PagesPublic_Hospitality_HospitalityPreBooking
    Inherits TalentBase01


#Region "Class Level Fields"
    Private _wfrPage As New WebFormResource
    Private _templateId As String
#End Region

#Region "Public Properties"

#End Region

#Region "Protected Methods"
    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        _templateId = TEBUtilities.CheckForDBNull_Int(Request.Params("TemplateId"))
        With _wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = Talent.eCommerce.Utilities.GetCurrentPageName
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Profile.IsAnonymous Then
            If AgentProfile.IsAgent Then
                Response.Redirect("~/PagesPublic/Profile/CustomerSelection.aspx?returnurl=" + Server.UrlEncode("~/PagesPublic/Hospitality/HospitalityPreBooking.aspx?templateId=") & _templateId)
            Else
                Response.Redirect("~/PagesPublic/Login/login.aspx?ReturnUrl=~/PagesPublic/Hospitality/HospitalityPreBooking.aspx?templateId=" & _templateId)
            End If
        Else
            'Only the questions are visible for the PWS site
            If Not AgentProfile.IsAgent Then
                uscEditProfileActivity.HeaderVisible = False
                uscEditProfileActivity.TemplateQuestionsVisible = True
                uscEditProfileActivity.ActivityAgent = GlobalConstants.BACKEND_PWS_AGENT
            Else
                uscEditProfileActivity.ActivityAgent = AgentProfile.Name
            End If

            uscEditProfileActivity.CommentsVisible = False
            uscEditProfileActivity.AttachmentsVisible = False
            uscEditProfileActivity.ActivityDate = DateAndTime.Today
            uscEditProfileActivity.ActivitySubject = _wfrPage.Content("HospitalityPreBookingActivitySubject", Talent.Common.Utilities.GetDefaultLanguage, True)
            uscEditProfileActivity.ActivitySubject = uscEditProfileActivity.ActivitySubject.Replace("<<ProductCode>>", Session("HospitalityPreBookingProductCode"))
            uscEditProfileActivity.ActivitySubject = uscEditProfileActivity.ActivitySubject.Replace("<<ProductDescription>>", Session("HospitalityPreBookingProductDescription"))
            uscEditProfileActivity.ActivitySubject = uscEditProfileActivity.ActivitySubject.Replace("<<PackageCode>>", Session("HospitalityPreBookingPackageCode"))
            uscEditProfileActivity.ActivitySubject = uscEditProfileActivity.ActivitySubject.Replace("<<PackageDescription>>", Session("HospitalityPreBookingPackageDescription"))
            uscEditProfileActivity.ActivityStatus = _wfrPage.Content("HospitalityPreBookingActivityStatus", Talent.Common.Utilities.GetDefaultLanguage, True)
            uscEditProfileActivity.Usage = "HOSPITALITYDATACAPTURE"
        End If
    End Sub

#End Region

End Class
