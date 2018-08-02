Imports System.Data
Imports TC = Talent.Common
Imports TEB = Talent.eCommerce

Partial Class UserControls_UserAlerts
    Inherits ControlBase

#Region "Class Level Fields"

    Private _ucr As New TC.UserControlResource
    Private _defaults As TEB.ECommerceModuleDefaults.DefaultValues
    Private _languageCode As String = TC.Utilities.GetDefaultLanguage

    Private Const SQLSERVER2005 As String = "SqlServer2005"
    Private Const KEYCODE As String = "UserAlerts.ascx"

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings(SQLSERVER2005).ConnectionString
            .KeyCode = KEYCODE
            .PageCode = TEB.Utilities.GetCurrentPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim moduleDefaults As New TEB.ECommerceModuleDefaults
        _defaults = moduleDefaults.GetDefaults
        If _defaults.AlertsEnabled And Not Profile.IsAnonymous Then
            plhUserAlerts.Visible = True
            TEB.Utilities.GetNumberOfUnreadAlerts()
            Dim numberOfUnreadAlerts As Integer = Session("NumberOfUnreadAlerts")
            Dim redirectQSResult As String = HttpContext.Current.Request.QueryString("reInd")
            Dim navigationLink As String = ResolveUrl("~/PagesLogin/Profile/Alerts.aspx")
            Dim unauthorisedMsgResult As String = HttpContext.Current.Request.QueryString("unauthorisedMsg")
            hlkAlertsRedirect.Visible = True

            If String.IsNullOrWhiteSpace(redirectQSResult) Then
                If String.IsNullOrWhiteSpace(unauthorisedMsgResult) Then

                    'standard alert
                    If numberOfUnreadAlerts = 0 Then
                        plhUserAlerts.Visible = False
                    ElseIf numberOfUnreadAlerts = 1 Then
                        ltlAlertsText02.Text = _ucr.Content("SingleAlertText", _languageCode, True)
                    Else
                        ltlAlertsText02.Text = _ucr.Content("MultipleAlertText", _languageCode, True)
                    End If
                    ltlAlertsText02.Text = ltlAlertsText02.Text.Replace("<<NUMBER_OF_ALERTS>>", numberOfUnreadAlerts)

                    hlkAlertsRedirect.NavigateUrl = navigationLink

                    If Session("CanShowAlertOnPageLoad") Is Nothing AndAlso Not AgentProfile.IsAgent Then
                        ltlJavaScriptString.Visible = True
                        ltlJavaScriptString.Text = _ucr.Attribute("JavaScriptString")
                        ltlJavaScriptString.Text = ltlJavaScriptString.Text.Replace("<<JS_FUNCTION_NAME>>", "function(){OpenAlertsWindow(""" & navigationLink & """);}")
                        Session("CanShowAlertOnPageLoad") = "FALSE"
                    Else
                        ltlJavaScriptString.Text = ""
                        ltlJavaScriptString.Visible = False
                    End If
                Else

                    'restriction unauthorised message alert
                    ltlAlertsText02.Text = _ucr.Content("RestrictiontMessageText", _languageCode, True)

                    navigationLink = navigationLink & "?unauthorisedMsg=" & unauthorisedMsgResult
                    hlkAlertsRedirect.NavigateUrl = navigationLink
                    If Session("RestrictedRedirectShowShadowBox") = True AndAlso Not AgentProfile.IsAgent Then
                        ltlJavaScriptString.Visible = True
                        ltlJavaScriptString.Text = _ucr.Attribute("JavaScriptString")
                        ltlJavaScriptString.Text = ltlJavaScriptString.Text.Replace("<<JS_FUNCTION_NAME>>", "function(){OpenAlertsWindow(""" & navigationLink & """);}")
                        Session("RestrictedRedirectShowShadowBox") = False
                    Else
                        ltlJavaScriptString.Text = ""
                        ltlJavaScriptString.Visible = False
                    End If
                End If
            Else

                'redirect alert with redirect information
                ltlAlertsText02.Text = _ucr.Content("RedirectMessageText", _languageCode, True)

                navigationLink = navigationLink & "?reInd=" & redirectQSResult
                hlkAlertsRedirect.NavigateUrl = navigationLink

                If Session("RedirectShowShadowBox") = True AndAlso Not AgentProfile.IsAgent Then
                    ltlJavaScriptString.Visible = True
                    ltlJavaScriptString.Text = _ucr.Attribute("JavaScriptString")
                    ltlJavaScriptString.Text = ltlJavaScriptString.Text.Replace("<<JS_FUNCTION_NAME>>", "function(){OpenAlertsWindow(""" & navigationLink & """);}")
                    Session("RedirectShowShadowBox") = False
                Else
                    ltlJavaScriptString.Text = ""
                    ltlJavaScriptString.Visible = False
                End If
            End If
        Else
            plhUserAlerts.Visible = False
        End If
    End Sub

    

#End Region

End Class
