Imports System.Collections.Generic
Imports System.Data
Imports Talent.Common
Imports System.Web
Imports System.Linq

Partial Class Package_ComponentGroupList
    Inherits PageControlBase

#Region "Private Properties"
    Private _wfrPage As New WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage()
    Private _partner As String = String.Empty
    Private _businessUnit As String = String.Empty
    Private Property LiveOrTest() As String
    Private _errMsg As TalentErrorMessages
#End Region

#Region "Constants"
    Const PAGECODE As String = "ComponentGroupList.aspx"
    Const STARTSYMBOL As String = "*"
    Const EDITCOMMANDNAME As String = "Edit"
    Const SETTINGCOMMANDNAME As String = "Setting"
    Const DELETECOMMANDNAME As String = "Delete"
    Const ALL As String = "ALL"
    Const CACHEDEPENDENCYNAMETOCLEAR = "Packages"
#End Region

#Region "Public Properties"

    Public ReadOnly Property BusinessUnit() As String
        Get
            Return Utilities.CheckForDBNull_String(Request.QueryString("BU"))
        End Get
    End Property
    Public ReadOnly Property Partner() As String
        Get
            Return Utilities.CheckForDBNull_String(Request.QueryString("Partner"))
        End Get
    End Property
    Public Property ComponentGroupColumnHeader() As String
    Public Property ComponentGroupTypeColumnHeader() As String
    Public Property SettingsColumnHeader() As String
    Public Property DeleteColumnHeader() As String
    Public Property EditColumnHeader() As String
    Public Property EditButtonText() As String
    Public Property DeleteButtonText() As String
    Public Property SettingButtonText() As String
    Public Shared NothingFoundText As String
    Public Shared Caching As Boolean
    Public Property DeleteConfirmMessage As String


