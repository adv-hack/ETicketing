Imports Talent.Common
Imports System.Data
Imports System.Data.SqlClient

Partial Class PagesMaint_EditControls
    Inherits PageControlBase
    Public wfrPage As New Talent.Common.WebFormResource

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private pageId As String = String.Empty
    Private pageName As String = String.Empty
    Private businessUnit As String = String.Empty
    Private partner As String = String.Empty
    Private Enum ControlType
        GlobalMode
        PageMode
    End Enum
    Private controlMode As ControlType = ControlType.GlobalMode

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        IsAuthorizedRole()
        'Get Querystring values
        businessUnit = Request.QueryString("BusinessUnit")
        partner = Request.QueryString("Partner")
        pageId = Request.QueryString("PageId")
        pageName = Request.QueryString("PageName")
        With wfrPage
            .BusinessUnit = "MAINTENANCE" 'TalentCache.GetBusinessUnit()
            .PartnerCode = "*ALL" 'TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "EditControls.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
            .PageCode = "EditControls.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
        End With
        If Not Page.IsPostBack Then
            setPageText()
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        checkQueryStringValuesAndControlType()
        plhControlTextLangChangesSaved.Visible = False
        plhControlAttributesChangesSaved.Visible = False

        If Not Page.IsPostBack Then
            'determine the page mode - global controls or page controls
            If controlMode = ControlType.PageMode Then
                Dim dtPage As New DataTable
                dtPage = TDataObjects.PageSettings.TblPage.GetByPageCode(pageName, businessUnit)
                If dtPage.Rows.Count > 0 Then
                    txtDescription.Text = dtPage.Rows(0)("DESCRIPTION").ToString
                Else
                    lstErrorMessages.Items.Add(wfrPage.Content("PageNotFound", _languageCode, True))
                    Exit Sub
                End If
                TDataObjects.PageSettings.TblPage.GetByPageCode(pageName, businessUnit)
                ddlControl.DataSource = TDataObjects.PageSettings.TblPageControl.GetByPageCode(pageName)
            Else
                ddlControl.DataSource = TDataObjects.PageSettings.TblPageControl.GetByPageCode(String.Empty)
            End If

            'Bind the data to the control drop down box
            Try
                ddlControl.DataTextField = "CONTROL_CODE"
                ddlControl.DataValueField = "CONTROL_CODE"
                ddlControl.DataBind()
            Catch ex As Exception
                'error with datasource
                lstErrorMessages.Items.Add(wfrPage.Content("DataSourceError", _languageCode, True))
            End Try

            'Check to see if there are any controls available
            If ddlControl.Items.Count = 0 Then
                lstErrorMessages.Items.Add(wfrPage.Content("ControlsNotFound", _languageCode, True))
                liControl.Visible = False
            Else
                ddlControl.Items.Insert(0, wfrPage.Content("PleaseSelectDDLLabel", _languageCode, True))
            End If
        End If
    End Sub

    Private Sub setPageText()
        'Get all page text from DB
        ltlEditControlsTitle.Text = wfrPage.Content("EditControlsTitle", _languageCode, True)
        ltlEditControlsLegend.Text = wfrPage.Content("EditControlsLegend", _languageCode, True)
        ltlEditControlTextLangLegend.Text = wfrPage.Content("EditControlTextLangLegend", _languageCode, True)
        ltlEditControlAttributesLegend.Text = wfrPage.Content("EditControlAttributesLegend", _languageCode, True)

        ltlBusinessUnit.Text = wfrPage.Content("BusinessUnitLabel", _languageCode, True)
        ltlPartner.Text = wfrPage.Content("PartnerLabel", _languageCode, True)
        ltlPageName.Text = wfrPage.Content("PageNameLabel", _languageCode, True)
        ltlDescription.Text = wfrPage.Content("DescriptionLabel", _languageCode, True)
        ltlControl.Text = wfrPage.Content("ControlLabel", _languageCode, True)
        ltlLanguage.Text = wfrPage.Content("LanguageLabel", _languageCode, True)

        ltlControlTextLangChangesSaved.Text = wfrPage.Content("ControlTextLangChangesSavedLabel", _languageCode, True)
        ltlControlAttributesChangesSaved.Text = wfrPage.Content("ControlAttributesChangesSavedLabel", _languageCode, True)
        ltlNoControlTextLangRows.Text = wfrPage.Content("NoControlTextLangRowsLabel", _languageCode, True)
        ltlNoControlTextAttributes.Text = wfrPage.Content("NoControlTextAttributesLabel", _languageCode, True)

        btnReturnToPageOverview.Text = wfrPage.Content("ReturnToPageOverviewButton", _languageCode, True)
        btnReturnToPageDetails.Text = wfrPage.Content("ReturnToPageDetailsButton", _languageCode, True)
    End Sub

    Private Sub getControlData()
        'Load language specfic control text and control attributes into the data repeaters

        'Control Text Lang
        rptControlTextLang.DataSource = TDataObjects.ControlSettings.TblControlTextLang.GetByControlCodeWithNoPageCode( _
                                        businessUnit, _
                                        partner, _
                                        ddlControl.SelectedValue, _
                                        False)
        rptControlTextLang.DataBind()
        plhControlTextLang.Visible = True
        If rptControlTextLang.Items.Count > 0 Then
            plhNoControlTextLangRows.Visible = False
            rptControlTextLang.Visible = True
        Else
            plhNoControlTextLangRows.Visible = True
            rptControlTextLang.Visible = False
        End If

        'Control Attributes
        rptControlAttributes.DataSource = TDataObjects.ControlSettings.TblControlAttribute.GetByControlCodeWithNoPageCode( _
                                        businessUnit, _
                                        partner, _
                                        ddlControl.SelectedValue, _
                                        False)
        rptControlAttributes.DataBind()
        plhControlAttributes.Visible = True
        If rptControlAttributes.Items.Count > 0 Then
            plhNoControlTextAttributes.Visible = False
            rptControlAttributes.Visible = True
        Else
            plhNoControlTextAttributes.Visible = True
            rptControlAttributes.Visible = False
        End If
    End Sub

    Private Function checkQueryStringValuesAndControlType() As ControlType
        'Check to see if we are editing a control that 
        'is available Globally or just to this specific page
        If Not String.IsNullOrEmpty(pageId) Then
            liPageName.Visible = True
            liDescription.Visible = True
            btnReturnToPageDetails.Visible = True
            controlMode = ControlType.PageMode
        Else
            controlMode = ControlType.GlobalMode
        End If

        'Checks for correct values in querystrings and handle errors
        lstErrorMessages.Items.Clear()
        If Not String.IsNullOrEmpty(businessUnit) Then
            txtBusinessUnit.Text = businessUnit
        Else
            lstErrorMessages.Items.Add(wfrPage.Content("NoBUErrorMessage", _languageCode, True))
        End If
        If Not String.IsNullOrEmpty(partner) Then
            txtPartner.Text = partner
        Else
            lstErrorMessages.Items.Add(wfrPage.Content("NoPartnerErrorMessage", _languageCode, True))
        End If

        If controlMode = ControlType.PageMode Then
            If String.IsNullOrEmpty(pageId) Then
                lstErrorMessages.Items.Add(wfrPage.Content("NoPageIdErrorMessage", _languageCode, True))
            End If

            If Not String.IsNullOrEmpty(pageName) Then
                txtPageName.Text = pageName
            Else
                lstErrorMessages.Items.Add(wfrPage.Content("NoPageNameErrorMessage", _languageCode, True))
            End If
        End If
        Return controlMode
    End Function

    '====================== Drop Down Lists Functions =============

    Protected Sub ddlControl_Select(ByVal sender As Object, ByVal e As System.EventArgs)
        If ddlControl.SelectedIndex = 0 Then
            lstErrorMessages.Items.Clear()
            liLanguage.Visible = False
            plhControlTextLang.Visible = False
            plhControlAttributes.Visible = False
        Else
            liLanguage.Visible = True
            ddlLanguage.Items.Clear()
            If pageName Is Nothing Then pageName = ""
            ddlLanguage.DataSource = TDataObjects.ControlSettings.TblControlTextLang.GetByControlCode(businessUnit, partner, pageName, ddlControl.SelectedValue).DefaultView.ToTable(True, "LANGUAGE_CODE")

            'Bind the data to the language drop down box
            Try
                ddlLanguage.DataTextField = "LANGUAGE_CODE"
                ddlLanguage.DataValueField = "LANGUAGE_CODE"
                ddlLanguage.DataBind()
            Catch ex As Exception
                'error with datasource
                lstErrorMessages.Items.Add(wfrPage.Content("DataSourceError", _languageCode, True))
            End Try
            If ddlLanguage.Items.Count = 0 Then
                lstErrorMessages.Items.Add(wfrPage.Content("LangNotFound", _languageCode, True))
                liLanguage.Visible = False
                plhControlTextLang.Visible = False
                plhControlAttributes.Visible = False
            Else
                ddlLanguage.Items.Insert(0, wfrPage.Content("PleaseSelectDDLLabel", _languageCode, True))
            End If
        End If
    End Sub

    Protected Sub ddlLanguage_Select(ByVal sender As Object, ByVal e As System.EventArgs)
        If ddlLanguage.SelectedIndex = 0 Then
            lstErrorMessages.Items.Clear()
            plhControlTextLang.Visible = False
            plhControlAttributes.Visible = False
        Else
            getControlData()
        End If
    End Sub

    '====================== Button Functions ======================

    Protected Sub btnReturnToPageOverview_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Response.Redirect("../PagesMaint/PageOverview.aspx?Partner=" + partner + "&BusinessUnit=" + businessUnit)
    End Sub

    Protected Sub btnReturnToPageDetails_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Response.Redirect("../PagesMaint/Page-Details.aspx?PageId=" + pageId + "&PageName=" + pageName + "&Partner=" + partner + "&BusinessUnit=" + businessUnit)
    End Sub

    Protected Sub btnSaveControlTextLangChanges(ByVal sender As Object, ByVal e As System.EventArgs)
        'Save control text lang changes to database
        If rptControlTextLang.Items.Count > 0 Then
            Dim ltlTextCode As Literal
            Dim txtControlContent As TextBox
            Dim hdfPageCode As HiddenField
            Dim intRowsAffected As Integer = 0

            For Each item As RepeaterItem In rptControlTextLang.Items
                Try
                    ltlTextCode = rptControlTextLang.Items(item.ItemIndex).FindControl("ltlControlTextLangTextCode")
                    txtControlContent = rptControlTextLang.Items(item.ItemIndex).FindControl("txtControlTextLangControlContent")
                    hdfPageCode = rptControlTextLang.Items(item.ItemIndex).FindControl("hdfControlTextLangPageCode")

                    'if control mode is global, set page_code to *ALL
                    If String.IsNullOrEmpty(pageName) Then
                        hdfPageCode.Value = "*ALL"
                    End If

                    'Increment the number of rows that have successfully changed
                    'Data Table Columns: BU, PARTNER, PAGE_CODE, LANGUAGE, CONTROL_CODE, TEXT_CODE, CONTROL_CONTENT
                    intRowsAffected = TDataObjects.ControlSettings.TblControlTextLang.Update( _
                                        businessUnit, _
                                        partner, _
                                        hdfPageCode.Value, _
                                        ddlLanguage.SelectedValue, _
                                        ddlControl.SelectedValue, _
                                        ltlTextCode.Text, _
                                        txtControlContent.Text)
                    ' if not found then try *all BU
                    If intRowsAffected = 0 Then
                        intRowsAffected = TDataObjects.ControlSettings.TblControlTextLang.Update( _
                                      "*ALL", _
                                      partner, _
                                      hdfPageCode.Value, _
                                      ddlLanguage.SelectedValue, _
                                      ddlControl.SelectedValue, _
                                      ltlTextCode.Text, _
                                      txtControlContent.Text)
                    End If
                    intRowsAffected += intRowsAffected

                Catch ex As Exception
                    lstErrorMessages.Items.Clear()
                    lstErrorMessages.Items.Add(wfrPage.Content("ControlUpdateError", _languageCode, True))
                    getControlData()
                End Try
            Next

            'Check to see if any rows have been updated
            If intRowsAffected = 0 Then
                lstErrorMessages.Items.Clear()
                lstErrorMessages.Items.Add(wfrPage.Content("ControlUpdateError", _languageCode, True))
                getControlData()
                plhControlTextLangChangesSaved.Visible = False
            Else
                plhControlTextLangChangesSaved.Visible = True
            End If
        End If
    End Sub

    Protected Sub btnSaveControlAttributesChanges(ByVal sender As Object, ByVal e As System.EventArgs)
        'Save control text lang changes to database
        If rptControlAttributes.Items.Count > 0 Then
            Dim ltlAttributeName As Literal
            Dim txtAttributeValue As TextBox
            Dim chkAttributeValue As CheckBox
            Dim attributeValue As String = String.Empty
            Dim hdfPageCode As HiddenField
            Dim intRowsAffected As Integer = 0

            For Each item As RepeaterItem In rptControlAttributes.Items
                Try
                    ltlAttributeName = rptControlAttributes.Items(item.ItemIndex).FindControl("ltlControlAttributeName")
                    txtAttributeValue = rptControlAttributes.Items(item.ItemIndex).FindControl("txtControlAttributeValue")
                    chkAttributeValue = rptControlAttributes.Items(item.ItemIndex).FindControl("chkControlAttrubuteValue")
                    hdfPageCode = rptControlAttributes.Items(item.ItemIndex).FindControl("hdfControlAttributePageCode")

                    'determine if the attribute value is represented by a textbox or checkbox
                    If txtAttributeValue.Visible Then
                        attributeValue = txtAttributeValue.Text
                    ElseIf chkAttributeValue.Visible Then
                        attributeValue = chkAttributeValue.Checked.ToString
                    End If

                    'if control mode is global, set page_code to *ALL
                    If String.IsNullOrEmpty(pageName) Then
                        hdfPageCode.Value = "*ALL"
                    End If

                    'Increment the number of rows that have successfully changed
                    'Data Table Columns: BU, PARTNER, PAGE_CODE, CONTROL_CODE, ATTR_NAME, ATTR_VALUE
                    intRowsAffected = TDataObjects.ControlSettings.TblControlAttribute.Update( _
                                        businessUnit, _
                                        partner, _
                                        hdfPageCode.Value, _
                                        ddlControl.SelectedValue, _
                                        ltlAttributeName.Text, _
                                        attributeValue)
                    ' if not found then try *all BU
                    If intRowsAffected = 0 Then
                        intRowsAffected = TDataObjects.ControlSettings.TblControlAttribute.Update( _
                                        "*ALL", _
                                        partner, _
                                        hdfPageCode.Value, _
                                        ddlControl.SelectedValue, _
                                        ltlAttributeName.Text, _
                                        attributeValue)
                    End If
                    intRowsAffected += intRowsAffected

                Catch ex As Exception
                    lstErrorMessages.Items.Clear()
                    lstErrorMessages.Items.Add(wfrPage.Content("ControlUpdateError", _languageCode, True))
                    getControlData()
                End Try
            Next

            'Check to see if any rows have been updated
            If intRowsAffected = 0 Then
                lstErrorMessages.Items.Clear()
                lstErrorMessages.Items.Add(wfrPage.Content("ControlUpdateError", _languageCode, True))
                getControlData()
            Else
                plhControlAttributesChangesSaved.Visible = True
            End If
        End If
    End Sub

    '====================== Repeater Functions ====================

    Protected Sub rptControlTextLang_OnItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.Footer Then
            Dim btnSaveControlTextLang As Button
            btnSaveControlTextLang = rptControlTextLang.Controls(rptControlTextLang.Controls.Count - 1).Controls(0).FindControl("btnSaveControlTextLang")
            btnSaveControlTextLang.Text = wfrPage.Content("SaveControlTextLangButton", _languageCode, True)
        End If
    End Sub

    Protected Sub rptControlTextLang_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptControlTextLang.ItemDataBound
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim row As DataRowView = CType(e.Item.DataItem, DataRowView)
            Dim ltlControlTextLangTextCode As Literal = CType(e.Item.FindControl("ltlControlTextLangTextCode"), Literal)
            Dim txtControlTextLangControlContent As TextBox = CType(e.Item.FindControl("txtControlTextLangControlContent"), TextBox)
            Dim hdfControlTextLangPageCode As HiddenField = CType(e.Item.FindControl("hdfControlTextLangPageCode"), HiddenField)
            If Not Utilities.CheckForDBNull_Boolean_DefaultFalse(row("HIDE_IN_MAINTENANCE")) Then
                ltlControlTextLangTextCode.Text = row("TEXT_CODE")
                txtControlTextLangControlContent.Text = row("CONTROL_CONTENT")
                hdfControlTextLangPageCode.Value = row("PAGE_CODE")
            Else
                ltlControlTextLangTextCode.Visible = False
                txtControlTextLangControlContent.Visible = False
                hdfControlTextLangPageCode.Visible = False
            End If
        End If
    End Sub

    Protected Sub rptControlAttributes_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptControlAttributes.ItemDataBound
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim row As DataRowView = CType(e.Item.DataItem, DataRowView)
            Dim ltlControlAttributeName As Literal = CType(e.Item.FindControl("ltlControlAttributeName"), Literal)
            Dim txtControlAttributeValue As TextBox = CType(e.Item.FindControl("txtControlAttributeValue"), TextBox)
            Dim chkControlAttrubuteValue As CheckBox = CType(e.Item.FindControl("chkControlAttrubuteValue"), CheckBox)
            Dim hdfControlAttributePageCode As HiddenField = CType(e.Item.FindControl("hdfControlAttributePageCode"), HiddenField)
            If Not Utilities.CheckForDBNull_Boolean_DefaultFalse(row("HIDE_IN_MAINTENANCE")) Then
                ltlControlAttributeName.Text = row("ATTR_NAME")
                txtControlAttributeValue.Text = row("ATTR_VALUE")
                hdfControlAttributePageCode.Value = row("PAGE_CODE")
            Else
                ltlControlAttributeName.Visible = False
                txtControlAttributeValue.Visible = False
                chkControlAttrubuteValue.Visible = False
                hdfControlAttributePageCode.Visible = False
            End If
        End If
    End Sub
    Protected Sub rptControlAttributes_OnItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            Try
                Dim booleanAttrValue As Boolean = False
                Dim itemIsCheckBox As Boolean = False

                If e.Item.DataItem("ATTR_VALUE").ToString.ToUpper = "FALSE" Then
                    booleanAttrValue = False
                    itemIsCheckBox = True
                ElseIf e.Item.DataItem("ATTR_VALUE").ToString.ToUpper = "TRUE" Then
                    booleanAttrValue = True
                    itemIsCheckBox = True
                Else
                    itemIsCheckBox = False
                End If

                If itemIsCheckBox Then
                    Dim chkControlAttrubuteValue As CheckBox
                    chkControlAttrubuteValue = rptControlAttributes.Controls(rptControlAttributes.Controls.Count - 1).Controls(0).FindControl("chkControlAttrubuteValue")
                    chkControlAttrubuteValue.Checked = booleanAttrValue
                    chkControlAttrubuteValue.Visible = True
                Else
                    Dim txtControlAttributeValue As TextBox
                    txtControlAttributeValue = rptControlAttributes.Controls(rptControlAttributes.Controls.Count - 1).Controls(0).FindControl("txtControlAttributeValue")
                    txtControlAttributeValue.Visible = True
                End If
            Catch ex As Exception
                Dim txtControlAttributeValue As TextBox
                txtControlAttributeValue = rptControlAttributes.Controls(rptControlAttributes.Controls.Count - 1).Controls(0).FindControl("txtControlAttributeValue")
                txtControlAttributeValue.Visible = True
            End Try
        ElseIf e.Item.ItemType = ListItemType.Footer Then
            Dim btnSaveAttributes As Button
            btnSaveAttributes = rptControlAttributes.Controls(rptControlAttributes.Controls.Count - 1).Controls(0).FindControl("btnSaveAttributes")
            btnSaveAttributes.Text = wfrPage.Content("SaveControlAttributesButton", _languageCode, True)
        End If
    End Sub

    Private Sub IsAuthorizedRole()
        If Not Utilities.isEnabled("AllowEditControls") Then
            Response.Redirect("../Error.aspx?Type=UNAUTH")
        End If
    End Sub

    
   
End Class
