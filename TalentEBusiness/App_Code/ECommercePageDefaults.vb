Imports Microsoft.VisualBasic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    ECommerce Page Defaults
'
'       Date                         
'
'       Author                       
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
    Public Class ECommercePageDefaults

        Public Class DefaultValues

            Private _bct_arent As String
            Private _bctUrl As String
            Private _BU As String
            Private _desc As String
            Private _html As Boolean
            Private _page As String
            Private _pageType As String
            Private _partner As String
            Private _qs As String
            Private _secure As Boolean
            Private _showPageHeader As Boolean
            Private _forceLogin As Boolean
            Private _allowGenericSales As Boolean
            Private _restrictingAlertName As String

            Public Property Bct_Parent() As String
                Get
                    Return _bct_arent
                End Get
                Set(ByVal value As String)
                    _bct_arent = value
                End Set
            End Property
            Public Property Bct_Url() As String
                Get
                    Return _bctUrl
                End Get
                Set(ByVal value As String)
                    _bctUrl = value
                End Set
            End Property
            Public Property Business_Unit() As String
                Get
                    Return _BU
                End Get
                Set(ByVal value As String)
                    _BU = value
                End Set
            End Property
            Public Property Description() As String
                Get
                    Return _desc
                End Get
                Set(ByVal value As String)
                    _desc = value
                End Set
            End Property
            Public Property Partner_Code() As String
                Get
                    Return _partner
                End Get
                Set(ByVal value As String)
                    _partner = value
                End Set
            End Property
            Public Property Html_In_Use() As Boolean
                Get
                    Return _html
                End Get
                Set(ByVal value As Boolean)
                    _html = value
                End Set
            End Property
            Public Property Page_Code() As String
                Get
                    Return _page
                End Get
                Set(ByVal value As String)
                    _page = value
                End Set
            End Property
            Public Property Page_QueryString() As String
                Get
                    Return _qs
                End Get
                Set(ByVal value As String)
                    _qs = value
                End Set
            End Property
            Public Property Page_Type() As String
                Get
                    Return _pageType
                End Get
                Set(ByVal value As String)
                    _pageType = value
                End Set
            End Property
            Public Property Show_Page_Header() As Boolean
                Get
                    Return _showPageHeader
                End Get
                Set(ByVal value As Boolean)
                    _showPageHeader = value
                End Set
            End Property
            Public Property Use_Secure_Url() As Boolean
                Get
                    Return _secure
                End Get
                Set(ByVal value As Boolean)
                    _secure = value
                End Set
            End Property
            Public Property Force_Login() As Boolean
                Get
                    Return _forceLogin
                End Get
                Set(ByVal value As Boolean)
                    _forceLogin = value
                End Set
            End Property
            Public Property Allow_Generic_Sales() As Boolean
                Get
                    Return _allowGenericSales
                End Get
                Set(ByVal value As Boolean)
                    _allowGenericSales = value
                End Set
            End Property

            Public Property RESTRICTING_ALERT_NAME() As String
                Get
                    Return _restrictingAlertName
                End Get
                Set(ByVal value As String)
                    _restrictingAlertName = value
                End Set
            End Property
        End Class

        Public Function GetDefaults() As DefaultValues
            ' Declare this first! Used for Logging function duration
            Dim timeSpan As TimeSpan = Now.TimeOfDay

            Dim defaults As New DefaultValues

            '  Dim pagename As String = HttpContext.Current.Request.Url.AbsolutePath
            ' pagename = pagename.Split("/")(pagename.Split("/").Length - 1)
            Dim pagename As String = Talent.eCommerce.Utilities.GetCurrentPageName().ToLower

            Dim cachename As String = GetCacheKey(pagename)
            If Not Talent.Common.TalentThreadSafe.ItemIsInCache(cachename) Then

                Dim pageDefaults As New TalentApplicationVariablesTableAdapters.tbl_pageTableAdapter
                Dim dt As Data.DataTable

                Dim _partner As String = TalentCache.GetPartner(HttpContext.Current.Profile)

                If String.IsNullOrEmpty(_partner) Then
                    dt = pageDefaults.GetDataBy_BU_Partner_Page(TalentCache.GetBusinessUnit, Talent.Common.Utilities.GetAllString, Talent.Common.Utilities.GetAllString)
                Else
                    dt = pageDefaults.GetDataBy_BU_Partner_Page(TalentCache.GetBusinessUnit, _partner, pagename)
                    If Not dt.Rows.Count > 0 Then
                        dt = pageDefaults.GetDataBy_BU_Partner_Page(TalentCache.GetBusinessUnit, Talent.Common.Utilities.GetAllString, pagename)
                    End If
                End If

                If dt.Rows.Count > 0 Then
                    Dim al As ArrayList = ProfileHelper.GetPropertyNames(defaults)
                    With defaults
                        For i As Integer = 0 To al.Count - 1
                            CallByName(defaults, al(i), CallType.Set, ProfileHelper.CheckDBNull(dt.Rows(0)(al(i).ToString)))
                        Next
                    End With
                End If

                TalentCache.AddPropertyToCache(cachename, defaults, "30", timeSpan.Zero, CacheItemPriority.Normal)

            Else
                defaults = CType(HttpContext.Current.Cache(cachename), DefaultValues)
            End If

            Utilities.TalentLogging.LoadTestLog("ECommercePageDefaults.vb", "GetDefaults", timeSpan)
            Return defaults
        End Function

        Private Function GetCacheKey(ByVal pageName As String) As String
            Return "ECommercePageDefaults" & TalentCache.GetBusinessUnit & pageName
        End Function

        Public Sub RemovePageCache(ByVal pageName As String)
            Try
                pageName = pageName.ToLower
                HttpContext.Current.Cache.Remove(GetCacheKey(pageName))
            Catch ex As Exception

            End Try
        End Sub
    End Class
End Namespace

