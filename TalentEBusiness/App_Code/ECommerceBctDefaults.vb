Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Collections.Generic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    Bread crumb traol defaults
'
'       Date                        mar 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      APCBCT- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'-------------------------------------------------------------------------------------------------

Namespace Talent.eCommerce
    Public Class ECommerceBctDefaults
        '------------------------------------------------
        ' Nested class for returning and storing in cache
        '------------------------------------------------
        Public Class BctEntry

            Private _page As String
            Private _url As String
            Private _text As String
            Private _type As String
            Private _moreItems As Boolean = False

            Public Property Page() As String
                Get
                    Return _page
                End Get
                Set(ByVal value As String)
                    _page = value
                End Set
            End Property
            Public Property Url() As String
                Get
                    Return _url
                End Get
                Set(ByVal value As String)
                    _url = value
                End Set
            End Property
            Public Property Text() As String
                Get
                    Return _text
                End Get
                Set(ByVal value As String)
                    _text = value
                End Set
            End Property
            Public Property Type() As String
                Get
                    Return _type
                End Get
                Set(ByVal value As String)
                    _type = value
                End Set
            End Property
            Public Property MoreItems() As Boolean
                Get
                    Return _moreItems
                End Get
                Set(ByVal value As Boolean)
                    _moreItems = value
                End Set
            End Property

        End Class

        Dim dtBct As New DataTable
        Dim listOfBctEntries As New List(Of BctEntry)
        Dim businessUnit As String = TalentCache.GetBusinessUnit()
        Dim partner As String = TalentCache.GetPartner(HttpContext.Current.Profile)
        Dim pagename As String = Talent.eCommerce.Utilities.GetCurrentPageName().ToLower
        Dim pageDefaults As New TalentApplicationVariablesTableAdapters.tbl_pageTableAdapter 

        Public Function GetBctDefaults() As List(Of BctEntry)
            ' Declare this first! Used for Logging function duration
            Dim timeSpan As TimeSpan = Now.TimeOfDay

            Dim cachename As String = "ECommerceBctDefaults" & businessUnit & pagename

            If Not Talent.Common.TalentThreadSafe.ItemIsInCache(cachename)  Then

                Dim dt As New DataTable

                ' Find current page
                dt = pageDefaults.GetDataBy_BU_Partner_Page(businessUnit, partner, pagename)
                If Not dtBct.Rows.Count > 0 Then
                    dt = pageDefaults.GetDataBy_BU_Partner_Page(businessUnit, Talent.Common.Utilities.GetAllString, pagename)
                    ' Continue to use *ALL instead of partner for subsequent reads
                    partner = Talent.Common.Utilities.GetAllString
                End If

                If dt.Rows.Count > 0 Then
                    ' Transfer all rows found into dtBct
                    dtBct.Merge(dt)
                    ' Get parents
                    For Each row As Data.DataRow In dt.Rows
                        If Not row("BCT_PARENT").Equals(DBNull.Value) Then
                            GetParents(businessUnit, partner, row("BCT_PARENT"))
                        End If
                    Next
                End If

                ' We now have a data table containing all BCT entries - iterate through in reverse and
                ' create a BctEntry for each. Cache this.
                Dim lastStandardSet As Boolean = False

                Dim i As Integer
                For i = dtBct.Rows.Count To 1 Step -1

                    Dim be As New BctEntry
                    If Not dtBct.Rows.Item(i - 1)("PAGE_CODE").Equals(DBNull.Value) Then
                        be.Page = dtBct.Rows.Item(i - 1)("PAGE_CODE")
                    End If
                    If Not dtBct.Rows.Item(i - 1)("BCT_URL").Equals(DBNull.Value) Then
                        be.Url = dtBct.Rows.Item(i - 1)("BCT_URL")
                    End If
                    If Not dtBct.Rows.Item(i - 1)("PAGE_TYPE").Equals(DBNull.Value) Then
                        be.Type = dtBct.Rows.Item(i - 1)("PAGE_TYPE")
                        If (dtBct.Rows.Item(i - 1)("PAGE_TYPE").Equals("STANDARD")) Or (dtBct.Rows.Item(i - 1)("PAGE_TYPE").Equals("ALLQUERYSTRING")) Then
                            ' Get language specific text
                            Dim dtPageLang As New DataTable
                            Dim pageLangDefaults As New TalentApplicationVariablesTableAdapters.tbl_page_langTableAdapter 
                            dtPageLang = pageLangDefaults.GetDataBy_BU_Partner_Page_Lang(businessUnit, partner, be.Page, Talent.Common.Utilities.GetDefaultLanguage)
                            If dtPageLang.Rows.Count > 0 Then
                                For Each row2 As Data.DataRow In dtPageLang.Rows
                                    be.Text = row2("PAGE_HEADER")
                                Next
                            End If

                        End If
                    End If
                    ' Are there more items?
                    If i > 1 Then
                        be.MoreItems = True
                    End If
                    ' Add to the list for cacheing
                    listOfBctEntries.Add(be)
                Next

                HttpContext.Current.Cache.Insert(cachename, _
                                             listOfBctEntries, _
                                             Nothing, _
                                             System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), _
                                             Caching.Cache.NoSlidingExpiration)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cachename)


            Else
                listOfBctEntries = CType(HttpContext.Current.Cache.Item(cachename), List(Of BctEntry))
            End If

            Utilities.TalentLogging.LoadTestLog("ECommerceBctDefaults.vb", "GetBctDefaults", timeSpan)
            Return listOfBctEntries
        End Function

        Private Sub GetParents(ByVal businessUnit As String, ByVal partner As String, ByVal page As String)
            Dim dt As New DataTable
            dt = pageDefaults.GetDataBy_BU_Partner_Page(businessUnit, partner, page)
            If dt.Rows.Count > 0 Then
                ' Transfer all rows found into dtBct
                For Each row As DataRow In dt.Rows
                    dtBct.ImportRow(row)
                Next
                ' And re-iterate
                For Each row As Data.DataRow In dt.Rows
                    If Not row("BCT_PARENT").Equals(DBNull.Value) Then
                        GetParents(businessUnit, partner, row("BCT_PARENT"))
                    End If
                Next
            End If
        End Sub

    End Class

End Namespace

