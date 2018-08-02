Imports Microsoft.VisualBasic
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
Imports System.Xml
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web
Imports System.Web.HttpServerUtility


Namespace Talent.eCommerce

    Public Class Utilities

        Public Shared Function GetCurrentPageName(Optional sPath As String = "") As String
            If sPath = String.Empty Then
                sPath = System.Web.HttpContext.Current.Request.Url.AbsolutePath
            Else
                sPath = System.Web.HttpContext.Current.Request.FilePath
            End If
            Dim oInfo As System.IO.FileInfo = New System.IO.FileInfo(sPath)
            Dim sRet As String = oInfo.Name

            If Not sRet.EndsWith(".aspx") Then
                'This assumes the page name is being retreieved from a webmethod such as: "VisualSeatSelection.aspx/GetSeating"
                'We need the page name "VisualSeatSelection.aspx" and not the method name "GetSeating"
                sRet = oInfo.Directory.Name
            End If
            'saveRet should be saved just before this case statement. Otherwise will cause Load issues
            Dim saveRet As String = sRet
            Select Case sRet.ToUpper
                Case "USERDEFINED.ASPX"
                    sRet = sRet & "?"
                    Dim queryString As NameValueCollection = HttpContext.Current.Request.QueryString
                    Dim defaultValues As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults
                    Dim pageCodeQueryKeys() As String = defaultValues.UserDefinedPageQueryKeys.Split(";")
                    If pageCodeQueryKeys IsNot Nothing AndAlso pageCodeQueryKeys.Length > 0 Then
                        For arrIndex As Integer = 0 To pageCodeQueryKeys.Length - 1
                            For Each queryKey As String In queryString.AllKeys
                                If pageCodeQueryKeys(arrIndex).ToLower() = queryKey.ToLower() Then
                                    sRet = sRet & queryKey & "=" & queryString.Item(queryKey) & "&"
                                    Exit For
                                End If
                            Next
                        Next
                    End If
                    sRet = sRet.TrimEnd("&")
                Case "PRODUCTHOME.ASPX"
                    If Not (String.IsNullOrEmpty(HttpContext.Current.Request.QueryString("ProductSubType"))) Then
                        sRet = sRet & "?" & "ProductSubType=" & HttpContext.Current.Request.QueryString("ProductSubType").ToUpper()
                    End If
                Case "PRODUCTSEASON.ASPX"
                    If Not (String.IsNullOrEmpty(HttpContext.Current.Request.QueryString("ProductSubType"))) Then
                        sRet = sRet & "?" & "ProductSubType=" & HttpContext.Current.Request.QueryString("ProductSubType").ToUpper()
                    End If
                Case "PRODUCTAWAY.ASPX"
                    If Not (String.IsNullOrEmpty(HttpContext.Current.Request.QueryString("ProductSubType"))) Then
                        sRet = sRet & "?" & "ProductSubType=" & HttpContext.Current.Request.QueryString("ProductSubType").ToUpper()
                    End If
                Case "PRODUCTEVENT.ASPX", "PRODUCTTRAVEL.ASPX"
                    If Not (String.IsNullOrEmpty(HttpContext.Current.Request.QueryString("ProductSubType"))) Then
                        sRet = sRet & "?" & "ProductSubType=" & HttpContext.Current.Request.QueryString("ProductSubType").ToUpper()
                    Else
                        sRet = sRet
                    End If
                Case "PRODUCTMEMBERSHIP.ASPX"
                    If Not (String.IsNullOrEmpty(HttpContext.Current.Request.QueryString("ProductSubType"))) Then
                        sRet = sRet & "?" & "ProductSubType=" & HttpContext.Current.Request.QueryString("ProductSubType").ToUpper()
                    End If
            End Select

            'Validate that a page exists if we have added a query string for product groups
            If saveRet <> sRet AndAlso sRet.ToUpper <> "USERDEFINED.ASPX" Then
                Dim pageCacheKey As String = "ecommerce-utilties-GetCurrentPageName" & sRet.ToUpper & TalentCache.GetBusinessUnit()
                If HttpContext.Current.Cache.Item(pageCacheKey) IsNot Nothing Then
                    sRet = HttpContext.Current.Cache.Item(pageCacheKey)
                Else
                    Dim tDataObjects As New Talent.Common.TalentDataObjects()
                    tDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
                    Dim dt As DataTable = tDataObjects.PageSettings.TblPage.GetByPageCode(sRet, TalentCache.GetBusinessUnit())
                    If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                        sRet = saveRet
                    End If
                    'Insert into Cache
                    HttpContext.Current.Cache.Insert(pageCacheKey,
                        sRet,
                        Nothing,
                        System.DateTime.Now.AddMinutes(30),
                        Caching.Cache.NoSlidingExpiration)
                End If
            End If

            Return sRet
        End Function

        Public Shared Sub CheckForEmptyGroup()
            '-----------------------------------------------------------------------
            '   CheckForEmptyGroup - Check if current level only contains empty groups
            '   If so redirect to product list level
            '-----------------------------------------------------------------------
            Dim businessUnit As String = TalentCache.GetBusinessUnit()
            Dim partner As String = TalentCache.GetPartner(HttpContext.Current.Profile)
            Dim currentPage As String = GetCurrentPageName()
            Dim grp1 As String = HttpContext.Current.Request("group1")
            Dim grp2 As String = HttpContext.Current.Request("group2")
            Dim grp3 As String = HttpContext.Current.Request("group3")
            Dim grp4 As String = HttpContext.Current.Request("group4")
            Dim grp5 As String = HttpContext.Current.Request("group5")
            Dim grp6 As String = HttpContext.Current.Request("group6")
            Dim grp7 As String = HttpContext.Current.Request("group7")
            Dim grp8 As String = HttpContext.Current.Request("group8")
            Dim grp9 As String = HttpContext.Current.Request("group9")
            Dim grp10 As String = HttpContext.Current.Request("group10")

            Dim currentQueryString As New NameValueCollection
            'Dim currentQueryString As new NameValueCollection = HttpContext.Current.Request.QueryString
            Dim newQueryString As String

            currentQueryString = HttpContext.Current.Request.QueryString

            Dim err As New ErrorObj
            '-------------------------------------------
            ' Find which browse page is the product list
            '-------------------------------------------
            Dim numberOfGroups As Integer

            Dim myDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
            numberOfGroups = def.NumberOfGroupLevels

            Dim redirectPage As String = String.Empty
            Dim redirectLevel As Integer = 0

            Select Case numberOfGroups
                Case Is = 1
                    redirectPage = "browse02.aspx"
                    redirectLevel = 2
                Case Is = 2
                    redirectPage = "browse03.aspx"
                    redirectLevel = 3
                Case Is = 3
                    redirectPage = "browse04.aspx"
                    redirectLevel = 4
                Case Is = 4
                    redirectPage = "browse05.aspx"
                    redirectLevel = 5
                Case Is = 5
                    redirectPage = "browse06.aspx"
                    redirectLevel = 6
                Case Is = 6
                    redirectPage = "browse07.aspx"
                    redirectLevel = 7
                Case Is = 7
                    redirectPage = "browse08.aspx"
                    redirectLevel = 8
                Case Is = 8
                    redirectPage = "browse09.aspx"
                    redirectLevel = 9
                Case Is = 9
                    redirectPage = "browse10.aspx"
                    redirectLevel = 10
                Case Is = 10
                    redirectPage = "browse11.aspx"
                    redirectLevel = 11
                Case Else
                    '--------
                    ' Default
                    '--------
                    redirectPage = "browse5.aspx"
                    redirectLevel = 5
            End Select

            If currentPage <> redirectPage Then

                '-------------------------------------
                ' Fill in empty groups in query string
                '-------------------------------------
                Dim sb2 As New StringBuilder
                With sb2
                    .Append(currentQueryString.ToString)
                    If redirectLevel >= 2 And currentQueryString("group1") Is Nothing Then
                        .Append("&group1=*EMPTY")
                    End If
                    If redirectLevel >= 3 And currentQueryString("group2") Is Nothing Then
                        .Append("&group2=*EMPTY")
                    End If
                    If redirectLevel >= 4 And currentQueryString("group3") Is Nothing Then
                        .Append("&group3=*EMPTY")
                    End If
                    If redirectLevel >= 5 And currentQueryString("group4") Is Nothing Then
                        .Append("&group4=*EMPTY")
                    End If
                    If redirectLevel >= 6 And currentQueryString("group5") Is Nothing Then
                        .Append("&group5=*EMPTY")
                    End If
                    If redirectLevel >= 7 And currentQueryString("group6") Is Nothing Then
                        .Append("&group6=*EMPTY")
                    End If
                    If redirectLevel >= 8 And currentQueryString("group7") Is Nothing Then
                        .Append("&group7=*EMPTY")
                    End If
                    If redirectLevel >= 9 And currentQueryString("group8") Is Nothing Then
                        .Append("&group8=*EMPTY")
                    End If
                    If redirectLevel >= 10 And currentQueryString("group9") Is Nothing Then
                        .Append("&group9=*EMPTY")
                    End If
                    If redirectLevel >= 11 And currentQueryString("group10") Is Nothing Then
                        .Append("&group10=*EMPTY")
                    End If
                End With
                newQueryString = sb2.ToString

                Dim fileName As String = String.Empty
                Dim strWhere As String = String.Empty
                Dim groupField As String = String.Empty
                Dim businessUnitField As String = String.Empty
                Dim partnerField As String = String.Empty
                Dim level As String = String.Empty
                Dim queryString As String = String.Empty
                Select Case currentPage
                    Case Is = "browse01.aspx"
                        level = "01"
                        queryString = "?"
                    Case Is = "browse02.aspx"
                        level = "02"
                        strWhere = "GROUP_L02_L01_GROUP = @GROUP1 "
                    Case Is = "browse03.aspx"
                        level = "03"
                        strWhere = "GROUP_L03_L01_GROUP = @GROUP1 AND " & _
                                   "GROUP_L03_L02_GROUP = @GROUP2"
                    Case Is = "browse04.aspx"
                        level = "04"
                        strWhere = "GROUP_L04_L01_GROUP = @GROUP1 AND " & _
                                   "GROUP_L04_L02_GROUP = @GROUP2 AND " & _
                                   "GROUP_L04_L03_GROUP = @GROUP3"
                    Case Is = "browse05.aspx"
                        level = "05"
                        strWhere = "GROUP_L05_L01_GROUP = @GROUP1 AND " & _
                                   "GROUP_L05_L02_GROUP = @GROUP2 AND " & _
                                   "GROUP_L05_L03_GROUP = @GROUP3 AND " & _
                                   "GROUP_L05_L04_GROUP = @GROUP4"
                    Case Is = "browse06.aspx"
                        level = "06"
                    Case Is = "browse07.aspx"
                        level = "07"
                    Case Is = "browse08.aspx"
                        level = "08"
                    Case Is = "browse09.aspx"
                        level = "09"
                    Case Is = "browse10.aspx"
                        level = "10"
                End Select

                fileName = "TBL_GROUP_LEVEL_" & level
                groupField = "GROUP_L" & level & "_L" & level & "_GROUP"
                businessUnitField = "GROUP_L" & level & "_BUSINESS_UNIT"
                partnerField = "GROUP_L" & level & "_PARTNER"
                '---------------------------------------------------------------
                ' Build SQL String e.g:
                ' SELECT * FROM TBL_GROUP_LEVEL_02 WHERE
                '    GROUP_L02_L01_GROUP = @GROUP1 AND
                '    GROUP_L02_BUSINESS_UNIT = @BUSINESS_UNIT AND 
                '   (GROUP_L02_PARTNER = @PARTNER OR GROUP_L02_PARTNER = '*ALL') 
                '---------------------------------------------------------------
                Dim sb As New StringBuilder
                With sb
                    .Append("SELECT * FROM ").Append(fileName).Append(" WHERE ")
                    If strWhere <> String.Empty Then
                        .Append(strWhere).Append(" AND ").Append(businessUnitField).Append(" = @BUSINESS_UNIT AND (")
                        .Append(partnerField).Append(" = @PARTNER OR ").Append(partnerField).Append(" = '" & Talent.Common.Utilities.GetAllString & "') ")
                    End If

                End With

                Dim strSelect1 As String = sb.ToString
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
                        .ErrorNumber = "TEAPUIL-01"
                        .HasError = True
                    End With
                End Try
                '---------------------------------------------------------------------
                If Not err.HasError Then
                    Try
                        '------------
                        ' Get details
                        '------------
                        Dim cmdSelect1 As SqlCommand = New SqlCommand(strSelect1, conTalent)

                        cmdSelect1.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = businessUnit
                        cmdSelect1.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partner
                        If strSelect1.Contains("@GROUP1") Then
                            cmdSelect1.Parameters.Add(New SqlParameter("@GROUP1", SqlDbType.Char, 20)).Value = grp1
                        End If
                        If strSelect1.Contains("@GROUP2") Then
                            cmdSelect1.Parameters.Add(New SqlParameter("@GROUP2", SqlDbType.Char, 20)).Value = grp2
                        End If
                        If strSelect1.Contains("@GROUP3") Then
                            cmdSelect1.Parameters.Add(New SqlParameter("@GROUP3", SqlDbType.Char, 20)).Value = grp3
                        End If
                        If strSelect1.Contains("@GROUP4") Then
                            cmdSelect1.Parameters.Add(New SqlParameter("@GROUP4", SqlDbType.Char, 20)).Value = grp4
                        End If
                        If strSelect1.Contains("@GROUP5") Then
                            cmdSelect1.Parameters.Add(New SqlParameter("@GROUP5", SqlDbType.Char, 20)).Value = grp5
                        End If

                        Dim dtrGroups As SqlDataReader = cmdSelect1.ExecuteReader()

                        If dtrGroups.HasRows = False Then
                            cmdSelect1.Parameters.RemoveAt("@BUSINESS_UNIT")
                            cmdSelect1.Parameters.RemoveAt("@PARTNER")
                            cmdSelect1.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = businessUnit
                            cmdSelect1.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                            dtrGroups = cmdSelect1.ExecuteReader()
                            If dtrGroups.HasRows = False Then
                                cmdSelect1.Parameters.RemoveAt("@BUSINESS_UNIT")
                                cmdSelect1.Parameters.RemoveAt("@PARTNER")
                                cmdSelect1.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                                cmdSelect1.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partner
                                dtrGroups = cmdSelect1.ExecuteReader()
                                If dtrGroups.HasRows = False Then
                                    cmdSelect1.Parameters.RemoveAt("@BUSINESS_UNIT")
                                    cmdSelect1.Parameters.RemoveAt("@PARTNER")
                                    cmdSelect1.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                                    cmdSelect1.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                                    dtrGroups = cmdSelect1.ExecuteReader()
                                End If
                            End If
                        End If

                        Dim count As Int32 = 0
                        Dim lastGroup As String = String.Empty
                        '-------------------------------------
                        ' Check if only 1 group and its *empty
                        '-------------------------------------
                        If dtrGroups.HasRows Then
                            While dtrGroups.Read()
                                lastGroup = dtrGroups(groupField)
                                count += 1
                            End While

                            If count = 1 And lastGroup.ToUpper = "*EMPTY" Then
                                HttpContext.Current.Response.Redirect("~/PagesPublic/ProductBrowse/" & _
                                            redirectPage & "?" & newQueryString)
                                Exit Sub
                            End If

                        End If
                        dtrGroups.Close()
                    Catch ex As Exception
                        Const strError8 As String = "Error during database access"
                        With err
                            .ErrorMessage = ex.Message
                            .ErrorStatus = strError8
                            .ErrorNumber = "TEAPUIL-06"
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
                        .ErrorNumber = "TEAPUIL-07"
                        .HasError = True
                    End With
                End Try

            End If


        End Sub

        Public Shared Function GetGroupDescription() As String
            ' Declare this first! Used for Logging function duration
            Dim timeSpan As TimeSpan = Now.TimeOfDay

            '-----------------------------------------------------------------------
            '   CheckForEmptyGroup - Check if current level only contains empty groups
            '                        If so redirect to product list level
            '-----------------------------------------------------------------------
            Dim groupDescription As String = String.Empty
            Dim err As New ErrorObj
            Dim conTalent As SqlConnection = Nothing
            Dim currentPage As String = GetCurrentPageName()
            Dim groupName As String = String.Empty
            While (groupName = String.Empty Or _
                             groupName.ToUpper = "*EMPTY") And _
                             currentPage <> "browse01.aspx"

                Select Case currentPage
                    Case Is = "browse02.aspx"
                        groupName = HttpContext.Current.Request("group1")
                        currentPage = "browse01.aspx"
                    Case Is = "browse03.aspx"
                        currentPage = "browse02.aspx"
                        groupName = HttpContext.Current.Request("group2")
                    Case Is = "browse04.aspx"
                        groupName = HttpContext.Current.Request("group3")
                        currentPage = "browse03.aspx"
                    Case Is = "browse05.aspx"
                        groupName = HttpContext.Current.Request("group4")
                        currentPage = "browse04.aspx"
                    Case Is = "browse06.aspx"
                        groupName = HttpContext.Current.Request("group5")
                        currentPage = "browse05.aspx"
                    Case Is = "browse07.aspx"
                        groupName = HttpContext.Current.Request("group6")
                        currentPage = "browse06.aspx"
                    Case Is = "browse08.aspx"
                        groupName = HttpContext.Current.Request("group7")
                        currentPage = "browse07.aspx"
                    Case Is = "browse09.aspx"
                        groupName = HttpContext.Current.Request("group8")
                        currentPage = "browse08.aspx"
                    Case Is = "browse10.aspx"
                        groupName = HttpContext.Current.Request("group9")
                        currentPage = "browse09.aspx"
                End Select
            End While

            Try
                Const SqlServer2005 As String = "TalentEBusinessDBConnectionString"
                conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SqlServer2005).ConnectionString)
                conTalent.Open()
            Catch ex As Exception
                Const strError1 As String = "Could not establish connection to the database"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError1
                    .ErrorNumber = "TEAPUIL-01"
                    .HasError = True
                End With
            End Try

            If Not err.HasError Then
                Try
                    '---------------------
                    ' Get details for Page
                    '---------------------
                    Const strSelect As String = "SELECT * FROM TBL_GROUP WITH (NOLOCK)  WHERE GROUP_NAME = @GROUP_NAME"
                    Dim cmdSelect As SqlCommand = New SqlCommand(strSelect, conTalent)
                    '-------------
                    ' Try for Page
                    '-------------
                    cmdSelect.Parameters.Add(New SqlParameter("@GROUP_NAME", SqlDbType.Char, 20)).Value = groupName
                    Dim dtrGroup As SqlDataReader = cmdSelect.ExecuteReader()
                    If dtrGroup.HasRows Then
                        dtrGroup.Read()
                        groupDescription = dtrGroup("GROUP_DESCRIPTION_1")
                    End If
                    dtrGroup.Close()
                Catch ex As Exception
                    Const strError8 As String = "Error during database access"
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError8
                        .ErrorNumber = "TEAPUIL-06"
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
                    .ErrorNumber = "TEAPUIL-07"
                    .HasError = True
                End With
            End Try

            Utilities.TalentLogging.LoadTestLog("Utilities.vb", "GetGroupDescription", timeSpan)
            Return groupDescription
        End Function

        Public Shared Function GetProductRelationsDefaults(ByVal strPage As String, ByVal strControlName As String) As Boolean

            'Dim dt1 As Data.DataTable
            'Dim dr1 As Data.DataRow
            'Dim businessUnit As String = TalentCache.GetBusinessUnit()
            'Dim partner As String = TalentCache.GetPartner(HttpContext.Current.Profile)
            'Dim cachename As String = "GetProductRelationsDefaults" & businessUnit & partner & strPage & strControlName

            'If HttpContext.Current.Cache.Item(cachename) Is Nothing Then
            '    Dim boolProductRelationsGraphicalAssociatedTop As Boolean = False
            '    Dim boolProductRelationsGraphicalAssociatedBottom As Boolean = False
            '    Dim boolProductRelationsGraphicalRelatedTop As Boolean = False
            '    Dim boolProductRelationsGraphicalRelatedBottom As Boolean = False
            '    Dim productRelationsDefaults As New TalentProductRelationsDefaultsTableAdapters.tbl_product_relations_defaultsTableAdapter
            '    dt1 = productRelationsDefaults.GetDataByBU_Partner_Qual_Page(businessUnit, partner, strPage)
            '    If dt1.Rows.Count = 0 Then
            '        dt1 = productRelationsDefaults.GetDataByBU_Partner_Qual_Page(businessUnit, Talent.Common.Utilities.GetAllString, strPage)
            '    End If
            '    If dt1.Rows.Count > 0 Then
            '        For Each dr1 In dt1.Rows
            '            If dr1("QUALIFIER").ToString.ToUpper.Trim = "2" Then
            '                If dr1("ONOFF") Then
            '                    If dr1("PAGE_POSITION").ToString.ToUpper.Trim = "1" Then
            '                        boolProductRelationsGraphicalAssociatedTop = True
            '                    Else
            '                        boolProductRelationsGraphicalAssociatedBottom = True
            '                    End If
            '                End If
            '            End If
            '            If dr1("QUALIFIER").ToString.ToUpper.Trim = "2" Then
            '                If dr1("ONOFF") Then
            '                    If dr1("PAGE_POSITION").ToString.ToUpper.Trim = "1" Then
            '                        boolProductRelationsGraphicalRelatedTop = True
            '                    Else
            '                        boolProductRelationsGraphicalRelatedBottom = True
            '                    End If
            '                End If
            '            End If
            '        Next
            '    End If
            '    HttpContext.Current.Cache.Insert("GetProductRelationsDefaults" & businessUnit & partner & strPage & "ProductRelationsGraphicalAssociatedTop", boolProductRelationsGraphicalAssociatedTop, Nothing, System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), Caching.Cache.NoSlidingExpiration)
            '    HttpContext.Current.Cache.Insert("GetProductRelationsDefaults" & businessUnit & partner & strPage & "ProductRelationsGraphicalAssociatedBottom", boolProductRelationsGraphicalAssociatedBottom, Nothing, System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), Caching.Cache.NoSlidingExpiration)
            '    HttpContext.Current.Cache.Insert("GetProductRelationsDefaults" & businessUnit & partner & strPage & "ProductRelationsGraphicalRelatedTop", boolProductRelationsGraphicalRelatedTop, Nothing, System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), Caching.Cache.NoSlidingExpiration)
            '    HttpContext.Current.Cache.Insert("GetProductRelationsDefaults" & businessUnit & partner & strPage & "ProductRelationsGraphicalRelatedBottom", boolProductRelationsGraphicalRelatedBottom, Nothing, System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), Caching.Cache.NoSlidingExpiration)
            '    Select Case strControlName
            '        Case Is = "ProductRelationsGraphicalRelatedTop"
            '            Return boolProductRelationsGraphicalRelatedTop
            '        Case Is = "ProductRelationsGraphicalRelatedBottom"
            '            Return boolProductRelationsGraphicalRelatedBottom
            '        Case Is = "ProductRelationsGraphicalAssociatedTop"
            '            Return boolProductRelationsGraphicalAssociatedTop
            '        Case Is = "ProductRelationsGraphicalAssociatedBottom"
            '            Return boolProductRelationsGraphicalAssociatedBottom
            '    End Select
            'Else
            '    Return CType(HttpContext.Current.Cache.Item(cachename), Boolean)
            'End If
            Return False
        End Function

        Public Shared Function GetProductListTemplateType() As String
            ' Declare this first! Used for Logging function duration
            Dim timeSpan As TimeSpan = Now.TimeOfDay

            '-----------------------------------------------------------------------
            '   CheckForEmptyGroup - Check if current level only contains empty groups
            '                        If so redirect to product list level
            '-----------------------------------------------------------------------
            Dim businessUnit As String = TalentCache.GetBusinessUnit()
            Dim partner As String = TalentCache.GetPartner(HttpContext.Current.Profile)
            Dim plTemplate As String = ""
            Dim err As New ErrorObj
            Dim conTalent As SqlConnection = Nothing
            Dim currentPage As String = GetCurrentPageName()
            'Dim groupName As String = String.Empty
            Dim groupLevel As Integer = 0

            Dim group1Name As String = "", _
                group2Name As String = "", _
                group3Name As String = "", _
                group4Name As String = "", _
                group5Name As String = "", _
                group6Name As String = "", _
                group7Name As String = "", _
                group8Name As String = "", _
                group9Name As String = ""

            Try
                If Not HttpContext.Current.Request("group1") Is Nothing Then group1Name = HttpContext.Current.Request("group1")
                If Not HttpContext.Current.Request("group2") Is Nothing Then group2Name = HttpContext.Current.Request("group2")
                If Not HttpContext.Current.Request("group3") Is Nothing Then group3Name = HttpContext.Current.Request("group3")
                If Not HttpContext.Current.Request("group4") Is Nothing Then group4Name = HttpContext.Current.Request("group4")
                If Not HttpContext.Current.Request("group5") Is Nothing Then group5Name = HttpContext.Current.Request("group5")
                If Not HttpContext.Current.Request("group6") Is Nothing Then group6Name = HttpContext.Current.Request("group6")
                If Not HttpContext.Current.Request("group7") Is Nothing Then group7Name = HttpContext.Current.Request("group7")
                If Not HttpContext.Current.Request("group8") Is Nothing Then group8Name = HttpContext.Current.Request("group8")
                If Not HttpContext.Current.Request("group9") Is Nothing Then group9Name = HttpContext.Current.Request("group9")
            Catch ex As Exception
            End Try

            While (groupLevel = 0) AndAlso _
                         currentPage <> "browse01.aspx"

                Select Case currentPage
                    Case Is = "browse02.aspx"
                        currentPage = "browse01.aspx"
                        groupLevel = 1
                    Case Is = "browse03.aspx"
                        currentPage = "browse02.aspx"
                        groupLevel = 2
                    Case Is = "browse04.aspx"
                        currentPage = "browse03.aspx"
                        groupLevel = 3
                    Case Is = "browse05.aspx"
                        currentPage = "browse04.aspx"
                        groupLevel = 4
                    Case Is = "browse06.aspx"
                        currentPage = "browse05.aspx"
                        groupLevel = 5
                    Case Is = "browse07.aspx"
                        currentPage = "browse06.aspx"
                        groupLevel = 6
                    Case Is = "browse08.aspx"
                        currentPage = "browse07.aspx"
                        groupLevel = 7
                    Case Is = "browse09.aspx"
                        currentPage = "browse08.aspx"
                        groupLevel = 8
                    Case Is = "browse10.aspx"
                        currentPage = "browse09.aspx"
                        groupLevel = 9
                End Select
            End While


            If Not err.HasError Then
                Try

                    'If we are to override the display as group value with the value from the group level 01
                    'table then set the group level to 1 and the code will only check for level 1 settings
                    Dim myDefaults As New ECommerceModuleDefaults
                    Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
                    If def.Override_Show_Children_As_Groups_Using_L01_Value Then
                        groupLevel = 1
                    End If

                    Dim cacheKey As String = "PRODUCT_LIST_TEMPLATE_TYPE" & TalentCache.GetBusinessUnit & "_" & TalentCache.GetPartner(HttpContext.Current.Profile)

                    '-----------------------------------------------
                    ' Get details for Page using current partner
                    '-----------------------------------------------
                    Dim strSelect As String = "", _
                        groupLevelStr As String = "GROUP_L01"
                    Select Case groupLevel
                        Case Is = 1
                            cacheKey += "_" & group1Name
                            strSelect = " SELECT * FROM TBL_GROUP_LEVEL_01 WITH (NOLOCK)  " & _
                                        " WHERE GROUP_L01_BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                        " AND GROUP_L01_PARTNER = @PARTNER " & _
                                        " AND GROUP_L01_L01_GROUP = @GROUP_NAME1"
                            groupLevelStr = "GROUP_L01"
                        Case Is = 2
                            cacheKey += "_" & group1Name & _
                                        "_" & group2Name
                            strSelect = " SELECT * FROM TBL_GROUP_LEVEL_02 WITH (NOLOCK)  " & _
                                        " WHERE GROUP_L02_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " AND GROUP_L02_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L02_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L02_L02_GROUP = @GROUP_NAME2 "
                            groupLevelStr = "GROUP_L02"
                        Case Is = 3
                            cacheKey += "_" & group1Name & _
                                        "_" & group2Name & _
                                        "_" & group3Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_03 WITH (NOLOCK)   " & _
                                        " WHERE GROUP_L03_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " AND GROUP_L03_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L03_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L03_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L03_L03_GROUP = @GROUP_NAME3 "
                            groupLevelStr = "GROUP_L03"
                        Case Is = 4
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_04 WITH (NOLOCK)   " & _
                                        " WHERE GROUP_L04_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " AND GROUP_L04_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L04_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L04_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L04_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L04_L04_GROUP = @GROUP_NAME4 "
                            groupLevelStr = "GROUP_L04"
                        Case Is = 5
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name & _
                                       "_" & group5Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_05 WITH (NOLOCK)   " & _
                                        " WHERE GROUP_L05_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " AND GROUP_L05_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L05_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L05_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L05_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L05_L04_GROUP = @GROUP_NAME4 " & _
                                        " AND GROUP_L05_L05_GROUP = @GROUP_NAME5 "
                            groupLevelStr = "GROUP_L05"
                        Case Is = 6
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name & _
                                       "_" & group5Name & _
                                       "_" & group6Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_06 WITH (NOLOCK)   " & _
                                        " WHERE GROUP_L06_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " AND GROUP_L06_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L06_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L06_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L06_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L06_L04_GROUP = @GROUP_NAME4 " & _
                                        " AND GROUP_L06_L05_GROUP = @GROUP_NAME5 " & _
                                        " AND GROUP_L06_L06_GROUP = @GROUP_NAME6 "
                            groupLevelStr = "GROUP_L06"
                        Case Is = 7
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name & _
                                       "_" & group5Name & _
                                       "_" & group6Name & _
                                       "_" & group7Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_07 WITH (NOLOCK)   " & _
                                        " WHERE GROUP_L07_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " AND GROUP_L07_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L07_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L07_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L07_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L07_L04_GROUP = @GROUP_NAME4 " & _
                                        " AND GROUP_L07_L05_GROUP = @GROUP_NAME5 " & _
                                        " AND GROUP_L07_L06_GROUP = @GROUP_NAME6 " & _
                                        " AND GROUP_L07_L07_GROUP = @GROUP_NAME7 "
                            groupLevelStr = "GROUP_L07"
                        Case Is = 8
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name & _
                                       "_" & group5Name & _
                                       "_" & group6Name & _
                                       "_" & group7Name & _
                                       "_" & group8Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_08 WITH (NOLOCK)   " & _
                                        " WHERE GROUP_L08_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " AND GROUP_L08_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L08_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L08_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L08_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L08_L04_GROUP = @GROUP_NAME4 " & _
                                        " AND GROUP_L08_L05_GROUP = @GROUP_NAME5 " & _
                                        " AND GROUP_L08_L06_GROUP = @GROUP_NAME6 " & _
                                        " AND GROUP_L08_L07_GROUP = @GROUP_NAME7 " & _
                                        " AND GROUP_L08_L08_GROUP = @GROUP_NAME8 "
                            groupLevelStr = "GROUP_L08"
                        Case Is = 9
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name & _
                                       "_" & group5Name & _
                                       "_" & group6Name & _
                                       "_" & group7Name & _
                                       "_" & group8Name & _
                                       "_" & group9Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_09 WITH (NOLOCK)   " & _
                                        " WHERE GROUP_L09_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " AND GROUP_L09_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L09_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L09_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L09_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L09_L04_GROUP = @GROUP_NAME4 " & _
                                        " AND GROUP_L09_L05_GROUP = @GROUP_NAME5 " & _
                                        " AND GROUP_L09_L06_GROUP = @GROUP_NAME6 " & _
                                        " AND GROUP_L09_L07_GROUP = @GROUP_NAME7 " & _
                                        " AND GROUP_L09_L08_GROUP = @GROUP_NAME8 " & _
                                        " AND GROUP_L09_L09_GROUP = @GROUP_NAME9 "
                            groupLevelStr = "GROUP_L09"
                    End Select

                    If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                        plTemplate = CStr(HttpContext.Current.Cache.Item(cacheKey))
                    Else
                        Const SqlServer2005 As String = "TalentEBusinessDBConnectionString"
                        conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SqlServer2005).ConnectionString)
                        conTalent.Open()

                        Dim cmdSelect As SqlCommand = New SqlCommand(strSelect, conTalent)
                        With cmdSelect.Parameters
                            .Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = businessUnit
                            .Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partner
                            If groupLevel >= 1 Then .Add(New SqlParameter("@GROUP_NAME1", SqlDbType.Char, 20)).Value = group1Name
                            If groupLevel >= 2 Then .Add(New SqlParameter("@GROUP_NAME2", SqlDbType.Char, 20)).Value = group2Name
                            If groupLevel >= 3 Then .Add(New SqlParameter("@GROUP_NAME3", SqlDbType.Char, 20)).Value = group3Name
                            If groupLevel >= 4 Then .Add(New SqlParameter("@GROUP_NAME4", SqlDbType.Char, 20)).Value = group4Name
                            If groupLevel >= 5 Then .Add(New SqlParameter("@GROUP_NAME5", SqlDbType.Char, 20)).Value = group5Name
                            If groupLevel >= 6 Then .Add(New SqlParameter("@GROUP_NAME6", SqlDbType.Char, 20)).Value = group6Name
                            If groupLevel >= 7 Then .Add(New SqlParameter("@GROUP_NAME7", SqlDbType.Char, 20)).Value = group7Name
                            If groupLevel >= 8 Then .Add(New SqlParameter("@GROUP_NAME8", SqlDbType.Char, 20)).Value = group8Name
                            If groupLevel >= 9 Then .Add(New SqlParameter("@GROUP_NAME9", SqlDbType.Char, 20)).Value = group9Name
                        End With

                        Dim dtrGroup As SqlDataReader = cmdSelect.ExecuteReader()
                        If dtrGroup.HasRows Then
                            dtrGroup.Read()
                            plTemplate = Utilities.CheckForDBNull_String(dtrGroup(groupLevelStr & "_PRODUCT_LIST_TEMPLATE"))
                            dtrGroup.Close()
                        Else
                            '-----------------------------------------------
                            ' Get details for Page using *ALL partner
                            '-----------------------------------------------
                            dtrGroup.Close()
                            cmdSelect.Parameters.RemoveAt("@PARTNER")
                            cmdSelect.Parameters("@PARTNER").Value = Talent.Common.Utilities.GetAllString

                            If dtrGroup.HasRows Then
                                dtrGroup.Read()
                                plTemplate = Utilities.CheckForDBNull_String(dtrGroup(groupLevelStr & "_PRODUCT_LIST_TEMPLATE"))
                                dtrGroup.Close()
                            Else
                                '-----------------------------------------------
                                ' Get details for Page using *ALL business units
                                '-----------------------------------------------
                                dtrGroup.Close()
                                cmdSelect.Parameters.RemoveAt("@BUSINESS_UNIT")
                                cmdSelect.Parameters.RemoveAt("@PARTNER")
                                cmdSelect.Parameters("@PARTNER").Value = partner
                                cmdSelect.Parameters("@BUSINESS_UNIT").Value = Talent.Common.Utilities.GetAllString

                                dtrGroup = cmdSelect.ExecuteReader()

                                If dtrGroup.HasRows Then
                                    dtrGroup.Read()
                                    plTemplate = Utilities.CheckForDBNull_String(dtrGroup(groupLevelStr & "_PRODUCT_LIST_TEMPLATE"))
                                    dtrGroup.Close()
                                Else
                                    '-----------------------------------------------
                                    ' Get details for Page using *ALL partner and business units
                                    '-----------------------------------------------
                                    dtrGroup.Close()
                                    cmdSelect.Parameters.RemoveAt("@BUSINESS_UNIT")
                                    cmdSelect.Parameters.RemoveAt("@PARTNER")
                                    cmdSelect.Parameters("@PARTNER").Value = Talent.Common.Utilities.GetAllString
                                    cmdSelect.Parameters("@BUSINESS_UNIT").Value = Talent.Common.Utilities.GetAllString

                                    dtrGroup = cmdSelect.ExecuteReader()

                                End If

                            End If

                            If dtrGroup.HasRows Then
                                dtrGroup.Read()
                                plTemplate = Utilities.CheckForDBNull_String(dtrGroup(groupLevelStr & "_PRODUCT_LIST_TEMPLATE"))
                            End If
                            dtrGroup.Close()
                        End If
                    End If
                    HttpContext.Current.Cache.Add(cacheKey, plTemplate, Nothing, Now.AddMinutes(30), timeSpan.Zero, CacheItemPriority.Normal, Nothing)
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)

                    'plTemplate = Utilities.CheckForDBNull_String(dtrGroup2(groupLevelStr & "_PRODUCT_LIST_TEMPLATE"))
                Catch ex As Exception
                    Const strError8 As String = "Error during database access"
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError8
                        .ErrorNumber = "TEAPUIL-200"
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
                    .ErrorNumber = "TEAPUIL-201"
                    .HasError = True
                End With
            End Try

            Utilities.TalentLogging.LoadTestLog("Utilities.vb", "GetProductListTemplateType", timeSpan)
            Return plTemplate
        End Function

        Public Shared Function ShowChildrenAsGroups() As Boolean
            ' Declare this first! Used for Logging function duration
            Dim timeSpan As TimeSpan = Now.TimeOfDay

            '-----------------------------------------------------------------------
            '   CheckForEmptyGroup - Check if current level only contains empty groups
            '                        If so redirect to product list level
            '-----------------------------------------------------------------------
            Dim businessUnit As String = TalentCache.GetBusinessUnit()
            Dim partner As String = TalentCache.GetPartner(HttpContext.Current.Profile)
            Dim showAsGroups As Boolean = False
            Dim err As New ErrorObj
            Dim conTalent As SqlConnection = Nothing
            Dim currentPage As String = GetCurrentPageName()
            'Dim groupName As String = String.Empty
            Dim groupLevel As Integer = 0

            Dim group1Name As String = "", _
                group2Name As String = "", _
                group3Name As String = "", _
                group4Name As String = "", _
                group5Name As String = "", _
                group6Name As String = "", _
                group7Name As String = "", _
                group8Name As String = "", _
                group9Name As String = ""

            Try
                If Not HttpContext.Current.Request("group1") Is Nothing Then group1Name = HttpContext.Current.Request("group1")
                If Not HttpContext.Current.Request("group2") Is Nothing Then group2Name = HttpContext.Current.Request("group2")
                If Not HttpContext.Current.Request("group3") Is Nothing Then group3Name = HttpContext.Current.Request("group3")
                If Not HttpContext.Current.Request("group4") Is Nothing Then group4Name = HttpContext.Current.Request("group4")
                If Not HttpContext.Current.Request("group5") Is Nothing Then group5Name = HttpContext.Current.Request("group5")
                If Not HttpContext.Current.Request("group6") Is Nothing Then group6Name = HttpContext.Current.Request("group6")
                If Not HttpContext.Current.Request("group7") Is Nothing Then group7Name = HttpContext.Current.Request("group7")
                If Not HttpContext.Current.Request("group8") Is Nothing Then group8Name = HttpContext.Current.Request("group8")
                If Not HttpContext.Current.Request("group9") Is Nothing Then group9Name = HttpContext.Current.Request("group9")
            Catch ex As Exception
            End Try

            While (groupLevel = 0) AndAlso _
                         currentPage <> "browse01.aspx"

                Select Case currentPage
                    Case Is = "browse02.aspx"
                        currentPage = "browse01.aspx"
                        groupLevel = 1
                    Case Is = "browse03.aspx"
                        currentPage = "browse02.aspx"
                        groupLevel = 2
                    Case Is = "browse04.aspx"
                        currentPage = "browse03.aspx"
                        groupLevel = 3
                    Case Is = "browse05.aspx"
                        currentPage = "browse04.aspx"
                        groupLevel = 4
                    Case Is = "browse06.aspx"
                        currentPage = "browse05.aspx"
                        groupLevel = 5
                    Case Is = "browse07.aspx"
                        currentPage = "browse06.aspx"
                        groupLevel = 6
                    Case Is = "browse08.aspx"
                        currentPage = "browse07.aspx"
                        groupLevel = 7
                    Case Is = "browse09.aspx"
                        currentPage = "browse08.aspx"
                        groupLevel = 8
                    Case Is = "browse10.aspx"
                        currentPage = "browse09.aspx"
                        groupLevel = 9
                End Select
            End While


            If Not err.HasError Then
                Try

                    'If we are to override the display as group value with the value from the group level 01
                    'table then set the group level to 1 and the code will only check for level 1 settings
                    Dim myDefaults As New ECommerceModuleDefaults
                    Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
                    If def.Override_Show_Children_As_Groups_Using_L01_Value Then
                        groupLevel = 1
                    End If

                    Dim cacheKey As String = "SHOWCHILDRENASGROUPS_" & TalentCache.GetBusinessUnit & "_" & TalentCache.GetPartner(HttpContext.Current.Profile)

                    '-----------------------------------------------
                    ' Get details for Page using current partner
                    '-----------------------------------------------
                    Dim strSelect As String = "", _
                        groupLevelStr As String = "GROUP_L01"
                    Select Case groupLevel
                        Case Is = 1
                            cacheKey += "_" & group1Name
                            strSelect = " SELECT * FROM TBL_GROUP_LEVEL_01 WITH (NOLOCK)  " & _
                                        " WHERE (GROUP_L01_BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                        " OR GROUP_L01_BUSINESS_UNIT = '*ALL')" & _
                                        " AND GROUP_L01_PARTNER = @PARTNER " & _
                                        " AND GROUP_L01_L01_GROUP = @GROUP_NAME1"
                            groupLevelStr = "GROUP_L01"
                        Case Is = 2
                            cacheKey += "_" & group1Name & _
                                        "_" & group2Name
                            strSelect = " SELECT * FROM TBL_GROUP_LEVEL_02 WITH (NOLOCK)  " & _
                                        " WHERE (GROUP_L02_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L02_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L02_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L02_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L02_L02_GROUP = @GROUP_NAME2 "
                            groupLevelStr = "GROUP_L02"
                        Case Is = 3
                            cacheKey += "_" & group1Name & _
                                        "_" & group2Name & _
                                        "_" & group3Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_03 WITH (NOLOCK)   " & _
                                        " WHERE (GROUP_L03_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L03_BUSINESS_UNIT = '*ALL')" & _
                                        " AND GROUP_L03_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L03_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L03_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L03_L03_GROUP = @GROUP_NAME3 "
                            groupLevelStr = "GROUP_L03"
                        Case Is = 4
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_04 WITH (NOLOCK)   " & _
                                        " WHERE (GROUP_L04_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L04_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L04_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L04_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L04_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L04_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L04_L04_GROUP = @GROUP_NAME4 "
                            groupLevelStr = "GROUP_L04"
                        Case Is = 5
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name & _
                                       "_" & group5Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_05 WITH (NOLOCK)   " & _
                                        " WHERE (GROUP_L05_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L05_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L05_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L05_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L05_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L05_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L05_L04_GROUP = @GROUP_NAME4 " & _
                                        " AND GROUP_L05_L05_GROUP = @GROUP_NAME5 "
                            groupLevelStr = "GROUP_L05"
                        Case Is = 6
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name & _
                                       "_" & group5Name & _
                                       "_" & group6Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_06 WITH (NOLOCK)   " & _
                                        " WHERE (GROUP_L06_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L06_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L06_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L06_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L06_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L06_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L06_L04_GROUP = @GROUP_NAME4 " & _
                                        " AND GROUP_L06_L05_GROUP = @GROUP_NAME5 " & _
                                        " AND GROUP_L06_L06_GROUP = @GROUP_NAME6 "
                            groupLevelStr = "GROUP_L06"
                        Case Is = 7
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name & _
                                       "_" & group5Name & _
                                       "_" & group6Name & _
                                       "_" & group7Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_07 WITH (NOLOCK)   " & _
                                        " WHERE (GROUP_L07_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L07_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L07_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L07_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L07_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L07_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L07_L04_GROUP = @GROUP_NAME4 " & _
                                        " AND GROUP_L07_L05_GROUP = @GROUP_NAME5 " & _
                                        " AND GROUP_L07_L06_GROUP = @GROUP_NAME6 " & _
                                        " AND GROUP_L07_L07_GROUP = @GROUP_NAME7 "
                            groupLevelStr = "GROUP_L07"
                        Case Is = 8
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name & _
                                       "_" & group5Name & _
                                       "_" & group6Name & _
                                       "_" & group7Name & _
                                       "_" & group8Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_08 WITH (NOLOCK)   " & _
                                         " WHERE (GROUP_L08_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L08_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L08_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L08_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L08_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L08_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L08_L04_GROUP = @GROUP_NAME4 " & _
                                        " AND GROUP_L08_L05_GROUP = @GROUP_NAME5 " & _
                                        " AND GROUP_L08_L06_GROUP = @GROUP_NAME6 " & _
                                        " AND GROUP_L08_L07_GROUP = @GROUP_NAME7 " & _
                                        " AND GROUP_L08_L08_GROUP = @GROUP_NAME8 "
                            groupLevelStr = "GROUP_L08"
                        Case Is = 9
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name & _
                                       "_" & group5Name & _
                                       "_" & group6Name & _
                                       "_" & group7Name & _
                                       "_" & group8Name & _
                                       "_" & group9Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_09 WITH (NOLOCK)   " & _
                                         " WHERE (GROUP_L09_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L09_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L09_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L09_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L09_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L09_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L09_L04_GROUP = @GROUP_NAME4 " & _
                                        " AND GROUP_L09_L05_GROUP = @GROUP_NAME5 " & _
                                        " AND GROUP_L09_L06_GROUP = @GROUP_NAME6 " & _
                                        " AND GROUP_L09_L07_GROUP = @GROUP_NAME7 " & _
                                        " AND GROUP_L09_L08_GROUP = @GROUP_NAME8 " & _
                                        " AND GROUP_L09_L09_GROUP = @GROUP_NAME9 "
                            groupLevelStr = "GROUP_L09"
                    End Select

                    If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                        showAsGroups = CBool(HttpContext.Current.Cache.Item(cacheKey))
                    Else
                        Const SqlServer2005 As String = "TalentEBusinessDBConnectionString"
                        conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SqlServer2005).ConnectionString)
                        conTalent.Open()

                        Dim cmdSelect As SqlCommand = New SqlCommand(strSelect, conTalent)
                        With cmdSelect.Parameters
                            .Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = businessUnit
                            .Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partner
                            If groupLevel >= 1 Then .Add(New SqlParameter("@GROUP_NAME1", SqlDbType.Char, 20)).Value = group1Name
                            If groupLevel >= 2 Then .Add(New SqlParameter("@GROUP_NAME2", SqlDbType.Char, 20)).Value = group2Name
                            If groupLevel >= 3 Then .Add(New SqlParameter("@GROUP_NAME3", SqlDbType.Char, 20)).Value = group3Name
                            If groupLevel >= 4 Then .Add(New SqlParameter("@GROUP_NAME4", SqlDbType.Char, 20)).Value = group4Name
                            If groupLevel >= 5 Then .Add(New SqlParameter("@GROUP_NAME5", SqlDbType.Char, 20)).Value = group5Name
                            If groupLevel >= 6 Then .Add(New SqlParameter("@GROUP_NAME6", SqlDbType.Char, 20)).Value = group6Name
                            If groupLevel >= 7 Then .Add(New SqlParameter("@GROUP_NAME7", SqlDbType.Char, 20)).Value = group7Name
                            If groupLevel >= 8 Then .Add(New SqlParameter("@GROUP_NAME8", SqlDbType.Char, 20)).Value = group8Name
                            If groupLevel >= 9 Then .Add(New SqlParameter("@GROUP_NAME9", SqlDbType.Char, 20)).Value = group9Name
                        End With

                        Dim dtrGroup As SqlDataReader = cmdSelect.ExecuteReader()
                        If dtrGroup.HasRows Then
                            dtrGroup.Read()
                            showAsGroups = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtrGroup(groupLevelStr & "_SHOW_CHILDREN_AS_GROUPS"))
                            dtrGroup.Close()
                        Else
                            '-----------------------------------------------
                            ' Get details for Page using *ALL partner
                            '-----------------------------------------------
                            dtrGroup.Close()
                            '   cmdSelect.Parameters.RemoveAt("@PARTNER")
                            cmdSelect.Parameters("@PARTNER").Value = Talent.Common.Utilities.GetAllString
                            dtrGroup = cmdSelect.ExecuteReader()

                            If dtrGroup.HasRows Then
                                dtrGroup.Read()
                                showAsGroups = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtrGroup(groupLevelStr & "_SHOW_CHILDREN_AS_GROUPS"))
                                dtrGroup.Close()
                            Else
                                '-----------------------------------------------
                                ' Get details for Page using *ALL business units
                                '-----------------------------------------------
                                dtrGroup.Close()
                                '  cmdSelect.Parameters.RemoveAt("@BUSINESS_UNIT")
                                '  cmdSelect.Parameters.RemoveAt("@PARTNER")
                                cmdSelect.Parameters("@PARTNER").Value = partner
                                cmdSelect.Parameters("@BUSINESS_UNIT").Value = Talent.Common.Utilities.GetAllString

                                dtrGroup = cmdSelect.ExecuteReader()

                                If dtrGroup.HasRows Then
                                    dtrGroup.Read()
                                    showAsGroups = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtrGroup(groupLevelStr & "_SHOW_CHILDREN_AS_GROUPS"))
                                    dtrGroup.Close()
                                Else
                                    '-----------------------------------------------
                                    ' Get details for Page using *ALL partner and business units
                                    '-----------------------------------------------
                                    dtrGroup.Close()
                                    cmdSelect.Parameters.RemoveAt("@BUSINESS_UNIT")
                                    cmdSelect.Parameters.RemoveAt("@PARTNER")
                                    cmdSelect.Parameters("@PARTNER").Value = Talent.Common.Utilities.GetAllString
                                    cmdSelect.Parameters("@BUSINESS_UNIT").Value = Talent.Common.Utilities.GetAllString

                                    dtrGroup = cmdSelect.ExecuteReader()

                                    If dtrGroup.HasRows Then
                                        dtrGroup.Read()
                                        showAsGroups = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtrGroup(groupLevelStr & "_SHOW_CHILDREN_AS_GROUPS"))
                                    End If
                                End If

                            End If
                            Try
                                dtrGroup.Close()
                            Catch ex As Exception

                            End Try

                        End If
                    End If
                    HttpContext.Current.Cache.Add(cacheKey, showAsGroups, Nothing, Now.AddMinutes(30), timeSpan.Zero, CacheItemPriority.Normal, Nothing)
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)


                Catch ex As Exception
                    Const strError8 As String = "Error during database access"
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError8
                        .ErrorNumber = "TEAPUIL-06"
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
                    .ErrorNumber = "TEAPUIL-07"
                    .HasError = True
                End With
            End Try

            Utilities.TalentLogging.LoadTestLog("Utilities.vb", "ShowChildrenAsGroups", timeSpan)
            Return showAsGroups
        End Function

        Public Shared Function ShowProductsDisplay() As Boolean
            ' Declare this first! Used for Logging function duration
            Dim timeSpan As TimeSpan = Now.TimeOfDay

            '-----------------------------------------------------------------------
            '   CheckForEmptyGroup - Check if current level only contains empty groups
            '                        If so redirect to product list level
            '-----------------------------------------------------------------------
            Dim businessUnit As String = TalentCache.GetBusinessUnit()
            Dim partner As String = TalentCache.GetPartner(HttpContext.Current.Profile)
            Dim showProducts As Boolean = True
            Dim err As New ErrorObj
            Dim conTalent As SqlConnection = Nothing
            Dim currentPage As String = GetCurrentPageName()
            'Dim groupName As String = String.Empty
            Dim groupLevel As Integer = 0

            Dim group1Name As String = "", _
                group2Name As String = "", _
                group3Name As String = "", _
                group4Name As String = "", _
                group5Name As String = "", _
                group6Name As String = "", _
                group7Name As String = "", _
                group8Name As String = "", _
                group9Name As String = ""

            Try
                If Not HttpContext.Current.Request("group1") Is Nothing Then group1Name = HttpContext.Current.Request("group1")
                If Not HttpContext.Current.Request("group2") Is Nothing Then group2Name = HttpContext.Current.Request("group2")
                If Not HttpContext.Current.Request("group3") Is Nothing Then group3Name = HttpContext.Current.Request("group3")
                If Not HttpContext.Current.Request("group4") Is Nothing Then group4Name = HttpContext.Current.Request("group4")
                If Not HttpContext.Current.Request("group5") Is Nothing Then group5Name = HttpContext.Current.Request("group5")
                If Not HttpContext.Current.Request("group6") Is Nothing Then group6Name = HttpContext.Current.Request("group6")
                If Not HttpContext.Current.Request("group7") Is Nothing Then group7Name = HttpContext.Current.Request("group7")
                If Not HttpContext.Current.Request("group8") Is Nothing Then group8Name = HttpContext.Current.Request("group8")
                If Not HttpContext.Current.Request("group9") Is Nothing Then group9Name = HttpContext.Current.Request("group9")
            Catch ex As Exception
            End Try

            While (groupLevel = 0) AndAlso _
                         currentPage <> "browse01.aspx"

                Select Case currentPage
                    Case Is = "browse02.aspx"
                        currentPage = "browse01.aspx"
                        groupLevel = 1
                    Case Is = "browse03.aspx"
                        currentPage = "browse02.aspx"
                        groupLevel = 2
                    Case Is = "browse04.aspx"
                        currentPage = "browse03.aspx"
                        groupLevel = 3
                    Case Is = "browse05.aspx"
                        currentPage = "browse04.aspx"
                        groupLevel = 4
                    Case Is = "browse06.aspx"
                        currentPage = "browse05.aspx"
                        groupLevel = 5
                    Case Is = "browse07.aspx"
                        currentPage = "browse06.aspx"
                        groupLevel = 6
                    Case Is = "browse08.aspx"
                        currentPage = "browse07.aspx"
                        groupLevel = 7
                    Case Is = "browse09.aspx"
                        currentPage = "browse08.aspx"
                        groupLevel = 8
                    Case Is = "browse10.aspx"
                        currentPage = "browse09.aspx"
                        groupLevel = 9
                End Select
            End While


            If Not err.HasError Then
                Try

                    'If we are to override the display as group value with the value from the group level 01
                    'table then set the group level to 1 and the code will only check for level 1 settings
                    Dim myDefaults As New ECommerceModuleDefaults
                    Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
                    If def.Override_Show_Products_Display_Using_L01_Value Then
                        groupLevel = 1
                    End If

                    Dim cacheKey As String = "SHOWPRODUCTSDISPLAY_" & TalentCache.GetBusinessUnit & "_" & TalentCache.GetPartner(HttpContext.Current.Profile)

                    '-----------------------------------------------
                    ' Get details for Page using current partner
                    '-----------------------------------------------
                    Dim strSelect As String = "", _
                        groupLevelStr As String = "GROUP_L01"
                    Select Case groupLevel
                        Case Is = 1
                            cacheKey += "_" & group1Name
                            strSelect = " SELECT * FROM TBL_GROUP_LEVEL_01 WITH (NOLOCK)  " & _
                                        " WHERE (GROUP_L01_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L01_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L01_PARTNER = @PARTNER " & _
                                        " AND GROUP_L01_L01_GROUP = @GROUP_NAME1"
                            groupLevelStr = "GROUP_L01"
                        Case Is = 2
                            cacheKey += "_" & group1Name & _
                                        "_" & group2Name
                            strSelect = " SELECT * FROM TBL_GROUP_LEVEL_02 WITH (NOLOCK)  " & _
                                        " WHERE (GROUP_L02_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L02_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L02_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L02_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L02_L02_GROUP = @GROUP_NAME2 "
                            groupLevelStr = "GROUP_L02"
                        Case Is = 3
                            cacheKey += "_" & group1Name & _
                                        "_" & group2Name & _
                                        "_" & group3Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_03 WITH (NOLOCK)   " & _
                                        " WHERE (GROUP_L03_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L03_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L03_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L03_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L03_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L03_L03_GROUP = @GROUP_NAME3 "
                            groupLevelStr = "GROUP_L03"
                        Case Is = 4
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_04 WITH (NOLOCK)   " & _
                                        " WHERE (GROUP_L04_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L04_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L04_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L04_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L04_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L04_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L04_L04_GROUP = @GROUP_NAME4 "
                            groupLevelStr = "GROUP_L04"
                        Case Is = 5
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name & _
                                       "_" & group5Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_05 WITH (NOLOCK)   " & _
                                        " WHERE (GROUP_L05_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L05_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L05_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L05_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L05_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L05_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L05_L04_GROUP = @GROUP_NAME4 " & _
                                        " AND GROUP_L05_L05_GROUP = @GROUP_NAME5 "
                            groupLevelStr = "GROUP_L05"
                        Case Is = 6
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name & _
                                       "_" & group5Name & _
                                       "_" & group6Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_06 WITH (NOLOCK)   " & _
                                        " WHERE (GROUP_L06_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L06_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L06_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L06_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L06_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L06_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L06_L04_GROUP = @GROUP_NAME4 " & _
                                        " AND GROUP_L06_L05_GROUP = @GROUP_NAME5 " & _
                                        " AND GROUP_L06_L06_GROUP = @GROUP_NAME6 "
                            groupLevelStr = "GROUP_L06"
                        Case Is = 7
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name & _
                                       "_" & group5Name & _
                                       "_" & group6Name & _
                                       "_" & group7Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_07 WITH (NOLOCK)   " & _
                                        " WHERE (GROUP_L07_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L07_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L07_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L07_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L07_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L07_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L07_L04_GROUP = @GROUP_NAME4 " & _
                                        " AND GROUP_L07_L05_GROUP = @GROUP_NAME5 " & _
                                        " AND GROUP_L07_L06_GROUP = @GROUP_NAME6 " & _
                                        " AND GROUP_L07_L07_GROUP = @GROUP_NAME7 "
                            groupLevelStr = "GROUP_L07"
                        Case Is = 8
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name & _
                                       "_" & group5Name & _
                                       "_" & group6Name & _
                                       "_" & group7Name & _
                                       "_" & group8Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_08 WITH (NOLOCK)   " & _
                                        " WHERE (GROUP_L08_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L08_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L08_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L08_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L08_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L08_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L08_L04_GROUP = @GROUP_NAME4 " & _
                                        " AND GROUP_L08_L05_GROUP = @GROUP_NAME5 " & _
                                        " AND GROUP_L08_L06_GROUP = @GROUP_NAME6 " & _
                                        " AND GROUP_L08_L07_GROUP = @GROUP_NAME7 " & _
                                        " AND GROUP_L08_L08_GROUP = @GROUP_NAME8 "
                            groupLevelStr = "GROUP_L08"
                        Case Is = 9
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name & _
                                       "_" & group5Name & _
                                       "_" & group6Name & _
                                       "_" & group7Name & _
                                       "_" & group8Name & _
                                       "_" & group9Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_09 WITH (NOLOCK)   " & _
                                        " WHERE (GROUP_L09_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L09_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L09_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L09_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L09_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L09_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L09_L04_GROUP = @GROUP_NAME4 " & _
                                        " AND GROUP_L09_L05_GROUP = @GROUP_NAME5 " & _
                                        " AND GROUP_L09_L06_GROUP = @GROUP_NAME6 " & _
                                        " AND GROUP_L09_L07_GROUP = @GROUP_NAME7 " & _
                                        " AND GROUP_L09_L08_GROUP = @GROUP_NAME8 " & _
                                        " AND GROUP_L09_L09_GROUP = @GROUP_NAME9 "
                            groupLevelStr = "GROUP_L09"
                    End Select

                    If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                        showProducts = CBool(HttpContext.Current.Cache.Item(cacheKey))
                    Else
                        Const SqlServer2005 As String = "TalentEBusinessDBConnectionString"
                        conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SqlServer2005).ConnectionString)
                        conTalent.Open()

                        Dim cmdSelect As SqlCommand = New SqlCommand(strSelect, conTalent)
                        With cmdSelect.Parameters
                            .Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = businessUnit
                            .Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partner
                            If groupLevel >= 1 Then .Add(New SqlParameter("@GROUP_NAME1", SqlDbType.Char, 20)).Value = group1Name
                            If groupLevel >= 2 Then .Add(New SqlParameter("@GROUP_NAME2", SqlDbType.Char, 20)).Value = group2Name
                            If groupLevel >= 3 Then .Add(New SqlParameter("@GROUP_NAME3", SqlDbType.Char, 20)).Value = group3Name
                            If groupLevel >= 4 Then .Add(New SqlParameter("@GROUP_NAME4", SqlDbType.Char, 20)).Value = group4Name
                            If groupLevel >= 5 Then .Add(New SqlParameter("@GROUP_NAME5", SqlDbType.Char, 20)).Value = group5Name
                            If groupLevel >= 6 Then .Add(New SqlParameter("@GROUP_NAME6", SqlDbType.Char, 20)).Value = group6Name
                            If groupLevel >= 7 Then .Add(New SqlParameter("@GROUP_NAME7", SqlDbType.Char, 20)).Value = group7Name
                            If groupLevel >= 8 Then .Add(New SqlParameter("@GROUP_NAME8", SqlDbType.Char, 20)).Value = group8Name
                            If groupLevel >= 9 Then .Add(New SqlParameter("@GROUP_NAME9", SqlDbType.Char, 20)).Value = group9Name
                        End With

                        Dim dtrGroup As SqlDataReader = cmdSelect.ExecuteReader()
                        If dtrGroup.HasRows Then
                            dtrGroup.Read()
                            showProducts = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrGroup(groupLevelStr & "_SHOW_PRODUCT_DISPLAY"))
                            dtrGroup.Close()
                        Else
                            '-----------------------------------------------
                            ' Get details for Page using *ALL partner
                            '-----------------------------------------------
                            dtrGroup.Close()

                            cmdSelect.Parameters.RemoveAt("@PARTNER")
                            cmdSelect.Parameters("@PARTNER").Value = Talent.Common.Utilities.GetAllString

                            If dtrGroup.HasRows Then
                                dtrGroup.Read()
                                showProducts = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrGroup(groupLevelStr & "_SHOW_PRODUCT_DISPLAY"))
                                dtrGroup.Close()
                            Else
                                '-----------------------------------------------
                                ' Get details for Page using *ALL business units
                                '-----------------------------------------------
                                dtrGroup.Close()
                                cmdSelect.Parameters.RemoveAt("@BUSINESS_UNIT")
                                cmdSelect.Parameters.RemoveAt("@PARTNER")
                                cmdSelect.Parameters("@PARTNER").Value = partner
                                cmdSelect.Parameters("@BUSINESS_UNIT").Value = Talent.Common.Utilities.GetAllString

                                dtrGroup = cmdSelect.ExecuteReader()

                                If dtrGroup.HasRows Then
                                    dtrGroup.Read()
                                    showProducts = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrGroup(groupLevelStr & "_SHOW_PRODUCT_DISPLAY"))
                                    dtrGroup.Close()
                                Else
                                    '-----------------------------------------------
                                    ' Get details for Page using *ALL partner and business units
                                    '-----------------------------------------------
                                    dtrGroup.Close()
                                    cmdSelect.Parameters.RemoveAt("@BUSINESS_UNIT")
                                    cmdSelect.Parameters.RemoveAt("@PARTNER")
                                    cmdSelect.Parameters("@PARTNER").Value = Talent.Common.Utilities.GetAllString
                                    cmdSelect.Parameters("@BUSINESS_UNIT").Value = Talent.Common.Utilities.GetAllString

                                    dtrGroup = cmdSelect.ExecuteReader()

                                End If

                            End If

                            If dtrGroup.HasRows Then
                                dtrGroup.Read()
                                showProducts = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrGroup(groupLevelStr & "_SHOW_PRODUCT_DISPLAY"))
                            End If
                            dtrGroup.Close()
                        End If
                    End If
                    HttpContext.Current.Cache.Add(cacheKey, showProducts, Nothing, Now.AddMinutes(30), timeSpan.Zero, CacheItemPriority.Normal, Nothing)
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)

                Catch ex As Exception
                    Const strError8 As String = "Error during database access"
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError8
                        .ErrorNumber = "TEAPUIL-06"
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
                    .ErrorNumber = "TEAPUIL-07"
                    .HasError = True
                End With
            End Try

            Utilities.TalentLogging.LoadTestLog("Utilities.vb", "ShowProductsDisplay", timeSpan)
            Return showProducts
        End Function

        Public Shared Function GetLocale() As String
            Dim locale As String = "en-GB"

            Return locale
        End Function

        Public Shared Function GetCurrentLanguageForDDLPopulation() As String
            Return String.Empty
        End Function

        Public Shared Function GetCurrentLanguage() As String
            Dim currentLang As String = String.Empty
            currentLang = "ENG"

            Return currentLang
        End Function

        Public Shared Function LoadXmlDocFromCache(ByVal cacheKey As String, ByVal docPath As String) As XmlDocument
            '-------------------------------------------------------------------------------------------
            '   Load XML doc from Cache if it's there, otherwise load from Xml file then add to cache

            Dim xmlDoc As New XmlDocument
            Dim businessUnit As String = TalentCache.GetBusinessUnit
            Dim partner As String = TalentCache.GetPartner(HttpContext.Current.Profile)

            ' The cache key needs to contain business unit and partner
            cacheKey = businessUnit & partner & cacheKey
            If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                xmlDoc.LoadXml(CType(HttpContext.Current.Cache.Item(cacheKey), String))
            Else
                '-------------------------------------------------------
                ' Reformat XML doc path to check for BU/PAR folder, then
                ' BU folder then Root
                '-------------------------------------------------------
                Dim absPath As String = HttpContext.Current.Server.MapPath(docPath)
                Dim pathArr As String() = absPath.Split("\")
                Dim index As Integer = 0
                Dim newPathBuPar As String = String.Empty
                Dim newPathBu As String = String.Empty

                Do While index < pathArr.Length
                    newPathBu = newPathBu & pathArr(index).ToString.Trim
                    If pathArr(index).ToString.Trim.ToUpper = "XMLDOCS" Then
                        newPathBu = newPathBu & "\" & TalentCache.GetBusinessUnit
                        newPathBuPar = newPathBu & "\" & TalentCache.GetPartner(HttpContext.Current.Profile)
                    End If
                    index += 1

                    If index < pathArr.Length Then
                        newPathBu = newPathBu & "\"
                    Else
                        '------------------------------------------------------------------------
                        ' End of array. Add last element to BU/PAR folder (BU folder already set)
                        '-----------------------------------------------------------------------
                        newPathBuPar = newPathBuPar & "\" & pathArr(pathArr.Length - 1).ToString.Trim
                    End If
                Loop
                '---------------------------------------
                ' Load XML doc from first path it exists
                '---------------------------------------
                If File.Exists(newPathBuPar) Then
                    xmlDoc.Load(newPathBuPar)
                Else
                    If File.Exists(newPathBu) Then
                        xmlDoc.Load(newPathBu)
                    Else
                        xmlDoc.Load(absPath)
                    End If
                End If
                HttpContext.Current.Cache.Insert(cacheKey, _
                       xmlDoc.InnerXml, _
                       Nothing, _
                       System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), _
                       Caching.Cache.NoSlidingExpiration)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)

            End If
            Return xmlDoc

        End Function

        Public Shared Function ShowProductsAsLists() As Boolean
            ' Declare this first! Used for Logging function duration
            Dim timeSpan As TimeSpan = Now.TimeOfDay

            '-----------------------------------------------------------------------
            '   CheckForEmptyGroup - Check if current level only contains empty groups
            '                        If so redirect to product list level
            '-----------------------------------------------------------------------
            Dim businessUnit As String = TalentCache.GetBusinessUnit()
            Dim partner As String = TalentCache.GetPartner(HttpContext.Current.Profile)
            Dim showProdsAsList As Boolean = True
            Dim err As New ErrorObj
            Dim conTalent As SqlConnection = Nothing
            Dim currentPage As String = GetCurrentPageName()
            Dim groupLevel As Integer = 0

            Dim group1Name As String = "", _
                group2Name As String = "", _
                group3Name As String = "", _
                group4Name As String = "", _
                group5Name As String = "", _
                group6Name As String = "", _
                group7Name As String = "", _
                group8Name As String = "", _
                group9Name As String = ""

            Try
                If Not HttpContext.Current.Request("group1") Is Nothing Then group1Name = HttpContext.Current.Request("group1")
                If Not HttpContext.Current.Request("group2") Is Nothing Then group2Name = HttpContext.Current.Request("group2")
                If Not HttpContext.Current.Request("group3") Is Nothing Then group3Name = HttpContext.Current.Request("group3")
                If Not HttpContext.Current.Request("group4") Is Nothing Then group4Name = HttpContext.Current.Request("group4")
                If Not HttpContext.Current.Request("group5") Is Nothing Then group5Name = HttpContext.Current.Request("group5")
                If Not HttpContext.Current.Request("group6") Is Nothing Then group6Name = HttpContext.Current.Request("group6")
                If Not HttpContext.Current.Request("group7") Is Nothing Then group7Name = HttpContext.Current.Request("group7")
                If Not HttpContext.Current.Request("group8") Is Nothing Then group8Name = HttpContext.Current.Request("group8")
                If Not HttpContext.Current.Request("group9") Is Nothing Then group9Name = HttpContext.Current.Request("group9")
            Catch ex As Exception
            End Try

            While (groupLevel = 0) AndAlso _
                         currentPage <> "browse01.aspx"

                Select Case currentPage
                    Case Is = "browse02.aspx"
                        currentPage = "browse01.aspx"
                        groupLevel = 1
                    Case Is = "browse03.aspx"
                        currentPage = "browse02.aspx"
                        groupLevel = 2
                    Case Is = "browse04.aspx"
                        currentPage = "browse03.aspx"
                        groupLevel = 3
                    Case Is = "browse05.aspx"
                        currentPage = "browse04.aspx"
                        groupLevel = 4
                    Case Is = "browse06.aspx"
                        currentPage = "browse05.aspx"
                        groupLevel = 5
                    Case Is = "browse07.aspx"
                        currentPage = "browse06.aspx"
                        groupLevel = 6
                    Case Is = "browse08.aspx"
                        currentPage = "browse07.aspx"
                        groupLevel = 7
                    Case Is = "browse09.aspx"
                        currentPage = "browse08.aspx"
                        groupLevel = 8
                    Case Is = "browse10.aspx"
                        currentPage = "browse09.aspx"
                        groupLevel = 9
                End Select
            End While

            If Not err.HasError Then
                Try

                    'If we are to override the display as group value with the value from the group level 01
                    'table then set the group level to 1 and the code will only check for level 1 settings
                    Dim myDefaults As New ECommerceModuleDefaults
                    Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
                    If def.Override_Show_Products_As_List_Using_L01_Value Then
                        groupLevel = 1
                    End If

                    Dim cacheKey As String = "SHOWPRODUCTSASLIST_" & TalentCache.GetBusinessUnit & "_" & TalentCache.GetPartner(HttpContext.Current.Profile)

                    '-----------------------------------------------
                    ' Get details for Page using current partner
                    '-----------------------------------------------
                    Dim strSelect As String = "", _
                        groupLevelStr As String = "GROUP_L01"
                    Select Case groupLevel
                        Case Is = 1
                            cacheKey += "_" & group1Name
                            strSelect = " SELECT * FROM TBL_GROUP_LEVEL_01 WITH (NOLOCK)  " & _
                                        " WHERE (GROUP_L01_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L01_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L01_PARTNER = @PARTNER " & _
                                        " AND GROUP_L01_L01_GROUP = @GROUP_NAME1"
                            groupLevelStr = "GROUP_L01"
                        Case Is = 2
                            cacheKey += "_" & group1Name & _
                                        "_" & group2Name
                            strSelect = " SELECT * FROM TBL_GROUP_LEVEL_02 WITH (NOLOCK)  " & _
                                        " WHERE (GROUP_L02_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L02_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L02_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L02_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L02_L02_GROUP = @GROUP_NAME2 "
                            groupLevelStr = "GROUP_L02"
                        Case Is = 3
                            cacheKey += "_" & group1Name & _
                                        "_" & group2Name & _
                                        "_" & group3Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_03 WITH (NOLOCK)   " & _
                                        " WHERE (GROUP_L03_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L03_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L03_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L03_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L03_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L03_L03_GROUP = @GROUP_NAME3 "
                            groupLevelStr = "GROUP_L03"
                        Case Is = 4
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_04 WITH (NOLOCK)   " & _
                                        " WHERE (GROUP_L04_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L04_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L04_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L04_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L04_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L04_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L04_L04_GROUP = @GROUP_NAME4 "
                            groupLevelStr = "GROUP_L04"
                        Case Is = 5
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name & _
                                       "_" & group5Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_05 WITH (NOLOCK)   " & _
                                        " WHERE (GROUP_L05_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L05_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L05_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L05_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L05_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L05_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L05_L04_GROUP = @GROUP_NAME4 " & _
                                        " AND GROUP_L05_L05_GROUP = @GROUP_NAME5 "
                            groupLevelStr = "GROUP_L05"
                        Case Is = 6
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name & _
                                       "_" & group5Name & _
                                       "_" & group6Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_06 WITH (NOLOCK)   " & _
                                        " WHERE (GROUP_L06_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L06_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L06_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L06_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L06_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L06_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L06_L04_GROUP = @GROUP_NAME4 " & _
                                        " AND GROUP_L06_L05_GROUP = @GROUP_NAME5 " & _
                                        " AND GROUP_L06_L06_GROUP = @GROUP_NAME6 "
                            groupLevelStr = "GROUP_L06"
                        Case Is = 7
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name & _
                                       "_" & group5Name & _
                                       "_" & group6Name & _
                                       "_" & group7Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_07 WITH (NOLOCK)   " & _
                                        " WHERE (GROUP_L07_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L07_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L07_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L07_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L07_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L07_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L07_L04_GROUP = @GROUP_NAME4 " & _
                                        " AND GROUP_L07_L05_GROUP = @GROUP_NAME5 " & _
                                        " AND GROUP_L07_L06_GROUP = @GROUP_NAME6 " & _
                                        " AND GROUP_L07_L07_GROUP = @GROUP_NAME7 "
                            groupLevelStr = "GROUP_L07"
                        Case Is = 8
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name & _
                                       "_" & group5Name & _
                                       "_" & group6Name & _
                                       "_" & group7Name & _
                                       "_" & group8Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_08 WITH (NOLOCK)   " & _
                                        " WHERE (GROUP_L08_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L08_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L08_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L08_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L08_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L08_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L08_L04_GROUP = @GROUP_NAME4 " & _
                                        " AND GROUP_L08_L05_GROUP = @GROUP_NAME5 " & _
                                        " AND GROUP_L08_L06_GROUP = @GROUP_NAME6 " & _
                                        " AND GROUP_L08_L07_GROUP = @GROUP_NAME7 " & _
                                        " AND GROUP_L08_L08_GROUP = @GROUP_NAME8 "
                            groupLevelStr = "GROUP_L08"
                        Case Is = 9
                            cacheKey += "_" & group1Name & _
                                       "_" & group2Name & _
                                       "_" & group3Name & _
                                       "_" & group4Name & _
                                       "_" & group5Name & _
                                       "_" & group6Name & _
                                       "_" & group7Name & _
                                       "_" & group8Name & _
                                       "_" & group9Name
                            strSelect = "SELECT * FROM TBL_GROUP_LEVEL_09 WITH (NOLOCK)   " & _
                                        " WHERE (GROUP_L09_BUSINESS_UNIT = @BUSINESS_UNIT  " & _
                                        " OR GROUP_L09_BUSINESS_UNIT = '*ALL') " & _
                                        " AND GROUP_L09_PARTNER = @PARTNER  " & _
                                        " AND GROUP_L09_L01_GROUP = @GROUP_NAME1 " & _
                                        " AND GROUP_L09_L02_GROUP = @GROUP_NAME2 " & _
                                        " AND GROUP_L09_L03_GROUP = @GROUP_NAME3 " & _
                                        " AND GROUP_L09_L04_GROUP = @GROUP_NAME4 " & _
                                        " AND GROUP_L09_L05_GROUP = @GROUP_NAME5 " & _
                                        " AND GROUP_L09_L06_GROUP = @GROUP_NAME6 " & _
                                        " AND GROUP_L09_L07_GROUP = @GROUP_NAME7 " & _
                                        " AND GROUP_L09_L08_GROUP = @GROUP_NAME8 " & _
                                        " AND GROUP_L09_L09_GROUP = @GROUP_NAME9 "
                            groupLevelStr = "GROUP_L09"
                    End Select

                    If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                        showProdsAsList = CBool(HttpContext.Current.Cache.Item(cacheKey))
                    Else
                        Const SqlServer2005 As String = "TalentEBusinessDBConnectionString"
                        conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SqlServer2005).ConnectionString)
                        conTalent.Open()

                        Dim cmdSelect As SqlCommand = New SqlCommand(strSelect, conTalent)
                        With cmdSelect.Parameters
                            .Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = businessUnit
                            .Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partner
                            If groupLevel >= 1 Then .Add(New SqlParameter("@GROUP_NAME1", SqlDbType.Char, 20)).Value = group1Name
                            If groupLevel >= 2 Then .Add(New SqlParameter("@GROUP_NAME2", SqlDbType.Char, 20)).Value = group2Name
                            If groupLevel >= 3 Then .Add(New SqlParameter("@GROUP_NAME3", SqlDbType.Char, 20)).Value = group3Name
                            If groupLevel >= 4 Then .Add(New SqlParameter("@GROUP_NAME4", SqlDbType.Char, 20)).Value = group4Name
                            If groupLevel >= 5 Then .Add(New SqlParameter("@GROUP_NAME5", SqlDbType.Char, 20)).Value = group5Name
                            If groupLevel >= 6 Then .Add(New SqlParameter("@GROUP_NAME6", SqlDbType.Char, 20)).Value = group6Name
                            If groupLevel >= 7 Then .Add(New SqlParameter("@GROUP_NAME7", SqlDbType.Char, 20)).Value = group7Name
                            If groupLevel >= 8 Then .Add(New SqlParameter("@GROUP_NAME8", SqlDbType.Char, 20)).Value = group8Name
                            If groupLevel >= 9 Then .Add(New SqlParameter("@GROUP_NAME9", SqlDbType.Char, 20)).Value = group9Name
                        End With

                        Dim dtrGroup As SqlDataReader = cmdSelect.ExecuteReader()
                        If dtrGroup.HasRows Then
                            dtrGroup.Read()
                            showProdsAsList = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrGroup(groupLevelStr & "_SHOW_PRODUCTS_AS_LIST"))
                            dtrGroup.Close()
                        Else
                            '-----------------------------------------------
                            ' Get details for Page using *ALL partner
                            '-----------------------------------------------
                            dtrGroup.Close()
                            '  cmdSelect.Parameters.RemoveAt("@PARTNER")
                            cmdSelect.Parameters("@PARTNER").Value = Talent.Common.Utilities.GetAllString
                            dtrGroup = cmdSelect.ExecuteReader()
                            If dtrGroup.HasRows Then
                                dtrGroup.Read()
                                showProdsAsList = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrGroup(groupLevelStr & "_SHOW_PRODUCTS_AS_LIST"))
                                dtrGroup.Close()
                            Else
                                '-----------------------------------------------
                                ' Get details for Page using *ALL business units
                                '-----------------------------------------------
                                dtrGroup.Close()
                                cmdSelect.Parameters.RemoveAt("@BUSINESS_UNIT")
                                cmdSelect.Parameters.RemoveAt("@PARTNER")
                                cmdSelect.Parameters("@PARTNER").Value = partner
                                cmdSelect.Parameters("@BUSINESS_UNIT").Value = Talent.Common.Utilities.GetAllString

                                dtrGroup = cmdSelect.ExecuteReader()

                                If dtrGroup.HasRows Then
                                    dtrGroup.Read()
                                    showProdsAsList = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrGroup(groupLevelStr & "_SHOW_PRODUCTS_AS_LIST"))
                                    dtrGroup.Close()
                                Else
                                    '-----------------------------------------------
                                    ' Get details for Page using *ALL partner and business units
                                    '-----------------------------------------------
                                    dtrGroup.Close()
                                    cmdSelect.Parameters.RemoveAt("@BUSINESS_UNIT")
                                    cmdSelect.Parameters.RemoveAt("@PARTNER")
                                    cmdSelect.Parameters("@PARTNER").Value = Talent.Common.Utilities.GetAllString
                                    cmdSelect.Parameters("@BUSINESS_UNIT").Value = Talent.Common.Utilities.GetAllString

                                    dtrGroup = cmdSelect.ExecuteReader()

                                    If dtrGroup.HasRows Then
                                        dtrGroup.Read()
                                        showProdsAsList = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrGroup(groupLevelStr & "_SHOW_PRODUCTS_AS_LIST"))
                                    End If
                                End If

                            End If
                            Try
                                dtrGroup.Close()
                            Catch ex As Exception
                            End Try

                        End If
                    End If
                    HttpContext.Current.Cache.Add(cacheKey, showProdsAsList, Nothing, Now.AddMinutes(30), timeSpan.Zero, CacheItemPriority.Normal, Nothing)
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)

                    'showProdsAsList = Utilities.CheckForDBNull_Boolean_DefaultTrue(dtrGroup2(groupLevelStr & "_SHOW_PRODUCTS_AS_LIST"))
                Catch ex As Exception
                    Const strError8 As String = "Error during database access"
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError8
                        .ErrorNumber = "TEAPUIL-06"
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
                    .ErrorNumber = "TEAPUIL-07"
                    .HasError = True
                End With
            End Try

            Utilities.TalentLogging.LoadTestLog("Utilities.vb", "ShowProductsAsLists", timeSpan)
            Return showProdsAsList
        End Function

        Public Shared Function GroupNavigationUsingAll() As Boolean
            '---------------------------
            ' Check if it's in the cache
            '---------------------------
            Dim useAll As Boolean = False
            Dim err As New ErrorObj

            Dim conTalent As SqlConnection = Nothing

            Dim businessUnit As String = TalentCache.GetBusinessUnit
            Dim partner As String = TalentCache.GetPartner(HttpContext.Current.Profile)
            Dim cacheKey As String = "GROUPNAVIGATIONUSINGALL" & _
                                    Talent.Common.Utilities.FixStringLength(businessUnit, 50) & _
                                    Talent.Common.Utilities.FixStringLength(partner, 50)

            If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                useAll = CType(HttpContext.Current.Cache.Item(cacheKey), Boolean)
            Else
                '---------------
                ' Get it from DB
                '---------------
                Try
                    Const SqlServer2005 As String = "TalentEBusinessDBConnectionString"
                    conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SqlServer2005).ConnectionString)
                    conTalent.Open()
                Catch ex As Exception
                    Const strError1 As String = "Could not establish connection to the database"
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError1
                        .ErrorNumber = "TEAPUIL-01"
                        .HasError = True
                    End With
                End Try

                If Not err.HasError Then
                    Try
                        '------------------
                        '
                        '------------------
                        Const strSelect As String = "SELECT TOP 1 * FROM TBL_GROUP_LEVEL_01 WITH (NOLOCK)  WHERE " & _
                                                        " (GROUP_L01_BUSINESS_UNIT = @BUSINESS_UNIT OR GROUP_L01_BUSINESS_UNIT = '*ALL')" & _
                                                        "AND GROUP_L01_PARTNER = @PARTNER"
                        Dim cmdSelect As SqlCommand = New SqlCommand(strSelect, conTalent)

                        cmdSelect.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = businessUnit
                        cmdSelect.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partner
                        Dim dtrGroup As SqlDataReader = cmdSelect.ExecuteReader()
                        If dtrGroup.HasRows Then
                            useAll = False
                        Else
                            useAll = True
                        End If
                        dtrGroup.Close()
                    Catch ex As Exception
                        Const strError8 As String = "Error during database access"
                        With err
                            .ErrorMessage = ex.Message
                            .ErrorStatus = strError8
                            .ErrorNumber = "TEAPUIL-06"
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
                        .ErrorNumber = "TEAPUIL-07"
                        .HasError = True
                    End With
                End Try

                HttpContext.Current.Cache.Insert(cacheKey, _
                    useAll, _
                    Nothing, _
                    System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), _
                    Caching.Cache.NoSlidingExpiration)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)


            End If
            Return useAll
        End Function

        Public Shared Function IsDBNull(ByVal obj As Object) As Boolean
            If obj.Equals(DBNull.Value) Then Return True Else Return False
        End Function

        Public Shared Function GetStoredProcedureGroup() As String
            '--------------------------------------------------------------------
            ' Return stored procedure group grom DB if set, and web.config if not
            '--------------------------------------------------------------------
            Dim storedProcedureGroup As String = String.Empty

            Dim myDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
            storedProcedureGroup = def.StoredProcedureGroup
            If String.IsNullOrEmpty(storedProcedureGroup) Then
                storedProcedureGroup = ConfigurationManager.AppSettings("DefaultStoredProcedureGroup")
            End If
            Return storedProcedureGroup

        End Function

        Public Shared Function GetCustomerDestinationDatabase() As String
            '------------------------------------
            ' Return destination database from DB
            '------------------------------------
            Dim myDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
            Return def.CustomerDestinationDatabase
        End Function

        Public Shared Function GetHtmlIncludePathAbsolute() As String
            '------------------------------------
            ' Return  Html Include Path from DB
            '------------------------------------
            Dim myDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
            Return def.HtmlPathAbsolute
        End Function

        Public Shared Function GetHtmlIncludePathRelative() As String
            '------------------------------------
            ' Return  Html Include Path from DB
            '------------------------------------
            Dim myDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
            Return def.HtmlIncludePathRelative
        End Function

        Public Shared Function GetImagePathAbsolute() As String
            '------------------------------------
            Dim myDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
            Return def.ImagePathAbsolute
        End Function

        Public Shared Function GetImagePathVirtual() As String
            '------------------------------------
            Dim myDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
            Return def.ImagePathVirtual
        End Function

        Public Shared Function GetImageSslPathVirtual() As String
            '------------------------------------
            Dim myDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
            Return def.ImageSslPathVirtual
        End Function

#Region "Check For DB Null Functions"

        Public Shared Function CheckForDBNull_Long(ByVal obj As Object) As Long
            Try
                If obj.Equals(DBNull.Value) OrElse obj.Equals(String.Empty) Then Return 0 Else Return CLng(obj)
            Catch ex As Exception
                Return 0
            End Try
        End Function
        Public Shared Function CheckForDBNull_Int(ByVal obj As Object) As Integer
            Try
                If obj.Equals(DBNull.Value) OrElse obj.Equals(String.Empty) Then Return 0 Else Return CInt(obj)
            Catch ex As Exception
                Return 0
            End Try
        End Function

        Public Shared Function CheckForDBNull_String(ByVal obj As Object) As String
            Try
                If obj.Equals(DBNull.Value) OrElse obj.Equals(String.Empty) Then Return String.Empty Else Return CStr(obj)
            Catch ex As Exception
                Return String.Empty
            End Try
        End Function

        Public Shared Function CheckForDBNull_Date(ByVal obj As Object) As Date
            Try
                If obj.Equals(DBNull.Value) Then Return Now Else Return CDate(obj)
            Catch ex As Exception
                Return Date.MinValue
            End Try
        End Function

        Public Shared Function CheckForDBNull_Decimal(ByVal obj As Object) As Decimal
            Try
                If obj.Equals(DBNull.Value) OrElse obj.Equals(String.Empty) Then Return 0 Else Return CDec(obj)
            Catch ex As Exception
                Return 0
            End Try
        End Function
        Public Shared Function CheckForDBNull_StringBoolean_DefaultFalse(ByVal obj As Object) As String
            Try
                If obj.Equals(DBNull.Value) OrElse obj.Equals(String.Empty) Then Return "false" Else Return CStr(obj).ToLower()
            Catch ex As Exception
                Return "false"
            End Try
        End Function
        Public Shared Function CheckForDBNull_Boolean_DefaultFalse(ByVal obj As Object) As Boolean
            Try
                If obj.Equals(DBNull.Value) OrElse obj.Equals(String.Empty) Then Return False Else Return CBool(obj)
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Shared Function CheckForDBNull_Boolean_DefaultTrue(ByVal obj As Object) As Boolean
            Try
                If obj.Equals(DBNull.Value) OrElse obj.Equals(String.Empty) Then Return True Else Return CBool(obj)
            Catch ex As Exception
                Return True
            End Try
        End Function

        Public Shared Function CheckForDBNullOrBlank_Boolean_DefaultFalse(ByVal obj As Object) As Boolean
            Try
                If obj.Equals(DBNull.Value) Then
                    Return False
                Else
                    If obj.ToString.Equals(String.Empty) Then
                        Return False
                    Else
                        Return CBool(obj)
                    End If
                End If
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Shared Function CheckForDBNullOrBlank_Boolean_DefaultTrue(ByVal obj As Object) As Boolean
            Try
                If obj.Equals(DBNull.Value) Then
                    Return True
                Else
                    If obj.ToString.Equals(String.Empty) Then
                        Return True
                    Else
                        Return CBool(obj)
                    End If
                End If
            Catch ex As Exception
                Return True
            End Try
        End Function

        Public Shared Function CheckIsDBNull(ByVal obj As Object) As Boolean
            Try
                If obj.Equals(DBNull.Value) Then Return True Else Return False
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Shared Function CheckForDBNull(ByVal value As Object) As Object
            Try
                If value.Equals(DBNull.Value) Then
                    Return Nothing
                Else
                    Return value
                End If
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Shared Function TrimWhenNotNull(ByVal inString As String) As String
            Try
                If String.IsNullOrWhiteSpace(inString) Then
                    Return String.Empty
                Else
                    Return inString.Trim
                End If
            Catch ex As Exception
                Return String.Empty
            End Try
        End Function

        Public Shared Function CheckForDBNull(ByVal value As Object, ByVal defaultReturnObject As Object) As Object
            Try
                If value.Equals(DBNull.Value) Then
                    Return defaultReturnObject
                Else
                    Return value
                End If
            Catch ex As Exception
                Return value
            End Try
        End Function

#End Region

        Public Shared Function GetCurrencySymbol() As String
            Return "�"
        End Function

        Public Shared Function RoundToValue(ByVal nValue As Object, ByVal nCeiling As Double, Optional ByVal RoundUp As Boolean = True) As Double

            Dim tmp As Integer
            Dim tmpVal As Object
            RoundToValue = 0.0
            If Not IsNumeric(nValue) Then Exit Function

            If CDec(nValue) = Decimal.Round(CDec(nValue), 2) Then Return CDbl(nValue)
            nValue = CDbl(nValue)

            'Round up to a whole integer -
            'Any decimal value will force a round to the next integer.
            'i.e. 0.01 = 1 or 0.8 = 1

            tmpVal = ((nValue / nCeiling) + (-0.5 + (RoundUp And 1)))
            tmp = Fix(tmpVal)
            tmpVal = CInt((tmpVal - tmp) * 10 ^ 0)
            nValue = tmp + tmpVal / 10 ^ 0

            'Multiply by ceiling value to set RoundtoValue
            RoundToValue = nValue * nCeiling

        End Function

        Public Shared Function GetHtmlFromFile(ByVal path As String) As String

         
            Dim htmlString As String = String.Empty

            '-----------------------
            ' Check if it's in cache
            '-----------------------
            Dim cacheKey As String = "Ecommerce_Utilities_GetHtmlFromFile" & path

            If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                htmlString = CType(HttpContext.Current.Cache.Item(cacheKey), String)
                Return htmlString
            Else
                Dim htmlRootPath As String = GetHtmlIncludePathRelative()
                Dim htmlPathAbsolute As String = GetHtmlIncludePathAbsolute()
                Dim strPath As String = String.Empty
                If Not String.IsNullOrEmpty(htmlPathAbsolute) Then If Not htmlPathAbsolute.EndsWith("\") Then htmlPathAbsolute += "\"
                If Not htmlRootPath.EndsWith("/") Then htmlRootPath += "/"
                '-------------------------------------------------------
                ' Check if set up with absolute paths instead of virtual
                '-------------------------------------------------------
                If Not String.IsNullOrEmpty(htmlPathAbsolute) Then
                    strPath = (htmlPathAbsolute.ToString & path)
                    strPath = ((strPath.Replace("/", "\")).Replace("\\", "\"))
                    Dim sr As StreamReader
                    Try
                        sr = File.OpenText(strPath)
                        htmlString = sr.ReadToEnd
                        sr.Close()
                        sr.Dispose()
                    Catch ex As Exception
                    End Try
                Else
                    strPath = HttpContext.Current.Server.MapPath(htmlRootPath & "/" & path)
                    Try
                        Dim sr As StreamReader = File.OpenText(strPath)
                        htmlString = sr.ReadToEnd
                        sr.Close()
                        sr.Dispose()
                    Catch ex As Exception
                    End Try
                End If
                '-----------------------
                ' Put in cache
                '-----------------------
                HttpContext.Current.Cache.Insert(cacheKey,
                        htmlString,
                        Nothing,
                        System.DateTime.Now.AddMinutes(30),
                        Caching.Cache.NoSlidingExpiration)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)

                Return htmlString
            End If
        End Function

        Public Shared Function GetCSSContentFromFile(ByVal path As String) As String
            Dim cssString As String = String.Empty
            Dim cacheKey As String = "Ecommerce_Utilities_GetCSSContentFromFile" & path
            If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                cssString = CType(HttpContext.Current.Cache.Item(cacheKey), String)
                Return cssString
            Else
                Dim htmlRootPath As String = GetHtmlIncludePathRelative()
                Dim htmlPathAbsolute As String = GetHtmlIncludePathAbsolute()
                Dim strPath As String = String.Empty
                If Not String.IsNullOrEmpty(htmlPathAbsolute) Then If Not htmlPathAbsolute.EndsWith("\") Then htmlPathAbsolute += "\"
                If Not htmlRootPath.EndsWith("/") Then htmlRootPath += "/"
                '-------------------------------------------------------
                ' Check if set up with absolute paths instead of virtual
                '-------------------------------------------------------
                If Not String.IsNullOrEmpty(htmlPathAbsolute) Then
                    strPath = (htmlPathAbsolute.ToString & path)
                    strPath = ((strPath.Replace("/", "\")).Replace("\\", "\"))
                    Dim sr As StreamReader
                    Try
                        sr = File.OpenText(strPath)
                        cssString = sr.ReadToEnd
                        sr.Close()
                        sr.Dispose()
                    Catch ex As Exception
                    End Try
                Else
                    strPath = HttpContext.Current.Server.MapPath(htmlRootPath & "/" & path)
                    Try
                        Dim sr As StreamReader = File.OpenText(strPath)
                        cssString = sr.ReadToEnd
                        sr.Close()
                        sr.Dispose()
                    Catch ex As Exception
                    End Try
                End If
                '-----------------------
                ' Put in cache
                '-----------------------
                HttpContext.Current.Cache.Insert(cacheKey,
                        cssString,
                        Nothing,
                        System.DateTime.Now.AddMinutes(30),
                        Caching.Cache.NoSlidingExpiration)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                Return cssString
            End If
        End Function

        Public Shared Function DoesHtmlFileExists(ByVal path As String) As Boolean
            Dim strPath As String = String.Empty

            Dim htmlPathAbsolute As String = GetHtmlIncludePathAbsolute()

            If Not String.IsNullOrEmpty(htmlPathAbsolute) Then

                If Not htmlPathAbsolute.EndsWith("\") Then
                    htmlPathAbsolute += "\"
                End If

                strPath = htmlPathAbsolute.ToString & path
                strPath = strPath.Replace("/", "\").Replace("\\", "\")
            Else

                Dim htmlRootPath As String = GetHtmlIncludePathRelative()
                If String.IsNullOrEmpty(htmlRootPath) Then
                    htmlRootPath = ConfigurationManager.AppSettings("htmlIncludeRootPath")
                End If
                If Not htmlRootPath.EndsWith("/") Then
                    htmlRootPath += "/"
                End If

                strPath = HttpContext.Current.Server.MapPath(htmlRootPath & path)
            End If

            Return File.Exists(strPath)

        End Function

        Public Shared Function GetCurrentApplicationUrl() As String

            Dim relativeUrl As String = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.TrimStart("~")
            Dim currentUrl As String = HttpContext.Current.Request.Url.AbsolutePath.Split("?")(0)

            Return currentUrl.Remove(currentUrl.Length - relativeUrl.Length, relativeUrl.Length)
        End Function

        Public Shared Function GetFreeDeliveryValue(ByVal partner As String) As Decimal
            Dim pricelistheaderTA As New TalentBasketDatasetTableAdapters.tbl_price_list_headerTableAdapter
            Dim pricelistheaders As TalentBasketDataset.tbl_price_list_headerDataTable = pricelistheaderTA.Get_PriceList_Header_By_BU_Partner(TalentCache.GetBusinessUnit, partner)
            If pricelistheaders.Rows.Count < 1 Then
                pricelistheaders = pricelistheaderTA.Get_PriceList_Header_By_BU_Partner(TalentCache.GetBusinessUnit, Talent.Common.Utilities.GetAllString)
            End If
            If pricelistheaders.Rows.Count > 0 Then
                Dim priceListHeader As TalentBasketDataset.tbl_price_list_headerRow = pricelistheaders.Rows(0)
                Return priceListHeader.FREE_DELIVERY_VALUE
            Else
                Return 0
            End If

        End Function

        Public Shared Function Serialize(ByVal objectToSerialize As Object) As Byte()
            Dim ms As New System.IO.MemoryStream
            Dim bf As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            bf.Serialize(ms, objectToSerialize)

            Dim enc As System.Text.Encoding = New System.Text.ASCIIEncoding
            Dim bytes(ms.Length) As Byte
            bytes = ms.ToArray

            Return bytes
        End Function

        Public Shared Function DeSerialize(ByVal objectByteArray As Byte(), ByVal ObjectToPopulate As Object) As Object
            Dim ms As New System.IO.MemoryStream(objectByteArray)
            Dim bf As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            ObjectToPopulate = bf.Deserialize(ms)
            Return ObjectToPopulate
        End Function

        Public Shared Sub SerializeObject(ByVal ObjectToStore As Object, _
                                            ByVal objectType As Type, _
                                            ByVal BusinessUnit As String, _
                                            ByVal Partner As String, _
                                            ByVal LoginID As String, _
                                            ByVal Origin As String, _
                                            Optional ByVal UniqueID As String = "")

            Dim serializer As New SerializedObjectsDataSetTableAdapters.tbl_serialized_objectsTableAdapter
            Dim serializedObject As Byte() = Serialize(ObjectToStore)

            Select Case objectType.Name
                Case Is = "TalentOrder"
                    Dim orders As SerializedObjectsDataSet.tbl_serialized_objectsDataTable
                    orders = serializer.Get_Order_Unprocessed(objectType.Name, _
                                                                Origin, _
                                                                BusinessUnit, _
                                                                Partner, _
                                                                LoginID, _
                                                                UniqueID)
                    If orders.Rows.Count > 0 Then
                        serializer.Update_Order_Object(serializedObject, _
                                                        Now, _
                                                        objectType.Name, _
                                                        Origin, _
                                                        BusinessUnit, _
                                                        Partner, _
                                                        LoginID, _
                                                        UniqueID)
                    Else
                        serializer.AddSerializedOrder(objectType.Name, _
                                                        Origin, _
                                                        BusinessUnit, _
                                                        Partner, _
                                                        LoginID, _
                                                        UniqueID, _
                                                        serializedObject, _
                                                        Now)

                    End If

                Case Is = "TalentCustomer"
                    Dim customers As SerializedObjectsDataSet.tbl_serialized_objectsDataTable
                    customers = serializer.Get_Customer_Unprocessed(objectType.Name, _
                                                                    Origin, _
                                                                    BusinessUnit, _
                                                                    Partner, _
                                                                    LoginID)
                    If customers.Rows.Count > 0 Then
                        serializer.Update_Customer_Object(serializedObject, _
                                                            Now, _
                                                            objectType.Name, _
                                                            Origin, _
                                                            BusinessUnit, _
                                                            Partner, _
                                                            LoginID)
                    Else
                        serializer.AddSerializedCustomer(objectType.Name, _
                                                        Origin, _
                                                        BusinessUnit, _
                                                        Partner, _
                                                        LoginID, _
                                                        serializedObject, _
                                                        Now)
                    End If

                Case Else
                    Dim serialized As SerializedObjectsDataSet.tbl_serialized_objectsDataTable
                    serialized = serializer.GetBy_ObjType_Origin_BU_Partner_Login_UniqueID_Unprocessed(objectType.Name, _
                                                                                                        Origin, _
                                                                                                        BusinessUnit, _
                                                                                                        Partner, _
                                                                                                        LoginID, _
                                                                                                        UniqueID)

                    If serialized.Rows.Count > 0 Then
                        serializer.Update_Object(serializedObject, _
                                                    Now, _
                                                    objectType.Name, _
                                                    Origin, _
                                                    BusinessUnit, _
                                                    Partner, _
                                                    LoginID, _
                                                    UniqueID)

                    Else
                        serializer.AddSerializedObject(objectType.Name, _
                                                    Origin, _
                                                    BusinessUnit, _
                                                    Partner, _
                                                    LoginID, _
                                                    UniqueID, _
                                                    serializedObject, _
                                                    Now, _
                                                    False, _
                                                    Nothing, _
                                                    0)

                    End If

            End Select

        End Sub

        Public Shared Sub SerializeObject(ByVal ObjectToStore As Object, _
                                            ByVal objectType As Type, _
                                            ByVal BusinessUnit As String, _
                                            ByVal Partner As String, _
                                            ByVal LoginID As String, _
                                            ByVal Origin As String, _
                                            ByVal sendEmail As Boolean, _
                                            ByVal context As Web.HttpContext, _
                                            Optional ByVal UniqueID As String = "")

            '
            ' Call the Serialize Object Method
            '
            SerializeObject(ObjectToStore, objectType, BusinessUnit, Partner, LoginID, Origin, UniqueID)

            If sendEmail Then

                '
                ' Create Global Page WebResoursce to retrieve email defaults
                '
                Dim languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
                Dim wfr As New Talent.Common.WebFormResource

                With wfr
                    .BusinessUnit = TalentCache.GetBusinessUnit()
                    .PageCode = "GLOBAL"
                    .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                    .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                    .KeyCode = "GLOBAL"
                End With

                Dim message As String
                message = wfr.Content("SerializeEmailHeaderText", languageCode, True) & vbCrLf & _
                                        "Login ID: " & LoginID & vbCrLf & _
                                        "ID: " & UniqueID & vbCrLf & _
                                        "Date: " & (CType(Now, Date)).ToString

                Talent.Common.Utilities.SMTP = ConfigurationManager.AppSettings("EmailSMTP").ToString.Trim
                Talent.Common.Utilities.SMTPPortNumber = Utilities.GetSMTPPortNumber
                Talent.Common.Utilities.Email_Send(wfr.Attribute("BackEndFailureEmailFrom"), _
                                                   wfr.Attribute("BackEndFailureEmailTo"), _
                                                   wfr.Content("SerializeEmailSubject", languageCode, True), _
                                                   message)

            End If
        End Sub

        Public Shared Function DeserializeObject(ByVal ObjectToPopulate As Object, _
                                                    ByVal objectType As Type, _
                                                    ByVal BusinessUnit As String, _
                                                    ByVal Partner As String, _
                                                    ByVal LoginID As String, _
                                                    ByVal Origin As String, _
                                                    Optional ByVal UniqueID As String = "") As Object

            Dim serializer As New SerializedObjectsDataSetTableAdapters.tbl_serialized_objectsTableAdapter
            Dim serializedObjects As New SerializedObjectsDataSet.tbl_serialized_objectsDataTable
            Dim serializedObject As SerializedObjectsDataSet.tbl_serialized_objectsRow

            Select Case objectType.Name
                Case Is = "TalentOrder"
                    ObjectToPopulate = New TalentOrder
                    serializedObjects = serializer.Get_Order_Unprocessed(objectType.Name, _
                                                                            Origin, _
                                                                            BusinessUnit, _
                                                                            Partner, _
                                                                            LoginID, _
                                                                            UniqueID)
                Case Is = "TalentCustomer"
                    ObjectToPopulate = New TalentCustomer
                    serializedObjects = serializer.Get_Customer_Unprocessed(objectType.Name, _
                                                                            Origin, _
                                                                            BusinessUnit, _
                                                                            Partner, _
                                                                            LoginID)

                Case Else
                    serializedObjects = serializer.GetBy_ObjType_Origin_BU_Partner_Login_UniqueID_Unprocessed(objectType.Name, _
                                                                                                                Origin, _
                                                                                                                BusinessUnit, _
                                                                                                                Partner, _
                                                                                                                LoginID, _
                                                                                                                UniqueID)
            End Select

            If serializedObjects.Rows.Count > 0 Then
                serializedObject = CType(serializedObjects.Rows(0), SerializedObjectsDataSet.tbl_serialized_objectsRow)
                ObjectToPopulate = DeSerialize(serializedObject.SERIALIZED_OBJECT, ObjectToPopulate)
            End If

            Return ObjectToPopulate

        End Function

        Public Shared Function loginUser(ByVal userID As String, ByVal password As String) As Boolean
            If CType(Membership.Provider, TalentMembershipProvider).ValidateUser(userID, password) Then
                FormsAuthentication.Authenticate(userID, password)
                FormsAuthentication.SetAuthCookie(userID, False)
                Return True
            End If
            Return False
        End Function

        Public Shared Function aMD5Hash(ByVal value As String) As String

            Dim objMD5 As New System.Security.Cryptography.MD5CryptoServiceProvider
            Dim arrData() As Byte
            Dim arrHash() As Byte

            arrData = Text.Encoding.UTF8.GetBytes(value)
            arrHash = objMD5.ComputeHash(arrData)
            objMD5 = Nothing

            Return ByteArrayToString(arrHash)

        End Function

        Public Shared Function SHA1Encrypt(ByVal value As String) As String
            Dim encoder As New UTF8Encoding
            Dim SHA1Hasher As New SHA1CryptoServiceProvider
            Dim arrData() As Byte
            Dim arrHash() As Byte

            arrData = encoder.GetBytes(value)
            arrHash = SHA1Hasher.ComputeHash(arrData)

            Return ByteArrayToString(arrHash)
        End Function

        Private Shared Function ByteArrayToString(ByVal arrInput() As Byte) As String
            Dim strOutput As New System.Text.StringBuilder(arrInput.Length)
            For i As Integer = 0 To arrInput.Length - 1
                strOutput.Append(arrInput(i).ToString("X2"))
            Next
            Return strOutput.ToString().ToLower
        End Function

        Public Shared Function TestForPromotions(ByVal basketHeaderID As Long, _
                                                    ByVal languageCode As String, _
                                                    ByVal partner As String, _
                                                    ByVal promoCode As String, _
                                                    ByVal invalidPromoCodeError As String, _
                                                    ByVal TempOrderID As String) As Data.DataTable

            Dim defs As New Talent.eCommerce.ECommerceModuleDefaults
            Dim values As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
            values = defs.GetDefaults


            Dim codesAndQty As New Generic.Dictionary(Of String, WebPriceProduct)

            Dim basket As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
            Dim basketItems As TalentBasketDataset.tbl_basket_detailDataTable = basket.GetBasketItems_ByHeaderID_NonTicketing(basketHeaderID)

            For Each tbi As TalentBasketDataset.tbl_basket_detailRow In basketItems
                If Not tbi.IS_FREE Then
                    With codesAndQty
                        .Add(tbi.PRODUCT, New WebPriceProduct(tbi.PRODUCT, tbi.QUANTITY, tbi.MASTER_PRODUCT))
                    End With
                End If
            Next

            Dim webPrices As Talent.Common.TalentWebPricing = Utilities.GetWebPrices_WithTotals(codesAndQty, _
                                                                                                CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, _
                                                                                                values.PromotionPriority, promoCode, invalidPromoCodeError)

            If Not webPrices.PromotionsResultsTable Is Nothing Then
                Return webPrices.PromotionsResultsTable
            Else
                Return Nothing
            End If
        End Function

        Public Shared Function GetLookupKey() As String
            Dim lookUpKey As String = String.Empty
            Dim businessUnit As String = TalentCache.GetBusinessUnit()
            Dim partner As String = TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile))

            Dim myDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()

            Dim myData As New TalentMembershipDatasetTableAdapters.tbl_login_lookupTableAdapter
            Dim dt2 As Data.DataTable = myData.GetDataByLoginId(businessUnit, _
                                                                      partner, _
                                                                      HttpContext.Current.Profile.UserName, _
                                                                      def.LoginLookupType.Trim)
            If dt2.Rows.Count > 0 Then
                lookUpKey = dt2.Rows(0)("LOOKUP_KEY").ToString
            End If
            Return lookUpKey

        End Function

        Public Shared Function GetSettingsObjectForAgent(Optional ByVal withConnStringList As Boolean = True) As Talent.Common.DESettings
            Dim settings As New Talent.Common.DESettings
            Dim myDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
            settings.BusinessUnit = TalentCache.GetBusinessUnit
            settings.Partner = TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile))
            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
            settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
            settings.CacheDependencyPath = def.CacheDependencyPath
            settings.OriginatingSourceCode = GlobalConstants.SOURCE
            settings.EcommerceModuleDefaultsValues = def
            settings.CanProcessFeesParallely = def.CanProcessFeesParallely
            If HttpContext.Current.Session IsNot Nothing AndAlso HttpContext.Current.Session("Agent") IsNot Nothing Then
                settings.IsAgent = True
                settings.OriginatingSource = GetOriginatingSource(HttpContext.Current.Session.Item("Agent"))
            End If
            settings.StoredProcedureGroup = def.StoredProcedureGroup
            If withConnStringList Then
                settings.ConnectionStringList = GetConnectionStringList()
            End If
            If CType(HttpContext.Current.Profile, TalentProfile) IsNot Nothing Then
                Dim talProfileUser As TalentProfileUser = CType(HttpContext.Current.Profile, TalentProfile).User
                If talProfileUser IsNot Nothing AndAlso talProfileUser.Details IsNot Nothing Then
                    If Not String.IsNullOrWhiteSpace(talProfileUser.Details.LoginID) Then
                        settings.LoginId = talProfileUser.Details.LoginID
                    End If
                End If
            End If
            'current page name
            settings.CurrentPageName = (New System.IO.FileInfo(System.Web.HttpContext.Current.Request.Url.AbsolutePath)).Name.ToLower
            Return settings
        End Function

        Public Shared Function GetSettingsObject(Optional ByVal withConnStringList As Boolean = True) As Talent.Common.DESettings
            Dim settings As New Talent.Common.DESettings
            Dim myDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
            settings.BusinessUnit = TalentCache.GetBusinessUnit
            settings.Partner = TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile), settings.BusinessUnit)
            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
            settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
            settings.CacheDependencyPath = def.CacheDependencyPath
            settings.OriginatingSourceCode = GlobalConstants.SOURCE
            settings.EcommerceModuleDefaultsValues = def
            settings.CanProcessFeesParallely = def.CanProcessFeesParallely
            settings.AgentEntity = GetAgentEntity(def.CacheDependencyPath)
            If HttpContext.Current.Session IsNot Nothing AndAlso HttpContext.Current.Session("Agent") IsNot Nothing Then
                settings.IsAgent = True
                settings.OriginatingSource = GetOriginatingSource(HttpContext.Current.Session.Item("Agent"))
            End If
            settings.StoredProcedureGroup = def.StoredProcedureGroup
            If withConnStringList Then
                settings.ConnectionStringList = GetConnectionStringList()
            End If
            settings.LoginId = GlobalConstants.GENERIC_CUSTOMER_NUMBER
            If CType(HttpContext.Current.Profile, TalentProfile) IsNot Nothing Then
                Dim talProfileUser As TalentProfileUser = CType(HttpContext.Current.Profile, TalentProfile).User
                If talProfileUser IsNot Nothing AndAlso talProfileUser.Details IsNot Nothing Then
                    If Not String.IsNullOrWhiteSpace(talProfileUser.Details.LoginID) Then
                        settings.LoginId = talProfileUser.Details.LoginID
                    End If
                End If
            End If
            'current page name
            settings.CurrentPageName = (New System.IO.FileInfo(System.Web.HttpContext.Current.Request.Url.AbsolutePath)).Name.ToLower
            settings.DeliveryCountryCode = GetDeliveryCountryISOAlpha3Code()
            settings.Language = GetCurrentLanguage()
            Return settings
        End Function

        Public Shared Function GetConnectionStringList() As Generic.List(Of String)
            Dim connStringList As New Generic.List(Of String)
            Dim cacheKey As String = "ConnectionStringList"
            If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                connStringList = HttpContext.Current.Cache.Item(cacheKey)
            Else
                Dim tempConnectionString As String = String.Empty
                If TryGetConnectionString("liveUpdateDatabase01", tempConnectionString) Then connStringList.Add(tempConnectionString)
                If TryGetConnectionString("liveUpdateDatabase02", tempConnectionString) Then connStringList.Add(tempConnectionString)
                If TryGetConnectionString("liveUpdateDatabase03", tempConnectionString) Then connStringList.Add(tempConnectionString)
                If TryGetConnectionString("liveUpdateDatabase04", tempConnectionString) Then connStringList.Add(tempConnectionString)
                If TryGetConnectionString("liveUpdateDatabase05", tempConnectionString) Then connStringList.Add(tempConnectionString)
                If TryGetConnectionString("liveUpdateDatabase06", tempConnectionString) Then connStringList.Add(tempConnectionString)
                If TryGetConnectionString("liveUpdateDatabase07", tempConnectionString) Then connStringList.Add(tempConnectionString)
                If TryGetConnectionString("liveUpdateDatabase08", tempConnectionString) Then connStringList.Add(tempConnectionString)
                If TryGetConnectionString("liveUpdateDatabase09", tempConnectionString) Then connStringList.Add(tempConnectionString)
                If TryGetConnectionString("liveUpdateDatabase10", tempConnectionString) Then connStringList.Add(tempConnectionString)

                'Cache the result
                TalentCache.AddPropertyToCache(cacheKey, connStringList, CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes")), TimeSpan.Zero, CacheItemPriority.Normal)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)

            End If

            Return connStringList

        End Function

        Private Shared Function TryGetConnectionString(ByVal appSetConnVariableName As String, ByRef connectionString As String) As Boolean
            Dim isExists As Boolean = False
            Try
                connectionString = ConfigurationManager.AppSettings(appSetConnVariableName).Trim()
                If connectionString.Length <= 0 Then
                    connectionString = String.Empty
                    isExists = False
                Else
                    isExists = True
                End If
            Catch ex As Exception
                ' Handle web.config not containing any entries for 'liveUpdateDatabaseXX' - in this case assume standard frontend database to update 
                If appSetConnVariableName = "liveUpdateDatabase01" Then
                    connectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                    isExists = True
                Else
                    connectionString = String.Empty
                    isExists = False
                End If
            End Try
            Return isExists

        End Function

        Public Shared Function GetCountryCode() As String
            Return "UK"
        End Function

        'Remove from 2017 R1 onwards
        'Public Shared Function FormatCurrency(ByVal value As Decimal) As String
        '    Dim currencyCode As String = GetCurrencyCode()
        '    Return AppendCurrencyDetails(value, currencyCode, TalentCache.GetBusinessUnit)
        'End Function

        'Remove from 2017 R1 onwards
        'Public Shared Function FormatCurrency(ByVal value As Decimal, ByVal currencyCode As String, ByVal businessUnit As String) As String
        '    Return AppendCurrencyDetails(value, currencyCode, businessUnit)
        'End Function

        'Remove from 2017 R1 onwards
        'Public Shared Function AppendCurrencyDetails(ByVal value As Decimal, ByVal currencyCode As String, ByVal businessUnit As String) As String
        '    Dim formattedValue As String = String.Empty
        '    Dim ruleType As String = String.Empty
        '    Dim newValue As Decimal = 0
        '    '--------------------------
        '    ' Get currency code details  
        '    '--------------------------
        '    Dim dt2 As New Data.DataTable
        '    Dim cacheKey As String = "GetCurrencyFormatRule - " & businessUnit & "|" & currencyCode
        '    If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
        '        dt2 = HttpContext.Current.Cache.Item(cacheKey)
        '    Else
        '        Dim appVariables As New TalentApplicationVariablesTableAdapters.tbl_currency_format_buTableAdapter
        '        dt2 = appVariables.GetDataByBuCurrencyCode(businessUnit, currencyCode)

        '        'Cache the result
        '        TalentCache.AddPropertyToCache(cacheKey, dt2, CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes")), TimeSpan.Zero, CacheItemPriority.Normal)
        '        Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)

        '    End If

        '    If dt2.Rows.Count > 0 Then
        '        ruleType = Utilities.CheckForDBNull_String(dt2.Rows(0)("RULE_TYPE"))
        '        newValue = value
        '        If Utilities.CheckForDBNull_String(dt2.Rows(0)("FORMAT_STRING")) <> String.Empty Then
        '            formattedValue = newValue.ToString(dt2.Rows(0)("FORMAT_STRING"))
        '        Else
        '            formattedValue = newValue.ToString
        '        End If
        '        '------------------------
        '        ' Check Rules:
        '        ' 1) Replace '.' with ','
        '        ' 2) ...
        '        '------------------------
        '        Select Case ruleType
        '            Case Is = "1"
        '                formattedValue = formattedValue.Replace(".", ",")
        '            Case Else
        '        End Select
        '    End If
        '    Return formattedValue
        'End Function

        'Remove from 2017 R1 onwards
        Public Shared Function GetCurrencyCode() As String
            Dim currencyCode As String = String.Empty

            Dim cacheKey As String = "GetCurrencyCode - " & TalentCache.GetBusinessUnit & "|" & TalentCache.GetPartnerByUserName(HttpContext.Current.Profile)
            If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                currencyCode = HttpContext.Current.Cache.Item(cacheKey)
            Else

                Dim priceListHeader As New TalentBasketDatasetTableAdapters.tbl_price_list_headerTableAdapter
                Dim dt As Data.DataTable = priceListHeader.Get_PriceList_Header_By_BU_Partner(TalentCache.GetBusinessUnit, TalentCache.GetPartnerByUserName(HttpContext.Current.Profile))
                If dt.Rows.Count = 0 Then
                    dt = priceListHeader.Get_PriceList_Header_By_BU_Partner(TalentCache.GetBusinessUnit, Talent.Common.Utilities.GetAllString)
                End If
                If dt.Rows.Count > 0 AndAlso Utilities.CheckForDBNull_String(dt.Rows(0)("CURRENCY_CODE").ToString) <> String.Empty Then
                    currencyCode = Utilities.CheckForDBNull_String(dt.Rows(0)("CURRENCY_CODE")).Trim
                Else
                    currencyCode = "ENG"
                End If

                'Cache the result
                TalentCache.AddPropertyToCache(cacheKey, currencyCode, CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes")), TimeSpan.Zero, CacheItemPriority.Normal)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
            End If

            Return currencyCode
        End Function

        Public Shared Function FindWebControl(ByVal controlID As String, ByVal controls As System.Web.UI.ControlCollection) As Control
            Dim foundControl As Control = Nothing
            Try
                Dim found As Boolean = False
                For Each ctrl As Control In controls
                    If ctrl.ID = controlID Then
                        found = True
                        foundControl = ctrl
                        Exit For
                    End If
                Next
                If Not found Then
                    For Each ctrl As Control In controls
                        If ctrl.Controls.Count > 0 Then
                            foundControl = FindWebControl(controlID, ctrl.Controls)
                            If Not foundControl Is Nothing Then
                                Exit For
                            End If
                        End If
                    Next
                End If
            Catch ex As Exception
                foundControl = Nothing
            End Try

            Return foundControl
        End Function

#Region "Promotions And Retail Pricing"

        ''' <summary>
        ''' Returns a data table of promotions based on the values provided
        ''' </summary>
        ''' <param name="activationMechanism">AUTO or CODE</param>
        ''' <param name="promotionCode">Optional: the specific promotion code to search for</param>
        ''' <returns>DataTable of promotions</returns>
        ''' <remarks></remarks>
        Public Shared Function GetPromotions(ByVal activationMechanism As String, Optional ByVal promotionCode As String = "") As DataTable
            Return TalentCache.GetPromotions(activationMechanism, promotionCode)
        End Function

        Public Shared Function GetPromotions_RequiredProducts(ByVal promoCode As String) As DataTable
            ' Declare this first! Used for Logging function duration
            Dim timeSpan As TimeSpan = Now.TimeOfDay

            Dim dt As New DataTable

            Dim cmd As New SqlCommand("", New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
            Dim SelectStr As String = " SELECT * FROM tbl_promotions_required_products WITH (NOLOCK)  " & _
                                      " WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                      " AND PARTNER_GROUP = @PARTNER_GROUP " & _
                                      " AND PARTNER = @PARTNER " & _
                                      " AND PROMOTION_CODE = @PROMOTION_CODE "

            cmd.CommandText = SelectStr

            With cmd.Parameters
                .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                .Add("@PARTNER_GROUP", SqlDbType.NVarChar).Value = Utilities.GetPartnerGroup
                .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(HttpContext.Current.Profile)
                .Add("@PROMOTION_CODE", SqlDbType.NVarChar).Value = promoCode
            End With

            Try
                cmd.Connection.Open()

                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)

                If dt.Rows.Count = 0 Then
                    cmd.Parameters("@PARTNER").Value = Talent.Common.Utilities.GetAllString
                    da.Fill(dt)
                    If dt.Rows.Count = 0 Then
                        cmd.Parameters("@PARTNER_GROUP").Value = Talent.Common.Utilities.GetAllString
                        da.Fill(dt)
                        If dt.Rows.Count = 0 Then
                            cmd.Parameters("@BUSINESS_UNIT").Value = Talent.Common.Utilities.GetAllString
                            da.Fill(dt)
                        End If
                    End If
                End If

                da.Dispose()
            Catch ex As Exception
            Finally
                If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            End Try

            Utilities.TalentLogging.LoadTestLog("Utilities.vb", "GetPromotions_RequiredProducts", timeSpan)
            Return dt
        End Function

        Public Shared Function GetPromotions_Redeemed(ByVal tempOrderID As String, ByVal loginID As String) As DataTable
            ' Declare this first! Used for Logging function duration
            Dim timeSpan As TimeSpan = Now.TimeOfDay

            Dim dt As New DataTable

            Dim cmd As New SqlCommand("", New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
            Dim SelectStr As String = " SELECT   * " & _
                                        " FROM tbl_promotions_redeemed WITH (NOLOCK) " & _
                                        " WHERE (TEMP_ORDER_ID = @TEMP_ORDER_ID) " & _
                                        " AND (LOGINID = @LOGINID) " & _
                                        " AND (BUSINESS_UNIT = @BUSINESS_UNIT) " & _
                                        " AND (PARTNER_GROUP = @PARTNER_GROUP) " & _
                                        " AND (PARTNER = @PARTNER)"

            cmd.CommandText = SelectStr

            With cmd.Parameters
                .Add("@TEMP_ORDER_ID", SqlDbType.NVarChar).Value = tempOrderID
                .Add("@LOGINID", SqlDbType.NVarChar).Value = loginID
                .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                .Add("@PARTNER_GROUP", SqlDbType.NVarChar).Value = Utilities.GetPartnerGroup
                .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(HttpContext.Current.Profile)
            End With

            Try
                cmd.Connection.Open()

                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)

                da.Dispose()
            Catch ex As Exception
            Finally
                If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            End Try

            Utilities.TalentLogging.LoadTestLog("Utilities.vb", "GetPromotions_Redeemed", timeSpan)
            Return dt
        End Function

        Public Shared Function GetPromotions_Users(ByVal promocode As String, ByVal loginid As String, ByVal fromdate As DateTime, ByVal todate As DateTime) As DataTable
            ' Declare this first! Used for Logging function duration
            Dim timeSpan As TimeSpan = Now.TimeOfDay

            Dim dt As New DataTable

            Dim cmd As New SqlCommand("", New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
            Dim SelectStr As String = " SELECT *  " & _
                                        " FROM tbl_promotions_users WITH (NOLOCK)   " & _
                                        " WHERE (BUSINESS_UNIT = @BUSINESS_UNIT)  " & _
                                        " AND (PARTNER_GROUP = @PARTNER_GROUP)  " & _
                                        " AND (PARTNER = @PARTNER)  " & _
                                        " AND (PROMOTION_CODE = @PROMOTION_CODE)  " & _
                                        " AND (USERNAME = @USERNAME)  " & _
                                        " AND (ACTIVE_FROM = @ACTIVE_FROM)  " & _
                                        " AND (ACTIVE_TO = @ACTIVE_TO)"

            cmd.CommandText = SelectStr

            With cmd.Parameters
                .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                .Add("@PARTNER_GROUP", SqlDbType.NVarChar).Value = Utilities.GetPartnerGroup
                .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(HttpContext.Current.Profile)
                .Add("@PROMOTION_CODE", SqlDbType.NVarChar).Value = promocode
                .Add("@USERNAME", SqlDbType.NVarChar).Value = loginid
                .Add("@ACTIVE_FROM", SqlDbType.DateTime).Value = fromdate
                .Add("@ACTIVE_TO", SqlDbType.DateTime).Value = todate
            End With

            Try
                cmd.Connection.Open()

                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)

                da.Dispose()
            Catch ex As Exception
            Finally
                If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            End Try

            Utilities.TalentLogging.LoadTestLog("Utilities.vb", "GetPromotions_Users", timeSpan)
            Return dt
        End Function

        Public Shared Function InsertPromotions_Users(ByVal promocode As String, ByVal loginid As String, ByVal fromdate As DateTime, ByVal todate As DateTime, ByVal usageCount As Integer) As Boolean
            ' Declare this first! Used for Logging function duration
            Dim timeSpan As TimeSpan = Now.TimeOfDay

            Dim result As Integer = 0

            Dim cmd As New SqlCommand("", New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
            Dim InsertStr As String = " INSERT INTO [tbl_promotions_users]  " & _
                                        " ( " & _
                                        "   [BUSINESS_UNIT],  " & _
                                        "   [PARTNER_GROUP],  " & _
                                        "   [PARTNER],  " & _
                                        "   [PROMOTION_CODE],  " & _
                                        "   [USERNAME],  " & _
                                        "   [ACTIVE_FROM],  " & _
                                        "   [ACTIVE_TO],  " & _
                                        "   [USAGE_COUNT] " & _
                                        " )  " & _
                                        " VALUES  " & _
                                        " ( " & _
                                        "   @BUSINESS_UNIT,  " & _
                                        "   @PARTNER_GROUP,  " & _
                                        "   @PARTNER,  " & _
                                        "   @PROMOTION_CODE,  " & _
                                        "   @USERNAME,  " & _
                                        "   @ACTIVE_FROM,  " & _
                                        "   @ACTIVE_TO,  " & _
                                        "   @USAGE_COUNT " & _
                                        " )"

            cmd.CommandText = InsertStr

            With cmd.Parameters
                .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                .Add("@PARTNER_GROUP", SqlDbType.NVarChar).Value = Utilities.GetPartnerGroup
                .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(HttpContext.Current.Profile)
                .Add("@PROMOTION_CODE", SqlDbType.NVarChar).Value = promocode
                .Add("@USERNAME", SqlDbType.NVarChar).Value = loginid
                .Add("@ACTIVE_FROM", SqlDbType.DateTime).Value = fromdate
                .Add("@ACTIVE_TO", SqlDbType.DateTime).Value = todate
                .Add("@USAGE_COUNT", SqlDbType.Int).Value = usageCount
            End With

            Try
                cmd.Connection.Open()
                result = cmd.ExecuteNonQuery
            Catch ex As Exception
            Finally
                If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            End Try

            Utilities.TalentLogging.LoadTestLog("Utilities.vb", "InsertPromotions_Users", timeSpan)
            If result > 0 Then Return True Else Return False
        End Function

        Public Shared Function UpdatePromotions_Users(ByVal promocode As String, ByVal loginid As String, ByVal fromdate As DateTime, ByVal todate As DateTime) As Boolean
            ' Declare this first! Used for Logging function duration
            Dim timeSpan As TimeSpan = Now.TimeOfDay

            Dim result As Integer = 0

            Dim cmd As New SqlCommand("", New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
            Dim UpdateStr As String = " UPDATE    tbl_promotions_users   " & _
                                        " SET USAGE_COUNT = USAGE_COUNT + 1 " & _
                                        " WHERE (BUSINESS_UNIT = @BUSINESS_UNIT)" & _
                                        " AND (PARTNER_GROUP = @PARTNER_GROUP) " & _
                                        " AND (PARTNER = @PARTNER) " & _
                                        " AND (PROMOTION_CODE = @PROMOTION_CODE) " & _
                                        " AND (USERNAME = @USERNAME) " & _
                                        " AND (ACTIVE_FROM = @ACTIVE_FROM) " & _
                                        " AND (ACTIVE_TO = @ACTIVE_TO) "

            cmd.CommandText = UpdateStr

            With cmd.Parameters
                .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                .Add("@PARTNER_GROUP", SqlDbType.NVarChar).Value = Utilities.GetPartnerGroup
                .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(HttpContext.Current.Profile)
                .Add("@PROMOTION_CODE", SqlDbType.NVarChar).Value = promocode
                .Add("@USERNAME", SqlDbType.NVarChar).Value = loginid
                .Add("@ACTIVE_FROM", SqlDbType.DateTime).Value = fromdate
                .Add("@ACTIVE_TO", SqlDbType.DateTime).Value = todate
            End With

            Try
                cmd.Connection.Open()
                result = cmd.ExecuteNonQuery
            Catch ex As Exception
            Finally
                If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            End Try

            Utilities.TalentLogging.LoadTestLog("Utilities.vb", "UpdatePromotions_Users", timeSpan)
            If result > 0 Then Return True Else Return False
        End Function

        Public Shared Function UpdatePromotions_RedeemCount(ByVal promoCode As String) As Boolean
            ' Declare this first! Used for Logging function duration
            Dim timeSpan As TimeSpan = Now.TimeOfDay
            Dim result As Integer = 0
            Dim cmd As New SqlCommand("", New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
            Dim UpdateStr As String = " UPDATE    tbl_promotions  " & _
                                        " SET REDEEM_COUNT = REDEEM_COUNT + 1 " & _
                                        " WHERE (ACTIVE = 1)" & _
                                        " AND (BUSINESS_UNIT = @BUSINESS_UNIT) " & _
                                        " AND (PARTNER = @PARTNER) " & _
                                        " AND (PROMOTION_CODE = @PROMOTION_CODE) "

            cmd.CommandText = UpdateStr

            With cmd.Parameters
                .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(HttpContext.Current.Profile)
                .Add("@PROMOTION_CODE", SqlDbType.NVarChar).Value = promoCode
            End With
            Try
                cmd.Connection.Open()
                result = cmd.ExecuteNonQuery
                If result <= 0 Then
                    cmd.Parameters.Clear()
                    With cmd.Parameters
                        .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                        .Add("@PARTNER", SqlDbType.NVarChar).Value = GetAllString()
                        .Add("@PROMOTION_CODE", SqlDbType.NVarChar).Value = promoCode
                    End With
                    result = cmd.ExecuteNonQuery
                End If
            Catch ex As Exception
            Finally
                If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            End Try

            If result > 0 Then Return True Else Return False
        End Function

        Public Shared Function GetPartnerGroup() As String
            Dim def As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults
            Dim pGroup As String = ""
            Select Case def.PartnerGroupType
                Case Is = 1
                    pGroup = def.PriceList
            End Select
            Return pGroup
        End Function

        Public Shared Function GetDeliveryOptions(ByVal totalOrderValueForComparison As Decimal, _
                                                    ByVal freeDefaultDelivery As Boolean) As Talent.Common.DEDeliveryCharges

            Dim myDefs As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults

            Dim dc As New Talent.Common.DEDeliveryCharges
            Try
                Dim mySettings As New Talent.Common.DESettings()
                With mySettings
                    .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                    .DestinationDatabase = "SQL2005"
                    .BusinessUnit = TalentCache.GetBusinessUnit
                    .Partner = TalentCache.GetPartner(HttpContext.Current.Profile)
                    .Language = Talent.Common.Utilities.GetDefaultLanguage
                End With

                Dim charges As New Talent.Common.TalentDeliveryCharge(totalOrderValueForComparison, freeDefaultDelivery)
                charges.Settings = mySettings

                charges.GetDeliveryOptions()
                If Not charges.DeliveryCharges Is Nothing Then
                    dc = charges.DeliveryCharges
                End If
            Catch ex As Exception
            End Try

            Return dc
        End Function

        Public Shared Function GetDeliveryOptions(ByVal cacheTimeMinutes As Integer, ByVal totalOrderValueForComparison As Decimal, _
                                                    ByVal freeDefaultDelivery As Boolean, ByVal totalWeight As Decimal, ByVal countryCodeToDeliver As String, ByVal countryNameToDeliver As String) As Talent.Common.DEDeliveryCharges

            Dim myDefs As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults

            Dim dc As New Talent.Common.DEDeliveryCharges
            Try
                Dim mySettings As New Talent.Common.DESettings()
                With mySettings
                    .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                    .DestinationDatabase = "SQL2005"
                    .BusinessUnit = TalentCache.GetBusinessUnit
                    .Partner = TalentCache.GetPartner(HttpContext.Current.Profile)
                    .Language = Talent.Common.Utilities.GetDefaultLanguage
                    .Cacheing = True
                    .CacheTimeMinutes = cacheTimeMinutes
                End With

                Dim charges As New Talent.Common.TalentDeliveryCharge(totalOrderValueForComparison, freeDefaultDelivery, totalWeight, countryCodeToDeliver, countryNameToDeliver)
                charges.Settings = mySettings
                charges.GetDeliveryOptionsByWeight()
                If Not charges.DeliveryCharges Is Nothing Then
                    dc = charges.DeliveryCharges
                End If
            Catch ex As Exception
            End Try

            Return dc
        End Function

        Public Shared Function GetBasketItemsTotalWeight(ByVal talBasketItems As Generic.List(Of DEBasketItem)) As Decimal
            Dim totalWeight As Decimal = 0
            If talBasketItems.Count > 0 Then
                For itemIndex As Integer = 0 To talBasketItems.Count - 1
                    If (UCase(talBasketItems.Item(itemIndex).MODULE_) <> "TICKETING") Then
                        If Not talBasketItems.Item(itemIndex).IS_FREE Then
                            totalWeight += ((talBasketItems.Item(itemIndex).Quantity) * (Utilities.CheckForDBNull_Decimal(talBasketItems.Item(itemIndex).WEIGHT)))
                        End If
                    End If
                Next
            End If
            Return totalWeight
        End Function

        Public Shared Sub GetDeliveryCountry(ByVal tempOrderId As String, ByRef countryCode As String, ByRef countryName As String)
            Dim tDataObjects As New TalentDataObjects
            Dim dtOrderHeader As New DataTable
            tDataObjects.Settings = GetSettingsObject()
            dtOrderHeader = tDataObjects.OrderSettings.TblOrderHeader.GetOrderHeaderByTempOrderID(tempOrderId)
            If dtOrderHeader IsNot Nothing AndAlso dtOrderHeader.Rows.Count > 0 Then
                countryCode = Utilities.CheckForDBNull_String(dtOrderHeader.Rows(0)("COUNTRY_CODE"))
                countryName = Utilities.CheckForDBNull_String(dtOrderHeader.Rows(0)("COUNTRY"))
            End If
        End Sub

        ''' <summary>
        ''' Gets the Prices from the DB for the product code given the Quantity supplied
        ''' </summary>
        ''' <param name="productCode"></param>
        ''' <param name="quantity"></param>
        ''' <param name="masterProductCode"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetWebPrices(ByVal productCode As String, _
                                            ByVal quantity As Decimal, _
                                            ByVal masterProductCode As String) As Talent.Common.DEWebPrice

            Dim products As New Generic.Dictionary(Of String, WebPriceProduct)
            products.Add(productCode, New WebPriceProduct(productCode, quantity, masterProductCode))

            Dim wp As New Talent.Common.DEWebPrice
            Try
                wp = GetWebPrices(products)(productCode)
                wp.PriceFound = True
            Catch ex As Exception
                wp.PriceFound = False
            End Try

            Return wp
        End Function

        ''' <summary>
        ''' Gets the Prices from the DB for the product code given the Quantity supplied and price list
        ''' </summary>
        ''' <param name="productCode"></param>
        ''' <param name="quantity"></param>
        ''' <param name="priceList"></param>
        ''' <param name="partnerGroup"></param>
        ''' <param name="masterProductCode"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetWebPrices(ByVal productCode As String, _
                                            ByVal quantity As Decimal, _
                                            ByVal priceList As String, _
                                            ByVal partnerGroup As String, _
                                            ByVal masterProductCode As String) As Talent.Common.DEWebPrice

            Dim myDefs As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults

            Dim products As New Generic.Dictionary(Of String, WebPriceProduct)
            products.Add(productCode, New WebPriceProduct(productCode, quantity, masterProductCode))

            Dim defaultPriceList As String = myDefs.DefaultPriceList
            If Not myDefs.UseGlobalPriceListWithCustomerPriceList Then defaultPriceList = myDefs.PriceList

            Dim wp As New Talent.Common.DEWebPrice
            Try
                Dim pricingSettings As New Talent.Common.DEWebPriceSetting(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString, _
                                                                      "SQL2005", _
                                                                      "", _
                                                                      TalentCache.GetBusinessUnit, _
                                                                      TalentCache.GetPartner(HttpContext.Current.Profile), _
                                                                      priceList, _
                                                                      False, _
                                                                      Talent.Common.Utilities.GetDefaultLanguage, _
                                                                      partnerGroup, _
                                                                      defaultPriceList, _
                                                                      GetUsersAttributeList)

                Dim productPricing As New Talent.Common.TalentWebPricing(pricingSettings, products, Not myDefs.ShowPricesExVAT)

                productPricing.GetWebPrices()
                If Not productPricing.RetrievedPrices Is Nothing Then
                    wp = productPricing.RetrievedPrices(productCode)
                    wp.PriceFound = True
                Else
                    wp.PriceFound = False
                End If
            Catch ex As Exception
            End Try

            Return wp
        End Function

        ''' <summary>
        ''' Gets a collection of Prices based on a Collection of product codes and quantities supplied
        ''' </summary>
        ''' <param name="products"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetWebPrices(ByVal products As Generic.Dictionary(Of String, WebPriceProduct)) As Generic.Dictionary(Of String, Talent.Common.DEWebPrice)

            Dim def As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults

            Dim defaultPriceList As String = TalentCache.GetPartner(HttpContext.Current.Profile)
            If Not def.UseGlobalPriceListWithCustomerPriceList Then defaultPriceList = TalentCache.GetPartner(HttpContext.Current.Profile)

            Dim pricingsettings As New Talent.Common.DEWebPriceSetting(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString, _
                                                                        "SQL2005", _
                                                                        "", _
                                                                        TalentCache.GetBusinessUnit, _
                                                                        TalentCache.GetPartner(HttpContext.Current.Profile), _
                                                                        def.PriceList, _
                                                                        False, _
                                                                        Talent.Common.Utilities.GetDefaultLanguage, _
                                                                        Utilities.GetPartnerGroup, _
                                                                        defaultPriceList, _
                                                                        GetUsersAttributeList)

            Dim productPricing As New Talent.Common.TalentWebPricing(pricingsettings, products, Not def.ShowPricesExVAT)

            productPricing.GetWebPrices()
            If productPricing.RetrievedPrices Is Nothing Then
                productPricing.RetrievedPrices = New Generic.Dictionary(Of String, Talent.Common.DEWebPrice)()
            End If
            Return productPricing.RetrievedPrices
        End Function

        ''' <summary>
        ''' Gets the Prices from the DB for the product code given the Quantity supplied and price list
        ''' </summary>
        ''' <param name="productCode"></param>
        ''' <param name="quantity"></param>
        ''' <param name="priceList"></param>
        ''' <param name="masterProductCode"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetWebPrices(ByVal productCode As String, _
                                            ByVal quantity As Decimal, _
                                            ByVal priceList As String, _
                                            ByVal masterProductCode As String) As Talent.Common.DEWebPrice

            Dim myDefs As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults
            Dim products As New Generic.Dictionary(Of String, WebPriceProduct)
            products.Add(productCode, New WebPriceProduct(productCode, quantity, masterProductCode))

            Dim defaultPriceList As String = myDefs.DefaultPriceList
            If Not myDefs.UseGlobalPriceListWithCustomerPriceList Then defaultPriceList = myDefs.PriceList

            Dim wp As New Talent.Common.DEWebPrice
            Try
                Dim pricingsettings As New Talent.Common.DEWebPriceSetting(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString, _
                                                                        "SQL2005", _
                                                                        "", _
                                                                        TalentCache.GetBusinessUnit, _
                                                                        TalentCache.GetPartner(HttpContext.Current.Profile), _
                                                                        priceList, _
                                                                        False, _
                                                                        Talent.Common.Utilities.GetDefaultLanguage, _
                                                                        Utilities.GetPartnerGroup, _
                                                                        defaultPriceList, _
                                                                        GetUsersAttributeList)

                Dim productPricing As New Talent.Common.TalentWebPricing(pricingsettings, products, Not myDefs.ShowPricesExVAT)

                productPricing.GetWebPrices()
                If Not productPricing.RetrievedPrices Is Nothing Then
                    wp = productPricing.RetrievedPrices(productCode)
                    wp.PriceFound = True
                Else
                    wp.PriceFound = False
                End If
            Catch ex As Exception
            End Try

            Return wp
        End Function

        ''' <summary>
        ''' Gets the web prices given the product code and quantity supplied, but will also check whether the product is part 
        ''' of an active promotion. If it is the product's promotion description and image path fields will be populated. 
        ''' </summary>
        ''' <param name="productCode"></param>
        ''' <param name="quantity"></param>
        ''' <param name="masterProductCode"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetWebPrices_WithPromoDetails(ByVal productCode As String, _
                                                                ByVal quantity As Decimal, _
                                                                ByVal masterProductCode As String) As Talent.Common.DEWebPrice

            Dim products As New Generic.Dictionary(Of String, WebPriceProduct)
            products.Add(productCode, New WebPriceProduct(productCode, quantity, masterProductCode))

            Dim wp As New Talent.Common.DEWebPrice
            Try
                wp = GetWebPrices_WithPromoDetails(products)(productCode)
            Catch ex As Exception
            End Try

            Return wp
        End Function

        ''' <summary>
        ''' Gets a collection of prices based on the collection of codes and quantities supplied, and checks whether the 
        ''' each product is part of an active promotion. If it is the product's promotion description and image path 
        ''' fields will be populated. 
        ''' </summary>
        ''' <param name="products"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetWebPrices_WithPromoDetails(ByVal products As Generic.Dictionary(Of String, WebPriceProduct)) As Generic.Dictionary(Of String, Talent.Common.DEWebPrice)

            Dim defs As New ECommerceModuleDefaults
            Dim values As New ECommerceModuleDefaults.DefaultValues
            values = defs.GetDefaults(TalentCache.GetPartner(HttpContext.Current.Profile), TalentCache.GetBusinessUnit())

            Dim defaultPriceList As String = values.DefaultPriceList
            If Not values.UseGlobalPriceListWithCustomerPriceList Then defaultPriceList = values.PriceList

            Dim pricingsettings As New Talent.Common.DEWebPriceSetting(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString, _
                                                                        "SQL2005", _
                                                                        "", _
                                                                        TalentCache.GetBusinessUnit, _
                                                                        TalentCache.GetPartner(HttpContext.Current.Profile), _
                                                                        values.PriceList, _
                                                                        False, _
                                                                        Talent.Common.Utilities.GetDefaultLanguage, _
                                                                        Utilities.GetPartnerGroup, _
                                                                        defaultPriceList, _
                                                                        GetUsersAttributeList)

            Dim productPricing As New Talent.Common.TalentWebPricing(pricingsettings, products, Not values.ShowPricesExVAT)

            productPricing.GetWebPrices_WithPromotionDetails()
            If productPricing.RetrievedPrices Is Nothing Then
                productPricing.RetrievedPrices = New Generic.Dictionary(Of String, Talent.Common.DEWebPrice)()
            End If
            Return productPricing.RetrievedPrices
        End Function

        ''' <summary>
        ''' Gets a the prices based on the product code and quantity supplied, and checks whether the 
        ''' product is part of an active promotion. If it is the product's promotion description and image path 
        ''' fields will be populated. 
        ''' Additionally, the total cost fileds of the TalentWebPricing object will be populated and the prices adjusted
        ''' if any promotions are activated as a result of the items passed in.
        ''' </summary>
        ''' <param name="productCode"></param>
        ''' <param name="quantity"></param>
        ''' <param name="tempOrderID"></param>
        ''' <param name="PromotionTypePriority"></param>
        ''' <param name="masterProductCode"></param>
        ''' <param name="PromotionCode"></param>
        ''' <param name="InvalidPromotionCode_ErrorText"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetWebPrices_WithTotals(ByVal productCode As String, _
                                                        ByVal quantity As Decimal, _
                                                        ByVal tempOrderID As String, _
                                                        ByVal PromotionTypePriority As String, _
                                                        ByVal masterProductCode As String, _
                                                        Optional ByVal PromotionCode As String = "", _
                                                        Optional ByVal InvalidPromotionCode_ErrorText As String = "") _
                                                                                                As Talent.Common.TalentWebPricing
            Dim products As New Generic.Dictionary(Of String, WebPriceProduct)
            products.Add(productCode, New WebPriceProduct(productCode, quantity, masterProductCode))

            Return GetWebPrices_WithTotals(products, tempOrderID, PromotionTypePriority, PromotionCode, InvalidPromotionCode_ErrorText)
        End Function

        ''' <summary>
        ''' Gets a the prices based on the product codes and quantities supplied, and checks whether the 
        ''' products are part of an active promotion. If any are, it's promotion description and image path 
        ''' fields will be populated. 
        ''' Additionally, the total cost fileds of the TalentWebPricing object will be populated and the prices adjusted
        ''' if any promotions are activated as a result of the items passed in.
        ''' </summary>
        ''' <param name="products"></param>
        ''' <param name="tempOrderID"></param>
        ''' <param name="PromotionTypePriority"></param>
        ''' <param name="PromotionCode"></param>
        ''' <param name="InvalidPromotionCode_ErrorText"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetWebPrices_WithTotals(ByVal products As Generic.Dictionary(Of String, WebPriceProduct), _
                                                        ByVal tempOrderID As String, _
                                                        ByVal PromotionTypePriority As String, _
                                                        Optional ByVal PromotionCode As String = "", _
                                                        Optional ByVal InvalidPromotionCode_ErrorText As String = "") _
                                                                                                    As Talent.Common.TalentWebPricing

            Dim defaults As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults

            Dim defaultPriceList As String = defaults.DefaultPriceList
            If Not defaults.UseGlobalPriceListWithCustomerPriceList Then defaultPriceList = defaults.PriceList

            Dim pricingsettings As New Talent.Common.DEWebPriceSetting(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString, _
                                                                        "SQL2005", _
                                                                        "", _
                                                                        TalentCache.GetBusinessUnit, _
                                                                        TalentCache.GetPartner(HttpContext.Current.Profile), _
                                                                        defaults.PriceList, _
                                                                        False, _
                                                                        Talent.Common.Utilities.GetDefaultLanguage, _
                                                                        Utilities.GetPartnerGroup, _
                                                                        defaultPriceList, _
                                                                        GetUsersAttributeList)
            ' Declare this first! Used for Logging function duration
            Dim timeSpan As TimeSpan = Now.TimeOfDay

            Dim productPricing As New Talent.Common.TalentWebPricing(pricingsettings, products, Not defaults.ShowPricesExVAT)

            If PromotionCode.Trim <> String.Empty Then
                ' OK
            Else

                ' Check if any promotions used
                Dim promoRedeemed As New Data.DataTable
                promoRedeemed = GetPromotions_Redeemed( _
                    CType(HttpContext.Current.Profile, TalentProfile).Basket.Temp_Order_Id, _
                    CType(HttpContext.Current.Profile, TalentProfile).UserName)

                If promoRedeemed.Rows.Count > 0 Then
                    For Each promo As Data.DataRow In promoRedeemed.Rows
                        If CheckForDBNull_String(promo("ACTIVATION_MECHANISM")) = Talent.Common.DBPromotions.code Then
                            If CheckForDBNullOrBlank_Boolean_DefaultFalse(promo("SUCCESS")) Then
                                PromotionCode = CheckForDBNull_String(promo("PROMOTION_CODE"))
                                Exit For
                            End If
                        End If
                    Next promo
                End If

            End If

            With pricingsettings
                .PromotionCode = PromotionCode
                .InvalidPromotionCodeErrorMessage = InvalidPromotionCode_ErrorText
                .TempOrderID = CType(HttpContext.Current.Profile, TalentProfile).Basket.Temp_Order_Id
                .PromotionTypePriority = PromotionTypePriority
                .BasketHeaderID = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
                .Username = CType(HttpContext.Current.Profile, TalentProfile).UserName
            End With

            productPricing.GetWebPricesWithTotals()


            If defaults.Call_Tax_WebService AndAlso CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems.Count > 0 Then
                Dim results As Data.DataSet
                Dim taxWS As New TaxWebService

                results = taxWS.CallTaxWebService("BASKET")
                If results.Tables.Count > 0 Then
                    Dim header As Data.DataTable = results.Tables(0)
                    If header.Rows.Count > 0 Then
                        If CheckForDBNullOrBlank_Boolean_DefaultFalse(header.Rows(0)("Success")) Then

                            'delivery charges
                            productPricing.Max_Delivery_Gross = CheckForDBNull_Decimal(header.Rows(0)("DeliveryGROSS"))
                            productPricing.Max_Delivery_Net = CheckForDBNull_Decimal(header.Rows(0)("DeliveryTAX"))
                            productPricing.Max_Delivery_Tax = CheckForDBNull_Decimal(header.Rows(0)("DeliveryNET"))

                            'items tax totals
                            productPricing.Total_Items_Tax1 = CheckForDBNull_Decimal(header.Rows(0)("TotalTax1"))
                            productPricing.Total_Items_Tax2 = CheckForDBNull_Decimal(header.Rows(0)("TotalTax2"))
                            productPricing.Total_Items_Tax3 = CheckForDBNull_Decimal(header.Rows(0)("TotalTax3"))
                            productPricing.Total_Items_Tax4 = CheckForDBNull_Decimal(header.Rows(0)("TotalTax4"))
                            productPricing.Total_Items_Tax5 = CheckForDBNull_Decimal(header.Rows(0)("TotalTax5"))
                            productPricing.Total_Items_Tax = (productPricing.Total_Items_Tax1 _
                                                                + productPricing.Total_Items_Tax2 _
                                                                + productPricing.Total_Items_Tax3 _
                                                                + productPricing.Total_Items_Tax4 _
                                                                + productPricing.Total_Items_Tax5) _
                                                                - productPricing.Max_Delivery_Tax

                            'order totals
                            productPricing.Total_Order_Value_Gross = CheckForDBNull_Decimal(header.Rows(0)("TotalGross"))
                            productPricing.Total_Order_Value_Net = productPricing.Total_Order_Value_Gross - productPricing.Total_Items_Tax
                            productPricing.Total_Order_Value_Tax = productPricing.Total_Items_Tax + productPricing.Max_Delivery_Tax

                            'item totals
                            productPricing.Total_Items_Value_Gross = productPricing.Total_Order_Value_Gross - productPricing.Max_Delivery_Gross
                            productPricing.Total_Items_Value_Net = CheckForDBNull_Decimal(header.Rows(0)("GoodsTotalNet"))
                        End If
                    End If
                End If
            End If
            If productPricing.RetrievedPrices Is Nothing Then
                productPricing.RetrievedPrices = New Generic.Dictionary(Of String, Talent.Common.DEWebPrice)()
            End If

            Utilities.TalentLogging.LoadTestLog("Utilities.vb", "GetWebPrices_WithTotals", timeSpan)
            Return productPricing
        End Function

        ''' <summary>
        ''' Returns a TalentWebPricing object that has been populated directly form the basket and it's price values, rather than
        ''' re-pricing everyting from the front-end db as occurs under standard functionality.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetPrices_Type2() As Talent.Common.TalentWebPricing

            Dim defaults As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults

            Dim defaultPriceList As String = defaults.DefaultPriceList
            If Not defaults.UseGlobalPriceListWithCustomerPriceList Then defaultPriceList = defaults.PriceList

            Dim pricingsettings As New Talent.Common.DEWebPriceSetting(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString, _
                                                                        "SQL2005", _
                                                                        "", _
                                                                        TalentCache.GetBusinessUnit, _
                                                                        TalentCache.GetPartner(HttpContext.Current.Profile), _
                                                                        defaults.PriceList, _
                                                                        False, _
                                                                        Talent.Common.Utilities.GetDefaultLanguage, _
                                                                        Utilities.GetPartnerGroup, _
                                                                        defaultPriceList, _
                                                                        GetUsersAttributeList)

            Dim products As New Generic.Dictionary(Of String, WebPriceProduct)
            Dim totals As New Talent.Common.TalentWebPricing(pricingsettings, products, Not defaults.ShowPricesExVAT)

            With pricingsettings
                .TempOrderID = CType(HttpContext.Current.Profile, TalentProfile).Basket.Temp_Order_Id
                .BasketHeaderID = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
                .Username = CType(HttpContext.Current.Profile, TalentProfile).UserName
            End With

            totals.Total_Items_Tax = 0
            totals.Total_Items_Value_Gross = 0
            totals.Total_Items_Value_Net = 0
            totals.Total_Items_Value_Tax = 0
            totals.Total_Order_Value_Gross = 0
            totals.Total_Order_Value_Net = 0
            totals.Total_Order_Value_Tax = 0
            totals.Total_Promotions_Value = 0
            totals.Max_Delivery_Gross = 0
            totals.Max_Delivery_Net = 0
            totals.Max_Delivery_Tax = 0
            totals.RetrievedPrices = New Generic.Dictionary(Of String, Talent.Common.DEWebPrice)()

            For Each tbi As TalentBasketItem In CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems
                Dim de As New DEWebPrice
                de.PRODUCT_CODE = tbi.Product
                de.Purchase_Price_Gross = tbi.Gross_Price
                de.Purchase_Price_Net = tbi.Net_Price
                de.Purchase_Price_Tax = tbi.Tax_Price
                de.RequestedQuantity = tbi.Quantity
                de.TAX_CODE = ""
                totals.RetrievedPrices.Add(de.PRODUCT_CODE, de)
            Next
            If totals.RetrievedPrices IsNot Nothing Then
                totals.CalculateTotals()
            Else
                totals.RetrievedPrices = New Generic.Dictionary(Of String, Talent.Common.DEWebPrice)()
            End If
            Return totals
        End Function

        Public Shared Function GetChorusPrice(ByVal productCode As String, ByVal quantity As Decimal) As DataTable

            Dim defaults As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults

            Dim results As New DataTable

            Dim tp As New TalentPricing
            Dim tpSettings As New Talent.Common.DESettings
            tpSettings.BusinessUnit = TalentCache.GetBusinessUnit
            tpSettings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
            tpSettings.DestinationDatabase = "CHORUS"
            tpSettings.StoredProcedureGroup = defaults.StoredProcedureGroup
            tp.Settings = tpSettings

            'Check the basket to see if the item already exists... if so, combine the quantities when retrieving the product price
            For Each tbi As TalentBasketItem In CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems
                If tbi.Product.ToLower = productCode.ToLower Then
                    quantity += tbi.Quantity
                End If
            Next

            'need to pass in product code and quantity
            Dim depna As New Talent.Common.DePNA
            If Not HttpContext.Current.Profile.IsAnonymous Then
                depna.AccountNumber1 = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Account_No_1
                depna.AccountNumber2 = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Account_No_2
            End If
            depna.Quantity = quantity
            depna.WebProductCode = productCode
            tp.Dep = depna
            tp.GetSinglePrice()

            If tp.ResultDataSet IsNot Nothing _
                AndAlso tp.ResultDataSet.Tables.Count > 0 _
                    AndAlso tp.ResultDataSet.Tables.Contains("ChorusProductPrices") Then
                results = tp.ResultDataSet.Tables("ChorusProductPrices")
            End If

            Return results

        End Function

        ''' <summary>
        ''' USED WHEN CALLING FROM WITHIN THE TAX WEBSERVICE CLASS ONLY
        ''' Gets a the prices based on the product codes and quantities supplied, and checks whether the 
        ''' products are part of an active promotion. If any are, it's promotion description and image path 
        ''' fields will be populated. 
        ''' Additionally, the total cost fileds of the TalentWebPricing object will be populated and the prices adjusted
        ''' if any promotions are activated as a result of the items passed in.
        ''' </summary>
        ''' <param name="products"></param>
        ''' <param name="tempOrderID"></param>
        ''' <param name="PromotionTypePriority"></param>
        ''' <param name="PromotionCode"></param>
        ''' <param name="InvalidPromotionCode_ErrorText"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetWebPrices_WithTotals_CalledFromTaxWS(ByVal products As Generic.Dictionary(Of String, WebPriceProduct), _
                                                                        ByVal tempOrderID As String, _
                                                                        ByVal PromotionTypePriority As String, _
                                                                        Optional ByVal PromotionCode As String = "", _
                                                                        Optional ByVal InvalidPromotionCode_ErrorText As String = "") _
                                                                                                    As Talent.Common.TalentWebPricing

            Dim defaults As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults

            Dim defaultPriceList As String = defaults.DefaultPriceList
            If Not defaults.UseGlobalPriceListWithCustomerPriceList Then defaultPriceList = defaults.PriceList

            Dim pricingsettings As New Talent.Common.DEWebPriceSetting(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString, _
                                                                        "SQL2005", _
                                                                        "", _
                                                                        TalentCache.GetBusinessUnit, _
                                                                        TalentCache.GetPartner(HttpContext.Current.Profile), _
                                                                        defaults.PriceList, _
                                                                        False, _
                                                                        Talent.Common.Utilities.GetDefaultLanguage, _
                                                                        Utilities.GetPartnerGroup, _
                                                                        defaultPriceList, _
                                                                        GetUsersAttributeList)

            Dim productPricing As New Talent.Common.TalentWebPricing(pricingsettings, products, Not defaults.ShowPricesExVAT)


            With pricingsettings
                .PromotionCode = PromotionCode
                .InvalidPromotionCodeErrorMessage = InvalidPromotionCode_ErrorText
                .TempOrderID = CType(HttpContext.Current.Profile, TalentProfile).Basket.Temp_Order_Id
                .PromotionTypePriority = PromotionTypePriority
                .BasketHeaderID = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
                .Username = CType(HttpContext.Current.Profile, TalentProfile).UserName
            End With

            productPricing.GetWebPricesWithTotals()
            If productPricing.RetrievedPrices Is Nothing Then
                productPricing.RetrievedPrices = New Generic.Dictionary(Of String, Talent.Common.DEWebPrice)()
            End If
            Return productPricing
        End Function

        Public Shared Function GetWebPrices_WithPromotionDetails(ByVal products As Generic.Dictionary(Of String, WebPriceProduct), _
                                                                Optional ByVal PromotionCode As String = "", _
                                                                Optional ByVal InvalidPromotionCode_ErrorText As String = "") _
                                                                                            As Talent.Common.TalentWebPricing

            Dim defaults As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults

            Dim defaultPriceList As String = defaults.DefaultPriceList
            If Not defaults.UseGlobalPriceListWithCustomerPriceList Then defaultPriceList = defaults.PriceList

            Dim pricingsettings As New Talent.Common.DEWebPriceSetting(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString, _
                                                                        "SQL2005", _
                                                                        "", _
                                                                        TalentCache.GetBusinessUnit, _
                                                                        TalentCache.GetPartner(HttpContext.Current.Profile), _
                                                                        defaults.PriceList, _
                                                                        False, _
                                                                        Talent.Common.Utilities.GetDefaultLanguage, _
                                                                        Utilities.GetPartnerGroup, _
                                                                        defaultPriceList, _
                                                                        GetUsersAttributeList)

            Dim productPricing As New Talent.Common.TalentWebPricing(pricingsettings, products, Not defaults.ShowPricesExVAT)


            With pricingsettings
                .PromotionCode = PromotionCode
                .InvalidPromotionCodeErrorMessage = InvalidPromotionCode_ErrorText
                .TempOrderID = CType(HttpContext.Current.Profile, TalentProfile).Basket.Temp_Order_Id
                .BasketHeaderID = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
                .Username = CType(HttpContext.Current.Profile, TalentProfile).UserName
            End With

            productPricing.GetWebPrices_WithPromotionDetails()
            If productPricing.RetrievedPrices Is Nothing Then
                productPricing.RetrievedPrices = New Generic.Dictionary(Of String, Talent.Common.DEWebPrice)()
            End If
            Return productPricing
        End Function

        Private Shared Function GetUsersAttributeList() As String
            Dim attribList As String = ""

            If Not CType(HttpContext.Current.Profile, TalentProfile).IsAnonymous Then
                attribList = CType(HttpContext.Current.Profile, TalentProfile).User.Details.ATTRIBUTES_LIST
            End If

            Return attribList
        End Function

        Public Shared Function GetPriceListType1(ByVal parmCountry As String, ByVal parmVatcode As String) As String
            Dim priceList As String = String.Empty
            Dim countryTable As New TalentApplicationVariablesTableAdapters.tbl_countryTableAdapter
            Dim dt As New Data.DataTable
            dt = countryTable.GetDataByCountryCode(parmCountry)
            If dt.Rows.Count > 0 Then
                If parmVatcode = String.Empty Then
                    priceList = dt.Rows(0)("REGISTRATION_PRICELIST_1").ToString
                Else
                    priceList = dt.Rows(0)("REGISTRATION_PRICELIST_2").ToString
                End If
            End If

            Return priceList
        End Function

#End Region

        Public Shared Function GetBranchType1(ByVal parmCountry As String, ByVal parmCompanyName As String, ByVal parmVatcode As String) As String
            Dim branch As String = String.Empty
            Dim countryTable As New TalentApplicationVariablesTableAdapters.tbl_countryTableAdapter
            Dim dt As New Data.DataTable
            dt = countryTable.GetDataByCountryCode(parmCountry)
            If dt.Rows.Count > 0 Then
                If parmCompanyName = String.Empty Then
                    '-----------------------------------------------
                    ' Registration branch 3 is the Restricted branch
                    ' (restrictions to payment types applied)
                    '-----------------------------------------------
                    branch = dt.Rows(0)("REGISTRATION_BRANCH_3").ToString
                Else
                    If parmVatcode = String.Empty Then
                        branch = dt.Rows(0)("REGISTRATION_BRANCH_1").ToString
                    Else
                        branch = dt.Rows(0)("REGISTRATION_BRANCH_2").ToString
                    End If
                End If
            End If

            Return branch
        End Function

        Public Shared Function GetPropertyNames(ByVal obj As Object) As ArrayList
            Dim al As New ArrayList
            Dim inf() As System.Reflection.PropertyInfo = obj.GetType.GetProperties
            For Each info As System.Reflection.PropertyInfo In inf
                al.Add(info.Name)
            Next
            Return al
        End Function

        Public Shared Function PopulateProperties(ByVal propertiesList As ArrayList, ByVal myRow As Data.DataRow, ByVal objectToPopulate As Object) As Object
            For i As Integer = 0 To propertiesList.Count - 1
                If myRow.Table.Columns.Contains(propertiesList(i)) Then
                    CallByName(objectToPopulate, propertiesList(i), CallType.Set, CheckForDBNull(myRow(propertiesList(i))))
                Else
                    'If the column does not exist, handle any properties on the class that we know of
                    Select Case propertiesList(i).ToString
                        Case Is = "MODULE_"
                            'Module is a KEYWORD in vb.net so the property name could not be called the same as the DB field name.
                            CallByName(objectToPopulate, propertiesList(i), CallType.Set, Utilities.CheckForDBNull_String(myRow("MODULE")))
                        Case Else
                            'Handle all other occurances
                    End Select
                End If
            Next

            Return objectToPopulate
        End Function

        Public Shared Function GetAllString() As String
            Return Talent.Common.Utilities.GetAllString
        End Function

        Public Shared Function SuppressDefaultDate(ByVal testDate As String) As String
            Dim returnDate As String = String.Empty
            If testDate <> "01/01/1900 00:00:00" AndAlso testDate.Length > 10 Then
                returnDate = testDate.Substring(0, 10)
            Else

            End If
            Return returnDate
        End Function

        Public Shared Function IsTicketingFee(ByVal module1 As String, ByVal product As String, ByVal feeCategory As String) As Boolean
            IsTicketingFee = False
            If UCase(module1) = GlobalConstants.BASKETMODULETICKETING.ToUpper Then
                Dim ecomDefaultValues As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults()
                If feeCategory IsNot Nothing AndAlso feeCategory.Trim.Length > 0 Then
                    IsTicketingFee = True
                ElseIf product.ToUpper = ecomDefaultValues.CashBackFeeCode.ToUpper Then
                    IsTicketingFee = True
                End If
            End If
            If UCase(module1) = "TICKETING" AndAlso feeCategory IsNot Nothing AndAlso feeCategory.Trim.Length > 0 Then
                IsTicketingFee = True
            End If
            Return IsTicketingFee
        End Function

        Public Shared Function IsTicketingFee(ByVal cashbackFeeCode As String, ByVal module1 As String, ByVal product As String, ByVal feeCategory As String) As Boolean
            Dim isFee As Boolean = False
            If UCase(module1) = "TICKETING" Then
                If feeCategory IsNot Nothing AndAlso feeCategory.Trim.Length > 0 Then
                    isFee = True
                ElseIf product.ToUpper = cashbackFeeCode.ToUpper Then
                    isFee = True
                End If
            End If
            Return isFee
        End Function

        Public Shared Function IsTicketingFee1(ByVal module1 As String, ByVal product As String) As Boolean
            IsTicketingFee1 = False
            If UCase(module1) = "TICKETING" Then
                If product = "ATFEE" OrElse product = "WSFEE" OrElse product = "CRFEE" _
                    OrElse product = "BKFEE" OrElse product = "DDFEE" OrElse product = "BBFEE" Then
                    IsTicketingFee1 = True
                End If
            End If
            Return IsTicketingFee1
        End Function

        Public Shared Function IsTicketingDBforRetailOrders() As Boolean

            Dim destinationDatabase As String
            Dim sessionKey As String = "IsTicketingDBforRetailOrders-CreateOrder-" & TalentCache.GetBusinessUnit & "-" & TalentCache.GetPartner(HttpContext.Current.Profile)

            If HttpContext.Current.Session.Item(sessionKey) IsNot Nothing Then
                destinationDatabase = CType(HttpContext.Current.Session.Item(sessionKey), String)
            Else
                Dim _TDataObjects = New Talent.Common.TalentDataObjects()
                Dim settings As New Talent.Common.DESettings
                settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                settings.DestinationDatabase = "SQL2005"
                _TDataObjects.Settings = settings
                destinationDatabase = _TDataObjects.AppVariableSettings.TblBuModuleDatabase.GetByModuleName("CreateOrder", TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile), GlobalConstants.STAR_ALL)

                HttpContext.Current.Session.Add(sessionKey, destinationDatabase)
            End If

            If destinationDatabase = GlobalConstants.TICKETING_DATABASE Then
                Return True
            Else
                Return False
            End If

        End Function

        Public Shared Function BasketContentTypeWithOverride() As String
            'Forward to payment processing
            Dim basketType As String
            If Talent.eCommerce.Utilities.IsTicketingDBforRetailOrders() Then
                basketType = "T"
            Else
                basketType = CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType
            End If
            Return basketType
        End Function

        Public Shared Function IsAdhocFee1(ByVal feeCode As String, Optional ByVal _module As String = "TICKETING") As Boolean
            Dim adhocFee As Boolean = False

            If _module.ToUpper = "TICKETING" Then
                Dim _TDataObjects = New Talent.Common.TalentDataObjects()
                Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
                settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                settings.DestinationDatabase = "SQL2005"
                _TDataObjects.Settings = settings
                Dim dtAdhocFees As DataTable = _TDataObjects.PaymentSettings.TblAdhocFees.GetByBUPartnerLang(TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile), Talent.Common.Utilities.GetDefaultLanguage, True)

                If dtAdhocFees.Rows.Count > 0 Then
                    For Each row As DataRow In dtAdhocFees.Rows
                        If feeCode.Trim.Equals(row("FEE_CODE").ToString.Trim) Then
                            adhocFee = True
                            Exit For
                        End If
                    Next
                End If
            End If

            Return adhocFee
        End Function

        Public Shared Function IsAgent() As Boolean
            Dim agentProfile As New Agent
            Return agentProfile.IsAgent
        End Function

        ''' <summary>
        ''' Handle Ticketing Basket Errors with string replacement
        ''' </summary>
        ''' <param name="errMsg">The error message object</param>
        ''' <param name="tbi">The basket item containing the message</param>
        ''' <returns>Formatted error message string</returns>
        ''' <remarks></remarks>
        Public Shared Function TicketingBasketErrors(ByVal errMsg As Talent.Common.TalentErrorMessages, ByVal tbi As TalentBasketItem) As String
            Dim sErrorText As String = String.Empty
            If tbi.MODULE_.Trim.ToUpper = "TICKETING" Then
                'Construct the key
                Dim sKey As String
                If IsAgent() AndAlso tbi.STOCK_ERROR_CODE.Trim = "G" AndAlso tbi.STOCK_REQUESTED > 0 Then
                    sKey = "G" & tbi.STOCK_REQUESTED.ToString.TrimStart("0")
                Else
                    sKey = tbi.STOCK_ERROR_CODE.Trim
                End If

                'Try to retrieve a product type error message first
                sErrorText = errMsg.GetErrorMessage("TicketingBasketErrorText", Talent.eCommerce.Utilities.GetCurrentPageName, sKey & "Type" & tbi.PRODUCT_TYPE, False).ERROR_MESSAGE

                'Retrieve the general error message when no product type message exists
                If sErrorText.Trim.Equals("") Then
                    sErrorText = errMsg.GetErrorMessage("TicketingBasketErrorText", Talent.eCommerce.Utilities.GetCurrentPageName, sKey).ERROR_MESSAGE
                End If

                'Replace the reserved words
                sErrorText = sErrorText.Replace("<<<transaction_limit>>>", tbi.STOCK_LIMIT.TrimStart("0"))
                Select Case tbi.STOCK_ERROR_CODE.Trim
                    Case Is = "M"
                        sErrorText = sErrorText.Replace("<<<pre_req>>>", tbi.STOCK_ERROR_DESCRIPTION.Trim)
                End Select
                sErrorText = sErrorText.Replace("<<<product>>>", tbi.PRODUCT_DESCRIPTION1.Trim)
                sErrorText = sErrorText.Replace("<<<user>>>", tbi.LOGINID.TrimStart("0"))
                sErrorText = sErrorText.Replace("<<<loyality_required>>>", tbi.STOCK_LIMIT.TrimStart("0"))
                sErrorText = sErrorText.Replace("<<<user_limit>>>", tbi.STOCK_LIMIT.TrimStart("0"))
                sErrorText = sErrorText.Replace("<<<user_current>>>", tbi.STOCK_REQUESTED.TrimStart("0"))
                sErrorText = sErrorText.Replace("<<<seat>>>", tbi.SEAT.Trim)
                sErrorText = sErrorText.Replace("<<<error>>>", tbi.STOCK_ERROR_DESCRIPTION.Trim)
            End If
            Return sErrorText
        End Function


        Public Shared Function GetTicketingProductPriceBands(ByVal productCode As String, ByVal priceCode As String, ByVal cacheMinutes As Integer, ByVal cacheDependencyPath As String) As DataTable
            Dim errObj As New ErrorObj
            Dim settingsEntity As New DESettings
            Dim talProduct As New TalentProduct
            Dim dtProductPriceBands As New DataTable
            settingsEntity.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settingsEntity.BusinessUnit = TalentCache.GetBusinessUnit()
            settingsEntity.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            settingsEntity.Cacheing = True
            settingsEntity.CacheTimeMinutes = cacheMinutes
            settingsEntity.CacheDependencyPath = cacheDependencyPath
            talProduct.Settings() = settingsEntity
            talProduct.De.ProductCode = Utilities.CheckForDBNull_String(productCode)
            talProduct.De.Src = "W"
            talProduct.De.PriceCode = priceCode
            errObj = talProduct.ProductDetails
            Return dtProductPriceBands
        End Function

        Public Shared Function GetTicketingProductDetailsTemplateID(ByVal productCode As String, ByVal priceCode As String, ByVal cacheMinutes As Integer, ByVal cacheDependencyPath As String) As String
            Dim errObj As New ErrorObj
            Dim settingsEntity As New DESettings
            Dim talProduct As New TalentProduct
            Dim dtProductProductDetails As New DataTable
            settingsEntity.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settingsEntity.BusinessUnit = TalentCache.GetBusinessUnit()
            settingsEntity.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            settingsEntity.Cacheing = True
            settingsEntity.CacheTimeMinutes = cacheMinutes
            settingsEntity.CacheDependencyPath = cacheDependencyPath
            talProduct.Settings() = settingsEntity
            talProduct.De.ProductCode = Utilities.CheckForDBNull_String(productCode)
            talProduct.De.Src = "W"
            talProduct.De.PriceCode = priceCode
            errObj = talProduct.ProductDetails
            If talProduct.ResultDataSet.Tables(2).Rows.Count > 0 AndAlso Not (String.IsNullOrWhiteSpace(talProduct.ResultDataSet.Tables(2).Rows(0).Item("TemplateID"))) Then
                Return talProduct.ResultDataSet.Tables(2).Rows(0).Item("TemplateID")
            Else
                Return "0"
            End If
        End Function

        Public Shared Function GetSupporterSportDDL() As DataTable
            Dim dt As New DataTable

            Dim cacheKey As String = TalentCache.GetBusinessUnit & "_" & TalentCache.GetPartner(HttpContext.Current.Profile) & "_GetSupporterSportDDL_" & GetCurrentLanguage()

            If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                dt = CType(HttpContext.Current.Cache.Item(cacheKey), DataTable)
            Else
                Const selectStr As String = " SELECT * FROM tbl_sport_bu s " & _
                                            "   INNER JOIN tbl_sport_lang sl " & _
                                            "   ON s.SPORT_CODE = sl.SPORT_CODE " & _
                                            " WHERE s.BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                            " AND s.PARTNER = @PARTNER " & _
                                            " AND sl.LANGUAGE_CODE = @LANGUAGE_CODE " & _
                                            " ORDER BY sl.DESCRIPTION "

                Dim cmd As New SqlCommand(selectStr, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))


                With cmd.Parameters
                    .Clear()
                    .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                    .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(HttpContext.Current.Profile)
                    .Add("@LANGUAGE_CODE", SqlDbType.NVarChar).Value = GetCurrentLanguage()
                End With

                Try
                    cmd.Connection.Open()
                Catch ex As Exception
                End Try

                If cmd.Connection.State = ConnectionState.Open Then
                    Try
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        If dt.Rows.Count = 0 Then
                            cmd.Parameters("@PARTNER").Value = GetAllString()
                            da.Fill(dt)
                            If dt.Rows.Count = 0 Then
                                cmd.Parameters("@BUSINESS_UNIT").Value = GetAllString()
                                da.Fill(dt)
                            End If
                        End If
                    Catch ex As Exception
                    End Try
                End If

                Try
                    If cmd.Connection.State = ConnectionState.Open Then
                        cmd.Connection.Close()
                    End If
                Catch ex As Exception
                End Try

                HttpContext.Current.Cache.Add(cacheKey, dt, Nothing, Now.AddMinutes(30), TimeSpan.Zero, CacheItemPriority.Normal, Nothing)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)

            End If

            Return dt
        End Function

        Public Shared Function GetSupporterSportTeamDDL() As DataTable
            Dim dt As New DataTable

            Dim cacheKey As String = TalentCache.GetBusinessUnit & "_" & TalentCache.GetPartner(HttpContext.Current.Profile) & "_GetSupporterSportTeamDDL_" & GetCurrentLanguage()

            If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                dt = CType(HttpContext.Current.Cache.Item(cacheKey), DataTable)
            Else
                Const selectStr As String = " SELECT * FROM tbl_sport_team_bu s " & _
                                            "   INNER JOIN tbl_team_lang sl " & _
                                            "   ON s.TEAM_CODE = sl.TEAM_CODE " & _
                                            " WHERE s.BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                            " AND s.PARTNER = @PARTNER " & _
                                            " AND sl.LANGUAGE_CODE = @LANGUAGE_CODE " & _
                                            " ORDER BY sl.DESCRIPTION "


                Dim cmd As New SqlCommand(selectStr, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))


                With cmd.Parameters
                    .Clear()
                    .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                    .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(HttpContext.Current.Profile)
                    .Add("@LANGUAGE_CODE", SqlDbType.NVarChar).Value = GetCurrentLanguage()
                End With

                Try
                    cmd.Connection.Open()
                Catch ex As Exception
                End Try

                If cmd.Connection.State = ConnectionState.Open Then
                    Try
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        If dt.Rows.Count = 0 Then
                            cmd.Parameters("@PARTNER").Value = GetAllString()
                            da.Fill(dt)
                            If dt.Rows.Count = 0 Then
                                cmd.Parameters("@BUSINESS_UNIT").Value = GetAllString()
                                da.Fill(dt)
                            End If
                        End If
                    Catch ex As Exception
                    End Try
                End If

                Try
                    If cmd.Connection.State = ConnectionState.Open Then
                        cmd.Connection.Close()
                    End If
                Catch ex As Exception
                End Try

                HttpContext.Current.Cache.Add(cacheKey, dt, Nothing, Now.AddMinutes(30), TimeSpan.Zero, CacheItemPriority.Normal, Nothing)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)

            End If

            Return dt
        End Function

        Public Shared Function GetSupporterSportTeamClubDDL() As DataTable
            Dim dt As New DataTable

            Dim cacheKey As String = TalentCache.GetBusinessUnit & "_" & TalentCache.GetPartner(HttpContext.Current.Profile) & "_GetSupporterSportTeamClubDDL_" & GetCurrentLanguage()

            If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                dt = CType(HttpContext.Current.Cache.Item(cacheKey), DataTable)
            Else
                Const selectStr As String = " SELECT * FROM tbl_sport_team_club_bu s " & _
                                            "   INNER JOIN tbl_supporter_club_lang sl " & _
                                            "   ON s.SC_CODE = sl.SC_CODE " & _
                                            " WHERE s.BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                            " AND s.PARTNER = @PARTNER " & _
                                            " AND sl.LANGUAGE_CODE = @LANGUAGE_CODE " & _
                                            " ORDER BY sl.DESCRIPTION "


                Dim cmd As New SqlCommand(selectStr, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))


                With cmd.Parameters
                    .Clear()
                    .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                    .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(HttpContext.Current.Profile)
                    .Add("@LANGUAGE_CODE", SqlDbType.NVarChar).Value = GetCurrentLanguage()
                End With

                Try
                    cmd.Connection.Open()
                Catch ex As Exception
                End Try

                If cmd.Connection.State = ConnectionState.Open Then
                    Try
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        If dt.Rows.Count = 0 Then
                            cmd.Parameters("@PARTNER").Value = GetAllString()
                            da.Fill(dt)
                            If dt.Rows.Count = 0 Then
                                cmd.Parameters("@BUSINESS_UNIT").Value = GetAllString()
                                da.Fill(dt)
                            End If
                        End If
                    Catch ex As Exception
                    End Try
                End If

                Try
                    If cmd.Connection.State = ConnectionState.Open Then
                        cmd.Connection.Close()
                    End If
                Catch ex As Exception
                End Try

                HttpContext.Current.Cache.Add(cacheKey, dt, Nothing, Now.AddMinutes(30), TimeSpan.Zero, CacheItemPriority.Normal, Nothing)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
            End If

            Return dt
        End Function

        Public Shared Function GetFavouriteTeamDDL() As DataTable
            Dim dt As New DataTable

            Dim cacheKey As String = TalentCache.GetBusinessUnit & "_" & TalentCache.GetPartner(HttpContext.Current.Profile) & "_GetFavouriteTeamDDL_" & GetCurrentLanguage()

            If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                dt = CType(HttpContext.Current.Cache.Item(cacheKey), DataTable)
            Else
                Const selectStr As String = " SELECT * FROM tbl_sport_team_bu s " & _
                                            "   INNER JOIN tbl_team_lang sl " & _
                                            "   ON s.TEAM_CODE = sl.TEAM_CODE " & _
                                            " WHERE s.BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                            " AND s.PARTNER = @PARTNER " & _
                                            " AND sl.LANGUAGE_CODE = @LANGUAGE_CODE " & _
                                            " ORDER BY sl.DESCRIPTION "


                Dim cmd As New SqlCommand(selectStr, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))


                With cmd.Parameters
                    .Clear()
                    .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                    .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(HttpContext.Current.Profile)
                    .Add("@LANGUAGE_CODE", SqlDbType.NVarChar).Value = GetCurrentLanguage()
                End With

                Try
                    cmd.Connection.Open()
                Catch ex As Exception
                End Try

                If cmd.Connection.State = ConnectionState.Open Then
                    Try
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        If dt.Rows.Count = 0 Then
                            cmd.Parameters("@PARTNER").Value = GetAllString()
                            da.Fill(dt)
                            If dt.Rows.Count = 0 Then
                                cmd.Parameters("@BUSINESS_UNIT").Value = GetAllString()
                                da.Fill(dt)
                            End If
                        End If
                    Catch ex As Exception
                    End Try
                End If

                Try
                    If cmd.Connection.State = ConnectionState.Open Then
                        cmd.Connection.Close()
                    End If
                Catch ex As Exception
                End Try

                HttpContext.Current.Cache.Add(cacheKey, dt, Nothing, Now.AddMinutes(30), TimeSpan.Zero, CacheItemPriority.Normal, Nothing)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)

            End If

            Return dt
        End Function

        Public Shared Function GetPreferredContactMethodDDL() As DataTable
            Dim dt As New DataTable

            Dim cacheKey As String = TalentCache.GetBusinessUnit & "_" & TalentCache.GetPartner(HttpContext.Current.Profile) & "_GetPreferredContactMethodDDL_" & GetCurrentLanguage()

            If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                dt = CType(HttpContext.Current.Cache.Item(cacheKey), DataTable)
            Else
                Const selectStr As String = " SELECT * FROM tbl_contact_method_bu s " & _
                                            "   INNER JOIN tbl_contact_method_lang sl " & _
                                            "   ON s.CONTACT_CODE = sl.CONTACT_CODE " & _
                                            " WHERE s.BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                            " AND s.PARTNER = @PARTNER " & _
                                            " AND sl.LANGUAGE_CODE = @LANGUAGE_CODE " & _
                                            " ORDER BY sl.DESCRIPTION "


                Dim cmd As New SqlCommand(selectStr, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))


                With cmd.Parameters
                    .Clear()
                    .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                    .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(HttpContext.Current.Profile)
                    .Add("@LANGUAGE_CODE", SqlDbType.NVarChar).Value = GetCurrentLanguage()
                End With

                Try
                    cmd.Connection.Open()
                Catch ex As Exception
                End Try

                If cmd.Connection.State = ConnectionState.Open Then
                    Try
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                        If dt.Rows.Count = 0 Then
                            cmd.Parameters("@PARTNER").Value = GetAllString()
                            da.Fill(dt)
                            If dt.Rows.Count = 0 Then
                                cmd.Parameters("@BUSINESS_UNIT").Value = GetAllString()
                                da.Fill(dt)
                            End If
                        End If
                    Catch ex As Exception
                    End Try
                End If

                Try
                    If cmd.Connection.State = ConnectionState.Open Then
                        cmd.Connection.Close()
                    End If
                Catch ex As Exception
                End Try

                HttpContext.Current.Cache.Add(cacheKey, dt, Nothing, Now.AddMinutes(30), TimeSpan.Zero, CacheItemPriority.Normal, Nothing)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)

            End If

            Return dt
        End Function

        Public Shared Function ShowPrices(ByVal profile As TalentProfile) As Boolean
            Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues
            def = moduleDefaults.GetDefaults
            Dim enablePriceView As Boolean = def.enablePriceView
            If enablePriceView Then
                'Only get the show price value from th partner/partner user if its set to true in module default
                If Not profile.PartnerInfo.Details Is Nothing Then
                    'If the partner_user flag is set to true we use the flag set in the partner table
                    If Not profile.PartnerInfo.Details.Enable_Price_View Then
                        enablePriceView = False
                        'Neither the partner_user flag or the module defaults are set not to show prices therefore we use
                        'the partner user flag value
                        'enablePriceView = Boolean.Parse(profile.User.Details.Enable_Price_View)
                        'Else : enablePriceView = Boolean.Parse(profile.PartnerInfo.Details.Enable_Price_View)
                        '  End If
                        '  Else : enablePriceView = def.enablePriceView
                    End If
                End If

            End If

            Return enablePriceView
        End Function

        Public Shared Function GetProductInfo(ByVal productcode As String) As Data.DataTable
            Dim err As Talent.Common.ErrorObj
            Dim product As Data.DataTable

            Dim prodInfo As New Talent.Common.DEProductInfo(TalentCache.GetBusinessUnit, _
                                                                    TalentCache.GetPartner(HttpContext.Current.Profile), _
                                                                    productcode, _
                                                                    Talent.Common.Utilities.GetDefaultLanguage)

            Dim DBProdInfo As New Talent.Common.DBProductInfo(prodInfo)
            DBProdInfo.Settings = Talent.eCommerce.Utilities.GetSettingsObject()

            'Get the product info
            '------------------------
            err = DBProdInfo.AccessDatabase()

            If Not err.HasError Then
                product = DBProdInfo.ResultDataSet.Tables("ProductInformation")
            Else
                'ERROR: could not retrieve product info
                product = Nothing
            End If
            Return product
        End Function

        Public Shared Function GetPreferredDeliveryDates(ByVal profile As TalentProfile, ByVal deliveryZoneCode As String, ByVal deliveryZoneType As String) As DataTable
            Dim dtPreferrredDeliveryDates As New DataTable

            If Not profile.IsAnonymous Then
                Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
                Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
                Try
                    Dim deliveryDate As New Talent.Common.TalentDeliveryDate
                    Dim DEdelDate As New Talent.Common.DEDeliveryDate(Now)
                    DEdelDate.DaysWithinPreferredDate = def.PreferredDeliveryDateMaxDays
                    Dim settings As New Talent.Common.DESettings
                    settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                    settings.BusinessUnit = TalentCache.GetBusinessUnit
                    settings.Partner = TalentCache.GetPartner(profile)
                    settings.StoredProcedureGroup = Utilities.GetStoredProcedureGroup

                    If def.CalculateStockLeadTime Then
                        DEdelDate.StockLeadTime = GetStockLeadTime(profile)
                    Else
                        DEdelDate.StockLeadTime = 0
                    End If

                    ' If home delivery then set delivery days
                    If Utilities.IsPartnerHomeDeliveryType(profile) Then
                        SetHomeDeliverySettings(DEdelDate, profile)
                    Else
                        DEdelDate.HomeDelivery = False
                    End If
                    DEdelDate.CarrierCode = profile.PartnerInfo.Details.CARRIER_CODE

                    settings.AccountNo1 = profile.User.Details.Account_No_1
                    settings.AccountNo2 = profile.User.Details.Account_No_2
                    settings.Cacheing = False
                    If String.IsNullOrEmpty(settings.AccountNo1) Then
                        settings.AccountNo1 = profile.PartnerInfo.Details.Account_No_1
                        settings.AccountNo2 = profile.PartnerInfo.Details.Account_No_2
                    End If
                    deliveryDate.DeliveryDate = DEdelDate
                    deliveryDate.Settings = settings
                    Dim err As Talent.Common.ErrorObj = deliveryDate.GetPreferredDeliveryDates(deliveryZoneCode, deliveryZoneType)
                    If deliveryDate.ResultDataSet IsNot Nothing AndAlso deliveryDate.ResultDataSet.Tables.Count > 0 Then
                        dtPreferrredDeliveryDates = deliveryDate.ResultDataSet.Tables("PreferredDeliveryDates")
                    End If
                    Dim dtDeliveryDate As Data.DataTable = deliveryDate.ResultDataSet.Tables("DeliveryDateHeader")
                Catch ex As Exception
                End Try
            End If
            Return dtPreferrredDeliveryDates
        End Function

        Public Shared Function GetDeliveryDate(ByVal profile As TalentProfile, Optional ByVal deliveryZoneCode As String = "", Optional ByVal deliveryZoneType As String = "") As Date

            Dim projectedDate As Date = Date.MinValue

            If Not profile.IsAnonymous Then

                Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
                Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults

                If def.CalculateDeliveryDate Then
                    Try
                        Dim deliveryDate As New Talent.Common.TalentDeliveryDate
                        Dim DEdelDate As New Talent.Common.DEDeliveryDate(Now)
                        Dim settings As New Talent.Common.DESettings
                        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                        settings.BusinessUnit = TalentCache.GetBusinessUnit

                        If def.CalculateStockLeadTime Then
                            DEdelDate.StockLeadTime = GetStockLeadTime(profile)
                        Else
                            DEdelDate.StockLeadTime = 0
                        End If

                        DEdelDate.CarrierCode = profile.PartnerInfo.Details.CARRIER_CODE

                        settings.StoredProcedureGroup = Utilities.GetStoredProcedureGroup
                        ' If home delivery then set delivery days
                        If Utilities.IsBasketHomeDelivery(profile) Then
                            SetHomeDeliverySettings(DEdelDate, profile)
                        Else
                            DEdelDate.HomeDelivery = False
                        End If

                        settings.AccountNo1 = profile.User.Details.Account_No_1
                        settings.AccountNo2 = profile.User.Details.Account_No_2

                        If String.IsNullOrEmpty(settings.AccountNo1) Then
                            settings.AccountNo1 = profile.PartnerInfo.Details.Account_No_1
                            settings.AccountNo2 = profile.PartnerInfo.Details.Account_No_2
                        End If

                        deliveryDate.DeliveryDate = DEdelDate
                        deliveryDate.Settings = settings

                        Dim err As Talent.Common.ErrorObj = deliveryDate.GetDeliveryDate(deliveryZoneCode, deliveryZoneType)

                        Dim dtDeliveryDate As Data.DataTable = deliveryDate.ResultDataSet.Tables("DeliveryDateHeader")

                        If dtDeliveryDate.Rows.Count > 0 Then
                            projectedDate = CDate(dtDeliveryDate.Rows(0)("ProjectedDeliveryDate"))
                        End If
                    Catch ex As Exception
                    End Try
                End If
            End If
            Return projectedDate
        End Function

        ''' <summary>
        ''' Get delivery date for an individual line item
        ''' </summary>
        ''' <param name="profile">the talent profile</param>
        ''' <param name="lineitem">the line item to get delivery date for</param>
        ''' <returns>delivery date for item</returns>
        ''' <remarks></remarks>
        Public Shared Function GetDeliveryDateLine(ByVal profile As TalentProfile, ByVal lineItem As String, Optional ByVal deliveryZoneCode As String = "", Optional ByVal deliveryZoneType As String = "") As Date

            Dim projectedDate As Date = Date.MinValue

            If Not profile.IsAnonymous Then

                Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
                Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults

                If def.CalculateDeliveryDate Then
                    Try
                        Dim deliveryDate As New Talent.Common.TalentDeliveryDate
                        Dim DEdelDate As New Talent.Common.DEDeliveryDate(Now)
                        Dim settings As New Talent.Common.DESettings
                        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                        settings.BusinessUnit = TalentCache.GetBusinessUnit

                        If def.CalculateStockLeadTime Then
                            ' by item only
                            Dim stockLeadDays As Integer = 0
                            Dim stockLeadDaysCompare As Integer = 0
                            Dim strStockLeadDays As String = String.Empty
                            Dim reStockCode As String = String.Empty
                            If Stock.GetStockBalance(lineItem) > 0 Then
                                'in stock
                            Else
                                Stock.GetNoStockDescription(lineItem, strStockLeadDays, reStockCode)

                                Try
                                    stockLeadDaysCompare = CInt(strStockLeadDays)
                                    If stockLeadDaysCompare > stockLeadDays Then
                                        stockLeadDays = stockLeadDaysCompare
                                    End If

                                Catch ex As Exception

                                End Try

                            End If

                            DEdelDate.StockLeadTime = stockLeadDays
                        Else
                            DEdelDate.StockLeadTime = 0
                        End If


                        DEdelDate.CarrierCode = profile.PartnerInfo.Details.CARRIER_CODE

                        settings.StoredProcedureGroup = Utilities.GetStoredProcedureGroup
                        ' If home delivery then set delivery days
                        If Utilities.IsBasketHomeDelivery(profile) Then
                            SetHomeDeliverySettings(DEdelDate, profile)
                        Else
                            DEdelDate.HomeDelivery = False
                        End If

                        settings.AccountNo1 = profile.User.Details.Account_No_1
                        settings.AccountNo2 = profile.User.Details.Account_No_2

                        If String.IsNullOrEmpty(settings.AccountNo1) Then
                            settings.AccountNo1 = profile.PartnerInfo.Details.Account_No_1
                            settings.AccountNo2 = profile.PartnerInfo.Details.Account_No_2
                        End If

                        deliveryDate.DeliveryDate = DEdelDate
                        deliveryDate.Settings = settings

                        Dim err As Talent.Common.ErrorObj = deliveryDate.GetDeliveryDate(deliveryZoneCode, deliveryZoneType)

                        Dim dtDeliveryDate As Data.DataTable = deliveryDate.ResultDataSet.Tables("DeliveryDateHeader")

                        If dtDeliveryDate.Rows.Count > 0 Then
                            projectedDate = CDate(dtDeliveryDate.Rows(0)("ProjectedDeliveryDate"))
                        End If
                    Catch ex As Exception
                    End Try
                End If
            End If
            Return projectedDate
        End Function
        Public Shared Sub SetHomeDeliverySettings(ByRef deDelDate As DEDeliveryDate, ByVal profile As TalentProfile)

            deDelDate.HomeDelivery = True
            If profile.PartnerInfo.Details.CARRIER_CODE <> String.Empty Then

                Dim talentDataObjects = New Talent.Common.TalentDataObjects()
                Dim settings2 As Talent.Common.DESettings = New Talent.Common.DESettings()
                Dim dt As Data.DataTable
                settings2.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                settings2.DestinationDatabase = "SQL2005"
                talentDataObjects.Settings = settings2
                dt = talentDataObjects.DeliverySettings.TblCarrier.GetByCarrier(profile.PartnerInfo.Details.CARRIER_CODE)
                If dt.Rows.Count > 0 Then
                    If CBool(dt.Rows(0)("DELIVER_MONDAY")) Then
                        deDelDate.HomeDeliveryDays = "Y"
                    Else
                        deDelDate.HomeDeliveryDays = "N"
                    End If
                    If CBool(dt.Rows(0)("DELIVER_TUESDAY")) Then
                        deDelDate.HomeDeliveryDays = deDelDate.HomeDeliveryDays + "Y"
                    Else
                        deDelDate.HomeDeliveryDays = deDelDate.HomeDeliveryDays + "N"
                    End If
                    If CBool(dt.Rows(0)("DELIVER_WEDNESDAY")) Then
                        deDelDate.HomeDeliveryDays = deDelDate.HomeDeliveryDays + "Y"
                    Else
                        deDelDate.HomeDeliveryDays = deDelDate.HomeDeliveryDays + "N"
                    End If
                    If CBool(dt.Rows(0)("DELIVER_THURSDAY")) Then
                        deDelDate.HomeDeliveryDays = deDelDate.HomeDeliveryDays + "Y"
                    Else
                        deDelDate.HomeDeliveryDays = deDelDate.HomeDeliveryDays + "N"
                    End If
                    If CBool(dt.Rows(0)("DELIVER_FRIDAY")) Then
                        deDelDate.HomeDeliveryDays = deDelDate.HomeDeliveryDays + "Y"
                    Else
                        deDelDate.HomeDeliveryDays = deDelDate.HomeDeliveryDays + "N"
                    End If
                    If CBool(dt.Rows(0)("DELIVER_SATURDAY")) Then
                        deDelDate.HomeDeliveryDays = deDelDate.HomeDeliveryDays + "Y"
                    Else
                        deDelDate.HomeDeliveryDays = deDelDate.HomeDeliveryDays + "N"
                    End If
                    If CBool(dt.Rows(0)("DELIVER_SUNDAY")) Then
                        deDelDate.HomeDeliveryDays = deDelDate.HomeDeliveryDays + "Y"
                    Else
                        deDelDate.HomeDeliveryDays = deDelDate.HomeDeliveryDays + "N"
                    End If

                End If
            End If

        End Sub

        Public Shared Function GetDeliveryDateQuickOrder(ByVal profile As TalentProfile, ByVal basket As TalentBasket, ByVal IsHomeDelivery As Boolean) As Date

            Dim projectedDate As Date = Date.MinValue

            If Not profile.IsAnonymous Then

                Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
                Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults

                If def.CalculateDeliveryDate Then
                    Try
                        Dim deliveryDate As New Talent.Common.TalentDeliveryDate
                        Dim DEdelDate As New Talent.Common.DEDeliveryDate(Now)
                        Dim settings As New Talent.Common.DESettings
                        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                        settings.BusinessUnit = TalentCache.GetBusinessUnit

                        If def.CalculateStockLeadTime Then
                            DEdelDate.StockLeadTime = GetStockLeadTime(profile)
                        Else
                            DEdelDate.StockLeadTime = 0
                        End If

                        ' If home delivery then set delivery days
                        If IsHomeDelivery Then
                            SetHomeDeliverySettings(DEdelDate, profile)
                        Else
                            DEdelDate.HomeDelivery = False
                        End If

                        settings.StoredProcedureGroup = Utilities.GetStoredProcedureGroup

                        DEdelDate.CarrierCode = profile.PartnerInfo.Details.CARRIER_CODE

                        If IsHomeDelivery Then
                            DEdelDate.HomeDelivery = True
                        Else
                            DEdelDate.HomeDelivery = False
                        End If

                        settings.AccountNo1 = profile.User.Details.Account_No_1
                        settings.AccountNo2 = profile.User.Details.Account_No_2

                        If String.IsNullOrEmpty(settings.AccountNo1) Then
                            settings.AccountNo1 = profile.PartnerInfo.Details.Account_No_1
                            settings.AccountNo2 = profile.PartnerInfo.Details.Account_No_2
                        End If

                        deliveryDate.DeliveryDate = DEdelDate
                        deliveryDate.Settings = settings
                        ' Pass in dummy values - they wont be used.
                        Dim err As Talent.Common.ErrorObj = deliveryDate.GetDeliveryDate("Z01", "1")

                        Dim dtDeliveryDate As Data.DataTable = deliveryDate.ResultDataSet.Tables("DeliveryDateHeader")

                        If dtDeliveryDate.Rows.Count > 0 Then
                            projectedDate = CDate(dtDeliveryDate.Rows(0)("ProjectedDeliveryDate"))
                        End If
                    Catch ex As Exception
                    End Try
                End If
            End If
            Return projectedDate
        End Function

        ''' <summary>
        ''' Gets the delivery zone type based on the delivery zone code used on the address table
        ''' </summary>
        ''' <param name="deliveryZoneCode">Delivery Zone Code relating to the address</param>
        ''' <returns><c>Delivery Zone Type</c>from DB table tbl_delivery_zone<c>if not</c>return nothing</returns>
        Public Shared Function GetDeliveryZoneType(ByVal deliveryZoneCode As String) As String
            'Return deliveryZoneType
            Dim businessUnit As String = TalentCache.GetBusinessUnit
            Dim partner As String = TalentCache.GetPartner(HttpContext.Current.Profile)
            Dim deliveryZoneType As String = String.Empty
            If deliveryZoneCode Is Nothing Then deliveryZoneCode = ""
            Dim cacheKey As String = "GETDELIVERYZONETYPE" & _
                                    Talent.Common.Utilities.FixStringLength(businessUnit, 50) & _
                                    Talent.Common.Utilities.FixStringLength(partner, 50) & _
                                    Talent.Common.Utilities.FixStringLength(deliveryZoneCode, 50)

            'Get string from cache. If it isn't in cache get from DB.
            If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                deliveryZoneType = CType(HttpContext.Current.Cache.Item(cacheKey), String)
            Else
                Dim dtDeliveryZone As New DataTable
                Dim sqlConn As New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
                Dim cmd As SqlCommand = Nothing
                Const selectStr As String = "SELECT TOP 1 [DELIVERY_ZONE_TYPE] " & _
                                            "FROM tbl_delivery_zone " & _
                                            "WHERE [BUSINESS_UNIT] = @BUSINESS_UNIT " & _
                                            "AND [PARTNER] = @PARTNER " & _
                                            "AND [DELIVERY_ZONE_CODE] = @DELIVERY_ZONE_CODE"
                cmd = New SqlCommand(selectStr, sqlConn)
                Dim dtr As SqlDataReader

                'open
                Try
                    sqlConn.Open()
                Catch ex As Exception
                End Try

                'Execute
                If sqlConn.State = ConnectionState.Open Then
                    Try
                        With cmd
                            .Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar, 50)).Value = businessUnit
                            .Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar, 100)).Value = partner
                            .Parameters.Add(New SqlParameter("@DELIVERY_ZONE_CODE", SqlDbType.NVarChar, 50)).Value = deliveryZoneCode
                            dtr = .ExecuteReader()
                        End With

                        If dtr.Read Then
                            deliveryZoneType = dtr.Item("DELIVERY_ZONE_TYPE").ToString
                        Else
                            dtr.Close()
                            cmd = New SqlCommand(selectStr, sqlConn)
                            With cmd
                                .Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar, 50)).Value = businessUnit
                                .Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar, 100)).Value = Utilities.GetAllString
                                .Parameters.Add(New SqlParameter("@DELIVERY_ZONE_CODE", SqlDbType.NVarChar, 50)).Value = deliveryZoneCode
                                dtr = .ExecuteReader()
                            End With

                            If dtr.Read Then
                                deliveryZoneType = dtr.Item("DELIVERY_ZONE_TYPE").ToString
                            Else
                                dtr.Close()
                                cmd = New SqlCommand(selectStr, sqlConn)
                                With cmd
                                    .Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar, 50)).Value = Utilities.GetAllString
                                    .Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar, 50)).Value = partner
                                    .Parameters.Add(New SqlParameter("@DELIVERY_ZONE_CODE", SqlDbType.NVarChar, 50)).Value = deliveryZoneCode
                                    dtr = .ExecuteReader()
                                End With

                                If dtr.Read Then
                                    deliveryZoneType = dtr.Item("DELIVERY_ZONE_TYPE").ToString
                                Else
                                    dtr.Close()
                                    cmd = New SqlCommand(selectStr, sqlConn)
                                    With cmd
                                        .Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar, 50)).Value = Utilities.GetAllString
                                        .Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar, 50)).Value = Utilities.GetAllString
                                        .Parameters.Add(New SqlParameter("@DELIVERY_ZONE_CODE", SqlDbType.NVarChar, 50)).Value = deliveryZoneCode
                                        dtr = .ExecuteReader()
                                    End With

                                    If dtr.Read Then
                                        deliveryZoneType = dtr.Item("DELIVERY_ZONE_TYPE").ToString
                                    End If
                                    dtr.Close()
                                End If
                            End If
                        End If

                    Catch ex As Exception
                    End Try
                End If

                'Close
                Try
                    If sqlConn.State = ConnectionState.Open Then
                        sqlConn.Close()
                    End If
                Catch ex As Exception
                End Try

                'Add to cache
                HttpContext.Current.Cache.Insert(cacheKey, _
                    deliveryZoneType, _
                    Nothing, _
                    System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), _
                    Caching.Cache.NoSlidingExpiration)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
            End If
            Return deliveryZoneType
        End Function

        Public Shared Function GetStaticContentPath(ByVal type As String, ByVal pathHierarchy As String) As String
            Dim path As String = String.Empty
            Try
                'Retrieve the ecommerce defaults
                Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
                Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
                Select Case type.ToUpper
                    Case Is = UCase("Javascript")
                        'Javascript settings
                        If def.JavaScriptPath.Trim.Equals("") Then
                            path = Talent.eCommerce.Utilities.GetCurrentApplicationUrl() & "/JavaScript/" & pathHierarchy
                        Else
                            If def.JavaScriptVersion.Trim.Equals("") Then
                                path = def.JavaScriptPath & "/"
                            Else
                                path = def.JavaScriptPath & "/" & def.JavaScriptVersion & "/"
                            End If
                        End If
                End Select

                ' Format the path
                path = (path.Replace("\", "/")).Replace("//", "/")
                path = path.Replace("http:/", "http://")
                path = path.Replace("https:/", "https://")

            Catch ex As Exception
            End Try

            Return path
        End Function
        ''' <summary>
        ''' Formate JS file reference use in page
        ''' </summary>
        ''' <param name="fileName">Js file name</param>
        ''' <param name="path">Path of js file in the format of "/Module/Test/" setting to nothing gets the file from /JavaScript/</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function FormatJavaScriptFileReference(ByVal fileName As String, ByVal path As String) As String
            If String.IsNullOrEmpty(fileName) Then Return String.Empty
            Return "<script src='" & GetStaticContentPath("JavaScript", path) & fileName & "?a=" & getVersionQueryString() & "' language='javascript' type='text/javascript'></script>"
        End Function

        ''' <summary>
        ''' Retrieve the version number for the current version of the software.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function getVersionQueryString() As String
            Dim tDataObjects As New TalentDataObjects
            Dim dtVersion As New DataTable
            Dim moduleDefaults As ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
            Dim defaultValues As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
            tDataObjects.Settings = GetSettingsObject()
            dtVersion = tDataObjects.VersionDeployed.TblVersionDeployed.GetDeployVersion()
            Dim queryString As String = String.Empty
            If Not String.IsNullOrEmpty(defaultValues.StaticContentQueryStringValue) Then
                'Retrieve and use default value if populated
                queryString = defaultValues.StaticContentQueryStringValue
            ElseIf dtVersion.Rows.Count > 0 Then
                'Retrieve the current version as we will use it to add a query string
                queryString = dtVersion.Rows(0).Item("VERSION") & "_" & dtVersion.Rows(0).Item("ID")
            End If
            Return queryString
        End Function

        Public Shared Function DecodeString(ByVal inputString As String) As String
            Dim outputString As String = String.Empty

            Try
                inputString = inputString.Replace("'", "")
                inputString = inputString.Replace("""", "")
                outputString = System.Web.HttpUtility.UrlDecode(inputString)
            Catch ex As Exception
                Return ""
            End Try

            Return outputString
        End Function

        Public Shared Function validateStringForXML(ByVal inputString As String) As String
            Dim outputString As String = String.Empty

            Try
                inputString = inputString.Replace("%", "%25")
                inputString = inputString.Replace("& ", "&amp; ")
                inputString = inputString.Replace("<", "&lt;")
                outputString = inputString.Replace(">", "&gt; ")
            Catch ex As Exception
                Return ""
            End Try

            Return outputString
        End Function

        ''' <summary>
        ''' Check to see if the current partner type is available for home delivery
        ''' If the partner type is both then the type will be set at basket level
        ''' </summary>
        ''' <param name="profile">The current profile object</param>
        ''' <returns>True if the partner is set for home delivery</returns>
        ''' <remarks></remarks>
        Public Shared Function IsPartnerHomeDeliveryType(ByVal profile As TalentProfile) As Boolean
            Dim partnerIsHomeDelivery As Boolean = False
            If Not profile.IsAnonymous Then
                If Not String.IsNullOrEmpty(CheckForDBNull_String(profile.PartnerInfo.Details.PARTNER_TYPE)) Then
                    If profile.PartnerInfo.Details.PARTNER_TYPE = "HOME" Then
                        partnerIsHomeDelivery = True
                    ElseIf profile.PartnerInfo.Details.PARTNER_TYPE = "BOTH" Then
                        partnerIsHomeDelivery = IsBasketHomeDelivery(profile)
                    End If
                End If
            End If
            Return partnerIsHomeDelivery
        End Function
        ''' <summary>
        ''' Check to see if the current basket is home delivery
        ''' </summary>
        ''' <param name="profile">The current profile object</param>
        ''' <returns>True if the basket  is set for home delivery</returns>
        ''' <remarks></remarks>
        Public Shared Function IsBasketHomeDelivery(ByVal profile As TalentProfile) As Boolean
            Dim basketIsHomeDelivery As Boolean = False
            If Not profile.IsAnonymous Then
                For Each tbi As TalentBasketItem In profile.Basket.BasketItems
                    If tbi.PRODUCT_SUB_TYPE = "HOMEDEL" Then
                        basketIsHomeDelivery = True
                        Exit For
                    End If
                Next

            End If
            Return basketIsHomeDelivery
        End Function
        ''' <summary>
        ''' Check to see if the postcode exists in QAS
        ''' </summary>
        ''' <param name="postcode">The postcode to check</param>
        ''' <returns>True if the postcode retrieves at least 1 address from QAS with a 100% match</returns>
        ''' <remarks></remarks>
        Public Shared Function QASPostCodeExists(ByVal postcode As String, ByVal addressingProvider As String) As Boolean
            Dim exists As Boolean = False
            Dim eRoute As com.qas.prowebintegration.Constants.Routes = com.qas.prowebintegration.Constants.Routes.Undefined


            Try

                Dim sServerURL As String = System.Configuration.ConfigurationManager.AppSettings(com.qas.prowebintegration.Constants.KEY_SERVER_URL)
                Dim sUsername As String = System.Configuration.ConfigurationManager.AppSettings(com.qas.prowebintegration.Constants.KEY_USER_NAME)
                Dim sPassword As String = System.Configuration.ConfigurationManager.AppSettings(com.qas.prowebintegration.Constants.KEY_PASSWORD)
                Select Case addressingProvider
                    Case Is = "QAS"
                        Dim m_Picklist As com.qas.proweb.Picklist
                        Dim searchService As com.qas.proweb.QuickAddress = New com.qas.proweb.QuickAddress(sServerURL)

                        searchService.Engine = com.qas.proweb.QuickAddress.EngineTypes.SingleLine
                        searchService.Flatten = True

                        m_Picklist = searchService.Search("GBR", postcode, com.qas.proweb.PromptSet.Types.Optimal).Picklist
                        If m_Picklist.Total > 0 Then
                            ' find one that matches 100%
                            For Each picklist As com.qas.proweb.PicklistItem In m_Picklist.Items
                                If picklist.Score = 100 Then
                                    exists = True
                                    Exit For
                                End If
                            Next
                        End If
                    Case Is = "QASONDEMAND"
                        Dim searchService As com.qas.prowebondemand.QuickAddress = New com.qas.prowebondemand.QuickAddress(sServerURL, sUsername, sPassword)
                        Dim m_Picklist As com.qas.prowebondemand.Picklist
                        searchService.Engine = com.qas.prowebondemand.QuickAddress.EngineTypes.Singleline
                        searchService.Flatten = True
                        m_Picklist = searchService.Search("GBR", postcode, com.qas.proweb.PromptSet.Types.Optimal).Picklist
                        If m_Picklist.Total > 0 Then
                            ' find one that matches 100%
                            For Each picklist As com.qas.prowebondemand.PicklistItem In m_Picklist.Items
                                If picklist.Score = 100 Then
                                    exists = True
                                    Exit For
                                End If
                            Next
                        End If
                End Select

            Catch ex As Exception
                Throw ex
            End Try

            Return exists
        End Function

        ''' <summary>
        ''' Caluculates how many days until all items will be instock
        ''' </summary>
        ''' <param name="profile">the talent basket</param>
        ''' <returns>number of days until all items are in stock</returns>
        ''' <remarks></remarks>
        Public Shared Function GetStockLeadTime(ByVal profile As TalentProfile) As Integer
            Dim stockLeadDays As Integer = 0
            Dim stockLeadDaysCompare As Integer = 0
            Dim strStockLeadDays As String = String.Empty
            Dim reStockCode As String = String.Empty
            Dim tempLeadDays As Integer = 0
            Dim itemCounter As Integer = 1

            For Each tbi As TalentBasketItem In profile.Basket.BasketItems
                If Stock.GetStockBalance(tbi.Product) > 0 Then
                    stockLeadDaysCompare = 0
                Else
                    Stock.GetNoStockDescription(tbi.Product, strStockLeadDays, reStockCode)
                    If Integer.TryParse(strStockLeadDays, tempLeadDays) Then
                        stockLeadDaysCompare = tempLeadDays
                    Else
                        stockLeadDaysCompare = 0
                    End If
                End If
                If itemCounter = 1 Then
                    stockLeadDays = stockLeadDaysCompare
                End If

                ' if basket is home del then get greatest lead time
                ' if trade then get lowest lead time
                If IsPartnerHomeDeliveryType(profile) OrElse IsBasketHomeDelivery(profile) Then
                    If stockLeadDaysCompare > stockLeadDays Then
                        stockLeadDays = stockLeadDaysCompare
                    End If
                Else
                    If stockLeadDaysCompare < stockLeadDays Then
                        stockLeadDays = stockLeadDaysCompare
                    End If
                End If
                itemCounter += 1
            Next
            Return stockLeadDays
        End Function

        ''' <summary>
        ''' Sales channels for the web:- 
        ''' Gets the originating source to decide which products are to be returned from backend.
        ''' This returns KIOSK If Kisokmode else agent name if exists from given session otherwise empty string
        ''' </summary>
        ''' <param name="agentFromSession">The agent name from session.</param>
        ''' <returns>Kisokmode then KIOSK else agent name if exists otherwise empty string</returns>
        Public Shared Function GetOriginatingSource(ByVal agentFromSession As String) As String
            Dim moduleDefaults As New ECommerceModuleDefaults
            Dim moduleDefaultsValue As ECommerceModuleDefaults.DefaultValues
            Dim originatingSource As String = String.Empty
            moduleDefaultsValue = moduleDefaults.GetDefaults()
            If moduleDefaultsValue.TicketingKioskMode Then
                originatingSource = "KIOSK"
            Else
                If Not agentFromSession Is Nothing Then
                    originatingSource = Convert.ToString(agentFromSession)
                Else
                    originatingSource = String.Empty
                End If
            End If
            Return originatingSource
        End Function

        ''' <summary>
        ''' Based on the page restriction, the function returns a string
        ''' </summary>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="partner">The partner.</param>
        ''' <param name="agentMode">if set to <c>true</c> [agent mode].</param>
        ''' <param name="agentType">Type of the agent.</param>
        ''' <param name="agentRestriction">if set to <c>true</c> [agent restriction].</param>
        ''' <param name="currentPageName">Name of the current page.</param>
        ''' <returns></returns>
        Public Shared Function PageRestrictions(ByVal businessUnit As String, ByVal partner As String, _
                                                ByVal agentMode As Boolean, ByVal agentType As String, _
                                                ByVal agentRestriction As Boolean, ByVal currentPageName As String) As String

            Dim restrictionMode As String = String.Empty
            If ((agentType Is Nothing)) Then
                agentType = String.Empty
            End If

            Dim totalPages As Integer = 7 'equal to the highest value in page name
            Dim currentPageNameValue As Integer = 0
            Dim agentRestrictionValue As Integer = 0

            'decide pagename value
            currentPageName = currentPageName.Trim().ToLower()
            Select Case currentPageName
                Case "registration.aspx"
                    currentPageNameValue = 1
                Case "registrationconfirmation.aspx"
                    currentPageNameValue = 2
                Case "updateprofile.aspx"
                    currentPageNameValue = 3
                Case "updateprofileconfirmation.aspx"
                    currentPageNameValue = 4
                Case "friendsandfamily.aspx"
                    currentPageNameValue = 5
                Case "smartcardeventmaintenance.aspx"
                    currentPageNameValue = 6
                Case "transactionenquiry.aspx"
                    currentPageNameValue = 7
            End Select

            'decide agent restriction value
            If agentRestriction Then
                agentRestrictionValue = 1
            Else
                agentRestrictionValue = 0
            End If

            'Declaration for all types of return type based on conditions

            'AMT - Agent Mode True, AMF - Agent Mode False

            'AMF_AgentType1(currentPageNameValue,0)
            Dim AMF_AgentType1(totalPages, 0) As String
            AMF_AgentType1(1, 0) = "AR_P"
            AMF_AgentType1(2, 0) = "ARC_P"
            AMF_AgentType1(3, 0) = String.Empty
            AMF_AgentType1(4, 0) = String.Empty
            AMF_AgentType1(5, 0) = String.Empty
            AMF_AgentType1(6, 0) = "SEM_P"
            AMF_AgentType1(7, 0) = "A2TE_P"

            'AMF_AgentType2(currentPageNameValue,0)
            Dim AMF_AgentType2(totalPages, 0) As String
            AMF_AgentType2(1, 0) = "AR_P"
            AMF_AgentType2(2, 0) = "ARC_P"
            AMF_AgentType2(3, 0) = String.Empty
            AMF_AgentType2(4, 0) = String.Empty
            AMF_AgentType2(5, 0) = String.Empty
            AMF_AgentType2(6, 0) = "SEM_P"
            AMF_AgentType2(7, 0) = String.Empty

            'AMT_AgentType1(currentPageNameValue,agentRestriction)
            Dim AMT_AgentType1(totalPages, 1) As String
            AMT_AgentType1(1, 0) = String.Empty
            AMT_AgentType1(2, 0) = String.Empty
            AMT_AgentType1(3, 0) = String.Empty
            AMT_AgentType1(4, 0) = String.Empty
            AMT_AgentType1(5, 0) = String.Empty
            AMT_AgentType1(6, 0) = String.Empty
            AMT_AgentType1(7, 0) = "ARAS_P"
            AMT_AgentType1(1, 1) = String.Empty
            AMT_AgentType1(2, 1) = String.Empty
            AMT_AgentType1(3, 1) = String.Empty
            AMT_AgentType1(4, 1) = String.Empty
            AMT_AgentType1(5, 1) = String.Empty
            AMT_AgentType1(6, 1) = String.Empty
            AMT_AgentType1(7, 1) = "ARAS_P"

            'AMT_AgentType2(currentPageNameValue,agentRestriction)
            Dim AMT_AgentType2(totalPages, 1) As String
            AMT_AgentType2(1, 0) = String.Empty
            AMT_AgentType2(2, 0) = String.Empty
            AMT_AgentType2(3, 0) = String.Empty
            AMT_AgentType2(4, 0) = String.Empty
            AMT_AgentType2(5, 0) = String.Empty
            AMT_AgentType2(6, 0) = String.Empty
            AMT_AgentType2(7, 0) = String.Empty
            AMT_AgentType2(1, 1) = String.Empty
            AMT_AgentType2(2, 1) = String.Empty
            AMT_AgentType2(3, 1) = "UP_AR"
            AMT_AgentType2(4, 1) = "UPC_AR"
            AMT_AgentType2(5, 1) = "FF_AR"
            AMT_AgentType2(6, 1) = String.Empty
            AMT_AgentType2(7, 1) = String.Empty

            'logic to decide the return type
            If agentMode Then
                If (agentType = "2") Then
                    restrictionMode = AMT_AgentType2(currentPageNameValue, agentRestrictionValue)
                ElseIf (agentType = "1") Then
                    restrictionMode = AMT_AgentType1(currentPageNameValue, agentRestrictionValue)
                Else
                    restrictionMode = String.Empty
                End If
            Else
                If (agentType = "2") Then
                    restrictionMode = AMF_AgentType2(currentPageNameValue, 0)
                ElseIf (agentType = "1") Then
                    restrictionMode = AMF_AgentType1(currentPageNameValue, 0)
                Else
                    restrictionMode = String.Empty
                End If
            End If

            'hardcoded restriction for agenttype 1 may need proper design in future
            If agentMode AndAlso (agentType = "1") Then
                If HttpContext.Current.Request.Url.AbsolutePath.ToUpper.IndexOf("PRODUCTBROWSE") > -1 Then
                    restrictionMode = "PROD_P"
                ElseIf HttpContext.Current.Request.Url.AbsolutePath.ToUpper.IndexOf("USERCONTROLLED") > -1 Then
                    restrictionMode = "USC_P"
                End If
            End If
            'Request.Url.AbsolutePath.ToUpper.IndexOf("PRODUCTBROWSE")

            If restrictionMode Is Nothing Then
                restrictionMode = String.Empty
            End If
            Return restrictionMode
        End Function

        ''' <summary>
        ''' Gets the clubs code details from tbl_partner_user_club based on loginid if no record then from tbl_club_details based on accountNo2
        ''' </summary>
        ''' <returns>Datatable</returns>
        Public Shared Function GetClubs() As DataTable

            GetClubs = New DataTable
            'Dim profileTalent As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
            'Dim accountNo2 As String = profileTalent.User.Details.Account_No_2
            'Dim loginID As String = profileTalent.User.Details.LoginID
            Dim agentProfile As New Agent
            Dim accountNo2 As String = agentProfile.Type
            Dim loginID As String = agentProfile.Name
            Dim tblPartnerUserClubData As DataTable = GetTblPartnerUserClub()
            tblPartnerUserClubData.DefaultView.RowFilter = "LOGINID='" & loginID & "' AND AGENT_TYPE='" & accountNo2 & "'"
            GetClubs = tblPartnerUserClubData.DefaultView.ToTable()
            tblPartnerUserClubData = Nothing
            If Not (GetClubs.Rows.Count > 0) Then
                'From tbl_club_details
                GetClubs.Clear()
                Dim tblClubDetailsData As DataTable = GetTblClubDetails()
                tblClubDetailsData.DefaultView.RowFilter = "AGENT_TYPE='" & accountNo2 & "'"
                GetClubs = tblClubDetailsData.DefaultView.ToTable()
                tblClubDetailsData = Nothing
            End If
            Return GetClubs

        End Function

        ''' <summary>
        ''' Gets the data from tbl_club_details and move to cache
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GetTblClubDetails() As DataTable
            GetTblClubDetails = New DataTable

            Dim cacheKey As String = TalentCache.GetBusinessUnit & "_" & TalentCache.GetPartner(HttpContext.Current.Profile) & "_CachedTblClubDetails"

            If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                GetTblClubDetails = CType(HttpContext.Current.Cache.Item(cacheKey), DataTable)
            Else
                Const selectStr As String = " SELECT * FROM tbl_club_details order by DISPLAY_SEQUENCE, CLUB_DESCRIPTION "
                Dim cmd As New SqlCommand(selectStr, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))

                Try
                    cmd.Connection.Open()
                Catch ex As Exception
                End Try

                If cmd.Connection.State = ConnectionState.Open Then
                    Try
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(GetTblClubDetails)
                    Catch ex As Exception
                    End Try
                End If

                Try
                    If cmd.Connection.State = ConnectionState.Open Then
                        cmd.Connection.Close()
                    End If
                Catch ex As Exception
                End Try

                HttpContext.Current.Cache.Add(cacheKey, GetTblClubDetails, Nothing, Now.AddMinutes(30), TimeSpan.Zero, CacheItemPriority.Normal, Nothing)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
            End If

            Return GetTblClubDetails
        End Function

        ''' <summary>
        ''' Gets the data from tbl_partner_user_club where available is true and having records in tbl_club_details based on club code
        ''' and moving them cache
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GetTblPartnerUserClub() As DataTable
            GetTblPartnerUserClub = New DataTable

            Dim cacheKey As String = TalentCache.GetBusinessUnit & "_" & TalentCache.GetPartner(HttpContext.Current.Profile) & "_CachedGetTblPartnerUserClub"

            If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                GetTblPartnerUserClub = CType(HttpContext.Current.Cache.Item(cacheKey), DataTable)
            Else
                Const selectStr As String = "SELECT " & _
                                "TPUC.PARTNER [PARTNER]" & _
                                ", TPUC.LOGINID LOGINID" & _
                                ", TPUC.CLUB_CODE CLUB_CODE" & _
                                ", TPUC.AVAILABLE AVAILABLE" & _
                                ", TPUC.IS_DEFAULT IS_DEFAULT" & _
                                ", TCD.CLUB_DESCRIPTION CLUB_DESCRIPTION" & _
                                ", TCD.DISPLAY_SEQUENCE DISPLAY_SEQUENCE" & _
                                ", TCD.CUSTOMER_VALIDATION_URL CUSTOMER_VALIDATION_URL" & _
                                ", TCD.VALID_CUSTOMER_FORWARD_URL VALID_CUSTOMER_FORWARD_URL" & _
                                ", TCD.INVALID_CUSTOMER_FORWARD_URL INVALID_CUSTOMER_FORWARD_URL" & _
                                ", TCD.STANDARD_CUSTOMER_FORWARD_URL STANDARD_CUSTOMER_FORWARD_URL" & _
                                ", TCD.NOISE_ENCRYPTION_KEY NOISE_ENCRYPTION_KEY" & _
                                ", TCD.SUPPLYNET_LOGINID SUPPLYNET_LOGINID" & _
                                ", TCD.SUPPLYNET_PASSWORD SUPPLYNET_PASSWORD" & _
                                ", TCD.SUPPLYNET_COMPANY SUPPLYNET_COMPANY" & _
                                ", TCD.AGENT_TYPE AGENT_TYPE" & _
                                " FROM TBL_PARTNER_USER_CLUB TPUC " & _
                                " INNER JOIN TBL_CLUB_DETAILS TCD ON TPUC.CLUB_CODE=TCD.CLUB_CODE " & _
                                " WHERE TPUC.AVAILABLE='TRUE' ORDER BY TCD.DISPLAY_SEQUENCE, TCD.CLUB_DESCRIPTION"

                Dim cmd As New SqlCommand(selectStr, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))

                Try
                    cmd.Connection.Open()
                Catch ex As Exception
                End Try

                If cmd.Connection.State = ConnectionState.Open Then
                    Try
                        Dim da As New SqlDataAdapter(cmd)
                        da.Fill(GetTblPartnerUserClub)
                    Catch ex As Exception
                    End Try
                End If

                Try
                    If cmd.Connection.State = ConnectionState.Open Then
                        cmd.Connection.Close()
                    End If
                Catch ex As Exception
                End Try

                HttpContext.Current.Cache.Add(cacheKey, GetTblPartnerUserClub, Nothing, Now.AddMinutes(30), TimeSpan.Zero, CacheItemPriority.Normal, Nothing)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
            End If

            Return GetTblPartnerUserClub
        End Function

        Public Shared Function TalentLogging() As Talent.Common.TalentLogging
            Dim pageLogging As New Talent.Common.TalentLogging
            pageLogging.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            Return pageLogging
        End Function

        ''' <summary>
        ''' Checks the page is accessible for agent only.
        ''' If the user is not an agent redirecting to NotFound page
        ''' </summary>
        Public Shared Sub CheckPageIsForAgentOnly()
            Dim wfr As New Talent.Common.WebFormResource
            Dim pageCode As String = ProfileHelper.GetPageName
            With wfr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = pageCode
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = pageCode
            End With
            If CType(wfr.Attribute("AgentAccess"), Boolean) = True Then
                If HttpContext.Current.Session("Agent") Is Nothing Then
                    HttpContext.Current.Response.Redirect("~/PagesPublic/Error/notFound.aspx")
                ElseIf HttpContext.Current.Session("Agent").ToString = String.Empty Then
                    HttpContext.Current.Response.Redirect("~/PagesPublic/Error/notFound.aspx")
                End If
            End If
        End Sub

        ''' <summary>
        ''' Gets the SMTP Port number from web.config
        ''' </summary>
        ''' <returns>Integer</returns>
        Public Shared Function GetSMTPPortNumber() As Integer
            Dim _smtpPortNumber As Integer = 25
            Try
                Dim _smtpPortNumberString As String = String.Empty
                _smtpPortNumberString = ConfigurationManager.AppSettings("EmailSMTPPortNumber").ToString.Trim
                If Not String.IsNullOrEmpty(_smtpPortNumberString) Then
                    _smtpPortNumber = CInt(_smtpPortNumberString)
                Else
                    _smtpPortNumber = 25
                End If
            Catch ex As Exception
                _smtpPortNumber = 25
            End Try
            Return _smtpPortNumber
        End Function

        ''' <summary>
        ''' Determines whether [is order already paid].
        ''' </summary>
        ''' <returns>
        ''' <c>true</c> if [is order already paid]; otherwise, <c>false</c>.
        ''' </returns>
        Public Shared Function IsOrderAlreadyPaid() As Boolean
            Dim isOrderPaid As Boolean = False
            Dim userProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
            If userProfile.Basket.BasketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE OrElse userProfile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
                If userProfile.Basket IsNot Nothing AndAlso (Not String.IsNullOrEmpty(userProfile.Basket.TempOrderID)) Then
                    Dim tempOrderID As String = userProfile.Basket.Temp_Order_Id
                    If tempOrderID.Length > 0 Then
                        Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
                        Dim dtOrderHeader As Data.DataTable = orders.Get_Header_By_Temp_Order_Id(tempOrderID)
                        If dtOrderHeader.Rows.Count > 0 _
                            AndAlso dtOrderHeader.Rows(0)("STATUS") >= Talent.Common.Utilities.GetOrderStatus("PAYMENT ACCEPTED") _
                            AndAlso dtOrderHeader.Rows(0)("STATUS") < Talent.Common.Utilities.GetOrderStatus("ORDER PROCESSED") Then
                            isOrderPaid = True
                        Else
                            isOrderPaid = False
                        End If
                        orders = Nothing
                    End If
                End If
            End If
            Return isOrderPaid
        End Function

        ''' <summary>
        ''' Gets the TKT available quantity.
        ''' </summary>
        ''' <param name="productCode">The product code.</param>
        ''' <returns></returns>
        Public Shared Function GetTKTAvailableQuantity(ByVal productCode As String) As Integer
            Dim ecomDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = ecomDefaults.GetDefaults()
            Dim quantityAvailable As Integer = 0
            Dim err As Talent.Common.ErrorObj
            Dim talProduct As New TalentProduct
            Dim productDetails As New DEProductDetails
            With talProduct
                .Settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .Settings.BusinessUnit = TalentCache.GetBusinessUnit()
                .Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
                .Settings.Cacheing = True
                .Settings.OriginatingSourceCode = "W"
                '                .Settings.CacheDependencyPath = def.CacheDependencyPath
            End With

            productDetails.ProductCode = productCode
            talProduct.De = productDetails
            err = talProduct.CourseAvailabilityPreCheck

            If (Not err.HasError) AndAlso (talProduct.ResultDataSet IsNot Nothing) Then
                Dim productRows() As DataRow = talProduct.ResultDataSet.Tables(1).Select("ProductCode ='" & productCode & "'")
                If productRows.Length > 0 Then
                    Dim available As String = productRows(0)("Availability").ToString.Trim
                    If IsNumeric(available) Then
                        quantityAvailable = available
                    End If
                End If
            Else
                talProduct = Nothing
                productDetails = Nothing
            End If
            Return quantityAvailable
        End Function

        Public Shared Function GetTKTAvailableQuantity(ByVal productType As String, ByVal productCode As String, ByVal campaignCode As String, ByVal standCode As String, ByVal areaCode As String) As Integer
            Dim ecomDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = ecomDefaults.GetDefaults()
            Dim quantityAvailable As Integer = 0
            Dim err As Talent.Common.ErrorObj
            Dim talProduct As New TalentProduct
            Dim productDetails As New DEProductDetails

            talProduct.Settings = Utilities.GetSettingsObject()


            talProduct.De.ProductCode = productCode
            talProduct.De.CapacityByStadium = def.CapacityByStadium
            talProduct.De.Src = "W"
            talProduct.De.CampaignCode = campaignCode
            talProduct.De.ProductType = productType
            talProduct.De.ComponentID = CATHelper.GetPackageComponentId(productCode, HttpContext.Current.Request("callid"))
            talProduct.AgentLevelCacheForProductStadiumAvailability = def.AgentLevelCacheForProductStadiumAvailability
            err = talProduct.ProductStadiumAvailability

            If (Not err.HasError) AndAlso (talProduct.ResultDataSet IsNot Nothing) Then
                Dim productRows() As DataRow = talProduct.ResultDataSet.Tables(1).Select("ProductCode ='" & productCode & "' AND StandCode='" & standCode & "' AND AreaCode='" & areaCode & "'")
                If productRows.Length > 0 Then
                    Dim available As String = productRows(0)("Reserved").ToString.Trim
                    If IsNumeric(available) Then
                        quantityAvailable = available
                    End If
                End If
            Else
                talProduct = Nothing
                productDetails = Nothing
            End If
            Return quantityAvailable
        End Function

        ''' <summary>
        ''' Determines whether [is course product] [the specified product stadium].
        ''' </summary>
        ''' <param name="productStadium">The product stadium.</param>
        ''' <returns>
        ''' <c>true</c> if [is course product] [the specified product stadium]; otherwise, <c>false</c>.
        ''' </returns>
        Public Shared Function IsCourseProduct(ByVal productStadium As String) As Boolean
            Dim isCoursePdt As Boolean = False
            If Not String.IsNullOrWhiteSpace(productStadium) Then
                Dim ecomDefaults As New ECommerceModuleDefaults
                Dim def As ECommerceModuleDefaults.DefaultValues = ecomDefaults.GetDefaults()
                'Get the default course stadium
                Dim courseStadium As String = def.CourseStadium
                If Not String.IsNullOrEmpty(courseStadium) Then
                    If productStadium.Trim.ToUpper() = courseStadium.Trim.ToUpper() Then
                        isCoursePdt = True
                    Else
                        isCoursePdt = False
                    End If
                End If
            End If
            Return isCoursePdt
        End Function

        Public Shared Function GetCurrentPageName(ByVal withQueryString As Boolean) As String
            Dim sPath As String = System.Web.HttpContext.Current.Request.Url.AbsolutePath
            Dim oInfo As System.IO.FileInfo = New System.IO.FileInfo(sPath)
            Dim pageName As String = oInfo.Name.ToLower()
            If withQueryString AndAlso HttpContext.Current.Request.QueryString.Count > 0 Then
                pageName = pageName & "?" & HttpContext.Current.Request.QueryString.ToString
            End If
            Return pageName
        End Function

        Public Shared Function GetCurrentPageNameWebMethod() As String
            Return GetCurrentPageName("WEBMETHOD")
        End Function

        ''' <summary>
        ''' Returns only stand information from full 16 char seat details string
        ''' </summary>
        ''' <param name="seatString">seat details string</param>
        ''' <returns>
        ''' stand as a string (3 chars)
        ''' </returns>
        Public Shared Function GetStandFromSeatDetails(ByVal seatString As String) As String
            Dim stand As String = String.Empty
            Try
                stand = seatString.Substring(0, 3).Trim()
            Catch ex As Exception
            End Try
            Return stand
        End Function

        ''' <summary>
        ''' Returns only area information from full 16 char seat details string
        ''' </summary>
        ''' <param name="seatString">seat details string</param>
        ''' <returns>
        ''' area as a string (4 chars)
        ''' </returns>
        Public Shared Function GetAreaFromSeatDetails(ByVal seatString As String) As String
            Dim area As String = String.Empty
            Try
                area = seatString.Substring(3, 4).Trim()
            Catch ex As Exception
            End Try
            Return area
        End Function

        ''' <summary>
        ''' Returns only row information from full 16 char seat details string
        ''' </summary>
        ''' <param name="seatString">seat details string</param>
        ''' <returns>
        ''' row as a string (4 chars)
        ''' </returns>
        Public Shared Function GetRowFromSeatDetails(ByVal seatString As String) As String
            Dim row As String = String.Empty
            Try
                row = seatString.Substring(7, 4).Trim()
            Catch ex As Exception
            End Try
            Return row
        End Function

        ''' <summary>
        ''' Returns seat information including alpha char from full 16 char seat details string
        ''' </summary>
        ''' <param name="seatString">seat details string</param>
        ''' <returns>
        ''' seat as a string (5 chars)
        ''' </returns>
        Public Shared Function GetSeatFromSeatDetails(ByVal seatString As String) As String
            Dim seat As String = String.Empty
            Try
                If seatString.Length > 15 Then
                    seat = seatString.Substring(11, 5).Trim()
                Else
                    seat = seatString.Substring(11, 4).Trim()
                End If
            Catch ex As Exception
            End Try
            Return seat
        End Function

        ''' <summary>
        ''' Call TALENT to retrieve the stand and area descriptions
        ''' </summary>
        ''' <param name="stadiumCode">The stadium code to use</param>
        ''' <param name="standCode">The stand code finding the description for</param>
        ''' <param name="areaCode">The area code finding the description for</param>
        ''' <param name="standDescription">The stand description</param>
        ''' <param name="areaDescription">The area description</param>
        ''' <remarks></remarks>
        Public Shared Sub RetrieveStandAndAreaDescriptions(ByVal stadiumCode As String, ByVal standCode As String, ByVal areaCode As String, ByRef standDescription As String, ByRef areaDescription As String)
            Dim talentProduct As New TalentProduct
            Dim deSettings As New DESettings
            Dim err As New ErrorObj
            Dim standAndAreaDescriptions As New DataTable
            deSettings = GetSettingsObject()
            deSettings.Cacheing = True
            deSettings.CacheTimeMinutes = 30
            talentProduct.Settings() = deSettings
            talentProduct.De.Src = GlobalConstants.SOURCE
            talentProduct.De.StadiumCode = stadiumCode
            err = talentProduct.StandDescriptions()

            If Not err.HasError AndAlso talentProduct.ResultDataSet IsNot Nothing Then
                If talentProduct.ResultDataSet.Tables.Count > 1 AndAlso talentProduct.ResultDataSet.Tables(0).Rows.Count > 0 Then
                    If String.IsNullOrWhiteSpace(talentProduct.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred").ToString) Then
                        If talentProduct.ResultDataSet.Tables(1).Rows.Count > 0 Then
                            standAndAreaDescriptions.Merge(talentProduct.ResultDataSet.Tables(1))
                            If Not String.IsNullOrWhiteSpace(standCode) AndAlso Not String.IsNullOrWhiteSpace(areaCode) Then
                                For Each row As DataRow In standAndAreaDescriptions.Rows
                                    If row("StandCode").ToString.Equals(standCode) AndAlso row("AreaCode").ToString.Equals(areaCode) Then
                                        standDescription = row("StandDescription").ToString().Trim()
                                        areaDescription = row("AreaDescription").ToString().Trim()
                                        Exit For
                                    End If
                                Next
                            End If
                        End If
                    End If
                End If
            End If
        End Sub

        Public Shared Sub DoSapOciCheckout()
            Dim myDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
            If def.ValidatePartnerDirectAccess AndAlso def.SapOciPartner = TalentCache.GetPartner(HttpContext.Current.Profile).ToString() Then
                If HttpContext.Current.Session("SAP_OCI_HOOK_URL") IsNot Nothing Then
                    HttpContext.Current.Response.Redirect("~/Redirect/PunchOut.aspx")
                Else
                    HttpContext.Current.Response.Redirect("~/PagesPublic/Error/SessionError.aspx?errortype=SAPOCIERROR")
                End If
            End If
        End Sub

        Public Shared Sub CheckBasketFreeItemHasOptions()
            Dim validFreeItems As Boolean = True
            Dim tDataObjects As New TalentDataObjects
            tDataObjects.Settings = GetSettingsObject()
            Dim BasketItems As DataTable = tDataObjects.BasketSettings.TblBasketDetail.GetNonTicketingDetailByBasketHeaderID(CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)
            For Each basketItem As DataRow In BasketItems.Rows
                If Utilities.CheckForDBNull_Boolean_DefaultFalse(basketItem("IS_FREE")) Then
                    If Utilities.CheckForDBNull_Boolean_DefaultFalse(basketItem("ALLOW_SELECT_OPTION")) Then
                        If Not (Utilities.CheckForDBNull_Boolean_DefaultFalse(basketItem("OPTION_SELECTED"))) Then
                            validFreeItems = False
                            Exit For
                        End If
                    End If
                End If
            Next
            If Not validFreeItems Then
                HttpContext.Current.Session("TalentErrorCode") = "FREEITEMOPTIONERROR"
                HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
            End If
        End Sub

        Shared Function IsTicketingFee(ByVal p1 As String) As Boolean
            Throw New NotImplementedException
        End Function

        Public Shared Function GetPhotoNameWithExt(ByVal loginID As String) As String
            Dim imgPath As String = GetPhotoPathByType("PERM_PHYSICAL")
            If Not String.IsNullOrWhiteSpace(imgPath) Then
                If File.Exists(imgPath & loginID & ".jpg") Then Return loginID & ".jpg"
                If File.Exists(imgPath & loginID & ".jpeg") Then Return loginID & ".jpeg"
                If File.Exists(imgPath & loginID & ".png") Then Return loginID & ".png"
                If File.Exists(imgPath & loginID & ".gif") Then Return loginID & ".gif"
            End If
            Return String.Empty
        End Function

        Public Shared Function DeleteImageFromPath(ByVal loginID As String, ByVal pathTypeOut As String) As Boolean
            Dim fileDeleted As Boolean = False
            Try
                Dim filePath As String = GetPhotoPathByType(pathTypeOut)
                If Not String.IsNullOrWhiteSpace(filePath) Then
                    filePath = filePath & loginID
                    If File.Exists(filePath & ".jpg") Then
                        File.Delete(filePath & ".jpg")
                        fileDeleted = True
                    End If
                    If Not fileDeleted Then
                        If File.Exists(filePath & ".jpeg") Then
                            File.Delete(filePath & ".jpeg")
                            fileDeleted = True
                        End If
                    End If
                    If Not fileDeleted Then
                        If File.Exists(filePath & ".gif") Then
                            File.Delete(filePath & ".gif")
                            fileDeleted = True
                        End If
                    End If
                    If Not fileDeleted Then
                        If File.Exists(filePath & ".png") Then
                            File.Delete(filePath & ".png")
                            fileDeleted = True
                        End If
                    End If
                    fileDeleted = True
                End If
            Catch ex As Exception
                fileDeleted = False
            End Try

            Return fileDeleted

        End Function

        Public Shared Function IsValidImageFile(ByVal stFileContent As Stream, ByVal fileExtension As String) As Boolean
            Dim isValidimg As Boolean = False
            Dim img As System.Drawing.Image = Nothing
            Try
                img = System.Drawing.Image.FromStream(stFileContent)
                If img IsNot Nothing Then
                    isValidimg = True
                End If
                img = Nothing
            Catch generatedExceptionName As OutOfMemoryException
                isValidimg = False
            Catch otherException As Exception
                isValidimg = False
            Finally
                img = Nothing
            End Try

            'Dim imageHeader As New Generic.Dictionary(Of String, Byte())()
            'Try
            '    imageHeader.Add("JPG", New Byte() {&HFF, &HD8, &HFF, &HE0})
            '    imageHeader.Add("JPEG", New Byte() {&HFF, &HD8, &HFF, &HE0})
            '    imageHeader.Add("PNG", New Byte() {&H89, &H50, &H4E, &H47})
            '    imageHeader.Add("TIF", New Byte() {&H49, &H49, &H2A, &H0})
            '    imageHeader.Add("TIFF", New Byte() {&H49, &H49, &H2A, &H0})
            '    imageHeader.Add("GIF", New Byte() {&H47, &H49, &H46, &H38})
            '    imageHeader.Add("BMP", New Byte() {&H42, &H4D})
            '    imageHeader.Add("ICO", New Byte() {&H0, &H0, &H1, &H0})

            '    Dim header As Byte()
            '    fileExtension = fileExtension.Replace(".", "")
            '    Dim tmp As Byte() = imageHeader(fileExtension.ToUpper())
            '    header = New Byte(tmp.Length - 1) {}
            '    stFileContent.Read(header, 0, header.Length)
            '    If CompareArray(tmp, header) Then
            '        isValidimg = True
            '    Else
            '        isValidimg = False
            '    End If
            'Catch ex As Exception
            '    isValidimg = False
            'Finally
            '    imageHeader = Nothing
            'End Try
            Return isValidimg
        End Function

        Private Shared Function CompareArray(ByVal a1 As Byte(), ByVal a2 As Byte()) As Boolean
            If a1.Length <> a2.Length Then
                Return False
            End If
            For i As Integer = 0 To a1.Length - 1
                If a1(i) <> a2(i) Then
                    Return False
                End If
            Next
            Return True
        End Function

        Public Shared Function GetPhotoPathByType(ByVal pathTypeOut As String) As String
            Dim pathOut As String = String.Empty
            Dim myDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
            Select Case pathTypeOut
                Case Is = "PERM_VIRTUAL"
                    pathOut = def.ProfilePhotoPermanentPath.Trim
                    If pathOut.Length > 0 AndAlso (Not pathOut.EndsWith("/")) Then
                        pathOut = pathOut & "/"
                    End If
                Case Is = "PERM_PHYSICAL"
                    pathOut = def.ProfilePhotoPermanentPathPhysical.Trim
                    If pathOut.Length > 0 AndAlso (Not pathOut.EndsWith("\")) Then
                        pathOut = pathOut & "\"
                    End If
                Case Is = "TEMP_VIRTUAL"
                    pathOut = def.ImageUploadTempPath.Trim
                    If pathOut.Length > 0 AndAlso (Not pathOut.EndsWith("/")) Then
                        pathOut = pathOut & "/"
                    End If
                Case Is = "TEMP_PHYSICAL"
                    pathOut = def.ImageUploadTempPathPhysical.Trim
                    If pathOut.Length > 0 AndAlso (Not pathOut.EndsWith("\")) Then
                        pathOut = pathOut & "\"
                    End If
            End Select
            Return pathOut
        End Function

        Public Shared Function IsSiteInMaintenance() As Boolean
            Dim isMaintenanceTime As Boolean = False
            Dim myDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
            Dim startTime As System.TimeSpan = Nothing
            Dim endTime As System.TimeSpan = Nothing
            Dim currentTime As System.TimeSpan = Now.TimeOfDay
            If CheckForDBNull_String(def.MaintenanceStartTime).Trim.Length > 0 AndAlso CheckForDBNull_String(def.MaintenanceEndTime).Trim.Length > 0 Then
                startTime = CDate(def.MaintenanceStartTime.Trim).TimeOfDay
                endTime = CDate(def.MaintenanceEndTime.Trim).TimeOfDay
                If startTime > endTime Then
                    If (currentTime >= startTime) AndAlso (currentTime >= endTime) Then
                        isMaintenanceTime = True
                    ElseIf (currentTime <= startTime) AndAlso (currentTime <= endTime) Then
                        isMaintenanceTime = True
                    End If
                Else
                    If (currentTime >= startTime) AndAlso (currentTime < endTime) Then
                        isMaintenanceTime = True
                    End If
                End If
            End If
            Return isMaintenanceTime
        End Function

        ''' <summary>
        ''' Gets the external payment gateway image URL for the given gateway name
        ''' </summary>
        ''' <param name="externalGatewayName">Name of the external gateway.</param><returns></returns>
        Public Shared Function GetExternalGatewayImgURL(ByVal externalGatewayName As String) As String
            Dim imgURL As String = String.Empty
            Dim myDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
            Dim wfr As New Talent.Common.WebFormResource
            With wfr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = "GLOBAL"
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "GLOBAL"
            End With
            If externalGatewayName.ToUpper() = "PP" Then
                imgURL = ImagePath.getImagePath("APPTHEME", wfr.Attribute("PayPalCheckOutImageURL"), wfr.BusinessUnit, wfr.PartnerCode)
            End If
            If externalGatewayName.ToUpper() = GlobalConstants.GOOGLECHECKOUTPAYMENTTYPE Then
                imgURL = ImagePath.getImagePath("APPTHEME", wfr.Attribute("GoogleCheckOutImageURL"), wfr.BusinessUnit, wfr.PartnerCode)
            End If
            If imgURL = String.Empty OrElse imgURL = def.MissingImagePath Then
                imgURL = String.Empty
            End If
            Return imgURL
        End Function

        ''' <summary>
        ''' Return an integer value of the number of unread alerts for the current user
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Sub GetNumberOfUnreadAlerts()
            Dim numberOfUnreadAlerts As Integer = 0
            If System.Web.HttpContext.Current.Session("NumberOfUnreadAlerts") = Nothing Then
                Try
                    Dim myDefaults As New ECommerceModuleDefaults
                    Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
                    If def.AlertsEnabled And Not HttpContext.Current.Profile.IsAnonymous Then
                        Dim tDataObjects As New TalentDataObjects
                        Dim dtUserAlerts As New DataTable
                        tDataObjects.Settings = GetSettingsObject()
                        dtUserAlerts = tDataObjects.AlertSettings.GetUnReadUserAlertsByBUPartnerLoginID(TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile), HttpContext.Current.Profile.UserName, False)
                        dtUserAlerts.DefaultView.RowFilter = "[ACTION_TYPE] = 0 OR [ACTION_TYPE] IS NULL"

                        numberOfUnreadAlerts = dtUserAlerts.DefaultView.ToTable().Rows.Count

                    End If
                Catch ex As Exception
                    numberOfUnreadAlerts = 0
                End Try
                System.Web.HttpContext.Current.Session("NumberOfUnreadAlerts") = numberOfUnreadAlerts
            End If
        End Sub

        Public Shared Sub ProcessUserAlertRedirect()
            Dim def As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults()
            If def.AlertsEnabled And Not HttpContext.Current.Profile.IsAnonymous Then
                If System.Web.HttpContext.Current.Session("IsUserAlertRedirectProcessed") <> HttpContext.Current.Profile.UserName Then
                    Dim tDataObjects As New TalentDataObjects
                    Dim dtUserAlerts As DataTable
                    tDataObjects.Settings = GetSettingsObject()
                    dtUserAlerts = tDataObjects.AlertSettings.GetUnReadUserAlertsByBUPartnerLoginID(TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile), HttpContext.Current.Profile.UserName, False)
                    Dim redirectView As New DataView(dtUserAlerts)
                    redirectView.RowFilter = "[ACTION] ='" & GlobalConstants.ALERT_ACTION_REDIRECT & "'"
                    Dim redirectOrderView As New DataView(redirectView.ToTable())
                    redirectOrderView.Sort = "[ACTIVATION_END_DATETIME] ASC"
                    For Each UserAlert As DataRow In redirectOrderView.ToTable().Rows
                        If Not String.IsNullOrEmpty(UserAlert.Item("ACTION_DETAILS")) And Not String.IsNullOrEmpty(UserAlert.Item("ID")) Then
                            tDataObjects.AlertSettings.MarkAlertAsReadReturnAlerts(TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile), HttpContext.Current.Profile.UserName, UserAlert.Item("ID"))
                            System.Web.HttpContext.Current.Session("IsUserAlertRedirectProcessed") = HttpContext.Current.Profile.UserName
                            Dim URL As String = UserAlert.Item("ACTION_DETAILS")
                            HttpContext.Current.Session("RedirectShowShadowBox") = True
                            If URL.Contains("?") Then
                                HttpContext.Current.Response.Redirect(URL & "&reInd=" & UserAlert.Item("ALERT_ID"))
                            Else
                                HttpContext.Current.Response.Redirect(URL & "?reInd=" & UserAlert.Item("ALERT_ID"))
                            End If
                        End If
                    Next
                End If
            End If

        End Sub
        Public Shared Function GetAlertDataViewByAction(ByVal action As String) As DataView
            Dim tDataObjects As New TalentDataObjects
            tDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            Dim dtUserAlerts As DataTable = tDataObjects.AlertSettings.GetUnReadUserAlertsByBUPartnerLoginID(TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile), HttpContext.Current.Profile.UserName, False)
            dtUserAlerts.DefaultView.RowFilter = "[ACTION] = '" & action & "'"
            Return dtUserAlerts.DefaultView
        End Function
        Public Shared Function IsPageRestrictedByAlert(ByVal alertName As String) As Boolean
            Dim isRestricted As Boolean
            If Not String.IsNullOrWhiteSpace(alertName) Then
                Dim tDataObjects As New TalentDataObjects
                tDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
                isRestricted = tDataObjects.AlertSettings.GetRestrictedAlertByAlertName(TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile), alertName, False)
                If Not isRestricted Then
                    tDataObjects.PageSettings.TblPage.UpdateRestrictingAlertName(Talent.eCommerce.Utilities.GetCurrentPageName(), TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile), String.Empty)
                    Dim pageDefaults As New ECommercePageDefaults
                    pageDefaults.RemovePageCache(Talent.eCommerce.Utilities.GetCurrentPageName())
                End If
            End If
            Return isRestricted
        End Function


        ''' <summary>
        ''' Formats the month/year options to 4 digit string to use when calling TALENT.
        ''' </summary>
        ''' <param name="month">1 or 2 digit string</param>
        ''' <param name="year">4 digit string</param>
        ''' <returns>4 digit formatted date</returns>
        ''' <remarks></remarks>
        Public Shared Function FormatCardDate(ByVal month As String, ByVal year As String, Optional ByVal defaultBlankDateToZeros As Boolean = False) As String
            Dim cardDate As String = String.Empty
            Const doubleZero As String = "00"
            Try
                If String.IsNullOrEmpty(month) Or String.IsNullOrEmpty(year) Then
                    cardDate = String.Empty
                Else
                    If Trim(month).Equals(GlobalConstants.CARD_DDL_INITIAL_VALUE.Trim()) Then
                        month = "00"
                    End If
                    If Trim(year).Equals(GlobalConstants.CARD_DDL_INITIAL_VALUE.Trim()) Then
                        year = "00"
                    End If
                    If defaultBlankDateToZeros Then
                        If month.Length = 1 Then
                            month = "0" & month
                        Else
                            If Not IsNumeric(month) Then month = doubleZero
                        End If
                        If Not IsNumeric(year) Then
                            cardDate = month & doubleZero
                        Else
                            cardDate = month & year.Substring(2, 2)
                        End If
                    Else
                        If month.Length = 1 Then month = "0" & month
                        cardDate = month & year.Substring(2, 2)
                    End If
                End If
            Catch ex As Exception
                cardDate = String.Empty
            End Try
            Return cardDate
        End Function

        Public Shared Function TicketTotalsAfterCAT(ByVal ticketPrice As Decimal) As Decimal
            Dim newTicketPrice As Decimal = ticketPrice
            Dim catMode As String = CType(HttpContext.Current.Profile, TalentProfile).Basket.CAT_MODE
            If Not String.IsNullOrWhiteSpace(catMode) Then
                If (catMode = GlobalConstants.CATMODE_CANCEL OrElse catMode = GlobalConstants.CATMODE_CANCELALL) Then
                    newTicketPrice = (ticketPrice * -1)
                ElseIf (catMode = GlobalConstants.CATMODE_AMEND OrElse catMode = GlobalConstants.CATMODE_TRANSFER) Then
                    newTicketPrice -= CType(HttpContext.Current.Profile, TalentProfile).Basket.CAT_PRICE
                End If
            End If
            Return newTicketPrice
        End Function

        Public Shared Function GetFeeValue(ByVal fee As String) As Decimal

            For Each tbi As TalentBasketItem In CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems
                If UCase(tbi.Product) = UCase(fee) Then
                    Return tbi.Gross_Price
                End If
            Next
            Return 0
        End Function

        Public Shared Sub FeesVisibilty(ByVal paymentType As String, ByRef displayCCFee As Boolean, ByRef displayDDFee As Boolean, ByRef displayCFFee As Boolean, ByVal controlName As String)
            Select Case paymentType
                Case GlobalConstants.CCPAYMENTTYPE, GlobalConstants.SAVEDCARDPAYMENTTYPE, GlobalConstants.PAYPALPAYMENTTYPE, GlobalConstants.CHIPANDPINPAYMENTTYPE, GlobalConstants.GOOGLECHECKOUTPAYMENTTYPE
                    displayCCFee = True
                    displayDDFee = False
                    displayCFFee = False
                Case GlobalConstants.DDPAYMENTTYPE
                    displayCCFee = False
                    displayDDFee = True
                    displayCFFee = False
                Case GlobalConstants.CFPAYMENTTYPE
                    displayCCFee = False
                    displayDDFee = False
                    displayCFFee = True
                Case GlobalConstants.CSPAYMENTTYPE, GlobalConstants.EPURSEPAYMENTTYPE, GlobalConstants.OAPAYMENTTYPE
                    displayCCFee = False
                    displayDDFee = False
                    displayCFFee = False
                Case Else
                    displayCCFee = True
                    displayDDFee = False
                    displayCFFee = False
            End Select
        End Sub

        ''' <summary>
        ''' Determines whether [is anonymous talent] agent as well as profile is anonymous
        ''' </summary><returns>
        '''   <c>true</c> if agent as well as profile is anonymous; otherwise, <c>false</c>.
        ''' </returns>
        Public Shared Function IsAnonymousTalent() As Boolean
            Dim isUnAuthenticatedTalent As Boolean = False
            Dim agentProfile As New Agent
            If CType(HttpContext.Current.Profile, TalentProfile).IsAnonymous AndAlso (Not agentProfile.IsAgent) Then
                isUnAuthenticatedTalent = True
            Else
                isUnAuthenticatedTalent = False
            End If
            Return isUnAuthenticatedTalent
        End Function

        ''' <summary>
        ''' Sets the site home/startup page based on the given input string, if this is blank, default to home.aspx
        ''' </summary>
        ''' <returns>The home page string, defaulted to home.aspx</returns>
        ''' <remarks></remarks>
        Public Shared Function GetSiteHomePage() As String
            Dim currentHomePage As String = "~/PagesPublic/Home/Home.aspx"
            Dim moduleDefaultsValue As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults
            If Not String.IsNullOrWhiteSpace(moduleDefaultsValue.ApplicationStartupPage) Then
                currentHomePage = moduleDefaultsValue.ApplicationStartupPage
            End If
            Return currentHomePage
        End Function

        Public Shared Function GetPaymentTypeCodeDescription(ByVal paymentTypeCode As String, ByVal businessUnit As String, ByVal partnerCode As String, ByVal languageCode As String) As String
            If Not String.IsNullOrWhiteSpace(paymentTypeCode) Then
                'there is a increase in minutes in caching, as it is base table
                Dim talDataObjects As New Talent.Common.TalentDataObjects()
                talDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
                Dim dtPaymentType As DataTable = talDataObjects.PaymentSettings.TblPaymentTypeLang.GetByBUPartnerLangCode(businessUnit, partnerCode, languageCode, True, 90)
                For rowIndex As Integer = 0 To dtPaymentType.Rows.Count - 1
                    If paymentTypeCode = Talent.eCommerce.Utilities.CheckForDBNull_String(dtPaymentType.Rows(rowIndex)("PAYMENT_TYPE_CODE")) Then
                        paymentTypeCode = Talent.eCommerce.Utilities.CheckForDBNull_String(dtPaymentType.Rows(rowIndex)("PAYMENT_TYPE_DESCRIPTION"))
                        Exit For
                    End If
                Next
            End If
            Return paymentTypeCode
        End Function

        ''' <summary>
        ''' Method to remove a cookie
        ''' </summary>
        ''' <param name="key">Key</param>
        ''' <remarks></remarks>
        Public Shared Sub RemoveCookie(ByVal key As String)
            With HttpContext.Current
                Dim cookie As New HttpCookie(.Server.UrlEncode(key))
                If Not IsNothing(cookie) Then
                    With cookie
                        .HttpOnly = True
                        .Expires = New DateTime(&H7CF, 10, 12)
                    End With
                    .Response.Cookies.Remove(.Server.UrlEncode(key)) 'Remove from server (has no effect on client)
                    .Response.Cookies.Add(cookie) 'Add expired cookie to client, effectively removing it
                End If
            End With
        End Sub

        ''' <summary>
        ''' Method to remove a cookie
        ''' </summary>
        ''' <param name="cookie">HTTP Cookie object</param>
        ''' <remarks></remarks>
        Public Shared Sub RemoveCookie(ByVal cookie As HttpCookie)
            With HttpContext.Current
                If Not IsNothing(cookie) Then
                    With cookie
                        .HttpOnly = True
                        .Expires = New DateTime(&H7CF, 10, 12)
                    End With
                    .Response.Cookies.Remove(cookie.Name) 'Remove from server (has no effect on client)
                    .Response.Cookies.Add(cookie) 'Add expired cookie to client, effectively removing it
                End If
            End With
        End Sub

        ''' <summary>
        ''' Update the password validate value
        ''' </summary>
        ''' <param name="loginId">The given Login id</param>
        ''' <param name="passwordValidated">The password validated value</param>
        ''' <remarks></remarks>
        Public Shared Sub UpdatePasswordValidated(ByVal loginId As String, ByVal passwordValidated As Boolean, Optional ByVal partner As String = "")
            Dim moduleDefaultsValue As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults
            If String.IsNullOrEmpty(partner) Then partner = TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile))
            If moduleDefaultsValue.RememberMeFunction AndAlso Not IsAgent() Then
                Dim tDataObjects As New Talent.Common.TalentDataObjects()
                tDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
                tDataObjects.ProfileSettings.tblAuthorizedUsers.UpdatePasswordValidated(partner, tDataObjects.Settings.BusinessUnit, loginId, passwordValidated)
            End If
        End Sub

        ''' <summary>
        ''' Gets all BusinessUnit related details URL, Device type, queue url etc entities.
        ''' </summary>
        ''' <returns>Dictionary(Of String, DEBusinessUnitURLDevice) Key is URL_GROUP + DEVICE_TYPE</returns>
        Public Shared Function GetAllBUURLDeviceEntities() As Dictionary(Of String, DEBusinessUnitURLDevice)
            Dim dicAllBUURLDeviceEntities As Dictionary(Of String, DEBusinessUnitURLDevice) = Nothing
            Dim talDataObjects As New Talent.Common.TalentDataObjects
            Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settings.DestinationDatabase = "SQL2005"
            settings.ConnectionStringList = Talent.eCommerce.Utilities.GetConnectionStringList
            talDataObjects.Settings = settings
            dicAllBUURLDeviceEntities = talDataObjects.AppVariableSettings.TblUrlBu.GetAllBusinessUnitURLDeviceList()
            Return (dicAllBUURLDeviceEntities)
        End Function

        ''' <summary>
        ''' Check the basket has anything in and if it does, does it contain the seat that is being worked with.
        ''' </summary>
        ''' <param name="productCode">The given product code to search for</param>
        ''' <param name="standCode">The specific stand code of the seat string</param>
        ''' <param name="areaCode">The specific area code of the seat string</param>
        ''' <param name="rowNumber">The specific row text of the seat string</param>
        ''' <param name="seatNumber">The specific seat text of the seat string</param>
        ''' <returns>True if the seat has been found</returns>
        ''' <remarks></remarks>
        Public Shared Function IsSeatInBasket(ByRef productCode As String, ByRef standCode As String, ByRef areaCode As String, ByRef rowNumber As String, ByRef seatNumber As String) As Boolean
            Dim seatFoundInBasket As Boolean = False
            Dim talProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
            If talProfile.Basket.BasketItems.Count > 0 Then
                For Each item As TalentBasketItem In talProfile.Basket.BasketItems
                    If item.MODULE_ = GlobalConstants.BASKETMODULETICKETING AndAlso item.Product = productCode AndAlso Not String.IsNullOrEmpty(item.SEAT) Then
                        Dim itemStandCode As String = Utilities.GetStandFromSeatDetails(item.SEAT)
                        Dim itemAreaCode As String = Utilities.GetAreaFromSeatDetails(item.SEAT)
                        Dim itemRowNumber As String = Utilities.GetRowFromSeatDetails(item.SEAT)
                        Dim itemSeatNumber As String = Utilities.GetSeatFromSeatDetails(item.SEAT)
                        If itemStandCode = standCode AndAlso itemAreaCode = areaCode AndAlso itemRowNumber = rowNumber AndAlso itemSeatNumber = seatNumber Then
                            seatFoundInBasket = True
                            Exit For
                        End If
                    End If
                Next
            End If
            Return seatFoundInBasket
        End Function

        Public Shared Function GetAgentEntity(ByVal cacheDependencyPath As String) As DEAgent
            Dim agentEntity As New DEAgent
            If HttpContext.Current.Session IsNot Nothing AndAlso _
                HttpContext.Current.Session("Agent") IsNot Nothing AndAlso _
                HttpContext.Current.Session("Agent").ToString.Trim.Length > 0 Then
                Dim talDataObject As New Talent.Common.TalentDataObjects()
                Dim settings As New Talent.Common.DESettings()
                settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                settings.CacheDependencyPath = cacheDependencyPath
                settings.DestinationDatabase = "SQL2005"
                talDataObject.Settings = settings
                Dim dtTblAgent As DataTable = talDataObject.AgentSettings.TblAgent.GetByAgentName(HttpContext.Current.Session.SessionID, HttpContext.Current.Session("Agent").ToString)
                If dtTblAgent IsNot Nothing AndAlso dtTblAgent.Rows.Count > 0 Then
                    agentEntity.AgentUsername = CheckForDBNull_String(dtTblAgent.Rows(0)("AGENT_NAME"))
                    agentEntity.AgentType = HttpContext.Current.Session("AgentType").ToString
                    agentEntity.Department = CheckForDBNull_String(dtTblAgent.Rows(0)("DEPARTMENT")).Trim
                    agentEntity.PrinterNameDefault = CheckForDBNull_String(dtTblAgent.Rows(0)("PRINTER_NAME_DEFAULT"))
                    agentEntity.BulkSalesMode = CheckForDBNull_Boolean_DefaultFalse(dtTblAgent.Rows(0)("BULK_SALES_MODE"))
                Else
                    If Not HttpContext.Current.Request.Url.PathAndQuery.ToLower.Contains("agentlogin.aspx?logout=expired") Then
                        HttpContext.Current.Session.Remove("Agent")
                        HttpContext.Current.Session("IsAgentProfileEmpty") = True
                        HttpContext.Current.Response.Redirect("~/PagesPublic/Agent/AgentLogin.aspx?logout=expired")
                    End If
                End If
            End If
            Return agentEntity
        End Function

        Public Shared Function GetBasketPaymentFeesEntityList(ByVal basketHeaderID As String) As List(Of DEFeesPayment)
            Dim basketPaymentFeesEntityList As New List(Of DEFeesPayment)
            Dim tDataObjects As New Talent.Common.TalentDataObjects()
            tDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            Dim dtBasketDetail As DataTable = tDataObjects.BasketSettings.TblBasketDetail.GetDetailByBasketHeaderID(basketHeaderID)
            If dtBasketDetail IsNot Nothing AndAlso dtBasketDetail.Rows.Count > 0 Then
                For rowIndex As Integer = 0 To dtBasketDetail.Rows.Count - 1
                    'is it a fee
                    If Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("FEE_CATEGORY")).Trim.Length > 0 Then
                        Dim feesPaymentEntity As New DEFeesPayment
                        feesPaymentEntity.FeeCategory = Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("FEE_CATEGORY")).Trim
                        feesPaymentEntity.FeeCode = Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("PRODUCT")).Trim
                        feesPaymentEntity.FeeDepartment = ""
                        feesPaymentEntity.FeeDescription = Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("PRODUCT_DESCRIPTION1")).Trim
                        feesPaymentEntity.FeeType = ""
                        feesPaymentEntity.FeeValue = Format(Utilities.CheckForDBNull_Decimal(dtBasketDetail.Rows(rowIndex)("GROSS_PRICE")), "0.00")
                        feesPaymentEntity.IsNegativeFee = (feesPaymentEntity.FeeValue < 0)
                        feesPaymentEntity.IsSystemFee = Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(dtBasketDetail.Rows(rowIndex)("IS_SYSTEM_FEE"))
                        feesPaymentEntity.IsTransactional = Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(dtBasketDetail.Rows(rowIndex)("IS_TRANSACTIONAL"))
                        feesPaymentEntity.IsChargeable = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(dtBasketDetail.Rows(rowIndex)("IS_INCLUDED"))
                        feesPaymentEntity.IsExternal = Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(dtBasketDetail.Rows(rowIndex)("IS_EXTERNAL"))
                        basketPaymentFeesEntityList.Add(feesPaymentEntity)
                    End If
                Next
            End If
            Return basketPaymentFeesEntityList
        End Function

        ''' <summary>
        ''' Check the basket request to see if it is valid and if there are any linked products
        ''' </summary>
        ''' <param name="page">The page request</param>
        ''' <param name="func">The page function request</param>
        ''' <param name="productHasRelatedProducts">If the product has any related products, set this</param>
        ''' <param name="productHasMandatoryRelatedProducts">If the product has any related mandatory products, set this</param>
        ''' <returns>Valid basket request</returns>
        ''' <remarks></remarks>
        Public Shared Function IsValidAddToBasketRequest(ByVal page As String, ByVal func As String, ByRef productHasRelatedProducts As Boolean, ByRef productHasMandatoryRelatedProducts As Boolean) As Boolean
            Dim isValid As Boolean = False
            isValid = CATHelper.IsNotCATRequestOrBasketNotHasCAT(page, func, -1)
            If isValid Then
                If HttpContext.Current.Request.QueryString("product") IsNot Nothing AndAlso Not String.IsNullOrEmpty(HttpContext.Current.Request.QueryString("product").Trim()) Then
                    If LCase(func) = "addtobasket" OrElse LCase(page) = "registrationparticipants.aspx" Then
                        Dim priceCode As String = String.Empty
                        Dim campaign As String = String.Empty
                        Dim productSubType As String = String.Empty
                        Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
                        If HttpContext.Current.Request.QueryString("pricecode") IsNot Nothing Then priceCode = HttpContext.Current.Request.QueryString("pricecode")
                        If HttpContext.Current.Request.QueryString("campaign") IsNot Nothing Then campaign = HttpContext.Current.Request.QueryString("campaign")
                        If HttpContext.Current.Request.QueryString("productsubtype") IsNot Nothing Then productSubType = HttpContext.Current.Request.QueryString("productsubtype")
                        Dim tDataObjects As New Talent.Common.TalentDataObjects()
                        tDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
                        Dim userProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
                        Dim linkedMasterProduct As String = tDataObjects.BasketSettings.TblBasketHeader.GetLinkedMasterProduct(userProfile.Basket.Basket_Header_ID)
                        If Not String.IsNullOrEmpty(linkedMasterProduct) AndAlso Not userProfile.Basket.DoesBasketContainProductCode(linkedMasterProduct) Then
                            tDataObjects.BasketSettings.TblBasketHeader.UpdateLinkedProductMaster(userProfile.Basket.Basket_Header_ID, String.Empty)
                        End If
                        If String.IsNullOrEmpty(linkedMasterProduct) OrElse linkedMasterProduct = HttpContext.Current.Request.QueryString("product").Trim() Then
                            ticketingGatewayFunctions.CheckForLinkedProducts(HttpContext.Current.Request.QueryString("product").Trim(), priceCode, campaign, productSubType, productHasRelatedProducts, productHasMandatoryRelatedProducts)
                        End If
                    End If
                End If
            Else
                'not valid to process linked products but valid to process the request
                productHasRelatedProducts = False
                productHasMandatoryRelatedProducts = False
                isValid = True
            End If
            Return isValid
        End Function

        ''' <summary>
        ''' Check the basket request to see if it is valid and if there are any linked products
        ''' </summary>
        ''' <param name="page">The page request</param>
        ''' <param name="func">The page function request</param>
        ''' <param name="product">The product code in the request</param>
        ''' <param name="priceCode">The price code in the request</param>
        ''' <param name="campaign">The campaign code in the request</param>
        ''' <param name="productHasRelatedProducts">If the product has any related products, set this</param>
        ''' <param name="productHasMandatoryRelatedProducts">If the product has any related mandatory products, set this</param>
        ''' <param name="seatCount">The number of seats in the request</param>
        ''' <returns>Valid basket request</returns>
        ''' <remarks></remarks>
        Public Shared Function IsValidAddToBasketRequest(ByVal page As String, ByVal func As String, ByVal product As String, ByVal priceCode As String, ByVal campaign As String, ByVal productSubType As String, ByRef productHasRelatedProducts As Boolean, ByRef productHasMandatoryRelatedProducts As Boolean, ByVal seatCount As Integer, ByVal pickingNewComponentSeat As Boolean) As Boolean
            Dim isValid As Boolean = False
            ' Changing seat is allowed in CAT for MDH. We don't need the CAT check call
            If Not pickingNewComponentSeat Then
                isValid = CATHelper.IsNotCATRequestOrBasketNotHasCAT(page, func, seatCount)
            End If
            Dim tDataObjects As New Talent.Common.TalentDataObjects()
            tDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            Dim userProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
            Dim linkedMasterProduct As String = tDataObjects.BasketSettings.TblBasketHeader.GetLinkedMasterProduct(userProfile.Basket.Basket_Header_ID)
            If isValid AndAlso product.Length > 0 Then
                Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
                If Not String.IsNullOrEmpty(linkedMasterProduct) AndAlso Not userProfile.Basket.DoesBasketContainProductCode(linkedMasterProduct) Then
                    tDataObjects.BasketSettings.TblBasketHeader.UpdateLinkedProductMaster(userProfile.Basket.Basket_Header_ID, String.Empty)
                End If
                If String.IsNullOrEmpty(linkedMasterProduct) OrElse linkedMasterProduct = HttpContext.Current.Request.QueryString("product").Trim() Then
                    ticketingGatewayFunctions.CheckForLinkedProducts(product, priceCode, campaign, productSubType, productHasRelatedProducts, productHasMandatoryRelatedProducts)
                End If
            Else
                'not valid to process linked products but valid to process the request
                productHasRelatedProducts = False
                productHasMandatoryRelatedProducts = False
                isValid = True
            End If
            Return isValid
        End Function

        ''' <summary>
        ''' Derterming what the PDF url will be if one is available for the given product code, or product type, or product sub type
        ''' </summary>
        ''' <param name="productCode">The product code</param>
        ''' <param name="productSubType">The product sub type</param>
        ''' <param name="productType">The product type</param>
        ''' <param name="businessUnit">The business unit</param>
        ''' <param name="partnerCode">The partner code</param>
        ''' <returns>The pdf url string</returns>
        ''' <remarks></remarks>
        Public Shared Function PDFLinkAvailable(ByVal productCode As String, ByVal productType As String, ByVal productSubType As String, ByVal businessUnit As String, ByVal partnerCode As String) As String
            Dim pdfUrl As String = String.Empty
            Dim htmlRootPathAbsolute As String = GetHtmlIncludePathAbsolute()
            pdfUrl = htmlRootPathAbsolute & "\" & businessUnit & "\" & partnerCode & "\Product\Other\" & productCode & ".pdf"
            Dim htmlRootPathRelative As String = GetHtmlIncludePathRelative()
            If System.IO.File.Exists(pdfUrl) Then
                pdfUrl = "~/Assets/" & htmlRootPathRelative & "/" & businessUnit & "/" & partnerCode & "/Product/Other/" & productCode & ".pdf"
            Else
                pdfUrl = htmlRootPathAbsolute & "\" & businessUnit & "\" & partnerCode & "\Product\Other\" & productSubType & ".pdf"
                If System.IO.File.Exists(pdfUrl) Then
                    pdfUrl = "~/Assets/" & htmlRootPathRelative & "/" & businessUnit & "/" & partnerCode & "/Product/Other/" & productSubType & ".pdf"
                Else
                    pdfUrl = htmlRootPathAbsolute & "\" & businessUnit & "\" & partnerCode & "\Product\Other\" & productType & ".pdf"
                    If System.IO.File.Exists(pdfUrl) Then
                        pdfUrl = "~/Assets/" & htmlRootPathRelative & "/" & businessUnit & "/" & partnerCode & "/Product/Other/" & productType & ".pdf"
                    Else
                        pdfUrl = String.Empty
                    End If
                End If
            End If
            Return pdfUrl
        End Function

        Public Shared Function UserUnderAge(ByVal Profile As TalentProfile) As Boolean
            'Dim defs As New Talent.eCommerce.ECommerceModuleDefaults
            'Dim values As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
            'Dim UnderAge As Boolean = False
            'values = defs.GetDefaults

            'If values.Use_Age_Check Then
            '    Dim products As New TalentProductInformationTableAdapters.tbl_productTableAdapter
            '    Dim orderLines As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
            '    Dim lines As New Data.DataTable

            '    Try
            '        'lines = orderLines.Get_BasketDetailControl_Lines(Profile.Basket.Basket_Header_ID, TalentCache.GetPartner(Profile), TalentCache.GetBusinessUnit, values.PriceList)
            '        lines = orderLines.GetBasketItems_ByHeaderID_NonTicketing(Profile.Basket.Basket_Header_ID)
            '        'If lines.Rows.Count < 1 Then
            '        '    lines = orderLines.Get_BasketDetailControl_Lines(Profile.Basket.Basket_Header_ID, Talent.Common.Utilities.GetAllString, TalentCache.GetBusinessUnit, values.PriceList)
            '        'End If
            '    Catch ex As Exception
            '        Logging.WriteLog(Profile.UserName, "UCCAUA-010", ex.Message, "Error getting basket details for age check in UserUnderAge()", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
            '    End Try

            '    If lines.Rows.Count > 0 Then
            '        Dim prods As Data.DataTable
            '        Dim age As Integer = ProfileHelper.GetAge(Profile.User.Details.DOB)
            '        For Each row As Data.DataRow In lines.Rows
            '            prods = products.GetDataByProduct_Code(CheckForDBNull_String(row("PRODUCT")))
            '            If CheckForDBNull_Int(prods.Rows(0)("PRODUCT_MINIMUM_AGE")) > age Then
            '                UnderAge = True
            '                Exit For
            '            End If
            '        Next
            '    Else
            '        'No items in basket so kick back to Basket page anyway
            '        UnderAge = True
            '    End If
            'End If

            'Return UnderAge
            Return False
        End Function
        Public Shared Function AllInStock_BackEndCheck(ByVal Profile As TalentProfile) As Boolean

            Dim myDefaults As New ECommerceModuleDefaults
            Dim defs As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()

            Dim AllInStock As Boolean = True
            If defs.Perform_Back_End_Stock_Check Then
                Dim dep As New Talent.Common.DePNA
                Dim des As New Talent.Common.DESettings
                Dim tls As New Talent.Common.TalentStock
                Dim err As New Talent.Common.ErrorObj
                Dim dt As Data.DataTable = Nothing
                Dim dRow As Data.DataRow = Nothing

                Dim strResults As New StringBuilder

                Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
                Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
                Dim stockLocation As String = def.StockLocation

                Dim productcodes As String = String.Empty
                Dim alternateSKUs As String = String.Empty

                Dim locations As String = String.Empty
                For Each bi As TalentBasketItem In Profile.Basket.BasketItems
                    'Only perform the check if products have been selected, this could be a tickets only basket
                    '   If Not productcodes.Trim.Equals("") Then
                    If bi.PRODUCT_TYPE = "" OrElse bi.PRODUCT_TYPE = "M" Then
                        If Not bi.MODULE_.ToUpper.Equals("TICKETING") Then
                            If Not String.IsNullOrEmpty(productcodes) Then productcodes += ","
                            productcodes += bi.Product
                            If Not String.IsNullOrEmpty(locations) Then locations += ","
                            locations += stockLocation
                            If Not String.IsNullOrEmpty(alternateSKUs) Then alternateSKUs += ","
                            alternateSKUs += bi.ALTERNATE_SKU
                        End If
                    End If
                Next

                If productcodes.EndsWith(",") Then productcodes = productcodes.TrimEnd(",")
                If locations.EndsWith(",") Then locations = locations.TrimEnd(",")
                If alternateSKUs.EndsWith(",") Then alternateSKUs = alternateSKUs.TrimEnd(",")

                'Only perform the check if products have been selected, this could be a tickets only basket
                If Not productcodes.Trim.Equals("") Then

                    dep.SKU = productcodes
                    dep.Warehouse = locations
                    dep.AlternateSKU = alternateSKUs

                    With des
                        .BusinessUnit = TalentCache.GetBusinessUnitGroup
                        .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                        .Cacheing = False
                        If Profile.PartnerInfo.Details.Account_No_3 Is Nothing _
                            OrElse Profile.PartnerInfo.Details.Account_No_3.Trim = String.Empty Then
                            .AccountNo3 = def.DEFAULT_COMPANY_CODE
                        Else
                            .AccountNo3 = Profile.PartnerInfo.Details.Account_No_3
                        End If
                        .AccountNo4 = Profile.PartnerInfo.Details.Account_No_4
                        .DestinationDatabase = "SYSTEM21"
                        .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
                        .RetryFailures = def.StockCheckRetry
                        .RetryAttempts = def.StockCheckRetryAttempts
                        .RetryWaitTime = def.StockCheckRetryWait
                        .RetryErrorNumbers = def.StockCheckRetryErrors
                        .IgnoreErrors = def.StockCheckIgnoreErrors
                        .LoginId = Profile.User.Details.LoginID
                        .AccountNo1 = Profile.User.Details.Account_No_1
                    End With
                    Try
                        With tls
                            .Settings = des
                            .Dep = dep
                            err = .GetMutlipleStock
                            If Not err.HasError Then dt = .ResultDataSet.Tables(0)
                        End With
                    Catch ex As Exception
                        Logging.WriteLog(Profile.UserName, "UCCASC-010", ex.Message, "Error contacting SYSTEM 21 for stock check", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
                    End Try


                    If Not err.HasError Then
                        '
                        ' We could have forced no error by ignoring the error code, therefore the datatable will be empty 
                        ' - just continue.
                        If Not dt Is Nothing Then
                            If dt.Rows.Count > 0 Then
                                Dim productcode As String = String.Empty, quantity As String = String.Empty
                                'Dim ItemErrorLabel As New Label
                                For Each row As Data.DataRow In dt.Rows
                                    For Each bi As TalentBasketItem In Profile.Basket.BasketItems
                                        productcode = bi.Product
                                        quantity = bi.Quantity
                                        If Utilities.CheckForDBNull_String(row("ProductNumber")).Trim = productcode Then
                                            If (defs.AllowCheckoutWhenBackEndUnavailable) And ((Utilities.CheckForDBNull_String(row("ErrorCode")).Trim = "UNAVAILABLE")) Then
                                                bi.STOCK_ERROR = False
                                                Exit For
                                            Else
                                                If Utilities.CheckForDBNull_Decimal(row("Quantity")) < CDec(quantity) Then
                                                    AllInStock = False
                                                    bi.STOCK_ERROR = True
                                                    bi.QUANTITY_AVAILABLE = Utilities.CheckForDBNull_Decimal(row("Quantity"))
                                                    '-------------------
                                                    ' Discontinued check
                                                    '-------------------
                                                    If defs.PerformDiscontinuedProductCheck Then
                                                        Dim dtProdInfo As New Data.DataTable
                                                        dtProdInfo = Utilities.GetProductInfo(productcode)
                                                        If Not dtProdInfo Is Nothing AndAlso dtProdInfo.Rows.Count > 0 Then
                                                            If Utilities.CheckForDBNull_Boolean_DefaultFalse(dtProdInfo.Rows(0)("DISCONTINUED")) Then
                                                                bi.STOCK_ERROR_CODE = "DISC"
                                                            End If
                                                        End If
                                                    End If

                                                    Exit For
                                                Else
                                                    bi.STOCK_ERROR = False
                                                    Exit For
                                                End If
                                            End If
                                        End If
                                    Next
                                Next
                            Else
                            End If
                        End If
                    Else
                        Logging.WriteLog(Profile.UserName, err, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
                        AllInStock = False
                    End If
                End If

                Profile.Basket.STOCK_ERROR = Not AllInStock

                'Save the basket regardless of stock status
                '------------------------------------------------
                Profile.Basket.IsDirty = True
                Profile.Save()
                '------------------------------------------------
            Else
                Profile.Basket.STOCK_ERROR = False

                'Save the basket regardless of stock status
                '------------------------------------------------
                Profile.Basket.IsDirty = True
                Profile.Save()
                '------------------------------------------------
            End If
            Return AllInStock
        End Function
        Public Shared Function RetrieveAlternativeProducts(ByVal Profile As TalentProfile) As Data.DataSet

            'If defs.Perform_Back_End_Stock_Check Then
            'Dim dep As New Talent.Common.DePNA
            Dim productCollection As New Collection
            Dim des As New Talent.Common.DESettings
            Dim talentProd As New Talent.Common.TalentProduct
            Dim err As New Talent.Common.ErrorObj
            Dim dt As Data.DataTable = Nothing
            Dim dRow As Data.DataRow = Nothing
            Dim results As New Data.DataSet

            Dim strResults As New StringBuilder

            Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults

            Dim basketDetail As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
            Dim dtBasket As Data.DataTable = basketDetail.GetBasketItems_ByHeaderID_ALL(CType(Profile.Basket.Basket_Header_ID, Long))

            '-----------------------------------------------------------
            ' Loop through basket and check if any items have alt items. 
            ' If so put out message. (basket in profile may not be
            ' loaded yet so need to go to DB)
            '-----------------------------------------------------------
            For Each row As Data.DataRow In dtBasket.Rows
                productCollection.Add(row("PRODUCT"))
            Next

            talentProd.ProductCollection = productCollection

            With des
                .BusinessUnit = TalentCache.GetBusinessUnitGroup
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                .BackOfficeConnectionString = ConfigurationManager.ConnectionStrings("SYSTEM21").ToString
                .Cacheing = False
                If Profile.PartnerInfo.Details.Account_No_3 Is Nothing _
                    OrElse Profile.PartnerInfo.Details.Account_No_3.Trim = String.Empty Then
                    .AccountNo3 = def.DEFAULT_COMPANY_CODE
                Else
                    .AccountNo3 = Profile.PartnerInfo.Details.Account_No_3
                End If
                .AccountNo4 = Profile.PartnerInfo.Details.Account_No_4
                .DestinationDatabase = "SYSTEM21"
                .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()

            End With
            Try
                With talentProd
                    .Settings = des
                    err = .RetrieveAlternativeProducts()
                    If Not err.HasError Then dt = .ResultDataSet.Tables(0)
                End With
            Catch ex As Exception
                Logging.WriteLog(Profile.UserName, "UCCASC-011", _
                                                    ex.Message, _
                                                    "Error contacting " & _
                                                         talentProd.Settings.DestinationDatabase & _
                                                          " for alt products check", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
            End Try

            If Not err.HasError Then
                results = talentProd.ResultDataSet
            Else
                Logging.WriteLog(Profile.UserName, err, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
            End If

            Return results
        End Function
        Public Shared Function SaveAddressToDB(ByVal StorePostcode As Boolean, ByVal Profile As TalentProfile) As Boolean
            'Try
            '    Dim ta As New TalentProfileAddress
            '    With ta
            '        .LoginID = Profile.User.Details.LoginID
            '        If StorePostcode Then
            '            If String.IsNullOrEmpty(building.Text.Trim) Then
            '                .Reference = Address2.Text & " " & Address3.Text & " (" & UCase(postcode.Text) & ")"
            '            Else
            '                .Reference = building.Text & " " & Address2.Text & " (" & UCase(postcode.Text) & ")"
            '            End If
            '        Else
            '            If String.IsNullOrEmpty(building.Text.Trim) Then
            '                .Reference = Address2.Text & " " & Address3.Text
            '            Else
            '                .Reference = building.Text & " " & Address2.Text
            '            End If
            '        End If
            '        .Type = ""
            '        .Default_Address = True
            '        .Address_Line_1 = building.Text
            '        .Address_Line_2 = Address2.Text
            '        .Address_Line_3 = Address3.Text
            '        .Address_Line_4 = Address4.Text
            '        .Address_Line_5 = Address5.Text
            '        .Post_Code = UCase(postcode.Text)
            '        .Country = CountryDDL.SelectedValue
            '        .Sequence = GetNextSequenceNo()
            '    End With

            '    'finally, check to see if the address reference is alrady taken
            '    'Dim testAddress As TalentProfileAddress = Profile.User.Addresses.ContainsKey(ta.Reference)
            '    ' If testAddress Is Nothing Then
            '    If Not Profile.User.Addresses.ContainsKey(ta.Reference) Then
            '        Profile.Provider.AddAddressToUserProfile(ta)
            '    Else
            '        'address already exists
            '        'The address already exists on this profile and so cannot be added
            '        ErrorLabel.Text = ucr.Content("AddressExistsErrorText", _languageCode, True)
            '        Return False
            '    End If
            '    Try
            '        Profile.Provider.UpdateDefaultAddress(ta.LoginID, Profile.PartnerInfo.Details.Partner, ta.Address_ID)
            '    Catch ex As Exception
            '        Logging.WriteLog(Profile.UserName, "UCCASA-020", ex.Message, "Error updating the user's default address", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
            '    End Try
            'Catch ex As Exception
            '    Logging.WriteLog(Profile.UserName, "UCCASA-010", ex.Message, "Error adding a new address for the user", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
            'End Try

            Return True
        End Function
        Public Shared Function SetDDLValues(ByVal Profile As TalentProfile) As DropDownList

            Dim PaymentTypeDDL As New DropDownList

            Dim myDefaults As New ECommerceModuleDefaults
            Dim ModuleDefaults As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()

            Dim TDataObjects = New Talent.Common.TalentDataObjects()
            TDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()

            PaymentTypeDDL.Items.Clear()

            'if a restricted payment method for this user is in the DDL then remove it
            If ModuleDefaults.RestrictUserPaymentType Then
                Dim mixedBasket As Boolean = False
                Dim retailBasket As Boolean = True
                If Profile.Basket.BasketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
                    retailBasket = True
                ElseIf Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
                    mixedBasket = True
                End If
                Dim dtPaymentTypes As Data.DataTable = TDataObjects.PaymentSettings.TblPaymentTypeBu.GetByBasketTypeAndBU(TalentCache.GetBusinessUnit(), False, retailBasket, mixedBasket, Utilities.IsAgent(), Not String.IsNullOrWhiteSpace(Profile.Basket.CAT_MODE), Profile.IsAnonymous)

                Dim lic As New ListItemCollection
                Dim li As ListItem
                Dim exists As Boolean = False
                For Each row As Data.DataRow In dtPaymentTypes.Rows
                    For Each item As ListItem In lic
                        If item.Value = row("PAYMENT_TYPE_CODE") Then
                            exists = True
                            Exit For
                        End If
                    Next
                    If Not exists Then
                        li = New ListItem(row("PAYMENT_TYPE_DESCRIPTION"), row("PAYMENT_TYPE_CODE"))
                        lic.Add(li)
                    Else
                        exists = False
                    End If
                Next
                If lic.Count > 0 Then
                    lic.Insert(0, New ListItem(" -- ", " -- "))
                End If

                Dim i = Profile.User.Details.RESTRICTED_PAYMENT_METHOD
                Dim a As String()
                Dim j As Integer
                If i = Nothing Or i = "" Then
                    i = "remove,nothing"
                End If
                Try
                    a = i.Split(",")
                Catch ex As Exception
                    ReDim a(0)
                    a(0) = i
                End Try
                'RESTRICTED_PAYMENT_METHOD
                Dim count1 = 0
                For Each li1 As ListItem In lic
                    If count1 > 0 Then
                        PaymentTypeDDL.Items.Add(li1)
                        Try
                            If a.Length > 0 Then
                                For j = 0 To a.GetUpperBound(0)
                                    '  Dim str1 As String = li1.Text.ToString.Trim
                                    Dim str1 As String = li1.Value.ToString.Trim
                                    Dim str2 As String = a(j).ToString.Trim
                                    If str1 = str2 Then
                                        PaymentTypeDDL.Items.Remove(li1)
                                    End If
                                Next
                            Else
                                'no items to remove
                            End If
                        Catch ex As Exception
                        End Try
                    End If
                    count1 += 1
                Next
                If PaymentTypeDDL.Items.Count = 1 Then
                    'force use of the one payment type
                    PaymentTypeDDL.SelectedIndex = 0
                    PaymentTypeDDL.Enabled = False

                ElseIf PaymentTypeDDL.Items.Count = 0 Then
                    'Get Default Item
                    'clear cache key
                    Try
                        HttpContext.Current.Cache.Remove(TalentCache.GetBusinessUnit & "*ALL" & "*ALL" & "PAYMENT_TYPE")
                    Catch ex As Exception
                    End Try
                    Try
                        HttpContext.Current.Cache.Remove("*ALL" & "*ALL" & "*ALL" & "PAYMENT_TYPE")
                    Catch ex As Exception
                    End Try
                    Try
                        HttpContext.Current.Cache.Remove(TalentCache.GetBusinessUnit & Profile.PartnerInfo.Details.Partner & "*ALL" & "PAYMENT_TYPE")
                    Catch ex As Exception
                    End Try

                    Dim licDef As ListItemCollection = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, "DEFAULT_PAYMENT_TYPE", "PAYMENT_TYPE")
                    If licDef.Count > 0 Then
                        Dim defaultPM As String = licDef.Item(1).Text
                        PaymentTypeDDL.Items.Add(defaultPM)
                    Else
                        PaymentTypeDDL.Items.Add("NoDefaultPaymentMethod")
                    End If
                End If
            Else
                'If we are not using restricted payment types
                Dim basketItemsCollection As Generic.List(Of Talent.Common.DEBasketItem) = Profile.Basket.BasketItems
                Dim isRetailBasket As Boolean = False
                Dim isTicketingBasket As Boolean = False
                Dim isMixedBasket As Boolean = False

                'Determine a mixed, ticketing or retail basket
                For Each basketItem As TalentBasketItem In basketItemsCollection
                    If String.IsNullOrEmpty(basketItem.MODULE_) Then
                        isRetailBasket = True
                    ElseIf basketItem.MODULE_ = "Ticketing" Then
                        isTicketingBasket = True
                    End If
                    If isRetailBasket And isTicketingBasket Then
                        isMixedBasket = True
                        isRetailBasket = False
                        isTicketingBasket = False
                        Exit For
                    End If
                Next

                'Get the payment options depending on the basket type
                Dim tDataObjects2 As New Talent.Common.TalentDataObjects()
                Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
                Dim paymentTypeDataTable As New Data.DataTable
                settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                settings.DestinationDatabase = "SQL2005"
                TDataObjects.Settings = settings
                paymentTypeDataTable = tDataObjects2.PaymentSettings.TblPaymentTypeBu.GetByBasketTypeAndBU(TalentCache.GetBusinessUnit, isTicketingBasket, isRetailBasket, isMixedBasket, Utilities.IsAgent(), True, Profile.IsAnonymous)

                'Build drop down list
                If paymentTypeDataTable.Rows.Count > 0 Then
                    For Each row As Data.DataRow In paymentTypeDataTable.Rows
                        Try
                            Dim listItem As New ListItem
                            listItem.Value = Utilities.CheckForDBNull_String(row("PAYMENT_TYPE_CODE"))
                            listItem.Text = Utilities.CheckForDBNull_String(row("PAYMENT_TYPE_DESCRIPTION"))
                            listItem.Selected = Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(row("DEFAULT_PAYMENT_TYPE"))
                            PaymentTypeDDL.Items.Add(listItem)
                        Catch ex As Exception
                        End Try
                    Next
                End If

                If PaymentTypeDDL.Items.Count = 1 Then
                    PaymentTypeDDL.SelectedIndex = 0
                    PaymentTypeDDL.Enabled = False
                ElseIf PaymentTypeDDL.Items.Count = 0 Then
                    PaymentTypeDDL.Items.Add("NoDefaultPaymentMethod")
                End If
            End If

            Return PaymentTypeDDL

        End Function
        Public Shared Function CreditCheck(ByVal Profile As TalentProfile, ByVal PaymentTypeDDL As DropDownList) As String

            Dim ret As String = String.Empty

            Dim myDefaults As New ECommerceModuleDefaults
            Dim ModuleDefaults As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()

            If ModuleDefaults.PerformCreditCheck Then
                '--------------------------------------------------------------
                ' Check if credit check is relevant for particular payment type
                '--------------------------------------------------------------
                Dim payTypesTA As New TalentApplicationVariablesTableAdapters.PaymentTypesTA
                Dim payTypes As TalentApplicationVariables.PaymentTypesDataTable = _
                                      payTypesTA.GetDataBy_PaymentTypeCode(PaymentTypeDDL.SelectedValue.ToString.Trim)

                If payTypes.Rows.Count > 0 _
                    AndAlso Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(payTypes.Rows(0)("CALL_CREDIT_LIMIT_CHECK")) Then

                    Dim deCred As New Talent.Common.DECreditCheck(Profile.User.Details.Account_No_1)
                    Dim dbCred As New Talent.Common.DBCreditCheck
                    Dim credCheck As New Talent.Common.TalentCreditCheck
                    Dim settings As New Talent.Common.DESettings

                    settings.AccountNo1 = Profile.User.Details.Account_No_1
                    settings.AccountNo2 = Profile.User.Details.Account_No_2

                    deCred.TotalOrderValue = Profile.GetMinimumPurchaseAmount

                    settings.BusinessUnit = TalentCache.GetBusinessUnit
                    settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                    settings.StoredProcedureGroup = Utilities.GetStoredProcedureGroup()

                    credCheck.Settings = settings
                    credCheck.CreditCheck = deCred

                    Dim err As Talent.Common.ErrorObj = credCheck.PerformCreditLimitCheck()

                    If Not err.HasError Then
                        Try
                            Dim dt As Data.DataTable = credCheck.ResultDataSet.Tables("CreditCheckHeader")
                            If dt.Rows.Count > 0 Then
                                Dim credStatus As String = dt.Rows(0)("CreditStatus")
                                Select Case credStatus
                                    Case Is = "1"
                                        'Credit will not be exceeded by new order
                                    Case Is = "2"
                                        'Order will cause credit limit to be exceeded
                                        If ModuleDefaults.DisableCheckoutIfOverCreditLimit Then
                                            ret = "CreditWillBeExceededError"
                                        End If
                                    Case Is = "3"
                                        'Credit limit has already been exceeded
                                        If ModuleDefaults.DisableCheckoutIfOverCreditLimit Then
                                            ret = "CreditExceededError"
                                        End If
                                End Select
                            End If
                        Catch ex As Exception

                        End Try
                    End If
                End If
            End If

        End Function
        Public Shared Sub UpdateValues_Standard(ByVal Profile As TalentProfile, Optional ByVal promoCode As String = "")
            Dim orderHeaderTA As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
            Dim orderDetailTA As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter

            'We are not using the Tax Web Service, so check the values in the standard manner
            'Dim promoCode As String = ""
            'Try
            '    Dim promoBox As Control = Utilities.FindWebControl("PromotionsBox", Me.Page.Controls)
            '    promoCode = CallByName(promoBox, "PromotionCode", CallType.Get)
            'Catch ex As Exception
            'End Try


            Dim basketAdapter As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
            Dim liveBasketItems As Data.DataTable = basketAdapter.GetBasketItems_ByHeaderID_NonTicketing( _
                                                       CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)

            Dim defaults As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults
            Dim totals As Talent.Common.TalentWebPricing

            'Select Case defaults.PricingType
            '    Case 2
            '        totals = Utilities.GetPrices_Type2

            '    Case Else
            '        Dim qtyAndCodes As New Generic.Dictionary(Of String, Talent.Common.WebPriceProduct)
            '        For Each bItem As Data.DataRow In liveBasketItems.Rows
            '            If UCase(Utilities.CheckForDBNull_String(bItem("MODULE"))) <> "TICKETING" Then
            '                If Not Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(bItem("IS_FREE")) Then
            '                    If Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(bItem("MASTER_PRODUCT"))) Then
            '                        'Check to see if the multibuys are configured for this master product
            '                        Dim myPrice As Talent.Common.DEWebPrice = Talent.eCommerce.Utilities.GetWebPrices(bItem("MASTER_PRODUCT"), 0, bItem("MASTER_PRODUCT"))
            '                        If myPrice.SALE_PRICE_BREAK_QUANTITY_1 > 0 OrElse myPrice.PRICE_BREAK_QUANTITY_1 > 0 Then
            '                            'multibuys are configured
            '                            If qtyAndCodes.ContainsKey(bItem("MASTER_PRODUCT")) Then
            '                                qtyAndCodes(bItem("MASTER_PRODUCT")).Quantity += Utilities.CheckForDBNull_Decimal(bItem("QUANTITY"))
            '                            Else
            '                                ' Pass in product otherwise Promotions don't work properly
            '                                ' qtyAndCodes.Add(bItem("MASTER_PRODUCT"), New Talent.Common.WebPriceProduct(bItem("MASTER_PRODUCT"), Utilities.CheckForDBNull_Decimal(bItem("QUANTITY")), bItem("MASTER_PRODUCT")))
            '                                qtyAndCodes.Add(bItem("MASTER_PRODUCT"), New Talent.Common.WebPriceProduct(bItem("PRODUCT"), Utilities.CheckForDBNull_Decimal(bItem("QUANTITY")), bItem("MASTER_PRODUCT")))
            '                            End If
            '                        Else
            '                            If Not qtyAndCodes.ContainsKey(Utilities.CheckForDBNull_String(bItem("PRODUCT"))) Then
            '                                qtyAndCodes.Add(Utilities.CheckForDBNull_String(bItem("PRODUCT")), New Talent.Common.WebPriceProduct(Utilities.CheckForDBNull_String(bItem("PRODUCT")), Utilities.CheckForDBNull_Decimal(bItem("QUANTITY")), bItem("MASTER_PRODUCT")))
            '                            End If
            '                        End If
            '                    Else
            '                        If Not qtyAndCodes.ContainsKey(Utilities.CheckForDBNull_String(bItem("PRODUCT"))) Then
            '                            qtyAndCodes.Add(Utilities.CheckForDBNull_String(bItem("PRODUCT")), New Talent.Common.WebPriceProduct(Utilities.CheckForDBNull_String(bItem("PRODUCT")), Utilities.CheckForDBNull_Decimal(bItem("QUANTITY")), Utilities.CheckForDBNull_String(bItem("MASTER_PRODUCT"))))
            '                        End If
            '                    End If
            '                End If
            '            End If
            '        Next

            '        totals = Utilities.GetWebPrices_WithTotals(qtyAndCodes, _
            '                                                    Profile.Basket.TempOrderID, _
            '                                                    defaults.PromotionPriority, _
            '                                                    promoCode)
            'End Select

            totals = Profile.Basket.WebPrices

            If Not totals Is Nothing Then

                Dim delG As Decimal = 0, _
                    delN As Decimal = 0, _
                    delT As Decimal = 0, _
                    delType As String = "", _
                    delDesc As String = ""

                If defaults.DeliveryCalculationInUse Then
                    Select Case UCase(defaults.DeliveryPriceCalculationType)
                        Case "UNIT", "WEIGHT"
                            Dim dc As New Talent.Common.DEDeliveryCharges.DEDeliveryCharge
                            If defaults.ShowPricesExVAT Then
                                dc = Talent.eCommerce.Utilities.GetSelectedDeliveryCharge(totals.Total_Items_Value_Net, totals.Qualifies_For_Free_Delivery, Profile)

                            Else
                                dc = Talent.eCommerce.Utilities.GetSelectedDeliveryCharge(totals.Total_Items_Value_Gross, totals.Qualifies_For_Free_Delivery, Profile)
                            End If
                            delG = dc.GROSS_VALUE
                            delN = dc.NET_VALUE
                            delT = dc.TAX_VALUE
                            delType = dc.DELIVERY_TYPE
                            delDesc = dc.DESCRIPTION1
                        Case Else
                            '---------------------------------------------
                            ' If free delivery is from promotion then keep
                            ' delivery values. This is for balancing on
                            ' the backend.
                            '---------------------------------------------
                            If Not totals.Qualifies_For_Free_Delivery OrElse totals.FreeDeliveryPromotion Then
                                delG = totals.Max_Delivery_Gross
                                delN = totals.Max_Delivery_Net
                                delT = totals.Max_Delivery_Tax
                            End If
                    End Select
                End If




                orderHeaderTA.UpdatePriceAndTaxValues(False, False, _
                                                       False, False, _
                                                       False, False, _
                                                       False, False, _
                                                       False, False, _
                                                       "", 0, _
                                                       "", 0, _
                                                       "", 0, _
                                                       "", 0, _
                                                       "", 0, _
                                                       Talent.eCommerce.Utilities.RoundToValue(totals.Total_Items_Value_Gross, 0.01, False), _
                                                       Talent.eCommerce.Utilities.RoundToValue(totals.Total_Items_Value_Net, 0.01, False), _
                                                       Talent.eCommerce.Utilities.RoundToValue(totals.Total_Items_Value_Tax, 0.01, False), _
                                                       Talent.eCommerce.Utilities.RoundToValue(delG, 0.01, False), _
                                                       Talent.eCommerce.Utilities.RoundToValue(delN, 0.01, False), _
                                                       Talent.eCommerce.Utilities.RoundToValue(delT, 0.01, False), _
                                                       Talent.eCommerce.Utilities.RoundToValue(totals.Total_Items_Value_Gross + delG, 0.01, False), _
                                                       delType, _
                                                       delDesc, _
                                                       Profile.Basket.TempOrderID, _
                                                       TalentCache.GetBusinessUnit, _
                                                       Profile.UserName)



                Select Case defaults.PricingType
                    Case 2 'Only re-add the order lines if not using pricing type 2
                    Case Else
                        Dim orderLines As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter
                        orderLines.Empty_Order(Profile.Basket.TempOrderID)

                        'create an order obj and call the Add_OrderLines_To_Order function
                        '------------------------------------------------------------
                        Dim myOrder As New Talent.eCommerce.Order
                        Dim tDataObjects As New TalentDataObjects
                        tDataObjects.Settings = GetSettingsObject()
                        Dim currencyCode As String = tDataObjects.PaymentSettings.GetCurrencyCode(TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                        myOrder.Add_OrderLines_To_Order(totals, Profile.Basket.TempOrderID, currencyCode)

                        Try
                            Dim promoVal As Decimal = 0
                            Dim promoPercentage As Decimal = 0
                            If Not totals Is Nothing Then
                                promoVal = totals.Total_Promotions_Value
                                If Not totals.PromotionsResultsTable Is Nothing _
                                    AndAlso totals.PromotionsResultsTable.Rows.Count > 0 Then
                                    For Each promo As Data.DataRow In totals.PromotionsResultsTable.Rows
                                        If CDec(promo("PromotionPercentageValue")) > 0 Then
                                            promoPercentage = CDec(promo("PromotionPercentageValue"))
                                            Exit For
                                        End If
                                    Next
                                End If
                            End If
                            orderHeaderTA.Set_Promotion_Value(Talent.eCommerce.Utilities.RoundToValue(promoVal, 0.01, False), promoPercentage, Profile.Basket.TempOrderID)
                            orderLines.Update_Promotion_Line_Values(0, promoPercentage, Profile.Basket.TempOrderID)
                        Catch ex As Exception
                        End Try
                End Select




            End If

        End Sub
        Public Shared Sub UpdateValues_TaxWebService(ByVal Profile As TalentProfile, Optional ByVal promoCode As String = "")
            Dim orderHeaderTA As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
            Dim orderDetailTA As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter

            Try 'to set the order tax values
                Dim taxCalc As New TaxWebService
                'Dim promoCode As String = ""
                'Try
                '    Dim promoBox As Control = Utilities.FindWebControl("PromotionsBox", Me.Page.Controls)
                '    promoCode = CallByName(promoBox, "PromotionCode", CallType.Get)
                'Catch ex As Exception
                'End Try

                Dim taxValues As Data.DataSet = taxCalc.CallTaxWebService("ORDER", _
                                                                            Profile.Basket.TempOrderID, _
                                                                            promoCode)

                If Not HttpContext.Current.Session.Item("DunhillWSError") Is Nothing Then
                    If CBool(HttpContext.Current.Session.Item("DunhillWSError")) Then
                        Logging.WriteLog(Profile.UserName, "ADE-TEST-FAILEDTAXCALC2", "TAX CALCULATOR ERROR", "", TalentCache.GetBusinessUnit, TalentCache.GetDefaultPartner)
                        HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                    End If
                End If

                If taxValues.Tables.Count > 0 Then
                    Dim header As Data.DataTable = taxValues.Tables(0)
                    Dim detail As Data.DataTable = taxValues.Tables(1)

                    If header.Rows.Count > 0 AndAlso detail.Rows.Count > 0 Then
                        Dim rw As Data.DataRow = header.Rows(0)

                        If CBool(rw("Success")) Then
                            Dim s1 As String = Utilities.CheckForDBNull_String(rw("TaxTypeCode1"))
                            Dim s5 As String = Utilities.CheckForDBNull_String(rw("TaxTypeCode5"))
                            orderHeaderTA.UpdatePriceAndTaxValues(Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(rw("DisplayInclusive1")), _
                                                                    Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(rw("DisplayTax1")), _
                                                                    Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(rw("DisplayInclusive2")), _
                                                                    Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(rw("DisplayTax2")), _
                                                                    Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(rw("DisplayInclusive3")), _
                                                                    Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(rw("DisplayTax3")), _
                                                                    Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(rw("DisplayInclusive4")), _
                                                                    Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(rw("DisplayTax4")), _
                                                                    Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(rw("DisplayInclusive5")), _
                                                                    Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(rw("DisplayTax5")), _
                                                                    Utilities.CheckForDBNull_String(rw("TaxTypeCode1")), _
                                                                    Utilities.CheckForDBNull_String(rw("TotalTax1")), _
                                                                    Utilities.CheckForDBNull_String(rw("TaxTypeCode2")), _
                                                                    Utilities.CheckForDBNull_String(rw("TotalTax2")), _
                                                                    Utilities.CheckForDBNull_String(rw("TaxTypeCode3")), _
                                                                    Utilities.CheckForDBNull_String(rw("TotalTax3")), _
                                                                    Utilities.CheckForDBNull_String(rw("TaxTypeCode4")), _
                                                                    Utilities.CheckForDBNull_String(rw("TotalTax4")), _
                                                                    Utilities.CheckForDBNull_String(rw("TaxTypeCode5")), _
                                                                    Utilities.CheckForDBNull_String(rw("TotalTax5")), _
                                                                    Talent.eCommerce.Utilities.RoundToValue(Utilities.CheckForDBNull_Decimal(rw("TotalGross")) - Utilities.CheckForDBNull_Decimal(rw("DeliveryGROSS")), 0.01, False), _
                                                                    Talent.eCommerce.Utilities.RoundToValue(Utilities.CheckForDBNull_Decimal(rw("GoodsTotalNet")), 0.01, False), _
                                                                    Talent.eCommerce.Utilities.RoundToValue(Utilities.CheckForDBNull_Decimal(rw("TotalTax1")) _
                                                                        + Utilities.CheckForDBNull_Decimal(rw("TotalTax2")) _
                                                                            + Utilities.CheckForDBNull_Decimal(rw("TotalTax3")) _
                                                                                + Utilities.CheckForDBNull_Decimal(rw("TotalTax4")) _
                                                                                    + Utilities.CheckForDBNull_Decimal(rw("TotalTax5")), 0.01, False), _
                                                                    Talent.eCommerce.Utilities.RoundToValue(Utilities.CheckForDBNull_Decimal(rw("DeliveryGROSS")), 0.01, False), _
                                                                    Talent.eCommerce.Utilities.RoundToValue(Utilities.CheckForDBNull_Decimal(rw("DeliveryNET")), 0.01, False), _
                                                                    Talent.eCommerce.Utilities.RoundToValue(Utilities.CheckForDBNull_Decimal(rw("DeliveryTAX")), 0.01, False), _
                                                                    Talent.eCommerce.Utilities.RoundToValue(Utilities.CheckForDBNull_Decimal(rw("TotalGross")), 0.01, False), _
                                                                    "", _
                                                                    "", _
                                                                    Profile.Basket.TempOrderID, _
                                                                    TalentCache.GetBusinessUnit, _
                                                                    Profile.UserName)

                            For Each dr As Data.DataRow In detail.Rows
                                Dim purchaseGross As Decimal = 0, _
                                    purchaseNet As Decimal = 0, _
                                    purchaseTax As Decimal = 0, _
                                    deliveryGross As Decimal = 0, _
                                    deliveryNet As Decimal = 0, _
                                    deliveryTax As Decimal = 0, _
                                    tariffCode As String = "", _
                                    lineGross As Decimal = 0, _
                                    lineNet As Decimal = 0, _
                                    lineTax As Decimal = 0, _
                                    taxAmount1 As Decimal = 0, _
                                    taxAmount2 As Decimal = 0, _
                                    taxAmount3 As Decimal = 0, _
                                    taxAmount4 As Decimal = 0, _
                                    taxAmount5 As Decimal = 0

                                Try
                                    tariffCode = dr("TariffCode")
                                Catch ex As Exception
                                End Try
                                Try
                                    taxAmount1 = CDec(dr("taxAmount1"))
                                Catch ex As Exception
                                End Try
                                Try
                                    taxAmount2 = CDec(dr("taxAmount2"))
                                Catch ex As Exception
                                End Try
                                Try
                                    taxAmount3 = CDec(dr("taxAmount3"))
                                Catch ex As Exception
                                End Try
                                Try
                                    taxAmount4 = CDec(dr("taxAmount4"))
                                Catch ex As Exception
                                End Try
                                Try
                                    taxAmount5 = CDec(dr("taxAmount5"))
                                Catch ex As Exception
                                End Try
                                Try
                                    purchaseGross = CDec(dr("GrossAmount")) / CDec(dr("Quantity"))
                                Catch ex As Exception
                                End Try
                                Try
                                    purchaseNet = CDec(dr("NetAmount")) / CDec(dr("Quantity"))
                                Catch ex As Exception
                                End Try
                                Try
                                    purchaseTax = (taxAmount1 + taxAmount2 + taxAmount3 + taxAmount4 + taxAmount5) / CDec(dr("Quantity"))
                                Catch ex As Exception
                                End Try
                                Try
                                    lineGross = purchaseGross * CDec(dr("Quantity"))
                                Catch ex As Exception
                                End Try
                                Try
                                    lineNet = purchaseNet * CDec(dr("Quantity"))
                                Catch ex As Exception
                                End Try
                                Try
                                    lineTax = purchaseTax * CDec(dr("Quantity"))
                                Catch ex As Exception
                                End Try
                                Try
                                    deliveryGross = CDec(dr("deliveryGross"))
                                Catch ex As Exception
                                End Try
                                Try
                                    deliveryNet = CDec(dr("deliveryNet"))
                                Catch ex As Exception
                                End Try
                                Try
                                    deliveryTax = CDec(dr("deliveryTax"))
                                Catch ex As Exception
                                End Try

                                orderDetailTA.Update_Detail_Price_And_Tax_Values(Talent.eCommerce.Utilities.RoundToValue(purchaseGross, 0.01, False), _
                                                                                Talent.eCommerce.Utilities.RoundToValue(purchaseNet, 0.01, False), _
                                                                                Talent.eCommerce.Utilities.RoundToValue(purchaseTax, 0.01, False), _
                                                                                Talent.eCommerce.Utilities.RoundToValue(deliveryGross, 0.01, False), _
                                                                                Talent.eCommerce.Utilities.RoundToValue(deliveryNet, 0.01, False), _
                                                                                Talent.eCommerce.Utilities.RoundToValue(deliveryTax, 0.01, False), _
                                                                                "", _
                                                                                tariffCode, _
                                                                                Talent.eCommerce.Utilities.RoundToValue(lineGross, 0.01, False), _
                                                                                Talent.eCommerce.Utilities.RoundToValue(lineNet, 0.01, False), _
                                                                                Talent.eCommerce.Utilities.RoundToValue(lineTax, 0.01, False), _
                                                                                Talent.eCommerce.Utilities.RoundToValue(taxAmount1, 0.01, False), _
                                                                                Talent.eCommerce.Utilities.RoundToValue(taxAmount2, 0.01, False), _
                                                                                Talent.eCommerce.Utilities.RoundToValue(taxAmount3, 0.01, False), _
                                                                                Talent.eCommerce.Utilities.RoundToValue(taxAmount4, 0.01, False), _
                                                                                Talent.eCommerce.Utilities.RoundToValue(taxAmount5, 0.01, False), _
                                                                                Profile.Basket.TempOrderID, _
                                                                                dr("SKUCode"))

                            Next
                        Else
                            Logging.WriteLog(Profile.UserName, "ADE-TEST-FAILEDTAXCALC3", "TAX CALCULATOR ERROR", "", TalentCache.GetBusinessUnit, TalentCache.GetDefaultPartner)
                            HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                        End If
                    Else
                        Logging.WriteLog(Profile.UserName, "ADE-TEST-FAILEDTAXCALC4", "TAX CALCULATOR ERROR", "", TalentCache.GetBusinessUnit, TalentCache.GetDefaultPartner)
                        HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                    End If
                Else
                    Logging.WriteLog(Profile.UserName, "ADE-TEST-FAILEDTAXCALC5", "TAX CALCULATOR ERROR", "", TalentCache.GetBusinessUnit, TalentCache.GetDefaultPartner)
                    HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                End If

            Catch ex As Exception
                Logging.WriteLog(Profile.UserName, "ADE-TEST-FAILEDTAXCALC1", "TAX CALCULATOR ERROR", "", TalentCache.GetBusinessUnit, TalentCache.GetDefaultPartner)
                HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
            End Try
        End Sub
        Public Shared Function GetSelectedDeliveryCharge(ByVal totalOrder As Decimal, ByVal freeDel As Boolean, ByVal Profile As TalentProfile) As Talent.Common.DEDeliveryCharges.DEDeliveryCharge

            Dim myDefaults As New ECommerceModuleDefaults
            Dim ModuleDefaults As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()

            Dim dcs As Talent.Common.DEDeliveryCharges = Nothing
            Select Case ModuleDefaults.DeliveryPriceCalculationType
                Case "UNIT"
                    dcs = Utilities.GetDeliveryOptions(totalOrder, freeDel)
                Case "WEIGHT"
                    Dim CacheTimeMinutes As Integer = 0
                    Dim countryCode As String = String.Empty
                    Dim countryName As String = String.Empty
                    Utilities.GetDeliveryCountry(Profile.Basket.Temp_Order_Id, countryCode, countryName)
                    dcs = Utilities.GetDeliveryOptions(Utilities.CheckForDBNull_Int(CacheTimeMinutes), totalOrder, freeDel, Utilities.GetBasketItemsTotalWeight(Profile.Basket.BasketItems), countryCode, countryName)
            End Select
            Return dcs.GetDeliveryCharge(DeliverySelection1SelectedDeliveryOption(Profile))
        End Function
        Public Shared Function DeliverySelection1SelectedDeliveryOption(ByVal Profile As TalentProfile) As String

            Dim ret As String = ""

            Dim myDefaults As New ECommerceModuleDefaults
            Dim defs As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()

            Dim dcs As New Talent.Common.DEDeliveryCharges
            Dim CacheTimeMinutes As Integer = 0

            If defs.DeliveryCalculationInUse Then
                Select Case Profile.Basket.BasketContentType
                    Case "M", "C"
                        Dim totals As Talent.Common.TalentWebPricing = Profile.Basket.WebPrices
                        If UCase(defs.DeliveryPriceCalculationType) = "UNIT" Then
                            If defs.ShowPricesExVAT Then
                                dcs = Utilities.GetDeliveryOptions(totals.Total_Items_Value_Net, totals.Qualifies_For_Free_Delivery)
                            Else
                                dcs = Utilities.GetDeliveryOptions(totals.Total_Items_Value_Gross, totals.Qualifies_For_Free_Delivery)
                            End If
                        ElseIf UCase(defs.DeliveryPriceCalculationType) = "WEIGHT" Then
                            Dim countryCode As String = String.Empty
                            Dim countryName As String = String.Empty
                            Utilities.GetDeliveryCountry(Profile.Basket.Temp_Order_Id, countryCode, countryName)
                            If defs.ShowPricesExVAT Then
                                dcs = Utilities.GetDeliveryOptions(Utilities.CheckForDBNull_Int(CacheTimeMinutes), totals.Total_Items_Value_Net, totals.Qualifies_For_Free_Delivery, Utilities.GetBasketItemsTotalWeight(Profile.Basket.BasketItems), countryCode, countryName)
                            Else
                                dcs = Utilities.GetDeliveryOptions(Utilities.CheckForDBNull_Int(CacheTimeMinutes), totals.Total_Items_Value_Gross, totals.Qualifies_For_Free_Delivery, Utilities.GetBasketItemsTotalWeight(Profile.Basket.BasketItems), countryCode, countryName)
                            End If
                        End If
                    Case Else
                        ' nothing
                End Select
            End If

            Dim DeliveryDDL1 As New DropDownList
            Dim DeliveryDDL2 As New DropDownList

            If dcs.DeliveryCharges.Count > 0 Then
                For Each dc As Talent.Common.DEDeliveryCharges.DEDeliveryCharge In dcs.DeliveryCharges
                    If dc.AVAILABLE Then
                        Dim li As ListItem
                        If Not String.IsNullOrEmpty(dc.LANG_DESCRIPTION1) Then
                            li = New ListItem(dc.LANG_DESCRIPTION1, dc.DELIVERY_TYPE)
                        Else
                            li = New ListItem(dc.DESCRIPTION1, dc.DELIVERY_TYPE)
                        End If
                        If Not dc.HasChildNodes Then
                            Dim tDataObjects As New TalentDataObjects
                            tDataObjects.Settings = GetSettingsObject()
                            Dim value As String = String.Empty
                            If defs.ShowPricesExVAT Then
                                value = tDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(dc.NET_VALUE, 0.01, False), TalentCache.GetBusinessUnit(), TalentCache.GetPartner(Profile))
                            Else
                                value = tDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(dc.GROSS_VALUE, 0.01, False), TalentCache.GetBusinessUnit(), TalentCache.GetPartner(Profile))
                            End If
                            li.Text += " - " & System.Web.HttpUtility.HtmlDecode(value)
                        End If
                        DeliveryDDL1.Items.Add(li)
                    End If
                Next


                Dim found As Boolean = False

                Dim ddl1count As Integer = 0
                Dim ddl2count As Integer = 0

                For Each dc As Talent.Common.DEDeliveryCharges.DEDeliveryCharge In dcs.DeliveryCharges
                    If dc.IS_DEFAULT Then
                        found = True
                    Else
                        For Each childDc As Talent.Common.DEDeliveryCharges.DEDeliveryCharge In dc.ChildNodes
                            If childDc.IS_DEFAULT Then
                                found = True
                            End If
                            If found Then
                                Exit For
                            Else
                                ddl2count += 1
                            End If
                        Next
                    End If
                    If found Then
                        Exit For
                    Else
                        ddl1count += 1
                    End If
                Next

                If found Then
                    DeliveryDDL1.SelectedIndex = ddl1count
                    '                    DeliveryDDL1_SelectedIndexChanged(DeliveryDDL1, New EventArgs)

                    If DeliveryDDL2.Visible Then DeliveryDDL2.SelectedIndex = ddl2count
                    '                    DeliveryDDL2_SelectedIndexChanged(DeliveryDDL1, New EventArgs)

                Else
                    '                    DeliveryDDL1_SelectedIndexChanged(DeliveryDDL1, New EventArgs)

                End If

                For Each dc As Talent.Common.DEDeliveryCharges.DEDeliveryCharge In dcs.DeliveryCharges
                    If dc.DELIVERY_TYPE = DeliveryDDL1.SelectedValue Then
                        If dc.HasChildNodes Then
                            ret = DeliveryDDL2.SelectedValue
                            Exit For
                        Else
                            ret = DeliveryDDL1.SelectedValue
                            Exit For
                        End If
                    End If
                Next

            End If


            Return ret
        End Function
        ''' <summary>
        ''' Handle when requesting an SSL page on port 443 (i.e. when SSL is passed straight through LB for SSL off-loading)
        ''' Return HttpContext.Current.Request.Url.AbsoluteUri by default
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function FormatSSLOffloadedURL() As String
            Dim retURI As String = String.Empty
            Dim SSLOffloaded As String = ConfigurationManager.AppSettings("SSLOffloaded")
            If SSLOffloaded IsNot Nothing AndAlso SSLOffloaded.ToLower = "true" Then
                If HttpContext.Current.Request.Url.Scheme = "http" And HttpContext.Current.Request.Url.Port = "443" Then
                    retURI = "https://" + HttpContext.Current.Request.Url.Host.ToString.Trim + HttpContext.Current.Request.Url.PathAndQuery
                End If
            End If
            If retURI = String.Empty Then
                retURI = HttpContext.Current.Request.Url.AbsoluteUri
            End If
            Return retURI
        End Function

#Region "Clear Cache Methods"

        Public Shared Sub ClearOtherCacheItems()

            'Onaccount cache
            Dim payment As New TalentPayment
            If Not HttpContext.Current.Profile.IsAnonymous Then
                payment.De.CustomerNumber = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID
                payment.RetrieveOnAccountDetailsClearCache()
            End If

            'Travel and events cache
            ClearTravelEventsProductCache()

            'Comments Cache
            ClearCommentsCache()

            'Reserved and sold seats cache
            ClearRerservedAndSoldSeatsCache()
            'Purchase history cache
            'ClearOrderEnquiryDetailsCache()
            ClearDeliveryAddressCntrlSessions()

        End Sub

        Public Shared Sub ClearOrderEnquiryDetailsCache()

            Dim talOrder As New TalentOrder
            'OrderEnquiryDetails cache clearance WS005
            If Not HttpContext.Current.Profile.IsAnonymous Then
                talOrder.Settings.AccountNo1 = Talent.Common.Utilities.PadLeadingZeros(CType(HttpContext.Current.Profile, TalentProfile).User.Details.Account_No_1, 12)
                talOrder.OrderEnquiryDetailsClearCache()
            End If
        End Sub

        Private Shared Sub ClearTravelEventsProductCache()
            Dim productInBasket As String = String.Empty
            If HttpContext.Current.Session("EventProductFoundInBasket") IsNot Nothing Then
                productInBasket = "E"
                HttpContext.Current.Session.Remove("EventProductFoundInBasket")
            End If
            If HttpContext.Current.Session("TravelProductFoundInBasket") IsNot Nothing Then
                If (productInBasket.Length > 0) Then
                    productInBasket = "TE"
                Else
                    productInBasket = "T"
                End If
                HttpContext.Current.Session.Remove("TravelProductFoundInBasket")
            End If
            Dim tempKeyName As String = String.Empty
            If productInBasket = "TE" Then
                For Each dicItem As DictionaryEntry In HttpContext.Current.Cache
                    tempKeyName = dicItem.Key.ToString()
                    If (tempKeyName.StartsWith("ProductListT") OrElse tempKeyName.StartsWith("ProductListE")) Then
                        HttpContext.Current.Cache.Remove(tempKeyName)
                    End If
                Next
            ElseIf productInBasket = "T" Then
                For Each dicItem As DictionaryEntry In HttpContext.Current.Cache
                    tempKeyName = dicItem.Key.ToString()
                    If (tempKeyName.StartsWith("ProductListT")) Then
                        HttpContext.Current.Cache.Remove(tempKeyName)
                    End If
                Next
            ElseIf productInBasket = "E" Then
                For Each dicItem As DictionaryEntry In HttpContext.Current.Cache
                    tempKeyName = dicItem.Key.ToString()
                    If (tempKeyName.StartsWith("ProductListE")) Then
                        HttpContext.Current.Cache.Remove(tempKeyName)
                    End If
                Next
            End If
        End Sub
        Private Shared Sub ClearRerservedAndSoldSeatsCache()
            Dim tempKeyName As String = String.Empty
            For Each dicItem As DictionaryEntry In HttpContext.Current.Cache
                tempKeyName = dicItem.Key.ToString()
                If (tempKeyName.StartsWith("RetrieveReservedAndSoldSeats" & HttpContext.Current.Profile.UserName)) Then
                    HttpContext.Current.Cache.Remove(tempKeyName)
                End If
            Next
        End Sub
        Private Shared Sub ClearCommentsCache()
            ' Remove sessiom variables used as temporary store for comments 
            Dim itemsToDelete As New Collection
            For Each item In HttpContext.Current.Session.Contents
                If item.ToString().StartsWith("comments_") Then

                    itemsToDelete.Add(item.ToString())
                End If
            Next
            For Each item As String In itemsToDelete
                HttpContext.Current.Session.Remove(item)
            Next
        End Sub

        ''' <summary>
        ''' Get the formatted date and time based on the given iseries formatted date
        ''' </summary>
        ''' <param name="productDate">The iseries date</param>
        ''' <param name="productTime">The iseries time</param>
        ''' <param name="dateSeparator">The date separator string</param>
        ''' <param name="globalDateFormat">The global date format module default</param>
        ''' <param name="globalCultureInfo">The global culture info module default</param>
        ''' <returns>The formatted string based on global date format string</returns>
        ''' <remarks></remarks>
        Public Shared Function GetFormattedDateAndTime(ByVal productDate As String, ByVal productTime As String, ByVal dateSeparator As String, _
                                                       ByVal globalDateFormat As String, ByVal globalCultureInfo As String) As String
            Dim formattedDateAndTime As String = String.Empty
            If Not (productDate.Trim = "0" Or productDate.Trim = "0000000") Then
                Dim dateValue As Date = Talent.Common.Utilities.ISeriesDate(productDate.Trim)
                If globalDateFormat = "yyyy/MM/dd" Then
                    Dim dateString As String = dateValue.ToString("dd MMMM")
                    Dim culture As New System.Globalization.CultureInfo(globalCultureInfo)
                    Dim day As String = culture.DateTimeFormat.DayNames(dateValue.DayOfWeek)
                    Dim year As String = dateValue.ToString("yyyy")
                    formattedDateAndTime = day & dateSeparator & dateString & dateSeparator & year
                    If productTime.Trim().Length > 0 Then formattedDateAndTime = formattedDateAndTime & dateSeparator & productTime
                Else
                    formattedDateAndTime = dateValue.ToString(globalDateFormat)
                    If productTime.Trim().Length > 0 Then formattedDateAndTime = formattedDateAndTime & dateSeparator & productTime
                End If
            End If
            Return formattedDateAndTime
        End Function
        ''' <summary>
        ''' Get the date in a basic format
        ''' </summary>
        ''' <param name="dtDate">The iseries date</param>
        ''' <returns>The formatted string based on global date format string</returns>
        ''' <remarks></remarks>
        Public Shared Function GetAgentDate(ByVal dtDate As Date) As String
            Dim strDate As String = String.Empty
            Try
                strDate = dtDate.ToString.Substring(0, 10)
                If strDate.Substring(0, 2) = "00" Then
                    strDate = String.Empty
                End If
            Catch ex As Exception

            End Try
            Return strDate
        End Function
#End Region

        Public Shared Function GetProductPriceCodesDescription(ByVal productCode As String, ByVal productType As String, ByVal stadiumCode As String, ByVal dtProductPriceCodes As DataTable, ByVal dtCampaignPriceCodes As DataTable) As DataTable

            Dim talPricing As New Talent.Common.TalentPricing
            Dim err As New Talent.Common.ErrorObj
            Dim dtProductPriceCodeDesc As New DataTable

            talPricing.Settings() = Talent.eCommerce.Utilities.GetSettingsObject()
            talPricing.De.ProductCode = productCode
            talPricing.De.ProductType = productType
            talPricing.De.StadiumCode = stadiumCode
            talPricing.De.Src = "W"
            'TODO: Caching 
            err = talPricing.ProductPriceCodeDescriptions(dtProductPriceCodes, dtCampaignPriceCodes)
            If (Not err.HasError) AndAlso (talPricing.ResultDataSet IsNot Nothing) Then
                If talPricing.ResultDataSet.Tables("Status").Rows.Count > 0 AndAlso talPricing.ResultDataSet.Tables("Status").Rows(0)(0) = "" Then
                    dtProductPriceCodeDesc = talPricing.ResultDataSet.Tables("ProductPriceCodes")
                End If
            End If
            Return dtProductPriceCodeDesc
        End Function

        Public Shared Function ReplaceSingleQuote(ByVal stringToReplace As String) As String
            If ((stringToReplace <> Nothing) And (stringToReplace.Length > 0)) Then
                stringToReplace = stringToReplace.Replace("'", "''")
            End If
            Return stringToReplace
        End Function

        ''' <summary>
        ''' Formats the 3 or 4 digit card expiry/start dates to a 4 digit display date with forward slash
        ''' </summary>
        ''' <param name="cardDate">The unformatted date string</param>
        ''' <returns>The formatted date string</returns>
        ''' <remarks></remarks>
        Public Shared Function GetFormattedCardDateForDisplay(ByVal cardDate As String) As String
            Dim formattedCardDate As String = cardDate
            If cardDate.Length = 4 Then
                formattedCardDate = cardDate.Substring(0, 2) & "/" & cardDate.Substring(2, 2)
            ElseIf cardDate.Length = 3 Then
                formattedCardDate = "0" & cardDate.Substring(0, 1) & "/" & cardDate.Substring(1, 2)
            Else
                formattedCardDate = String.Empty
            End If
            Return formattedCardDate
        End Function

        ''' <summary>
        ''' Get the correctly formatted destination URL for either seat selection or stand and area selection based on the product setup
        ''' </summary>
        ''' <param name="stadiumName">The given stadium name</param>
        ''' <param name="stadiumCode">The current stadium code</param>
        ''' <param name="productCode">The current product code</param>
        ''' <param name="campaignCode">The campaign code</param>
        ''' <param name="productType">The current product type</param>
        ''' <param name="productSubType">The product sub type</param>
        ''' <param name="productHomeAsAway">Is the product a home as away game</param>
        ''' <param name="restrictGraphical">Is the product restricted for graphical sales</param>
        ''' <param name="isLinkedProduct">If called from the linked product page we never want to go to stand area selection page</param>
        ''' <returns>The correctly formatted URL</returns>
        ''' <remarks></remarks>
        Public Shared Function GetFormattedSeatSelectionUrl(ByRef stadiumName As String, ByVal stadiumCode As String, ByVal productCode As String, ByVal campaignCode As String, _
                                               ByVal productType As String, ByVal productSubType As String, ByVal productHomeAsAway As String, ByVal restrictGraphical As Boolean, Optional ByVal isLinkedProduct As Boolean = False) As String
            Dim redirectUrl As String = String.Empty
            If String.IsNullOrEmpty(stadiumName) Then
                Dim tDataObjects As New TalentDataObjects
                tDataObjects.Settings = GetSettingsObject()
                stadiumName = tDataObjects.StadiumSettings.TblStadiums.GetStadiumNameByStadiumCode(stadiumCode, TalentCache.GetBusinessUnit())
            End If
            If stadiumName.Length = 0 OrElse restrictGraphical Then
                If isLinkedProduct Then
                    Return String.Empty
                Else
                    redirectUrl = "~/PagesPublic/ProductBrowse/StandAndAreaSelection.aspx?stadium={0}&product={1}&campaign={2}&type={3}&productsubtype={4}&productIsHomeAsAway={5}"
                End If
            Else
                redirectUrl = "~/PagesPublic/ProductBrowse/VisualSeatSelection.aspx?stadium={0}&product={1}&campaign={2}&type={3}&productsubtype={4}&productIsHomeAsAway={5}"
            End If
            redirectUrl = String.Format(redirectUrl, stadiumCode.Trim, productCode.Trim, campaignCode.Trim, productType.Trim, productSubType.Trim, productHomeAsAway.Trim)
            Return redirectUrl
        End Function

        ''' <summary>
        ''' Gets the formatted seat for display for all product types. Home/away/season tickets are seat based.
        ''' Membership and Event products are numeric and seat data doesn't apply.
        ''' Travel products if stand is time or if the coach details are given.
        ''' </summary>
        ''' <param name="seat">The seat.</param>
        ''' <param name="productType">Type of the product.</param>
        ''' <param name="allocatedSeat">The allocated seat details</param>
        ''' <param name="mode2">The mode to return an empty string</param>
        ''' <returns>The formatted string</returns>
        Public Shared Function GetFormattedSeatForDisplay(ByVal seat As String, ByVal productType As String, ByVal allocatedSeat As String, Optional mode2 As Boolean = False) As String
            If Not String.IsNullOrWhiteSpace(seat) AndAlso Not String.IsNullOrWhiteSpace(productType) Then
                Dim forwardSlash As String = "/"
                Select Case productType
                    Case Is = GlobalConstants.HOMEPRODUCTTYPE, GlobalConstants.SEASONTICKETPRODUCTTYPE
                        seat = seat.TrimEnd(forwardSlash)
                    Case Is = GlobalConstants.AWAYPRODUCTTYPE
                        If allocatedSeat.Length > 0 Then
                            seat = allocatedSeat.TrimEnd(forwardSlash)
                        Else
                            seat = seat.TrimEnd(forwardSlash)
                        End If
                    Case Is = GlobalConstants.MEMBERSHIPPRODUCTTYPE, GlobalConstants.EVENTPRODUCTTYPE
                        If Not mode2 Then
                            seat = String.Empty
                        Else
                            Dim formattedSeat As New DESeatDetails
                            formattedSeat.FormattedSeat = seat
                            seat = formattedSeat.Stand & formattedSeat.Area & formattedSeat.Row & formattedSeat.Seat & formattedSeat.AlphaSuffix
                        End If
                    Case Is = GlobalConstants.TRAVELPRODUCTTYPE
                        Dim formattedSeat As New DESeatDetails
                        formattedSeat.FormattedSeat = seat
                        seat = formattedSeat.Stand & formattedSeat.Area
                End Select
            End If
            Return seat
        End Function

        Public Shared Function ProcessPartPayment() As Boolean
            Dim canProcessBookingFees As Boolean = False
            Dim talPartPayment As New TalentPartPayment
            talPartPayment.Settings = GetSettingsObject()
            talPartPayment.BasketHeaderID = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
            Dim tDataObjects As New Talent.Common.TalentDataObjects
            tDataObjects.Settings = talPartPayment.Settings
            Dim CardTypeFeeCategory As Dictionary(Of String, String) = tDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(tDataObjects.Settings.BusinessUnit)
            Dim FulfilmentFeeCategory As Dictionary(Of String, String) = tDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(tDataObjects.Settings.BusinessUnit)
            talPartPayment.ProcessPartPaymentBookingFees()
            canProcessBookingFees = talPartPayment.canProcessBookingFees
            Return canProcessBookingFees
        End Function

        Public Shared Function IsPartPayRequiresBookFeeProcess() As Boolean
            Dim talPartPayment As New TalentPartPayment
            talPartPayment.Settings = GetSettingsObject()
            talPartPayment.BasketHeaderID = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
            Dim tDataObjects As New Talent.Common.TalentDataObjects
            tDataObjects.Settings = talPartPayment.Settings
            Dim CardTypeFeeCategory As Dictionary(Of String, String) = tDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(tDataObjects.Settings.BusinessUnit)
            Dim FulfilmentFeeCategory As Dictionary(Of String, String) = tDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(tDataObjects.Settings.BusinessUnit)
            talPartPayment.ProcessPartPaymentBookingFees()
            Return talPartPayment.canProcessBookingFees
        End Function

        Public Shared Function GetPartPaymentFlag() As String
            Dim talPartPayment As New TalentPartPayment
            talPartPayment.Settings = GetSettingsObject()
            talPartPayment.BasketHeaderID = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
            Dim tDataObjects As New Talent.Common.TalentDataObjects
            tDataObjects.Settings = talPartPayment.Settings
            Dim CardTypeFeeCategory As Dictionary(Of String, String) = tDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(tDataObjects.Settings.BusinessUnit)
            Dim FulfilmentFeeCategory As Dictionary(Of String, String) = tDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(tDataObjects.Settings.BusinessUnit)
            Dim partPaymentFlag As String = talPartPayment.GetPartPaymentFlag()
            Return partPaymentFlag
        End Function

        Private Shared Function GetFeeValueForCurrentPartPayment(ByVal feeValue As Decimal) As Decimal
            Dim talPartPayment As New TalentPartPayment
            talPartPayment.Settings = GetSettingsObject()
            talPartPayment.BasketHeaderID = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
            Dim tDataObjects As New Talent.Common.TalentDataObjects
            tDataObjects.Settings = talPartPayment.Settings
            Dim CardTypeFeeCategory As Dictionary(Of String, String) = tDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(tDataObjects.Settings.BusinessUnit)
            Dim FulfilmentFeeCategory As Dictionary(Of String, String) = tDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(tDataObjects.Settings.BusinessUnit)
            feeValue = talPartPayment.GetFeeValueForCurrentPartPayment(feeValue)
            Return feeValue
        End Function

        Public Shared Sub GetBookingFeeValuesForCurrentPartPayment(ByVal currentPartPayAmount As Decimal, ByRef feeValue As Decimal, ByRef feeValueActual As Decimal)
            Dim tempBasketSummary As DEBasketSummary = CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketSummary
            Dim tDataObjects As New Talent.Common.TalentDataObjects
            tDataObjects.Settings = GetSettingsObject()
            Dim CardTypeFeeCategory As Dictionary(Of String, String) = tDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(tDataObjects.Settings.BusinessUnit)
            Dim FulfilmentFeeCategory As Dictionary(Of String, String) = tDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(tDataObjects.Settings.BusinessUnit)
            If Talent.Common.Utilities.IsBookingFeesPercentageBased(tDataObjects.Settings, CardTypeFeeCategory, FulfilmentFeeCategory) Then
                feeValue = Math.Ceiling(((currentPartPayAmount / tempBasketSummary.TotalBasket) * tempBasketSummary.FeesBookingActual) * 100) / 100
                feeValueActual = feeValue
            Else
                feeValue = GetFeeValueForCurrentPartPayment(tempBasketSummary.FeesBookingTotal)
                feeValueActual = tempBasketSummary.FeesBookingActual
            End If
        End Sub

        Public Shared Function GetModifiedWebPrices(ByVal webPricesModifyingMode As Integer, ByVal tempOrderID As String, ByVal liveBasketItems As DataTable, ByVal totals As TalentWebPricing, ByVal currentPageName As String) As TalentWebPricing
            If webPricesModifyingMode <> Nothing AndAlso webPricesModifyingMode > 0 Then
                Try
                    Dim talentWebPriceModifier As New TalentWebPricingModifier(CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID, webPricesModifyingMode)
                    talentWebPriceModifier.CurrentPageName = currentPageName
                    totals = talentWebPriceModifier.GetModifiedWebPrices(CType(HttpContext.Current.Profile, TalentProfile).Basket.Temp_Order_Id, liveBasketItems, totals)
                Catch ex As Exception
                    Utilities.TalentLogging.ExceptionLog("GetModifiedWebPrices", ex)
                End Try
            End If
            Return totals
        End Function

        Public Shared Function GetDeliveryCharges(ByVal isDeliveryCalculationInUse As Boolean) As DEDeliveryCharges.DEDeliveryCharge
            Dim deliveryChargeEntity As DEDeliveryCharges.DEDeliveryCharge = Nothing
            Try
                If isDeliveryCalculationInUse AndAlso HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Session IsNot Nothing AndAlso
                                HttpContext.Current.Session("DeliveryChargeRetail") IsNot Nothing Then
                    deliveryChargeEntity = CType(HttpContext.Current.Session("DeliveryChargeRetail"), DEDeliveryCharges.DEDeliveryCharge)
                End If
            Catch ex As Exception
                deliveryChargeEntity = Nothing
            End Try
            Return deliveryChargeEntity
        End Function

        Public Shared Function IsValidForTaxExemption(ByVal tempOrderID As String) As Boolean
            Dim isValid As Boolean = False
            Dim tDataObjects As New Talent.Common.TalentDataObjects
            tDataObjects.Settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
            tDataObjects.Settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
            tDataObjects.Settings.BusinessUnit = TalentCache.GetBusinessUnit()
            Dim dtOrderHeader As DataTable = tDataObjects.OrderSettings.TblOrderHeader.GetOrderHeaderByTempOrderID(tempOrderID)
            If dtOrderHeader IsNot Nothing AndAlso dtOrderHeader.Rows.Count > 0 Then
                If Not String.IsNullOrWhiteSpace(Utilities.CheckForDBNull_String(dtOrderHeader.Rows(0)("COUNTRY_CODE"))) Then
                    isValid = tDataObjects.ProfileSettings.tblCountry.CanCountryAllowVATExemption(Utilities.CheckForDBNull_String(dtOrderHeader.Rows(0)("COUNTRY_CODE")))
                End If
            End If
            Return isValid
        End Function

        Public Shared Function UpdateRetailBasketItems(ByVal basketHeaderID As String, ByVal webPrices As TalentWebPricing) As Boolean
            Dim isSuccess As Boolean = False
            Dim tDataObjects As New Talent.Common.TalentDataObjects
            tDataObjects.Settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
            tDataObjects.Settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
            tDataObjects.Settings.BusinessUnit = TalentCache.GetBusinessUnit()
            If tDataObjects.BasketSettings.TblBasketDetail.UpdateRetailBasketItems(basketHeaderID, webPrices) > 0 Then
                isSuccess = True
            End If
            Return isSuccess
        End Function

        Public Shared Function GetPostbackControl(ByVal targPage As Page) As Control
            If targPage.IsPostBack Then
                Dim ctlName As String = targPage.Request.Form("__EVENTTARGET")
                If ctlName.Trim().Length > 0 Then
                    Return targPage.FindControl(ctlName)
                End If
                Dim keyName As String
                For Each keyName In targPage.Request.Form
                    Dim ctl As Control = targPage.FindControl(keyName)
                    If Not ctl Is Nothing Then
                        If TypeOf ctl Is Button Then
                            Return ctl
                        End If
                    End If
                Next
            End If

            Return Nothing
        End Function

        Private Shared Function GetDeliveryCountryISOAlpha3Code() As String
            Dim deliveryCountryISOAlpha3Code As String = String.Empty
            If CType(HttpContext.Current.Profile, TalentProfile) IsNot Nothing AndAlso (Not CType(HttpContext.Current.Profile, TalentProfile).IsAnonymous) Then
                Dim deliveryCountryCodeOrDesc As String = String.Empty
                'delivery address country if user selected in delivery control
                If HttpContext.Current.Session IsNot Nothing Then
                    If HttpContext.Current.Session("StoredDeliveryAddress") IsNot Nothing Then
                        deliveryCountryCodeOrDesc = CType(HttpContext.Current.Session("StoredDeliveryAddress"), DEDeliveryDetails).CountryCode
                    ElseIf HttpContext.Current.Session("StoredTicketingDeliveryAddress") IsNot Nothing Then
                        deliveryCountryCodeOrDesc = CType(HttpContext.Current.Session("StoredTicketingDeliveryAddress"), DEDeliveryDetails).CountryCode
                    ElseIf HttpContext.Current.Session("StoredRetailDeliveryAddress") IsNot Nothing Then
                        deliveryCountryCodeOrDesc = CType(HttpContext.Current.Session("StoredRetailDeliveryAddress"), DEDeliveryDetails).CountryCode
                    End If
                End If
                If String.IsNullOrWhiteSpace(deliveryCountryCodeOrDesc) Then
                    deliveryCountryCodeOrDesc = GetDeliveryCountry()
                End If
                deliveryCountryISOAlpha3Code = GetDeliveryCountryISOAlpha3Code(deliveryCountryCodeOrDesc)
            End If

            If String.IsNullOrWhiteSpace(deliveryCountryISOAlpha3Code) Then
                deliveryCountryISOAlpha3Code = String.Empty
            End If
            Return deliveryCountryISOAlpha3Code
        End Function

        Public Shared Function GetDeliveryCountryISOAlpha3Code(ByVal countryCodeOrDescription As String) As String
            Dim countryISOAlpha3Code As String = String.Empty
            If Not String.IsNullOrWhiteSpace(countryCodeOrDescription) Then
                Dim tDataObjects As New Talent.Common.TalentDataObjects()
                Dim talentDefaults As New TalentDefaults
                tDataObjects.Settings.Cacheing = True
                tDataObjects.Settings.BusinessUnit = TalentCache.GetBusinessUnit
                tDataObjects.Settings.Partner = TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile))
                tDataObjects.Settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                tDataObjects.Settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
                tDataObjects.Settings.OriginatingSourceCode = GlobalConstants.SOURCE
                countryISOAlpha3Code = tDataObjects.ProfileSettings.tblCountry.GetISOAlpha3ByCodeOrDescription(countryCodeOrDescription)
            End If
            Return countryISOAlpha3Code
        End Function

        Private Shared Function GetDeliveryCountry() As String
            Dim country As String = String.Empty
            If CType(HttpContext.Current.Profile, TalentProfile) IsNot Nothing AndAlso (Not CType(HttpContext.Current.Profile, TalentProfile).IsAnonymous) Then
                'registered address country
                If String.IsNullOrWhiteSpace(country) AndAlso CType(HttpContext.Current.Profile, TalentProfile).User IsNot Nothing AndAlso CType(HttpContext.Current.Profile, TalentProfile).User.Addresses IsNot Nothing Then
                    country = ProfileHelper.ProfileAddressEnumerator(0, CType(HttpContext.Current.Profile, TalentProfile).User.Addresses).Country
                End If
            End If
            Return country
        End Function

        Private Shared Function IsTKTFulfilmentValidationRequired() As Boolean
            Dim isExists As Boolean = False
            For Each item As DEBasketItem In CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems
                If item.CURR_FULFIL_SLCTN = "P" OrElse item.CURR_FULFIL_SLCTN = "R" Then
                    isExists = True
                    Exit For
                End If
            Next
            Return isExists
        End Function

        Public Shared Sub ClearDeliveryAddressCntrlSessions()
            HttpContext.Current.Session("StoredRetailDeliveryAddress") = Nothing
            HttpContext.Current.Session("StoredTicketingDeliveryAddress") = Nothing
            HttpContext.Current.Session("DeliveryDate") = Nothing
            HttpContext.Current.Session("NewAddressReference") = Nothing
            HttpContext.Current.Session("dtPreferredDeliveryDates") = Nothing
            HttpContext.Current.Session("StoredDeliveryAddress") = Nothing
            HttpContext.Current.Session("DeliveryDetails") = Nothing
            HttpContext.Current.Session("deliveryAddressValidated") = Nothing
            HttpContext.Current.Session("DeliveryChargeRetail") = Nothing
        End Sub

        Public Shared Function IsValidTicketFulfilmentDeliveryCountry(ByVal countryCode As String) As Boolean
            Dim isValid As Boolean = True
            If IsTKTFulfilmentValidationRequired() Then
                If Not String.IsNullOrWhiteSpace(countryCode) AndAlso countryCode <> "--" Then
                    Dim countryEntity As DECountry = Nothing
                    Dim talDefaults As New TalentDefaults
                    talDefaults.Settings = GetSettingsObject()
                    countryEntity = talDefaults.RetrieveSpecificCountryDefinition(countryCode)
                    If countryEntity IsNot Nothing Then


                    End If
                End If
            End If
            Return isValid
        End Function

        Public Shared Function SetVGAttributesSession(ByVal basketPaymentID As Long, ByVal vgAttributes As DEVanguard, ByVal basketPaymentEntity As DEBasketPayment) As Boolean
            Dim isSet As Boolean = False
            Try
                If HttpContext.Current.Session IsNot Nothing Then
                    If basketPaymentID <> Nothing AndAlso basketPaymentID > 0 AndAlso vgAttributes IsNot Nothing Then
                        If HttpContext.Current.Session("VG_BasketPaymentID") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(HttpContext.Current.Session("VG_BasketPaymentID")) Then
                            'remove those session before instantiate new one
                            If HttpContext.Current.Session("VG_VGAttributes_" & HttpContext.Current.Session("VG_BasketPaymentID").ToString()) IsNot Nothing Then
                                HttpContext.Current.Session.Remove("VG_VGAttributes_" & HttpContext.Current.Session("VG_BasketPaymentID").ToString())
                                HttpContext.Current.Session.Remove("VG_VGBasketPayment_" & HttpContext.Current.Session("VG_BasketPaymentID").ToString())
                            End If
                        End If
                        HttpContext.Current.Session("VG_BasketPaymentID") = basketPaymentID
                        HttpContext.Current.Session("VG_VGAttributes_" & basketPaymentID.ToString()) = vgAttributes
                        HttpContext.Current.Session("VG_VGBasketPayment_" & basketPaymentID.ToString()) = basketPaymentEntity
                        isSet = True
                    End If
                End If
            Catch ex As Exception
                isSet = False
            End Try
            Return isSet
        End Function

        Public Shared Function TryGetVGAttributesSession(ByRef basketPaymentID As Long, ByRef vgAttributes As DEVanguard, ByRef basketPaymentEntity As DEBasketPayment) As Boolean
            'todo this has to make a dbcall to populate the below session to make sure payment process not depend on session variable even app pool is recycled.
            Dim isExists As Boolean = False
            If HttpContext.Current.Session IsNot Nothing Then
                If HttpContext.Current.Session("VG_BasketPaymentID") IsNot Nothing Then
                    If HttpContext.Current.Session("VG_VGAttributes_" & HttpContext.Current.Session("VG_BasketPaymentID").ToString()) IsNot Nothing Then
                        basketPaymentID = HttpContext.Current.Session("VG_BasketPaymentID")
                        vgAttributes = CType(HttpContext.Current.Session("VG_VGAttributes_" & basketPaymentID.ToString()), DEVanguard)
                        basketPaymentEntity = CType(HttpContext.Current.Session("VG_VGBasketPayment_" & basketPaymentID.ToString()), DEBasketPayment)
                        isExists = True
                    End If
                End If
            End If
            Return isExists
        End Function


        ''' <summary>
        ''' Session usage in application increased, so clear those as early as possible
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Sub ClearAllSessions()
            If Not IsAgent() Then
                If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Session IsNot Nothing Then
                    HttpContext.Current.Session.Clear()
                    HttpContext.Current.Session.Abandon()
                End If
            End If
        End Sub

        Public Shared Function SetVanguardDefaultAttributes(ByVal dicVanguardAttributes As Dictionary(Of String, String), ByVal talVanguard As TalentVanguard) As TalentVanguard
            If dicVanguardAttributes IsNot Nothing AndAlso dicVanguardAttributes.Count > 0 Then
                Try
                    talVanguard.VanguardAttributes.CDATAWrapping = CheckForDBNull_Boolean_DefaultTrue(dicVanguardAttributes("CDATA_WRAPPING"))
                    talVanguard.VanguardAttributes.FullCapture = CheckForDBNull_Boolean_DefaultTrue(dicVanguardAttributes("FULL_CAPTURE"))
                    talVanguard.VanguardAttributes.CardCapturePage = CheckForDBNull_String(dicVanguardAttributes("CARD_CAPTURE_PAGE"))
                    talVanguard.VanguardAttributes.TerminalCountryCode = CheckForDBNull_String(dicVanguardAttributes("TERMINAL_COUNTRY_CODE"))
                    talVanguard.VanguardAttributes.TalentVGGatewayPage = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) & CheckForDBNull_String(dicVanguardAttributes("TALENT_VGGATEWAYPAGE"))
                    talVanguard.VanguardAttributes.CanLogTheProcessXML = CheckForDBNull_Boolean_DefaultFalse(dicVanguardAttributes("CAN_LOG_PROCESS_XML"))
                    talVanguard.VanguardAttributes.CurrencyExponent = CheckForDBNull_String(dicVanguardAttributes("CURRENCY_EXPONENT"))
                    talVanguard.VanguardAttributes.IframeWidth = CheckForDBNull_Int(dicVanguardAttributes("IFRAME_WIDTH"))
                    talVanguard.VanguardAttributes.TransactionCurrencyCode = CheckForDBNull_String(dicVanguardAttributes("TRANSACTION_CURRENCY_CODE"))
                    talVanguard.VanguardAttributes.TransactionDescription = CheckForDBNull_String(dicVanguardAttributes("TRANSACTION_DESCRIPTION"))
                    talVanguard.VanguardAttributes.BrowserUserAgentHeader = HttpContext.Current.Request.UserAgent 'CheckForDBNull_String(dicVanguardAttributes("BROWSER_USER_AGENT_HEADER"))
                    talVanguard.VanguardAttributes.BrowserAcceptHeader = CheckForDBNull_String(dicVanguardAttributes("BROWSER_ACCEPT_HEADER"))
                    talVanguard.VanguardAttributes.CacheConfigSeconds = CheckForDBNull_Int(dicVanguardAttributes("CONFIG_CACHE_SECONDS"))
                    'Dim currencycode As String = "826" 'Retrieve as per Retail Logic
                    'If currenciesTable.Rows.Count > 0 AndAlso TECUtilities.CheckForDBNull_String(currenciesTable.Rows(0)("CURRENCY_CODE_1").ToString) <> String.Empty Then
                    '    currencycode = currenciesTable.Rows(0)("CURRENCY_CODE_1").ToString
                    'End If
                Catch ex As Exception

                End Try

            End If
            Return talVanguard
        End Function

        Public Shared Function GetVanguardDefaultAttribute(ByVal dicVanguardAttributes As Dictionary(Of String, String), ByVal keyName As String, ByVal defaultValue As String) As String
            If dicVanguardAttributes IsNot Nothing AndAlso dicVanguardAttributes.Count > 0 Then
                Try
                    If dicVanguardAttributes.ContainsKey(keyName) Then
                        defaultValue = dicVanguardAttributes(keyName)
                    End If
                Catch ex As Exception

                End Try
            End If
            Return defaultValue
        End Function

        Public Shared Sub TicketingCheckoutExternalStart(ByVal isPagePostback As Boolean, ByVal paymentGatewayType As String, ByVal isPartPayment As Boolean)
            If isPagePostback Then
                If (HttpContext.Current.Session("CheckoutExternalStarted") Is Nothing) Then
                    If paymentGatewayType.ToUpper() = GlobalConstants.PAYMENTGATEWAY_VANGUARD Then
                        HttpContext.Current.Session("ExternalGatewayPayType") = GlobalConstants.PAYMENTTYPE_VANGUARD
                        If Not isPartPayment Then
                            Select Case Talent.eCommerce.Utilities.BasketContentTypeWithOverride
                                Case GlobalConstants.TICKETINGBASKETCONTENTTYPE, GlobalConstants.COMBINEDBASKETCONTENTTYPE
                                    Talent.eCommerce.Checkout.RemovePaymentDetailsFromSession()
                                    Dim ticketGatewayFunc As New TicketingGatewayFunctions
                                    ticketGatewayFunc.CheckoutExternalFailure(False, CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID, True)
                                    ticketGatewayFunc = New TicketingGatewayFunctions
                                    ticketGatewayFunc.CheckoutExternal(False)
                            End Select
                            If HttpContext.Current.Session("TalentErrorCode") IsNot Nothing AndAlso HttpContext.Current.Session("TalentErrorCode").ToString.Trim.Length > 0 Then
                                HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/basket.aspx")
                            ElseIf HttpContext.Current.Session("TicketingGatewayError") IsNot Nothing AndAlso HttpContext.Current.Session("TicketingGatewayError").ToString.Trim.Length > 0 Then
                                HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/basket.aspx")
                            Else
                                HttpContext.Current.Session("CheckoutExternalStarted") = True
                            End If
                        End If
                    End If
                End If
            Else
                'reset the session variable here
                If HttpContext.Current.Session("CheckoutExternalStarted") IsNot Nothing Then
                    HttpContext.Current.Session.Remove("CheckoutExternalStarted")
                End If
            End If
        End Sub


        ''' <summary>
        ''' Add to the last customer selected or added to the session to display on customer selection
        ''' If the item is either listed or has hit capacity, remove the exsiting one or the last one and add the new one to the list
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Sub addCustomerLoggedInToSession(ByVal NoOfRecentlyUsedCustomers As Integer, ByVal LoginID As String, ByVal Forename As String, ByVal Surname As String)

            If IsAgent() Then
                If NoOfRecentlyUsedCustomers > 0 Then
                    Dim lastNCustomerLogins As New List(Of String())
                    Dim customerSearched As String
                    Dim hasPair As Boolean = False
                    Dim key As String
                    Dim value() As String
                    key = LoginID
                    customerSearched = String.Format("{0} - {1}", key.TrimStart(GlobalConstants.LEADING_ZEROS), Forename & " " & Surname)
                    value = {key, customerSearched}

                    If HttpContext.Current.Session("LastNCustomerLogins") IsNot Nothing Then
                        lastNCustomerLogins = CType(HttpContext.Current.Session("LastNCustomerLogins"), List(Of String()))
                        For i As Integer = 0 To lastNCustomerLogins.Count - 1
                            If lastNCustomerLogins(i)(0) = key Then
                                lastNCustomerLogins.RemoveAt(i)
                                hasPair = True
                                Exit For
                            End If
                        Next
                        If Not hasPair Then
                            If lastNCustomerLogins.Count >= NoOfRecentlyUsedCustomers Then
                                lastNCustomerLogins.RemoveAt(0)
                            End If
                        End If
                    End If

                    lastNCustomerLogins.Add(value)
                    HttpContext.Current.Session("LastNCustomerLogins") = lastNCustomerLogins
                End If
            End If

        End Sub

        ''' <summary>
        ''' Get the seat restriction description based on the given restriction code
        ''' </summary>
        ''' <param name="restrictionCode">The restriction code</param>
        ''' <returns>The restriction description</returns>
        ''' <remarks></remarks>
        Public Shared Function RestrictedSeatDescription(ByVal restrictionCode As String) As String
            Dim desc As String = String.Empty
            If restrictionCode.Trim.Length > 0 Then
                Dim ucr As New UserControlResource
                Dim err As New ErrorObj
                Dim sett As DESettings = Utilities.GetSettingsObject()
                Dim util As New Talent.Common.TalentUtiltities

                With ucr
                    .BusinessUnit = TalentCache.GetBusinessUnit
                    .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                    .PageCode = Talent.Common.Utilities.GetAllString
                    .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                    .KeyCode = "TicketingBasketDetails.ascx"
                End With
                sett.Cacheing = True
                If String.IsNullOrEmpty(ucr.Attribute("RestrictionCacheTimeMinutes")) Then
                    sett.CacheTimeMinutes = 60
                Else
                    sett.CacheTimeMinutes = CInt(ucr.Attribute("RestrictionCacheTimeMinutes"))
                End If

                'Retrive the seat restriction descriptions
                util.Settings = sett
                util.DescriptionKey = "SRSC"
                err = util.RetrieveDescriptionEntries

                'Find the one relevant to this code
                If Not err.HasError AndAlso Not util.ResultDataSet Is Nothing AndAlso util.ResultDataSet.Tables.Count > 1 Then
                    For Each dr As DataRow In util.ResultDataSet.Tables(1).Rows
                        If dr.Item("Code").ToString.Trim = restrictionCode.Trim Then
                            desc = ucr.Content("RestrictionDescriptionPrefix", Talent.Common.Utilities.GetDefaultLanguage, True) & dr.Item("Description").ToString.Trim
                            Exit For
                        End If
                    Next
                End If
            End If
            Return desc
        End Function

        ''' <summary>
        ''' Create string from dictionary
        ''' </summary>
        ''' <param name="values">Dictionary to format</param>
        ''' <returns>The formatted string</returns>
        ''' <remarks></remarks>
        Public Shared Function Stringify(ByVal values As Dictionary(Of String, String)) As String
            Dim results As New List(Of String)
            For Each key In values.Keys
                results.Add(String.Format("{0}={1}", key, values(key)))
            Next
            Return String.Join(",", results.ToArray)
        End Function

        ''' <summary>
        ''' Remove a querystring item by key
        ''' </summary>
        ''' <param name="url">url to process</param>
        ''' <param name="key">key to remove from the string</param>
        ''' <returns>The URL without the querystring</returns>
        ''' <remarks></remarks>
        Public Shared Function RemoveQueryStringByKey(ByVal url As String, ByVal key As String) As String
            Dim urlPath = url.Split("?")
            If urlPath.Length > 1 Then
                Dim newQueryString = HttpUtility.ParseQueryString(urlPath(1))
                newQueryString.Remove(key)
                If newQueryString.Count > 0 Then
                    Return String.Format("{0}?{1}", urlPath(0), newQueryString)
                Else
                    Return urlPath(0)
                End If
            Else
                Return url
            End If
        End Function
    End Class
End Namespace