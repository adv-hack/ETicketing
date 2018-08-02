Imports Talent.Common
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports TCUtilities = Talent.Common.Utilities
Imports System.Data
Imports System.Globalization

Partial Class PagesPublic_ProductBrowse_PartnerPromotions
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _wfrPage As WebFormResource = Nothing
    Private _ucr As UserControlResource = Nothing
    Private _languageCode As String = Nothing
    Private _errMsg As TalentErrorMessages = Nothing
    Private _promotionCode As String = String.Empty
    Private _dtPromotionProducts As DataTable = Nothing

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        _wfrPage = New WebFormResource
        With _wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "PartnerPromotions.aspx"
        End With
        _languageCode = TCUtilities.GetDefaultLanguage
        _errMsg = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), _wfrPage.FrontEndConnectionString)
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Request.Form("promotion-code") IsNot Nothing Then
            'Check to see if the promotion code has been posted in via an external form
            _promotionCode = TCUtilities.TripleDESDecode(Request.Form("promotion-code"), ModuleDefaults.NOISE_ENCRYPTION_KEY)
            If _promotionCode.Length = 0 Then
                'Try again without decoding
                _promotionCode = Request.Form("promotion-code")
            End If
        Else
            'If promotion code is passed in via querystring (this will happen if the user has queued first)
            'The words "promotion code" are hidden from the querystring for security.
            If Request.QueryString("xcvs") IsNot Nothing Then
                _promotionCode = TCUtilities.TripleDESDecode(Request.QueryString("xcvs"), ModuleDefaults.NOISE_ENCRYPTION_KEY)
            Else
                If Session("PartnerPromotionCode") IsNot Nothing Then
                    _promotionCode = Session("PartnerPromotionCode")
                End If
                If Request.QueryString("promotion") IsNot Nothing Then
                    _promotionCode = Request.QueryString("promotion")
                End If
            End If
        End If
        If _promotionCode.Length = 0 Then
            Response.Redirect("~/PagesPublic/Error/NotFound.aspx")
        Else
            validatePromotionCode()
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        plhErrorMessage.Visible = (ltlErrorMessage.Text.Length > 0)
    End Sub

    Protected Sub rptPartnerPromotions_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptPartnerPromotions.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim hplNavigateUrl As HyperLink = CType(e.Item.FindControl("hplNavigateUrl"), HyperLink)
            Dim OppositionImage As Image = CType(e.Item.FindControl("OppositionImage"), Image)
            Dim CompetitionImage As Image = CType(e.Item.FindControl("CompetitionImage"), Image)
            Dim plhSponsorImage As PlaceHolder = CType(e.Item.FindControl("plhSponsorImage"), PlaceHolder)
            Dim SponsorImage As Image = CType(e.Item.FindControl("SponsorImage"), Image)
            Dim plhProductDescription As PlaceHolder = CType(e.Item.FindControl("plhProductDescription"), PlaceHolder)
            Dim ltlProductDescription As Literal = CType(e.Item.FindControl("ltlProductDescription"), Literal)
            Dim plhProductCompetition As PlaceHolder = CType(e.Item.FindControl("plhProductCompetition"), PlaceHolder)
            Dim ltlProductCompetitionLabel As Literal = CType(e.Item.FindControl("ltlProductCompetitionLabel"), Literal)
            Dim ltlProductCompetitionValue As Literal = CType(e.Item.FindControl("ltlProductCompetitionValue"), Literal)
            Dim plhProductDate As PlaceHolder = CType(e.Item.FindControl("plhProductDate"), PlaceHolder)
            Dim ltlProductDateLabel As Literal = CType(e.Item.FindControl("ltlProductDateLabel"), Literal)
            Dim ltlProductDateValue As Literal = CType(e.Item.FindControl("ltlProductDateValue"), Literal)
            Dim plhProductTime As PlaceHolder = CType(e.Item.FindControl("plhProductTime"), PlaceHolder)
            Dim ltlProductTimeLabel As Literal = CType(e.Item.FindControl("ltlProductTimeLabel"), Literal)
            Dim ltlProductTimeValue As Literal = CType(e.Item.FindControl("ltlProductTimeValue"), Literal)
            Dim plhLoyaltyPoints As PlaceHolder = CType(e.Item.FindControl("plhLoyaltyPoints"), PlaceHolder)
            Dim ltlLoyaltyPointsLabel As Literal = CType(e.Item.FindControl("ltlLoyaltyPointsLabel"), Literal)
            Dim ltlLoyaltyPointsValue As Literal = CType(e.Item.FindControl("ltlLoyaltyPointsValue"), Literal)
            Dim plhAge As PlaceHolder = CType(e.Item.FindControl("plhAge"), PlaceHolder)
            Dim ltlAgeLabel As Literal = CType(e.Item.FindControl("ltlAgeLabel"), Literal)
            Dim ltlAgeValue As Literal = CType(e.Item.FindControl("ltlAgeValue"), Literal)
            Dim plhDuration As PlaceHolder = CType(e.Item.FindControl("plhDuration"), PlaceHolder)
            Dim ltlDurationLabel As Literal = CType(e.Item.FindControl("ltlDurationLabel"), Literal)
            Dim ltlDurationValue As Literal = CType(e.Item.FindControl("ltlDurationValue"), Literal)
            Dim plhLocation As PlaceHolder = CType(e.Item.FindControl("plhLocation"), PlaceHolder)
            Dim ltlLocationLabel As Literal = CType(e.Item.FindControl("ltlLocationLabel"), Literal)
            Dim ltlLocationValue As Literal = CType(e.Item.FindControl("ltlLocationValue"), Literal)
            Dim ltlExtendedText1 As Literal = CType(e.Item.FindControl("ltlExtendedText1"), Literal)
            Dim ltlExtendedText2 As Literal = CType(e.Item.FindControl("ltlExtendedText2"), Literal)
            Dim ltlExtendedText3 As Literal = CType(e.Item.FindControl("ltlExtendedText3"), Literal)
            Dim ltlExtendedText4 As Literal = CType(e.Item.FindControl("ltlExtendedText4"), Literal)
            Dim ltlExtendedText5 As Literal = CType(e.Item.FindControl("ltlExtendedText5"), Literal)
            Dim pnlExtendedText As PlaceHolder = CType(e.Item.FindControl("pnlExtendedText"), PlaceHolder)
            Dim productCode As String = TEBUtilities.CheckForDBNull_String(e.Item.DataItem("ProductCode"))
            Dim productType As String = TEBUtilities.CheckForDBNull_String(e.Item.DataItem("ProductType"))
            Dim stadiumCode As String = TEBUtilities.CheckForDBNull_String(e.Item.DataItem("StadiumCode"))
            Dim dtProductDetails As New DataTable

            dtProductDetails = retrieveProductDetails(productCode)
            If dtProductDetails.Rows.Count > 0 Then
                If TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(dtProductDetails.Rows(0)("AvailableOnline")) AndAlso Not TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(dtProductDetails.Rows(0)("IsSoldOut")) Then
                    hplNavigateUrl.NavigateUrl = getProductNavigationUrl(productCode, productType, TEBUtilities.CheckForDBNull_String(dtProductDetails.Rows(0)("ProductSubType")), String.Empty, stadiumCode, _
                                                                         TEBUtilities.CheckForDBNull_String(dtProductDetails.Rows(0)("HomeAsAway")), TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(dtProductDetails.Rows(0)("RestrictGraphical")))
                    OppositionImage.ImageUrl = ProductDetail.GetImageURL("PRODTICKETING", TEBUtilities.CheckForDBNull_String(dtProductDetails.Rows(0)("OppositionCode")))
                    CompetitionImage.ImageUrl = ProductDetail.GetImageURL("PRODCOMPETITION", dtProductDetails.Rows(0)("Competition"))
                    SponsorImage.ImageUrl = ProductDetail.GetImageURL("PRODSPONSOR", dtProductDetails.Rows(0)("Sponsor"))
                    If OppositionImage.ImageUrl = ModuleDefaults.MissingImagePath Then OppositionImage.Visible = False
                    If CompetitionImage.ImageUrl = ModuleDefaults.MissingImagePath Then CompetitionImage.Visible = False
                    If SponsorImage.ImageUrl = ModuleDefaults.MissingImagePath Then plhSponsorImage.Visible = False

                    ltlProductDescription.Text = TEBUtilities.CheckForDBNull_String(dtProductDetails.Rows(0)("ProductDescription"))
                    plhProductDescription.Visible = (ltlProductDescription.Text.Length > 0)
                    ltlProductCompetitionLabel.Text = _ucr.Content("ProductCompetitionLabel", _languageCode, True)
                    ltlProductCompetitionValue.Text = TEBUtilities.CheckForDBNull_String(dtProductDetails.Rows(0)("ProductText1"))
                    plhProductCompetition.Visible = (ltlProductCompetitionLabel.Text.Length > 0 AndAlso ltlProductCompetitionValue.Text.Length > 0)
                    If Not dtProductDetails.Rows(0)("HideDate") Then
                        ltlProductDateLabel.Text = _ucr.Content("ProductDateLabel", _languageCode, True)
                        ltlProductDateValue.Text = TEBUtilities.GetFormattedDateAndTime(dtProductDetails.Rows(0)("MDTE08Date").ToString(), String.Empty, " ", ModuleDefaults.GlobalDateFormat, ModuleDefaults.Culture)
                    Else
                        ltlProductDateLabel.Text = String.Empty
                        ltlProductDateValue.Text = String.Empty
                    End If
                    plhProductDate.Visible = (ltlProductDateLabel.Text.Length > 0 AndAlso ltlProductDateValue.Text.Length > 0)
                    If (TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("ShowTimeLabel"))) AndAlso Not dtProductDetails.Rows(0)("HideTime") Then
                        ltlProductTimeLabel.Text = _ucr.Content("KickoffLabel", _languageCode, True)
                        ltlProductTimeValue.Text = dtProductDetails.Rows(0)("ProductTime")
                    Else
                        ltlProductTimeLabel.Text = String.Empty
                        ltlProductTimeValue.Text = String.Empty
                    End If
                    plhProductTime.Visible = (ltlProductTimeLabel.Text.Length > 0 AndAlso ltlProductTimeValue.Text.Length > 0)
                    ltlLoyaltyPointsLabel.Text = _ucr.Content("LoyaltyPointsLabel", _languageCode, True)
                    ltlLoyaltyPointsValue.Text = dtProductDetails.Rows(0)("ProductReqdLoyalityPoints").ToString().TrimStart(GlobalConstants.LEADING_ZEROS)
                    plhLoyaltyPoints.Visible = ltlLoyaltyPointsLabel.Text.Length > 0 AndAlso ltlLoyaltyPointsValue.Text.Length > 0

                    plhAge.Visible = (TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("ShowAgeLabel") AndAlso TEBUtilities.CheckForDBNull_Int(dtProductDetails.Rows(0)("FromAge")) > 0 AndAlso TEBUtilities.CheckForDBNull_Int(dtProductDetails.Rows(0)("ToAge")) > 0))
                    If plhAge.Visible Then
                        Dim ageRange As String = String.Empty
                        ageRange = dtProductDetails.Rows(0)("FromAge").ToString.Trim & _ucr.Content("AgeRangeDividerText", _languageCode, True) & dtProductDetails.Rows(0)("ToAge").ToString.Trim
                        If String.IsNullOrEmpty(ageRange) Then
                            plhAge.Visible = False
                        Else
                            plhAge.Visible = True
                            ltlAgeLabel.Text = _ucr.Content("AgeLabelText", _languageCode, True)
                            ltlAgeValue.Text = ageRange
                        End If
                    End If
                    plhDuration.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("ShowDurationLabel"))
                    If plhDuration.Visible Then
                        ltlDurationLabel.Text = _ucr.Content("DurationLabelText", _languageCode, True)
                        ltlDurationValue.Text = TEBUtilities.CheckForDBNull_String(dtProductDetails.Rows(0)("Duration"))
                    End If
                    plhLocation.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("ShowLocationLabel"))
                    If plhLocation.Visible Then
                        ltlLocationLabel.Text = _ucr.Content("LocationLabelText", _languageCode, True)
                        ltlLocationValue.Text = TEBUtilities.CheckForDBNull_String(dtProductDetails.Rows(0)("Location"))
                    End If

                    ltlExtendedText1.Text = dtProductDetails.Rows(0)("ProductDetail1").ToString.Trim
                    ltlExtendedText2.Text = dtProductDetails.Rows(0)("ProductDetail2").ToString.Trim
                    ltlExtendedText3.Text = dtProductDetails.Rows(0)("ProductDetail3").ToString.Trim
                    ltlExtendedText4.Text = dtProductDetails.Rows(0)("ProductDetail4").ToString.Trim
                    ltlExtendedText5.Text = dtProductDetails.Rows(0)("ProductDetail5").ToString.Trim
                    pnlExtendedText.Visible = (ltlExtendedText1.Text.Trim().Length > 0 OrElse ltlExtendedText2.Text.Trim().Length > 0 OrElse _
                            ltlExtendedText3.Text.Trim().Length > 0 OrElse ltlExtendedText4.Text.Trim().Length > 0 OrElse ltlExtendedText5.Text.Trim().Length > 0)
                Else
                    e.Item.Visible = False
                End If
                End If
        End If
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Validate the promotion code and retrieve the products, bind the products to the repeater
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub validatePromotionCode()
        Dim promotionCodeIsValid As Boolean = True
        Dim errorCode As String = String.Empty
        promotionCodeIsValid = retrieveProductsByPromotion(errorCode)

        If promotionCodeIsValid Then
            Try
                If _dtPromotionProducts IsNot Nothing AndAlso _dtPromotionProducts.Rows.Count > 0 Then
                    _ucr = New UserControlResource
                    With _ucr
                        .BusinessUnit = TalentCache.GetBusinessUnit()
                        .PageCode = TEBUtilities.GetCurrentPageName
                        .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                        .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                        .KeyCode = "ProductDetail.ascx"
                    End With
                    rptPartnerPromotions.DataSource = _dtPromotionProducts
                    rptPartnerPromotions.DataBind()
                    Session("PartnerPromotionCode") = _promotionCode
                    populatePromotionSpecificContent()
                Else
                    ltlErrorMessage.Text = _errMsg.GetErrorMessage("PartnerPromotions", _wfrPage.PageCode, "GenericError").ERROR_MESSAGE
                End If
            Catch ex As Exception
                ltlErrorMessage.Text = _ucr.Content("GenericError", _languageCode, True)
            End Try
        Else
            ltlErrorMessage.Text = _errMsg.GetErrorMessage(errorCode).ERROR_MESSAGE
        End If
    End Sub

    ''' <summary>
    ''' Populate the promotions specific content to the page if there are products that are available for the given promotion
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populatePromotionSpecificContent()
        If rptPartnerPromotions.Items.Count > 0 Then
            Dim contentType As String = String.Empty
            Dim dtPromotionsSpecificContent As New DataTable
            Dim contentToRender As New StringBuilder

            dtPromotionsSpecificContent = TDataObjects.PromotionsSettings.TblPromotionsSpecificContent.GetPromotionSpecificContent(contentType, _promotionCode)
            For Each row As DataRow In dtPromotionsSpecificContent.Rows
                contentToRender.Append(row("PROMOTION_CONTENT").ToString())
            Next
            ltlPromotionSpecificContent.Text = contentToRender.ToString()
        End If
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Get the product promotion data and return true if the retrieval is successful
    ''' </summary>
    ''' <param name="errorCode">The error code returned</param>
    ''' <returns>True if successful</returns>
    ''' <remarks></remarks>
    Private Function retrieveProductsByPromotion(ByRef errorCode As String) As Boolean
        Dim promotionValid As Boolean = False
        Dim err As New ErrorObj
        Dim settings As DESettings = TEBUtilities.GetSettingsObject()
        Dim promotions As New TalentPromotions
        Dim de As New DEPromotions

        de.PromotionCode = _promotionCode
        de.Source = GlobalConstants.SOURCE
        promotions.Dep = de
        promotions.Settings = settings
        err = promotions.GetPartnerPromotions()

        If Not err.HasError AndAlso promotions.ResultDataSet IsNot Nothing Then
            _dtPromotionProducts = promotions.ResultDataSet.Tables("PartnerPromotions")
            promotionValid = True
        Else
            promotionValid = False
            ltlErrorMessage.Text = _errMsg.GetErrorMessage("PartnerPromotions", _wfrPage.PageCode, "GenericError").ERROR_MESSAGE
        End If

        Return promotionValid
    End Function

    ''' <summary>
    ''' Get the product details datatable from WS007R
    ''' </summary>
    ''' <param name="productCode">The given product code to retrieve</param>
    ''' <returns>Data table of results</returns>
    ''' <remarks></remarks>
    Private Function retrieveProductDetails(ByVal productCode As String) As DataTable
        Dim product As New TalentProduct
        Dim de As New DEProductDetails
        Dim settings As DESettings = TEBUtilities.GetSettingsObject()
        Dim productDetails As New DataTable
        Dim err As New ErrorObj
        settings.Cacheing = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_wfrPage.Attribute("ProductDetailsCaching"))
        settings.CacheTimeMinutes = TEBUtilities.CheckForDBNull_Int(_wfrPage.Attribute("ProductDetailsCacheTimeInMins"))
        de.ProductCode = productCode
        de.AllowPriceException = True
        product.Settings = settings
        product.De = de
        err = product.ProductDetails()

        If Not err.HasError AndAlso product.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred").ToString() <> GlobalConstants.ERRORFLAG Then
            productDetails = product.ResultDataSet.Tables("ProductDetails")
        End If
        Return productDetails
    End Function

    ''' <summary>
    ''' Get the formatted URL for the product page based on the product parameters
    ''' </summary>
    ''' <param name="productCode">The given product code</param>
    ''' <param name="productType">The given product type</param>
    ''' <param name="productSubType">The given product sub type</param>
    ''' <param name="campaignCode">The given campaign/price code</param>
    ''' <param name="stadiumCode">The given stadium code</param>
    ''' <param name="productHomeAsAway">The given home as away value</param>
    ''' <param name="restrictGraphical">The retrict graphical value</param>
    ''' <returns>The fully formatted URL</returns>
    ''' <remarks></remarks>
    Private Function getProductNavigationUrl(ByVal productCode As String, ByVal productType As String, ByVal productSubType As String, ByVal campaignCode As String, ByVal stadiumCode As String, ByVal productHomeAsAway As String, ByVal restrictGraphical As Boolean) As String
        Dim navigateUrl As New StringBuilder
        Select Case productType
            Case Is = GlobalConstants.HOMEPRODUCTTYPE, GlobalConstants.SEASONTICKETPRODUCTTYPE
                navigateUrl.Append(ResolveUrl(TEBUtilities.GetFormattedSeatSelectionUrl(String.Empty, stadiumCode, productCode, campaignCode, productType, productSubType, productHomeAsAway, restrictGraphical)))
            Case Is = GlobalConstants.AWAYPRODUCTTYPE
                navigateUrl.Append(ResolveUrl("~/PagesPublic/ProductBrowse/ProductAway.aspx?IsSingleProduct=TRUE"))
                If productCode.Length > 0 Then navigateUrl.Append("&ProductCode=").Append(productCode)
                If productType.Length > 0 Then navigateUrl.Append("&ProductType=").Append(productType)
                If productSubType.Length > 0 Then navigateUrl.Append("&ProductSubType=").Append(productSubType)
            Case Is = GlobalConstants.MEMBERSHIPPRODUCTTYPE
                navigateUrl.Append(ResolveUrl("~/PagesPublic/ProductBrowse/ProductMembership.aspx?IsSingleProduct=TRUE"))
                If productCode.Length > 0 Then navigateUrl.Append("&ProductCode=").Append(productCode)
                If productType.Length > 0 Then navigateUrl.Append("&ProductType=").Append(productType)
                If productSubType.Length > 0 Then navigateUrl.Append("&ProductSubType=").Append(productSubType)
            Case Is = GlobalConstants.TRAVELPRODUCTTYPE
                navigateUrl.Append(ResolveUrl("~/PagesPublic/ProductBrowse/ProductTravel.aspx?IsSingleProduct=TRUE"))
                If productCode.Length > 0 Then navigateUrl.Append("&ProductCode=").Append(productCode)
                If productType.Length > 0 Then navigateUrl.Append("&ProductType=").Append(productType)
                If productSubType.Length > 0 Then navigateUrl.Append("&ProductSubType=").Append(productSubType)
            Case Is = GlobalConstants.EVENTPRODUCTTYPE
                navigateUrl.Append(ResolveUrl("~/PagesPublic/ProductBrowse/ProductEvent.aspx?IsSingleProduct=TRUE"))
                If productCode.Length > 0 Then navigateUrl.Append("&ProductCode=").Append(productCode)
                If productType.Length > 0 Then navigateUrl.Append("&ProductType=").Append(productType)
                If productSubType.Length > 0 Then navigateUrl.Append("&ProductSubType=").Append(productSubType)
        End Select
        Return navigateUrl.ToString()
    End Function

    ''' <summary>
    ''' Get the formatted date string based on the iSeries 7 char date format and the year
    ''' </summary>
    ''' <param name="ProductDate">Date as 1141111</param>
    ''' <param name="ProductYear">Year as 2014</param>
    ''' <returns>Formatted date based on culture and system defaults</returns>
    ''' <remarks></remarks>
    Protected Function GetFormattedProductDate(ByVal ProductDate As String, ByVal ProductYear As String) As String
        Dim str As String = String.Empty
        Dim dateValue As Date = Talent.Common.Utilities.ISeriesDate(ProductDate.Trim)
        ' if date format differ
        If ModuleDefaults.GlobalDateFormat = "yyyy/MM/dd" Then
            Dim dateString As String = dateValue.ToString("dd MMMM")
            Dim culture As New CultureInfo(ModuleDefaults.Culture)
            Dim day As String = culture.DateTimeFormat.DayNames(dateValue.DayOfWeek)
            Dim dateSeparator As String = _ucr.Content("DateSeparator", _languageCode, True)
            str = day & dateSeparator & dateString & dateSeparator & ProductYear
        Else
            Dim culture As New CultureInfo(ModuleDefaults.Culture)
            str = dateValue.ToString(ModuleDefaults.GlobalDateFormat, culture)
        End If
        Return str
    End Function

#End Region

End Class
