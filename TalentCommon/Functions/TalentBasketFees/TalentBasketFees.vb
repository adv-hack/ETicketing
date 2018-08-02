Namespace TalentBasketFees
    <Serializable()> _
    Friend Class TalentBasketFees
        Inherits TalentBase

        Private _basketFeesEntityList As List(Of DEBasketFees) = Nothing
        Public Property Department() As String = ""
        Public Property BasketEntity() As DEBasket = Nothing
        Public Property BasketItemEntityList() As List(Of DEBasketItem) = Nothing
        Public Property BasketItemRequiresFees As List(Of DEBasketItem) = Nothing
        Public Property BasketDetailMergedList As List(Of DEBasketMergedDetail) = Nothing
        Public Property BasketHeaderMergedEntity As DEBasketMergedHeader = Nothing
        Public Property ProductExcludeFees() As Dictionary(Of String, String) = Nothing
        Public Property ConsiderCATDetailsOnCATTypeStatus() As String = "NOTHING"

        Public Property BasketFeesEntityList As List(Of DEBasketFees)
            Set(ByVal value As List(Of DEBasketFees))
                _basketFeesEntityList = value
            End Set
            Get
                Return _basketFeesEntityList
            End Get
        End Property

        Public Sub New()
            _basketFeesEntityList = New List(Of DEBasketFees)
        End Sub

        Protected Function CastDEFeesToDEBasketFees(ByVal feesEntity As DEFees) As DEBasketFees
            Dim basketFeesEntity As New DEBasketFees
            basketFeesEntity.FeeCode = feesEntity.FeeCode
            basketFeesEntity.FeeDescription = feesEntity.FeeDescription
            basketFeesEntity.FeeDepartment = feesEntity.FeeDepartment
            basketFeesEntity.FeeType = feesEntity.FeeType
            basketFeesEntity.FeeCategory = feesEntity.FeeCategory
            basketFeesEntity.FeeValue = feesEntity.FeeValue
            basketFeesEntity.CardType = feesEntity.CardType
            basketFeesEntity.IsNegativeFee = feesEntity.IsNegativeFee
            basketFeesEntity.IsSystemFee = feesEntity.IsSystemFee
            basketFeesEntity.IsTransactional = feesEntity.IsTransactional
            basketFeesEntity.IsChargeable = feesEntity.IsChargeable
            basketFeesEntity.FeeFunction = feesEntity.FeeFunction
            basketFeesEntity.ApplyFeeTo = feesEntity.ApplyFeeTo
            basketFeesEntity.ChargeType = feesEntity.ChargeType
            basketFeesEntity.GeographicalZone = feesEntity.GeographicalZone
            Return basketFeesEntity
        End Function

        Protected Function IsExistsInBasket(ByVal feeCode As String)
            Dim isExists As Boolean = False
            If BasketItemEntityList IsNot Nothing AndAlso BasketItemEntityList.Count > 0 Then
                For itemIndex As Integer = 0 To BasketItemEntityList.Count - 1
                    If BasketItemEntityList(itemIndex).Product = feeCode Then
                        isExists = True
                    End If
                Next
            End If
            Return isExists
        End Function

        Protected Function IsExistsInBasket(ByVal feeEntity As DEFees)
            Dim isExists As Boolean = False
            If BasketItemEntityList IsNot Nothing AndAlso BasketItemEntityList.Count > 0 Then
                For itemIndex As Integer = 0 To BasketItemEntityList.Count - 1
                    If BasketItemEntityList(itemIndex).Product = feeEntity.ProductCode Then
                        isExists = True
                    End If
                Next
            End If
            Return isExists
        End Function

        Protected Function IsProductNotExcludedFromFees(ByVal productCode As String) As Boolean
            Dim isNotExcluded As Boolean = True
            If ProductExcludeFees IsNot Nothing And ProductExcludeFees.Count > 0 Then
                If ProductExcludeFees.ContainsKey(productCode) Then
                    isNotExcluded = False
                End If
            End If
            Return isNotExcluded
        End Function

        Protected Function GetMatchedIndexFromSelectionHierarchy(ByVal basketItem As DEBasketItem, ByVal feeEntityList As List(Of DEFees)) As Integer
            Dim matchedItemIndex As Integer = -1
            matchedItemIndex = isFeeSelectionRule1(feeEntityList, basketItem)
            If matchedItemIndex < 0 Then
                matchedItemIndex = isFeeSelectionRule2(feeEntityList, basketItem)
            End If
            If matchedItemIndex < 0 Then
                matchedItemIndex = isFeeSelectionRule3(feeEntityList, basketItem)
            End If
            If matchedItemIndex < 0 Then
                matchedItemIndex = isFeeSelectionRule4(feeEntityList, basketItem)
            End If
            If matchedItemIndex < 0 Then
                matchedItemIndex = isFeeSelectionRule5(feeEntityList, basketItem)
            End If
            If matchedItemIndex < 0 Then
                matchedItemIndex = isFeeSelectionRule6(feeEntityList, basketItem)
            End If
            If matchedItemIndex < 0 Then
                matchedItemIndex = isFeeSelectionRule7(feeEntityList, basketItem)
            End If
            If matchedItemIndex < 0 Then
                matchedItemIndex = isFeeSelectionRule8(feeEntityList, basketItem)
            End If
            Return matchedItemIndex
        End Function

        Protected Function GetMatchedIndexFromSelectionHierarchy(ByVal basketItem As DEBasketItem, ByVal feeEntityList As List(Of DEFees), ByVal cardType As String) As Integer
            Dim matchedItemIndex As Integer = -1
            matchedItemIndex = isFeeSelectionRule1(feeEntityList, basketItem, cardType)
            If matchedItemIndex < 0 Then
                matchedItemIndex = isFeeSelectionRule2(feeEntityList, basketItem, cardType)
            End If
            If matchedItemIndex < 0 Then
                matchedItemIndex = isFeeSelectionRule3(feeEntityList, basketItem, cardType)
            End If
            If matchedItemIndex < 0 Then
                matchedItemIndex = isFeeSelectionRule4(feeEntityList, basketItem, cardType)
            End If
            If matchedItemIndex < 0 Then
                matchedItemIndex = isFeeSelectionRule5(feeEntityList, basketItem, cardType)
            End If
            If matchedItemIndex < 0 Then
                matchedItemIndex = isFeeSelectionRule6(feeEntityList, basketItem, cardType)
            End If
            If matchedItemIndex < 0 Then
                matchedItemIndex = isFeeSelectionRule7(feeEntityList, basketItem, cardType)
            End If
            If matchedItemIndex < 0 Then
                matchedItemIndex = isFeeSelectionRule8(feeEntityList, basketItem, cardType)
            End If
            If matchedItemIndex < 0 Then
                matchedItemIndex = isFeeSelectionRule9(feeEntityList, basketItem, cardType)
            End If
            If matchedItemIndex < 0 Then
                matchedItemIndex = isFeeSelectionRule10(feeEntityList, basketItem, cardType)
            End If
            If matchedItemIndex < 0 Then
                matchedItemIndex = isFeeSelectionRule11(feeEntityList, basketItem, cardType)
            End If
            If matchedItemIndex < 0 Then
                matchedItemIndex = isFeeSelectionRule12(feeEntityList, basketItem, cardType)
            End If
            If matchedItemIndex < 0 Then
                matchedItemIndex = isFeeSelectionRule13(feeEntityList, basketItem, cardType)
            End If
            If matchedItemIndex < 0 Then
                matchedItemIndex = isFeeSelectionRule14(feeEntityList, basketItem, cardType)
            End If
            If matchedItemIndex < 0 Then
                matchedItemIndex = isFeeSelectionRule15(feeEntityList, basketItem, cardType)
            End If
            If matchedItemIndex < 0 Then
                matchedItemIndex = isFeeSelectionRule16(feeEntityList, basketItem, cardType)
            End If
            Return matchedItemIndex
        End Function

        Protected Function GetFeeValueFeeEntity(ByVal feeList As List(Of DEFees), ByVal basketDetailMerged As DEBasketMergedDetail) As DEFees
            Dim matchedFeeEntity As DEFees = Nothing
            'check for product type, product code matching
            For itemIndex As Integer = 0 To feeList.Count - 1
                If isFeeValueSatisfiesRule(feeList(itemIndex), basketDetailMerged.Product, basketDetailMerged.Product_Type_Actual) Then
                    matchedFeeEntity = feeList(itemIndex)
                End If
            Next

            If matchedFeeEntity Is Nothing Then
                'check for product code matching
                For itemIndex As Integer = 0 To feeList.Count - 1
                    If isFeeValueSatisfiesRule(feeList(itemIndex), basketDetailMerged.Product, "") Then
                        matchedFeeEntity = feeList(itemIndex)
                    End If
                Next
            End If

            If matchedFeeEntity Is Nothing Then
                'check for product type matching
                For itemIndex As Integer = 0 To feeList.Count - 1
                    If isFeeValueSatisfiesRule(feeList(itemIndex), "", basketDetailMerged.Product_Type_Actual) Then
                        matchedFeeEntity = feeList(itemIndex)
                    End If
                Next
            End If

            If matchedFeeEntity Is Nothing Then
                'no product code, no product type match so only the feecode
                For itemIndex As Integer = 0 To feeList.Count - 1
                    If isFeeValueSatisfiesRule(feeList(itemIndex), "", "") Then
                        matchedFeeEntity = feeList(itemIndex)
                    End If
                Next
            End If

            Return matchedFeeEntity

        End Function

        Protected Function GetFeeValueFeeEntity(ByVal feeList As List(Of DEFees), ByVal basketDetailMerged As DEBasketMergedDetail, ByVal feeCode As String) As DEFees
            Dim matchedFeeEntity As DEFees = Nothing

            'check for product type, product code matching
            For itemIndex As Integer = 0 To feeList.Count - 1
                If feeList(itemIndex).FeeCode = feeCode AndAlso isFeeValueSatisfiesRule(feeList(itemIndex), basketDetailMerged.Product, basketDetailMerged.Product_Type_Actual) Then
                    matchedFeeEntity = feeList(itemIndex)
                End If
            Next

            If matchedFeeEntity Is Nothing Then
                'check for product code matching
                For itemIndex As Integer = 0 To feeList.Count - 1
                    If feeList(itemIndex).FeeCode = feeCode AndAlso isFeeValueSatisfiesRule(feeList(itemIndex), basketDetailMerged.Product, "") Then
                        matchedFeeEntity = feeList(itemIndex)
                    End If
                Next
            End If

            If matchedFeeEntity Is Nothing Then
                'check for product type matching
                For itemIndex As Integer = 0 To feeList.Count - 1
                    If feeList(itemIndex).FeeCode = feeCode AndAlso isFeeValueSatisfiesRule(feeList(itemIndex), "", basketDetailMerged.Product_Type_Actual) Then
                        matchedFeeEntity = feeList(itemIndex)
                    End If
                Next
            End If
            If matchedFeeEntity Is Nothing Then
                'no product code, no product type match so only the feecode
                For itemIndex As Integer = 0 To feeList.Count - 1
                    If feeList(itemIndex).FeeCode = feeCode AndAlso isFeeValueSatisfiesRule(feeList(itemIndex), "", "") Then
                        matchedFeeEntity = feeList(itemIndex)
                    End If
                Next
            End If
            Return matchedFeeEntity
        End Function

        Protected Function GetFeeValueForBasketItem(ByVal isItBookingFee As Boolean, ByVal feeList As List(Of DEFees), ByVal basketDetailMerged As DEBasketMergedDetail, ByVal feeValue As Decimal, ByVal feeCode As String, ByVal isCATDetailsConsidered As Boolean, ByVal basketHeaderMergedEntity As DEBasketMergedHeader, ByVal canChargeProductType As Boolean, ByVal canChargeTicketType As Boolean) As Decimal

            Dim matchedFeeEntity As DEFees = GetFeeValueFeeEntity(feeList, basketDetailMerged, feeCode)

            feeValue = GetFeeValueForBasketItem(isItBookingFee, matchedFeeEntity, basketDetailMerged, feeValue, isCATDetailsConsidered, basketHeaderMergedEntity, canChargeProductType, canChargeTicketType)

            Return feeValue

        End Function

        Protected Function GetFeeValueForBasketItem(ByVal isItBookingFee As Boolean, ByVal feeList As List(Of DEFees), ByVal basketDetailMerged As DEBasketMergedDetail, ByVal feeValue As Decimal, ByVal isCATDetailsConsidered As Boolean, ByVal basketHeaderMergedEntity As DEBasketMergedHeader, ByVal canChargeProductType As Boolean, ByVal canChargeTicketType As Boolean) As Decimal

            Dim matchedFeeEntity As DEFees = GetFeeValueFeeEntity(feeList, basketDetailMerged)

            feeValue = GetFeeValueForBasketItem(isItBookingFee, matchedFeeEntity, basketDetailMerged, feeValue, isCATDetailsConsidered, basketHeaderMergedEntity, canChargeProductType, canChargeTicketType)

            Return feeValue

        End Function

        Private Function CanAllowFeeEntityToCharge(ByVal basketDetailMerged As DEBasketMergedDetail, ByVal matchedFeeEntity As DEFees) As Boolean
            Dim canAllow As Boolean = False
            If basketDetailMerged.ModuleOfItem = GlobalConstants.BASKETMODULETICKETING AndAlso
                     (matchedFeeEntity.ApplyFeeTo = GlobalConstants.FEEAPPLYTO_TICKETING OrElse
                     matchedFeeEntity.ApplyFeeTo = GlobalConstants.FEEAPPLYTO_BOTH) Then
                canAllow = True
            ElseIf basketDetailMerged.ModuleOfItem = GlobalConstants.BASKETMODULEMERCHANDISE AndAlso
                 (matchedFeeEntity.ApplyFeeTo = GlobalConstants.FEEAPPLYTO_MERCHANDISE OrElse
                 matchedFeeEntity.ApplyFeeTo = GlobalConstants.FEEAPPLYTO_BOTH) Then
                canAllow = True
            End If
            Return canAllow
        End Function

        ''' <summary>
        ''' Get the value of the fee based on whether or not the fee can be charged, whether it is a product type or ticket type fee, whether it is "F" for Fixed (or percentage) and whether it is a CAT basket or not.
        ''' If the basket is CAT the quantity of tickets changes so the calculation to work out the fee value could vary.
        ''' </summary>
        ''' <param name="isItBookingFee">Is this a booking fee or not</param>
        ''' <param name="matchedFeeEntity">The matching fee data entity</param>
        ''' <param name="basketDetailMerged">The current basket item (this could be many of the same tickets) Eg. 3 x seats for 1 unique home game</param>
        ''' <param name="feeValue">The default fee value being worked with</param>
        ''' <param name="isCATDetailsConsidered">Is this a CAT basket?</param>
        ''' <param name="basketHeaderMergedEntity">The current merged basket header</param>
        ''' <param name="canChargeProductType">Can this fee charge by product type</param>
        ''' <param name="canChargeTicketType">Can this fee be charged by ticket type</param>
        ''' <returns>The fee value</returns>
        ''' <remarks></remarks>
        Private Function GetFeeValueForBasketItem(ByVal isItBookingFee As Boolean, ByVal matchedFeeEntity As DEFees, ByVal basketDetailMerged As DEBasketMergedDetail, ByVal feeValue As Decimal, ByVal isCATDetailsConsidered As Boolean, ByVal basketHeaderMergedEntity As DEBasketMergedHeader, ByVal canChargeProductType As Boolean, ByVal canChargeTicketType As Boolean) As Decimal
            If matchedFeeEntity IsNot Nothing Then
                If CanAllowFeeEntityToCharge(basketDetailMerged, matchedFeeEntity) Then
                    If canChargeProductType AndAlso (matchedFeeEntity.FeeType = GlobalConstants.FEETYPE_PRODUCT) Then
                        If matchedFeeEntity.ChargeType = GlobalConstants.FEECHARGETYPE_FIXED Then
                            feeValue += matchedFeeEntity.FeeValue
                            If isItBookingFee Then basketDetailMerged.IsProcessedForBookingFee = True
                        Else
                            'charge type is percentage as it is ticket based percentage 
                            'we have to calculate on the total ticket price of that product type
                            If isCATDetailsConsidered Then
                                feeValue += basketDetailMerged.Price_After_CAT * (matchedFeeEntity.FeeValue / 100)
                            Else
                                feeValue += basketDetailMerged.Price * (matchedFeeEntity.FeeValue / 100)
                            End If
                            If isItBookingFee Then basketDetailMerged.IsProcessedForBookingFee = True
                        End If
                    ElseIf canChargeTicketType AndAlso (matchedFeeEntity.FeeType = GlobalConstants.FEETYPE_TICKET) Then
                        If matchedFeeEntity.ChargeType = GlobalConstants.FEECHARGETYPE_FIXED Then
                            If isCATDetailsConsidered Then
                                feeValue += (matchedFeeEntity.FeeValue * basketDetailMerged.Quantity_After_CAT)
                            Else
                                feeValue += (matchedFeeEntity.FeeValue * basketDetailMerged.Quantity)
                            End If

                            If isItBookingFee Then basketDetailMerged.IsProcessedForBookingFee = True
                        Else
                            'charge type is percentage as it is ticket based percentage 
                            'we have to calculate on the total ticket price of that product type
                            If isCATDetailsConsidered Then
                                feeValue += basketDetailMerged.Price_After_CAT * (matchedFeeEntity.FeeValue / 100)
                            Else
                                feeValue += basketDetailMerged.Price * (matchedFeeEntity.FeeValue / 100)
                            End If
                            If isItBookingFee Then basketDetailMerged.IsProcessedForBookingFee = True
                        End If
                    End If
                End If
            End If

            Return feeValue

        End Function
        Protected Function GetFeeValueForCATBasketItem(ByVal feeList As List(Of DEFees), ByVal basketDetailMerged As DEBasketMergedDetail, ByVal feeValue As Decimal, ByVal feeCode As String, ByVal quantity As Integer, ByVal canChargeProductType As Boolean, ByVal canChargeTicketType As Boolean) As Decimal

            Dim matchedFeeEntity As DEFees = GetFeeValueFeeEntity(feeList, basketDetailMerged, feeCode)

            If matchedFeeEntity IsNot Nothing Then
                If canChargeProductType AndAlso matchedFeeEntity.FeeType = GlobalConstants.FEETYPE_PRODUCT Then
                    If basketDetailMerged.ModuleOfItem = GlobalConstants.BASKETMODULETICKETING AndAlso
                             (matchedFeeEntity.ApplyFeeTo = GlobalConstants.FEEAPPLYTO_TICKETING OrElse
                             matchedFeeEntity.ApplyFeeTo = GlobalConstants.FEEAPPLYTO_BOTH) Then
                        feeValue += matchedFeeEntity.FeeValue
                    ElseIf basketDetailMerged.ModuleOfItem = GlobalConstants.BASKETMODULEMERCHANDISE AndAlso
                         (matchedFeeEntity.ApplyFeeTo = GlobalConstants.FEEAPPLYTO_MERCHANDISE OrElse
                         matchedFeeEntity.ApplyFeeTo = GlobalConstants.FEEAPPLYTO_BOTH) Then
                        feeValue += matchedFeeEntity.FeeValue
                    End If
                ElseIf canChargeTicketType AndAlso matchedFeeEntity.FeeType = GlobalConstants.FEETYPE_TICKET Then
                    If basketDetailMerged.ModuleOfItem = GlobalConstants.BASKETMODULETICKETING AndAlso
                             (matchedFeeEntity.ApplyFeeTo = GlobalConstants.FEEAPPLYTO_TICKETING OrElse
                             matchedFeeEntity.ApplyFeeTo = GlobalConstants.FEEAPPLYTO_BOTH) Then
                        feeValue += (matchedFeeEntity.FeeValue * quantity)
                    ElseIf basketDetailMerged.ModuleOfItem = GlobalConstants.BASKETMODULEMERCHANDISE AndAlso
                         (matchedFeeEntity.ApplyFeeTo = GlobalConstants.FEEAPPLYTO_MERCHANDISE OrElse
                         matchedFeeEntity.ApplyFeeTo = GlobalConstants.FEEAPPLYTO_BOTH) Then
                        feeValue += (matchedFeeEntity.FeeValue * quantity)
                    End If
                End If
            End If

            Return feeValue

        End Function

        Protected Function GetFeeValueForCATBasketItem(ByVal feeList As List(Of DEFees), ByVal basketDetailMerged As DEBasketMergedDetail, ByVal feeValue As Decimal, ByVal quantity As Integer, ByVal canChargeProductType As Boolean, ByVal canChargeTicketType As Boolean) As Decimal

            Dim matchedFeeEntity As DEFees = GetFeeValueFeeEntity(feeList, basketDetailMerged)

            If matchedFeeEntity IsNot Nothing Then
                If canChargeProductType AndAlso matchedFeeEntity.FeeType = GlobalConstants.FEETYPE_PRODUCT Then
                    feeValue += matchedFeeEntity.FeeValue
                ElseIf canChargeTicketType AndAlso matchedFeeEntity.FeeType = GlobalConstants.FEETYPE_TICKET Then
                    If basketDetailMerged.ModuleOfItem = GlobalConstants.BASKETMODULETICKETING AndAlso
                             (matchedFeeEntity.ApplyFeeTo = GlobalConstants.FEEAPPLYTO_TICKETING OrElse
                             matchedFeeEntity.ApplyFeeTo = GlobalConstants.FEEAPPLYTO_BOTH) Then
                        feeValue += (matchedFeeEntity.FeeValue * quantity)
                    ElseIf basketDetailMerged.ModuleOfItem = GlobalConstants.BASKETMODULEMERCHANDISE AndAlso
                         (matchedFeeEntity.ApplyFeeTo = GlobalConstants.FEEAPPLYTO_MERCHANDISE OrElse
                         matchedFeeEntity.ApplyFeeTo = GlobalConstants.FEEAPPLYTO_BOTH) Then
                        feeValue += (matchedFeeEntity.FeeValue * quantity)
                    End If
                End If
            End If

            Return feeValue

        End Function

        Protected Sub LogError(ByVal sourceClass As String, ByVal sourceMethod As String, ByVal errorCode As String, ByVal errorMessage As String)

        End Sub

        Protected Sub LogError(ByVal sourceClass As String, ByVal sourceMethod As String, ByVal errorCode As String, ByVal errorMessage As String, ByVal errObj As ErrorObj)

        End Sub

        Protected Sub LogError(ByVal sourceClass As String, ByVal sourceMethod As String, ByVal errorCode As String, ByVal errorMessage As String, ByVal ex As Exception)

        End Sub

        Protected Sub AssignChargingFeeTypeOnCAT(ByRef canChargeTransactionType As Boolean, ByRef canChargeProductType As Boolean, ByRef canChargeTicketType As Boolean, ByVal ConsiderCATStatus As String)
            canChargeTransactionType = False
            canChargeProductType = False
            canChargeTicketType = False
            Select Case ConsiderCATStatus
                Case GlobalConstants.FEE_CONSIDER_CAT_ALL_TYPE
                    canChargeTransactionType = True
                    canChargeProductType = True
                    canChargeTicketType = True
                Case GlobalConstants.FEE_CONSIDER_CAT_TRAN_TYPE
                    canChargeTransactionType = True
                Case GlobalConstants.FEE_CONSIDER_CAT_PDT_TYPE
                    canChargeProductType = True
                Case GlobalConstants.FEE_CONSIDER_CAT_TKT_TYPE
                    canChargeTicketType = True
                Case GlobalConstants.FEE_CONSIDER_CAT_BOTH_TRAN_PROD_TYPE
                    canChargeTransactionType = True
                    canChargeProductType = True
                Case GlobalConstants.FEE_CONSIDER_CAT_BOTH_TRAN_TKT_TYPE
                    canChargeTransactionType = True
                    canChargeTicketType = True
                Case GlobalConstants.FEE_CONSIDER_CAT_BOTH_PROD_TKT_TYPE
                    canChargeProductType = True
                    canChargeTicketType = True
            End Select
        End Sub

        Private Function isFeeValueSatisfiesRule(ByVal feeEntity As DEFees, ByVal productCode As String, ByVal productType As String) As Boolean
            Dim isMatching As Boolean = False
            If feeEntity.ProductType = productType _
                AndAlso feeEntity.ProductCode = productCode Then
                isMatching = True
            End If
            Return isMatching
        End Function

        Private Function isFeeSatisfiesRule(ByVal feeEntity As DEFees, ByVal department As String, ByVal productCode As String, ByVal productType As String) As Boolean
            Dim isMatching As Boolean = False
            If feeEntity.FeeDepartment = department _
                AndAlso feeEntity.ProductType = productType _
                AndAlso feeEntity.ProductCode = productCode Then
                isMatching = True
            End If
            Return isMatching
        End Function

        Private Function isFeeSatisfiesRule(ByVal feeEntity As DEFees, ByVal department As String, ByVal productCode As String, ByVal productType As String, ByVal cardType As String) As Boolean
            Dim isMatching As Boolean = False
            If feeEntity.FeeDepartment = department _
                AndAlso feeEntity.CardType = cardType _
                AndAlso feeEntity.ProductType = productType _
                AndAlso feeEntity.ProductCode = productCode Then
                isMatching = True
            End If
            Return isMatching
        End Function

        Private Function isFeeSelectionRule1(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem, ByVal cardType As String) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), Me.Department, basketItemEntity.Product, basketItemEntity.PRODUCT_TYPE_ACTUAL, cardType) Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule2(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem, ByVal cardType As String) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), Me.Department, basketItemEntity.Product, "", cardType) Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule3(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem, ByVal cardType As String) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), Me.Department, "", basketItemEntity.PRODUCT_TYPE_ACTUAL, cardType) Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule4(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem, ByVal cardType As String) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), Me.Department, "", "", cardType) Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule5(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem, ByVal cardType As String) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), Me.Department, basketItemEntity.Product, basketItemEntity.PRODUCT_TYPE_ACTUAL, "") Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule6(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem, ByVal cardType As String) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), Me.Department, basketItemEntity.Product, "", "") Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule7(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem, ByVal cardType As String) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), Me.Department, "", basketItemEntity.PRODUCT_TYPE_ACTUAL, "") Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule8(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem, ByVal cardType As String) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), Me.Department, "", "", "") Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule9(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem, ByVal cardType As String) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), "", basketItemEntity.Product, basketItemEntity.PRODUCT_TYPE_ACTUAL, cardType) Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule10(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem, ByVal cardType As String) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), "", basketItemEntity.Product, "", cardType) Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule11(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem, ByVal cardType As String) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), "", "", basketItemEntity.PRODUCT_TYPE_ACTUAL, cardType) Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule12(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem, ByVal cardType As String) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), "", "", "", cardType) Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule13(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem, ByVal cardType As String) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), "", basketItemEntity.Product, basketItemEntity.PRODUCT_TYPE_ACTUAL, "") Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule14(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem, ByVal cardType As String) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), "", basketItemEntity.Product, "", "") Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule15(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem, ByVal cardType As String) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), "", "", basketItemEntity.PRODUCT_TYPE_ACTUAL, "") Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule16(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem, ByVal cardType As String) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), "", "", "", "") Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule1(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), Me.Department, basketItemEntity.Product, basketItemEntity.PRODUCT_TYPE_ACTUAL) Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule2(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), Me.Department, basketItemEntity.Product, "") Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule3(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), Me.Department, "", basketItemEntity.PRODUCT_TYPE_ACTUAL) Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule4(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), Me.Department, "", "") Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule5(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), "", basketItemEntity.Product, basketItemEntity.PRODUCT_TYPE_ACTUAL) Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule6(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), "", basketItemEntity.Product, "") Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule7(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), "", "", basketItemEntity.PRODUCT_TYPE_ACTUAL) Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function

        Private Function isFeeSelectionRule8(ByVal feeEntityList As List(Of DEFees), ByVal basketItemEntity As DEBasketItem) As Integer
            Dim matchedItemIndex As Integer = -1
            For itemIndex As Integer = 0 To feeEntityList.Count - 1
                If isFeeSatisfiesRule(feeEntityList(itemIndex), "", "", "") Then
                    matchedItemIndex = itemIndex
                    Exit For
                End If
            Next
            Return matchedItemIndex
        End Function


    End Class
End Namespace

