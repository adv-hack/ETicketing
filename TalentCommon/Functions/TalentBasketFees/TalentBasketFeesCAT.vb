Namespace TalentBasketFees
    <Serializable()> _
    Friend Class TalentBasketFeesCAT
        Inherits TalentBasketFees

        Private Const SOURCECLASS As String = "TalentBasketFeesCAT"

        Private _validFeesCATForBasket As New List(Of DEFees)

        Public Property CATFeesList As List(Of DEFees) = Nothing

        Public Function ProcessCATFeesForBasket() As ErrorObj
            Dim errObj As New ErrorObj
            Try
                PopulateValidFeesCAT()
                If BasketEntity.CAT_MODE.Trim.Length > 0 AndAlso ConsiderCATDetailsOnCATTypeStatus <> GlobalConstants.FEE_CONSIDER_CAT_STATUS_STANDARD Then
                    ProcessValidCATFeesForCATBasket()
                Else
                    ProcessValidCATFeesForBasket()
                End If
            Catch ex As Exception
                errObj.HasError = True
                errObj.ErrorNumber = "TCTBFCAT-001"
                errObj.ErrorStatus = "While processing cat fees cancel or amend or transfer for basket"
                LogError(SOURCECLASS, "", errObj.ErrorNumber, errObj.ErrorStatus, ex)
            End Try
            Return errObj
        End Function

        Private Sub PopulateValidFeesCAT()
            If CATFeesList IsNot Nothing AndAlso BasketItemRequiresFees IsNot Nothing Then
                For basketItemIndex As Integer = 0 To BasketItemRequiresFees.Count - 1
                    If IsProductNotExcludedFromFees(BasketItemRequiresFees(basketItemIndex).Product) AndAlso BasketItemRequiresFees(basketItemIndex).MODULE_ = GlobalConstants.BASKETMODULETICKETING Then
                        Dim matchedItemIndex As Integer = GetMatchedIndexFromSelectionHierarchy(BasketItemRequiresFees(basketItemIndex), CATFeesList)
                        'fee found
                        If matchedItemIndex > -1 Then
                            AddValidFeesCATForBasket(CATFeesList(matchedItemIndex))
                        End If
                    End If
                Next
            End If
        End Sub

        Private Sub AddValidFeesCATForBasket(ByVal feeEntity As DEFees)
            Dim canAddThisFeeEntity As Boolean = True
            If _validFeesCATForBasket.Count > 0 Then
                For itemIndex As Integer = 0 To _validFeesCATForBasket.Count - 1
                    If feeEntity.Equal(_validFeesCATForBasket(itemIndex)) Then
                        canAddThisFeeEntity = False
                        Exit For
                    End If
                Next
            End If
            If canAddThisFeeEntity Then
                _validFeesCATForBasket.Add(feeEntity)
            End If
        End Sub

        Private Sub ProcessValidCATFeesForBasket()
            If _validFeesCATForBasket IsNot Nothing AndAlso _validFeesCATForBasket.Count > 0 Then

                Dim isTransactionBased As Boolean = False
                Dim CATFeeValue As Decimal = 0
                'highest transaction based fee value
                For validFeeItemIndex As Integer = 0 To _validFeesCATForBasket.Count - 1
                    If _validFeesCATForBasket(validFeeItemIndex).FeeType = GlobalConstants.FEETYPE_TRANSACTION _
                        AndAlso _validFeesCATForBasket(validFeeItemIndex).ChargeType = GlobalConstants.FEECHARGETYPE_FIXED _
                        AndAlso CATFeeValue <= _validFeesCATForBasket(validFeeItemIndex).FeeValue Then
                        CATFeeValue = _validFeesCATForBasket(validFeeItemIndex).FeeValue
                        isTransactionBased = True
                    End If
                Next
                'ticket type and product type fees
                For basketItemIndex As Integer = 0 To BasketDetailMergedList.Count - 1
                    If IsProductNotExcludedFromFees(BasketDetailMergedList(basketItemIndex).Product) Then
                        CATFeeValue = GetFeeValueForBasketItem(False, _validFeesCATForBasket, BasketDetailMergedList(basketItemIndex), CATFeeValue, False, BasketHeaderMergedEntity, True, True)
                    End If
                Next

                Dim basketFeesEntity As DEBasketFees = CastDEFeesToDEBasketFees(_validFeesCATForBasket(0))
                'now override required properties
                basketFeesEntity.BasketHeaderID = BasketEntity.Basket_Header_ID
                basketFeesEntity.FeeValue = CATFeeValue
                basketFeesEntity.IsTransactional = isTransactionBased
                BasketFeesEntityList.Add(basketFeesEntity)

            End If
        End Sub

        Private Sub ProcessValidCATFeesForCATBasket()
            If _validFeesCATForBasket IsNot Nothing AndAlso _validFeesCATForBasket.Count > 0 Then

                Dim canChargeTransactionType As Boolean = False
                Dim canChargeProductType As Boolean = False
                Dim canChargeTicketType As Boolean = False

                AssignChargingFeeTypeOnCAT(canChargeTransactionType, canChargeProductType, canChargeTicketType, ConsiderCATDetailsOnCATTypeStatus)

                Dim isTransactionBased As Boolean = False
                Dim CATFeeValue As Decimal = 0
                'highest transaction based fee value
                If canChargeTransactionType Then
                    For validFeeItemIndex As Integer = 0 To _validFeesCATForBasket.Count - 1
                        If _validFeesCATForBasket(validFeeItemIndex).FeeType = GlobalConstants.FEETYPE_TRANSACTION _
                            AndAlso _validFeesCATForBasket(validFeeItemIndex).ChargeType = GlobalConstants.FEECHARGETYPE_FIXED _
                            AndAlso CATFeeValue <= _validFeesCATForBasket(validFeeItemIndex).FeeValue Then
                            CATFeeValue = _validFeesCATForBasket(validFeeItemIndex).FeeValue
                            isTransactionBased = True
                        End If
                    Next
                End If

                'ticket type and product type fees
                If canChargeProductType OrElse canChargeTicketType Then
                    For basketItemIndex As Integer = 0 To BasketDetailMergedList.Count - 1
                        If IsProductNotExcludedFromFees(BasketDetailMergedList(basketItemIndex).Product) Then
                            CATFeeValue = GetFeeValueForBasketItem(False, _validFeesCATForBasket, BasketDetailMergedList(basketItemIndex), CATFeeValue, True, BasketHeaderMergedEntity, canChargeProductType, canChargeTicketType)
                        End If
                    Next
                End If


                Dim basketFeesEntity As DEBasketFees = CastDEFeesToDEBasketFees(_validFeesCATForBasket(0))
                'now override required properties
                basketFeesEntity.BasketHeaderID = BasketEntity.Basket_Header_ID
                basketFeesEntity.FeeValue = CATFeeValue
                basketFeesEntity.IsTransactional = isTransactionBased
                BasketFeesEntityList.Add(basketFeesEntity)

            End If
        End Sub

    End Class
End Namespace
