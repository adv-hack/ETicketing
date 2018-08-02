Imports Talent.eCommerce

Partial Class UserControls_TicketingLoyalty
    Inherits ControlBase

    Private myDefs As ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults
    Dim ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Private _controlIsActive As Boolean
    Public Property ControlIsActive() As Boolean
        Get
            Return _controlIsActive
        End Get
        Set(ByVal value As Boolean)
            _controlIsActive = value
        End Set
    End Property

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If myDefs.Loyalty_Points_In_Use Then
            Me.ControlIsActive = True
            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "TicketingLoyalty.ascx"
            End With
        Else
            Me.ControlIsActive = False
            Me.Visible = False
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.ControlIsActive Then
            'LoyaltyHeader.Text = ucr.Content("LoyaltyHeaderText", _languageCode, True)
            'LoyaltyHeader.NavigateUrl = ucr.Attribute("LoyaltyLinkDestinationUrl")
            If Profile.IsAnonymous Then
                'LoyaltyLink.Visible = False
                'LoyaltyImage.Visible = False
                LoyaltyLabel.Text = ucr.Content("LoyaltyLoggedOutLabelText", _languageCode, True)
            Else
                'LoyaltyLink.NavigateUrl = ucr.Attribute("LoyaltyLinkDestinationUrl")
                'LoyaltyImage.ImageUrl = ucr.Attribute("LoyaltyIconImageUrl")
                'LoyaltyImage.AlternateText = ""
                LoyaltyLabel.Text = ucr.Content("LoyaltyLoggedInLabelText", _languageCode, True).Replace("<LoyaltyPoints>", Profile.User.Details.Ticketing_Loyalty_Points)
                'If LoyaltyImage.ImageUrl = String.Empty Then
                    'LoyaltyImage.Visible = False
                'End If
            End If
        End If
    End Sub

End Class
