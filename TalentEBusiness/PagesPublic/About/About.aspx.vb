
Partial Class PagesPublic_About_About
    Inherits TalentBase01

    Private wfr As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Page.IsPostBack Then
            setupUCR()
            loadLabels()
        End If
    End Sub

    Sub setupUCR()
        With wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "About.aspx"
        End With
    End Sub

    Sub loadLabels()
        AboutPageLabel.Text = wfr.Content("AboutPageText", _languageCode, True)
        Dim dt As Data.DataTable = TDataObjects.VersionDeployed.TblVersionDeployed.GetDeployVersion()
        If dt.Rows.Count > 0 Then
            ProcessTypeLabel.Text = Talent.eCommerce.Utilities.CheckForDBNull_String(dt.Rows(0).Item("PROCESSTYPE"))
            VersionLabel.Text = Talent.eCommerce.Utilities.CheckForDBNull_String(dt.Rows(0).Item("VERSION"))
            UpgradeDateLabel.Text = Talent.eCommerce.Utilities.CheckForDBNull_String(dt.Rows(0).Item("DATEUPDATED"))
        End If
    End Sub

End Class
