Imports Talent.eCommerce
Partial Class PagesPublic_Error_PrerequisiteMissing
    Inherits TalentBase01

    Private _wfr As Talent.Common.WebFormResource = Nothing
    Private _languageCode As String = String.Empty

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        _wfr = New Talent.Common.WebFormResource
        With _wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
            .KeyCode = "PrerequisiteMissing.aspx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'cookies=disabled
        If Request("cookies") IsNot Nothing AndAlso Request("cookies").ToUpper = "DISABLED" Then
            plhCookiesDisabled.Visible = True
            ltlCookiesDisabled.Visible = True
            ltlCookiesDisabled.Text = _wfr.Content("CookiesDisabledMessage", _languageCode, True)
        End If
        ltlJavascriptDisabled.Text = _wfr.Content("JavascriptDisabledMessage", _languageCode, True)
        LogRequestDetails()
    End Sub

    Private Sub LogRequestDetails()
        Try
            If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("LogClientDetails")) Then
                Dim sbServerVariableValues As New StringBuilder
                Dim arrServerVariables() As String = Talent.eCommerce.Utilities.CheckForDBNull_String(_wfr.Attribute("ServerVariablesToLog")).Trim.Split(New Char() {";"c}, System.StringSplitOptions.RemoveEmptyEntries)
                If arrServerVariables.Length > 0 Then
                    For arrIndex As Integer = 0 To (arrServerVariables.Length - 1)
                        sbServerVariableValues.Append(Request(arrServerVariables(arrIndex)) & ControlChars.Tab)
                    Next
                Else
                    For Each serverVariable As String In Request.ServerVariables
                        sbServerVariableValues.Append(Request(serverVariable) & ControlChars.Tab)
                    Next
                End If
                'now log it in text file
                Me.pageLogging.GeneralLog("PrerequisiteMissing.aspx", "LogRequestDetails", sbServerVariableValues.ToString, "PrerequisiteErrorPageLog")
            End If
        Catch ex As Exception

        End Try
    End Sub
End Class
