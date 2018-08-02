Imports System.Data
Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities

Partial Class UserControls_FavouriteSeatSelection
    Inherits ControlBase

#Region "Class Level Fields"

    Private _ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _productCode As String = String.Empty
    Private _productPriceBand As String = String.Empty
    Private _campaignCode As String = String.Empty
    Private _productHomeAsAway As String = String.Empty
    Private _productType As String = String.Empty
    Private _productSubType As String = String.Empty
    Private _productStadium As String = String.Empty
    Private _canStadiumUseFavouriteSeat As Boolean = False
    Private _errMsg As Talent.Common.TalentErrorMessages

#End Region

#Region "Public Properties"

    Public Property ProductCode() As String
        Get
            Return _productCode
        End Get
        Set(ByVal value As String)
            _productCode = value
        End Set
    End Property
    Public Property ProductPriceBand() As String
        Get
            Return _productPriceBand
        End Get
        Set(ByVal value As String)
            _productPriceBand = value
        End Set
    End Property
    Public Property CampaignCode() As String
        Get
            Return _campaignCode
        End Get
        Set(ByVal value As String)
            _campaignCode = value
        End Set
    End Property
    Public Property ProductHomeAsAway() As String
        Get
            Return _productHomeAsAway
        End Get
        Set(ByVal value As String)
            _productHomeAsAway = value
        End Set
    End Property
    Public Property ProductType() As String
        Get
            Return _productType
        End Get
        Set(ByVal value As String)
            _productType = value
        End Set
    End Property
    Public Property ProductSubType() As String
        Get
            Return _productSubType
        End Get
        Set(ByVal value As String)
            _productSubType = value
        End Set
    End Property
    Public Property ProductStadium() As String
        Get
            Return _productStadium
        End Get
        Set(ByVal value As String)
            _productStadium = value
        End Set
    End Property
    Public Property CanStadiumUseFavouriteSeat() As Boolean
        Get
            Return _canStadiumUseFavouriteSeat
        End Get
        Set(ByVal value As Boolean)
            _canStadiumUseFavouriteSeat = value
        End Set
    End Property
    Public Property CSSClassName As String
    Public Property ProductDescription() As String
