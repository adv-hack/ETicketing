Imports Talent.Common
Imports Talent.eCommerce
Imports System.Data
Imports System.Linq
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class UserControls_ProductHotList
    Inherits ControlBase

#Region "Private Variables"

    Private _ucr As UserControlResource = Nothing
    Private _businessUnit As String = Nothing
    Private _partnerCode As String = Nothing
    Private _languageCode As String = Nothing
    Private _dtProductList As DataTable = Nothing
    Private _dtStadiums As DataTable = Nothing

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _ucr = New UserControlResource
        _businessUnit = TalentCache.GetBusinessUnit()
        _partnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage()
        With _ucr
            .BusinessUnit = _businessUnit
            .PageCode = TEBUtilities.GetCurrentPageName()
            .PartnerCode = _partnerCode
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "ProductHotList.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("ShowProductHotList")) Then
            _dtStadiums = TDataObjects.StadiumSettings.TblStadiums.GetAllStadiums(_businessUnit)
            populateHTMLList()
        Else
            plhProductHotList.Visible = False
        End If
    End Sub

    Protected Sub rptProductHotList_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptProductHotList.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then
            Dim hplVisualSeatSelection As HyperLink = CType(e.Item.FindControl("hplVisualSeatSelection"), HyperLink)
            Dim hvfProductStadium As HiddenField = CType(e.Item.FindControl("hvfProductStadium"), HiddenField)
            Dim hvfCampaignCode As HiddenField = CType(e.Item.FindControl("hvfCampaignCode"), HiddenField)
            Dim hvfProductCode As HiddenField = CType(e.Item.FindControl("hvfProductCode"), HiddenField)
            Dim hvfProductHomeAsAway As HiddenField = CType(e.Item.FindControl("hvfProductHomeAsAway"), HiddenField)
            Dim hvfProductSubType As HiddenField = CType(e.Item.FindControl("hvfProductSubType"), HiddenField)
            Dim hvfProductDate As HiddenField = CType(e.Item.FindControl("hvfProductDate"), HiddenField)
            Dim hvfProductType As HiddenField = CType(e.Item.FindControl("hvfProductType"), HiddenField)
            hplVisualSeatSelection.NavigateUrl = setVisualSeatSelectionUrl(doesStadiumExistInStadiums(hvfProductStadium.Value.Trim), hvfProductStadium.Value.Trim, hvfProductCode.Value.Trim, hvfCampaignCode.Value.Trim, hvfProductType.Value.Trim, hvfProductSubType.Value.Trim, hvfProductHomeAsAway.Value.Trim)
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub populateHTMLList()
        Try
            _dtProductList = GetXmlFeed()
            Dim dvProductList As New DataView(_dtProductList)
            dvProductList.RowFilter = "(ProductType ='" & TEBUtilities.CheckForDBNull_String(_ucr.Attribute("ProductType1")) & "' AND ProductSubType= '" & TEBUtilities.CheckForDBNull_String(_ucr.Attribute("ProductSubType1")) & "') OR (ProductType ='" & TEBUtilities.CheckForDBNull_String(_ucr.Attribute("ProductType2")) & "' AND ProductSubType= '" & TEBUtilities.CheckForDBNull_String(_ucr.Attribute("ProductSubType2")) & "' )"
            dvProductList.Sort = "ProductDateYear ASC"
            Dim rows = dvProductList.ToTable().Rows.Cast(Of DataRow)().Take(TEBUtilities.CheckForDBNull_String(_ucr.Attribute("NumberOfProductsListed")))
            Dim dtPresentedProductList As DataTable = _dtProductList.Clone()
            For Each row In rows
                dtPresentedProductList.ImportRow(row)
            Next
            If dtPresentedProductList.Rows.Count > 0 Then
                plhProductHotList.Visible = True
                rptProductHotList.DataSource = dtPresentedProductList
                rptProductHotList.DataBind()
            Else
                plhProductHotList.Visible = False
            End If
        Catch ex As Exception
            plhProductHotList.Visible = False
        End Try
    End Sub

#End Region

