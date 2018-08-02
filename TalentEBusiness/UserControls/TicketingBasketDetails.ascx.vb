Imports System.Data.SqlClient
Imports System.Data
Imports System.Collections.Generic
Imports System.Globalization
Imports Talent.eCommerce
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports TCUtilities = Talent.Common.Utilities
Imports Talent.Common.UtilityExtension

Partial Class UserControls_TicketingBasketDetails
    Inherits ControlBase

#Region "Class Level Fields"

    Private _ucr As New Talent.Common.UserControlResource
    Private _errMsg As Talent.Common.TalentErrorMessages
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _displayCustomerSearchLink As Boolean = False
    Private _errorList As New BulletedList
    Private _isCoursePdt As Boolean = False
    Private _newParticipantsLabelText As String = String.Empty
    Private _dicParticipantMembers As Dictionary(Of String, String)
    Private _dicProductDetailsByPriceCode As New Dictionary(Of String, DataSet)
    Private _showBasketOnCheckoutPage As Boolean = False
    Private _fulfilmentList As List(Of KeyValuePair(Of String, String)) = Nothing
    Private _talentCustomer As Talent.Common.TalentCustomer = Nothing
    Private _talentErrObj As New Talent.Common.ErrorObj
    Private _deCustomer As New Talent.Common.DECustomer
    Private _deSettings As New Talent.Common.DESettings
    Private _prodTypeMTTP As String = String.Empty
    Private _addJSSelectAttributes As Boolean = False
    Private _displayItemsInCancBasket As Boolean = False

#End Region

