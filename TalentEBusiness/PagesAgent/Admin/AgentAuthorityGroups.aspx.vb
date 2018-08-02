Imports System.Data
Imports Talent.Common.DataObjects
Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class PagesAgent_Admin_AgentAuthorityGroups
    Inherits TalentBase01

#Region "Constants"
    Const KEYCODE As String = "AgentAuthorityGroups.aspx"
#End Region

#Region "Public Properties"

    Public LoadingText As String = String.Empty

#End Region

#Region "Class Level Fields"

    Private _wfrPage As WebFormResource = Nothing
    Private _languageCode As String = String.Empty
    Private _results As DataTable = Nothing
    Private _errMessage As TalentErrorMessages = Nothing
    Private _dateValid As Boolean = True
    Private _errorMessage As String = String.Empty
    Private _agentGroups As DataTable = Nothing

    Private PleaseSelectGroup As String = String.Empty
    Private PleaseSelectCategory As String = String.Empty

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If AgentProfile.AgentPermissions.IsAdministrator Then
            _wfrPage = New WebFormResource
            _languageCode = TCUtilities.GetDefaultLanguage()
            With _wfrPage
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = ProfileHelper.GetPageName
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = KEYCODE
            End With

            LoadLabels()
            LoadGroups()
            LoadCategories()
            plhUpdateGroupErrorMessage.Visible = False
            plhUpdateGroupSuccessMessage.Visible = False
        Else
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        ddlCategories.Enabled = (Not ddlGroups.SelectedValue = "0")
        btnUpdatePermissions.Enabled = ddlCategories.Enabled
    End Sub

    Protected Sub ddlGroups_Index_Changed(sender As Object, e As EventArgs)
        LoadPermissions()
    End Sub

    Protected Sub ddlCategories_Index_Changed(sender As Object, e As EventArgs)
        LoadPermissions()
    End Sub

    Protected Sub chkListPermissions_Changed(sender As Object, e As EventArgs)
        Dim Permissions As StringBuilder = Nothing
        Dim groupId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim permissionStatus As Boolean = False
        Dim permissionId As Integer = 0
        Dim checkBoxSelected As String = String.Empty
        Dim IsCheckBoxSelected As Boolean = False
        Dim Index As Integer = 0
        Dim CheckBoxListObj As New CheckBoxList()

        CheckBoxListObj = sender
        checkBoxSelected = Request("__EVENTTARGET") 'With the help of Request Object we are able to identify which checkbox was clicked by user.
        Index = checkBoxSelected.Split("$")(checkBoxSelected.Split("$").Length - 1)
        IsCheckBoxSelected = CheckBoxListObj.Items(Index).Selected
        If IsCheckBoxSelected Then permissionStatus = True
        groupId = ddlGroups.SelectedValue

        'Get Permission ID 
        permissionId = CheckBoxListObj.Items(Index).Value
        affectedRows = TDataObjects.TalentDefinitionsSettings.TblAgentGroupPermissions.SavePermissionsForGroup(groupId, permissionId, permissionStatus)
        If (affectedRows > 0) Then LoadPermissions()
    End Sub

    Protected Sub btnSaveNewGroup_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim groupAdded As Boolean = False
        Dim groupId As Integer = 0
        groupAdded = TDataObjects.TalentDefinitionsSettings.InsertGroup(txtNewGroupName.Text.Trim(), ddlBasedOnGroups.SelectedValue, groupId)
        If (groupAdded) Then
            LoadGroups()
            txtNewGroupName.Text = String.Empty
            LoadCategories()
            LoadPermissions()
        End If
    End Sub

    Protected Sub btnUpdatePermissions_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim err As ErrorObj
        Dim cacheDependancyFileName As String = String.Empty
        plhUpdateGroupErrorMessage.Visible = False
        plhUpdateGroupSuccessMessage.Visible = False
        cacheDependancyFileName = GlobalConstants.AGENT_PERMISSIONS_CACHEKEY_PREFIX & ddlGroups.SelectedValue

        err = TCUtilities.ClearCacheDependencyOnAllServers(BusinessUnit, cacheDependancyFileName, ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString())
        If err.HasError Then
            plhUpdateGroupErrorMessage.Visible = True
            ltlUpdateGroupErrorMessage.Text = _wfrPage.Content("UpdatedGroupPermissionsError", _languageCode, True).Replace("<<ERROR_MESSAGE>>", err.ErrorMessage)
        Else
            plhUpdateGroupSuccessMessage.Visible = True
            ltlUpdateGroupSuccessMessage.Text = _wfrPage.Content("UpdatedGroupPermissionsSuccess", _languageCode, True)
        End If
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Load Agent Groups, Based on Groups dropdownlists
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadGroups()
        Dim err As New ErrorObj
        _agentGroups = TDataObjects.TalentDefinitionsSettings.GetAllGroupsWithTypes(False)
        If _agentGroups IsNot Nothing Then

            ' Based On
            ddlBasedOnGroups.DataSource = _agentGroups
            ddlBasedOnGroups.DataTextField = "GROUP_NAME"
            ddlBasedOnGroups.DataValueField = "GROUP_ID"
            ddlBasedOnGroups.DataBind()
            ddlBasedOnGroups.Items.Insert(0, New ListItem(PleaseSelectGroup, "0"))

            ' Groups
            ddlGroups.DataSource = _agentGroups
            ddlGroups.DataTextField = "GROUP_NAME"
            ddlGroups.DataValueField = "GROUP_ID"
            ddlGroups.DataBind()
            ddlGroups.Items.Insert(0, New ListItem(PleaseSelectGroup, "0"))
        End If
    End Sub

    ''' <summary>
    ''' Load categories 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadCategories()
        Dim Categories As DataTable = Nothing
        Categories = TDataObjects.TalentDefinitionsSettings.TblAgentPermissionCategories.GetAllCategories()

        If Categories IsNot Nothing Then
            ddlCategories.DataSource = Categories
            ddlCategories.DataTextField = "CATEGORY_NAME"
            ddlCategories.DataValueField = "CATEGORY_ID"
            ddlCategories.DataBind()
            ' Default dropdown value
            ddlCategories.Items.Insert(0, New ListItem(PleaseSelectCategory, "0"))
        End If
    End Sub

    ''' <summary>
    ''' Load permissions based on group,category selected 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadPermissions()
        Dim Permissions As DataTable = Nothing
        Dim err As New ErrorObj
        Dim categoryId As Integer = 0
        Dim groupId As Integer = 0
        Dim groupIsReadOnly As Boolean = False

        If (ddlGroups.SelectedIndex > 0) Then
            groupId = ddlGroups.SelectedValue
            For Each row As DataRow In _agentGroups.Rows
                If row("GROUP_ID") = groupId Then
                    If row("TYPE_DESC") = GlobalConstants.AGENT_AUTHORITY_SYSTEM_TYPE Then
                        groupIsReadOnly = True
                        Exit For
                    End If
                End If
            Next
        End If
        If (ddlCategories.SelectedIndex > 0) Then
            categoryId = ddlCategories.SelectedValue
        End If

        If groupId <= 0 OrElse categoryId <= 0 Then
            chkListPermissions.Items.Clear()
        Else
            Permissions = TDataObjects.TalentDefinitionsSettings.GetPermissionsByCategoryAndGroup(groupId, categoryId, False)
            If Permissions IsNot Nothing Then
                chkListPermissions.DataSource = Permissions
                chkListPermissions.DataTextField = "PERMISSION_DESCRIPTION"
                chkListPermissions.DataValueField = "PERMISSION_ID"
                chkListPermissions.DataBind()

                If (Permissions.Rows.Count > 0) Then
                    For i As Integer = 0 To Permissions.Rows.Count - 1
                        With Permissions.Rows(i)
                            If (.Item("IS_CHECKED") = 1) Then
                                chkListPermissions.Items(i).Selected = True
                            End If
                        End With
                    Next
                End If
                chkListPermissions.Enabled = Not groupIsReadOnly
            End If
        End If
        plhUpdateGroupErrorMessage.Visible = False
        plhUpdateGroupSuccessMessage.Visible = False
    End Sub

    ''' <summary>
    ''' Load labels and captions on the page 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadLabels()
        ltlGroupsAndPermissions.Text = _wfrPage.Content("ltlGroupsAndPermissions", _languageCode, True)
        lblCurrentGroup.Text = _wfrPage.Content("lblCurrentGroup", _languageCode, True)
        lblCategory.Text = _wfrPage.Content("lblCategory", _languageCode, True)
        ltlAddNewGroup.Text = _wfrPage.Content("ltlAddNewGroup", _languageCode, True)
        lblNewGroupName.Text = _wfrPage.Content("lblNewGroupName", _languageCode, True)
        lblBasedOn.Text = _wfrPage.Content("lblBasedOn", _languageCode, True)
        btnSaveNewGroup.Text = _wfrPage.Content("btnSaveNewGroup", _languageCode, True)
        LoadingText = _wfrPage.Content("lblLoadingText", _languageCode, True)
        rfvGroupName.ErrorMessage = _wfrPage.Content("errGroupNameMandatory", _languageCode, True)
        PleaseSelectGroup = _wfrPage.Content("lblPleaseSelectGroup", _languageCode, True)
        PleaseSelectCategory = _wfrPage.Content("lblPleaseSelectCategory", _languageCode, True)
        btnUpdatePermissions.Text = _wfrPage.Content("btnUpdateGroupPermissions", _languageCode, True)
    End Sub

#End Region

End Class
