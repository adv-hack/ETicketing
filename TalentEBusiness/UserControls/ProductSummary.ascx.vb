Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities

Partial Class UserControls_ProductSummary
    Inherits System.Web.UI.UserControl

#Region "Class Level Fields"

    Private _productType As String = String.Empty
    Private _productSubType As String = String.Empty
    Private _productCode As String = String.Empty
    Private _includeFolderName As String = String.Empty
    Private _ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = String.Empty
    Private _businessUnit As String = String.Empty
    Private _partner As String = String.Empty
    Private _currentPage As String = String.Empty
    Private _isProductHtmlIncluded As Boolean = False
    Private def As ECommerceModuleDefaults.DefaultValues

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        _businessUnit = TalentCache.GetBusinessUnit
        _partner = TalentCache.GetPartner(HttpContext.Current.Profile)
        _currentPage = Talent.eCommerce.Utilities.GetCurrentPageName()
        With _ucr
            .BusinessUnit = _businessUnit
            .PageCode = _currentPage
            .PartnerCode = _partner
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "TicketingProductSummary.ascx"
        End With
        If IsQueryStringExists() Then
            plhProductSummaryError.Visible = False
            plhProductSummary.Visible = True
        Else
            plhProductSummary.Visible = False
            plhProductSummaryError.Visible = True
            ltlProductSummaryError.Text = _ucr.Content("QueryStringMissingError", _languageCode, True)
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            If plhProductSummary.Visible Then
                Dim moduleDefaults As New ECommerceModuleDefaults
                def = moduleDefaults.GetDefaults()
                LoadProductImage()
                LoadProductHtmlInclude()
            End If
            If Not Request.UrlReferrer Is Nothing Then
                hdfReturnUrl.Value = Request.UrlReferrer.PathAndQuery
            Else
                hdfReturnUrl.Value = ""
            End If

        End If
        btnPageReturn.Text = _ucr.Content("PageReturnButtonText", _languageCode, True)
    End Sub

    Protected Sub btnPageReturn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPageReturn.Click
        If hdfReturnUrl.Value = "" Then
            Response.Redirect(Utilities.GetSiteHomePage())
        Else
            Response.Redirect(hdfReturnUrl.Value)
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Function IsQueryStringExists() As Boolean
        Dim isExists As Boolean = False
        If Not (String.IsNullOrEmpty(Request.QueryString("ProductType"))) _
        AndAlso Not (String.IsNullOrEmpty(Request.QueryString("ProductCode"))) _
        AndAlso Not (String.IsNullOrEmpty(Request.QueryString("IncludeFolderName"))) Then
            _productType = Request.QueryString("ProductType").Trim()
            _productCode = Request.QueryString("ProductCode").Trim()
            _includeFolderName = Request.QueryString("IncludeFolderName").Trim()
            If Not String.IsNullOrEmpty(Request.QueryString("ProductSubType")) Then _productSubType = Request.QueryString("ProductSubType").Trim()
            isExists = True
            Dim masterBodyTagControl As HtmlGenericControl = CType(Me.Page.Master.FindControl("MasterBodyTag"), HtmlGenericControl)
            If masterBodyTagControl IsNot Nothing Then
                masterBodyTagControl.Attributes.Add("class", masterBodyTagControl.Attributes.Item("class") & " " & _productType & " " & _productCode & " modal")
            End If
        End If
        Return isExists
    End Function

    Private Sub LoadProductImage()
        Dim productImagePath As String = String.Empty
        Dim productImageName As String = _productType & _productCode
        productImagePath = ImagePath.getImagePath("PRODCORPORATE", productImageName, _businessUnit, _partner)
        If productImagePath.Contains(def.MissingImagePath) Then
            productImagePath = String.Empty
        End If
        If Not (productImagePath.Trim.Length > 0) Then
            productImageName = _productType
            productImagePath = ImagePath.getImagePath("PRODCORPORATE", productImageName, _businessUnit, _partner)
        End If
        If productImagePath.Contains(def.MissingImagePath) Then
            productImagePath = String.Empty
        End If
        If Not (productImagePath.Trim.Length > 0) Then
            plhProductSummaryImage.Visible = False
        Else
            plhProductSummaryImage.Visible = True
            imgProductSummary.ImageUrl = productImagePath
        End If
    End Sub

    Private Sub LoadProductHtmlInclude()
        Dim htmlPath As String = "Product/" & _includeFolderName
        Dim productHtmlContent As String = String.Empty
        Dim productHtmlName As String = _productCode
        'Get htm file
        If Not _isProductHtmlIncluded Then
            IncludeProductHtml(_businessUnit & "\" & _partner & "\" & htmlPath & "\" & productHtmlName & ".htm")
        End If
        If Not _isProductHtmlIncluded Then
            IncludeProductHtml(_businessUnit & "\" & _partner & "\" & htmlPath & "\" & productHtmlName & ".html")
        End If
        If Not _isProductHtmlIncluded Then
            productHtmlName = _productType
            IncludeProductHtml(_businessUnit & "\" & _partner & "\" & htmlPath & "\" & productHtmlName & ".htm")
        End If
        If Not _isProductHtmlIncluded Then
            IncludeProductHtml(_businessUnit & "\" & _partner & "\" & htmlPath & "\" & productHtmlName & ".html")
        End If
        If Not _isProductHtmlIncluded Then
            productHtmlName = _productSubType
            IncludeProductHtml(_businessUnit & "\" & _partner & "\" & htmlPath & "\" & productHtmlName & ".htm")
        End If
        If Not _isProductHtmlIncluded Then
            IncludeProductHtml(_businessUnit & "\" & _partner & "\" & htmlPath & "\" & productHtmlName & ".html")
        End If
        If _isProductHtmlIncluded Then
            plhProductSummaryHtml.Visible = True
        Else
            plhProductSummaryHtml.Visible = False
        End If
    End Sub

    Private Sub IncludeProductHtml(ByVal htmlPath As String)
        Dim productHtmlContent As String = String.Empty
        productHtmlContent = GetHtmlFromFile(htmlPath)
        If Not (String.IsNullOrEmpty(productHtmlContent)) Then
            ltlProductSummaryHtml.Text = productHtmlContent
            _isProductHtmlIncluded = True
        Else
            _isProductHtmlIncluded = False
        End If
    End Sub
#End Region

End Class

