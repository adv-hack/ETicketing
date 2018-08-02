Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports System.Data

Partial Class PagesMaint_Page_Details
    Inherits System.Web.UI.Page

#Region "Class Level Fields"

    Private _wfrPage As New WebFormResource
    Private _languageCode As String = String.Empty
    Private _objPage As New pageselectionTableAdapters.PageTemplateTableAdapter
    Private _objPageOpt As New PageDataSetTableAdapters.PageDateTableAdapter
    Private _objPageTempate As New PageDataSetTableAdapters.TemplatePageTableAdapter
    Private _objPageLang As New PageHeaderAndTitlesTableAdapters.tbl_page_langTableAdapter
    Private _objPageHtml As New PageAddHtmlIncludeTableAdapters.tbl_page_htmlTableAdapter

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _wfrPage
            .BusinessUnit = GlobalConstants.MAINTENANCEBUSINESSUNIT
            .PageCode = String.Empty
            .PartnerCode = GlobalConstants.STARALLPARTNER
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "Page-Details.aspx"
            .PageCode = "Page-Details.aspx"
        End With
        _languageCode = TCUtilities.GetDefaultLanguage
        IsAuthorizedRole()
        setLabel()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        plhErrorMessage.Visible = False
        If IsPostBack = False Then
            If String.IsNullOrEmpty(Request.QueryString("BusinessUnit")) Then
                Response.Redirect("../MaintenancePortal.aspx")
            End If
            If String.IsNullOrEmpty(Request.QueryString("Partner")) Then
                Response.Redirect("../MaintenancePortal.aspx")
            End If

            BreadcrumbTrailParentDropDownList.DataSource = _objPageOpt.GetDataByBUPTOrderByPageCode(Request.QueryString("businessUnit"), Request.QueryString("Partner"), False)
            BreadcrumbTrailParentDropDownList.DataBind()
            BreadcrumbTrailParentDropDownList.Items.Insert(0, "None")
            BreadcrumbTrailParentDropDownList.SelectedIndex = 0

            If Request.QueryString("Status") = "New" Then
                BULabel1.Text = Request.QueryString("businessUnit")
                PartnerLabel1.Text = Request.QueryString("Partner")
                PageHTButton.Enabled = False
                DefineHTMLButton.Enabled = False
            End If

            TemplateDropDownList.DataSource = _objPage.GetData()
            TemplateDropDownList.DataTextField = "TEMPLATE_NAME"
            TemplateDropDownList.DataValueField = "TEMPLATE_NAME"
            TemplateDropDownList.DataBind()

            If Request.QueryString("Status") = "New" Then
                PageNameTextBox.ReadOnly = False
                PageNameTextBox.CssClass = "input-l"
                titleLabel.Text = "Add Page"
            Else
                PageNameTextBox.ReadOnly = True
                PageNameTextBox.CssClass = "input-l input-readonly"
                fillDate()
            End If
            If Session("AdditionMade") = True Then
                Session("AdditionMade") = False
                plhErrorMessage.Visible = True
                ErrorLabel.Text = _wfrPage.Content("SuccAdd", _languageCode, True)
            End If
            TemplateImage.Src = "~/images/" + TemplateDropDownList.SelectedItem.Text + ".gif"
            If Session("UpdatesMade") = True Then
                Session("UpdatesMade") = False
                plhErrorMessage.Visible = True
                ErrorLabel.Text = _wfrPage.Content("SuccUpdate", _languageCode, True)
            End If
        End If
        DisableControls()
    End Sub

    Protected Sub PageHTButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles PageHTButton.Click
        If ViewState("iCheck") = 1 Then
            If Request.QueryString("Status") = "New" Then
                Response.Redirect("../PagesMaint/PageHeadersAndTitles.aspx?PageId=" + ViewState("PageID").ToString + "&PageName=" + "UserDefined.aspx?page=" + PageNameTextBox.Text + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
            Else
                Response.Redirect("../PagesMaint/PageHeadersAndTitles.aspx?PageId=" + Request.QueryString("PageId") + "&PageName=" + Request.QueryString("PageName") + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
            End If
        Else
            If Request.QueryString("Status") = "New" Then
                Response.Redirect("../PagesMaint/PageHeadersAndTitles.aspx?PageId=" + ViewState("PageID").ToString + "&PageName=" + "UserDefined.aspx?page=" + PageNameTextBox.Text + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
            Else
                Response.Redirect("../PagesMaint/PageHeadersAndTitles.aspx?PageId=" + Request.QueryString("PageId") + "&PageName=" + Request.QueryString("PageName") + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
            End If
        End If
    End Sub

    Protected Sub DefineHTMLButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles DefineHTMLButton.Click
        If String.IsNullOrEmpty(PageNameTextBox.Text) AndAlso Request.QueryString("Status") = "New" Then
            ErrorLabel.Text = _wfrPage.Content("MissingPageName", _languageCode, True)
            plhErrorMessage.Visible = True
        Else
            If ViewState("iCheck") = 1 Then
                If Request.QueryString("Status") = "New" Then
                    Response.Redirect("../PagesMaint/PageHtmlIncludes.aspx?PageName=" + "UserDefined.aspx?page=" + PageNameTextBox.Text + "&PageId=" + ViewState("PageID").ToString + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
                Else
                    Response.Redirect("../PagesMaint/PageHtmlIncludes.aspx?PageName=" + Request.QueryString("PageName") + "&PageId=" + Request.QueryString("PageId") + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
                End If
            Else
                If Request.QueryString("Status") = "New" Then
                    Response.Redirect("../PagesMaint/PageHtmlIncludes.aspx?PageName=" + "UserDefined.aspx?page=" + PageNameTextBox.Text + "&PageId=" + ViewState("PageID").ToString + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
                Else
                    Response.Redirect("../PagesMaint/PageHtmlIncludes.aspx?PageName=" + Request.QueryString("PageName") + "&PageId=" + Request.QueryString("PageId") + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
                End If
            End If
        End If
        
    End Sub

    Protected Sub ConfirmButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ConfirmButton.Click
        Dim iForceLoginCheckBox As Integer = 0
        If ForceLoginCheckBox.Checked Then iForceLoginCheckBox = 1

        Dim BTP As String
        If BreadcrumbTrailParentDropDownList.SelectedIndex = 0 Then
            BTP = ""
        Else
            BTP = BreadcrumbTrailParentDropDownList.SelectedValue
        End If

        If Request.QueryString("status") = "New" Then
            Dim checkVal As Integer
            checkVal = _objPageOpt.CheckPageCode(BULabel1.Text, PartnerLabel1.Text, "UserDefined.aspx?page=" + PageNameTextBox.Text)
            If checkVal = 0 Then
                Try
                    Dim retValInsert As Integer
                    retValInsert = _objPageOpt.PageDetailInsertQuery(BULabel1.Text, PartnerLabel1.Text, "UserDefined.aspx?page=" + PageNameTextBox.Text, DescriptionTextBox.Text.ToString, PageQuerystringTextBox.Text.ToString, Convert.ToInt16(UseSecureURLCheckBox.Checked), Convert.ToInt16(HTMLInUseCheckBox.Checked), PageTpeDropDownList.SelectedValue, Convert.ToInt16(ShowPageHeaderCheckBox.Checked), BreadcrumbTrailURLTextBox.Text, BTP, iForceLoginCheckBox, PageInUseCheckBox.Checked)
                    _objPageTempate.InsertPageTemplateInsert(BULabel1.Text, PartnerLabel1.Text, "UserDefined.aspx?page=" + PageNameTextBox.Text, TemplateDropDownList.SelectedValue)
                    ViewState("PageID") = _objPageOpt.GetPageId(BULabel1.Text, PartnerLabel1.Text, "UserDefined.aspx?page=" + PageNameTextBox.Text)
                    If retValInsert > 0 Then
                        plhErrorMessage.Visible = True
                        ErrorLabel.Text = _wfrPage.Content("SuccAdd", _languageCode, True)
                        Session("AdditionMade") = True
                    Else
                        plhErrorMessage.Visible = True
                        ErrorLabel.Text = _wfrPage.Content("UnScc", _languageCode, True)
                    End If
                    PageHTButton.Enabled = True
                    DefineHTMLButton.Enabled = True
                Catch ex As Exception
                    plhErrorMessage.Visible = True
                    ErrorLabel.Text = _wfrPage.Content("UnScc", _languageCode, True)
                End Try
            Else
                plhErrorMessage.Visible = True
                ErrorLabel.Text = _wfrPage.Content("AlreadyExists", _languageCode, True)
            End If
            ' If the record is added we need to reload the page ode to ensure query string is as expected.
            If Session("AdditionMade") = True Then
                Response.Redirect("../PagesMaint/Page-Details.aspx?PageName=" + "UserDefined.aspx?page=" + PageNameTextBox.Text + "&PageId=" + ViewState("PageID").ToString + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
            End If
        Else
            Try
                Dim retVal As Integer
                If ViewState("iCheck") = 1 Then
                    retVal = _objPageOpt.CheckPageCodeUpdate(Request.QueryString("PageId"), Request.QueryString("PageName"), Request.QueryString("BusinessUnit"), Request.QueryString("Partner"))
                Else
                    retVal = _objPageOpt.CheckPageCodeUpdate(Request.QueryString("PageId"), PageNameTextBox.Text, Request.QueryString("BusinessUnit"), Request.QueryString("Partner"))
                End If
                If retVal = 0 Then

                    If ViewState("iCheck") = 1 Then
                        _objPageOpt.PageDetailsUpdate("UserDefined.aspx?page=" + PageNameTextBox.Text, DescriptionTextBox.Text, PageQuerystringTextBox.Text, Convert.ToInt16(UseSecureURLCheckBox.Checked), Convert.ToInt16(HTMLInUseCheckBox.Checked), PageTpeDropDownList.SelectedValue, Convert.ToInt16(ShowPageHeaderCheckBox.Checked), BreadcrumbTrailURLTextBox.Text, BTP, iForceLoginCheckBox, PageInUseCheckBox.Checked, Request.QueryString("PageId"))
                        _objPageLang.UpdatePageLangPageCode(Request.QueryString("PageName"), "UserDefined.aspx?page=" + PageNameTextBox.Text, BULabel1.Text, PartnerLabel1.Text)
                        _objPageHtml.UpdatePageHtmlPageCode(Request.QueryString("PageName"), "UserDefined.aspx?page=" + PageNameTextBox.Text, BULabel1.Text, PartnerLabel1.Text)
                    Else
                        _objPageOpt.PageDetailsUpdate(PageNameTextBox.Text, DescriptionTextBox.Text, PageQuerystringTextBox.Text, Convert.ToInt16(UseSecureURLCheckBox.Checked), Convert.ToInt16(HTMLInUseCheckBox.Checked), PageTpeDropDownList.SelectedValue, Convert.ToInt16(ShowPageHeaderCheckBox.Checked), BreadcrumbTrailURLTextBox.Text, BTP, iForceLoginCheckBox, PageInUseCheckBox.Checked, Request.QueryString("PageId"))
                    End If
                    Dim retValTem As Integer
                    If ViewState("iCheck") = 1 Then
                        retValTem = _objPageTempate.UpdatePageTemplate(TemplateDropDownList.SelectedValue, BULabel1.Text, PartnerLabel1.Text, Request.QueryString("PageName"), "UserDefined.aspx?page=" + PageNameTextBox.Text)
                    Else
                        retValTem = _objPageTempate.UpdatePageTemplate(TemplateDropDownList.SelectedValue, BULabel1.Text, PartnerLabel1.Text, PageNameTextBox.Text, PageNameTextBox.Text)
                    End If

                    If retValTem = 0 Then
                        If ViewState("iCheck") = 1 Then
                            _objPageTempate.InsertPageTemplateInsert(BULabel1.Text, PartnerLabel1.Text, "UserDefined.aspx?page=" + PageNameTextBox.Text, TemplateDropDownList.SelectedValue)
                        Else
                            _objPageTempate.InsertPageTemplateInsert(BULabel1.Text, PartnerLabel1.Text, PageNameTextBox.Text, TemplateDropDownList.SelectedValue)
                        End If
                    End If
                    plhErrorMessage.Visible = True
                    ErrorLabel.Text = _wfrPage.Content("SuccUpdate", _languageCode, True)
                    Session("UpdatesMade") = True
                Else
                    plhErrorMessage.Visible = True
                    ErrorLabel.Text = _wfrPage.Content("AlreadyExists", _languageCode, True)
                End If
            Catch ex As Exception
                plhErrorMessage.Visible = True
                ErrorLabel.Text = _wfrPage.Content("UnScc", _languageCode, True)
            End Try

            ' If the record is updated we need to reload the page with the new changes.
            ' For example if the page name changes we need to reload the page using the new page name in the querystring
            If Session("UpdatesMade") = True Then
                If ViewState("iCheck") = 1 Then
                    Response.Redirect("../PagesMaint/Page-Details.aspx?PageName=" + "UserDefined.aspx?page=" + PageNameTextBox.Text + "&PageId=" + Request.QueryString("PageID").ToString + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
                Else
                    Response.Redirect("../PagesMaint/Page-Details.aspx?PageName=" + PageNameTextBox.Text + "&PageId=" + Request.QueryString("PageID").ToString + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
                End If

            End If
        End If
    End Sub

    Protected Sub TemplateDropDownList_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TemplateDropDownList.SelectedIndexChanged
        TemplateImage.Src = "..\images\" + TemplateDropDownList.SelectedItem.Text + ".gif"
    End Sub

    Protected Sub ReturToPageButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ReturToPageButton.Click
        Response.Redirect("PageOverview.aspx?BusinessUnit=" + Request.QueryString("BusinessUnit") + "&Partner=" + Request.QueryString("Partner"))
    End Sub

    Protected Sub CancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CancelButton.Click
        Response.Redirect("PageOverview.aspx?BusinessUnit=" + Request.QueryString("BusinessUnit") + "&Partner=" + Request.QueryString("Partner"))
    End Sub

    Protected Sub EditControls_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles EditControlsButton.Click
        Response.Redirect("../PagesMaint/EditControls.aspx?PageName=" + Request.QueryString("PageName") + "&PageId=" + Request.QueryString("PageId") + "&BusinessUnit=" + Request.QueryString("BusinessUnit") + "&Partner=" + Request.QueryString("Partner"))
    End Sub

    Protected Sub btnHideInMaintenance_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnHideInMaintenance.Click
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As DESettings = New DESettings()
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
        settings.BusinessUnit = Request.QueryString("businessUnit")
        settings.ConnectionStringList = Utilities.GetConnectionStringList()
        tDataObjects.Settings = settings
        Dim affectedRows As Integer = 0
        Dim pageId As Integer = Utilities.CheckForDBNull_Int(Request.QueryString("PageId"))
        If pageId > 0 Then
            affectedRows = tDataObjects.PageSettings.TblPage.UpdateHideInMaintenanceToTrue(pageId)
            If affectedRows > 0 Then
                Response.Redirect("PageOverview.aspx?BusinessUnit=" + Request.QueryString("BusinessUnit") + "&Partner=" + Request.QueryString("Partner"))
            End If
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub setLabel()
        instructionsLabel.Text = _wfrPage.Content("instructionsLabel", _languageCode, True)
        PartnerLabel.Text = _wfrPage.Content("partnerLabel", _languageCode, True)
        PageNameLabel.Text = _wfrPage.Content("PageNameLabel", _languageCode, True)
        DescriptionLabel.Text = _wfrPage.Content("DescriptionLabel", _languageCode, True)
        PageQuerystringLabel.Text = _wfrPage.Content("PageQuerystringLabel", _languageCode, True)
        PageInUseLabel.Text = _wfrPage.Content("PageInUseLabel", _languageCode, True)
        UseSecureURLLabel.Text = _wfrPage.Content("UseSecureURLLabel", _languageCode, True)
        BULabel.Text = _wfrPage.Content("BULabel", _languageCode, True)
        HTMLInUseLabel.Text = _wfrPage.Content("HTMLInUseLabel", _languageCode, True)
        PageTypeLabel.Text = _wfrPage.Content("PageTypeLabel", _languageCode, True)
        ShowPageHeaderLabel.Text = _wfrPage.Content("ShowPageHeaderLabel", _languageCode, True)
        BreadcrumbTrailURLLabel.Text = _wfrPage.Content("BreadcrumbTrailURLLabel", _languageCode, True)
        BreadcrumbTrailParentLabel.Text = _wfrPage.Content("BreadcrumbTrailParentLabel", _languageCode, True)
        ForceLoginLabel.Text = _wfrPage.Content("ForceLoginLabel", _languageCode, True)
        TemplateLabel.Text = _wfrPage.Content("TemplateLabel", _languageCode, True)
        ReturToPageButton.Text = _wfrPage.Content("ReturToPageButton", _languageCode, True)
        PageHTButton.Text = _wfrPage.Content("PageHTButton", _languageCode, True)
        DefineHTMLButton.Text = _wfrPage.Content("DefineHTMLButton", _languageCode, True)
        ConfirmButton.Text = _wfrPage.Content("ConfirmButton", _languageCode, True)
        CancelButton.Text = _wfrPage.Content("CancelButton", _languageCode, True)
        EditControlsButton.Text = _wfrPage.Content("EditControlsButton", _languageCode, True)
        If ConfigurationManager.AppSettings("DisplayHidePageButton") IsNot Nothing Then
            btnHideInMaintenance.Visible = Utilities.CheckForDBNull_Boolean_DefaultFalse(ConfigurationManager.AppSettings("DisplayHidePageButton"))
            If btnHideInMaintenance.Visible Then btnHideInMaintenance.Text = _wfrPage.Content("HideInMaintenanceButton", _languageCode, True)
        Else
            btnHideInMaintenance.Visible = False
        End If
    End Sub

    Private Sub fillDate()
        Dim dt As New DataTable
        dt = _objPageOpt.GetDataByPageDetails(Request.QueryString("BusinessUnit"), Request.QueryString("Partner"), Request.QueryString("PageName").ToString)
        If dt.Rows.Count = 0 Then
            plhErrorMessage.Visible = True
            ErrorLabel.Text = "This page does not exist."
        Else
            BULabel1.Text = dt.Rows(0).Item("BUSINESS_UNIT").ToString
            PartnerLabel1.Text = dt.Rows(0).Item("PARTNER_CODE").ToString
            If dt.Rows(0).Item("PAGE_CODE").ToString.ToLower = "userdefined.aspx" Or dt.Rows(0).Item("PAGE_CODE").ToString.ToLower.StartsWith("userdefined.aspx?page=") Then
                PageNameTextBox.Text = dt.Rows(0).Item("PAGE_CODE").ToString.ToLower.Replace("userdefined.aspx?page=", "")
                ViewState("iCheck") = 1
                titleLabel.Text = "Change Page Details: " + dt.Rows(0).Item("PAGE_CODE").ToString.ToLower.Replace("userdefined.aspx?page=", "")
                PageNameTextBox.ReadOnly = False
                PageNameTextBox.CssClass = "input-l"
            Else
                ViewState("iCheck") = 0
                PageNameTextBox.Text = dt.Rows(0).Item("PAGE_CODE").ToString
                titleLabel.Text = "Change Page Details: " + dt.Rows(0).Item("PAGE_CODE")
            End If

            DescriptionTextBox.Text = dt.Rows(0).Item("DESCRIPTION").ToString
            PageQuerystringTextBox.Text = dt.Rows(0).Item("PAGE_QUERYSTRING").ToString
            PageInUseCheckBox.Checked = Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultFalse(dt.Rows(0).Item("IN_USE"))

            UseSecureURLCheckBox.Checked = Convert.ToBoolean(dt.Rows(0).Item("USE_SECURE_URL"))
            HTMLInUseCheckBox.Checked = Convert.ToBoolean(dt.Rows(0).Item("HTML_IN_USE"))
            PageTpeDropDownList.SelectedValue = dt.Rows(0).Item("PAGE_TYPE").ToString
            ShowPageHeaderCheckBox.Checked = Convert.ToBoolean(dt.Rows(0).Item("SHOW_PAGE_HEADER").ToString)
            BreadcrumbTrailURLTextBox.Text = dt.Rows(0).Item("BCT_URL").ToString

            If dt.Rows(0).Item("BCT_PARENT").ToString = "" Then
                BreadcrumbTrailParentDropDownList.SelectedIndex = 0
            Else
                BreadcrumbTrailParentDropDownList.SelectedValue = dt.Rows(0).Item("BCT_PARENT").ToString
            End If

            ForceLoginCheckBox.Checked = Convert.ToBoolean(dt.Rows(0).Item("FORCE_LOGIN").ToString)
            Dim dtTem As New DataTable
            dtTem = _objPageTempate.GetDataByPageCode(dt.Rows(0).Item("BUSINESS_UNIT").ToString, dt.Rows(0).Item("PARTNER_CODE").ToString, dt.Rows(0).Item("PAGE_CODE").ToString)

            If dtTem.Rows.Count = 0 Then
                dtTem = _objPageTempate.GetDataByPageCode(dt.Rows(0).Item("BUSINESS_UNIT").ToString, Talent.Common.Utilities.GetAllString, dt.Rows(0).Item("PAGE_CODE").ToString)
            End If

            If dtTem.Rows.Count = 0 Then
                dtTem = _objPageTempate.GetDataByPageCode(Talent.Common.Utilities.GetAllString, Talent.Common.Utilities.GetAllString, dt.Rows(0).Item("PAGE_CODE").ToString)
            End If
            If dtTem.Rows.Count <> 0 Then
                For Each li As ListItem In TemplateDropDownList.Items
                    li.Selected = False
                    If UCase(li.Value) = UCase(Utilities.CheckForDBNull_String(dtTem.Rows(0).Item("TEMPLATE_NAME").ToString())) Then
                        li.Selected = True
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub DisableControls()
        PageHTButton.Enabled = Utilities.isEnabled("AllowPageHeadersAndTitles")
        EditControlsButton.Enabled = Utilities.isEnabled("AllowEditControls")
        DefineHTMLButton.Enabled = Utilities.isEnabled("DefineHTMLIncludes")
        PageHTButton.Visible = PageHTButton.Enabled
        EditControlsButton.Visible = EditControlsButton.Enabled
        DefineHTMLButton.Visible = DefineHTMLButton.Enabled

        If Not Utilities.isEnabled("BasicPageUpdatesOnly") Then
            PageNameTextBox.CssClass = "input-enabled"
            DescriptionTextBox.CssClass = "input-enabled"
            PageQuerystringTextBox.CssClass = "input-enabled"
            PageTpeDropDownList.CssClass = "input-enabled"
            PageNameTextBox.CssClass = "input-enabled"
            BreadcrumbTrailParentDropDownList.CssClass = "input-enabled"
            TemplateDropDownList.CssClass = "input-enabled"
            lnkTemplate.Visible = True
            PageNameTextBox.Enabled = True
            DescriptionTextBox.Enabled = True
            PageQuerystringTextBox.Enabled = True
            PageTpeDropDownList.Enabled = True
            BreadcrumbTrailURLTextBox.Enabled = True
            BreadcrumbTrailParentDropDownList.Enabled = True
            TemplateDropDownList.Enabled = True
            ForceLoginCheckBox.Enabled = True
            ShowPageHeaderCheckBox.Enabled = True
            UseSecureURLCheckBox.Enabled = True
            HTMLInUseCheckBox.Enabled = True
            PageInUseCheckBox.Enabled = True
            If Request.QueryString("Status") = "New" Then
                Me.lnkTemplate.Visible = True
                Me.TemplateDropDownList.Enabled = True
            Else
                Me.lnkTemplate.Visible = False
                Me.TemplateDropDownList.Enabled = False
            End If
        Else
            PageNameTextBox.CssClass = "input-disabled"
            DescriptionTextBox.CssClass = "input-disabled"
            PageQuerystringTextBox.CssClass = "input-disabled"
            PageTpeDropDownList.CssClass = "input-disabled"
            PageNameTextBox.CssClass = "input-disabled"
            BreadcrumbTrailParentDropDownList.CssClass = "input-disabled"
            TemplateDropDownList.CssClass = "input-disabled"
            lnkTemplate.Visible = False
            PageNameTextBox.Enabled = False
            DescriptionTextBox.Enabled = False
            PageQuerystringTextBox.Enabled = False
            PageTpeDropDownList.Enabled = False
            BreadcrumbTrailURLTextBox.Enabled = False
            BreadcrumbTrailParentDropDownList.Enabled = False
            ForceLoginCheckBox.Enabled = True
            ShowPageHeaderCheckBox.Enabled = True
            UseSecureURLCheckBox.Enabled = True
            HTMLInUseCheckBox.Enabled = True
            PageInUseCheckBox.Enabled = True
        End If
    End Sub

    Private Sub IsAuthorizedRole()
        If Not Utilities.isEnabled("AllowAddNewPage") _
            AndAlso Not String.IsNullOrWhiteSpace(Request.QueryString("status")) AndAlso (Request.QueryString("status") = "New") Then
            Response.Redirect("../Error.aspx?Type=UNAUTH")
        End If
    End Sub

#End Region

End Class