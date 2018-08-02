Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.eCommerce
Imports Talent.Common
Imports System.Xml
Imports System.Globalization
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Group Graphical
'
'       Date                        13/03/07
'
'       Author                      Andrew Green
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      UCGG- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_GroupGraphical
    Inherits ControlBase

    Private _businessUnit As String
    Private _partner As String
    Private _currentPage As String
    Private _PageNumber As Integer = 1
    Private _HasRows As Boolean = False
    Private _IsPaging As Boolean = False
    Private _usage As String = Talent.Common.Utilities.GetAllString
    Private _display As Boolean = True
    Dim groupList As GroupListGen
    Private ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private requestGroups() As String = {"GROUP_L01_GROUP = @GROUP1", "GROUP_L02_GROUP = @GROUP2", _
                                         "GROUP_L03_GROUP = @GROUP3", "GROUP_L04_GROUP = @GROUP4", _
                                         "GROUP_L05_GROUP = @GROUP5", "GROUP_L06_GROUP = @GROUP6", _
                                         "GROUP_L07_GROUP = @GROUP7", "GROUP_L08_GROUP = @GROUP8", _
                                         "GROUP_L09_GROUP = @GROUP9", "GROUP_L10_GROUP = @GROUP10"}
    Private pageLevel() As String = {"browse01.aspx", "browse02.aspx", "browse03.aspx", "browse04.aspx", "browse05.aspx", _
                                     "browse06.aspx", "browse07.aspx", "browse08.aspx", "browse09.aspx", "browse10.aspx"}

    Public Property Display() As Boolean
        Get
            Return _display
        End Get
        Set(ByVal value As Boolean)
            _display = value
        End Set
    End Property
    Public Property PageNumber() As Integer
        Get
            Return _PageNumber
        End Get
        Set(ByVal i As Integer)
            _PageNumber = i
        End Set
    End Property

    Public WriteOnly Property IsPaging() As Boolean
        Set(ByVal value As Boolean)
            _IsPaging = value
        End Set
    End Property


    Public ReadOnly Property HasRows() As Boolean
        Get
            Return _HasRows
        End Get
    End Property

    Public Property Usage() As String
        Get
            Return _usage
        End Get
        Set(ByVal value As String)
            _usage = value
        End Set
    End Property


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _businessUnit = TalentCache.GetBusinessUnit
        _partner = TalentCache.GetPartner(HttpContext.Current.Profile)
        _currentPage = Talent.eCommerce.Utilities.GetCurrentPageName()

        If IsValidID(Request("page")) Then
            _PageNumber = Int32.Parse(Request("page"))
            _IsPaging = True
        End If

        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = Usage()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "groupGraphical.ascx"
        End With

        If Display Then
            pnlGroupGraphical.Visible = True
        Else
            pnlGroupGraphical.Visible = False
        End If

        dlsGroupGraphical.RepeatColumns = CInt(ucr.Attribute("RepeatColumns"))

        '        If Not Page.IsPostBack And Display Then
        If Display Then
            LoadGroups()
        End If

    End Sub

    Protected Sub LoadGroups()

        Dim pageGroupList As IList
        Dim filePath As String = String.Empty


        groupList = New GroupListGen
        groupList.PageSize = CType(ucr.Attribute("PageSize"), Integer)

        '
        ' Determine the current level and the parent groups already selected
        Dim currentPageLevel As Integer = Array.IndexOf(pageLevel, _currentPage)
        Dim strGroup01 As String = String.Empty
        Dim strGroup02 As String = String.Empty
        Dim strGroup03 As String = String.Empty
        Dim strGroup04 As String = String.Empty
        Dim strGroup05 As String = String.Empty
        Dim strGroup06 As String = String.Empty
        Dim strGroup07 As String = String.Empty
        Dim strGroup08 As String = String.Empty
        Dim strGroup09 As String = String.Empty

        '
        ' Determine the groups
        If currentPageLevel >= 1 Then strGroup01 = Request("group1").ToString.Trim
        If currentPageLevel >= 2 Then strGroup02 = Request("group2").ToString.Trim
        If currentPageLevel >= 3 Then strGroup03 = Request("group3").ToString.Trim
        If currentPageLevel >= 4 Then strGroup04 = Request("group4").ToString.Trim
        If currentPageLevel >= 5 Then strGroup05 = Request("group5").ToString.Trim
        If currentPageLevel >= 6 Then strGroup06 = Request("group6").ToString.Trim
        If currentPageLevel >= 7 Then strGroup07 = Request("group7").ToString.Trim
        If currentPageLevel >= 8 Then strGroup08 = Request("group8").ToString.Trim
        If currentPageLevel >= 9 Then strGroup09 = Request("group9").ToString.Trim

        '
        ' Determine which partner (*ALL or specific partner) is used at top-level and
        ' then use this partner for all subsequent levels
        ''Dim groupDetails As Talent.eCommerce.GroupLevelDetails
        ''Dim dets As Talent.eCommerce.GroupLevelDetails.GroupDetails
        ''groupDetails = New Talent.eCommerce.GroupLevelDetails
        ''dets = groupDetails.GetGroupLevelDetails()
        ''Dim group_partner As String = dets.GroupPartner
        Dim group_partner As String = String.Empty
        If Talent.eCommerce.Utilities.GroupNavigationUsingAll() Then
            group_partner = Talent.Common.Utilities.GetAllString
        Else
            group_partner = _partner
        End If

        '
        ' Get groups to display 
        Dim egd As New ECommerceGroupDefaults
        Dim dt As DataTable = Nothing
        Dim dr As DataRow = Nothing
        Select Case currentPageLevel
            Case Is = 0
                dt = egd.GetGroups(_businessUnit, _partner)
                If dt.Rows.Count = 0 Then
                    dt = egd.GetGroups(_businessUnit, Talent.Common.Utilities.GetAllString)
                End If
            Case Is = 1
                dt = egd.GetGroups(_businessUnit, group_partner, strGroup01)
            Case Is = 2
                dt = egd.GetGroups(_businessUnit, group_partner, strGroup01, strGroup02)
            Case Is = 3
                dt = egd.GetGroups(_businessUnit, group_partner, strGroup01, strGroup02, strGroup03)
            Case Is = 4
                dt = egd.GetGroups(_businessUnit, group_partner, strGroup01, strGroup02, strGroup03, strGroup04)
            Case Is = 5
                dt = egd.GetGroups(_businessUnit, group_partner, strGroup01, strGroup02, strGroup03, strGroup04, strGroup05)
            Case Is = 6
                dt = egd.GetGroups(_businessUnit, group_partner, strGroup01, strGroup02, strGroup03, strGroup04, strGroup05, strGroup06)
            Case Is = 7
                dt = egd.GetGroups(_businessUnit, group_partner, strGroup01, strGroup02, strGroup03, strGroup04, strGroup05, strGroup06, strGroup07)
            Case Is = 8
                dt = egd.GetGroups(_businessUnit, group_partner, strGroup01, strGroup02, strGroup03, strGroup04, strGroup05, strGroup06, strGroup07, strGroup08)
            Case Is = 9
                dt = egd.GetGroups(_businessUnit, group_partner, strGroup01, strGroup02, strGroup03, strGroup04, strGroup05, strGroup06, strGroup07, strGroup08, strGroup09)
        End Select


        '
        ' Load a GroupLevelDetails.GroupDetails object for each child group found
        ' and then add this object to the pageGroupList
        If dt.Rows.Count > 0 Then
            _HasRows = True

            For Each dr In dt.Rows

                Dim gd As New GroupLevelDetails.GroupDetails
                If dr("LANG_DESCRIPTION1").ToString.Trim = "" Then
                    gd.GroupDescription1 = Talent.eCommerce.Utilities.CheckForDBNull_String(dr("DESCRIPTION1"))
                Else
                    gd.GroupDescription1 = Talent.eCommerce.Utilities.CheckForDBNull_String(dr("LANG_DESCRIPTION1"))
                End If
                If dr("LANG_DESCRIPTION2").ToString.Trim = "" Then
                    gd.GroupDescription2 = Talent.eCommerce.Utilities.CheckForDBNull_String(dr("DESCRIPTION2"))
                Else
                    gd.GroupDescription2 = Talent.eCommerce.Utilities.CheckForDBNull_String(dr("LANG_DESCRIPTION2"))
                End If
                gd.GroupHtml1 = ""
                gd.GroupHtml2 = ""
                gd.GroupHtml3 = ""
                gd.GroupMetaDescription = ""
                gd.GroupMetaKeywords = ""
                gd.GroupName = Talent.eCommerce.Utilities.CheckForDBNull_String(dr("GROUP_NAME"))
                gd.GroupPageTitle = ""

                '-----------------
                ' Find thumb image
                '-----------------
                filePath = ImagePath.getGroupImagePath("GROUPTHUMB", gd.GroupName, _businessUnit, _partner)
                gd.GroupImagePath = filePath

                If gd.GroupImagePath <> String.Empty Then
                    gd.GroupAltText = gd.GroupDescription1
                Else
                    gd.GroupAltText = String.Empty
                End If

                '----------------------------
                ' Build navigate query string
                '----------------------------
                Dim myDefaults As New ECommerceModuleDefaults
                Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
                Dim intMaxNoOfGroupLevels As Integer = def.NumberOfGroupLevels
                Dim sbQry As New StringBuilder
                With sbQry
                    .Append("../PagesPublic/ProductBrowse/")
                    Select Case currentPageLevel
                        Case Is = 1
                            .Append("browse03.aspx?group1=").Append(strGroup01)
                            .Append("&group2=").Append(dr("GROUP_NAME"))
                        Case Is = 2
                            .Append("browse04.aspx?group1=").Append(strGroup01)
                            .Append("&group2=").Append(strGroup02)
                            .Append("&group3=").Append(dr("GROUP_NAME"))
                        Case Is = 3
                            .Append("browse05.aspx?group1=").Append(strGroup01)
                            .Append("&group2=").Append(strGroup02)
                            .Append("&group3=").Append(strGroup03)
                            .Append("&group4=").Append(dr("GROUP_NAME"))
                        Case Is = 4
                            .Append("browse06.aspx?group1=").Append(strGroup01)
                            .Append("&group2=").Append(strGroup02)
                            .Append("&group3=").Append(strGroup03)
                            .Append("&group4=").Append(strGroup04)
                            .Append("&group5=").Append(dr("GROUP_NAME"))
                        Case Is = 5
                            .Append("browse07.aspx?group1=").Append(strGroup01)
                            .Append("&group2=").Append(strGroup02)
                            .Append("&group3=").Append(strGroup03)
                            .Append("&group4=").Append(strGroup04)
                            .Append("&group5=").Append(strGroup05)
                            .Append("&group6=").Append(dr("GROUP_NAME"))
                        Case Is = 6
                            .Append("browse08.aspx?group1=").Append(strGroup01)
                            .Append("&group2=").Append(strGroup02)
                            .Append("&group3=").Append(strGroup03)
                            .Append("&group4=").Append(strGroup04)
                            .Append("&group5=").Append(strGroup05)
                            .Append("&group6=").Append(strGroup06)
                            .Append("&group7=").Append(dr("GROUP_NAME"))
                        Case Is = 7
                            .Append("browse09.aspx?group1=").Append(strGroup01)
                            .Append("&group2=").Append(strGroup02)
                            .Append("&group3=").Append(strGroup03)
                            .Append("&group4=").Append(strGroup04)
                            .Append("&group5=").Append(strGroup05)
                            .Append("&group6=").Append(strGroup06)
                            .Append("&group7=").Append(strGroup07)
                            .Append("&group8=").Append(dr("GROUP_NAME"))
                        Case Is = 8
                            .Append("browse10.aspx?group1=").Append(strGroup01)
                            .Append("&group2=").Append(strGroup02)
                            .Append("&group3=").Append(strGroup03)
                            .Append("&group4=").Append(strGroup04)
                            .Append("&group5=").Append(strGroup05)
                            .Append("&group6=").Append(strGroup06)
                            .Append("&group7=").Append(strGroup07)
                            .Append("&group8=").Append(strGroup08)
                            .Append("&group9=").Append(dr("GROUP_NAME"))
                    End Select
                End With
                gd.GroupNavigateURL = sbQry.ToString
                groupList.Add(gd)
            Next
        End If

        '
        ' And bind the pageGroupList to the Data List control 
        If Not groupList Is Nothing Then
            pageGroupList = groupList.GetPageGroups(_PageNumber)
            dlsGroupGraphical.DataSource = pageGroupList
            dlsGroupGraphical.DataBind()
        End If


    End Sub

    Public Function NowShowingResultsString() As String

        Dim startAmount As Integer = 0
        Dim endAmount As Integer = 0

        Dim displaying1 As String = ucr.Content("DisplayingText1", _languageCode, True)
        Dim displaying2 As String = ucr.Content("DisplayingText2", _languageCode, True)
        Dim displaying3 As String = ucr.Content("DisplayingText3", _languageCode, True)
        Dim noProductsFound As String = ucr.Content("NoProductsFoundText", _languageCode, True)
        Dim str As String = ""

        If groupList.Count > 0 Then
            startAmount = ((_PageNumber - 1) * groupList.PageSize) + 1
            If groupList.Count < (_PageNumber * groupList.PageSize) Then
                endAmount = groupList.Count
            Else
                endAmount = (_PageNumber * groupList.PageSize)
            End If
            str = displaying1 & " " & startAmount & " " & displaying2 & " " & endAmount & " " & displaying3 & " " & groupList.Count
        Else
            str = noProductsFound
        End If

        Return str

    End Function

    Public Function PagingString() As String

        Dim str As String = ""
        Dim linkPage As String = _currentPage & "?" & Request.QueryString.ToString
        Dim firstText As String = ucr.Content("firstText", _languageCode, True)
        Dim previousText As String = ucr.Content("previousText", _languageCode, True)
        Dim nextText As String = ucr.Content("nextText", _languageCode, True)
        Dim lastText As String = ucr.Content("lastText", _languageCode, True)
        Dim pageSeperator As String = ucr.Attribute("pageSeperator")
        Dim numberOfLinks As Integer = CType(ucr.Attribute("numberOfLinks"), Integer)
        Dim showFirstLast As Boolean = CType(ucr.Attribute("showFirstLast"), Boolean)
        Dim showSeperator As Boolean = CType(ucr.Attribute("showSeperator"), Boolean)
        Dim nextPreviousAsImages As Boolean = CType(ucr.Attribute("nextPreviousAsImages"), Boolean)
        Dim previousImage As String = ucr.Attribute("previousImage")
        Dim nextImage As String = ucr.Attribute("nextImage")

        If linkPage.Contains("&page=") Then
            linkPage = linkPage.Substring(0, linkPage.IndexOf("&page"))
        End If
        '----------------------------------------------------------------
        'If there is only one page of data, don't display any paging info
        '----------------------------------------------------------------
        If groupList.Count <= groupList.PageSize Then
            Return ""
        End If
        '--------------------------------------------------
        'Is this a valid page for the amount of order lines
        '--------------------------------------------------
        If _PageNumber > groupList.NumberOfPages Then
            '----------------------------------------------------------------------
            'The requested page number is to high for the amount of pages available
            'Re-set the page to page 1.
            '----------------------------------------------------------------------
            _PageNumber = 1
        End If
        '--------------------------------------------
        'Display the 'First' Link and the 'Next' Link
        '--------------------------------------------
        If _PageNumber > 1 Then
            If showFirstLast Then
                str = str & "<a href=""" & linkPage & "&page=1"">" & firstText & "</a> "
            End If
            If nextPreviousAsImages Then
                str = str & "<a href=""" & linkPage & "&page=" & (_PageNumber - 1) & """>" & "<img src=""" & previousImage & """>" & "</a> "
            Else
                str = str & "<a href=""" & linkPage & "&page=" & (_PageNumber - 1) & """>" & previousText & "</a> "
            End If
        End If
        '-----------------------------------------------------------------------------------------------
        'Display the page links
        'No more than number_of_links Links should be displayed
        'Note: if number_of_links is an ever number and the current pagenumber is greater
        'than number_of_links then the the actual number of links displayed will be number_of_links + 1
        'this is ensure that the current page appears in the centre.
        '-----------------------------------------------------------------------------------------------
        Dim counter As Integer = 1
        If groupList.NumberOfPages <= numberOfLinks Then
            '----------------------------------------------------------
            'List out the pages, current page does not have a hyperlink
            '----------------------------------------------------------
            For counter = 1 To groupList.NumberOfPages
                If counter = _PageNumber Then
                    If counter = groupList.NumberOfPages Or Not showSeperator Then
                        str = str & " " & counter & " "
                    Else
                        str = str & " " & counter & " " & pageSeperator & " "
                    End If
                Else
                    If counter = groupList.NumberOfPages Or Not showSeperator Then
                        str = str & " <a href=""" & linkPage & "&page=" & counter & """>" & counter & "</a> "
                    Else
                        str = str & " <a href=""" & linkPage & "&page=" & counter & """>" & counter & "</a> " & " " & pageSeperator
                    End If
                End If
            Next
        Else
            '------------------------------------------------------------------------------
            'If the current page is greater than number_of_links then the current page will
            'be shown in the centre of the list of links.
            '------------------------------------------------------------------------------
            If _PageNumber <= numberOfLinks Then
                For counter = 1 To numberOfLinks
                    If counter = _PageNumber Then
                        If counter = numberOfLinks Or Not showSeperator Then
                            str = str & " " & counter & " "
                        Else
                            str = str & " " & counter & " " & pageSeperator & " "
                        End If
                    Else
                        If counter = numberOfLinks Or Not showSeperator Then
                            str = str & " <a href=""" & linkPage & "&page=" & counter & """>" & counter & "</a> "
                        Else
                            str = str & " <a href=""" & linkPage & "&page=" & counter & """>" & counter & "</a> " & " " & pageSeperator
                        End If
                    End If
                Next
            ElseIf _PageNumber > (groupList.NumberOfPages - numberOfLinks) Then
                '--------------------------------------------------
                'display the last 'number_of_links number' of links
                '--------------------------------------------------
                For counter = (groupList.NumberOfPages - numberOfLinks) To groupList.NumberOfPages
                    If counter = _PageNumber Then
                        If counter = groupList.NumberOfPages Or Not showSeperator Then
                            str = str & " " & counter & " "
                        Else
                            str = str & " " & counter & " " & pageSeperator & " "
                        End If
                    Else
                        If counter = groupList.NumberOfPages Or Not showSeperator Then
                            str = str & " <a href=""" & linkPage & "&page=" & counter & """>" & counter & "</a> "
                        Else
                            str = str & " <a href=""" & linkPage & "&page=" & counter & """>" & counter & "</a> " & " " & pageSeperator
                        End If
                    End If
                Next
            Else
                Dim halfWay As Integer = numberOfLinks / 2
                For counter = 1 To halfWay - 1
                    str = str & " <a href=""" & linkPage & "&page=" & (_PageNumber - halfWay + counter) & """>" & (_PageNumber - halfWay + counter) & "</a> "
                Next
                str = str & " " & _PageNumber & " "
                For counter = 1 To halfWay
                    If counter = halfWay Or Not showSeperator Then
                        str = str & " <a href=""" & linkPage & "&page=" & (_PageNumber + counter) & """>" & (_PageNumber + counter) & "</a> "
                    Else
                        str = str & " <a href=""" & linkPage & "&page=" & (_PageNumber + counter) & """>" & (_PageNumber + counter) & "</a> " & " " & pageSeperator
                    End If
                Next
            End If
        End If
        '-------------------------------------------
        'Display the 'Next' link and the 'Last' link
        '-------------------------------------------
        If _PageNumber < groupList.NumberOfPages Then
            If nextPreviousAsImages Then
                str = str & "<a href=""" & linkPage & "&page=" & (_PageNumber + 1) & """>" & "<img src=""" & nextImage & """>" & "</a> "
            Else
                str = str & "<a href=""" & linkPage & "&page=" & (_PageNumber + 1) & """>" & nextText & "</a> "
            End If
            If showFirstLast Then
                str = str & "<a href=""" & linkPage & "&page=" & groupList.NumberOfPages & """>" & lastText & "</a>"
            End If
        End If

        Return str

    End Function

    Function IsValidID(ByVal strID As String) As Boolean
        Dim intID As Int32
        Try
            intID = Int32.Parse(strID)
        Catch
            Return False
        End Try
        '----------------------
        ' don't allow negatives
        '----------------------
        If intID < 0 Then
            Return False
        End If
        Return True
    End Function

End Class