#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "FavouriteSeatSelection.ascx"
        End With
        _errMsg = New Talent.Common.TalentErrorMessages(_languageCode, _ucr.BusinessUnit, TalentCache.GetPartner(Profile), _ucr.FrontEndConnectionString)
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        hfProductCode.Value = ProductCode
        hfProductPriceBand.Value = ProductPriceBand
        hfCampaignCode.Value = CampaignCode
        hfProductHomeAsAway.Value = ProductHomeAsAway
        hfProductType.Value = ProductType
        hfProductSubType.Value = ProductSubType
        hfProductStadium.Value = ProductStadium
        If Not AgentProfile.BulkSalesMode AndAlso ModuleDefaults.FavouriteSeatFunction AndAlso _productHomeAsAway <> "Y" AndAlso _canStadiumUseFavouriteSeat AndAlso (_productType = GlobalConstants.HOMEPRODUCTTYPE Or _productType = GlobalConstants.SEASONTICKETPRODUCTTYPE) Then
            If Profile.IsAnonymous Then
                Dim favouriteSeatNotAvailableText As String = _ucr.Content("FavouriteSeatFunctionNotAvailableText", _languageCode, True)
                If Not String.IsNullOrWhiteSpace(favouriteSeatNotAvailableText) Then
                    ltlFavouriteSeatNotAvailable.Text = favouriteSeatNotAvailableText
                    CSSClassName = "no-options"
                    pnlFavouriteSeat.Visible = True
                    plhFavouriteSeatAvailable.Visible = False
                    plhFavouriteSeatNotAvailable.Visible = True
                Else
                    pnlFavouriteSeat.Visible = False
                End If
            Else
                Dim favouriteSeatExistsInCurrentGame As Boolean = False
                Dim dsAvailableStands As DataSet = getAvailableStands()
                If Not dsAvailableStands Is Nothing Then
                    If dsAvailableStands.Tables.Count > 1 Then
                        If dsAvailableStands.Tables("StadiumAvailability").Rows.Count > 0 Then
                            Dim stand As String = GetStandFromSeatDetails(Profile.User.Details.Favourite_Seat)
                            Dim area As String = GetAreaFromSeatDetails(Profile.User.Details.Favourite_Seat)
                            For Each row As DataRow In dsAvailableStands.Tables("StadiumAvailability").Rows
                                If row("StandCode").Equals(stand) Then
                                    If row("AreaCode").Equals(area) Then
                                        favouriteSeatExistsInCurrentGame = True
                                        Exit For
                                    End If
                                End If
                            Next
                        End If
                    End If
                End If
                If favouriteSeatExistsInCurrentGame Then
                    pnlFavouriteSeat.Visible = True
                    Dim favouriteSeatInstructionsText As String = _ucr.Content("FavouriteSeatInstructions", _languageCode, True)
                    If Not String.IsNullOrWhiteSpace(favouriteSeatInstructionsText) Then
                        ltlFavouriteSeatInstructions.Text = favouriteSeatInstructionsText
                        plhInstructions.Visible = True
                    Else
                        plhInstructions.Visible = False
                    End If
                    ltlFavouriteSeatLegend.Text = _ucr.Content("FavouriteSeatLegend", _languageCode, True)
                    lblFavouriteSeatQuantity.Text = _ucr.Content("FavouriteSeatQuantityLabel", _languageCode, True)
                    btnFavouriteSeatBuy.Text = _ucr.Content("FavouriteSeatBuyButtonText", _languageCode, True)
                    plhFavouriteSeatAvailable.Visible = True
                    plhFavouriteSeatNotAvailable.Visible = False
                    setQuantityDefaults()
                Else
                    Dim favouriteSeatNotAvailableText As String = String.Empty
                    If String.IsNullOrWhiteSpace(Profile.User.Details.Favourite_Seat) Then
                        favouriteSeatNotAvailableText = _ucr.Content("FavouriteSeatFunctionNotAvailableText", _languageCode, True)
                    Else
                        favouriteSeatNotAvailableText = _ucr.Content("FavouriteSeatNotAvailableText", _languageCode, True)
                    End If
                    If Not String.IsNullOrWhiteSpace(favouriteSeatNotAvailableText) Then
                        ltlFavouriteSeatNotAvailable.Text = favouriteSeatNotAvailableText
                        CSSClassName = "no-options"
                        pnlFavouriteSeat.Visible = True
                        plhFavouriteSeatAvailable.Visible = False
                        plhFavouriteSeatNotAvailable.Visible = True
                    Else
                        pnlFavouriteSeat.Visible = False
                    End If
                End If
            End If
            Dim favouriteSeatHeaderText As String = _ucr.Content("FavouriteSeatHeader", _languageCode, True)
            If Not String.IsNullOrWhiteSpace(favouriteSeatHeaderText) Then
                ltlFavouriteSeatHeader.Text = favouriteSeatHeaderText
                plhHeader.Visible = True
            Else
                plhHeader.Visible = False
            End If
        Else
            Me.Visible = False
        End If
    End Sub

    Protected Sub btnFavouriteSeatBuy_OnClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnFavouriteSeatBuy.Click
        If Not Information.IsNumeric(Me.txtFavouriteSeatQuantity.Text) Or Me.txtFavouriteSeatQuantity.Text = "0" Or Me.txtFavouriteSeatQuantity.Text = "00" Then
            Dim errMessage As String = _ucr.Content("TxtQuantityRegExMessage", _languageCode, True)
            Dim blErrorMessages As BulletedList = Nothing
            blErrorMessages = Parent.FindControl("blErrorMessages")
            If blErrorMessages IsNot Nothing Then
                If blErrorMessages.Items.FindByText(errMessage) Is Nothing Then blErrorMessages.Items.Add(errMessage)
            End If
            txtFavouriteSeatQuantity.Text = String.Empty
        Else
            Session("errormsg") = ""
            Session("FavouriteSeatUsed") = True
            Dim ticketingGatewayString As New StringBuilder
            ticketingGatewayString.Append("~/Redirect/TicketingGateway.aspx?page=")
            ticketingGatewayString.Append(Talent.eCommerce.Utilities.GetCurrentPageName(False))
            ticketingGatewayString.Append("&function=AddToBasket&product=" & hfProductCode.Value.Trim)
            ticketingGatewayString.Append("&stand=" & GetStandFromSeatDetails(Profile.User.Details.Favourite_Seat).Trim)
            ticketingGatewayString.Append("&area=" & GetAreaFromSeatDetails(Profile.User.Details.Favourite_Seat).Trim)
            ticketingGatewayString.Append("&quantity=" & txtFavouriteSeatQuantity.Text.Trim)
            ticketingGatewayString.Append("&priceBand=" & hfProductPriceBand.Value)
            ticketingGatewayString.Append("&campaign=" & hfCampaignCode.Value)
            ticketingGatewayString.Append("&productIsHomeAsAway=" & hfProductHomeAsAway.Value)
            ticketingGatewayString.Append("&productsubtype=" & hfProductSubType.Value.Trim)
            ticketingGatewayString.Append("&productstadium=" & hfProductStadium.Value.Trim)
            ticketingGatewayString.Append("&type=" & hfProductType.Value.Trim)
            ticketingGatewayString.Append("&favouriteSeat=true")
            Response.Redirect(ticketingGatewayString.ToString)
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Function getAvailableStands() As DataSet
        Dim dsAvailableStands As DataSet = Nothing
        Dim settings As New DESettings
        Dim productDetails As New DEProductDetails
        Dim product As New TalentProduct
        Dim err As New ErrorObj
        _ucr.KeyCode = "StandAndAreaSelection.ascx"

        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.BusinessUnit = TalentCache.GetBusinessUnit()
        settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
        settings.Cacheing = CType(_ucr.Attribute("Cacheing"), Boolean)
        settings.CacheTimeMinutes = CType(_ucr.Attribute("CacheTimeMinutes"), Integer)
        settings.CacheTimeMinutesSecondFunction = CType(_ucr.Attribute("StandDescriptionsCacheTimeMinutes"), Integer)
        settings.CacheDependencyPath = ModuleDefaults.CacheDependencyPath
        settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
        _ucr.KeyCode = "FavouriteSeatSelection.ascx"

        productDetails.ProductCode = _productCode
        productDetails.Src = "W"
        productDetails.ProductType = ProductType
        productDetails.CampaignCode = CampaignCode
        productDetails.ProductHomeAsAway = ProductHomeAsAway
        productDetails.ComponentID = CATHelper.GetPackageComponentId(_productCode, HttpContext.Current.Request("callid"))
        Dim agent As New Agent
        If agent.IsAgent Then
            productDetails.AvailableToSell03 = agent.IsAvailableToSell03
            productDetails.AvailableToSellAvailableTickets = agent.SellAvailableTickets
        Else
            productDetails.AvailableToSell03 = True
            productDetails.AvailableToSellAvailableTickets = False
        End If
        product.Settings() = settings
        product.De = productDetails
        err = product.AvailableStands()
        dsAvailableStands = product.ResultDataSet
        Return dsAvailableStands
    End Function

    ''' <summary>
    ''' Set the quantity defaults based on CAT or quantity defintions (product relationships or Bulk Sales Mode)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setQuantityDefaults()
        If CATHelper.IsItCATRequest(-2) AndAlso Not CATHelper.IsPackageTransferRequested() Then
            txtFavouriteSeatQuantity.Text = "1"
            txtFavouriteSeatQuantity.ReadOnly = True
            txtFavouriteSeatQuantity.Attributes.Remove("onkeyup")
            rngQuantity.Enabled = False
        Else
            Dim tGatewayFunctions As New TicketingGatewayFunctions
            Dim minQuantity As Integer = 0
            Dim maxQuantity As Integer = 0
            Dim defaultQuantity As String = String.Empty
            Dim isReadOnly As Boolean = False
            tGatewayFunctions.GetQuantityDefintions(ProductCode, minQuantity, maxQuantity, defaultQuantity, isReadOnly)
            txtFavouriteSeatQuantity.Attributes.Add("min", minQuantity)
            txtFavouriteSeatQuantity.Attributes.Add("max", maxQuantity)
            txtFavouriteSeatQuantity.MaxLength = maxQuantity.ToString().Length
            rngQuantity.Enabled = True
            rngQuantity.MinimumValue = minQuantity
            rngQuantity.MaximumValue = maxQuantity
            rngQuantity.Type = ValidationDataType.Integer
            rfvQuantity.ErrorMessage = _errMsg.GetErrorMessage("QZ5").ERROR_MESSAGE
            If minQuantity = maxQuantity Then
                rngQuantity.ErrorMessage = _errMsg.GetErrorMessage("QZ4").ERROR_MESSAGE.Replace("<<MAX_VALUE>>", maxQuantity)
            Else
                rngQuantity.ErrorMessage = _errMsg.GetErrorMessage("QZ3").ERROR_MESSAGE.Replace("<<MIN_VALUE>>", minQuantity).Replace("<<MAX_VALUE>>", maxQuantity)
            End If
            rngQuantity.ErrorMessage = rngQuantity.ErrorMessage.Replace("<<PRODUCT_DESCRIPTION>>", ProductDescription)

        End If
    End Sub
#End Region

End Class
