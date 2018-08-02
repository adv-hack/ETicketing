Imports System.Collections.Generic
Imports System.Data
Imports Talent.Common
Imports System.Web
Imports Talent.Common.UtilityExtension
Imports System.Linq


Partial Class Package_ComponentGroupDetails
    Inherits PageControlBase

#Region "Constants"
    Const PAGECODE As String = "ComponentGroupDetails.aspx"
    Const STARTSYMBOL As String = "*"
    Const NOOFQUANTITYTOSELECTINADDMODE = 0
    Const CACHEDEPENDENCYNAMETOCLEAR = "Packages"
    Const COMPONENTDESCRIPTION As String = "ComponentDescription"
    Const COMPONENTID As String = "ComponentId"
    Const AREANOTREQUIREDTEXT As String = "Not Required"
    Const AREANOTREQUIREDVALUE As String = "NotR"
#End Region

#Region "Private Properties"
    Private _wfrPage As New WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage()
    Private Shared _partner As String = String.Empty
    Private Shared _businessUnit As String = String.Empty
    Private Shared MinTextValue As String
    Private Shared MaxTextValue As String
    Private Property LiveOrTest() As String
    Private ComponentGroupID As Long = 0
    Private _errMsg As TalentErrorMessages
#End Region

#Region "Public Properties"

    Public Shared Caching As Boolean

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


    Public Shared NothingFoundText As String

#End Region

