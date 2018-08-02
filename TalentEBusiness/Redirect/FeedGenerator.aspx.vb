Imports Talent.Common
Imports Talent.eCommerce
Partial Class Redirect_FeedGenerator
    Inherits System.Web.UI.Page
    Private _feedType As String = String.Empty
    Private _productType As String = String.Empty
    Private _productSubType As String = String.Empty
    Private _productTypeAllFilters As String = String.Empty
    Private _moduleDefaults As ECommerceModuleDefaults = Nothing
    Private _defaultValues As ECommerceModuleDefaults.DefaultValues = Nothing
    Private Const FEED_TYPE_RSS As String = "RSS"
    Private Const FEED_TYPE_ATOM As String = "ATOM"
    Private Const PRODUCT_TYPE As String = "ERR"
    Private Const PRODUCT_SUB_TYPE As String = ""

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        AssignResponseHeaders()
        ValidateInputs()
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _moduleDefaults = New ECommerceModuleDefaults
        _defaultValues = _moduleDefaults.GetDefaults()
        Response.Write(GetXmlFeed())
    End Sub

    Private Sub AssignResponseHeaders()
        Response.ContentType = "text/xml"
    End Sub

    Private Sub ValidateInputs()
        _feedType = GetValueFromQuerystring("FeedType", FEED_TYPE_RSS)
        _productType = GetValueFromQuerystring("ProductType", PRODUCT_TYPE)
        _productSubType = GetValueFromQuerystring("ProductSubType", PRODUCT_SUB_TYPE).Trim.ToUpper
        _productTypeAllFilters = GetValueFromQuerystring("ProductTypeAllFilters", "")
    End Sub

    Private Function GetValueFromQuerystring(ByVal queryStringName As String, ByVal defaultValue As String) As String
        Dim tempQueryValue As String = String.Empty
        tempQueryValue = Request.QueryString(queryStringName)
        If String.IsNullOrWhiteSpace(tempQueryValue) Then
            tempQueryValue = defaultValue
        Else
            tempQueryValue = tempQueryValue.Trim
        End If
        If queryStringName = "FeedType" Then
            If tempQueryValue <> FEED_TYPE_RSS AndAlso tempQueryValue <> FEED_TYPE_ATOM Then
                tempQueryValue = FEED_TYPE_RSS
            End If
        End If
        Return tempQueryValue.ToUpper()
    End Function

    Private Function GetXmlFeed() As String
        Dim timeSpan As TimeSpan = Now.TimeOfDay
        Dim xmlFeed As String = String.Empty
        Try
            Dim feedsEntity As New DEFeeds
            feedsEntity.Feed_Type = _feedType
            feedsEntity.Product_Type = _productType
            feedsEntity.Product_Sub_Type = _productSubType
            feedsEntity.Product_Type_All_Filters = _productTypeAllFilters
            feedsEntity.Site_Url = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "")
            feedsEntity.Site_Url = ResolveUrl("~/")
            feedsEntity.Corporate_Stadium = _defaultValues.CorporateStadium
            feedsEntity.Ticketing_Stadium = _defaultValues.TicketingStadium
            Dim talFeeds As New TalentFeeds
            Dim errObj As New ErrorObj
            talFeeds.Settings = GetSettingsEntity()
            talFeeds.FeedsEntity = feedsEntity
            errObj = talFeeds.GetXMLFeed()
            If Not errObj.HasError Then
                xmlFeed = talFeeds.XMLFeed
            End If
        Catch ex As Exception
            'todo ck
        End Try
        Talent.eCommerce.Utilities.TalentLogging.LoadTestLog("Redirect_FeedGenerator.aspx.vb", "GetXmlFeed", timeSpan)
        Return xmlFeed
    End Function

    Private Function GetSettingsEntity() As DESettings
        Dim settingsEntity As New DESettings
        settingsEntity.BusinessUnit = TalentCache.GetBusinessUnit
        settingsEntity.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
        settingsEntity.DestinationDatabase = "SQL2005"
        settingsEntity.CacheDependencyPath = _defaultValues.CacheDependencyPath
        settingsEntity.OriginatingSourceCode = "W"
        settingsEntity.StoredProcedureGroup = _defaultValues.StoredProcedureGroup
        settingsEntity.Cacheing = True
        Return settingsEntity
    End Function


End Class
