Imports Talent.eCommerce

Partial Class PagesPublic_changePassword
    Inherits TalentBase01

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Dim wfr As New Talent.Common.WebFormResource
        'Dim _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
        'With wfr
        '    .BusinessUnit = TalentCache.GetBusinessUnit()
        '    .PageCode = ProfileHelper.GetPageName
        '    .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
        '    .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        '    .KeyCode = "changePassword.aspx"
        'End With
        'uscChangePassword.Visible = False
        'uscChangePassword.CurrentPasswordBox.Visible = False
        'uscChangePassword.lblCurrentPasswordLabel.Visible = False
        'MessageLabel.Text = wfr.Content("ExpiredLoginKey", _languageCode, True)

        '' If MyBase.ModuleDefaults.useEncryptedPassword Then
        '' Check for query string
        'If Not Request("LoginKey") Is Nothing Then
        '    '
        '    ' Retrieve the login record from the database
        '    Dim dt As Data.DataTable = Me.TDataObjects.ProfileSettings.tblForgottenPassword.GetByHashedToken(Request("LoginKey"))
        '    If dt.Rows.Count = 1 Then
        '        Dim currentTime As DateTime
        '        Dim timestamp As DateTime
        '        Dim usedTime As DateTime
        '        currentTime = DateTime.Now
        '        timestamp = CDate(dt.Rows.Item(0).ItemArray(5))
        '        usedTime = CDate(dt.Rows.Item(0).ItemArray(6))
        '        '
        '        'Valid Key found so control is available to the user.
        '        uscChangePassword.LoginID = dt.Rows.Item(0).ItemArray(3)
        '        uscChangePassword.DoAutoLogin = True
        '        '
        '        ' Only used the key if it not already used.
        '        If usedTime = New Date(1900, 1, 1) Then

        '            Dim expiryTime As Integer = Me.ModuleDefaults.ForgottenPasswordExpiryTime
        '            If expiryTime = 0 Then
        '                expiryTime = 5
        '            End If
        '            '
        '            'Check that the timestamp is within the timeout period
        '            If currentTime < timestamp.AddMinutes(expiryTime) Then
        '                uscChangePassword.Visible = True
        '                MessageLabel.Text = wfr.Content("InstructionText", _languageCode, True)
        '            End If
        '        End If
        '    End If
        'End If
        'plhMessage.Visible = (MessageLabel.Text.Length > 0)
    End Sub
End Class