#Region "Protected Methods"
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        With _wfrPage
            .BusinessUnit = GlobalConstants.MAINTENANCEBUSINESSUNIT
            .PartnerCode = GlobalConstants.STARALLPARTNER
            .PageCode = PAGECODE
            .KeyCode = PAGECODE
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        End With
        hlnkComponent.NavigateUrl = String.Format("~/Package/ComponentGroupList.aspx?BU={0}&Partner={1}", BusinessUnit, Partner)
        hlnkComponent.Text = _wfrPage.Content("ComponentGroupListPage", _languageCode, True)
        _errMsg = New TalentErrorMessages(_languageCode, GlobalConstants.MAINTENANCEBUSINESSUNIT, GlobalConstants.STARALLPARTNER, ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)
        Dim querystrings As String = "?" & GetQuerystrings()
        Dim okToRetrieveVouchers As Boolean = False
        If Request.QueryString("BU") IsNot Nothing Then
            _businessUnit = Utilities.CheckForDBNull_String(Request.QueryString("BU"))
            If Request.QueryString("Partner") IsNot Nothing Then
                _partner = Utilities.CheckForDBNull_String(Request.QueryString("Partner"))
                okToRetrieveVouchers = True
            End If
        End If

        If Not Request.QueryString("ComponentGroupId") Is Nothing Then
            ComponentGroupID = Long.Parse(Request.QueryString("ComponentGroupId").ToString())
        End If

        '/****************************
        'When a user control needs to be created for a new component group, refer to either TravelAndAccommodation or ComponentGroup user controls.
        'The user controls do not interact with any model. The model is provided by the parent page (this page) and all the operations
        'are executed from this parent page. The user control only needs to inform this controller class what it wants to do with the model
        'and then raise the event pasing the information. This class takes care of Add/Delete/Amend/Edit operation with the model and then refreshes the
        'model of the user control.
        '/****************************
        'Only one event handler will handle the operation performed by the usercontrols.
        'If any new component groups are added and therefore any new user control is developed in the future,
        'the user control should declare this event and raise the event to handle the operation.
        'There is no need to handle the edit/delete/add operation separately.
        AddHandler CG.ComponentsInComponentGroupChangedEvent, AddressOf ComponentsInComponentGroupChanged
        AddHandler TA.ComponentsInComponentGroupChangedEvent, AddressOf ComponentsInComponentGroupChanged
        AddHandler TA.GetAllAreasForComponentEvent, AddressOf GetAllAreasForComponent

        If Not Page.IsPostBack Then
            If okToRetrieveVouchers Then
                SetPageText()
                'ddlNumberOfComponentsToSelect.DataSource = GetNumberOfComponentsToSelect()
                'ddlNumberOfComponentsToSelect.DataTextField = "Value"
                'ddlNumberOfComponentsToSelect.DataValueField = "Key"
                'ddlNumberOfComponentsToSelect.DataBind()
                If Not ComponentGroupID = 0 Then
                    'Edit mode - user cannot change the type
                    GetComponentGroupDetails()
                    ddlComponentGroupType.Enabled = False
                    plhValidation.Visible = True    'No Of components are visible only in edit mode
                Else
                    'Add Mode
                    ddlComponentGroupType.DataSource = GetComponentGroupTypes()
                    ddlComponentGroupType.DataTextField = "Value"
                    ddlComponentGroupType.DataValueField = "Key"
                    ddlComponentGroupType.DataBind()
                    ddlComponentGroupType.SelectedIndex = 0
                    ddlComponentGroupType.Enabled = True
                    txtComponentGroupCode.ReadOnly = False 'The component group is entered by the user in add mode
                    phlCG.Visible = False   ' In add mode, the usercontrols should be invisible
                    phlTA.Visible = False
                End If


                'AssignErorMessages()
                'FillValues()
            End If
        End If
    End Sub

    Protected Sub btnCancelComponentGroupAddInsert_Click(sender As Object, e As System.EventArgs) Handles btnCancelComponentGroupAddInsert.Click
        Response.Redirect(String.Format("~/Package/ComponentGroupList.aspx?BU={0}&Partner={1}", BusinessUnit, Partner))
    End Sub

    ''' <summary>
    ''' Called when the user adds/updates a component group.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnAddOrUpdateComponentGroup_Click(sender As Object, e As System.EventArgs) Handles btnAddOrUpdateComponentGroup.Click
        Dim talPackage As New TalentPackage
        Dim err As ErrorObj
        Dim ds1 As DataSet
        Dim compDefaults As New List(Of ComponentDefault)

        talPackage.Settings = Utilities.GetSettingsObject
        'talPackage.Settings.BackOfficeConnectionString = TDataObjects.AppVariableSettings.TblDatabaseVersion.TalentAdminDatabaseConnectionString()
        talPackage.Settings.Cacheing = Caching
        talPackage.Settings.CacheTimeMinutes = CType(_wfrPage.Attribute("ComponentCacheTimeMinutes"), Integer)
        If ComponentGroupID = 0 Then
            talPackage.DePackages.Mode = OperationMode.Add
            talPackage.DePackages.QuantityToSelect = NOOFQUANTITYTOSELECTINADDMODE   'Initially no quantity needs to be selected in add mode
        Else
            talPackage.DePackages.Mode = OperationMode.Edit
            talPackage.DePackages.QuantityToSelect = 0 'ddlNumberOfComponentsToSelect.SelectedValue.ConvertSelectionToIntegerValue()
        End If
        talPackage.DePackages.ComponentGroupID = ComponentGroupID
        talPackage.DePackages.ComponentCode = Server.HtmlEncode(txtComponentGroupCode.Text.Trim())
        talPackage.DePackages.ComponentGroupDescription = Server.HtmlEncode(txtComponentGroupDescription.Text.Trim())

        talPackage.DePackages.ComponentType = ddlComponentGroupType.SelectedValue
        talPackage.DePackages.NoneReqOpt = ddlNoneReqOpt.SelectedValue

        If talPackage.DePackages.Mode = OperationMode.Add Then
        ElseIf talPackage.DePackages.Mode = OperationMode.Edit Then
            If talPackage.DePackages.ComponentType = ComponentGroups.CG Then
                PrepareComponentDefaultObject(False, Utilities.CheckForDBNull_Int(txtQuantityToComponentRatioFrom.Text.Trim), Utilities.CheckForDBNull_Int(txtQuantityToComponentRatioTo.Text.Trim),
                                              ddlQuantityToComponentRatio.SelectedValue.ConvertISeriesDefaultTypeToEnum(), compDefaults)

                talPackage.DePackages.ComponentDefaults = compDefaults
            ElseIf talPackage.DePackages.ComponentType = ComponentGroups.TA Then
                PrepareComponentDefaultObject(False, Utilities.CheckForDBNull_Int(txtQuantityToRoomRatioFrom.Text.Trim), Utilities.CheckForDBNull_Int(txtQuantityToRoomRatioTo.Text.Trim),
                                              ddlQuantityToRoomRatio.SelectedValue.ConvertISeriesDefaultTypeToEnum(), compDefaults)

                PrepareComponentDefaultObject(False, Utilities.CheckForDBNull_Int(txtQuantityToPeopleRatioFrom.Text.Trim), Utilities.CheckForDBNull_Int(txtQuantityToPeopleRatioTo.Text.Trim),
                                              ddlQuantityToPeopleRatio.SelectedValue.ConvertISeriesDefaultTypeToEnum(), compDefaults)

                PrepareComponentDefaultObject(False, Utilities.CheckForDBNull_Int(txtQuantityToCarRatioFrom.Text.Trim), Utilities.CheckForDBNull_Int(txtQuantityToCarRatioTo.Text.Trim),
                                              ddlQuantityToCarRatio.SelectedValue.ConvertISeriesDefaultTypeToEnum(), compDefaults)

                talPackage.DePackages.ComponentDefaults = compDefaults
            End If
        End If

        err = talPackage.AddEditDeleteComponentGroup()
        ds1 = talPackage.ResultDataSet

        If err.HasError OrElse ds1.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
            If ds1 Is Nothing Then
                ShowMessage("AddOrUpdateComponentError")
            Else
                If ds1.Tables("StatusResults").Rows(0).Item("ReturnCode") = "F3" Then
                    ShowMessage("ComponentGroupCodeAlreadyExistError")
                Else
                    ShowMessage("AddOrUpdateComponentError")
                End If
            End If

        Else
            plhErrorList.Visible = False
            ClearCacheDependency()
            Response.Redirect(String.Format("~/Package/ComponentGroupDetails.aspx?BU={0}&Partner={1}&ComponentGroupId={2}", BusinessUnit, Partner, ds1.Tables("StatusResults").Rows(0).Item("ComponentGroupId")))
        End If

    End Sub

