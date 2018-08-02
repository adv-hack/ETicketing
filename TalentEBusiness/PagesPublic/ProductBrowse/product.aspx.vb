Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
Imports System.Web.UI.WebControls

Partial Class PagesPublic_product
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _wfr As Talent.Common.WebFormResource = Nothing
    Private _languageCode As String = String.Empty
    Private _businessUnit As String = String.Empty
    Private _partner As String = String.Empty
    Private _currentPage As String = String.Empty

#End Region

#Region "Public Properties"

    Public Property TabTitle1 As String = String.Empty
    Public Property TabTitle2 As String = String.Empty
    Public Property TabTitle3 As String = String.Empty
    Public Property TabTitle4 As String = String.Empty
    Public Property TabTitle5 As String = String.Empty
    Public Property TabTitle6 As String = String.Empty
    Public Property ProductHTML7Title As String = String.Empty
    Public Property ProductHTML8Title As String = String.Empty
    Public Property ProductHTML9Title As String = String.Empty


#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _wfr = New Talent.Common.WebFormResource
        _languageCode = Utilities.GetCurrentLanguage
        _businessUnit = TalentCache.GetBusinessUnit
        _partner = TalentCache.GetPartner(Profile)
        _currentPage = Talent.eCommerce.Utilities.GetCurrentPageName()

        With _wfr
            .BusinessUnit = _businessUnit
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = _partner
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "product.aspx"
        End With

        Dim productCode As String = Request("product")
        Dim productData As New TalentProductInformationTableAdapters.tbl_productTableAdapter
        Dim productDataTable As TalentProductInformation.tbl_productDataTable
        Dim productRow As TalentProductInformation.tbl_productRow

        setupTextAndDisplay()

        If Not productCode Is Nothing Then
            productDataTable = productData.GetDataByProduct_Code(productCode)
            If productDataTable.Rows.Count > 0 Then
                productRow = productDataTable.Rows(0)
                plhProductCode.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfr.Attribute("DisplayProductCode"))

                ltlProductDescription2.Text = CheckForDBNull_String(productRow.PRODUCT_DESCRIPTION_2)
                plhProductDescription2.Visible = (ltlProductDescription2.Text.Length > 0)
                ltlProductDescription3.Text = CheckForDBNull_String(productRow.PRODUCT_DESCRIPTION_3)
                plhProductDescription3.Visible = (ltlProductDescription3.Text.Length > 0)
                ltlProductDescription4.Text = CheckForDBNull_String(productRow.PRODUCT_DESCRIPTION_4)
                plhProductDescription4.Visible = (ltlProductDescription4.Text.Length > 0)
                ltlProductDescription5.Text = CheckForDBNull_String(productRow.PRODUCT_DESCRIPTION_5)
                plhProductDescription5.Visible = (ltlProductDescription5.Text.Length > 0)
                ltlProductCode.Text = productCode
                '----------------------------------------------
                ' Check whether HTML include is from DB or File
                '----------------------------------------------
                If ModuleDefaults.ProductHTML Then
                    Select Case ModuleDefaults.ProductHTMLType
                        Case Is = "DB"
                            If (Not productRow.IsPRODUCT_HTML_1Null) Then
                                lblHTML1.Text = CheckForDBNull_String(productRow.PRODUCT_HTML_1)
                            End If
                            If (Not productRow.IsPRODUCT_HTML_2Null) Then
                                lblHTML2.Text = CheckForDBNull_String(productRow.PRODUCT_HTML_2)
                            End If
                            If (Not productRow.IsPRODUCT_HTML_3Null) Then
                                lblHTML3.Text = CheckForDBNull_String(productRow.PRODUCT_HTML_3)
                            End If
                            If (Not productRow.IsPRODUCT_HTML_4Null) Then
                                lblHTML4.Text = CheckForDBNull_String(productRow.PRODUCT_HTML_4)
                            End If
                            If (Not productRow.IsPRODUCT_HTML_5Null) Then
                                lblHTML5.Text = CheckForDBNull_String(productRow.PRODUCT_HTML_5)
                            End If
                            If (Not productRow.IsPRODUCT_HTML_6Null) Then
                                lblHTML6.Text = CheckForDBNull_String(productRow.PRODUCT_HTML_6)
                            End If
                            If (Not productRow.IsPRODUCT_HTML_7Null) Then
                                lblHTML7.Text = CheckForDBNull_String(productRow.PRODUCT_HTML_7)
                            End If
                            If (Not productRow.IsPRODUCT_HTML_8Null) Then
                                lblHTML8.Text = CheckForDBNull_String(productRow.PRODUCT_HTML_8)
                            End If
                            If (Not productRow.IsPRODUCT_HTML_9Null) Then
                                lblHTML9.Text = CheckForDBNull_String(productRow.PRODUCT_HTML_9)
                            End If
                            'lblHTML8.Text = CheckForDBNull_String(productRow.PRODUCT_HTML_8)
                            'lblHTML9.Text = CheckForDBNull_String(productRow.PRODUCT_HTML_9)
                        Case Is = "FILE"
                            Dim htmlRootpath As String = ModuleDefaults.HtmlIncludePathRelative
                            If String.IsNullOrEmpty(htmlRootpath) Then htmlRootpath = ConfigurationManager.AppSettings("htmlIncludeRootPath")
                            '---------------
                            ' Try for BU/PAR
                            '---------------

                            'Test for .htm and .html
                            lblHTML1.Text = GetHtmlFromFile(_businessUnit & "/" & _partner & "/" & "Product/1/" & productCode & ".htm")
                            If String.IsNullOrEmpty(lblHTML1.Text) Then _
                                            lblHTML1.Text = GetHtmlFromFile(_businessUnit & "/" & _partner & "/" & "Product/1/" & productCode & ".html")
                            '--------------------
                            If String.IsNullOrEmpty(lblHTML1.Text) Then
                                lblHTML1.Text = GetHtmlFromFile(_businessUnit & "/" & "Product/1/" & productCode & ".htm")
                                If String.IsNullOrEmpty(lblHTML1.Text) Then _
                                            lblHTML1.Text = GetHtmlFromFile(_businessUnit & "/" & "Product/1/" & productCode & ".html")
                                If String.IsNullOrEmpty(lblHTML1.Text) Then
                                    '-------------
                                    ' Try for root
                                    '-------------
                                    lblHTML1.Text = GetHtmlFromFile("Product/1/" & productCode & ".htm")
                                    If String.IsNullOrEmpty(lblHTML1.Text) Then _
                                                lblHTML1.Text = GetHtmlFromFile("Product/1/" & productCode & ".html")
                                End If
                            End If
                            lblHTML2.Text = GetHtmlFromFile(_businessUnit & "/" & _partner & "/" & "Product/2/" & productCode & ".htm")
                            If String.IsNullOrEmpty(lblHTML2.Text) Then _
                                        lblHTML2.Text = GetHtmlFromFile(_businessUnit & "/" & _partner & "/" & "Product/2/" & productCode & ".html")
                            If String.IsNullOrEmpty(lblHTML2.Text) Then
                                '-----------
                                ' Try for BU
                                '-----------
                                lblHTML2.Text = GetHtmlFromFile(_businessUnit & "/" & "Product/2/" & productCode & ".htm")
                                If String.IsNullOrEmpty(lblHTML2.Text) Then _
                                            lblHTML2.Text = GetHtmlFromFile(_businessUnit & "/" & "Product/2/" & productCode & ".html")
                                If String.IsNullOrEmpty(lblHTML2.Text) Then
                                    '-------------
                                    ' Try for root
                                    '-------------
                                    lblHTML2.Text = GetHtmlFromFile("Product/2/" & productCode & ".htm")
                                    If String.IsNullOrEmpty(lblHTML2.Text) Then _
                                                lblHTML2.Text = GetHtmlFromFile("Product/2/" & productCode & ".html")
                                End If
                            End If
                        Case Else
                    End Select

                End If

                plhTabTitle1.Visible = False
                plhTabTitle2.Visible = False
                plhTabTitle3.Visible = False
                plhTabTitle4.Visible = False
                plhTabTitle5.Visible = False
                plhTabTitle6.Visible = False

                plhContent1.Visible = False
                plhContent2.Visible = False
                plhContent3.Visible = False
                plhContent4.Visible = False
                plhContent5.Visible = False
                plhContent6.Visible = False

                Dim tabIsActivated As Boolean = False

                If (Not String.IsNullOrWhiteSpace(TabTitle1) AndAlso Not String.IsNullOrWhiteSpace(lblHTML1.Text)) Then
                    plhTabTitle1.Visible = True
                    plhContent1.Visible = True
                    plhTabTitle1li.Attributes("class") = "tabs-title is-active"
                    tabIsActivated = True
                End If
                If (Not String.IsNullOrWhiteSpace(TabTitle2) AndAlso Not String.IsNullOrWhiteSpace(lblHTML2.Text)) Then
                    plhTabTitle2.Visible = True
                    plhContent2.Visible = True
                    If Not tabIsActivated Then
                        plhTabTitle2li.Attributes("class") = "tabs-title is-active"
                        tabIsActivated = True
                    End If
                End If
                If (Not String.IsNullOrWhiteSpace(TabTitle3) AndAlso Not String.IsNullOrWhiteSpace(lblHTML3.Text)) Then
                    plhTabTitle3.Visible = True
                    plhContent3.Visible = True
                    If Not tabIsActivated Then
                        plhTabTitle3li.Attributes("class") = "tabs-title is-active"
                        tabIsActivated = True
                    End If
                End If
                If (Not String.IsNullOrWhiteSpace(TabTitle4) AndAlso Not String.IsNullOrWhiteSpace(lblHTML4.Text)) Then
                    plhTabTitle4.Visible = True
                    plhContent4.Visible = True
                    If Not tabIsActivated Then
                        plhTabTitle4li.Attributes("class") = "tabs-title is-active"
                        tabIsActivated = True
                    End If
                End If
                If (Not String.IsNullOrWhiteSpace(TabTitle5) AndAlso Not String.IsNullOrWhiteSpace(lblHTML5.Text)) Then
                    plhTabTitle5.Visible = True
                    plhContent5.Visible = True
                    If Not tabIsActivated Then
                        plhTabTitle5li.Attributes("class") = "tabs-title is-active"
                        tabIsActivated = True
                    End If
                End If
                If (Not String.IsNullOrWhiteSpace(TabTitle6) AndAlso Not String.IsNullOrWhiteSpace(lblHTML6.Text)) Then
                    plhTabTitle6.Visible = True
                    plhContent6.Visible = True
                    If Not tabIsActivated Then
                        plhTabTitle6li.Attributes("class") = "tabs-title is-active"
                        tabIsActivated = True
                    End If
                End If

                'The following lines control the visiblity of PRODUCT_HTML_7-9
                If (Not String.IsNullOrWhiteSpace(ProductHTML7Title) AndAlso Not String.IsNullOrWhiteSpace(lblHTML7.Text)) Then
                    plhTabTitle7.Visible = True
                    plhContent7.Visible = True
                End If
                If (Not String.IsNullOrWhiteSpace(ProductHTML8Title) AndAlso Not String.IsNullOrWhiteSpace(lblHTML8.Text)) Then
                    plhTabTitle8.Visible = True
                    plhContent8.Visible = True
                End If
                If (Not String.IsNullOrWhiteSpace(ProductHTML9Title) AndAlso Not String.IsNullOrWhiteSpace(lblHTML9.Text)) Then
                    plhTabTitle9.Visible = True
                    plhContent9.Visible = True
                End If
            Else
                blErrorMessages.Items.Add(_wfr.Content("NoProductsText", _languageCode, True))
            End If
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        blErrorMessages.Items.Clear()
        If Not String.IsNullOrEmpty(ProductControls1.ErrorMessage) Then
            blErrorMessages.Items.Add(ProductControls1.ErrorMessage)
        End If
        Dim productOptions As UserControls_ProductOptions = ProductControls1.FindControl("ProductOptions1")
        If productOptions IsNot Nothing Then
            If productOptions.ErrorMessage IsNot Nothing Then
                If blErrorMessages.Items.FindByText(productOptions.ErrorMessage) Is Nothing Then
                    blErrorMessages.Items.Add(productOptions.ErrorMessage)
                End If
            End If
        End If
        plhErrorMessage.Visible = (blErrorMessages.Items.Count > 0)
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Setup the page text properties and display options
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setupTextAndDisplay()

        Dim prodCode As String = Request("product")

        imgProduct.ImageUrl = ImagePath.getMagicZoomOrMainImage(prodCode, _businessUnit, _partner)
        If (imgProduct.ImageUrl = "") Then
            imgProduct.ImageUrl = ModuleDefaults.MissingImagePath
        End If
        If (ProductMagicZoomAdditionalImages.MagicZoomVisibility = True) Then
            imgProduct.Visible = False
        End If
        imgProduct.Visible = False

        plhProductImage.Visible = True

        TabTitle1 = _wfr.Content("TabTitle1", _languageCode, True)
        TabTitle2 = _wfr.Content("TabTitle2", _languageCode, True)
        TabTitle3 = _wfr.Content("TabTitle3", _languageCode, True)
        TabTitle4 = _wfr.Content("TabTitle4", _languageCode, True)
        TabTitle5 = _wfr.Content("TabTitle5", _languageCode, True)
        TabTitle6 = _wfr.Content("TabTitle6", _languageCode, True)
        ProductHTML7Title = _wfr.Content("ProductHTML7Title", _languageCode, True)
        ProductHTML8Title = _wfr.Content("ProductHTML8Title", _languageCode, True)
        ProductHTML9Title = _wfr.Content("ProductHTML9Title", _languageCode, True)

        If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("DisplayPriceBreakTable")) Then
            ucPriceBreakTable.Visible = True
            ucPriceBreakTable.Display = True
        Else
            ucPriceBreakTable.Visible = False
            ucPriceBreakTable.Display = False
        End If

        ltlProductCodeText.Text = _wfr.Content("ProductCodeText", _languageCode, True)
    End Sub

#End Region



End Class
