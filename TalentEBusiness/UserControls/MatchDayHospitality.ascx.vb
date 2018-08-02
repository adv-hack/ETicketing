Imports System.Data
Imports Talent.eCommerce
Imports Talent.Common
Imports EcomUtilities = Talent.eCommerce.Utilities
Imports CommonUtilities = Talent.Common.Utilities

Partial Class UserControls_MatchDayHospitality
    Inherits ControlBase

#Region "Class Level Fields"

    Private _productCode As String = String.Empty
    Private _businessUnit As String = String.Empty
    Private _partner As String = String.Empty
    Private _languageCode As String = String.Empty
    Private _currentPageName As String = String.Empty
    Private _dtComponentList As DataTable = Nothing
    Private _enabled As Boolean = False
    Private Const PKGSUMMARYLINK As String = "~/PagesPublic/ProductBrowse/MatchDayHospitalitySummary.aspx"
    Private _ucr As UserControlResource = Nothing
    Private def As ECommerceModuleDefaults.DefaultValues

#End Region

#Region "Properties"
    Public Property Enabled() As Boolean
        Get
            Return _enabled
        End Get
        Set(ByVal value As Boolean)
            _enabled = value
        End Set
    End Property

    Public Property ProductCode() As String
        Get
            Return _productCode
        End Get
        Set(ByVal value As String)
            _productCode = value
        End Set
    End Property

#End Region

