'todo
'tbl_fees_relations
'fee apply type differnt id
'fee code any code
'fee category Finance
'so if basket is applicable for finance fees
'insert tbl_basket_fees from this class for the card type code / payment type for example zebra CF
Namespace TalentBasketFees
    <Serializable()> _
    Friend Class TalentBasketFeesFinance
        Inherits TalentBasketFees

        Private Const SOURCECLASS As String = "TalentBasketFeesFinance"

        Private _validFeesFinanceForBasket As New List(Of DEFees)

        Public Property FinanceFeesList As List(Of DEFees) = Nothing

        Public Function ProcessFinanceFeesForBasket() As ErrorObj
            Dim errObj As New ErrorObj
            Try
                PopulateValidFeesFinance()
                If BasketEntity.CAT_MODE.Trim.Length > 0 AndAlso ConsiderCATDetailsOnCATTypeStatus <> GlobalConstants.FEE_CONSIDER_CAT_STATUS_STANDARD Then
                    ProcessValidFinanceFeesForCATBasket()
                Else
                    ProcessValidFinanceFeesForBasket()
                End If
            Catch ex As Exception
                errObj.HasError = True
                errObj.ErrorNumber = "TCTBFFIN-001"
                errObj.ErrorStatus = "While processing finance fees for basket"
                LogError(SOURCECLASS, "", errObj.ErrorNumber, errObj.ErrorStatus, ex)
            End Try
            Return errObj
        End Function

        Private Sub PopulateValidFeesFinance()
            If FinanceFeesList IsNot Nothing AndAlso BasketItemRequiresFees IsNot Nothing Then
                For basketItemIndex As Integer = 0 To BasketItemRequiresFees.Count - 1
                    If IsProductNotExcludedFromFees(BasketItemRequiresFees(basketItemIndex).Product) AndAlso BasketItemRequiresFees(basketItemIndex).MODULE_ = GlobalConstants.BASKETMODULETICKETING Then
                        Dim matchedItemIndex As Integer = GetMatchedIndexFromSelectionHierarchy(BasketItemRequiresFees(basketItemIndex), FinanceFeesList)
                        'fee found
                        If matchedItemIndex > -1 Then
                            AddValidFeesFinanceForBasket(FinanceFeesList(matchedItemIndex))
                        End If
                    End If
                Next
            End If
        End Sub

        Private Sub AddValidFeesFinanceForBasket(ByVal feeEntity As DEFees)
            Dim canAddThisFeeEntity As Boolean = True
            If _validFeesFinanceForBasket.Count > 0 Then
                For itemIndex As Integer = 0 To _validFeesFinanceForBasket.Count - 1
                    If feeEntity.Equal(_validFeesFinanceForBasket(itemIndex)) Then
                        canAddThisFeeEntity = False
                        Exit For
                    End If
                Next
            End If
            If canAddThisFeeEntity Then
                _validFeesFinanceForBasket.Add(feeEntity)
            End If
        End Sub

        Private Sub ProcessValidFinanceFeesForBasket()
            If _validFeesFinanceForBasket IsNot Nothing AndAlso _validFeesFinanceForBasket.Count > 0 Then
                Dim isTransactionBased As Boolean = False
                Dim financeFeeValue As Decimal = 0
                'transaction based fee value
                For validFeeItemIndex As Integer = 0 To _validFeesFinanceForBasket.Count - 1
                    If _validFeesFinanceForBasket(validFeeItemIndex).FeeType = GlobalConstants.FEETYPE_TRANSACTION _
                        AndAlso _validFeesFinanceForBasket(validFeeItemIndex).ChargeType = GlobalConstants.FEECHARGETYPE_FIXED _
                        AndAlso financeFeeValue <= _validFeesFinanceForBasket(validFeeItemIndex).FeeValue Then
                        financeFeeValue = _validFeesFinanceForBasket(validFeeItemIndex).FeeValue
                        isTransactionBased = True
                    End If
                Next
                'ticket type and product type fees
                For basketItemIndex As Integer = 0 To BasketDetailMergedList.Count - 1
                    If BasketDetailMergedList(basketItemIndex).ModuleOfItem = GlobalConstants.BASKETMODULETICKETING AndAlso String.IsNullOrWhiteSpace(BasketDetailMergedList(basketItemIndex).FeeCategory) Then
                        If IsProductNotExcludedFromFees(BasketDetailMergedList(basketItemIndex).Product) Then
                            financeFeeValue = GetFeeValueForBasketItem(False, _validFeesFinanceForBasket, BasketDetailMergedList(basketItemIndex), financeFeeValue, False, BasketHeaderMergedEntity, True, True)
                        End If
                    End If
                Next
                Dim basketFeesEntity As DEBasketFees = CastDEFeesToDEBasketFees(_validFeesFinanceForBasket(0))
                'now override required properties
                basketFeesEntity.BasketHeaderID = BasketEntity.Basket_Header_ID
                basketFeesEntity.FeeApplyType = GlobalConstants.FEEAPPLYTYPE_FINANCE
                basketFeesEntity.FeeValue = financeFeeValue
                basketFeesEntity.IsTransactional = isTransactionBased
                BasketFeesEntityList.Add(basketFeesEntity)
            End If
        End Sub

        Private Sub ProcessValidFinanceFeesForCATBasket()
            If _validFeesFinanceForBasket IsNot Nothing AndAlso _validFeesFinanceForBasket.Count > 0 Then

                Dim canChargeTransactionType As Boolean = False
                Dim canChargeProductType As Boolean = False
                Dim canChargeTicketType As Boolean = False

                AssignChargingFeeTypeOnCAT(canChargeTransactionType, canChargeProductType, canChargeTicketType, ConsiderCATDetailsOnCATTypeStatus)

                Dim isTransactionBased As Boolean = False
                Dim financeFeeValue As Decimal = 0
                'transaction based fee value
                If canChargeTransactionType Then
                    For validFeeItemIndex As Integer = 0 To _validFeesFinanceForBasket.Count - 1
                        If _validFeesFinanceForBasket(validFeeItemIndex).FeeType = GlobalConstants.FEETYPE_TRANSACTION _
                            AndAlso _validFeesFinanceForBasket(validFeeItemIndex).ChargeType = GlobalConstants.FEECHARGETYPE_FIXED _
                            AndAlso financeFeeValue <= _validFeesFinanceForBasket(validFeeItemIndex).FeeValue Then
                            financeFeeValue = _validFeesFinanceForBasket(validFeeItemIndex).FeeValue
                            isTransactionBased = True
                        End If
                    Next
                End If

                'ticket type and product type fees
                If canChargeProductType OrElse canChargeTicketType Then
                    For basketItemIndex As Integer = 0 To BasketDetailMergedList.Count - 1
                        If BasketDetailMergedList(basketItemIndex).ModuleOfItem = GlobalConstants.BASKETMODULETICKETING AndAlso String.IsNullOrWhiteSpace(BasketDetailMergedList(basketItemIndex).FeeCategory) Then
                            If IsProductNotExcludedFromFees(BasketDetailMergedList(basketItemIndex).Product) Then
                                financeFeeValue = GetFeeValueForBasketItem(False, _validFeesFinanceForBasket, BasketDetailMergedList(basketItemIndex), financeFeeValue, True, BasketHeaderMergedEntity, canChargeProductType, canChargeTicketType)
                            End If
                        End If
                    Next
                End If

                Dim basketFeesEntity As DEBasketFees = CastDEFeesToDEBasketFees(_validFeesFinanceForBasket(0))
                'now override required properties
                basketFeesEntity.BasketHeaderID = BasketEntity.Basket_Header_ID
                basketFeesEntity.FeeApplyType = GlobalConstants.FEEAPPLYTYPE_FINANCE
                basketFeesEntity.FeeValue = financeFeeValue
                basketFeesEntity.IsTransactional = isTransactionBased
                BasketFeesEntityList.Add(basketFeesEntity)
            End If
        End Sub

    End Class
End Namespace

