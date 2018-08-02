Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.eCommerce
Imports Talent.Common
Imports System.Xml

Partial Class UserControls_PageLeftProductNav
    Inherits ControlBase

#Region "Class Level Fields"

    Private _treeBuilder As New StringBuilder()
    Private _businessUnit As String
    Private _partner As String
    Private _currentPage As String

#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _businessUnit = TalentCache.GetBusinessUnit()
        _partner = TalentCache.GetPartner(HttpContext.Current.Profile)
        _currentPage = Talent.eCommerce.Utilities.GetCurrentPageName()
        '-------------------------------
        ' Check for selected 
        '-------------------------------
        BuildTopLevelTreeNodes()
        Session("OrderDDLChanged") = False
    End Sub

    Protected Sub trvProductNav_SelectedNodeChanged(ByVal sender As Object, ByVal e As System.EventArgs) 'Handles trvProductNav.SelectedNodeChanged
        '-------------------------------------------------------------------------------
        ' When a navigate node is clicked, obtain the navigate URL from XML Document via
        ' the Node Path and save the Tree in session for loading on the next page
        '-------------------------------------------------------------------------------
        Dim strNavUrl As String = String.Empty
        Dim trvSender As TreeView = CType(sender, TreeView)
        strNavUrl = SelectedNodeChanged(sender, e, "LEFTPRODUCTNAVXML")
        Response.Redirect(strNavUrl)
    End Sub

    Protected Sub trvProductNav_TreeNodePopulate(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.TreeNodeEventArgs) Handles trvProductNav.TreeNodePopulate
        '------------------------------------------------------------------------------------
        ' trvTopUserNav_TreeNodePopulate - Handle a click on an expansion node for TopUserNav
        '------------------------------------------------------------------------------------
        Dim xmlDoc As New XmlDocument
        Dim xmllist As XmlNodeList
        Dim Node1 As XmlNode
        Dim strText As String = String.Empty
        Dim strValue As String = String.Empty
        Dim nodeCount As Integer = 0
        '------------------------------------------------------------------------
        ' Split out Value Path of clicked on Node. This will contain the index of
        ' each item in the path, which will then be used to navigate back through
        ' the XML Doc
        '------------------------------------------------------------------------
        Dim valArr() As String = e.Node.ValuePath.Split("/")
        Try
            If e.Node.ChildNodes.Count = 0 Then
                '----------------
                ' Load in XML Doc
                '----------------
                xmlDoc = LoadXmlDocFromCache("LEFTPRODUCTNAVXML")
                xmllist = xmlDoc.SelectNodes("Navigation/L1Node")
                Dim nextLevelNodeList As XmlNodeList = Nothing
                Dim strLevel As String = String.Empty

                Select Case e.Node.Depth
                    '------------------------------------------------------
                    ' Build next set of nodes for clicked item from XML Doc
                    '------------------------------------------------------
                    Case Is = 0
                        nextLevelNodeList = xmllist(valArr(0)).SelectNodes("L2Node")
                        strLevel = "L2"
                    Case Is = 1
                        nextLevelNodeList = xmllist(valArr(0)).SelectNodes("L2Node"). _
                                            Item(valArr(1)).SelectNodes("L3Node")
                        strLevel = "L3"
                    Case Is = 2
                        nextLevelNodeList = xmllist(valArr(0)).SelectNodes("L2Node"). _
                                            Item(valArr(1)).SelectNodes("L3Node"). _
                                            Item(valArr(2)).SelectNodes("L4Node")
                        strLevel = "L4"
                    Case Is = 3
                        nextLevelNodeList = xmllist(valArr(0)).SelectNodes("L2Node"). _
                                            Item(valArr(1)).SelectNodes("L3Node"). _
                                            Item(valArr(2)).SelectNodes("L4Node"). _
                                            Item(valArr(3)).SelectNodes("L5Node")

                        strLevel = "L5"
                    Case Is = 4
                        nextLevelNodeList = xmllist(valArr(0)).SelectNodes("L2Node"). _
                                            Item(valArr(1)).SelectNodes("L3Node"). _
                                            Item(valArr(2)).SelectNodes("L4Node"). _
                                            Item(valArr(3)).SelectNodes("L5Node"). _
                                            Item(valArr(4)).SelectNodes("L6Node")
                        strLevel = "L6"
                    Case Is = 5
                        nextLevelNodeList = xmllist(valArr(0)).SelectNodes("L2Node"). _
                                            Item(valArr(1)).SelectNodes("L3Node"). _
                                            Item(valArr(2)).SelectNodes("L4Node"). _
                                            Item(valArr(3)).SelectNodes("L5Node"). _
                                            Item(valArr(4)).SelectNodes("L6Node"). _
                                            Item(valArr(5)).SelectNodes("L7Node")
                        strLevel = "L7"
                    Case Is = 6
                        nextLevelNodeList = xmllist(valArr(0)).SelectNodes("L2Node"). _
                                            Item(valArr(1)).SelectNodes("L3Node"). _
                                            Item(valArr(2)).SelectNodes("L4Node"). _
                                            Item(valArr(3)).SelectNodes("L5Node"). _
                                            Item(valArr(4)).SelectNodes("L6Node"). _
                                            Item(valArr(5)).SelectNodes("L7Node"). _
                                            Item(valArr(6)).SelectNodes("L8Node")
                        strLevel = "L8"
                    Case Is = 7
                        nextLevelNodeList = xmllist(valArr(0)).SelectNodes("L2Node"). _
                                            Item(valArr(1)).SelectNodes("L3Node"). _
                                            Item(valArr(2)).SelectNodes("L4Node"). _
                                            Item(valArr(3)).SelectNodes("L5Node"). _
                                            Item(valArr(4)).SelectNodes("L6Node"). _
                                            Item(valArr(5)).SelectNodes("L7Node"). _
                                            Item(valArr(6)).SelectNodes("L8Node"). _
                                            Item(valArr(7)).SelectNodes("L9Node")
                        strLevel = "L9"
                    Case Is = 8
                        nextLevelNodeList = xmllist(valArr(0)).SelectNodes("L2Node"). _
                                            Item(valArr(1)).SelectNodes("L3Node"). _
                                            Item(valArr(2)).SelectNodes("L4Node"). _
                                            Item(valArr(3)).SelectNodes("L5Node"). _
                                            Item(valArr(4)).SelectNodes("L6Node"). _
                                            Item(valArr(5)).SelectNodes("L7Node"). _
                                            Item(valArr(6)).SelectNodes("L8Node"). _
                                            Item(valArr(7)).SelectNodes("L9Node"). _
                                            Item(valArr(8)).SelectNodes("L10Node")
                        strLevel = "L10"
                    Case Else

                End Select
                '----------------------------------------------------------------------------------
                ' If still under maximum depth then build list of tree nodes from list of XML nodes
                '----------------------------------------------------------------------------------
                If e.Node.Depth <= 10 Then
                    For Each Node1 In nextLevelNodeList
                        Dim NewNode As New TreeNode(Node1.SelectSingleNode(strLevel & "Text").InnerText, nodeCount)
                        Dim strGroup As String = Node1.SelectSingleNode(strLevel & "Group").InnerText
                        '------------------------------------------
                        ' Don't add the node if it's an EMPTY group
                        '------------------------------------------
                        If strGroup <> "*EMPTY" Then
                            NewNode.PopulateOnDemand = True
                            NewNode.SelectAction = TreeNodeSelectAction.SelectExpand
                            e.Node.ChildNodes.Add(NewNode)
                            nodeCount += 1
                        End If
                    Next
                End If
            End If
        Catch xmlEx As XmlException
            'MsgBox(xmlEx.Message)
        Catch ex As Exception
            'MsgBox(ex.Message)
        End Try
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Not Page.IsPostBack Then
            Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
            trvProductNav.ExpandDepth = def.Menu_LeftProductNav_TreeExpandDepth
        Else
            If Session("OrderDDLChanged") = False Then
                Dim strNavUrl As String = String.Empty
                strNavUrl = SelectedNodeChanged(trvProductNav, e, "LEFTPRODUCTNAVXML")
                Response.Redirect(strNavUrl)
            End If
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub BuildTopLevelTreeNodes()
        Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
        Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
        trvProductNav.Nodes.Clear()
        Dim xmlDoc As New XmlDocument
        '--------------------------------------------------------------
        ' Is it in session? If so copy out nodes, and clear the session
        '--------------------------------------------------------------
        If Not Session("trvLeftProductNav") Is Nothing Then
            Dim trn As New TreeNodeCollection
            trn = CType(Session("trvLeftProductNav"), TreeNodeCollection)
            Dim intCount As Int32 = trn.Count
            Dim newNodeArray() As TreeNode = New TreeNode(intCount - 1) {}
            trn.CopyTo(newNodeArray, 0)

            Dim i As Int32 = 0
            While (i < intCount)
                If newNodeArray(i).Selected Then

                End If
                trvProductNav.Nodes.Add(newNodeArray(i))
                i += 1
            End While
            Session("trvLeftProductNav") = Nothing
        Else
            '----------------
            ' Load in XML Doc
            '----------------
            xmlDoc = LoadXmlDocFromCache("LEFTPRODUCTNAVXML")
            '-----------------------------------------
            ' Build top level set up nodes, unexpanded
            '-----------------------------------------
            Dim level1nodes As XmlNodeList = xmlDoc.SelectNodes("Navigation/L1Node")
            Dim xmlNode As XmlNode
            Dim strText As String = String.Empty
            Dim strL1Group As String = String.Empty
            Dim strValue As String = String.Empty
            Dim nodeCount As Integer = 0
            Dim currentGroup01 As String = String.Empty

            If Not Request("group1") Is Nothing Then
                currentGroup01 = Request("group1").ToString.Trim
            End If
            For Each xmlNode In level1nodes
                strText = xmlNode.SelectSingleNode("L1Text").InnerText
                strL1Group = xmlNode.SelectSingleNode("L1Group").InnerText
                Dim NewNode As New TreeNode(strText, nodeCount)
                '------------------------------------
                ' Set up link if it's a navigate node
                ' and whether can expand or not
                '------------------------------------
                NewNode.PopulateOnDemand = True
                NewNode.SelectAction = TreeNodeSelectAction.SelectExpand
                '-------------------------------------------------------
                ' If configured to hide unselected top level groups then
                ' then non-display all other top level nodes
                '-------------------------------------------------------
                If def.HideAllTopLevelGroups Then
                    NewNode.SelectAction = TreeNodeSelectAction.None
                    NewNode.Text = String.Empty
                    If currentGroup01.ToUpper <> strL1Group.ToUpper Then
                        NewNode.PopulateOnDemand = False
                    End If
                Else
                    If def.HideUnselectedTopLevelGroups AndAlso currentGroup01.ToUpper <> strL1Group.ToUpper Then
                        NewNode.SelectAction = TreeNodeSelectAction.None
                        NewNode.Text = String.Empty
                        NewNode.PopulateOnDemand = False
                    End If
                End If

                trvProductNav.Nodes.Add(NewNode)
                nodeCount += 1
            Next

            '-----------------------------------------------------
            ' Check if need to expand nodes (e.g. if from bookmark
            '-----------------------------------------------------
            If Not Request("group1") Is Nothing Then
                '-----------------------------------------------------------
                ' Recursively expand the nodes according to the query string
                '-----------------------------------------------------------
                ExpandGroupNodes(xmlDoc.SelectNodes("Navigation/L1Node"), "L1Group", Request("group1"), Nothing)
            End If
        End If
    End Sub

    Private Sub ExpandGroupNodes(ByRef ndlXmlNodeList As XmlNodeList, ByVal groupName As String, ByVal groupRequest As String, ByRef currentTreeNode As TreeNode)
        '------------------------------------------------------------------
        ' ExpandGroupNodes - Recursively expand nodes to match query string
        ' 
        Dim count As Integer = 0
        Dim newGroupName As String = String.Empty
        Dim newGroupRequest As String = String.Empty
        Dim newNodesSelect As String = String.Empty
        Dim nextNode As XmlNode = Nothing
        '-------------------------------------------------------------
        ' Check for already expanded to correct level for current page
        '-------------------------------------------------------------
        Dim blExitSub As Boolean = False
        _currentPage = Talent.eCommerce.Utilities.GetCurrentPageName()
        Select Case _currentPage
            Case Is = "browse01.aspx"
                blExitSub = True
            Case Is = "browse02.aspx"
                If groupName = "L2Group" Then
                    blExitSub = True
                End If
            Case Is = "browse03.aspx"
                If groupName = "L3Group" Then
                    blExitSub = True
                End If
            Case Is = "browse04.aspx"
                If groupName = "L4Group" Then
                    blExitSub = True
                End If
            Case Is = "browse05.aspx"
                If groupName = "L5Group" Then
                    blExitSub = True
                End If
            Case Is = "browse06.aspx"
                If groupName = "L6Group" Then
                    blExitSub = True
                End If
            Case Is = "browse07.aspx"
                If groupName = "L7Group" Then
                    blExitSub = True
                End If
            Case Is = "browse08.aspx"
                If groupName = "L8Group" Then
                    blExitSub = True
                End If
            Case Is = "browse09.aspx"
                If groupName = "L9Group" Then
                    blExitSub = True
                End If
            Case Is = "browse10.aspx"
                If groupName = "L10Group" Then
                    blExitSub = True
                End If
        End Select

        If Not blExitSub Then
            For Each ndNode As XmlNode In ndlXmlNodeList
                If ndNode.SelectSingleNode(groupName).InnerText.ToUpper = groupRequest.ToUpper Then
                    '--------------------
                    ' Expand current node 
                    '--------------------
                    If currentTreeNode Is Nothing Then
                        trvProductNav.Nodes(count).ToggleExpandState()
                    Else
                        currentTreeNode.ChildNodes(count).ToggleExpandState()
                        currentTreeNode.ChildNodes(count).Selected = True
                    End If
                    nextNode = ndNode
                    '-------------------------
                    ' Set parms for next level
                    '-------------------------
                    Select Case groupName
                        Case Is = "L1Group"
                            newGroupName = "L2Group"
                            newGroupRequest = Request("group2")
                            newNodesSelect = "L2Node"
                        Case Is = "L2Group"
                            newGroupName = "L3Group"
                            newGroupRequest = Request("group3")
                            newNodesSelect = "L3Node"
                        Case Is = "L3Group"
                            newGroupName = "L4Group"
                            newGroupRequest = Request("group4")
                            newNodesSelect = "L4Node"
                        Case Is = "L4Group"
                            newGroupName = "L5Group"
                            newGroupRequest = Request("group5")
                            newNodesSelect = "L5Node"
                        Case Is = "L5Group"
                            newGroupName = "L6Group"
                            newGroupRequest = Request("group6")
                            newNodesSelect = "L6Node"
                        Case Is = "L6Group"
                            newGroupName = "L7Group"
                            newGroupRequest = Request("group7")
                            newNodesSelect = "L7Node"
                        Case Is = "L7Group"
                            newGroupName = "L8Group"
                            newGroupRequest = Request("group8")
                            newNodesSelect = "L8Node"
                        Case Is = "L8Group"
                            newGroupName = "L9Group"
                            newGroupRequest = Request("group9")
                            newNodesSelect = "L9Node"
                        Case Is = "L9Group"
                            newGroupName = "L10Group"
                            newGroupRequest = Request("group10")
                            newNodesSelect = "L10Node"
                        Case Is = "L10Group"
                            'newGroupName = "L3Group"
                            'newGroupRequest = Request("group3")
                    End Select

                    Exit For
                End If
                count += 1
            Next
            '--------------------------------------------------
            ' Base case - next level request is empty. Exit sub
            '--------------------------------------------------
            If newGroupRequest = String.Empty OrElse newGroupRequest = "*EMPTY" OrElse (Not currentTreeNode Is Nothing AndAlso currentTreeNode.ChildNodes(count).ChildNodes.Count = 0) Then
            Else
                '-----------------------------------------
                ' Otherwise recursive expand rest of nodes
                '-----------------------------------------
                If Not currentTreeNode Is Nothing Then
                    ExpandGroupNodes(nextNode.SelectNodes(newNodesSelect), newGroupName, newGroupRequest, currentTreeNode.ChildNodes(count))
                Else
                    ExpandGroupNodes(nextNode.SelectNodes(newNodesSelect), newGroupName, newGroupRequest, trvProductNav.Nodes(count))
                End If
            End If
        End If
    End Sub

