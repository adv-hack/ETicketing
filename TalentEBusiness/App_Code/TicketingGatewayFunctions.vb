''Imports Microsoft.VisualBasic
Imports System.Collections.Generic
Imports System.Data
Imports System.IO
Imports iTextSharp.tool
Imports System.Web
Imports TalentBusinessLogic.DataTransferObjects.Hospitality
Imports TalentBusinessLogic.ModelBuilders.Hospitality
Imports TalentBusinessLogic.Models
Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.CATHelper
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports TCUtilities = Talent.Common.Utilities

''' <summary>
''' This is the shared Ticketing Gateway Functionality designed to be used without the use of redirects to TicketingGateway.aspx
''' </summary>
''' <remarks></remarks>
Public Class TicketingGatewayFunctions

#Region "Contstants"

    Const KEYCODE As String = "TicketingGateway.aspx"
    Const PAGECODE As String = "TicketingGateway.aspx"

#End Region

#Region "Class Level Fields"

    Private _product As New ArrayList
    Private _seat As New ArrayList
    Private _customer As New ArrayList
    Private _concession As New ArrayList
    Private _priceCode As New ArrayList
    Private _priceCodeOverridden As New ArrayList
    Private _productType As New ArrayList
    Private _originalCust As New ArrayList
    Private _fulfilmentMethod As New ArrayList
    Private _isPackage As New ArrayList
    Private _bulkSalesID As New ArrayList
    Private _bulkSalesQuantity As New ArrayList
    Private _allocatedSeat As New ArrayList
    Private _missingParamErrorCode As String = String.Empty
    Private _productHasMandatoryRelatedProducts As Boolean = False
    Private _redirectToLinkedProductsPage As Boolean = False
    Private _redirectUrl As String = String.Empty
    Private _customerSelection As New Char

#End Region

#Region "Public Properties"

    Public ProductHasRelatedProducts As Boolean = False

#End Region

#Region "Product (add to basket) Functions"

    ''' <summary>
    ''' Add multiple products to basket based on the given list of products
    ''' </summary>
    ''' <param name="productListDataEntity">List of DEAddTicketingItems</param>
    ''' <remarks></remarks>
    Public Function AddMultipleProductsToBasket(ByRef productListDataEntity As List(Of DEAddTicketingItems), Optional ByVal linkedProductId As Integer = 0) As ErrorObj
        Dim err As New ErrorObj
        If productListDataEntity.Count > 0 Then
            Dim basket As New Talent.Common.TalentBasket
            Dim ticketingItemDetails As New DETicketingItemDetails
            Dim talProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
            Dim returnErrorCode As String = String.Empty
            Dim returnError As Boolean = False
            If talProfile.IsAnonymous Then
                ticketingItemDetails.CustomerNo = GlobalConstants.GENERIC_CUSTOMER_NUMBER
            Else
                ticketingItemDetails.CustomerNo = talProfile.UserName
            End If
            ticketingItemDetails.SessionId = talProfile.Basket.Basket_Header_ID
            ticketingItemDetails.LinkedProductId = linkedProductId
            ticketingItemDetails.Src = GlobalConstants.SOURCE
            basket.De = ticketingItemDetails
            basket.ListOfDEAddTicketingItems = productListDataEntity
            basket.Settings = TEBUtilities.GetSettingsObject()
            Dim talDataObjects As New TalentDataObjects
            talDataObjects.Settings = TEBUtilities.GetSettingsObject()
            basket.FulfilmentFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(talDataObjects.Settings.BusinessUnit)
            basket.CardTypeFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(talDataObjects.Settings.BusinessUnit)

            TCUtilities.TalentCommonLog("AddMultipleProductsToBasket", ticketingItemDetails.CustomerNo, "TalentEBusiness Request to Talent.Common.AmendBasket.AddMultipleTicketingItemsReturnBasket")
            err = basket.AddMultipleTicketingItemsReturnBasket()

            'First check for an error during comms or with basket status
            returnErrorCode = CheckResponseForError(basket.ResultDataSet, err)
            If returnErrorCode.Length > 0 Then
                returnError = True
                If returnErrorCode.Equals("NC") Then returnErrorCode = "NS"
                err.HasError = returnError
                err.ErrorNumber = returnErrorCode
            Else
                If Not returnError Then
                    'Loop through each product in the array to find any individual product errors
                    For Each productDataEntity As DEAddTicketingItems In productListDataEntity
                        If Not String.IsNullOrEmpty(productDataEntity.ErrorCode) Then
                            err.HasError = True
                            err.ErrorNumber = GlobalConstants.ERRORWHENADDINGMULTIPLEPRODUCTS
                            If productDataEntity.ErrorCode = "NC" OrElse productDataEntity.ErrorCode = "NS" Then
                                Dim talProduct As New TalentProduct
                                talProduct.De.ProductCode = productDataEntity.ProductCode
                                talProduct.De.CampaignCode = productDataEntity.CampaignCode
                                talProduct.De.ComponentID = CATHelper.GetPackageComponentId(productDataEntity.ProductCode, HttpContext.Current.Request("callid"))
                                talProduct.Settings.Company = basket.Settings.Company
                                Dim agent As New Agent
                                If agent.IsAgent Then
                                    talProduct.De.AvailableToSell03 = agent.IsAvailableToSell03
                                    talProduct.De.AvailableToSellAvailableTickets = agent.SellAvailableTickets
                                Else
                                    talProduct.De.AvailableToSell03 = True
                                    talProduct.De.AvailableToSellAvailableTickets = False
                                End If
                                talProduct.AvailableStandsClearCache()
                                talProduct.AvailableStandsWithoutDescriptionsClearCache()
                                productDataEntity.ErrorCode = "NS"
                            End If
                        End If
                    Next
                End If
            End If
        Else
            err.HasError = True
            err.ErrorNumber = "MF"
        End If
        Return err
    End Function

    ''' <summary>
    ''' Add the selected seats array to the basket
    ''' </summary>
    ''' <param name="productCode">The given product code</param>
    ''' <param name="productType">The given product type</param>
    ''' <param name="productSubType">The given product sub type</param>
    ''' <param name="stadiumCode">The given stadium code</param>
    ''' <param name="campaignCode">The given campaign code</param>
    ''' <param name="seatArray">The seat array string</param>
    ''' <param name="isProductBundle">Is the product a bundle type</param>
    ''' <param name="oldSeat">The seat being changed. Used for ST exceptions and changing hospitality component seats.</param>
    ''' <param name="priceBreakId">The given price break ID</param>
    ''' <param name="isSeatSelectionPage">Are we adding a seat to the basket based from VisualSeatSelection.aspx or SeatSelection.aspx?</param>
    ''' <param name="pickingNewComponentSeat">True if changing a component seat</param>
    ''' <param name="callId">The coporate call ID value</param>
    ''' <returns>Redirect string</returns>
    ''' <remarks></remarks>
    Public Function SeatSelectionAddToBasket(ByVal productCode As String, ByVal productType As String, ByVal productSubType As String, ByVal stadiumCode As String, ByVal campaignCode As String, ByVal seatArray As List(Of DESeatDetails), ByVal isProductBundle As Boolean, ByVal oldSeat As String, ByVal priceBreakId As Long, ByVal isSeatSelectionPage As Boolean, ByVal pickingNewComponentSeat As Boolean, ByVal callId As Long, ByVal changeAllSeats As Boolean, Optional ByVal packageIdParm As String = "", Optional ByVal componentIdParm As String = "") As String
        Dim err As New ErrorObj
        Dim deATI As DEAddTicketingItems = Nothing
        Dim dtProductRelations As New DataTable
        Dim talProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
        Dim talAgent As New Agent
        Dim missingParam As Boolean = IsCATParamMissing()
        Dim returnError As Boolean = False
        Dim returnErrorCode As String = String.Empty
        Dim catSeatPayRef As String = String.Empty
        Dim isItCATSeatSelection As Boolean = False
        Dim returnUrl As String = String.Empty
        Dim talDataObjects As New TalentDataObjects
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults
        Dim redirectToComponentOrBookingPage As String = String.Empty
        Dim redirectToSeasonTicketExceptionsPage As Boolean = False
        Dim packageId As Long = 0

        If isSeatSelectionPage Then
            returnUrl = HttpContext.Current.Request.Url.AbsoluteUri
        Else
            returnUrl = "~/PagesPublic/ProductBrowse/VisualSeatSelection.aspx?stadium={0}&product={1}&campaign={2}&type={3}&productsubtype={4}&oldseat={5}"
        End If
        returnUrl = String.Format(returnUrl, stadiumCode, productCode, campaignCode, productType, productSubType, oldSeat)
        _redirectUrl = String.Empty
        talDataObjects.Settings = TEBUtilities.GetSettingsObject()
        If String.IsNullOrEmpty(productCode) Then
            missingParam = True
        ElseIf String.IsNullOrEmpty(productType) Then
            missingParam = True
        ElseIf seatArray.Count = 0 Then
            missingParam = True
        End If
        Dim linkedMasterProduct As String = talDataObjects.BasketSettings.TblBasketHeader.GetLinkedMasterProduct(talProfile.Basket.Basket_Header_ID)
        If (Not missingParam) AndAlso (TEBUtilities.IsValidAddToBasketRequest("seatselection.aspx", "addtobasket", productCode, String.Empty, campaignCode, productSubType, ProductHasRelatedProducts, _productHasMandatoryRelatedProducts, seatArray.Count, pickingNewComponentSeat)) Then
            If String.IsNullOrEmpty(linkedMasterProduct) Then
                HttpContext.Current.Session("QuantityRequested") = seatArray.Count
            End If
            deATI = GetCATTicketingItemDetails(True)
            With deATI
                .SessionId = talProfile.Basket.Basket_Header_ID
                .ProductCode = productCode
                .ProductType = productType
                .CampaignCode = campaignCode
                .LinkedMasterProduct = If(productCode.Equals(linkedMasterProduct), String.Empty, linkedMasterProduct)
                .LinkedProductID = ReturnLinkedProductIDFromBasket(linkedMasterProduct)
                .Source = GlobalConstants.SOURCE
                .ProductHasMandtoryRelatedProducts = _productHasMandatoryRelatedProducts
                .AllMandatoryLinkedProductsAdded = HaveAllLinkedMandatoryProductsBeenAdded(productCode, linkedMasterProduct)
                .SeatSelectionArray = CATHelper.GetFormattedSeatList(seatArray, oldSeat, changeAllSeats)
                If talProfile.IsAnonymous Then
                    .CustomerNumber = GlobalConstants.GENERIC_CUSTOMER_NUMBER
                Else
                    .CustomerNumber = talProfile.User.Details.LoginID.PadLeft(12, "0")
                End If
                If talAgent.IsAgent() AndAlso talAgent.Type() = "2" Then
                    .ByPassPreReqCheck = "Y"
                Else
                    .ByPassPreReqCheck = "N"
                End If
                If oldSeat.Trim().Length > 0 OrElse changeAllSeats Then
                    If oldSeat.Trim().Length > 0 Then
                        If oldSeat = GlobalConstants.ST_EXCCEPTION_UNALLOCATED_SEAT Then
                            .STExceptionChangeRemoveMode = GlobalConstants.ST_EXCCEPTION_CHOOSE_SEAT
                        Else
                            .STExceptionChangeRemoveMode = GlobalConstants.ST_EXCCEPTION_CHANGE_SEAT
                        End If
                        .STExceptionSeasonTicketProductCode = talDataObjects.BasketSettings.TblBasketDetailExceptions.GetSTProductCode(.SessionId, .ProductCode, GlobalConstants.BASKETMODULETICKETING)
                    End If
                    .PickingNewComponentSeat = pickingNewComponentSeat
                End If
                If Not String.IsNullOrEmpty(packageIdParm) Then
                    .PackageID = packageIdParm
                End If
                If Not String.IsNullOrEmpty(componentIdParm) Then
                    .SeatComponentID = componentIdParm
                End If
            End With

            Dim basket As New Talent.Common.TalentBasket
            basket.DeAddTicketingItems = deATI
            basket.Settings = TEBUtilities.GetSettingsObject()
            basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            basket.Settings.StoredProcedureGroup = TEBUtilities.GetStoredProcedureGroup
            basket.Settings.OriginatingSource = TEBUtilities.GetOriginatingSource(talAgent.Name)
            basket.FulfilmentFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(talDataObjects.Settings.BusinessUnit)
            basket.CardTypeFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(talDataObjects.Settings.BusinessUnit)

            Talent.Common.Utilities.TalentCommonLog("TicketingGateway.SeatSelection_AddToBasket", deATI.CustomerNumber, "TalentEBusiness Request to Talent.Common.AmendBasket.AddTicketingItemsReturnBasket")
            err = basket.AddTicketingItemsReturnBasket
            Talent.Common.Utilities.TalentCommonLog("TicketingGateway.SeatSelection_AddToBasket", deATI.CustomerNumber, "TalentEBusiness Response from Talent.Common.AmendBasket.AddTicketingItemsReturnBasket")
            returnErrorCode = CheckResponseForError(basket.ResultDataSet, err)

            If basket.BasketRequiresRedirectToBookingOrComponentPage <> String.Empty AndAlso basket.BasketRequiresRedirectToBookingOrComponentPage <> "N" Then
                redirectToComponentOrBookingPage = basket.BasketRequiresRedirectToBookingOrComponentPage
                packageId = basket.DeAddTicketingItems.PackageID
            End If
            If pickingNewComponentSeat Then
                For Each basketItem As DEBasketItem In talProfile.Basket.BasketItems
                    If basketItem.Product = productCode Then
                        packageId = basketItem.PACKAGE_ID
                        basket.DeAddTicketingItems.PackageID = packageId
                        basket.DeAddTicketingItems.CallID = callId
                        Exit For
                    End If
                Next
            End If
            SetSessionValuesFromBasket(basket)

            If basket.OrphanSeatRemaining Then
                HttpContext.Current.Session("TicketingGatewayError") = GlobalConstants.ORPHANSEATERROR
                Dim tProduct As New TalentProduct
                tProduct.Settings = basket.Settings
                'clear cache for each different stand/area combination
                For Each seat As DESeatDetails In seatArray
                    tProduct.De.StandCode = seat.Stand
                    tProduct.De.AreaCode = seat.Area
                    tProduct.De.ProductCode = productCode
                    tProduct.De.CampaignCode = campaignCode
                    tProduct.De.ComponentID = CATHelper.GetPackageComponentId(productCode, HttpContext.Current.Request("callid"))
                    tProduct.De.PriceBreakId = priceBreakId
                    tProduct.ProductSeatAvailabilityClearCache()
                Next
                redirectToComponentOrBookingPage = False
            End If

            If Not String.IsNullOrWhiteSpace(HttpContext.Current.Session("catmode")) Then
                HttpContext.Current.Session("catpayref") = HttpContext.Current.Session("payref")
            End If

            isItCATSeatSelection = (IsCATSessionsRemoved() AndAlso (HttpContext.Current.Request("callid") IsNot Nothing) AndAlso
                                        IsNumeric(HttpContext.Current.Request("callid")) AndAlso (HttpContext.Current.Request("callid") <> "0"))
            If returnErrorCode.Length > 0 Then
                If returnErrorCode.Equals("NC") Then
                    Dim tp As New Talent.Common.TalentProduct
                    tp.De.ProductCode = productCode
                    tp.De.CampaignCode = campaignCode
                    tp.De.ComponentID = CATHelper.GetPackageComponentId(productCode, HttpContext.Current.Request("callid"))
                    Dim agent As New Agent
                    If agent.IsAgent Then
                        tp.De.AvailableToSell03 = agent.IsAvailableToSell03
                        tp.De.AvailableToSellAvailableTickets = agent.SellAvailableTickets
                    Else
                        tp.De.AvailableToSell03 = True
                        tp.De.AvailableToSellAvailableTickets = False
                    End If
                    tp.AvailableStandsClearCache()
                    tp.AvailableStandsWithoutDescriptionsClearCache()
                    returnErrorCode = "NS"
                End If
                returnError = True
            End If

            If basket.BasketHasExceptionSeats Then
                If productType = GlobalConstants.SEASONTICKETPRODUCTTYPE Then
                    redirectToSeasonTicketExceptionsPage = True
                ElseIf productType = GlobalConstants.HOMEPRODUCTTYPE Then
                    If oldSeat.Trim().Length > 0 Then
                        redirectToSeasonTicketExceptionsPage = True
                    End If
                End If
            End If
        Else
            returnError = True
            returnErrorCode = "PARAMERR"
            Talent.eCommerce.Logging.WriteLog(talProfile.UserName, "SS_ATB_" & returnErrorCode, "Missing Parameter in TicketingGateway - SeatSelection_AddToBasket", "Error")
            _redirectUrl = returnUrl
        End If

        'Handle the redirect back to the seat selection page or the basket page
        If isItCATSeatSelection Then
            If returnError Then
                redirectToComponentOrBookingPage = False
                HttpContext.Current.Session("TicketingGatewayError") = returnErrorCode
            End If
            If returnErrorCode = "WA" OrElse returnErrorCode = "AC" Then
                Talent.eCommerce.Utilities.ClearOrderEnquiryDetailsCache()
            End If
            _redirectUrl = returnUrl
        Else
            If UCase(productType) = GlobalConstants.SEASONTICKETPRODUCTTYPE AndAlso Not isProductBundle Then
                If returnError Then
                    Talent.Common.Utilities.TalentCommonLog("TicketingGateway.ProductSeason_AddToBasket", "", "TalentEBusiness Redirect to ~/PagesPublic/ProductBrowse/VisualSeatSelection.aspx")
                    HttpContext.Current.Session("TicketingGatewayError") = returnErrorCode
                    _redirectUrl = returnUrl
                Else
                    If (moduleDefaults.PPS_ENABLE_1 AndAlso Not talAgent.BulkSalesMode) Then
                        _redirectUrl = "~/PagesPublic/ProductBrowse/TicketingPrePayments.aspx?ppspage=1&product=" & productCode & "&pricecode=" & campaignCode
                    ElseIf (moduleDefaults.PPS_ENABLE_2 AndAlso Not talAgent.BulkSalesMode) Then
                        _redirectUrl = "~/PagesPublic/ProductBrowse/TicketingPrePayments.aspx?ppspage=2&product=" & productCode & "&pricecode=" & campaignCode
                    Else
                        _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
                        _redirectToLinkedProductsPage = True
                    End If
                End If
            Else
                If returnError Then
                    HttpContext.Current.Session("TicketingGatewayError") = returnErrorCode
                    _redirectUrl = returnUrl
                Else
                    If pickingNewComponentSeat = True Then
                        _redirectUrl = "~/PagesPublic/Hospitality/HospitalityBooking.aspx?product=" & productCode & "&packageID=" & packageId.ToString
                    Else
                        _redirectToLinkedProductsPage = True
                        If moduleDefaults.HomeProduct_ForwardToBasket Then
                            If _redirectUrl.Length = 0 Then _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
                        Else
                            _redirectUrl = "~/PagesPublic/ProductBrowse/productHome.aspx" & GetProductDetailQueryString(True)
                        End If
                    End If
                End If
            End If
        End If

        Return HandleRedirect(_redirectUrl, _redirectToLinkedProductsPage, productCode, campaignCode, productSubType, redirectToComponentOrBookingPage, redirectToSeasonTicketExceptionsPage, packageId, linkedMasterProduct)
    End Function

#End Region

