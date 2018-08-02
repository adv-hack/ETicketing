Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    Group Level Details. Return information for group
'
'       Date                        060307
'
'       Author                      Ben Ford
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'       
'--------------------------------------------------------------------------------------------------
Namespace Talent.eCommerce

    Public Class GroupLevelDetails
        '------------------------------------------------
        ' Nested class for returning and storing in cache
        '------------------------------------------------
        Public Class GroupDetails

            Private _groupDescription1 As String
            Private _groupDescription2 As String
            Private _groupPageTitle As String
            Private _groupHtml1 As String
            Private _groupHtml2 As String
            Private _groupHtml3 As String
            Private _groupMetaDescription As String
            Private _groupMetaKeywords As String
            Private _groupName As String
            Private _groupImagePath As String
            Private _groupNavigateURL As String
            Private _groupPartner As String
            Private _groupAltText As String
            Private _groupTheme As String

            Public Property GroupDescription1() As String
                Get
                    Return _groupDescription1
                End Get
                Set(ByVal value As String)
                    _groupDescription1 = value
                End Set
            End Property
            Public Property GroupDescription2() As String
                Get
                    Return _groupDescription2
                End Get
                Set(ByVal value As String)
                    _groupDescription2 = value
                End Set
            End Property
            Public Property GroupPageTitle() As String
                Get
                    Return _groupPageTitle
                End Get
                Set(ByVal value As String)
                    _groupPageTitle = value
                End Set
            End Property
            Public Property GroupHtml1() As String
                Get
                    Return _groupHtml1
                End Get
                Set(ByVal value As String)
                    _groupHtml1 = value
                End Set
            End Property
            Public Property GroupHtml2() As String
                Get
                    Return _groupHtml2
                End Get
                Set(ByVal value As String)
                    _groupHtml2 = value
                End Set
            End Property
            Public Property GroupHtml3() As String
                Get
                    Return _groupHtml3
                End Get
                Set(ByVal value As String)
                    _groupHtml3 = value
                End Set
            End Property
            Public Property GroupMetaDescription() As String
                Get
                    Return _groupMetaDescription
                End Get
                Set(ByVal value As String)
                    _groupMetaDescription = value
                End Set
            End Property
            Public Property GroupMetaKeywords() As String
                Get
                    Return _groupMetaKeywords
                End Get
                Set(ByVal value As String)
                    _groupMetaKeywords = value
                End Set
            End Property
            Public Property GroupName() As String
                Get
                    Return _groupName
                End Get
                Set(ByVal value As String)
                    _groupName = value
                End Set
            End Property
            Public Property GroupImagePath() As String
                Get
                    Return _groupImagePath
                End Get
                Set(ByVal value As String)
                    _groupImagePath = value
                End Set
            End Property
            Public Property GroupNavigateURL() As String
                Get
                    Return _groupNavigateURL
                End Get
                Set(ByVal value As String)
                    _groupNavigateURL = value
                End Set
            End Property
            Public Property GroupPartner() As String
                Get
                    Return _groupPartner
                End Get
                Set(ByVal value As String)
                    _groupPartner = value
                End Set
            End Property
            Public Property GroupAltText() As String
                Get
                    Return _groupAltText
                End Get
                Set(ByVal value As String)
                    _groupAltText = value
                End Set
            End Property
            Public Property GroupTheme() As String
                Get
                    Return _groupTheme
                End Get
                Set(ByVal value As String)
                    _groupTheme = value
                End Set
            End Property

            Private _showProductDisplay As Boolean
            Public Property ShowProductDisplay() As Boolean
                Get
                    Return _showProductDisplay
                End Get
                Set(ByVal value As Boolean)
                    _showProductDisplay = value
                End Set
            End Property

            Private _showChildrenAsGroups As Boolean
            Public Property ShowChildrenAsGroups() As Boolean
                Get
                    Return _showChildrenAsGroups
                End Get
                Set(ByVal value As Boolean)
                    _showChildrenAsGroups = value
                End Set
            End Property

        End Class


        Public Function GetGroupLevelDetails(Optional ByVal pageName As String = "") As GroupDetails

            Dim level As Integer
            Dim dets As New GroupDetails
            Dim cacheKey As String
            Dim cacheKeyGroups As String = String.Empty
            Dim _businessUnit As String = TalentCache.GetBusinessUnit()
            Dim _partner As String = TalentCache.GetPartner(HttpContext.Current.Profile)

            Dim group1 As String = String.Empty
            Dim group2 As String = String.Empty
            Dim group3 As String = String.Empty
            Dim group4 As String = String.Empty
            Dim group5 As String = String.Empty
            Dim group6 As String = String.Empty
            Dim group7 As String = String.Empty
            Dim group8 As String = String.Empty
            Dim group9 As String = String.Empty
            Dim group10 As String = String.Empty

            If Not HttpContext.Current.Request("group1") Is Nothing Then
                group1 = HttpContext.Current.Request("group1")
            End If
            If Not HttpContext.Current.Request("group2") Is Nothing Then
                group2 = HttpContext.Current.Request("group2")
            End If
            If Not HttpContext.Current.Request("group3") Is Nothing Then
                group3 = HttpContext.Current.Request("group3")
            End If
            If Not HttpContext.Current.Request("group4") Is Nothing Then
                group4 = HttpContext.Current.Request("group4")
            End If
            If Not HttpContext.Current.Request("group5") Is Nothing Then
                group5 = HttpContext.Current.Request("group5")
            End If
            If Not HttpContext.Current.Request("group6") Is Nothing Then
                group6 = HttpContext.Current.Request("group6")
            End If
            If Not HttpContext.Current.Request("group7") Is Nothing Then
                group7 = HttpContext.Current.Request("group7")
            End If
            If Not HttpContext.Current.Request("group8") Is Nothing Then
                group8 = HttpContext.Current.Request("group8")
            End If
            If Not HttpContext.Current.Request("group9") Is Nothing Then
                group9 = HttpContext.Current.Request("group9")
            End If
            If Not HttpContext.Current.Request("group10") Is Nothing Then
                group10 = HttpContext.Current.Request("group10")
            End If

            Dim currentPage As String = Utilities.GetCurrentPageName()

            'Overrid the page name?
            If pageName <> "" Then
                currentPage = pageName
            End If

            Dim groupName As String = String.Empty
            '-------------------------------------
            ' Set group depending on current page
            ' If current group is *EMPTY then look
            ' at previous group
            '-------------------------------------
            While (groupName = String.Empty Or _
                   groupName = "*EMPTY") And _
                   currentPage <> "browse01.aspx"

                Select Case currentPage
                    Case Is = "browse01.aspx"
                        groupName = HttpContext.Current.Request("group1")
                        currentPage = "browse01.aspx"
                        level = 1
                    Case Is = "browse02.aspx"
                        groupName = HttpContext.Current.Request("group1")
                        currentPage = "browse01.aspx"
                        level = 1
                    Case Is = "browse03.aspx"
                        groupName = HttpContext.Current.Request("group2")
                        cacheKeyGroups = Talent.Common.Utilities.FixStringLength(group1, 20)
                        currentPage = "browse02.aspx"
                        level = 2
                    Case Is = "browse04.aspx"
                        groupName = HttpContext.Current.Request("group3")
                        currentPage = "browse03.aspx"
                        cacheKeyGroups = Talent.Common.Utilities.FixStringLength(group1, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group2, 20)
                        level = 3
                    Case Is = "browse05.aspx"
                        groupName = HttpContext.Current.Request("group4")
                        currentPage = "browse04.aspx"
                        cacheKeyGroups = Talent.Common.Utilities.FixStringLength(group1, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group2, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group3, 20)
                        level = 4
                    Case Is = "browse06.aspx"
                        groupName = HttpContext.Current.Request("group5")
                        currentPage = "browse05.aspx"
                        cacheKeyGroups = Talent.Common.Utilities.FixStringLength(group1, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group2, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group3, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group4, 20)
                        level = 5
                    Case Is = "browse07.aspx"
                        groupName = HttpContext.Current.Request("group6")
                        currentPage = "browse06.aspx"
                        cacheKeyGroups = Talent.Common.Utilities.FixStringLength(group1, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group2, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group3, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group4, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group5, 20)
                        level = 6
                    Case Is = "browse08.aspx"
                        groupName = HttpContext.Current.Request("group7")
                        currentPage = "browse07.aspx"
                        cacheKeyGroups = Talent.Common.Utilities.FixStringLength(group1, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group2, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group3, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group4, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group5, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group6, 20)
                        level = 7
                    Case Is = "browse09.aspx"
                        groupName = HttpContext.Current.Request("group8")
                        currentPage = "browse08.aspx"
                        cacheKeyGroups = Talent.Common.Utilities.FixStringLength(group1, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group2, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group3, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group4, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group5, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group6, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group7, 20)
                        level = 8
                    Case Is = "browse10.aspx"
                        groupName = HttpContext.Current.Request("group9")
                        currentPage = "browse09.aspx"
                        cacheKeyGroups = Talent.Common.Utilities.FixStringLength(group1, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group2, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group3, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group4, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group5, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group6, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group7, 20) & _
                                         Talent.Common.Utilities.FixStringLength(group8, 20)
                        level = 9
                End Select
            End While

            If groupName <> String.Empty Then

                '-----------------------
                ' Check if it's in cache
                '-----------------------
                cacheKey = "GroupLevelDetails" & Talent.Common.Utilities.FixStringLength(_businessUnit, 50) & _
                                                    Talent.Common.Utilities.FixStringLength(_partner, 50) & _
                                                    cacheKeyGroups & _
                                                    Talent.Common.Utilities.FixStringLength(groupName, 20) & _
                                                    Talent.Common.Utilities.FixStringLength(level, 2)

                If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                    dets = CType(HttpContext.Current.Cache.Item(cacheKey), GroupDetails)
                Else

                    Dim err As New Talent.Common.ErrorObj
                    Dim strSelect1 As String

                    Dim sb As New StringBuilder
                    With sb
                        .Append("SELECT * FROM ")
                        Select Case level
                            Case Is = 1
                                .Append("TBL_GROUP_LEVEL_01 WHERE ")
                                .Append(" GROUP_L01_BUSINESS_UNIT = @BUSINESS_UNIT ")
                                .Append(" AND GROUP_L01_PARTNER = @PARTNER ")
                                .Append(" AND GROUP_L01_L01_GROUP = @GROUP1")
                            Case Is = 2
                                .Append("TBL_GROUP_LEVEL_02 WHERE ")
                                .Append(" GROUP_L02_BUSINESS_UNIT = @BUSINESS_UNIT  ")
                                .Append(" AND GROUP_L02_PARTNER = @PARTNER ")
                                .Append(" AND GROUP_L02_L01_GROUP = @GROUP1")
                                .Append(" AND GROUP_L02_L02_GROUP = @GROUP2")
                            Case Is = 3
                                .Append("TBL_GROUP_LEVEL_03 WHERE ")
                                .Append(" GROUP_L03_BUSINESS_UNIT = @BUSINESS_UNIT ")
                                .Append(" AND GROUP_L03_PARTNER = @PARTNER ")
                                .Append(" AND GROUP_L03_L01_GROUP = @GROUP1")
                                .Append(" AND GROUP_L03_L02_GROUP = @GROUP2")
                                .Append(" AND GROUP_L03_L03_GROUP = @GROUP3")
                            Case Is = 4
                                .Append("TBL_GROUP_LEVEL_04 WHERE ")
                                .Append(" GROUP_L04_BUSINESS_UNIT = @BUSINESS_UNIT ")
                                .Append(" AND GROUP_L04_PARTNER = @PARTNER ")
                                .Append(" AND GROUP_L04_L01_GROUP = @GROUP1")
                                .Append(" AND GROUP_L04_L02_GROUP = @GROUP2")
                                .Append(" AND GROUP_L04_L03_GROUP = @GROUP3")
                                .Append(" AND GROUP_L04_L04_GROUP = @GROUP4")
                            Case Is = 5
                                .Append("TBL_GROUP_LEVEL_05 WHERE ")
                                .Append(" GROUP_L05_BUSINESS_UNIT = @BUSINESS_UNIT ")
                                .Append(" AND GROUP_L05_PARTNER = @PARTNER ")
                                .Append(" AND GROUP_L05_L01_GROUP = @GROUP1")
                                .Append(" AND GROUP_L05_L02_GROUP = @GROUP2")
                                .Append(" AND GROUP_L05_L03_GROUP = @GROUP3")
                                .Append(" AND GROUP_L05_L04_GROUP = @GROUP4")
                                .Append(" AND GROUP_L05_L05_GROUP = @GROUP5")
                            Case Is = 6
                                .Append("TBL_GROUP_LEVEL_06 WHERE ")
                                .Append(" GROUP_L06_BUSINESS_UNIT = @BUSINESS_UNIT ")
                                .Append(" AND GROUP_L06_PARTNER = @PARTNER ")
                                .Append(" AND GROUP_L06_L01_GROUP = @GROUP1")
                                .Append(" AND GROUP_L06_L02_GROUP = @GROUP2")
                                .Append(" AND GROUP_L06_L03_GROUP = @GROUP3")
                                .Append(" AND GROUP_L06_L04_GROUP = @GROUP4")
                                .Append(" AND GROUP_L06_L05_GROUP = @GROUP5")
                                .Append(" AND GROUP_L06_L06_GROUP = @GROUP6")
                            Case Is = 7
                                .Append("TBL_GROUP_LEVEL_07 WHERE ")
                                .Append(" GROUP_L07_BUSINESS_UNIT = @BUSINESS_UNIT ")
                                .Append(" AND GROUP_L07_PARTNER = @PARTNER ")
                                .Append(" AND GROUP_L07_L01_GROUP = @GROUP1")
                                .Append(" AND GROUP_L07_L02_GROUP = @GROUP2")
                                .Append(" AND GROUP_L07_L03_GROUP = @GROUP3")
                                .Append(" AND GROUP_L07_L04_GROUP = @GROUP4")
                                .Append(" AND GROUP_L07_L05_GROUP = @GROUP5")
                                .Append(" AND GROUP_L07_L06_GROUP = @GROUP6")
                                .Append(" AND GROUP_L07_L07_GROUP = @GROUP7")
                            Case Is = 8
                                .Append("TBL_GROUP_LEVEL_08 WHERE  ")
                                .Append(" GROUP_L08_BUSINESS_UNIT = @BUSINESS_UNIT  ")
                                .Append(" AND GROUP_L08_PARTNER = @PARTNER ")
                                .Append(" AND GROUP_L08_L01_GROUP = @GROUP1")
                                .Append(" AND GROUP_L08_L02_GROUP = @GROUP2")
                                .Append(" AND GROUP_L08_L03_GROUP = @GROUP3")
                                .Append(" AND GROUP_L08_L04_GROUP = @GROUP4")
                                .Append(" AND GROUP_L08_L05_GROUP = @GROUP5")
                                .Append(" AND GROUP_L08_L06_GROUP = @GROUP6")
                                .Append(" AND GROUP_L08_L07_GROUP = @GROUP7")
                                .Append(" AND GROUP_L08_L08_GROUP = @GROUP8")
                            Case Is = 9
                                .Append("TBL_GROUP_LEVEL_09 WHERE ")
                                .Append(" GROUP_L09_BUSINESS_UNIT = @BUSINESS_UNIT ")
                                .Append(" AND GROUP_L09_PARTNER = @PARTNER ")
                                .Append(" AND GROUP_L09_L01_GROUP = @GROUP1")
                                .Append(" AND GROUP_L09_L02_GROUP = @GROUP2")
                                .Append(" AND GROUP_L09_L03_GROUP = @GROUP3")
                                .Append(" AND GROUP_L09_L04_GROUP = @GROUP4")
                                .Append(" AND GROUP_L09_L05_GROUP = @GROUP5")
                                .Append(" AND GROUP_L09_L06_GROUP = @GROUP6")
                                .Append(" AND GROUP_L09_L07_GROUP = @GROUP7")
                                .Append(" AND GROUP_L09_L08_GROUP = @GROUP8")
                                .Append(" AND GROUP_L09_L09_GROUP = @GROUP9")
                            Case Is = 10
                                .Append("TBL_GROUP_LEVEL_10 WHERE GROUP_L10_L10_GROUP = @GROUP ")
                                .Append(" AND GROUP_L010_BUSINESS_UNIT = @BUSINESS_UNIT ")
                                .Append(" AND GROUP_L10_PARTNER = @PARTNER ")
                                .Append(" AND GROUP_L10_L01_GROUP = @GROUP1")
                                .Append(" AND GROUP_L10_L02_GROUP = @GROUP2")
                                .Append(" AND GROUP_L10_L03_GROUP = @GROUP3")
                                .Append(" AND GROUP_L10_L04_GROUP = @GROUP4")
                                .Append(" AND GROUP_L10_L05_GROUP = @GROUP5")
                                .Append(" AND GROUP_L10_L06_GROUP = @GROUP6")
                                .Append(" AND GROUP_L10_L07_GROUP = @GROUP7")
                                .Append(" AND GROUP_L10_L08_GROUP = @GROUP8")
                                .Append(" AND GROUP_L10_L09_GROUP = @GROUP9")
                                .Append(" AND GROUP_L10_L10_GROUP = @GROUP10")
                        End Select
                    End With

                    strSelect1 = sb.ToString

                    Dim conTalent As SqlConnection = Nothing
                    '--------------------------------------------------------------------
                    Try
                        Const SqlServer2005 As String = "TalentEBusinessDBConnectionString"
                        conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SqlServer2005).ConnectionString)
                        conTalent.Open()
                    Catch ex As Exception
                        Const strError1 As String = "Could not establish connection to the database"
                        With err
                            .ErrorMessage = ex.Message
                            .ErrorStatus = strError1
                            .ErrorNumber = "GLOBAL-01"
                            .HasError = True
                        End With
                    End Try
                    '---------------------------------------------------------------------
                    If Not err.HasError Then
                        Try
                            '------------
                            ' Get details
                            '------------ 
                            Dim cmdSelect1 As System.Data.SqlClient.SqlCommand = New System.Data.SqlClient.SqlCommand(strSelect1, conTalent)
                            With cmdSelect1.Parameters
                                ' cmdSelect1.Parameters.Add(New SqlParameter("@GROUP", SqlDbType.NVarChar, 20)).Value = groupName
                                .Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar, 50)).Value = _businessUnit
                                .Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar, 50)).Value = _partner

                                AddGroupParms(strSelect1, cmdSelect1, group1, group2, group3, group4, group5, group6, group7, group8, group9, group10)

                            End With

                            Dim dtrDets As System.Data.SqlClient.SqlDataReader = cmdSelect1.ExecuteReader()

                            If dtrDets.HasRows Then
                                dtrDets.Read()
                                dets = SetDets(dtrDets, level)

                                dtrDets.Close()
                            Else
                                dtrDets.Close()
                                '---------------------
                                ' Try for all partners
                                '---------------------
                                cmdSelect1 = New System.Data.SqlClient.SqlCommand(strSelect1, conTalent)
                                ' cmdSelect1.Parameters.Add(New SqlParameter("@GROUP", SqlDbType.NVarChar, 20)).Value = groupName
                                cmdSelect1.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar, 50)).Value = _businessUnit
                                cmdSelect1.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar, 50)).Value = Talent.Common.Utilities.GetAllString

                                AddGroupParms(strSelect1, cmdSelect1, group1, group2, group3, group4, group5, group6, group7, group8, group9, group10)

                                dtrDets = cmdSelect1.ExecuteReader()
                                If dtrDets.HasRows Then
                                    dtrDets.Read()
                                    dets = SetDets(dtrDets, level)
                                Else
                                    dtrDets.Close()
                                    '---------------------
                                    ' Try for all business units
                                    '---------------------
                                    cmdSelect1 = New System.Data.SqlClient.SqlCommand(strSelect1, conTalent)

                                    cmdSelect1.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar, 50)).Value = Talent.Common.Utilities.GetAllString
                                    cmdSelect1.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar, 50)).Value = _partner

                                    AddGroupParms(strSelect1, cmdSelect1, group1, group2, group3, group4, group5, group6, group7, group8, group9, group10)

                                    dtrDets = cmdSelect1.ExecuteReader()
                                    If dtrDets.HasRows Then
                                        dtrDets.Read()
                                        dets = SetDets(dtrDets, level)
                                    Else
                                        dtrDets.Close()
                                        '---------------------
                                        ' Try for all business units and partners
                                        '---------------------
                                        cmdSelect1 = New System.Data.SqlClient.SqlCommand(strSelect1, conTalent)

                                        cmdSelect1.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar, 50)).Value = Talent.Common.Utilities.GetAllString
                                        cmdSelect1.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar, 50)).Value = Talent.Common.Utilities.GetAllString

                                        AddGroupParms(strSelect1, cmdSelect1, group1, group2, group3, group4, group5, group6, group7, group8, group9, group10)

                                        dtrDets = cmdSelect1.ExecuteReader()

                                        If dtrDets.HasRows Then
                                            dtrDets.Read()
                                            dets = SetDets(dtrDets, level)
                                        End If
                                    End If
                                End If
                                dtrDets.Close()
                            End If

                            'If any of the details are not filled, fill them from tbl_group
                            If dets.GroupDescription1.Equals(String.Empty) Or _
                                dets.GroupDescription2.Equals(String.Empty) Or _
                                dets.GroupHtml1.Equals(String.Empty) Or _
                                dets.GroupHtml2.Equals(String.Empty) Or _
                                dets.GroupHtml3.Equals(String.Empty) Or _
                                dets.GroupMetaDescription.Equals(String.Empty) Or _
                                dets.GroupMetaKeywords.Equals(String.Empty) Or _
                                dets.GroupPageTitle.Equals(String.Empty) Then

                                strSelect1 = "SELECT * FROM tbl_group WHERE group_name = @group_name"
                                cmdSelect1 = New System.Data.SqlClient.SqlCommand(strSelect1, conTalent)

                                With cmdSelect1.Parameters
                                    .Add(New SqlParameter("@group_name", SqlDbType.NVarChar, 20)).Value = groupName
                                End With

                                Dim dtrGroup As System.Data.SqlClient.SqlDataReader = cmdSelect1.ExecuteReader()
                                If dtrGroup.HasRows Then
                                    dtrGroup.Read()
                                    If dets.GroupDescription1.Equals(String.Empty) Then
                                        dets.GroupDescription1 = dtrGroup("GROUP_DESCRIPTION_1")
                                    End If
                                    If dets.GroupDescription2.Equals(String.Empty) Then
                                        dets.GroupDescription2 = dtrGroup("GROUP_DESCRIPTION_2")
                                    End If
                                    If dets.GroupHtml1.Equals(String.Empty) Then
                                        dets.GroupHtml1 = dtrGroup("GROUP_HTML_1")
                                    End If
                                    If dets.GroupHtml2.Equals(String.Empty) Then
                                        dets.GroupHtml2 = dtrGroup("GROUP_HTML_2")
                                    End If
                                    If dets.GroupHtml3.Equals(String.Empty) Then
                                        dets.GroupHtml3 = dtrGroup("GROUP_HTML_3")
                                    End If
                                    If dets.GroupPageTitle.Equals(String.Empty) Then
                                        dets.GroupPageTitle = dtrGroup("GROUP_PAGE_TITLE")
                                    End If
                                    If dets.GroupMetaDescription.Equals(String.Empty) Then
                                        dets.GroupMetaDescription = dtrGroup("GROUP_META_DESCRIPTION")
                                    End If
                                    If dets.GroupMetaKeywords.Equals(String.Empty) Then
                                        dets.GroupMetaKeywords = dtrGroup("GROUP_META_KEYWORDS")
                                    End If
                                    dtrGroup.Close()
                                End If

                            End If

                        Catch ex As Exception
                            Const strError8 As String = "Error during database access"
                            With err
                                .ErrorMessage = ex.Message
                                .ErrorStatus = strError8
                                .ErrorNumber = "GLOBAL-06"
                                .HasError = True
                            End With
                        End Try
                    End If
                    '------
                    ' Close
                    '------
                    Try
                        conTalent.Close()
                    Catch ex As Exception
                        Const strError9 As String = "Failed to close database connection"
                        With err
                            .ErrorMessage = ex.Message
                            .ErrorStatus = strError9
                            .ErrorNumber = "GLOBAL-07"
                            .HasError = True
                        End With
                    End Try
                    '-------------
                    ' Add to cache
                    '-------------
                    HttpContext.Current.Cache.Insert(cacheKey, _
                                                     dets, _
                                                     Nothing, _
                                                     System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), _
                                                     Caching.Cache.NoSlidingExpiration)
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)

                End If

            End If

            Return dets
        End Function

        Private Sub AddGroupParms(ByVal strSelect As String, _
                                    ByRef cmdSelect As System.Data.SqlClient.SqlCommand, _
                                    ByVal group1 As String, _
                                    ByVal group2 As String, _
                                    ByVal group3 As String, _
                                    ByVal group4 As String, _
                                    ByVal group5 As String, _
                                    ByVal group6 As String, _
                                    ByVal group7 As String, _
                                    ByVal group8 As String, _
                                    ByVal group9 As String, _
                                    ByVal group10 As String)
            With cmdSelect.Parameters
                If strSelect.Contains("@GROUP1") Then
                    .Add(New SqlParameter("@GROUP1", SqlDbType.NVarChar, 20)).Value = group1
                End If
                If strSelect.Contains("@GROUP2") Then
                    .Add(New SqlParameter("@GROUP2", SqlDbType.NVarChar, 20)).Value = group2
                End If
                If strSelect.Contains("@GROUP3") Then
                    .Add(New SqlParameter("@GROUP3", SqlDbType.NVarChar, 20)).Value = group3
                End If
                If strSelect.Contains("@GROUP4") Then
                    .Add(New SqlParameter("@GROUP4", SqlDbType.NVarChar, 20)).Value = group4
                End If
                If strSelect.Contains("@GROUP5") Then
                    .Add(New SqlParameter("@GROUP5", SqlDbType.NVarChar, 20)).Value = group5
                End If
                If strSelect.Contains("@GROUP6") Then
                    .Add(New SqlParameter("@GROUP6", SqlDbType.NVarChar, 20)).Value = group6
                End If
                If strSelect.Contains("@GROUP7") Then
                    .Add(New SqlParameter("@GROUP7", SqlDbType.NVarChar, 20)).Value = group7
                End If
                If strSelect.Contains("@GROUP8") Then
                    .Add(New SqlParameter("@GROUP8", SqlDbType.NVarChar, 20)).Value = group8
                End If
                If strSelect.Contains("@GROUP9") Then
                    .Add(New SqlParameter("@GROUP9", SqlDbType.NVarChar, 20)).Value = group9
                End If
                If strSelect.Contains("@GROUP10") Then
                    .Add(New SqlParameter("@GROUP10", SqlDbType.NVarChar, 20)).Value = group10
                End If
            End With

        End Sub
        '---------------------------------------
        ' SetDets - set details from Data Reader
        '---------------------------------------
        Private Function SetDets(ByVal dtrDets As SqlDataReader, ByVal level As Integer) As GroupDetails
            Dim dets As New GroupDetails
            Select Case level
                Case Is = 1
                    dets.GroupDescription1 = dtrDets("GROUP_L01_DESCRIPTION_1")
                    dets.GroupDescription2 = dtrDets("GROUP_L01_DESCRIPTION_2")
                    dets.GroupHtml1 = dtrDets("GROUP_L01_HTML_1")
                    dets.GroupHtml2 = dtrDets("GROUP_L01_HTML_2")
                    dets.GroupHtml3 = dtrDets("GROUP_L01_HTML_3")
                    dets.GroupMetaDescription = dtrDets("GROUP_L01_META_DESCRIPTION")
                    dets.GroupMetaKeywords = dtrDets("GROUP_L01_META_KEYWORDS")
                    dets.GroupPageTitle = dtrDets("GROUP_L01_PAGE_TITLE")
                    dets.GroupPartner = dtrDets("GROUP_L01_PARTNER")
                    dets.GroupTheme = Utilities.CheckForDBNull_String(dtrDets("GROUP_L01_THEME"))
                    dets.ShowProductDisplay = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrDets("GROUP_L01_SHOW_PRODUCT_DISPLAY"))
                    dets.ShowChildrenAsGroups = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrDets("GROUP_L01_SHOW_CHILDREN_AS_GROUPS"))
                Case Is = 2
                    dets.GroupDescription1 = dtrDets("GROUP_L02_DESCRIPTION_1")
                    dets.GroupDescription2 = dtrDets("GROUP_L02_DESCRIPTION_2")
                    dets.GroupHtml1 = dtrDets("GROUP_L02_HTML_1")
                    dets.GroupHtml2 = dtrDets("GROUP_L02_HTML_2")
                    dets.GroupHtml3 = dtrDets("GROUP_L02_HTML_3")
                    dets.GroupMetaDescription = dtrDets("GROUP_L02_META_DESCRIPTION")
                    dets.GroupMetaKeywords = dtrDets("GROUP_L02_META_KEYWORDS")
                    dets.GroupPageTitle = dtrDets("GROUP_L02_PAGE_TITLE")
                    dets.GroupPartner = dtrDets("GROUP_L02_PARTNER")
                    dets.GroupTheme = Utilities.CheckForDBNull_String(dtrDets("GROUP_L02_THEME"))
                    dets.ShowProductDisplay = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrDets("GROUP_L02_SHOW_PRODUCT_DISPLAY"))
                    dets.ShowChildrenAsGroups = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrDets("GROUP_L02_SHOW_CHILDREN_AS_GROUPS"))
                Case Is = 3
                    dets.GroupDescription1 = dtrDets("GROUP_L03_DESCRIPTION_1")
                    dets.GroupDescription2 = dtrDets("GROUP_L03_DESCRIPTION_2")
                    dets.GroupHtml1 = dtrDets("GROUP_L03_HTML_1")
                    dets.GroupHtml2 = dtrDets("GROUP_L03_HTML_2")
                    dets.GroupHtml3 = dtrDets("GROUP_L03_HTML_3")
                    dets.GroupMetaDescription = dtrDets("GROUP_L03_META_DESCRIPTION")
                    dets.GroupMetaKeywords = dtrDets("GROUP_L03_META_KEYWORDS")
                    dets.GroupPageTitle = dtrDets("GROUP_L03_PAGE_TITLE")
                    dets.GroupPartner = dtrDets("GROUP_L03_PARTNER")
                    dets.GroupTheme = Utilities.CheckForDBNull_String(dtrDets("GROUP_L03_THEME"))
                    dets.ShowProductDisplay = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrDets("GROUP_L03_SHOW_PRODUCT_DISPLAY"))
                    dets.ShowChildrenAsGroups = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrDets("GROUP_L03_SHOW_CHILDREN_AS_GROUPS"))
                Case Is = 4
                    dets.GroupDescription1 = dtrDets("GROUP_L04_DESCRIPTION_1")
                    dets.GroupDescription2 = dtrDets("GROUP_L04_DESCRIPTION_2")
                    dets.GroupHtml1 = dtrDets("GROUP_L04_HTML_1")
                    dets.GroupHtml2 = dtrDets("GROUP_L04_HTML_2")
                    dets.GroupHtml3 = dtrDets("GROUP_L04_HTML_3")
                    dets.GroupMetaDescription = dtrDets("GROUP_L04_META_DESCRIPTION")
                    dets.GroupMetaKeywords = dtrDets("GROUP_L04_META_KEYWORDS")
                    dets.GroupPageTitle = dtrDets("GROUP_L04_PAGE_TITLE")
                    dets.GroupPartner = dtrDets("GROUP_L04_PARTNER")
                    dets.GroupTheme = Utilities.CheckForDBNull_String(dtrDets("GROUP_L04_THEME"))
                    dets.ShowProductDisplay = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrDets("GROUP_L04_SHOW_PRODUCT_DISPLAY"))
                    dets.ShowChildrenAsGroups = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrDets("GROUP_L04_SHOW_CHILDREN_AS_GROUPS"))
                Case Is = 5
                    dets.GroupDescription1 = dtrDets("GROUP_L05_DESCRIPTION_1")
                    dets.GroupDescription2 = dtrDets("GROUP_L05_DESCRIPTION_2")
                    dets.GroupHtml1 = dtrDets("GROUP_L05_HTML_1")
                    dets.GroupHtml2 = dtrDets("GROUP_L05_HTML_2")
                    dets.GroupHtml3 = dtrDets("GROUP_L05_HTML_3")
                    dets.GroupMetaDescription = dtrDets("GROUP_L05_META_DESCRIPTION")
                    dets.GroupMetaKeywords = dtrDets("GROUP_L05_META_KEYWORDS")
                    dets.GroupPageTitle = dtrDets("GROUP_L05_PAGE_TITLE")
                    dets.GroupPartner = dtrDets("GROUP_L05_PARTNER")
                    dets.GroupTheme = Utilities.CheckForDBNull_String(dtrDets("GROUP_L05_THEME"))
                    dets.ShowProductDisplay = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrDets("GROUP_L05_SHOW_PRODUCT_DISPLAY"))
                    dets.ShowChildrenAsGroups = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrDets("GROUP_L05_SHOW_CHILDREN_AS_GROUPS"))
                Case Is = 6
                    dets.GroupDescription1 = dtrDets("GROUP_L06_DESCRIPTION_1")
                    dets.GroupDescription2 = dtrDets("GROUP_L06_DESCRIPTION_2")
                    dets.GroupHtml1 = dtrDets("GROUP_L06_HTML_1")
                    dets.GroupHtml2 = dtrDets("GROUP_L06_HTML_2")
                    dets.GroupHtml3 = dtrDets("GROUP_L06_HTML_3")
                    dets.GroupMetaDescription = dtrDets("GROUP_L06_META_DESCRIPTION")
                    dets.GroupMetaKeywords = dtrDets("GROUP_L06_META_KEYWORDS")
                    dets.GroupPageTitle = dtrDets("GROUP_L06_PAGE_TITLE")
                    dets.GroupPartner = dtrDets("GROUP_L06_PARTNER")
                    dets.GroupTheme = Utilities.CheckForDBNull_String(dtrDets("GROUP_L06_THEME"))
                    dets.ShowProductDisplay = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrDets("GROUP_L06_SHOW_PRODUCT_DISPLAY"))
                    dets.ShowChildrenAsGroups = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrDets("GROUP_L06_SHOW_CHILDREN_AS_GROUPS"))
                Case Is = 7
                    dets.GroupDescription1 = dtrDets("GROUP_L07_DESCRIPTION_1")
                    dets.GroupDescription2 = dtrDets("GROUP_L07_DESCRIPTION_2")
                    dets.GroupHtml1 = dtrDets("GROUP_L07_HTML_1")
                    dets.GroupHtml2 = dtrDets("GROUP_L07_HTML_2")
                    dets.GroupHtml3 = dtrDets("GROUP_L07_HTML_3")
                    dets.GroupMetaDescription = dtrDets("GROUP_L07_META_DESCRIPTION")
                    dets.GroupMetaKeywords = dtrDets("GROUP_L07_META_KEYWORDS")
                    dets.GroupPageTitle = dtrDets("GROUP_L07_PAGE_TITLE")
                    dets.GroupPartner = dtrDets("GROUP_L07_PARTNER")
                    dets.GroupTheme = Utilities.CheckForDBNull_String(dtrDets("GROUP_L07_THEME"))
                    dets.ShowProductDisplay = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrDets("GROUP_L07_SHOW_PRODUCT_DISPLAY"))
                    dets.ShowChildrenAsGroups = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrDets("GROUP_L07_SHOW_CHILDREN_AS_GROUPS"))
                Case Is = 8
                    dets.GroupDescription1 = dtrDets("GROUP_L08_DESCRIPTION_1")
                    dets.GroupDescription2 = dtrDets("GROUP_L08_DESCRIPTION_2")
                    dets.GroupHtml1 = dtrDets("GROUP_L08_HTML_1")
                    dets.GroupHtml2 = dtrDets("GROUP_L08_HTML_2")
                    dets.GroupHtml3 = dtrDets("GROUP_L08_HTML_3")
                    dets.GroupMetaDescription = dtrDets("GROUP_L08_META_DESCRIPTION")
                    dets.GroupMetaKeywords = dtrDets("GROUP_L08_META_KEYWORDS")
                    dets.GroupPageTitle = dtrDets("GROUP_L08_PAGE_TITLE")
                    dets.GroupPartner = dtrDets("GROUP_L08_PARTNER")
                    dets.GroupTheme = Utilities.CheckForDBNull_String(dtrDets("GROUP_L08_THEME"))
                    dets.ShowProductDisplay = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrDets("GROUP_L08_SHOW_PRODUCT_DISPLAY"))
                    dets.ShowChildrenAsGroups = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrDets("GROUP_L08_SHOW_CHILDREN_AS_GROUPS"))
                Case Is = 9
                    dets.GroupDescription1 = dtrDets("GROUP_L09_DESCRIPTION_1")
                    dets.GroupDescription2 = dtrDets("GROUP_L09_DESCRIPTION_2")
                    dets.GroupHtml1 = dtrDets("GROUP_L09_HTML_1")
                    dets.GroupHtml2 = dtrDets("GROUP_L09_HTML_2")
                    dets.GroupHtml3 = dtrDets("GROUP_L09_HTML_3")
                    dets.GroupMetaDescription = dtrDets("GROUP_L09_META_DESCRIPTION")
                    dets.GroupMetaKeywords = dtrDets("GROUP_L09_META_KEYWORDS")
                    dets.GroupPageTitle = dtrDets("GROUP_L09_PAGE_TITLE")
                    dets.GroupPartner = dtrDets("GROUP_L09_PARTNER")
                    dets.GroupTheme = Utilities.CheckForDBNull_String(dtrDets("GROUP_L09_THEME"))
                    dets.ShowProductDisplay = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrDets("GROUP_L09_SHOW_PRODUCT_DISPLAY"))
                    dets.ShowChildrenAsGroups = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrDets("GROUP_L09_SHOW_CHILDREN_AS_GROUPS"))
                Case Is = 10
                    dets.GroupDescription1 = dtrDets("GROUP_L10_DESCRIPTION_1")
                    dets.GroupDescription2 = dtrDets("GROUP_L10_DESCRIPTION_2")
                    dets.GroupHtml1 = dtrDets("GROUP_L10_HTML_1")
                    dets.GroupHtml2 = dtrDets("GROUP_L10_HTML_2")
                    dets.GroupHtml3 = dtrDets("GROUP_L10_HTML_3")
                    dets.GroupMetaDescription = dtrDets("GROUP_L10_META_DESCRIPTION")
                    dets.GroupMetaKeywords = dtrDets("GROUP_L10_META_KEYWORDS")
                    dets.GroupPageTitle = dtrDets("GROUP_L10_PAGE_TITLE")
                    dets.GroupPartner = dtrDets("GROUP_L10_PARTNER")
                    dets.GroupTheme = Utilities.CheckForDBNull_String(dtrDets("GROUP_L10_THEME"))
                    dets.ShowProductDisplay = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrDets("GROUP_L10_SHOW_PRODUCT_DISPLAY"))
                    dets.ShowChildrenAsGroups = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrDets("GROUP_L10_SHOW_CHILDREN_AS_GROUPS"))
            End Select
            Return dets
        End Function

    End Class
End Namespace

