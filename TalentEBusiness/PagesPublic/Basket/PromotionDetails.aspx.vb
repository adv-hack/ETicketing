Imports Talent.Common
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports TCUtilities = Talent.Common.Utilities
Imports System.Data

Partial Class PagesPublic_Basket_PromotionDetails
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _wfrPage As New WebFormResource
    Private _languageCode As String = String.Empty
    Private _originalPrice As Decimal = 0
    Private _salePrice As Decimal = 0

#End Region

#Region "Constants"

    Const KEYCODE As String = "PromotionDetails.aspx"

#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With _wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = KEYCODE
        End With
        _languageCode = TCUtilities.GetDefaultLanguage()

        'Ticketing Promotions
        Dim ticketingPromotionToShow As Boolean = False
        If Request.QueryString("promotionid1") IsNot Nothing Then ticketingPromotionToShow = True
        If Request.QueryString("promotionid2") IsNot Nothing Then ticketingPromotionToShow = True
        If Request.QueryString("promotionid3") IsNot Nothing Then ticketingPromotionToShow = True
        If ticketingPromotionToShow Then
            If Request.QueryString("originalprice") IsNot Nothing Then
                _originalPrice = TEBUtilities.CheckForDBNull_Decimal(Request.QueryString("originalprice"))
            End If
            If Request.QueryString("saleprice") IsNot Nothing Then
                _salePrice = TEBUtilities.CheckForDBNull_Decimal(Request.QueryString("saleprice"))
            End If
            ticketingPromotionToShow = bindRepeater()
        End If
        plhTicketingPromotionDetails.Visible = ticketingPromotionToShow

        'Merchandise Promotions
        Dim merchandisePromotionToShow As Boolean = handleMerchandisePromotions()

        plhMerchandisePromotionDetails.Visible = merchandisePromotionToShow
        If Not ticketingPromotionToShow AndAlso Not merchandisePromotionToShow Then
            plhPromotionNotFound.Visible = False
            ltlPromotionNotFound.Text = _wfrPage.Content("NoPromotionsFound", _languageCode, True)
        End If

    End Sub

    Protected Sub rptTicketingPromotionDetails_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptTicketingPromotionDetails.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            If Not bindPromotions(e.Item.DataItem, e.Item) Then
                e.Item.Visible = False
            End If
        End If
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Set the text values on the page based on the given promotion data table
    ''' </summary>
    ''' <param name="promotionTable">A data table of promotions</param>
    ''' <remarks></remarks>
    Private Sub setPageText(ByVal promotionTable As DataTable, ByRef e As RepeaterItem)
        Dim ltlTicketingPromotionDescription1Label As Literal = CType(e.FindControl("ltlTicketingPromotionDescription1Label"), Literal)
        Dim ltlTicketingPromotionStartDateLabel As Literal = CType(e.FindControl("ltlTicketingPromotionStartDateLabel"), Literal)
        Dim ltlTicketingPromotionEndDateLabel As Literal = CType(e.FindControl("ltlTicketingPromotionEndDateLabel"), Literal)
        Dim ltlTicketingPromotionDescription1Value As Literal = CType(e.FindControl("ltlTicketingPromotionDescription1Value"), Literal)
        Dim ltlTicketingPromotionDescription2Value As Literal = CType(e.FindControl("ltlTicketingPromotionDescription2Value"), Literal)
        Dim ltlTicketingPromotionStartDateValue As Literal = CType(e.FindControl("ltlTicketingPromotionStartDateValue"), Literal)
        Dim ltlTicketingPromotionEndDateValue As Literal = CType(e.FindControl("ltlTicketingPromotionEndDateValue"), Literal)
        Dim ltlTicketingFeeRemoved As Literal = CType(e.FindControl("ltlTicketingFeeRemoved"), Literal)
        Dim plhTicketingFeeInformation As PlaceHolder = CType(e.FindControl("plhTicketingFeeInformation"), PlaceHolder)

        ltlTicketingPromotionDescription1Label.Text = _wfrPage.Content("PromotionDescriptionLabel", _languageCode, True)
        ltlTicketingPromotionStartDateLabel.Text = _wfrPage.Content("PromotionStartDateLabel", _languageCode, True)
        ltlTicketingPromotionEndDateLabel.Text = _wfrPage.Content("PromotionEndDateLabel", _languageCode, True)
        ltlTicketingPromotionDescription1Value.Text = TEBUtilities.CheckForDBNull_String(promotionTable.Rows(0)("ShortDescription"))
        ltlTicketingPromotionDescription2Value.Text = TEBUtilities.CheckForDBNull_String(promotionTable.Rows(0)("Description1")) & TEBUtilities.CheckForDBNull_String(promotionTable.Rows(0)("Description2"))
        ltlTicketingPromotionStartDateValue.Text = TEBUtilities.CheckForDBNull_String(promotionTable.Rows(0)("StartDate"))
        ltlTicketingPromotionEndDateValue.Text = TEBUtilities.CheckForDBNull_String(promotionTable.Rows(0)("EndDate"))
        If Not String.IsNullOrWhiteSpace(TEBUtilities.CheckForDBNull_String(promotionTable.Rows(0)("FeesRemoved"))) Then
            ltlTicketingFeeRemoved.Text = getFeeDescription(TEBUtilities.CheckForDBNull_String(promotionTable.Rows(0)("FeesRemoved")))
        Else
            plhTicketingFeeInformation.Visible = False
        End If
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Bind the repeater of promotions and prices
    ''' </summary>
    ''' <returns><c>True</c> if the promotions have been set correctly otherwise <c>False</c></returns>
    ''' <remarks></remarks>
    Private Function bindRepeater() As Boolean
        Dim hasPromotions As Boolean = False
        Try
            Dim promotionList As New Generic.List(Of String)
            If Request.QueryString("promotionid1") IsNot Nothing Then promotionList.Add(TEBUtilities.CheckForDBNull_String(Request.QueryString("promotionid1")))
            If Request.QueryString("promotionid2") IsNot Nothing Then promotionList.Add(TEBUtilities.CheckForDBNull_String(Request.QueryString("promotionid2")))
            If Request.QueryString("promotionid3") IsNot Nothing Then promotionList.Add(TEBUtilities.CheckForDBNull_String(Request.QueryString("promotionid3")))
            If promotionList.Count > 0 Then
                rptTicketingPromotionDetails.DataSource = promotionList
                rptTicketingPromotionDetails.DataBind()

                If _originalPrice > 0 Then
                    ltlTicketingPromotionOriginalPriceLabel.Text = _wfrPage.Content("PromotionOriginalPriceLabel", _languageCode, True)
                    ltlTicketingPromotionNewPriceLabel.Text = _wfrPage.Content("PromotionNewPriceLabel", _languageCode, True)
                    ltlTicketingPromotionDiscountPriceLabel.Text = _wfrPage.Content("PromotionDiscountPriceLabel", _languageCode, True)
                    ltlTicketingPromotionOriginalPriceValue.Text = TDataObjects.PaymentSettings.FormatCurrency(_originalPrice, _wfrPage.BusinessUnit, _wfrPage.PartnerCode)
                    ltlTicketingPromotionNewPriceValue.Text = TDataObjects.PaymentSettings.FormatCurrency(_salePrice, _wfrPage.BusinessUnit, _wfrPage.PartnerCode)
                    ltlTicketingPromotionDiscountPriceValue.Text = TDataObjects.PaymentSettings.FormatCurrency(_originalPrice - _salePrice, _wfrPage.BusinessUnit, _wfrPage.PartnerCode)
                Else
                    plhTicketingPriceInformation.Visible = False
                End If
                hasPromotions = True
            End If
        Catch ex As Exception
            hasPromotions = False
        End Try
        Return hasPromotions
    End Function

    ''' <summary>
    ''' Try to call TALENT and bind the promotions to the page
    ''' </summary>
    ''' <returns>a boolean value to indicate whether or not promotions have been set</returns>
    ''' <remarks></remarks>
    Private Function bindPromotions(ByVal promotionId As String, ByRef e As RepeaterItem) As Boolean
        Dim hasPromotions As Boolean = False
        Dim promotions As New TalentPromotions
        Dim promoSettings As New DEPromotionSettings
        promoSettings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        promoSettings.BusinessUnit = TalentCache.GetBusinessUnit()
        promoSettings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
        If Profile.IsAnonymous Then
            promoSettings.AccountNo1 = TCUtilities.PadLeadingZeros(GlobalConstants.GENERIC_CUSTOMER_NUMBER, 12)
        Else
            promoSettings.AccountNo1 = TCUtilities.PadLeadingZeros(Profile.User.Details.Account_No_1, 12)
        End If
        promoSettings.Cacheing = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_wfrPage.Attribute("Caching"))
        promoSettings.CacheTimeMinutes = TEBUtilities.CheckForDBNull_Int(_wfrPage.Attribute("CacheTimeInMins"))
        promoSettings.CacheDependencyPath = ModuleDefaults.CacheDependencyPath
        promoSettings.OriginatingSource = TEBUtilities.GetOriginatingSource(Session.Item("Agent"))
        promoSettings.IncludeProductPurchasers = String.Empty

        Dim promotionDataEntity As New DEPromotions
        Dim err As New ErrorObj
        promotionDataEntity.PromotionId = promotionId
        promotions.Dep = promotionDataEntity
        promotions.Settings = promoSettings
        err = promotions.GetPromotionDetails()

        If Not err.HasError AndAlso promotions.ResultDataSet IsNot Nothing AndAlso promotions.ResultDataSet.Tables.Count > 0 Then
            If promotions.ResultDataSet.Tables(0).Rows.Count > 0 Then
                setPageText(promotions.ResultDataSet.Tables(0), e)
                hasPromotions = True
            End If
        End If
        Return hasPromotions
    End Function

    ''' <summary>
    ''' Retreives the text description against the fee code that is passed in
    ''' </summary>
    ''' <param name="feeCode">The fee code as a string</param>
    ''' <returns>The fee description</returns>
    ''' <remarks></remarks>
    Private Function getFeeDescription(ByVal feeCode As String) As String
        Dim feeDescription As String = String.Empty
        Select Case feeCode
            Case Is = GlobalConstants.BKFEE : feeDescription = _wfrPage.Content("BKFEERemoved", _languageCode, True)
            Case Is = GlobalConstants.WSFEE : feeDescription = _wfrPage.Content("WSFEERemoved", _languageCode, True)
            Case Is = GlobalConstants.DDFEE : feeDescription = _wfrPage.Content("DDFEERemoved", _languageCode, True)
            Case Is = GlobalConstants.CRFEE : feeDescription = _wfrPage.Content("CRFEERemoved", _languageCode, True)
            Case Is = GlobalConstants.ALLFEES : feeDescription = _wfrPage.Content("ALLFEESRemoved", _languageCode, True)
            Case Else : feeDescription = _wfrPage.Content("UnknownFeeRemoved", _languageCode, True)
        End Select
        Return feeDescription
    End Function

    ''' <summary>
    ''' Set any merchandise promotion details
    ''' </summary>
    ''' <returns><c>True</c> if the retail promotions are set otherwise <c>False</c></returns>
    ''' <remarks></remarks>
    Private Function handleMerchandisePromotions() As Boolean
        Dim hasMerchandisePromotion As Boolean = False
        blMerchandisePromotions.Items.Clear()
        If ModuleDefaults.PricingType <> 2 Then
            Dim promoResults As Data.DataTable
            Dim talentWebPricing As TalentWebPricing = Nothing
            If (Profile.Basket.WebPrices IsNot Nothing) Then
                talentWebPricing = Profile.Basket.WebPrices
            End If
            If (talentWebPricing Is Nothing) Then
                promoResults = New Data.DataTable
            Else
                If Not talentWebPricing.PromotionsResultsTable Is Nothing Then
                    promoResults = talentWebPricing.PromotionsResultsTable
                Else
                    promoResults = New Data.DataTable
                End If
            End If

            If Not promoResults Is Nothing AndAlso promoResults.Rows.Count > 0 Then
                For Each result As Data.DataRow In promoResults.Rows
                    If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(result("Success")) Then
                        Dim sPromoDisplay As String = "<<PromotionDisplayName>> ( x <<ApplicationCount>>)"
                        If result("ActivationMechanism") = DBPromotions.code Then
                            Dim sPromoDisplayCode As String = TEBUtilities.CheckForDBNull_String(_wfrPage.Content("PromotionsSummaryDisplayCode", _languageCode, True))
                            If sPromoDisplayCode <> String.Empty Then
                                sPromoDisplay = sPromoDisplayCode
                            End If
                        ElseIf result("ActivationMechanism") = DBPromotions.auto Then
                            Dim sPromoDisplayAuto As String = TEBUtilities.CheckForDBNull_String(_wfrPage.Content("PromotionsSummaryDisplayAuto", _languageCode, True))
                            If sPromoDisplayAuto <> String.Empty Then
                                sPromoDisplay = sPromoDisplayAuto
                            End If
                        End If
                        blMerchandisePromotions.Items.Add(sPromoDisplay.Replace("<<PromotionDisplayName>>", result("PromotionDisplayName")).Replace("<<ApplicationCount>>", result("ApplicationCount")).Replace("<<PromotionCode>>", result("PromotionCode")))
                        hasMerchandisePromotion = True
                    End If
                Next
            End If
        End If
        Return hasMerchandisePromotion
    End Function

#End Region

End Class