#End Region

#Region "Private Function"

    Private Function SelectedNodeChanged(ByRef trvSender As TreeView, ByVal e As System.EventArgs, ByVal strCacheKey As String) As String
        '---------------------------------------------------------------------
        ' SelectedNodeChanged - Return the URL when a navigate node is clicked 
        ' from XML Document via the Node Path 
        '---------------------------------------------------------------------
        Dim xmlDoc As New XmlDocument
        '----------------
        ' Load in XML Doc
        '----------------
        xmlDoc = LoadXmlDocFromCache(strCacheKey)
        Dim xmllist As XmlNodeList = xmlDoc.SelectNodes("Navigation/L1Node")
        Dim nodeSelectedNode As TreeNode = trvSender.SelectedNode
        Dim strNavUrl As String = String.Empty
        If nodeSelectedNode IsNot Nothing Then
            Dim valArr() As String = nodeSelectedNode.ValuePath.Split("/")
            Select Case nodeSelectedNode.Depth
                Case Is = 0
                    strNavUrl = xmllist(valArr(0)).SelectSingleNode("L1NavigateURL").InnerText
                Case Is = 1
                    strNavUrl = xmllist(valArr(0)).SelectNodes("L2Node"). _
                               Item(valArr(1)).SelectSingleNode("L2NavigateURL").InnerText
                Case Is = 2
                    strNavUrl = xmllist(valArr(0)).SelectNodes("L2Node"). _
                                Item(valArr(1)).SelectNodes("L3Node"). _
                                Item(valArr(2)).SelectSingleNode("L3NavigateURL").InnerText
                Case Is = 3
                    strNavUrl = xmllist(valArr(0)).SelectNodes("L2Node"). _
                             Item(valArr(1)).SelectNodes("L3Node"). _
                             Item(valArr(2)).SelectNodes("L4Node"). _
                             Item(valArr(3)).SelectSingleNode("L4NavigateURL").InnerText
                Case Is = 4
                    strNavUrl = xmllist(valArr(0)).SelectNodes("L2Node"). _
                             Item(valArr(1)).SelectNodes("L3Node"). _
                             Item(valArr(2)).SelectNodes("L4Node"). _
                             Item(valArr(3)).SelectNodes("L5Node"). _
                             Item(valArr(4)).SelectSingleNode("L5NavigateURL").InnerText
                Case Is = 5
                    strNavUrl = xmllist(valArr(0)).SelectNodes("L2Node"). _
                             Item(valArr(1)).SelectNodes("L3Node"). _
                             Item(valArr(2)).SelectNodes("L4Node"). _
                             Item(valArr(3)).SelectNodes("L5Node"). _
                             Item(valArr(4)).SelectNodes("L6Node"). _
                             Item(valArr(5)).SelectSingleNode("L6NavigateURL").InnerText
                Case Is = 6
                    strNavUrl = xmllist(valArr(0)).SelectNodes("L2Node"). _
                             Item(valArr(1)).SelectNodes("L3Node"). _
                             Item(valArr(2)).SelectNodes("L4Node"). _
                             Item(valArr(3)).SelectNodes("L5Node"). _
                             Item(valArr(4)).SelectNodes("L6Node"). _
                             Item(valArr(5)).SelectNodes("L7Node"). _
                             Item(valArr(6)).SelectSingleNode("L7NavigateURL").InnerText
                Case Is = 7
                    strNavUrl = xmllist(valArr(0)).SelectNodes("L2Node"). _
                             Item(valArr(1)).SelectNodes("L3Node"). _
                             Item(valArr(2)).SelectNodes("L4Node"). _
                             Item(valArr(3)).SelectNodes("L5Node"). _
                             Item(valArr(4)).SelectNodes("L6Node"). _
                             Item(valArr(5)).SelectNodes("L7Node"). _
                             Item(valArr(6)).SelectNodes("L8Node"). _
                             Item(valArr(7)).SelectSingleNode("L8NavigateURL").InnerText
                Case Is = 8
                    strNavUrl = xmllist(valArr(0)).SelectNodes("L2Node"). _
                             Item(valArr(1)).SelectNodes("L3Node"). _
                             Item(valArr(2)).SelectNodes("L4Node"). _
                             Item(valArr(3)).SelectNodes("L5Node"). _
                             Item(valArr(4)).SelectNodes("L6Node"). _
                             Item(valArr(5)).SelectNodes("L7Node"). _
                             Item(valArr(6)).SelectNodes("L8Node"). _
                             Item(valArr(7)).SelectNodes("L9Node"). _
                             Item(valArr(8)).SelectSingleNode("L9NavigateURL").InnerText
                Case Is = 9
                    strNavUrl = xmllist(valArr(0)).SelectNodes("L2Node"). _
                             Item(valArr(1)).SelectNodes("L3Node"). _
                             Item(valArr(2)).SelectNodes("L4Node"). _
                             Item(valArr(3)).SelectNodes("L5Node"). _
                             Item(valArr(4)).SelectNodes("L6Node"). _
                             Item(valArr(5)).SelectNodes("L7Node"). _
                             Item(valArr(6)).SelectNodes("L8Node"). _
                             Item(valArr(7)).SelectNodes("L9Node"). _
                             Item(valArr(8)).SelectNodes("L10Node"). _
                             Item(valArr(9)).SelectSingleNode("L10NavigateURL").InnerText
            End Select
            trvSender.SelectedNode.Selected = True
        Else
            strNavUrl = Request.Url.AbsoluteUri
        End If
        Return strNavUrl
    End Function

    Private Function LoadXmlDocFromCache(ByVal cacheKey As String) As XmlDocument
        '----------------------------------------------------------------
        ' Load XML doc from Cache if it's there, otherwise load from File
        ' then add to cache
        Dim xmlDoc As New XmlDocument
        Dim err As New ErrorObj
        cacheKey &= "PageLeftProductNav_PageLayout" & _businessUnit & _partner & _currentPage

        If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            xmlDoc.LoadXml(CType(HttpContext.Current.Cache.Item(cacheKey), String))
        Else
            '--------------------------------------------------------------------
            Dim conTalent As SqlConnection = Nothing
            Dim partnerParm As String = Nothing
            '------------------------------------------------------------------------
            ' Determine whether navigation through product groups for this particular
            ' partner should be done using '*ALL' as the partner
            '------------------------------------------------------------------------
            If Talent.eCommerce.Utilities.GroupNavigationUsingAll() Then
                partnerParm = Talent.Common.Utilities.GetAllString
            Else
                partnerParm = _partner
            End If
            Dim FrontEndConnectionString As String = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            Try

                'Open the connection to the front end database
                conTalent = New SqlConnection(FrontEndConnectionString)
                conTalent.Open()

                '------------
                ' Get details
                '------------
                Const strSelect1 As String = "SELECT * FROM TBL_GROUP_LEVEL_01 AS LV WITH (NOLOCK)  INNER JOIN TBL_GROUP AS GR WITH (NOLOCK)  " & _
                                             " ON LV.GROUP_L01_L01_GROUP = GR.GROUP_NAME" & _
                                             " WHERE GROUP_L01_BUSINESS_UNIT = @BUSINESS_UNIT AND GROUP_L01_PARTNER = @PARTNER AND GROUP_L01_SHOW_IN_NAVIGATION = 1" & _
                                             " ORDER BY LV.GROUP_L01_SEQUENCE"
                Dim cmdSelect1 As SqlCommand = New SqlCommand(strSelect1, conTalent)
                Dim ndNewNode, ndNavigation, ndText, ndNavigateURL, ndGroup As XmlNode

                ndNavigation = xmlDoc.CreateElement("Navigation")
                xmlDoc.AppendChild(ndNavigation)

                '----------
                ' 1st Level
                '----------
                cmdSelect1.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = _businessUnit
                cmdSelect1.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partnerParm
                Dim dtrL1Nav As SqlDataReader
                dtrL1Nav = cmdSelect1.ExecuteReader()

                If dtrL1Nav.HasRows = False Then
                    ' Try all partners
                    cmdSelect1.Parameters.RemoveAt("@BUSINESS_UNIT")
                    cmdSelect1.Parameters.RemoveAt("@PARTNER")
                    cmdSelect1.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = _businessUnit
                    cmdSelect1.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                    dtrL1Nav.Close()
                    dtrL1Nav = cmdSelect1.ExecuteReader()
                    If dtrL1Nav.HasRows = False Then
                        dtrL1Nav.Close()
                        'Try all business units
                        cmdSelect1.Parameters.RemoveAt("@BUSINESS_UNIT")
                        cmdSelect1.Parameters.RemoveAt("@PARTNER")
                        cmdSelect1.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                        cmdSelect1.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partnerParm
                        dtrL1Nav = cmdSelect1.ExecuteReader()
                        If dtrL1Nav.HasRows = False Then
                            'Try all business units and partners
                            cmdSelect1.Parameters.RemoveAt("@BUSINESS_UNIT")
                            cmdSelect1.Parameters.RemoveAt("@PARTNER")
                            cmdSelect1.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                            cmdSelect1.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                            dtrL1Nav = cmdSelect1.ExecuteReader()
                        End If
                    End If
                End If

                If dtrL1Nav.HasRows Then
                    While dtrL1Nav.Read()
                        ndNewNode = xmlDoc.CreateElement("L1Node")
                        ndText = xmlDoc.CreateElement("L1Text")
                        ndText.InnerText = dtrL1Nav("GROUP_DESCRIPTION_1")
                        ndGroup = xmlDoc.CreateElement("L1Group")
                        ndGroup.InnerText = dtrL1Nav("GROUP_L01_L01_GROUP")
                        ndNavigateURL = xmlDoc.CreateElement("L1NavigateURL")
                        ndNavigateURL.InnerText = "~/PagesPublic/ProductBrowse/browse02.aspx?group1=" & dtrL1Nav("GROUP_L01_L01_GROUP")
                        ndNewNode.AppendChild(ndText)
                        ndNewNode.AppendChild(ndGroup)
                        ndNewNode.AppendChild(ndNavigateURL)
                        ndNavigation.AppendChild(ndNewNode)
                    End While
                End If
                dtrL1Nav.Close()
                '----------
                ' 2nd Level
                '----------
                Dim strSelect2 As String = "SELECT * FROM TBL_GROUP_LEVEL_02 AS LV WITH (NOLOCK)  INNER JOIN TBL_GROUP AS GR WITH (NOLOCK)  " & _
                                             " ON LV.GROUP_L02_L02_GROUP = GR.GROUP_NAME" & _
                                             " WHERE GROUP_L02_BUSINESS_UNIT = @BUSINESS_UNIT AND (GROUP_L02_PARTNER = @PARTNER OR GROUP_L02_PARTNER = @param6) AND GROUP_L02_L01_GROUP = @L1GROUP AND GROUP_L02_SHOW_IN_NAVIGATION = 1" & _
                                             " ORDER BY LV.GROUP_L02_SEQUENCE"

                Dim ndlL1nodes As XmlNodeList = xmlDoc.SelectNodes("Navigation/L1Node")
                For Each ndL1node As XmlNode In ndlL1nodes
                    Dim cmdSelect2 As SqlCommand = New SqlCommand(strSelect2, conTalent)
                    cmdSelect2.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = _businessUnit
                    cmdSelect2.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partnerParm
                    cmdSelect2.Parameters.Add(New SqlParameter("@L1GROUP", SqlDbType.Char, 20)).Value = ndL1node.SelectSingleNode("L1Group").InnerText
                    cmdSelect2.Parameters.Add(New SqlParameter("@param6", SqlDbType.Char, 20)).Value = Talent.Common.Utilities.GetAllString
                    Dim dtrL2Nav As SqlDataReader = cmdSelect2.ExecuteReader
                    If dtrL2Nav.HasRows = False Then
                        ' Try all partners
                        cmdSelect2.Parameters.RemoveAt("@BUSINESS_UNIT")
                        cmdSelect2.Parameters.RemoveAt("@PARTNER")
                        cmdSelect2.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = _businessUnit
                        cmdSelect2.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                        dtrL2Nav.Close()
                        dtrL2Nav = cmdSelect2.ExecuteReader()
                        If dtrL2Nav.HasRows = False Then
                            'Try all business units
                            cmdSelect2.Parameters.RemoveAt("@BUSINESS_UNIT")
                            cmdSelect2.Parameters.RemoveAt("@PARTNER")
                            cmdSelect2.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                            cmdSelect2.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partnerParm
                            dtrL2Nav.Close()
                            dtrL2Nav = cmdSelect2.ExecuteReader()
                            If dtrL2Nav.HasRows = False Then
                                'Try all business units and partners
                                cmdSelect2.Parameters.RemoveAt("@BUSINESS_UNIT")
                                cmdSelect2.Parameters.RemoveAt("@PARTNER")
                                cmdSelect2.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                                cmdSelect2.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                                dtrL2Nav.Close()
                                dtrL2Nav = cmdSelect2.ExecuteReader()
                            End If
                        End If
                    End If

                    If dtrL2Nav.HasRows Then
                        While dtrL2Nav.Read()
                            ndNewNode = xmlDoc.CreateElement("L2Node")
                            ndText = xmlDoc.CreateElement("L2Text")
                            ndText.InnerText = dtrL2Nav("GROUP_DESCRIPTION_1")
                            ndGroup = xmlDoc.CreateElement("L2Group")
                            ndGroup.InnerText = dtrL2Nav("GROUP_L02_L02_GROUP")
                            ndNavigateURL = xmlDoc.CreateElement("L2NavigateURL")
                            ndNavigateURL.InnerText = "~/PagesPublic/ProductBrowse/browse03.aspx?group1=" & dtrL2Nav("GROUP_L02_L01_GROUP") & _
                                                        "&group2=" & dtrL2Nav("GROUP_L02_L02_GROUP")

                            ndNewNode.AppendChild(ndText)
                            ndNewNode.AppendChild(ndGroup)
                            ndNewNode.AppendChild(ndNavigateURL)
                            ndL1node.AppendChild(ndNewNode)
                        End While
                    End If
                    dtrL2Nav.Close()
                Next
                '----------
                ' 3rd Level
                '----------
                Const strSelect3 As String = "SELECT * FROM TBL_GROUP_LEVEL_03 AS LV WITH (NOLOCK)  INNER JOIN TBL_GROUP AS GR WITH (NOLOCK)  " & _
                                             " ON LV.GROUP_L03_L03_GROUP = GR.GROUP_NAME" & _
                                             " WHERE GROUP_L03_BUSINESS_UNIT = @BUSINESS_UNIT AND (GROUP_L03_PARTNER = @PARTNER OR GROUP_L03_PARTNER = @param6) AND " & _
                                             " GROUP_L03_L01_GROUP = @L1GROUP AND GROUP_L03_L02_GROUP = @L2GROUP AND GROUP_L03_SHOW_IN_NAVIGATION = 1" & _
                                             " ORDER BY LV.GROUP_L03_SEQUENCE"
                Dim cmdSelect3 As SqlCommand
                Dim ndlL2nodes As XmlNodeList = xmlDoc.SelectNodes("Navigation/L1Node/L2Node")
                For Each ndL2node As XmlNode In ndlL2nodes
                    cmdSelect3 = New SqlCommand(strSelect3, conTalent)

                    cmdSelect3.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = _businessUnit
                    cmdSelect3.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char)).Value = partnerParm
                    cmdSelect3.Parameters.Add(New SqlParameter("@L1GROUP", SqlDbType.Char)).Value = ndL2node.ParentNode.SelectSingleNode("L1Group").InnerText
                    cmdSelect3.Parameters.Add(New SqlParameter("@L2GROUP", SqlDbType.Char)).Value = ndL2node.SelectSingleNode("L2Group").InnerText
                    cmdSelect3.Parameters.Add(New SqlParameter("@param6", SqlDbType.Char)).Value = Talent.Common.Utilities.GetAllString
                    Dim dtrL3Nav As SqlDataReader = cmdSelect3.ExecuteReader

                    If dtrL3Nav.HasRows = False Then
                        ' Try all partners
                        cmdSelect3.Parameters.RemoveAt("@BUSINESS_UNIT")
                        cmdSelect3.Parameters.RemoveAt("@PARTNER")
                        cmdSelect3.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = _businessUnit
                        cmdSelect3.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                        dtrL3Nav.Close()
                        dtrL3Nav = cmdSelect3.ExecuteReader()
                        If dtrL3Nav.HasRows = False Then
                            'Try all business units
                            cmdSelect3.Parameters.RemoveAt("@BUSINESS_UNIT")
                            cmdSelect3.Parameters.RemoveAt("@PARTNER")
                            cmdSelect3.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                            cmdSelect3.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partnerParm
                            dtrL3Nav.Close()
                            dtrL3Nav = cmdSelect3.ExecuteReader()
                            If dtrL3Nav.HasRows = False Then
                                'Try all business units and partners
                                cmdSelect3.Parameters.RemoveAt("@BUSINESS_UNIT")
                                cmdSelect3.Parameters.RemoveAt("@PARTNER")
                                cmdSelect3.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                                cmdSelect3.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                                dtrL3Nav.Close()
                                dtrL3Nav = cmdSelect3.ExecuteReader()
                            End If
                        End If
                    End If
                    If dtrL3Nav.HasRows Then
                        While dtrL3Nav.Read()
                            ndNewNode = xmlDoc.CreateElement("L3Node")
                            ndText = xmlDoc.CreateElement("L3Text")
                            ndText.InnerText = dtrL3Nav("GROUP_DESCRIPTION_1")
                            ndGroup = xmlDoc.CreateElement("L3Group")
                            ndGroup.InnerText = dtrL3Nav("GROUP_L03_L03_GROUP")
                            ndNavigateURL = xmlDoc.CreateElement("L3NavigateURL")
                            ndNavigateURL.InnerText = "~/PagesPublic/ProductBrowse/browse04.aspx?group1=" & dtrL3Nav("GROUP_L03_L01_GROUP") & _
                                                        "&group2=" & dtrL3Nav("GROUP_L03_L02_GROUP") & _
                                                        "&group3=" & dtrL3Nav("GROUP_L03_L03_GROUP")
                            ndNewNode.AppendChild(ndText)
                            ndNewNode.AppendChild(ndGroup)
                            ndNewNode.AppendChild(ndNavigateURL)
                            ndL2node.AppendChild(ndNewNode)
                        End While
                    End If
                    dtrL3Nav.Close()
                Next
                '----------
                ' 4th Level
                '----------
                Const strSelect4 As String = "SELECT * FROM TBL_GROUP_LEVEL_04 AS LV WITH (NOLOCK)  INNER JOIN TBL_GROUP AS GR WITH (NOLOCK)  " & _
                                             " ON LV.GROUP_L04_L04_GROUP = GR.GROUP_NAME" & _
                                             " WHERE GROUP_L04_BUSINESS_UNIT = @BUSINESS_UNIT AND (GROUP_L04_PARTNER = @PARTNER OR GROUP_L04_PARTNER = @param6 ) AND " & _
                                             " GROUP_L04_L01_GROUP = @L1GROUP AND GROUP_L04_L02_GROUP = @L2GROUP AND " & _
                                             " GROUP_L04_L03_GROUP = @L3GROUP AND GROUP_L04_SHOW_IN_NAVIGATION = 1" & _
                                             " ORDER BY LV.GROUP_L04_SEQUENCE"
                Dim cmdSelect4 As SqlCommand
                Dim ndlL3nodes As XmlNodeList = xmlDoc.SelectNodes("Navigation/L1Node/L2Node/L3Node")
                For Each ndL3node As XmlNode In ndlL3nodes
                    cmdSelect4 = New SqlCommand(strSelect4, conTalent)
                    cmdSelect4.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = _businessUnit
                    cmdSelect4.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partnerParm
                    cmdSelect4.Parameters.Add(New SqlParameter("@L1GROUP", SqlDbType.Char, 20)).Value = ndL3node.ParentNode.ParentNode.SelectSingleNode("L1Group").InnerText
                    cmdSelect4.Parameters.Add(New SqlParameter("@L2GROUP", SqlDbType.Char, 20)).Value = ndL3node.ParentNode.SelectSingleNode("L2Group").InnerText
                    cmdSelect4.Parameters.Add(New SqlParameter("@L3GROUP", SqlDbType.Char, 20)).Value = ndL3node.SelectSingleNode("L3Group").InnerText
                    cmdSelect4.Parameters.Add(New SqlParameter("@param6", SqlDbType.Char)).Value = Talent.Common.Utilities.GetAllString

                    Dim dtrL4Nav As SqlDataReader = cmdSelect4.ExecuteReader
                    If dtrL1Nav.HasRows = False Then
                        ' dtrL4Nav all partners
                        cmdSelect4.Parameters.RemoveAt("@BUSINESS_UNIT")
                        cmdSelect4.Parameters.RemoveAt("@PARTNER")
                        cmdSelect4.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = _businessUnit
                        cmdSelect4.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                        dtrL4Nav.Close()
                        dtrL4Nav = cmdSelect4.ExecuteReader()
                        If dtrL4Nav.HasRows = False Then
                            'Try all business units
                            cmdSelect4.Parameters.RemoveAt("@BUSINESS_UNIT")
                            cmdSelect4.Parameters.RemoveAt("@PARTNER")
                            cmdSelect4.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                            cmdSelect4.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partnerParm
                            dtrL4Nav.Close()
                            dtrL4Nav = cmdSelect4.ExecuteReader()
                            If dtrL4Nav.HasRows = False Then
                                'Try all business units and partners
                                cmdSelect4.Parameters.RemoveAt("@BUSINESS_UNIT")
                                cmdSelect4.Parameters.RemoveAt("@PARTNER")
                                cmdSelect4.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                                cmdSelect4.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                                dtrL4Nav.Close()
                                dtrL4Nav = cmdSelect4.ExecuteReader()
                            End If
                        End If
                    End If

                    If dtrL4Nav.HasRows Then
                        While dtrL4Nav.Read()
                            ndNewNode = xmlDoc.CreateElement("L4Node")
                            ndText = xmlDoc.CreateElement("L4Text")
                            ndText.InnerText = dtrL4Nav("GROUP_DESCRIPTION_1")
                            ndGroup = xmlDoc.CreateElement("L4Group")
                            ndGroup.InnerText = dtrL4Nav("GROUP_L04_L04_GROUP")
                            ndNavigateURL = xmlDoc.CreateElement("L4NavigateURL")
                            ndNavigateURL.InnerText = "~/PagesPublic/ProductBrowse/browse05.aspx?group1=" & dtrL4Nav("GROUP_L04_L01_GROUP") & _
                                                        "&group2=" & dtrL4Nav("GROUP_L04_L02_GROUP") & _
                                                        "&group3=" & dtrL4Nav("GROUP_L04_L03_GROUP") & _
                                                        "&group4=" & dtrL4Nav("GROUP_L04_L04_GROUP")
                            ndNewNode.AppendChild(ndText)
                            ndNewNode.AppendChild(ndGroup)
                            ndNewNode.AppendChild(ndNavigateURL)
                            ndL3node.AppendChild(ndNewNode)
                        End While
                    End If
                    dtrL4Nav.Close()
                Next
                '----------
                ' 5th Level
                '----------
                Const strSelect5 As String = "SELECT * FROM TBL_GROUP_LEVEL_05 AS LV WITH (NOLOCK)  INNER JOIN TBL_GROUP AS GR WITH (NOLOCK)  " & _
                                             " ON LV.GROUP_L05_L05_GROUP = GR.GROUP_NAME" & _
                                             " WHERE GROUP_L05_BUSINESS_UNIT = @BUSINESS_UNIT AND (GROUP_L05_PARTNER = @PARTNER OR GROUP_L05_PARTNER = @param6 ) AND " & _
                                             " GROUP_L05_L01_GROUP = @L1GROUP AND GROUP_L05_L02_GROUP = @L2GROUP AND " & _
                                             " GROUP_L05_L03_GROUP = @L3GROUP  AND" & _
                                              " GROUP_L05_L04_GROUP = @L4GROUP AND GROUP_L05_SHOW_IN_NAVIGATION = 1" & _
                                             " ORDER BY LV.GROUP_L05_SEQUENCE"
                Dim cmdSelect5 As SqlCommand
                Dim ndlL4nodes As XmlNodeList = xmlDoc.SelectNodes("Navigation/L1Node/L2Node/L3Node/L4Node")
                For Each ndL4node As XmlNode In ndlL4nodes
                    cmdSelect5 = New SqlCommand(strSelect5, conTalent)
                    cmdSelect5.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = _businessUnit
                    cmdSelect5.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partnerParm
                    cmdSelect5.Parameters.Add(New SqlParameter("@L1GROUP", SqlDbType.Char, 20)).Value = ndL4node.ParentNode.ParentNode.ParentNode.SelectSingleNode("L1Group").InnerText
                    cmdSelect5.Parameters.Add(New SqlParameter("@L2GROUP", SqlDbType.Char, 20)).Value = ndL4node.ParentNode.ParentNode.SelectSingleNode("L2Group").InnerText
                    cmdSelect5.Parameters.Add(New SqlParameter("@L3GROUP", SqlDbType.Char, 20)).Value = ndL4node.ParentNode.SelectSingleNode("L3Group").InnerText
                    cmdSelect5.Parameters.Add(New SqlParameter("@L4GROUP", SqlDbType.Char, 20)).Value = ndL4node.SelectSingleNode("L4Group").InnerText
                    cmdSelect5.Parameters.Add(New SqlParameter("@param6", SqlDbType.Char)).Value = Talent.Common.Utilities.GetAllString

                    Dim dtrL5Nav As SqlDataReader = cmdSelect5.ExecuteReader
                    If dtrL5Nav.HasRows = False Then
                        ' Try all partners
                        cmdSelect5.Parameters.RemoveAt("@BUSINESS_UNIT")
                        cmdSelect5.Parameters.RemoveAt("@PARTNER")
                        cmdSelect5.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = _businessUnit
                        cmdSelect5.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                        dtrL5Nav.Close()
                        dtrL5Nav = cmdSelect5.ExecuteReader()
                        If dtrL5Nav.HasRows = False Then
                            'Try all business units
                            cmdSelect5.Parameters.RemoveAt("@BUSINESS_UNIT")
                            cmdSelect5.Parameters.RemoveAt("@PARTNER")
                            cmdSelect5.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                            cmdSelect5.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partnerParm
                            dtrL5Nav.Close()
                            dtrL5Nav = cmdSelect5.ExecuteReader()
                            If dtrL5Nav.HasRows = False Then
                                'Try all business units and partners
                                cmdSelect5.Parameters.RemoveAt("@BUSINESS_UNIT")
                                cmdSelect5.Parameters.RemoveAt("@PARTNER")
                                cmdSelect5.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                                cmdSelect5.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                                dtrL5Nav.Close()
                                dtrL5Nav = cmdSelect5.ExecuteReader()
                            End If
                        End If
                    End If

                    If dtrL5Nav.HasRows Then
                        While dtrL5Nav.Read()
                            ndNewNode = xmlDoc.CreateElement("L5Node")
                            ndText = xmlDoc.CreateElement("L5Text")
                            ndText.InnerText = dtrL5Nav("GROUP_DESCRIPTION_1")
                            ndGroup = xmlDoc.CreateElement("L5Group")
                            ndGroup.InnerText = dtrL5Nav("GROUP_L05_L05_GROUP")
                            ndNavigateURL = xmlDoc.CreateElement("L5NavigateURL")
                            ndNavigateURL.InnerText = "~/PagesPublic/ProductBrowse/browse06.aspx?group1=" & dtrL5Nav("GROUP_L05_L01_GROUP") & _
                                                        "&group2=" & dtrL5Nav("GROUP_L05_L02_GROUP") & _
                                                        "&group3=" & dtrL5Nav("GROUP_L05_L03_GROUP") & _
                                                        "&group4=" & dtrL5Nav("GROUP_L05_L04_GROUP") & _
                                                        "&group5=" & dtrL5Nav("GROUP_L05_L05_GROUP")
                            ndNewNode.AppendChild(ndText)
                            ndNewNode.AppendChild(ndGroup)
                            ndNewNode.AppendChild(ndNavigateURL)
                            ndL4node.AppendChild(ndNewNode)
                        End While
                    End If
                    dtrL5Nav.Close()
                Next
                '----------
                ' 6th Level
                '----------
                Const strSelect6 As String = "SELECT * FROM TBL_GROUP_LEVEL_06 AS LV WITH (NOLOCK)  INNER JOIN TBL_GROUP AS GR WITH (NOLOCK)  " & _
                                             " ON LV.GROUP_L06_L06_GROUP = GR.GROUP_NAME" & _
                                             " WHERE GROUP_L06_BUSINESS_UNIT = @BUSINESS_UNIT AND (GROUP_L06_PARTNER = @PARTNER OR GROUP_L06_PARTNER = @param6 ) AND " & _
                                             " GROUP_L06_L01_GROUP = @L1GROUP AND GROUP_L06_L02_GROUP = @L2GROUP AND " & _
                                             " GROUP_L06_L03_GROUP = @L3GROUP AND" & _
                                             " GROUP_L06_L04_GROUP = @L4GROUP AND " & _
                                             " GROUP_L06_L05_GROUP = @L5GROUP AND GROUP_L06_SHOW_IN_NAVIGATION = 1" & _
                                             " ORDER BY LV.GROUP_L06_SEQUENCE"
                Dim cmdSelect6 As SqlCommand
                Dim ndlL5nodes As XmlNodeList = xmlDoc.SelectNodes("Navigation/L1Node/L2Node/L3Node/L4Node/L5Node")
                For Each ndL5node As XmlNode In ndlL5nodes
                    cmdSelect6 = New SqlCommand(strSelect6, conTalent)
                    cmdSelect6.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = _businessUnit
                    cmdSelect6.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partnerParm
                    cmdSelect6.Parameters.Add(New SqlParameter("@L1GROUP", SqlDbType.Char, 20)).Value = ndL5node.ParentNode.ParentNode.ParentNode.ParentNode.SelectSingleNode("L1Group").InnerText
                    cmdSelect6.Parameters.Add(New SqlParameter("@L2GROUP", SqlDbType.Char, 20)).Value = ndL5node.ParentNode.ParentNode.ParentNode.SelectSingleNode("L2Group").InnerText
                    cmdSelect6.Parameters.Add(New SqlParameter("@L3GROUP", SqlDbType.Char, 20)).Value = ndL5node.ParentNode.ParentNode.SelectSingleNode("L3Group").InnerText
                    cmdSelect6.Parameters.Add(New SqlParameter("@L4GROUP", SqlDbType.Char, 20)).Value = ndL5node.ParentNode.SelectSingleNode("L4Group").InnerText
                    cmdSelect6.Parameters.Add(New SqlParameter("@L5GROUP", SqlDbType.Char, 20)).Value = ndL5node.SelectSingleNode("L5Group").InnerText
                    cmdSelect6.Parameters.Add(New SqlParameter("@param6", SqlDbType.Char)).Value = Talent.Common.Utilities.GetAllString

                    Dim dtrL6Nav As SqlDataReader = cmdSelect6.ExecuteReader
                    If dtrL6Nav.HasRows = False Then
                        ' Try all partners
                        cmdSelect6.Parameters.RemoveAt("@BUSINESS_UNIT")
                        cmdSelect6.Parameters.RemoveAt("@PARTNER")
                        cmdSelect6.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = _businessUnit
                        cmdSelect6.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                        dtrL6Nav.Close()
                        dtrL6Nav = cmdSelect6.ExecuteReader()
                        If dtrL6Nav.HasRows = False Then
                            'Try all business units
                            cmdSelect6.Parameters.RemoveAt("@BUSINESS_UNIT")
                            cmdSelect6.Parameters.RemoveAt("@PARTNER")
                            cmdSelect6.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                            cmdSelect6.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partnerParm
                            dtrL6Nav.Close()
                            dtrL6Nav = cmdSelect6.ExecuteReader()
                            If dtrL6Nav.HasRows = False Then
                                'Try all business units and partners
                                cmdSelect6.Parameters.RemoveAt("@BUSINESS_UNIT")
                                cmdSelect6.Parameters.RemoveAt("@PARTNER")
                                cmdSelect6.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                                cmdSelect6.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                                dtrL6Nav.Close()
                                dtrL6Nav = cmdSelect6.ExecuteReader()
                            End If
                        End If
                    End If

                    If dtrL6Nav.HasRows Then
                        While dtrL6Nav.Read()
                            ndNewNode = xmlDoc.CreateElement("L6Node")
                            ndText = xmlDoc.CreateElement("L6Text")
                            ndText.InnerText = dtrL6Nav("GROUP_DESCRIPTION_1")
                            ndGroup = xmlDoc.CreateElement("L6Group")
                            ndGroup.InnerText = dtrL6Nav("GROUP_L06_L06_GROUP")
                            ndNavigateURL = xmlDoc.CreateElement("L6NavigateURL")
                            ndNavigateURL.InnerText = "~/PagesPublic/ProductBrowse/browse07.aspx?group1=" & dtrL6Nav("GROUP_L06_L01_GROUP") & _
                                                        "&group2=" & dtrL6Nav("GROUP_L06_L02_GROUP") & _
                                                        "&group3=" & dtrL6Nav("GROUP_L06_L03_GROUP") & _
                                                        "&group4=" & dtrL6Nav("GROUP_L06_L04_GROUP") & _
                                                        "&group5=" & dtrL6Nav("GROUP_L06_L05_GROUP") & _
                                                        "&group6=" & dtrL6Nav("GROUP_L06_L06_GROUP")
                            ndNewNode.AppendChild(ndText)
                            ndNewNode.AppendChild(ndGroup)
                            ndNewNode.AppendChild(ndNavigateURL)
                            ndL5node.AppendChild(ndNewNode)
                        End While
                    End If
                    dtrL6Nav.Close()
                Next
                '----------
                ' 7th Level
                '----------
                Const strSelect7 As String = "SELECT * FROM TBL_GROUP_LEVEL_07 AS LV WITH (NOLOCK)  INNER JOIN TBL_GROUP AS GR WITH (NOLOCK)  " & _
                                             " ON LV.GROUP_L07_L07_GROUP = GR.GROUP_NAME" & _
                                             " WHERE GROUP_L07_BUSINESS_UNIT = @BUSINESS_UNIT AND (GROUP_L07_PARTNER = @PARTNER OR GROUP_L07_PARTNER = @param6 ) AND " & _
                                             " GROUP_L07_L01_GROUP = @L1GROUP AND GROUP_L07_L02_GROUP = @L2GROUP AND " & _
                                             " GROUP_L07_L03_GROUP = @L3GROUP AND" & _
                                             " GROUP_L07_L04_GROUP = @L4GROUP AND " & _
                                             " GROUP_L07_L05_GROUP = @L5GROUP AND " & _
                                             " GROUP_L07_L06_GROUP = @L6GROUP AND GROUP_L07_SHOW_IN_NAVIGATION = 1" & _
                                             " ORDER BY LV.GROUP_L07_SEQUENCE"
                Dim cmdSelect7 As SqlCommand
                Dim ndlL6nodes As XmlNodeList = xmlDoc.SelectNodes("Navigation/L1Node/L2Node/L3Node/L4Node/L5Node/L6Node")
                For Each ndL6node As XmlNode In ndlL6nodes
                    cmdSelect7 = New SqlCommand(strSelect7, conTalent)
                    cmdSelect7.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = _businessUnit
                    cmdSelect7.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partnerParm
                    cmdSelect7.Parameters.Add(New SqlParameter("@L1GROUP", SqlDbType.Char, 20)).Value = ndL6node.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.SelectSingleNode("L1Group").InnerText
                    cmdSelect7.Parameters.Add(New SqlParameter("@L2GROUP", SqlDbType.Char, 20)).Value = ndL6node.ParentNode.ParentNode.ParentNode.ParentNode.SelectSingleNode("L2Group").InnerText
                    cmdSelect7.Parameters.Add(New SqlParameter("@L3GROUP", SqlDbType.Char, 20)).Value = ndL6node.ParentNode.ParentNode.ParentNode.SelectSingleNode("L3Group").InnerText
                    cmdSelect7.Parameters.Add(New SqlParameter("@L4GROUP", SqlDbType.Char, 20)).Value = ndL6node.ParentNode.ParentNode.SelectSingleNode("L4Group").InnerText
                    cmdSelect7.Parameters.Add(New SqlParameter("@L5GROUP", SqlDbType.Char, 20)).Value = ndL6node.ParentNode.SelectSingleNode("L5Group").InnerText
                    cmdSelect7.Parameters.Add(New SqlParameter("@L6GROUP", SqlDbType.Char, 20)).Value = ndL6node.SelectSingleNode("L6Group").InnerText
                    cmdSelect7.Parameters.Add(New SqlParameter("@param6", SqlDbType.Char)).Value = Talent.Common.Utilities.GetAllString

                    Dim dtrL7Nav As SqlDataReader = cmdSelect7.ExecuteReader
                    If dtrL7Nav.HasRows = False Then
                        ' Try all partners
                        cmdSelect7.Parameters.RemoveAt("@BUSINESS_UNIT")
                        cmdSelect7.Parameters.RemoveAt("@PARTNER")
                        cmdSelect7.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = _businessUnit
                        cmdSelect7.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                        dtrL7Nav.Close()
                        dtrL7Nav = cmdSelect7.ExecuteReader()
                        If dtrL7Nav.HasRows = False Then
                            'Try all business units
                            cmdSelect7.Parameters.RemoveAt("@BUSINESS_UNIT")
                            cmdSelect7.Parameters.RemoveAt("@PARTNER")
                            cmdSelect7.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                            cmdSelect7.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partnerParm
                            dtrL7Nav.Close()
                            dtrL7Nav = cmdSelect7.ExecuteReader()
                            If dtrL7Nav.HasRows = False Then
                                'Try all business units and partners
                                cmdSelect7.Parameters.RemoveAt("@BUSINESS_UNIT")
                                cmdSelect7.Parameters.RemoveAt("@PARTNER")
                                cmdSelect7.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                                cmdSelect7.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                                dtrL7Nav.Close()
                                dtrL7Nav = cmdSelect7.ExecuteReader()
                            End If
                        End If
                    End If

                    If dtrL7Nav.HasRows Then
                        While dtrL7Nav.Read()
                            ndNewNode = xmlDoc.CreateElement("L7Node")
                            ndText = xmlDoc.CreateElement("L7Text")
                            ndText.InnerText = dtrL7Nav("GROUP_DESCRIPTION_1")
                            ndGroup = xmlDoc.CreateElement("L7Group")
                            ndGroup.InnerText = dtrL7Nav("GROUP_L07_L07_GROUP")
                            ndNavigateURL = xmlDoc.CreateElement("L7NavigateURL")
                            ndNavigateURL.InnerText = "~/PagesPublic/ProductBrowse/browse08.aspx?group1=" & dtrL7Nav("GROUP_L07_L01_GROUP") & _
                                                        "&group2=" & dtrL7Nav("GROUP_L07_L02_GROUP") & _
                                                        "&group3=" & dtrL7Nav("GROUP_L07_L03_GROUP") & _
                                                        "&group4=" & dtrL7Nav("GROUP_L07_L04_GROUP") & _
                                                        "&group5=" & dtrL7Nav("GROUP_L07_L05_GROUP") & _
                                                        "&group6=" & dtrL7Nav("GROUP_L07_L06_GROUP") & _
                                                        "&group7=" & dtrL7Nav("GROUP_L07_L07_GROUP")
                            ndNewNode.AppendChild(ndText)
                            ndNewNode.AppendChild(ndGroup)
                            ndNewNode.AppendChild(ndNavigateURL)
                            ndL6node.AppendChild(ndNewNode)
                        End While
                    End If
                    dtrL7Nav.Close()
                Next
                '----------
                ' 8th Level
                '----------
                Const strSelect8 As String = "SELECT * FROM TBL_GROUP_LEVEL_08 AS LV WITH (NOLOCK)  INNER JOIN TBL_GROUP AS GR WITH (NOLOCK)  " & _
                                             " ON LV.GROUP_L08_L08_GROUP = GR.GROUP_NAME" & _
                                             " WHERE GROUP_L08_BUSINESS_UNIT = @BUSINESS_UNIT AND (GROUP_L08_PARTNER = @PARTNER OR GROUP_L08_PARTNER = @param6 ) AND " & _
                                             " GROUP_L08_L01_GROUP = @L1GROUP AND GROUP_L08_L02_GROUP = @L2GROUP AND " & _
                                             " GROUP_L08_L03_GROUP = @L3GROUP AND" & _
                                             " GROUP_L08_L04_GROUP = @L4GROUP AND " & _
                                             " GROUP_L08_L05_GROUP = @L5GROUP AND " & _
                                             " GROUP_L08_L06_GROUP = @L6GROUP AND " & _
                                             " GROUP_L08_L07_GROUP = @L7GROUP AND GROUP_L08_SHOW_IN_NAVIGATION = 1" & _
                                             " ORDER BY LV.GROUP_L08_SEQUENCE"
                Dim cmdSelect8 As SqlCommand
                Dim ndlL7nodes As XmlNodeList = xmlDoc.SelectNodes("Navigation/L1Node/L2Node/L3Node/L4Node/L5Node/L6Node/L7Node")
                For Each ndL7node As XmlNode In ndlL7nodes
                    cmdSelect8 = New SqlCommand(strSelect8, conTalent)
                    cmdSelect8.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = _businessUnit
                    cmdSelect8.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partnerParm
                    cmdSelect8.Parameters.Add(New SqlParameter("@L1GROUP", SqlDbType.Char, 20)).Value = ndL7node.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.SelectSingleNode("L1Group").InnerText
                    cmdSelect8.Parameters.Add(New SqlParameter("@L2GROUP", SqlDbType.Char, 20)).Value = ndL7node.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.SelectSingleNode("L2Group").InnerText
                    cmdSelect8.Parameters.Add(New SqlParameter("@L3GROUP", SqlDbType.Char, 20)).Value = ndL7node.ParentNode.ParentNode.ParentNode.ParentNode.SelectSingleNode("L3Group").InnerText
                    cmdSelect8.Parameters.Add(New SqlParameter("@L4GROUP", SqlDbType.Char, 20)).Value = ndL7node.ParentNode.ParentNode.ParentNode.SelectSingleNode("L4Group").InnerText
                    cmdSelect8.Parameters.Add(New SqlParameter("@L5GROUP", SqlDbType.Char, 20)).Value = ndL7node.ParentNode.ParentNode.SelectSingleNode("L5Group").InnerText
                    cmdSelect8.Parameters.Add(New SqlParameter("@L6GROUP", SqlDbType.Char, 20)).Value = ndL7node.ParentNode.SelectSingleNode("L6Group").InnerText
                    cmdSelect8.Parameters.Add(New SqlParameter("@L7GROUP", SqlDbType.Char, 20)).Value = ndL7node.SelectSingleNode("L7Group").InnerText
                    cmdSelect8.Parameters.Add(New SqlParameter("@param6", SqlDbType.Char)).Value = Talent.Common.Utilities.GetAllString

                    Dim dtrL8Nav As SqlDataReader = cmdSelect8.ExecuteReader
                    If dtrL8Nav.HasRows = False Then
                        ' Try all partners
                        cmdSelect8.Parameters.RemoveAt("@BUSINESS_UNIT")
                        cmdSelect8.Parameters.RemoveAt("@PARTNER")
                        cmdSelect8.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = _businessUnit
                        cmdSelect8.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                        dtrL8Nav.Close()
                        dtrL8Nav = cmdSelect8.ExecuteReader()

                        If dtrL8Nav.HasRows = False Then
                            'Try all business units
                            cmdSelect8.Parameters.RemoveAt("@BUSINESS_UNIT")
                            cmdSelect8.Parameters.RemoveAt("@PARTNER")
                            cmdSelect8.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                            cmdSelect8.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partnerParm
                            dtrL8Nav.Close()
                            dtrL8Nav = cmdSelect8.ExecuteReader()
                            If dtrL8Nav.HasRows = False Then
                                'Try all business units and partners
                                cmdSelect8.Parameters.RemoveAt("@BUSINESS_UNIT")
                                cmdSelect8.Parameters.RemoveAt("@PARTNER")
                                cmdSelect8.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                                cmdSelect8.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                                dtrL8Nav.Close()
                                dtrL8Nav = cmdSelect8.ExecuteReader()
                            End If
                        End If
                    End If

                    If dtrL8Nav.HasRows Then
                        While dtrL8Nav.Read()
                            ndNewNode = xmlDoc.CreateElement("L8Node")
                            ndText = xmlDoc.CreateElement("L8Text")
                            ndText.InnerText = dtrL8Nav("GROUP_DESCRIPTION_1")
                            ndGroup = xmlDoc.CreateElement("L8Group")
                            ndGroup.InnerText = dtrL8Nav("GROUP_L08_L08_GROUP")
                            ndNavigateURL = xmlDoc.CreateElement("L8NavigateURL")
                            ndNavigateURL.InnerText = "~/PagesPublic/ProductBrowse/browse09.aspx?group1=" & dtrL8Nav("GROUP_L08_L01_GROUP") & _
                                                        "&group2=" & dtrL8Nav("GROUP_L08_L02_GROUP") & _
                                                        "&group3=" & dtrL8Nav("GROUP_L08_L03_GROUP") & _
                                                        "&group4=" & dtrL8Nav("GROUP_L08_L04_GROUP") & _
                                                        "&group5=" & dtrL8Nav("GROUP_L08_L05_GROUP") & _
                                                        "&group6=" & dtrL8Nav("GROUP_L08_L06_GROUP") & _
                                                        "&group7=" & dtrL8Nav("GROUP_L08_L07_GROUP") & _
                                                        "&group8=" & dtrL8Nav("GROUP_L08_L08_GROUP")
                            ndNewNode.AppendChild(ndText)
                            ndNewNode.AppendChild(ndGroup)
                            ndNewNode.AppendChild(ndNavigateURL)
                            ndL7node.AppendChild(ndNewNode)
                        End While

                    End If
                    dtrL8Nav.Close()
                Next
                '----------
                ' 9th Level
                '----------
                Const strSelect9 As String = "SELECT * FROM TBL_GROUP_LEVEL_09 AS LV WITH (NOLOCK)  INNER JOIN TBL_GROUP AS GR WITH (NOLOCK)  " & _
                                             " ON LV.GROUP_L09_L09_GROUP = GR.GROUP_NAME" & _
                                             " WHERE GROUP_L09_BUSINESS_UNIT = @BUSINESS_UNIT AND (GROUP_L09_PARTNER = @PARTNER OR GROUP_L09_PARTNER = @param6 ) AND " & _
                                             " GROUP_L09_L01_GROUP = @L1GROUP AND GROUP_L09_L02_GROUP = @L2GROUP AND " & _
                                             " GROUP_L09_L03_GROUP = @L3GROUP AND" & _
                                             " GROUP_L09_L04_GROUP = @L4GROUP AND " & _
                                             " GROUP_L09_L05_GROUP = @L5GROUP AND " & _
                                             " GROUP_L09_L06_GROUP = @L6GROUP AND " & _
                                             " GROUP_L09_L07_GROUP = @L7GROUP AND " & _
                                             " GROUP_L09_L08_GROUP = @L8GROUP AND GROUP_L09_SHOW_IN_NAVIGATION = 1" & _
                                             " ORDER BY LV.GROUP_L09_SEQUENCE"
                Dim cmdSelect9 As SqlCommand
                Dim ndlL8nodes As XmlNodeList = xmlDoc.SelectNodes("Navigation/L1Node/L2Node/L3Node/L4Node/L5Node/L6Node/L7Node/L8Node")
                For Each ndL8node As XmlNode In ndlL8nodes
                    cmdSelect9 = New SqlCommand(strSelect9, conTalent)
                    cmdSelect9.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = _businessUnit
                    cmdSelect9.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partnerParm
                    cmdSelect9.Parameters.Add(New SqlParameter("@L1GROUP", SqlDbType.Char, 20)).Value = ndL8node.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.SelectSingleNode("L1Group").InnerText
                    cmdSelect9.Parameters.Add(New SqlParameter("@L2GROUP", SqlDbType.Char, 20)).Value = ndL8node.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.SelectSingleNode("L2Group").InnerText
                    cmdSelect9.Parameters.Add(New SqlParameter("@L3GROUP", SqlDbType.Char, 20)).Value = ndL8node.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.SelectSingleNode("L3Group").InnerText
                    cmdSelect9.Parameters.Add(New SqlParameter("@L4GROUP", SqlDbType.Char, 20)).Value = ndL8node.ParentNode.ParentNode.ParentNode.ParentNode.SelectSingleNode("L4Group").InnerText
                    cmdSelect9.Parameters.Add(New SqlParameter("@L5GROUP", SqlDbType.Char, 20)).Value = ndL8node.ParentNode.ParentNode.ParentNode.SelectSingleNode("L5Group").InnerText
                    cmdSelect9.Parameters.Add(New SqlParameter("@L6GROUP", SqlDbType.Char, 20)).Value = ndL8node.ParentNode.ParentNode.SelectSingleNode("L6Group").InnerText
                    cmdSelect9.Parameters.Add(New SqlParameter("@L7GROUP", SqlDbType.Char, 20)).Value = ndL8node.ParentNode.SelectSingleNode("L7Group").InnerText
                    cmdSelect9.Parameters.Add(New SqlParameter("@L8GROUP", SqlDbType.Char, 20)).Value = ndL8node.SelectSingleNode("L8Group").InnerText
                    cmdSelect9.Parameters.Add(New SqlParameter("@param6", SqlDbType.Char)).Value = Talent.Common.Utilities.GetAllString

                    Dim dtrL9Nav As SqlDataReader = cmdSelect9.ExecuteReader
                    If dtrL9Nav.HasRows = False Then
                        ' Try all partners
                        cmdSelect9.Parameters.RemoveAt("@BUSINESS_UNIT")
                        cmdSelect9.Parameters.RemoveAt("@PARTNER")
                        cmdSelect9.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = _businessUnit
                        cmdSelect9.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                        dtrL9Nav.Close()
                        dtrL9Nav = cmdSelect9.ExecuteReader()
                        If dtrL9Nav.HasRows = False Then
                            'Try all business units
                            cmdSelect9.Parameters.RemoveAt("@BUSINESS_UNIT")
                            cmdSelect9.Parameters.RemoveAt("@PARTNER")
                            cmdSelect9.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                            cmdSelect9.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partnerParm
                            dtrL9Nav.Close()
                            dtrL9Nav = cmdSelect9.ExecuteReader()
                            If dtrL9Nav.HasRows = False Then
                                'Try all business units and partners
                                cmdSelect9.Parameters.RemoveAt("@BUSINESS_UNIT")
                                cmdSelect9.Parameters.RemoveAt("@PARTNER")
                                cmdSelect9.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                                cmdSelect9.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                                dtrL9Nav.Close()
                                dtrL9Nav = cmdSelect9.ExecuteReader()
                            End If
                        End If
                    End If

                    If dtrL9Nav.HasRows Then
                        While dtrL9Nav.Read()
                            ndNewNode = xmlDoc.CreateElement("L9Node")
                            ndText = xmlDoc.CreateElement("L9Text")
                            ndText.InnerText = dtrL9Nav("GROUP_DESCRIPTION_1")
                            ndGroup = xmlDoc.CreateElement("L9Group")
                            ndGroup.InnerText = dtrL9Nav("GROUP_L09_L09_GROUP")
                            ndNavigateURL = xmlDoc.CreateElement("L9NavigateURL")
                            ndNavigateURL.InnerText = "~/PagesPublic/ProductBrowse/browse10.aspx?group1=" & dtrL9Nav("GROUP_L09_L01_GROUP") & _
                                                        "&group2=" & dtrL9Nav("GROUP_L09_L02_GROUP") & _
                                                        "&group3=" & dtrL9Nav("GROUP_L09_L03_GROUP") & _
                                                        "&group4=" & dtrL9Nav("GROUP_L09_L04_GROUP") & _
                                                        "&group5=" & dtrL9Nav("GROUP_L09_L05_GROUP") & _
                                                        "&group6=" & dtrL9Nav("GROUP_L09_L06_GROUP") & _
                                                        "&group7=" & dtrL9Nav("GROUP_L09_L07_GROUP") & _
                                                        "&group8=" & dtrL9Nav("GROUP_L09_L08_GROUP") & _
                                                        "&group9=" & dtrL9Nav("GROUP_L09_L09_GROUP")
                            ndNewNode.AppendChild(ndText)
                            ndNewNode.AppendChild(ndGroup)
                            ndNewNode.AppendChild(ndNavigateURL)
                            ndL8node.AppendChild(ndNewNode)
                        End While
                    End If
                    dtrL9Nav.Close()
                Next

            Catch ex As Exception
                Const strError8 As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8
                    .ErrorNumber = "TTPXXXX-06"
                    .HasError = True
                End With
            Finally
                '--------------------------------------------------------------------
                ' Close
                '
                conTalent.Close()
            End Try
            HttpContext.Current.Cache.Insert(cacheKey, xmlDoc.InnerXml, Nothing, System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), Caching.Cache.NoSlidingExpiration)
            Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
        End If
        Return xmlDoc
    End Function

#End Region

End Class