Imports PageAddHeadersAndTitlesDataset
Imports System.Data
Imports Talent.Common

Partial Class PagesMaint_PageAddHeadersAndTitles
    Inherits System.Web.UI.Page

#Region "Class Level Fields"

    Private _PgAddAdp As New PageAddHeadersAndTitlesDatasetTableAdapters.LangPageTableAdapter
    Private _wfrPage As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _QSBusinessUnit As String = String.Empty
    Private _QSPartner As String = String.Empty
    Private _QSPageCode As String = String.Empty
    Private _QSPageId As String = String.Empty
    Private _QSKey As String = String.Empty
    Private _QSMode As String = String.Empty

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _QSBusinessUnit = Request.QueryString("BusinessUnit")
        _QSPartner = Request.QueryString("Partner")
        _QSPageCode = Request.QueryString("PageName")
        _QSPageId = Request.QueryString("PageId")
        _QSKey = Request.QueryString("ID")
        _QSMode = Request.QueryString("Mode")

        With _wfrPage
            .BusinessUnit = GlobalConstants.MAINTENANCEBUSINESSUNIT
            .PageCode = String.Empty
            .PartnerCode = GlobalConstants.STARALLPARTNER
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "PageAddHeadersAndTitles.aspx"
            .PageCode = "PageAddHeadersAndTitles.aspx"
        End With

        If String.IsNullOrEmpty(_QSBusinessUnit) Or String.IsNullOrEmpty(_QSPartner) Or String.IsNullOrEmpty(_QSPageCode) Or String.IsNullOrEmpty(_QSPageId) Or String.IsNullOrEmpty(_QSMode) Then
            Response.Redirect("../MaintenancePortal.aspx")
        End If
        If _QSMode = "Add" Then
            SearchDataSource1.SelectMethod = "GetLanguageNotInData"
        Else
            SearchDataSource1.SelectMethod = "GetLanguageInData"
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then
            setLabel()
        End If
    End Sub

    Protected Sub ConfirmButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ConfirmButton.Click
        Dim PLanguage As String = DropDownLanguage.SelectedValue
        If _QSMode = "Add" Then
            Dim ans As Integer = _PgAddAdp.InsertLang(_QSBusinessUnit, _QSPartner, _QSPageCode, PLanguage, TextPageTitle.Text, TextMetaKeywords.Text, TextMetaDescription.Text, TextPageHeader.Text)
            If ans > 0 Then
                Response.Write(_wfrPage.Content("SuccAdd", _languageCode, True))
            Else
                Response.Write(_wfrPage.Content("UnScc", _languageCode, True))
            End If
        Else
            Dim ans As Integer = _PgAddAdp.UpdateLang(TextPageTitle.Text, TextMetaKeywords.Text, TextMetaDescription.Text, TextPageHeader.Text, _QSKey)
            If ans > 0 Then
                Response.Write(_wfrPage.Content("SuccUpdate", _languageCode, True))
            Else
                Response.Write(_wfrPage.Content("UnScc", _languageCode, True))
            End If
        End If
        Response.Redirect("PageHeadersAndTitles.aspx?PageName=" + _QSPageCode + "&PageId=" + _QSPageId + "&Partner=" + _QSPartner + "&BusinessUnit=" + _QSBusinessUnit)
    End Sub

    Protected Sub CancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CancelButton.Click
        Response.Redirect("PageHeadersAndTitles.aspx?PageName=" + _QSPageCode + "&PageId=" + _QSPageId + "&Partner=" + _QSPartner + "&BusinessUnit=" + _QSBusinessUnit)
    End Sub

    Protected Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
        If _QSMode = "Add" And DropDownLanguage.Items.Count = 0 Then
            Response.Write(_wfrPage.Content("LngAlready", _languageCode, True))
            Response.Redirect("PageHeadersAndTitles.aspx?PageName=" + _QSPageCode + "&PageId=" + _QSPageId + "&Partner=" + _QSPartner + "&BusinessUnit=" + _QSBusinessUnit)
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub setLabel()
        If _QSMode = "Add" Then
            TitleLabel.Text = "Add "
        Else
            TitleLabel.Text = "Change "
        End If
        TitleLabel.Text = TitleLabel.Text + "Page Header and Title: " ' Page Name
        instructionsLabel.Text = _wfrPage.Content("instructionsLabel", _languageCode, True)
        businessunitLabel.Text = _wfrPage.Content("businessunitLabel", _languageCode, True)
        partnerLabel.Text = _wfrPage.Content("partnerLabel", _languageCode, True)
        PageNameLabel.Text = _wfrPage.Content("PageNameLabel", _languageCode, True)
        DescriptionLabel.Text = _wfrPage.Content("DescriptionLabel", _languageCode, True)
        LanguageLabel.Text = _wfrPage.Content("LanguageLabel", _languageCode, True)
        PageHeaderLabel.Text = _wfrPage.Content("PageHeaderLabel", _languageCode, True)
        PageTitleLabel.Text = _wfrPage.Content("PageTitleLabel", _languageCode, True)
        MetaDescriptionLabel.Text = _wfrPage.Content("MetaDescriptionLabel", _languageCode, True)
        MetaKeywordsLabel.Text = _wfrPage.Content("MetaKeywordsLabel", _languageCode, True)
        ConfirmButton.Text = _wfrPage.Content("ConfirmButton", _languageCode, True)
        CancelButton.Text = _wfrPage.Content("CancelButton", _languageCode, True)

        'Set Values
        businessunit.Text = _QSBusinessUnit
        partner.Text = _QSPartner
        PageName.Text = _QSPageCode
        Description.Text = _PgAddAdp.GetPageDesc(_QSBusinessUnit, _QSPartner, _QSPageCode)
        TitleLabel.Text = TitleLabel.Text + Description.Text
        DropDownLanguage.Enabled = True

        If _QSMode = "Edit" Then
            Dim DT As New DataTable
            DT = _PgAddAdp.GetHeadersAndTitlesData(Request.QueryString("ID"))
            DropDownLanguage.SelectedValue = DT.Rows(0)("LANGUAGE_CODE").ToString
            DropDownLanguage.Enabled = False
            TextPageHeader.Text = DT.Rows(0)("PAGE_HEADER").ToString
            TextPageTitle.Text = DT.Rows(0)("TITLE").ToString
            TextMetaDescription.Text = DT.Rows(0)("META_DESC").ToString
            TextMetaKeywords.Text = DT.Rows(0)("META_KEY").ToString
        End If
    End Sub

#End Region

End Class