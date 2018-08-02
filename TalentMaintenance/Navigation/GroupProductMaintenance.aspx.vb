Imports MaintenancePortalDataSetTableAdapters
Imports Talent.Common

Partial Class Navigation_GroupProductMaintenance

#Region "Class Variables"
    Inherits System.Web.UI.Page
    Dim objBUTableAdapter As New BUSINESS_UNIT_TableAdapter
    Dim objPTableAdapter As New PARTNER_TableAdapter
    Public wfrPage As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Dim objGroup1 As New NavigationDataSetTableAdapters.tbl_groupTableAdapter
    Dim _businessUnit As String = String.Empty
    Dim _partner As String = String.Empty
    Dim _noOfGroupLevels As String = String.Empty
    Private _group1 As String = String.Empty
    Private _group2 As String = String.Empty
    Private _group3 As String = String.Empty
    Private _group4 As String = String.Empty
    Private _group5 As String = String.Empty
    Private _group6 As String = String.Empty
    Private _group7 As String = String.Empty
    Private _group8 As String = String.Empty
    Private _group9 As String = String.Empty
    Private _group10 As String = String.Empty
    Private _groupPath As String = String.Empty
    Private _currentGroupIsAdhoc As Boolean = False
    Private _currentLevel As String = String.Empty
    Private _currentGroup As String = String.Empty
    Private _parmGroup1 As String = String.Empty
    Private _parmGroup2 As String = String.Empty
    Private _parmGroup3 As String = String.Empty
    Private _parmGroup4 As String = String.Empty
    Private _parmGroup5 As String = String.Empty
    Private _parmGroup6 As String = String.Empty
    Private _parmGroup7 As String = String.Empty
    Private _parmGroup8 As String = String.Empty
    Private _parmGroup9 As String = String.Empty
    Private _parmGroup10 As String = String.Empty