#End Region

#Region "Private Methods"

    Private Sub PrepareComponentDefaultObject(ByVal IsDefault As Boolean, ByVal DefaultFrom As Integer,
                                          ByVal DefaultTo As Integer, ByVal Group As DefaultType, ByRef compDefaults As List(Of ComponentDefault))

        Dim compDefault As New ComponentDefault()
        compDefault.IsDefault = IsDefault
        compDefault.DefaultFrom = Utilities.CheckForDBNull_Int(DefaultFrom)
        compDefault.DefaultTo = Utilities.CheckForDBNull_Int(DefaultTo)
        compDefault.Group = Group
        compDefaults.Add(compDefault)
    End Sub

    ''' <summary>
    ''' Sets the initial values of the CG component Group user control.
    ''' </summary>
    ''' <param name="dtComponentDetails">DataTable containing components in the component group</param>
    ''' <param name="dtComponentGroupDetails">DataTable containing components group details</param>
    ''' <remarks></remarks>
    Private Sub SetCGComponents(ByVal dtComponentDetails As DataTable, ByVal dtComponentGroupDetails As DataTable)
        If phlCG.Visible Then
            CG.RepeaterValue1 = COMPONENTDESCRIPTION
            CG.RepeaterCommandArgumentValue = COMPONENTID
            Dim dv As DataView = dtComponentDetails.DefaultView
            dtComponentDetails.DefaultView.Sort = "ComponentSequence"
            CG.ComponentsInComponentGroup = dtComponentDetails.DefaultView.ToTable

            Dim ComponentsAlreadyAddedToGroup = (From r In dtComponentDetails.AsEnumerable()
                                                    Select r.Field(Of String)("ComponentDescription")).ToList()
            Dim StadiumCode = (From r In dtComponentGroupDetails.AsEnumerable
                              Select r.Field(Of String)("GroupStadiumCode")).FirstOrDefault()
            CG.AllComponents = GetComponentList(ComponentsAlreadyAddedToGroup, StadiumCode)

            '*****Bind the per ticket/per group drop downs**************
            Dim strPerGroup As String = GetTextFromDatabase("PerGroupText")
            Dim strPerTicket As String = GetTextFromDatabase("PerTicketText")
            Dim dicPerGroupPerTicket As New Dictionary(Of String, String)
            dicPerGroupPerTicket.Add("G", strPerGroup)
            dicPerGroupPerTicket.Add("T", strPerTicket)

            ddlQuantityToComponentRatio.DataSource = dicPerGroupPerTicket
            ddlQuantityToComponentRatio.DataTextField = "Value"
            ddlQuantityToComponentRatio.DataValueField = "Key"
            ddlQuantityToComponentRatio.DataBind()
        End If
    End Sub

    ''' <summary>
    ''' Sets the initial values of the TA component Group user control.
    ''' </summary>
    ''' <param name="dtComponentDetails">DataTable containing components in the component group</param>
    ''' <remarks></remarks>
    Private Sub SetTAComponents(ByVal dtComponentDetails As DataTable, ByVal dtComponentGroupDetails As DataTable)
        If phlTA.Visible Then

            TA.CarAreaText = GetTextFromDatabase("CarAreaText")
            TA.PeopleAreaText = GetTextFromDatabase("PeopleAreaText")
            TA.TravelOutboundAreaText = GetTextFromDatabase("TravelOutboundAreaText")
            TA.TravelReturnAreaText = GetTextFromDatabase("TravelReturnAreaText")
            TA.RoomType1Text = GetTextFromDatabase("RoomType1Text")
            TA.RoomType2Text = GetTextFromDatabase("RoomType2Text")
            TA.OptionalExtras1Text = GetTextFromDatabase("OptionalExtras1Text")
            TA.OptionalExtras2Text = GetTextFromDatabase("OptionalExtras2Text")
            TA.OptionalExtras3Text = GetTextFromDatabase("OptionalExtras3Text")
            TA.OptionalExtras4Text = GetTextFromDatabase("OptionalExtras4Text")
            TA.QuantityAreaText = GetTextFromDatabase("QuantityAreaText")
            TA.FreeCarParkPassText = GetTextFromDatabase("FreeCarParkPassText")
            TA.PeopleToPassRatioText = GetTextFromDatabase("PeopleToPassRatioText")
            TA.RoomToPeopleRatioText = GetTextFromDatabase("RoomToPeopleRatioText")
            TA.CarToPeopleRatioText = GetTextFromDatabase("CarToPeopleRatioText")
            TA.Extra1ToPeopleRatioText = GetTextFromDatabase("Extra1ToPeopleRatioText")
            TA.Extra2ToPeopleRatioText = GetTextFromDatabase("Extra2ToPeopleRatioText")
            TA.Extra3ToPeopleRatioText = GetTextFromDatabase("Extra3ToPeopleRatioText")
            TA.Extra4ToPeopleRatioText = GetTextFromDatabase("Extra4ToPeopleRatioText")
            TA.DailyReturnTravelText = GetTextFromDatabase("DailyReturnTravelText")
            TA.NegativePriceForPeopleAreaText = GetTextFromDatabase("NegativePriceForPeopleArea")
            TA.AreaDefinitionText = GetTextFromDatabase("AreaDefinition")

            TA.DurationSettingsText = GetTextFromDatabase("DurationSettingsText")
            TA.AllAvailableDatesText = GetTextFromDatabase("AllAvailableDatesText")
            TA.StartDayAdjustmentText = GetTextFromDatabase("StartDayAdjustmentText")
            TA.EndDayAdjustmentText = GetTextFromDatabase("EndDayAdjustmentText")
            TA.DaysText = GetTextFromDatabase("DaysText")
            TA.ExtraDaysChargeable = GetTextFromDatabase("ExtraDaysChargeable")
            TA.DiscountOnDailyRateText = GetTextFromDatabase("DiscountOnDailyRateText")

            TA.DefaultDropdownSelectionValue = AREANOTREQUIREDVALUE
            dtComponentDetails.DefaultView.Sort = "ComponentSequence"
            TA.ComponentsInComponentGroup = dtComponentDetails.DefaultView.ToTable
            Dim ComponentsAlreadyAddedToGroup = (From r In dtComponentDetails.AsEnumerable()
                                                    Select r.Field(Of String)("ComponentDescription")).ToList()
            Dim StadiumCode = (From r In dtComponentGroupDetails.AsEnumerable
                              Select r.Field(Of String)("GroupStadiumCode")).FirstOrDefault()
            TA.AllComponents = GetComponentList(ComponentsAlreadyAddedToGroup, StadiumCode)
            TA.AllAreasInComponentGroup = GetAllAreas()

            '*****Bind the per ticket/per group drop downs**************
            Dim strPerGroup As String = GetTextFromDatabase("PerGroupText")
            Dim strPerTicket As String = GetTextFromDatabase("PerTicketText")
            Dim dicPerGroupPerTicket As New Dictionary(Of String, String)
            dicPerGroupPerTicket.Add("G", strPerGroup)
            If Not String.IsNullOrWhiteSpace(strPerTicket) Then dicPerGroupPerTicket.Add("T", strPerTicket)

            ddlQuantityToPeopleRatio.DataSource = dicPerGroupPerTicket
            ddlQuantityToPeopleRatio.DataTextField = "Value"
            ddlQuantityToPeopleRatio.DataValueField = "Key"
            ddlQuantityToPeopleRatio.DataBind()

            dicPerGroupPerTicket.Clear()
            dicPerGroupPerTicket.Add("G", GetTextFromDatabase("RoomGroupText"))
            dicPerGroupPerTicket.Add("T", GetTextFromDatabase("AllowedPerRoomText"))
            ddlQuantityToRoomRatio.DataSource = dicPerGroupPerTicket
            ddlQuantityToRoomRatio.DataTextField = "Value"
            ddlQuantityToRoomRatio.DataValueField = "Key"
            ddlQuantityToRoomRatio.DataBind()

            dicPerGroupPerTicket.Clear()
            dicPerGroupPerTicket.Add("G", GetTextFromDatabase("CarGroupText"))
            dicPerGroupPerTicket.Add("T", GetTextFromDatabase("AllowedPerCarText"))
            ddlQuantityToCarRatio.DataSource = dicPerGroupPerTicket
            ddlQuantityToCarRatio.DataTextField = "Value"
            ddlQuantityToCarRatio.DataValueField = "Key"
            ddlQuantityToCarRatio.DataBind()
        End If
    End Sub

    ''' <summary>
    ''' Retreives all the Areas
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetAllAreas() As IDictionary(Of String, String)
        Dim dicAllAreas As New Dictionary(Of String, String)
        Dim talPackage As New TalentPackage
        Dim err As ErrorObj
        Dim ds1 As DataSet

        talPackage.Settings = Utilities.GetSettingsObject
        talPackage.Settings.Cacheing = Caching
        talPackage.Settings.CacheTimeMinutes = CType(_wfrPage.Attribute("ComponentCacheTimeMinutes"), Integer)
        talPackage.DePackages.ComponentGroupID = ComponentGroupID
        talPackage.DePackages.ComponentID = If(Session("CurrentComponentId") Is Nothing, 0, Session("CurrentComponentId"))
        err = talPackage.GetComponentDetails()
        ds1 = talPackage.ResultDataSet

        dicAllAreas.Add(AREANOTREQUIREDVALUE, AREANOTREQUIREDTEXT)

        Dim lstComponents = From r In ds1.Tables("Area").AsEnumerable()
                            Order By r.Field(Of String)("AreaDescription")
                            Select r

        For Each row As DataRow In lstComponents
            If Not String.IsNullOrEmpty(row("AreaCode")) Then
                If Not dicAllAreas.ContainsKey(row("AreaCode")) Then
                    dicAllAreas.Add(row("AreaCode"), row("AreaDescription"))
                End If
            End If

        Next

        Return dicAllAreas
    End Function

    ''' <summary>
    ''' Retreives all the components.
    ''' </summary>
    ''' <returns>IDictionary object with ComponentID as key and Description as value</returns>
    ''' <remarks></remarks>
    Private Function GetComponentList(ByVal ComponentsAlreadyAddedToGroup As IList(Of String), ByVal StadiumCode As String) As IDictionary(Of Long, String)
        Dim dicComponentList As New Dictionary(Of Long, String)
        Dim talPackage As New TalentPackage
        Dim err As ErrorObj
        Dim ds1 As DataSet
        Dim lstComponents As EnumerableRowCollection(Of DataRow)

        talPackage.Settings = Utilities.GetSettingsObject
        talPackage.Settings.Cacheing = Caching
        talPackage.Settings.CacheTimeMinutes = CType(_wfrPage.Attribute("ComponentCacheTimeMinutes"), Integer)
        talPackage.DePackages.ComponentGroupID = ComponentGroupID

        err = talPackage.GetComponentList()
        ds1 = talPackage.ResultDataSet

        If String.IsNullOrEmpty(StadiumCode) Then
            lstComponents = From r In ds1.Tables("ComponentList").AsEnumerable()
                            Where Not ComponentsAlreadyAddedToGroup.Contains(r.Field(Of String)("Description"))
                            Order By r.Field(Of String)("Description")
                            Select r
        Else
            lstComponents = From r In ds1.Tables("ComponentList").AsEnumerable()
                                Where Not ComponentsAlreadyAddedToGroup.Contains(r.Field(Of String)("Description")) AndAlso
                                r.Field(Of String)("ComponentStadiumCode") = StadiumCode
                                Order By r.Field(Of String)("Description")
                                Select r
        End If

        For Each row As DataRow In lstComponents
            dicComponentList.Add(row("ComponentID"), row("Description"))
        Next

        Return dicComponentList
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

    ''' <summary>
    ''' Populates the list for number of component dropdown.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>List of 11 values.</remarks>
    Private Function GetNumberOfComponentsToSelect() As IDictionary(Of Integer, String)
        Dim numberOfComponentsList As IDictionary(Of Integer, String)
        numberOfComponentsList = New Dictionary(Of Integer, String) From {{0, "Not Applicable"}, {1, "1"}, {2, "2"}, {3, "3"}, {4, "4"},
                                                                          {5, "5"}, {6, "6"}, {7, "7"}, {8, "8"}, {9, "9"}, {10, "10"}}
        Return numberOfComponentsList
    End Function

    ''' <summary>
    ''' Gets the details of the selected component group.
    ''' </summary>
    ''' <remarks>It retreives the component group details, hides/shows the placeholders and populates the fields depending upon the component type.</remarks>
    Private Sub GetComponentGroupDetails()
        Dim result As Boolean = True
        Dim talPackage As New TalentPackage
        Dim err As ErrorObj
        Dim ds1 As DataSet

        talPackage.Settings = Utilities.GetSettingsObject
        talPackage.Settings.Cacheing = Caching
        talPackage.Settings.CacheTimeMinutes = CType(_wfrPage.Attribute("ComponentCacheTimeMinutes"), Integer)
        talPackage.DePackages.ComponentGroupID = ComponentGroupID
        err = talPackage.GetComponentGroupDetails()
        ds1 = talPackage.ResultDataSet
        If err.HasError OrElse ds1.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
            ShowMessage("FailedToRetrieveComponentGroup")
        Else
            txtComponentGroupDescription.Text = Server.HtmlDecode(ds1.Tables("ComponentGroupDetails").Rows(0)("GroupDescription").ToString())
            ddlComponentGroupType.Items.Add(ds1.Tables("ComponentGroupDetails").Rows(0)("Type").ToString())
            Select Case ddlComponentGroupType.Items(0).Value
                Case ComponentGroups.TA
                    plhValidationCG.Visible = False
                    plhValidationTA.Visible = True
                    phlCG.Visible = False
                    phlTA.Visible = True
                    SetTAComponents(ds1.Tables("ComponentDetails"), ds1.Tables("ComponentGroupDetails"))
                    txtQuantityToRoomRatioFrom.Text = ds1.Tables("ComponentGroupDetails").Rows(0)("Quantity1FromValue").ToString()
                    txtQuantityToRoomRatioTo.Text = ds1.Tables("ComponentGroupDetails").Rows(0)("Quantity1ToValue").ToString()
                    ddlQuantityToRoomRatio.SelectedValue = ds1.Tables("ComponentGroupDetails").Rows(0)("Default1Type").ToString
                    txtQuantityToPeopleRatioFrom.Text = ds1.Tables("ComponentGroupDetails").Rows(0)("Quantity2FromValue").ToString()
                    txtQuantityToPeopleRatioTo.Text = ds1.Tables("ComponentGroupDetails").Rows(0)("Quantity2ToValue").ToString()
                    ddlQuantityToPeopleRatio.SelectedValue = ds1.Tables("ComponentGroupDetails").Rows(0)("Default2Type").ToString
                    txtQuantityToCarRatioFrom.Text = ds1.Tables("ComponentGroupDetails").Rows(0)("Quantity3FromValue").ToString()
                    txtQuantityToCarRatioTo.Text = ds1.Tables("ComponentGroupDetails").Rows(0)("Quantity3ToValue").ToString()
                    ddlQuantityToCarRatio.SelectedValue = ds1.Tables("ComponentGroupDetails").Rows(0)("Default3Type").ToString
                Case ComponentGroups.CG
                    plhValidationCG.Visible = True
                    plhValidationTA.Visible = False
                    phlCG.Visible = True
                    phlTA.Visible = False
                    SetCGComponents(ds1.Tables("ComponentDetails"), ds1.Tables("ComponentGroupDetails"))
                    txtQuantityToComponentRatioFrom.Text = ds1.Tables("ComponentGroupDetails").Rows(0)("Quantity1FromValue").ToString()
                    txtQuantityToComponentRatioTo.Text = ds1.Tables("ComponentGroupDetails").Rows(0)("Quantity1ToValue").ToString()
                    ddlQuantityToComponentRatio.SelectedValue = ds1.Tables("ComponentGroupDetails").Rows(0)("Default1Type").ToString

            End Select

            ddlNoneReqOpt.SelectedValue = ds1.Tables("ComponentGroupDetails").Rows(0)("NoneReqOpt").ToString
            txtComponentGroupCode.Text = ds1.Tables("ComponentGroupDetails").Rows(0)("GroupCode").ToString()
            txtComponentGroupCode.ReadOnly = True

            'ddlNumberOfComponentsToSelect.SelectedValue = ds1.Tables("ComponentGroupDetails").Rows(0)("QuantityToSelect").ToString()


        End If
    End Sub

    ''' <summary>
    ''' Gets component group description and type for an already existing group
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetComponentDescriptionAndType()
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
            Dim ComponentGroupDescription = (From r In ds1.Tables("ComponentGroupList").AsEnumerable()
                                            Where r.Field(Of Long)("ComponentGroupID") = ComponentGroupID
                                            Select New With {
                                                                .ComponentGroupDescription = r.Field(Of String)("Description"),
                                                                .ComponentGroupType = r.Field(Of String)("ComponentGroupType")
                                                            }).FirstOrDefault

            txtComponentGroupDescription.Text = ComponentGroupDescription.ComponentGroupDescription
            ddlComponentGroupType.Items.Add(ComponentGroupDescription.ComponentGroupType)

            Select Case ComponentGroupDescription.ComponentGroupType
                Case ComponentGroups.TA
                    plhValidationCG.Visible = False
                    plhValidationTA.Visible = True
                Case ComponentGroups.CG
                    plhValidationCG.Visible = True
                    plhValidationTA.Visible = False
            End Select

            txtComponentGroupCode.ReadOnly = True
        End If
    End Sub

    ''' <summary>
    ''' Retreives the distinct Component Group Types.
    ''' </summary>
    ''' <returns>List of distinct component group types.</returns>
    ''' <remarks>It prepares the list of component group types and sorts it. </remarks>
    Private Function GetComponentGroupTypes() As IDictionary(Of String, String)

        'There will always be 2 groups: TA and CG, hence they are retreived from SQL database 
        Dim ComponentGroupList As New SortedDictionary(Of String, String)
        ComponentGroupList.Add("TA", _wfrPage.Content("TA", _languageCode, False))
        'The drop down list should only contain the travel and accommodation item.  Due to time constraints we have had to drop the component group type
        'ComponentGroupList.Add(_wfrPage.Content("CG", _languageCode, False))
        ComponentGroupList.OrderBy(Function(x) x.Value)
        Return ComponentGroupList

    End Function

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
    Private Sub SetPageText()
        Page.Title = GetTextFromDatabase("pagetitleLabel")
        pagetitleLabel.Text = GetTextFromDatabase("pagetitleLabel")
        lblSelectedComponentGroupDesc.Text = GetTextFromDatabase("lblSelectedComponentGroupDesc")
        lblComponentGroupType.Text = GetTextFromDatabase("lblComponentGroupType")
        pageinstructionsLabel.Text = GetTextFromDatabase("pageinstructionsLabel")
        If ComponentGroupID = 0 Then
            btnAddOrUpdateComponentGroup.Text = GetTextFromDatabase("btnAddOrUpdateComponentGroupADD")
        Else
            btnAddOrUpdateComponentGroup.Text = GetTextFromDatabase("btnAddOrUpdateComponentGroupUPDATE")
        End If

        btnCancelComponentGroupAddInsert.Text = GetTextFromDatabase("btnCancelComponentGroupAddInsert")
        Caching = Boolean.Parse(GetAttributeFromDatabase("GetComponentGroupDetailCaching"))
        'lblNumberOfComponentsToSelect.Text = GetTextFromDatabase("lblNumberOfComponentsToSelect")
        lblValidation.Text = GetTextFromDatabase("lblValidation")
        lblQuantityToRoomRatio.Text = GetTextFromDatabase("QuantityToRoomRatio")
        lblQuantityToPeopleRatio.Text = GetTextFromDatabase("QuantityToPeopleRatio")
        lblQuantityToCarRatio.Text = GetTextFromDatabase("QuantityToCarRatio")
        lblQuantityToComponentRatio.Text = GetTextFromDatabase("QuantityToComponentRatio")
        lblComponentGroupCode.Text = GetTextFromDatabase("lblComponentGroupCode")
        MinTextValue = GetTextFromDatabase("MinTextValue")
        MaxTextValue = GetTextFromDatabase("MaxTextValue")
        lblMin0.Text = MinTextValue
        lblMin1.Text = MinTextValue
        lblMin2.Text = MinTextValue
        lblMin3.Text = MinTextValue
        lblMax0.Text = MaxTextValue
        lblMax1.Text = MaxTextValue
        lblMax2.Text = MaxTextValue
        lblMax3.Text = MaxTextValue

        lblNoneReqOption.Text = GetTextFromDatabase("NoneReqOptLbl")

        Dim noneReqOptList As New SortedDictionary(Of String, String)

        noneReqOptList.Add("0", GetTextFromDatabase("NoneReqOpt0"))
        noneReqOptList.Add("1", GetTextFromDatabase("NoneReqOpt1"))
        noneReqOptList.Add("2", GetTextFromDatabase("NoneReqOpt2"))

        ddlNoneReqOpt.DataSource = noneReqOptList
        ddlNoneReqOpt.DataTextField = "Value"
        ddlNoneReqOpt.DataValueField = "Key"
        ddlNoneReqOpt.DataBind()
        ddlNoneReqOpt.SelectedIndex = 0
        ddlNoneReqOpt.Enabled = True

        'noneReqOptList.OrderBy(Function(x) x.Value)

    End Sub

    ''' <summary>
    ''' Handles the Add/Update/Delete component event which is raised by the component group user controls.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Any new component group (user control) should declare and raise the event for Add/Update/Delete component operation.</remarks>
    Private Sub ComponentsInComponentGroupChanged(sender As Object, e As System.EventArgs)
        Dim compOperationDetails As IComponentOperationDetails = DirectCast(sender, IComponentOperationDetails)
        If compOperationDetails.RepCommandEventArgs Is Nothing Then
            'Handles the Add/Edit/Delete operations triggerred by controls other than repeater.
            AddEditDeleteComponent(compOperationDetails.Operation, compOperationDetails.Id, compOperationDetails.TalPackage, compOperationDetails.SequenceMode)
        Else
            'Handles the Add/Edit/Delete operations triggerred by repeater.
            AddEditDeleteComponent(compOperationDetails.Operation, compOperationDetails.RepCommandEventArgs.CommandArgument, compOperationDetails.TalPackage, compOperationDetails.SequenceMode)
        End If
    End Sub

    Private Sub AddEditDeleteComponent(ByVal opMode As OperationMode, ByVal ComponentId As Long,
                                       Optional ByVal sourceTalPackage As TalentPackage = Nothing, Optional ByVal seqMode As SequenceMode = Nothing)
        If opMode = OperationMode.Cancel Then
            GetComponentGroupDetails() 'Refresh the component details for the selected component group
            Return
        End If

        Dim talPackage As New TalentPackage
        Dim err As ErrorObj
        Dim ds1 As DataSet

        talPackage.Settings = Utilities.GetSettingsObject
        talPackage.Settings.Cacheing = Caching
        talPackage.Settings.CacheTimeMinutes = CType(_wfrPage.Attribute("ComponentCacheTimeMinutes"), Integer)
        talPackage.DePackages.ComponentGroupID = ComponentGroupID
        talPackage.DePackages.ComponentID = ComponentId
        Session("CurrentComponentId") = ComponentId

        'The data is not to be edited. The usercontrol only wants the data before it updates the component.
        If (opMode = OperationMode.Amend) Then
            GetComponentGroupDetails()
            Return
        End If

        talPackage.DePackages.Mode = opMode
        If (opMode = OperationMode.Edit) Then
            PreparePackageObjectForEditComponent(talPackage, sourceTalPackage)
        ElseIf (opMode = OperationMode.Sequence) Then
            talPackage.DePackages.Sequence = seqMode
        End If

        err = talPackage.AddEditDeleteComponent()
        ds1 = talPackage.ResultDataSet

        If err.HasError OrElse ds1.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
            ClearCacheDependency()
            talPackage.RemoveItemFromCache(talPackage.Settings.ModuleName & talPackage.Settings.Company & talPackage.DePackages.ComponentGroupID)
            GetComponentGroupDetails()
            ShowMessage("AddOrUpdateComponentError")
        Else
            plhErrorList.Visible = False
            ClearCacheDependency()
            talPackage.RemoveItemFromCache(talPackage.Settings.ModuleName & talPackage.Settings.Company & talPackage.DePackages.ComponentGroupID)
            GetComponentGroupDetails() 'Refresh the component details for the selected component group
        End If


    End Sub

    Private Sub PreparePackageObjectForEditComponent(ByVal talPackage As TalentPackage, ByVal sourceTalPackage As TalentPackage)
        talPackage.DePackages.ComponentID = sourceTalPackage.DePackages.ComponentID
        talPackage.DePackages.ComponentSequence = sourceTalPackage.DePackages.ComponentSequence
        talPackage.DePackages.ComponentType = sourceTalPackage.DePackages.ComponentType
        talPackage.DePackages.AreaInComponents = sourceTalPackage.DePackages.AreaInComponents
        talPackage.DePackages.ComponentDefaults = sourceTalPackage.DePackages.ComponentDefaults
        talPackage.DePackages.AllAvailableDates = sourceTalPackage.DePackages.AllAvailableDates
        talPackage.DePackages.StartDayAdjustment.DayAdjustment = sourceTalPackage.DePackages.StartDayAdjustment.DayAdjustment
        talPackage.DePackages.StartDayAdjustment.Days = sourceTalPackage.DePackages.StartDayAdjustment.Days
        talPackage.DePackages.EndDayAdjustment.DayAdjustment = sourceTalPackage.DePackages.EndDayAdjustment.DayAdjustment
        talPackage.DePackages.EndDayAdjustment.Days = sourceTalPackage.DePackages.EndDayAdjustment.Days
        talPackage.DePackages.ExtraDaysChargeable = sourceTalPackage.DePackages.ExtraDaysChargeable
        talPackage.DePackages.DiscountOnDailyRate = sourceTalPackage.DePackages.DiscountOnDailyRate
        talPackage.DePackages.QuantityArea = sourceTalPackage.DePackages.QuantityArea
    End Sub

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

    ''' <summary>
    ''' Returns the available areas in the component.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub GetAllAreasForComponent(sender As Object, e As System.EventArgs)
        TA.AllAreasInComponentGroup = GetAllAreas()
    End Sub

#End Region

  

   
End Class
