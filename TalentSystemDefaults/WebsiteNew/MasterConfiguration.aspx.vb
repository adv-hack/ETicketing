Imports TalentSystemDefaults
Imports TalentSystemDefaults.DataAccess.ConfigObjects
Imports TalentSystemDefaults.DataEntities
Imports TalentSystemDefaults.DataAccess.DataObjects

Partial Class MasterConfiguration

    Inherits System.Web.UI.Page

    Const SELECT_VALUE As String = "---Select value---"

    Property displayTabs As String()
    Property dataTypes As String()

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        displayTabs = DMFactory.GetDisplayTabs(GetDESettings())
        dataTypes = DMFactory.GetDataTypes()
        If (IsPostBack) Then
            If Request.Params.Get("__EVENTTARGET") = ddlTableName.UniqueID Then
                SetProperties()
                ' reset
                txtDefaultName.Text = String.Empty
            End If
        Else
            Session.Clear()
            SetTableNames()
            Session("selectedRows") = New List(Of Integer)
        End If
    End Sub

    Private Sub SetProperties()
        plhDefaultKey1.Visible = False
        txtDefaultKey1.Text = String.Empty
        plhDefaultKey2.Visible = False
        txtDefaultKey2.Text = String.Empty
        plhDefaultKey3.Visible = False
        txtDefaultKey3.Text = String.Empty
        plhDefaultKey4.Visible = False
        txtDefaultKey4.Text = String.Empty
        plhVariableKey1.Visible = False
        txtVariableKey1.Text = String.Empty
        plhVariableKey2.Visible = False
        txtVariableKey2.Text = String.Empty
        plhVariableKey3.Visible = False
        txtVariableKey3.Text = String.Empty
        plhVariableKey4.Visible = False
        txtVariableKey4.Text = String.Empty
        plhDefaultName.Visible = False
        plhBaseDefinition.Visible = False
        plhLoad.Visible = False
        plhButtons.Visible = False
        plhClassDetails.Visible = False
        plhCompanyCode.Visible = False
        plhColumns.Visible = False
        plhBusinessUnitSelector.Visible = False

        gridConfigData.DataSource = Nothing
        gridConfigData.DataBind()

        Dim tableName As String = ddlTableName.SelectedValue
        If Not tableName = SELECT_VALUE Then
            If (DMFactory.IsiSeriesTable(tableName)) Then
                plhCompanyCode.Visible = True
            Else
                plhBusinessUnitSelector.Visible = True
            End If
            If (DMFactory.EnableSelectedColumn(tableName)) Then
                plhColumns.Visible = True
            End If
            Dim defaultKeyValues As Dictionary(Of String, String) = DMFactory.GetDefaultKeyValues(tableName)

            For Each key In defaultKeyValues.Keys
                Dim value As Boolean = CType(defaultKeyValues(key), Boolean)
                Select Case key
                    Case "DefaultKey1Active"
                        plhDefaultKey1.Visible = value

                    Case "DefaultKey2Active"
                        plhDefaultKey2.Visible = value

                    Case "DefaultKey3Active"
                        plhDefaultKey3.Visible = value

                    Case "DefaultKey4Active"
                        plhDefaultKey4.Visible = value
                End Select
            Next

            Dim variableKeyValues As Dictionary(Of String, String) = DMFactory.GetVariableKeyValues(tableName)

            For Each key In variableKeyValues.Keys
                Dim value As Boolean = CType(variableKeyValues(key), Boolean)
                Select Case key
                    Case "VariableKey1Active"
                        plhVariableKey1.Visible = value
                    Case "VariableKey2Active"
                        plhVariableKey2.Visible = value
                    Case "VariableKey3Active"
                        plhVariableKey3.Visible = value
                    Case "VariableKey4Active"
                        plhVariableKey4.Visible = value
                End Select
            Next
            plhLoad.Visible = True
            plhBaseDefinition.Visible = DMFactory.IsBaseDefinitionEnabled(tableName)
            plhDefaultName.Visible = DMFactory.IsDisplayNameEnabled(tableName)
        End If
    End Sub

    Private Sub SetTableNames()
        Dim values As List(Of String) = New List(Of String)
        values.Add(SELECT_VALUE)
        values.AddRange(DMFactory.GetTableNames())

        ddlTableName.DataSource = values
        ddlTableName.DataBind()
    End Sub

    Protected Sub btnLoad_Click(sender As Object, e As EventArgs) Handles btnLoad.Click
        Dim results As List(Of ConfigurationEntity)
        blErrorMessages.Items.Clear()

        Dim settings As DESettings = GetDESettings()

        Dim dbObject As DBObjectBase = DMFactory.GetDBObject(ddlTableName.SelectedValue, settings)
        Dim defaultKey As String = txtDefaultKey1.Text & txtDefaultKey2.Text & txtDefaultKey3.Text & txtDefaultKey4.Text
        Dim variableKey As String = txtVariableKey1.Text & txtVariableKey2.Text & txtVariableKey3.Text & txtVariableKey4.Text
        Dim defaultName As String = txtDefaultName.Text.Trim
        Dim key As String = "data_" & ddlTableName.SelectedValue & defaultKey & variableKey & defaultName

        If dbObject Is Nothing Then
            gridConfigData.DataSource = Nothing
            gridConfigData.DataBind()

        Else
            If (Session(key) Is Nothing) Then
                Dim defaultKeys() As String = {txtDefaultKey1.Text, txtDefaultKey2.Text, txtDefaultKey3.Text, txtDefaultKey4.Text}
                Dim variableKeys() As String = {txtVariableKey1.Text, txtVariableKey2.Text, txtVariableKey3.Text, txtVariableKey4.Text}
                If (DMFactory.IsiSeriesTable(ddlTableName.SelectedValue)) Then

                    If (plhColumns.Visible) Then
                        results = dbObject.RetrieveAlliSeriesValues(txtCompanyCode.Text, defaultKeys, variableKeys, txtColumns.Text)
                    Else
                        results = dbObject.RetrieveAlliSeriesValues(txtCompanyCode.Text, defaultKeys, variableKeys)
                    End If
                Else
                    results = dbObject.RetrieveAllValues(GetBusinessUnit(), defaultKeys, variableKeys, defaultName)
                End If

                If results.Count > 0 Then
                    settings.VariableKey1 = txtVariableKey1.Text
                    settings.VariableKey2 = txtVariableKey2.Text
                    settings.VariableKey3 = txtVariableKey3.Text
                    settings.VariableKey4 = txtVariableKey4.Text

                    Dim dataObject As New tbl_config_detail(settings)

                    Dim defaultValues() As String = results.Select(Function(c) c.DefaultName).ToArray()

                    If (plhBaseDefinition.Visible AndAlso rbVariableKey1.Checked) Then
                        Dim masterConfigIds() As String = dataObject.GetMasterConfigIds(ddlTableName.SelectedValue, defaultValues)
                        If (masterConfigIds.Length = defaultValues.Length) Then
                            Dim i As Integer = 0
                            For Each config As ConfigurationEntity In results
                                config.MasterConfigId = masterConfigIds(i)
                                i = i + 1
                            Next
                        Else
                            blErrorMessages.Items.Add("No base definitions found")
                            Return
                        End If
                    End If

                    If (plhBaseDefinition.Visible AndAlso rbAll.Checked) Then
                        For Each config As ConfigurationEntity In results
                            config.VariableKey1 = rbAll.Text
                        Next
                    End If

                    Session(key) = results
                End If
            End If

            Dim allItems As List(Of ConfigurationEntity) = GetAllItems()

            If (allItems.Count > 0) Then
                gridConfigData.DataSource = allItems
                gridConfigData.DataBind()
                plhButtons.Visible = True
                plhClassDetails.Visible = True

                ShowHideColumns(allItems)
            End If
        End If
    End Sub

    Private Function GetBusinessUnit() As String
        If rbUK.Checked Then
            Return rbUK.Value
        Else
            Return rbBoxoffice.Value
        End If
    End Function

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        blErrorMessages.Items.Clear()

        If (txtClassName.Text.Trim = String.Empty) Then
            blErrorMessages.Items.Add("Class name is mandatory")
        End If

        If (Not rbShowAsModuleYes.Checked AndAlso Not rbShowAsModuleNo.Checked) Then
            blErrorMessages.Items.Add("Show as module selection is mandatory")
        End If

        If (rbShowAsModuleYes.Checked AndAlso String.IsNullOrEmpty(txtModuleTitle.Text.Trim)) Then
            blErrorMessages.Items.Add("Module title is mandatory")
        End If

        If (blErrorMessages.Items.Count > 0) Then
            plhErrorList.Visible = True
            Return
        End If

        Dim guidGenerator As New GUIDGenerator()
        Dim entities As New List(Of ConfigurationEntity)
        For Each row As GridViewRow In gridConfigData.Rows

        Next

        For Each item As GridViewRow In gridConfigData.Rows
            Dim length = item.Cells.Count
            Dim values(length) As String

            For i As Integer = 0 To (length - 1)
                values(i) = GetValue(item, i)
            Next

            Dim entity = New ConfigurationEntity(values(1), values(2), {values(3), values(4), values(5), values(6)}, {values(7), values(8), values(9), values(10)}, values(11), values(12), values(13), values(14), values(15), values(16), values(17), values(18))
            Dim validationGroup As New List(Of String)
            If (values(19)) Then
                validationGroup.Add("VG.Mandatory")
            End If
            If (values(20)) Then
                validationGroup.Add("VG.MinLength")
            End If
            If (values(21)) Then
                validationGroup.Add("VG.MaxLength")
            End If
            entity.ValidationGroup = validationGroup
            Dim minLengthStr As String = values(22).ToString.Trim
            Dim maxLengthStr As String = values(23).ToString.Trim
            If (Not String.IsNullOrEmpty(minLengthStr)) Then
                entity.MinLength = CType(minLengthStr, Integer)
            End If
            If (Not String.IsNullOrEmpty(maxLengthStr)) Then
                entity.MaxLength = CType(maxLengthStr, Integer)
            End If
            entities.Add(entity)
        Next

        Dim dataObject As New tbl_config_detail(GetDESettings())
        Dim moduleName As String = txtClassName.Text.Trim.Replace("DM", String.Empty)
        If (dataObject.SaveAll(entities.ToArray(), moduleName)) Then
            Session("MasterConfigs") = entities.ToArray()
            Session("DataModule") = New DataModuleClass(txtClassName.Text, txtModuleTitle.Text, rbShowAsModuleYes.Checked)
            Response.Redirect("/MasterConfigurationResult.aspx")
        End If
    End Sub

    Private Function GetValue(ByRef item As GridViewRow, ByVal i As Integer) As Object
        Dim value As Object = Nothing
        Dim control As Control = item.Cells(i).Controls(1)
        If TypeOf control Is TextBox Then
            value = CType(control, TextBox).Text.Trim
        ElseIf TypeOf control Is DropDownList Then
            value = CType(control, DropDownList).SelectedValue
        ElseIf TypeOf control Is CheckBox Then
            value = CType(control, CheckBox).Checked
        End If
        Return value.ToString.Trim
    End Function

    Private Function GetDESettings() As DESettings
        Dim settings As New DESettings
        With settings
            .BusinessUnit = GetBusinessUnit()
            .DefaultBusinessUnit = ConfigurationManager.AppSettings("DefaultBusinessUnit")
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .DestinationDatabase = DestinationDatabase.SQL2005.ToString
        End With

        Return settings
    End Function

    Private Sub ShowHideColumns(ByVal allItems As List(Of ConfigurationEntity))
        Dim hideMasterConfig As Boolean = True
        Dim hideDK1 As Boolean = True
        Dim hideDK2 As Boolean = True
        Dim hideDK3 As Boolean = True
        Dim hideDK4 As Boolean = True
        Dim hideVK1 As Boolean = True
        Dim hideVK2 As Boolean = True
        Dim hideVK3 As Boolean = True
        Dim hideVK4 As Boolean = True

        For Each c As ConfigurationEntity In allItems
            hideMasterConfig = hideMasterConfig And c.MasterConfigId = String.Empty
            hideDK1 = hideDK1 And c.DefaultKey1 = String.Empty
            hideDK2 = hideDK2 And c.DefaultKey2 = String.Empty
            hideDK3 = hideDK3 And c.DefaultKey3 = String.Empty
            hideDK4 = hideDK4 And c.DefaultKey4 = String.Empty
            hideVK1 = hideVK1 And c.VariableKey1 = String.Empty
            hideVK2 = hideVK2 And c.VariableKey2 = String.Empty
            hideVK3 = hideVK3 And c.VariableKey3 = String.Empty
            hideVK4 = hideVK4 And c.VariableKey4 = String.Empty
        Next

        gridConfigData.Columns(2).Visible = Not hideMasterConfig
        gridConfigData.Columns(3).Visible = Not hideDK1
        gridConfigData.Columns(4).Visible = Not hideDK2
        gridConfigData.Columns(5).Visible = Not hideDK3
        gridConfigData.Columns(6).Visible = Not hideDK4
        gridConfigData.Columns(7).Visible = Not hideVK1
        gridConfigData.Columns(8).Visible = Not hideVK2
        gridConfigData.Columns(9).Visible = Not hideVK3
        gridConfigData.Columns(10).Visible = Not hideVK4
    End Sub

    Protected Sub rbShowAsModuleYes_CheckedChanged(sender As Object, e As EventArgs)
        If (rbShowAsModuleYes.Checked) Then
            plhModuleTitle.Visible = True
        End If
    End Sub

    Protected Sub rbShowAsModuleNo_CheckedChanged(sender As Object, e As EventArgs)
        If (rbShowAsModuleNo.Checked) Then
            plhModuleTitle.Visible = False
        End If
    End Sub

    Protected Sub gridConfigData_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        Dim ddlDisplayTab As DropDownList = e.Row.FindControl("ddlDisplayTab")
        If ddlDisplayTab IsNot Nothing Then
            ddlDisplayTab.DataSource = displayTabs
            ddlDisplayTab.DataBind()
        End If

        Dim ddlDataType As DropDownList = e.Row.FindControl("ddlDataType")
        If ddlDataType IsNot Nothing Then
            ddlDataType.DataSource = dataTypes
            ddlDataType.DataBind()
        End If
    End Sub

    Protected Sub cbSelect_CheckedChanged(sender As Object, e As EventArgs)
        Dim checkBox As CheckBox = CType(sender, CheckBox)
        Dim rowIndex = CType(checkBox.Parent.Parent, GridViewRow).RowIndex

        Dim selectedRows As List(Of Integer) = GetSelectedRows()
        If (checkBox.Checked) Then
            selectedRows.Add(rowIndex)
        Else
            selectedRows.Remove(rowIndex)
        End If
    End Sub

    Private Function GetSelectedRows() As List(Of Integer)
        Return Session("selectedRows")
    End Function

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        Dim allItems As List(Of ConfigurationEntity) = GetAllItems()
        Dim selectedRows As List(Of Integer) = Session("selectedRows")
        Dim removeItems As New List(Of ConfigurationEntity)
        If (selectedRows IsNot Nothing) Then
            For Each i As Integer In selectedRows
                removeItems.Add(allItems(i))
            Next
            For Each item As ConfigurationEntity In removeItems
                RemoveItem(item)
            Next

            gridConfigData.DataSource = GetAllItems()
            gridConfigData.DataBind()
            Session("selectedRows") = New List(Of Integer)
        End If
    End Sub

    Protected Function GetAllItems() As List(Of ConfigurationEntity)
        Dim allItems As New List(Of ConfigurationEntity)

        For Each key As String In Session.Keys
            If (key.Contains("data_")) Then
                Dim list As List(Of ConfigurationEntity) = CType(Session(key), List(Of ConfigurationEntity))
                allItems.AddRange(list)
            End If
        Next
        Return allItems
    End Function

    Protected Sub RemoveItem(ByVal item As ConfigurationEntity)
        For Each key As String In Session.Keys
            If (key.Contains("data_")) Then
                Dim list As List(Of ConfigurationEntity) = CType(Session(key), List(Of ConfigurationEntity))
                If (list.Contains(item)) Then
                    list.Remove(item)
                    Return
                End If
            End If
        Next
    End Sub

    Protected Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        Session.Clear()
        plhButtons.Visible = False
        gridConfigData.DataSource = GetAllItems()
        gridConfigData.DataBind()
        Session("selectedRows") = New List(Of Integer)
    End Sub
End Class