#Region "Public Properties"

    Public Property Usage() As String
    Public Property PageCode() As String
    Public Property SelectProductType() As String
    Public Property SelectProductSubType() As String
    Public Property DisplayTicketHeader() As Boolean
    Public Property CurrentProductDetailsResultSet() As DataSet
    Public Property NewCustomerConfirmation() As String
    Public Property NewCustomerOptionText() As String
    Public Property FriendsAndFamilyRegOption() As String
    Public seatHeaderText As String
    
    Public ReadOnly Property Event_Repeater() As Repeater
        Get
            Return EventRepeater
        End Get
    End Property
    Public ReadOnly Property ShowBasketOnCheckoutPage As Boolean
        Get
            Return _showBasketOnCheckoutPage
        End Get
    End Property
    Public Property ErrorListForRepeater As New Dictionary(Of String, TalentBasketItem)

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.Common.Utilities.GetAllString
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "TicketingBasketDetails.ascx"
        End With
        _errMsg = New Talent.Common.TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
        PageCode = _ucr.PageCode
        setOrderLevelFulfilment()
        hdfHighlightUpdateBasketText.Value = Utilities.CheckForDBNull_String(_ucr.Content("UpdateBasketPromptText", _languageCode, True).Trim)
        hdfUpdateBasketPromptDuration.Value = Utilities.CheckForDBNull_Int(_ucr.Attribute("UpdateBasketPromptDuration").Trim)
        hdfShowUpdateBasketPrompt.Value = Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowUpdateBasketPrompt").Trim)
        hdfHighlightUpdateBasketButtonWhenRequired.Value = Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("HighlightUpdateBasketButtonWhenRequired").Trim)
        hdfIsAgent.Value = AgentProfile.IsAgent
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Handle error codes on the basket items
        HandleBasketErrors()
        _newParticipantsLabelText = _ucr.Content("NewParticipantsLabelText", _languageCode, True)
        NewCustomerConfirmation = _ucr.Content("NewCustomerConfirmationText", _languageCode, True)
        NewCustomerOptionText = _ucr.Content("NewCustomerOptionText", _languageCode, True)
        FriendsAndFamilyRegOption = _ucr.Content("FriendsAndFamilyRegOptionText", _languageCode, True)
        If Session("DicParticipantMembers") IsNot Nothing Then
            _dicParticipantMembers = CType(Session("DicParticipantMembers"), Dictionary(Of String, String))
            Session("DicParticipantMembers") = Nothing
            Session.Remove("DicParticipantMembers")
        End If

        _dicProductDetailsByPriceCode.Clear()
        '
        ' In a cancelation basket, there are exceptions where we need to include additional items.
        ' DDRefund products for example.
        '
        If Profile.Basket.CAT_MODE = GlobalConstants.CATMODE_CANCEL Or Profile.Basket.CAT_MODE = GlobalConstants.CATMODE_CANCELALL Then
            _displayItemsInCancBasket = checkVisibleBasketItemsForCATBasket()
        End If
        If Profile.Basket.CAT_MODE <> GlobalConstants.CATMODE_CANCEL Or _displayItemsInCancBasket Then
            Select Case Profile.Basket.BasketContentType
                Case "C", "T"
                    If Usage.ToUpper = "PAYMENT" AndAlso Not Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("ShowBasketContentsOnPaymentScreen")) Then
                        Me.Visible = False
                    Else
                        _showBasketOnCheckoutPage = True
                        BindEventRepeater()
                        If ModuleDefaults.DISPLAY_FREE_TICKETING_ITEMS Then BindFreeItemsRepeater()
                        If DisplayTicketHeader Then

                            If EventRepeater.Items.Count > 0 Then
                                If Not String.IsNullOrEmpty(SelectProductType) AndAlso SelectProductType.Equals("S") Then
                                    ltlTicketingBasketHeaderLabel.Text = _ucr.Content("SeasonTicketSummaryHeaderLabel", _languageCode, True)
                                Else
                                    ltlTicketingBasketHeaderLabel.Text = _ucr.Content("TicketingBasketHeaderLabel", _languageCode, True)
                                End If
                            Else
                                ltlTicketingBasketHeaderLabel.Visible = False
                            End If
                        Else
                            ltlTicketingBasketHeaderLabel.Visible = False
                        End If

                        'Display the multi stadium text where applicable
                        If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowMultiStadiumMainTextForBasket")) Then
                            ltlMultiStadiumMainTextLabel.Text = _ucr.Content("MultiStadiumMainTextForBasket", _languageCode, True)
                            plhMultiStadiumText.Visible = True
                        Else
                            plhMultiStadiumText.Visible = False
                        End If
                    End If
                    CustomerSearchLink()
                Case Else
                    Me.Visible = False
            End Select
        Else
            Me.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' If we are in a CAT_CANCEL basket, there are certain times we do need to display basket items.
    ''' Direct Debit Refund Products are one example.
    ''' Loop through the basket for items marked with ***** = true
    ''' </summary>
    Private Function checkVisibleBasketItemsForCATBasket() As Boolean
        For Each item As TalentBasketItem In Profile.Basket.BasketItems
            If item.DISPLAY_IN_A_CANCEL_BASKET Then
                Return True
            End If
        Next
        Return False
    End Function

    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        Select Case sender.ID
            Case Is = "ltlCustomerHeaderLabel"
                CType(sender, Literal).Text = _ucr.Content("CustomerHeaderLabel", _languageCode, True)
            Case Is = "ltlPriceCodeHeaderLabel"
                CType(sender, Literal).Text = _ucr.Content("PriceCodeHeaderLabel", _languageCode, True)
            Case Is = "ltlBandHeaderLabel"
                CType(sender, Literal).Text = _ucr.Content("BandHeaderLabel", _languageCode, True)
            Case Is = "ltlPriceHeaderLabel"
                CType(sender, Literal).Text = _ucr.Content("ltlPriceHeaderLabel", _languageCode, True)
            Case Is = "ltlFulfilmentHeaderLabel"
                CType(sender, Literal).Text = _ucr.Content("FulfilmentHeaderLabel", _languageCode, True)
            Case Is = "ltlSeatHeaderLabel"
                Dim ltlSeatHeaderLabel As Literal = CType(sender, Literal)
                Dim seatsRepeater As Repeater = CType(ltlSeatHeaderLabel.BindingContainer.Parent, Repeater)
                Dim TravelProductLabel As Literal = CType(seatsRepeater.Items(0).FindControl("TravelProductLabel"), Literal)
                If TravelProductLabel.Visible Then
                    CType(sender, Literal).Text = _ucr.Content("TravelProductSeatHeaderText", _languageCode, True)
                    seatHeaderText = _ucr.Content("TravelProductSeatHeaderText", _languageCode, True)
                Else
                    CType(sender, Literal).Text = _ucr.Content("SeatHeaderLabel", _languageCode, True)
                    seatHeaderText = _ucr.Content("SeatHeaderLabel", _languageCode, True)
                End If
            Case Is = "RemoveButton"
                CType(sender, Button).Text = _ucr.Content("RemoveButton", _languageCode, True)
            Case Is = "ltlFreeItemsDescriptionHeader"
                If FreeItemsRepeater.Visible Then
                    CType(sender, Literal).Text = _ucr.Content("FreeTicketingItemsDescriptionText", _languageCode, True)
                End If
            Case Is = "ltlFreeItemsDescriptionHeader"
                CType(sender, Literal).Text = _ucr.Content("FreeItemsDescriptionHeaderText", _languageCode, True)
            Case Is = "ltlFreeItemsPriceBandHeader"
                CType(sender, Literal).Text = _ucr.Content("FreeItemsPriceBandHeaderText", _languageCode, True)
            Case Is = "ltlFreeItemsMemberNumberHeader"
                CType(sender, Literal).Text = _ucr.Content("FreeItemsMemberNumberHeaderText", _languageCode, True)
            Case Is = "ltlFreeItemsSeatDetailsHeader"
                CType(sender, Literal).Text = _ucr.Content("FreeItemsSeatDetailsHeaderText", _languageCode, True)
            Case Is = "ltlPackageQuantityHeaderLabel"
                CType(sender, Literal).Text = _ucr.Content("PackageQuantityHeaderLabel", _languageCode, True)
            Case Is = "ltlNetPriceHeaderLabel"
                CType(sender, Literal).Text = _ucr.Content("netPriceHeaderLabel", _languageCode, True)
        End Select
    End Sub

    Protected Sub EventRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles EventRepeater.ItemDataBound
        Dim eventImage As Image = CType(e.Item.FindControl("EventImage"), Image)
        Dim ltlDescription1 As Literal = CType(e.Item.FindControl("ltlDescription1"), Literal)
        Dim ltlDescription2 As Literal = CType(e.Item.FindControl("ltlDescription2"), Literal)
        Dim ltlDescription3 As Literal = CType(e.Item.FindControl("ltlDescription3"), Literal)
        Dim ltlDescription4 As Literal = CType(e.Item.FindControl("ltlDescription4"), Literal)
        Dim ltlDescription5 As Literal = CType(e.Item.FindControl("ltlDescription5"), Literal)
        Dim ltlSpecificContent1 As Literal = CType(e.Item.FindControl("ltlSpecificContent1"), Literal)
        Dim standRepeater As Repeater = CType(e.Item.FindControl("StandRepeater"), Repeater)
        Dim moreTicketsLink As HyperLink = CType(e.Item.FindControl("MoreTicketsLink"), HyperLink)
        Dim plhSeasonTicketExceptionsLink As PlaceHolder = CType(e.Item.FindControl("plhSeasonTicketExceptionsLink"), PlaceHolder)
        Dim hplSeasonTicketExceptions As HyperLink = CType(e.Item.FindControl("hplSeasonTicketExceptions"), HyperLink)
        Dim lnkEditParticipantsLink As HyperLink = CType(e.Item.FindControl("lnkEditParticipantsLink"), HyperLink)
        Dim divEventRepeater As HtmlControl = CType(e.Item.FindControl("divEventRepeater"), HtmlControl)
        Dim plhFulfilment As PlaceHolder = CType(e.Item.FindControl("plhFulfilment"), PlaceHolder)
        Dim dr As DataRow = CType(e.Item.DataItem, DataRowView).Row
        Dim isPackageIDExists As Boolean = False
        Dim currentFulFilment As String = String.Empty
        Dim fulfilmentOption As Boolean = True

        lnkEditParticipantsLink.Enabled = False
        lnkEditParticipantsLink.Visible = False
        plhSeasonTicketExceptionsLink.Visible = False
        moreTicketsLink.Text = _ucr.Content("MoreTicketsLinkText", _languageCode, True)
        lnkEditParticipantsLink.Text = _ucr.Content("EditParticipantsLink", _languageCode, True)

        If Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(dr("PRODUCT"))) Then
            If ModuleDefaults.TicketingKioskMode Then
                fulfilmentOption = False
            End If
            If String.Compare(Profile.Basket.USER_SELECT_FULFIL, "Y", True) <> 0 Then
                fulfilmentOption = False
            End If
            If TEBUtilities.IsAnonymousTalent Then
                fulfilmentOption = False
            End If
            If Profile.Basket.CAT_MODE = GlobalConstants.CATMODE_CANCEL Or Profile.Basket.CAT_MODE = GlobalConstants.CATMODE_CANCELALL Then
                fulfilmentOption = False
            End If
            Select Case UCase(Usage)
                Case Is = "ORDER", "PAYMENT" : fulfilmentOption = False
            End Select
            _fulfilmentList = New List(Of KeyValuePair(Of String, String))
            'Create the basket data table
            Dim dt As New DataTable
            With dt.Columns
                .Add("PRODUCT", GetType(String))
                .Add("PRODUCT_DESCRIPTION6", GetType(String))
                .Add("PRODUCT_DESCRIPTION7", GetType(String))
                .Add("PACKAGE_ID", GetType(Decimal))
            End With

            Dim dRow As DataRow = Nothing
            Dim sStand As String = "*NONE"
            For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
                If tbi.Product.Trim = dr("PRODUCT").ToString.Trim Then
                    isPackageIDExists = Utilities.CheckForDBNull_Decimal(tbi.PACKAGE_ID) > 0
                    eventImage.ImageUrl = ImagePath.getImagePath("PRODTICKETING", Utilities.CheckForDBNull_String(tbi.PRODUCT_DESCRIPTION3).Trim(), TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                    eventImage.Visible = (eventImage.ImageUrl <> ModuleDefaults.MissingImagePath)
                    ltlDescription1.Text = Utilities.CheckForDBNull_String(tbi.PRODUCT_DESCRIPTION1).Trim()
                    ltlDescription2.Text = Utilities.CheckForDBNull_String(tbi.PRODUCT_DESCRIPTION2).Trim()
                    OutputProductSpecificErrors(tbi)

                    ' Bundle products may have date column stored as a range.
                    Dim bundleDateRange As String() = Utilities.CheckForDBNull_String(tbi.PRODUCT_DESCRIPTION4).Trim().Split("-")

                    If bundleDateRange.Length > 1 Then
                        If ModuleDefaults.ShowBundleDateAsRange Then
                            ltlDescription4.Text = FormatDate(bundleDateRange(0)) + " - " + FormatDate(bundleDateRange(1))
                        Else
                            ltlDescription4.Text = FormatDate(bundleDateRange(0))
                            ltlDescription5.Text = _ucr.Content("KickOffText", _languageCode, True) & Utilities.CheckForDBNull_String(tbi.PRODUCT_DESCRIPTION5)
                        End If
                    Else
                        ltlDescription4.Text = FormatDate(Utilities.CheckForDBNull_String(tbi.PRODUCT_DESCRIPTION4).Trim())
                        If Utilities.CheckForDBNull_String(tbi.PRODUCT_DESCRIPTION5) = String.Empty Then
                            ltlDescription5.Text = String.Empty
                        Else
                            ltlDescription5.Text = _ucr.Content("KickOffText", _languageCode, True) & Utilities.CheckForDBNull_String(tbi.PRODUCT_DESCRIPTION5)
                        End If
                    End If

                    'Add Product specific content from tbl_product_specific_content
                    Dim dtSpecificContent As DataTable = TDataObjects.ProductsSettings.TblProductSpecificContent.GetProductContent("Basket", tbi.Product)
                    If dtSpecificContent.Rows.Count > 0 Then
                        ltlSpecificContent1.Text = dtSpecificContent.Rows(0).Item("Product_Content").ToString
                    End If
                    'Get the Product Type
                    Try

                        ' Set up the calls to add items to ticketing basket
                        Dim _talentErrObj As New Talent.Common.ErrorObj
                        Dim _talentProduct As New Talent.Common.TalentProduct
                        Dim _deSettings As Talent.Common.DESettings = TEBUtilities.GetSettingsObject()
                        _deSettings.Cacheing = True
                        _deSettings.CacheTimeMinutes = CInt(_ucr.Attribute("CacheTimeMinutes"))
                        _deSettings.CacheDependencyPath = ModuleDefaults.CacheDependencyPath
                        _talentProduct.Settings() = _deSettings
                        _talentProduct.De.ProductCode = Utilities.CheckForDBNull_String(dr("PRODUCT"))
                        _talentProduct.De.ProductType = tbi.PRODUCT_TYPE_ACTUAL
                        ' For away-type products set AllowPriceException to True as this forces WS007R to return all price codes for the product regardless of web-ready flag on SD006 (for BUI only!)
                        If AgentProfile.IsAgent And tbi.PRODUCT_TYPE_ACTUAL = GlobalConstants.AWAYPRODUCTTYPE Then
                            _talentProduct.De.AllowPriceException = True
                        End If
                        _talentProduct.De.Src = "W"
                        _talentProduct.De.PriceCode = tbi.PRICE_CODE
                        _talentErrObj = _talentProduct.ProductDetails
                        CurrentProductDetailsResultSet = _talentProduct.ResultDataSet

                        If Not _talentErrObj.HasError Then
                            If Not _dicProductDetailsByPriceCode.ContainsKey(tbi.Product & tbi.PRICE_CODE) Then
                                _dicProductDetailsByPriceCode.Add(tbi.Product & tbi.PRICE_CODE, _talentProduct.ResultDataSet)
                            End If

                            Try
                                Dim tbl1 As DataTable = CurrentProductDetailsResultSet.Tables(2)
                                Dim rw As DataRow = tbl1.Rows(0)
                                Dim tempProductSubType As String = String.Empty
                                Select Case Utilities.CheckForDBNull_String(rw("ProductType"))
                                    Case Is = "H"
                                        If isPackageIDExists Then
                                            moreTicketsLink.NavigateUrl = "~/PagesPublic/ProductBrowse/MatchDayHospitality.aspx"
                                            divEventRepeater.Attributes("class") = "panel ebiz-ticketing-products ebiz-type-" & Utilities.CheckForDBNull_String(rw("ProductType")) & " ebiz-code-" & Utilities.CheckForDBNull_String(rw("ProductCode"))
                                        Else
                                            tempProductSubType = Utilities.CheckForDBNull_String(rw("ProductSubType")).Trim
                                            _isCoursePdt = Utilities.IsCourseProduct(Utilities.CheckForDBNull_String(rw("ProductStadium")))
                                            If _isCoursePdt Then
                                                lnkEditParticipantsLink.Enabled = True
                                                lnkEditParticipantsLink.Visible = True
                                                lnkEditParticipantsLink.NavigateUrl = "~/PagesPublic/Profile/RegistrationParticipants.aspx?productSubType=" & tempProductSubType & "&ProductCode=" & Utilities.CheckForDBNull_String(dr("PRODUCT")) & "&productStadium=" & Utilities.CheckForDBNull_String(rw("ProductStadium")) & "&productType=" & Utilities.CheckForDBNull_String(rw("ProductType")) & "&seat=" & tbi.SEAT
                                            End If
                                            If (tempProductSubType.Length > 0) Then
                                                moreTicketsLink.NavigateUrl = "~/PagesPublic/ProductBrowse/ProductHome.aspx?productSubType=" & tempProductSubType
                                            Else
                                                moreTicketsLink.NavigateUrl = "~/PagesPublic/ProductBrowse/ProductHome.aspx"
                                            End If
                                            divEventRepeater.Attributes("class") = "panel ebiz-ticketing-products ebiz-type-" & Utilities.CheckForDBNull_String(rw("ProductType")) & " ebiz-code-" & Utilities.CheckForDBNull_String(rw("ProductCode"))
                                        End If
                                    Case Is = "A"
                                        moreTicketsLink.NavigateUrl = "~/PagesPublic/ProductBrowse/ProductAway.aspx"
                                        divEventRepeater.Attributes("class") = "panel ebiz-ticketing-products ebiz-type-" & Utilities.CheckForDBNull_String(rw("ProductType")) & " ebiz-code-" & Utilities.CheckForDBNull_String(rw("ProductCode"))
                                    Case Is = "C"
                                        ltlDescription5.Text = String.Empty
                                        If tbi.PRODUCT_TYPE = "P" Then
                                            moreTicketsLink.NavigateUrl = "~/PagesPublic/ProductBrowse/TicketingPrePayments.aspx?ppspage=1"
                                            divEventRepeater.Attributes("class") = "panel ebiz-ticketing-products ebiz-type-" & Utilities.CheckForDBNull_String(rw("ProductType")) & " ebiz-code-" & Utilities.CheckForDBNull_String(rw("ProductCode"))
                                            moreTicketsLink.Text = _ucr.Content("ChangeSchemesLinkText", _languageCode, True)
                                        Else
                                            If Not String.IsNullOrWhiteSpace(ModuleDefaults.EPurseTopUpProductCode) AndAlso tbi.Product = ModuleDefaults.EPurseTopUpProductCode Then
                                                CType(e.Item.FindControl("plhMoreTickets"), PlaceHolder).Visible = False
                                                CType(e.Item.FindControl("phEditParticipantsLink"), PlaceHolder).Visible = False
                                            Else
                                                tempProductSubType = Utilities.CheckForDBNull_String(rw("ProductSubType"))
                                                If (tempProductSubType.Length > 0) Then
                                                    moreTicketsLink.NavigateUrl = "~/PagesPublic/ProductBrowse/ProductMembership.aspx?productSubType=" & tempProductSubType
                                                Else
                                                    moreTicketsLink.NavigateUrl = "~/PagesPublic/ProductBrowse/ProductMembership.aspx"
                                                End If
                                            End If
                                            divEventRepeater.Attributes("class") = "panel ebiz-ticketing-products ebiz-type-" & Utilities.CheckForDBNull_String(rw("ProductType")) & " ebiz-code-" & Utilities.CheckForDBNull_String(rw("ProductCode"))
                                        End If
                                    Case Is = "T"
                                        Session("TravelProductFoundInBasket") = "Y"
                                        tempProductSubType = Utilities.CheckForDBNull_String(rw("ProductSubType"))
                                        If (tempProductSubType.Length > 0) Then
                                            moreTicketsLink.NavigateUrl = "~/PagesPublic/ProductBrowse/ProductTravel.aspx?productSubType=" & tempProductSubType
                                        Else
                                            moreTicketsLink.NavigateUrl = "~/PagesPublic/ProductBrowse/ProductTravel.aspx"
                                        End If
                                        divEventRepeater.Attributes("class") = "panel ebiz-ticketing-products ebiz-type-" & Utilities.CheckForDBNull_String(rw("ProductType")) & " ebiz-code-" & Utilities.CheckForDBNull_String(rw("ProductCode"))
                                    Case Is = "E"
                                        Session("EventProductFoundInBasket") = "Y"
                                        tempProductSubType = Utilities.CheckForDBNull_String(rw("ProductSubType")).Trim
                                        _isCoursePdt = Utilities.IsCourseProduct(Utilities.CheckForDBNull_String(rw("ProductStadium")))
                                        If _isCoursePdt Then
                                            lnkEditParticipantsLink.Enabled = True
                                            lnkEditParticipantsLink.Visible = True
                                            lnkEditParticipantsLink.NavigateUrl = "~/PagesPublic/Profile/RegistrationParticipants.aspx?productSubType=" & tempProductSubType & "&ProductCode=" & Utilities.CheckForDBNull_String(dr("PRODUCT")) & "&productStadium=" & Utilities.CheckForDBNull_String(rw("ProductStadium")) & "&productType=" & Utilities.CheckForDBNull_String(rw("ProductType"))
                                        End If
                                        If (tempProductSubType.Length > 0) Then
                                            moreTicketsLink.NavigateUrl = "~/PagesPublic/ProductBrowse/ProductEvent.aspx?productSubType=" & tempProductSubType
                                        Else
                                            moreTicketsLink.NavigateUrl = "~/PagesPublic/ProductBrowse/ProductEvent.aspx"
                                        End If
                                        divEventRepeater.Attributes("class") = "panel ebiz-ticketing-products ebiz-type-" & Utilities.CheckForDBNull_String(rw("ProductType")) & " ebiz-code-" & Utilities.CheckForDBNull_String(rw("ProductCode"))
                                    Case Is = "S"
                                        If isPackageIDExists Then
                                            moreTicketsLink.NavigateUrl = "~/PagesPublic/ProductBrowse/MatchDayHospitality.aspx"
                                            divEventRepeater.Attributes("class") = "panel ebiz-ticketing-products ebiz-type-" & Utilities.CheckForDBNull_String(rw("ProductType")) & " ebiz-code-" & Utilities.CheckForDBNull_String(rw("ProductCode"))
                                        Else
                                            If tbi.PRODUCT_TYPE <> "T" Then fulfilmentOption = False
                                            If tbi.RESERVED_SEAT = "Y" Then
                                                moreTicketsLink.NavigateUrl = "~/Redirect/TicketingGateway.aspx?page=home.aspx&function=AddSeasonTicketRenewalsToBasket"
                                                moreTicketsLink.Text = _ucr.Content("ReAddSeasonTicketsLinkText", _languageCode, True)
                                                divEventRepeater.Attributes("class") = "panel ebiz-ticketing-products ebiz-type-" & Utilities.CheckForDBNull_String(rw("ProductType")) & " ebiz-code-" & Utilities.CheckForDBNull_String(rw("ProductCode"))
                                            Else
                                                _isCoursePdt = Utilities.IsCourseProduct(Utilities.CheckForDBNull_String(rw("ProductStadium")))
                                                If _isCoursePdt Then
                                                    lnkEditParticipantsLink.Enabled = True
                                                    lnkEditParticipantsLink.Visible = True
                                                    lnkEditParticipantsLink.NavigateUrl = "~/PagesPublic/Profile/RegistrationParticipants.aspx?productSubType=" & tempProductSubType & "&ProductCode=" & Utilities.CheckForDBNull_String(dr("PRODUCT")) & "&productStadium=" & Utilities.CheckForDBNull_String(rw("ProductStadium")) & "&productType=" & Utilities.CheckForDBNull_String(rw("ProductType")) & "&seat=" & tbi.SEAT
                                                End If
                                                CType(e.Item.FindControl("plhMoreTickets"), PlaceHolder).Visible = False
                                                divEventRepeater.Attributes("class") = "panel ebiz-ticketing-products ebiz-type-" & Utilities.CheckForDBNull_String(rw("ProductType")) & " ebiz-code-" & Utilities.CheckForDBNull_String(rw("ProductCode"))
                                            End If
                                            ltlDescription5.Text = String.Empty
                                            If Not String.IsNullOrEmpty(SelectProductType) AndAlso SelectProductType.Equals("S") Then
                                                moreTicketsLink.Visible = False
                                            End If
                                            Dim dtBasketDetailExcetpions As DataTable = TDataObjects.BasketSettings.TblBasketDetailExceptions.GetByBasketDetailHeaderIDAndModule(Profile.Basket.Basket_Header_ID, GlobalConstants.BASKETMODULETICKETING)
                                            If Usage = "BASKET" AndAlso dtBasketDetailExcetpions IsNot Nothing AndAlso dtBasketDetailExcetpions.Rows.Count > 0 Then
                                                plhSeasonTicketExceptionsLink.Visible = True
                                                hplSeasonTicketExceptions.Text = _ucr.Content("SeasonTicketExceptionsLinkText", _languageCode, True)
                                                hplSeasonTicketExceptions.NavigateUrl = "~/PagesPublic/ProductBrowse/SeasonTicketExceptions.aspx"
                                            End If
                                        End If
                                    Case Else
                                        fulfilmentOption = False
                                End Select

                                If isPackageIDExists Then
                                    moreTicketsLink.Enabled = False
                                    moreTicketsLink.Visible = False
                                End If

                                If Profile.Basket.CAT_MODE = GlobalConstants.CATMODE_CANCEL OrElse Profile.Basket.CAT_MODE = GlobalConstants.CATMODE_CANCELALL Then
                                    moreTicketsLink.Enabled = False
                                    moreTicketsLink.Visible = False
                                End If

                                If Session("PartnerPromotionCode") IsNot Nothing Then
                                    moreTicketsLink.NavigateUrl = "~/PagesPublic/ProductBrowse/PartnerPromotions.aspx"
                                End If

                            Catch ex As Exception
                            End Try

                        End If
                    Catch ex As Exception
                    End Try

                    'Have we got a different stand
                    If sStand <> tbi.PRODUCT_DESCRIPTION6.Trim Then
                        'Add the new product to the datatable
                        dRow = Nothing
                        dRow = dt.NewRow
                        dRow("PRODUCT") = tbi.Product.Trim
                        dRow("PRODUCT_DESCRIPTION6") = tbi.PRODUCT_DESCRIPTION6.Trim
                        dRow("PRODUCT_DESCRIPTION7") = tbi.PRODUCT_DESCRIPTION7.Trim
                        dRow("PACKAGE_ID") = Utilities.CheckForDBNull_Decimal(tbi.PACKAGE_ID)
                        Dim alreadyAdded As Boolean = False
                        For Each row As DataRow In dt.Rows
                            If row("PRODUCT") = tbi.Product.Trim() AndAlso row("PRODUCT_DESCRIPTION6") = tbi.PRODUCT_DESCRIPTION6.Trim Then
                                alreadyAdded = True
                                Exit For
                            End If
                        Next
                        If Not alreadyAdded Then dt.Rows.Add(dRow)

                        'Save the stand
                        sStand = tbi.PRODUCT_DESCRIPTION6.Trim
                    End If

                    ' Allow Packages if product_type is 'C' as this indicates a bundle.
                    If Utilities.CheckForDBNull_Decimal(tbi.PACKAGE_ID) > 0 Then
                        If Not Utilities.CheckForDBNull_String(tbi.PRODUCT_TYPE) = "C" Then
                            fulfilmentOption = False
                        End If
                    End If
                    If fulfilmentOption Then
                        If String.Compare(tbi.FULFIL_OPT_POST.Trim, "Y", True) = 0 Then PopulateFulFilmentList(GlobalConstants.POST_FULFILMENT, "Post")
                        If String.Compare(tbi.FULFIL_OPT_COLL.Trim, "Y", True) = 0 Then PopulateFulFilmentList(GlobalConstants.COLLECT_FULFILMENT, "Collect")
                        If String.Compare(tbi.FULFIL_OPT_PAH.Trim, "Y", True) = 0 Then PopulateFulFilmentList(GlobalConstants.PRINT_AT_HOME_FULFILMENT, "PrintAtHome")
                        If String.Compare(Utilities.CheckForDBNull_String(tbi.FULFIL_OPT_PRINT).Trim, "Y", True) = 0 And AgentProfile.IsAgent And AgentProfile.Type = "2" Then PopulateFulFilmentList(GlobalConstants.PRINT_FULFILMENT, "Print")
                        If String.Compare(tbi.FULFIL_OPT_REGPOST.Trim, "Y", True) = 0 Then PopulateFulFilmentList(GlobalConstants.REG_POST_FULFILMENT, "RegPost")
                        If String.Compare(tbi.FULFIL_OPT_UPL.Trim, "Y", True) = 0 Then PopulateFulFilmentList(GlobalConstants.SMARTCARD_UPLOAD_FULFILMENT, "SmartcardUpload")
                        If tbi.CURR_FULFIL_SLCTN.Trim <> "0" Then currentFulFilment = tbi.CURR_FULFIL_SLCTN.Trim
                    End If
                End If
                Select Case UCase(Usage)
                    Case Is = "ORDER", "PAYMENT"
                        lnkEditParticipantsLink.Enabled = False
                        lnkEditParticipantsLink.Visible = False
                        moreTicketsLink.Enabled = False
                        moreTicketsLink.Visible = False
                End Select
            Next

            If dt.Rows.Count > 0 Then
                standRepeater.DataSource = dt
                standRepeater.DataBind()
            End If
            If fulfilmentOption Then
                Dim eventFulfilmentDDL As DropDownList = CType(e.Item.FindControl("EventFulfilmentDDL"), DropDownList)
                If eventFulfilmentDDL IsNot Nothing Then
                    For Each Pair As KeyValuePair(Of String, String) In _fulfilmentList
                        eventFulfilmentDDL.Items.Add(New ListItem(Pair.Value, Pair.Key))
                    Next
                    If eventFulfilmentDDL.Items.Count > 0 Then
                        eventFulfilmentDDL.SelectedValue = currentFulFilment
                        CType(e.Item.FindControl("lblEventFulfilment"), Label).Text = _ucr.Content("FulfilmentLabelText", _languageCode, True)
                        If TalentDefaults.IsOrderLevelFulfilmentEnabled Then plhFulfilment.Visible = False
                    Else
                        plhFulfilment.Visible = False
                    End If
                End If
            Else
                plhFulfilment.Visible = False
            End If
            _fulfilmentList = Nothing
        End If

        CType(e.Item.FindControl("plhDescription1"), PlaceHolder).Visible = (ltlDescription1.Text.Length > 0)
        CType(e.Item.FindControl("plhDescription2"), PlaceHolder).Visible = (ltlDescription2.Text.Length > 0)
        CType(e.Item.FindControl("plhDescription3"), PlaceHolder).Visible = (ltlDescription3.Text.Length > 0)
        CType(e.Item.FindControl("plhDescription4"), PlaceHolder).Visible = (ltlDescription4.Text.Length > 0)
        CType(e.Item.FindControl("plhDescription5"), PlaceHolder).Visible = (ltlDescription5.Text.Length > 0)
        CType(e.Item.FindControl("plhSpecificContent1"), PlaceHolder).Visible = (ltlSpecificContent1.Text.Length > 0)
    End Sub

    Private Sub OutputProductSpecificErrors(ByVal tbi As TalentBasketItem)

        ' Error added in the repeater so I can easily retrieve the error description
        If Session("ProductTransactionTicketLimitExceeded") IsNot Nothing Then
            If String.IsNullOrWhiteSpace(Session("ProductTransactionTicketLimitProductCode")) OrElse tbi.Product.ToString.Trim = Session("ProductTransactionTicketLimitProductCode").ToString.Trim Then
                Dim transactionError As String = _ucr.Content("ProductTransactionTicketLimitExceededMessage", _languageCode, True).Replace("<<<ProductTransactionTicketLimit>>>", Session("ProductTransactionTicketLimit"))
                If String.IsNullOrWhiteSpace(Session("ProductTransactionTicketLimitProductCode")) Then
                    transactionError = transactionError.Replace("<<<ProductDescription>>>", String.Empty)
                Else
                    transactionError = transactionError.Replace("<<<ProductDescription>>>", tbi.PRODUCT_DESCRIPTION1.ToString.Trim)
                End If
                _errorList.Items.Add(transactionError)
                Session("ProductTransactionTicketLimitExceeded") = Nothing
                Session("ProductTransactionTicketLimit") = Nothing
                Session("ProductTransactionTicketLimitProductCode") = Nothing
            End If
        End If

    End Sub



    Private Function FormatDate(ByVal dateParm As String) As String
        Try
            Dim dateValue As Date = dateParm
            Dim culture As New CultureInfo(ModuleDefaults.Culture)
            If Date.Compare(dateValue, Now) > 0 Then
                If ModuleDefaults.GlobalDateFormat = "yyyy/MM/dd" Then
                    Dim dateString As String = dateValue.ToString("dd MMMM")
                    Dim day As String = culture.DateTimeFormat.DayNames(dateValue.DayOfWeek)
                    Dim dateSeparator As String = _ucr.Content("DateSeparator", _languageCode, True)
                    Return day & dateSeparator & dateString & dateSeparator & dateValue.Year
                Else
                    Return dateValue.ToString(ModuleDefaults.GlobalDateFormat, culture)
                End If
            Else
                Return String.Empty
            End If
        Catch ex As Exception
            Return String.Empty
        End Try
    End Function

    Protected Sub DoStandItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        Dim standLabel As Literal = CType(e.Item.FindControl("ltlStandLabel"), Literal)
        Dim standPreLabel As Literal = CType(e.Item.FindControl("ltlStandPreLabel"), Literal)
        Dim areaRepeater As Repeater = CType(e.Item.FindControl("AreaRepeater"), Repeater)
        Dim plhStandDetails As PlaceHolder = CType(e.Item.FindControl("plhStandDetails"), PlaceHolder)
        Dim plhPackageDetails As PlaceHolder = CType(e.Item.FindControl("plhPackageDetails"), PlaceHolder)
        Dim lnkPackageBooking As HyperLink = CType(e.Item.FindControl("lnkpackageBooking"), HyperLink)
        Dim PackageCommentsLabel As Literal = CType(e.Item.FindControl("ltlPackageCommentsLabel"), Literal)
        Dim plhPackageComments As PlaceHolder = CType(e.Item.FindControl("plhPackageComments"), PlaceHolder)
        Dim dr As DataRow = CType(e.Item.DataItem, DataRowView).Row
        Dim packageID As Decimal = Utilities.CheckForDBNull_Decimal(dr("PACKAGE_ID"))
        'plhPackageDetails
        'PackagePreLabel
        'PackageDescriptionLabel
        'PackageCommentsLabel
        If packageID > 0 Then
            plhPackageDetails.Visible = True
            plhStandDetails.Visible = False
            lnkPackageBooking.Text = _ucr.Content("ViewOrEditPackageBookingText", _languageCode, True).Replace("<<PackageDescription>>", Utilities.CheckForDBNull_String(dr("PRODUCT_DESCRIPTION6")))
            Dim packageRow As DataRow = LoadPackageDetail(Utilities.CheckForDBNull_String(dr("PRODUCT")), Utilities.CheckForDBNull_String(dr("PACKAGE_ID")))
            If packageRow IsNot Nothing Then
                PackageCommentsLabel.Text = packageRow("Comment1") & " " & packageRow("Comment2")
            End If
        Else
            plhPackageDetails.Visible = False
            plhStandDetails.Visible = True
            If String.IsNullOrEmpty(Utilities.CheckForDBNull_String(dr("PRODUCT_DESCRIPTION6"))) Then
                plhStandDetails.Visible = False
            Else
                standPreLabel.Text = _ucr.Content("StandLabelText", _languageCode, True)
                standLabel.Text = dr("PRODUCT_DESCRIPTION6")
            End If
        End If
        plhPackageComments.Visible = (PackageCommentsLabel.Text.Trim().Length > 0)

        'Create the basket data table
        Dim dt As New DataTable
        With dt.Columns
            .Add("PRODUCT", GetType(String))
            .Add("PRODUCT_DESCRIPTION6", GetType(String))
            .Add("PRODUCT_DESCRIPTION7", GetType(String))
        End With

        'New Code
        Dim dRow As DataRow = Nothing
        Dim sArea As String = "*NONE"
        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            If tbi.Product.Trim = dr("PRODUCT").ToString.Trim AndAlso tbi.PRODUCT_DESCRIPTION6.Trim = dr("PRODUCT_DESCRIPTION6").ToString.Trim AndAlso sArea.Trim <> tbi.PRODUCT_DESCRIPTION7.Trim Then
                dRow = Nothing
                dRow = dt.NewRow
                dRow("PRODUCT") = tbi.Product.Trim
                dRow("PRODUCT_DESCRIPTION6") = tbi.PRODUCT_DESCRIPTION6.Trim
                dRow("PRODUCT_DESCRIPTION7") = tbi.PRODUCT_DESCRIPTION7.Trim
                Dim alreadyAdded As Boolean = False
                For Each row As DataRow In dt.Rows
                    If row("PRODUCT") = tbi.Product.Trim() AndAlso row("PRODUCT_DESCRIPTION6") = tbi.PRODUCT_DESCRIPTION6.Trim AndAlso row("PRODUCT_DESCRIPTION7") = tbi.PRODUCT_DESCRIPTION7.Trim Then
                        alreadyAdded = True
                        Exit For
                    End If
                Next
                If Not alreadyAdded Then dt.Rows.Add(dRow)

                'Save the area
                sArea = tbi.PRODUCT_DESCRIPTION7.Trim

                If (Utilities.CheckForDBNull_Decimal(tbi.PACKAGE_ID) > 0 AndAlso Not String.IsNullOrEmpty(tbi.Product)) Then
                    Dim currentUrl As String = HttpContext.Current.Request.Url.AbsolutePath.Split("?")(0)
                    If Not currentUrl.ToUpper().Contains("CHECKOUTORDERCONFIRMATION.ASPX") Then
                        lnkPackageBooking.Enabled = True
                        lnkPackageBooking.NavigateUrl = "~/PagesPublic/Hospitality/HospitalityBooking.aspx?product=" + tbi.Product + "&packageid=" + tbi.PACKAGE_ID.ToString()
                    Else
                        lnkPackageBooking.Visible = False
                    End If
                Else
                    lnkPackageBooking.Enabled = False
                End If


            End If
        Next
        If dt.Rows.Count > 0 Then
            areaRepeater.DataSource = dt
            areaRepeater.DataBind()
        End If
    End Sub

    Protected Sub DoAreaItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        Dim areaLabel As Literal = CType(e.Item.FindControl("ltlAreaLabel"), Literal)
        Dim plhAreaDetails As PlaceHolder = CType(e.Item.FindControl("plhAreaDetails"), PlaceHolder)
        Dim areaPreLabel As Literal = CType(e.Item.FindControl("ltlAreaPreLabel"), Literal)
        Dim SeatsRepeater As Repeater = CType(e.Item.FindControl("SeatsRepeater"), Repeater)
        Dim dr As DataRow = CType(e.Item.DataItem, DataRowView).Row
        If String.IsNullOrEmpty(Utilities.CheckForDBNull_String(dr("PRODUCT_DESCRIPTION7"))) Then
            plhAreaDetails.Visible = False
        Else
            areaPreLabel.Text = _ucr.Content("AreaLabelText", _languageCode, True)
            areaLabel.Text = dr("PRODUCT_DESCRIPTION7")
            plhAreaDetails.Visible = True
        End If

        'Create the basket data table
        Dim dt As New DataTable
        With dt.Columns
            .Add("BASKET_DETAIL_ID", GetType(String))
            .Add("PRODUCT", GetType(String))
            .Add("PRODUCT_TYPE", GetType(String))
            .Add("SEAT", GetType(String))
            .Add("LOGINID", GetType(String))
            .Add("PRICE", GetType(String))
            .Add("PRICE_BAND", GetType(String))
            .Add("PRICE_CODE", GetType(String))
            .Add("RESERVED_SEAT", GetType(String))
            .Add("RESTRICTION_CODE", GetType(String))
            .Add("FULFIL_OPT_POST", GetType(String))
            .Add("FULFIL_OPT_COLL", GetType(String))
            .Add("FULFIL_OPT_PAH", GetType(String))
            .Add("FULFIL_OPT_UPL", GetType(String))
            .Add("FULFIL_OPT_REGPOST", GetType(String))
            .Add("FULFIL_OPT_PRINT", GetType(String))
            .Add("CURR_FULFIL_SLCTN", GetType(String))
            .Add("QUANTITY", GetType(String))
            .Add("PACKAGE_ID", GetType(String))
            .Add("NET_PRICE", GetType(String))
            .Add("TRAVEL_PRODUCT_SELECTED", GetType(Boolean))
            .Add("CAN_SAVE_AS_FAVOURITE_SEAT", GetType(Boolean))
            .Add("ORIGINAL_LOGINID", GetType(String))
            .Add("ORIGINAL_PRICE", GetType(String))
            .Add("VALID_PRICE_BANDS", GetType(String))
            .Add("CUSTOMER_ALLOCATION", GetType(Boolean))
            .Add("VOUCHER_CODE", GetType(String))
            .Add("VOUCHER_ID", GetType(String))
            .Add("UNRESERVED", GetType(Boolean))
            .Add("ROVING", GetType(Boolean))
            .Add("BULK_SALES_ID", GetType(Integer))
            .Add("ALLOCATED_SEAT", GetType(String))
            .Add("RESERVATION_CODE", GetType(String))
            .Add("RESTRICTED_BASKET_OPTIONS", GetType(Boolean))
            .Add("DISPLAY_IN_A_CANCEL_BASKET", GetType(Boolean))
            .Add("PRICE_BAND_DESCRIPTION", GetType(String))
        End With

        'New Code - Need to add module to the if statement
        Dim dRow As DataRow = Nothing
        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            If tbi.Product.Trim = dr("PRODUCT").ToString.Trim AndAlso _
                    tbi.PRODUCT_DESCRIPTION6.Trim = dr("PRODUCT_DESCRIPTION6").ToString.Trim AndAlso _
                        tbi.PRODUCT_DESCRIPTION7.Trim = dr("PRODUCT_DESCRIPTION7").ToString.Trim Then

                'Display the pacakage summary when in coporate mode.  TODO: Is this only valid for ticketing packages?????
                If Utilities.CheckForDBNull_String(tbi.PACKAGE_TYPE).Equals(GlobalConstants.PROMOTED_EVENT_PACKAGE_TYPE) Then
                    Dim packageRow As DataRow = LoadPackageDetail(Utilities.CheckForDBNull_String(dr("PRODUCT")), tbi.PACKAGE_ID)
                    Dim packageType As String = String.Empty
                    ' If packageRow IsNot Nothing Then packageType = packageRow("packageType")
                    Dim uscPackageSummary As UserControls_Package_PackageSummary = CType(e.Item.FindControl("uscPackageSummary"), UserControls_Package_PackageSummary)
                    uscPackageSummary.Display = True
                    uscPackageSummary.PackageId = tbi.PACKAGE_ID
                    uscPackageSummary.ProductCode = tbi.Product
                    If Usage = "BASKET" Then
                        uscPackageSummary.Mode = PackageSummaryMode.Basket
                    Else
                        uscPackageSummary.Mode = PackageSummaryMode.Output
                    End If
                    uscPackageSummary.TBI = tbi
                Else
                    dRow = Nothing
                    dRow = dt.NewRow
                    dRow("BASKET_DETAIL_ID") = tbi.Basket_Detail_ID
                    dRow("PRODUCT") = tbi.Product
                    dRow("PRODUCT_TYPE") = tbi.PRODUCT_TYPE
                    dRow("SEAT") = tbi.SEAT
                    dRow("LOGINID") = tbi.LOGINID
                    dRow("PRICE") = tbi.Gross_Price
                    dRow("PRICE_BAND") = tbi.PRICE_BAND
                    dRow("PRICE_CODE") = tbi.PRICE_CODE
                    dRow("RESERVED_SEAT") = tbi.RESERVED_SEAT
                    dRow("RESTRICTION_CODE") = tbi.RESTRICTION_CODE
                    dRow("FULFIL_OPT_POST") = tbi.FULFIL_OPT_POST
                    dRow("FULFIL_OPT_COLL") = tbi.FULFIL_OPT_COLL
                    dRow("FULFIL_OPT_PAH") = tbi.FULFIL_OPT_PAH
                    dRow("FULFIL_OPT_UPL") = tbi.FULFIL_OPT_UPL
                    dRow("FULFIL_OPT_REGPOST") = tbi.FULFIL_OPT_REGPOST
                    dRow("FULFIL_OPT_PRINT") = Talent.eCommerce.Utilities.CheckForDBNull_String(tbi.FULFIL_OPT_PRINT)
                    dRow("CURR_FULFIL_SLCTN") = tbi.CURR_FULFIL_SLCTN
                    dRow("QUANTITY") = tbi.Quantity
                    dRow("PACKAGE_ID") = tbi.PACKAGE_ID
                    dRow("NET_PRICE") = tbi.Net_Price
                    dRow("TRAVEL_PRODUCT_SELECTED") = tbi.TRAVEL_PRODUCT_SELECTED
                    dRow("CAN_SAVE_AS_FAVOURITE_SEAT") = tbi.CAN_SAVE_AS_FAVOURITE_SEAT
                    dRow("ORIGINAL_LOGINID") = tbi.ORIGINAL_LOGINID
                    dRow("ORIGINAL_PRICE") = tbi.ORIGINAL_PRICE
                    dRow("VALID_PRICE_BANDS") = tbi.VALID_PRICE_BANDS
                    dRow("CUSTOMER_ALLOCATION") = Utilities.CheckForDBNull_Boolean_DefaultFalse(If(String.IsNullOrEmpty(tbi.CUSTOMER_ALLOCATION), False, tbi.CUSTOMER_ALLOCATION.ConvertFromISeriesYesNoToBoolean()))
                    dRow("VOUCHER_CODE") = Utilities.CheckForDBNull_String(tbi.VOUCHER_CODE)
                    dRow("VOUCHER_ID") = Utilities.CheckForDBNull_String(tbi.VOUCHER_DEFINITION_ID)
                    If Utilities.CheckForDBNull_String(tbi.ROVING) = "ROVING" Then
                        dRow("ROVING") = True
                    Else
                        dRow("ROVING") = False
                    End If
                    If Utilities.CheckForDBNull_String(tbi.ROVING) = "UNRESERVED" Then
                        dRow("UNRESERVED") = True
                    Else
                        dRow("UNRESERVED") = False
                    End If
                    dRow("BULK_SALES_ID") = tbi.BULK_SALES_ID
                    dRow("ALLOCATED_SEAT") = tbi.ALLOCATED_SEAT
                    dRow("RESERVATION_CODE") = tbi.RESERVATION_CODE
                    dRow("RESTRICTED_BASKET_OPTIONS") = tbi.RESTRICTED_BASKET_OPTIONS
                    dRow("DISPLAY_IN_A_CANCEL_BASKET") = tbi.DISPLAY_IN_A_CANCEL_BASKET
                    dRow("PRICE_BAND_DESCRIPTION") = tbi.GROUP_LEVEL_09
                    dt.Rows.Add(dRow)
                End If

                'check for corporate
                If Utilities.CheckForDBNull_Decimal(tbi.PACKAGE_ID) > 0 Then
                    areaPreLabel.Visible = False
                    areaLabel.Visible = False
                End If
            End If
        Next
        If dt.Rows.Count > 0 Then
            SeatsRepeater.DataSource = dt
            SeatsRepeater.DataBind()
        End If
    End Sub

    Protected Sub DoSeatsItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        If e.Item.ItemIndex > -1 Then
            Dim hdfBasketDetailID As HiddenField = CType(e.Item.FindControl("hdfBasketDetailID"), HiddenField)
            Dim customerDDL As DropDownList = CType(e.Item.FindControl("CustomerDDL"), DropDownList)
            Dim customerLabel As Label = CType(e.Item.FindControl("customerLabel"), Label)
            Dim custLoginLink As HyperLink = CType(e.Item.FindControl("LoginLink1"), HyperLink)
            Dim bandLoginLink As HyperLink = CType(e.Item.FindControl("LoginLink2"), HyperLink)
            Dim seatMessage As Label = CType(e.Item.FindControl("seatMessage"), Label)
            Dim bandMessage As Label = CType(e.Item.FindControl("BandMessage"), Label)
            Dim PriceBandDDL As DropDownList = CType(e.Item.FindControl("PriceBandDDL"), DropDownList)
            Dim ProductCode As Label = CType(e.Item.FindControl("ProductCode"), Label)
            Dim OriginalCustomer As Label = CType(e.Item.FindControl("OriginalCustomer"), Label)
            Dim OriginalBand As Label = CType(e.Item.FindControl("OriginalBand"), Label)
            Dim OriginalFulfilment As Label = CType(e.Item.FindControl("OriginalFulfilment"), Label)
            Dim OriginalFulfilmentCode As Label = CType(e.Item.FindControl("OriginalFulfilmentCode"), Label)
            Dim PriceLabel As Label = CType(e.Item.FindControl("PriceLabel"), Label)
            Dim RemoveButton As Button = CType(e.Item.FindControl("RemoveButton"), Button)
            Dim bandCol As HtmlTableCell = CType(e.Item.FindControl("bandCol"), HtmlTableCell)
            Dim seatCol As HtmlTableCell = CType(e.Item.FindControl("seatCol"), HtmlTableCell)
            Dim fulfilmentCol As HtmlTableCell = CType(e.Item.FindControl("fulfilmentCol"), HtmlTableCell)
            Dim removeCol As HtmlTableCell = CType(e.Item.FindControl("removeCol"), HtmlTableCell)
            Dim packageQuantityCol As HtmlTableCell = CType(e.Item.FindControl("packageQuantityCol"), HtmlTableCell)
            Dim PackageQuantityLabel As Label = CType(e.Item.FindControl("PackageQuantityLabel"), Label)
            Dim PackageIDForQuantity As Label = CType(e.Item.FindControl("PackageIDForQuantity"), Label)
            Dim dr As DataRow = CType(e.Item.DataItem, DataRowView).Row
            Dim priceCode As Label = CType(e.Item.FindControl("PriceCode"), Label)
            Dim productType As Label = CType(e.Item.FindControl("ProductType"), Label)
            Dim productTypeMTTP As Label = CType(e.Item.FindControl("ProductTypeMTTP"), Label)
            Dim netPriceCol As HtmlTableCell = CType(e.Item.FindControl("netPriceCol"), HtmlTableCell)
            Dim netPriceLabel As Label = CType(e.Item.FindControl("NetPriceLabel"), Label)
            Dim NewParticipantLabel As Label = CType(e.Item.FindControl("NewParticipantLabel"), Label)
            Dim hplComments As HtmlAnchor = CType(e.Item.FindControl("hplComments"), HtmlAnchor)
            Dim hplPromotions As HyperLink = CType(e.Item.FindControl("hplPromotions"), HyperLink)
            Dim hplVouchers As HyperLink = CType(e.Item.FindControl("hplVouchers"), HyperLink)
            Dim priceCodeCol As HtmlTableCell = CType(e.Item.FindControl("priceCodeCol"), HtmlTableCell)
            Dim PriceCodeDDL As DropDownList = CType(e.Item.FindControl("PriceCodeDDL"), DropDownList)
            Dim lblPriceCodeDescription As Label = CType(e.Item.FindControl("lblPriceCodeDescription"), Label)
            Dim EventFulfilmentDDL As DropDownList = CType(e.Item.FindControl("EventFulfilmentDDL"), DropDownList)
            Dim OriginalPriceCode As Label = CType(e.Item.FindControl("OriginalPriceCode"), Label)
            Dim hidFFRegURL As HiddenField = CType(e.Item.FindControl("hidFFRegURL"), HiddenField)
            Dim hdfNewCustomerURL As HiddenField = CType(e.Item.FindControl("hdfNewCustomerURL"), HiddenField)
            Dim bulkSalesQuantityCol As HtmlTableCell = CType(e.Item.FindControl("bulkSalesQuantityCol"), HtmlTableCell)
            Dim txtBulkSalesQuantity As TextBox = CType(e.Item.FindControl("txtBulkSalesQuantity"), TextBox)
            Dim lblBulkSalesQuantity As Label = CType(e.Item.FindControl("lblBulkSalesQuantity"), Label)
            Dim rfvBulkSalesQuantity As RequiredFieldValidator = CType(e.Item.FindControl("rfvBulkSalesQuantity"), RequiredFieldValidator)
            Dim rgxBulkSalesQuantity As RegularExpressionValidator = CType(e.Item.FindControl("rgxBulkSalesQuantity"), RegularExpressionValidator)
            Dim hdfBulkSalesID As HiddenField = CType(e.Item.FindControl("hdfBulkSalesID"), HiddenField)

            'Set basic basket item data
            hdfBasketDetailID.Value = Utilities.CheckForDBNull_String(dr("BASKET_DETAIL_ID"))
            setBasketKioskMode(customerLabel, fulfilmentCol, OriginalFulfilment, bandCol, priceCodeCol, PriceBandDDL, customerDDL, OriginalCustomer)
            PriceLabel.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.CheckForDBNull_Decimal(dr("PRICE")), _ucr.BusinessUnit, _ucr.PartnerCode)
            _prodTypeMTTP = Utilities.CheckForDBNull_String(CurrentProductDetailsResultSet.Tables(2).Rows(0)("ProductType")).Trim()
            OriginalCustomer.Text = Profile.UserName.TrimStart("0")
            priceCode.Text = Utilities.CheckForDBNull_String(dr("PRICE_CODE"))
            OriginalPriceCode.Text = Utilities.CheckForDBNull_String(dr("PRICE_CODE"))
            productType.Text = Utilities.CheckForDBNull_String(dr("PRODUCT_TYPE"))
            productTypeMTTP.Text = _prodTypeMTTP
            ProductCode.Text = dr("PRODUCT")

            'Set display of table columns and functions based on whether or not the user is logged in
            If (Not TEBUtilities.IsAnonymousTalent) Then
                custLoginLink.Visible = False
                bandLoginLink.Visible = False
                seatMessage.Visible = False
                bandMessage.Visible = False
                setFulfilmentDetails(dr, fulfilmentCol, OriginalFulfilment, OriginalFulfilmentCode)
                setPriceCodeOptions(priceCodeCol, PriceCodeDDL, ProductCode, priceCode, lblPriceCodeDescription, dr)
                setDDLTriggers(PriceCodeDDL, PriceBandDDL, EventFulfilmentDDL)
                setPromotionsLink(dr, hplPromotions, PriceCodeDDL)
                setVouchersLink(dr, hplVouchers)
                setCommentsLink(e, dr, hplComments, ProductCode)
            Else
                priceCodeCol.Visible = False
                PriceCodeDDL.Visible = False
                PriceBandDDL.Visible = False
                customerDDL.Visible = False
                bandCol.Visible = False
                seatMessage.Visible = False
                bandMessage.Visible = False
                fulfilmentCol.Visible = False
                OriginalFulfilment.Visible = False
                custLoginLink.Visible = True
                bandLoginLink.Visible = True
                hplComments.Visible = False
                hplPromotions.Visible = False
                hplVouchers.Visible = False
                custLoginLink.Text = _ucr.Content("LoginLinkText", _languageCode, True)
                custLoginLink.NavigateUrl = "~/PagesPublic/Login/Login.aspx?ReturnUrl=" & Server.UrlEncode(Request.Url.PathAndQuery)
                bandLoginLink.Text = _ucr.Content("LoginLinkText", _languageCode, True)
                bandLoginLink.NavigateUrl = "~/PagesPublic/Login/Login.aspx?ReturnUrl=" & Server.UrlEncode(Request.Url.PathAndQuery)
                If AgentProfile.IsAgent OrElse Utilities.CheckForDBNull_String(CurrentProductDetailsResultSet.Tables(2).Rows(0)("AllowPriceBandAlterations")).Trim() = GlobalConstants.PRICE_BAND_ALTERATIONS_PLUS_ANONYMOUS Then
                    PriceBandDDL.Visible = True
                    bandCol.Visible = True
                    bandLoginLink.Visible = False
                End If
            End If

            'Set remaining basket functions regardless of whether the user is logged in or not
            setCustomerDetails(dr, customerDDL, OriginalCustomer, NewParticipantLabel, hidFFRegURL, custLoginLink, hdfNewCustomerURL)
            setSeatDetails(dr, seatCol, e)
            PriceBandProcessing(dr, _talentErrObj, OriginalBand, PriceBandDDL, bandMessage, bandCol, bandLoginLink, seatMessage, PriceLabel, customerDDL, custLoginLink, customerLabel, OriginalCustomer)
            SetPriceBandDetails(dr, customerDDL, PriceBandDDL, OriginalBand, NewParticipantLabel)
            setMatchDayHospitalityBasket(dr, fulfilmentCol, OriginalFulfilment, productTypeMTTP, bandCol, seatCol, PriceBandDDL, customerDDL, priceCodeCol, PriceCodeDDL, packageQuantityCol, PackageIDForQuantity, PackageQuantityLabel, netPriceCol, netPriceLabel, customerLabel)
            setFavouriteSeatDetails(dr, e)
            setEPurseBasket(dr, customerDDL, customerLabel, OriginalCustomer, seatCol, priceCodeCol, bandCol)
            setRestrictedBasketOptions(dr, customerDDL, customerLabel, OriginalCustomer, seatCol, priceCodeCol, bandCol)
            setBulkSalesModeDisplay(dr, hdfBulkSalesID, txtBulkSalesQuantity, lblBulkSalesQuantity, seatCol, bulkSalesQuantityCol, rfvBulkSalesQuantity, rgxBulkSalesQuantity)
            setTicketExchangeOption(dr, e)

            'Finalise the basket layout if the page is Checkout.aspx or CheckoutOrderConfirmation.aspx
            If Usage = "ORDER" OrElse Usage = "PAYMENT" Then
                basketLayoutForCheckoutPages(priceCodeCol, PriceCodeDDL, OriginalPriceCode, priceCode, custLoginLink, OriginalCustomer, customerLabel, customerDDL, PriceBandDDL, OriginalBand, OriginalFulfilment, removeCol, lblBulkSalesQuantity, txtBulkSalesQuantity)
            End If
            If Not _displayCustomerSearchLink Then
                If customerDDL.Visible Then
                    _displayCustomerSearchLink = True
                End If
            End If
            If Session("AssignedAlternativeSeat") IsNot Nothing Then
                _errorList.Items.Add(_ucr.Content("AssignedAlternativeSeat", _languageCode, True))
                Session("AssignedAlternativeSeat") = Nothing
            End If
        End If
    End Sub

    Protected Sub SeatColumnPreRender(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim pCode As String = ""
        Dim packageIDForQuantity As String = ""
        Try
            pCode = CType(CType(CType(CType(sender, HtmlTableCell).Parent, RepeaterItem).Parent, Repeater).Items(0).FindControl("ProductCode"), Label).Text
            packageIDForQuantity = CType(CType(CType(CType(sender, HtmlTableCell).Parent, RepeaterItem).Parent, Repeater).Items(0).FindControl("PackageIDForQuantity"), Label).Text
        Catch ex As Exception
        End Try
        For Each ri1 As RepeaterItem In EventRepeater.Items
            Dim rpt2 As Repeater = CType(ri1.FindControl("StandRepeater"), Repeater)
            For Each ri2 As RepeaterItem In rpt2.Items
                Dim rpt3 As Repeater = CType(ri2.FindControl("AreaRepeater"), Repeater)
                For Each ri3 As RepeaterItem In rpt3.Items
                    Dim rpt4 As Repeater = CType(ri3.FindControl("SeatsRepeater"), Repeater)
                    Dim hideHeader As Boolean = False
                    For Each ri4 As RepeaterItem In rpt4.Items
                        If ri4.ItemIndex <> -1 Then
                            If Not CType(ri4.FindControl("ProductCode"), Label).Text = pCode Then
                                'If the product code does not match then we are not interested in this product
                                Exit For
                            Else
                                Dim seatCol As HtmlTableCell = CType(ri4.FindControl("seatCol"), HtmlTableCell)
                                hideHeader = Not seatCol.Visible
                                Exit For
                            End If
                        End If
                    Next
                    If hideHeader Then
                        CType(sender, HtmlTableCell).Visible = False
                    End If
                    Select Case UCase(Usage)
                        Case Is = "BASKET"
                        Case Is = "ORDER", "PAYMENT"
                            CType(CType(CType(sender, HtmlTableCell).Parent, RepeaterItem).FindControl("removeCol"), HtmlTableCell).Visible = False
                    End Select
                Next
            Next
        Next
    End Sub

    Protected Sub PriceCodeColumnPreRender(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim pCode As String = ""
        Try
            pCode = CType(CType(CType(CType(sender, HtmlTableCell).Parent, RepeaterItem).Parent, Repeater).Items(0).FindControl("ProductCode"), Label).Text
        Catch ex As Exception
        End Try
        For Each ri1 As RepeaterItem In EventRepeater.Items
            Dim rpt2 As Repeater = CType(ri1.FindControl("StandRepeater"), Repeater)
            For Each ri2 As RepeaterItem In rpt2.Items
                Dim rpt3 As Repeater = CType(ri2.FindControl("AreaRepeater"), Repeater)
                For Each ri3 As RepeaterItem In rpt3.Items
                    Dim rpt4 As Repeater = CType(ri3.FindControl("SeatsRepeater"), Repeater)
                    Dim hideHeader As Boolean = False
                    For Each ri4 As RepeaterItem In rpt4.Items
                        If ri4.ItemIndex <> -1 Then
                            If Not CType(ri4.FindControl("ProductCode"), Label).Text = pCode Then
                                'If the product code does not match then we are not interested in this product
                                Exit For
                            Else
                                Dim priceCodeCol As HtmlTableCell = CType(ri4.FindControl("priceCodeCol"), HtmlTableCell)
                                hideHeader = Not priceCodeCol.Visible
                                Exit For
                            End If

                        End If
                    Next
                    If hideHeader Then
                        CType(sender, HtmlTableCell).Visible = False
                    End If
                Next
            Next
        Next
    End Sub

    Protected Sub PriceBandColumnPreRender(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim pCode As String = ""
        Try
            pCode = CType(CType(CType(CType(sender, HtmlTableCell).Parent, RepeaterItem).Parent, Repeater).Items(0).FindControl("ProductCode"), Label).Text
        Catch ex As Exception
        End Try
        For Each ri1 As RepeaterItem In EventRepeater.Items
            Dim rpt2 As Repeater = CType(ri1.FindControl("StandRepeater"), Repeater)
            For Each ri2 As RepeaterItem In rpt2.Items
                Dim rpt3 As Repeater = CType(ri2.FindControl("AreaRepeater"), Repeater)
                For Each ri3 As RepeaterItem In rpt3.Items
                    Dim rpt4 As Repeater = CType(ri3.FindControl("SeatsRepeater"), Repeater)
                    Dim hideHeader As Boolean = False
                    For Each ri4 As RepeaterItem In rpt4.Items
                        If ri4.ItemIndex <> -1 Then
                            If Not CType(ri4.FindControl("ProductCode"), Label).Text = pCode Then
                                'If the product code does not match then we are not interested in this product
                                Exit For
                            Else
                                Dim bandCol As HtmlTableCell = CType(ri4.FindControl("bandCol"), HtmlTableCell)
                                hideHeader = Not bandCol.Visible
                                Exit For
                            End If
                        End If
                    Next
                    If hideHeader Then
                        CType(sender, HtmlTableCell).Visible = False
                    End If
                Next
            Next
        Next
    End Sub

    Protected Sub FulfilmentColumnPreRender(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim productTypeMTTP As String = ""
        Try
            productTypeMTTP = CType(CType(CType(CType(sender, HtmlTableCell).Parent, RepeaterItem).Parent, Repeater).Items(0).FindControl("ProductTypeMTTP"), Label).Text
        Catch ex As Exception
        End Try
        For Each ri1 As RepeaterItem In EventRepeater.Items
            Dim rpt2 As Repeater = CType(ri1.FindControl("StandRepeater"), Repeater)
            For Each ri2 As RepeaterItem In rpt2.Items
                Dim rpt3 As Repeater = CType(ri2.FindControl("AreaRepeater"), Repeater)
                For Each ri3 As RepeaterItem In rpt3.Items
                    Dim rpt4 As Repeater = CType(ri3.FindControl("SeatsRepeater"), Repeater)
                    Dim hideHeader As Boolean = False
                    For Each ri4 As RepeaterItem In rpt4.Items
                        If ri4.ItemIndex <> -1 Then
                            If Not CType(ri4.FindControl("ProductTypeMTTP"), Label).Text = productTypeMTTP Then
                                Exit For
                            Else
                                Dim fulfilmentCol As HtmlTableCell = CType(ri4.FindControl("fulfilmentCol"), HtmlTableCell)
                                hideHeader = Not fulfilmentCol.Visible
                                Exit For
                            End If

                        End If
                    Next
                    If hideHeader Then
                        CType(sender, HtmlTableCell).Visible = False
                    End If
                Next
            Next
        Next
    End Sub

    Protected Sub PackageColumnsPreRender(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim packageIDForQuantity As String = ""
        Try
            packageIDForQuantity = CType(CType(CType(CType(sender, HtmlTableCell).Parent, RepeaterItem).Parent, Repeater).Items(0).FindControl("PackageIDForQuantity"), Label).Text
        Catch ex As Exception
        End Try
        For Each ri1 As RepeaterItem In EventRepeater.Items
            Dim rpt2 As Repeater = CType(ri1.FindControl("StandRepeater"), Repeater)
            For Each ri2 As RepeaterItem In rpt2.Items
                Dim rpt3 As Repeater = CType(ri2.FindControl("AreaRepeater"), Repeater)
                For Each ri3 As RepeaterItem In rpt3.Items
                    Dim rpt4 As Repeater = CType(ri3.FindControl("SeatsRepeater"), Repeater)
                    Dim hideHeader As Boolean = False
                    For Each ri4 As RepeaterItem In rpt4.Items
                        If ri4.ItemIndex <> -1 Then
                            If Not CType(ri4.FindControl("PackageIDForQuantity"), Label).Text = packageIDForQuantity Then
                                Exit For
                            Else
                                If CType(sender, HtmlTableCell).ID = "packageQuantityCol" Then
                                    Dim packageQuantityCol As HtmlTableCell = CType(ri4.FindControl("packageQuantityCol"), HtmlTableCell)
                                    hideHeader = Not packageQuantityCol.Visible
                                Else
                                    If Not (Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("displayNetPrice"))) Then
                                        hideHeader = True
                                    Else
                                        Dim netPriceCol As HtmlTableCell = CType(ri4.FindControl("netPriceCol"), HtmlTableCell)
                                        hideHeader = Not netPriceCol.Visible
                                    End If
                                End If
                                Exit For
                            End If
                        End If
                    Next
                    If hideHeader Then
                        CType(sender, HtmlTableCell).Visible = False
                    End If
                Next
            Next
        Next
    End Sub

    Protected Sub BulkSalesQuantityColumnPreRender(ByVal sender As Object, ByVal e As System.EventArgs)
        For Each ri1 As RepeaterItem In EventRepeater.Items
            Dim rpt2 As Repeater = CType(ri1.FindControl("StandRepeater"), Repeater)
            For Each ri2 As RepeaterItem In rpt2.Items
                Dim rpt3 As Repeater = CType(ri2.FindControl("AreaRepeater"), Repeater)
                For Each ri3 As RepeaterItem In rpt3.Items
                    Dim rpt4 As Repeater = CType(ri3.FindControl("SeatsRepeater"), Repeater)
                    For Each ri4 As RepeaterItem In rpt4.Items
                        Dim showColumn As Boolean = False
                        If AgentProfile.IsAgent Then
                            For Each item As TalentBasketItem In Profile.Basket.BasketItems
                                If item.BULK_SALES_ID > 0 Then
                                    showColumn = True
                                    Exit For
                                End If
                            Next
                            If showColumn AndAlso AgentProfile.BulkSalesMode Then
                                showColumn = True
                            Else
                                showColumn = False
                            End If
                        End If
                        CType(sender, HtmlTableCell).Visible = showColumn
                    Next
                Next
            Next
        Next
    End Sub

    Protected Sub OverridePromotionCheckBoxPreRender(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim pCode As String = ""
        Try
            pCode = CType(CType(CType(CType(sender, CheckBox).Parent.Parent, RepeaterItem).Parent, Repeater).Items(0).FindControl("ProductCode"), Label).Text
        Catch ex As Exception
        End Try

        For Each ri1 As RepeaterItem In EventRepeater.Items
            Dim rpt2 As Repeater = CType(ri1.FindControl("StandRepeater"), Repeater)
            For Each ri2 As RepeaterItem In rpt2.Items
                Dim rpt3 As Repeater = CType(ri2.FindControl("AreaRepeater"), Repeater)
                For Each ri3 As RepeaterItem In rpt3.Items
                    Dim rpt4 As Repeater = CType(ri3.FindControl("SeatsRepeater"), Repeater)
                    Dim showOverridePromotionPriceCodeCheckBox As Boolean = False
                    Dim isItemProcessedPromoCheck As Boolean = False
                    Dim str As String = ""
                    For Each ri4 As RepeaterItem In rpt4.Items
                        If ri4.ItemIndex <> -1 Then
                            If Not CType(ri4.FindControl("ProductCode"), Label).Text = pCode Then
                                'If the product code does not match then we are not 
                                'interested in this product
                                isItemProcessedPromoCheck = False
                                Exit For
                            Else
                                isItemProcessedPromoCheck = True
                                Dim promotionHyperLink As HyperLink = CType(ri4.FindControl("hplPromotions"), HyperLink)
                                Dim priceCodeDDL As DropDownList = CType(ri4.FindControl("priceCodeDDL"), DropDownList)
                                showOverridePromotionPriceCodeCheckBox = (promotionHyperLink.Visible AndAlso priceCodeDDL.Visible)
                                If showOverridePromotionPriceCodeCheckBox Then
                                    If (priceCodeDDL IsNot Nothing) AndAlso (priceCodeDDL.Visible) Then
                                        str = str & "document.getElementById('" & priceCodeDDL.ClientID & "').disabled=false;"
                                    End If
                                End If
                            End If
                        End If
                    Next
                    If isItemProcessedPromoCheck AndAlso (Not showOverridePromotionPriceCodeCheckBox) Then
                        CType(sender, CheckBox).Visible = False
                    Else
                        If Not String.IsNullOrEmpty(str) Then
                            CType(sender, CheckBox).CausesValidation = False
                            CType(sender, CheckBox).Attributes.Add("onclick", str)
                        End If
                    End If
                Next
            Next

        Next

    End Sub

    Protected Sub DoRemoveTicket(ByVal sender As Object, ByVal e As EventArgs)
        Dim master As RepeaterItem = CType(CType(CType(sender, Button).Parent, HtmlTableCell).Parent, RepeaterItem)
        Dim ProductCode As Label = CType(master.FindControl("ProductCode"), Label)
        Dim hdSeatLabel As HiddenField = CType(master.FindControl("hdSeatlabel"), HiddenField)
        Dim PriceCode As Label = CType(master.FindControl("PriceCode"), Label)
        Dim PackageID As Label = CType(master.FindControl("PackageIDForQuantity"), Label)
        Dim CustomerDDL As DropDownList = CType(master.FindControl("CustomerDDL"), DropDownList)
        Dim hdfBulkSalesID As HiddenField = CType(master.FindControl("hdfBulkSalesID"), HiddenField)
        Dim customerNumber As String = GlobalConstants.GENERIC_CUSTOMER_NUMBER
        If Not Profile.IsAnonymous Then
            customerNumber = CustomerDDL.SelectedValue
        End If
        Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
        If AgentProfile.BulkSalesMode AndAlso TEBUtilities.CheckForDBNull_Int(hdfBulkSalesID.Value) > 0 Then
            ticketingGatewayFunctions.Basket_RemoveBulkRecordFromBasket(True, String.Empty, customerNumber, hdfBulkSalesID.Value)
        Else
            ticketingGatewayFunctions.Basket_RemoveFromBasket(True, String.Empty, ProductCode.Text, hdSeatLabel.Value, PriceCode.Text, PackageID.Text, customerNumber)
        End If
    End Sub

    Protected Sub btnSaveFavouriteSeat_OnClick(ByVal sender As Object, ByVal e As EventArgs)
        Dim seatRepeaterItem As RepeaterItem = CType(sender, Button).Parent.Parent
        Dim hdSeatLabel As HiddenField = CType(seatRepeaterItem.FindControl("hdSeatLabel"), HiddenField)
        If hdSeatLabel.Value.Length > 0 Then
            Dim deCustomerDetailsV11 As New Talent.Common.DECustomerV11
            Dim deCustomerDetails As New Generic.List(Of Talent.Common.DECustomer)
            deCustomerDetails.Add(New Talent.Common.DECustomer)
            deCustomerDetails(0).CustomerNumber = Profile.User.Details.LoginID
            deCustomerDetails(0).SingleFieldMode = Talent.Common.SingleModeFieldsEnum.FSEAT
            deCustomerDetails(0).FavouriteStand = Talent.eCommerce.Utilities.GetStandFromSeatDetails(hdSeatLabel.Value)
            deCustomerDetails(0).FavouriteArea = Talent.eCommerce.Utilities.GetAreaFromSeatDetails(hdSeatLabel.Value)
            deCustomerDetails(0).FavouriteRow = Talent.eCommerce.Utilities.GetRowFromSeatDetails(hdSeatLabel.Value)
            deCustomerDetails(0).FavouriteSeat = Talent.eCommerce.Utilities.GetSeatFromSeatDetails(hdSeatLabel.Value)
            deCustomerDetailsV11.DECustomersV1 = deCustomerDetails
            Dim err As New Talent.Common.ErrorObj
            Dim talentCust As New Talent.Common.TalentCustomer
            With talentCust
                .DeV11 = deCustomerDetailsV11
                With .Settings
                    .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                    .BusinessUnit = TalentCache.GetBusinessUnit
                    .Partner = TalentCache.GetPartner(Profile)
                    .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
                End With
                err = .UpdateCustomerDetailsSingleMode()
            End With
            If (Not err.HasError) AndAlso talentCust.ResultDataSet IsNot Nothing Then
                If talentCust.ResultDataSet.Tables.Count > 0 AndAlso talentCust.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                    err.HasError = True
                Else
                    Profile.User.Details.Favourite_Seat = hdSeatLabel.Value.Trim
                    Profile.Save()
                    Dim plhFavouriteSeatMessage As New PlaceHolder
                    plhFavouriteSeatMessage = CType(Me.Parent.Parent.FindControl("plhFavouriteSeatMessage"), PlaceHolder)
                    If plhFavouriteSeatMessage IsNot Nothing Then
                        plhFavouriteSeatMessage.Visible = True
                        CType(Me.Parent.Parent.FindControl("ltlFavouriteSeatMessage"), Literal).Text = _ucr.Content("FavouriteSeatSaved", _languageCode, True)
                    End If
                End If
            Else
                err.HasError = True
            End If

            If err.HasError Then
                CType(Me.Parent.Parent.FindControl("plhFavouriteSeatMessage"), PlaceHolder).Visible = False
                Dim errMessage As String = _errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
                                            Talent.eCommerce.Utilities.GetCurrentPageName, _
                                            "FailedSavingFavSeat").ERROR_MESSAGE
                Dim errorList As BulletedList = Me.Parent.Parent.FindControl("ErrorList")
                If Not errorList Is Nothing Then
                    If errorList.Items.FindByText(errMessage) Is Nothing Then errorList.Items.Add(errMessage)
                End If
            End If

            deCustomerDetailsV11 = Nothing
            deCustomerDetails = Nothing
            talentCust = Nothing
        End If
    End Sub

    Protected Sub FreeItemsRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles FreeItemsRepeater.ItemDataBound
        If e.Item.ItemIndex > -1 Then
            Dim ProductCode As Label = CType(e.Item.FindControl("ProductCode"), Label)
            Dim Description As Label = CType(e.Item.FindControl("Description"), Label)
            Dim PriceBand As Label = CType(e.Item.FindControl("PriceBand"), Label)
            Dim MemberNumber As Label = CType(e.Item.FindControl("MemberNumber"), Label)
            Dim SeatDetails As Label = CType(e.Item.FindControl("SeatDetails"), Label)
            Dim tbi As TalentBasketItem = CType(e.Item.DataItem, TalentBasketItem)
            Try
                Dim tbl As DataTable = CurrentProductDetailsResultSet.Tables(1)
                For Each rw As DataRow In tbl.Rows
                    If UCase(Utilities.CheckForDBNull_String(rw("PriceBand"))) = UCase(tbi.PRICE_BAND) Then
                        PriceBand.Text = Utilities.CheckForDBNull_String(rw("PriceBandDescription"))
                        Exit For
                    End If
                Next
            Catch ex As Exception
            End Try

            ProductCode.Text = tbi.Product
            Description.Text = tbi.PRODUCT_DESCRIPTION1
            If String.IsNullOrEmpty(PriceBand.Text) Then PriceBand.Text = tbi.PRICE_BAND
            MemberNumber.Text = tbi.LOGINID
            SeatDetails.Text = tbi.SEAT

            ' Only display the seat information for PPS schemes.  
            If Not tbi.PRODUCT_TYPE.Equals("P") Then
                SeatDetails.Visible = False
            End If
        End If
    End Sub

#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Build a string of changed items based on the ticketing basket. Pass in the flag to determine if the bulk sales limit has been exceeded
    ''' </summary>
    ''' <param name="ExceededBulkSalesBasketLimit">The bulk sales limit exceeded flag</param>
    ''' <param name="forceBasketUpdate">Flag to force reading the basket items, even if nothing has changed</param>
    ''' <returns>Formatted string of basket item updates</returns>
    ''' <remarks></remarks>
    Public Function GetUpdatedItemsQueryString(ByRef ExceededBulkSalesBasketLimit As Boolean, Optional forceBasketUpdate As Boolean = False) As String
        Dim redirectQuery As New StringBuilder
        Dim updatedItems As Generic.List(Of RepeaterItem) = GetChangedSeatLevelRepeaterItems(ExceededBulkSalesBasketLimit, forceBasketUpdate)
        Dim tempCustomer As String = String.Empty
        Dim i As Integer = 0
        If updatedItems.Count > 0 Then
            For Each updateItem As RepeaterItem In updatedItems
                i += 1
                Dim updateItemProdCode As Label = CType(updateItem.FindControl("ProductCode"), Label)
                Dim updateItemOCust As Label = CType(updateItem.FindControl("OriginalCustomer"), Label)
                Dim updateItemOBand As Label = CType(updateItem.FindControl("OriginalBand"), Label)
                Dim updateItemCustDDL As DropDownList = CType(updateItem.FindControl("CustomerDDL"), DropDownList)
                Dim updateItemPBandDDL As DropDownList = CType(updateItem.FindControl("PriceBandDDL"), DropDownList)
                Dim updateItemFulfilment As Label = CType(updateItem.FindControl("OriginalFulfilment"), Label)
                Dim updateItemFulfilmentCode As Label = CType(updateItem.FindControl("OriginalFulfilmentCode"), Label)
                Dim updateItemSeatlbl As Literal = CType(updateItem.FindControl("Seatlabel"), Literal)
                Dim updateItemPriceLbl As Label = CType(updateItem.FindControl("PriceLabel"), Label)
                Dim updateItemPriceCode As Label = CType(updateItem.FindControl("PriceCode"), Label)
                Dim updateItemProductType As Label = CType(updateItem.FindControl("ProductType"), Label)
                Dim updateItemPCodeDDL As DropDownList = CType(updateItem.FindControl("PriceCodeDDL"), DropDownList)
                Dim updateItemHdnSeat2 As HiddenField = CType(updateItem.FindControl("hdSeatLabel2"), HiddenField)
                Dim updateItemBulkSalesID As HiddenField = CType(updateItem.FindControl("hdfBulkSalesID"), HiddenField)
                Dim updateItemBulkSalesQuantity As TextBox = CType(updateItem.FindControl("txtBulkSalesQuantity"), TextBox)
                Dim updateItemAllocatedSeat As HiddenField = CType(updateItem.FindControl("hdfNewAllocatedSeatString"), HiddenField)
                Dim packageIDLabel As Label = CType(updateItem.FindControl("PackageIDForQuantity"), Label)

                redirectQuery.Append("&product").Append(i.ToString).Append("=").Append(Server.UrlEncode(updateItemProdCode.Text))
                If updateItemProductType.Text = "M" Then
                    redirectQuery.Append("&seat").Append(i.ToString).Append("=").Append(Server.UrlEncode(updateItemHdnSeat2.Value.Trim))
                Else
                    redirectQuery.Append("&seat").Append(i.ToString).Append("=").Append(Server.UrlEncode(updateItemHdnSeat2.Value.Trim))
                End If
                If TEBUtilities.IsAnonymousTalent AndAlso Profile.IsAnonymous Then
                    tempCustomer = GlobalConstants.GENERIC_CUSTOMER_NUMBER
                Else
                    tempCustomer = updateItemCustDDL.SelectedValue
                End If
                redirectQuery.Append("&customer").Append(i.ToString).Append("=").Append(Server.UrlEncode(tempCustomer.PadLeft(12, "0")))
                redirectQuery.Append("&concession").Append(i.ToString).Append("=").Append(Server.UrlEncode(updateItemPBandDDL.SelectedValue))
                redirectQuery.Append("&priceCode").Append(i.ToString).Append("=").Append(Server.UrlEncode(updateItemPriceCode.Text))
                If AgentProfile.IsAgent Then
                    If (Not String.IsNullOrEmpty(Session("priceCode")) AndAlso updateItemProductType.Text = "S") Then
                        redirectQuery.Append("&priceCodeOverridden").Append(i.ToString).Append("=").Append(Server.UrlEncode(Session("priceCode")))
                        Session("priceCode") = ""
                    Else
                        redirectQuery.Append(" & priceCodeOverridden").Append(i.ToString).Append("=").Append(Server.UrlEncode(updateItemPCodeDDL.SelectedValue))
                    End If
                Else
                    redirectQuery.Append("&priceCodeOverridden").Append(i.ToString).Append("=").Append(Server.UrlEncode(""))
                End If
                redirectQuery.Append("&productType").Append(i.ToString).Append("=").Append(Server.UrlEncode(updateItemProductType.Text))
                redirectQuery.Append("&originalCust").Append(i.ToString).Append("=").Append(Server.UrlEncode(updateItemOCust.Text.PadLeft(12, "0")))
                redirectQuery.Append("&fulfilmentMethod").Append(i.ToString).Append("=").Append(Server.UrlEncode(updateItemFulfilmentCode.Text))
                redirectQuery.Append("&bulkSalesID").Append(i.ToString).Append("=").Append(Server.UrlEncode(updateItemBulkSalesID.Value))
                redirectQuery.Append("&bulkSalesQuantity").Append(i.ToString).Append("=").Append(Server.UrlEncode(updateItemBulkSalesQuantity.Text))
                redirectQuery.Append("&allocatedSeat").Append(i.ToString).Append("=").Append(Server.UrlEncode(updateItemAllocatedSeat.Value))
                If packageIDLabel.Text.Length > 0 Then
                    redirectQuery.Append("&isPackage").Append(i.ToString).Append("=Y&packageid").Append(i.ToString).Append("=").Append(packageIDLabel.Text)
                Else
                    redirectQuery.Append("&isPackage").Append(i.ToString).Append("=N")
                End If
            Next
        End If

        'Get the changed Package items
        redirectQuery.Append(GetChangedPackageLevelItems(i))

        'Set the bulk sales session error
        If ExceededBulkSalesBasketLimit Then Session("ExceededBulkSalesBasketLimit") = True
        Return redirectQuery.ToString()
    End Function

    ''' <summary>
    ''' This function retrieves all the changed basket items from what the current basket is compared to the items the user has changed in their browser.
    ''' </summary>
    ''' <param name="ExceededBulkSalesBasketLimit">Value to indicate if the quantity of tickets will exceed the bulk sales limit</param>
    ''' <param name="forceBasketUpdate">Force the basket to update even if nothing has changed on the page</param>
    ''' <returns>The changed basket items</returns>
    ''' <remarks></remarks>
    Public Function GetChangedSeatLevelRepeaterItems(ByRef ExceededBulkSalesBasketLimit As Boolean, Optional forceBasketUpdate As Boolean = False) As Generic.List(Of RepeaterItem)
        Dim items As New Generic.List(Of RepeaterItem)
        Dim selectedFulfilment As String = String.Empty
        Dim tempPriceCode As String = String.Empty
        Dim bulkSalesQuantity As Integer = 0

        If ModuleDefaults.AllowManualIntervention Then
            Session("AllowManualIntervention") = chkManualIntervention.Checked
        End If

        If TalentDefaults.IsOrderLevelFulfilmentEnabled Then
            selectedFulfilment = ddlOrderFulfilment.SelectedValue
        End If

        For Each ri1 As RepeaterItem In EventRepeater.Items 'loop through each event
            If Not TalentDefaults.IsOrderLevelFulfilmentEnabled Then
                Dim eventFulfilment As DropDownList = CType(ri1.FindControl("EventFulfilmentDDL"), DropDownList)
                If eventFulfilment IsNot Nothing Then
                    selectedFulfilment = eventFulfilment.SelectedValue
                Else
                    selectedFulfilment = String.Empty
                End If
            End If
            Dim rpt2 As Repeater = CType(ri1.FindControl("StandRepeater"), Repeater)
            For Each ri2 As RepeaterItem In rpt2.Items 'loop through each stand
                Dim rpt3 As Repeater = CType(ri2.FindControl("AreaRepeater"), Repeater)
                For Each ri3 As RepeaterItem In rpt3.Items 'loop through each area
                    Dim rpt4 As Repeater = CType(ri3.FindControl("SeatsRepeater"), Repeater)
                    For Each ri4 As RepeaterItem In rpt4.Items 'loop through each ticket/seat
                        If ri4.ItemIndex <> -1 Then
                            Dim ri4BasketDetailID As HiddenField = CType(ri4.FindControl("hdfBasketDetailID"), HiddenField)
                            Dim ri4oCust As Label = CType(ri4.FindControl("OriginalCustomer"), Label)
                            Dim ri4oBand As Label = CType(ri4.FindControl("OriginalBand"), Label)
                            Dim ri4custDDL As DropDownList = CType(ri4.FindControl("CustomerDDL"), DropDownList)
                            Dim ri4pBandDDL As DropDownList = CType(ri4.FindControl("PriceBandDDL"), DropDownList)
                            Dim ri4oFulFilment As Label = CType(ri4.FindControl("OriginalFulfilment"), Label)
                            Dim ri4oFulFilmentCode As Label = CType(ri4.FindControl("OriginalFulfilmentCode"), Label)
                            Dim ri4priceLbl As Label = CType(ri4.FindControl("PriceLabel"), Label)
                            Dim ri4priceCodeDDL As DropDownList = CType(ri4.FindControl("PriceCodeDDL"), DropDownList)
                            Dim ri4oPriceCode As Label = CType(ri4.FindControl("OriginalPriceCode"), Label)
                            Dim ri4BulkSalesID As HiddenField = CType(ri4.FindControl("hdfBulkSalesID"), HiddenField)
                            Dim ri4BulkSalesQuantity As TextBox = CType(ri4.FindControl("txtBulkSalesQuantity"), TextBox)
                            Dim riAllocatedSeat As PlaceHolder = CType(ri4.FindControl("plhAllocatedSeat"), PlaceHolder)
                            Dim hdfNewAllocatedSeatString As HiddenField = CType(ri4.FindControl("hdfNewAllocatedSeatString"), HiddenField)

                            If riAllocatedSeat IsNot Nothing AndAlso riAllocatedSeat.Visible Then
                                hdfNewAllocatedSeatString.Value = getAllocatedSeat(ri4)
                            End If
                            If ri4priceCodeDDL IsNot Nothing AndAlso ri4priceCodeDDL.Visible Then
                                tempPriceCode = ri4priceCodeDDL.SelectedValue
                            End If
                            If IsNumeric(ri4custDDL.SelectedValue) Then
                                If forceBasketUpdate Then
                                    items.Add(ri4)
                                Else
                                    If Not String.IsNullOrEmpty(TCUtilities.PadLeadingZeros(ri4oCust.Text, 12)) AndAlso Not TEBUtilities.IsAnonymousTalent AndAlso Not (TCUtilities.PadLeadingZeros(ri4oCust.Text, 12)).Contains(ri4custDDL.SelectedValue) Then
                                        If ri4oFulFilmentCode IsNot Nothing Then CType(ri4.FindControl("OriginalFulfilmentCode"), Label).Text = selectedFulfilment
                                        If (ri4priceCodeDDL IsNot Nothing AndAlso ri4priceCodeDDL.Visible) Then CType(ri4.FindControl("OriginalPriceCode"), Label).Text = tempPriceCode
                                        items.Add(ri4)
                                    ElseIf Not String.IsNullOrEmpty(ri4oBand.Text.Trim) AndAlso (Not ri4oBand.Text = ri4pBandDDL.SelectedValue AndAlso ri4pBandDDL.Visible) Then
                                        If ri4oFulFilmentCode IsNot Nothing Then CType(ri4.FindControl("OriginalFulfilmentCode"), Label).Text = selectedFulfilment
                                        If (ri4priceCodeDDL IsNot Nothing AndAlso ri4priceCodeDDL.Visible) Then CType(ri4.FindControl("OriginalPriceCode"), Label).Text = tempPriceCode
                                        items.Add(ri4)
                                    ElseIf (ri4oFulFilmentCode.Text.Trim <> "0") AndAlso (Not String.IsNullOrEmpty(selectedFulfilment)) AndAlso (Not ri4oFulFilmentCode.Text = selectedFulfilment) Then
                                        If ri4oFulFilmentCode IsNot Nothing Then CType(ri4.FindControl("OriginalFulfilmentCode"), Label).Text = selectedFulfilment
                                        If (ri4priceCodeDDL IsNot Nothing AndAlso ri4priceCodeDDL.Visible) Then CType(ri4.FindControl("OriginalPriceCode"), Label).Text = tempPriceCode
                                        items.Add(ri4)
                                    ElseIf Not String.IsNullOrEmpty(ri4oPriceCode.Text.Trim) AndAlso (Not ri4oPriceCode.Text = ri4priceCodeDDL.SelectedValue AndAlso ri4priceCodeDDL.Visible) Then
                                        If ri4oFulFilmentCode IsNot Nothing Then CType(ri4.FindControl("OriginalFulfilmentCode"), Label).Text = selectedFulfilment
                                        If (ri4priceCodeDDL IsNot Nothing AndAlso ri4priceCodeDDL.Visible) Then CType(ri4.FindControl("OriginalPriceCode"), Label).Text = tempPriceCode
                                        items.Add(ri4)
                                    ElseIf riAllocatedSeat IsNot Nothing AndAlso riAllocatedSeat.Visible Then
                                        'Do this check incase only the allocated seat details have changed.
                                        For Each basketItem As TalentBasketItem In Profile.Basket.BasketItems
                                            If basketItem.Basket_Detail_ID = ri4BasketDetailID.Value Then
                                                If basketItem.ALLOCATED_SEAT <> getAllocatedSeat(ri4) Then
                                                    items.Add(ri4)
                                                End If
                                                Exit For
                                            End If
                                        Next
                                    ElseIf AgentProfile.BulkSalesMode Then
                                        For Each item As TalentBasketItem In Profile.Basket.BasketItems
                                            If item.BULK_SALES_ID = TEBUtilities.CheckForDBNull_Decimal(ri4BulkSalesID.Value) Then
                                                If item.Quantity <> TEBUtilities.CheckForDBNull_Decimal(ri4BulkSalesQuantity.Text) Then items.Add(ri4)
                                                Exit For
                                            End If
                                        Next
                                    End If
                                End If
                            End If
                            bulkSalesQuantity += TEBUtilities.CheckForDBNull_Int(ri4BulkSalesQuantity.Text)
                        End If
                    Next
                Next
            Next
        Next

        ExceededBulkSalesBasketLimit = (AgentProfile.BulkSalesMode AndAlso bulkSalesQuantity > ModuleDefaults.BulkSalesModeBasketLimit)

        Return items
    End Function

    Public Function GetChangedPackageLevelItems(ByVal i As Integer) As String
        Dim ret As String = String.Empty
        Dim selectedFulfilment As String = String.Empty

        If ModuleDefaults.AllowManualIntervention Then
            Session("AllowManualIntervention") = chkManualIntervention.Checked
        End If

        If TalentDefaults.IsOrderLevelFulfilmentEnabled Then
            selectedFulfilment = ddlOrderFulfilment.SelectedValue
        End If

        For Each ri1 As RepeaterItem In EventRepeater.Items 'loop through each event
            If Not TalentDefaults.IsOrderLevelFulfilmentEnabled Then
                Dim eventFulfilment As DropDownList = CType(ri1.FindControl("EventFulfilmentDDL"), DropDownList)
                If eventFulfilment IsNot Nothing Then
                    selectedFulfilment = eventFulfilment.SelectedValue
                Else
                    selectedFulfilment = String.Empty
                End If
            End If
            Dim rpt2 As Repeater = CType(ri1.FindControl("StandRepeater"), Repeater)
            For Each ri2 As RepeaterItem In rpt2.Items 'loop through each stand
                Dim rpt3 As Repeater = CType(ri2.FindControl("AreaRepeater"), Repeater)
                For Each ri3 As RepeaterItem In rpt3.Items 'loop through each area
                    Dim uscPackageSummary As UserControls_Package_PackageSummary = CType(ri3.FindControl("uscPackageSummary"), UserControls_Package_PackageSummary)
                    If Not uscPackageSummary Is Nothing Then
                        Dim thisTBI As TalentBasketItem = uscPackageSummary.TBI
                        If Not thisTBI Is Nothing Then
                            If thisTBI.CURR_FULFIL_SLCTN <> selectedFulfilment Then
                                i = i + 1
                                ret += "&product" & i.ToString & "=" & thisTBI.Product
                                ret += "&customer" & i.ToString & "=" & thisTBI.LOGINID
                                ret += "&concession" & i.ToString & "=" & thisTBI.PRICE_BAND
                                ret += "&priceCode" & i.ToString & "=" & thisTBI.PRICE_CODE
                                ret += "&priceCodeOverridden" & i.ToString & "=" & Server.UrlEncode("")
                                ret += "&productType" & i.ToString & "=" & thisTBI.PRODUCT_TYPE_ACTUAL
                                ret += "&originalCust" & i.ToString & "=" & thisTBI.LOGINID
                                ret += "&fulfilmentMethod" & i.ToString & "=" & selectedFulfilment
                                ret += "&isPackage" & i.ToString & "=Y"
                                ret += "&seat" & i.ToString & "=" & thisTBI.PACKAGE_ID
                                If thisTBI.BULK_SALES_ID <> 0 Then
                                    ret += "&bulkSalesID" & i.ToString & "=" & thisTBI.BULK_SALES_ID
                                    ret += "&bulkSalesQuantity" & i.ToString & "=" & thisTBI.Quantity
                                End If
                            End If
                        End If
                    End If
                Next
            Next
        Next
        Return ret
    End Function

    Public Function GetTextTable(ByVal ContentName As String) As String
        Return _ucr.Content(ContentName, _languageCode, True)
    End Function

#End Region

#Region "Private Functions"

    Private Function GetOriginatingSource(ByVal agentFromSession As String) As String
        Dim originatingSource As String = String.Empty
        If ModuleDefaults.TicketingKioskMode Then
            originatingSource = "KIOSK"
        Else
            If Not agentFromSession Is Nothing Then
                originatingSource = Convert.ToString(agentFromSession)
            Else
                originatingSource = String.Empty
            End If
        End If
        Return originatingSource
    End Function

    Private Function LoadPackageDetail(ByVal productCode As String, ByVal packageID As String) As DataRow
        Dim product As New Talent.Common.TalentProduct
        Dim settings As Talent.Common.DESettings = Utilities.GetSettingsObject()
        Dim err As New Talent.Common.ErrorObj
        Dim dtPackageList As New DataTable
        Dim packageRow As DataRow = Nothing
        settings.Cacheing = CType(_ucr.Attribute("Cacheing"), Boolean)
        settings.CacheTimeMinutes = CType(_ucr.Attribute("CacheTimeMinutes"), Integer)
        settings.CacheDependencyPath = ModuleDefaults.CacheDependencyPath
        product.Settings() = settings
        product.De.ProductCode = productCode
        product.De.Src = "W"
        err = product.ProductHospitality
        If (Not err.HasError) AndAlso (product.ResultDataSet IsNot Nothing) Then
            dtPackageList = product.ResultDataSet.Tables(1)
        End If
        Dim drPackageList() As DataRow = dtPackageList.Select("PackageID='" & Talent.Common.Utilities.PadLeadingZeros(packageID, 13) & "'")
        If drPackageList.Length > 0 Then
            packageRow = drPackageList(0)
        Else
            packageRow = Nothing
        End If
        Return packageRow
    End Function

    Private Function GetProductPriceCodesDescription(ByVal productCode As String, ByVal productType As String, ByVal stadiumCode As String, ByVal dtProductPriceCodes As DataTable, ByVal dtCampaignPriceCodes As DataTable) As DataTable
        Dim talPricing As New Talent.Common.TalentPricing
        Dim settings As Talent.Common.DESettings = Utilities.GetSettingsObject()
        Dim err As New Talent.Common.ErrorObj
        Dim dtProductPriceCodeDesc As New DataTable
        settings.Cacheing = CType(_ucr.Attribute("Cacheing"), Boolean)
        settings.CacheTimeMinutes = CType(_ucr.Attribute("CacheTimeMinutes"), Integer)
        settings.CacheDependencyPath = ModuleDefaults.CacheDependencyPath
        talPricing.Settings() = settings
        talPricing.De.ProductCode = productCode
        talPricing.De.ProductType = productType
        talPricing.De.StadiumCode = stadiumCode
        talPricing.De.Src = "W"
        err = talPricing.ProductPriceCodeDescriptions(dtProductPriceCodes, dtCampaignPriceCodes)
        If (Not err.HasError) AndAlso (talPricing.ResultDataSet IsNot Nothing) Then
            If talPricing.ResultDataSet.Tables("Status").Rows.Count > 0 AndAlso talPricing.ResultDataSet.Tables("Status").Rows(0)(0) = "" Then
                dtProductPriceCodeDesc = talPricing.ResultDataSet.Tables("ProductPriceCodes")
            End If
        End If
        Return dtProductPriceCodeDesc
    End Function

    Private Function GetStadiumPriceCodesDescription(ByVal stadiumCode As String) As DataTable
        Dim talPricing As New Talent.Common.TalentPricing
        Dim settings As Talent.Common.DESettings = Utilities.GetSettingsObject()
        Dim err As New Talent.Common.ErrorObj
        Dim dtStadiumPriceCodeDesc As New DataTable
        settings.Cacheing = CType(_ucr.Attribute("Cacheing"), Boolean)
        settings.CacheTimeMinutes = CType(_ucr.Attribute("CacheTimeMinutes"), Integer)
        settings.CacheDependencyPath = ModuleDefaults.CacheDependencyPath
        talPricing.Settings() = settings
        talPricing.De.StadiumCode = stadiumCode
        talPricing.De.Src = "W"
        err = talPricing.StadiumPriceCodeDescriptions()
        If (Not err.HasError) AndAlso (talPricing.ResultDataSet IsNot Nothing) Then
            If talPricing.ResultDataSet.Tables("Status").Rows.Count > 0 AndAlso talPricing.ResultDataSet.Tables("Status").Rows(0)(0) = "" Then
                dtStadiumPriceCodeDesc = talPricing.ResultDataSet.Tables("StadiumPriceCodes")
            End If
        End If
        Return dtStadiumPriceCodeDesc
    End Function

    Private Function FindPriceBandDescription(ByVal dr As Data.DataRow) As String
        Dim ret As String = String.Empty
        If Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(dr("PRICE_BAND"))) Then
            Dim tbl1, tbl2 As DataTable
            If _dicProductDetailsByPriceCode.ContainsKey(dr("PRODUCT") & dr("PRICE_CODE")) Then
                Dim ds As DataSet = _dicProductDetailsByPriceCode.Item(dr("PRODUCT") & dr("PRICE_CODE"))
                tbl1 = ds.Tables(0)
                tbl2 = ds.Tables(1)
            Else
                tbl1 = CurrentProductDetailsResultSet.Tables(0)
                tbl2 = CurrentProductDetailsResultSet.Tables(1)
            End If

            If CurrentProductDetailsResultSet.Tables.Count > 1 Then
                If String.IsNullOrEmpty(Utilities.CheckForDBNull_String(tbl1.Rows(0)(0))) Then
                    For Each rw As DataRow In tbl2.Rows
                        If Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(rw("PriceBand"))) Then
                            If Utilities.CheckForDBNull_String(dr("PRICE_BAND")) = Utilities.CheckForDBNull_String(rw("PriceBand")) Then
                                If String.IsNullOrEmpty(ModuleDefaults.Culture) Or ModuleDefaults.Culture = "en-GB" Then
                                    ret = Utilities.CheckForDBNull_String(rw("PriceBandDescription"))
                                    Exit For
                                Else
                                    ret = Server.HtmlEncode(Utilities.CheckForDBNull_String(rw("PriceBandDescription")))
                                    Exit For
                                End If
                            End If
                        End If
                    Next
                End If
            End If
        End If
        Return ret
    End Function

    ''' <summary>
    ''' Get the customer name based on the given customer number from the customer data table
    ''' </summary>
    ''' <param name="customerNumber">The given customer number to work with</param>
    ''' <param name="dtCustomers">The data set of customer information</param>
    ''' <returns>The formatted customer name</returns>
    ''' <remarks></remarks>
    Private Function getCustomerNameFromCustomerNumber(ByVal customerNumber As String, ByRef dtCustomers As DataSet) As String
        Dim customerName As String = String.Empty
        If Profile.User.Details.LoginID <> customerNumber AndAlso dtCustomers IsNot Nothing _
            AndAlso dtCustomers.Tables.Count > 1 AndAlso dtCustomers.Tables(1).Rows.Count > 0 Then
            For Each row As DataRow In dtCustomers.Tables(1).Rows
                If row("AssociatedCustomerNumber").ToString() = customerNumber Then
                    customerName = row("Forename").ToString().Trim() & " " & row("Surname").ToString().Trim()
                    Exit For
                End If
            Next
        Else
            customerName = Profile.User.Details.Full_Name
        End If
        customerName = " - " & customerName
        Return customerName
    End Function

    ''' <summary>
    ''' Get the proper fulfilment text value based on the given fulfilment code
    ''' </summary>
    ''' <param name="fulfilmentCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getFulfilmentTextByFulfilmentCode(ByVal fulfilmentCode As String) As String
        Dim fulfilmentText As String = String.Empty
        Select Case fulfilmentCode
            Case GlobalConstants.POST_FULFILMENT : fulfilmentText = _ucr.Content("Post", _languageCode, True)
            Case GlobalConstants.COLLECT_FULFILMENT : fulfilmentText = _ucr.Content("Collect", _languageCode, True)
            Case GlobalConstants.PRINT_AT_HOME_FULFILMENT : fulfilmentText = _ucr.Content("PrintAtHome", _languageCode, True)
            Case GlobalConstants.SMARTCARD_UPLOAD_FULFILMENT : fulfilmentText = _ucr.Content("SmartcardUpload", _languageCode, True)
            Case GlobalConstants.REG_POST_FULFILMENT : fulfilmentText = _ucr.Content("RegPost", _languageCode, True)
            Case GlobalConstants.PRINT_FULFILMENT : fulfilmentText = _ucr.Content("Print", _languageCode, True)
        End Select
        Return fulfilmentText
    End Function

    ''' <summary>
    ''' Determine whether or not to show the seat allocation text boxes and if they're shown populated them based on the details in the basket
    ''' This uses the Seat Details data entity to format the seat without spaces in the text boxes
    ''' </summary>
    ''' <param name="e">The basket item repeater event argument object</param>
    ''' <param name="allocatedSeatString">The allocated seat string from the basket item needed to show in the text boxes</param>
    ''' <returns>Visibility of the option against the basket item</returns>
    ''' <remarks></remarks>
    Private Function setAllocatedSeat(ByRef e As RepeaterItemEventArgs, ByVal allocatedSeatString As String) As Boolean
        Dim isAllocatedSeatOptionVisible As Boolean = False
        If Not Profile.IsAnonymous AndAlso AgentProfile.IsAgent AndAlso Not AgentProfile.BulkSalesMode Then
            isAllocatedSeatOptionVisible = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowAllocatedSeatOption"))
            If isAllocatedSeatOptionVisible Then
                Dim txtAllocatedStand As TextBox = CType(e.Item.FindControl("txtAllocatedStand"), TextBox)
                Dim txtAllocatedArea As TextBox = CType(e.Item.FindControl("txtAllocatedArea"), TextBox)
                Dim txtAllocatedRow As TextBox = CType(e.Item.FindControl("txtAllocatedRow"), TextBox)
                Dim txtAllocatedSeatNumber As TextBox = CType(e.Item.FindControl("txtAllocatedSeatNumber"), TextBox)
                Dim txtAllocatedSeatSuffix As TextBox = CType(e.Item.FindControl("txtAllocatedSeatSuffix"), TextBox)
                Dim rgxAllocatedStand As RegularExpressionValidator = CType(e.Item.FindControl("rgxAllocatedStand"), RegularExpressionValidator)
                Dim rgxAllocatedArea As RegularExpressionValidator = CType(e.Item.FindControl("rgxAllocatedArea"), RegularExpressionValidator)
                Dim rgxAllocatedRow As RegularExpressionValidator = CType(e.Item.FindControl("rgxAllocatedRow"), RegularExpressionValidator)
                Dim rgxAllocatedSeatNumber As RegularExpressionValidator = CType(e.Item.FindControl("rgxAllocatedSeatNumber"), RegularExpressionValidator)
                Dim rgxAllocatedSeatSuffix As RegularExpressionValidator = CType(e.Item.FindControl("rgxAllocatedSeatSuffix"), RegularExpressionValidator)
                Dim ltlAllocatedSeat As Literal = CType(e.Item.FindControl("ltlAllocatedSeat"), Literal)
                Dim allocatedSeat As New Talent.Common.DESeatDetails

                allocatedSeat.FormattedSeat = allocatedSeatString
                If Utilities.GetCurrentPageName().ToUpper.Equals("BASKET.ASPX") Then
                    txtAllocatedStand.Text = allocatedSeat.Stand
                    txtAllocatedArea.Text = allocatedSeat.Area
                    txtAllocatedRow.Text = allocatedSeat.Row
                    txtAllocatedSeatNumber.Text = allocatedSeat.Seat
                    txtAllocatedSeatSuffix.Text = allocatedSeat.AlphaSuffix
                    rgxAllocatedStand.ErrorMessage = _ucr.Content("InvalidAllocatedStandErrorMessage", _languageCode, True)
                    rgxAllocatedArea.ErrorMessage = _ucr.Content("InvalidAllocatedAreaErrorMessage", _languageCode, True)
                    rgxAllocatedRow.ErrorMessage = _ucr.Content("InvalidAllocatedRowErrorMessage", _languageCode, True)
                    rgxAllocatedSeatNumber.ErrorMessage = _ucr.Content("InvalidAllocatedSeatNumberErrorMessage", _languageCode, True)
                    rgxAllocatedSeatSuffix.ErrorMessage = _ucr.Content("InvalidAllocatedSeatSuffixErrorMessage", _languageCode, True)
                    ltlAllocatedSeat.Visible = False
                Else
                    txtAllocatedStand.Visible = False
                    txtAllocatedArea.Visible = False
                    txtAllocatedRow.Visible = False
                    txtAllocatedSeatNumber.Visible = False
                    txtAllocatedSeatSuffix.Visible = False
                    ltlAllocatedSeat.Visible = True
                    ltlAllocatedSeat.Text = allocatedSeat.FormattedSeat
                End If
            End If
        End If
        Return isAllocatedSeatOptionVisible
    End Function

    ''' <summary>
    ''' Get the formatted allocated seat details from the text boxes based on the item being worked with
    ''' </summary>
    ''' <param name="item">The current repeater item</param>
    ''' <returns>The formatted allocated seat details based on the text boxes</returns>
    ''' <remarks></remarks>
    Private Function getAllocatedSeat(ByRef item As RepeaterItem) As String
        Dim formattedString As String = String.Empty
        Dim allocatedSeat As New Talent.Common.DESeatDetails
        Dim txtAllocatedStand As TextBox = CType(item.FindControl("txtAllocatedStand"), TextBox)
        Dim txtAllocatedArea As TextBox = CType(item.FindControl("txtAllocatedArea"), TextBox)
        Dim txtAllocatedRow As TextBox = CType(item.FindControl("txtAllocatedRow"), TextBox)
        Dim txtAllocatedSeatNumber As TextBox = CType(item.FindControl("txtAllocatedSeatNumber"), TextBox)
        Dim txtAllocatedSeatSuffix As TextBox = CType(item.FindControl("txtAllocatedSeatSuffix"), TextBox)
        allocatedSeat.Stand = txtAllocatedStand.Text.Trim().ToUpper()
        allocatedSeat.Area = txtAllocatedArea.Text.Trim().ToUpper()
        allocatedSeat.Row = txtAllocatedRow.Text.Trim().ToUpper()
        allocatedSeat.Seat = txtAllocatedSeatNumber.Text.Trim().ToUpper()
        allocatedSeat.AlphaSuffix = txtAllocatedSeatSuffix.Text.Trim().ToUpper()
        formattedString = allocatedSeat.FormattedSeat
        Return formattedString
    End Function