#Region "Public Methods"

    Public Function LoadPackageDetails() As Boolean
        Dim product As New Talent.Common.TalentProduct
        Dim settings As New Talent.Common.DESettings
        Dim err As New Talent.Common.ErrorObj
        Dim dtPackageList As New DataTable
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.BusinessUnit = _businessUnit
        settings.StoredProcedureGroup = EcomUtilities.GetStoredProcedureGroup()
        settings.Cacheing = CType(_ucr.Attribute("Cacheing"), Boolean)
        settings.CacheTimeMinutes = CType(_ucr.Attribute("CacheTimeMinutes"), Integer)
        settings.CacheDependencyPath = ModuleDefaults.CacheDependencyPath
        product.Settings() = settings
        product.De.ProductCode = _productCode
        product.De.Src = "W"
        err = product.ProductHospitality
        If (Not err.HasError) AndAlso (product.ResultDataSet IsNot Nothing) Then
            If product.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred").ToString.Equals(GlobalConstants.ERRORFLAG) Then
                If product.ResultDataSet.Tables(0).Rows(0)("ReturnCode").ToString.Equals("NR") Then
                    Me.Visible = False
                    Return False
                Else
                    Me.Visible = False
                    Return False
                End If
            Else
                dtPackageList = product.ResultDataSet.Tables(1)
                _dtComponentList = product.ResultDataSet.Tables(2)
                rptMDHPackages.DataSource = dtPackageList
                rptMDHPackages.DataBind()
                Return True
            End If
        Else
            Me.Visible = False
            Return False
        End If
    End Function

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _ucr = New UserControlResource
        _languageCode = CommonUtilities.GetDefaultLanguage
        _currentPageName = EcomUtilities.GetCurrentPageName
        _businessUnit = TalentCache.GetBusinessUnit()
        _partner = TalentCache.GetPartner(HttpContext.Current.Profile)
        With _ucr
            .BusinessUnit = _businessUnit
            .PageCode = _currentPageName
            .PartnerCode = _partner
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "MatchDayHospitality.ascx"
        End With
        Dim moduleDefaults As New ECommerceModuleDefaults
    End Sub

    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        Select Case CType(sender, Literal).ID
            Case Is = "HospitalityHeaderLabel"
                CType(sender, Literal).Text = _ucr.Content("HospitalityHeaderLabel", _languageCode, True)
            Case Is = "PriceHeaderLabel"
                CType(sender, Literal).Text = _ucr.Content("PriceHeaderLabel", _languageCode, True)
            Case Is = "NetPriceHeaderLabel"
                CType(sender, Literal).Text = _ucr.Content("NetPriceHeaderLabel", _languageCode, True)
            Case Is = "QuantityHeaderLabel"
                CType(sender, Literal).Text = _ucr.Content("QuantityHeaderLabel", _languageCode, True)
            Case Is = "AddToBasketHeaderLabel"
                CType(sender, Literal).Text = _ucr.Content("AddToBasketHeaderLabel", _languageCode, True)
            Case Is = "ViewHeaderLabel"
                CType(sender, Literal).Text = _ucr.Content("ViewHeaderLabel", _languageCode, True)
        End Select
    End Sub

    ''' <summary>
    ''' Adds the package items.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    Protected Sub AddPackageItemsToBasket(ByVal sender As Object, ByVal e As EventArgs)
        Dim hfPackageID As New HiddenField
        Dim hfSeatComponentID As New HiddenField
        Dim hfProductCode As New HiddenField
        Dim ddlQuantity As New DropDownList
        Dim btnAddToBasket As New Button
        btnAddToBasket = CType(sender, Button)
        Dim tempControl As Control = Nothing
        Dim tempControlInChild As Control = Nothing
        Dim packageID As String = String.Empty
        Dim seatComponentID As String = String.Empty
        Dim productCode As String = String.Empty
        Dim quantity As String = String.Empty
        For Each tempControl In btnAddToBasket.Parent.Parent.Controls
            Select Case tempControl.ID
                Case Is = "hfPackageID"
                    hfPackageID = CType(tempControl, HiddenField)
                    packageID = hfPackageID.Value
                Case Is = "hfSeatComponentID"
                    hfSeatComponentID = CType(tempControl, HiddenField)
                    seatComponentID = hfSeatComponentID.Value
                Case Is = "hfProductCode"
                    hfProductCode = CType(tempControl, HiddenField)
                    productCode = hfProductCode.Value
                Case Is = "quantityCol"
                    For Each tempControlInChild In tempControl.Controls
                        Select Case tempControlInChild.ID
                            Case Is = "ddlQuantity"
                                ddlQuantity = CType(tempControlInChild, DropDownList)
                                quantity = ddlQuantity.SelectedValue
                        End Select
                    Next
            End Select
        Next

        If quantity <= 0 Then
            Dim errorlist As BulletedList = CType(Me.Parent.TemplateControl.FindControl("errorlist"), BulletedList)
            Dim errMessage As String = _ucr.Content("ZeroQuantityErrorText", _languageCode, True)
            Dim isErrNotExists As Boolean = True
            For Each errItem As ListItem In errorlist.Items
                If errItem.Value = errMessage Then
                    isErrNotExists = False
                    Exit For
                End If
            Next
            If isErrNotExists Then
                errorlist.Items.Add(errMessage)
            End If
        Else
            Response.Redirect("~/Redirect/TicketingGateway.aspx?page=" & _
                                    _currentPageName & _
                                    "&function=AddToBasket&product=" & productCode.Trim & _
                                    "&quantity=" & quantity.Trim & _
                                    "&packageID=" & packageID.Trim & _
                                    "&seatComponentID=" & seatComponentID.Trim & _
                                    "&type=" & "H")
        End If
    End Sub

    Protected Sub netPricePreRender(ByVal sender As Object, ByVal e As System.EventArgs)
        If Not (CommonUtilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("displayNetPrice"))) Then
            CType(sender, HtmlTableCell).Visible = False
        End If
    End Sub

    Protected Sub rptMDHPackages_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptMDHPackages.ItemDataBound
        If e.Item.ItemIndex = -1 Then
        ElseIf e.Item.ItemIndex > -1 Then
            Dim dr As DataRow = CType(e.Item.DataItem, DataRowView).Row
            Dim strQuantity As String = (CommonUtilities.CheckForDBNull_String(dr("MaxQuantity"))).Trim
            Dim hfProductCode As HiddenField = CType(e.Item.FindControl("hfProductCode"), HiddenField)
            Dim hfPackageID As HiddenField = CType(e.Item.FindControl("hfPackageID"), HiddenField)
            Dim hfPackageCode As HiddenField = CType(e.Item.FindControl("hfPackageCode"), HiddenField)
            Dim hfSeatComponentID As HiddenField = CType(e.Item.FindControl("hfSeatComponentID"), HiddenField)
            Dim hlkDescription As HyperLink = CType(e.Item.FindControl("hlkDescription"), HyperLink)
            Dim lblDescription As Label = CType(e.Item.FindControl("lblDescription"), Label)
            Dim ltlComments As Literal = CType(e.Item.FindControl("ltlComments"), Literal)
            Dim plhComments As PlaceHolder = CType(e.Item.FindControl("plhComments"), PlaceHolder)
            Dim ltlPrice As Literal = CType(e.Item.FindControl("ltlPrice"), Literal)
            Dim ltlNetPrice As Literal = CType(e.Item.FindControl("ltlNetPrice"), Literal)
            Dim colNetPrice As HtmlTableCell = CType(e.Item.FindControl("colNetPrice"), HtmlTableCell)
            Dim imgViewArea As Image = CType(e.Item.FindControl("imgViewArea"), Image)
            hfProductCode.Value = _productCode
            hfPackageID.Value = dr("PackageID")
            hfPackageCode.Value = dr("PackageCode")
            hfSeatComponentID.Value = GetSeatComponentID(dr("PackageID").ToString)
            hlkDescription.Text = dr("PackageDescription")
            lblDescription.Text = dr("PackageDescription")
            ltlComments.Text = dr("Comment1") & dr("Comment2")
            plhComments.Visible = (ltlComments.Text.Length > 0)
            ltlPrice.Text = CommonUtilities.FormatPrice(dr("Price"))

            'Determine the net price
            If Not (CommonUtilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("displayNetPrice"))) Then
                colNetPrice.Visible = False
            Else
                Dim NetPrice As Decimal = 0
                If CommonUtilities.CheckForDBNull_Decimal(dr("Price")) > 0 AndAlso CommonUtilities.CheckForDBNull_Decimal(dr("VatValue")) > 0 Then
                    NetPrice = CommonUtilities.CheckForDBNull_Decimal(dr("Price")) - CommonUtilities.CheckForDBNull_Decimal(dr("VatValue"))
                    If NetPrice < 0 Then
                        NetPrice = 0
                    End If
                Else
                    If CommonUtilities.CheckForDBNull_Decimal(dr("Price")) > 0 Then
                        NetPrice = CommonUtilities.CheckForDBNull_Decimal(dr("Price"))
                    End If
                End If
                ltlNetPrice.Text = CommonUtilities.FormatPrice(NetPrice.ToString)
            End If

            hlkDescription.Visible = False
            lblDescription.Visible = False

            Dim pdfLink As String = PDFLinkAvailable(hfPackageCode.Value)
            If Not String.IsNullOrEmpty(pdfLink) Then
                hlkDescription.Visible = True
                hlkDescription.NavigateUrl = pdfLink
                hlkDescription.Target = "_blank"
            ElseIf IsHtmlIncludeExists(hfPackageCode.Value) Then
                hlkDescription.Visible = True
                Dim summaryLink As String = PKGSUMMARYLINK & "?ProductCode=" & Server.UrlEncode(hfProductCode.Value) & "&PackageID=" & Server.UrlEncode(hfPackageID.Value) & "&PackageCode=" & Server.UrlEncode(hfPackageCode.Value)
                hlkDescription.NavigateUrl = summaryLink
                hlkDescription.Attributes.Add("data-open", "description-modal")
                hlkDescription.Attributes.Add("class", "true ebiz-open-modal")
                Dim plhDescription As PlaceHolder = CType(e.Item.FindControl("plhDescription"), PlaceHolder)
                Dim div As HtmlGenericControl = New HtmlGenericControl("div")
                div.Attributes.Add("id", "description-modal")
                div.Attributes.Add("class", "reveal ebiz-reveal-ajax")
                div.Attributes.Add("data-reveal", "")
                plhDescription.Controls.Add(div)
            Else
                lblDescription.Visible = True
            End If

            If Not (IsSoldOut(dr("Availability"), strQuantity, e)) Then
                Dim ddlQuantity As DropDownList = CType(e.Item.FindControl("ddlQuantity"), DropDownList)
                Dim btnAddToBasket As Button = CType(e.Item.FindControl("btnAddToBasket"), Button)
                Dim allowedQuantity As Integer = CInt(strQuantity)
                For quanCounter As Integer = 0 To allowedQuantity
                    ddlQuantity.Items.Add(New ListItem(quanCounter, quanCounter))
                Next
                btnAddToBasket.Text = _ucr.Content("AddToBasketButtonText", _languageCode, True)
            Else
                Dim ltlSoldOut As Literal = CType(e.Item.FindControl("ltlSoldOut"), Literal)
                ltlSoldOut.Text = _ucr.Content("PackageSoldOutText", _languageCode, True)
            End If

            Dim hlkView As HyperLink = CType(e.Item.FindControl("hlkView"), HyperLink)
            Dim viewImgURL As String = ViewFromAreaImgURL(hfPackageCode.Value)
            If String.IsNullOrEmpty(viewImgURL) Then
                hlkView.Visible = False
            Else
                hlkView.Text = _ucr.Content("ViewHyperLinkText", _languageCode, True)
                imgViewArea.ImageUrl = viewImgURL
                hlkView.Attributes.Add("data-open", "view-area-" & e.Item.ItemIndex)
            End If
        End If
    End Sub

    Protected Sub rptMDHPackages_PreRender(sender As Object, e As System.EventArgs) Handles rptMDHPackages.PreRender
        Dim hasViewFromArea As Boolean = False
        For Each item As RepeaterItem In rptMDHPackages.Items
            Dim hlkView As HyperLink = CType(item.FindControl("hlkView"), HyperLink)
            If hlkView IsNot Nothing AndAlso hlkView.Visible Then
                hasViewFromArea = True
                Exit For
            End If
        Next
        Dim hlkViewCol As HtmlTableCell = Nothing
        For Each item As RepeaterItem In rptMDHPackages.Items
            hlkViewCol = CType(item.FindControl("hlkViewCol"), HtmlTableCell)
            If hlkViewCol IsNot Nothing Then
                hlkViewCol.Visible = hasViewFromArea
            End If
        Next
        hlkViewCol = CType(rptMDHPackages.Controls(0).Controls(0).FindControl("hlkViewCol"), HtmlTableCell)
        If hlkViewCol IsNot Nothing Then
            hlkViewCol.Visible = hasViewFromArea
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Function PopulateQuantityDDL(ByVal maxQuantity As String) As ListItem()
        Dim allowedQuantity As Integer = 0
        Dim listQuantity() As ListItem = Nothing
        If IsNumeric(maxQuantity) Then
            allowedQuantity = CInt(maxQuantity.Trim)
        Else
            allowedQuantity = 0
        End If
        For quanCounter As Integer = 0 To allowedQuantity
            listQuantity(quanCounter) = New ListItem(quanCounter, quanCounter)
        Next
        Return listQuantity
    End Function

    Private Function IsSoldOut(ByVal availability As String, ByVal quantity As String, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) As Boolean
        Dim packageAvailability As Integer = 0
        Dim allowedQuantity As Integer = 0
        If IsNumeric(availability) Then
            packageAvailability = CInt(availability)
        Else
            packageAvailability = 0
        End If
        If IsNumeric(quantity) Then
            allowedQuantity = CInt(quantity)
        Else
            allowedQuantity = 0
        End If
        Dim quantityCol As HtmlTableCell = CType(e.Item.FindControl("quantityCol"), HtmlTableCell)
        Dim btnAddToBasketCol As HtmlTableCell = CType(e.Item.FindControl("btnAddToBasketCol"), HtmlTableCell)
        Dim soldOutCol As HtmlTableCell = CType(e.Item.FindControl("soldOutCol"), HtmlTableCell)
        soldOutCol.Visible = True
        If (packageAvailability > 0) AndAlso (allowedQuantity > 0) Then
            quantityCol.Visible = True
            btnAddToBasketCol.Visible = True
            soldOutCol.Visible = False
        Else
            quantityCol.Visible = False
            btnAddToBasketCol.Visible = False
            soldOutCol.Visible = True
        End If
        Return soldOutCol.Visible
    End Function

    Private Function GetSeatComponentID(ByVal packageID As String) As String
        Dim seatComponentID As String = String.Empty
        Dim drComponentList() As DataRow = _dtComponentList.Select("PackageID='" & packageID & "' AND IsSeatComponent='True'")
        If drComponentList.Length > 0 Then
            seatComponentID = drComponentList(0)("ComponentID")
        End If
        Return seatComponentID
    End Function

    Private Function PDFLinkAvailable(ByVal packageCode As String) As String
        Dim pdfUrl As String = String.Empty
        Dim htmlRootPathAbsolute As String = Talent.eCommerce.Utilities.GetHtmlIncludePathAbsolute
        pdfUrl = htmlRootPathAbsolute & "\" & _businessUnit & "\" & _ucr.PartnerCode & "\Product\Corporate\" & packageCode & ".pdf"
        If System.IO.File.Exists(pdfUrl) Then
            Dim htmlRootPathRelative As String = Talent.eCommerce.Utilities.GetHtmlIncludePathRelative
            pdfUrl = "~/Assets/" & htmlRootPathRelative & "/" & _businessUnit & "/" & _ucr.PartnerCode & "/Product/Corporate/" & packageCode & ".pdf"
        Else
            pdfUrl = String.Empty
        End If
        Return pdfUrl
    End Function

    Private Function IsHtmlIncludeExists(ByVal packageCode As String) As Boolean
        Dim htmlPath As String = "Product/Corporate"
        Dim packageHtmlContent As String = String.Empty
        Dim isHtmExists As Boolean = False
        If Talent.eCommerce.Utilities.DoesHtmlFileExists(_businessUnit & "/" & _partner & "/" & htmlPath & "/" & packageCode & ".htm") Then
            isHtmExists = True
        ElseIf Talent.eCommerce.Utilities.DoesHtmlFileExists(_businessUnit & "/" & _partner & "/" & htmlPath & "/" & packageCode & ".html") Then
            isHtmExists = True
        End If
        Return isHtmExists
    End Function

    Private Function ViewFromAreaImgURL(ByVal packageCode As String) As String
        Dim imgURL As String = String.Empty
        Dim viewImageName As String = packageCode
        imgURL = ImagePath.getImagePath("PRODCORPORATE", viewImageName, _businessUnit, _partner)
        If imgURL.Contains(ModuleDefaults.MissingImagePath) Then
            imgURL = String.Empty
        End If
        Return imgURL
    End Function

#End Region

End Class