#Region "Basket Functions"

    Public Sub Basket_UpdateBasketDeliveryCountry()
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults
        Dim returnError As Boolean = False
        Dim returnErrorCode As String = String.Empty
        Dim missingParam As Boolean = False
        Dim exitLoop As Boolean = False
        Dim count As Integer = 1
        Dim agentProfile As New Talent.eCommerce.Agent

        Try
            If HttpContext.Current.Session("DeliveryDetails") Is Nothing OrElse CType(HttpContext.Current.Session("DeliveryDetails"), DEDeliveryDetails).CountryCode.Trim.Length <= 0 Then
                missingParam = True
            End If
        Catch ex As Exception
            missingParam = True
        End Try

        If Not missingParam Then
            Dim deATI As New Talent.Common.DEAmendTicketingItems
            With deATI
                .CustomerSelection = ""
                .SessionID = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
                If Not CType(HttpContext.Current.Profile, TalentProfile).IsAnonymous Then
                    .CustomerNo = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID.PadLeft(12, "0")
                Else
                    .CustomerNo = "000000000000"
                End If
                .Src = GlobalConstants.SOURCE
                .ByPassPreReqCheck = "N"
            End With

            Dim basket As New Talent.Common.TalentBasket
            basket.DeAmendTicketingItems = deATI
            basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            basket.Settings.PerformWatchListCheck = moduleDefaults.PerformAgentWatchListCheck
            basket.Settings.OriginatingSource = TEBUtilities.GetOriginatingSource(agentProfile.Name)
            Dim talDataObjects As New TalentDataObjects
            talDataObjects.Settings = TEBUtilities.GetSettingsObject()
            basket.FulfilmentFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(talDataObjects.Settings.BusinessUnit)
            basket.CardTypeFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(talDataObjects.Settings.BusinessUnit)
            Dim err As New Talent.Common.ErrorObj
            err = basket.AmendTicketingItemsReturnBasket

            returnErrorCode = CheckResponseForError(basket.ResultDataSet, err)
            If returnErrorCode.Length > 0 Then
                returnError = True
            End If
        Else
            returnError = True
            returnErrorCode = "PARAMERR"
            Talent.eCommerce.Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "B_UB_" & _missingParamErrorCode, "Missing Parameter in TicketingGateway - Basket_UpdateBasket", "Error")
        End If

        If returnError Then
            HttpContext.Current.Session("TicketingGatewayError") = returnErrorCode
            HttpContext.Current.Session("TalentErrorCode") = returnErrorCode
            HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
        End If
    End Sub
    ''' <summary>
    ''' Update the basket based on the current session object
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Basket_UpdateBasket()
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults
        Dim returnError As Boolean = False
        Dim returnErrorCode As String = String.Empty
        Dim missingParam As Boolean = False
        Dim exitLoop As Boolean = False
        Dim count As Integer = 1
        Dim agentProfile As New Talent.eCommerce.Agent

        Try
            If HttpContext.Current.Session("RedirectUpdateBasket") Is Nothing Then
                missingParam = ExtractQueryStringValues()
            Else
                If String.IsNullOrEmpty(HttpContext.Current.Session("RedirectUpdateBasket")) Then
                    missingParam = ExtractQueryStringValues()
                Else
                    Dim sessionRedirectUpdateBasket As String = HttpContext.Current.Session("RedirectUpdateBasket")
                    HttpContext.Current.Session.Remove("RedirectUpdateBasket")
                    missingParam = ExtractSessionValues(sessionRedirectUpdateBasket)
                End If
            End If
        Catch ex As Exception
            missingParam = ExtractQueryStringValues()
        End Try

        If Not missingParam Then
            Dim deATI As New Talent.Common.DEAmendTicketingItems
            With deATI
                .CustomerSelection = _customerSelection
                .SessionID = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
                If Not CType(HttpContext.Current.Profile, TalentProfile).IsAnonymous Then
                    .CustomerNo = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID.PadLeft(12, "0")
                Else
                    .CustomerNo = "000000000000"
                End If

                .Src = GlobalConstants.SOURCE

                Dim i As Integer
                For i = 0 To _product.Count - 1
                    Dim deTBI As New Talent.Common.DETicketingBasketItem
                    deTBI.BulkSalesID = _bulkSalesID(i)
                    deTBI.BulkSalesQuantity = _bulkSalesQuantity(i)
                    deTBI.PriceBand = _concession(i)
                    deTBI.ProductCode = _product(i)
                    If _productType(i).Equals("M") Then
                        If CType(HttpContext.Current.Profile, TalentProfile).IsAnonymous Then
                            deTBI.Seat = _seat(i)
                        Else
                            deTBI.Seat = _seat(i)
                        End If
                    ElseIf _productType(i).Equals("T") Then
                        deTBI.Seat = _seat(i)
                    ElseIf IsItPackageItem(i) Then
                        deTBI.PackageID = _seat(i)
                    Else
                        If _seat(i).length > 0 Then
                            deTBI.Seat = _seat(i).Substring(0, 3).trim & "/" & _seat(i).Substring(3, 4).trim & "/" & _seat(i).Substring(7, 4).trim & "/" & _seat(i).Substring(11, 4).trim
                            If _seat(i).ToString().Length > 15 Then
                                If _seat(i).Substring(15, 1).trim <> "" Then
                                    deTBI.Seat = deTBI.Seat.Trim & "/" & _seat(i).Substring(15, 1).trim
                                End If
                            End If
                        Else
                            deTBI.Seat = _seat(i)
                        End If
                    End If
                    deTBI.AllocatedMemberNo = _customer(i).padleft(12, "0")
                    deTBI.PurchaseMemberNo = .CustomerNo.PadLeft(12, "0")
                    deTBI.PriceCode = _priceCode(i)
                    deTBI.FulfilmentMethod = _fulfilmentMethod(i)
                    deTBI.PriceCode = _priceCode(i)
                    If _priceCodeOverridden(i).ToString.Trim = "" Then
                        deTBI.PriceCodeOverridden = _priceCode(i)
                    Else
                        deTBI.PriceCodeOverridden = _priceCodeOverridden(i)
                    End If
                    deTBI.AllocatedSeat.FormattedSeat = _allocatedSeat(i)
                    deATI.CollAmendItems.Add(deTBI)
                Next
                If agentProfile.IsAgent AndAlso agentProfile.Name.Length > 0 AndAlso agentProfile.Type = "2" Then
                    .ByPassPreReqCheck = "Y"
                Else
                    .ByPassPreReqCheck = "N"
                End If
            End With

            If _customerSelection = "Y" Then
                Dim customer As New TalentCustomer
                Dim deCustV11 As New DECustomerV11
                Dim deCustV1 As New DECustomer
                deCustV11.DECustomersV1.Add(deCustV1)
                With customer
                    .DeV11 = deCustV11
                End With
                deCustV1.CustomerNumber = deATI.CustomerNo
                deCustV1.IncludeBoxOfficeLinks = agentProfile.IsAgent
                customer.CustomerAssociationsClearSession()
            End If

            Dim basket As New Talent.Common.TalentBasket
            basket.DeAmendTicketingItems = deATI
            basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            basket.Settings.PerformWatchListCheck = moduleDefaults.PerformAgentWatchListCheck
            basket.Settings.OriginatingSource = TEBUtilities.GetOriginatingSource(agentProfile.Name)
            Dim talDataObjects As New TalentDataObjects
            talDataObjects.Settings = TEBUtilities.GetSettingsObject()
            basket.FulfilmentFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(talDataObjects.Settings.BusinessUnit)
            basket.CardTypeFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(talDataObjects.Settings.BusinessUnit)
            Dim err As New Talent.Common.ErrorObj
            err = basket.AmendTicketingItemsReturnBasket

            returnErrorCode = CheckResponseForError(basket.ResultDataSet, err)
            If returnErrorCode.Length > 0 Then
                returnError = True
            End If
        Else
            returnError = True
            returnErrorCode = "PARAMERR"
            Talent.eCommerce.Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "B_UB_" & _missingParamErrorCode, "Missing Parameter in TicketingGateway - Basket_UpdateBasket", "Error")
        End If

        If returnError Then
            HttpContext.Current.Session("TicketingGatewayError") = returnErrorCode
            HttpContext.Current.Session("TalentErrorCode") = returnErrorCode
        Else
            If Not String.IsNullOrEmpty(HttpContext.Current.Request.QueryString("returnUrl")) Then
                Dim urlString = HttpContext.Current.Request.QueryString("returnUrl")
                Dim validatedUri As Uri
                If Uri.TryCreate(urlString, UriKind.RelativeOrAbsolute, validatedUri) Then
                    HttpContext.Current.Response.Redirect(urlString)
                End If
            End If
        End If

        HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
    End Sub

    ''' <summary>
    ''' Checkout action performed from the basket page/mini basket for Retail
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Basket_Checkout_Retail(ByVal Profile As TalentProfile)

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' CheckoutDeliveryAddress.ascx.vb | Page_Init() + Page_PreRender()
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim eComDefs As ECommerceModuleDefaults
        Dim defs As ECommerceModuleDefaults.DefaultValues
        eComDefs = New ECommerceModuleDefaults
        defs = eComDefs.GetDefaults

        Talent.eCommerce.Utilities.DoSapOciCheckout()
        Talent.eCommerce.Utilities.CheckBasketFreeItemHasOptions()
        If Talent.eCommerce.Utilities.UserUnderAge(Profile) Then
            HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
        Else
            If Not defs.AllowCheckoutWhenNoStock AndAlso Not Talent.eCommerce.Utilities.AllInStock_BackEndCheck(Profile) Then
                HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
            End If

            '-------------------------------------------------------
            ' Check for discontinued products which are out of stock
            '-------------------------------------------------------
            If defs.PerformDiscontinuedProductCheck Then
                For Each bi As TalentBasketItem In Profile.Basket.BasketItems
                    If bi.STOCK_ERROR AndAlso bi.STOCK_ERROR_CODE = "DISC" Then
                        HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                    End If
                Next
            End If

            '-------------------------------------------------------
            ' Check for mandatory account codes
            '-------------------------------------------------------
            If Not Profile.PartnerInfo.Details Is Nothing Then
                If Not Profile.PartnerInfo.Details.COST_CENTRE Is Nothing And Not Profile.PartnerInfo.Details.COST_CENTRE Is String.Empty Then
                    For Each bi As TalentBasketItem In Profile.Basket.BasketItems
                        If bi.Cost_Centre = Nothing Or bi.Cost_Centre = String.Empty Or bi.Account_Code = Nothing Or bi.Account_Code = String.Empty Then
                            HttpContext.Current.Session("TalentErrorCode") = "CC"
                            HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                        End If
                    Next
                End If
            End If


            '-------------------------------------
            ' Check for alt products.  If any exist then redirect to basket allowing user to select alt products
            '-------------------------------------
            If defs.RetrieveAlternativeProductsAtCheckout Then
                Dim ds As New Data.DataSet
                ds = Talent.eCommerce.Utilities.RetrieveAlternativeProducts(Profile)

                If Not ds Is Nothing AndAlso ds.Tables.Count > 0 AndAlso ds.Tables.Item("ALTPRODUCTRESULTS").Rows.Count > 0 Then
                    ' Save to session - this will be checked and cleared immediately in basket
                    HttpContext.Current.Session("AlternativeProducts") = ds
                    HttpContext.Current.Session.Remove("CheckoutBreadCrumbTrail")
                    HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                End If

            End If
            If defs.DISPLAY_PAGE_BEFORE_CHECKOUT Then
                If HttpContext.Current.Session.Item("CheckoutPromotionsShown") Is Nothing _
                    OrElse Not CBool(HttpContext.Current.Session.Item("CheckoutPromotionsShown")) Then
                    HttpContext.Current.Session.Item("CheckoutPromotionsShown") = True
                    HttpContext.Current.Response.Redirect(defs.PAGE_BEFORE_CHECKOUT)
                End If
            End If
        End If

        '        If defs.UseEPOSOptions Then HttpContext.Current.Response.Redirect("~/PagesLogin/Checkout/CheckoutOrderSummary.aspx")
        If defs.UseEPOSOptions Then
            Exit Sub
        End If


        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' Start CheckoutDeliveryAddress.ascx.vb | proceed_Click()
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Try
            HttpContext.Current.Session.Add("CheckoutBasketState", Profile.Basket)
        Catch ex As Exception
        End Try
        Checkout.CheckBasketValidity()

        Dim deliveryDate As Date = Date.MinValue
        If HttpContext.Current.Session("DeliveryDate") IsNot Nothing Then
            deliveryDate = HttpContext.Current.Session("DeliveryDate")
        End If

        ' Set default dummy details to be used when creating the order
        Dim DeliveryInstructions As String = ""
        Dim DeliveryContact As String = ""
        Dim building As String = ""
        Dim Address2 As String = ""
        Dim Address3 As String = ""
        Dim Address4 As String = ""
        Dim Address5 As String = ""
        Dim postcode As String = ""
        Dim country As String = ""
        Dim PurchaseOrder As String = ""
        Dim addressExternalID As String = ""

        '
        ' Need to specify a country to enable DeliverySelection to work correctly:
        '
        ' 1.  Use country on users main address 
        ' 2.  Or use default country for BU
        ' 3.  (Store as either value or code)
        '

        ' Populate CountryDLL
        Dim CountryDDL As New DropDownList
        CountryDDL.DataSource = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, "DELIVERY", "COUNTRY")
        CountryDDL.DataTextField = "Text"
        CountryDDL.DataValueField = "Value"
        CountryDDL.DataBind()

        ' Use country on users main address
        Dim enumer As Collections.Generic.Dictionary(Of String, TalentProfileAddress).KeyCollection.Enumerator
        enumer = Profile.User.Addresses.Keys.GetEnumerator
        Dim userAddress As TalentProfileAddress
        While enumer.MoveNext
            userAddress = Profile.User.Addresses(enumer.Current)
            building = userAddress.Address_Line_1
            Address2 = userAddress.Address_Line_2
            Address3 = userAddress.Address_Line_3
            Address4 = userAddress.Address_Line_4
            Address5 = userAddress.Address_Line_5
            postcode = userAddress.Post_Code
            country = userAddress.Country
            Exit While
        End While
        If country <> "" Then
            Dim i As Integer = 0
            For Each item As System.Web.UI.WebControls.ListItem In CountryDDL.Items
                If item.Text.ToLower.Trim = country.ToLower.Trim Then
                    CountryDDL.SelectedItem.Text = item.Text
                    CountryDDL.SelectedItem.Value = item.Value
                    item.Selected = True
                    CountryDDL.SelectedIndex = i
                End If
                i += 1
            Next

        End If

        ' Overide to default for BU if required
        If defs.UseDefaultCountryOnDeliveryAddress Then
            Dim defaultCountry As String = TalentCache.GetDefaultCountryForBU()
            If defaultCountry <> String.Empty Then
                CountryDDL.SelectedValue = defaultCountry
                For Each item As System.Web.UI.WebControls.ListItem In CountryDDL.Items
                    If item.Text.ToLower.Trim = defaultCountry.ToLower.Trim Then
                        CountryDDL.SelectedItem.Text = item.Text
                        CountryDDL.SelectedItem.Value = item.Value
                        item.Selected = True
                    End If
                Next
            End If
        End If

        ' Store as name or code?
        If defs.StoreCountryAsWholeName Then
            country = CountryDDL.SelectedItem.Text
        Else
            country = CountryDDL.SelectedItem.Value
        End If


        Dim order As New Order(DeliveryInstructions,
                                DeliveryContact,
                                building,
                                Address2,
                                Address3,
                                Address4,
                                Address5,
                                postcode,
                                country,
                                PurchaseOrder,
                                addressExternalID,
                                deliveryDate, CountryDDL.SelectedItem.Value)

        'If CreateOrder() Then
        If order.CreateOrder() Then
            Try
                Dim status As New TalentBasketDatasetTableAdapters.tbl_order_statusTableAdapter
                status.Insert_Order_Status_Flow(TalentCache.GetBusinessUnit,
                                    Profile.Basket.TempOrderID,
                                    Talent.Common.Utilities.GetOrderStatus("DELIVERY"),
                                    Now,
                                    "")
            Catch ex As Exception
                Logging.WriteLog(Profile.UserName, "UCCAPR-010", ex.Message, "Error Inserting Order Status", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "TicketingGateayFunctions.ascx")
            End Try
            '            Response.Redirect("~/PagesLogin/Checkout/CheckoutOrderSummary.aspx")
        Else
            '            ErrorLabel.Text = ucr.Content("OrderCreationErrorText", _languageCode, True)
            HttpContext.Current.Session("TalentErrorCode") = "Order Creation error"
            HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx?errorMessage=")
        End If


        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' Start CheckoutOrderSummary.ascx.vb | Page_Init() + Page_PreRender() + Page_Load()
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If (HttpContext.Current.Session("RedirectByPromotionBox") IsNot Nothing) Then
            HttpContext.Current.Session("CheckoutBasketState") = Profile.Basket
            HttpContext.Current.Session.Remove("RedirectByPromotionBox")
        End If

        ' Min/Max checks
        Dim minPurchaseQuantity = defs.MinimumPurchaseQuantity
        Dim minPurchaseAmount = defs.MinimumPurchaseAmount
        Dim useMinQuantity = defs.UseMinimumPurchaseQuantity
        Dim useMinAmount = defs.UseMinimumPurchaseAmount
        If Profile.User.Details.Use_Minimum_Purchase_Quantity Then
            useMinQuantity = True
            minPurchaseQuantity = Profile.User.Details.Minimum_Purchase_Quantity
        ElseIf Profile.PartnerInfo.Details.Use_Minimum_Purchase_Quantity Then
            useMinQuantity = True
            minPurchaseQuantity = Profile.PartnerInfo.Details.Minimum_Purchase_Quantity
        End If
        If Profile.User.Details.Use_Minimum_Purchase_Amount Then
            useMinAmount = True
            minPurchaseAmount = Profile.User.Details.Minimum_Purchase_Amount
        ElseIf Profile.PartnerInfo.Details.Use_Minimum_Purchase_Amount Then
            useMinAmount = True
            minPurchaseAmount = Profile.PartnerInfo.Details.Minimum_Purchase_Amount
        End If
        If useMinQuantity AndAlso Profile.Basket.BasketItems.Count < minPurchaseQuantity Then
            HttpContext.Current.Session("TalentErrorCode") = "QuantityTooLow"
            HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
        End If
        If useMinAmount AndAlso Profile.Basket.BasketSummary.TotalBasket < defs.MinimumPurchaseAmount Then
            HttpContext.Current.Session("TalentErrorCode") = "AmountTooLow"
            HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
        End If

        ' Credit check
        Dim PaymentTypeDDL As DropDownList = Talent.eCommerce.Utilities.SetDDLValues(Profile)
        Dim errorText As String = String.Empty
        errorText = Talent.eCommerce.Utilities.CreditCheck(Profile, PaymentTypeDDL)
        If errorText <> String.Empty Then
            HttpContext.Current.Session("TalentErrorCode") = errorText
            HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
        End If


        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' Start CheckoutOrderSummary.ascx.vb | proceed_Click()
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' already done this!
        '        Talent.eCommerce.Utilities.CheckBasketFreeItemHasOptions()

        HttpContext.Current.Session("HSBCRequest") = Nothing
        HttpContext.Current.Session("StoredDeliveryAddress") = Nothing
        HttpContext.Current.Session("StoredTicketingDeliveryAddress") = Nothing
        HttpContext.Current.Session("StoredRetailDeliveryAddress") = Nothing


    End Sub

    ''' <summary>
    ''' Checkout action performed from the basket page/mini basket
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Basket_Checkout(Optional ByVal redirectOnBasketCheckout As Boolean = True)
        Dim returnError As Boolean = False
        Dim returnErrorCode As String = String.Empty
        Dim missingParam As Boolean = False
        Dim exitLoop As Boolean = False
        Dim count As Integer = 1
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults
        HttpContext.Current.Session("TicketingGatewayError") = Nothing
        Try
            If HttpContext.Current.Session("RedirectCheckout") Is Nothing Then
                missingParam = ExtractQueryStringValues()
            Else
                If String.IsNullOrEmpty(HttpContext.Current.Session("RedirectCheckout")) Then
                    missingParam = ExtractQueryStringValues()
                Else
                    Dim sessionRedirectCheckout As String = HttpContext.Current.Session("RedirectCheckout")
                    HttpContext.Current.Session.Remove("RedirectCheckout")
                    missingParam = ExtractSessionValues(sessionRedirectCheckout)
                End If
            End If
        Catch ex As Exception
            missingParam = ExtractQueryStringValues()
        End Try

        If Not missingParam Then
            ' No seats passed in, go straight to checkout
            If _product.Count > 0 Then
                Dim deATI As New DEAmendTicketingItems
                With deATI
                    .SessionID = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
                    If Not CType(HttpContext.Current.Profile, TalentProfile).IsAnonymous Then
                        .CustomerNo = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID.PadLeft(12, "0")
                    Else
                        .CustomerNo = GlobalConstants.GENERIC_CUSTOMER_NUMBER
                    End If
                    .Src = GlobalConstants.SOURCE

                    Dim i As Integer
                    For i = 0 To _product.Count - 1
                        Dim deTBI As New DETicketingBasketItem
                        deTBI.PriceBand = _concession(i)
                        deTBI.ProductCode = _product(i)
                        If _productType(i).Equals("M") Then
                            deTBI.Seat = _seat(i)
                        ElseIf _productType(i).Equals("T") Then
                            deTBI.Seat = _seat(i)
                        ElseIf IsItPackageItem(i) Then
                            deTBI.PackageID = _seat(i)
                        Else
                            If _seat(i).ToString().Length > 0 Then
                                deTBI.Seat = _seat(i).Substring(0, 3).trim & "/" & _seat(i).Substring(3, 4).trim & "/" & _seat(i).Substring(7, 4).trim & "/" & _seat(i).Substring(11, 4).trim
                                If _seat(i).ToString().Length > 15 Then
                                    If _seat(i).Substring(15, 1).trim <> "" Then
                                        deTBI.Seat = deTBI.Seat.Trim & "/" & _seat(i).Substring(15, 1).trim
                                    End If
                                End If
                            Else
                                deTBI.Seat = _seat(i)
                            End If
                        End If
                        deTBI.AllocatedMemberNo = _customer(i).padleft(12, "0")
                        deTBI.PurchaseMemberNo = .CustomerNo.PadLeft(12, "0")
                        deTBI.PriceCode = _priceCode(i)
                        If _priceCodeOverridden(i).ToString.Trim = "" Then
                            deTBI.PriceCodeOverridden = _priceCode(i)
                        Else
                            deTBI.PriceCodeOverridden = _priceCodeOverridden(i)
                        End If
                        deTBI.FulfilmentMethod = _fulfilmentMethod(i)
                        deTBI.BulkSalesID = _bulkSalesID(i)
                        deTBI.BulkSalesQuantity = _bulkSalesQuantity(i)
                        deTBI.AllocatedSeat.FormattedSeat = _allocatedSeat(i)
                        deATI.CollAmendItems.Add(deTBI)
                    Next
                    If Not HttpContext.Current.Session("Agent") Is Nothing AndAlso Not HttpContext.Current.Session("Agent") = "" AndAlso
                        Not HttpContext.Current.Session("AgentType") Is Nothing AndAlso HttpContext.Current.Session("AgentType") = "2" Then
                        .ByPassPreReqCheck = "Y"
                    Else
                        .ByPassPreReqCheck = "N"
                    End If
                End With

                Dim basket As New Talent.Common.TalentBasket
                basket.DeAmendTicketingItems = deATI
                basket.Settings = TEBUtilities.GetSettingsObject()
                basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
                basket.Settings.StoredProcedureGroup = TEBUtilities.GetStoredProcedureGroup
                basket.Settings.PerformWatchListCheck = moduleDefaults.PerformAgentWatchListCheck
                basket.Settings.OriginatingSource = TEBUtilities.GetOriginatingSource(HttpContext.Current.Session.Item("Agent"))
                Dim talDataObjects As New TalentDataObjects
                talDataObjects.Settings = TEBUtilities.GetSettingsObject()
                basket.FulfilmentFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(talDataObjects.Settings.BusinessUnit)
                basket.CardTypeFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(talDataObjects.Settings.BusinessUnit)
                Dim err As New ErrorObj
                err = basket.AmendTicketingItemsReturnBasket
                returnErrorCode = CheckResponseForError(basket.ResultDataSet, err)
                If returnErrorCode.Length > 0 Then
                    returnError = True
                End If
            End If
        Else
            returnError = True
            returnErrorCode = "PARAMERR"
            Talent.eCommerce.Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "B_CH_" & _missingParamErrorCode, "Missing Parameter in TicketingGateway - Basket_Checkout", "Error")
        End If

        If returnError Then
            HttpContext.Current.Session("TicketingGatewayError") = returnErrorCode
            HttpContext.Current.Session("TalentErrorCode") = returnErrorCode
            HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
        Else
            If redirectOnBasketCheckout Then HttpContext.Current.Response.Redirect("~/PagesLogin/Checkout/Checkout.aspx")
        End If
    End Sub

    ''' <summary>
    ''' To clear backend basket as well as front end ticketing basket
    ''' </summary>
    Public Sub Basket_ClearBasket(ByVal canRedirect As Boolean)
        Dim returnError As Boolean = False,
            returnErrorCode As String = String.Empty,
            missingParam As Boolean = False

        'We need to remove any ticketing items when the user logs out.  A new basket id will exist
        'for this user but we must clear the old basket.  Override the header id.
        Dim userProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)

        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults

        Dim basketHeaderId As Long = 0
        If Not String.IsNullOrEmpty(HttpContext.Current.Request.QueryString("basketHeaderId")) Then
            basketHeaderId = userProfile.Basket.Basket_Header_ID
            userProfile.Basket.Basket_Header_ID = HttpContext.Current.Request.QueryString("basketHeaderId")
        End If

        Dim deTID As New Talent.Common.DETicketingItemDetails
        With deTID
            .SessionId = userProfile.Basket.Basket_Header_ID
            If Not userProfile.IsAnonymous Then
                .CustomerNo = userProfile.User.Details.LoginID.PadLeft(12, "0")
            Else
                .CustomerNo = "000000000000"
            End If
            .Src = "W"
        End With

        Dim basket As New Talent.Common.TalentBasket
        basket.De = deTID
        basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
        basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
        basket.Settings.OriginatingSource = TEBUtilities.GetOriginatingSource(HttpContext.Current.Session.Item("Agent"))
        basket.Settings.PerformWatchListCheck = moduleDefaults.PerformAgentWatchListCheck
        Dim talDataObjects As New TalentDataObjects
        talDataObjects.Settings = TEBUtilities.GetSettingsObject()
        basket.FulfilmentFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(talDataObjects.Settings.BusinessUnit)
        basket.CardTypeFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(talDataObjects.Settings.BusinessUnit)
        Dim err As New Talent.Common.ErrorObj
        err = basket.RemoveTicketingItemsReturnBasket

        returnErrorCode = CheckResponseForError(basket.ResultDataSet, err)

        If returnErrorCode.Length > 0 Then
            returnError = True
        End If

        If Not returnError Then
            HttpContext.Current.Session("StoredDeliveryAddress") = Nothing
            DeleteBasketDetail()
        Else
            HttpContext.Current.Session("TicketingGatewayError") = returnErrorCode
            HttpContext.Current.Session("TalentErrorCode") = returnErrorCode
        End If

        'Reset the basket header id when called from logout
        If basketHeaderId > 0 Then
            userProfile.Basket.Basket_Header_ID = basketHeaderId
        End If
        If HttpContext.Current.Session("CanClearAllSessions") IsNot Nothing Then
            Talent.eCommerce.Utilities.ClearAllSessions()
        End If
        If canRedirect Then
            If Not String.IsNullOrEmpty(HttpContext.Current.Request.QueryString("returnUrl")) Then
                HttpContext.Current.Response.Redirect(HttpContext.Current.Request.QueryString("returnUrl"))
            Else
                HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
            End If
        End If

    End Sub

    ''' <summary>
    ''' Removes the tbl_basket_header record for the current login ID.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DeleteBasketHeader()
        Dim basketHeader As New TalentBasketDatasetTableAdapters.tbl_basket_headerTableAdapter
        Try
            basketHeader.Delete_Users_Basket_Before_Migration(TalentCache.GetBusinessUnit, TalentCache.GetDefaultPartner, CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID.PadLeft(12, "0"))
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Deletes the tbl_basket_detail records for ticketing items where the the current profile basket header id matches the current session.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DeleteBasketDetail()

        Dim deleteStr As String = " DELETE FROM tbl_basket_detail " &
                                                    " WHERE MODULE = 'Ticketing' " &
                                                    " AND BASKET_HEADER_ID = @BasketHeaderID " &
                                                    " "

        Dim connStr As String = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString,
            cmd As New System.Data.SqlClient.SqlCommand(deleteStr, New System.Data.SqlClient.SqlConnection(connStr))
        Try
            cmd.Connection.Open()
        Catch ex As Exception
        End Try

        If cmd.Connection.State = Data.ConnectionState.Open Then
            cmd.CommandText = deleteStr
            cmd.Parameters.Add("@BasketHeaderID", Data.SqlDbType.BigInt).Value = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
            cmd.ExecuteNonQuery()
        End If

        Try
            cmd.Connection.Close()
        Catch ex As Exception
        End Try

        Dim payment As New Talent.Common.TalentPayment
        Dim settings As DESettings = Talent.eCommerce.Utilities.GetSettingsObject()
        payment.RemoveItemFromCache("RetrieveCashback" & settings.Company & CType(HttpContext.Current.Profile, TalentProfile).UserName)
        'now delete basket fees
        DeleteBasketFees()
    End Sub

    ''' <summary>
    ''' Calls the back end to update the WS003 basket with tbl_basket_details
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub RefreshBasketContent()
        Dim basket As New Talent.Common.TalentBasket
        basket.De.SessionId = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
        If HttpContext.Current.Profile.IsAnonymous Then
            basket.De.CustomerNo = GlobalConstants.GENERIC_CUSTOMER_NUMBER
        Else
            basket.De.CustomerNo = CType(HttpContext.Current.Profile, TalentProfile).UserName
        End If
        basket.De.Src = GlobalConstants.SOURCE
        basket.Settings = TEBUtilities.GetSettingsObject()
        basket.Settings.OriginatingSource = TEBUtilities.GetOriginatingSource(HttpContext.Current.Session.Item("Agent"))
        Dim Err As ErrorObj = basket.RetrieveTicketingItems()
        Dim returnErrorCode As String = CheckResponseForError(basket.ResultDataSet, Err)
    End Sub

    ''' <summary>
    ''' Updates tbl_basket_header with the given dataset
    ''' </summary>
    ''' <param name="results">Dataset of the basket from TALENT</param>
    ''' <remarks></remarks>
    Public Sub UpdateBasketHeader(ByVal results As DataSet)
        Try
            If results.Tables.Count > 1 Then

                Dim UpdateStr As String = "UPDATE tbl_basket_header SET MARKETING_CAMPAIGN = @MarketingCampaign," &
                                                " USER_SELECT_FULFIL = @UserSelectFulfil," &
                                                " PAYMENT_OPTIONS = @PaymentOptions," &
                                                " RESTRICT_PAYMENT_OPTIONS = @restrictpaymentoptions," &
                                                " PAYMENT_ACCOUNT_ID = @PaymentAccountId, " &
                                                " CAT_MODE = @CatMode, " &
                                                " CAT_PRICE = @CatPrice " &
                                                " WHERE BASKET_HEADER_ID = @BasketHeaderID"

                Dim connStr As String = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                Dim cmd As New System.Data.SqlClient.SqlCommand(UpdateStr, New System.Data.SqlClient.SqlConnection(connStr))

                With cmd
                    .Connection.Open()
                    .Parameters.Add("@BasketHeaderID", Data.SqlDbType.BigInt).Value = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID

                    If results.Tables(1).Rows(0).Item("MarketingCampaign").ToString.Trim = "Y" Then
                        .Parameters.Add("@MarketingCampaign", Data.SqlDbType.Bit).Value = True
                    Else
                        .Parameters.Add("@MarketingCampaign", Data.SqlDbType.Bit).Value = False
                    End If

                    .Parameters.Add("@UserSelectFulfil", Data.SqlDbType.NVarChar).Value = results.Tables(1).Rows(0).Item("UserSelectFulfilment").ToString.Trim
                    .Parameters.Add("@PaymentOptions", Data.SqlDbType.NVarChar).Value = results.Tables(1).Rows(0).Item("PaymentOptions").ToString().Trim()

                    If results.Tables(1).Rows(0).Item("restrictpaymentoptions").ToString().Trim() = "Y" Then
                        .Parameters.Add("@restrictpaymentoptions", Data.SqlDbType.Bit).Value = True
                    Else
                        .Parameters.Add("@restrictpaymentoptions", Data.SqlDbType.Bit).Value = False
                    End If

                    .Parameters.Add("@PaymentAccountId", Data.SqlDbType.NVarChar).Value = results.Tables(1).Rows(0).Item("PaymentAccountId").ToString.Trim
                    .Parameters.Add("@CatMode", Data.SqlDbType.VarChar).Value = results.Tables(1).Rows(0).Item("CATMode").ToString.Trim
                    .Parameters.Add("@CatPrice", Data.SqlDbType.VarChar).Value = results.Tables(1).Rows(0).Item("CATPrice").ToString.Trim
                    cmd.ExecuteNonQuery()
                End With
            End If

        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Verify the basket and update it with the external order number
    ''' </summary>
    ''' <param name="basketHeaderId">The given basket header id</param>
    ''' <param name="verificationMode">The verification mode</param>
    ''' <param name="externalOrderNumber">The external order number to update on the basket</param>
    ''' <returns>Return any error codes</returns>
    ''' <remarks></remarks>
    Public Function VerifyAndUpdateExtOrderNumberToBasket(ByVal basketHeaderId As String, ByVal verificationMode As String, ByVal externalOrderNumber As String) As String
        Dim err As New ErrorObj
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults
        Dim returnErrorCode As String = String.Empty
        'Call Backend
        Dim talBasket As New Talent.Common.TalentBasket
        Dim amendBasketEntity As New DEAmendBasket
        With amendBasketEntity
            .BasketId = basketHeaderId
            .BasketVerificationMode = verificationMode
            .ExternalOrderNumber = externalOrderNumber
        End With
        With talBasket
            .Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            .Settings.BusinessUnit = TalentCache.GetBusinessUnit
            .Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            .Settings.Cacheing = False
            .Settings.OriginatingSource = TEBUtilities.GetOriginatingSource(HttpContext.Current.Session.Item("Agent"))
            .Dep = amendBasketEntity
        End With

        err = talBasket.VerifyAndUpdateExtOrderNumberToBasket
        returnErrorCode = CheckResponseForError(talBasket.ResultDataSet, err)
        If returnErrorCode.Length > 0 Then
            err.HasError = True
            err.ErrorMessage = err.ErrorMessage & ";" & returnErrorCode
        End If
        Return returnErrorCode
    End Function

    Public Sub Basket_RemoveFromBasket(ByVal canRedirect As Boolean, ByVal returnURL As String, ByVal productCode As String, ByVal seat As String, ByVal priceCode As String, ByVal packageID As String, ByVal customerNumber As String)
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults
        Dim returnError As Boolean = False
        Dim returnErrorCode As String = String.Empty
        Dim missingParam As Boolean = False
        Dim product As String = productCode
        Dim originalCust As String = customerNumber
        Dim orphanSeatRemaining As Boolean = False

        If String.IsNullOrEmpty(product) Then
            missingParam = True
        ElseIf String.IsNullOrEmpty(originalCust) Then
            missingParam = True
        End If
        If String.IsNullOrEmpty(seat) Then
            If String.IsNullOrEmpty(packageID) Then
                missingParam = True
            End If
        End If

        If Not missingParam Then
            Dim deTID As New Talent.Common.DETicketingItemDetails
            With deTID
                .SessionId = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
                If Not HttpContext.Current.Profile.IsAnonymous Then
                    .CustomerNo = originalCust
                Else
                    .CustomerNo = "000000000000"
                End If
                .ProductCode = product
                .SeatDetails1.Stand = seat.Substring(0, 3)
                .SeatDetails1.Area = seat.Substring(3, 4)
                .SeatDetails1.Row = seat.Substring(7, 4)
                .SeatDetails1.Seat = seat.Substring(11, 4)
                .SeatDetails1.AlphaSuffix = seat.Substring(15, 1)
                .PriceCode = priceCode
                .PackageID = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(packageID)
                .Src = "W"
                If Not HttpContext.Current.Session("Agent") Is Nothing AndAlso Not HttpContext.Current.Session("Agent") = "" AndAlso Not HttpContext.Current.Session("AgentType") Is Nothing AndAlso HttpContext.Current.Session("AgentType") = "2" Then
                    .ByPassPreReqCheck = "Y"
                Else
                    .ByPassPreReqCheck = "N"
                End If
            End With

            Dim basket As New Talent.Common.TalentBasket
            basket.De = deTID
            basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            basket.Settings.OriginatingSource = Talent.eCommerce.Utilities.GetOriginatingSource(HttpContext.Current.Session.Item("Agent"))
            basket.Settings.PerformWatchListCheck = moduleDefaults.PerformAgentWatchListCheck
            Dim talDataObjects As New TalentDataObjects
            talDataObjects.Settings = TEBUtilities.GetSettingsObject()
            basket.FulfilmentFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(talDataObjects.Settings.BusinessUnit)
            basket.CardTypeFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(talDataObjects.Settings.BusinessUnit)
            Dim err As New Talent.Common.ErrorObj
            err = basket.RemoveTicketingItemsReturnBasket
            returnErrorCode = CheckResponseForError(basket.ResultDataSet, err)
            orphanSeatRemaining = basket.OrphanSeatRemaining

            If returnErrorCode.Length > 0 Then returnError = True
        Else
            returnError = True
            returnErrorCode = "PARAMERR"
            Talent.eCommerce.Logging.WriteLog(HttpContext.Current.Profile.UserName, "B_RFB_" & _missingParamErrorCode, "Missing Parameter in TicketingGatewayFunction - Basket_RemoveFromBasket", "Error")
        End If

        _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
        If Not String.IsNullOrWhiteSpace(returnURL) Then
            _redirectUrl = HttpContext.Current.Request.QueryString("returnURL")
        End If

        If returnError Then
            HttpContext.Current.Session("TicketingGatewayError") = returnErrorCode
            HttpContext.Current.Session("TalentErrorCode") = returnErrorCode
            canRedirect = True
            _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
        End If
        If Not returnError AndAlso orphanSeatRemaining Then
            HttpContext.Current.Session("TicketingGatewayError") = GlobalConstants.ORPHANSEATERROR
            HttpContext.Current.Session("TalentErrorCode") = GlobalConstants.ORPHANSEATERROR
        End If
        If canRedirect Then
            HttpContext.Current.Response.Redirect(_redirectUrl)
        End If
    End Sub

    Public Sub Basket_RemoveBulkRecordFromBasket(ByVal canRedirect As Boolean, ByVal returnURL As String, ByVal customerNumber As String, ByVal bulkSalesID As String)
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults
        Dim returnError As Boolean = False
        Dim returnErrorCode As String = String.Empty
        Dim orphanSeatRemaining As Boolean = False

        If bulkSalesID.Length > 0 Then
            Dim deTID As New Talent.Common.DETicketingItemDetails
            Dim basket As New Talent.Common.TalentBasket
            Dim talDataObjects As New TalentDataObjects
            Dim err As New Talent.Common.ErrorObj

            With deTID
                .SessionId = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
                If Not HttpContext.Current.Profile.IsAnonymous Then
                    .CustomerNo = customerNumber
                Else
                    .CustomerNo = GlobalConstants.GENERIC_CUSTOMER_NUMBER
                End If
                .BulkSalesID = bulkSalesID
                .Src = GlobalConstants.SOURCE
            End With
            basket.De = deTID
            basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            basket.Settings.OriginatingSource = Talent.eCommerce.Utilities.GetOriginatingSource(HttpContext.Current.Session.Item("Agent"))
            basket.Settings.PerformWatchListCheck = moduleDefaults.PerformAgentWatchListCheck
            talDataObjects.Settings = TEBUtilities.GetSettingsObject()
            basket.FulfilmentFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(talDataObjects.Settings.BusinessUnit)
            basket.CardTypeFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(talDataObjects.Settings.BusinessUnit)
            err = basket.RemoveTicketingItemsReturnBasket
            returnErrorCode = CheckResponseForError(basket.ResultDataSet, err)
            orphanSeatRemaining = basket.OrphanSeatRemaining
            If returnErrorCode.Length > 0 Then returnError = True
        Else
            returnError = True
            returnErrorCode = "PARAMERR"
            Talent.eCommerce.Logging.WriteLog(HttpContext.Current.Profile.UserName, "B_RFB_" & _missingParamErrorCode, "Missing Parameter in TicketingGatewayFunction - Basket_RemoveFromBasket", "Error")
        End If

        _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
        If Not String.IsNullOrWhiteSpace(returnURL) Then
            _redirectUrl = HttpContext.Current.Request.QueryString("returnURL")
        End If

        If returnError Then
            HttpContext.Current.Session("TicketingGatewayError") = returnErrorCode
            HttpContext.Current.Session("TalentErrorCode") = returnErrorCode
            canRedirect = True
            _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
        End If
        If Not returnError AndAlso orphanSeatRemaining Then
            HttpContext.Current.Session("TicketingGatewayError") = GlobalConstants.ORPHANSEATERROR
            HttpContext.Current.Session("TalentErrorCode") = GlobalConstants.ORPHANSEATERROR
        End If
        If canRedirect Then
            HttpContext.Current.Response.Redirect(_redirectUrl)
        End If
    End Sub

    ''' <summary>
    ''' This function is likely to be redundant now
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Basket_UpdateAndRemoveFromBasket()
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults
        Dim returnError As Boolean = False
        Dim returnErrorCode As String = String.Empty
        Dim missingParam As Boolean = False
        Dim removeProduct As String = HttpContext.Current.Request("removeProduct")
        Dim removeSeat As String = HttpContext.Current.Request("removeSeat")
        Dim exitLoop As Boolean = False
        Dim count As Integer = 1
        Dim talDataObjects As New TalentDataObjects

        If String.IsNullOrEmpty(removeProduct) Then
            missingParam = True
        ElseIf String.IsNullOrEmpty(removeSeat) Then
            missingParam = True
        Else
            missingParam = ExtractQueryStringValues()
        End If

        If Not missingParam Then
            Dim deATI As New Talent.Common.DEAmendTicketingItems
            With deATI
                .SessionID = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
                If Not CType(HttpContext.Current.Profile, TalentProfile).IsAnonymous Then
                    .CustomerNo = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID.PadLeft(12, "0")
                Else
                    .CustomerNo = GlobalConstants.GENERIC_CUSTOMER_NUMBER
                End If
                .Src = GlobalConstants.SOURCE

                Dim i As Integer
                For i = 0 To _product.Count - 1
                    Dim deTBI As New Talent.Common.DETicketingBasketItem
                    deTBI.PriceBand = _concession(i)
                    deTBI.ProductCode = _product(i)
                    If _productType(i).Equals("M") Then
                        ' Populate seat with original customer.  We have no seat for memberships so we need original customer to identify the correct record to update 
                        deTBI.Seat = _originalCust(i)
                    ElseIf _productType(i).Equals("T") Then
                        deTBI.Seat = _seat(i)
                    Else
                        deTBI.Seat = _seat(i).Substring(0, 3).trim & "/" & _seat(i).Substring(3, 4).trim & "/" & _seat(i).Substring(7, 4).trim & "/" & _seat(i).Substring(11, 4).trim
                        If _seat(i).Substring(15, 1).trim <> "" Then
                            deTBI.Seat = deTBI.Seat.Trim & "/" & _seat(i).Substring(15, 1).trim
                        End If
                    End If
                    deTBI.AllocatedMemberNo = _customer(i).padleft(12, "0")
                    deTBI.PurchaseMemberNo = .CustomerNo.PadLeft(12, "0")
                    deTBI.PriceCode = _priceCode(i)
                    deTBI.FulfilmentMethod = _fulfilmentMethod(i)
                    deATI.CollAmendItems.Add(deTBI)
                Next
                If Not HttpContext.Current.Session("Agent") Is Nothing AndAlso Not HttpContext.Current.Session("Agent") = "" AndAlso Not HttpContext.Current.Session("AgentType") Is Nothing AndAlso HttpContext.Current.Session("AgentType") = "2" Then
                    .ByPassPreReqCheck = "Y"
                Else
                    .ByPassPreReqCheck = "N"
                End If
            End With

            Dim basket As New Talent.Common.TalentBasket
            basket.DeAmendTicketingItems = deATI
            basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            basket.Settings.OriginatingSource = TEBUtilities.GetOriginatingSource(HttpContext.Current.Session.Item("Agent"))
            basket.Settings.PerformWatchListCheck = moduleDefaults.PerformAgentWatchListCheck
            talDataObjects.Settings = TEBUtilities.GetSettingsObject()
            basket.FulfilmentFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(basket.Settings.BusinessUnit)
            basket.CardTypeFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(basket.Settings.BusinessUnit)
            Dim err As New Talent.Common.ErrorObj
            err = basket.AmendTicketingItemsReturnBasket
            returnErrorCode = CheckResponseForError(basket.ResultDataSet, err)

            If returnErrorCode.Length > 0 Then
                returnError = True
            End If
        Else
            returnError = True
            returnErrorCode = "PARAMERR"
            Talent.eCommerce.Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "B_UARFB_" & Me._missingParamErrorCode, "Missing Parameter in TicketingGateway - Basket_UpdateAndRemoveFromBasket", "Error")
        End If
        If returnError Then
            HttpContext.Current.Session("TalentErrorCode") = returnErrorCode
            HttpContext.Current.Session("TicketingGatewayError") = returnErrorCode
            _redirectUrl = "~/PagesPublic/Basket/Basket.aspx"
        Else
            ' Forward to same page to remove items
            _redirectUrl = "~/Redirect/TicketingGateway.aspx?page=Basket.aspx&function=RemoveFromBasket&product=" & removeProduct & "&seat=" & removeSeat
        End If
        HttpContext.Current.Response.Redirect(_redirectUrl)
    End Sub

    Public Sub Login_ClearAndAdd(ByRef _redirectUrl As String, ByRef _redirectToProductLinkingPage As Boolean)
        Dim returnError As Boolean = False
        Dim returnErrorCode As String = String.Empty
        Dim missingParam As Boolean = False
        Dim exitLoop As Boolean = False
        Dim count As Integer = 1
        Dim returnURL As String = HttpContext.Current.Request("returnURL")
        Dim talDataObjects As New TalentDataObjects

        If String.IsNullOrEmpty(returnURL) Then
            missingParam = True
        Else
            Try
                If HttpContext.Current.Session("ClearAndAdd") Is Nothing Then
                    missingParam = ExtractQueryStringValues()
                Else
                    If String.IsNullOrEmpty(HttpContext.Current.Session("ClearAndAdd")) Then
                        missingParam = ExtractQueryStringValues()
                    Else
                        Dim sessionClearAndAdd As String = HttpContext.Current.Session("ClearAndAdd")
                        HttpContext.Current.Session.Remove("ClearAndAdd")
                        missingParam = ExtractSessionValues(sessionClearAndAdd)
                    End If
                End If
            Catch ex As Exception
                missingParam = ExtractQueryStringValues()
            End Try
        End If

        If Not missingParam Then
            Dim deATI As New Talent.Common.DEAmendTicketingItems
            With deATI
                .SessionID = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
                .CustomerNo = GlobalConstants.GENERIC_CUSTOMER_NUMBER
                .Src = GlobalConstants.SOURCE
                Dim i As Integer
                For i = 0 To _product.Count - 1
                    Dim deTBI As New Talent.Common.DETicketingBasketItem
                    deTBI.PriceBand = _concession(i)
                    deTBI.ProductCode = _product(i)
                    deTBI.BulkSalesID = _bulkSalesID(i)
                    deTBI.BulkSalesQuantity = _bulkSalesQuantity(i)
                    If _productType(i).Equals("M") Then
                        deTBI.Seat = _seat(i)
                    ElseIf _productType(i).Equals("T") Then
                        deTBI.Seat = _seat(i)
                    Else
                        If _isPackage(i).ToString.Equals("N") Then
                            If _seat(i).ToString().Length > 0 Then
                                deTBI.Seat = _seat(i).Substring(0, 3).trim & "/" & _seat(i).Substring(3, 4).trim & "/" & _seat(i).Substring(7, 4).trim & "/" & _seat(i).Substring(11, 4).trim
                                If _seat(i).Substring(15, 1).trim <> "" Then
                                    deTBI.Seat = deTBI.Seat.Trim & "/" & _seat(i).Substring(15, 1).trim
                                End If
                            Else
                                deTBI.Seat = _seat(i)
                            End If
                        Else
                            deTBI.PackageID = TEBUtilities.CheckForDBNull_Decimal(_seat(i))
                        End If
                    End If
                    deTBI.AllocatedMemberNo = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID.PadLeft(12, "0")
                    deTBI.PurchaseMemberNo = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID.PadLeft(12, "0")
                    deTBI.PriceCode = _priceCode(i)
                    If _priceCodeOverridden(i).ToString.Trim = "" Then
                        deTBI.PriceCodeOverridden = _priceCode(i)
                    Else
                        deTBI.PriceCodeOverridden = _priceCodeOverridden(i)
                    End If
                    deTBI.FulfilmentMethod = _fulfilmentMethod(i)
                    deATI.CollAmendItems.Add(deTBI)
                Next
            End With

            Dim basket As New Talent.Common.TalentBasket
            basket.DeAmendTicketingItems = deATI
            basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            basket.Settings.OriginatingSource = TEBUtilities.GetOriginatingSource(HttpContext.Current.Session.Item("Agent"))
            talDataObjects.Settings = TEBUtilities.GetSettingsObject()
            basket.FulfilmentFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(basket.Settings.BusinessUnit)
            basket.CardTypeFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(basket.Settings.BusinessUnit)
            Dim err As New Talent.Common.ErrorObj
            err = basket.AmendTicketingItemsReturnBasket
            returnErrorCode = CheckResponseForError(basket.ResultDataSet, err)

            If returnErrorCode.Length > 0 Then returnError = True
        Else
            returnError = True
            returnErrorCode = "PARAMERR"
            Talent.eCommerce.Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "LI_CLAA_" & Me._missingParamErrorCode, "Missing Parameter in TicketingGateway - Login_ClearAndAdd", "Error")
        End If
        If returnError Then
            HttpContext.Current.Session("TicketingGatewayError") = returnErrorCode
            HttpContext.Current.Session("TalentErrorCode") = returnErrorCode
        End If
        If HttpContext.Current.Session("ReservedSeatAvailableToBuy") Is Nothing Then
            _redirectUrl = returnURL
            _redirectToProductLinkingPage = True
        Else
            If String.IsNullOrEmpty(HttpContext.Current.Session("ReservedSeatAvailableToBuy")) Then
                _redirectUrl = returnURL
                _redirectToProductLinkingPage = True
            Else
                If CStr(HttpContext.Current.Session("ReservedSeatAvailableToBuy")) = "False" Then
                    HttpContext.Current.Session.Remove("ReservedSeatAvailableToBuy")
                    _redirectUrl = returnURL
                    _redirectToProductLinkingPage = True
                Else
                    returnURL = HttpContext.Current.Session("ReservedSeatAvailableToBuy")
                    HttpContext.Current.Session.Remove("ReservedSeatAvailableToBuy")
                    _redirectUrl = returnURL
                    _redirectToProductLinkingPage = False
                End If
            End If
        End If
    End Sub

    Private Function IsItPackageItem(ByVal indexID As Integer) As Boolean
        Dim isPackage As Boolean = False
        If _productType(indexID).Equals("P") Then
            If _isPackage IsNot Nothing AndAlso _isPackage(indexID) = "Y" Then
                isPackage = True
            End If
        End If
        Return isPackage
    End Function

    Private Sub DeleteBasketFees()
        Dim tDataObjects As New Talent.Common.TalentDataObjects()
        tDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        tDataObjects.BasketSettings.TblBasketFees.DeleteAll(CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)
    End Sub

    ''' <summary>
    ''' Override the basket restriction error codes and return an updated basket
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Basket_OverrideRestriction()
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults
        Dim returnError As Boolean = False
        Dim returnErrorCode As String = String.Empty
        Dim missingParam As Boolean = False
        Dim exitLoop As Boolean = False
        Dim count As Integer = 1
        Dim agentProfile As New Talent.eCommerce.Agent

        If String.IsNullOrEmpty(HttpContext.Current.Session("RedirectUpdateBasket")) Then
            missingParam = True
        Else
            If Not String.IsNullOrEmpty(HttpContext.Current.Request.QueryString("ProductCodeInError")) AndAlso
                Not String.IsNullOrEmpty(HttpContext.Current.Request.QueryString("StandCodeInError")) AndAlso
                Not String.IsNullOrEmpty(HttpContext.Current.Request.QueryString("ErrorCodeToOverride")) Then
                Dim sessionRedirectUpdateBasket As String = HttpContext.Current.Session("RedirectUpdateBasket")
                HttpContext.Current.Session.Remove("RedirectUpdateBasket")
                missingParam = ExtractSessionValues(sessionRedirectUpdateBasket, True)
            Else
                missingParam = True
            End If
        End If

        If Not missingParam Then
            Dim deATI As New Talent.Common.DEAmendTicketingItems
            With deATI
                .CustomerSelection = _customerSelection
                .SessionID = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
                If Not CType(HttpContext.Current.Profile, TalentProfile).IsAnonymous Then
                    .CustomerNo = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID.PadLeft(12, "0")
                Else
                    .CustomerNo = GlobalConstants.GENERIC_CUSTOMER_NUMBER
                End If
                .Src = GlobalConstants.SOURCE
                .ProductCodeInError = HttpContext.Current.Request.QueryString("ProductCodeInError")
                .StandCodeInError = HttpContext.Current.Request.QueryString("StandCodeInError")
                .OverrideBasketErrorCode = HttpContext.Current.Request.QueryString("ErrorCodeToOverride")

                Dim i As Integer
                For i = 0 To _product.Count - 1
                    Dim deTBI As New Talent.Common.DETicketingBasketItem
                    deTBI.BulkSalesID = _bulkSalesID(i)
                    deTBI.BulkSalesQuantity = _bulkSalesQuantity(i)
                    deTBI.PriceBand = _concession(i)
                    deTBI.ProductCode = _product(i)
                    If _productType(i).Equals("M") Then
                        deTBI.Seat = _seat(i)
                    ElseIf _productType(i).Equals("T") Then
                        deTBI.Seat = _seat(i)
                    ElseIf IsItPackageItem(i) Then
                        deTBI.PackageID = _seat(i)
                    Else
                        If _seat(i).length > 0 Then
                            deTBI.Seat = _seat(i).Substring(0, 3).trim & "/" & _seat(i).Substring(3, 4).trim & "/" & _seat(i).Substring(7, 4).trim & "/" & _seat(i).Substring(11, 4).trim
                            If _seat(i).ToString().Length > 15 Then
                                If _seat(i).Substring(15, 1).trim <> String.Empty Then
                                    deTBI.Seat = deTBI.Seat.Trim & "/" & _seat(i).Substring(15, 1).trim
                                End If
                            End If
                        Else
                            deTBI.Seat = _seat(i)
                        End If
                    End If
                    deTBI.AllocatedMemberNo = _customer(i).padleft(12, GlobalConstants.LEADING_ZEROS)
                    deTBI.PurchaseMemberNo = .CustomerNo.PadLeft(12, GlobalConstants.LEADING_ZEROS)
                    deTBI.PriceCode = _priceCode(i)
                    deTBI.FulfilmentMethod = _fulfilmentMethod(i)
                    deTBI.PriceCode = _priceCode(i)
                    If _priceCodeOverridden(i).ToString.Trim = String.Empty Then
                        deTBI.PriceCodeOverridden = _priceCode(i)
                    Else
                        deTBI.PriceCodeOverridden = _priceCodeOverridden(i)
                    End If
                    deTBI.AllocatedSeat.FormattedSeat = _allocatedSeat(i)
                    deATI.CollAmendItems.Add(deTBI)
                Next
                If agentProfile.IsAgent AndAlso agentProfile.Name.Length > 0 AndAlso agentProfile.Type = "2" Then
                    .ByPassPreReqCheck = "Y"
                Else
                    .ByPassPreReqCheck = "N"
                End If
            End With

            If _customerSelection = "Y" Then
                Dim customer As New TalentCustomer
                Dim deCustV11 As New DECustomerV11
                Dim deCustV1 As New DECustomer
                deCustV11.DECustomersV1.Add(deCustV1)
                With customer
                    .DeV11 = deCustV11
                End With
                deCustV1.CustomerNumber = deATI.CustomerNo
                deCustV1.IncludeBoxOfficeLinks = agentProfile.IsAgent
                customer.CustomerAssociationsClearSession()
            End If

            Dim basket As New Talent.Common.TalentBasket
            basket.DeAmendTicketingItems = deATI
            basket.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            basket.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            basket.Settings.PerformWatchListCheck = moduleDefaults.PerformAgentWatchListCheck
            basket.Settings.OriginatingSource = TEBUtilities.GetOriginatingSource(agentProfile.Name)
            Dim talDataObjects As New TalentDataObjects
            talDataObjects.Settings = TEBUtilities.GetSettingsObject()
            basket.FulfilmentFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(talDataObjects.Settings.BusinessUnit)
            basket.CardTypeFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(talDataObjects.Settings.BusinessUnit)
            Dim err As New Talent.Common.ErrorObj
            err = basket.AmendTicketingItemsReturnBasket

            returnErrorCode = CheckResponseForError(basket.ResultDataSet, err)
            If returnErrorCode.Length > 0 Then
                returnError = True
            End If
        Else
            returnError = True
            returnErrorCode = "PARAMERR"
            Talent.eCommerce.Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "B_OR_" & _missingParamErrorCode, "Missing Parameter in TicketingGateway - Basket_OverrideRestriction", "Error")
        End If

        If returnError Then
            HttpContext.Current.Session("TicketingGatewayError") = returnErrorCode
            HttpContext.Current.Session("TalentErrorCode") = returnErrorCode
        End If
        HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
    End Sub

