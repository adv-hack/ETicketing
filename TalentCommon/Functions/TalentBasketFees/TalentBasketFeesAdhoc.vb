Namespace TalentBasketFees
    <Serializable()> _
    Friend Class TalentBasketFeesAdhoc
        Inherits TalentBasketFees

        Private Const SOURCECLASS As String = "TalentBasketFeesAdhoc"

        Private _validFeesAdhocForBasket As New List(Of DEFees)

        Public Property AdhocFeesList As List(Of DEFees) = Nothing

        Public Function ProcessAdhocFeesForBasket() As ErrorObj
            Dim errObj As New ErrorObj
            Try
                PopulateValidFeesAdhoc()
                If BasketEntity.CAT_MODE.Trim.Length > 0 AndAlso ConsiderCATDetailsOnCATTypeStatus <> GlobalConstants.FEE_CONSIDER_CAT_STATUS_STANDARD Then
                    ProcessValidAdhocFeesForCATBasket()
                Else
                    ProcessValidAdhocFeesForBasket()
                End If
            Catch ex As Exception
                errObj.HasError = True
                errObj.ErrorNumber = "TCTBFADH-001"
                errObj.ErrorStatus = "While processing adhoc fees for basket"
                LogError(SOURCECLASS, "", errObj.ErrorNumber, errObj.ErrorStatus, ex)
            End Try
            Return errObj
        End Function

        Private Sub PopulateValidFeesAdhoc()
            If AdhocFeesList IsNot Nothing AndAlso BasketItemRequiresFees IsNot Nothing Then
                Dim adhocFeeCodes As New List(Of String)
                For itemIndex As Integer = 0 To AdhocFeesList.Count - 1
                    If Not adhocFeeCodes.Contains(AdhocFeesList(itemIndex).FeeCode) Then
                        adhocFeeCodes.Add(AdhocFeesList(itemIndex).FeeCode)
                    End If
                Next
                For itemIndex As Integer = 0 To adhocFeeCodes.Count - 1
                    Dim feeCodeBasedAdhocFeesList As New List(Of DEFees)
                    For feeItemIndex As Integer = 0 To AdhocFeesList.Count - 1
                        If AdhocFeesList(feeItemIndex).FeeCode = adhocFeeCodes(itemIndex) Then
                            feeCodeBasedAdhocFeesList.Add(AdhocFeesList(feeItemIndex))
                        End If
                    Next
                    If feeCodeBasedAdhocFeesList.Count > 0 Then
                        For basketItemIndex As Integer = 0 To BasketItemRequiresFees.Count - 1
                            Dim matchedItemIndex As Integer = GetMatchedIndexFromSelectionHierarchy(BasketItemRequiresFees(basketItemIndex), feeCodeBasedAdhocFeesList)
                            'fee found
                            If matchedItemIndex > -1 Then
                                AddValidFeesAdhoc(feeCodeBasedAdhocFeesList(matchedItemIndex))
                            End If
                        Next
                    End If
                Next
            End If
        End Sub

        Private Sub AddValidFeesAdhoc(ByVal feeEntity As DEFees)
            Dim canAddThisFeeEntity As Boolean = True
            If _validFeesAdhocForBasket.Count > 0 Then
                For itemIndex As Integer = 0 To _validFeesAdhocForBasket.Count - 1
                    If feeEntity.Equal(_validFeesAdhocForBasket(itemIndex)) Then
                        canAddThisFeeEntity = False
                        Exit For
                    End If
                Next
            End If
            If canAddThisFeeEntity Then
                _validFeesAdhocForBasket.Add(feeEntity)
            End If
        End Sub

        Private Sub ProcessValidAdhocFeesForBasket()
            If _validFeesAdhocForBasket IsNot Nothing AndAlso _validFeesAdhocForBasket.Count > 0 Then

                Dim adhocFeeCodes As New List(Of String)
                For itemIndex As Integer = 0 To _validFeesAdhocForBasket.Count - 1
                    If Not adhocFeeCodes.Contains(_validFeesAdhocForBasket(itemIndex).FeeCode) Then
                        adhocFeeCodes.Add(_validFeesAdhocForBasket(itemIndex).FeeCode)
                    End If
                Next
                If adhocFeeCodes.Count > 0 Then
                    Dim FeeValueAdhocForBasket As New List(Of DEFees)
                    For itemIndex As Integer = 0 To adhocFeeCodes.Count - 1
                        Dim feeValueForThisFeeCode As Decimal = 0
                        'highest transaction based fee value
                        Dim isTransactionBased As Boolean = False
                        For validFeeItemIndex As Integer = 0 To _validFeesAdhocForBasket.Count - 1
                            If _validFeesAdhocForBasket(validFeeItemIndex).FeeCode = adhocFeeCodes(itemIndex) _
                                AndAlso _validFeesAdhocForBasket(validFeeItemIndex).FeeType = GlobalConstants.FEETYPE_TRANSACTION _
                                AndAlso _validFeesAdhocForBasket(validFeeItemIndex).ChargeType = GlobalConstants.FEECHARGETYPE_FIXED _
                                AndAlso feeValueForThisFeeCode <= _validFeesAdhocForBasket(validFeeItemIndex).FeeValue Then
                                feeValueForThisFeeCode = _validFeesAdhocForBasket(validFeeItemIndex).FeeValue
                                isTransactionBased = True
                            End If
                        Next
                        'ticket type and product type fees
                        For basketItemIndex As Integer = 0 To BasketDetailMergedList.Count - 1
                            feeValueForThisFeeCode = GetFeeValueForBasketItem(False, _validFeesAdhocForBasket, BasketDetailMergedList(basketItemIndex), feeValueForThisFeeCode, adhocFeeCodes(itemIndex), False, BasketHeaderMergedEntity, True, True)
                        Next

                        Dim descItemIndex As Integer = -1
                        For descItemIndex = 0 To _validFeesAdhocForBasket.Count - 1
                            If _validFeesAdhocForBasket(descItemIndex).FeeCode = adhocFeeCodes(itemIndex) Then
                                Exit For
                            End If
                        Next
                        Dim basketFeesEntity As DEBasketFees = CastDEFeesToDEBasketFees(_validFeesAdhocForBasket(descItemIndex))
                        'now override required properties
                        basketFeesEntity.BasketHeaderID = BasketEntity.Basket_Header_ID
                        basketFeesEntity.FeeCode = adhocFeeCodes(itemIndex)
                        basketFeesEntity.FeeValue = feeValueForThisFeeCode
                        basketFeesEntity.IsTransactional = isTransactionBased
                        BasketFeesEntityList.Add(basketFeesEntity)
                    Next
                End If
            End If
        End Sub

        Private Sub ProcessValidAdhocFeesForCATBasket()
            If _validFeesAdhocForBasket IsNot Nothing AndAlso _validFeesAdhocForBasket.Count > 0 Then

                Dim canChargeTransactionType As Boolean = False
                Dim canChargeProductType As Boolean = False
                Dim canChargeTicketType As Boolean = False

                AssignChargingFeeTypeOnCAT(canChargeTransactionType, canChargeProductType, canChargeTicketType, ConsiderCATDetailsOnCATTypeStatus)

                Dim adhocFeeCodes As New List(Of String)
                For itemIndex As Integer = 0 To _validFeesAdhocForBasket.Count - 1
                    If Not adhocFeeCodes.Contains(_validFeesAdhocForBasket(itemIndex).FeeCode) Then
                        adhocFeeCodes.Add(_validFeesAdhocForBasket(itemIndex).FeeCode)
                    End If
                Next
                If adhocFeeCodes.Count > 0 Then
                    Dim FeeValueAdhocForBasket As New List(Of DEFees)
                    For itemIndex As Integer = 0 To adhocFeeCodes.Count - 1
                        Dim feeValueForThisFeeCode As Decimal = 0
                        'highest transaction based fee value
                        Dim isTransactionBased As Boolean = False
                        If canChargeTransactionType Then
                            For validFeeItemIndex As Integer = 0 To _validFeesAdhocForBasket.Count - 1
                                If _validFeesAdhocForBasket(validFeeItemIndex).FeeCode = adhocFeeCodes(itemIndex) _
                                    AndAlso _validFeesAdhocForBasket(validFeeItemIndex).FeeType = GlobalConstants.FEETYPE_TRANSACTION _
                                    AndAlso _validFeesAdhocForBasket(validFeeItemIndex).ChargeType = GlobalConstants.FEECHARGETYPE_FIXED _
                                    AndAlso feeValueForThisFeeCode <= _validFeesAdhocForBasket(validFeeItemIndex).FeeValue Then
                                    feeValueForThisFeeCode = _validFeesAdhocForBasket(validFeeItemIndex).FeeValue
                                    isTransactionBased = True
                                End If
                            Next
                        End If

                        'ticket type and product type fees
                        If canChargeProductType OrElse canChargeTicketType Then
                            For basketItemIndex As Integer = 0 To BasketDetailMergedList.Count - 1
                                feeValueForThisFeeCode = GetFeeValueForBasketItem(False, _validFeesAdhocForBasket, BasketDetailMergedList(basketItemIndex), feeValueForThisFeeCode, adhocFeeCodes(itemIndex), True, BasketHeaderMergedEntity, canChargeProductType, canChargeTicketType)
                            Next
                        End If


                        Dim descItemIndex As Integer = -1
                        For descItemIndex = 0 To _validFeesAdhocForBasket.Count - 1
                            If _validFeesAdhocForBasket(descItemIndex).FeeCode = adhocFeeCodes(itemIndex) Then
                                Exit For
                            End If
                        Next
                        Dim basketFeesEntity As DEBasketFees = CastDEFeesToDEBasketFees(_validFeesAdhocForBasket(descItemIndex))
                        'now override required properties
                        basketFeesEntity.BasketHeaderID = BasketEntity.Basket_Header_ID
                        basketFeesEntity.FeeCode = adhocFeeCodes(itemIndex)
                        basketFeesEntity.FeeValue = feeValueForThisFeeCode
                        basketFeesEntity.IsTransactional = isTransactionBased
                        BasketFeesEntityList.Add(basketFeesEntity)
                    Next
                End If
            End If
        End Sub

    End Class
End Namespace
