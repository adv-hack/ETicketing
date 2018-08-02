Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports Talent.Common

Public Class TalentCache

    ''' <summary>
    ''' Returns language and control specific list items for populating Drop Down Lists
    ''' </summary>
    ''' <param name="language">LANGUAGE CODE for the language in use.
    ''' N.B. if blank string is passed will return the standard English set
    ''' </param>
    ''' <param name="Qualifier">Options: REGISTRATION, UPDATE, DELIVERY, PAYMENT, PAYMENT_TYPE</param>
    ''' <param name="control">Options: TITLE, COUNTRY, CARD, DESCRIPTION, PAYMENT_TYPE</param>
    ''' <param name="type">Used for DESCRIPTION</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetDropDownControlText(ByVal language As String, _
                                                    ByVal Qualifier As String, _
                                                    ByVal control As String, _
                                                    Optional ByVal type As String = "") As ListItemCollection

        Dim lic As New ListItemCollection
        Dim li As ListItem
        Dim dt As New Data.DataTable

        Dim exists As Boolean = False

        dt = GetDDLDataTable(control, Qualifier, type)

        For Each row As Data.DataRow In dt.Rows
            For Each item As ListItem In lic
                If item.Value = row(control & "_CODE") Then
                    exists = True
                    Exit For
                End If
            Next
            If Not exists Then
                Select Case UCase(control)
                    'Case Is = "PAYMENT_TYPE"
                    '    li = New ListItem(row(control & "_DESCRIPTION"), Talent.Common.Utilities.CheckForDBNull_String(row("NAVIGATE_URL")))
                    '    lic.Add(li)
                    Case Else
                        li = New ListItem(row(control & "_DESCRIPTION"), row(control & "_CODE"))
                        lic.Add(li)
                End Select
            Else
                exists = False
            End If
        Next

        'replace any items with their language specific equivalent if any exist
        For Each item As ListItem In lic
            For Each row As Data.DataRow In dt.Rows
                If row(control & "_CODE") = item.Value AndAlso Not String.IsNullOrEmpty(language) AndAlso ProfileHelper.CheckDBNull(row(control & "_LANGUAGE")) = language Then
                    item.Text = row("LANGUAGE_DESCRIPTION")
                End If
            Next
        Next
        If lic.Count > 0 Then
            lic.Insert(0, New ListItem(" -- ", " -- "))
        End If
        Return lic
    End Function

    Public Shared Function GetListCollectionFromTitlesDataTable(ByVal titlesDataTable As DataTable) As ListItemCollection
        Dim collection As New ListItemCollection
        If titlesDataTable.Rows.Count > 0 Then
            collection.Insert(0, New ListItem(" -- ", " -- "))
            For Each title As DataRow In titlesDataTable.Rows
                collection.Add(New ListItem(title("TitleDescription"), title("TitleCode")))
            Next
        End If
        Return collection
    End Function

    Protected Shared Function GetDDLDataTable(ByVal control As String, _
                                                ByVal qualifier As String, _
                                                Optional ByVal type As String = "") As Data.DataTable

        control = UCase(control)
        Dim all As String = Talent.Common.Utilities.GetAllString
        Dim bu As String = GetBusinessUnit()
        Dim part As String = GetPartner(HttpContext.Current.Profile)
        Dim cacheKey As String = bu & part & qualifier & control

        If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            Return GetPropertyFromCache(cacheKey)
        Else
            Dim dt As New Data.DataTable
            dt = GetPropertyFromDB(bu, part, control, qualifier, type)
            AddPropertyToCache(cacheKey, dt, 30, TimeSpan.Zero, CacheItemPriority.Low)
            Return dt
        End If

    End Function



    Protected Shared Function GetPropertyFromDB(ByVal bu As String, _
                                                    ByVal part As String, _
                                                    ByVal control As String, _
                                                    ByVal qualifier As String, _
                                                    Optional ByVal type As String = "") As Data.DataTable

        Dim all As String = Talent.Common.Utilities.GetAllString
        Dim titles As New TalentApplicationVariablesTableAdapters.tbl_title_buTableAdapter 
        Dim countries As New TalentApplicationVariablesTableAdapters.tbl_country_buTableAdapter 
        Dim desc As New TalentDescriptionsDataSetTableAdapters.tbl_ebusiness_descriptions_buTableAdapter 
        Dim card As New TalentApplicationVariablesTableAdapters.tbl_creditcard_buTableAdapter 
        Dim pay As New TalentApplicationVariablesTableAdapters.PaymentTypesTA
        Dim dt As New Data.DataTable


        'If no result, try to get from DB
        Select Case UCase(control)
            Case Is = "TITLE"
                dt = titles.Get_Titles_By_Lang_BU_Partner_Qualifier(qualifier, bu, part)
            Case Is = "COUNTRY"
                dt = countries.Get_By_BU_Partner_Country_Lang(qualifier, bu, part)
            Case Is = "DESCRIPTION"
                dt = desc.GetDatabyBUetc(qualifier, bu, part, type)
            Case Is = "CARD"
                dt = card.Get_CreditCards(qualifier, bu, part)
            Case Is = "PAYMENT_TYPE"
                If qualifier = "PAYMENT_TYPE" Then
                    dt = pay.GetData_PaymentTypes(qualifier, bu, part)
                ElseIf qualifier = "DEFAULT_PAYMENT_TYPE" Then
                    dt = pay.GetData_DefaultPaymentType(qualifier, bu, part)
                End If
        End Select

        If dt.Rows.Count = 0 Then

            'If this is no cache here either
            'Try to get from DB using *ALL as qualifier
            Select Case UCase(control)
                Case Is = "TITLE"
                    dt = titles.Get_Titles_By_Lang_BU_Partner_Qualifier(all, bu, part)
                Case Is = "COUNTRY"
                    dt = countries.Get_By_BU_Partner_Country_Lang(all, bu, part)
                Case Is = "DESCRIPTION"
                    dt = desc.GetDatabyBUetc(all, bu, part, type)
                Case Is = "CARD"
                    dt = card.Get_CreditCards(all, bu, part)
                Case Is = "PAYMENT_TYPE"
                    If qualifier = "PAYMENT_TYPE" Then
                        dt = pay.GetData_PaymentTypes(qualifier, bu, part)
                    ElseIf qualifier = "DEFAULT_PAYMENT_TYPE" Then
                        dt = pay.GetData_DefaultPaymentType(qualifier, bu, part)
                    End If
            End Select

            If dt.Rows.Count = 0 Then

                'If this is no cache here either
                'Try with only partner *ALL
                Select Case UCase(control)
                    Case Is = "TITLE"
                        dt = titles.Get_Titles_By_Lang_BU_Partner_Qualifier(qualifier, bu, all)
                    Case Is = "COUNTRY"
                        dt = countries.Get_By_BU_Partner_Country_Lang(qualifier, bu, all)
                    Case Is = "DESCRIPTION"
                        dt = desc.GetDatabyBUetc(qualifier, bu, all, type)
                    Case Is = "CARD"
                        dt = card.Get_CreditCards(qualifier, bu, all)
                    Case Is = "PAYMENT_TYPE"
                        If qualifier = "PAYMENT_TYPE" Then
                            dt = pay.GetData_PaymentTypes(qualifier, bu, all)
                        ElseIf qualifier = "DEFAULT_PAYMENT_TYPE" Then
                            dt = pay.GetData_DefaultPaymentType(qualifier, bu, all)
                        End If
                End Select

                If dt.Rows.Count = 0 Then

                    'If this is no cache here either
                    'Try with only BU *ALL
                    Select Case UCase(control)
                        Case Is = "TITLE"
                            dt = titles.Get_Titles_By_Lang_BU_Partner_Qualifier(qualifier, all, part)
                        Case Is = "COUNTRY"
                            dt = countries.Get_By_BU_Partner_Country_Lang(qualifier, all, part)
                        Case Is = "DESCRIPTION"
                            dt = desc.GetDatabyBUetc(qualifier, all, part, type)
                        Case Is = "CARD"
                            dt = card.Get_CreditCards(qualifier, all, part)
                        Case Is = "PAYMENT_TYPE"
                            If qualifier = "PAYMENT_TYPE" Then
                                dt = pay.GetData_PaymentTypes(qualifier, all, part)
                            ElseIf qualifier = "DEFAULT_PAYMENT_TYPE" Then
                                dt = pay.GetData_DefaultPaymentType(qualifier, all, part)
                            End If
                    End Select

                    If dt.Rows.Count = 0 Then

                        'If this is no cache here either
                        'Try with both partner & BU *ALL
                        Select Case UCase(control)
                            Case Is = "TITLE"
                                dt = titles.Get_Titles_By_Lang_BU_Partner_Qualifier(qualifier, all, all)
                            Case Is = "COUNTRY"
                                dt = countries.Get_By_BU_Partner_Country_Lang(qualifier, all, all)
                            Case Is = "DESCRIPTION"
                                dt = desc.GetDatabyBUetc(qualifier, all, all, type)
                            Case Is = "CARD"
                                dt = card.Get_CreditCards(qualifier, all, all)
                            Case Is = "PAYMENT_TYPE"
                                If qualifier = "PAYMENT_TYPE" Then
                                    dt = pay.GetData_PaymentTypes(qualifier, all, all)
                                ElseIf qualifier = "DEFAULT_PAYMENT_TYPE" Then
                                    dt = pay.GetData_DefaultPaymentType(qualifier, all, all)
                                End If
                        End Select


                        If dt.Rows.Count = 0 Then

                            'If this is no cache here either
                            'Try to get from DB using *ALL as qualifier and partner
                            Select Case UCase(control)
                                Case Is = "TITLE"
                                    dt = titles.Get_Titles_By_Lang_BU_Partner_Qualifier(all, bu, all)
                                Case Is = "COUNTRY"
                                    dt = countries.Get_By_BU_Partner_Country_Lang(all, bu, all)
                                Case Is = "DESCRIPTION"
                                    dt = desc.GetDatabyBUetc(all, bu, all, type)
                                Case Is = "CARD"
                                    dt = card.Get_CreditCards(all, bu, all)
                                Case Is = "PAYMENT_TYPE"
                                    If qualifier = "PAYMENT_TYPE" Then
                                        dt = pay.GetData_PaymentTypes(all, bu, all)
                                    ElseIf qualifier = "DEFAULT_PAYMENT_TYPE" Then
                                        dt = pay.GetData_DefaultPaymentType(all, bu, all)
                                    End If
                            End Select

                            If dt.Rows.Count = 0 Then

                                'If this is no cache here either
                                'Try to get from DB using *ALL as qualifier and partner
                                Select Case UCase(control)
                                    Case Is = "TITLE"
                                        dt = titles.Get_Titles_By_Lang_BU_Partner_Qualifier(all, all, all)
                                    Case Is = "COUNTRY"
                                        dt = countries.Get_By_BU_Partner_Country_Lang(all, all, all)
                                    Case Is = "DESCRIPTION"
                                        dt = desc.GetDatabyBUetc(all, all, all, type)
                                    Case Is = "CARD"
                                        dt = card.Get_CreditCards(all, all, all)
                                    Case Is = "PAYMENT_TYPE"
                                        If qualifier = "PAYMENT_TYPE" Then
                                            dt = pay.GetData_PaymentTypes(all, all, all)
                                        ElseIf qualifier = "DEFAULT_PAYMENT_TYPE" Then
                                            dt = pay.GetData_DefaultPaymentType(all, all, all)
                                        End If
                                End Select

                            End If
                        End If
                    End If
                End If
            End If
        End If
        Return dt
    End Function

    Protected Shared Function GetPropertyFromCache(ByVal CacheName As String) As Object
        Return HttpContext.Current.Cache.Item(CacheName)
    End Function

    Public Shared Sub AddPropertyToCache(ByVal CacheName As String, ByVal value As Object, ByVal ExpireInXMinutes As Integer, ByVal SlidingTime As System.TimeSpan, ByVal Priority As CacheItemPriority)
        'If Not HttpContext.Current.Cache(CacheName) Is Nothing Then
        'HttpContext.Current.Cache.Remove(CacheName)
        'End If

        Try
            If HttpContext.Current.Cache(CacheName) Is Nothing Then
                HttpContext.Current.Cache.Add(CacheName, value, Nothing, Now.AddMinutes(ExpireInXMinutes), SlidingTime, Priority, Nothing)
            End If
            Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(CacheName)
        Catch ex As Exception
        End Try

    End Sub

    Public Shared Function GetDefaultPartner(Optional ByVal businessUnit$ = Nothing) As String
        If businessUnit Is Nothing OrElse Trim(businessUnit) = String.Empty Then
            businessUnit = TalentCache.GetBusinessUnit()
        End If
        Dim partner As String = Nothing
        '-----------------------
        ' Check if it's in cache
        '-----------------------
        Dim cacheKey As String = "TalentCache_GetDefaultParner" & businessUnit
        If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            partner = CType(HttpContext.Current.Cache.Item(cacheKey), String)
            Return partner
        Else
            Dim dt As New System.Data.DataTable
            Try
                ' Try to retrieve the table from cache
                dt = CType(HttpContext.Current.Cache("DefaultPartnersTable"), System.Data.DataTable)
                If dt Is Nothing Then
                    ' Populate from the database when the cache retrieval fails
                    Dim partners As New TalentApplicationVariablesTableAdapters.tbl_authorized_partnersTableAdapter
                    dt = partners.Get_Default_Partners
                    AddPropertyToCache("DefaultPartnersTable", dt, 30, TimeSpan.Zero, CacheItemPriority.Normal)
                End If

                'Retrieve the default partner for this business unit
                If Not dt Is Nothing Then
                    For Each row As System.Data.DataRow In dt.Rows
                        If row("BUSINESS_UNIT") = businessUnit Then
                            partner = row("PARTNER")
                        End If
                        '-----------------------
                        ' Put in cache
                        '-----------------------
                        HttpContext.Current.Cache.Insert("TalentCache_GetDefaultParner" & row("BUSINESS_UNIT"),
                                row("PARTNER"),
                                Nothing,
                                System.DateTime.Now.AddMinutes(30),
                                Caching.Cache.NoSlidingExpiration)
                        Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord("TalentCache_GetDefaultParner" & row("BUSINESS_UNIT"))
                    Next
                    Return partner
                End If
                Return Talent.Common.Utilities.GetAllString
            Catch ex As Exception
                Return Talent.Common.Utilities.GetAllString
            End Try
        End If
    End Function

    Public Shared Function GetPartner(ByVal profile As TalentProfile, Optional ByVal businessUnit$ = Nothing) As String
        Try
            If Not profile Is Nothing Then
                If profile.IsAnonymous Then
                    Return GetDefaultPartner(businessUnit)
                Else
                    If Not profile.PartnerInfo Is Nothing _
                        AndAlso Not profile.PartnerInfo.Details Is Nothing _
                            AndAlso Not profile.PartnerInfo.Details.Partner Is Nothing Then
                        Return profile.PartnerInfo.Details.Partner
                    Else
                        Return GetDefaultPartner(businessUnit)
                    End If
                End If
            Else
                Return GetDefaultPartner(businessUnit)
            End If
        Catch ex As Exception
            Talent.Common.Utilities.TalentCommonLog("Exception Logging", "TalentCache.vb - GetPartner - 8.1", ex.Message)
            Return GetDefaultPartner(businessUnit)
        End Try

    End Function

    Public Shared Function GetPartnerByUserName(ByVal profile As TalentProfile) As String
        Dim tempPartner As String = String.Empty
        Try
            If HttpContext.Current.Session(profile.UserName) IsNot Nothing Then
                tempPartner = HttpContext.Current.Session(profile.UserName).ToString
            Else
                If (profile IsNot Nothing) AndAlso (Not profile.IsAnonymous) Then
                    'get from db and move to session
                    Dim userProfileDetails As New TalentMembershipDatasetTableAdapters.tbl_partner_userTableAdapter
                    Dim dt As System.Data.DataTable
                    dt = userProfileDetails.GetDataByLoginIDNoPartner(profile.UserName)
                    If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                        tempPartner = Talent.eCommerce.Utilities.CheckForDBNull_String(dt.Rows(0)("PARTNER"))
                        HttpContext.Current.Session(profile.UserName) = tempPartner
                    End If
                End If
            End If
            If String.IsNullOrWhiteSpace(tempPartner) Then
                tempPartner = GetDefaultPartner()
            End If
        Catch ex As Exception
            Dim logDetails As String = String.Empty
            If profile.UserName IsNot Nothing Then
                logDetails = profile.UserName
            Else
                logDetails = "Profile.UserName is nothing"
            End If
            Talent.eCommerce.Utilities.TalentLogging.GeneralLog("TalentCache.vb", "GetPartnerByUserName", logDetails & " : Exception:" & ex.Message & ":" & ex.StackTrace, "ProfileLog")
            tempPartner = GetDefaultPartner()
        End Try
        Return tempPartner
    End Function

    Public Shared Function GetBusinessUnitURLDeviceEntity() As DEBusinessUnitURLDevice
        Dim url() As String
        'Get the url and split it on the "/" char
        url = HttpContext.Current.Request.Url.ToString.Split("/")
        ' Check for localhost:port (debugging locally) and treat as localhost
        Dim j As Integer = 0
        If HttpContext.Current.Request.Url.ToString.Contains("localhost:") Then
            Do While j < url.Length
                If url(j).Contains("localhost:") Then
                    url(j) = "localhost"
                    Exit Do
                End If
                j += 1
            Loop
        End If
        Dim BusinessUnitURLDeviceEntity As DEBusinessUnitURLDevice = TryToGetCachedBusinessUnit(url)
        If BusinessUnitURLDeviceEntity Is Nothing OrElse String.IsNullOrEmpty(BusinessUnitURLDeviceEntity.BusinessUnit) Then
            BusinessUnitURLDeviceEntity = GetBusinessUnitFromDB(url)
        End If
        Return BusinessUnitURLDeviceEntity
    End Function

    Public Shared Function GetBusinessUnit() As String
        Dim url() As String
        'Get the url and split it on the "/" char
        url = HttpContext.Current.Request.Url.ToString.Split("/")

        ' Check for localhost:port (debugging locally) and treat as localhost
        Dim j As Integer = 0
        If HttpContext.Current.Request.Url.ToString.Contains("localhost:") Then
            Do While j < url.Length
                If url(j).Contains("localhost:") Then
                    url(j) = "localhost"
                    Exit Do
                End If
                j += 1
            Loop
        End If
        '-------------------------
        ' Try to get BU from cache
        '-------------------------
        Dim cachedBusinessUnit As String = String.Empty
        Dim BusinessUnitURLDeviceEntity As DEBusinessUnitURLDevice = TryToGetCachedBusinessUnit(url)
        '----------------------------
        ' If not in cache get from DB
        '-----------------------------
        If BusinessUnitURLDeviceEntity Is Nothing OrElse String.IsNullOrEmpty(BusinessUnitURLDeviceEntity.BusinessUnit) Then
            BusinessUnitURLDeviceEntity = GetBusinessUnitFromDB(url)
            cachedBusinessUnit = BusinessUnitURLDeviceEntity.BusinessUnit
        Else
            cachedBusinessUnit = BusinessUnitURLDeviceEntity.BusinessUnit
        End If

        If ConfigurationManager.AppSettings("OverwriteBU") IsNot Nothing Then
            cachedBusinessUnit = ConfigurationManager.AppSettings("OverwriteBU").ToString()
        End If

        Return cachedBusinessUnit
    End Function

    Public Shared Function GetBusinessUnitGroup() As String
        Dim url() As String
        'Get the url and split it on the "/" char
        url = HttpContext.Current.Request.Url.ToString.Split("/")

        ' Check for localhost:port (debugging locally) and treat as localhost
        Dim j As Integer = 0
        If HttpContext.Current.Request.Url.ToString.Contains("localhost:") Then
            Do While j < url.Length
                If url(j).Contains("localhost:") Then
                    url(j) = "localhost"
                    Exit Do
                End If
                j += 1
            Loop
        End If

        Dim BusinessUnitURLDeviceEntity As DEBusinessUnitURLDevice
        Dim cachedBusinessUnitGroup As String = String.Empty
        BusinessUnitURLDeviceEntity = TryToGetCachedBusinessUnit(url)
        '---------------------------------
        ' If not in cache then get from DB
        '---------------------------------
        If BusinessUnitURLDeviceEntity Is Nothing OrElse (String.IsNullOrEmpty(BusinessUnitURLDeviceEntity.BusinessUnitGroup) AndAlso String.IsNullOrEmpty(BusinessUnitURLDeviceEntity.BusinessUnit)) Then
            BusinessUnitURLDeviceEntity = GetBusinessUnitFromDB(url)
        End If
        '-----------------------------------------
        ' If Group is empty then use Business Unit
        '-----------------------------------------
        If String.IsNullOrEmpty(BusinessUnitURLDeviceEntity.BusinessUnitGroup) Then
            cachedBusinessUnitGroup = BusinessUnitURLDeviceEntity.BusinessUnit
        Else
            cachedBusinessUnitGroup = BusinessUnitURLDeviceEntity.BusinessUnitGroup
        End If
        Return cachedBusinessUnitGroup

    End Function

    Protected Shared Function TryToGetCachedBusinessUnit(ByVal url() As String) As DEBusinessUnitURLDevice
        'Loop backwards through the length of the array
        Dim businessUnitURLDeviceEntity As DEBusinessUnitURLDevice = Nothing
        For i As Integer = url.Length - 1 To 0 Step -1
            Dim cacheString As String = GetNextURLString(url, i)
            If HttpContext.Current.Cache("BU" & cacheString) Is Nothing Then
            Else
                Return CType(HttpContext.Current.Cache("BU" & cacheString), DEBusinessUnitURLDevice)
            End If
        Next
        Return businessUnitURLDeviceEntity
    End Function

    Protected Shared Function GetBusinessUnitFromDB(ByVal url() As String) As DEBusinessUnitURLDevice
        Dim BusinessUnitURLDeviceEntity As New DEBusinessUnitURLDevice
        Dim cacheString As String
        'Loop backwards through the length of the array
        For i As Integer = url.Length - 1 To 0 Step -1
            cacheString = GetNextURLString(url, i)
            Dim tDataObjects As New Talent.Common.TalentDataObjects
            tDataObjects.Settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString()
            tDataObjects.Settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
            Dim dt As System.Data.DataTable = tDataObjects.AppVariableSettings.TblUrlBu.GetBUByURL(cacheString)
            If dt.Rows.Count > 0 Then
                If Not Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultFalse(dt.Rows(0).Item("DISABLED")) Then
                    BusinessUnitURLDeviceEntity.BusinessUnit = dt.Rows(0).Item("BUSINESS_UNIT")
                    BusinessUnitURLDeviceEntity.BusinessUnitGroup = Utilities.CheckForDBNull_String(dt.Rows(0).Item("BU_GROUP"))
                    BusinessUnitURLDeviceEntity.DeviceType = Utilities.CheckForDBNull_String(dt.Rows(0).Item("DEVICE_TYPE"))
                    BusinessUnitURLDeviceEntity.URL = Utilities.CheckForDBNull_String(dt.Rows(0).Item("URL"))
                    BusinessUnitURLDeviceEntity.URLGroup = Utilities.CheckForDBNull_String(dt.Rows(0).Item("URL_GROUP"))
                    'todo call ecom module defaults data object for the given bu to get queue url
                    AddPropertyToCache("BU" & cacheString, BusinessUnitURLDeviceEntity, 30, TimeSpan.Zero, CacheItemPriority.Normal)
                    Return BusinessUnitURLDeviceEntity
                End If
            End If
        Next
        Return BusinessUnitURLDeviceEntity
    End Function

    Protected Shared Function GetNextURLString(ByVal url() As String, ByVal i As Integer) As String
        Dim cacheString As String = String.Empty
        'Contruct the url string
        cacheString = String.Empty
        For j As Integer = 0 To i
            If url(j) = "http:" OrElse url(j) = "https:" OrElse url(j) = "" Then
            Else
                cacheString += url(j) & "/"
            End If
        Next

        'Remove the end "/"
        If cacheString.EndsWith("/") Then
            cacheString = cacheString.TrimEnd("/")
        End If

        '' if it's localhost and a specified port (debugging) then remove port
        'If cacheString.Contains("localhost:") And i = 2 Then
        '    cacheString = "localhost"
        'End If
        Return cacheString
    End Function
    '------------------------------------------------------------
    ' Return all business units which are available for selection
    '------------------------------------------------------------
    Public Shared Function GetAvailableBusinessUnits() As Data.DataTable
        Dim dt As New Data.DataTable
        Dim bu As String = GetBusinessUnit()
        Dim partner As String = GetPartner(HttpContext.Current.Profile)
        Dim cacheKey As String = "AVAILABLE_BUSINESS_UNITS"

        If Not GetPropertyFromCache(cacheKey) Is Nothing Then
            dt = CType(GetPropertyFromCache(cacheKey), Data.DataTable)
        Else
            Dim BUs As New TalentApplicationVariablesTableAdapters.tbl_language_buTableAdapter 
            dt = BUs.GetUniqueBUWhereAllowSelectionTrue
            AddPropertyToCache(cacheKey, dt, 30, TimeSpan.Zero, CacheItemPriority.Low)
        End If

        Return dt
    End Function
    '-----------------------------------------------------------------
    ' Return all languages which are which are available for selection
    '-----------------------------------------------------------------
    Public Shared Function GetAvailableLanguagesForBU(ByVal bu As String) As Data.DataTable
        Dim dt As New Data.DataTable
        Dim dtLanguages As New Data.DataTable("LANGUAGES")
        dtLanguages.Columns.Add("LANGUAGE_DESC")
        dtLanguages.Columns.Add("LANGUAGE")
        Dim drNew As Data.DataRow

        Dim cacheKey As String = "AVAILABLE_LANGUAGE_BU"

        If Not GetPropertyFromCache(cacheKey) Is Nothing Then
            dt = CType(GetPropertyFromCache(cacheKey), Data.DataTable)
        Else
            Dim BUs As New TalentApplicationVariablesTableAdapters.tbl_language_buTableAdapter 
            dt = BUs.GetAllAvailableLanguageBUs
            AddPropertyToCache(cacheKey, dt, 30, TimeSpan.Zero, CacheItemPriority.Low)
        End If
        '-------------
        ' Filter by BU
        '-------------
        Dim drc As Data.DataRow() = dt.Select("BUSINESS_UNIT = '" & bu.Trim & "'", "LANGUAGE_DESCRIPTION ASC")
        For Each dr As Data.DataRow In drc
            drNew = dtLanguages.NewRow()
            drNew("LANGUAGE_DESC") = dr("LANGUAGE_DESCRIPTION")
            drNew("LANGUAGE") = dr("LANGUAGE")
            dtLanguages.Rows.Add(drNew)
        Next

        Return dtLanguages
    End Function

    ''' <summary>
    ''' Returns a data table of promotions based on the values provided
    ''' </summary>
    ''' <param name="activationMechanism">AUTO or CODE</param>
    ''' <param name="promotionCode">Optional: the specific promotion code to search for</param>
    ''' <returns>DataTable of promotions</returns>
    ''' <remarks></remarks>
    Public Shared Function GetPromotions(ByVal activationMechanism As String, Optional ByVal promotionCode As String = "") As DataTable
        Dim dt As New DataTable
        Dim cacheKey As String = TalentCache.GetBusinessUnit & "_" & TalentCache.GetPartner(HttpContext.Current.Profile) & "_PromotionsTable"

        If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey)  Then
            dt = CType(HttpContext.Current.Cache.Item(cacheKey), DataTable)
        Else
            Dim cmd As New SqlCommand("", New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
            Dim SelectStr As String = " SELECT * FROM tbl_promotions WITH (NOLOCK)  " & _
                                      " WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                      " AND PARTNER = @PARTNER " & _
                                      " AND START_DATE <= @CURRENT_DATE " & _
                                      " AND END_DATE >= @CURRENT_DATE " & _
                                      " AND REDEEM_COUNT <= REDEEM_MAX " & _
                                      " AND ACTIVATION_MECHANISM = @ACTIVATION_MECHANISM " & _
                                      " AND ACTIVE = 'True' "

            'If a promo code has been supplied, include it in the statement
            If Not String.IsNullOrEmpty(promotionCode) Then
                SelectStr += " AND PROMOTION_CODE = @PROMOTION_CODE "
            End If
            SelectStr += " ORDER BY PRIORITY_SEQUENCE ASC "

            cmd.CommandText = SelectStr

            With cmd.Parameters
                .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(HttpContext.Current.Profile)
                .Add("@CURRENT_DATE", SqlDbType.DateTime).Value = Now
                .Add("@ACTIVATION_MECHANISM", SqlDbType.NVarChar).Value = UCase(activationMechanism)
                If Not String.IsNullOrEmpty(promotionCode) Then
                    .Add("@PROMOTION_CODE", SqlDbType.NVarChar).Value = promotionCode
                End If
            End With

            Try
                cmd.Connection.Open()

                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)

                If dt.Rows.Count = 0 Then
                    cmd.Parameters("@PARTNER").Value = Talent.Common.Utilities.GetAllString
                    da.Fill(dt)
                    If dt.Rows.Count = 0 Then
                        cmd.Parameters("@BUSINESS_UNIT").Value = Talent.Common.Utilities.GetAllString
                        da.Fill(dt)
                    End If
                End If

                da.Dispose()

                AddPropertyToCache(cacheKey, dt, 30, TimeSpan.Zero, CacheItemPriority.Low)

            Catch ex As Exception
            Finally
                cmd.Connection.Close()
            End Try
        End If

        Return dt
    End Function

    Public Shared Function GetDefaultCountryForBU() As String
        Dim defaultCountry As String = String.Empty

        Dim cacheString As String = "DEFAULTCOUNTRY" & TalentCache.GetBusinessUnit
        If Not Talent.Common.TalentThreadSafe.ItemIsInCache(cacheString) Then
            ' Get from DB
            Dim countries As New TalentApplicationVariablesTableAdapters.tbl_country_buTableAdapter
            Dim dt As Data.DataTable = countries.GetDefaultCountryByBU(TalentCache.GetBusinessUnit)
            If dt.Rows.Count > 0 Then
                defaultCountry = dt.Rows(0)("COUNTRY_CODE").ToString
            End If
            AddPropertyToCache(cacheString, defaultCountry, 30, TimeSpan.Zero, CacheItemPriority.Normal)
        Else
            defaultCountry = HttpContext.Current.Cache(cacheString).ToString
        End If

        Return defaultCountry

    End Function

    Public Shared Function GetBusinessUnitDeviceType() As String

        Dim url() As String
        'Get the url and split it on the "/" char
        url = HttpContext.Current.Request.Url.ToString.Split("/")

        ' Check for localhost:port (debugging locally) and treat as localhost
        Dim j As Integer = 0
        If HttpContext.Current.Request.Url.ToString.Contains("localhost:") Then
            Do While j < url.Length
                If url(j).Contains("localhost:") Then
                    url(j) = "localhost"
                    Exit Do
                End If
                j += 1
            Loop
        End If
        '-------------------------
        ' Try to get BU from cache
        '-------------------------
        Dim cachedBusinessUnitDeviceType As String = String.Empty
        Dim BusinessUnitURLDeviceEntity As DEBusinessUnitURLDevice = TryToGetCachedBusinessUnit(url)
        '----------------------------
        ' If not in cache get from DB
        '-----------------------------
        If BusinessUnitURLDeviceEntity Is Nothing OrElse String.IsNullOrEmpty(BusinessUnitURLDeviceEntity.DeviceType) Then
            BusinessUnitURLDeviceEntity = GetBusinessUnitFromDB(url)
            cachedBusinessUnitDeviceType = BusinessUnitURLDeviceEntity.DeviceType
        Else
            cachedBusinessUnitDeviceType = BusinessUnitURLDeviceEntity.DeviceType
        End If

        Return cachedBusinessUnitDeviceType
    End Function

    Public Shared Function GetBusinessUnitURLGroup() As String

        Dim url() As String
        'Get the url and split it on the "/" char
        url = HttpContext.Current.Request.Url.ToString.Split("/")

        ' Check for localhost:port (debugging locally) and treat as localhost
        Dim j As Integer = 0
        If HttpContext.Current.Request.Url.ToString.Contains("localhost:") Then
            Do While j < url.Length
                If url(j).Contains("localhost:") Then
                    url(j) = "localhost"
                    Exit Do
                End If
                j += 1
            Loop
        End If
        '-------------------------
        ' Try to get BU from cache
        '-------------------------
        Dim cachedBusinessUnitUrlGroup As String = String.Empty
        Dim BusinessUnitURLDeviceEntity As DEBusinessUnitURLDevice = TryToGetCachedBusinessUnit(url)
        '----------------------------
        ' If not in cache get from DB
        '-----------------------------
        If BusinessUnitURLDeviceEntity Is Nothing OrElse String.IsNullOrEmpty(BusinessUnitURLDeviceEntity.URLGroup) Then
            BusinessUnitURLDeviceEntity = GetBusinessUnitFromDB(url)
            cachedBusinessUnitUrlGroup = BusinessUnitURLDeviceEntity.URLGroup
        Else
            cachedBusinessUnitUrlGroup = BusinessUnitURLDeviceEntity.URLGroup
        End If

        Return cachedBusinessUnitUrlGroup
    End Function

End Class