#Region "Private Functions"

    Private Function GetXmlFeed() As DataTable
        Dim settings As DESettings = TEBUtilities.GetSettingsObject()
        Dim feeds As New TalentFeeds
        Dim deFeeds As New DEFeeds
        Dim err As New ErrorObj
        deFeeds.Corporate_Stadium = ModuleDefaults.CorporateStadium
        deFeeds.Ticketing_Stadium = ModuleDefaults.TicketingStadium
        deFeeds.Product_Type = "ALL"
        feeds.FeedsEntity = deFeeds
        settings.Cacheing = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("Caching"))
        settings.CacheTimeMinutes = TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("CacheTimeInMins"))
        feeds.Settings = settings
        err = feeds.GetXMLFeed
        If err.HasError Then
            Return New DataTable
        Else
            If feeds.ProductsDataView IsNot Nothing Then
                Return feeds.ProductsDataView.ToTable
            Else
                Return New DataTable
            End If
        End If
    End Function

    Private Function setVisualSeatSelectionUrl(ByRef stadiumName As Boolean, ByVal stadiumCode As String, ByVal productCode As String, ByVal campaignCode As String, _
                                               ByVal productType As String, ByVal productSubType As String, ByVal productHomeAsAway As String) As String
        Dim redirectUrl As String = String.Empty
        If productType = "H" Or productType = "S" Then
            If stadiumName Then
                redirectUrl = "~/PagesPublic/ProductBrowse/VisualSeatSelection.aspx?stadium={0}&product={1}&campaign={2}&type={3}&productsubtype={4}&productIsHomeAsAway={5}"
            Else
                redirectUrl = "~/PagesPublic/ProductBrowse/StandAndAreaSelection.aspx?stadium={0}&product={1}&campaign={2}&type={3}&productsubtype={4}&productIsHomeAsAway={5}"
            End If
            redirectUrl = String.Format(redirectUrl, stadiumCode, productCode, campaignCode, productType, productSubType, productHomeAsAway)
        ElseIf productType = "C" Then
            redirectUrl = "~/pagespublic/ProductBrowse/ProductMembership.aspx?ProductSubType={0}&IsSingleProduct=TRUE&ProductType={1}&ProductCode={2}"
            redirectUrl = String.Format(redirectUrl, productSubType, productType, productCode)
        ElseIf productType = "A" Then
            redirectUrl = "~/pagespublic/ProductBrowse/ProductAway.aspx?ProductSubType={0}&IsSingleProduct=TRUE&ProductType={1}&ProductCode={2}"
            redirectUrl = String.Format(redirectUrl, productSubType, productType, productCode)
        ElseIf productType = "T" Then
            redirectUrl = "~/pagespublic/ProductBrowse/ProductTravel.aspx?ProductSubType={0}&IsSingleProduct=TRUE&ProductType={1}&ProductCode={2}"
            redirectUrl = String.Format(redirectUrl, productSubType, productType, productCode)
        ElseIf productType = "E" Then
            redirectUrl = "~/pagespublic/ProductBrowse/ProductEvent.aspx?ProductSubType={0}&IsSingleProduct=TRUE&ProductType={1}&ProductCode={2}"
            redirectUrl = String.Format(redirectUrl, productSubType, productType, productCode)
        End If
        Return redirectUrl
    End Function

    Private Function doesStadiumExistInStadiums(ByVal stadiumCode As String) As Boolean
        For Each stadium As DataRow In _dtStadiums.Rows
            If stadium("STADIUM_CODE") = stadiumCode Then
                Return True
            End If
        Next
        Return False
    End Function

#End Region

#Region "Public Functions"

    Protected Function GetImageURL(ByVal p1Value As String, ByVal productcode As String) As String
        Dim str As String = ImagePath.getImagePath(p1Value, productcode, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
        Return str
    End Function

    ''' <summary>
    ''' Get the descriptive name for the product type
    ''' </summary>
    ''' <param name="productType">The product type</param>
    ''' <returns>The descriptive product type</returns>
    ''' <remarks></remarks>
    Protected Function GetProductTypeDescription(ByVal productType As String) As String
        Dim productTypeDescription As String = productType
        If (_ucr.Content("ProductTypeDescription-" & productType, _languageCode, True)).Length > 0 Then
            productTypeDescription = _ucr.Content("ProductTypeDescription-" & productType, _languageCode, True)
        End If
        Return productTypeDescription
    End Function

#End Region


End Class