#End Region

#Region "Private Methods"

    Private Sub HandleBasketErrors()
        Dim sErrorCode As String = String.Empty
        Dim sErrorText As String = String.Empty
        Dim doneErrorX As Boolean = False

        Dim bindTheErrorListRepater As Boolean = False
        _errorList = CType(Utilities.FindWebControl("ErrorList", Me.Page.Controls), BulletedList)

        'New Code
        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            sErrorCode = Utilities.CheckForDBNull_String(tbi.STOCK_ERROR_CODE).Trim
            If sErrorCode <> "" Then
                '
                ' If current page is CheckOutPaymentDetails.aspx then return to Basket.
                ' Else, display errors in Error_Label on BasketDetails.ascx 
                '
                If UCase(Utilities.GetCurrentPageName) = "CHECKOUT.ASPX" Then
                    Session("ResetErrorLabel") = "Y"
                    'Allow continuation to Checkout if the items are discontinued
                    If sErrorCode <> "DISC" Then
                        Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                    End If
                Else
                    Select Case Usage.ToUpper
                        Case "ORDER", "PAYMENT"
                            'do nothing
                        Case Else
                            ' Display appropriate error code
                            sErrorText = Utilities.TicketingBasketErrors(_errMsg, tbi)
                            If AgentProfile.IsAgent Then
                                If sErrorText <> String.Empty AndAlso Not errorListForRepeater.ContainsKey(sErrorText) Then
                                    errorListForRepeater.Add(sErrorText, tbi)
                                    bindTheErrorListRepater = True
                                End If
                            Else
                                If sErrorText <> String.Empty AndAlso Not _errorList.Items.Contains(New ListItem(sErrorText)) Then
                                    _errorList.Items.Add(sErrorText)
                                End If
                            End If
                    End Select
                End If
            End If
        Next

        'Part refund not allowed if original sale was paid for using credit finance, but may cancel fully
        If Profile.Basket.CAT_MODE <> GlobalConstants.CATMODE_CANCEL Then
            If Profile.Basket.BasketSummary.TotalBasket < 0 AndAlso Profile.Basket.ORIGINAL_SALE_PAID_WITH_CF Then
                sErrorText = _errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
                                          Talent.eCommerce.Utilities.GetCurrentPageName, _
                                          "CantPartRefundCreditFinanceSale").ERROR_MESSAGE
                If sErrorText <> String.Empty AndAlso Not _errorList.Items.Contains(New ListItem(sErrorText)) Then
                    _errorList.Items.Add(sErrorText)
                End If
            End If
        End If

        'Bulk Sales Limit check
        If AgentProfile.BulkSalesMode Then
            Dim showError As Boolean = False
            If Session("ExceededBulkSalesBasketLimit") IsNot Nothing AndAlso Session("ExceededBulkSalesBasketLimit") = True Then
                showError = True
                Session("ExceededBulkSalesBasketLimit") = Nothing
            Else
                showError = (Profile.Basket.BasketSummary.TotalItemsTicketing > ModuleDefaults.BulkSalesModeBasketLimit)
            End If
            If showError Then
                sErrorText = _errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, Talent.eCommerce.Utilities.GetCurrentPageName, "ExceededBulkSalesBasketLimit").ERROR_MESSAGE
                sErrorText = sErrorText.Replace("<<BULK_SALES_LIMIT>>", ModuleDefaults.BulkSalesModeBasketLimit)
                If sErrorText <> String.Empty AndAlso Not _errorList.Items.Contains(New ListItem(sErrorText)) Then
                    _errorList.Items.Add(sErrorText)
                End If
            End If
        End If
    End Sub

    Private Sub BindEventRepeater()
        'Create the basket data table
        Dim dt As New DataTable
        With dt.Columns
            .Add("PRODUCT", GetType(String))
        End With

        'New Code
        Dim dRow As DataRow = Nothing
        Dim sProduct As String = String.Empty
        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            If UCase(tbi.MODULE_) = "TICKETING" AndAlso tbi.FEE_CATEGORY = "" AndAlso Not tbi.IS_FREE Then
                If Not Utilities.IsTicketingFee(tbi.MODULE_, tbi.Product.Trim, tbi.FEE_CATEGORY) Then 'Product cannot be a fee
                    If tbi.Product.Trim <> sProduct Then 'Is this a new product
                        If String.IsNullOrEmpty(SelectProductType) Or Not String.IsNullOrEmpty(SelectProductType) AndAlso tbi.PRODUCT_TYPE = SelectProductType Then 'Is it the right type
                            If String.IsNullOrEmpty(SelectProductSubType) Or Not String.IsNullOrEmpty(SelectProductSubType) AndAlso tbi.PRODUCT_SUB_TYPE = SelectProductSubType Then

                                ' For CAT-Cancelation Baskets we can only display specific items
                                If Not _displayItemsInCancBasket Or (_displayItemsInCancBasket And tbi.DISPLAY_IN_A_CANCEL_BASKET) Then

                                    'Add the new product to the datatable
                                    'Ensure it hasn't already been added
                                    Dim productAlreadyAdded As Boolean = False
                                    For Each row As DataRow In dt.Rows
                                        If row("PRODUCT") = tbi.Product.Trim Then productAlreadyAdded = True
                                    Next
                                    If Not productAlreadyAdded Then
                                        dRow = Nothing
                                        dRow = dt.NewRow
                                        dRow("PRODUCT") = tbi.Product.Trim
                                        dt.Rows.Add(dRow)
                                    End If

                                    'Save the product
                                    sProduct = tbi.Product.Trim
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        Next
        If dt.Rows.Count > 0 Then
            EventRepeater.DataSource = dt
            EventRepeater.DataBind()
        End If
    End Sub

    Private Sub BindFreeItemsRepeater()
        Dim freeItems As New Generic.List(Of TalentBasketItem)
        Dim freePPS As Boolean = False
        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            If UCase(tbi.MODULE_) = "TICKETING" AndAlso tbi.IS_FREE Then
                If String.IsNullOrEmpty(SelectProductType) Or Not String.IsNullOrEmpty(SelectProductType) AndAlso tbi.PRODUCT_TYPE = SelectProductType Then 'Is it the right type
                    If String.IsNullOrEmpty(SelectProductSubType) Or Not String.IsNullOrEmpty(SelectProductSubType) AndAlso tbi.PRODUCT_SUB_TYPE = SelectProductSubType Then
                        freeItems.Add(tbi)

                        'Set visibility flags
                        Select Case tbi.PRODUCT_TYPE.Trim
                            Case Is = "P" : freePPS = True
                        End Select
                    End If
                End If
            End If
        Next

        If freeItems.Count > 0 Then
            FreeItemsRepeater.DataSource = freeItems
            FreeItemsRepeater.DataBind()

            ' Set visibilty of header fields
            If Not freePPS Then
                CType(Utilities.FindWebControl("ltlFreeItemsSeatDetailsHeader", Me.Controls), Literal).Visible = False
            End If
        Else
            FreeItemsRepeater.Visible = False
        End If
    End Sub

    Private Sub CustomerSearchLink()
        If _displayCustomerSearchLink Then
            Dim searchLinkVisibility As String = _ucr.Attribute("CustomerSearchLinkVisible").ToUpper
            If searchLinkVisibility = "TRUE" And AgentProfile.IsAgent And Session("AgentType") = "2" Then
                hlkCustomerSearch.Text = _ucr.Content("CustomerSearchLinkText", _languageCode, True)
                hlkCustomerSearch.NavigateUrl = "~/PagesLogin/Profile/CustomerSelection.aspx"
                plhCustomerSearch.Visible = True
            Else
                plhCustomerSearch.Visible = False
            End If
        Else
            plhCustomerSearch.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Set the order level fulfilment option if it is enabled. The smartcard upload fulfilment option won't work in conjuction with this fulfilment type.
    ''' </summary>
    ''' <remarks></remarks>

    Private Sub setOrderLevelFulfilment()
        If TalentDefaults.IsOrderLevelFulfilmentEnabled AndAlso Utilities.GetCurrentPageName().ToUpper = "BASKET.ASPX" And
            (Profile.Basket.CAT_MODE <> GlobalConstants.CATMODE_CANCEL And Profile.Basket.CAT_MODE <> GlobalConstants.CATMODE_CANCELALL) Then

            Dim hasCollectFulfilment As Boolean = True
            Dim hasPostFulfilment As Boolean = True
            Dim hasRegPostFulfilment As Boolean = True
            Dim hasPrintAtHomeFulfilment As Boolean = True
            Dim hasPrintFulfilment As Boolean = True
            Dim defaultFulfilmentOption As String = String.Empty
            Dim counter As Integer = 0
            Const Y As String = "Y"
            plhOrderFulfilment.Visible = True
            lblOrderFulfilment.Text = _ucr.Content("FulfilmentLabelText", _languageCode, True)
            If _addJSSelectAttributes Then ddlOrderFulfilment.Attributes.Add("onchange", "optionsUpdated();")
            ddlOrderFulfilment.Items.Clear()
            'Loop through each item in the basket and determine what fulfilment options are available for the entire basket
            For Each item As TalentBasketItem In Profile.Basket.BasketItems

                If UCase(item.MODULE_) = "TICKETING" AndAlso item.FEE_CATEGORY = String.Empty AndAlso Not item.IS_FREE AndAlso item.PRODUCT_TYPE_ACTUAL <> GlobalConstants.SEASONTICKETPRODUCTTYPE Then
                    If Not Utilities.IsTicketingFee(item.MODULE_, item.Product.Trim, item.FEE_CATEGORY) Then
                        If hasCollectFulfilment AndAlso Not item.FULFIL_OPT_COLL = Y Then
                            hasCollectFulfilment = False
                        End If
                        If hasPostFulfilment AndAlso Not item.FULFIL_OPT_POST = Y Then
                            hasPostFulfilment = False
                        End If
                        If hasRegPostFulfilment AndAlso Not item.FULFIL_OPT_REGPOST = Y Then
                            hasRegPostFulfilment = False
                        End If
                        If hasPrintAtHomeFulfilment AndAlso Not item.FULFIL_OPT_PAH = Y Then
                            hasPrintAtHomeFulfilment = False
                        End If
                        If hasPrintFulfilment AndAlso Not item.FULFIL_OPT_PRINT = Y Then
                            hasPrintFulfilment = False
                        End If

                        'Check to see if all the fulfilment options are the same for each item in the basket
                        If counter = 0 Then
                            defaultFulfilmentOption = item.CURR_FULFIL_SLCTN
                        Else
                            If defaultFulfilmentOption <> item.CURR_FULFIL_SLCTN Then defaultFulfilmentOption = String.Empty
                        End If
                        counter += 1
                    End If
                End If
            Next
            If counter > 0 Then
                If hasCollectFulfilment Then ddlOrderFulfilment.Items.Add(New ListItem(getFulfilmentTextByFulfilmentCode(GlobalConstants.COLLECT_FULFILMENT), GlobalConstants.COLLECT_FULFILMENT))
                If hasPostFulfilment Then ddlOrderFulfilment.Items.Add(New ListItem(getFulfilmentTextByFulfilmentCode(GlobalConstants.POST_FULFILMENT), GlobalConstants.POST_FULFILMENT))
                If hasRegPostFulfilment Then ddlOrderFulfilment.Items.Add(New ListItem(getFulfilmentTextByFulfilmentCode(GlobalConstants.REG_POST_FULFILMENT), GlobalConstants.REG_POST_FULFILMENT))
                If hasPrintAtHomeFulfilment Then ddlOrderFulfilment.Items.Add(New ListItem(getFulfilmentTextByFulfilmentCode(GlobalConstants.PRINT_AT_HOME_FULFILMENT), GlobalConstants.PRINT_AT_HOME_FULFILMENT))
                If hasPrintFulfilment Then ddlOrderFulfilment.Items.Add(New ListItem(getFulfilmentTextByFulfilmentCode(GlobalConstants.PRINT_FULFILMENT), GlobalConstants.PRINT_FULFILMENT))
            End If
            If ddlOrderFulfilment.Items.Count > 0 Then
                If defaultFulfilmentOption <> String.Empty Then
                    ddlOrderFulfilment.SelectedValue = defaultFulfilmentOption
                End If
            Else
                plhOrderFulfilment.Visible = False
                rfvOrderFulfilment.Enabled = False
            End If
            If plhOrderFulfilment.Visible Then
                ddlOrderFulfilment.Items.Insert(0, New ListItem(_ucr.Content("PleaseSelectFulfilment", _languageCode, True), String.Empty))
                If defaultFulfilmentOption = String.Empty Then
                    ddlOrderFulfilment.SelectedIndex = 0
                Else
                    ddlOrderFulfilment.SelectedValue = defaultFulfilmentOption
                End If
                rfvOrderFulfilment.Enabled = True
                rfvOrderFulfilment.ErrorMessage = _ucr.Content("PleaseSelectFulfilmentErrorText", _languageCode, True)
            End If

            If ModuleDefaults.AllowManualIntervention Then
                If AgentProfile.IsAgent Then
                    plhManualIntervention.Visible = True
                    lblManualIntervention.Text = _ucr.Content("ManualIntervention", _languageCode, True)
                    If Session("AllowManualIntervention") IsNot Nothing Then
                        chkManualIntervention.Checked = Session("AllowManualIntervention")
                    End If
                Else
                    plhManualIntervention.Visible = False
                End If
            Else
                plhManualIntervention.Visible = False
            End If
            plhOrderLevel.Visible = True
            If Not plhOrderFulfilment.Visible AndAlso Not plhManualIntervention.Visible Then
                plhOrderLevel.Visible = False
            End If
        Else
            plhOrderFulfilment.Visible = False
            rfvOrderFulfilment.Enabled = False
            plhOrderLevel.Visible = False
        End If

    End Sub

    Private Sub setBasketKioskMode(ByRef customerLabel As Label, ByRef fulfilmentCol As HtmlTableCell, ByRef OriginalFulfilment As Label, ByRef bandCol As HtmlTableCell, ByRef priceCodeCol As HtmlTableCell, ByRef PriceBandDDL As DropDownList, ByRef customerDDL As DropDownList, ByRef OriginalCustomer As Label)
        If ModuleDefaults.TicketingKioskMode Then
            customerLabel.Visible = True
            customerLabel.Text = Profile.UserName.TrimStart("0") & " - "
            If Profile.User.Details.Full_Name IsNot Nothing Then customerLabel.Text = customerLabel.Text & Profile.User.Details.Full_Name
            fulfilmentCol.Visible = False
            OriginalFulfilment.Visible = False
            bandCol.Visible = False
            priceCodeCol.Visible = False
            PriceBandDDL.Visible = False
            customerDDL.Visible = False
            OriginalCustomer.Visible = False
        End If
    End Sub

    Private Sub PopulateFulFilmentList(ByVal fulfilmentValue As String, ByVal fulfilmentText As String)
        Dim foundKey As Boolean = False
        For Each Pair As KeyValuePair(Of String, String) In _fulfilmentList
            If Pair.Key = fulfilmentValue Then
                foundKey = True
                Exit For
            End If
        Next
        If Not foundKey Then
            _fulfilmentList.Add(New KeyValuePair(Of String, String)(fulfilmentValue, _ucr.Content(fulfilmentText, _languageCode, True)))
        End If
    End Sub

    Private Sub PriceBandProcessing(ByVal dr As Data.DataRow, ByRef _talentErrObj As Talent.Common.ErrorObj, ByRef OriginalBand As Label, ByRef PriceBandDDL As DropDownList, ByRef bandMessage As Label, ByRef bandCol As HtmlTableCell, _
                                    ByRef bandLoginLink As HyperLink, ByRef seatMessage As Label, ByRef PriceLabel As Label, ByRef customerDDL As DropDownList, ByRef custLoginLink As HyperLink, ByRef customerLabel As Label, ByRef OriginalCustomer As Label)
        If Not _talentErrObj.HasError Then
            Dim tbl1, tbl2 As DataTable
            If _dicProductDetailsByPriceCode.ContainsKey(dr("PRODUCT") & dr("PRICE_CODE")) Then
                Dim ds As DataSet = _dicProductDetailsByPriceCode.Item(dr("PRODUCT") & dr("PRICE_CODE"))
                tbl1 = ds.Tables(0)
                tbl2 = ds.Tables(1)
            Else
                tbl1 = CurrentProductDetailsResultSet.Tables(0)
                tbl2 = CurrentProductDetailsResultSet.Tables(1)
            End If
            If CurrentProductDetailsResultSet.Tables.Count > 1 Then
                OriginalBand.Text = Utilities.CheckForDBNull_String(dr("PRICE_BAND"))
                If String.IsNullOrEmpty(Utilities.CheckForDBNull_String(tbl1.Rows(0)(0))) Then

                    If CDec(dr("PRICE")) = 0 AndAlso dr("PRODUCT_TYPE") = "P" Then
                        PriceBandDDL.Visible = False
                        bandMessage.Text = _ucr.Content("NoPriceBandText", _languageCode, True)
                        bandMessage.Visible = True
                    Else
                        PriceBandDDL.Items.Clear()
                        Dim validPriceBands As String = dr("VALID_PRICE_BANDS").ToString
                        If validPriceBands = "DoNotDisplay" Then
                            bandCol.Visible = False
                        Else
                            Dim addCurrentPriceBand As Boolean = True
                            For Each rw As DataRow In tbl2.Rows
                                ' check if a list of valid price bands (which have prices) is returned. If so need to 
                                ' make sure the price band is valid before adding it to the drop down.
                                If validPriceBands <> String.Empty AndAlso Not validPriceBands.Contains(rw("PriceBand").ToString) Then
                                Else
                                    If Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(rw("PriceBand"))) Then
                                        If String.IsNullOrEmpty(ModuleDefaults.Culture) Or ModuleDefaults.Culture = "en-GB" Then
                                            PriceBandDDL.Items.Add(New ListItem(Utilities.CheckForDBNull_String(rw("PriceBandDescription")), Utilities.CheckForDBNull_String(rw("PriceBand"))))
                                        Else
                                            PriceBandDDL.Items.Add(New ListItem(Server.HtmlEncode(Utilities.CheckForDBNull_String(rw("PriceBandDescription"))), Server.HtmlEncode(Utilities.CheckForDBNull_String(rw("PriceBand")))))
                                        End If
                                        If Utilities.CheckForDBNull_String(rw("PriceBand")) = Utilities.CheckForDBNull_String(dr("PRICE_BAND")) Then
                                            addCurrentPriceBand = False
                                        End If
                                    End If
                                End If
                            Next
                            ' also add current price band to the drop down too if necessary (promotion-related price bands may not be web ready so not considered 'valid', above).
                            If addCurrentPriceBand And Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(dr("PRICE_BAND"))) Then
                                If String.IsNullOrEmpty(ModuleDefaults.Culture) Or ModuleDefaults.Culture = "en-GB" Then
                                    PriceBandDDL.Items.Add(New ListItem(Utilities.CheckForDBNull_String(dr("PRICE_BAND_DESCRIPTION")), Utilities.CheckForDBNull_String(dr("PRICE_BAND"))))
                                Else
                                    PriceBandDDL.Items.Add(New ListItem(Server.HtmlEncode(Utilities.CheckForDBNull_String(dr("PRICE_BAND_DESCRIPTION"))), Server.HtmlEncode(Utilities.CheckForDBNull_String(dr("PRICE_BAND")))))
                                End If
                            End If
                        End If

                        Dim index As Integer = 0
                        Dim found As Boolean = False
                        For Each item As ListItem In PriceBandDDL.Items
                            If item.Value = Utilities.CheckForDBNull_String(dr("PRICE_BAND")) Then
                                found = True
                                Exit For
                            End If
                            index += 1
                        Next

                        If found Then
                            PriceBandDDL.SelectedIndex = index
                        Else
                            Dim disabledPriceBands() As String = Split(_ucr.Attribute("DisabledPriceBands"), ",")
                            For Each priceBand As String In disabledPriceBands
                                If dr("PRICE_BAND").ToString.Equals(priceBand) Then
                                    PriceBandDDL.Visible = False
                                    bandMessage.Text = _ucr.Content("DisabledPriceBandsText", _languageCode, True)
                                    bandMessage.Visible = True
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                Else
                    bandCol.Visible = False
                End If
            Else
                bandCol.Visible = False
            End If
        End If

        If PriceBandDDL.Items.Count = 0 AndAlso bandCol.Visible = True Then
            'Attempt to find the price description otherwise just the price band code is displayed
            Dim sPriceBandDescription As String = String.Empty
            sPriceBandDescription = FindPriceBandDescription(dr)
            If sPriceBandDescription <> String.Empty Then
                PriceBandDDL.Items.Add(New ListItem(Utilities.CheckForDBNull_String(sPriceBandDescription), Utilities.CheckForDBNull_String(dr("PRICE_BAND"))))
            Else
                PriceBandDDL.Items.Add(Utilities.CheckForDBNull_String(dr("PRICE_BAND")))
            End If
        End If

        If Not (UCase(Usage) = "ORDER" Or UCase(Usage) = "PAYMENT") Then
            Dim allowReassignmentOfReservedSeats As Boolean = False
            If Not Profile.IsAnonymous AndAlso Not ModuleDefaults.AllowReassignmentOfReservedSeats Is Nothing AndAlso ModuleDefaults.AllowReassignmentOfReservedSeats AndAlso (dr("LOGINID") = Profile.User.Details.LoginID OrElse _
                                                    Utilities.CheckForDBNull_String(dr("ORIGINAL_LOGINID").ToString) = Profile.User.Details.LoginID) Then
                allowReassignmentOfReservedSeats = True
            End If

            ' Is this seat is reserved
            If Utilities.CheckForDBNull_String(dr("RESERVED_SEAT")).Trim = "Y" Then
                If allowReassignmentOfReservedSeats Then
                    seatMessage.Text = _ucr.Content("ReservedSeatText", _languageCode, True)
                    seatMessage.Visible = True
                Else
                    PriceBandDDL.Visible = False
                    seatMessage.Visible = True
                    bandMessage.Visible = True
                    bandLoginLink.Visible = False
                    seatMessage.Text = _ucr.Content("ReservedSeatText", _languageCode, True)
                    If PriceBandDDL.SelectedItem IsNot Nothing Then
                        bandMessage.Text = PriceBandDDL.SelectedItem.Text
                    End If
                    OriginalBand.Text = Utilities.CheckForDBNull_String(dr("PRICE_BAND"))
                    OriginalCustomer.Text = customerDDL.SelectedItem.Value.TrimStart("0") & getCustomerNameFromCustomerNumber(customerDDL.SelectedItem.Value, _talentCustomer.ResultDataSet)
                    OriginalCustomer.Visible = True
                End If
            Else
                If Not Profile.IsAnonymous AndAlso Not dr("CUSTOMER_ALLOCATION") Then
                    customerDDL.Visible = False
                    seatMessage.Visible = False
                    custLoginLink.Visible = False
                    customerLabel.Visible = True
                    customerLabel.Text = Profile.UserName.TrimStart("0") & " - " & Profile.User.Details.Full_Name
                End If
            End If

            ' Price band is output only for PWS when we do not have concession sales switched on for that
            ' product type.
            If Not AgentProfile.IsAgent AndAlso Utilities.CheckForDBNull_String(CurrentProductDetailsResultSet.Tables(2).Rows(0)("AllowPriceBandAlterations")).Trim() = GlobalConstants.PRICE_BAND_ALTERATIONS_RESTRICTED Then
                PriceBandDDL.Visible = False
                bandMessage.Visible = True
                bandLoginLink.Visible = False
                If PriceBandDDL.SelectedItem IsNot Nothing Then
                    bandMessage.Text = PriceBandDDL.SelectedItem.Text
                End If
                Dim disabledPriceBands() As String = Split(_ucr.Attribute("DisabledPriceBands"), ",")
                For Each priceBand As String In disabledPriceBands
                    If dr("PRICE_BAND").ToString.Equals(priceBand) Then
                        PriceBandDDL.Visible = False
                        bandMessage.Text = _ucr.Content("DisabledPriceBandsText", _languageCode, True)
                        bandMessage.Visible = True
                        Exit For
                    End If
                Next
                OriginalBand.Text = Utilities.CheckForDBNull_String(dr("PRICE_BAND"))
            End If

            'price band and customer cannot be changed for PPS and season ticket renewals
            If (Utilities.CheckForDBNull_String(dr("PRODUCT_TYPE")) = "P" Or _
                (Utilities.CheckForDBNull_String(dr("PRODUCT_TYPE")) = "S" AndAlso Utilities.CheckForDBNull_String(dr("RESERVED_SEAT")).Trim = "Y")) Then
                If Utilities.CheckForDBNull_String(dr("PRODUCT_TYPE")) = "S" Then
                    PriceLabel.Visible = True
                Else
                    PriceLabel.Visible = False
                End If
                PriceBandDDL.Visible = False
                bandMessage.Visible = True
                If bandMessage.Text.Length = 0 Then bandMessage.Text = PriceBandDDL.SelectedItem.Text
                custLoginLink.Visible = False
                bandLoginLink.Visible = False
                customerDDL.Visible = False
                OriginalCustomer.Text = customerDDL.SelectedItem.Value.TrimStart("0") & getCustomerNameFromCustomerNumber(customerDDL.SelectedItem.Value, _talentCustomer.ResultDataSet)
                OriginalCustomer.Visible = True
                OriginalBand.Text = Utilities.CheckForDBNull_String(dr("PRICE_BAND"))
            End If
        End If
    End Sub

    Private Sub setCustomerDetails(ByRef dr As DataRow, ByRef customerDDL As DropDownList, ByRef OriginalCustomer As Label, ByRef NewParticipantLabel As Label, ByRef hidFFRegURL As HiddenField, ByRef custLoginLink As HyperLink, ByRef hdfNewCustomerURL As HiddenField)
        If Not Profile.IsAnonymous Then
            customerDDL.Items.Clear()
            customerDDL.Items.Add(New ListItem(Profile.UserName.TrimStart("0") & " - " & Profile.User.Details.Full_Name, Profile.UserName))
            _deSettings = Utilities.GetSettingsObject()
            _deCustomer.UserName = Profile.UserName
            _deCustomer.CustomerNumber = Profile.User.Details.LoginID
            _deCustomer.Source = GlobalConstants.SOURCE
            _talentCustomer = New Talent.Common.TalentCustomer
            If AgentProfile.IsAgent Then
                _deCustomer.IncludeBoxOfficeLinks = True
            End If
            Dim _deCustV11 As New Talent.Common.DECustomerV11
            _deCustV11.DECustomersV1.Add(_deCustomer)
            _talentCustomer.DeV11 = _deCustV11
            _talentCustomer.Settings = _deSettings

            _talentErrObj = _talentCustomer.CustomerAssociations
            If Not _talentErrObj.HasError Then
                If _talentCustomer.ResultDataSet.Tables.Count > 1 Then
                    Dim tbl1 As DataTable = _talentCustomer.ResultDataSet.Tables(0)
                    Dim tbl2 As DataTable = _talentCustomer.ResultDataSet.Tables(1)
                    Dim dvFriendsAndFamilySearch As DataView = New DataView(tbl2)
                    dvFriendsAndFamilySearch.Sort = "AssociatedCustomerNumber ASC"
                    For Each rw As DataRow In dvFriendsAndFamilySearch.ToTable().Rows
                        customerDDL.Items.Add(New ListItem(Utilities.CheckForDBNull_String(rw("AssociatedCustomerNumber")).TrimStart("0") & " - " & Utilities.CheckForDBNull_String(rw("Forename")) &
                                        " " & Utilities.CheckForDBNull_String(rw("Surname")), Utilities.CheckForDBNull_String(rw("AssociatedCustomerNumber"))))
                    Next

                    'Add customers listed in the BasketCustomerSearch session if they exist and we are in RAS mode
                    If AgentProfile.IsAgent AndAlso AgentProfile.Type = "2" And Not Session("BasketCustomerSearch") Is Nothing Then
                        Dim dtBasketCustomerSearch As New DataTable
                        dtBasketCustomerSearch = Session("BasketCustomerSearch")
                        Dim customerAlreadyListed As Boolean = False

                        For Each row As DataRow In dtBasketCustomerSearch.Rows
                            If row("CustomerNumber") = Profile.User.Details.LoginID Then
                                For Each listitem As ListItem In customerDDL.Items
                                    If row("FoundCustomerNumber") = listitem.Value Then
                                        customerAlreadyListed = True
                                    End If
                                Next
                                If Not customerAlreadyListed Then
                                    'Only add customers that don't already exist and aren't the current user and aren't set to "999999999999"
                                    customerDDL.Items.Add(New ListItem(Utilities.CheckForDBNull_String(row("FoundCustomerNumber")).TrimStart("0") & " - " & Utilities.CheckForDBNull_String(row("Forename")) &
                                        " " & Utilities.CheckForDBNull_String(row("Surname")), Utilities.CheckForDBNull_String(row("FoundCustomerNumber"))))
                                End If
                            End If
                            customerAlreadyListed = False
                        Next
                    End If

                    Dim index As Integer = 0
                    Dim found As Boolean = False
                    For Each item As ListItem In customerDDL.Items
                        If item.Value = Utilities.CheckForDBNull_String(dr("LOGINID")) Then
                            found = True
                            Exit For
                        End If
                        index += 1
                    Next

                    If found Then
                        customerDDL.SelectedIndex = index
                        OriginalCustomer.Text = customerDDL.SelectedItem.Value.Substring(0, 12).TrimStart("0")
                    End If
                    If _isCoursePdt Then
                        If _dicParticipantMembers IsNot Nothing AndAlso _dicParticipantMembers.Count >= 0 Then
                            If Not _dicParticipantMembers.ContainsKey(customerDDL.SelectedValue) Then
                                NewParticipantLabel.Visible = True
                                NewParticipantLabel.Text = _newParticipantsLabelText
                            End If
                        End If
                    End If

                    Dim newCustomerURL As String = String.Empty
                    Dim FFRegURL As String = String.Empty
                    'Check if agent has access for friends & family
                    If (AgentProfile.IsAgent And AgentProfile.AgentPermissions.CanAccessFriendsAndFamily) Or Not AgentProfile.IsAgent Then
                        'Additional F&F processing for group tickets, giving the ability to register a new user from the basket
                        If Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("FriendsAndFamilyRegOption")) Then
                            Dim friendsAndFamilyRegOptionText As String = String.Empty
                            friendsAndFamilyRegOptionText = Utilities.CheckForDBNull_String(_ucr.Content("FriendsAndFamilyRegOptionText", _languageCode, True))
                            If Not String.IsNullOrEmpty(friendsAndFamilyRegOptionText) Then
                                Dim listItem, tempListItem As New ListItem
                                listItem.Text = friendsAndFamilyRegOptionText
                                listItem.Value = ResolveUrl(TEBUtilities.CheckForDBNull_String(_ucr.Attribute("FriendsAndFamilyRegUrl")))
                                tempListItem = customerDDL.Items.FindByValue(listItem.Value)
                                If tempListItem Is Nothing Then
                                    customerDDL.Items.Add(listItem)
                                End If
                                hidFFRegURL.Value = listItem.Value
                                FFRegURL = listItem.Value
                                _addJSSelectAttributes = True
                            End If
                        End If
                    End If

                    'Check if agent has access on Amend Customer details
                    If (AgentProfile.IsAgent And AgentProfile.AgentPermissions.CanAddOrAmendCustomer) Or Not AgentProfile.IsAgent Then
                        'Additional Generic sales via BUI
                        If AgentProfile.IsAgent AndAlso Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("NewCustomerRegOption")) Then
                            Dim newCustomerOptionText As String = String.Empty
                            newCustomerOptionText = Utilities.CheckForDBNull_String(_ucr.Content("NewCustomerOptionText", _languageCode, True))
                            If Not String.IsNullOrEmpty(newCustomerOptionText) Then
                                Dim listItem, tempListItem As New ListItem
                                listItem.Text = newCustomerOptionText
                                listItem.Value = ResolveUrl(TEBUtilities.CheckForDBNull_String(_ucr.Attribute("NewCustomerUrl")) & "&basketId=" & dr("BASKET_DETAIL_ID") & "&displaymode=basket")
                                tempListItem = customerDDL.Items.FindByValue(listItem.Value)
                                If tempListItem Is Nothing Then
                                    customerDDL.Items.Add(listItem)
                                End If
                                hdfNewCustomerURL.Value = listItem.Value
                                newCustomerURL = listItem.Value
                                _addJSSelectAttributes = True
                            End If
                        End If
                        If _addJSSelectAttributes Then
                            customerDDL.Attributes.Add("onchange", "CustomerDDLRedirect('" & customerDDL.ClientID & "','" & newCustomerURL & "', '" & FFRegURL & "')")
                        End If
                    End If
                End If
            End If

        Else
            'profile.IsAnonymous
            customerDDL.Items.Add(New ListItem(GlobalConstants.GENERIC_CUSTOMER_NUMBER, GlobalConstants.GENERIC_CUSTOMER_NUMBER))
            customerDDL.Visible = False
            custLoginLink.Visible = True
            If AgentProfile.IsAgent Then
                custLoginLink.Text = _ucr.Content("GenericBasketLoginLinkText", _languageCode, True)
                custLoginLink.NavigateUrl = "~/PagesPublic/Profile/CustomerSelection.aspx?ReturnUrl=" & Server.UrlEncode(Request.Url.PathAndQuery)
            Else
                custLoginLink.Text = _ucr.Content("LoginLinkText", _languageCode, True)
                custLoginLink.NavigateUrl = "~/PagesPublic/Login/Login.aspx?ReturnUrl=" & Server.UrlEncode(Request.Url.PathAndQuery)
            End If
        End If
    End Sub

    Private Sub setSeatDetails(ByRef dr As DataRow, ByRef seatCol As HtmlTableCell, ByRef e As RepeaterItemEventArgs)
        Dim Seatlabel As Literal = CType(e.Item.FindControl("Seatlabel"), Literal)
        Dim RestrictedSeat As Label = CType(e.Item.FindControl("RestrictedSeat"), Label)
        Dim TravelProductLabel As Literal = CType(e.Item.FindControl("TravelProductLabel"), Literal)
        Dim hdSeatlabel As HiddenField = CType(e.Item.FindControl("hdSeatlabel"), HiddenField)
        Dim hdSeatlabel2 As HiddenField = CType(e.Item.FindControl("hdSeatlabel2"), HiddenField)
        Dim plhAllocatedSeat As PlaceHolder = CType(e.Item.FindControl("plhAllocatedSeat"), PlaceHolder)
        hdSeatlabel.Value = Utilities.CheckForDBNull_String(dr("SEAT"))
        hdSeatlabel2.Value = Utilities.CheckForDBNull_String(dr("SEAT"))

        Select Case _prodTypeMTTP
            Case Is = GlobalConstants.HOMEPRODUCTTYPE, GlobalConstants.SEASONTICKETPRODUCTTYPE
                Seatlabel.Visible = True
                TravelProductLabel.Visible = False
                plhAllocatedSeat.Visible = False
            Case Is = GlobalConstants.AWAYPRODUCTTYPE
                Seatlabel.Visible = False
                plhAllocatedSeat.Visible = setAllocatedSeat(e, Utilities.CheckForDBNull_String(dr("ALLOCATED_SEAT")))
                seatCol.Visible = plhAllocatedSeat.Visible
                TravelProductLabel.Visible = False
            Case Is = GlobalConstants.TRAVELPRODUCTTYPE
                Seatlabel.Visible = False
                TravelProductLabel.Text = Utilities.CheckForDBNull_String(dr("SEAT")).Substring(0, 7).Trim
                TravelProductLabel.Visible = True
                plhAllocatedSeat.Visible = False
            Case Is = GlobalConstants.EVENTPRODUCTTYPE
                seatCol.Visible = False
                TravelProductLabel.Visible = False
                plhAllocatedSeat.Visible = False
            Case Is = GlobalConstants.MEMBERSHIPPRODUCTTYPE
                If TEBUtilities.CheckForDBNull_String(dr("PRODUCT_TYPE")).Trim() = GlobalConstants.PPSPRODUCTTYPE Then
                    seatCol.Visible = True
                Else
                    seatCol.Visible = False
                End If
                TravelProductLabel.Visible = False
                plhAllocatedSeat.Visible = False
        End Select

        If seatCol.Visible Then
            ' if seat is roving then just display Roving text
            If CBool(dr("ROVING")) Then
                Seatlabel.Text = _ucr.Content("RovingAreaText", _languageCode, True)
            ElseIf CBool(dr("UNRESERVED")) Then
                Seatlabel.Text = _ucr.Content("UnreservedAreaText", _languageCode, True)
            Else
                Select Case ModuleDefaults.SeatDisplay
                    Case Is = 1
                        'Show stand, area and row ( stand is first 3 chars, area is next 4, and row is the next 4 )
                        Seatlabel.Text = Utilities.CheckForDBNull_String(dr("SEAT")).Substring(0, 11)
                        hdSeatlabel2.Value = Seatlabel.Text
                    Case Is = 2
                        'Show stand, area, row and seat number (seat number is next 4 chars after above)
                        Seatlabel.Text = Utilities.CheckForDBNull_String(dr("SEAT")).Substring(0, 15)
                        hdSeatlabel2.Value = Seatlabel.Text
                    Case Else
                        'Show all (including alpha suffix)
                        Dim seating As New Talent.Common.DESeatDetails
                        seating.UnFormattedSeat = Utilities.CheckForDBNull_String(dr("SEAT"))
                        hdSeatlabel2.Value = Utilities.CheckForDBNull_String(dr("SEAT"))
                        Seatlabel.Text = seating.FormattedSeat
                End Select
                RestrictedSeat.Text = TEBUtilities.RestrictedSeatDescription(dr("RESTRICTION_CODE"))
            End If
        End If
    End Sub

    Private Sub SetPriceBandDetails(ByRef dr As DataRow, ByRef customerDDL As DropDownList, ByRef PriceBandDDL As DropDownList, ByRef OriginalBand As Label, ByRef NewParticipantLabel As Label)
        If _isCoursePdt Then
            customerDDL.Enabled = False
            PriceBandDDL.Visible = False
            OriginalBand.Visible = True
            Dim priceBandDescRows() As DataRow = CurrentProductDetailsResultSet.Tables(1).Select("PRICEBAND = '" & Utilities.CheckForDBNull_String(dr("PRICE_BAND")) & "'")
            If priceBandDescRows.Length > 0 Then
                OriginalBand.Text = priceBandDescRows(0)("PRICEBANDDESCRIPTION")
            Else
                OriginalBand.Text = Utilities.CheckForDBNull_String(dr("PRICE_BAND"))
            End If
        Else
            customerDDL.Enabled = True
            NewParticipantLabel.Visible = False
        End If
    End Sub

    Private Sub setVouchersLink(ByRef dr As DataRow, ByRef hplVouchers As HyperLink)
        If hplVouchers IsNot Nothing Then
            If dr("VOUCHER_ID") = 0 OrElse String.IsNullOrEmpty(dr("VOUCHER_CODE")) Then
                hplVouchers.Visible = False
            End If
            If hplVouchers.Visible Then
                Dim navigateUrl As New StringBuilder
                navigateUrl.Append("~/PagesLogin/Vouchers/VoucherDetails.aspx?voucherid=")
                navigateUrl.Append(Utilities.CheckForDBNull_String(dr("VOUCHER_ID")))
                navigateUrl.Append("&vouchercode=")
                navigateUrl.Append(Utilities.CheckForDBNull_String(dr("VOUCHER_CODE")))
                navigateUrl.Append("&returnurl=")
                navigateUrl.Append(Request.RawUrl)
                hplVouchers.NavigateUrl = navigateUrl.ToString()
            End If
        End If
    End Sub

    Private Sub setPromotionsLink(ByRef dr As DataRow, ByRef hplPromotions As HyperLink, ByRef PriceCodeDDL As DropDownList)
        hplPromotions.Visible = False
        If Not Utilities.GetCurrentPageName().ToUpper.Equals("CHECKOUTORDERCONFIRMATION.ASPX") Then
            Dim dtPromotions As DataTable = TDataObjects.BasketSettings.TblBasketPromotionItems.GetByBasketDetailIDAndModule(Profile.Basket.Basket_Header_ID, GlobalConstants.BASKETMODULETICKETING)
            If dtPromotions.Rows.Count > 0 Then
                Dim dvPromotions As New DataView(dtPromotions)
                Dim rowFilter As New StringBuilder
                rowFilter.Append("BASKET_DETAIL_ID = ").Append(dr("BASKET_DETAIL_ID"))
                dvPromotions.RowFilter = rowFilter.ToString()
                If dvPromotions.Count > 0 Then
                    Dim i As Integer = 1
                    Dim navigateUrl As New StringBuilder
                    navigateUrl.Append("~/PagesPublic/Basket/PromotionDetails.aspx?originalprice=")
                    navigateUrl.Append(dr("ORIGINAL_PRICE"))
                    navigateUrl.Append("&saleprice=")
                    navigateUrl.Append(dr("PRICE"))
                    For Each row As DataRowView In dvPromotions
                        navigateUrl.Append("&promotionid").Append(i).Append("=")
                        navigateUrl.Append(Utilities.CheckForDBNull_String(row("PROMOTION_ID")))
                        i += 1
                    Next
                    navigateUrl.Append("&returnurl=")
                    navigateUrl.Append(Request.RawUrl)
                    hplPromotions.NavigateUrl = navigateUrl.ToString()
                    If PriceCodeDDL.Visible Then PriceCodeDDL.Enabled = False
                    hplPromotions.Visible = True
                End If
            End If
        End If
    End Sub

    Private Sub setCommentsLink(ByRef e As RepeaterItemEventArgs, ByRef dr As DataRow, ByRef hplComments As HtmlAnchor, ByRef ProductCode As Label)
        Dim showComments As Boolean = False
        If Profile.IsAnonymous OrElse AgentProfile.BulkSalesMode Then
            hplComments.Visible = False
        Else
            If Utilities.CheckForDBNull_Decimal(dr("PACKAGE_ID")) > 0 Then
                Dim packageRow As DataRow = LoadPackageDetail(Utilities.CheckForDBNull_String(dr("PRODUCT")), Utilities.CheckForDBNull_String(dr("PACKAGE_ID")))
                If packageRow IsNot Nothing AndAlso packageRow("AllowComments") Then showComments = True
            End If
            If Not showComments Then
                If CurrentProductDetailsResultSet IsNot Nothing AndAlso CurrentProductDetailsResultSet.Tables.Count = 5 AndAlso CurrentProductDetailsResultSet.Tables(2).Rows.Count > 0 Then
                    If Utilities.CheckForDBNull_String(CurrentProductDetailsResultSet.Tables(2).Rows(0)("AllowComments")) = True Then showComments = True
                End If
            End If
            If Profile.Basket.CAT_MODE = GlobalConstants.CATMODE_AMEND Then
                showComments = False
            End If

            If showComments Then
                Dim ltlCommentsContent As Literal = CType(e.Item.FindControl("ltlCommentsContent"), Literal)
                hplComments.Visible = True
                ltlCommentsContent.Text = _ucr.Content("AddCommentsLabel", _languageCode, True)
                If Utilities.CheckForDBNull_Decimal(dr("PACKAGE_ID")) > 0 Then
                    Dim packageID As String = Utilities.CheckForDBNull_Decimal(dr("PACKAGE_ID")).ToString
                    hplComments.HRef = "~/PagesPublic/Basket/Comments.aspx" + "?PackageID=" + packageID + "&Customer=" + Profile.User.Details.Account_No_1 + "&Product=" + ProductCode.Text
                Else
                    hplComments.HRef = "~/PagesPublic/Basket/Comments.aspx" + "?Seat=" + dr("SEAT") + "&Customer=" + Profile.User.Details.Account_No_1 + "&Product=" + ProductCode.Text
                End If
            Else
                hplComments.Visible = False
            End If
        End If
    End Sub

    Private Sub setFavouriteSeatDetails(ByRef dr As DataRow, ByRef e As RepeaterItemEventArgs)
        Dim btnSaveFavouriteSeat As Button = CType(e.Item.FindControl("btnSaveFavouriteSeat"), Button)
        If Not Profile.User.Details Is Nothing And ModuleDefaults.FavouriteSeatFunction And Utilities.GetCurrentPageName().ToUpper.Equals("BASKET.ASPX") Then
            'Set favourite seat message
            If Session("FavouriteSeatUsed") IsNot Nothing Then
                If Session("FavouriteSeatUsed") = True Then
                    Session.Remove("FavouriteSeatUsed")
                    If dr("SEAT").ToString.Trim <> Profile.User.Details.Favourite_Seat.Trim Then
                        Dim favouriteSeatInBasket As Boolean = False
                        For Each item As TalentBasketItem In Profile.Basket.BasketItems
                            If Profile.User.Details.Favourite_Seat.Trim = item.SEAT.Trim Then favouriteSeatInBasket = True
                        Next
                        If Not favouriteSeatInBasket Then _errorList.Items.Add(_ucr.Content("NextBestSeatAdded", _languageCode, True))
                    End If
                End If
            End If

            'Set favourite seat button
            If Utilities.CheckForDBNull_Boolean_DefaultFalse(dr("CAN_SAVE_AS_FAVOURITE_SEAT")) Then
                btnSaveFavouriteSeat.Text = _ucr.Content("SaveFavouriteSeatButtonText", _languageCode, True)
                btnSaveFavouriteSeat.Visible = True
            Else
                btnSaveFavouriteSeat.Visible = False
            End If
        Else
            btnSaveFavouriteSeat.Visible = False
        End If
    End Sub

    Private Sub setFulfilmentDetails(ByRef dr As DataRow, ByRef fulfilmentCol As HtmlTableCell, ByRef OriginalFulfilment As Label, ByRef OriginalFulfilmentCode As Label)
        If TalentDefaults.IsOrderLevelFulfilmentEnabled Then
            fulfilmentCol.Visible = False
        Else
            If String.Compare(Profile.Basket.USER_SELECT_FULFIL, "Y", True) <> 0 Then
                fulfilmentCol.Visible = False
            Else
                If _prodTypeMTTP = GlobalConstants.HOMEPRODUCTTYPE Or _prodTypeMTTP = GlobalConstants.TRAVELPRODUCTTYPE Or _prodTypeMTTP = GlobalConstants.EVENTPRODUCTTYPE Or _prodTypeMTTP = GlobalConstants.AWAYPRODUCTTYPE Then
                    fulfilmentCol.Visible = True
                    OriginalFulfilment.Visible = True
                Else
                    fulfilmentCol.Visible = False
                End If
            End If
        End If
        Dim fulfilmentCode As String = Utilities.CheckForDBNull_String(dr("CURR_FULFIL_SLCTN")).Trim.ToUpper()
        OriginalFulfilmentCode.Text = fulfilmentCode
        OriginalFulfilment.Text = getFulfilmentTextByFulfilmentCode(fulfilmentCode)
    End Sub

    ''' <summary>
    ''' Setup the price code options. Only visible for agents. Show the price code drop down list for H, S and C type products.
    ''' Show the description for A type products otherwise hid the price code column
    ''' </summary>
    ''' <param name="priceCodeCol">The td column</param>
    ''' <param name="PriceCodeDDL">The price code drop down list</param>
    ''' <param name="ProductCode">The product code</param>
    ''' <param name="priceCode">The price code</param>
    ''' <param name="lblPriceCodeDescription">The price code description label</param>
    ''' <param name="dr">The current Data Row</param>
    ''' <remarks></remarks>
    Private Sub setPriceCodeOptions(ByRef priceCodeCol As HtmlTableCell, ByRef PriceCodeDDL As DropDownList, ByRef ProductCode As Label, ByRef priceCode As Label, ByRef lblPriceCodeDescription As Label, ByRef dr As DataRow)
        If AgentProfile.IsAgent Then
            If (_prodTypeMTTP = GlobalConstants.HOMEPRODUCTTYPE OrElse _prodTypeMTTP = GlobalConstants.MEMBERSHIPPRODUCTTYPE OrElse _prodTypeMTTP = GlobalConstants.SEASONTICKETPRODUCTTYPE OrElse _prodTypeMTTP = GlobalConstants.AWAYPRODUCTTYPE) Then
                priceCodeCol.Visible = True
                PriceCodeDDL.Visible = True
            Else
                priceCodeCol.Visible = False
                PriceCodeDDL.Visible = False
            End If
        Else
            priceCodeCol.Visible = False
            PriceCodeDDL.Visible = False
        End If

        If priceCodeCol.Visible AndAlso PriceCodeDDL.Visible Then
            Dim dtProductPriceCode As DataTable = GetProductPriceCodesDescription(ProductCode.Text, _prodTypeMTTP, Utilities.CheckForDBNull_String(CurrentProductDetailsResultSet.Tables(2).Rows(0)("ProductStadium")), CurrentProductDetailsResultSet.Tables("PriceCodes"), CurrentProductDetailsResultSet.Tables("CampaignCodes"))
            Dim dtPriceCodeDDL = dtProductPriceCode.Copy

            ' We need to add in the ticket exchange price code for TE added items.
            If TalentDefaults.TicketExchangeEnabled AndAlso Utilities.CheckForDBNull_String(CurrentProductDetailsResultSet.Tables(2).Rows(0)("TicketExchangeEnabled")).Trim() Then
                If _prodTypeMTTP = GlobalConstants.HOMEPRODUCTTYPE Then
                    If TEBUtilities.CheckForDBNull_String(dr("RESERVATION_CODE")) = GlobalConstants.RES_CODE_TICKET_EXCHANGE Then
                        Dim dtStadiumPriceCode = GetStadiumPriceCodesDescription(CurrentProductDetailsResultSet.Tables(2).Rows(0)("ProductStadium"))
                        If dtStadiumPriceCode.Rows.Count > 0 Then

                            Dim TicketExchangePriceCodeRow As DataRow = dtStadiumPriceCode.Select("PriceCode = '" + GlobalConstants.TICKET_EXCHANGE_PRICE_CODE + "'")(0)
                            If Not TicketExchangePriceCodeRow Is Nothing Then
                                Dim dRow As DataRow = dtPriceCodeDDL.NewRow()
                                dRow("PriceCode") = TicketExchangePriceCodeRow("PriceCode")
                                dRow("PriceCodeDescription") = TicketExchangePriceCodeRow("PriceCodeDescription")
                                dRow("PriceCodeLongDescription") = TicketExchangePriceCodeRow("PriceCodeLongDescription")
                                dRow("FreeOfCharge") = TicketExchangePriceCodeRow("FreeOfCharge")
                                dtPriceCodeDDL.Rows.Add(dRow)
                            End If
                        End If
                    End If
                End If
            End If

            If dtProductPriceCode.Rows.Count > 0 Then
                PriceCodeDDL.DataSource = dtPriceCodeDDL
                If Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowLongPriceCodeDescription")) Then
                    PriceCodeDDL.DataTextField = "PriceCodeLongDescription"
                Else
                    PriceCodeDDL.DataTextField = "PriceCodeDescription"
                End If

                PriceCodeDDL.DataValueField = "PriceCode"
                PriceCodeDDL.DataBind()
                PriceCodeDDL.SelectedValue = priceCode.Text
            Else
                priceCodeCol.Visible = False
                PriceCodeDDL.Visible = False
            End If
        End If
    End Sub

    Public Sub setDDLTriggers(ByRef PriceCodeDDL As DropDownList, ByRef PriceBandDDL As DropDownList, ByRef EventFulfilmentDDL As DropDownList)
        If PriceCodeDDL IsNot Nothing Then PriceCodeDDL.Attributes.Add("onchange", "optionsUpdated();")
        If PriceBandDDL IsNot Nothing Then PriceBandDDL.Attributes.Add("onchange", "optionsUpdated();")
        If EventFulfilmentDDL IsNot Nothing Then EventFulfilmentDDL.Attributes.Add("onchange", "optionsUpdated();")
    End Sub

    Private Sub setMatchDayHospitalityBasket(ByRef dr As DataRow, ByRef fulfilmentCol As HtmlTableCell, ByRef OriginalFulfilment As Label, ByRef ProductTypeMTTP As Label, ByRef bandCol As HtmlTableCell, ByRef seatCol As HtmlTableCell, _
                        ByRef PriceBandDDL As DropDownList, ByRef customerDDL As DropDownList, ByRef priceCodeCol As HtmlTableCell, ByRef PriceCodeDDL As DropDownList, ByRef packageQuantityCol As HtmlTableCell, _
                        ByRef PackageIDForQuantity As Label, ByRef PackageQuantityLabel As Label, ByRef netPriceCol As HtmlTableCell, ByRef netPriceLabel As Label, ByRef customerLabel As Label)
        Dim selectedPackageID As String = String.Empty
        If Utilities.CheckForDBNull_Decimal(dr("PACKAGE_ID")) > 0 Then
            selectedPackageID = Utilities.CheckForDBNull_String(dr("PACKAGE_ID")).Trim
        End If
        If selectedPackageID.Length > 0 Then
            fulfilmentCol.Visible = False
            OriginalFulfilment.Visible = False
            ProductTypeMTTP.Text = String.Empty
            bandCol.Visible = False
            seatCol.Visible = False
            PriceBandDDL.Visible = False
            customerDDL.Visible = False
            priceCodeCol.Visible = False
            PriceCodeDDL.Visible = False
            packageQuantityCol.Visible = True
            PackageIDForQuantity.Visible = False
            PackageQuantityLabel.Visible = True
            PackageIDForQuantity.Text = selectedPackageID
            PackageQuantityLabel.Text = (Utilities.CheckForDBNull_Int(dr("QUANTITY")))
            If Not (Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("displayNetPrice"))) Then
                netPriceCol.Visible = False
                netPriceLabel.Visible = False
            Else
                netPriceCol.Visible = True
                netPriceLabel.Visible = True
                netPriceLabel.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.CheckForDBNull_Decimal(dr("NET_PRICE")), _ucr.BusinessUnit, _ucr.PartnerCode)
            End If
            If Not TEBUtilities.IsAnonymousTalent Then
                customerLabel.Visible = True
                Try
                    customerLabel.Text = Profile.UserName.TrimStart("0") & " - " & Profile.User.Details.Full_Name
                Catch ex As Exception
                End Try
            End If
        Else
            packageQuantityCol.Visible = False
            PackageIDForQuantity.Visible = False
            PackageQuantityLabel.Visible = False
            netPriceCol.Visible = False
            netPriceLabel.Visible = False
        End If
    End Sub

    Private Sub setEPurseBasket(ByRef dr As DataRow, ByRef customerDDL As DropDownList, ByRef customerLabel As Label, ByRef OriginalCustomer As Label, ByRef seatCol As HtmlTableCell, ByRef priceCodeCol As HtmlTableCell, ByRef bandCol As HtmlTableCell)
        If Not String.IsNullOrEmpty(ModuleDefaults.EPurseTopUpProductCode) AndAlso dr("PRODUCT").ToString() = ModuleDefaults.EPurseTopUpProductCode Then
            customerDDL.Visible = False
            customerLabel.Visible = True
            customerLabel.Text = customerDDL.SelectedItem.Text
            OriginalCustomer.Visible = False
            seatCol.Visible = False
            priceCodeCol.Visible = False
            bandCol.Visible = False
        End If
    End Sub


    Private Sub setRestrictedBasketOptions(ByRef dr As DataRow, ByRef customerDDL As DropDownList, ByRef customerLabel As Label, ByRef OriginalCustomer As Label, ByRef seatCol As HtmlTableCell, ByRef priceCodeCol As HtmlTableCell, ByRef bandCol As HtmlTableCell)
        If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(dr("RESTRICTED_BASKET_OPTIONS")) Then
            customerDDL.Visible = False
            customerLabel.Visible = True
            customerLabel.Text = customerDDL.SelectedItem.Text
            OriginalCustomer.Visible = False
            seatCol.Visible = False
            priceCodeCol.Visible = False
            bandCol.Visible = False
        End If
    End Sub

    Private Sub setBulkSalesModeDisplay(ByRef dr As DataRow, ByRef hdfBulkSalesID As HiddenField, ByRef txtBulkSalesQuantity As TextBox, ByRef lblBulkSalesQuantity As Label, ByRef seatCol As HtmlTableCell, ByRef bulkSalesQuantityCol As HtmlTableCell, _
                                        ByRef rfvBulkSalesQuantity As RequiredFieldValidator, ByRef rgxBulkSalesQuantity As RegularExpressionValidator)
        hdfBulkSalesID.Visible = (dr("BULK_SALES_ID") > 0)
        rfvBulkSalesQuantity.Enabled = False
        rgxBulkSalesQuantity.Enabled = False
        If hdfBulkSalesID.Visible Then
            lblBulkSalesQuantity.Visible = (_prodTypeMTTP = GlobalConstants.HOMEPRODUCTTYPE OrElse _prodTypeMTTP = GlobalConstants.SEASONTICKETPRODUCTTYPE)
            txtBulkSalesQuantity.Visible = Not lblBulkSalesQuantity.Visible
            txtBulkSalesQuantity.Text = TEBUtilities.CheckForDBNull_Int(dr("QUANTITY"))
            lblBulkSalesQuantity.Text = TEBUtilities.CheckForDBNull_Int(dr("QUANTITY"))
            hdfBulkSalesID.Value = dr("BULK_SALES_ID")
            seatCol.Visible = False
            bulkSalesQuantityCol.Visible = True
            If txtBulkSalesQuantity.Visible Then
                Dim productDescription As String = CurrentProductDetailsResultSet.Tables(2).Rows(0)("ProductDescription")
                rfvBulkSalesQuantity.Enabled = True
                rfvBulkSalesQuantity.ErrorMessage = _ucr.Content("BulkSalesRequiredQuantityError", _languageCode, True).Replace("<<PRODUCT_DESCRIPTION>>", productDescription)
                rgxBulkSalesQuantity.Enabled = True
                rgxBulkSalesQuantity.ErrorMessage = _ucr.Content("BulkSalesInvalidQuantityError", _languageCode, True).Replace("<<PRODUCT_DESCRIPTION>>", productDescription)
                rgxBulkSalesQuantity.ValidationExpression = "^[1-9][0-9]*$"
            End If
        Else
            bulkSalesQuantityCol.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Set the ticket exchange options against the seat currently being worked with.
    ''' Show an icon and disable the price band as it doesn't apply to a ticket exchange seat since the price is set by the ticket seller.
    ''' </summary>
    ''' <param name="dr">The current data row</param>
    ''' <param name="e">The repeater item event arguments</param>
    ''' <remarks></remarks>
    Private Sub setTicketExchangeOption(ByRef dr As DataRow, ByRef e As RepeaterItemEventArgs)
        Dim plhTicketExchangeSeat As PlaceHolder = CType(e.Item.FindControl("plhTicketExchangeSeat"), PlaceHolder)
        Dim iconTicketExchange As HtmlGenericControl = CType(e.Item.FindControl("iconTicketExchange"), HtmlGenericControl)
        Dim PriceBandDDL As DropDownList = CType(e.Item.FindControl("PriceBandDDL"), DropDownList)
        Dim PriceCodeDDL As DropDownList = CType(e.Item.FindControl("PriceCodeDDL"), DropDownList)
        plhTicketExchangeSeat.Visible = False
        If TalentDefaults.TicketExchangeEnabled AndAlso Utilities.CheckForDBNull_String(CurrentProductDetailsResultSet.Tables(2).Rows(0)("TicketExchangeEnabled")).Trim() Then
            If _prodTypeMTTP = GlobalConstants.HOMEPRODUCTTYPE Then
                If TEBUtilities.CheckForDBNull_String(dr("RESERVATION_CODE")) = GlobalConstants.RES_CODE_TICKET_EXCHANGE Then
                    plhTicketExchangeSeat.Visible = True
                    iconTicketExchange.Attributes.Add("title", _ucr.Content("TicketExchangeIconTitleText", _languageCode, True))
                    PriceBandDDL.Enabled = False
                    PriceCodeDDL.Enabled = False
                End If
            End If
        End If
    End Sub

    Private Sub basketLayoutForCheckoutPages(ByRef priceCodeCol As HtmlTableCell, ByRef PriceCodeDDL As DropDownList, ByRef OriginalPriceCode As Label, ByRef priceCode As Label, ByRef custLoginLink As HyperLink, ByRef OriginalCustomer As Label, ByRef customerLabel As Label, _
                                             ByRef customerDDL As DropDownList, ByRef PriceBandDDL As DropDownList, ByRef OriginalBand As Label, ByRef OriginalFulfilment As Label, ByRef removeCol As HtmlTableCell, ByRef lblBulkSalesQuantity As Label, ByRef txtBulkSalesQuantity As TextBox)
        If Not AgentProfile.IsAgent Then
            priceCodeCol.Visible = False
            PriceCodeDDL.Visible = False
        Else
            PriceCodeDDL.Visible = False
            If priceCodeCol.Visible AndAlso PriceCodeDDL.SelectedItem IsNot Nothing Then
                OriginalPriceCode.Text = PriceCodeDDL.SelectedItem.Text
            Else
                OriginalPriceCode.Text = priceCode.Text
            End If
            OriginalPriceCode.Visible = True
            If Profile.IsAnonymous Then
                custLoginLink.Visible = False
                OriginalCustomer.Text = _ucr.Content("GenericCustomer_DisplayText", _languageCode, True)
                OriginalCustomer.Visible = True
                customerLabel.Visible = False
            End If
        End If
        customerDDL.Visible = False
        If Not Profile.IsAnonymous Then
            OriginalCustomer.Text = customerDDL.SelectedItem.Value.TrimStart("0") & getCustomerNameFromCustomerNumber(customerDDL.SelectedItem.Value, _talentCustomer.ResultDataSet)
            OriginalCustomer.Visible = True
            customerLabel.Visible = False
        End If
        PriceBandDDL.Visible = False
        OriginalBand.Text = If(PriceBandDDL.SelectedItem Is Nothing, String.Empty, PriceBandDDL.SelectedItem.Text)
        OriginalBand.Visible = True
        OriginalFulfilment.Visible = True
        removeCol.Visible = False
        txtBulkSalesQuantity.Visible = False
        lblBulkSalesQuantity.Visible = True
    End Sub

#End Region

End Class