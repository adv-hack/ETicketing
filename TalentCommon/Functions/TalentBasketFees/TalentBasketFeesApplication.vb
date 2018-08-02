Namespace TalentBasketFees
    <Serializable()> _
    Friend Class TalentBasketFeesApplication
        Inherits TalentBasketFees

        Private Const SOURCECLASS As String = "TalentBasketFeesApplication"

        Private _validFeesApplicationTypeForBasket As New List(Of DEFees)

        Public Property ApplicationFeesList As List(Of DEFees) = Nothing

        Public Function ProcessApplicationFeesForBasket() As ErrorObj
            Dim errObj As New ErrorObj
            Try

                PopulateValidFeesApplication()
                If BasketEntity.CAT_MODE.Trim.Length > 0 AndAlso ConsiderCATDetailsOnCATTypeStatus <> GlobalConstants.FEE_CONSIDER_CAT_STATUS_STANDARD Then
                    ProcessValidApplicationFeesForCATBasket()
                Else
                    ProcessValidApplicationFeesForBasket()
                End If
            Catch ex As Exception
                errObj.HasError = True
                errObj.ErrorNumber = "TCTBFAPP-001"
                errObj.ErrorStatus = "While processing application fees websales or supplynet for basket"
                LogError(SOURCECLASS, "", errObj.ErrorNumber, errObj.ErrorStatus, ex)
            End Try
            Return errObj
        End Function

        Private Sub PopulateValidFeesApplication()
            If ApplicationFeesList IsNot Nothing AndAlso BasketItemRequiresFees IsNot Nothing Then
                For basketItemIndex As Integer = 0 To BasketItemRequiresFees.Count - 1
                    If IsProductNotExcludedFromFees(BasketItemRequiresFees(basketItemIndex).Product) AndAlso BasketItemRequiresFees(basketItemIndex).MODULE_ = GlobalConstants.BASKETMODULETICKETING Then
                        Dim matchedItemIndex As Integer = GetMatchedIndexFromSelectionHierarchy(BasketItemRequiresFees(basketItemIndex), ApplicationFeesList)
                        'fee found
                        If matchedItemIndex > -1 Then
                            AddValidFeesApplicationForBasket(ApplicationFeesList(matchedItemIndex))
                        End If
                    End If
                Next
            End If
        End Sub

        Private Sub AddValidFeesApplicationForBasket(ByVal feeEntity As DEFees)
            Dim canAddThisFeeEntity As Boolean = True
            If _validFeesApplicationTypeForBasket.Count > 0 Then
                For itemIndex As Integer = 0 To _validFeesApplicationTypeForBasket.Count - 1
                    If feeEntity.Equal(_validFeesApplicationTypeForBasket(itemIndex)) Then
                        canAddThisFeeEntity = False
                        Exit For
                    End If
                Next
            End If
            If canAddThisFeeEntity Then
                _validFeesApplicationTypeForBasket.Add(feeEntity)
            End If
        End Sub

        Private Sub ProcessValidApplicationFeesForBasket()
            If _validFeesApplicationTypeForBasket IsNot Nothing AndAlso _validFeesApplicationTypeForBasket.Count > 0 Then
                'highest transaction based fee value
                Dim isTransactionBased As Boolean = False
                Dim applicationFeeValue As Decimal = 0
                For validFeeItemIndex As Integer = 0 To _validFeesApplicationTypeForBasket.Count - 1
                    If _validFeesApplicationTypeForBasket(validFeeItemIndex).FeeType = GlobalConstants.FEETYPE_TRANSACTION _
                        AndAlso _validFeesApplicationTypeForBasket(validFeeItemIndex).ChargeType = GlobalConstants.FEECHARGETYPE_FIXED _
                        AndAlso applicationFeeValue <= _validFeesApplicationTypeForBasket(validFeeItemIndex).FeeValue Then
                        applicationFeeValue = _validFeesApplicationTypeForBasket(validFeeItemIndex).FeeValue
                        isTransactionBased = True
                    End If
                Next
                'ticket type and product type fees
                For basketItemIndex As Integer = 0 To BasketDetailMergedList.Count - 1
                    If BasketDetailMergedList(basketItemIndex).ModuleOfItem = GlobalConstants.BASKETMODULETICKETING AndAlso String.IsNullOrWhiteSpace(BasketDetailMergedList(basketItemIndex).FeeCategory) Then
                        If IsProductNotExcludedFromFees(BasketDetailMergedList(basketItemIndex).Product) Then
                            applicationFeeValue = GetFeeValueForBasketItem(False, _validFeesApplicationTypeForBasket, BasketDetailMergedList(basketItemIndex), applicationFeeValue, False, BasketHeaderMergedEntity, True, True)
                        End If
                    End If
                Next

                Dim basketFeesEntity As DEBasketFees = CastDEFeesToDEBasketFees(_validFeesApplicationTypeForBasket(0))
                'now override required properties
                basketFeesEntity.BasketHeaderID = BasketEntity.Basket_Header_ID
                basketFeesEntity.FeeValue = applicationFeeValue
                basketFeesEntity.IsTransactional = isTransactionBased
                BasketFeesEntityList.Add(basketFeesEntity)
            End If
        End Sub

        Private Sub ProcessValidApplicationFeesForCATBasket()
            If _validFeesApplicationTypeForBasket IsNot Nothing AndAlso _validFeesApplicationTypeForBasket.Count > 0 Then

                Dim canChargeTransactionType As Boolean = False
                Dim canChargeProductType As Boolean = False
                Dim canChargeTicketType As Boolean = False

                AssignChargingFeeTypeOnCAT(canChargeTransactionType, canChargeProductType, canChargeTicketType, ConsiderCATDetailsOnCATTypeStatus)

                Dim applicationFeeValue As Decimal = 0

                'highest transaction based fee value
                Dim isTransactionBased As Boolean = False
                If canChargeTransactionType Then
                    For validFeeItemIndex As Integer = 0 To _validFeesApplicationTypeForBasket.Count - 1
                        If _validFeesApplicationTypeForBasket(validFeeItemIndex).FeeType = GlobalConstants.FEETYPE_TRANSACTION _
                            AndAlso _validFeesApplicationTypeForBasket(validFeeItemIndex).ChargeType = GlobalConstants.FEECHARGETYPE_FIXED _
                            AndAlso applicationFeeValue <= _validFeesApplicationTypeForBasket(validFeeItemIndex).FeeValue Then
                            applicationFeeValue = _validFeesApplicationTypeForBasket(validFeeItemIndex).FeeValue
                            isTransactionBased = True
                        End If
                    Next
                End If

                'ticket type and product type fees
                If canChargeProductType OrElse canChargeTicketType Then
                    For basketItemIndex As Integer = 0 To BasketDetailMergedList.Count - 1
                        If BasketDetailMergedList(basketItemIndex).ModuleOfItem = GlobalConstants.BASKETMODULETICKETING AndAlso String.IsNullOrWhiteSpace(BasketDetailMergedList(basketItemIndex).FeeCategory) Then
                            If IsProductNotExcludedFromFees(BasketDetailMergedList(basketItemIndex).Product) Then
                                applicationFeeValue = GetFeeValueForBasketItem(False, _validFeesApplicationTypeForBasket, BasketDetailMergedList(basketItemIndex), applicationFeeValue, True, BasketHeaderMergedEntity, canChargeProductType, canChargeTicketType)
                            End If
                        End If
                    Next
                End If

                Dim basketFeesEntity As DEBasketFees = CastDEFeesToDEBasketFees(_validFeesApplicationTypeForBasket(0))
                'now override required properties
                basketFeesEntity.BasketHeaderID = BasketEntity.Basket_Header_ID
                basketFeesEntity.FeeValue = applicationFeeValue
                basketFeesEntity.IsTransactional = isTransactionBased
                BasketFeesEntityList.Add(basketFeesEntity)
            End If
        End Sub
    End Class
End Namespace
