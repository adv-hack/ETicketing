
Partial Class Login_GeneratePasswordHash
    Inherits System.Web.UI.Page

    Private _wfrPage As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With _wfrPage
            .BusinessUnit = GlobalConstants.MAINTENANCEBUSINESSUNIT
            .PartnerCode = GlobalConstants.STARALLPARTNER
            .PageCode = "GeneratePasswordHash.aspx"
            .KeyCode = "GeneratePasswordHash.aspx"
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        End With
        ltlHomeLink.Text = _wfrPage.Content("HomeLink", _languageCode, True)
        Dim master As MasterPage1 = CType(Me.Master, MasterPage1)
        master.HeaderText = _wfrPage.Content("HeaderText", _languageCode, True)
    End Sub

    Protected Sub GoButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles GoButton.Click
        HashTitleLabel.Text = EncryptionTypeList.SelectedValue & " Hash"
        HashLabel.Text = FormsAuthentication.HashPasswordForStoringInConfigFile(PasswordBox.Text, EncryptionTypeList.SelectedValue)
    End Sub
End Class
