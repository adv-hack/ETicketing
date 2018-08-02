Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities

Partial Class PagesLogin_FriendsAndFamily
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _wfrPage As WebFormResource = Nothing

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If (AgentProfile.IsAgent And AgentProfile.AgentPermissions.CanAccessFriendsAndFamily) Or Not AgentProfile.IsAgent Then
            _wfrPage = New WebFormResource
            With _wfrPage
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = String.Empty
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = Talent.eCommerce.Utilities.GetCurrentPageName
                .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
            End With
        Else
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim myTicketingMenu As New TalentTicketingMenu
        myTicketingMenu.LoadTicketingProducts(TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), Talent.eCommerce.Utilities.GetCurrentLanguage)
        If myTicketingMenu.TicketingProductIsActive("FRIENDSFAMILY") Then
            'Check defaults to see if we need to perform a check to see if the current user 
            'is allowed to access the F&F options based on their age. Otherwise display as normal.
            If ModuleDefaults.CheckUserCanAccessFandF Then
                If canCurrentUserCanAccessFandF() Then
                    ErrorLabel.Text = ""
                    FriendsAndFamilyOptions1.Visible = True
                    FriendsAndFamilyDetails1.Visible = True
                Else
                    ErrorLabel.Text = _wfrPage.Content("ErrorMessageUserCannotUseFandF", _languageCode, True)
                    FriendsAndFamilyOptions1.Visible = False
                    FriendsAndFamilyDetails1.Visible = False
                End If
            Else
                ErrorLabel.Text = ""
                FriendsAndFamilyOptions1.Visible = True
                FriendsAndFamilyDetails1.Visible = True
            End If
        Else
            ErrorLabel.Text = _wfrPage.Content("ErrorMessageUnavailable", _languageCode, True)
            FriendsAndFamilyOptions1.Visible = False
            FriendsAndFamilyDetails1.Visible = False
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
        plhErrorMessage.Visible = (ErrorLabel.Text.Length > 0)
    End Sub

#End Region

#Region "Private Functions"

    Private Function canCurrentUserCanAccessFandF() As Boolean
        Dim userAllowedAccess As Boolean = False
        Dim ageBreakDefaultValue As String = ModuleDefaults.AgeAtWhichIDFieldsAreMandatory
        Dim ageBreakNumericValue As Integer = 0
        Try
            Dim currentUserDateOfBirth As Date = Profile.User.Details.DOB
            If Not String.IsNullOrEmpty(ageBreakDefaultValue) AndAlso ageBreakDefaultValue >= 0 Then
                ageBreakNumericValue = CType(ageBreakDefaultValue, Integer)
                If (Date.Now.AddYears(-ageBreakNumericValue)) > currentUserDateOfBirth Then
                    userAllowedAccess = True
                Else
                    userAllowedAccess = False
                End If
            Else
                userAllowedAccess = False
            End If
        Catch ex As Exception
            userAllowedAccess = False
        End Try
        Return userAllowedAccess
    End Function

#End Region

End Class