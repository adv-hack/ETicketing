Namespace TalentBasketFees
    <Serializable()> _
    Friend Class TalentBasketFeesFulfilment
        Inherits TalentBasketFees

        Private Const SSOURCECLASS As String = "TalentBasketFeesFulfilment"

        Private _validFeesFulfilmentForBasket As New List(Of DEFees)

        Public Property FulfilmentFeesList As List(Of DEFees) = Nothing

        Public Function ProcessFulfilmentFeesForBasket() As ErrorObj
            Dim errObj As New ErrorObj
            Try
                PopulateValidFeesFulfilment()
                If BasketEntity.CAT_MODE.Trim.Length > 0 AndAlso ConsiderCATDetailsOnCATTypeStatus <> GlobalConstants.FEE_CONSIDER_CAT_STATUS_STANDARD Then
                    ProcessValidFulfilmentFeesForCATBasket()
                Else
                    ProcessValidFulfilmentFeesForBasket()
                End If

            Catch ex As Exception
                errObj.HasError = True
                errObj.ErrorNumber = "TCTBFFUL-001"
                errObj.ErrorStatus = "While processing fulfilment fees for basket"
                LogError(SSOURCECLASS, "", errObj.ErrorNumber, errObj.ErrorStatus, ex)
            End Try
            Return errObj
        End Function

        Private Function GetMatchedIndexFromSatisfiedRule(ByVal departmentToMatch As String, ByVal fulfilmentFeeCode As String, ByVal productType As String, ByVal productCode As String, ByVal geographicalZone As String) As Integer
            Dim matchedIndex As Integer = -1
            For itemIndex As Integer = 0 To FulfilmentFeesList.Count - 1
                If FulfilmentFeesList(itemIndex).FeeDepartment = departmentToMatch _
                    AndAlso FulfilmentFeesList(itemIndex).FeeCode = fulfilmentFeeCode _
                    AndAlso FulfilmentFeesList(itemIndex).ProductType = productType _
                    AndAlso FulfilmentFeesList(itemIndex).ProductCode = productCode _
                    AndAlso FulfilmentFeesList(itemIndex).GeographicalZone = geographicalZone Then
                    matchedIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedIndex
        End Function

        Private Sub PopulateValidFeesFulfilment()

            If FulfilmentFeesList IsNot Nothing Then

                For basketItemIndex As Integer = 0 To BasketItemRequiresFees.Count - 1

                    If BasketItemRequiresFees(basketItemIndex).MODULE_ = GlobalConstants.BASKETMODULETICKETING AndAlso IsProductNotExcludedFromFees(BasketItemRequiresFees(basketItemIndex).Product) Then

                        Dim fulfilmentFeeCode As String = String.Empty
                        Dim tempFeeNotFound As Boolean = True
                        Dim itemIndex As Integer = 0
                        If Not FulfilmentFeeCategory.TryGetValue(BasketItemRequiresFees(basketItemIndex).CURR_FULFIL_SLCTN, fulfilmentFeeCode) Then
                            'assign a default code for the product which is available for that product
                            'is this scenerio will occur
                            'now occuring for package till new fulfilment logic developed
                        End If

                        'now start the filtering
                        If Not String.IsNullOrWhiteSpace(fulfilmentFeeCode) Then
                            'Department, Fulfilment Fee Code, Product Code, Product Type, Delivery Zone Code
                            itemIndex = GetMatchedIndexFromSatisfiedRule(Me.Department, fulfilmentFeeCode, BasketItemRequiresFees(basketItemIndex).PRODUCT_TYPE_ACTUAL, BasketItemRequiresFees(basketItemIndex).Product, BasketEntity.DELIVERY_ZONE_CODE)

                            If itemIndex < 0 Then
                                'Department, Fulfilment Fee Code, Product Code, Delivery Zone Code
                                itemIndex = GetMatchedIndexFromSatisfiedRule(Me.Department, fulfilmentFeeCode, "", BasketItemRequiresFees(basketItemIndex).Product, BasketEntity.DELIVERY_ZONE_CODE)
                            End If

                            If itemIndex < 0 Then
                                'Department, Fulfilment Fee Code, Product Type, Delivery Zone Code
                                itemIndex = GetMatchedIndexFromSatisfiedRule(Me.Department, fulfilmentFeeCode, BasketItemRequiresFees(basketItemIndex).PRODUCT_TYPE_ACTUAL, "", BasketEntity.DELIVERY_ZONE_CODE)
                            End If

                            If itemIndex < 0 Then
                                'Department, Fulfilment Fee Code, Delivery Zone Code
                                itemIndex = GetMatchedIndexFromSatisfiedRule(Me.Department, fulfilmentFeeCode, "", "", BasketEntity.DELIVERY_ZONE_CODE)
                            End If

                            If itemIndex < 0 Then
                                'Empty Department, Fulfilment Fee Code, Product Code, Product Type, Delivery Zone Code
                                itemIndex = GetMatchedIndexFromSatisfiedRule("", fulfilmentFeeCode, BasketItemRequiresFees(basketItemIndex).PRODUCT_TYPE_ACTUAL, BasketItemRequiresFees(basketItemIndex).Product, BasketEntity.DELIVERY_ZONE_CODE)
                            End If

                            If itemIndex < 0 Then
                                'Empty Department, Fulfilment Fee Code, Product Code, Delivery Zone Code
                                itemIndex = GetMatchedIndexFromSatisfiedRule("", fulfilmentFeeCode, "", BasketItemRequiresFees(basketItemIndex).Product, BasketEntity.DELIVERY_ZONE_CODE)
                            End If

                            If itemIndex < 0 Then
                                'Empty Department, Fulfilment Fee Code, Product Type, Delivery Zone Code
                                itemIndex = GetMatchedIndexFromSatisfiedRule("", fulfilmentFeeCode, BasketItemRequiresFees(basketItemIndex).PRODUCT_TYPE_ACTUAL, "", BasketEntity.DELIVERY_ZONE_CODE)
                            End If

                            If itemIndex < 0 Then
                                'Empty Department, Fulfilment Fee Code, Delivery Zone Code
                                itemIndex = GetMatchedIndexFromSatisfiedRule("", fulfilmentFeeCode, "", "", BasketEntity.DELIVERY_ZONE_CODE)
                            End If

                            If itemIndex < 0 Then
                                'Department, Fulfilment Fee Code, Product Code, Product Type, Empty Delivery Zone code
                                itemIndex = GetMatchedIndexFromSatisfiedRule(Me.Department, fulfilmentFeeCode, BasketItemRequiresFees(basketItemIndex).PRODUCT_TYPE_ACTUAL, BasketItemRequiresFees(basketItemIndex).Product, "")
                            End If

                            If itemIndex < 0 Then
                                'Department, Fulfilment Fee Code, Product Code, Empty Delivery Zone code
                                itemIndex = GetMatchedIndexFromSatisfiedRule(Me.Department, fulfilmentFeeCode, "", BasketItemRequiresFees(basketItemIndex).Product, "")
                            End If

                            If itemIndex < 0 Then
                                'Department, Fulfilment Fee Code, Product Type, Empty Delivery Zone code
                                itemIndex = GetMatchedIndexFromSatisfiedRule(Me.Department, fulfilmentFeeCode, BasketItemRequiresFees(basketItemIndex).PRODUCT_TYPE_ACTUAL, "", "")
                            End If

                            If itemIndex < 0 Then
                                'Department, Fulfilment Fee Code, Empty Delivery Zone code
                                itemIndex = GetMatchedIndexFromSatisfiedRule(Me.Department, fulfilmentFeeCode, "", "", "")
                            End If

                            If itemIndex < 0 Then
                                'Empty Department, Fulfilment Fee Code, Product Code, Product Type, Empty Delivery Zone code
                                itemIndex = GetMatchedIndexFromSatisfiedRule("", fulfilmentFeeCode, BasketItemRequiresFees(basketItemIndex).PRODUCT_TYPE_ACTUAL, BasketItemRequiresFees(basketItemIndex).Product, "")
                            End If

                            If itemIndex < 0 Then
                                'Empty Department, Fulfilment Fee Code, Product Code, Empty Delivery Zone code
                                itemIndex = GetMatchedIndexFromSatisfiedRule("", fulfilmentFeeCode, "", BasketItemRequiresFees(basketItemIndex).Product, "")
                            End If

                            If itemIndex < 0 Then
                                'Empty Department, Fulfilment Fee Code, Product Type, Empty Delivery Zone code
                                itemIndex = GetMatchedIndexFromSatisfiedRule("", fulfilmentFeeCode, BasketItemRequiresFees(basketItemIndex).PRODUCT_TYPE_ACTUAL, "", "")
                            End If

                            If itemIndex < 0 Then
                                'Empty Department, Fulfilment Fee Code, Empty Delivery Zone code
                                itemIndex = GetMatchedIndexFromSatisfiedRule("", fulfilmentFeeCode, "", "", "")
                            End If

                            'is fee found
                            If itemIndex > -1 Then
                                AddValidFeesFulfilmentForBasket(FulfilmentFeesList(itemIndex))
                            End If
                        End If

                    End If 'product not excluded if ends

                Next
            End If
        End Sub

        Private Sub AddValidFeesFulfilmentForBasket(ByVal feeEntity As DEFees)
            Dim canAddThisFeeEntity As Boolean = True
            If _validFeesFulfilmentForBasket.Count > 0 Then
                For itemIndex As Integer = 0 To _validFeesFulfilmentForBasket.Count - 1
                    If feeEntity.Equal(_validFeesFulfilmentForBasket(itemIndex)) Then
                        canAddThisFeeEntity = False
                        Exit For
                    End If
                Next
            End If
            If canAddThisFeeEntity Then
                _validFeesFulfilmentForBasket.Add(feeEntity)
            End If
        End Sub

        Private Sub ProcessValidFulfilmentFeesForBasket()
            If _validFeesFulfilmentForBasket IsNot Nothing AndAlso _validFeesFulfilmentForBasket.Count > 0 Then
                Dim fulfilmentFeeCodes As New List(Of String)

                For itemIndex As Integer = 0 To _validFeesFulfilmentForBasket.Count - 1
                    If Not fulfilmentFeeCodes.Contains(_validFeesFulfilmentForBasket(itemIndex).FeeCode) Then
                        fulfilmentFeeCodes.Add(_validFeesFulfilmentForBasket(itemIndex).FeeCode)
                    End If
                Next

                If fulfilmentFeeCodes.Count > 0 Then
                    Dim FeeValueFulfilmentForBasket As New List(Of DEFees)
                    For itemIndex As Integer = 0 To fulfilmentFeeCodes.Count - 1
                        Dim feeValueForThisFeeCode As Decimal = 0
                        'transaction based fee value
                        Dim isTransactionBased As Boolean = False
                        For validFeeItemIndex As Integer = 0 To _validFeesFulfilmentForBasket.Count - 1
                            If _validFeesFulfilmentForBasket(validFeeItemIndex).FeeCode = fulfilmentFeeCodes(itemIndex) _
                                AndAlso _validFeesFulfilmentForBasket(validFeeItemIndex).FeeType = GlobalConstants.FEETYPE_TRANSACTION _
                                AndAlso _validFeesFulfilmentForBasket(validFeeItemIndex).ChargeType = GlobalConstants.FEECHARGETYPE_FIXED _
                                AndAlso feeValueForThisFeeCode <= _validFeesFulfilmentForBasket(validFeeItemIndex).FeeValue Then
                                feeValueForThisFeeCode = _validFeesFulfilmentForBasket(validFeeItemIndex).FeeValue
                                isTransactionBased = True
                            End If
                        Next

                        Dim tempFulfilmentFeeCode As String = String.Empty
                        For basketItemIndex As Integer = 0 To BasketDetailMergedList.Count - 1
                            If IsProductNotExcludedFromFees(BasketDetailMergedList(basketItemIndex).Product) Then
                                'get fulfilment fee code
                                If BasketDetailMergedList(basketItemIndex).ModuleOfItem = GlobalConstants.BASKETMODULETICKETING AndAlso FulfilmentFeeCategory.TryGetValue(BasketDetailMergedList(basketItemIndex).Curr_Fulfil_slctn, tempFulfilmentFeeCode) Then
                                    'is fulfilment fee code = current fee code in the list then this basket item 
                                    'has to be calulcated for fees
                                    If tempFulfilmentFeeCode = fulfilmentFeeCodes(itemIndex) Then
                                        feeValueForThisFeeCode = GetFeeValueForBasketItem(False, _validFeesFulfilmentForBasket, BasketDetailMergedList(basketItemIndex), feeValueForThisFeeCode, fulfilmentFeeCodes(itemIndex), False, BasketHeaderMergedEntity, True, True)
                                    End If
                                End If
                            End If
                        Next



                        Dim descItemIndex As Integer = -1
                        For descItemIndex = 0 To _validFeesFulfilmentForBasket.Count - 1
                            If _validFeesFulfilmentForBasket(descItemIndex).FeeCode = fulfilmentFeeCodes(itemIndex) Then
                                Exit For
                            End If
                        Next

                        Dim basketFeesEntity As DEBasketFees = CastDEFeesToDEBasketFees(_validFeesFulfilmentForBasket(descItemIndex))
                        'now override required properties
                        basketFeesEntity.BasketHeaderID = BasketEntity.Basket_Header_ID
                        basketFeesEntity.FeeApplyType = GlobalConstants.FEEAPPLYTYPE_FULFILMENT
                        basketFeesEntity.FeeCode = fulfilmentFeeCodes(itemIndex)
                        basketFeesEntity.FeeValue = feeValueForThisFeeCode
                        basketFeesEntity.IsTransactional = isTransactionBased
                        BasketFeesEntityList.Add(basketFeesEntity)


                    Next
                End If
            End If
        End Sub

        Private Sub ProcessValidFulfilmentFeesForCATBasket()
            If _validFeesFulfilmentForBasket IsNot Nothing AndAlso _validFeesFulfilmentForBasket.Count > 0 Then

                Dim isFulfilmentChangedOnCAT As Boolean = False
                Dim isFulfilmentSameWithExtraTicket As Boolean = False
                For basketItemIndex As Integer = 0 To BasketDetailMergedList.Count - 1
                    If IsProductNotExcludedFromFees(BasketDetailMergedList(basketItemIndex).Product) Then
                        If BasketDetailMergedList(basketItemIndex).Curr_Fulfil_slctn.Trim <> BasketDetailMergedList(basketItemIndex).CAT_Fulfilment.Trim Then
                            isFulfilmentChangedOnCAT = True
                            Exit For
                        ElseIf BasketDetailMergedList(basketItemIndex).Curr_Fulfil_slctn.Trim = BasketDetailMergedList(basketItemIndex).CAT_Fulfilment.Trim Then
                            If BasketDetailMergedList(basketItemIndex).Quantity_After_CAT > 0 Then
                                isFulfilmentSameWithExtraTicket = True
                                Exit For
                            End If
                        End If
                    End If
                Next

                If (isFulfilmentChangedOnCAT OrElse isFulfilmentSameWithExtraTicket) Then
                    Dim canChargeTransactionType As Boolean = False
                    Dim canChargeProductType As Boolean = False
                    Dim canChargeTicketType As Boolean = False
                    Dim quanityToCharge As Integer = 0

                    AssignChargingFeeTypeOnCAT(canChargeTransactionType, canChargeProductType, canChargeTicketType, ConsiderCATDetailsOnCATTypeStatus)

                    If isFulfilmentSameWithExtraTicket Then
                        'if fulfilment is same we can chrage only for extra ticket if the fee type is TICKET
                        canChargeTransactionType = False
                        canChargeProductType = False
                    End If

                    Dim fulfilmentFeeCodes As New List(Of String)

                    For itemIndex As Integer = 0 To _validFeesFulfilmentForBasket.Count - 1
                        If Not fulfilmentFeeCodes.Contains(_validFeesFulfilmentForBasket(itemIndex).FeeCode) Then
                            fulfilmentFeeCodes.Add(_validFeesFulfilmentForBasket(itemIndex).FeeCode)
                        End If
                    Next

                    If fulfilmentFeeCodes.Count > 0 Then
                        Dim FeeValueFulfilmentForBasket As New List(Of DEFees)
                        For itemIndex As Integer = 0 To fulfilmentFeeCodes.Count - 1
                            Dim feeValueForThisFeeCode As Decimal = 0
                            'transaction based fee value
                            Dim isTransactionBased As Boolean = False
                            If canChargeTransactionType Then
                                For validFeeItemIndex As Integer = 0 To _validFeesFulfilmentForBasket.Count - 1
                                    If _validFeesFulfilmentForBasket(validFeeItemIndex).FeeCode = fulfilmentFeeCodes(itemIndex) _
                                        AndAlso _validFeesFulfilmentForBasket(validFeeItemIndex).FeeType = GlobalConstants.FEETYPE_TRANSACTION _
                                        AndAlso _validFeesFulfilmentForBasket(validFeeItemIndex).ChargeType = GlobalConstants.FEECHARGETYPE_FIXED _
                                        AndAlso feeValueForThisFeeCode <= _validFeesFulfilmentForBasket(validFeeItemIndex).FeeValue Then
                                        feeValueForThisFeeCode = _validFeesFulfilmentForBasket(validFeeItemIndex).FeeValue
                                        isTransactionBased = True
                                    End If
                                Next
                            End If


                            Dim tempFulfilmentFeeCode As String = String.Empty
                            If canChargeProductType OrElse canChargeTicketType Then
                                For basketItemIndex As Integer = 0 To BasketDetailMergedList.Count - 1
                                    If IsProductNotExcludedFromFees(BasketDetailMergedList(basketItemIndex).Product) Then
                                        'get fulfilment fee code
                                        If FulfilmentFeeCategory.TryGetValue(BasketDetailMergedList(basketItemIndex).Curr_Fulfil_slctn, tempFulfilmentFeeCode) Then
                                            'is fulfilment fee code = current fee code in the list then this basket item 
                                            'has to be calulcated for fees
                                            If tempFulfilmentFeeCode = fulfilmentFeeCodes(itemIndex) Then
                                                feeValueForThisFeeCode = GetFeeValueForBasketItem(False, _validFeesFulfilmentForBasket, BasketDetailMergedList(basketItemIndex), feeValueForThisFeeCode, fulfilmentFeeCodes(itemIndex), False, BasketHeaderMergedEntity, canChargeProductType, canChargeTicketType)
                                            End If
                                        End If
                                    End If
                                Next
                            End If


                            Dim descItemIndex As Integer = -1
                            For descItemIndex = 0 To _validFeesFulfilmentForBasket.Count - 1
                                If _validFeesFulfilmentForBasket(descItemIndex).FeeCode = fulfilmentFeeCodes(itemIndex) Then
                                    Exit For
                                End If
                            Next

                            Dim basketFeesEntity As DEBasketFees = CastDEFeesToDEBasketFees(_validFeesFulfilmentForBasket(descItemIndex))
                            'now override required properties
                            basketFeesEntity.BasketHeaderID = BasketEntity.Basket_Header_ID
                            basketFeesEntity.FeeApplyType = GlobalConstants.FEEAPPLYTYPE_FULFILMENT
                            basketFeesEntity.FeeCode = fulfilmentFeeCodes(itemIndex)
                            basketFeesEntity.FeeValue = feeValueForThisFeeCode
                            basketFeesEntity.IsTransactional = isTransactionBased
                            BasketFeesEntityList.Add(basketFeesEntity)

                        Next
                    End If
                End If
                
            End If
        End Sub

    End Class
End Namespace
