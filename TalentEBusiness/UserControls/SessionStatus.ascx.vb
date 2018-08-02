Imports Talent.eCommerce

Partial Class UserControls_SessionStatus
    Inherits ControlBase

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Dim ucr As New Talent.Common.UserControlResource

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "SessionStatus.ascx"
        End With

        '----------------------------
        ' Don't show if in Agent mode
        '----------------------------
        If Not Session.Item("NoiseSessionStartTime") Is Nothing AndAlso Session("Agent") Is Nothing Then
            Dim mydefs As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults
            If mydefs.NOISE_MAX_SESSION_KEEP_ALIVE_MINUTES > 0 Then
                plhSeassionStatus.visible = True
                Dim startTime As DateTime = CType(Session.Item("NoiseSessionStartTime"), DateTime)
                Dim myTime As DateTime = Now
                Dim endTime As DateTime = startTime.AddMinutes(mydefs.NOISE_MAX_SESSION_KEEP_ALIVE_MINUTES)

                Dim remaining As Double = 0

                If endTime > myTime Then
                    remaining = endTime.Subtract(myTime).TotalMinutes()
                Else
                    remaining = -myTime.Subtract(endTime).TotalMinutes()
                End If

                statusHeader.Text = ucr.Content("SessionStatusHeaderText", _languageCode, True)

                Dim currentPageName As String = Talent.eCommerce.Utilities.GetCurrentPageName
                If LCase(currentPageName) = LCase("checkout.aspx") _
                    OrElse LCase(currentPageName) = LCase("checkoutorderconfirmation.aspx") Then
                    remaining += mydefs.NOISE_MAX_SESSION_CHECKOUT_ADD_MINUTES
                    statusContent.Text = ucr.Content("SessionStatusCheckoutContentText", _languageCode, True).Replace("<<RemainingMinutes>>", remaining.ToString("######"))
                Else
                    statusContent.Text = ucr.Content("SessionStatusContentText", _languageCode, True).Replace("<<RemainingMinutes>>", remaining.ToString("######"))
                End If
            Else
                plhSeassionStatus.Visible = False
                Me.Visible = False
            End If
        Else
            plhSeassionStatus.Visible = False
            Me.Visible = False
        End If

    End Sub
End Class