#End Region

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        With _wfrPage
            .BusinessUnit = GlobalConstants.MAINTENANCEBUSINESSUNIT
            .PartnerCode = GlobalConstants.STARALLPARTNER
            .PageCode = PAGECODE
            .KeyCode = PAGECODE
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        End With

        _errMsg = New TalentErrorMessages(_languageCode, GlobalConstants.MAINTENANCEBUSINESSUNIT, GlobalConstants.STARALLPARTNER, ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)
        Form.DefaultButton = btnClearOptions.UniqueID

        Dim querystrings As String = "?" & GetQuerystrings()
        Dim okToRetrieveVouchers As Boolean = False
        blErrorMessages.Items.Clear()
        If Request.QueryString("BU") Is Nothing OrElse Request.QueryString("Partner") Is Nothing Then
            plhErrorList.Visible = True
            blErrorMessages.Items.Add(_wfrPage.Content("NoBUAndpartner", _languageCode, True))
        Else
            If Request.QueryString("BU") IsNot Nothing Then
                _businessUnit = Utilities.CheckForDBNull_String(Request.QueryString("BU"))
                If Request.QueryString("Partner") IsNot Nothing Then
                    _partner = Utilities.CheckForDBNull_String(Request.QueryString("Partner"))
                    okToRetrieveVouchers = True
                End If
            End If

        End If

        

        ComponentGroupColumnHeader = GetTextFromDatabase("ComponentGroupColumnHeaderText")
        ComponentGroupTypeColumnHeader = GetTextFromDatabase("ComponentGroupTypeColumnHeaderText")
        SettingsColumnHeader = GetTextFromDatabase("SettingsColumnHeaderText")
        DeleteColumnHeader = GetTextFromDatabase("DeleteColumnHeaderText")
        EditColumnHeader = GetTextFromDatabase("EditColumnHeaderText")
        SettingButtonText = GetTextFromDatabase("SettingButtonText")
        DeleteButtonText = GetTextFromDatabase("DeleteButtonText")
        EditButtonText = GetTextFromDatabase("EditButtonText")
        DeleteConfirmMessage = GetTextFromDatabase("DeleteConfirmMessage")


        If Not Page.IsPostBack Then
            'hlnkComponentGroup.NavigateUrl = hlnkComponentGroup.NavigateUrl & querystrings
            If okToRetrieveVouchers Then
                ddlComponentGroupType.DataSource = GetComponentGroupTypes()
                ddlComponentGroupType.DataTextField = "Value"
                ddlComponentGroupType.DataValueField = "Key"
                ddlComponentGroupType.DataBind()
                ddlComponentGroupType.SelectedIndex = 0

                plhComponentGroupSearch.Visible = True
                setPageText()
                LoadComponentGroups(String.Empty, ddlComponentGroupType.SelectedValue)
            Else
                pageinstructionsLabel.Text = _wfrPage.Content("PageInstructions_NoBusinessUnit", _languageCode, True)
            End If
        End If

    End Sub

    Protected Sub rptComponentGroup_ItemCommand(source As Object, e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptComponentGroup.ItemCommand
        Select Case (e.CommandName)
            Case EDITCOMMANDNAME
                CallComponentDetailsPage(True, e)
            Case SETTINGCOMMANDNAME
                CallDisplaySettingsPage(e.CommandArgument)
            Case DELETECOMMANDNAME
                DeleteComponentGroup(e.CommandArgument)
            Case Else

        End Select

    End Sub

    Protected Sub btnAddComponentGroup_Click(sender As Object, e As System.EventArgs) Handles btnAddComponentGroup.Click
        CallComponentDetailsPage(False)
    End Sub

    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        If TypeOf sender Is Label Then
            DirectCast(sender, Label).Text = _wfrPage.Content(DirectCast(sender, Label).ID, _languageCode, True)
        ElseIf TypeOf sender Is Button Then
            DirectCast(sender, Button).Text = _wfrPage.Content(DirectCast(sender, Button).ID, _languageCode, True)
        End If
    End Sub

#Region "Private Methods"

    ''' <summary>
    ''' Shows Generic errors/messages on the page.
    ''' </summary>
    ''' <param name="ErrorKey">Key to retreive the error text.</param>
    ''' <remarks></remarks>
    Private Sub ShowMessage(ByVal ErrorKey As String)
        plhErrorList.Visible = True
        blErrorMessages.Items.Clear()
        blErrorMessages.Items.Add(GetTextFromDatabase(ErrorKey))
    End Sub

    ''' <summary>
    ''' Shows Specific errors/messages on the page.
    ''' </summary>
    ''' <param name="ErrorKey">Key to retreive the error text.</param>
    ''' <remarks></remarks>
    Private Sub ShowError(ByVal ErrorKey As String)
        Dim talentErrorMsg As TalentErrorMessage = _errMsg.GetErrorMessage(ErrorKey)
        plhErrorList.Visible = True
        blErrorMessages.Items.Clear()
        blErrorMessages.Items.Add(talentErrorMsg.ERROR_MESSAGE)
    End Sub

    Private Sub DeleteComponentGroup(ByVal ComponentGroupId As Long)
        Dim result As Boolean = True
        Dim talPackage As New TalentPackage
        Dim err As ErrorObj
        Dim ds1 As DataSet

        talPackage.Settings = Utilities.GetSettingsObject
        talPackage.Settings.Cacheing = Caching
        talPackage.Settings.CacheTimeMinutes = CType(_wfrPage.Attribute("ComponentCacheTimeMinutes"), Integer)
        talPackage.DePackages.Mode = OperationMode.Delete
        talPackage.DePackages.ComponentGroupID = ComponentGroupId


        err = talPackage.AddEditDeleteComponentGroup()
        ds1 = talPackage.ResultDataSet

        If err.HasError Then
            ShowError(err.ErrorNumber)
        ElseIf ds1.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
            ShowError(ds1.Tables("StatusResults").Rows(0).Item("ReturnCode").ToString())
        Else
            ClearCacheDependency()
            'talPackage.RemoveItemFromCache(talPackage.Settings.ModuleName & talPackage.Settings.Company)
            'Response.Redirect(Request.RawUrl)
            LoadComponentGroups(String.Empty, ALL)
        End If
    End Sub

    Private Sub CallComponentDetailsPage(Optional ByVal IsEdit As Boolean = False, Optional ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs = Nothing)
        Dim ComponentGroupId As Long = 0
        If (IsEdit) Then
            ComponentGroupId = e.CommandArgument
        End If
        Response.Redirect(String.Format("~/Package/ComponentGroupDetails.aspx?BU={0}&Partner={1}&ComponentGroupId={2}", BusinessUnit, Partner, ComponentGroupId))

    End Sub

    Private Sub CallDisplaySettingsPage(ByVal ComponentGroupId As Long)
        Response.Redirect(String.Format("~/Package/ComponentGroupDisplaySettings.aspx?BU={0}&Partner={1}&ComponentGroupId={2}", BusinessUnit, Partner, ComponentGroupId))
    End Sub

    Private Function GetTextFromDatabase(ByVal sKey As String) As String
        Return _wfrPage.Content(sKey, _languageCode, True)
    End Function

    Private Function GetAttributeFromDatabase(ByVal sKey As String) As String
        Return _wfrPage.Attribute(sKey)
    End Function

    ''' <summary>
    ''' Set the text properties of the page controls
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setPageText()
        Page.Title = GetTextFromDatabase("PageTitle")
        pagetitleLabel.Text = GetTextFromDatabase("PageTitle")
        pageinstructionsLabel.Text = GetTextFromDatabase("PageInstructions")
        lblComponentGroupSearch.Text = GetTextFromDatabase("SearchComponentGroupText")
        lblComponentGroupType.Text = GetTextFromDatabase("SearchComponentGroupTypeText")
        btnClearOptions.Text = GetTextFromDatabase("ClearComponentGroupButtonText")
        btnAddComponentGroup.Text = GetTextFromDatabase("AddComponentGroupButtonText")
        NothingFoundText = GetTextFromDatabase("NothingFoundText")
        aceComponentGroupSearch.CompletionSetCount = Utilities.CheckForDBNull_Int(GetAttributeFromDatabase("ACECompletionSetCount"))
        Caching = Boolean.Parse(GetAttributeFromDatabase("GetComponentGroupCaching"))
        lblNoGroups.Text = GetTextFromDatabase("NoComponentGroupsFound")
    End Sub

    ''' <summary>
    ''' Retreives the BU and Partner values from the querystring.
    ''' </summary>
    ''' <returns>String containing BU and PARTNER values</returns>
    ''' <remarks></remarks>
    Private Function GetQuerystrings() As String
        Dim linkQuerystring As String = String.Empty
        If Not String.IsNullOrWhiteSpace(Request.QueryString("BU")) Then
            linkQuerystring = "BU=" & Request.QueryString("BU").Trim.ToUpper
        Else
            linkQuerystring = "BU=*ALL"
        End If
        If Not String.IsNullOrWhiteSpace(Request.QueryString("Partner")) Then
            linkQuerystring = linkQuerystring & "&Partner=" & Request.QueryString("Partner").Trim.ToUpper
        Else
            linkQuerystring = linkQuerystring & "&Partner=*ALL"
        End If
        Return linkQuerystring
    End Function

    ''' <summary>
    ''' Retreives the Component Groups using the search criteria
    ''' </summary>
    ''' <param name="ComponentGroupDescription">Component Group description as search criteria.</param>
    ''' <param name="ComponentGroupType">Component Group type as search criteria.</param>
    ''' <returns>True if repeater is bound successfully to the datatable else false</returns>
    ''' <remarks></remarks>
    Private Function LoadComponentGroups(ByVal ComponentGroupDescription As String, ByVal ComponentGroupType As String) As Boolean

        Dim result As Boolean = True
        Dim talPackage As New TalentPackage
        Dim err As ErrorObj
        Dim ds1 As DataSet

        talPackage.Settings = Utilities.GetSettingsObject
        talPackage.Settings.Cacheing = Caching
        talPackage.Settings.CacheTimeMinutes = CType(_wfrPage.Attribute("ComponentCacheTimeMinutes"), Integer)

        err = talPackage.GetComponentGroupList()
        ds1 = talPackage.ResultDataSet

        If err.HasError OrElse ds1.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
            ShowMessage("FailedToRetrieveComponentGroup")
        Else

            Dim fullComponentGroupListTable As DataTable = ds1.Tables("ComponentGroupList")
            Dim matchedRows() As DataRow = Nothing
            Try

                If fullComponentGroupListTable.Rows.Count > 0 Then
                    Dim strSelectQuery As StringBuilder = New StringBuilder()
                    strSelectQuery.Append(" 1=1 ")


                    If ComponentGroupDescription.Length > 0 Then
                        strSelectQuery.Append(" AND Description LIKE '%" & Server.HtmlEncode(ComponentGroupDescription) & "%'")
                    End If
                    If Not ComponentGroupType = ALL Then
                        strSelectQuery = strSelectQuery.Append(" AND ComponentGroupType= '" & ComponentGroupType & "'")
                    End If

                    matchedRows = fullComponentGroupListTable.Select(strSelectQuery.ToString())
                    result = bindRepeater(matchedRows, fullComponentGroupListTable)

                End If
            Catch
                result = False
            End Try

        End If

        Return result

    End Function

    ''' <summary>
    ''' Binds the repeater with the component Group datatable.
    ''' </summary>
    ''' <param name="rowsToBind">Array of rows to bind to the repater.</param>
    ''' <param name="allComponentGroupListTable"></param>
    ''' <returns>True if repeater is bound successfully to the datatable else false</returns>
    ''' <remarks></remarks>
    Private Function bindRepeater(ByVal rowsToBind As DataRow(), ByVal allComponentGroupListTable As DataTable) As Boolean
        Dim repeaterBoundSuccessfully As Boolean = False
        Try
            Dim tempTableForRepeater As New DataTable
            Dim tempRow As DataRow = Nothing
            With tempTableForRepeater.Columns
                .Add("ComponentGroupId", GetType(Int64))
                .Add("Description", GetType(String))
                .Add("ComponentGroupType", GetType(String))
            End With
            For Each row As DataRow In rowsToBind
                tempRow = Nothing
                tempRow = tempTableForRepeater.NewRow
                tempRow("ComponentGroupId") = row("ComponentGroupId")
                tempRow("Description") = Server.HtmlDecode(row("Description"))
                tempRow("ComponentGroupType") = row("ComponentGroupType")
                tempTableForRepeater.Rows.Add(tempRow)
            Next
            If tempTableForRepeater.Rows.Count > 0 Then
                repeaterBoundSuccessfully = True
                rptComponentGroup.DataSource = tempTableForRepeater
                rptComponentGroup.DataBind()
            End If
        Catch ex As Exception
            repeaterBoundSuccessfully = False
        End Try
        Return repeaterBoundSuccessfully
    End Function

    ''' <summary>
    ''' Searches and binds the Component Group records to the repeater.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub findComponentGroups()

        If LoadComponentGroups(txtComponentGroupSearch.Text, ddlComponentGroupType.SelectedValue) Then
            plhNoComponentGroups.Visible = False
            plhComponentGroupSearch.Visible = True
        Else
            plhNoComponentGroups.Visible = True
            plhComponentGroupSearch.Visible = False
        End If

    End Sub

    ''' <summary>
    ''' Retreives the distinct Component Group Types.
    ''' </summary>
    ''' <returns>List of distinct component group types.</returns>
    ''' <remarks>It prepares the list of component group types, sorts it and adds 'ALL' option to the list.</remarks>
    Private Function GetComponentGroupTypes() As IDictionary(Of String, String)

        'There will always be 2 groups: TA and CG, hence they are retreived from SQL database 
        Dim ComponentGroupList As New SortedDictionary(Of String, String)
        ComponentGroupList.Add("TA", _wfrPage.Content("TA", _languageCode, False))
        'The drop down list should only contain the travel and accommodation item.  Due to time constraints we have had to drop the component group type
        'ComponentGroupList.Add(_wfrPage.Content("CG", _languageCode, False))
        ComponentGroupList.Add("ALL", ALL)
        'ComponentGroupList.OrderBy(Function(x) x.Value)
        Return ComponentGroupList

    End Function

    ''' <summary>
    ''' Retrieves Component Groups.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>This is a shared function and is used by the AutoCompleteExtender to retreive the records asynchronously. </remarks>
    Private Shared Function GetComponentGroups() As DataTable

        Dim result As Boolean = True
        Dim talPackage As New TalentPackage
        Dim err As ErrorObj
        Dim ds1 As DataSet

        talPackage.Settings = Utilities.GetSettingsObject
        talPackage.Settings.Cacheing = Caching
        talPackage.Settings.CacheTimeMinutes = 30

        err = talPackage.GetComponentGroupList()
        ds1 = talPackage.ResultDataSet

        Return ds1.Tables("ComponentGroupList")

    End Function

    ''' <summary>
    ''' Clears the cache on all servers.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ClearCacheDependency()
        Dim err As ErrorObj
        err = Talent.Common.Utilities.ClearCacheDependencyOnAllServers(BusinessUnit, CACHEDEPENDENCYNAMETOCLEAR, ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString())
        If err.HasError Then
            ShowMessage("FailedToclearCache")
        End If
    End Sub
#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Get the list of component groups descriptions for the auto complete extender
    ''' </summary>
    ''' <param name="prefixText">The text to filter on</param>
    ''' <param name="count">The number of items in the list</param>
    ''' <returns>List of strings to use</returns>
    ''' <remarks></remarks>
    <System.Web.Script.Services.ScriptMethod(), System.Web.Services.WebMethod()> _
    Public Shared Function GetComponentGroupList(ByVal prefixText As String, ByVal count As Integer) As List(Of String)
        Dim ComponentGroupDescList As List(Of String) = New List(Of String)
        Dim allComponentGroupTable As DataTable = GetComponentGroups()
        If allComponentGroupTable.Rows.Count > 0 Then
            Dim matchedRows() As DataRow = Nothing
            If prefixText.StartsWith(STARTSYMBOL) Then
                matchedRows = allComponentGroupTable.DefaultView.ToTable(True, "Description").Select("Description LIKE'%" & prefixText.Replace(STARTSYMBOL, "") & "%'")
            Else
                matchedRows = allComponentGroupTable.DefaultView.ToTable(True, "Description").Select("Description LIKE'%" & prefixText & "%'")
            End If
            If matchedRows.Length > 0 Then
                Dim itemsToBind As Integer = matchedRows.Length - 1
                If count < matchedRows.Length - 1 Then itemsToBind = count - 1
                For rowIndex As Integer = 0 To itemsToBind
                    ComponentGroupDescList.Add(matchedRows(rowIndex)("Description").ToString().Trim())
                Next
            Else
                ComponentGroupDescList.Add(NothingFoundText)
            End If
        End If
        Return ComponentGroupDescList
    End Function
#End Region

End Class