#End Region

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With wfrPage
            .BusinessUnit = "MAINTENANCE"
            .PageCode = String.Empty
            .PartnerCode = "*ALL"
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "GroupProductMaintenance.aspx"
            .PageCode = "GroupProductMaintenance.aspx"
        End With
        setLabel()
    End Sub

    Public Sub setLabel()
        Dim liGroupHomeLink As New ListItem
        liGroupHomeLink.Value = "NavigationMaintenance.aspx?BU="
        liGroupHomeLink.Value += Request.QueryString("BU")
        liGroupHomeLink.Value += "&partner="
        liGroupHomeLink.Value += Request.QueryString("partner")
        liGroupHomeLink.Text = "Navigation Overview"
        navigationOptions.Items.Add(liGroupHomeLink)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim master As MasterPage1 = CType(Me.Master, MasterPage1)
        master.HeaderText = "Group Navigation Maintenance"
        If String.IsNullOrEmpty(Request.QueryString("BU")) Then
            Response.Redirect("../MaintenancePortal.aspx")
        Else
            _businessUnit = Request("BU")
        End If

        If String.IsNullOrEmpty(Request.QueryString("partner")) Then
            Response.Redirect("../MaintenancePortal.aspx")
        Else
            _partner = Request("partner")
        End If

        If IsPostBack = False Then

        End If

        '-----------------------
        ' Get no of group Levels
        '-----------------------
        Dim objDefaults As New TalentApplicationVariablesTableAdapters.tbl_ecommerce_module_defaults_buTableAdapter
        Dim dt As New Data.DataTable
        dt = objDefaults.GetDataByDefaultName(_businessUnit, _partner, "NUMBER_OF_GROUP_LEVELS")
        If dt.Rows.Count > 0 Then
            _noOfGroupLevels = dt.Rows(0)("VALUE")
        End If

        SetGroupDetails()
        path.Text = _groupPath

        If Not _currentGroup.Trim = String.Empty Then
            Dim objGroupData As New NavigationDataSetTableAdapters.tbl_groupTableAdapter
            titleLabel.Text = "Maintain Products - " & Utilities.CheckForDBNull_String( _
                    objGroupData.GetDataByGroupName(_currentGroup).Rows(0)("GROUP_DESCRIPTION_1"))
        End If
        If Not Page.IsPostBack Then
            LoadProducts()
        End If

    End Sub

    Protected Sub SetGroupDetails()
        If Not Request("group1") Is Nothing Then
            _group1 = Request("group1")
            _groupPath = _group1
            _currentLevel = "01"
            _currentGroup = _group1
        End If
        If Not Request("group2") Is Nothing Then
            _group2 = Request("group2")
            _groupPath &= "-" & _group2
            _currentLevel = "02"
            _currentGroup = _group2
        End If
        If Not Request("group3") Is Nothing Then
            _group3 = Request("group3")
            _groupPath &= "-" & _group3
            _currentLevel = "03"
            _currentGroup = _group3
        End If
        If Not Request("group4") Is Nothing Then
            _group4 = Request("group4")
            _groupPath &= "-" & _group4
            _currentLevel = "04"
            _currentGroup = _group4
        End If
        If Not Request("group5") Is Nothing Then
            _group5 = Request("group5")
            _groupPath &= "-" & _group5
            _currentLevel = "05"
            _currentGroup = _group5
        End If
        If Not Request("group6") Is Nothing Then
            _group6 = Request("group6")
            _groupPath &= "-" & _group6
            _currentLevel = "06"
            _currentGroup = _group6
        End If
        If Not Request("group7") Is Nothing Then
            _group7 = Request("group7")
            _groupPath &= "-" & _group7
            _currentLevel = "07"
            _currentGroup = _group7
        End If
        If Not Request("group8") Is Nothing Then
            _group8 = Request("group8")
            _groupPath &= "-" & _group8
            _currentLevel = "08"
            _currentGroup = _group8
        End If
        If Not Request("group9") Is Nothing Then
            _group9 = Request("group9")
            _groupPath &= "-" & _group9
            _currentLevel = "09"
            _currentGroup = _group9
        End If
        If Not Request("group10") Is Nothing Then
            _group10 = Request("group10")
            _groupPath &= "-" & _group10
            _currentLevel = "10"
            _currentGroup = _group10
        End If
        If Not Request("BU") Is Nothing Then
            _businessUnit = Request("BU")
        End If
        If Not Request("partner") Is Nothing Then
            _partner = Request("partner")
        End If
        '-----------------------
        ' get current group info
        '-----------------------
        Dim dt As New Data.DataTable
        Select Case _currentLevel
            Case Is = "01"
                Dim objGroup As New NavigationDataSetTableAdapters.tbl_group_level_01TableAdapter
                dt = objGroup.GetDataByGroup(_businessUnit, _partner, _group1)
            Case Is = "02"
                Dim objGroup As New NavigationDataSetTableAdapters.tbl_group_level_02TableAdapter
                dt = objGroup.GetDataByGroup(_businessUnit, _partner, _group1, _group2)
            Case Is = "03"
                Dim objGroup As New NavigationDataSetTableAdapters.tbl_group_level_03TableAdapter
                dt = objGroup.GetDataByGroup(_businessUnit, _partner, _group1, _group2, _group3)
            Case Is = "04"
                Dim objGroup As New NavigationDataSetTableAdapters.tbl_group_level_04TableAdapter
                dt = objGroup.GetDataByGroup(_businessUnit, _partner, _group1, _group2, _group3, _group4)
            Case Is = "05"
                Dim objGroup As New NavigationDataSetTableAdapters.tbl_group_level_05TableAdapter
                dt = objGroup.GetDataByGroup(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5)
            Case Is = "06"
                Dim objGroup As New NavigationDataSetTableAdapters.tbl_group_level_06TableAdapter
                dt = objGroup.GetDataByGroup(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5, _group6)
            Case Is = "07"
                Dim objGroup As New NavigationDataSetTableAdapters.tbl_group_level_07TableAdapter
                dt = objGroup.GetDataByGroup(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5, _group6, _group7)
            Case Is = "08"
                Dim objGroup As New NavigationDataSetTableAdapters.tbl_group_level_08TableAdapter
                dt = objGroup.GetDataByGroup(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5, _group6, _group7, _group8)
            Case Is = "09"
                Dim objGroup As New NavigationDataSetTableAdapters.tbl_group_level_09TableAdapter
                dt = objGroup.GetDataByGroup(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5, _group6, _group7, _group8, _group9)
            Case Is = "10"
                Dim objGroup As New NavigationDataSetTableAdapters.tbl_group_level_10TableAdapter
                dt = objGroup.GetDataByGroup(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5, _group6, _group7, _group8, _group9, _group10)
        End Select
        If dt.Rows.Count > 0 Then
            _currentGroupIsAdhoc = Utilities.CheckForDBNull_Boolean_DefaultFalse(dt.Rows(0)("GROUP_L" & _currentLevel & "_ADHOC_GROUP"))
        End If
    End Sub

    Protected Sub LoadProducts()
        Dim objGP As New NavigationDataSetTableAdapters.tbl_group_productTableAdapter
        Dim objProd As New NavigationDataSetTableAdapters.tbl_productTableAdapter
        '---------------------------------------------
        ' Set listbox datasources against correct file
        '---------------------------------------------
        Select Case _currentLevel
            Case Is = "01"
                lbxProductsIn.DataSource = objGP.GetDataByLevel1(_businessUnit, _partner, _group1)
                lbxAdhocProductsIn.DataSource = objGP.GetDataByAdhocGroup1(_businessUnit, _partner, _group1)
                lbxProductsOut.DataSource = objProd.GetData_notInGroupProductLevel1(_group1, _businessUnit, _partner)

            Case Is = "02"
                lbxProductsIn.DataSource = objGP.GetDataByLevel2(_businessUnit, _partner, _group1, _group2)
                lbxAdhocProductsIn.DataSource = objGP.GetDataByAdhocGroup2(_businessUnit, _partner, _group1, _group2)
                lbxProductsOut.DataSource = objProd.GetData_notInGroupProductLevel2(_group3, _businessUnit, _partner, _group1)

            Case Is = "03"
                lbxProductsIn.DataSource = objGP.GetDataByLevel3(_businessUnit, _partner, _group1, _group2, _group3)
                lbxAdhocProductsIn.DataSource = objGP.GetDataByAdhocGroup3(_businessUnit, _partner, _group1, _group2, _group3)
                lbxProductsOut.DataSource = objProd.GetData_notInGroupProductLevel3(_group3, _businessUnit, _partner, _group1, _group2)

            Case Is = "04"
                lbxProductsIn.DataSource = objGP.GetDataByLevel4(_businessUnit, _partner, _group1, _group2, _group3, _group4)
                lbxAdhocProductsIn.DataSource = objGP.GetDataByAdhocGroup4(_businessUnit, _partner, _group1, _group2, _group3, _group4)
                lbxProductsOut.DataSource = objProd.GetData_notInGroupProductLevel4(_group3, _businessUnit, _partner, _group1, _group2, _group4)

            Case Is = "05"
                lbxProductsIn.DataSource = objGP.GetDataByLevel5(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5)
                lbxAdhocProductsIn.DataSource = objGP.GetDataByAdhocGroup5(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5)
                lbxProductsOut.DataSource = objProd.GetData_notInGroupProductLevel5(_group3, _businessUnit, _partner, _group1, _group2, _group4, _group5)

            Case Is = "06"
                lbxProductsIn.DataSource = objGP.GetDataByLevel6(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5, _group6)
                lbxAdhocProductsIn.DataSource = objGP.GetDataByAdhocGroup6(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5, _group6)
                lbxProductsOut.DataSource = objProd.GetData_notInGroupProductLevel6(_group3, _businessUnit, _partner, _group1, _group2, _group4, _group5, _group6)

            Case Is = "07"
                lbxProductsIn.DataSource = objGP.GetDataByLevel7(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5, _group6, _group7)
                lbxAdhocProductsIn.DataSource = objGP.GetDataByAdhocGroup7(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5, _group6, _group7)
                lbxProductsOut.DataSource = objProd.GetData_notInGroupProductLevel7(_group3, _businessUnit, _partner, _group1, _group2, _group4, _group5, _group6, _group7)

            Case Is = "08"
                lbxProductsIn.DataSource = objGP.GetDataByLevel8(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5, _group6, _group7, _group8)
                lbxAdhocProductsIn.DataSource = objGP.GetDataByAdhocGroup8(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5, _group6, _group7, _group8)
                lbxProductsOut.DataSource = objProd.GetData_notInGroupProductLevel8(_group3, _businessUnit, _partner, _group1, _group2, _group4, _group5, _group6, _group7, _group8)

            Case Is = "09"
                lbxProductsIn.DataSource = objGP.GetDataByLevel9(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5, _group6, _group7, _group8, _group9)
                lbxAdhocProductsIn.DataSource = objGP.GetDataByAdhocGroup9(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5, _group6, _group7, _group8, _group9)
                lbxProductsOut.DataSource = objProd.GetData_notInGroupProductLevel9(_group3, _businessUnit, _partner, _group1, _group2, _group4, _group5, _group6, _group7, _group8, _group9)

            Case Is = "10"
                lbxProductsIn.DataSource = objGP.GetDataByLevel10(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5, _group6, _group7, _group8, _group9, _group10)
                lbxAdhocProductsIn.DataSource = objGP.GetDataByAdhocGroup10(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5, _group6, _group7, _group8, _group9, _group10)
                lbxProductsOut.DataSource = objProd.GetData_notInGroupProductLevel10(_group3, _businessUnit, _partner, _group1, _group2, _group4, _group5, _group6, _group7, _group8, _group9, _group10)

        End Select

        lbxProductsIn.DataTextField = "PRODUCT"
        lbxProductsIn.DataValueField = "GROUP_PRODUCT_ID"
        lbxProductsIn.DataBind()

        lbxAdhocProductsIn.DataTextField = "PRODUCT"
        lbxAdhocProductsIn.DataValueField = "GROUP_PRODUCT_ID"
        lbxAdhocProductsIn.DataBind()

        lbxProductsOut.DataValueField = "PRODUCT_CODE"
        lbxProductsOut.DataBind()

        lbxProductsIn.Enabled = False
        lbxAdhocProductsIn.Enabled = True
        lbxProductsOut.Enabled = True
    End Sub

    Protected Sub btnReturnToNavigation_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReturnToNavigation.Click
        Response.Redirect(Replace(Request.Url.AbsoluteUri, "GroupProductMaintenance.aspx", "NavigationMaintenance.aspx"))
    End Sub

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        FillEmptyGroups()
        Dim objGroupProd As New NavigationDataSetTableAdapters.tbl_group_productTableAdapter
        For Each item As ListItem In lbxProductsOut.Items
            If item.Selected Then
                objGroupProd.Insert(_businessUnit, _partner, _parmGroup1, _parmGroup2, _parmGroup3, _parmGroup4, _parmGroup5, _parmGroup6, _
                            _parmGroup7, _parmGroup8, _parmGroup9, _parmGroup10, item.Value, "ZZ" & item.Value, True)
            End If
        Next
        '-------------------------------------------------------------------------------
        ' Check if it's an Adhoc group and need's to have *empty group levels created...
        '-------------------------------------------------------------------------------
        Dim groupHasChildren As Boolean = HasChildren()
        Dim intCurrentLevel As Integer = CInt(_currentLevel)
        Dim intNoOfGroupLevels As Integer = CInt(_noOfGroupLevels)

        If _currentGroupIsAdhoc AndAlso Not groupHasChildren AndAlso intCurrentLevel < intNoOfGroupLevels Then
            FillEmptyGroups()
            Dim dt As New Data.DataTable
            If intCurrentLevel < 2 AndAlso intNoOfGroupLevels >= 2 Then
                Dim objCreate As New NavigationDataSetTableAdapters.tbl_group_level_02TableAdapter
                objCreate.InsertQuery(_businessUnit, _partner, _parmGroup1, _parmGroup2, String.Empty, String.Empty, String.Empty, _
                                       String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, _
                                       String.Empty, String.Empty, String.Empty, False, False, False, False, False, _
                                       String.Empty, False, True)
            End If
            If intCurrentLevel < 3 AndAlso intNoOfGroupLevels >= 3 Then
                Dim objCreate As New NavigationDataSetTableAdapters.tbl_group_level_03TableAdapter
                objCreate.InsertQuery(_businessUnit, _partner, _parmGroup1, _parmGroup2, _parmGroup3, String.Empty, String.Empty, String.Empty, _
                                       String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, _
                                       String.Empty, String.Empty, String.Empty, False, False, False, False, False, _
                                       String.Empty, False, True)
            End If
            If intCurrentLevel < 4 AndAlso intNoOfGroupLevels >= 4 Then
                Dim objCreate As New NavigationDataSetTableAdapters.tbl_group_level_04TableAdapter
                objCreate.InsertQuery(_businessUnit, _partner, _parmGroup1, _parmGroup2, _parmGroup3, _parmGroup4, String.Empty, String.Empty, String.Empty, _
                                       String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, _
                                       String.Empty, String.Empty, String.Empty, False, False, False, False, False, _
                                       String.Empty, False, True)
            End If
            If intCurrentLevel < 5 AndAlso intNoOfGroupLevels >= 5 Then
                Dim objCreate As New NavigationDataSetTableAdapters.tbl_group_level_05TableAdapter
                objCreate.InsertQuery(_businessUnit, _partner, _parmGroup1, _parmGroup2, _parmGroup3, _parmGroup4, _parmGroup5, String.Empty, String.Empty, String.Empty, _
                                       String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, _
                                       String.Empty, String.Empty, String.Empty, False, False, False, False, False, _
                                       String.Empty, False, True)
            End If
            If intCurrentLevel < 6 AndAlso intNoOfGroupLevels >= 6 Then
                Dim objCreate As New NavigationDataSetTableAdapters.tbl_group_level_06TableAdapter
                objCreate.InsertQuery(_businessUnit, _partner, _parmGroup1, _parmGroup2, _parmGroup3, _parmGroup4, _parmGroup5, _parmGroup6, String.Empty, String.Empty, String.Empty, _
                                       String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, _
                                       String.Empty, String.Empty, String.Empty, False, False, False, False, False, _
                                       String.Empty, False, True)
            End If
            If intCurrentLevel < 7 AndAlso intNoOfGroupLevels >= 7 Then
                Dim objCreate As New NavigationDataSetTableAdapters.tbl_group_level_07TableAdapter
                objCreate.InsertQuery(_businessUnit, _partner, _parmGroup1, _parmGroup2, _parmGroup3, _parmGroup4, _parmGroup5, _parmGroup6, _parmGroup7, String.Empty, String.Empty, String.Empty, _
                                       String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, _
                                       String.Empty, String.Empty, String.Empty, False, False, False, False, False, _
                                       String.Empty, False, True)
            End If
            If intCurrentLevel < 8 AndAlso intNoOfGroupLevels >= 8 Then
                Dim objCreate As New NavigationDataSetTableAdapters.tbl_group_level_08TableAdapter
                objCreate.InsertQuery(_businessUnit, _partner, _parmGroup1, _parmGroup2, _parmGroup3, _parmGroup4, _parmGroup5, _parmGroup6, _parmGroup7, _parmGroup8, String.Empty, String.Empty, String.Empty, _
                                       String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, _
                                       String.Empty, String.Empty, String.Empty, False, False, False, False, False, _
                                       String.Empty, False, True)
            End If
            If intCurrentLevel < 9 AndAlso intNoOfGroupLevels >= 9 Then
                Dim objCreate As New NavigationDataSetTableAdapters.tbl_group_level_09TableAdapter
                objCreate.InsertQuery(_businessUnit, _partner, _parmGroup1, _parmGroup2, _parmGroup3, _parmGroup4, _parmGroup5, _parmGroup6, _parmGroup7, _parmGroup8, _parmGroup9, String.Empty, String.Empty, String.Empty, _
                                       String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, _
                                       String.Empty, String.Empty, String.Empty, False, False, False, False, False, _
                                       String.Empty, False, True)
            End If
            If intCurrentLevel < 10 AndAlso intNoOfGroupLevels >= 10 Then
                Dim objCreate As New NavigationDataSetTableAdapters.tbl_group_level_10TableAdapter
                objCreate.InsertQuery(_businessUnit, _partner, _parmGroup1, _parmGroup2, _parmGroup3, _parmGroup4, _parmGroup5, _parmGroup6, _parmGroup7, _parmGroup8, _parmGroup9, _parmGroup10, String.Empty, String.Empty, String.Empty, _
                                       String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, _
                                       String.Empty, String.Empty, String.Empty, False, False, False, False, False, _
                                       String.Empty, False, True)
            End If

        End If

        Response.Redirect(Request.Url.AbsoluteUri)
    End Sub

    Protected Function HasChildren() As Boolean
        Dim rtnHasChildren As Boolean = False
        Dim intChildren As Integer = 0
        Select Case _currentLevel
            Case Is = "01"
                Dim objGroup As New NavigationDataSetTableAdapters.tbl_group_level_02TableAdapter
                intChildren = objGroup.CheckExistsAsChildGroup(_businessUnit, _partner, _group1)
            Case Is = "02"
                Dim objGroup As New NavigationDataSetTableAdapters.tbl_group_level_03TableAdapter
                intChildren = objGroup.CheckExistsAsChildGroup(_businessUnit, _partner, _group1, _group2)
            Case Is = "03"
                Dim objGroup As New NavigationDataSetTableAdapters.tbl_group_level_04TableAdapter
                intChildren = objGroup.CheckExistsAsChildGroup(_businessUnit, _partner, _group1, _group2, _group3)
            Case Is = "04"
                Dim objGroup As New NavigationDataSetTableAdapters.tbl_group_level_05TableAdapter
                intChildren = objGroup.CheckExistsAsChildGroup(_businessUnit, _partner, _group1, _group2, _group3, _group4)
            Case Is = "05"
                Dim objGroup As New NavigationDataSetTableAdapters.tbl_group_level_06TableAdapter
                intChildren = objGroup.CheckExistsAsChildGroup(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5)
            Case Is = "06"
                Dim objGroup As New NavigationDataSetTableAdapters.tbl_group_level_07TableAdapter
                intChildren = objGroup.CheckExistsAsChildGroup(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5, _group6)
            Case Is = "07"
                Dim objGroup As New NavigationDataSetTableAdapters.tbl_group_level_08TableAdapter
                intChildren = objGroup.CheckExistsAsChildGroup(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5, _group6, _group7)
            Case Is = "08"
                Dim objGroup As New NavigationDataSetTableAdapters.tbl_group_level_09TableAdapter
                intChildren = objGroup.CheckExistsAsChildGroup(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5, _group6, _group7, _group8)
            Case Is = "09"
                Dim objGroup As New NavigationDataSetTableAdapters.tbl_group_level_10TableAdapter
                intChildren = objGroup.CheckExistsAsChildGroup(_businessUnit, _partner, _group1, _group2, _group3, _group4, _group5, _group6, _group7, _group8, _group9)
            Case Is = "10"
                intChildren = 0
        End Select
        If intChildren > 0 Then
            rtnHasChildren = True
        End If

        Return rtnHasChildren

    End Function

    Protected Sub btnRemove_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRemove.Click
        FillEmptyGroups()
        Dim objGroupProd As New NavigationDataSetTableAdapters.tbl_group_productTableAdapter
        For Each item As ListItem In lbxAdhocProductsIn.Items
            If item.Selected Then
                objGroupProd.Delete(item.Value)
            End If
        Next
        Response.Redirect(Request.Url.AbsoluteUri)
    End Sub

    Protected Sub FillEmptyGroups()
        _parmGroup1 = String.Empty
        _parmGroup2 = String.Empty
        _parmGroup3 = String.Empty
        _parmGroup4 = String.Empty
        _parmGroup5 = String.Empty
        _parmGroup6 = String.Empty
        _parmGroup7 = String.Empty
        _parmGroup8 = String.Empty
        _parmGroup9 = String.Empty
        _parmGroup10 = String.Empty

        _parmGroup1 = _group1
        If CInt(_noOfGroupLevels) >= 2 Then
            If _group2 = String.Empty Then
                _parmGroup2 = "*EMPTY"
            Else
                _parmGroup2 = _group2
            End If
        End If
        If CInt(_noOfGroupLevels) >= 3 Then
            If _group3 = String.Empty Then
                _parmGroup3 = "*EMPTY"
            Else
                _parmGroup3 = _group3
            End If
        End If
        If CInt(_noOfGroupLevels) >= 4 Then
            If _group4 = String.Empty Then
                _parmGroup4 = "*EMPTY"
            Else
                _parmGroup4 = _group4
            End If
        End If
        If CInt(_noOfGroupLevels) >= 5 Then
            If _group5 = String.Empty Then
                _parmGroup5 = "*EMPTY"
            Else
                _parmGroup5 = _group5
            End If
        End If
        If CInt(_noOfGroupLevels) >= 6 Then
            If _group6 = String.Empty Then
                _parmGroup6 = "*EMPTY"
            Else
                _parmGroup6 = _group6
            End If
        End If
        If CInt(_noOfGroupLevels) >= 7 Then
            If _group7 = String.Empty Then
                _parmGroup7 = "*EMPTY"
            Else
                _parmGroup7 = _group7
            End If
        End If
        If CInt(_noOfGroupLevels) >= 8 Then
            If _group8 = String.Empty Then
                _parmGroup8 = "*EMPTY"
            Else
                _parmGroup8 = _group8
            End If
        End If
        If CInt(_noOfGroupLevels) >= 9 Then
            If _group9 = String.Empty Then
                _parmGroup9 = "*EMPTY"
            Else
                _parmGroup9 = _group9
            End If
        End If
        If CInt(_noOfGroupLevels) >= 10 Then
            If _group10 = String.Empty Then
                _parmGroup10 = "*EMPTY"
            Else
                _parmGroup10 = _group10
            End If
        End If
    End Sub

End Class
