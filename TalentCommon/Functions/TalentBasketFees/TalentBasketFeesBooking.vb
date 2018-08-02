Namespace TalentBasketFees
    <Serializable()> _
    Friend Class TalentBasketFeesBooking
        Inherits TalentBasketFees

        Private Const SOURCECLASS As String = "TalentBasketFeesBooking"
        Private _validFeesCardTypeForBasket As New Dictionary(Of String, List(Of DEFees))

        Private _totalTicketValue As Decimal = 0
        Private _totalExceptionValue As Decimal = 0
        Private _totalFeeValue As Decimal = 0
        Private _totalRetailValue As Decimal = 0
        Private _totalBasket As Decimal = 0
        Private _IsPercentageBasedProcess As Boolean = False
        Public Property BookingFeesList As List(Of DEFees) = Nothing
        Public Property DeliveryChargeEntity() As DEDeliveryCharges.DEDeliveryCharge = Nothing


        Public Function ProcessBookingFeesForBasket() As ErrorObj
            Dim errObj As New ErrorObj
            Try
                PopulateValidFeesBooking()
                If BasketEntity.CAT_MODE.Trim.Length > 0 AndAlso ConsiderCATDetailsOnCATTypeStatus <> GlobalConstants.FEE_CONSIDER_CAT_STATUS_STANDARD Then
                    ProcessValidBookingFeesForCATBasket()
                Else
                    ProcessValidBookingFeesForBasket()
                End If
            Catch ex As Exception
                errObj.HasError = True
                errObj.ErrorNumber = "TCTBFBOK-001"
                errObj.ErrorStatus = "While processing booking fees for basket"
                LogError(SOURCECLASS, "", errObj.ErrorNumber, errObj.ErrorStatus, ex)
            End Try
            Return errObj
        End Function

        Private Sub GetPartPayment(ByRef patPayWithoutFee As Decimal, ByRef partPayFee As Decimal)
            Dim dtPartPayments As DataTable = Utilities.RetrievePartPayments(Settings, BasketEntity.Basket_Header_ID)
            If dtPartPayments IsNot Nothing AndAlso dtPartPayments.Rows.Count > 0 Then
                For rowIndex As Integer = 0 To dtPartPayments.Rows.Count - 1
                    partPayFee += Utilities.CheckForDBNull_Decimal(dtPartPayments.Rows(rowIndex)("FeeValue"))
                    patPayWithoutFee += Utilities.CheckForDBNull_Decimal(dtPartPayments.Rows(rowIndex)("PaymentAmount")) - Utilities.CheckForDBNull_Decimal(dtPartPayments.Rows(rowIndex)("FeeValue"))
                Next
            End If
        End Sub

        Private Function GetPartPaymentWithoutFee() As Decimal
            Dim partPaymentWithoutFee As Decimal = 0
            Dim dtPartPayments As DataTable = Utilities.RetrievePartPayments(Settings, BasketEntity.Basket_Header_ID)
            If dtPartPayments IsNot Nothing AndAlso dtPartPayments.Rows.Count > 0 Then
                For rowIndex As Integer = 0 To dtPartPayments.Rows.Count - 1
                    partPaymentWithoutFee += Utilities.CheckForDBNull_Decimal(dtPartPayments.Rows(rowIndex)("PaymentAmount")) - Utilities.CheckForDBNull_Decimal(dtPartPayments.Rows(rowIndex)("FeeValue"))
                Next
            End If
            Return partPaymentWithoutFee
        End Function

        Private Function GetPartPaymentFee() As Decimal
            Dim partPaymentFee As Decimal = 0
            Dim dtPartPayments As DataTable = Utilities.RetrievePartPayments(Settings, BasketEntity.Basket_Header_ID)
            If dtPartPayments IsNot Nothing AndAlso dtPartPayments.Rows.Count > 0 Then
                For rowIndex As Integer = 0 To dtPartPayments.Rows.Count - 1
                    partPaymentFee += Utilities.CheckForDBNull_Decimal(dtPartPayments.Rows(rowIndex)("FeeValue"))
                Next
            End If
            Return partPaymentFee
        End Function

        Private Sub PopulateAllTotalValues()

            _totalTicketValue = 0
            _totalExceptionValue = 0
            _totalFeeValue = 0
            _totalRetailValue = 0
            _totalBasket = 0
            Dim isFeeNotYetProcessed As Boolean = True
            For itemIndex As Integer = 0 To BasketItemEntityList.Count - 1
                isFeeNotYetProcessed = True
                For basketItemIndex As Integer = 0 To BasketDetailMergedList.Count - 1
                    If (BasketDetailMergedList(basketItemIndex).Product = BasketItemEntityList(itemIndex).Product) AndAlso BasketDetailMergedList(basketItemIndex).IsProcessedForBookingFee Then
                        isFeeNotYetProcessed = False
                        _totalBasket += BasketItemEntityList(itemIndex).Gross_Price
                        Exit For
                    End If
                Next
                If isFeeNotYetProcessed Then
                    If (Utilities.IsTicketingFee(Settings.EcommerceModuleDefaultsValues.CashBackFeeCode, BasketItemEntityList(itemIndex).MODULE_, BasketItemEntityList(itemIndex).Product, BasketItemEntityList(itemIndex).FEE_CATEGORY)) Then
                        If (BasketItemEntityList(itemIndex).FEE_CATEGORY <> GlobalConstants.FEECATEGORY_BOOKING) Then
                            If BasketItemEntityList(itemIndex).IS_INCLUDED Then
                                'fee product
                                _totalFeeValue += BasketItemEntityList(itemIndex).Gross_Price
                            End If
                        End If
                    ElseIf BasketItemEntityList(itemIndex).MODULE_.ToUpper = GlobalConstants.BASKETMODULETICKETING.ToUpper Then
                        'ticket product
                        _totalTicketValue += BasketItemEntityList(itemIndex).Gross_Price
                    Else
                        'retail product
                        _totalRetailValue += (BasketItemEntityList(itemIndex).Gross_Price * BasketItemEntityList(itemIndex).Quantity)
                    End If
                End If
            Next
            If _totalRetailValue <> 0 Then
                _totalRetailValue += Utilities.CheckForDBNull_Decimal(GetDeliveryGrossValue())
            End If
            _totalBasket += _totalTicketValue + _totalFeeValue + _totalRetailValue - BasketEntity.CAT_PRICE
            'total exception not included with total unless it is required in percentage fees calculation
            _totalExceptionValue = GetExceptionsTicketTotal()
        End Sub

        Private Sub PopulateValidFeesBooking()

            Dim feeEntityList As List(Of DEFees) = Nothing

            If BookingFeesList IsNot Nothing AndAlso BasketItemRequiresFees IsNot Nothing Then

                If CardTypeFeeCategory IsNot Nothing Then

                    Dim tempFeeEntityList As List(Of DEFees) = BookingFeesList

                    'loop for each card type
                    For Each cardType As KeyValuePair(Of String, String) In CardTypeFeeCategory

                        feeEntityList = tempFeeEntityList

                        For basketItemIndex As Integer = 0 To BasketItemRequiresFees.Count - 1

                            If IsProductNotExcludedFromFees(BasketItemRequiresFees(basketItemIndex).Product) Then

                                Dim matchedItemIndex As Integer = GetMatchedIndexFromSelectionHierarchy(BasketItemRequiresFees(basketItemIndex), BookingFeesList, cardType.Key)
                                'fee found
                                If matchedItemIndex > -1 Then
                                    AddValidFeesCardTypeForBasket(cardType.Key, feeEntityList(matchedItemIndex))
                                End If

                            End If

                        Next

                    Next

                End If

            End If

        End Sub

        Private Sub AddValidFeesCardTypeForBasket(ByVal cardType As String, ByVal feeEntity As DEFees)

            Dim tempFeesEntityList As List(Of DEFees) = Nothing
            Dim canAddThisFeeEntity As Boolean = True

            If _validFeesCardTypeForBasket.TryGetValue(cardType, tempFeesEntityList) Then
                If tempFeesEntityList.Count > 0 Then
                    For itemIndex As Integer = 0 To tempFeesEntityList.Count - 1
                        If feeEntity.Equal(tempFeesEntityList(itemIndex)) Then
                            canAddThisFeeEntity = False
                            Exit For
                        End If
                    Next
                End If
            Else
                tempFeesEntityList = New List(Of DEFees)
            End If

            If canAddThisFeeEntity Then
                tempFeesEntityList.Add(feeEntity)
                _validFeesCardTypeForBasket(cardType) = tempFeesEntityList
                If Not _IsPercentageBasedProcess Then _IsPercentageBasedProcess = (feeEntity.ChargeType = GlobalConstants.FEECHARGETYPE_PERCENTAGE)
            End If

        End Sub

        Private Sub ProcessValidBookingFeesForBasket()
            If _IsPercentageBasedProcess Then
                ProcessValidBookingFeesForBasketPercentage()
            Else
                ProcessValidBookingFeesForBasketFixed()
            End If
        End Sub

        Private Sub ProcessValidBookingFeesForBasketPercentage()
            If _validFeesCardTypeForBasket IsNot Nothing AndAlso _validFeesCardTypeForBasket.Count > 0 Then

                'how much paid in part payment
                Dim partPaymentWithoutFee As Decimal = 0
                Dim partPaymentFee As Decimal = 0
                GetPartPayment(partPaymentWithoutFee, partPaymentFee)

                For Each cardTypeFeeEntites As KeyValuePair(Of String, List(Of DEFees)) In _validFeesCardTypeForBasket
                    Dim feeValueForThisCardType As Decimal = 0
                    'ticket type and product type fees only applicable to ticketing
                    For basketItemIndex As Integer = 0 To BasketDetailMergedList.Count - 1
                        BasketDetailMergedList(basketItemIndex).IsProcessedForBookingFee = False
                        If BasketDetailMergedList(basketItemIndex).ModuleOfItem = GlobalConstants.BASKETMODULETICKETING AndAlso String.IsNullOrWhiteSpace(BasketDetailMergedList(basketItemIndex).FeeCategory) Then
                            If IsProductNotExcludedFromFees(BasketDetailMergedList(basketItemIndex).Product) Then
                                feeValueForThisCardType = GetFeeValueForBasketItem(True, cardTypeFeeEntites.Value, BasketDetailMergedList(basketItemIndex), feeValueForThisCardType, False, BasketHeaderMergedEntity, True, True)
                            Else
                                BasketDetailMergedList(basketItemIndex).IsProcessedForBookingFee = True
                            End If
                        End If
                    Next

                    PopulateAllTotalValues()

                    'highest transaction based fee value
                    Dim isTransactionBased As Boolean = False
                    For validFeeItemIndex As Integer = 0 To cardTypeFeeEntites.Value.Count - 1
                        If cardTypeFeeEntites.Value(validFeeItemIndex).ChargeType = GlobalConstants.FEECHARGETYPE_PERCENTAGE Then
                            'Percentage
                            If cardTypeFeeEntites.Value(validFeeItemIndex).FeeType = GlobalConstants.FEETYPE_TRANSACTION Then
                                feeValueForThisCardType += (_totalTicketValue + _totalFeeValue + _totalRetailValue + partPaymentFee + _totalExceptionValue) * (cardTypeFeeEntites.Value(validFeeItemIndex).FeeValue / 100)
                                _totalBasket = _totalBasket + _totalExceptionValue
                                isTransactionBased = True
                            End If
                        End If
                    Next

                    'total basket
                    _totalBasket = _totalBasket + partPaymentFee + feeValueForThisCardType

                    'actual fee value after taken the part partment + part payment fee
                    feeValueForThisCardType = (((_totalBasket - (partPaymentFee + partPaymentWithoutFee)) / _totalBasket) * feeValueForThisCardType) + partPaymentFee

                    Dim basketFeesEntity As DEBasketFees = CastDEFeesToDEBasketFees(cardTypeFeeEntites.Value(0))
                    'now override required properties
                    basketFeesEntity.BasketHeaderID = BasketEntity.Basket_Header_ID
                    basketFeesEntity.CardType = cardTypeFeeEntites.Key
                    basketFeesEntity.FeeApplyType = GlobalConstants.FEEAPPLYTYPE_BOOKING
                    basketFeesEntity.FeeValue = feeValueForThisCardType
                    basketFeesEntity.IsTransactional = isTransactionBased
                    BasketFeesEntityList.Add(basketFeesEntity)
                Next
            End If
        End Sub

        Private Sub ProcessValidBookingFeesForBasketFixed()
            If _validFeesCardTypeForBasket IsNot Nothing AndAlso _validFeesCardTypeForBasket.Count > 0 Then

                For Each cardTypeFeeEntites As KeyValuePair(Of String, List(Of DEFees)) In _validFeesCardTypeForBasket
                    Dim feeValueForThisCardType As Decimal = 0
                    'highest transaction based fee value
                    Dim isTransactionBased As Boolean = False
                    For validFeeItemIndex As Integer = 0 To cardTypeFeeEntites.Value.Count - 1
                        If cardTypeFeeEntites.Value(validFeeItemIndex).FeeType = GlobalConstants.FEETYPE_TRANSACTION _
                            AndAlso feeValueForThisCardType <= cardTypeFeeEntites.Value(validFeeItemIndex).FeeValue Then
                            feeValueForThisCardType = cardTypeFeeEntites.Value(validFeeItemIndex).FeeValue
                            isTransactionBased = True
                        End If
                    Next

                    'ticket type and product type fees
                    For basketItemIndex As Integer = 0 To BasketDetailMergedList.Count - 1
                        If BasketDetailMergedList(basketItemIndex).ModuleOfItem = GlobalConstants.BASKETMODULETICKETING AndAlso String.IsNullOrWhiteSpace(BasketDetailMergedList(basketItemIndex).FeeCategory) Then
                            If IsProductNotExcludedFromFees(BasketDetailMergedList(basketItemIndex).Product) Then
                                feeValueForThisCardType = GetFeeValueForBasketItem(True, cardTypeFeeEntites.Value, BasketDetailMergedList(basketItemIndex), feeValueForThisCardType, False, BasketHeaderMergedEntity, True, True)
                            End If
                        End If
                    Next

                    Dim basketFeesEntity As DEBasketFees = CastDEFeesToDEBasketFees(cardTypeFeeEntites.Value(0))
                    'now override required properties
                    basketFeesEntity.BasketHeaderID = BasketEntity.Basket_Header_ID
                    basketFeesEntity.CardType = cardTypeFeeEntites.Key
                    basketFeesEntity.FeeApplyType = GlobalConstants.FEEAPPLYTYPE_BOOKING
                    basketFeesEntity.FeeValue = feeValueForThisCardType
                    basketFeesEntity.IsTransactional = isTransactionBased
                    BasketFeesEntityList.Add(basketFeesEntity)
                Next
            End If
        End Sub

        Private Sub ProcessValidBookingFeesForCATBasket()
            If _IsPercentageBasedProcess Then
                ProcessValidBookingFeesForCATBasketPercentage()
            Else
                ProcessValidBookingFeesForCATBasketFixed()
            End If
        End Sub

        Private Sub ProcessValidBookingFeesForCATBasketPercentage()
            If _validFeesCardTypeForBasket IsNot Nothing AndAlso _validFeesCardTypeForBasket.Count > 0 Then

                Dim canChargeTransactionType As Boolean = False
                Dim canChargeProductType As Boolean = False
                Dim canChargeTicketType As Boolean = False

                AssignChargingFeeTypeOnCAT(canChargeTransactionType, canChargeProductType, canChargeTicketType, ConsiderCATDetailsOnCATTypeStatus)

                'how much paid in part payment
                Dim partPaymentWithoutFee As Decimal = 0
                Dim partPaymentFee As Decimal = 0
                GetPartPayment(partPaymentWithoutFee, partPaymentFee)

                For Each cardTypeFeeEntites As KeyValuePair(Of String, List(Of DEFees)) In _validFeesCardTypeForBasket
                    Dim feeValueForThisCardType As Decimal = 0
                    'ticket type and product type fees only applicable to ticketing
                    For basketItemIndex As Integer = 0 To BasketDetailMergedList.Count - 1
                        BasketDetailMergedList(basketItemIndex).IsProcessedForBookingFee = False
                        If BasketDetailMergedList(basketItemIndex).ModuleOfItem = GlobalConstants.BASKETMODULETICKETING AndAlso String.IsNullOrWhiteSpace(BasketDetailMergedList(basketItemIndex).FeeCategory) Then
                            If IsProductNotExcludedFromFees(BasketDetailMergedList(basketItemIndex).Product) Then
                                feeValueForThisCardType = GetFeeValueForBasketItem(True, cardTypeFeeEntites.Value, BasketDetailMergedList(basketItemIndex), feeValueForThisCardType, True, BasketHeaderMergedEntity, canChargeProductType, canChargeTicketType)
                            Else
                                BasketDetailMergedList(basketItemIndex).IsProcessedForBookingFee = True
                            End If
                        End If
                    Next

                    PopulateAllTotalValues()

                    'highest transaction based fee value
                    Dim isTransactionBased As Boolean = False
                    If canChargeTransactionType Then
                        For validFeeItemIndex As Integer = 0 To cardTypeFeeEntites.Value.Count - 1
                            If cardTypeFeeEntites.Value(validFeeItemIndex).ChargeType = GlobalConstants.FEECHARGETYPE_PERCENTAGE Then
                                'Percentage
                                If cardTypeFeeEntites.Value(validFeeItemIndex).FeeType = GlobalConstants.FEETYPE_TRANSACTION Then
                                    feeValueForThisCardType += (_totalTicketValue + _totalFeeValue + _totalRetailValue + partPaymentFee - BasketEntity.CAT_PRICE) * (cardTypeFeeEntites.Value(validFeeItemIndex).FeeValue / 100)
                                    isTransactionBased = True
                                End If
                            End If
                        Next
                    End If

                    'total basket
                    _totalBasket = _totalBasket + partPaymentFee

                    'actual fee value after taken the part partment + part payment fee
                    feeValueForThisCardType = (((_totalBasket - (partPaymentFee + partPaymentWithoutFee)) / _totalBasket) * feeValueForThisCardType) + partPaymentFee

                    Dim basketFeesEntity As DEBasketFees = CastDEFeesToDEBasketFees(cardTypeFeeEntites.Value(0))
                    'now override required properties
                    basketFeesEntity.BasketHeaderID = BasketEntity.Basket_Header_ID
                    basketFeesEntity.CardType = cardTypeFeeEntites.Key
                    basketFeesEntity.FeeApplyType = GlobalConstants.FEEAPPLYTYPE_BOOKING
                    basketFeesEntity.FeeValue = feeValueForThisCardType
                    basketFeesEntity.IsTransactional = isTransactionBased
                    BasketFeesEntityList.Add(basketFeesEntity)
                Next
            End If
        End Sub

        Private Sub ProcessValidBookingFeesForCATBasketFixed()
            If _validFeesCardTypeForBasket IsNot Nothing AndAlso _validFeesCardTypeForBasket.Count > 0 Then

                Dim canChargeTransactionType As Boolean = False
                Dim canChargeProductType As Boolean = False
                Dim canChargeTicketType As Boolean = False

                AssignChargingFeeTypeOnCAT(canChargeTransactionType, canChargeProductType, canChargeTicketType, ConsiderCATDetailsOnCATTypeStatus)

                For Each cardTypeFeeEntites As KeyValuePair(Of String, List(Of DEFees)) In _validFeesCardTypeForBasket
                    Dim feeValueForThisCardType As Decimal = 0
                    'highest transaction based fee value
                    Dim isTransactionBased As Boolean = False
                    If canChargeTransactionType Then
                        For validFeeItemIndex As Integer = 0 To cardTypeFeeEntites.Value.Count - 1
                            If cardTypeFeeEntites.Value(validFeeItemIndex).FeeType = GlobalConstants.FEETYPE_TRANSACTION _
                                AndAlso feeValueForThisCardType <= cardTypeFeeEntites.Value(validFeeItemIndex).FeeValue Then
                                feeValueForThisCardType = cardTypeFeeEntites.Value(validFeeItemIndex).FeeValue
                                isTransactionBased = True
                            End If
                        Next
                    End If

                    'ticket type and product type fees
                    If canChargeProductType OrElse canChargeTicketType Then
                        For basketItemIndex As Integer = 0 To BasketDetailMergedList.Count - 1
                            If BasketDetailMergedList(basketItemIndex).ModuleOfItem = GlobalConstants.BASKETMODULETICKETING AndAlso String.IsNullOrWhiteSpace(BasketDetailMergedList(basketItemIndex).FeeCategory) Then
                                If IsProductNotExcludedFromFees(BasketDetailMergedList(basketItemIndex).Product) Then
                                    feeValueForThisCardType = GetFeeValueForBasketItem(True, cardTypeFeeEntites.Value, BasketDetailMergedList(basketItemIndex), feeValueForThisCardType, True, BasketHeaderMergedEntity, canChargeProductType, canChargeTicketType)
                                End If
                            End If
                        Next
                    End If


                    Dim basketFeesEntity As DEBasketFees = CastDEFeesToDEBasketFees(cardTypeFeeEntites.Value(0))
                    'now override required properties
                    basketFeesEntity.BasketHeaderID = BasketEntity.Basket_Header_ID
                    basketFeesEntity.CardType = cardTypeFeeEntites.Key
                    basketFeesEntity.FeeApplyType = GlobalConstants.FEEAPPLYTYPE_BOOKING
                    basketFeesEntity.FeeValue = feeValueForThisCardType
                    basketFeesEntity.IsTransactional = isTransactionBased
                    BasketFeesEntityList.Add(basketFeesEntity)
                Next
            End If
        End Sub

        Private Function GetDeliveryGrossValue() As Decimal
            Dim delGrossValue As Decimal = 0
            If DeliveryChargeEntity IsNot Nothing Then
                delGrossValue = Utilities.CheckForDBNull_Decimal(DeliveryChargeEntity.GROSS_VALUE)
            Else
                Dim dtOrderHeader As DataTable = TDataObjects.OrderSettings.TblOrderHeader.GetOrderHeaderByTempOrderID(BasketEntity.Temp_Order_Id)
                If dtOrderHeader IsNot Nothing AndAlso dtOrderHeader.Rows.Count > 0 Then
                    delGrossValue = Utilities.CheckForDBNull_Decimal(dtOrderHeader.Rows(0)("TOTAL_DELIVERY_GROSS"))
                End If
            End If
            Return delGrossValue
        End Function

        Private Function GetExceptionsTicketTotal() As Decimal
            Dim totalExceptionValue As Decimal = 0
            Dim isNegativePrice As Boolean = False
            Dim dtBasketDetailExceptions As DataTable = TDataObjects.BasketSettings.TblBasketDetailExceptions.GetByBasketDetailHeaderIDAndModule(BasketEntity.Basket_Header_ID, GlobalConstants.BASKETMODULETICKETING)
            If dtBasketDetailExceptions IsNot Nothing AndAlso dtBasketDetailExceptions.Rows.Count > 0 Then
                For Each row As DataRow In dtBasketDetailExceptions.Rows
                    totalExceptionValue += Utilities.CheckForDBNull_Decimal(row("PRICE"))
                    If Utilities.CheckForDBNull_String(row("CAT_FLAG")).Trim.Length > 0 AndAlso Utilities.CheckForDBNull_String(row("CAT_FLAG")) = GlobalConstants.CATMODE_CANCEL Then
                        isNegativePrice = True
                    End If
                Next
            End If
            If isNegativePrice Then
                totalExceptionValue = (totalExceptionValue * -1)
            End If

            Return totalExceptionValue
        End Function

    End Class
End Namespace
