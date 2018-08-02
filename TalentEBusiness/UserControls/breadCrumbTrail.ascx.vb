Imports System.data
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Bread Crumb Trail
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      UCBCT- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_breadCrumbTrail
    Inherits ControlBase

    Private ucr As New Talent.Common.UserControlResource
    'Private separator As String = String.Empty
    Private queryString As NameValueCollection = HttpContext.Current.Request.QueryString
    Private err As ErrorObj


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If ModuleDefaults.BctInUse Then
            Dim count As Integer = 0
            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .PageCode = Talent.Common.Utilities.GetAllString
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "breadCrumbTrail.ascx"
            End With

            'separator = ucr.Content("Separator", Talent.Common.Utilities.GetDefaultLanguage, True)
            Dim prefixText As String = ucr.Content("Prefixtext", Talent.Common.Utilities.GetDefaultLanguage, True)
            If prefixText <> String.Empty Then
                lblBct.Text = prefixText
            End If

            Try
                If Me.CheckoutBCTAction Then
                    ProcessCheckoutBCT()
                Else
                    Dim bctEntires As List(Of Talent.eCommerce.ECommerceBctDefaults.BctEntry) = CType(Me.Page, TalentBase01).BctEntries()
                    For Each entry In bctEntires
                        count += 1
                        If entry.MoreItems Then
                            If entry.Type.Equals("STANDARD") Then
                                If count > 1 Then
                                    'lblBct.Text &= separator
                                End If
                                lblBct.Text &= "<li><a href=""" & entry.Url & """>" & entry.Text & "</a></li>"
                            Else
                                BuildNonStandardBctEntries()
                                Exit For
                            End If
                        Else
                            If entry.Type.Equals("STANDARD") Then
                                If count > 1 Then
                                    'lblBct.Text &= separator
                                End If
                                lblBct.Text &= "<li class=""current"">" & entry.Text & "</li>"
                            Else
                                BuildNonStandardBctEntries()
                                Exit For
                            End If
                        End If
                    Next
                End If

            Catch ex As Exception
            End Try
        Else
            Me.Visible = False
        End If
    End Sub

    Private Sub ProcessCheckoutBCT()
        Dim cBcts As New List(Of Talent.eCommerce.ECommerceBctDefaults.BctEntry)
        Dim myList As List(Of Talent.eCommerce.ECommerceBctDefaults.BctEntry) = Me.GetCheckoutBCTList
       
        If Session("CheckoutBreadCrumbTrail") Is Nothing Then
            Session.Add("CheckoutBreadCrumbTrail", cBcts)
        Else
            cBcts = Session("CheckoutBreadCrumbTrail")
        End If

        For Each myBe As Talent.eCommerce.ECommerceBctDefaults.BctEntry In myList
            If myBe.Page.ToUpper = Talent.eCommerce.Utilities.GetCurrentPageName.ToUpper Then

                'if the entry does not exist in the list already then add it 
                If Not cBcts.Contains(myBe) Then
                    cBcts.Add(myBe)
                Else
                    'if the entry does exist, then find it and remove all entries after that point
                    Dim i As Integer = 0
                    For Each cBct As Talent.eCommerce.ECommerceBctDefaults.BctEntry In cBcts
                        If cBct.Page.ToUpper = myBe.Page.ToUpper Then
                            Exit For
                        End If
                        i += 1
                    Next

                    If cBcts.Count - 1 > i Then
                        cBcts.RemoveRange(i + 1, (cBcts.Count) - (i + 1))
                    End If

                End If
                Exit For
            End If
        Next

        If Not cBcts Is Nothing Then
            For i As Integer = 0 To cBcts.Count - 1
                Dim myBe As Talent.eCommerce.ECommerceBctDefaults.BctEntry = cBcts(i)
                If i > 0 Then
                    'lblBct.Text &= separator
                End If
                If i = cBcts.Count - 1 Then
                    lblBct.Text &= myBe.Text
                Else
                    lblBct.Text &= "<li><a href=""" & myBe.Url & """>" & myBe.Text & "</a></li>"
                End If
            Next
        End If

    End Sub

    Private Function CheckoutBCTAction() As Boolean

        Dim returnVal As Boolean = False
        Dim myList As List(Of Talent.eCommerce.ECommerceBctDefaults.BctEntry) = GetCheckoutBCTList()
        If Not myList Is Nothing AndAlso myList.Count > 0 Then
            For Each myBe As Talent.eCommerce.ECommerceBctDefaults.BctEntry In myList
                If myBe.Page.ToUpper = Talent.eCommerce.Utilities.GetCurrentPageName.ToUpper Then
                    returnVal = True
                    Exit For
                End If
            Next
        End If

        Return returnVal

    End Function

    Protected Function GetCheckoutBCTList() As List(Of Talent.eCommerce.ECommerceBctDefaults.BctEntry)
        Dim checkoutCacheKey As String = "CheckoutPages_PageDetails_CachedTableRows"
        Dim dt As DataTable
        Dim drs As DataRow()
        Dim myList As New List(Of Talent.eCommerce.ECommerceBctDefaults.BctEntry)

        Dim bu As String = TalentCache.GetBusinessUnit
        Dim part As String = TalentCache.GetPartner(Profile)

        If Talent.Common.TalentThreadSafe.ItemIsInCache(checkoutCacheKey) Then
            myList = HttpContext.Current.Cache.Item(checkoutCacheKey)
        Else
            Dim pageDefaults As New TalentApplicationVariablesTableAdapters.tbl_pageTableAdapter
            dt = pageDefaults.GetDataBy_BU_Partner(bu, part)

            If dt.Rows.Count = 0 Then
                part = Utilities.GetAllString
                dt = pageDefaults.GetDataBy_BU_Partner(bu, part)
            End If

            If dt.Rows.Count > 0 Then
                drs = dt.Select("PAGE_TYPE = 'CHECKOUT'")
                Dim dtPageLang As New DataTable
                Dim pageLangDefaults As New TalentApplicationVariablesTableAdapters.tbl_page_langTableAdapter

                For i As Integer = 0 To drs.Length - 1
                    Dim dr As DataRow = drs(i)
                    Dim myBe As New Talent.eCommerce.ECommerceBctDefaults.BctEntry
                    With myBe
                        .Page = Utilities.CheckForDBNull_String(dr("PAGE_CODE"))
                        .Url = Utilities.CheckForDBNull_String(dr("BCT_URL"))
                        .Type = Utilities.CheckForDBNull_String(dr("PAGE_TYPE"))
                    End With

                    dtPageLang = pageLangDefaults.GetDataBy_BU_Partner_Page_Lang(bu, _
                                                                                   part, _
                                                                                   Utilities.CheckForDBNull_String(dr("PAGE_CODE")), _
                                                                                   Talent.Common.Utilities.GetDefaultLanguage)
                    If dtPageLang.Rows.Count > 0 Then
                        myBe.Text = Utilities.CheckForDBNull_String(dtPageLang.Rows(0)("PAGE_HEADER"))
                    Else
                        myBe.Text = Utilities.CheckForDBNull_String(dr("DESCRIPTION"))
                    End If

                    If Not myList.Contains(myBe) Then myList.Add(myBe)
                Next

                'Cache the result
                TalentCache.AddPropertyToCache(checkoutCacheKey, myList, CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes")), TimeSpan.Zero, CacheItemPriority.Normal)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(checkoutCacheKey)
            End If
        End If

        Return myList
    End Function
    Private Function containsProductPage(ByVal bctEntries As List(Of Talent.eCommerce.ECommerceBctDefaults.BctEntry)) As Boolean
        For Each entry In bctEntries
            If entry.Type = "PRODUCT" Then
                Return True
            End If
        Next
        Return False
    End Function
    Private Sub BuildNonStandardBctEntries()

        Dim bctText As String = String.Empty
        Dim bctQueryString As String = String.Empty
        Dim count As Integer = 0
        Dim entries As List(Of Talent.eCommerce.ECommerceBctDefaults.BctEntry) = CType(Me.Page, TalentBase01).BctEntries()
        For Each entry In entries
            count += 1
            If Not entry.Type.Equals("STANDARD") Then
                If entry.MoreItems Then

                    Select Case entry.Type
                        Case "BROWSE"
                            bctText = GetBrowseDescription(queryString, entry.Page)
                            If Not bctText.Equals("*EMPTY") Then
                                If count > 1 Then
                                    'If Not String.IsNullOrEmpty(bctText.Trim) Then lblBct.Text &= separator
                                End If
                                bctQueryString = GetBrowseQueryString(queryString, entry.Page)
                            Else
                                bctText = String.Empty
                                bctQueryString = String.Empty
                            End If
                        Case "PRODUCT"
                            If count > 1 Then
                                'lblBct.Text &= separator
                            End If
                            bctText = GetProductDescription()
                            bctQueryString = GetProductQueryString(queryString)
                        Case "ALLQUERYSTRING"
                            If count > 1 Then
                                'lblBct.Text &= separator
                            End If
                            bctText = entry.Text.Trim
                            bctQueryString = "?" & queryString.ToString.Trim
                    End Select
                    If IsBCTClassCurrent(queryString, entry.Page) AndAlso Not containsProductPage(entries) Then
                        If Not String.IsNullOrEmpty(bctText) Then lblBct.Text &= "<li class=""current""><a href=""" & "../../PagesPublic/ProductBrowse/" & entry.Page & bctQueryString & """>" & bctText & "</a></li>"
                    Else
                        If Not String.IsNullOrEmpty(bctText) Then lblBct.Text &= "<li><a href=""" & "../../PagesPublic/ProductBrowse/" & entry.Page & bctQueryString & """>" & bctText & "</a></li>"
                    End If

                Else
                    Select Case entry.Type
                        Case "BROWSE"
                            bctText = GetBrowseDescription(queryString, entry.Page)
                            If Not bctText.Equals("*EMPTY") Then
                                If count > 1 Then
                                    'lblBct.Text &= separator
                                End If
                            Else
                                bctText = String.Empty
                            End If
                        Case "PRODUCT"
                            If count > 1 Then
                                'lblBct.Text &= separator
                            End If
                            bctText = GetProductDescription()
                        Case "ALLQUERYSTRING"
                            If count > 1 Then
                                'lblBct.Text &= separator
                            End If
                            bctText = entry.Text.Trim
                    End Select
                    If Not String.IsNullOrEmpty(bctText) Then lblBct.Text &= "<li class=""current"">" & bctText & "</li>"
                End If
            End If
        Next

    End Sub

    Private Function GetBrowseDescription(ByVal queryString As NameValueCollection, ByVal page As String) As String
        Dim browseDescription As String = String.Empty
        Dim talentSqlAccessDetail As New TalentDataAccess
        talentSqlAccessDetail.Settings = TDataObjects.Settings
        'talentSqlAccessDetail.Settings.Cacheing = False
        'talentSqlAccessDetail.Settings.CacheTimeMinutes = 90
        'talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
        'Handle *EMPTY
        If page.Equals("browse02.aspx") AndAlso Not queryString.Item("group1") Is Nothing AndAlso queryString.Item("group1").Equals("*EMPTY") _
        Or page.Equals("browse03.aspx") AndAlso Not queryString.Item("group2") Is Nothing AndAlso queryString.Item("group2").Equals("*EMPTY") _
        Or page.Equals("browse04.aspx") AndAlso Not queryString.Item("group3") Is Nothing AndAlso queryString.Item("group3").Equals("*EMPTY") _
        Or page.Equals("browse05.aspx") AndAlso Not queryString.Item("group4") Is Nothing AndAlso queryString.Item("group4").Equals("*EMPTY") _
        Or page.Equals("browse06.aspx") AndAlso Not queryString.Item("group5") Is Nothing AndAlso queryString.Item("group5").Equals("*EMPTY") _
        Or page.Equals("browse07.aspx") AndAlso Not queryString.Item("group6") Is Nothing AndAlso queryString.Item("group6").Equals("*EMPTY") _
        Or page.Equals("browse08.aspx") AndAlso Not queryString.Item("group7") Is Nothing AndAlso queryString.Item("group7").Equals("*EMPTY") _
        Or page.Equals("browse09.aspx") AndAlso Not queryString.Item("group8") Is Nothing AndAlso queryString.Item("group8").Equals("*EMPTY") _
        Or page.Equals("browse10.aspx") AndAlso Not queryString.Item("group9") Is Nothing AndAlso queryString.Item("group9").Equals("*EMPTY") Then
            browseDescription = "*EMPTY"
        Else
            'retrieve browse description from group level tables
            Dim groups As List(Of String) = New List(Of String)
            For value As Integer = 1 To 9
                Dim groupText As String = queryString.Item("group" & value)
                If Not groupText Is Nothing Then
                    If Not groupText.Equals("*EMPTY") AndAlso Not String.IsNullOrEmpty(groupText) Then
                        groups.Add(groupText)
                    End If
                End If
            Next
            Dim dtBctDetails As DataRow = TDataObjects.ProductsSettings.GetBCTDetails(groups, page, TalentCache.GetPartner(HttpContext.Current.Profile), TalentCache.GetBusinessUnit)

            If Not dtBctDetails Is Nothing Then
                Select Case page
                    Case "browse02.aspx"
                        browseDescription = dtBctDetails("GROUP_L01_DESCRIPTION_1")
                    Case "browse03.aspx"
                        browseDescription = dtBctDetails("GROUP_L02_DESCRIPTION_1")
                    Case "browse04.aspx"
                        browseDescription = dtBctDetails("GROUP_L03_DESCRIPTION_1")
                    Case "browse05.aspx"
                        browseDescription = dtBctDetails("GROUP_L04_DESCRIPTION_1")
                    Case "browse06.aspx"
                        browseDescription = dtBctDetails("GROUP_L05_DESCRIPTION_1")
                    Case "browse07.aspx"
                        browseDescription = dtBctDetails("GROUP_L06_DESCRIPTION_1")
                    Case "browse08.aspx"
                        browseDescription = dtBctDetails("GROUP_L07_DESCRIPTION_1")
                    Case "browse09.aspx"
                        browseDescription = dtBctDetails("GROUP_L08_DESCRIPTION_1")
                    Case "browse10.aspx"
                        browseDescription = dtBctDetails("GROUP_L09_DESCRIPTION_1")
                End Select
            End If

            'retrieve description from group table
            If browseDescription.Equals(String.Empty) Then
                Dim group As String = String.Empty
                Select Case page
                    Case "browse02.aspx"
                        group = queryString.Item("group1")
                    Case "browse03.aspx"
                        group = queryString.Item("group2")
                    Case "browse04.aspx"
                        group = queryString.Item("group3")
                    Case "browse05.aspx"
                        group = queryString.Item("group4")
                    Case "browse06.aspx"
                        group = queryString.Item("group5")
                    Case "browse07.aspx"
                        group = queryString.Item("group6")
                    Case "browse08.aspx"
                        group = queryString.Item("group7")
                    Case "browse09.aspx"
                        group = queryString.Item("group8")
                    Case "browse10.aspx"
                        group = queryString.Item("group9")
                End Select
                browseDescription = TDataObjects.ProductsSettings.GetBCTGroupDescription(group)
            End If

            End If



        Return browseDescription
    End Function
    Private Function IsBCTClassCurrent(ByVal queryString As NameValueCollection, ByVal page As String) As Boolean
        Dim pageNumber As Integer = CType(page.Substring(6, 2), Integer)
        pageNumber += 1
        Dim pageBrowse As String = "browse*.aspx"
        Dim pageNumberString As String = String.Empty
        If pageNumber < 10 Then
            pageNumberString = "0" & pageNumber
        Else
            pageNumberString = pageNumber
        End If
        Dim nextPage As String = pageBrowse.Replace("*", pageNumberString)
        If nextPage.Equals("browse02.aspx") AndAlso Not queryString.Item("group1") Is Nothing AndAlso queryString.Item("group1").Equals("*EMPTY") _
        Or nextPage.Equals("browse03.aspx") AndAlso Not queryString.Item("group2") Is Nothing AndAlso queryString.Item("group2").Equals("*EMPTY") _
        Or nextPage.Equals("browse04.aspx") AndAlso Not queryString.Item("group3") Is Nothing AndAlso queryString.Item("group3").Equals("*EMPTY") _
        Or nextPage.Equals("browse05.aspx") AndAlso Not queryString.Item("group4") Is Nothing AndAlso queryString.Item("group4").Equals("*EMPTY") _
        Or nextPage.Equals("browse06.aspx") AndAlso Not queryString.Item("group5") Is Nothing AndAlso queryString.Item("group5").Equals("*EMPTY") _
        Or nextPage.Equals("browse07.aspx") AndAlso Not queryString.Item("group6") Is Nothing AndAlso queryString.Item("group6").Equals("*EMPTY") _
        Or nextPage.Equals("browse08.aspx") AndAlso Not queryString.Item("group7") Is Nothing AndAlso queryString.Item("group7").Equals("*EMPTY") _
        Or nextPage.Equals("browse09.aspx") AndAlso Not queryString.Item("group8") Is Nothing AndAlso queryString.Item("group8").Equals("*EMPTY") _
        Or nextPage.Equals("browse10.aspx") AndAlso Not queryString.Item("group9") Is Nothing AndAlso queryString.Item("group9").Equals("*EMPTY") Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function GetProductDescription() As String
        Dim productDescription As String = String.Empty
        Dim groups As List(Of String) = New List(Of String)
        For value As Integer = 1 To 9
            Dim groupText As String = queryString.Item("group" & value)
            If Not groupText Is Nothing Then
                If Not groupText.Equals("*EMPTY") AndAlso Not String.IsNullOrEmpty(groupText) Then
                    groups.Add(groupText)
                Else
                    Exit For
                End If
            End If
        Next
        Dim drGroupProducts As DataRow = TDataObjects.ProductsSettings.GetGroupProducts(groups, queryString, TalentCache.GetPartner(HttpContext.Current.Profile), TalentCache.GetBusinessUnit)

        If Not drGroupProducts Is Nothing Then
            productDescription = drGroupProducts("PRODUCT_DESCRIPTION_1")
        End If
        Return productDescription
    End Function

    Private Function GetBrowseQueryString(ByVal queryString As NameValueCollection, ByVal page As String) As String
        Dim productQueryString As New StringBuilder
        Dim i As Integer = 1

        With productQueryString

            If page.Equals("browse02.aspx") Or page.Equals("browse03.aspx") Or page.Equals("browse04.aspx") _
                Or page.Equals("browse05.aspx") Or page.Equals("browse06.aspx") Or page.Equals("browse07.aspx") _
                Or page.Equals("browse08.aspx") Or page.Equals("browse09.aspx") Or page.Equals("browse10.aspx") Then
                If Not queryString.Item("group1") Is Nothing Then
                    .Append("?group1=")
                    .Append(queryString.Item("group1"))
                End If
            End If
            If page.Equals("browse03.aspx") Or page.Equals("browse04.aspx") _
                Or page.Equals("browse05.aspx") Or page.Equals("browse06.aspx") Or page.Equals("browse07.aspx") _
                Or page.Equals("browse08.aspx") Or page.Equals("browse09.aspx") Or page.Equals("browse10.aspx") Then
                If Not queryString.Item("group2") Is Nothing Then
                    .Append("&group2=")
                    .Append(queryString.Item("group2"))
                End If
            End If
            If page.Equals("browse04.aspx") _
                Or page.Equals("browse05.aspx") Or page.Equals("browse06.aspx") Or page.Equals("browse07.aspx") _
                Or page.Equals("browse08.aspx") Or page.Equals("browse09.aspx") Or page.Equals("browse10.aspx") Then
                If Not queryString.Item("group3") Is Nothing Then
                    .Append("&group3=")
                    .Append(queryString.Item("group3"))
                End If
            End If
            If page.Equals("browse05.aspx") Or page.Equals("browse06.aspx") Or page.Equals("browse07.aspx") _
                Or page.Equals("browse08.aspx") Or page.Equals("browse09.aspx") Or page.Equals("browse10.aspx") Then
                If Not queryString.Item("group4") Is Nothing Then
                    .Append("&group4=")
                    .Append(queryString.Item("group4"))
                End If
            End If
            If page.Equals("browse06.aspx") Or page.Equals("browse07.aspx") _
                Or page.Equals("browse08.aspx") Or page.Equals("browse09.aspx") Or page.Equals("browse10.aspx") Then
                If Not queryString.Item("group5") Is Nothing Then
                    .Append("&group5=")
                    .Append(queryString.Item("group5"))
                End If
            End If
            If page.Equals("browse07.aspx") _
                Or page.Equals("browse08.aspx") Or page.Equals("browse09.aspx") Or page.Equals("browse10.aspx") Then
                If Not queryString.Item("group6") Is Nothing Then
                    .Append("&group6=")
                    .Append(queryString.Item("group6"))
                End If
            End If
            If page.Equals("browse08.aspx") Or page.Equals("browse09.aspx") Or page.Equals("browse10.aspx") Then
                If Not queryString.Item("group7") Is Nothing Then
                    .Append("&group7=")
                    .Append(queryString.Item("group7"))
                End If
            End If
            If page.Equals("browse09.aspx") Or page.Equals("browse10.aspx") Then
                If Not queryString.Item("group8") Is Nothing Then
                    .Append("&group8=")
                    .Append(queryString.Item("group8"))
                End If
            End If
            If page.Equals("browse10.aspx") Then
                If Not queryString.Item("group9") Is Nothing Then
                    .Append("&group9=")
                    .Append(queryString.Item("group9"))
                End If
            End If

        End With

        Return productQueryString.ToString
    End Function

    Private Function GetProductQueryString(ByVal queryString As NameValueCollection) As String
        Dim productQueryString As New StringBuilder
        Dim i As Integer = 1

        With productQueryString
            For i = 1 To 10
                If Not queryString.Item("group" & i.ToString) Is Nothing Then
                    If i = 1 Then
                        .Append("?")
                    Else
                        .Append("&")
                    End If
                    .Append("group" & i.ToString & "=")
                    .Append(queryString.Item("group" & i.ToString))
                End If
            Next
            If Not queryString.Item("product") Is Nothing Then
                .Append("&product=")
                .Append(queryString.Item("product"))
            End If
        End With

        Return productQueryString.ToString
    End Function

End Class