Imports System.Data
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities


Partial Class UserControls_MatchDayHospitalitySummary
    Inherits System.Web.UI.UserControl

#Region "Class Level Fields"

    Private _packageID As String = String.Empty
    Private _packageCode As String = String.Empty
    Private _productCode As String = String.Empty
    Private _ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = String.Empty
    Private _businessUnit As String = String.Empty
    Private _partner As String = String.Empty
    Private _currentPage As String = String.Empty
    Private _isPackageHtmlIncluded As Boolean = False
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
            .KeyCode = "MatchDayHospitalitySummary.ascx"
        End With
        If IsQueryStringExists() Then
            plhMDHSummaryError.Visible = False
            plhMDHSummary.Visible = True
        Else
            plhMDHSummary.Visible = False
            plhMDHSummaryError.Visible = True
            lblMDHSummaryError.Text = _ucr.Content("QueryStringMissingError", _languageCode, True)
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim moduleDefaults As New ECommerceModuleDefaults
            def = moduleDefaults.GetDefaults()
            If plhMDHSummaryImage.Visible Then
                plhMDHSummaryHtml.Visible = False
                ViewFromAreaImgURL(Request.QueryString("PackageCode"))
            ElseIf plhMDHSummary.Visible Then
                plhMDHSummaryImage.Visible = False
                plhPackage.Visible = False
                LoadPackageHtmlInclude()
            End If
        End If
        btnPageReturn.Text = _ucr.Content("PageReturnButtonText", _languageCode, True)
    End Sub

    Protected Sub rptComponents_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptComponents.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim ltlCompQuanity As Literal = CType(e.Item.FindControl("ltlCompQuantity"), Literal), _
            ltlCompDescription As Literal = CType(e.Item.FindControl("ltlCompDescription"), Literal), _
            ltlCompComments As Literal = CType(e.Item.FindControl("ltlCompComments"), Literal)
            Dim componentRow As DataRow = CType(e.Item.DataItem, DataRow)

            If CInt(componentRow("NumberOfUnits").ToString.Trim()) > 0 Then
                ltlCompQuanity.Text = componentRow("NumberOfUnits").ToString.Replace("0", "")
                ltlCompQuanity.Text += "x&nbsp;"
            Else
                ltlCompQuanity.Visible = False
            End If
            ltlCompDescription.Text = componentRow("ComponentDescription")
            ltlCompComments.Text = componentRow("Comment1")
        End If
    End Sub

    Protected Sub btnPageReturn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPageReturn.Click
        Response.Redirect("~/PagesPublic/ProductBrowse/MatchDayHospitality.aspx")
    End Sub

#End Region

#Region "Private Methods"

    Private Function IsQueryStringExists() As Boolean
        Dim isExists As Boolean = False
        plhMDHSummaryImage.Visible = False
        plhMDHSummary.Visible = False
        If Not String.IsNullOrWhiteSpace(Request.QueryString("ViewArea")) AndAlso Not (String.IsNullOrWhiteSpace(Request.QueryString("PackageCode"))) Then
            plhMDHSummaryImage.Visible = True
            isExists = True
        ElseIf Not (String.IsNullOrEmpty(Request.QueryString("PackageID"))) _
            AndAlso Not (String.IsNullOrEmpty(Request.QueryString("PackageCode"))) _
            AndAlso Not (String.IsNullOrEmpty(Request.QueryString("ProductCode"))) Then
            _packageID = Request.QueryString("PackageID").Trim
            _productCode = Request.QueryString("ProductCode").Trim
            _packageCode = Request.QueryString("PackageCode").Trim
            isExists = True
        End If
        If isExists Then
            Dim masterBodyTagControl As HtmlGenericControl = CType(Me.Page.Master.FindControl("MasterBodyTag"), HtmlGenericControl)
            If masterBodyTagControl IsNot Nothing Then
                masterBodyTagControl.Attributes.Add("class", masterBodyTagControl.Attributes.Item("class") & " " & "CH" & " " & _productCode & " modal")
            End If
        End If
        Return isExists
    End Function

    Private Sub LoadPackageHtmlInclude()
        Dim htmlPath As String = "Product/Corporate"
        Dim packageHtmlContent As String = String.Empty
        Dim packageHtmlName As String = _packageCode
        'Get htm file
        If Not _isPackageHtmlIncluded Then
            IncludePackageHtml(_businessUnit & "/" & _partner & "/" & htmlPath & "/" & packageHtmlName & ".htm")
        End If
        If Not _isPackageHtmlIncluded Then
            IncludePackageHtml(_businessUnit & "/" & _partner & "/" & htmlPath & "/" & packageHtmlName & ".html")
        End If
        If _isPackageHtmlIncluded Then
            plhMDHSummaryHtml.Visible = True
        Else
            plhMDHSummaryHtml.Visible = False
        End If
    End Sub

    Private Sub IncludePackageHtml(ByVal htmlPath As String)
        Dim packageHtmlContent As String = String.Empty
        packageHtmlContent = GetHtmlFromFile(htmlPath)
        If Not (String.IsNullOrEmpty(packageHtmlContent)) Then
            ltlMDHSummaryHtml.Text = packageHtmlContent
            _isPackageHtmlIncluded = True
        Else
            _isPackageHtmlIncluded = False
        End If
    End Sub

    Private Sub ViewFromAreaImgURL(ByVal packageCode As String)
        Dim imgURL As String = String.Empty
        imgURL = ImagePath.getImagePath("PRODCORPORATE", packageCode, _businessUnit, _partner)
        If Not imgURL.Contains(def.MissingImagePath) Then
            plhMDHSummaryImage.Visible = True
            imgMDHSummary.ImageUrl = imgURL
        Else
            plhMDHSummaryImage.Visible = False
        End If

    End Sub

