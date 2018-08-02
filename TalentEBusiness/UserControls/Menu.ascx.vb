Imports Talent.eCommerce

Partial Class UserControls_Menu
    Inherits ControlBase

    Const Header As String = "HEADER"
    Const Top As String = "TOP"
    Const Left As String = "LEFT"
    Const Right As String = "RIGHT"
    Const Centre As String = "CENTRE"
    Const Footer As String = "FOOTER"
    Const Before As String = "BEFORE"
    Const After As String = "AFTER"
    Dim LocationsList As New ArrayList
    Dim PositionsList As New ArrayList

    Public Enum MyLocations
        header = 0
        top = 1
        left = 2
        right = 3
        centre = 4
        footer = 5
    End Enum
    Public Enum MyPosition
        before = 0
        after = 1
    End Enum

    Private _display As Boolean
    Public Property Display() As Boolean
        Get
            Return _display
        End Get
        Set(ByVal value As Boolean)
            _display = value
        End Set
    End Property

    Private _location As MyLocations
    Public Property Location() As MyLocations
        Get
            Return _location
        End Get
        Set(ByVal value As MyLocations)
            _location = value
        End Set
    End Property

    Private _position As MyPosition
    Public Property Position() As MyPosition
        Get
            Return _position
        End Get
        Set(ByVal value As MyPosition)
            _position = value
        End Set
    End Property

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With LocationsList
            .Add(Header)
            .Add(Top)
            .Add(Left)
            .Add(Right)
            .Add(Centre)
            .Add(Footer)
        End With
        With PositionsList
            .Add(Before)
            .Add(After)
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Display Then
            'Data from cache so not needed 
            'If Not Page.IsPostBack Then
            LoadMenus()
            'End If
        Else
        Me.Visible = False
        End If
    End Sub

    Protected Sub LoadMenus()
        Dim myMenus As New TalentMenus
        myMenus.LoadMenus(TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), Utilities.GetCurrentPageName, LocationsList(Location), PositionsList(Position))

        If Me.ID = "mnuAgentMyAccount" Then
            MenuTree.Visible = False
            MenuTree.Enabled = False
            MenuList.Attributes.Remove("class")
            MenuList.Attributes.Add("class", "side-nav ebiz-agent-my-account-nav")
            PopulateMenuList(myMenus)
        Else

            Select Case Location
                Case MyLocations.centre, MyLocations.header, MyLocations.top, MyLocations.right, MyLocations.footer
                    MenuTree.Visible = False
                    MenuTree.Enabled = False
                    PopulateMenuList(myMenus)

                Case MyLocations.left
                    MenuList.Visible = False
                    PopulateMenuTree(myMenus)

            End Select
        End If

    End Sub

    Protected Sub PopulateMenuTree(ByVal myMenus As TalentMenus)
        Dim myNode As New TreeNode

        For Each tm As TalentMenu In myMenus.Menus
            For Each tmi As TalentMenuItem In tm.MenuItems
                myNode = New TreeNode
                myNode.NavigateUrl = tmi.Navigate_Url
                myNode.Text = tmi.Display_Content
                MenuTree.Nodes.Add(myNode)

            Next
        Next
    End Sub

    Protected Sub PopulateMenuList(ByVal myMenus As TalentMenus)

        Dim li As New HtmlGenericControl("li")
        Dim hl As New HyperLink
        Dim img As New Image
        For Each tm As TalentMenu In myMenus.Menus
            For Each tmi As TalentMenuItem In tm.MenuItems

                li = New HtmlGenericControl("li")
                hl = New HyperLink

                hl.Text = tmi.Display_Content
                hl.NavigateUrl = tmi.Navigate_Url

                Select Case Location
                    Case MyLocations.centre
                        img = New Image
                        img.ImageUrl = tmi.Image_URL
                        img.AlternateText = tmi.Display_Content
                        hl.Controls.Add(img)
                End Select

                li.Attributes.Add("class", tmi.Css_Class)
                li.Controls.Add(hl)

                MenuList.Controls.Add(li)
            Next
        Next
    End Sub


End Class
