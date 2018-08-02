Imports Talent.Common
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports System.Data

Partial Class UserControls_ProfileAttributes
    Inherits ControlBase
    Public Property BlockGridStyleClass() As String = String.Empty
    Private _ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Protected ColumnHeaderText_Category As String
    Protected ColumnHeaderText_Attribute As String
    Protected customerAttributes As DataTable
    Protected attributeCategories As DataTable
    Protected attributeDefinitions As DataTable


    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        'Check if agent has access on Attributes menu item
        If (AgentProfile.IsAgent And AgentProfile.AgentPermissions.CanAccessAttributes) Or Not AgentProfile.IsAgent Then
            With _ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .PageCode = TEBUtilities.GetCurrentPageName()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "ProfileAttributes.ascx"
            End With
            BlockGridStyleClass = _ucr.Attribute("BlockGridStyleClass")
        Else
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Profile.User Is Nothing OrElse Profile.User.Details Is Nothing Then
            If AgentProfile.IsAgent Then
                Response.Redirect("~/PagesPublic/Profile/CustomerSelection.aspxx")
            Else
                Response.Redirect("~/PagesPublic/Login/Login.aspx")
            End If
        Else
            InitialiseClassLevelFields()

            ClearErrorMessages()

            LoadAttributeCategories()
            LoadCustomerAttributeDataset()

            If Page.IsPostBack = False Then
                AddDDLCategoryOptions(attributeCategories)
            End If

        End If

    End Sub

    Private Sub InitialiseClassLevelFields()

        If AgentProfile.IsAgent Then
            divCustomerAttributeAdd.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_ucr.Attribute("AllowAdd"))
        Else
            divCustomerAttributeAdd.Visible = False
        End If

        lblCategoryDropdown.Text = _ucr.Content("lblCategoryDropDown", _languageCode, True)
        lblAttributeDropdown.Text = _ucr.Content("lblAttributeDropDown", _languageCode, True)
        btnAdd.Text = _ucr.Content("btnAddButton", _languageCode, True)

    End Sub

    Private Sub LoadCustomerAttributeDataset()

        Dim profile As TalentProfile = HttpContext.Current.Profile
        Dim customerNumber As String = profile.User.Details.LoginID

        Dim Err As ErrorObj
        Dim resultSet As DataSet

        Dim tcrm As New TalentCRM()
        With tcrm
            .De.CustomerNumber = customerNumber
            .De.AttributeOperation = "R"
            .Settings = TEBUtilities.GetSettingsObject()
            .Settings.Cacheing = True
            Err = .RetrieveCustomerAttributes()
            resultSet = .ResultDataSet
            If Err.HasError OrElse resultSet Is Nothing OrElse resultSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows(0).Item("ErrorOccurred").ToString() = GlobalConstants.ERRORFLAG OrElse resultSet.Tables("CustomerAttributes").Rows.Count = 0 Then
                divCustomerAttributesRepeater.Visible = False
                plhErrorList.Visible = True
                blErrorMessages.Items.Add(_ucr.Content("msgNoResultsText", _languageCode, True))
            Else
                If resultSet.Tables.Contains("CustomerAttributes") Then
                    Me.customerAttributes = resultSet.Tables("CustomerAttributes")
                    If AgentProfile.IsAgent Then
                        AppendDefaultCategories(customerAttributes)
                    Else
                        FilterPWSCustomerAttributes(customerAttributes)
                    End If

                    'Filter to get distinct categories
                    Dim view As New DataView(customerAttributes)
                    rptCustomerAttributes.DataSource = view.ToTable(True, "CategoryCode", "CategoryDescription")
                    rptCustomerAttributes.DataBind()
                    ColumnHeaderText_Category = _ucr.Content("HeaderText_Category", _languageCode, True)
                    ColumnHeaderText_Attribute = _ucr.Content("HeaderText_Attribute", _languageCode, True)
                    divCustomerAttributesRepeater.Visible = True
                    plhErrorList.Visible = False
                Else
                    divCustomerAttributesRepeater.Visible = False
                    plhErrorList.Visible = True
                    blErrorMessages.Items.Add(_ucr.Content("msgNoResultsText", _languageCode, True))
                End If
            End If
        End With
    End Sub

    Private Sub FilterPWSCustomerAttributes(dataTable As DataTable)
        Dim rows As DataRowCollection = dataTable.Rows
        For i As Integer = rows.Count - 1 To 0 Step -1
            Dim row As DataRow = rows(i)
            If Not IsCategoryDisplayInPWS(row("CategoryCode")) Then
                rows.Remove(row)
            End If
        Next
    End Sub

    Private Sub AppendDefaultCategories(customerAttributes As DataTable)
        Dim rows() As DataRow = Me.attributeCategories.Select("DisplayByDefault='1'")

        If rows.Length > 0 Then
            For i As Integer = 0 To rows.Length - 1 Step 1
                Dim categoryCode As String = rows(i).Item("AttributeCategorySequence")
                Dim categoryDescription As String = rows(i).Item("AttributeCategoryDescription")

                Dim filter As String = "CategoryCode='" & categoryCode & "'"

                If (customerAttributes.Select(filter).Length = 0) Then
                    Dim row As DataRow = customerAttributes.NewRow
                    row("CategoryCode") = categoryCode
                    row("CategoryDescription") = categoryDescription
                    row("AttributeCode") = String.Empty
                    row("AttributeDescription") = "&nbsp;"
                    row("AttributeKey") = -1
                    customerAttributes.Rows.Add(row)
                End If
            Next
        End If
    End Sub

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim categoryCode As String = ddlCategory.SelectedValue
        Dim attributeCode As String = ddlAttribute.SelectedValue

        If AttributeNotSelected(categoryCode) OrElse AttributeNotSelected(attributeCode) Then
            ShowErrorMessage("msgMandatoryAttributeForAdd")
            Return
        End If

        Dim categoryText As String = ddlCategory.SelectedItem.Text
        Dim attributeText As String = ddlAttribute.SelectedItem.Text

        Dim canReplace As Boolean = False

        If IsCategoryOutputOnly(categoryCode) Then
            ShowErrorMessage("msgCategoryOutputOnly")
            Return
        End If

        Dim attributeCount As Integer = GetAttributeCount(categoryCode)
        Dim attributeNewCount As Integer = attributeCount + 1
        Dim min As Integer = GetAttributeMinCount(categoryCode)
        Dim max As Integer = GetAttributeMaxCount(categoryCode)

        If IsMaxLimitReached(attributeNewCount, max) Then
            If (min = max AndAlso max = attributeCount) Then
                canReplace = True
            Else
                ShowErrorMessage("msgMaxAttributeError", categoryText, max)
                Return
            End If
        End If

        If AttributeAlreadyExists(attributeCode) Then
            ShowErrorMessage("msgDuplicateAttribute")
            Return
        End If

        If (canReplace) Then
            Dim attribute As String() = GetAttribute(categoryCode)
            Dim attributeId As String = GetAttributeDefinitionID(attributeCode, categoryCode)

            ProcessAttribute(attribute(0), categoryCode, attribute(1), "D")
            ProcessAttribute(attributeCode, categoryCode, attributeId, "C")

            ShowErrorMessage("msgAttributeReplaced", categoryText, attribute(2), attributeText)
        Else
            Dim attributeId As String = GetAttributeDefinitionID(attributeCode, categoryCode)
            ProcessAttribute(attributeCode, categoryCode, attributeId, "C")
        End If

        LoadCustomerAttributeDataset()
    End Sub

    Private Sub LoadAttributeCategories()

        Dim profile As TalentProfile = HttpContext.Current.Profile
        Dim customerNumber As String = profile.User.Details.LoginID

        Dim DeCRM As New DETalentCRM()
        DeCRM.CustomerNumber = customerNumber

        Dim Err As ErrorObj
        Dim resultSet As DataSet

        Dim deSettings As New DESettings
        deSettings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        deSettings.BusinessUnit = TalentCache.GetBusinessUnit()
        deSettings.StoredProcedureGroup = TEBUtilities.GetStoredProcedureGroup()
        deSettings.Cacheing = True
        deSettings.OriginatingSourceCode = "W"

        Dim tcrm As New TalentCRM()
        With tcrm
            .De = DeCRM
            .Settings = deSettings
            Err = .RetrieveAttributeCategories()
            resultSet = .ResultDataSet


            If Err.HasError Then
                ' Handle errors
            Else
                Me.attributeCategories = resultSet.Tables("AttributeCategories")
            End If
        End With

    End Sub

    Protected Sub ddlCategory_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)

        If (ddlCategory.SelectedValue = "-1") Then
            ddlAttribute.Items.Clear()
            Return
        End If

        GetAttributeDefinitions(True)

    End Sub

    Protected Sub rptCustomerAttributes_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs)

        If e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem Then
            Return
        End If

        Dim dv As DataRowView = DirectCast(e.Item.DataItem, DataRowView)
        Dim rptAttributes As Repeater = DirectCast(e.Item.FindControl("rptAttribute"), Repeater)
        If dv IsNot Nothing AndAlso dv.DataView IsNot Nothing Then

            Dim filter As String = "CategoryCode = " & DirectCast(DataBinder.Eval(e.Item.DataItem, "CategoryCode"), Decimal)
            rptAttributes.DataSource = Me.customerAttributes.Select(filter)
            rptAttributes.DataBind()
        End If

        Dim categoryCode As Decimal = DirectCast(DataBinder.Eval(e.Item.DataItem, "CategoryCode"), Decimal)
        Dim lblErrorMessage = DirectCast(e.Item.FindControl("lblErrorMessage"), Label)

        If (AgentProfile.IsAgent) Then ValidateCategory(categoryCode, lblErrorMessage)
    End Sub

    Private Sub ProcessAttribute(ByVal attributeCode As String, ByVal categoryCode As String, ByVal attributeID As String, ByVal attributeOperation As String)

        Dim profile As TalentProfile = HttpContext.Current.Profile
        Dim customerNumber As String = String.Empty
        Dim userName As String = String.Empty
        If profile.User IsNot Nothing AndAlso profile.User.Details IsNot Nothing Then
            customerNumber = profile.User.Details.LoginID
            If AgentProfile.IsAgent Then
                userName = AgentProfile.Name
            End If
        End If

        Dim DeCRM As New DETalentCRM()
        DeCRM.CustomerNumber = customerNumber
        DeCRM.UserName = userName
        DeCRM.AttributeCategoryCode = categoryCode
        DeCRM.AttributeCode = attributeCode
        DeCRM.AttributeID = attributeID
        DeCRM.AttributeOperation = attributeOperation
        If DeCRM.AttributeOperation.Trim.Length = 0 Then DeCRM.AttributeOperation = "R"

        Dim Err As ErrorObj

        Dim deSettings As New DESettings
        deSettings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        deSettings.BusinessUnit = TalentCache.GetBusinessUnit()
        deSettings.StoredProcedureGroup = TEBUtilities.GetStoredProcedureGroup()
        deSettings.Cacheing = True
        deSettings.OriginatingSourceCode = "W"

        Dim tcrm As New TalentCRM()
        With tcrm
            .De = DeCRM
            .Settings = deSettings
            If DeCRM.AttributeOperation = "C" Or DeCRM.AttributeOperation = "D" Then
                .RetrieveCustomerAttributesClearCache()
            End If
            Err = .RetrieveCustomerAttributes()

            If Err.HasError Then
                ' Handle errors
            End If
        End With

    End Sub

    Protected Sub rptAttribute_ItemCommand(ByVal source As Object, ByVal e As RepeaterCommandEventArgs)

        If e.CommandName = "ProcessDelete" Then
            Dim categoryCode As String = CType(e.Item.FindControl("hidCategoryCode"), HiddenField).Value
            Dim attributeCode As String = CType(e.Item.FindControl("hidAttributeCode"), HiddenField).Value
            Dim attributeID As String = CType(e.Item.FindControl("hidAttributeID"), HiddenField).Value

            Dim attributeCount As Integer = GetAttributeCount(categoryCode) - 1
            Dim min As Integer = GetAttributeMinCount(categoryCode)

            Dim categoryDescription = GetCategoryDescription(categoryCode)

            If IsMinLimitReached(attributeCount, min) Then
                ShowErrorMessage("msgMinAttributeError", min, categoryDescription)
                Return
            End If

            ProcessAttribute(attributeCode, categoryCode, attributeID, "D")

            LoadCustomerAttributeDataset()
        End If

    End Sub

    Protected Sub rptAttribute_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs)
        Dim visibility As Boolean = False

        If AgentProfile.IsAgent Then
            Dim categoryCode As String = CType(e.Item.FindControl("hidCategoryCode"), HiddenField).Value
            Dim attributeCode As String = CType(e.Item.FindControl("hidAttributeCode"), HiddenField).Value
            Dim attributeID As String = CType(e.Item.FindControl("hidAttributeID"), HiddenField).Value

            Dim outputOnly As Boolean = IsCategoryOutputOnly(categoryCode)
            Dim allowDelete As Boolean = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_ucr.Attribute("AllowDelete"))

            If Not attributeCode = String.Empty AndAlso Not outputOnly AndAlso allowDelete Then
                visibility = True
            End If
        End If

        e.Item.FindControl("plhDelete").Visible = visibility
    End Sub

    Private Function AttributeNotSelected(attributeCode As String) As Boolean
        Return attributeCode Is Nothing OrElse attributeCode = "" OrElse attributeCode = "-1"
    End Function

    Private Function AttributeAlreadyExists(ByVal attributeCode As String) As Boolean

        Dim filter As String = "AttributeCode='" & attributeCode & "'"
        If Me.customerAttributes Is Nothing Then
            Return False
        Else
            Dim rows() As DataRow = Me.customerAttributes.Select(filter)
            Return rows.Length > 0
        End If
    End Function

    Private Function IsCategoryOutputOnly(ByVal categoryCode As String) As Boolean
        Dim result As Boolean = False

        Dim filter As String = "AttributeCategorySequence='" & categoryCode & "'"
        Dim rows() As DataRow = Me.attributeCategories.Select(filter)

        If rows.Length = 1 Then
            Dim row As DataRow = rows(0)
            result = (row("OutputOnly") = 1)
            If Trim(row.Item("AttributeType")).ToLower = ("SYS").ToLower Then
                If (AgentProfile.AgentPermissions.CanAmendSystemAttributesForCustomers) Then
                    result = 0
                Else
                    result = 1
                End If
            End If
        End If

        Return result
    End Function

    Private Function IsMaxLimitReached(ByVal attributeCount As Integer, ByVal max As Integer) As Boolean
        Dim result As Boolean = False

        If Not max = -1 And Not max = 0 Then
            result = (attributeCount > max)
        End If

        Return result
    End Function

    Private Function IsMinLimitReached(ByVal attributeCount As Integer, ByVal min As Integer) As Boolean
        Dim result As Boolean = False

        If Not min = -1 Then
            result = (attributeCount < min)
        End If

        Return result
    End Function

    Private Function GetAttributeMaxCount(ByVal categoryCode As String) As Integer
        Dim result As Integer = -1
        Dim filter As String = "AttributeCategorySequence='" & categoryCode & "'"
        Dim rows() As DataRow = Me.attributeCategories.Select(filter)

        If rows.Length = 1 Then
            Dim row As DataRow = rows(0)
            If Not String.IsNullOrEmpty(row("MaxCount")) Then
                result = row("MaxCount")
            End If
        End If
        Return result
    End Function

    Private Function GetAttributeMinCount(ByVal categoryCode As String) As Integer
        Dim result As Integer = -1
        Dim filter As String = "AttributeCategorySequence='" & categoryCode & "'"
        Dim rows() As DataRow = Me.attributeCategories.Select(filter)

        If rows.Length = 1 Then
            Dim row As DataRow = rows(0)
            If Not String.IsNullOrEmpty(row("MinCount")) Then
                result = row("MinCount")
            End If
        End If

        Return result
    End Function

    Private Function GetAttributeCount(ByVal categoryCode As String) As Integer
        Dim filter As String = "CategoryCode='" & categoryCode & "' AND AttributeCode <> ''"
        If Me.customerAttributes Is Nothing Then
            Return 0
        Else
            Dim rows() As DataRow = Me.customerAttributes.Select(filter)
            Return rows.Length
        End If
    End Function

    Private Function GetCategoryDescription(ByVal categoryCode As String) As String
        Dim result As String = String.Empty
        Dim filter As String = "AttributeCategorySequence='" & categoryCode & "'"
        Dim rows() As DataRow = Me.attributeCategories.Select(filter)

        If rows.Length = 1 Then
            Dim row As DataRow = rows(0)
            result = row("AttributeCategoryDescription")
        End If

        Return result
    End Function

    Private Sub GetAttributeDefinitions(ByVal boolLoadDDL As Boolean)
        Dim DeCRM As New DETalentCRM()
        DeCRM.AttributeCategoryCode = ddlCategory.SelectedValue.ToString

        Dim Err As ErrorObj
        Dim resultSet As DataSet

        Dim deSettings As New DESettings
        deSettings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        deSettings.BusinessUnit = TalentCache.GetBusinessUnit()
        deSettings.StoredProcedureGroup = TEBUtilities.GetStoredProcedureGroup()
        deSettings.Cacheing = True
        deSettings.OriginatingSourceCode = "W"

        Dim tcrm As New TalentCRM()
        With tcrm
            .De = DeCRM
            .Settings = deSettings
            Err = .RetrieveAttributeDefinition()
            resultSet = .ResultDataSet

            If Err.HasError Then
                ' Handle errors
            Else
                Me.attributeDefinitions = resultSet.Tables("AttributeDefinitions")
                If boolLoadDDL Then AddDDLAttributeOptions(resultSet.Tables("AttributeDefinitions"))
            End If

        End With
    End Sub

    Private Function GetAttributeDefinitionID(ByVal attributeCode As String, categoryCode As String) As String
        Dim result As String = String.Empty

        GetAttributeDefinitions(False)

        Dim filter As String = "AttributeCode='" & attributeCode & "' AND AttributeCategoryCode='" & categoryCode & "'"
        Dim rows() As DataRow = Me.attributeDefinitions.Select(filter)

        If rows.Length = 1 Then
            Dim row As DataRow = rows(0)
            result = row("AttributeDefinitionID")
        End If
        Return result
    End Function

    Private Function GetAttribute(categoryCode As String) As String()
        Dim result(3) As String

        Dim filter As String = "CategoryCode = '" & categoryCode & "'"
        Dim rows() As DataRow = Me.customerAttributes.Select(filter)

        Dim minDate As Integer = Integer.MaxValue

        For Each r As DataRow In rows
            Dim dt As Integer = Integer.Parse(r("UpdatedDate"))

            If (dt < minDate) Then
                minDate = dt
            End If
        Next

        filter = "CategoryCode = '" & categoryCode & "'" & " AND UpdatedDate='" & minDate & "'"
        rows = Me.customerAttributes.Select(filter)

        Dim minTime As Integer = Integer.MaxValue
        Dim row As DataRow = Nothing

        For Each r As DataRow In rows
            Dim time As Integer = Integer.Parse(r("UpdatedTime"))
            If (time < minTime) Then
                minTime = time
                row = r
            End If
        Next

        If row IsNot Nothing Then
            result(0) = row("AttributeCode")
            result(1) = row("AttributeKey")
            result(2) = row("AttributeDescription")
        End If

        Return result
    End Function

    Private Function IsCategoryDisplayInPWS(ByVal categoryCode As String) As Boolean
        Dim result As Boolean = False
        Dim filter As String = "AttributeCategorySequence='" & categoryCode & "'"
        Dim rows() As DataRow = Me.attributeCategories.Select(filter)

        If rows.Length = 1 Then
            Dim row As DataRow = rows(0)
            result = (row("DisplayInPWS") = 1)
        End If

        Return result
    End Function

    Private Sub ClearErrorMessages()
        plhErrorList.Visible = False
        blErrorMessages.Items.Clear()
    End Sub

    Private Sub ShowErrorMessage(ByVal messageCode As String, ByVal ParamArray params As String())
        plhErrorList.Visible = True
        blErrorMessages.Items.Add(String.Format(_ucr.Content(messageCode, _languageCode, True), params))
    End Sub

    Private Sub ValidateCategory(categoryCode As String, lblErrorMessage As Label)
        Dim categoryDescription As String = GetCategoryDescription(categoryCode)
        Dim attributeCount As Integer = GetAttributeCount(categoryCode)
        Dim max As Integer = GetAttributeMaxCount(categoryCode)
        Dim min As Integer = GetAttributeMinCount(categoryCode)

        If IsMaxLimitReached(attributeCount, max) Then
            lblErrorMessage.Visible = True
            lblErrorMessage.Text = String.Format(_ucr.Content("msgMaxAttributeError", _languageCode, True), categoryDescription, max)

        ElseIf IsMinLimitReached(attributeCount, min) Then
            lblErrorMessage.Visible = True
            lblErrorMessage.Text = String.Format(_ucr.Content("msgMinAttributeError", _languageCode, True), min, categoryDescription)
        End If
    End Sub

    Private Sub AddDDLCategoryOptions(dt As DataTable)
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            AddDDLCategoryOption(_ucr.Content("PleaseSelectOptionText", _languageCode, True), "-1")
            For Each row As DataRow In dt.Rows
                If Trim(row.Item("AttributeType")).ToLower = ("SYS").ToLower Then
                    If (AgentProfile.AgentPermissions.CanAmendSystemAttributesForCustomers) Then
                        AddDDLCategoryOption(row("AttributeCategoryDescription"), row("AttributeCategorySequence"))
                    End If
                Else
                    AddDDLCategoryOption(row("AttributeCategoryDescription"), row("AttributeCategorySequence"))
                End If

            Next
        End If
    End Sub

    Private Sub AddDDLAttributeOptions(dt As DataTable)
        ddlAttribute.Items.Clear()
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            AddDDLAttributeOption(_ucr.Content("PleaseSelectOptionText", _languageCode, True), "-1")
            For Each row As DataRow In dt.Rows
                AddDDLAttributeOption(row("AttributeDescription"), row("AttributeCode"))
            Next
        Else
            AddDDLAttributeOption(_ucr.Content("NoAttributesOptionText", _languageCode, True), "-1")
        End If
    End Sub

    Private Sub AddDDLCategoryOption(ByVal optionText As String, ByVal optionValue As String)
        AddDDLOption(ddlCategory, optionText, optionValue)
    End Sub


    Private Sub AddDDLAttributeOption(ByVal optionText As String, ByVal optionValue As String)
        AddDDLOption(ddlAttribute, optionText, optionValue)
    End Sub

    Private Sub AddDDLOption(ByRef ddl As DropDownList, ByVal optionText As String, ByVal optionValue As String)
        If Not String.IsNullOrEmpty(optionText) Then
            Dim lstitem As New ListItem
            lstitem.Text = optionText
            lstitem.Value = optionValue
            ddl.Items.Add(lstitem)
            lstitem = Nothing
        End If
    End Sub
End Class