#Region "Removed functionalities to improve performance"
    ''' <summary>
    ''' Loads the package image.
    ''' </summary>
    Private Sub LoadPackageImage()
        Dim packageImagePath As String = String.Empty
        Dim packageImageName As String = _packageID & _productCode
        packageImagePath = ImagePath.getImagePath("PRODCORPORATE", packageImageName, _businessUnit, _partner)
        If packageImagePath.Contains(def.MissingImagePath) Then
            packageImagePath = String.Empty
        End If
        If Not (packageImagePath.Trim.Length > 0) Then
            packageImageName = _packageID
            packageImagePath = ImagePath.getImagePath("PRODCORPORATE", packageImageName, _businessUnit, _partner)
        End If
        If packageImagePath.Contains(def.MissingImagePath) Then
            packageImagePath = String.Empty
        End If
        If Not (packageImagePath.Trim.Length > 0) Then
            plhMDHSummaryImage.Visible = False
        Else
            plhMDHSummaryImage.Visible = True
            imgMDHSummary.ImageUrl = packageImagePath
        End If
    End Sub



    ''' <summary>
    ''' Loads the package detail.
    ''' </summary>
    Private Sub LoadPackageDetail()
        If Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("LoadBackEndPackageDetail")) Then
            ltlPackageHeader.Text = _ucr.Content("PackageHeaderText", _languageCode, True)
            ltlComponentHeader.Text = _ucr.Content("ComponentHeaderText", _languageCode, True)
            Dim product As New Talent.Common.TalentProduct
            Dim settings As New Talent.Common.DESettings
            Dim err As New Talent.Common.ErrorObj
            Dim dtPackageList As New DataTable
            Dim dtComponentList As New DataTable
            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settings.BusinessUnit = _businessUnit
            settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            settings.Cacheing = CType(_ucr.Attribute("Cacheing"), Boolean)
            settings.CacheTimeMinutes = CType(_ucr.Attribute("CacheTimeMinutes"), Integer)
            settings.CacheDependencyPath = def.CacheDependencyPath
            product.Settings() = settings
            product.De.ProductCode = _productCode
            product.De.Src = "W"
            err = product.ProductHospitality
            If (Not err.HasError) AndAlso (product.ResultDataSet IsNot Nothing) Then
                dtPackageList = product.ResultDataSet.Tables(1)
                dtComponentList = product.ResultDataSet.Tables(2)
            End If
            Dim drPackageList() As DataRow = dtPackageList.Select("PackageID='" & _packageID & "'")
            If drPackageList.Length > 0 Then
                plhPackage.Visible = True
                ltlPackageDescription.Text = drPackageList(0)("PackageDescription")
                ltlPackageComments.Text = drPackageList(0)("Comment1") & " " & drPackageList(0)("Comment2")
                Dim drComponentList() As DataRow = dtComponentList.Select("PackageID='" & _packageID & "'")
                If drComponentList.Length > 0 Then
                    ltlComponentHeader.Visible = True
                    rptComponents.Visible = True
                    rptComponents.DataSource = drComponentList
                    rptComponents.DataBind()
                Else
                    ltlComponentHeader.Visible = False
                    rptComponents.Visible = False
                End If
            Else
                plhPackage.Visible = False
            End If
        End If
    End Sub
#End Region

#End Region

End Class
