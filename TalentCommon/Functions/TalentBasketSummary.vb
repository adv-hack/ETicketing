Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports Talent.Common.TalentBasketFees
Imports Talent.Common.Utilities
Public Class TalentBasketSummary
    Inherits TalentBase

    Public Sub New()

    End Sub

    'basket summary display controller
    Private _basketSummaryEntity As DEBasketSummary = Nothing
    Private _dtBasketHeader As DataTable = Nothing
    Private _dtBasketDetail As DataTable = Nothing
    Private _dtBasketDetailExceptions As DataTable = Nothing
    Private _dtBasketFees As DataTable = Nothing
    Public Property ResultDataSet As DataSet
    Public Property DeliveryChargeEntity() As DEDeliveryCharges.DEDeliveryCharge = Nothing
    Public Property BasketEntity() As DEBasket = Nothing
    Public Property BasketItemEntityList() As List(Of DEBasketItem) = Nothing
    Public Property TalentApplicationType() As String = Nothing
    Public Property WebPrices() As TalentWebPricing = Nothing
    Public Property LoginID As String = String.Empty
    Public Property IsAuthenticated As Boolean = False
    Public Property IsUpdatedBasketProcess As Boolean = False

    Public ReadOnly Property BasketSummaryEntity As DEBasketSummary
        Get
            Return _basketSummaryEntity
        End Get
    End Property

    ''' <summary>
    ''' Creates the basket entity, basket detail entity, basket summary entity
    ''' </summary>
    Public Function GetBasket(ByVal basketHeaderID As String, ByVal canProcessBookingFees As Boolean) As ErrorObj
        Dim errObj As New ErrorObj
        Try
            If canProcessBookingFees Then
                ProcessBasketForBookingFees(basketHeaderID)
            End If
            ResultDataSet = New DataSet
            PopulateBasketRelatedDataTable(basketHeaderID)
            _dtBasketHeader.TableName = "BasketHeader"
            _dtBasketDetail.TableName = "BasketDetail"
            _dtBasketDetailExceptions.TableName = "BasketDetailExceptions"
            ResultDataSet.Tables.Add(_dtBasketHeader.Copy)
            ResultDataSet.Tables.Add(_dtBasketDetail.Copy)
            ResultDataSet.Tables.Add(_dtBasketDetailExceptions.Copy)
            BasketEntity = New DEBasket
            Dim properties As ArrayList = Utilities.GetPropertyNames(BasketEntity)
            BasketEntity = PopulateProperties(properties, _dtBasketHeader, BasketEntity, 0)
            Dim basketItemEntity As DEBasketItem = Nothing
            basketItemEntity = New DEBasketItem
            properties = Utilities.GetPropertyNames(basketItemEntity)
            If _dtBasketDetail.Rows.Count > 0 Then
                For rowIndex As Integer = 0 To _dtBasketDetail.Rows.Count - 1
                    basketItemEntity = New DEBasketItem
                    basketItemEntity = PopulateProperties(properties, _dtBasketDetail.Rows(rowIndex), basketItemEntity)
                    BasketEntity.BasketItems.Add(basketItemEntity)
                Next
            End If
            PopulateBasketSummary(basketHeaderID)
        Catch ex As Exception
            errObj.HasError = True
            errObj.ErrorNumber = "TACTBS-001"
            errObj.ErrorMessage = "Error while getting the basket detail. " & ex.Message
        End Try
        Return errObj
    End Function

    Public Function UpdateBasketPayTypeOrCardType(ByVal basketHeaderID As String, ByVal existingPaymentType As String, ByVal existingCardTypeCode As String, ByVal currentPaymentType As String, ByVal currentCardTypeCode As String, ByVal isToUpdatePayTypeInBsktHeader As Boolean, ByVal canProcessPartPayment As Boolean) As Boolean
        Dim isUpdated As Boolean = False
        Dim talBasketFeesProcessor As New TalentBasketFeesProcessor
        talBasketFeesProcessor.Settings = Settings
        talBasketFeesProcessor.CardTypeFeeCategory = CardTypeFeeCategory
        talBasketFeesProcessor.FulfilmentFeeCategory = FulfilmentFeeCategory
        talBasketFeesProcessor.DeliveryChargeEntity = DeliveryChargeEntity
        If talBasketFeesProcessor.UpdateBasketFeesByPaymentType(basketHeaderID, LoginID, existingPaymentType, existingCardTypeCode, currentPaymentType, currentCardTypeCode, False, isToUpdatePayTypeInBsktHeader, canProcessPartPayment) Then
            isUpdated = True
        End If
        Return isUpdated
    End Function

    Public Function UpdateBasketPayTypeOrCardType(ByVal basketHeaderID As String, ByVal currentPaymentType As String, ByVal currentCardTypeCode As String, ByVal isToUpdatePayTypeInBsktHeader As Boolean, ByVal canProcessPartPayment As Boolean) As Boolean
        Dim isUpdated As Boolean = False
        _dtBasketHeader = TDataObjects.BasketSettings.TblBasketHeader.GetHeaderByBasketHeaderID(basketHeaderID)
        If _dtBasketHeader IsNot Nothing AndAlso _dtBasketHeader.Rows.Count > 0 Then
            Dim talBasketFeesProcessor As New TalentBasketFeesProcessor
            talBasketFeesProcessor.Settings = Settings
            talBasketFeesProcessor.CardTypeFeeCategory = CardTypeFeeCategory
            talBasketFeesProcessor.FulfilmentFeeCategory = FulfilmentFeeCategory
            talBasketFeesProcessor.DeliveryChargeEntity = DeliveryChargeEntity
            If talBasketFeesProcessor.UpdateBasketFeesByPaymentType(basketHeaderID, LoginID, Utilities.CheckForDBNull_String(_dtBasketHeader.Rows(0)("PAYMENT_TYPE")), Utilities.CheckForDBNull_String(_dtBasketHeader.Rows(0)("CARD_TYPE_CODE")), currentPaymentType, currentCardTypeCode, False, isToUpdatePayTypeInBsktHeader, canProcessPartPayment) Then
                isUpdated = True
            End If
        End If
        Return isUpdated
    End Function

    Friend Function ProcessBasketForSummary() As ErrorObj
        Dim errObj As New ErrorObj

        'Fees Processing
        Dim talBasketFeesProcessor As New TalentBasketFeesProcessor
        talBasketFeesProcessor.Settings = Settings
        talBasketFeesProcessor.BasketEntity = BasketEntity
        talBasketFeesProcessor.BasketItemEntityList = BasketEntity.BasketItems
        If CardTypeFeeCategory Is Nothing OrElse CardTypeFeeCategory.Count <= 0 Then
            CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(Settings.BusinessUnit)
        End If
        If FulfilmentFeeCategory Is Nothing OrElse FulfilmentFeeCategory.Count <= 0 Then
            FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(Settings.BusinessUnit)
        End If
        talBasketFeesProcessor.CardTypeFeeCategory = CardTypeFeeCategory
        talBasketFeesProcessor.FulfilmentFeeCategory = FulfilmentFeeCategory
        talBasketFeesProcessor.IsUpdatedBasketProcess = IsUpdatedBasketProcess
        talBasketFeesProcessor.DeliveryChargeEntity = DeliveryChargeEntity
        errObj = talBasketFeesProcessor.ProcessBasketForFees

        'Future Processing are
        'Promotion Processing
        'Validation Processing
        'Product Pricing

        Return errObj
    End Function

    Private Sub PopulateBasketRelatedDataTable(ByVal basketHeaderID As String)
        _dtBasketHeader = TDataObjects.BasketSettings.TblBasketHeader.GetHeaderByBasketHeaderID(basketHeaderID)
        _dtBasketDetail = TDataObjects.BasketSettings.TblBasketDetail.GetDetailByBasketHeaderID(basketHeaderID)
        _dtBasketFees = TDataObjects.BasketSettings.TblBasketFees.GetFeesByBasketHeaderID(basketHeaderID)
        _dtBasketDetailExceptions = TDataObjects.BasketSettings.TblBasketDetailExceptions.GetByBasketDetailHeaderIDAndModule(basketHeaderID, GlobalConstants.BASKETMODULETICKETING)
    End Sub

    Private Sub PopulateBasketSummary(ByVal basketHeaderID As String)

        _basketSummaryEntity = New DEBasketSummary
        If _dtBasketHeader IsNot Nothing AndAlso _dtBasketHeader.Rows.Count > 0 Then
            If _dtBasketDetail IsNot Nothing AndAlso _dtBasketDetail.Rows.Count > 0 Then
                'now call all the fields to populate
                PopulateBasketSummaryDisplay()
                PopulateBasketSummaryOtherFeesDisplay()
                PopulateBasketSummaryFeeValueActual()
                ValidateBasketSummaryForError()
            End If
        End If
    End Sub

    Private Sub PopulateBasketSummaryOtherFeesDisplay()
        If _dtBasketFees IsNot Nothing AndAlso _dtBasketFees.Rows.Count > 0 Then
            For rowIndex As Integer = 0 To _dtBasketFees.Rows.Count - 1
                _basketSummaryEntity.AddToBasketSummaryFees(Utilities.CheckForDBNull_String(_dtBasketFees.Rows(rowIndex)("CARD_TYPE_CODE")),
                                                            Utilities.CheckForDBNull_String(_dtBasketFees.Rows(rowIndex)("FEE_CODE")),
                                                            Utilities.CheckForDBNull_String(_dtBasketFees.Rows(rowIndex)("FEE_DESCRIPTION")),
                                                            Utilities.CheckForDBNull_String(_dtBasketFees.Rows(rowIndex)("FEE_CATEGORY")),
                                                            Utilities.CheckForDBNull_Decimal(_dtBasketFees.Rows(rowIndex)("FEE_VALUE")),
                                                            Utilities.CheckForDBNull_Boolean_DefaultFalse(_dtBasketFees.Rows(rowIndex)("IS_SYSTEM_FEE")),
                                                            Utilities.CheckForDBNull_Boolean_DefaultFalse(_dtBasketFees.Rows(rowIndex)("IS_TRANSACTIONAL")),
                                                            _basketSummaryEntity.FeesDICNonSystemInDetail.ContainsKey(Utilities.CheckForDBNull_String(_dtBasketFees.Rows(rowIndex)("FEE_CODE"))))
            Next
        End If

        Dim dtsummaryfees As DataTable = _basketSummaryEntity.SummaryFeesTable

        dtsummaryfees.DefaultView.RowFilter = "FEE_CATEGORY = '" & GlobalConstants.FEECATEGORY_CHARITY & "'"
        _basketSummaryEntity.FeesDTCharity = dtsummaryfees.DefaultView.ToTable

        dtsummaryfees.DefaultView.RowFilter = ""
        dtsummaryfees.DefaultView.RowFilter = "FEE_CATEGORY = '" & GlobalConstants.FEECATEGORY_BOOKING & "'"
        _basketSummaryEntity.FeesDTCardTypeBooking = dtsummaryfees.DefaultView.ToTable

        dtsummaryfees.DefaultView.RowFilter = ""
        dtsummaryfees.DefaultView.RowFilter = "FEE_CATEGORY = '" & GlobalConstants.FEECATEGORY_ADHOC & "'"
        _basketSummaryEntity.FeesDTAdhoc = dtsummaryfees.DefaultView.ToTable

        If Settings.IsAgent Then
            dtsummaryfees.DefaultView.RowFilter = ""
            dtsummaryfees.DefaultView.RowFilter = "FEE_CATEGORY = '" & GlobalConstants.FEECATEGORY_VARIABLE & "' AND IS_EXISTS_IN_BASKET = 0 "
            _basketSummaryEntity.FeesDTVariable = dtsummaryfees.DefaultView.ToTable

            _dtBasketDetail.DefaultView.RowFilter = ""
            _dtBasketDetail.DefaultView.RowFilter = "FEE_CATEGORY = '" & GlobalConstants.FEECATEGORY_VARIABLE & "'"
            _basketSummaryEntity.FeesDTVariableApplied = _dtBasketDetail.DefaultView.ToTable
        End If

        dtsummaryfees.DefaultView.RowFilter = ""

        _dtBasketDetail.DefaultView.RowFilter = ""

    End Sub

    Private Sub PopulateBasketSummaryDisplay()

        'totals amounts
        Dim totalBasket As Decimal = 0,
        totalBasketWithoutPayTypeFees As Decimal = 0,
        totalPayTypeFeeInDetail As Decimal = 0,
        totalMerchandise As Decimal = 0,
        totalTicketingFees As Decimal = 0,
        totalTicketPrice As Decimal = 0,
        totalTicketing As Decimal = 0,
        totalTicketingPayTypeFee As Decimal = 0,
        totalTicketingOnAccount As Decimal = 0,
        totalTicketDiscountTotal As Decimal = 0,
        totalTicketingBuyBack As Decimal = 0,
        totalTicketingPartPayment As Decimal = 0,
        totalTicketingPartPaymentByCC As Decimal = 0,
        totalTicketingPartPaymentByOthers As Decimal = 0,
        totalTicketingPartPaymentFeesValue As Decimal = 0,
        totalTicketingExceptionSeatValue As Decimal = 0

        'total items
        Dim totalItemsTicketing As Integer = 0,
        totalItemsAppliedCharityFee As Integer = 0,
        totalItemsAppliedAdhocFee As Integer = 0,
        totalItemsAppliedVariableFee As Integer = 0,
        totalItemsTicketingDiscount As Integer = 0,
        totalItemsMerchandise As Integer = 0

        'fees
        Dim feesAdhocTotal As Decimal = 0,
        feesAdhocShow As Boolean = False,
        feesCharityTotal As Decimal = 0,
        feesCharityShow As Boolean = False,
        feesVariableTotal As Decimal = 0,
        feesVariableShow As Boolean = False

        'others
        Dim isDisplayType As Boolean = False,
        sequence As Integer = 100,
        canAddToFeesSummary = False,
        canAddToBasketSummary = False,
        displayLabelCode As String = "",
        tempGrossPrice As Decimal = 0
        Dim canAddPartPaymentFee As Boolean = False
        Dim isOrderConfirmation As Boolean = False
        ProcessPartPaymentAmount(isOrderConfirmation, totalTicketingPartPayment, totalTicketingPartPaymentByCC, totalTicketingPartPaymentByOthers, totalTicketingPartPaymentFeesValue)
        totalMerchandise = ProcessMerchandiseAndGetTotal()
        totalTicketingExceptionSeatValue = ProcessSTExceptionsSeatUpgradesTotal()

        Dim pdtGrossPrice As Decimal = 0
        For rowIndex As Integer = 0 To _dtBasketDetail.Rows.Count - 1
            pdtGrossPrice = 0
            If Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(rowIndex)("MODULE")).Trim.ToUpper = GlobalConstants.BASKETMODULETICKETING.ToUpper Then

                canAddToFeesSummary = False
                canAddToBasketSummary = False
                'is it fee
                If Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(rowIndex)("FEE_CATEGORY")).Trim.Length > 0 Then

                    tempGrossPrice = 0
                    canAddToBasketSummary = True
                    isDisplayType = False

                    If Utilities.CheckForDBNull_Boolean_DefaultFalse(_dtBasketDetail.Rows(rowIndex)("IS_INCLUDED")) Then
                        tempGrossPrice = Utilities.CheckForDBNull_Decimal(_dtBasketDetail.Rows(rowIndex)("GROSS_PRICE"))
                    End If
                    If Not Utilities.CheckForDBNull_Boolean_DefaultFalse(_dtBasketDetail.Rows(rowIndex)("IS_SYSTEM_FEE")) Then
                        _basketSummaryEntity.AddToNonSystemFeeInDetail(Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(rowIndex)("PRODUCT")), Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(rowIndex)("FEE_CATEGORY")))
                    End If

                    totalTicketingFees += tempGrossPrice

                    Select Case Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(rowIndex)("FEE_CATEGORY"))
                        Case GlobalConstants.FEECATEGORY_CHARITY
                            displayLabelCode = Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(rowIndex)("FEE_CATEGORY"))
                            feesCharityShow = True
                            feesCharityTotal += tempGrossPrice
                            isDisplayType = False
                            totalItemsAppliedCharityFee += 1
                        Case GlobalConstants.FEECATEGORY_ADHOC
                            displayLabelCode = Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(rowIndex)("FEE_CATEGORY"))
                            feesAdhocShow = True
                            feesAdhocTotal += tempGrossPrice
                            isDisplayType = False
                            totalItemsAppliedAdhocFee += 1
                        Case GlobalConstants.FEECATEGORY_VARIABLE
                            displayLabelCode = Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(rowIndex)("FEE_CATEGORY"))
                            feesVariableShow = True
                            feesVariableTotal += tempGrossPrice
                            isDisplayType = False
                            totalItemsAppliedVariableFee += 1
                        Case GlobalConstants.FEECATEGORY_BOOKING
                            totalPayTypeFeeInDetail += Utilities.CheckForDBNull_Decimal(_dtBasketDetail.Rows(rowIndex)("GROSS_PRICE"))
                            totalTicketingPayTypeFee += tempGrossPrice
                            displayLabelCode = Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(rowIndex)("FEE_CATEGORY"))
                            sequence = GlobalConstants.BSKTSMRY_DSPLY_SEQ_BOOKING
                            isDisplayType = True
                        Case GlobalConstants.FEECATEGORY_DIRECTDEBIT
                            totalTicketingPayTypeFee += tempGrossPrice
                            totalPayTypeFeeInDetail += Utilities.CheckForDBNull_Decimal(_dtBasketDetail.Rows(rowIndex)("GROSS_PRICE"))
                            displayLabelCode = Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(rowIndex)("FEE_CATEGORY"))
                            sequence = GlobalConstants.BSKTSMRY_DSPLY_SEQ_DIRECTDEBIT
                            isDisplayType = True
                            canAddToFeesSummary = True
                        Case GlobalConstants.FEECATEGORY_FINANCE
                            totalTicketingPayTypeFee += tempGrossPrice
                            totalPayTypeFeeInDetail += Utilities.CheckForDBNull_Decimal(_dtBasketDetail.Rows(rowIndex)("GROSS_PRICE"))
                            displayLabelCode = Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(rowIndex)("FEE_CATEGORY"))
                            sequence = GlobalConstants.BSKTSMRY_DSPLY_SEQ_FINANCE
                            isDisplayType = True
                            canAddToFeesSummary = True
                        Case GlobalConstants.FEECATEGORY_WEBSALES
                            displayLabelCode = Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(rowIndex)("FEE_CATEGORY"))
                            sequence = GlobalConstants.BSKTSMRY_DSPLY_SEQ_WEBSALES
                            isDisplayType = True
                            canAddToFeesSummary = True
                        Case GlobalConstants.FEECATEGORY_BUYBACK
                            displayLabelCode = Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(rowIndex)("FEE_CATEGORY"))
                            sequence = GlobalConstants.BSKTSMRY_DSPLY_SEQ_BUYBACK
                            totalTicketingBuyBack += tempGrossPrice
                            totalTicketingFees -= tempGrossPrice
                            isDisplayType = True
                        Case Else
                            displayLabelCode = Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(rowIndex)("FEE_CATEGORY"))
                            isDisplayType = True
                            sequence = GlobalConstants.BSKTSMRY_DSPLY_SEQ_FEES_DEFAULT
                            canAddToFeesSummary = True
                    End Select

                    'on account (cashback)
                ElseIf Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(rowIndex)("PRODUCT")).Trim = Settings.EcommerceModuleDefaultsValues.CashBackFeeCode Then
                    displayLabelCode = Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(rowIndex)("PRODUCT"))
                    sequence = GlobalConstants.BSKTSMRY_DSPLY_SEQ_ONACCOUNT
                    canAddToBasketSummary = True
                    isDisplayType = True
                    totalTicketingOnAccount += Utilities.CheckForDBNull_Decimal(_dtBasketDetail.Rows(rowIndex)("GROSS_PRICE"))
                Else
                    'is this product is package or game
                    If Utilities.CheckForDBNull_BigInt(_dtBasketDetail.Rows(rowIndex)("PACKAGE_ID")) > 0 _
                        AndAlso Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(rowIndex)("PRODUCT_TYPE_ACTUAL")).Trim.ToUpper = GlobalConstants.PACKAGEPRODUCTTYPE Then
                        totalItemsTicketing += Utilities.CheckForDBNull_Int(_dtBasketDetail.Rows(rowIndex)("QUANTITY"))
                    ElseIf Utilities.CheckForDBNull_BigInt(_dtBasketDetail.Rows(rowIndex)("BULK_SALES_ID")) > 0 Then
                        totalItemsTicketing += Utilities.CheckForDBNull_Int(_dtBasketDetail.Rows(rowIndex)("QUANTITY"))
                    Else
                        totalItemsTicketing += 1
                    End If

                    If Utilities.CheckForDBNull_Decimal(_dtBasketDetail.Rows(rowIndex)("ORIGINAL_PRICE")) = 0 Then
                        totalTicketPrice += Utilities.CheckForDBNull_Decimal(_dtBasketDetail.Rows(rowIndex)("GROSS_PRICE"))
                    Else
                        totalItemsTicketingDiscount += 1
                        totalTicketPrice += Utilities.CheckForDBNull_Decimal(_dtBasketDetail.Rows(rowIndex)("ORIGINAL_PRICE"))
                        totalTicketDiscountTotal += (Utilities.CheckForDBNull_Decimal(_dtBasketDetail.Rows(rowIndex)("GROSS_PRICE")) - Utilities.CheckForDBNull_Decimal(_dtBasketDetail.Rows(rowIndex)("ORIGINAL_PRICE")))
                    End If

                End If
                pdtGrossPrice = Utilities.CheckForDBNull_Decimal(_dtBasketDetail.Rows(rowIndex)("GROSS_PRICE"))

                If canAddToBasketSummary Then
                    _basketSummaryEntity.AddToBasketSummary(displayLabelCode,
                                                                            Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(rowIndex)("PRODUCT")),
                                                                            Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(rowIndex)("FEE_CATEGORY")),
                                                                            pdtGrossPrice,
                                                                            Utilities.CheckForDBNull_Decimal(_dtBasketDetail.Rows(rowIndex)("NET_PRICE")),
                                                                            Utilities.CheckForDBNull_Decimal(_dtBasketDetail.Rows(rowIndex)("TAX_PRICE")),
                                                                            Utilities.CheckForDBNull_Boolean_DefaultFalse(_dtBasketDetail.Rows(rowIndex)("IS_SYSTEM_FEE")),
                                                                            Utilities.CheckForDBNull_Boolean_DefaultFalse(_dtBasketDetail.Rows(rowIndex)("IS_EXTERNAL")),
                                                                            Utilities.CheckForDBNull_Boolean_DefaultFalse(_dtBasketDetail.Rows(rowIndex)("IS_INCLUDED")),
                                                                            isDisplayType, sequence)
                End If

                If canAddToFeesSummary Then
                    _basketSummaryEntity.AddToBasketSummaryFees("",
                                                                    Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(rowIndex)("PRODUCT")),
                                                                    Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(rowIndex)("PRODUCT_DESCRIPTION1")),
                                                                    Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(rowIndex)("FEE_CATEGORY")),
                                                                    pdtGrossPrice,
                                                                    Utilities.CheckForDBNull_Boolean_DefaultFalse(_dtBasketDetail.Rows(rowIndex)("IS_SYSTEM_FEE")),
                                                                    Utilities.CheckForDBNull_Boolean_DefaultFalse(_dtBasketDetail.Rows(rowIndex)("IS_TRANSACTIONAL")),
                                                                    True)

                End If
            Else
                'this product is merchandise
                totalItemsMerchandise += Utilities.CheckForDBNull_Int(_dtBasketDetail.Rows(rowIndex)("QUANTITY"))
            End If
        Next

        'totalTicketPrice - only ticket price
        'totalTicketingFees - fees include adhoc, variable, charity, pay type fees
        'totalTicketingPayTypeFee - fees include booking, direct debit, finance
        'totalTicketingPartPayment - part payment epurse etc
        'totalTicketingOnAccount - onaccount alias cashback
        'totalTicketingExceptionSeatValue - season ticket exceptions seat upgrade price (this is applied when buying a season ticket and one of the seats in the season ticket has an added value on it)

        Dim totalTicketingFeesWithoutPayTypeFee As Decimal = totalTicketingFees - totalTicketingPayTypeFee
        'total ticket price after cat ticket price if basket in cat mode
        totalTicketPrice = ProcessCATAndGetTicketPriceAfterCAT(totalTicketPrice)
        'total basket without pay type fee
        totalBasket = totalTicketPrice + totalTicketingFeesWithoutPayTypeFee
        'total basket including discounts (rem. totalTicketDiscountTotal will be a negative and is therefore added to the total)
        totalBasket = totalBasket + totalTicketDiscountTotal
        'total basket after part payment by others
        totalBasket -= totalTicketingPartPaymentByOthers
        'total Basket after onaccount cashback, 
        'cashback is already added as negative amount in tbl basket detail
        'so just include that amount to total basket
        totalBasket += totalTicketingOnAccount
        'total Basket after buyback, 
        'buyback is already added as negative amount in tbl basket detail
        'so just include that amount to total basket
        totalBasket += totalTicketingBuyBack
        'total Basket with merchandise
        totalBasket += totalMerchandise
        'total basket with any season ticket excceptions seat upgrades
        totalBasket += totalTicketingExceptionSeatValue
        'so now total basket has merchandise, ticket price, part payment, onaccount, all fees including refund fees except pay type fee
        totalBasketWithoutPayTypeFees = totalBasket
        'now decide whether pay type required or not
        If (totalBasket > 0 OrElse Utilities.CheckForDBNull_String(_dtBasketHeader.Rows(0)("CAT_MODE")).Trim = GlobalConstants.CATMODE_CANCEL OrElse Utilities.CheckForDBNull_String(_dtBasketHeader.Rows(0)("CAT_MODE")).Trim = GlobalConstants.CATMODE_CANCELALL) Then
            totalBasket += totalTicketingPayTypeFee
        Else
            If (totalBasket = 0 AndAlso totalTicketingPayTypeFee > 0) OrElse (totalBasket <= 0 AndAlso Utilities.CheckForDBNull_String(_dtBasketHeader.Rows(0)("CAT_MODE")).Trim.Length > 0) Then
                'as pay type fee is not considered remove from basket detail
                If TDataObjects.BasketSettings.TblBasketDetail.DeleteAllPayTypeFees(_dtBasketHeader.Rows(0)("BASKET_HEADER_ID")) > 0 Then
                    'all pay type fees are deleted, now delete the same in display summary which is already added above
                    If _basketSummaryEntity.RemoveAllPayTypeFromBasketSummary Then
                        If Not (totalBasket <= 0 AndAlso Utilities.CheckForDBNull_String(_dtBasketHeader.Rows(0)("CAT_MODE")).Trim.Length > 0) Then
                            _basketSummaryEntity.SetBasketErrorDetails(GlobalConstants.BSKTSMRY_ERR_CODE_BTC, "~/PagesLogin/Checkout/Checkout.aspx")
                        End If
                    Else
                        'some issue log it
                    End If
                Else
                    'some issue log it
                End If
            End If
        End If

        'now take away the part payment made by cc
        totalBasket -= (totalTicketingPartPaymentByCC + totalTicketingPartPaymentFeesValue)

        'charity fees
        If feesCharityShow Then
            _basketSummaryEntity.AddToBasketSummary(GlobalConstants.FEECATEGORY_CHARITY,
                                                        GlobalConstants.FEECATEGORY_CHARITY,
                                                        GlobalConstants.FEECATEGORY_CHARITY,
                                                        feesCharityTotal,
                                                        0,
                                                        0,
                                                        False,
                                                        False,
                                                        True,
                                                        True, GlobalConstants.BSKTSMRY_DSPLY_SEQ_TOTAL_CHARITY)
        End If

        'adhoc fees
        If feesAdhocShow Then
            _basketSummaryEntity.AddToBasketSummary(GlobalConstants.FEECATEGORY_ADHOC,
                                                        GlobalConstants.FEECATEGORY_ADHOC,
                                                        GlobalConstants.FEECATEGORY_ADHOC,
                                                        feesAdhocTotal,
                                                        0,
                                                        0,
                                                        False,
                                                        False,
                                                        True,
                                                        True, GlobalConstants.BSKTSMRY_DSPLY_SEQ_TOTAL_ADHOC)
        End If

        'variable fees
        If feesVariableShow Then
            _basketSummaryEntity.AddToBasketSummary(GlobalConstants.FEECATEGORY_VARIABLE,
                                                        GlobalConstants.FEECATEGORY_VARIABLE,
                                                        GlobalConstants.FEECATEGORY_VARIABLE,
                                                        feesVariableTotal,
                                                        0,
                                                        0,
                                                        False,
                                                        False,
                                                        True,
                                                        True, GlobalConstants.BSKTSMRY_DSPLY_SEQ_TOTAL_VARIABLE)
        End If


        'ticket total price
        If totalItemsTicketing > 0 Then
            _basketSummaryEntity.AddToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_TICKET_PRICE,
                                                        GlobalConstants.BSKTSMRY_TOTAL_TICKET_PRICE,
                                                        "",
                                                        totalTicketPrice,
                                                        0,
                                                        0,
                                                        False,
                                                        False,
                                                        True,
                                                        True, GlobalConstants.BSKTSMRY_DSPLY_SEQ_TOTAL_TICKET_PRICE)
        End If

        'ticket discount total
        If totalTicketDiscountTotal <> 0 Then
            _basketSummaryEntity.AddToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_TICKET_DISCOUNT,
                                                        GlobalConstants.BSKTSMRY_TOTAL_TICKET_DISCOUNT,
                                                        "",
                                                        totalTicketDiscountTotal,
                                                        0,
                                                        0,
                                                        False,
                                                        False,
                                                        True,
                                                        True, GlobalConstants.BSKTSMRY_DSPLY_SEQ_TOTAL_TICKET_DISCOUNT)
        End If


        'total ticket fees include all fees
        _basketSummaryEntity.AddToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_TICKET_FEES,
                                                    GlobalConstants.BSKTSMRY_TOTAL_TICKET_FEES,
                                                    "",
                                                    totalTicketingFees,
                                                    0,
                                                    0,
                                                    False,
                                                    False,
                                                    False,
                                                    False, GlobalConstants.BSKTSMRY_DSPLY_SEQ_DEFAULT)

        'total ticket price include all and total ticket price
        _basketSummaryEntity.AddToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_TICKETING,
                                                    GlobalConstants.BSKTSMRY_TOTAL_TICKETING,
                                                    "",
                                                    totalTicketPrice + totalTicketingFees,
                                                    0,
                                                    0,
                                                    False,
                                                    False,
                                                    False,
                                                    False, GlobalConstants.BSKTSMRY_DSPLY_SEQ_DEFAULT)

        'total ticketing items include only product or games
        _basketSummaryEntity.AddToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_ITEMS_TICKETING,
                                                    GlobalConstants.BSKTSMRY_TOTAL_ITEMS_TICKETING,
                                                    "",
                                                    totalItemsTicketing,
                                                    0,
                                                    0,
                                                    False,
                                                    False,
                                                    False,
                                                    False, GlobalConstants.BSKTSMRY_DSPLY_SEQ_DEFAULT)

        'total number of charity fees applied in basket
        _basketSummaryEntity.AddToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_ITEMS_APPLIED_CHRTY,
                                                   GlobalConstants.BSKTSMRY_TOTAL_ITEMS_APPLIED_CHRTY,
                                                   "",
                                                   totalItemsAppliedCharityFee,
                                                   0,
                                                   0,
                                                   False,
                                                   False,
                                                   False,
                                                   False, GlobalConstants.BSKTSMRY_DSPLY_SEQ_DEFAULT)

        'total number of adhoc fees applied in basket
        _basketSummaryEntity.AddToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_ITEMS_APPLIED_ADHOC,
                                                   GlobalConstants.BSKTSMRY_TOTAL_ITEMS_APPLIED_ADHOC,
                                                   "",
                                                   totalItemsAppliedAdhocFee,
                                                   0,
                                                   0,
                                                   False,
                                                   False,
                                                   False,
                                                   False, GlobalConstants.BSKTSMRY_DSPLY_SEQ_DEFAULT)

        'total number of variable fees applied in basket
        _basketSummaryEntity.AddToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_ITEMS_APPLIED_VRBLE,
                                                   GlobalConstants.BSKTSMRY_TOTAL_ITEMS_APPLIED_VRBLE,
                                                   "",
                                                   totalItemsAppliedVariableFee,
                                                   0,
                                                   0,
                                                   False,
                                                   False,
                                                   False,
                                                   False, GlobalConstants.BSKTSMRY_DSPLY_SEQ_DEFAULT)

        'total basket value
        _basketSummaryEntity.AddToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_BASKET,
                                                            GlobalConstants.BSKTSMRY_TOTAL_BASKET,
                                                            "",
                                                            totalBasket,
                                                            0,
                                                            0,
                                                            False,
                                                            False,
                                                            False,
                                                            False, GlobalConstants.BSKTSMRY_DSPLY_SEQ_DEFAULT)

        'total basket value without pay type fee
        _basketSummaryEntity.AddToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_BASKET_WO_PAYFEE,
                                                            GlobalConstants.BSKTSMRY_TOTAL_BASKET_WO_PAYFEE,
                                                            "",
                                                            totalBasketWithoutPayTypeFees,
                                                            0,
                                                            0,
                                                            False,
                                                            False,
                                                            False,
                                                            False, GlobalConstants.BSKTSMRY_DSPLY_SEQ_DEFAULT)

        'total merchandise items 
        _basketSummaryEntity.AddToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_ITEMS_MERCHANDISE,
                                                    GlobalConstants.BSKTSMRY_TOTAL_ITEMS_MERCHANDISE,
                                                    "",
                                                    totalItemsMerchandise,
                                                    0,
                                                    0,
                                                    False,
                                                    False,
                                                    False,
                                                    False, GlobalConstants.BSKTSMRY_DSPLY_SEQ_DEFAULT)


    End Sub

    Private Sub PopulateBasketSummaryFeeValueActual()
        Dim feeValueActual As Decimal = 0
        Dim currentCardTypeCode As String = Utilities.CheckForDBNull_String(_dtBasketHeader.Rows(0)("CARD_TYPE_CODE"))
        If currentCardTypeCode.Trim = "" Then
            Dim dicPayTypeFeeCategory As Dictionary(Of String, DEFeesRelations) = TDataObjects.FeesSettings.TblFeesRelations.GetPayTypeFeeCategoryList(Settings.BusinessUnit)
            If dicPayTypeFeeCategory IsNot Nothing Then
                Dim feeRelationsEntity As DEFeesRelations = Nothing
                If dicPayTypeFeeCategory.TryGetValue(Utilities.CheckForDBNull_String(_dtBasketHeader.Rows(0)("PAYMENT_TYPE")), feeRelationsEntity) Then
                    If feeRelationsEntity.FeeCategory = GlobalConstants.FEECATEGORY_BOOKING Then
                        currentCardTypeCode = GlobalConstants.FEE_BOOKING_CARDTYPE_EMPTY
                    End If
                End If
            End If
        End If

        If _basketSummaryEntity.FeesDTCardTypeBooking IsNot Nothing AndAlso
            _basketSummaryEntity.FeesDTCardTypeBooking.Rows.Count > 0 Then
            For rowIndex As Integer = 0 To _basketSummaryEntity.FeesDTCardTypeBooking.Rows.Count - 1
                If Utilities.CheckForDBNull_String(_basketSummaryEntity.FeesDTCardTypeBooking.Rows(rowIndex)("CARD_TYPE_CODE")) = currentCardTypeCode Then
                    _basketSummaryEntity.AddToBasketSummary(GlobalConstants.BSKTSMRY_BOOKING_FEE_ACTUAL,
                                                                    GlobalConstants.BSKTSMRY_BOOKING_FEE_ACTUAL,
                                                                    Utilities.CheckForDBNull_String(_basketSummaryEntity.FeesDTCardTypeBooking.Rows(rowIndex)("FEE_CATEGORY")),
                                                                    Utilities.CheckForDBNull_Decimal(_basketSummaryEntity.FeesDTCardTypeBooking.Rows(rowIndex)("FEE_VALUE")),
                                                                    Utilities.CheckForDBNull_Decimal(_basketSummaryEntity.FeesDTCardTypeBooking.Rows(rowIndex)("FEE_VALUE")),
                                                                    0,
                                                                    Utilities.CheckForDBNull_Boolean_DefaultFalse(_basketSummaryEntity.FeesDTCardTypeBooking.Rows(rowIndex)("IS_SYSTEM_FEE")),
                                                                    False,
                                                                    False,
                                                                    False, GlobalConstants.BSKTSMRY_DSPLY_SEQ_DEFAULT)
                End If
            Next
        End If
    End Sub

    Private Sub ValidateBasketSummaryForError()
        If _basketSummaryEntity.BasketErrorCode.Trim.Length <= 0 Then
            'if basket is in sale mode basket total should be >=0 so negative basket allowed only in CAT mode
            If _basketSummaryEntity.TotalBasket < 0 Then
                If Utilities.CheckForDBNull_String(_dtBasketHeader.Rows(0)("CAT_MODE")).Trim = "" Then
                    ' If AdHoc refund being made it's possible to have negative basket in sale mode (just check for presence possibly we should check the value but lot more work !
                    If Not basketContainsAdHocRefund() Then
                        'not a valid basket total for checkout
                        _basketSummaryEntity.SetBasketErrorDetails(GlobalConstants.BSKTSMRY_ERR_CODE_NSB, "~/PagesLogin/Checkout/Checkout.aspx")
                    End If
                End If
            End If
        End If

        'todo this
        'Check to see if any part payments have been applied and that they are not greater that then basket total
        'If Profile.Basket.CAT_MODE <> GlobalConstants.CATMODE_CANCEL Then
        '    calculatePartPayments()
        '    If _partPaymentsTotal > Profile.Basket.GetBasketTotal() Then
        '        Dim errorMessage As String = errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, Talent.eCommerce.Utilities.GetCurrentPageName, "EPursePaymentsExceedBasketTotal").ERROR_MESSAGE
        '        Dim errorListItem As New ListItem
        '        errorListItem.Value = errorMessage
        '        If Not ErrorList.Items.Contains(errorListItem) Then ErrorList.Items.Add(errorListItem)
        '        Session("PartPaymentError") = True
        '    Else
        '        Session("PartPaymentError") = Nothing
        '    End If
        'End If
    End Sub
    ' Returns whether basket contains an Ad Hoc Refund product (these are setup backend and are on MD042TBL)   
    Private Function basketContainsAdHocRefund() As Boolean
        Dim doesbasketContainsAdHocRefund As Boolean = False
        Dim dsProductDetails As New DataSet
        Dim err As ErrorObj = Nothing
        For Each row As DataRow In _dtBasketDetail.Rows
            Dim productCode As String = Utilities.CheckForDBNull_String(_dtBasketDetail.Rows(0)("Product")).Trim
            Dim tp As New TalentProduct
            Dim deP As New Talent.Common.DEProductDetails
            deP.ProductCode = productCode
            Dim saveMeCacheSettings As Boolean = Me.Settings.Cacheing
            With tp
                Dim settings As New DESettings
                .Settings = Me.Settings
                .Settings.Cacheing = True
                .De = deP
            End With
            err = tp.ProductDetails
            dsProductDetails = tp.ResultDataSet
            ' Need to restore this classes cache settings as '.Settings = Me.Settings' may have changed them   
            Me.Settings.Cacheing = saveMeCacheSettings
            If Not err.HasError AndAlso dsProductDetails IsNot Nothing AndAlso dsProductDetails.Tables.Count > 0 Then
                If dsProductDetails.Tables(0).Rows.Count > 0 Then
                    If dsProductDetails.Tables(0).Rows(0)("ErrorOccurred") <> GlobalConstants.ERRORFLAG Then
                        dsProductDetails = dsProductDetails
                        If dsProductDetails.Tables(2).Rows(0)("isAdHocREfund") Then
                            doesbasketContainsAdHocRefund = True
                        End If
                    End If

                End If
            End If
        Next
        Return doesbasketContainsAdHocRefund
    End Function

    Private Function ProcessBasketForBookingFees(ByVal basketHeaderID As String) As ErrorObj
        Dim errObj As New ErrorObj
        Dim talBasketFeesProcessor As New TalentBasketFeesProcessor
        talBasketFeesProcessor.Settings = Settings
        If CardTypeFeeCategory Is Nothing OrElse CardTypeFeeCategory.Count <= 0 Then
            CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(Settings.BusinessUnit)
        End If
        If FulfilmentFeeCategory Is Nothing OrElse FulfilmentFeeCategory.Count <= 0 Then
            FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(Settings.BusinessUnit)
        End If
        talBasketFeesProcessor.CardTypeFeeCategory = CardTypeFeeCategory
        talBasketFeesProcessor.FulfilmentFeeCategory = FulfilmentFeeCategory
        talBasketFeesProcessor.DeliveryChargeEntity = DeliveryChargeEntity
        errObj = talBasketFeesProcessor.ProcessBasketForBookingFees(basketHeaderID)
        Return errObj
    End Function

    Private Function ProcessCATAndGetTicketPriceAfterCAT(ByVal ticketPrice As Decimal) As Decimal

        Dim newTicketPrice As Decimal = ticketPrice

        'total cat price
        If _dtBasketHeader IsNot Nothing AndAlso _dtBasketHeader.Rows.Count > 0 AndAlso Utilities.CheckForDBNull_String(_dtBasketHeader.Rows(0)("CAT_MODE")).Trim.Length > 0 Then
            Dim catMode As String = Utilities.CheckForDBNull_String(_dtBasketHeader.Rows(0)("CAT_MODE")).Trim

            If (catMode = GlobalConstants.CATMODE_CANCEL OrElse catMode = GlobalConstants.CATMODE_CANCELALL) Then
                newTicketPrice = (ticketPrice * -1)
            ElseIf (catMode = GlobalConstants.CATMODE_AMEND OrElse catMode = GlobalConstants.CATMODE_TRANSFER) Then
                newTicketPrice -= Utilities.CheckForDBNull_Decimal(_dtBasketHeader.Rows(0)("CAT_PRICE"))
                _basketSummaryEntity.AddToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_CAT_TICKET_PRICE,
                                                    GlobalConstants.BSKTSMRY_TOTAL_CAT_TICKET_PRICE,
                                                    GlobalConstants.BSKTSMRY_TOTAL_CAT_TICKET_PRICE,
                                                    Utilities.CheckForDBNull_Decimal(_dtBasketHeader.Rows(0)("CAT_PRICE")) * (-1),
                                                    0,
                                                    0,
                                                    False,
                                                    False,
                                                    False,
                                                    False, GlobalConstants.BSKTSMRY_DSPLY_SEQ_TOTAL_CAT_TICKET_PRICE)
            End If

        End If

        Return newTicketPrice

    End Function

    Private Function ProcessPartPaymentAmount(ByRef isOrderConfirmation As Boolean, ByRef partPaymentsTotal As Decimal, ByRef partPaymentsTotalByCC As Decimal, ByRef partPaymentsTotalByOthers As Decimal, ByRef partPaymentsTotalFeesValue As Decimal) As Boolean
        Try
            If Settings.EcommerceModuleDefaultsValues.RetrievePartPayments Then
                Dim dtPartPayments As New DataTable
                Dim err As New Talent.Common.ErrorObj
                Dim talPayment As New Talent.Common.TalentPayment
                Dim paymentEntity As New Talent.Common.DEPayments
                talPayment.Settings = Settings
                With paymentEntity
                    .SessionId = BasketEntity.Basket_Header_ID
                    .CustomerNumber = LoginID
                End With
                talPayment.De = paymentEntity
                err = talPayment.RetrievePartPayments

                ' Was the call successful
                If Not err.HasError AndAlso _
                    Not talPayment.ResultDataSet Is Nothing AndAlso _
                    talPayment.ResultDataSet.Tables.Count = 2 AndAlso _
                    talPayment.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") <> GlobalConstants.ERRORFLAG Then
                    dtPartPayments = talPayment.ResultDataSet.Tables("PartPayments")
                    For Each dr As DataRow In dtPartPayments.Rows
                        If (Utilities.CheckForDBNull_String(dr.Item("PaymentMethod")) = GlobalConstants.CCPAYMENTTYPE) OrElse
                            (Utilities.CheckForDBNull_String(dr.Item("PaymentMethod")) = GlobalConstants.SAVEDCARDPAYMENTTYPE) OrElse
                            (Utilities.CheckForDBNull_String(dr.Item("PaymentMethod")) = GlobalConstants.CHIPANDPINPAYMENTTYPE) Then
                            partPaymentsTotalByCC = partPaymentsTotalByCC + (CType(dr.Item("PaymentAmount"), Decimal) - CType(dr.Item("FeeValue"), Decimal))
                        Else
                            partPaymentsTotalByOthers = partPaymentsTotalByOthers + (CType(dr.Item("PaymentAmount"), Decimal) - CType(dr.Item("FeeValue"), Decimal))
                        End If
                        partPaymentsTotal = partPaymentsTotal + CType(dr.Item("PaymentAmount"), Decimal)
                        partPaymentsTotalFeesValue += CType(dr.Item("FeeValue"), Decimal)
                    Next
                End If
                'total part payment to summary
                _basketSummaryEntity.AddToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_PART_PAYMENTS,
                                                            GlobalConstants.BSKTSMRY_TOTAL_PART_PAYMENTS,
                                                            "",
                                                            partPaymentsTotal,
                                                            0,
                                                            0,
                                                            False,
                                                            False,
                                                            False,
                                                            False, GlobalConstants.BSKTSMRY_DSPLY_SEQ_TOTAL_PART_PAYMENTS)
                _basketSummaryEntity.AddToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_PART_PAYMENTS_BY_CREDITCARD,
                                                            GlobalConstants.BSKTSMRY_TOTAL_PART_PAYMENTS_BY_CREDITCARD,
                                                            "",
                                                            partPaymentsTotalByCC,
                                                            0,
                                                            0,
                                                            False,
                                                            False,
                                                            False,
                                                            False, GlobalConstants.BSKTSMRY_DSPLY_SEQ_TOTAL_PART_PAYMENTS)
                _basketSummaryEntity.AddToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_PART_PAYMENTS_BY_OTHERS,
                                                            GlobalConstants.BSKTSMRY_TOTAL_PART_PAYMENTS_BY_OTHERS,
                                                            "",
                                                            partPaymentsTotalByOthers,
                                                            0,
                                                            0,
                                                            False,
                                                            False,
                                                            False,
                                                            False, GlobalConstants.BSKTSMRY_DSPLY_SEQ_TOTAL_PART_PAYMENTS)
            End If
        Catch ex As Exception
            partPaymentsTotal = 0
            partPaymentsTotalByCC = 0
            partPaymentsTotalByOthers = 0
            isOrderConfirmation = False
        End Try
        Return True
    End Function

    Private Function ProcessMerchandiseAndGetTotal() As Decimal
        'todo promotion and not delivery calculation in use
        Dim totalMerchandise As Decimal = 0

        If WebPrices IsNot Nothing AndAlso (WebPrices.Total_Items_Value_Net <> 0 OrElse WebPrices.Total_Items_Value_Gross <> 0) Then
            Dim totalMerchandiseItemsPrice As Decimal = 0
            Dim deliveryNet As Decimal = 0
            Dim deliveryTax As Decimal = 0
            Dim deliveryGross As Decimal = 0
            Dim canShowDeliveryPrice As Boolean = TryGetMerchandiseDeliveryValue(deliveryNet, deliveryTax, deliveryGross)


            If WebPrices.Total_Promotions_Value <> 0 Then
                AddMerchandiseToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_MERCHANDISE_PROMOTIONS_PRICE, WebPrices.Total_Promotions_Value, True, GlobalConstants.BSKTSMRY_DSPLY_SEQ_TOTAL_MERCHANDISE_PROMOTIONS, True)
            End If
            If Settings.EcommerceModuleDefaultsValues.ShowPricesExVAT Then
                'items price net
                AddMerchandiseToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_MERCHANDISE_ITEMS_PRICE, WebPrices.Total_Items_Value_Net, True, GlobalConstants.BSKTSMRY_DSPLY_SEQ_TOTAL_MERCHANDISE_ITEMS, True)
                If canShowDeliveryPrice Then
                    'delivery price net
                    AddMerchandiseToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_MERCHANDISE_DELIVERY_PRICE, deliveryNet, True, GlobalConstants.BSKTSMRY_DSPLY_SEQ_TOTAL_MERCHANDISE_DELIVERY, True)
                End If
                'items tax + delivery tax
                AddMerchandiseToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_MERCHANDISE_VAT, (WebPrices.Total_Items_Value_Tax + deliveryTax), True, GlobalConstants.BSKTSMRY_DSPLY_SEQ_TOTAL_MERCHANDISE_VAT, True)
            Else
                'items price gross
                AddMerchandiseToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_MERCHANDISE_ITEMS_PRICE, WebPrices.Total_Items_Value_Gross, True, GlobalConstants.BSKTSMRY_DSPLY_SEQ_TOTAL_MERCHANDISE_ITEMS, True)
                If canShowDeliveryPrice Then
                    'delivery price gross
                    AddMerchandiseToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_MERCHANDISE_DELIVERY_PRICE, deliveryGross, True, GlobalConstants.BSKTSMRY_DSPLY_SEQ_TOTAL_MERCHANDISE_DELIVERY, True)
                End If
            End If
            'total merchandise
            totalMerchandise = WebPrices.Total_Order_Value_Gross + deliveryGross - WebPrices.Total_Promotions_Value
            AddMerchandiseToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_MERCHANDISE, totalMerchandise, False, GlobalConstants.BSKTSMRY_DSPLY_SEQ_TOTAL_MERCHANDISE, True)
        End If

        Return totalMerchandise
    End Function

    Private Sub AddMerchandiseToBasketSummary(ByVal displayCode As String, ByVal price As Decimal, ByVal isDisplayType As Boolean, ByVal displaySeq As Integer, ByVal isIncluded As Boolean)
        _basketSummaryEntity.AddToBasketSummary(displayCode,
                                            displayCode,
                                            displayCode,
                                            price,
                                            0,
                                            0,
                                            False,
                                            False,
                                            isIncluded,
                                            isDisplayType, displaySeq)
    End Sub

    Private Function TryGetMerchandiseDeliveryValue(ByRef deliveryNet As Decimal, ByRef deliveryTax As Decimal, ByRef deliveryGross As Decimal) As Boolean
        Dim canDisplay As Boolean = False
        If Settings.EcommerceModuleDefaultsValues.DeliveryCalculationInUse Then
            'If Settings.CurrentPageName = "checkout.aspx" OrElse Settings.CurrentPageName = "checkoutorderconfirmation.aspx" Then
            If DeliveryChargeEntity IsNot Nothing Then
                canDisplay = True
                deliveryNet = Utilities.CheckForDBNull_Decimal(DeliveryChargeEntity.NET_VALUE)
                deliveryTax = Utilities.CheckForDBNull_Decimal(DeliveryChargeEntity.TAX_VALUE)
                deliveryGross = Utilities.CheckForDBNull_Decimal(DeliveryChargeEntity.GROSS_VALUE)
            Else
                Dim dtOrderHeader As DataTable = TDataObjects.OrderSettings.TblOrderHeader.GetOrderHeaderByTempOrderID(BasketEntity.Temp_Order_Id)
                If dtOrderHeader IsNot Nothing AndAlso dtOrderHeader.Rows.Count > 0 Then
                    canDisplay = True
                    deliveryNet = Utilities.CheckForDBNull_Decimal(dtOrderHeader.Rows(0)("TOTAL_DELIVERY_NET"))
                    deliveryTax = Utilities.CheckForDBNull_Decimal(dtOrderHeader.Rows(0)("TOTAL_DELIVERY_TAX"))
                    deliveryGross = Utilities.CheckForDBNull_Decimal(dtOrderHeader.Rows(0)("TOTAL_DELIVERY_GROSS"))
                End If
            End If
            'End If
        End If
        Return canDisplay
    End Function

    Private Function ProcessSTExceptionsSeatUpgradesTotal() As Decimal
        Dim totalExceptionValue As Decimal = 0
        Dim isNegativePrice As Boolean = False
        If _dtBasketDetailExceptions IsNot Nothing AndAlso _dtBasketDetailExceptions.Rows.Count > 0 Then
            For Each row As DataRow In _dtBasketDetailExceptions.Rows
                totalExceptionValue += Utilities.CheckForDBNull_Decimal(row("PRICE"))
                If Utilities.CheckForDBNull_String(row("CAT_FLAG")).Trim.Length > 0 AndAlso Utilities.CheckForDBNull_String(row("CAT_FLAG")) = GlobalConstants.CATMODE_CANCEL Then
                    isNegativePrice = True
                End If
            Next
        End If
        If isNegativePrice Then
            totalExceptionValue = (totalExceptionValue * -1)
        End If
        If totalExceptionValue <> 0 Then
            _basketSummaryEntity.AddToBasketSummary(GlobalConstants.BSKTSMRY_TOTAL_ST_EXCEPTIONS_PRICE,
                                                        GlobalConstants.BSKTSMRY_TOTAL_ST_EXCEPTIONS_PRICE,
                                                        String.Empty,
                                                        totalExceptionValue,
                                                        0,
                                                        0,
                                                        False,
                                                        False,
                                                        True,
                                                        True, GlobalConstants.BSKTSMRY_DSPLY_SEQ_ST_EXCEPTIONS_PRICE)
        End If
        Return totalExceptionValue
    End Function

End Class



