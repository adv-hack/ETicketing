'todo
'tbl_fees_relations
'fee apply type differnt id
'fee code DDFEE
'fee category direct debit
'so if basket is applicable for direct debit fees
'insert tbl_basket_fees from this class for the card type code for example visa debit card
'this has to be processed like booking fee like card
Namespace TalentBasketFees
    <Serializable()> _
    Friend Class TalentBasketFeesDirectDebit
        Inherits TalentBasketFees

        Private Const SOURCECLASS As String = "TalentBasketFeesDirectDebit"

        Private _validFeesDirectDebitForBasket As New List(Of DEFees)

        Public Property DirectDebitFeesList As List(Of DEFees) = Nothing

        Public Function ProcessDirectDebitFeesForBasket() As ErrorObj
            Dim errObj As New ErrorObj
            Try
                PopulateValidFeesDirectDebit()
                If BasketEntity.CAT_MODE.Trim.Length > 0 AndAlso ConsiderCATDetailsOnCATTypeStatus <> GlobalConstants.FEE_CONSIDER_CAT_STATUS_STANDARD Then
                    ProcessValidDirectDebitFeesForCATBasket()
                Else
                    ProcessValidDirectDebitFeesForBasket()
                End If
            Catch ex As Exception
                errObj.HasError = True
                errObj.ErrorNumber = "TCTBFDD-001"
                errObj.ErrorStatus = "While processing direct debit fees for basket"
                LogError(SOURCECLASS, "", errObj.ErrorNumber, errObj.ErrorStatus, ex)
            End Try
            Return errObj
        End Function

        Private Sub PopulateValidFeesDirectDebit()
            If DirectDebitFeesList IsNot Nothing AndAlso BasketItemRequiresFees IsNot Nothing Then
                For basketItemIndex As Integer = 0 To BasketItemRequiresFees.Count - 1
                    If IsProductNotExcludedFromFees(BasketItemRequiresFees(basketItemIndex).Product) AndAlso BasketItemRequiresFees(basketItemIndex).MODULE_ = GlobalConstants.BASKETMODULETICKETING Then
                        Dim matchedItemIndex As Integer = GetMatchedIndexFromSelectionHierarchy(BasketItemRequiresFees(basketItemIndex), DirectDebitFeesList)
                        'fee found
                        If matchedItemIndex > -1 Then
                            AddValidFeesDirectDebitForBasket(DirectDebitFeesList(matchedItemIndex))
                        End If
                    End If
                Next
            End If
        End Sub

        Private Sub AddValidFeesDirectDebitForBasket(ByVal feeEntity As DEFees)
            Dim canAddThisFeeEntity As Boolean = True
            If _validFeesDirectDebitForBasket.Count > 0 Then
                For itemIndex As Integer = 0 To _validFeesDirectDebitForBasket.Count - 1
                    If feeEntity.Equal(_validFeesDirectDebitForBasket(itemIndex)) Then
                        canAddThisFeeEntity = False
                        Exit For
                    End If
                Next
            End If
            If canAddThisFeeEntity Then
                _validFeesDirectDebitForBasket.Add(feeEntity)
            End If
        End Sub

        Private Sub ProcessValidDirectDebitFeesForBasket()
            If _validFeesDirectDebitForBasket IsNot Nothing AndAlso _validFeesDirectDebitForBasket.Count > 0 Then

                Dim isTransactionBased As Boolean = False
                Dim directDebitFeeValue As Decimal = 0
                'transaction based fee value
                For validFeeItemIndex As Integer = 0 To _validFeesDirectDebitForBasket.Count - 1
                    If _validFeesDirectDebitForBasket(validFeeItemIndex).FeeType = GlobalConstants.FEETYPE_TRANSACTION _
                        AndAlso _validFeesDirectDebitForBasket(validFeeItemIndex).ChargeType = GlobalConstants.FEECHARGETYPE_FIXED _
                        AndAlso directDebitFeeValue <= _validFeesDirectDebitForBasket(validFeeItemIndex).FeeValue Then
                        directDebitFeeValue = _validFeesDirectDebitForBasket(validFeeItemIndex).FeeValue
                        isTransactionBased = True
                    End If
                Next
                'ticket type and product type fees
                For basketItemIndex As Integer = 0 To BasketDetailMergedList.Count - 1
                    If BasketDetailMergedList(basketItemIndex).ModuleOfItem = GlobalConstants.BASKETMODULETICKETING AndAlso String.IsNullOrWhiteSpace(BasketDetailMergedList(basketItemIndex).FeeCategory) Then
                        If IsProductNotExcludedFromFees(BasketDetailMergedList(basketItemIndex).Product) Then
                            directDebitFeeValue = GetFeeValueForBasketItem(False, _validFeesDirectDebitForBasket, BasketDetailMergedList(basketItemIndex), directDebitFeeValue, False, BasketHeaderMergedEntity, True, True)
                        End If
                    End If
                Next

                Dim basketFeesEntity As DEBasketFees = CastDEFeesToDEBasketFees(_validFeesDirectDebitForBasket(0))
                'now override required properties
                basketFeesEntity.BasketHeaderID = BasketEntity.Basket_Header_ID
                basketFeesEntity.FeeApplyType = GlobalConstants.FEEAPPLYTYPE_DIRECTDEBIT
                basketFeesEntity.FeeValue = directDebitFeeValue
                basketFeesEntity.IsTransactional = isTransactionBased
                BasketFeesEntityList.Add(basketFeesEntity)
            End If
        End Sub

        Private Sub ProcessValidDirectDebitFeesForCATBasket()
            If _validFeesDirectDebitForBasket IsNot Nothing AndAlso _validFeesDirectDebitForBasket.Count > 0 Then

                Dim canChargeTransactionType As Boolean = False
                Dim canChargeProductType As Boolean = False
                Dim canChargeTicketType As Boolean = False

                AssignChargingFeeTypeOnCAT(canChargeTransactionType, canChargeProductType, canChargeTicketType, ConsiderCATDetailsOnCATTypeStatus)

                Dim isTransactionBased As Boolean = False
                Dim directDebitFeeValue As Decimal = 0
                'transaction based fee value
                If canChargeTransactionType Then
                    For validFeeItemIndex As Integer = 0 To _validFeesDirectDebitForBasket.Count - 1
                        If _validFeesDirectDebitForBasket(validFeeItemIndex).FeeType = GlobalConstants.FEETYPE_TRANSACTION _
                            AndAlso _validFeesDirectDebitForBasket(validFeeItemIndex).ChargeType = GlobalConstants.FEECHARGETYPE_FIXED _
                            AndAlso directDebitFeeValue <= _validFeesDirectDebitForBasket(validFeeItemIndex).FeeValue Then
                            directDebitFeeValue = _validFeesDirectDebitForBasket(validFeeItemIndex).FeeValue
                            isTransactionBased = True
                        End If
                    Next
                End If

                'ticket type and product type fees
                If canChargeProductType OrElse canChargeTicketType Then
                    For basketItemIndex As Integer = 0 To BasketDetailMergedList.Count - 1
                        If BasketDetailMergedList(basketItemIndex).ModuleOfItem = GlobalConstants.BASKETMODULETICKETING AndAlso String.IsNullOrWhiteSpace(BasketDetailMergedList(basketItemIndex).FeeCategory) Then
                            If IsProductNotExcludedFromFees(BasketDetailMergedList(basketItemIndex).Product) Then
                                directDebitFeeValue = GetFeeValueForBasketItem(False, _validFeesDirectDebitForBasket, BasketDetailMergedList(basketItemIndex), directDebitFeeValue, True, BasketHeaderMergedEntity, canChargeProductType, canChargeTicketType)
                            End If
                        End If
                    Next
                End If

                Dim basketFeesEntity As DEBasketFees = CastDEFeesToDEBasketFees(_validFeesDirectDebitForBasket(0))
                'now override required properties
                basketFeesEntity.BasketHeaderID = BasketEntity.Basket_Header_ID
                basketFeesEntity.FeeApplyType = GlobalConstants.FEEAPPLYTYPE_DIRECTDEBIT
                basketFeesEntity.FeeValue = directDebitFeeValue
                basketFeesEntity.IsTransactional = isTransactionBased
                BasketFeesEntityList.Add(basketFeesEntity)
            End If
        End Sub

    End Class
End Namespace