#End Region

#Region "Checkout Functions"

    ''' <summary>
    ''' The main checkout payment details routine
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CheckoutPayment(Optional ByVal UseASavedCard As Boolean = False, Optional ByRef redirectUrl As String = "", Optional ByRef paymentSuccess As Boolean = False, Optional ByVal amountPartPayment As Decimal = 0)
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults
        Dim paymentType As String = Checkout.RetrievePaymentItemFromSession("PaymentType", GlobalConstants.CHECKOUTASPXSTAGE)
        Dim performCheckoutPayment As Boolean = True
        Select Case moduleDefaults.PaymentGatewayExternal
            Case Is = GlobalConstants.ECENTRICGATEWAY
                If paymentType = GlobalConstants.CCPAYMENTTYPE OrElse paymentType = GlobalConstants.CSPAYMENTTYPE Then
                    performCheckoutPayment = False
                    CheckoutECentric(paymentSuccess)
                End If
            Case Else
                performCheckoutPayment = True
        End Select

        If performCheckoutPayment Then


            ' Send the retail information to the ticketing database when completing the sale
            If amountPartPayment = 0 Then
                Dim errorRedirect As Boolean = True
                If paymentType = GlobalConstants.CHIPANDPINPAYMENTTYPE OrElse paymentType = GlobalConstants.POINTOFSALEPAYMENTTYPE Then
                    errorRedirect = False
                End If
                If Not String.IsNullOrWhiteSpace(SendRetailToTicketing(False, errorRedirect)) Then
                    Exit Sub
                End If
            End If


            Dim talentLogging As New TalentLogging
            Dim referrerUrl As String = String.Empty
            Try
                referrerUrl = HttpContext.Current.Request.UrlReferrer.AbsoluteUri
            Catch ex As Exception
                referrerUrl = ex.Message
            End Try

            talentLogging.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            Dim username As String = ""
            If Not HttpContext.Current.Profile.IsAnonymous Then
                username = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID
            Else
                username = HttpContext.Current.Profile.UserName
            End If
            talentLogging.GeneralLog(ProfileHelper.GetPageName, "CheckoutPaymentDetails_Payment", "Customer: " & username & ", Referrer URL: " & referrerUrl, "3dSecureLogging")

            Dim payment As New TalentPayment
            Dim returnErrorCode As String = String.Empty
            Dim returnError As Boolean = False
            Dim wfrPage As New Talent.Common.WebFormResource
            With wfrPage
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = KEYCODE
                .PageCode = PAGECODE
            End With


            payment.PayDetails = Checkout.RetrievePaymentDetailsFromSession()
            payment.De = payment.PayDetails.Item(GlobalConstants.CHECKOUTASPXSTAGE)
            If UseASavedCard Then payment.De.SaveMyCardMode = GlobalConstants.SAVECARDUSEMODE
            If payment.De.CardHolderName Is Nothing Then payment.De.CardHolderName = String.Empty
            If payment.De.Installments Is Nothing Then payment.De.Installments = String.Empty

            'set values from Retail Logic form post
            payment.De.ThreeDSecureMode = moduleDefaults.Payment3DSecureProvider
            payment.De.ThreeDSecureECI = Checkout.Retrieve3dDetails("eci")
            payment.De.ThreeDSecureCAVV = Checkout.Retrieve3dDetails("cavv")
            If moduleDefaults.Payment3DSecureProvider = "WINBANK" AndAlso (Not TEBUtilities.IsAgent()) Then
                payment.De.ThreeDSecureTransactionId = Checkout.Retrieve3dDetails("talent3dSecureTransactionID")
            Else
                payment.De.ThreeDSecureTransactionId = Checkout.Retrieve3dDetails("XID")
            End If
            payment.De.ThreeDSecureStatus = Checkout.Retrieve3dDetails("authenticationStatus")
            payment.De.ThreeDSecureCardScheme = Checkout.Retrieve3dDetails("sID")
            payment.De.ThreeDSecureATSData = Checkout.Retrieve3dDetails("atsData")
            payment.De.ThreeDSecureAuthenticationStatus = Checkout.Retrieve3dDetails("authenticationStatus")
            payment.De.ThreeDSecureXid = Checkout.Retrieve3dDetails("XID")
            payment.De.ThreeDSecureEnrolled = Checkout.Retrieve3dDetails("enrolled")
            payment.De.ThreeDSecurePAResStatus = Checkout.Retrieve3dDetails("PAResStatus")
            payment.De.ThreeDSecureSignatureVerification = Checkout.Retrieve3dDetails("signatureVerification")
            payment.De.AccountId = CType(HttpContext.Current.Profile, TalentProfile).Basket.PAYMENT_ACCOUNT_ID

            If Not CType(HttpContext.Current.Profile, TalentProfile).User.Details Is Nothing Then
                payment.De.CustomerNumber = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID
            End If

            Dim merchandiseAmount As Decimal = 0
            If CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType <> GlobalConstants.TICKETINGBASKETCONTENTTYPE Then
                merchandiseAmount = Checkout.MerchandiseTotalFromOrderHeader
                payment.De.Amount = Format(merchandiseAmount, "0.00")
                Checkout.UpdateOrderStatus("PAYMENT ATTEMPTED", "Card/Account ending: " & Checkout.RetrieveAccountEnd() & ". Attempt Payment.")
            End If
            If HttpContext.Current.Session("customerPresent") IsNot Nothing AndAlso HttpContext.Current.Session("customerPresent") = True Then
                payment.De.CustomerPresent = True
            Else
                payment.De.CustomerPresent = False
            End If
            If Not HttpContext.Current.Session("AutoEnrolPPS") Is Nothing AndAlso Boolean.Parse(HttpContext.Current.Session("AutoEnrolPPS")) Then
                payment.De.AutoEnrol = True
            End If
            If Not HttpContext.Current.Session("GiftAidSelected") Is Nothing AndAlso Boolean.Parse(HttpContext.Current.Session("GiftAidSelected")) Then
                payment.De.GiftAid = True
            End If
            If Not HttpContext.Current.Session("MarketingCampaign") Is Nothing Then payment.De.MarketingCampaign = HttpContext.Current.Session("MarketingCampaign")
            If Not HttpContext.Current.Session("CAMPAIGN_CODE") Is Nothing Then payment.De.SessionCampaignCode = HttpContext.Current.Session("CAMPAIGN_CODE")
            If HttpContext.Current.Session("AllowManualIntervention") IsNot Nothing AndAlso Boolean.Parse(HttpContext.Current.Session("AllowManualIntervention")) Then payment.De.AllowManualIntervention = HttpContext.Current.Session("AllowManualIntervention")
            payment.De.BasketContentType = CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType
            payment.De.BasketPaymentFeesEntityList = TEBUtilities.GetBasketPaymentFeesEntityList(CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)
            payment.De.FeesCount = payment.De.BasketPaymentFeesEntityList.Count
            'pass in the cat mode also 
            payment.De.CATMode = CType(HttpContext.Current.Profile, TalentProfile).Basket.CAT_MODE
            payment.Settings = TEBUtilities.GetSettingsObject()
            payment.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            payment.Settings.StoredProcedureGroup = TEBUtilities.GetStoredProcedureGroup
            payment.Settings.Cacheing = CType(wfrPage.Attribute("PaymentCacheing"), Boolean)
            payment.Settings.CacheTimeMinutes = CType(wfrPage.Attribute("PaymentCacheTimeMinutes"), Integer)
            payment.Settings.CacheDependencyPath = moduleDefaults.CacheDependencyPath
            If TEBUtilities.IsAgent() Then
                payment.Settings.OriginatingSource = Convert.ToString(HttpContext.Current.Session.Item("Agent"))
                payment.Settings.AgentType = Convert.ToString(HttpContext.Current.Session.Item("AgentType"))
            End If

            ' Store delivery (address) details against the payment object, then clear the session so that it is not used in the next session and always reset back from the checkout page
            If HttpContext.Current.Session("DeliveryDetails") IsNot Nothing Then
                payment.DeDelDetails = HttpContext.Current.Session("DeliveryDetails")
                If amountPartPayment = 0 Then HttpContext.Current.Session("DeliveryDetails") = Nothing
            End If

            'Remove any part payments from session
            payment.RetrievePartPaymentsClearSession()

            'Take Payment
            Dim err As New ErrorObj
            payment.De.PartPaymentApplyTypeFlag = TEBUtilities.GetPartPaymentFlag()
            If amountPartPayment > 0 Then
                If String.IsNullOrWhiteSpace(payment.De.Amount) Then
                    payment.De.RetailAmount = "0.00"
                Else
                    payment.De.RetailAmount = payment.De.Amount
                End If
                payment.De.Amount = amountPartPayment

                payment.De.FeesPartPaymentEntity.FeeCode = GlobalConstants.BKFEE
                payment.De.FeesPartPaymentEntity.FeeCategory = GlobalConstants.FEECATEGORY_BOOKING
                payment.De.FeesPartPaymentEntity.CardType = CType(HttpContext.Current.Profile, TalentProfile).Basket.CARD_TYPE_CODE
                TEBUtilities.GetBookingFeeValuesForCurrentPartPayment(amountPartPayment, payment.De.FeesPartPaymentEntity.FeeValue, payment.De.FeesPartPaymentEntity.FeeValueActual)
                If amountPartPayment <= payment.De.FeesPartPaymentEntity.FeeValue Then
                    returnError = True
                    redirectUrl = "~/PagesLogin/Checkout/checkout.aspx"
                    HttpContext.Current.Session("TicketingGatewayError") = "TEB-PPALFV"
                    HttpContext.Current.Session("TalentErrorCode") = "TEB-PPALFV"
                Else
                    err = payment.TakePartPayment
                    returnErrorCode = CheckResponseForError(payment.ResultDataSet, err)
                    If returnErrorCode.Length > 0 Then
                        returnError = True
                        HttpContext.Current.Session("TicketingGatewayError") = returnErrorCode
                        HttpContext.Current.Session("TalentErrorCode") = returnErrorCode
                    End If
                    If Not returnError Then paymentSuccess = True
                End If

            Else
                err = payment.TakePayment
                'This is set and checked on the checkout page, it is not reset on the checkout page, hence it must be done here
                'It is reset regardless of whether payment is successful or not.
                HttpContext.Current.Session("TakeFinalPayment") = Nothing
                returnErrorCode = CheckResponseForError(payment.ResultDataSet, err)
                talentLogging.GeneralLog(ProfileHelper.GetPageName, "CheckoutPaymentDetails_Payment - Response From TALENT", "Customer: " & username & ", TALENT RESPONSE (return code): " & returnErrorCode, "3dSecureLogging")
                If returnErrorCode.Length > 0 Then returnError = True

                'Was payment taken successfully
                If Not returnError Then
                    'Finalise the order
                    Dim webOrderID As String = String.Empty
                    If payment.De.PaymentType <> GlobalConstants.CFPAYMENTTYPE AndAlso merchandiseAmount = 0 Then Checkout.RemovePaymentDetailsFromSession()
                    If Checkout.FinaliseOrder(payment.De.PaymentType, webOrderID) Then
                        Dim OrderLevelFulfilmentMethod As String = payment.OrderLevelFulfilmentMethod
                        'Checkout Order Confirmation now does the same as the finalise function so there is no need to call it here
                        Dim paymentRef As String = GetPaymentRefFromPayment(payment.ResultDataSet)
                        If String.IsNullOrEmpty(paymentRef) Then
                            If webOrderID.Length = 0 Then
                                HttpContext.Current.Session("TalentErrorCode") = "BAA"
                                HttpContext.Current.Session("TicketingGatewayError") = "BAA"
                            Else
                                paymentRef = webOrderID
                            End If
                        End If
                        Dim order As New Order()
                        order.ProcessMerchandiseInBackend(True, False, String.Empty, False, paymentRef)

                        Dim paymentAmount As String = GetPaymentAmountFromPayment(payment.ResultDataSet, merchandiseAmount)
                        If Not moduleDefaults.NotificationsOnConfirmPage Then
                            If moduleDefaults.ConfirmationEmail AndAlso (Not String.IsNullOrWhiteSpace(paymentRef)) AndAlso payment.SendEmail Then
                                Dim Order_Email As New Talent.eCommerce.Order_Email
                                Dim fileAttachments As List(Of String) = Nothing
                                If String.IsNullOrWhiteSpace(payment.De.CATMode) Then
                                    fileAttachments = GetHospitalityPDFAttachmentList()
                                    Order_Email.SendTicketingConfirmationEmail(paymentRef, Nothing, Nothing, fileAttachments)
                                ElseIf (Not String.IsNullOrWhiteSpace(payment.De.CATMode)) AndAlso (Not HttpContext.Current.Profile.IsAnonymous) Then
                                    If payment.De.CATMode = GlobalConstants.CATMODE_AMEND Then
                                        fileAttachments = GetHospitalityPDFAttachmentList()
                                    End If
                                    Order_Email.SendCATConfirmationEmail(paymentRef, payment.De.CATMode, Nothing, fileAttachments)
                                End If
                            End If
                        Else
                            If Not payment.SendEmail Then
                                HttpContext.Current.Session("DontSendEmail") = True
                            End If
                        End If
                        HttpContext.Current.Session("CAMPAIGN_CODE") = Nothing
                        redirectUrl = "~/PagesLogin/Checkout/CheckoutOrderConfirmation.aspx?PaymentRef=" & paymentRef.Trim & "&paymentType=" & payment.De.PaymentType.Trim & "&paymentAmount=" & paymentAmount
                        If Trim(OrderLevelFulfilmentMethod) <> String.Empty Then
                            redirectUrl = Trim(redirectUrl) & "&OrderLevelFulfilmentMethod=" & OrderLevelFulfilmentMethod.ToString
                        End If
                        If payment.DDScheduleAdjusted Then
                            redirectUrl = Trim(redirectUrl) & "&DDScheduleAdjusted=" & payment.DDScheduleAdjusted.ToString
                        End If
                    Else
                        Checkout.UpdateOrderStatus("ORDER FAILED")
                        HttpContext.Current.Session("TalentErrorCode") = "BAB"
                        HttpContext.Current.Session("TicketingGatewayError") = "BAB"
                        DeleteBasketDetail()
                        redirectUrl = "~/PagesPublic/Basket/basket.aspx"
                    End If
                Else
                    HttpContext.Current.Session("TicketingGatewayError") = returnErrorCode
                    HttpContext.Current.Session("TalentErrorCode") = returnErrorCode
                    Select Case returnErrorCode
                        Case Is = "W2", "**"
                            ' Something fatal went wrong on a previous attempt for this basket ID, so
                            ' clear the basket, which will regenerate basket ID, redirect to basket.
                            Checkout.UpdateOrderStatus("PROCESS FAILED", "Fatal Error Code " & returnErrorCode & ". Payment may or may not have been taken")
                            DeleteBasketHeader()
                            '
                            ' if they are paying by credit Finance then change the error message that the user will see
                            '
                            If returnErrorCode = "W2" And payment.De.PaymentType = "CF" Then
                                HttpContext.Current.Session("TicketingGatewayError") = "W2CF"
                            End If
                            redirectUrl = "~/PagesPublic/Basket/basket.aspx"

                        Case "WF", "W7"
                            ' WF indicates no basket records found. This (normally) is caused only by Webmon.
                            ' In this case clear front-end basket.
                            DeleteBasketDetail()
                            If returnErrorCode = "W7" Then
                                HttpContext.Current.Session("TicketingGatewayError") = "W7"
                            End If

                            Checkout.UpdateOrderStatus("PAYMENT REJECTED", "Error Code " & returnErrorCode)
                            redirectUrl = "~/PagesPublic/Basket/basket.aspx"

                        Case "NS", "99", "MF", "W1", "WS", "WT", "WM", "WR", "NF"
                            ' Any other type of error - let them amend the basket themselves, or 
                            ' have another go.
                            Checkout.UpdateOrderStatus("PAYMENT REJECTED", "Error Code " & returnErrorCode)
                            RefreshBasketContent()
                            redirectUrl = "~/PagesPublic/Basket/basket.aspx"

                        Case Is = "IV", "IX", "IY", "IW"
                            ' Credit Card error, redirect to payment page to try again
                            HttpContext.Current.Session("CardNumber") = payment.De.CardNumber
                            HttpContext.Current.Session("ExpiryDate") = payment.De.ExpiryDate
                            HttpContext.Current.Session("StartDate") = payment.De.StartDate
                            HttpContext.Current.Session("IssueNumber") = payment.De.IssueNumber
                            HttpContext.Current.Session("CV2Number") = payment.De.CV2Number
                            Checkout.UpdateOrderStatus("PAYMENT REJECTED", "Error Code " & returnErrorCode)
                            redirectUrl = "~/PagesLogin/Checkout/checkout.aspx"

                        Case Is = "DM", "DA", "DN", "DC", "DJ"
                            HttpContext.Current.Session("TalentErrorCode") = Nothing
                            Checkout.UpdateOrderStatus("PAYMENT REJECTED", "Error Code " & returnErrorCode)
                            redirectUrl = "~/PagesLogin/Checkout/checkout.aspx"

                        Case Is = "CT", "CN", "CB", "CE", "CM", "CR", "CJ", "CW", "CP", "CO"
                            'Credit Finance Return error code
                            Checkout.UpdateOrderStatus("PAYMENT REJECTED", "Error Code " & returnErrorCode)
                            HttpContext.Current.Session("TalentErrorCode") = Nothing
                            redirectUrl = "~/PagesLogin/Checkout/checkout.aspx"

                        Case Is = "C1", "C2", "C3", "C4", "C5", "C6"
                            'Chip and Pin return error codes
                            Checkout.UpdateOrderStatus("PAYMENT REJECTED", "Error Code " & returnErrorCode)
                            redirectUrl = "~/PagesLogin/Checkout/checkout.aspx"

                        Case Is = "P1", "P2", "P3", "P4", "P5", "P6"
                            Checkout.UpdateOrderStatus("PAYMENT REJECTED", "Error Code " & returnErrorCode)
                            ' Point-of-Sale has been Cancelled
                            If returnErrorCode = "P1" Then
                                Checkout.RemovePaymentDetailsFromSession()
                                RefreshBasketContent()
                                HttpContext.Current.Session("TalentErrorCode") = Nothing
                                HttpContext.Current.Session("TicketingGatewayError") = Nothing
                                FormsAuthentication.SignOut()
                                redirectUrl = TEBUtilities.GetSiteHomePage()
                            End If

                            ' Point-of-Sale has requested Amend Basket function
                            If returnErrorCode = "P2" Then
                                HttpContext.Current.Session("TalentErrorCode") = Nothing
                                HttpContext.Current.Session("TicketingGatewayError") = Nothing
                                redirectUrl = "~/PagesPublic/Basket/basket.aspx"
                            End If

                            ' Point-of-Sale errors:
                            '   P3 = PoS terminal couldn't be contacted in IP and port specified
                            '   P4 = Not all data could be sent to terminal successfully
                            '   P5 = Basket XML generated as blanks
                            '   P6 = Invalid TalentCommand received from PoS terminal
                            If returnErrorCode = "P3" Or returnErrorCode = "P4" Or returnErrorCode = "P5" Or returnErrorCode = "P6" Then
                                redirectUrl = "~/PagesLogin/Checkout/checkout.aspx"
                            End If

                        Case Else
                            ' Communication error, back to payment page
                            HttpContext.Current.Session("CardNumber") = payment.De.CardNumber
                            HttpContext.Current.Session("ExpiryDate") = payment.De.ExpiryDate
                            HttpContext.Current.Session("StartDate") = payment.De.StartDate
                            HttpContext.Current.Session("IssueNumber") = payment.De.IssueNumber
                            HttpContext.Current.Session("CV2Number") = payment.De.CV2Number
                            Checkout.UpdateOrderStatus("PAYMENT REJECTED", "Error Code " & returnErrorCode)
                            RefreshBasketContent()
                            redirectUrl = "~/PagesPublic/Basket/basket.aspx"

                    End Select
                End If

                Select Case payment.De.PaymentType
                    Case Is = GlobalConstants.CHIPANDPINPAYMENTTYPE
                        'don't perform the redirect here
                    Case Is = GlobalConstants.POINTOFSALEPAYMENTTYPE
                        If returnErrorCode = "P1" Or returnErrorCode = "P2" Then
                            HttpContext.Current.Response.Redirect(redirectUrl)
                        Else
                            'don't perform the redirect here
                        End If
                    Case Else
                        HttpContext.Current.Response.Redirect(redirectUrl)
                End Select
            End If
        End If
    End Sub

    Private Function SendRetailToTicketing(ByVal paymentTaken As Boolean, ByVal redirectValid As Boolean) As String

        Dim errorCode As String = String.Empty

        'Create the retail products on the iseries
        Dim basketType As String = CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType
        If Talent.eCommerce.Utilities.IsTicketingDBforRetailOrders AndAlso (basketType = "C" Or basketType = "M") Then
            Dim retailBasket As New Talent.Common.TalentBasket
            Dim retailErr As ErrorObj
            retailBasket.Settings = Talent.eCommerce.Utilities.GetSettingsObject
            retailErr = retailBasket.AddRetailToTicketingBasket(CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID.ToString, CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, paymentTaken)

            ' Handle the errors
            If retailErr.HasError OrElse retailBasket.ResultDataSet Is Nothing OrElse retailBasket.ResultDataSet.Tables.Count = 0 OrElse Not String.IsNullOrWhiteSpace(retailBasket.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred")) Then

                If retailErr.HasError OrElse retailBasket.ResultDataSet Is Nothing OrElse retailBasket.ResultDataSet.Tables.Count = 0 Then
                    If paymentTaken Then
                        errorCode = "**"
                    Else
                        errorCode = "W9"
                    End If
                Else
                    errorCode = retailBasket.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode")
                End If

                HttpContext.Current.Session("TicketingGatewayError") = errorCode
                HttpContext.Current.Session("TalentErrorCode") = errorCode
                If redirectValid Then HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/basket.aspx")
            End If
        End If

        Return errorCode
    End Function

    ''' <summary>
    ''' Use the eCentric Payment Gateway to take the payment from the card
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CheckoutECentric(ByRef success As Boolean)
        Const ECENTRIC_PAYMENT_LOG As String = "ECentricPaymentLog"
        Dim talentLogging As New TalentLogging
        Dim client As PaymentGatewayServiceClient = Nothing
        Dim responseDetails As New ResponseDetail
        Dim talProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
        Dim transactionId As String = Now.Ticks.ToString()
        Dim amount, authAmount As Long
        Dim customerDetails As String = "Customer: " & talProfile.UserName
        Dim basketDetails As String = "Basket: " & talProfile.Basket.Basket_Header_ID & " Value: " & talProfile.Basket.BasketSummary.TotalBasket & " Transaction ID: " & transactionId
        Dim updateiSeriesBasket As Boolean = (talProfile.Basket.BasketContentType <> GlobalConstants.MERCHANDISEBASKETCONTENTTYPE)
        HttpContext.Current.Session("ExternalGatewayPayType") = Checkout.RetrievePaymentItemFromSession("PaymentType", GlobalConstants.CHECKOUTASPXSTAGE)
        HttpContext.Current.Session("ExtPaymentReferenceNo") = transactionId
        HttpContext.Current.Session("ExtPaymentAmount") = talProfile.Basket.BasketSummary.TotalBasket
        talentLogging.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString

        talentLogging.GeneralLog(customerDetails, basketDetails, "Mark basket ready for payment", ECENTRIC_PAYMENT_LOG)
        If updateiSeriesBasket Then CheckoutExternal(False)
        talentLogging.GeneralLog(customerDetails, basketDetails, "Attempt payment", ECENTRIC_PAYMENT_LOG)

        Try
            populateECentricClientObject(client, talProfile, responseDetails, transactionId, amount, authAmount)
            If responseDetails.Code = GlobalConstants.ECENTRICSUCCESSCODE1 AndAlso amount = authAmount Then
                success = True
                talentLogging.GeneralLog(customerDetails, basketDetails, "Payment successful", ECENTRIC_PAYMENT_LOG)
            Else
                success = False
                'Check the response details code for any card declined error codes and set a card refused error message
                'Otherwise set a generic error message
                Select Case responseDetails.Code
                    Case Is = "05", "91", "51", "41", "87" : HttpContext.Current.Session("TicketingGatewayError") = "IY"
                    Case Else : HttpContext.Current.Session("TicketingGatewayError") = GlobalConstants.ECENTRICGENERICERROR
                End Select
                With responseDetails
                    talentLogging.GeneralLog(customerDetails, basketDetails, "Payment failed (1): Code:" & .Code & " Description:" & .Description _
                                             & " Client Message:" & .ClientMessage & " Source:" & .Source, ECENTRIC_PAYMENT_LOG)
                End With
            End If
        Catch ex As Exception
            success = False
            'There has been a conversion, comms, application error therefore payment has not been taken and a generic error must be shown
            If responseDetails IsNot Nothing AndAlso responseDetails.Code IsNot Nothing AndAlso responseDetails.Code.Length > 0 Then
                With responseDetails
                    talentLogging.GeneralLog(customerDetails, basketDetails,
                                "Payment failed (2): Code:" & .Code & " Description:" & .Description & " Client Message:" & .ClientMessage & " Source:" & .Source &
                                "Exception Details:" & ex.Message & " Exception Stack Trace:" & ex.StackTrace, ECENTRIC_PAYMENT_LOG)
                End With
            Else
                talentLogging.GeneralLog(customerDetails, basketDetails, "Payment failed (3): No response object to look at. Exception Details:" & ex.Message & " Exception Stack Trace:" & ex.StackTrace, ECENTRIC_PAYMENT_LOG)
            End If
            HttpContext.Current.Session("TicketingGatewayError") = GlobalConstants.ECENTRICGENERICERROR
        Finally
            If client IsNot Nothing Then client.Close()
            HttpContext.Current.Session("MD") = Nothing
            HttpContext.Current.Session("PARes") = Nothing
            HttpContext.Current.Session("AcsUrl") = Nothing
            HttpContext.Current.Session("3DSecureTransactionId") = Nothing
        End Try

        If success Then
            talentLogging.GeneralLog(customerDetails, basketDetails, "Mark basket as paid", ECENTRIC_PAYMENT_LOG)
            If updateiSeriesBasket Then CheckoutExternalSuccess(True, talProfile.Basket.Basket_Header_ID, True, String.Empty)
        Else
            talentLogging.GeneralLog(customerDetails, basketDetails, "Mark basket as failed (reset)", ECENTRIC_PAYMENT_LOG)
            If updateiSeriesBasket Then CheckoutExternalFailure(True, talProfile.Basket.Basket_Header_ID, False)
        End If
    End Sub

    ''' <summary>
    ''' Populate the eCentric payment gateway client object and call the payment gateway
    ''' </summary>
    ''' <param name="client">The payment gateway client object</param>
    ''' <param name="talProfile">The profile object</param>
    ''' <param name="responseDetails">The payment gateway response details object</param>
    ''' <param name="transactionId">The given transaction id</param>
    ''' <param name="amount">The amount that is being requested</param>
    ''' <param name="authAmount">The amount that has been authorised</param>
    ''' <remarks></remarks>
    Private Sub populateECentricClientObject(ByRef client As PaymentGatewayServiceClient, ByRef talProfile As TalentProfile, ByRef responseDetails As ResponseDetail,
                                                    ByRef transactionId As String, ByRef amount As Long, ByRef authAmount As Long)
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults
        client = New PaymentGatewayServiceClient()
        Dim msgHeader As New MessageHeader
        Dim transactionStatus As TransactionStatusType
        Dim returnString As String = String.Empty
        Dim priceListHeaderTableAdapter As New TalentBasketDatasetTableAdapters.tbl_price_list_headerTableAdapter
        Dim priceListHeaderTable As Data.DataTable = priceListHeaderTableAdapter.GetPriceListHeaderByPriceList(moduleDefaults.PriceList)
        Dim currenciesTableAdapter As New TalentApplicationVariablesTableAdapters.tbl_currencyTableAdapter
        Dim currenciesTable As Data.DataTable = currenciesTableAdapter.GetDataByCurrencyCode(priceListHeaderTable.Rows(0)("CURRENCY_CODE").ToString)
        Dim merchantId As String = moduleDefaults.PaymentDetails1
        Dim previousTransactionId As String = HttpContext.Current.Session("3DSecureTransactionId") 'When there is no session object, set 'Nothing' as the value as this is nullable
        Dim reconId As String = HttpContext.Current.Session.SessionID
        Dim currencyCode As String = String.Empty
        If currenciesTable.Rows.Count > 0 AndAlso TEBUtilities.CheckForDBNull_String(currenciesTable.Rows(0)("CURRENCY_CODE").ToString) <> String.Empty Then
            currencyCode = currenciesTable.Rows(0)("CURRENCY_CODE").ToString
        End If
        Dim orderNumber As String = talProfile.Basket.Basket_Header_ID
        Dim forename As String = talProfile.User.Details.Forename
        Dim surname As String = talProfile.User.Details.Surname
        Dim emailAddress As String = talProfile.User.Details.Email
        Dim mobilePhoneNum As String = talProfile.User.Details.Mobile_Number
        Dim homePhoneNum As String = talProfile.User.Details.Telephone_Number
        Dim workPhoneNum As String = talProfile.User.Details.Work_Number
        Dim pAResPayload As String = HttpContext.Current.Session("PARes") 'When there is no session object, set 'Nothing' as the value as this is nullable
        amount = talProfile.Basket.BasketSummary.TotalBasket.ToString("F2").Replace(".", String.Empty)

        Dim card As New BankCard
        card.CardAssociation = Checkout.RetrievePaymentItemFromSession("CardType", GlobalConstants.CHECKOUTASPXSTAGE)
        card.CardholderName = Checkout.RetrievePaymentItemFromSession("CardHolderName", GlobalConstants.CHECKOUTASPXSTAGE)
        card.CardNumber = Checkout.RetrievePaymentItemFromSession("CardNumber", GlobalConstants.CHECKOUTASPXSTAGE)
        card.ExpiryMonth = Checkout.RetrievePaymentItemFromSession("ExpiryDate", GlobalConstants.CHECKOUTASPXSTAGE).Substring(0, 2)
        card.ExpiryYear = Checkout.RetrievePaymentItemFromSession("ExpiryDate", GlobalConstants.CHECKOUTASPXSTAGE).Substring(2, 2)
        card.SecurityCode = Checkout.RetrievePaymentItemFromSession("CV2Number", GlobalConstants.CHECKOUTASPXSTAGE)
        card.ExpiryYearSpecified = True
        card.ExpiryMonthSpecified = True
        card.CardTypeSpecified = True

        Dim billingAddress As New Address
        Dim myAddress As TalentProfileAddress = ProfileHelper.ProfileAddressEnumerator(0, talProfile.User.Addresses)
        billingAddress.AddressLine1 = myAddress.Address_Line_1
        billingAddress.AddressLine2 = myAddress.Address_Line_2
        billingAddress.Neighbourhood = myAddress.Address_Line_3
        billingAddress.City = myAddress.Address_Line_4
        billingAddress.Region = myAddress.Address_Line_5
        billingAddress.PostCode = myAddress.Post_Code
        Dim tDataObjects As New Talent.Common.TalentDataObjects()
        tDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        billingAddress.CountryCode = tDataObjects.ProfileSettings.tblCountry.GetCountryCodeByDescription(myAddress.Country)
        Dim shippingAddress As Address = billingAddress

        returnString = client.Payment(msgHeader, merchantId, transactionId, previousTransactionId, Now, reconId, amount, currencyCode, 0, orderNumber,
                                      PaymentServiceType.CardNotPresent, card, card.CardType, forename, surname, billingAddress, shippingAddress,
                                      emailAddress, mobilePhoneNum, homePhoneNum, workPhoneNum, pAResPayload, transactionStatus, responseDetails, authAmount)
    End Sub

    ''' <summary>
    ''' External Checkout routine, marks the payment to pending until the external payment has been taken.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CheckoutExternal(ByVal canRedirect As Boolean)
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults
        Dim transferURL As String = String.Empty
        Dim returnErrorCode As String = String.Empty
        Dim err As New ErrorObj
        Dim payment As New Talent.Common.TalentPayment
        Dim dePayment As New Talent.Common.DEPayments

        With dePayment
            .SessionId = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
            .Source = GlobalConstants.SOURCE
            .PaymentType = HttpContext.Current.Session("ExternalGatewayPayType").ToString()
            .exPayment = "1" 'This flag instructs the back end not to take payment but set everything payment pending
        End With
        Dim wfrPage As New Talent.Common.WebFormResource
        With wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "TicketingGateway.aspx"
            .PageCode = "TicketingGateway.aspx"
        End With

        payment.PayDetails = Checkout.RetrievePaymentDetailsFromSession()
        payment.De = dePayment
        payment.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        payment.Settings.BusinessUnit = TalentCache.GetBusinessUnit
        payment.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
        payment.Settings.Cacheing = CType(wfrPage.Attribute("PaymentCacheing"), Boolean)
        payment.Settings.CacheTimeMinutes = CType(wfrPage.Attribute("PaymentCacheTimeMinutes"), Integer)
        payment.Settings.CacheDependencyPath = moduleDefaults.CacheDependencyPath
        payment.Settings.OriginatingSource = TEBUtilities.GetOriginatingSource(HttpContext.Current.Session.Item("Agent"))
        payment.De.BasketPaymentFeesEntityList = TEBUtilities.GetBasketPaymentFeesEntityList(CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)
        payment.De.FeesCount = payment.De.BasketPaymentFeesEntityList.Count
        payment.De.BasketContentType = CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType
        If Not CType(HttpContext.Current.Profile, TalentProfile).User.Details Is Nothing Then
            payment.De.CustomerNumber = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID
        End If

        If HttpContext.Current.Session("GiftAidSelected") IsNot Nothing AndAlso Boolean.Parse(HttpContext.Current.Session("GiftAidSelected")) Then payment.De.GiftAid = True
        err = payment.PaymentPending

        'Remove any part payments from session
        payment.RetrievePartPaymentsClearSession()

        returnErrorCode = CheckResponseForError(payment.ResultDataSet, err)
        If returnErrorCode.Length > 0 Then
            err.HasError = True
            HttpContext.Current.Session("TalentErrorCode") = returnErrorCode
            HttpContext.Current.Session("TicketingGatewayError") = returnErrorCode
        End If
        If err.HasError Then
            Select Case returnErrorCode
                Case Is = "W2", "**"
                    DeleteBasketHeader()
                Case "WF"
                    ' WF indicates no basket records found. This (normally) is caused only by Webmon.
                    ' In this case clear front-end basket.
                    DeleteBasketDetail()
                Case "NS", "99", "MF", "W1", "WS", "WT", "WM", "WR", "NF"
                    RefreshBasketContent()
            End Select
            HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/basket.aspx")
        Else
            If canRedirect Then
                transferURL = HttpContext.Current.Session("ExternalGatewayURL").ToString()
                HttpContext.Current.Response.Redirect(transferURL)
            End If
        End If
    End Sub


    ''' <summary>
    ''' External Checkout routine, marks the payment as success.
    ''' </summary>
    ''' <param name="canRedirect">if set to <c>true</c> [can redirect].</param>
    ''' <param name="basketHeaderId">The basket header id.</param>
    ''' <param name="canSendConfirmEmail">if set to <c>true</c> [can send confirm email].</param>
    ''' <param name="originatingSource">The originating source.</param>
    Public Sub CheckoutExternalSuccess(ByVal canRedirect As Boolean, ByVal basketHeaderId As String, ByVal canSendConfirmEmail As Boolean, ByVal originatingSource As String, Optional ByVal amountPartPayment As Decimal = 0)
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults
        Dim payment As TalentPayment = Nothing
        Dim err As New ErrorObj
        Dim returnErrorCode As String = String.Empty
        Dim ExtPayReference As String = HttpContext.Current.Session("ExtPaymentReferenceNo").ToString()
        Dim returnError As Boolean = False
        If amountPartPayment = 0 Then
            'todo how to handle this if error occured, payment is already taken
            'We need to send the retail information to ticketing before we complete
            If Not String.IsNullOrWhiteSpace(SendRetailToTicketing(True, True)) Then
                Exit Sub
            End If
        End If

        Dim wfrPage As New WebFormResource
        With wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "TicketingGateway.aspx"
            .PageCode = "TicketingGateway.aspx"
        End With

        payment = New TalentPayment
        Dim dePayment As New DEPayments
        payment.PayDetails = Checkout.RetrievePaymentDetailsFromSession()
        Dim accountEnd As String = String.Empty
        If HttpContext.Current.Session("ExternalGatewayPayType").ToString() = GlobalConstants.PAYMENTTYPE_VANGUARD Then
            dePayment = payment.PayDetails.Item(GlobalConstants.CHECKOUTASPXSTAGE)
        Else
            dePayment.CardNumber = ExtPayReference
        End If
        accountEnd = dePayment.CardNumber
        With dePayment
            If String.IsNullOrWhiteSpace(basketHeaderId) Then
                .SessionId = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
            Else
                .SessionId = basketHeaderId
            End If
            .Source = GlobalConstants.SOURCE
            .PaymentType = HttpContext.Current.Session("ExternalGatewayPayType").ToString()
            .exPayment = "2" 'This flag instructs the back end not to take payment but call completion routines
        End With
        payment.De = dePayment
        payment.De.BasketContentType = CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType
        'vanguard phase 2
        'If payment.De.Installments Is Nothing Then payment.De.Installments = String.Empty
        'payment.De.AccountId = CType(HttpContext.Current.Profile, TalentProfile).Basket.PAYMENT_ACCOUNT_ID

        If Not CType(HttpContext.Current.Profile, TalentProfile).User.Details Is Nothing Then
            payment.De.CustomerNumber = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID
        End If

        Dim merchandiseAmount As Decimal = 0
        If CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType <> GlobalConstants.TICKETINGBASKETCONTENTTYPE Then
            merchandiseAmount = Checkout.MerchandiseTotalFromOrderHeader
            payment.De.Amount = Format(merchandiseAmount, "0.00")
            Checkout.UpdateOrderStatus("PAYMENT ATTEMPTED", "Card/Account ending: " & accountEnd & ". Attempt Payment.")
        End If
        'vanguard phase 2
        'If HttpContext.Current.Session("customerPresent") IsNot Nothing AndAlso HttpContext.Current.Session("customerPresent") = True Then
        '    payment.De.CustomerPresent = True
        'Else
        '    payment.De.CustomerPresent = False
        'End If
        If Not HttpContext.Current.Session("AutoEnrolPPS") Is Nothing AndAlso Boolean.Parse(HttpContext.Current.Session("AutoEnrolPPS")) Then
            payment.De.AutoEnrol = True
        End If
        If Not HttpContext.Current.Session("GiftAidSelected") Is Nothing AndAlso Boolean.Parse(HttpContext.Current.Session("GiftAidSelected")) Then
            payment.De.GiftAid = True
        End If
        If Not HttpContext.Current.Session("MarketingCampaign") Is Nothing Then payment.De.MarketingCampaign = HttpContext.Current.Session("MarketingCampaign")
        If Not HttpContext.Current.Session("CAMPAIGN_CODE") Is Nothing Then payment.De.SessionCampaignCode = HttpContext.Current.Session("CAMPAIGN_CODE")
        If HttpContext.Current.Session("AllowManualIntervention") IsNot Nothing AndAlso Boolean.Parse(HttpContext.Current.Session("AllowManualIntervention")) Then payment.De.AllowManualIntervention = HttpContext.Current.Session("AllowManualIntervention")

        payment.De.BasketPaymentFeesEntityList = TEBUtilities.GetBasketPaymentFeesEntityList(CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)
        payment.De.FeesCount = payment.De.BasketPaymentFeesEntityList.Count
        'pass in the cat mode also 
        payment.De.CATMode = CType(HttpContext.Current.Profile, TalentProfile).Basket.CAT_MODE

        payment.Settings = TEBUtilities.GetSettingsObject()
        payment.Settings.BusinessUnit = TalentCache.GetBusinessUnit
        payment.Settings.StoredProcedureGroup = TEBUtilities.GetStoredProcedureGroup
        payment.Settings.Cacheing = CType(wfrPage.Attribute("PaymentCacheing"), Boolean)
        payment.Settings.CacheTimeMinutes = CType(wfrPage.Attribute("PaymentCacheTimeMinutes"), Integer)
        payment.Settings.CacheDependencyPath = moduleDefaults.CacheDependencyPath

        If String.IsNullOrWhiteSpace(originatingSource) Then
            If TEBUtilities.IsAgent() Then
                payment.Settings.OriginatingSource = Convert.ToString(HttpContext.Current.Session.Item("Agent"))
                payment.Settings.AgentType = Convert.ToString(HttpContext.Current.Session.Item("AgentType"))
            Else
                payment.Settings.OriginatingSource = TEBUtilities.GetOriginatingSource(Nothing)
            End If
        End If
        If Not CType(HttpContext.Current.Profile, TalentProfile).User.Details Is Nothing Then
            payment.De.CustomerNumber = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID
        End If
        ' Store delivery (address) details against the payment object, then clear the session so that it is not used in the next session and always reset back from the checkout page
        If HttpContext.Current.Session("DeliveryDetails") IsNot Nothing Then
            payment.DeDelDetails = HttpContext.Current.Session("DeliveryDetails")
            If amountPartPayment = 0 Then HttpContext.Current.Session("DeliveryDetails") = Nothing
        End If

        'Remove any part payments from session
        payment.RetrievePartPaymentsClearSession()

        If HttpContext.Current.Session("customerPresent") IsNot Nothing AndAlso HttpContext.Current.Session("customerPresent") = True Then
            payment.De.CustomerPresent = True
        Else
            payment.De.CustomerPresent = False
        End If

        If HttpContext.Current.Session("AutoEnrolPPS") IsNot Nothing AndAlso Boolean.Parse(HttpContext.Current.Session("AutoEnrolPPS")) Then payment.De.AutoEnrol = True
        If HttpContext.Current.Session("GiftAidSelected") IsNot Nothing AndAlso Boolean.Parse(HttpContext.Current.Session("GiftAidSelected")) Then payment.De.GiftAid = True
        If HttpContext.Current.Session("MarketingCampaign") IsNot Nothing Then payment.De.MarketingCampaign = HttpContext.Current.Session("MarketingCampaign")
        If HttpContext.Current.Session("CAMPAIGN_CODE") IsNot Nothing Then payment.De.SessionCampaignCode = HttpContext.Current.Session("CAMPAIGN_CODE")
        If HttpContext.Current.Session("AllowManualIntervention") IsNot Nothing AndAlso Boolean.Parse(HttpContext.Current.Session("AllowManualIntervention")) Then payment.De.AllowManualIntervention = HttpContext.Current.Session("AllowManualIntervention")

        payment.De.PartPaymentApplyTypeFlag = TEBUtilities.GetPartPaymentFlag()
        If amountPartPayment > 0 Then
            If String.IsNullOrWhiteSpace(payment.De.Amount) Then
                payment.De.RetailAmount = "0.00"
            Else
                payment.De.RetailAmount = payment.De.Amount
            End If
            payment.De.Amount = amountPartPayment

            payment.De.FeesPartPaymentEntity.FeeCode = GlobalConstants.BKFEE
            payment.De.FeesPartPaymentEntity.FeeCategory = GlobalConstants.FEECATEGORY_BOOKING
            payment.De.FeesPartPaymentEntity.CardType = CType(HttpContext.Current.Profile, TalentProfile).Basket.CARD_TYPE_CODE
            TEBUtilities.GetBookingFeeValuesForCurrentPartPayment(amountPartPayment, payment.De.FeesPartPaymentEntity.FeeValue, payment.De.FeesPartPaymentEntity.FeeValueActual)
            If amountPartPayment <= payment.De.FeesPartPaymentEntity.FeeValue Then
                returnError = True
                'redirectUrl = "~/PagesLogin/Checkout/checkout.aspx"
                HttpContext.Current.Session("TicketingGatewayError") = "TEB-PPALFV"
                HttpContext.Current.Session("TalentErrorCode") = "TEB-PPALFV"
            Else
                err = payment.TakePartPayment
                returnErrorCode = CheckResponseForError(payment.ResultDataSet, err)
                If returnErrorCode.Length > 0 Then
                    returnError = True
                    HttpContext.Current.Session("TicketingGatewayError") = returnErrorCode
                    HttpContext.Current.Session("TalentErrorCode") = returnErrorCode
                End If
                'If Not returnError Then paymentSuccess = True
            End If
        Else
            err = payment.TakePayment
            returnErrorCode = CheckResponseForError(payment.ResultDataSet, err)
            If returnErrorCode.Length > 0 Then
                err.HasError = True
                HttpContext.Current.Session("TalentErrorCode") = returnErrorCode
                HttpContext.Current.Session("TicketingGatewayError") = returnErrorCode
            End If
            If err.HasError Then
                If canRedirect Then
                    HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/basket.aspx")
                End If
            Else
                Dim paymentRef As String = GetPaymentRefFromPayment(payment.ResultDataSet)
                If String.IsNullOrEmpty(paymentRef) Then
                    HttpContext.Current.Session("TalentErrorCode") = "BAA"
                    HttpContext.Current.Session("TicketingGatewayError") = "BAA"
                Else
                    HttpContext.Current.Session("TalentPaymentReference") = paymentRef
                    If moduleDefaults.PaymentGatewayExternal = GlobalConstants.PAYMENTGATEWAY_VANGUARD Then
                        Dim tDataObjects As New Talent.Common.TalentDataObjects()
                        tDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
                        tDataObjects.BasketSettings.TblBasketPayment.UpdateTicketingPaymentRef(basketHeaderId, paymentRef)
                    End If
                End If
                Dim paymentAmount As String = String.Empty

                paymentAmount = GetPaymentAmountFromPayment(payment.ResultDataSet, merchandiseAmount)

                If payment.De.PaymentType = GlobalConstants.PAYPALPAYMENTTYPE Then
                    paymentAmount = HttpContext.Current.Session("ExtPaymentAmount")
                Else
                    HttpContext.Current.Session("ExtPaymentAmount") = paymentAmount
                End If

                If canSendConfirmEmail AndAlso moduleDefaults.ConfirmationEmail AndAlso (Not String.IsNullOrWhiteSpace(paymentRef)) Then
                    Dim Order_Email As New Talent.eCommerce.Order_Email
                    Order_Email.SendTicketingConfirmationEmail(paymentRef)
                End If
                If canRedirect Then
                    Dim order As New Order()
                    order.ProcessMerchandiseInBackend(False, False, String.Empty, False)
                    HttpContext.Current.Response.Redirect("~/PagesLogin/Checkout/CheckoutOrderConfirmation.aspx?PaymentRef=" & paymentRef.Trim & "&paymentType=" & HttpContext.Current.Session("ExternalGatewayPayType").ToString() & "&paymentAmount=" & paymentAmount)
                End If
            End If
        End If


    End Sub

    ''' <summary>
    ''' External Checkout routine, marks the payment as failure.
    ''' </summary>
    ''' <param name="canRedirect">if set to <c>true</c> [can redirect].</param>
    ''' <param name="basketHeaderId">The basket header id.</param>
    Public Sub CheckoutExternalFailure(ByVal canRedirect As Boolean, ByVal basketHeaderId As String, ByVal canResetPayProcess As Boolean)
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults
        Dim redirectURL As String = String.Empty
        Dim err As New ErrorObj
        Dim returnErrorCode As String = String.Empty
        Dim payment As New TalentPayment
        Dim dePayment As New DEPayments
        With dePayment
            If String.IsNullOrWhiteSpace(basketHeaderId) Then
                .SessionId = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
            Else
                .SessionId = basketHeaderId
            End If
            .Source = GlobalConstants.SOURCE
            If HttpContext.Current.Session("ExternalGatewayPayType") IsNot Nothing Then
                .PaymentType = HttpContext.Current.Session("ExternalGatewayPayType").ToString()
            End If
            .exPayment = "3" 'This flag instructs the back end to reset everything from payment pending
            .CanResetPayProcess = canResetPayProcess
        End With
        If HttpContext.Current.Session("CheckoutExternalStarted") IsNot Nothing Then
            HttpContext.Current.Session.Remove("CheckoutExternalStarted")
        End If
        Dim wfrPage As New WebFormResource
        With wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "TicketingGateway.aspx"
            .PageCode = "TicketingGateway.aspx"
        End With
        payment.De = dePayment
        payment.Settings = TEBUtilities.GetSettingsObject()
        payment.Settings.BusinessUnit = TalentCache.GetBusinessUnit
        payment.Settings.StoredProcedureGroup = TEBUtilities.GetStoredProcedureGroup
        payment.Settings.Cacheing = CType(wfrPage.Attribute("PaymentCacheing"), Boolean)
        payment.Settings.CacheTimeMinutes = CType(wfrPage.Attribute("PaymentCacheTimeMinutes"), Integer)
        payment.Settings.CacheDependencyPath = moduleDefaults.CacheDependencyPath
        payment.Settings.OriginatingSource = TEBUtilities.GetOriginatingSource(HttpContext.Current.Session.Item("Agent"))
        payment.De.BasketContentType = CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType
        err = payment.PaymentPending
        returnErrorCode = CheckResponseForError(payment.ResultDataSet, err)

        If returnErrorCode.Length > 0 Then
            err.HasError = True
            HttpContext.Current.Session("TalentErrorCode") = returnErrorCode
            HttpContext.Current.Session("TicketingGatewayError") = returnErrorCode
            redirectURL = "~/PagesPublic/Basket/basket.aspx"
        Else
            'error only in external payment gateway
            redirectURL = "~/PagesLogin/Checkout/checkout.aspx"
        End If
        Select Case returnErrorCode
            Case Is = "W2", "**"
                DeleteBasketHeader()
            Case "WF"
                ' WF indicates no basket records found. This (normally) is caused only by Webmon.
                ' In this case clear front-end basket.
                DeleteBasketDetail()
            Case "NS", "99", "MF", "W1", "WS", "WT", "WM", "WR", "NF"
                RefreshBasketContent()
        End Select
        If canRedirect Then
            HttpContext.Current.Response.Redirect(redirectURL)
        End If
    End Sub

#End Region

#Region "Linked Products Functions"

    ''' <summary>
    ''' Check the product relations tables for a link to the current product code or price code
    ''' </summary>
    ''' <param name="productCode">The current product code to check against</param>
    ''' <param name="priceCode">The current price code to check against</param>
    ''' <param name="campaignCode">The current campaign code to check against</param>
    ''' <param name="productHasRelatedProducts">Boolean to indicate the current product has related products</param>
    ''' <param name="productHasMandatoryRelatedProducts">Boolean to indicate the current product has mandatory related products</param>
    ''' <remarks></remarks>
    Public Sub CheckForLinkedProducts(ByVal productCode As String, ByVal priceCode As String, ByVal campaignCode As String, ByVal productSubType As String, ByRef productHasRelatedProducts As Boolean, ByRef productHasMandatoryRelatedProducts As Boolean)
        Dim dtProductRelations As New DataTable
        If Not String.IsNullOrEmpty(campaignCode) Then priceCode = campaignCode 'Overwrite the price code with any campaign code
        productHasRelatedProducts = DoesProductHaveRelatedProducts(productCode, dtProductRelations, priceCode, productSubType)
        If productHasRelatedProducts Then
            productHasMandatoryRelatedProducts = IsAnyLinkedProductsMandatory(productCode, dtProductRelations)
        Else
            productHasMandatoryRelatedProducts = False
        End If
    End Sub

    ''' <summary>
    ''' Checks the current product for related products
    ''' </summary>
    ''' <param name="ProductCode">The given product code</param>
    ''' <param name="dtProductRelations">The given product datatable to be populate</param>
    ''' <param name="priceCode">The price code for the given product code</param>
    ''' <param name="productSubType">The product sub type for the given product code</param>
    ''' <returns>True if the product has related products</returns>
    ''' <remarks></remarks>
    Public Function DoesProductHaveRelatedProducts(ByVal ProductCode As String, ByRef dtProductRelations As DataTable, ByVal priceCode As String, ByVal productSubType As String) As Boolean
        Dim productHasRelatedProducts As Boolean = False
        Dim tDataObjects As New Talent.Common.TalentDataObjects()
        tDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        dtProductRelations = tDataObjects.ProductsSettings.TblProductRelations.DoesProductHaveType2RelatedProducts(TalentCache.GetBusinessUnit(), TalentCache.GetDefaultPartner(), ProductCode, priceCode, productSubType)
        If dtProductRelations IsNot Nothing AndAlso dtProductRelations.Rows.Count > 0 Then productHasRelatedProducts = True
        Return productHasRelatedProducts
    End Function

    ''' <summary>
    ''' Check the given product code for mandatory related products in the given data table
    ''' </summary>
    ''' <param name="productCode">The product code to check on</param>
    ''' <param name="dtProductRelations">The datatable of related products</param>
    ''' <returns>True if the product has product relations that are mandatory</returns>
    ''' <remarks></remarks>
    Public Function IsAnyLinkedProductsMandatory(ByRef productCode As String, ByRef dtProductRelations As DataTable) As Boolean
        Dim hasMandatoryLinkedProducts As Boolean = False
        For Each row As DataRow In dtProductRelations.Rows
            If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(row("RELATED_PRODUCT_MANDATORY")) Then
                hasMandatoryLinkedProducts = True
                Exit For
            End If
        Next
        Return hasMandatoryLinkedProducts
    End Function

    ''' <summary>
    ''' Check to see if we need to go to the linked products or component selection page, otherwise redirect to the given url
    ''' </summary>
    ''' <param name="redirectUrl">The destination URL</param>
    ''' <param name="redirectToLinkedProductsPage">True or false to redirect to the linked product page</param>
    ''' <param name="productCode">The linked product code</param>
    ''' <param name="priceCode">The linked price code</param>
    ''' <param name="productSubType">The linked product sub type</param>
    ''' <param name="redirectToComponentSelectionPage">Redirect to Booking ("B") or Component Selection ("Y") Page.</param>
    ''' <param name="redirectToSeasonTicketExceptionsPage">True or false to redirect to the season ticket exception seat page</param>
    ''' <param name="packageId">The package id for the component selection page</param>
    ''' <param name="linkedMasterProduct">The linked master product</param>
    ''' <param name="bookingReference">The Booking Reference for the call</param>
    ''' <returns>A destination URL as a string</returns>
    ''' <remarks></remarks>
    Public Function HandleRedirect(ByVal redirectUrl As String, ByVal redirectToLinkedProductsPage As Boolean, ByVal productCode As String, ByVal priceCode As String, ByVal productSubType As String, ByVal redirectToComponentSelectionOrBookingPage As String, ByVal redirectToSeasonTicketExceptionsPage As Boolean, ByVal packageId As Long, Optional ByVal linkedMasterProduct As String = "", Optional ByVal bookingReference As Long = 0) As String
        Dim tDataObjects As New Talent.Common.TalentDataObjects()
        tDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        Dim userProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
        Dim newRedirectUrl As String = redirectUrl
        Dim talAgent As New Agent
        If redirectToComponentSelectionOrBookingPage = GlobalConstants.REDIRECT_TO_COMPONENT_SELECTION_PAGE Then
            Dim talPackage As New TalentPackage
            talPackage.RemoveCustomerPackageSession(packageId, productCode, 0)
            newRedirectUrl = "~/PagesPublic/ProductBrowse/ComponentSelection.aspx?product=" & productCode & "&PackageId=" & packageId & "&stage=1&productsubtype=" & productSubType
        ElseIf redirectToComponentSelectionOrBookingPage = GlobalConstants.REDIRECT_TO_BOOKING_PAGE Then
            Dim talPackage As New TalentPackage
            talPackage.RemoveCustomerPackageSession(packageId, productCode, 0)
            newRedirectUrl = "~/PagesPublic/Hospitality/HospitalityBooking.aspx?product=" & productCode & "&packageId=" & packageId & "&callid=" & bookingReference
        ElseIf linkedMasterProduct.Trim.Length = 0 AndAlso redirectToSeasonTicketExceptionsPage Then
            newRedirectUrl = "~/PagesPublic/ProductBrowse/SeasonTicketExceptions.aspx"
        ElseIf (String.IsNullOrWhiteSpace(CType(HttpContext.Current.Profile, TalentProfile).Basket.CAT_MODE)) AndAlso redirectToLinkedProductsPage Then
            ' Update the BasketHeader record with the LinkedProductMaster 
            Dim affectedRows As Integer = 0
            If linkedMasterProduct.Trim.Length <> 0 And linkedMasterProduct <> productCode Then
                ' If selecting a linked product seat using select seat method then no requirement to re-update the BasketHeader
                For Each item As TalentBasketItem In CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems
                    If item.Product = linkedMasterProduct Then
                        newRedirectUrl = "~/PagesPublic/ProductBrowse/LinkedProducts.aspx?pricecode=" & item.PRICE_CODE & "&productsubtype=" & item.PRODUCT_SUB_TYPE
                        Exit For
                    End If
                Next
            Else
                If ProductHasRelatedProducts Then
                    affectedRows = tDataObjects.BasketSettings.TblBasketHeader.UpdateLinkedProductMaster(userProfile.Basket.Basket_Header_ID, productCode) 'Need to do this line again on selecting a second product with linked products in order to resolve this JIRA: https://advancedticketing.atlassian.net/browse/TALITFC-48
                    newRedirectUrl = "~/PagesPublic/ProductBrowse/LinkedProducts.aspx?pricecode=" & priceCode & "&productsubtype=" & productSubType
                End If
            End If
        End If
        newRedirectUrl = CATHelper.GetCATPackageURLParamFromQueryString(newRedirectUrl)
        Return newRedirectUrl
    End Function

    ''' <summary>
    ''' Check to see if a master product that has more than 1 mandatory linked product has got all of it's subsequent products added to the basket (or being added to the basket)
    ''' First, retrieve the list of mandatory products, then check to see if they are all in the basket. Of the ones not in the basket check to see if they are in the process of being added
    ''' </summary>
    ''' <param name="productBeingAdded">Current product being added to the basket</param>
    ''' <param name="LinkedMasterProduct">The linked master product</param>
    ''' <param name="priceCode">The given price code</param>
    ''' <param name="campaignCode">The given campaign code</param>
    ''' <param name="productSubType">The given product sub type</param>
    ''' <returns>True if the all the products are in the basket or being added to the basket, otherwise false</returns>
    ''' <remarks></remarks>
    Public Function HaveAllLinkedMandatoryProductsBeenAdded(ByVal productBeingAdded As String, ByVal LinkedMasterProduct As String) As Boolean
        Dim allLinkedMandatoryProductsHaveBeenAdded As Boolean = False
        If LinkedMasterProduct.Length > 0 AndAlso productBeingAdded <> LinkedMasterProduct Then
            Dim dtProductRelations As New DataTable
            Dim productHasMandatoryRelatedProducts As Boolean = False
            Dim linkedMasterProductCodePriceCode As String = String.Empty
            Dim linkedMasterProductCodeProductSubType As String = String.Empty
            Dim linkedMasterProductCodeIsInBasket As Boolean = False
            getLinkedMasterProductDetails(LinkedMasterProduct, linkedMasterProductCodeIsInBasket, linkedMasterProductCodeProductSubType, linkedMasterProductCodePriceCode)
            If linkedMasterProductCodeIsInBasket AndAlso DoesProductHaveRelatedProducts(LinkedMasterProduct, dtProductRelations, linkedMasterProductCodePriceCode, linkedMasterProductCodeProductSubType) Then
                productHasMandatoryRelatedProducts = IsAnyLinkedProductsMandatory(LinkedMasterProduct, dtProductRelations)
            End If
            If productHasMandatoryRelatedProducts Then
                Dim listOfMandatoryProducts As New List(Of String)
                For Each row As DataRow In dtProductRelations.Rows
                    If row("PRODUCT") = LinkedMasterProduct Then
                        If CBool(row("RELATED_PRODUCT_MANDATORY")) Then
                            listOfMandatoryProducts.Add(row("RELATED_PRODUCT"))
                        End If
                    End If
                Next

                If listOfMandatoryProducts.Count > 0 Then
                    Dim listofMandatoryProductsNotInBasket As List(Of String) = listOfMandatoryProducts
                    For Each item As TalentBasketItem In CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems
                        If item.MODULE_ = GlobalConstants.BASKETMODULETICKETING AndAlso item.FEE_CATEGORY.Length = 0 AndAlso item.Product <> LinkedMasterProduct Then
                            If listOfMandatoryProducts.Contains(item.Product) AndAlso listofMandatoryProductsNotInBasket.Contains(item.Product) Then
                                listofMandatoryProductsNotInBasket.Remove(item.Product)
                            End If
                        End If
                    Next

                    If listofMandatoryProductsNotInBasket.Count = 0 Then
                        allLinkedMandatoryProductsHaveBeenAdded = True
                    ElseIf listofMandatoryProductsNotInBasket.Count = 1 AndAlso listofMandatoryProductsNotInBasket.Contains(productBeingAdded) Then
                        allLinkedMandatoryProductsHaveBeenAdded = True
                    Else
                        allLinkedMandatoryProductsHaveBeenAdded = False
                    End If
                End If
            End If
        End If
        Return allLinkedMandatoryProductsHaveBeenAdded
    End Function

    ''' <summary>
    ''' Get the linked product ID from the basket items based on the given master product code
    ''' </summary>
    ''' <param name="linkedMasterProduct">The master product</param>
    ''' <returns>The linked product ID</returns>
    ''' <remarks></remarks>
    Public Function ReturnLinkedProductIDFromBasket(ByVal linkedMasterProduct As String) As Integer
        Dim linkedProductID As Integer = 0
        For Each item As TalentBasketItem In CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems
            If item.Product = linkedMasterProduct Then
                If item.LINKED_PRODUCT_ID <> 0 Then
                    linkedProductID = item.LINKED_PRODUCT_ID
                    Exit For
                End If
            End If
        Next
        Return linkedProductID
    End Function

    ''' <summary>
    ''' Get the quantity definition value for the product specified that's being added based on bulk sales mode and product relationships
    ''' </summary>
    ''' <param name="productCode">The product being added</param>
    ''' <param name="minQuantity">The minimum quantity value of the product being added</param>
    ''' <param name="maxQuantity">The maximum quantity value of the product being added</param>
    ''' <param name="defaultQuantity">The default quantity of the product being added, this is a string as it could be empty rather than 0</param>
    ''' <param name="isReadOnly">Is the quantity box read-only. Only applies to Linked Products</param>
    ''' <param name="isForVisualSeating">Is the quantity defintions for the SVG seating</param>
    ''' <param name="restrictToSingleQuantitySelection">Set this when the user is choosing an exception seat on visual seat selection or changing a hospitality component seat</param>
    ''' <param name="dtComponents">DataTable of Components</param>
    ''' <param name="dtComponentsSeats">DataTable of ComponentsSeats</param>
    ''' <param name="changeAllSeats">changeAllSeats flag is determined whether user can change all seats or not</param>
    ''' <remarks></remarks>
    Public Sub GetQuantityDefintions(ByVal productCode As String, ByRef minQuantity As Integer, ByRef maxQuantity As Integer, ByRef defaultQuantity As String, ByRef isReadOnly As Boolean, Optional isForVisualSeating As Boolean = False, Optional restrictToSingleQuantitySelection As Boolean = False, Optional dtComponents As DataTable = Nothing, Optional dtComponentsSeats As DataTable = Nothing, Optional changeAllSeats As Boolean = False)
        minQuantity = 0
        defaultQuantity = String.Empty
        Dim agentProfile As New Talent.eCommerce.Agent
        isReadOnly = False
        If isForVisualSeating Then
            If CATHelper.IsItCATRequest(-2) AndAlso Not CATHelper.IsPackageTransferRequested() Then
                maxQuantity = 1
                minQuantity = 1
            Else
                If restrictToSingleQuantitySelection Then
                    maxQuantity = 1
                    minQuantity = 1
                Else
                    If changeAllSeats Then
                        If agentProfile.IsAgent Then
                            If dtComponents IsNot Nothing AndAlso dtComponents.Rows.Count > 0 Then
                                minQuantity = TEBUtilities.CheckForDBNull_Int(dtComponents.Rows(0).Item("MinQty"))
                                maxQuantity = TEBUtilities.CheckForDBNull_Int(dtComponents.Rows(0).Item("MaxQty"))
                            End If
                        Else
                            minQuantity = dtComponentsSeats.Rows.Count
                            maxQuantity = dtComponentsSeats.Rows.Count
                        End If
                    Else
                        maxQuantity = 500
                    End If
                End If
            End If
        Else
            maxQuantity = 99
        End If


        If agentProfile.BulkSalesMode Then
            If isForVisualSeating Then
                maxQuantity = 500
            Else
                Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults
                maxQuantity = moduleDefaults.BulkSalesModeBasketLimit
            End If
        Else
            Dim linkedMasterProductCode As String = String.Empty
            Dim linkedMasterProductCodeIsInBasket As Boolean = False
            Dim linkedMasterProductCodePriceCode As String = String.Empty
            Dim linkedMasterProductCodeSubType As String = String.Empty
            Dim basket As TalentBasket = CType(HttpContext.Current.Profile, TalentProfile).Basket
            Dim tDataObjects As New Talent.Common.TalentDataObjects
            tDataObjects.Settings = TEBUtilities.GetSettingsObject()
            linkedMasterProductCode = tDataObjects.BasketSettings.TblBasketHeader.GetLinkedMasterProduct(basket.Basket_Header_ID)
            If Not String.IsNullOrEmpty(linkedMasterProductCode) Then
                getLinkedMasterProductDetails(linkedMasterProductCode, linkedMasterProductCodeIsInBasket, linkedMasterProductCodeSubType, linkedMasterProductCodePriceCode)
                If linkedMasterProductCodeIsInBasket Then
                    Dim dtProductRelations As New DataTable
                    If DoesProductHaveRelatedProducts(linkedMasterProductCode, dtProductRelations, linkedMasterProductCodePriceCode, linkedMasterProductCodeSubType) Then
                        getProductRelationsQuantityDefinitions(dtProductRelations, linkedMasterProductCode, productCode, minQuantity, maxQuantity, defaultQuantity, isReadOnly)
                    End If
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Get the master product information based on what's in the basket
    ''' </summary>
    ''' <param name="linkedMasterProductCode">The linked master product</param>
    ''' <param name="linkedMasterProductCodeIsInBasket">Is the linked master product in the basket</param>
    ''' <param name="linkedMasterProductCodeSubType">What is the linked master product sub type</param>
    ''' <param name="linkedMasterProductCodePriceCode">What is the linked master product price code</param>
    ''' <remarks></remarks>
    Private Sub getLinkedMasterProductDetails(ByVal linkedMasterProductCode As String, ByRef linkedMasterProductCodeIsInBasket As Boolean, ByRef linkedMasterProductCodeSubType As String, ByRef linkedMasterProductCodePriceCode As String)
        For Each item As TalentBasketItem In CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems
            If item.Product = linkedMasterProductCode Then
                linkedMasterProductCodeIsInBasket = True
                linkedMasterProductCodeSubType = item.PRODUCT_SUB_TYPE
                linkedMasterProductCodePriceCode = item.PRICE_CODE
                Exit For
            End If
        Next
    End Sub

    ''' <summary>
    ''' Get the quantity defintions details for the given product relationship
    ''' </summary>
    ''' <param name="dtProductRelations">The product relationship data table with rows based on the current product information</param>
    ''' <param name="masterProductCode">The master product code</param>
    ''' <param name="childProductCode">The product code being added (product being linked to). Not the master product</param>
    ''' <param name="minQuantity">The minimum quantity value of the product being added</param>
    ''' <param name="maxQuantity">The maximum quantity value of the product being added</param>
    ''' <param name="defaultQuantity">The default quantity value of the product being added</param>
    ''' <param name="isReadOnly">Is the quantity value readonly, only applies on the linked products page</param>
    ''' <remarks></remarks>
    Private Sub getProductRelationsQuantityDefinitions(ByRef dtProductRelations As DataTable, ByRef masterProductCode As String, ByRef childProductCode As String, ByRef minQuantity As Integer, ByRef maxQuantity As Integer, ByRef defaultQuantity As String, ByRef isReadOnly As Boolean)
        For Each row As DataRow In dtProductRelations.Rows
            If row("RELATED_PRODUCT") = childProductCode Then
                Dim numOfMasterProductsInBasket As Integer = 0
                Dim numOfChildProductsInBasket As Integer = 0
                Dim useRatios As Boolean = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(row("RELATED_TICKETING_PRODUCT_QTY_RATIO"))
                Dim roundUp As Boolean = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(row("RELATED_TICKETING_PRODUCT_QTY_ROUND_UP"))
                linkedProductsInBasket(masterProductCode, childProductCode, numOfMasterProductsInBasket, numOfChildProductsInBasket)

                If Not row("RELATED_TICKETING_PRODUCT_QTY").Equals(DBNull.Value) Then
                    If useRatios Then
                        defaultQuantity = getRealValueFromRatio(numOfMasterProductsInBasket, TEBUtilities.CheckForDBNull_Int(row("RELATED_TICKETING_PRODUCT_QTY")), roundUp)
                        defaultQuantity = defaultQuantity - numOfChildProductsInBasket
                        If defaultQuantity < 0 Then defaultQuantity = 0
                    Else
                        defaultQuantity = TEBUtilities.CheckForDBNull_Int(row("RELATED_TICKETING_PRODUCT_QTY"))
                    End If
                End If

                isReadOnly = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(row("RELATED_TICKETING_PRODUCT_QTY_READONLY"))
                If Not isReadOnly Then
                    If Not row("RELATED_TICKETING_PRODUCT_QTY_MIN").Equals(DBNull.Value) Then
                        If useRatios Then
                            minQuantity = getRealValueFromRatio(numOfMasterProductsInBasket, CDec(row("RELATED_TICKETING_PRODUCT_QTY_MIN")), roundUp)
                            minQuantity = minQuantity - numOfChildProductsInBasket
                            If minQuantity < 0 Then minQuantity = 0
                        Else
                            minQuantity = CInt(row("RELATED_TICKETING_PRODUCT_QTY_MIN"))
                        End If
                    End If
                    If Not row("RELATED_TICKETING_PRODUCT_QTY_MAX").Equals(DBNull.Value) Then
                        If useRatios Then
                            maxQuantity = getRealValueFromRatio(numOfMasterProductsInBasket, CDec(row("RELATED_TICKETING_PRODUCT_QTY_MAX")), roundUp)
                            maxQuantity = maxQuantity - numOfChildProductsInBasket
                            If maxQuantity < 0 Then maxQuantity = 0
                        Else
                            maxQuantity = CInt(row("RELATED_TICKETING_PRODUCT_QTY_MAX"))
                        End If
                        If (minQuantity = 0 AndAlso maxQuantity = 0) Then
                            defaultQuantity = String.Empty
                            isReadOnly = True
                        End If
                    End If
                End If
                Exit For
            End If
        Next
    End Sub

    ''' <summary>
    ''' Retrieve the number of master products and child products that are in the basket
    ''' </summary>
    ''' <param name="masterProduct">The linked master product to look for</param>
    ''' <param name="childProduct">The linked child product to look for</param>
    ''' <param name="numOfMasterProductsInBasket">The number of linked master products found in the basket</param>
    ''' <param name="numOfChildProductsInBasket">The number of linked child products found in the basket</param>
    ''' <remarks></remarks>
    Private Sub linkedProductsInBasket(ByVal masterProduct As String, ByVal childProduct As String, ByRef numOfMasterProductsInBasket As Integer, ByRef numOfChildProductsInBasket As Integer)
        numOfMasterProductsInBasket = 0
        numOfChildProductsInBasket = 0
        For Each item As TalentBasketItem In CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems
            If item.Product = masterProduct Then numOfMasterProductsInBasket += 1
            If item.Product = childProduct Then numOfChildProductsInBasket += 1
        Next
    End Sub

    ''' <summary>
    ''' Get the real whole number to use for default quantity/min/max validation based on the quantity requested and the product link setup
    ''' </summary>
    ''' <param name="quantity">The quantity requested</param>
    ''' <param name="ratio">The ratio to work with</param>
    ''' <param name="roundUp">Round up or down</param>
    ''' <returns>Whole number as a string</returns>
    ''' <remarks></remarks>
    Private Function getRealValueFromRatio(ByVal quantity As Integer, ByVal ratio As Decimal, ByRef roundUp As Boolean) As String
        Dim realValue As String = String.Empty
        Dim wholeNumber As Integer = 0
        ratio = ratio / 100
        If roundUp Then
            wholeNumber = CInt(Decimal.Round(ratio, 0, MidpointRounding.AwayFromZero))
        Else
            wholeNumber = CInt(Decimal.Round(ratio, 0, MidpointRounding.ToEven))
        End If
        realValue = CStr(wholeNumber * quantity)
        Return realValue
    End Function

#End Region

#Region "Other Functions"

    ''' <summary>
    ''' Retrieves the payment reference from the given dataset
    ''' </summary>
    ''' <param name="resultDataSet">Dataset object from checkout function call WS608R</param>
    ''' <returns>Payment Reference as a string</returns>
    ''' <remarks></remarks>
    Public Function GetPaymentRefFromPayment(ByVal resultDataSet As DataSet) As String
        Dim paymentRef As String = String.Empty
        If resultDataSet IsNot Nothing AndAlso resultDataSet.Tables.Count > 0 Then
            Dim paymentTable As DataTable = resultDataSet.Tables(0)
            For Each row As Data.DataRow In paymentTable.Rows
                If Not row("PaymentReference").Equals(String.Empty) Then
                    paymentRef = row("PaymentReference")
                    Exit For
                End If
            Next
        End If
        Return paymentRef
    End Function

    ''' <summary>
    ''' Retrieves the payment amount from the given dataset. If the dataset is empty but there is a merchandise value, add the merchandise value anyway.
    ''' </summary>
    ''' <param name="resultDataSet">Dataset object from checkout function call WS608R</param>
    ''' <param name="merchandiseAmount">Merchandise amount as decimal</param>
    ''' <returns>Payment Amount as a string</returns>
    ''' <remarks></remarks>
    Public Function GetPaymentAmountFromPayment(ByVal resultDataSet As DataSet, ByVal merchandiseAmount As Decimal) As String
        Dim paymentAmnt As String = String.Empty
        Dim ticketsTotal As Decimal = 0
        If resultDataSet IsNot Nothing Then
            If resultDataSet.Tables.Count > 1 AndAlso resultDataSet.Tables(1).Rows.Count > 0 Then
                Try
                    ticketsTotal = CDec(resultDataSet.Tables(1).Rows(0).Item("TotalPrice"))
                Catch ex As Exception
                    ticketsTotal = 0
                End Try

                'Retail total is included in the ticket total when the orders are stored in the ticketing DB
                If Talent.eCommerce.Utilities.IsTicketingDBforRetailOrders Then
                    paymentAmnt = CStr(ticketsTotal)
                Else
                    paymentAmnt = CStr(ticketsTotal + merchandiseAmount)
                End If
            End If
        Else
            If merchandiseAmount > 0 Then
                paymentAmnt = merchandiseAmount.ToString()
            End If
        End If
        Return paymentAmnt
    End Function

    ''' <summary>
    ''' Checks the DBAmendBasket.ResultDataSet for an error table and any errors contained within it.
    ''' </summary>
    ''' <param name="resultDataSet">DBAmendBasket.ResultDataSet as a dataset</param>
    ''' <param name="err">Talent.Common.ErrorObj</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckResponseForError(ByVal resultDataSet As DataSet, ByVal err As ErrorObj) As String
        Dim errorCode As String = String.Empty
        If resultDataSet IsNot Nothing Then
            If resultDataSet.Tables.Count > 0 Then
                Dim errorTable As DataTable = resultDataSet.Tables(0)
                If resultDataSet.Tables(0) IsNot Nothing Then
                    For Each row As Data.DataRow In errorTable.Rows
                        If Not row("ReturnCode").Equals(String.Empty) Then
                            errorCode = row("ReturnCode")
                            Exit For
                        End If
                    Next
                Else
                    errorCode = "AAD"
                End If
            Else
                errorCode = "AAC"
            End If
            If resultDataSet IsNot Nothing AndAlso resultDataSet.Tables("BasketProcessorStatus") IsNot Nothing Then
                If resultDataSet.Tables("BasketProcessorStatus").Rows.Count > 0 Then
                    If resultDataSet.Tables("BasketProcessorStatus").Rows(0)("ErrorOccurred") = "E" Then
                        errorCode = resultDataSet.Tables("BasketProcessorStatus").Rows(0)("ReturnCode")
                    End If
                End If
            End If
        Else
            If err.HasError Then
                errorCode = "AAB"
            End If
        End If
        If errorCode.Length = 0 Then
            If err.HasError Then
                errorCode = err.ErrorStatus
            End If
        End If
        Return errorCode
    End Function

    ''' <summary>
    ''' Sets any session items from the add ticketing item basket request
    ''' </summary>
    ''' <param name="basket">Talent Basket from talent common</param>
    ''' <remarks></remarks>
    Public Sub SetSessionValuesFromBasket(ByVal basket As Talent.Common.TalentBasket)
        HttpContext.Current.Session("ProductTransactionTicketLimitExceeded") = Nothing
        HttpContext.Current.Session("ProductTransactionTicketLimit") = Nothing
        HttpContext.Current.Session("ProductTransactionTicketLimitProductCode") = Nothing
        If Not basket.DeAddTicketingItems Is Nothing Then
            If basket.DeAddTicketingItems.ProductTransactionTicketLimitExceeded Then
                HttpContext.Current.Session("ProductTransactionTicketLimitExceeded") = True
                HttpContext.Current.Session("ProductTransactionTicketLimit") = basket.DeAddTicketingItems.ProductTransactionTicketLimit
                If Not String.IsNullOrWhiteSpace(basket.DeAddTicketingItems.ProductCode) Then
                    HttpContext.Current.Session("ProductTransactionTicketLimitProductCode") = basket.DeAddTicketingItems.ProductCode
                End If
            End If
        End If
        If basket.DeAddTicketingItems.PackageID > 0 Then
            Dim Package As New TalentPackage
            Package.RemoveCustomerPackageSession(basket.DeAddTicketingItems.PackageID, basket.DeAddTicketingItems.ProductCode, basket.DeAddTicketingItems.CallID)
        End If
    End Sub

    Public Function ProcessNewBasket(ByVal results As DataSet) As Boolean
        Dim settingsEntity As DESettings = TEBUtilities.GetSettingsObject()
        Dim talBasketProcessor As New Talent.Common.TalentBasketProcessor
        Dim talDataObjects As New TalentDataObjects
        Dim processResult As Boolean = False
        Try
            talDataObjects.Settings = settingsEntity
            talBasketProcessor.Settings = settingsEntity
            talBasketProcessor.FulfilmentFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(talDataObjects.Settings.BusinessUnit)
            talBasketProcessor.CardTypeFeeCategory = talDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(talDataObjects.Settings.BusinessUnit)
            Dim errObj As ErrorObj = talBasketProcessor.ProcessNewBasket(GlobalConstants.SOURCE, CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID, results)
            If errObj.HasError Then
                processResult = False
            Else
                processResult = True
                HttpContext.Current.Session("TicketingBasketChanged") = "Y"
            End If
        Catch ex As Exception
            processResult = False
        End Try
        Return processResult
    End Function

    ''' <summary>
    ''' Extracts the tickets selected from the session variable and populates the arrays.
    ''' </summary>
    ''' <param name="updateBasketSession">The session string object</param>
    ''' <returns>A boolean value to indicate the extraction is successful</returns>
    ''' <remarks></remarks>
    Public Function ExtractSessionValues(ByVal updateBasketSession As String, Optional ByVal excludePriceBandCheck As Boolean = False) As Boolean
        Dim missingParam As Boolean = False
        Dim exitLoop As Boolean = False
        Dim count As Integer = 1
        Dim nameValueCollection As New NameValueCollection
        nameValueCollection = HttpUtility.ParseQueryString(updateBasketSession)
        _fulfilmentMethod.Clear()
        _isPackage.Clear()
        _product.Clear()
        _customer.Clear()
        _concession.Clear()
        _priceCode.Clear()
        _productType.Clear()
        _originalCust.Clear()
        _seat.Clear()
        _priceCodeOverridden.Clear()
        _bulkSalesID.Clear()
        _bulkSalesQuantity.Clear()
        _allocatedSeat.Clear()
        _customerSelection = "N"
        While Not exitLoop
            If Not HttpContext.Current.Server.UrlDecode(nameValueCollection("fulfilmentMethod" & count)) Is Nothing Then
                _fulfilmentMethod.Add(HttpContext.Current.Server.UrlDecode(nameValueCollection("fulfilmentMethod" & count)))
            End If
            If Not HttpContext.Current.Server.UrlDecode(nameValueCollection("isPackage" & count)) Is Nothing Then
                If ((HttpContext.Current.Server.UrlDecode(nameValueCollection("isPackage" & count))).Trim).Equals("Y") Then
                    _isPackage.Add("Y")
                Else
                    _isPackage.Add("N")
                End If
            Else
                _isPackage.Add("N")
            End If
            If Not HttpContext.Current.Server.UrlDecode(nameValueCollection("priceCodeOverridden" & count)) Is Nothing Then
                _priceCodeOverridden.Add(HttpContext.Current.Server.UrlDecode(nameValueCollection("priceCodeOverridden" & count)))
            Else
                _priceCodeOverridden.Add("")
            End If

            If Not HttpContext.Current.Server.UrlDecode(nameValueCollection("product" & count)) Is Nothing _
            AndAlso Not HttpContext.Current.Server.UrlDecode(nameValueCollection("product" & count)).Equals(String.Empty) Then
                _product.Add(HttpContext.Current.Server.UrlDecode(nameValueCollection("product" & count)))
                If Not HttpContext.Current.Server.UrlDecode(nameValueCollection("customer" & count)) Is Nothing _
                AndAlso Not HttpContext.Current.Server.UrlDecode(nameValueCollection("customer" & count)).Equals(String.Empty) Then
                    _customer.Add(HttpContext.Current.Server.UrlDecode(nameValueCollection("customer" & count)))
                    If (Not HttpContext.Current.Server.UrlDecode(nameValueCollection("concession" & count)) Is Nothing _
                    AndAlso Not HttpContext.Current.Server.UrlDecode(nameValueCollection("concession" & count)).Equals(String.Empty)) OrElse excludePriceBandCheck Then
                        _concession.Add(HttpContext.Current.Server.UrlDecode(nameValueCollection("concession" & count)))
                        If Not HttpContext.Current.Server.UrlDecode(nameValueCollection("priceCode" & count)) Is Nothing _
                        AndAlso Not HttpContext.Current.Server.UrlDecode(nameValueCollection("priceCode" & count)).Equals(String.Empty) Then
                            _priceCode.Add(HttpContext.Current.Server.UrlDecode(nameValueCollection("priceCode" & count)))
                        Else
                            _priceCode.Add(String.Empty)
                        End If
                        If Not HttpContext.Current.Server.UrlDecode(nameValueCollection("productType" & count)) Is Nothing _
                            AndAlso Not HttpContext.Current.Server.UrlDecode(nameValueCollection("productType" & count)).Equals(String.Empty) Then
                            _productType.Add(HttpContext.Current.Server.UrlDecode(nameValueCollection("productType" & count)))
                            If Not HttpContext.Current.Server.UrlDecode(nameValueCollection("originalCust" & count)) Is Nothing _
                            AndAlso Not HttpContext.Current.Server.UrlDecode(nameValueCollection("originalCust" & count)).Equals(String.Empty) Then
                                _originalCust.Add(HttpContext.Current.Server.UrlDecode(nameValueCollection("originalCust" & count)))

                                'Check Optional seat information based on normal sales, package sales or bulk sales
                                If Not HttpContext.Current.Server.UrlDecode(nameValueCollection("seat" & count)) Is Nothing _
                                AndAlso Not HttpContext.Current.Server.UrlDecode(nameValueCollection("seat" & count)).Equals(String.Empty) Then
                                    _seat.Add(HttpContext.Current.Server.UrlDecode(nameValueCollection("seat" & count)))
                                Else
                                    _seat.Add(String.Empty)
                                End If

                                If Not HttpContext.Current.Server.UrlDecode(nameValueCollection("bulkSalesID" & count)) Is Nothing _
                                AndAlso Not HttpContext.Current.Server.UrlDecode(nameValueCollection("bulkSalesID" & count)).Equals(String.Empty) Then
                                    _bulkSalesID.Add(HttpContext.Current.Server.UrlDecode(nameValueCollection("bulkSalesID" & count)))
                                    If Not HttpContext.Current.Server.UrlDecode(nameValueCollection("bulkSalesQuantity" & count)) Is Nothing _
                                    AndAlso Not HttpContext.Current.Server.UrlDecode(nameValueCollection("bulkSalesQuantity" & count)).Equals(String.Empty) Then
                                        _bulkSalesQuantity.Add(HttpContext.Current.Server.UrlDecode(nameValueCollection("bulkSalesQuantity" & count)))
                                    Else
                                        _bulkSalesQuantity.Add(0)
                                    End If
                                Else
                                    _bulkSalesID.Add(0)
                                    _bulkSalesQuantity.Add(0)
                                End If

                                'Both Seat and bulkID is Nothing or Empty then check for package id
                                If (HttpContext.Current.Server.UrlDecode(nameValueCollection("seat" & count)) Is Nothing OrElse HttpContext.Current.Server.UrlDecode(nameValueCollection("seat" & count)).Equals(String.Empty)) _
                                    AndAlso (HttpContext.Current.Server.UrlDecode(nameValueCollection("bulkSalesID" & count)) Is Nothing OrElse HttpContext.Current.Server.UrlDecode(nameValueCollection("bulkSalesID" & count)).Equals(String.Empty)) Then
                                    If (HttpContext.Current.Server.UrlDecode(nameValueCollection("packageid" & count)) Is Nothing OrElse HttpContext.Current.Server.UrlDecode(nameValueCollection("packageid" & count)).Equals(String.Empty)) Then
                                        _missingParamErrorCode = "SE-PKGID"
                                        missingParam = True
                                    End If
                                End If

                                'allocated seat details - this isn't error checked as it's not mandatory
                                If Not HttpContext.Current.Server.UrlDecode(nameValueCollection("allocatedSeat" & count)) Is Nothing _
                                    AndAlso Not HttpContext.Current.Server.UrlDecode(nameValueCollection("allocatedSeat" & count)).Equals(String.Empty) Then
                                    _allocatedSeat.Add(HttpContext.Current.Server.UrlDecode(nameValueCollection("allocatedSeat" & count)))
                                Else
                                    _allocatedSeat.Add(String.Empty)
                                End If
                            Else
                                'Original Customer is Nothing or Empty
                                _missingParamErrorCode = "OC"
                                missingParam = True
                            End If
                        Else
                            'Price Code is Nothing or Empty
                            _missingParamErrorCode = "PC"
                            missingParam = True
                        End If
                    Else
                        'Concession is Nothing or Empty
                        _missingParamErrorCode = "CO"
                        missingParam = True
                    End If
                Else
                    'Customer is Nothing or Empty
                    _missingParamErrorCode = "CU"
                    missingParam = True
                End If
            Else
                exitLoop = True
            End If
            If missingParam Then
                exitLoop = True
            End If
            count += 1
        End While

        Return missingParam

    End Function

    ''' <summary>
    ''' Extracts the tickets selected from the querystring and populates the arrays.
    ''' </summary>
    ''' <returns>A boolean value to indicate the extraction is successful</returns>
    ''' <remarks></remarks>
    Public Function ExtractQueryStringValues() As Boolean
        Dim missingParam As Boolean = False
        Dim exitLoop As Boolean = False
        Dim count As Integer = 1
        _fulfilmentMethod.Clear()
        _isPackage.Clear()
        _product.Clear()
        _customer.Clear()
        _concession.Clear()
        _priceCode.Clear()
        _productType.Clear()
        _originalCust.Clear()
        _seat.Clear()
        _priceCodeOverridden.Clear()
        _bulkSalesID.Clear()
        _customerSelection = ""
        If Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("customerSelection")) Is Nothing _
                       AndAlso Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("customerSelection")).Equals(String.Empty) Then
            _customerSelection = "Y"
        Else
            _customerSelection = "N"
        End If
        While Not exitLoop
            If Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("fulfilmentMethod" & count)) Is Nothing Then
                _fulfilmentMethod.Add(HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("fulfilmentMethod" & count)))
            End If
            If Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("isPackage" & count)) Is Nothing Then
                If ((HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("isPackage" & count))).Trim).Equals("Y") Then
                    _isPackage.Add("Y")
                Else
                    _isPackage.Add("N")
                End If
            Else
                _isPackage.Add("N")
            End If

            If Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("priceCodeOverridden" & count)) Is Nothing Then
                _priceCodeOverridden.Add(HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("priceCodeOverridden" & count)))
            Else
                _priceCodeOverridden.Add("")
            End If
            If Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("product" & count)) Is Nothing _
            AndAlso Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("product" & count)).Equals(String.Empty) Then
                _product.Add(HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("product" & count)))
                If Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("customer" & count)) Is Nothing _
                AndAlso Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("customer" & count)).Equals(String.Empty) Then
                    _customer.Add(HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("customer" & count)))
                    If Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("concession" & count)) Is Nothing _
                    AndAlso Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("concession" & count)).Equals(String.Empty) Then
                        _concession.Add(HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("concession" & count)))
                        If (Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("priceCode" & count)) Is Nothing _
                        AndAlso Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("priceCode" & count)).Equals(String.Empty)) _
                         OrElse (HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("productType" & count)).Equals("T") _
                         Or HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("productType" & count)).Equals("E")) Then
                            Dim priceCodeValue = HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("priceCode" & count))
                            Dim priceCode = If(Not priceCodeValue.Equals(String.Empty), priceCodeValue, String.Empty)
                            _priceCode.Add(priceCode)
                            If Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("productType" & count)) Is Nothing _
                            AndAlso Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("productType" & count)).Equals(String.Empty) Then
                                _productType.Add(HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("productType" & count)))
                                If Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("originalCust" & count)) Is Nothing _
                                AndAlso Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("originalCust" & count)).Equals(String.Empty) Then
                                    _originalCust.Add(HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("originalCust" & count)))

                                    'Check Optional seat information based on normal sales, package sales or bulk sales
                                    If Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("seat" & count)) Is Nothing _
                                    AndAlso Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("seat" & count)).Equals(String.Empty) Then
                                        _seat.Add(HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("seat" & count)))
                                        _bulkSalesID.Add(0)
                                        _bulkSalesQuantity.Add(0)
                                    Else
                                        If Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("bulkSalesID" & count)) Is Nothing _
                                        AndAlso Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("bulkSalesID" & count)).Equals(String.Empty) Then
                                            _bulkSalesID.Add(HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("bulkSalesID" & count)))
                                            If Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("bulkSalesQuantity" & count)) Is Nothing _
                                            AndAlso Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("bulkSalesQuantity" & count)).Equals(String.Empty) Then
                                                _bulkSalesQuantity.Add(HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("bulkSalesQuantity" & count)))
                                                'Bulk Selling Mode
                                                _seat.Add(String.Empty)
                                            End If
                                        Else
                                            'Both Seat and PackageID is Nothing or Empty
                                            _missingParamErrorCode = "SE-PKGID"
                                            missingParam = True
                                            _bulkSalesID.Add(0)
                                            _bulkSalesQuantity.Add(0)
                                        End If
                                    End If

                                    'allocated seat details - this isn't error checked as it's not mandatory
                                    If Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("allocatedSeat" & count)) Is Nothing _
                                        AndAlso Not HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("allocatedSeat" & count)).Equals(String.Empty) Then
                                        _allocatedSeat.Add(HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.QueryString("allocatedSeat" & count)))
                                    Else
                                        _allocatedSeat.Add(String.Empty)
                                    End If
                                Else
                                    'Original Customer is Nothing or Empty
                                    _missingParamErrorCode = "OC"
                                    missingParam = True
                                End If
                            Else
                                'Product Type is Nothing or Empty
                                _missingParamErrorCode = "PT"
                                missingParam = True
                            End If
                        Else
                            'Price Code is Nothing or Empty
                            _missingParamErrorCode = "PC"
                            missingParam = True
                        End If
                    Else
                        'Concession is Nothing or Empty
                        _missingParamErrorCode = "CO"
                        missingParam = True
                    End If
                Else
                    'Customer is Nothing or Empty
                    _missingParamErrorCode = "CU"
                    missingParam = True
                End If
            Else
                exitLoop = True
            End If
            If missingParam Then
                exitLoop = True
            End If
            count += 1
        End While

        Return missingParam

    End Function

    Private Function GetPartPaymentTotalFee() As Decimal
        Dim partPayTotalFee As Decimal = 0
        Dim dtPartPayments As New DataTable
        Dim err As New Talent.Common.ErrorObj
        Dim talPayment As New Talent.Common.TalentPayment
        Dim paymentEntity As New Talent.Common.DEPayments
        talPayment.Settings = TEBUtilities.GetSettingsObject
        With paymentEntity
            .SessionId = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
            .CustomerNumber = talPayment.Settings.LoginId
        End With
        talPayment.De = paymentEntity
        err = talPayment.RetrievePartPayments

        ' Was the call successful
        If Not err.HasError AndAlso
            Not talPayment.ResultDataSet Is Nothing AndAlso
            talPayment.ResultDataSet.Tables.Count = 2 AndAlso
            talPayment.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") <> GlobalConstants.ERRORFLAG Then
            dtPartPayments = talPayment.ResultDataSet.Tables("PartPayments")
            For Each dr As DataRow In dtPartPayments.Rows
                partPayTotalFee += CType(dr.Item("FeeValue"), Decimal)
            Next
        End If
        Return partPayTotalFee
    End Function

    Public Function GetProductDetailQueryString(ByVal includeQuerystring As Boolean) As String
        Dim queryStrings As String = String.Empty
        If includeQuerystring AndAlso (Not (String.IsNullOrEmpty(HttpContext.Current.Request.QueryString("productsubtype")))) Then
            queryStrings = "?productsubtype=" & HttpContext.Current.Request.QueryString("productsubtype")
            If Not (HttpContext.Current.Session("ProductDetailCurrentPage") Is Nothing) Then
                queryStrings = queryStrings & "&" & GetProductDetailCurrentPage()
            End If
        Else
            If Not (HttpContext.Current.Session("ProductDetailCurrentPage") Is Nothing) Then
                queryStrings = queryStrings & "?" & GetProductDetailCurrentPage()
            End If
        End If
        Return queryStrings
    End Function

    Private Function GetProductDetailCurrentPage() As String
        Dim currentPage As String = String.Empty
        currentPage = "Page=" & HttpContext.Current.Session("ProductDetailCurrentPage")
        HttpContext.Current.Session("ProductDetailCurrentPage") = Nothing
        HttpContext.Current.Session.Remove("ProductDetailCurrentPage")
        Return currentPage
    End Function

    ''' <summary>
    ''' Get the list of PDF attachments for the current basket based on the hospitality packages being purchased at the end of sale only
    ''' </summary>
    ''' <param name="ticketingPaymentReference">The ticketing payment reference to find basket records for</param>
    ''' <returns>A generic list of string values representing a path to each PDF document</returns>
    ''' <remarks></remarks>
    Public Function GetHospitalityPDFAttachmentList(Optional ByVal ticketingPaymentReference As String = "") As List(Of String)
        Dim pdfAttachments As New List(Of String)
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults
        Dim basket As TalentBasket = CType(HttpContext.Current.Profile, TalentProfile).Basket

        If moduleDefaults.HospitalityPDFAttachmentsOnConfirmationEmail Then
            If basket.BasketItems.Count > 0 Then
                For Each item As TalentBasketItem In basket.BasketItems
                    If item.PACKAGE_ID > 0 Then
                        Dim htmlContent As String = String.Empty
                        Dim cssContent As String = String.Empty
                        Dim fileName As String = String.Concat(item.CALL_ID.ToString(), "-", Now.ToString("ddMMyyyy-HHmm"), ".pdf")
                        Dim pdfCreator As New CreatePDF
                        Dim pdfPathAndFile As String = String.Empty
                        Dim leadSourceList As List(Of LeadSourceDetails) = Nothing
                        Dim filePath As String = moduleDefaults.HtmlPathAbsolute
                        If Not filePath.EndsWith("\") Then filePath &= "\"
                        filePath &= "HospitalityPDF\"
                        htmlContent &= ProcessHospitalityDetailsView(item.Product, item.PACKAGE_ID, item.CALL_ID, leadSourceList)
                        htmlContent &= ProcessHospitalityBookingView(basket.Basket_Header_ID, item.LOGINID, item.Product, item.PACKAGE_ID, item.CALL_ID, leadSourceList)
                        cssContent = Talent.eCommerce.Utilities.GetCSSContentFromFile("HospitalityPDF\" & item.PACKAGE_ID & "_package.css")
                        If cssContent.Length = 0 Then cssContent = Talent.eCommerce.Utilities.GetCSSContentFromFile("HospitalityPDF\package.css")
                        pdfPathAndFile = pdfCreator.CreateFile(fileName, filePath, htmlContent, cssContent)
                        pdfAttachments.Add(pdfPathAndFile)
                    End If
                Next
            Else
                If ticketingPaymentReference.Length > 0 Then
                    Dim talDataObjects As New TalentDataObjects
                    Dim basketDetails As New DataSet
                    talDataObjects.Settings = TEBUtilities.GetSettingsObject()
                    basketDetails = talDataObjects.BasketSettings.GetBasketRecordsByTicketingPaymentReference(ticketingPaymentReference)
                    If basketDetails IsNot Nothing AndAlso basketDetails.Tables.Count > 0 AndAlso basketDetails.Tables(0).Rows.Count > 0 Then
                        For Each row As DataRow In basketDetails.Tables(0).Rows
                            Dim packageId As String = TEBUtilities.CheckForDBNull_String(row("PACKAGE_ID"))
                            If TCUtilities.CheckForDBNull_BigInt(packageId) > 0 Then
                                Dim product As String = TEBUtilities.CheckForDBNull_String(row("PRODUCT"))
                                Dim customerNumber As String = TEBUtilities.CheckForDBNull_String(row("LOGINID"))
                                Dim callId As String = TEBUtilities.CheckForDBNull_Long(row("CALL_ID"))
                                Dim htmlContent As String = String.Empty
                                Dim cssContent As String = String.Empty
                                Dim fileName As String = String.Concat(callId, "-", Now.ToString("ddMMyyyy-HHmm"), ".pdf")
                                Dim pdfCreator As New CreatePDF
                                Dim pdfPathAndFile As String = String.Empty
                                Dim leadSourceList As List(Of LeadSourceDetails) = Nothing
                                Dim filePath As String = moduleDefaults.HtmlPathAbsolute
                                If Not filePath.EndsWith("\") Then filePath &= "\"
                                filePath &= "HospitalityPDF\"
                                htmlContent &= ProcessHospitalityDetailsView(product, packageId, callId, leadSourceList)
                                htmlContent &= ProcessHospitalityBookingView(basket.Basket_Header_ID, customerNumber, product, packageId, callId, leadSourceList)
                                cssContent = Talent.eCommerce.Utilities.GetCSSContentFromFile("HospitalityPDF\" & packageId & "_package.css")
                                If cssContent.Length = 0 Then cssContent = Talent.eCommerce.Utilities.GetCSSContentFromFile("HospitalityPDF\package.css")
                                pdfPathAndFile = pdfCreator.CreateFile(fileName, filePath, htmlContent, cssContent)
                                pdfAttachments.Add(pdfPathAndFile)
                            End If
                        Next
                    End If
                End If
            End If
        End If
        Return pdfAttachments
    End Function

    ''' <summary>
    ''' Process the hospitality details view model for the current package and product being purchased
    ''' </summary>
    ''' <param name="product">The ticketing product code</param>
    ''' <param name="packageId">The hospitality package Id</param>
    ''' <param name="callId">The hospitality booking reference</param>
    ''' <param name="leadSourceList">The object to populate with the lead source details</param>
    ''' <returns>A formatted HTML string from the partial view of the view model</returns>
    ''' <remarks></remarks>
    Public Function ProcessHospitalityDetailsView(ByRef product As String, ByRef packageId As String, ByRef callId As String, ByRef leadSourceList As List(Of LeadSourceDetails)) As String
        Dim viewRenderer As New WebFormMVCUtil
        Dim detailsInputModel As New HospitalityDetailsInputModel
        Dim detailsViewModel As New HospitalityDetailsViewModel
        Dim detailsBuilder As New HospitalityDetailsBuilder
        Dim writer As New StringWriter
        Dim content As String = String.Empty
        Dim talProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
        Dim address As TalentProfileAddress = ProfileHelper.ProfileAddressEnumerator(0, talProfile.User.Addresses)
        Dim addressStringBuilder As New StringBuilder

        detailsInputModel.ProductCode = product
        detailsInputModel.PackageID = packageId
        detailsViewModel = detailsBuilder.GetProductPackageDetails(detailsInputModel)
        If Not detailsViewModel.Error.HasError Then
            detailsViewModel.CallId = callId
            If address.Address_Line_1.Length > 0 Then addressStringBuilder.Append(address.Address_Line_1)
            If address.Address_Line_2.Length > 0 Then
                If addressStringBuilder.Length > 0 Then addressStringBuilder.Append(", ")
                addressStringBuilder.Append(address.Address_Line_2)
            End If
            If address.Address_Line_3.Length > 0 Then
                If addressStringBuilder.Length > 0 Then addressStringBuilder.Append(", ")
                addressStringBuilder.Append(address.Address_Line_3)
            End If
            If address.Address_Line_4.Length > 0 Then
                If addressStringBuilder.Length > 0 Then addressStringBuilder.Append(", ")
                addressStringBuilder.Append(address.Address_Line_4)
            End If
            If address.Address_Line_5.Length > 0 Then
                If addressStringBuilder.Length > 0 Then addressStringBuilder.Append(", ")
                addressStringBuilder.Append(address.Address_Line_5)
            End If
            If address.Post_Code.Length > 0 Then
                If addressStringBuilder.Length > 0 Then addressStringBuilder.Append(", ")
                addressStringBuilder.Append(address.Post_Code)
            End If
            detailsViewModel.CustomerAddress = addressStringBuilder.ToString()
            detailsViewModel.CustomerName = talProfile.User.Details.Full_Name
            detailsViewModel.CompanyName = talProfile.User.Details.CompanyName
            detailsViewModel.MobileNumber = talProfile.User.Details.Mobile_Number
            detailsViewModel.HomeNumber = talProfile.User.Details.Telephone_Number
            detailsViewModel.WorkNumber = talProfile.User.Details.Work_Number
            leadSourceList = detailsViewModel.LeadSourceDetails
            Try
                writer = viewRenderer.RenderPartial("~/Views/PartialViews/_HospitalityDetails.cshtml", detailsViewModel)
            Catch ex As Exception
                Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "processHospitalityDetailsView-Error", ex.StackTrace, ex.Message)
            End Try
        End If
        content = writer.GetStringBuilder().ToString()
        writer.Dispose()
        Return content
    End Function

    ''' <summary>
    ''' Process the hospitality booking view model for the current package and product being purchased
    ''' </summary>
    ''' <param name="basketId">The current basket Id which is required when retrieving Q and A</param>
    ''' <param name="customerNumber">The customer number for the booking</param>
    ''' <param name="product">The ticketing product code</param>
    ''' <param name="packageId">The hospitality package Id</param>
    ''' <param name="callId">The hospitality booking reference</param>
    ''' <param name="leadSourceList">The lead source list object used to populate the selected lead source description</param>
    ''' <returns>A formatted HTML string from the partial view of the view model</returns>
    ''' <remarks></remarks>
    Public Function ProcessHospitalityBookingView(ByRef basketId As String, ByRef customerNumber As String, ByRef product As String, ByRef packageId As String, ByRef callId As String, ByRef leadSourceList As List(Of LeadSourceDetails)) As String
        Dim viewRenderer As New WebFormMVCUtil
        Dim bookingInputModel As New HospitalityBookingInputModel
        Dim bookingViewModel As New HospitalityBookingViewModel
        Dim bookingBuilder As New HospitalityBookingBuilder
        Dim writer As New StringWriter
        Dim content As String = String.Empty
        bookingInputModel.BasketID = basketId
        bookingInputModel.CustomerNumber = customerNumber
        bookingInputModel.ProductCode = product
        bookingInputModel.PackageID = packageId
        bookingInputModel.CallId = callId
        bookingInputModel.IsSoldBooking = True
        bookingViewModel = bookingBuilder.RetrieveHospitalityBooking(bookingInputModel)
        If Not bookingViewModel.Error.HasError Then
            Try
                bookingViewModel.PaymentOwnerCustomerForename = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Forename
                bookingViewModel.PaymentOwnerCustomerSurname = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Surname
                If bookingViewModel.PackageDetailsList(0).LeadSourceID > 0 Then
                    If leadSourceList.Exists(Function(x) x.LeadSourceID = bookingViewModel.PackageDetailsList(0).LeadSourceID) Then
                        bookingViewModel.PackageDetailsList(0).LeadSourceDescription = leadSourceList.Find(Function(x) x.LeadSourceID = bookingViewModel.PackageDetailsList(0).LeadSourceID).LeadSourceDescription
                    End If
                End If
                writer = viewRenderer.RenderPartial("~/Views/PartialViews/_HospitalityBooking.cshtml", bookingViewModel)
            Catch ex As Exception
                Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "processHospitalityBookingView-Error", ex.StackTrace, ex.Message)
            End Try
        End If
        content = writer.GetStringBuilder().ToString()
        writer.Dispose()
        Return content
    End Function

#End Region


End Class