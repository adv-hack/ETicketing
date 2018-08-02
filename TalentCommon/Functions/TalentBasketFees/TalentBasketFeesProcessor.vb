Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports Talent.Common.Utilities
Namespace TalentBasketFees
    Friend Class TalentBasketFeesProcessor
        Inherits TalentBase

#Region "Class Level Fields"
        Private Const CLASSNAME As String = "TalentBasketFeesProcessor"
        Private _feesToProcess As Dictionary(Of String, List(Of DEFees)) = Nothing
        Private _feeCodesToProcess As Dictionary(Of String, String) = Nothing
        Private _productExcludeFees As Dictionary(Of String, String) = Nothing
        Private _feesCategoryList As List(Of String) = Nothing
        Private _defaultCardTypeCode As String = Nothing
        Private _basketItemsRequiresFees As List(Of DEBasketItem) = Nothing
        Private _basketHeaderMergedEntity As DEBasketMergedHeader = Nothing
        Private _basketDetailMergedList As List(Of DEBasketMergedDetail) = Nothing
        Private _dtExcludedFeesForBasket As DataTable = Nothing
        Private _webSalesDepartment As String = ""
        Private _departmentToProcess As String = ""
        Private _considerCATDetailsStatusFeeCategoryList As Dictionary(Of String, String) = Nothing
        'DEBasketFeees is in DEFees.vb
        Private _basketFeesEntityList As List(Of DEBasketFees) = Nothing
        Private _basketContentType As String = String.Empty
        Private _canProcessBookingFeeCategory As Boolean = False
#End Region

#Region "Properties"
        Public Property BasketEntity() As DEBasket = Nothing
        Public Property BasketItemEntityList() As List(Of DEBasketItem) = Nothing
        Public Property LoginId As String = String.Empty
        Public Property IsUpdatedBasketProcess As Boolean = False
        Public Property DeliveryChargeEntity() As DEDeliveryCharges.DEDeliveryCharge = Nothing

        Public ReadOnly Property ProductExcludeFees() As Dictionary(Of String, String)
            Get
                Return _productExcludeFees
            End Get
        End Property
#End Region

#Region "Public Methods"

        Public Function ProcessBasketForFees() As ErrorObj
            Dim errObj As New ErrorObj
            Try
                If IsValidBasketToProcess(False) Then

                    'process the basket for each fee category
                    ProcessFeeCategories(GetFeeCategoryToProcess())
                Else
                    'if basket has item then process basket level fee category
                    ProcessFeeCategoriesBasketLevel()
                End If
            Catch ex As Exception
                errObj.HasError = True
                errObj.ErrorNumber = "TACTBFP-001"
                errObj.ErrorStatus = "Error while processing basket for basket fees"
                TalentLogger.Logging(CLASSNAME, "ProcessBasketForFees", "Error while processing basket for basket fees", errObj, ex, LogTypeConstants.TCBMFEESLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
            End Try
            Return errObj
        End Function

        Public Function UpdateBasketFeesByPaymentType(ByVal basketHeaderID As String, ByVal loginID As String, ByVal existingPaymentType As String, ByVal existingCardType As String, ByVal currentPaymentType As String, ByVal currentCardType As String, ByRef isPayTypeAddedToDetail As Boolean, ByVal isToUpdatePayTypeInBsktHeader As Boolean, ByVal canProcessPartPayment As Boolean) As Integer
            Dim affectedRows As Integer = 0
            If existingPaymentType = GlobalConstants.CCPAYMENTTYPE AndAlso currentPaymentType = GlobalConstants.CCPAYMENTTYPE Then
                If currentCardType.Trim.Length > 0 AndAlso existingCardType <> currentCardType Then
                    affectedRows = UpdateBookingFee(basketHeaderID, loginID, existingPaymentType, existingCardType, currentPaymentType, currentCardType, isPayTypeAddedToDetail)
                ElseIf currentCardType.Trim.Length = 0 AndAlso isToUpdatePayTypeInBsktHeader Then
                    currentCardType = TDataObjects.PaymentSettings.TblCreditCardBu.GetDefaultCardTypeCode()
                    affectedRows = UpdateBookingFee(basketHeaderID, loginID, existingPaymentType, existingCardType, currentPaymentType, currentCardType, isPayTypeAddedToDetail)
                End If
            ElseIf existingPaymentType <> currentPaymentType Then
                'add the required in detail from basket fees
                Dim DeletefeeCode As String = String.Empty
                Dim DeletefeeCategory As String = String.Empty
                Dim dicPayTypeFeeCategory As Dictionary(Of String, DEFeesRelations) = TDataObjects.FeesSettings.TblFeesRelations.GetPayTypeFeeCategoryList(Settings.BusinessUnit)
                _defaultCardTypeCode = TDataObjects.PaymentSettings.TblCreditCardBu.GetDefaultCardTypeCode()
                If dicPayTypeFeeCategory IsNot Nothing Then
                    Dim canDelete As Boolean = True
                    Dim feeRelationsEntity As DEFeesRelations = Nothing
                    If dicPayTypeFeeCategory.TryGetValue(existingPaymentType, feeRelationsEntity) Then
                        Dim currentFeeRelationEntity As DEFeesRelations = Nothing
                        If dicPayTypeFeeCategory.TryGetValue(currentPaymentType, currentFeeRelationEntity) Then
                            If currentFeeRelationEntity.FeeCode = feeRelationsEntity.FeeCode AndAlso currentFeeRelationEntity.FeeCategory = feeRelationsEntity.FeeCategory Then
                                canDelete = False
                            End If
                        End If
                        If canDelete Then
                            'delete the fee code
                            affectedRows = TDataObjects.BasketSettings.TblBasketDetail.DeleteFee(feeRelationsEntity.FeeCode, basketHeaderID, feeRelationsEntity.FeeCategory)
                        Else
                            'not deleted if not find the fees for the given card type in further steps then delete it
                            DeletefeeCode = feeRelationsEntity.FeeCode
                            DeletefeeCategory = feeRelationsEntity.FeeCategory
                        End If
                    End If
                    feeRelationsEntity = Nothing
                    If dicPayTypeFeeCategory.TryGetValue(currentPaymentType, feeRelationsEntity) Then
                        If (feeRelationsEntity.FeeCategory = GlobalConstants.FEECATEGORY_BOOKING AndAlso currentCardType.Trim.Length <= 0) Then
                            If currentPaymentType = GlobalConstants.SAVEDCARDPAYMENTTYPE OrElse currentPaymentType = GlobalConstants.CCPAYMENTTYPE Then
                                currentCardType = _defaultCardTypeCode
                            Else
                                currentCardType = GlobalConstants.FEE_BOOKING_CARDTYPE_EMPTY
                            End If
                        End If

                        Dim dtBasketFees As DataTable = TDataObjects.BasketSettings.TblBasketFees.GetFeeByHeaderIDCodeCategory(basketHeaderID, feeRelationsEntity.FeeCategory, feeRelationsEntity.FeeCode, currentCardType)
                        If dtBasketFees IsNot Nothing AndAlso dtBasketFees.Rows.Count > 0 Then
                            'build basketfeesentity
                            Dim basketFeesEntity As New DEBasketFees
                            basketFeesEntity.BasketHeaderID = basketHeaderID
                            basketFeesEntity.CardType = Utilities.CheckForDBNull_String(dtBasketFees.Rows(0)("CARD_TYPE_CODE"))
                            basketFeesEntity.FeeApplyType = Utilities.CheckForDBNull_Int(dtBasketFees.Rows(0)("FEE_APPLY_TYPE"))
                            basketFeesEntity.FeeCode = Utilities.CheckForDBNull_String(dtBasketFees.Rows(0)("FEE_CODE"))
                            basketFeesEntity.FeeCategory = Utilities.CheckForDBNull_String(dtBasketFees.Rows(0)("FEE_CATEGORY"))
                            basketFeesEntity.FeeDescription = Utilities.CheckForDBNull_String(dtBasketFees.Rows(0)("FEE_DESCRIPTION"))
                            basketFeesEntity.FeeValue = Utilities.CheckForDBNull_Decimal(dtBasketFees.Rows(0)("FEE_VALUE"))
                            basketFeesEntity.IsSystemFee = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketFees.Rows(0)("IS_SYSTEM_FEE"))
                            basketFeesEntity.IsTransactional = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketFees.Rows(0)("IS_TRANSACTIONAL"))
                            basketFeesEntity.IsNegativeFee = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketFees.Rows(0)("IS_NEGATIVE_FEE"))
                            If basketFeesEntity.FeeValue <> Nothing Then
                                If basketFeesEntity.IsNegativeFee Then
                                    basketFeesEntity.FeeValue = basketFeesEntity.FeeValue * (-1)
                                End If
                            End If
                            Dim bookingFeeInBasketDetail As Decimal = 0
                            If canDelete AndAlso TDataObjects.BasketSettings.TblBasketDetail.TryGetFeeValue(bookingFeeInBasketDetail, basketHeaderID, GlobalConstants.BKFEE, GlobalConstants.FEECATEGORY_BOOKING) Then
                                canDelete = False
                            End If
                            If Not canDelete Then
                                affectedRows = TDataObjects.BasketSettings.TblBasketDetail.UpdateFee(basketHeaderID, loginID, basketFeesEntity)
                            Else
                                'insert the choosen one - only if not in the basket already
                                If Not TDataObjects.BasketSettings.TblBasketDetail.IsSystemFeeInBasketAlready(basketHeaderID, loginID, basketFeesEntity, False) Then
                                    affectedRows = TDataObjects.BasketSettings.TblBasketDetail.InsertFee(basketHeaderID, loginID, basketFeesEntity, False)
                                End If
                            End If
                            If affectedRows > 0 Then
                                isPayTypeAddedToDetail = True
                                'If currentPaymentType = GlobalConstants.CCPAYMENTTYPE Then
                                If currentPaymentType = GlobalConstants.SAVEDCARDPAYMENTTYPE OrElse currentPaymentType = GlobalConstants.CCPAYMENTTYPE Then
                                    currentCardType = basketFeesEntity.CardType
                                Else
                                    currentCardType = ""
                                End If
                                'update basket header
                                affectedRows = TDataObjects.BasketSettings.TblBasketHeader.UpdatePaymentAndCardType(basketHeaderID, feeRelationsEntity.ApplyRelatedCode, currentCardType)
                            End If
                        Else
                            'not deleted but not able to find fees for that card type then delete it
                            If Not canDelete Then
                                'delete the fee code
                                affectedRows = TDataObjects.BasketSettings.TblBasketDetail.DeleteFee(DeletefeeCode, basketHeaderID, DeletefeeCategory)
                            End If
                            If currentCardType = GlobalConstants.FEE_BOOKING_CARDTYPE_EMPTY Then currentCardType = ""
                            'card type fee value may not exists if excluded so update with given pay and card types
                            affectedRows = TDataObjects.BasketSettings.TblBasketHeader.UpdatePaymentAndCardType(basketHeaderID, currentPaymentType, currentCardType)
                        End If
                    Else
                        'pay type may not exists if no fees is related to it
                        affectedRows = TDataObjects.BasketSettings.TblBasketHeader.UpdatePaymentAndCardType(basketHeaderID, currentPaymentType, currentCardType)
                    End If
                End If
            End If
            If canProcessPartPayment Then
                Dim canProcessBookingFee As Boolean = ProcessPartPaymentFees(basketHeaderID, currentPaymentType)
                If canProcessBookingFee Then
                    Dim bookingFeeInBasketDetail As Decimal = 0
                    If Not TDataObjects.BasketSettings.TblBasketDetail.TryGetFeeValue(bookingFeeInBasketDetail, basketHeaderID, GlobalConstants.BKFEE, GlobalConstants.FEECATEGORY_BOOKING) Then
                        Dim partPaymentFee As Decimal = 0
                        Dim dtPartPayments As DataTable = Utilities.RetrievePartPayments(Settings, basketHeaderID)
                        If dtPartPayments IsNot Nothing AndAlso dtPartPayments.Rows.Count > 0 Then
                            For rowIndex As Integer = 0 To dtPartPayments.Rows.Count - 1
                                partPaymentFee += Utilities.CheckForDBNull_Decimal(dtPartPayments.Rows(rowIndex)("FeeValue"))
                            Next
                        End If
                        If partPaymentFee > 0 Then
                            'build basketfeesentity
                            Dim basketFeesEntity As New DEBasketFees
                            basketFeesEntity.BasketHeaderID = basketHeaderID
                            basketFeesEntity.CardType = ""
                            basketFeesEntity.FeeApplyType = 2
                            basketFeesEntity.FeeCode = GlobalConstants.BKFEE
                            basketFeesEntity.FeeCategory = GlobalConstants.FEECATEGORY_BOOKING
                            basketFeesEntity.FeeDescription = GlobalConstants.FEECATEGORY_BOOKING
                            basketFeesEntity.FeeValue = partPaymentFee
                            basketFeesEntity.IsSystemFee = True
                            basketFeesEntity.IsTransactional = False
                            affectedRows = TDataObjects.BasketSettings.TblBasketDetail.InsertFee(basketHeaderID, loginID, basketFeesEntity, False)
                        End If
                    End If
                End If
            End If
            Return affectedRows
        End Function

        Public Function ProcessBasketForBookingFees(ByVal basketHeaderID As String) As ErrorObj
            Dim err As New ErrorObj
            ProcessFeeCategoryBooking(True, basketHeaderID)
            Return err
        End Function

        Friend Function ProcessBookingFeesForPartPayment(ByVal basketHeaderID As String) As ErrorObj
            Dim err As New ErrorObj
            ProcessFeeCategoryBooking(True, basketHeaderID)
            Return err
        End Function

#End Region

#Region "Private Methods"

        Private Sub RePopulateBasketEntityAndItsList(ByVal basketHeaderID As String)
            Dim latestBasketEntity As New DEBasket
            Dim latestBasketItemEntity As New DEBasketItem
            Dim dtBasket As New DataTable
            dtBasket = TDataObjects.BasketSettings.TblBasketHeader.GetHeaderByBasketHeaderID(basketHeaderID)
            Dim properties As ArrayList = Utilities.GetPropertyNames(latestBasketEntity)
            latestBasketEntity = PopulateProperties(properties, dtBasket, latestBasketEntity, 0)
            properties = Utilities.GetPropertyNames(latestBasketItemEntity)
            dtBasket = TDataObjects.BasketSettings.TblBasketDetail.GetDetailByBasketHeaderID(basketHeaderID)
            If dtBasket.Rows.Count > 0 Then
                For rowIndex As Integer = 0 To dtBasket.Rows.Count - 1
                    latestBasketItemEntity = New DEBasketItem
                    latestBasketItemEntity = PopulateProperties(properties, dtBasket.Rows(rowIndex), latestBasketItemEntity)
                    latestBasketEntity.BasketItems.Add(latestBasketItemEntity)
                Next
            End If
            BasketEntity = latestBasketEntity
            BasketItemEntityList = latestBasketEntity.BasketItems
        End Sub

        Private Function UpdateBookingFee(ByVal basketHeaderID As String, ByVal loginID As String, ByVal existingPaymentType As String, ByVal existingCardType As String, ByVal currentPaymentType As String, ByVal currentCardType As String, ByRef isPayTypeAddedToDetail As Boolean) As Integer
            'update the bkfee in detail with required value
            'add the required in detail from basket fees
            Dim affectedRows As Integer = 0
            Dim dicPayTypeFeeCategory As Dictionary(Of String, DEFeesRelations) = TDataObjects.FeesSettings.TblFeesRelations.GetPayTypeFeeCategoryList(Settings.BusinessUnit)
            If dicPayTypeFeeCategory IsNot Nothing Then
                Dim feeRelationsEntity As DEFeesRelations = Nothing
                If dicPayTypeFeeCategory.TryGetValue(currentPaymentType, feeRelationsEntity) Then
                    Dim dtBasketFees As DataTable = TDataObjects.BasketSettings.TblBasketFees.GetFeeByHeaderIDCodeCategory(basketHeaderID, feeRelationsEntity.FeeCategory, feeRelationsEntity.FeeCode, currentCardType)
                    If dtBasketFees IsNot Nothing AndAlso dtBasketFees.Rows.Count > 0 Then
                        'build basketfeesentity
                        Dim basketFeesEntity As New DEBasketFees
                        basketFeesEntity.BasketHeaderID = basketHeaderID
                        basketFeesEntity.CardType = Utilities.CheckForDBNull_String(dtBasketFees.Rows(0)("CARD_TYPE_CODE"))
                        basketFeesEntity.FeeApplyType = Utilities.CheckForDBNull_Int(dtBasketFees.Rows(0)("FEE_APPLY_TYPE"))
                        basketFeesEntity.FeeCode = Utilities.CheckForDBNull_String(dtBasketFees.Rows(0)("FEE_CODE"))
                        basketFeesEntity.FeeCategory = Utilities.CheckForDBNull_String(dtBasketFees.Rows(0)("FEE_CATEGORY"))
                        basketFeesEntity.FeeDescription = Utilities.CheckForDBNull_String(dtBasketFees.Rows(0)("FEE_DESCRIPTION"))
                        basketFeesEntity.FeeValue = Utilities.CheckForDBNull_Decimal(dtBasketFees.Rows(0)("FEE_VALUE"))
                        basketFeesEntity.IsSystemFee = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketFees.Rows(0)("IS_SYSTEM_FEE"))
                        basketFeesEntity.IsTransactional = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketFees.Rows(0)("IS_TRANSACTIONAL"))
                        basketFeesEntity.IsNegativeFee = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketFees.Rows(0)("IS_NEGATIVE_FEE"))
                        If basketFeesEntity.FeeValue <> Nothing Then
                            If basketFeesEntity.IsNegativeFee Then
                                basketFeesEntity.FeeValue = basketFeesEntity.FeeValue * (-1)
                            End If
                        End If
                        affectedRows = TDataObjects.BasketSettings.TblBasketDetail.UpdateOrInsertFee(basketHeaderID, loginID, basketFeesEntity)
                        If affectedRows > 0 Then
                            isPayTypeAddedToDetail = True
                            'update basket header
                            affectedRows = TDataObjects.BasketSettings.TblBasketHeader.UpdatePaymentAndCardType(basketHeaderID, feeRelationsEntity.ApplyRelatedCode, basketFeesEntity.CardType)
                        End If
                    Else

                        'related card type fees is not exists in basket fees so delete the booking fee from basket detail
                        affectedRows = 0
                        affectedRows = TDataObjects.BasketSettings.TblBasketDetail.DeleteFee(feeRelationsEntity.FeeCode, basketHeaderID, feeRelationsEntity.FeeCategory)

                        'card type fee value may not exists if excluded so update with given pay and card types
                        affectedRows = 0
                        affectedRows = TDataObjects.BasketSettings.TblBasketHeader.UpdatePaymentAndCardType(basketHeaderID, currentPaymentType, currentCardType)

                    End If
                Else
                    'pay type may not exists if no fees is related to it 
                    'looks like incorrect configuration in fee relation table
                    affectedRows = TDataObjects.BasketSettings.TblBasketHeader.UpdatePaymentAndCardType(basketHeaderID, currentPaymentType, currentCardType)
                End If
            End If
            Return affectedRows
        End Function

        Private Sub ProcessFeeCategories(ByVal feeCategoriesToProcess As List(Of String))
            If feeCategoriesToProcess IsNot Nothing AndAlso feeCategoriesToProcess.Count > 0 Then
                AssignGlobalVariablesForProcess()
                'got the fee category decide on parallel processing
                If Settings.CanProcessFeesParallely Then
                    Parallel.ForEach(feeCategoriesToProcess, Sub(feeCategory)
                                                                 ProcessRespectiveFees(feeCategory)
                                                             End Sub)
                Else
                    For itemIndex As Integer = 0 To feeCategoriesToProcess.Count - 1
                        ProcessRespectiveFees(feeCategoriesToProcess.Item(itemIndex))
                    Next
                End If
            End If
            'basket processed valid fees are available
            'now move it to basket fees or update basket detail
            MoveBasketFeesToBasketTables(False)

            'all fee category are processed, now process booking
            ProcessFeeCategoryBooking(False, BasketEntity.Basket_Header_ID)

        End Sub

        Private Sub ProcessFeeCategoriesBasketLevel()
            If BasketEntity IsNot Nothing Then
                If BasketItemEntityList IsNot Nothing AndAlso BasketItemEntityList.Count > 0 Then
                    TDataObjects.BasketSettings.TblBasketDetail.DeleteFeesByFeeCategory(BasketEntity.Basket_Header_ID, GlobalConstants.FEECATEGORY_ADHOC)
                    Dim feeCategoriesToProcess As New List(Of String)
                    For itemIndex As Integer = 0 To _feesCategoryList.Count - 1
                        If _feesCategoryList.Item(itemIndex) = GlobalConstants.FEECATEGORY_CHARITY OrElse _feesCategoryList.Item(itemIndex) = GlobalConstants.FEECATEGORY_VARIABLE Then
                            If IsFeeCategoryValidForProcessing(_feesCategoryList.Item(itemIndex)) Then
                                feeCategoriesToProcess.Add(_feesCategoryList.Item(itemIndex))
                            End If
                        End If
                    Next
                    ProcessFeeCategories(feeCategoriesToProcess)
                End If
            End If
        End Sub

        Private Sub PopulateBasketFeesEntityList(ByVal basketFeesEntities As List(Of DEBasketFees))
            'todo lock block required if parallel processing is enabled
            If basketFeesEntities IsNot Nothing AndAlso basketFeesEntities.Count > 0 Then
                For itemIndex As Integer = 0 To basketFeesEntities.Count - 1
                    If basketFeesEntities(itemIndex).IsChargeable AndAlso IsFeeNotExcludedForBasket(basketFeesEntities(itemIndex)) Then
                        If basketFeesEntities(itemIndex).FeeValue <> 0 OrElse basketFeesEntities(itemIndex).FeeCategory = GlobalConstants.FEECATEGORY_VARIABLE Then
                            'always round up fee value so 6.004 to 6.01
                            basketFeesEntities(itemIndex).FeeValue = Math.Ceiling(basketFeesEntities(itemIndex).FeeValue * 100) / 100
                            _basketFeesEntityList.Add(basketFeesEntities(itemIndex))

                        End If
                    End If
                Next
            End If
        End Sub

        Private Sub ProcessRespectiveFees(ByVal feeCategory As String)
            Select Case feeCategory
                Case GlobalConstants.FEECATEGORY_BOOKING
                    ProcessFeesBookingForBasket()
                Case GlobalConstants.FEECATEGORY_DIRECTDEBIT
                    ProcessFeesDirectDebitForBasket()
                Case GlobalConstants.FEECATEGORY_FINANCE
                    ProcessFeesFinanceForBasket()
                Case GlobalConstants.FEECATEGORY_CHARITY
                    ProcessFeesCharityForBasket()
                Case GlobalConstants.FEECATEGORY_ADHOC
                    ProcessFeesAdhocForBasket()
                Case GlobalConstants.FEECATEGORY_VARIABLE
                    ProcessFeesVariableForBasket()
                Case GlobalConstants.FEECATEGORY_FULFILMENT
                    ProcessFeesFulfilmentForBasket()
                Case GlobalConstants.FEECATEGORY_CANCEL
                    ProcessFeesCATForBasket(GlobalConstants.FEECATEGORY_CANCEL)
                Case GlobalConstants.FEECATEGORY_AMEND
                    ProcessFeesCATForBasket(GlobalConstants.FEECATEGORY_AMEND)
                Case GlobalConstants.FEECATEGORY_TRANSFER
                    ProcessFeesCATForBasket(GlobalConstants.FEECATEGORY_TRANSFER)
                Case GlobalConstants.FEECATEGORY_WEBSALES
                    ProcessFeesApplicationForBasket(GlobalConstants.FEECATEGORY_WEBSALES)
                Case GlobalConstants.FEECATEGORY_SUPPLYNET
                    ProcessFeesApplicationForBasket(GlobalConstants.FEECATEGORY_SUPPLYNET)
            End Select
        End Sub

        Private Sub ProcessFeeCategoryBooking(ByVal isItExternalCall As Boolean, ByVal basketHeaderID As String)

            If isItExternalCall Then
                RePopulateBasketEntityAndItsList(basketHeaderID)
                IsValidBasketToProcess(True)
                AssignGlobalVariablesForProcess()
                ProcessFeesBookingForBasket()
                MoveBasketFeesToBasketTables(True)
            Else
                If _canProcessBookingFeeCategory Then
                    'fee category is allowed to process
                    RePopulateBasketEntityAndItsList(basketHeaderID)
                    _basketFeesEntityList = New List(Of DEBasketFees)
                    IsValidBasketToProcess(True)
                    ProcessFeesBookingForBasket()
                    MoveBasketFeesToBasketTables(True)
                Else
                    'category is not valid for the current basket, delete all existing booking fee
                    Dim affectedRows As Integer = TDataObjects.BasketSettings.TblBasketFees.DeleteBookingFee(basketHeaderID, False)
                End If
            End If

            'Get the current payment type from tbl_basket_header, and call ProcessPartPayment based on the payment type
            Dim dtBasket As DataTable = TDataObjects.BasketSettings.TblBasketHeader.GetHeaderByBasketHeaderID(basketHeaderID)
            Dim currentPaymentType As String = String.Empty
            If dtBasket.Rows.Count > 0 Then
                currentPaymentType = dtBasket.Rows(0).Item("PAYMENT_TYPE")
            End If

            Dim canProcessBookingFee As Boolean = ProcessPartPaymentFees(basketHeaderID, currentPaymentType)
            If canProcessBookingFee Then
                Dim bookingFeeInBasketDetail As Decimal = 0
                If Not TDataObjects.BasketSettings.TblBasketDetail.TryGetFeeValue(bookingFeeInBasketDetail, basketHeaderID, GlobalConstants.BKFEE, GlobalConstants.FEECATEGORY_BOOKING) Then
                    Dim partPaymentFee As Decimal = 0
                    Dim dtPartPayments As DataTable = Utilities.RetrievePartPayments(Settings, basketHeaderID)
                    If dtPartPayments IsNot Nothing AndAlso dtPartPayments.Rows.Count > 0 Then
                        For rowIndex As Integer = 0 To dtPartPayments.Rows.Count - 1
                            partPaymentFee += Utilities.CheckForDBNull_Decimal(dtPartPayments.Rows(rowIndex)("FeeValue"))
                        Next
                    End If
                    If partPaymentFee > 0 Then
                        'build basketfeesentity
                        Dim basketFeesEntity As New DEBasketFees
                        basketFeesEntity.BasketHeaderID = basketHeaderID
                        basketFeesEntity.CardType = ""
                        basketFeesEntity.FeeApplyType = 2
                        basketFeesEntity.FeeCode = GlobalConstants.BKFEE
                        basketFeesEntity.FeeCategory = GlobalConstants.FEECATEGORY_BOOKING
                        basketFeesEntity.FeeDescription = GlobalConstants.FEECATEGORY_BOOKING
                        basketFeesEntity.FeeValue = partPaymentFee
                        basketFeesEntity.IsSystemFee = True
                        basketFeesEntity.IsTransactional = False
                        TDataObjects.BasketSettings.TblBasketDetail.InsertFee(basketHeaderID, LoginId, basketFeesEntity, False)
                    End If
                End If
            End If

        End Sub

        Private Sub ProcessFeesBookingForBasket()
            Dim feeEntityList As List(Of DEFees) = Nothing
            If _feesToProcess.TryGetValue(GlobalConstants.FEECATEGORY_BOOKING, feeEntityList) Then
                Dim errObj As New ErrorObj
                Dim talBasketFeesBooking As New TalentBasketFeesBooking
                talBasketFeesBooking.Settings = Settings
                talBasketFeesBooking.CardTypeFeeCategory = CardTypeFeeCategory
                talBasketFeesBooking.BookingFeesList = feeEntityList
                talBasketFeesBooking.BasketEntity = BasketEntity
                talBasketFeesBooking.BasketItemEntityList = BasketItemEntityList
                talBasketFeesBooking.BasketItemRequiresFees = _basketItemsRequiresFees
                talBasketFeesBooking.BasketDetailMergedList = _basketDetailMergedList
                talBasketFeesBooking.BasketHeaderMergedEntity = _basketHeaderMergedEntity
                talBasketFeesBooking.ProductExcludeFees = _productExcludeFees
                talBasketFeesBooking.Department = _departmentToProcess
                talBasketFeesBooking.ConsiderCATDetailsOnCATTypeStatus = GetConsiderCATDetailsOnCATStatus(GlobalConstants.FEECATEGORY_BOOKING)
                talBasketFeesBooking.DeliveryChargeEntity = DeliveryChargeEntity
                errObj = talBasketFeesBooking.ProcessBookingFeesForBasket()
                If Not errObj.HasError Then
                    PopulateBasketFeesEntityList(talBasketFeesBooking.BasketFeesEntityList)
                End If
                _defaultCardTypeCode = BasketEntity.CARD_TYPE_CODE
                If String.IsNullOrWhiteSpace(_defaultCardTypeCode) Then
                    _defaultCardTypeCode = TDataObjects.PaymentSettings.TblCreditCardBu.GetDefaultCardTypeCode()
                End If
            End If
        End Sub

        Private Sub ProcessFeesDirectDebitForBasket()
            Dim feeEntityList As List(Of DEFees) = Nothing
            If _feesToProcess.TryGetValue(GlobalConstants.FEECATEGORY_DIRECTDEBIT, feeEntityList) Then
                Dim errObj As New ErrorObj
                Dim talBasketFeesDirectDebit As New TalentBasketFeesDirectDebit
                talBasketFeesDirectDebit.Settings = Settings
                talBasketFeesDirectDebit.DirectDebitFeesList = feeEntityList
                talBasketFeesDirectDebit.BasketEntity = BasketEntity
                talBasketFeesDirectDebit.BasketItemEntityList = BasketItemEntityList
                talBasketFeesDirectDebit.BasketItemRequiresFees = _basketItemsRequiresFees
                talBasketFeesDirectDebit.BasketDetailMergedList = _basketDetailMergedList
                talBasketFeesDirectDebit.BasketHeaderMergedEntity = _basketHeaderMergedEntity
                talBasketFeesDirectDebit.ProductExcludeFees = _productExcludeFees
                talBasketFeesDirectDebit.Department = _departmentToProcess
                talBasketFeesDirectDebit.ConsiderCATDetailsOnCATTypeStatus = GetConsiderCATDetailsOnCATStatus(GlobalConstants.FEECATEGORY_DIRECTDEBIT)
                errObj = talBasketFeesDirectDebit.ProcessDirectDebitFeesForBasket()
                If Not errObj.HasError Then
                    PopulateBasketFeesEntityList(talBasketFeesDirectDebit.BasketFeesEntityList)
                End If
            End If
        End Sub

        Private Sub ProcessFeesFinanceForBasket()
            Dim feeEntityList As List(Of DEFees) = Nothing
            If _feesToProcess.TryGetValue(GlobalConstants.FEECATEGORY_FINANCE, feeEntityList) Then
                Dim errObj As New ErrorObj
                Dim talBasketFeesFinance As New TalentBasketFeesFinance
                talBasketFeesFinance.Settings = Settings
                talBasketFeesFinance.FinanceFeesList = feeEntityList
                talBasketFeesFinance.BasketEntity = BasketEntity
                talBasketFeesFinance.BasketItemEntityList = BasketItemEntityList
                talBasketFeesFinance.BasketItemRequiresFees = _basketItemsRequiresFees
                talBasketFeesFinance.BasketDetailMergedList = _basketDetailMergedList
                talBasketFeesFinance.BasketHeaderMergedEntity = _basketHeaderMergedEntity
                talBasketFeesFinance.ProductExcludeFees = _productExcludeFees
                talBasketFeesFinance.Department = _departmentToProcess
                talBasketFeesFinance.ConsiderCATDetailsOnCATTypeStatus = GetConsiderCATDetailsOnCATStatus(GlobalConstants.FEECATEGORY_FINANCE)
                errObj = talBasketFeesFinance.ProcessFinanceFeesForBasket()
                If Not errObj.HasError Then
                    PopulateBasketFeesEntityList(talBasketFeesFinance.BasketFeesEntityList)
                End If
            End If
        End Sub

        Private Sub ProcessFeesCharityForBasket()
            Dim feeEntityList As List(Of DEFees) = Nothing
            If _feesToProcess.TryGetValue(GlobalConstants.FEECATEGORY_CHARITY, feeEntityList) Then
                Dim errObj As New ErrorObj
                Dim talBasketFeesCharity As New TalentBasketFeesCharity
                talBasketFeesCharity.Settings = Settings
                talBasketFeesCharity.CharityFeesList = feeEntityList
                talBasketFeesCharity.BasketEntity = BasketEntity
                talBasketFeesCharity.BasketItemEntityList = BasketItemEntityList
                talBasketFeesCharity.BasketItemRequiresFees = _basketItemsRequiresFees
                talBasketFeesCharity.BasketDetailMergedList = _basketDetailMergedList
                talBasketFeesCharity.BasketHeaderMergedEntity = _basketHeaderMergedEntity
                talBasketFeesCharity.ProductExcludeFees = _productExcludeFees
                talBasketFeesCharity.Department = _departmentToProcess
                talBasketFeesCharity.ConsiderCATDetailsOnCATTypeStatus = GetConsiderCATDetailsOnCATStatus(GlobalConstants.FEECATEGORY_CHARITY)
                errObj = talBasketFeesCharity.ProcessCharityFeesForBasket
                If Not errObj.HasError Then
                    PopulateBasketFeesEntityList(talBasketFeesCharity.BasketFeesEntityList)
                End If
            End If
        End Sub

        Private Sub ProcessFeesAdhocForBasket()
            Dim feeEntityList As List(Of DEFees) = Nothing
            If TryGetFeelistByBasketContentType(GlobalConstants.FEECATEGORY_ADHOC, feeEntityList) Then
                Dim errObj As New ErrorObj
                Dim talBasketFeesAdhoc As New TalentBasketFeesAdhoc
                talBasketFeesAdhoc.Settings = Settings
                talBasketFeesAdhoc.AdhocFeesList = feeEntityList
                talBasketFeesAdhoc.BasketEntity = BasketEntity
                talBasketFeesAdhoc.BasketItemEntityList = BasketItemEntityList
                talBasketFeesAdhoc.BasketItemRequiresFees = _basketItemsRequiresFees
                talBasketFeesAdhoc.BasketDetailMergedList = _basketDetailMergedList
                talBasketFeesAdhoc.BasketHeaderMergedEntity = _basketHeaderMergedEntity
                talBasketFeesAdhoc.ProductExcludeFees = _productExcludeFees
                talBasketFeesAdhoc.Department = _departmentToProcess
                talBasketFeesAdhoc.ConsiderCATDetailsOnCATTypeStatus = GetConsiderCATDetailsOnCATStatus(GlobalConstants.FEECATEGORY_ADHOC)
                errObj = talBasketFeesAdhoc.ProcessAdhocFeesForBasket()
                If Not errObj.HasError Then
                    PopulateBasketFeesEntityList(talBasketFeesAdhoc.BasketFeesEntityList)
                End If
            End If
        End Sub

        Private Sub ProcessFeesVariableForBasket()
            Dim feeEntityList As List(Of DEFees) = Nothing
            If _feesToProcess.TryGetValue(GlobalConstants.FEECATEGORY_VARIABLE, feeEntityList) Then
                Dim errObj As New ErrorObj
                Dim talBasketFeesVariable As New TalentBasketFeesVariable
                talBasketFeesVariable.Settings = Settings
                talBasketFeesVariable.VariableFeesList = feeEntityList
                talBasketFeesVariable.BasketEntity = BasketEntity
                talBasketFeesVariable.BasketItemEntityList = BasketItemEntityList
                talBasketFeesVariable.BasketItemRequiresFees = _basketItemsRequiresFees
                talBasketFeesVariable.BasketDetailMergedList = _basketDetailMergedList
                talBasketFeesVariable.BasketHeaderMergedEntity = _basketHeaderMergedEntity
                talBasketFeesVariable.ProductExcludeFees = _productExcludeFees
                talBasketFeesVariable.Department = _departmentToProcess
                talBasketFeesVariable.ConsiderCATDetailsOnCATTypeStatus = GetConsiderCATDetailsOnCATStatus(GlobalConstants.FEECATEGORY_VARIABLE)
                errObj = talBasketFeesVariable.ProcessVariableFeesForBasket
                If Not errObj.HasError Then
                    PopulateBasketFeesEntityList(talBasketFeesVariable.BasketFeesEntityList)
                End If
            End If
        End Sub

        Private Sub ProcessFeesFulfilmentForBasket()
            Dim feeEntityList As List(Of DEFees) = Nothing
            If _feesToProcess.TryGetValue(GlobalConstants.FEECATEGORY_FULFILMENT, feeEntityList) Then
                Dim errObj As New ErrorObj
                Dim talBasketFeesFulfilment As New TalentBasketFeesFulfilment
                talBasketFeesFulfilment.Settings = Settings
                talBasketFeesFulfilment.FulfilmentFeeCategory = FulfilmentFeeCategory
                talBasketFeesFulfilment.FulfilmentFeesList = feeEntityList
                talBasketFeesFulfilment.BasketEntity = BasketEntity
                talBasketFeesFulfilment.BasketItemEntityList = BasketItemEntityList
                talBasketFeesFulfilment.BasketItemRequiresFees = _basketItemsRequiresFees
                talBasketFeesFulfilment.BasketDetailMergedList = _basketDetailMergedList
                talBasketFeesFulfilment.ProductExcludeFees = _productExcludeFees
                talBasketFeesFulfilment.Department = _departmentToProcess
                talBasketFeesFulfilment.ConsiderCATDetailsOnCATTypeStatus = GetConsiderCATDetailsOnCATStatus(GlobalConstants.FEECATEGORY_FULFILMENT)
                errObj = talBasketFeesFulfilment.ProcessFulfilmentFeesForBasket()
                If Not errObj.HasError Then
                    PopulateBasketFeesEntityList(talBasketFeesFulfilment.BasketFeesEntityList)
                End If
            End If
        End Sub

        Private Sub ProcessFeesCATForBasket(ByVal feeCategory As String)
            Dim feeEntityList As List(Of DEFees) = Nothing
            If _feesToProcess.TryGetValue(feeCategory, feeEntityList) Then
                Dim errObj As New ErrorObj
                Dim talBasketFeesCat As New TalentBasketFeesCAT
                talBasketFeesCat.Settings = Settings
                talBasketFeesCat.CATFeesList = feeEntityList
                talBasketFeesCat.BasketEntity = BasketEntity
                talBasketFeesCat.BasketItemEntityList = BasketItemEntityList
                talBasketFeesCat.BasketItemRequiresFees = _basketItemsRequiresFees
                talBasketFeesCat.BasketDetailMergedList = _basketDetailMergedList
                talBasketFeesCat.BasketHeaderMergedEntity = _basketHeaderMergedEntity
                talBasketFeesCat.ProductExcludeFees = _productExcludeFees
                talBasketFeesCat.Department = _departmentToProcess
                talBasketFeesCat.ConsiderCATDetailsOnCATTypeStatus = GetConsiderCATDetailsOnCATStatus(feeCategory)
                errObj = talBasketFeesCat.ProcessCATFeesForBasket
                If Not errObj.HasError Then
                    PopulateBasketFeesEntityList(talBasketFeesCat.BasketFeesEntityList)
                End If
            End If
        End Sub

        Private Sub ProcessFeesApplicationForBasket(ByVal feeCategory As String)
            Dim feeEntityList As List(Of DEFees) = Nothing
            If _feesToProcess.TryGetValue(feeCategory, feeEntityList) Then
                Dim errObj As New ErrorObj
                Dim talBasketFeesApp As New TalentBasketFeesApplication
                talBasketFeesApp.Settings = Settings
                talBasketFeesApp.ApplicationFeesList = feeEntityList
                talBasketFeesApp.BasketEntity = BasketEntity
                talBasketFeesApp.BasketItemEntityList = BasketItemEntityList
                talBasketFeesApp.BasketItemRequiresFees = _basketItemsRequiresFees
                talBasketFeesApp.BasketDetailMergedList = _basketDetailMergedList
                talBasketFeesApp.BasketHeaderMergedEntity = _basketHeaderMergedEntity
                talBasketFeesApp.ProductExcludeFees = _productExcludeFees
                talBasketFeesApp.Department = _departmentToProcess
                talBasketFeesApp.ConsiderCATDetailsOnCATTypeStatus = GetConsiderCATDetailsOnCATStatus(feeCategory)
                errObj = talBasketFeesApp.ProcessApplicationFeesForBasket
                If Not errObj.HasError Then
                    PopulateBasketFeesEntityList(talBasketFeesApp.BasketFeesEntityList)
                End If
            End If
        End Sub

        Private Sub PopulateBasketItemsRequiresFees(ByVal isBookingFeeProcess As Boolean)
            _basketItemsRequiresFees = New List(Of DEBasketItem)
            _basketDetailMergedList = New List(Of DEBasketMergedDetail)
            If BasketEntity IsNot Nothing Then
                If BasketItemEntityList IsNot Nothing AndAlso BasketItemEntityList.Count > 0 Then
                    Dim hasTickets As Boolean = False
                    Dim hasProducts As Boolean = False
                    If BasketEntity.CAT_MODE.Trim.Length > 0 Then
                        For itemIndex As Integer = 0 To BasketItemEntityList.Count - 1
                            If IsProductRequiresFees(BasketItemEntityList(itemIndex), isBookingFeeProcess) Then
                                _basketItemsRequiresFees.Add(BasketItemEntityList(itemIndex))
                                PopulateBasketDetailMergListCAT(BasketItemEntityList(itemIndex), BasketEntity)
                            End If
                            Select Case BasketItemEntityList(itemIndex).MODULE_.ToUpper
                                Case "TICKETING"
                                    If Not IsProductItselfAFee(BasketItemEntityList(itemIndex).Product) Then
                                        hasTickets = True
                                    End If
                                Case "ECOMMERCE"
                                    hasProducts = True
                                Case Else
                                    hasProducts = True
                            End Select
                        Next
                    Else
                        For itemIndex As Integer = 0 To BasketItemEntityList.Count - 1
                            If IsProductRequiresFees(BasketItemEntityList(itemIndex), isBookingFeeProcess) Then
                                _basketItemsRequiresFees.Add(BasketItemEntityList(itemIndex))
                                PopulateBasketDetailMergList(BasketItemEntityList(itemIndex), BasketEntity)
                            End If
                            Select Case BasketItemEntityList(itemIndex).MODULE_.ToUpper
                                Case "TICKETING"
                                    If Not IsProductItselfAFee(BasketItemEntityList(itemIndex).Product) Then
                                        hasTickets = True
                                    End If
                                Case "ECOMMERCE"
                                    hasProducts = True
                                Case Else
                                    hasProducts = True
                            End Select
                        Next
                    End If
                    If hasProducts And hasTickets Then
                        _basketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE
                    ElseIf hasProducts And Not hasTickets Then
                        _basketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE
                    ElseIf Not hasProducts And hasTickets Then
                        _basketContentType = GlobalConstants.TICKETINGBASKETCONTENTTYPE
                    End If
                End If
            End If
        End Sub

        Private Sub PopulateBasketHeaderMergList()
            _basketHeaderMergedEntity = New DEBasketMergedHeader
            If _basketDetailMergedList.Count > 0 Then
                For itemIndex As Integer = 0 To _basketDetailMergedList.Count - 1
                    If _basketDetailMergedList(itemIndex).ModuleOfItem = GlobalConstants.BASKETMODULETICKETING Then
                        _basketHeaderMergedEntity.TotalBasket += _basketDetailMergedList(itemIndex).Price
                        _basketHeaderMergedEntity.TotalBasketTicketsOnly += _basketDetailMergedList(itemIndex).Price
                    Else
                        _basketHeaderMergedEntity.TotalBasket += _basketDetailMergedList(itemIndex).Price
                        _basketHeaderMergedEntity.TotalBasketRetailOnly += _basketDetailMergedList(itemIndex).Price
                    End If
                Next
            End If
        End Sub

        Private Sub PopulateBasketDetailMergList(ByVal basketItem As DEBasketItem, ByVal basketHeader As DEBasket)
            Dim isNotExists As Boolean = True
            If _basketDetailMergedList.Count > 0 Then
                For itemIndex As Integer = 0 To _basketDetailMergedList.Count - 1
                    If _basketDetailMergedList(itemIndex).Product = basketItem.Product Then
                        _basketDetailMergedList(itemIndex).Quantity += basketItem.Quantity
                        _basketDetailMergedList(itemIndex).Price += basketItem.Gross_Price
                        isNotExists = False
                        Exit For
                    End If
                Next
            End If
            If isNotExists Then
                Dim basketItemMerged As New DEBasketMergedDetail(basketItem, basketHeader)
                _basketDetailMergedList.Add(basketItemMerged)
            End If
        End Sub

        Private Sub PopulateBasketDetailMergListCAT(ByVal basketItem As DEBasketItem, ByVal basketHeader As DEBasket)
            Dim isNotExists As Boolean = True
            If _basketDetailMergedList.Count > 0 Then
                For itemIndex As Integer = 0 To _basketDetailMergedList.Count - 1
                    If _basketDetailMergedList(itemIndex).Product = basketItem.Product Then
                        _basketDetailMergedList(itemIndex).Quantity += basketItem.Quantity
                        _basketDetailMergedList(itemIndex).Price += basketItem.Gross_Price
                        _basketDetailMergedList(itemIndex).Quantity_After_CAT += (basketItem.Quantity - basketItem.CAT_QUANTITY)
                        If _basketDetailMergedList(itemIndex).Quantity_After_CAT < 0 Then _basketDetailMergedList(itemIndex).Quantity_After_CAT = 0
                        _basketDetailMergedList(itemIndex).Price_After_CAT += (basketItem.Gross_Price - basketHeader.CAT_PRICE)
                        If _basketDetailMergedList(itemIndex).Price_After_CAT < 0 Then _basketDetailMergedList(itemIndex).Price_After_CAT = 0
                        isNotExists = False
                        Exit For
                    End If
                Next
            End If
            If isNotExists Then
                Dim basketItemMerged As New DEBasketMergedDetail(basketItem, basketHeader)
                _basketDetailMergedList.Add(basketItemMerged)
            End If
        End Sub

        Private Sub AssignGlobalVariablesForProcess()
            _basketFeesEntityList = New List(Of DEBasketFees)
            'default card type code for process booking fees
            _defaultCardTypeCode = TDataObjects.PaymentSettings.TblCreditCardBu.GetDefaultCardTypeCode()
            'get the fees which are excluded for basket
            _dtExcludedFeesForBasket = TDataObjects.BasketSettings.TblBasketFees.GetExcludedFeesByBasketHeaderID(BasketEntity.Basket_Header_ID)
            'assign the department to process
            _departmentToProcess = GetDepartment(Settings.AgentEntity)
        End Sub

        Private Sub GetConsiderCATDetailsFlagFeeCategoryList()
            If BasketEntity.CAT_MODE.Trim.Length > 0 Then
                _considerCATDetailsStatusFeeCategoryList = TDataObjects.FeesSettings.TblFeesRelations.GetConsiderCATDetailsStatusFeeCategoryList(Settings.BusinessUnit)
            End If
        End Sub

        Private Sub MoveBasketFeesToBasketTables(ByVal isBookingFeeProcess As Boolean)
            If BasketItemEntityList IsNot Nothing AndAlso BasketItemEntityList.Count > 0 Then
                Dim isNotUpdatedPayTypeInHeader As Boolean = True
                Dim affectedRows As Integer = 0
                For itemIndex As Integer = 0 To BasketItemEntityList.Count - 1
                    If IsProductItselfAFee(BasketItemEntityList(itemIndex).Product) Then
                        'ok this basket product is fee product
                        'it may be valid for previous basket
                        'but is it valid fee product for current basket
                        'if exists update if not delete it
                        Dim basketFeesItemIndex As Integer = -1
                        If TryGetBasketDetailFeePdtIndex(BasketItemEntityList(itemIndex), basketFeesItemIndex) Then
                            'update the fee value
                            If _basketFeesEntityList(basketFeesItemIndex).FeeCategory = GlobalConstants.FEECATEGORY_VARIABLE Then
                                Dim existingFeeValue As Decimal = _basketFeesEntityList(basketFeesItemIndex).FeeValue
                                _basketFeesEntityList(basketFeesItemIndex).FeeValue = BasketItemEntityList(itemIndex).Net_Price
                                affectedRows = TDataObjects.BasketSettings.TblBasketDetail.UpdateFee(BasketEntity.Basket_Header_ID, LoginId, _basketFeesEntityList(basketFeesItemIndex))
                                _basketFeesEntityList(basketFeesItemIndex).FeeValue = existingFeeValue
                            Else
                                If _basketFeesEntityList(basketFeesItemIndex).FeeValue <> Nothing AndAlso _basketFeesEntityList(basketFeesItemIndex).IsNegativeFee Then
                                    _basketFeesEntityList(basketFeesItemIndex).FeeValue = _basketFeesEntityList(basketFeesItemIndex).FeeValue * (-1)
                                End If
                                affectedRows = TDataObjects.BasketSettings.TblBasketDetail.UpdateFee(BasketEntity.Basket_Header_ID, LoginId, _basketFeesEntityList(basketFeesItemIndex))
                            End If

                            If affectedRows <> Nothing AndAlso affectedRows > 0 Then
                                If _basketFeesEntityList(basketFeesItemIndex).FeeCategory = GlobalConstants.FEECATEGORY_BOOKING Then isNotUpdatedPayTypeInHeader = False
                                _basketFeesEntityList(basketFeesItemIndex).ExistsInBasket = _basketFeesEntityList(basketFeesItemIndex).IsSystemFee
                            Else
                                'error log it
                            End If
                        Else
                            'delete the fee value if it is not from external
                            If Not BasketItemEntityList(itemIndex).IS_EXTERNAL Then
                                'delete booking fee only when you process booking fee
                                If isBookingFeeProcess Then
                                    If BasketItemEntityList(itemIndex).FEE_CATEGORY = GlobalConstants.FEECATEGORY_BOOKING Then
                                        TDataObjects.BasketSettings.TblBasketDetail.DeleteFee(BasketItemEntityList(itemIndex).Product, BasketEntity.Basket_Header_ID)
                                    End If
                                Else
                                    If BasketItemEntityList(itemIndex).FEE_CATEGORY <> GlobalConstants.FEECATEGORY_BOOKING Then
                                        If BasketItemEntityList(itemIndex).Product <> Settings.EcommerceModuleDefaultsValues.CashBackFeeCode Then
                                            TDataObjects.BasketSettings.TblBasketDetail.DeleteFee(BasketItemEntityList(itemIndex).Product, BasketEntity.Basket_Header_ID)
                                        End If
                                    End If
                                End If
                            End If
                        End If

                    End If
                Next

                'add the fees which has to be added to basket detail / basket fees table
                'clear basket fees table first
                If isBookingFeeProcess Then
                    affectedRows = TDataObjects.BasketSettings.TblBasketFees.DeleteBookingFee(BasketEntity.Basket_Header_ID, False)
                Else
                    affectedRows = TDataObjects.BasketSettings.TblBasketFees.DeleteExceptBookingFee(BasketEntity.Basket_Header_ID, False)
                End If

                Dim canApplyInBasketDetail As Boolean = False,
                    canApplyInBasketFees As Boolean = False,
                    canUpdatePayTypeInHeader As Boolean = False,
                    paymentType As String = String.Empty
                For itemIndex As Integer = 0 To _basketFeesEntityList.Count - 1
                    Dim canAddBookingFees As Boolean = True
                    If BasketEntity.PAYMENT_TYPE = GlobalConstants.DDPAYMENTTYPE And _basketFeesEntityList(itemIndex).FeeCategory = GlobalConstants.FEECATEGORY_BOOKING Then
                        canAddBookingFees = False
                    End If

                    If canAddBookingFees Then
                        affectedRows = 0
                        canApplyInBasketDetail = False
                        canApplyInBasketFees = False
                        canUpdatePayTypeInHeader = False
                        If Not _basketFeesEntityList(itemIndex).ExistsInBasket Then
                            FindWhereToApplyThisFee(_basketFeesEntityList(itemIndex), canApplyInBasketDetail, canApplyInBasketFees, canUpdatePayTypeInHeader, paymentType)
                            If canApplyInBasketDetail Then
                                affectedRows = 0
                                If _basketFeesEntityList(itemIndex).FeeValue <> Nothing AndAlso _basketFeesEntityList(itemIndex).IsNegativeFee Then
                                    _basketFeesEntityList(itemIndex).FeeValue = _basketFeesEntityList(itemIndex).FeeValue * (-1)
                                End If
                                affectedRows = TDataObjects.BasketSettings.TblBasketDetail.UpdateOrInsertFee(_basketFeesEntityList(itemIndex).BasketHeaderID, LoginId, _basketFeesEntityList(itemIndex))
                            End If
                            If canApplyInBasketFees Then
                                affectedRows = 0
                                affectedRows = TDataObjects.BasketSettings.TblBasketFees.Insert(BasketEntity.Basket_Header_ID, _basketFeesEntityList(itemIndex), False)
                            End If
                            If isNotUpdatedPayTypeInHeader AndAlso canUpdatePayTypeInHeader Then
                                affectedRows = 0
                                affectedRows = TDataObjects.BasketSettings.TblBasketHeader.UpdatePaymentAndCardType(_basketFeesEntityList(itemIndex).BasketHeaderID, paymentType, _basketFeesEntityList(itemIndex).CardType)
                                isNotUpdatedPayTypeInHeader = False
                            End If
                        Else
                            If _basketFeesEntityList(itemIndex).FeeCategory = GlobalConstants.FEECATEGORY_BOOKING Then
                                affectedRows = TDataObjects.BasketSettings.TblBasketFees.Insert(BasketEntity.Basket_Header_ID, _basketFeesEntityList(itemIndex), False)
                            End If
                        End If
                    End If
                Next

                If isNotUpdatedPayTypeInHeader AndAlso (Not IsUpdatedBasketProcess) Then
                    'booking fee may be excluded from basket so update the basket header 
                    'with default pay and card type
                    If BasketEntity.PAYMENT_TYPE = GlobalConstants.DDPAYMENTTYPE Then
                        TDataObjects.BasketSettings.TblBasketHeader.UpdatePaymentAndCardType(BasketEntity.Basket_Header_ID, GlobalConstants.DDPAYMENTTYPE, _defaultCardTypeCode)
                    Else
                        TDataObjects.BasketSettings.TblBasketHeader.UpdatePaymentAndCardType(BasketEntity.Basket_Header_ID, GlobalConstants.CCPAYMENTTYPE, _defaultCardTypeCode)
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' Make a decision about which table to move a given fee to based on the properties in the fee entity. This is done based on fee category.
        ''' </summary>
        ''' <param name="basketFeeEntity">The fee data entity currently working with</param>
        ''' <param name="canApplyInBasketDetail">Can this fee be applied to the basket detail table</param>
        ''' <param name="canApplyInBasketFees">Can this fee be applied to the basket fees table</param>
        ''' <param name="canUpdatePayTypeInHeader">Can the payment type column be updated in tbl_basket_header</param>
        ''' <param name="paymentType">What should the payment type column be updated to?</param>
        ''' <remarks></remarks>
        Private Sub FindWhereToApplyThisFee(ByVal basketFeeEntity As DEBasketFees, ByRef canApplyInBasketDetail As Boolean, ByRef canApplyInBasketFees As Boolean, ByRef canUpdatePayTypeInHeader As Boolean, ByRef paymentType As String)
            If basketFeeEntity.IsSystemFee Then
                'is it payment type fee
                Select Case basketFeeEntity.FeeCategory
                    Case GlobalConstants.FEECATEGORY_BOOKING
                        canApplyInBasketFees = True
                        If (BasketEntity.PAYMENT_TYPE = "" OrElse BasketEntity.PAYMENT_TYPE = GlobalConstants.CCPAYMENTTYPE) AndAlso basketFeeEntity.CardType = _defaultCardTypeCode Then
                            canApplyInBasketDetail = True
                            canUpdatePayTypeInHeader = True
                            paymentType = GlobalConstants.CCPAYMENTTYPE
                        ElseIf (BasketEntity.PAYMENT_TYPE IsNot Nothing) AndAlso (BasketEntity.PAYMENT_TYPE.Trim.Length > 0) Then
                            Dim dicPayTypeFeeCategory As Dictionary(Of String, DEFeesRelations) = TDataObjects.FeesSettings.TblFeesRelations.GetPayTypeFeeCategoryList(Settings.BusinessUnit)
                            If dicPayTypeFeeCategory IsNot Nothing Then
                                Dim feeRelationsEntity As DEFeesRelations = Nothing
                                If dicPayTypeFeeCategory.TryGetValue(BasketEntity.PAYMENT_TYPE, feeRelationsEntity) Then
                                    If feeRelationsEntity.FeeCategory = GlobalConstants.FEECATEGORY_BOOKING Then
                                        Dim tempCardType As String = GlobalConstants.FEE_BOOKING_CARDTYPE_EMPTY
                                        If BasketEntity.CARD_TYPE_CODE.Trim.Length > 0 Then
                                            tempCardType = BasketEntity.CARD_TYPE_CODE
                                        End If
                                        If tempCardType = basketFeeEntity.CardType Then
                                            canApplyInBasketDetail = True
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    Case GlobalConstants.FEECATEGORY_DIRECTDEBIT
                        If BasketEntity.PAYMENT_TYPE = GlobalConstants.DDPAYMENTTYPE Then
                            canApplyInBasketDetail = True
                            canApplyInBasketFees = True
                            canUpdatePayTypeInHeader = True
                            paymentType = GlobalConstants.DDPAYMENTTYPE
                        Else
                            canApplyInBasketFees = True
                        End If
                    Case GlobalConstants.FEECATEGORY_FINANCE
                        If BasketEntity.PAYMENT_TYPE = GlobalConstants.CFPAYMENTTYPE Then
                            canApplyInBasketDetail = True
                            canApplyInBasketFees = True
                            canUpdatePayTypeInHeader = True
                            paymentType = GlobalConstants.CFPAYMENTTYPE
                        Else
                            canApplyInBasketFees = True
                        End If
                    Case Else
                        canApplyInBasketDetail = True
                End Select
            Else
                canApplyInBasketFees = True
            End If
        End Sub

        Private Function GetDepartment(ByVal agentEntity As DEAgent) As String
            Dim department As String = ""
            If agentEntity IsNot Nothing AndAlso agentEntity.AgentUsername.Trim.Length > 0 Then
                department = agentEntity.Department
            Else
                department = _webSalesDepartment
            End If
            Return department
        End Function

        Private Function GetFeeCategoryToProcess() As List(Of String)
            Dim feeCategoriesToProcess As New List(Of String)

            For itemIndex As Integer = 0 To _feesCategoryList.Count - 1

                If IsFeeCategoryValidForProcessing(_feesCategoryList.Item(itemIndex)) Then
                    feeCategoriesToProcess.Add(_feesCategoryList.Item(itemIndex))
                End If

            Next

            Return feeCategoriesToProcess
        End Function

        Private Function IsFeeCategoryValidForProcessing(ByVal feeCategoryName As String) As Boolean
            Dim isValid As Boolean = True
            Select Case feeCategoryName
                Case GlobalConstants.FEECATEGORY_CANCEL
                    If BasketEntity.CAT_MODE.Trim <> GlobalConstants.CATMODE_CANCEL Then
                        isValid = False
                    End If
                Case GlobalConstants.FEECATEGORY_AMEND
                    If BasketEntity.CAT_MODE.Trim <> GlobalConstants.CATMODE_AMEND Then
                        isValid = False
                    End If
                Case GlobalConstants.FEECATEGORY_TRANSFER
                    If BasketEntity.CAT_MODE.Trim <> GlobalConstants.CATMODE_TRANSFER Then
                        isValid = False
                    End If
                Case GlobalConstants.FEECATEGORY_WEBSALES
                    If _basketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
                        isValid = False
                    Else
                        If Settings.OriginatingSourceCode <> GlobalConstants.SOURCE Then
                            isValid = False
                        ElseIf Settings.OriginatingSourceCode = GlobalConstants.SOURCE AndAlso Not String.IsNullOrWhiteSpace(Settings.AgentEntity.AgentUsername) Then
                            isValid = False
                        ElseIf BasketEntity.CAT_MODE.Trim.Length > 0 Then
                            isValid = False
                        End If
                    End If

                Case GlobalConstants.FEECATEGORY_SUPPLYNET
                    If Settings.OriginatingSourceCode <> GlobalConstants.SOURCESUPPLYNET Then
                        isValid = False
                    ElseIf BasketEntity.CAT_MODE.Trim = GlobalConstants.CATMODE_CANCEL Then
                        isValid = False
                    End If
                Case GlobalConstants.FEECATEGORY_ADHOC
                    If Settings.OriginatingSourceCode = GlobalConstants.SOURCESUPPLYNET Then
                        isValid = False
                    ElseIf BasketEntity.CAT_MODE.Trim = GlobalConstants.CATMODE_CANCEL Then
                        isValid = False
                    End If
                Case GlobalConstants.FEECATEGORY_VARIABLE
                    If _basketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
                        isValid = False
                    Else
                        If Settings.OriginatingSourceCode = GlobalConstants.SOURCESUPPLYNET Then
                            isValid = False
                        ElseIf String.IsNullOrWhiteSpace(Settings.AgentEntity.AgentUsername) Then
                            isValid = False
                        ElseIf BasketEntity.CAT_MODE.Trim = GlobalConstants.CATMODE_CANCEL Then
                            isValid = False
                        End If
                    End If
                Case GlobalConstants.FEECATEGORY_BOOKING
                    If (_basketContentType <> GlobalConstants.MERCHANDISEBASKETCONTENTTYPE) AndAlso (Not BasketEntity.PAYMENT_OPTIONS.Contains(GlobalConstants.CCPAYMENTTYPE)) Then
                        isValid = False
                    ElseIf BasketEntity.CAT_MODE.Trim = GlobalConstants.CATMODE_CANCEL Then
                        isValid = False
                    End If
                    'booking has to be processed but not now in the end
                    If isValid Then
                        _canProcessBookingFeeCategory = isValid
                        isValid = False
                    End If
                Case GlobalConstants.FEECATEGORY_DIRECTDEBIT
                    If _basketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
                        isValid = False
                    Else
                        If Not BasketEntity.PAYMENT_OPTIONS.Contains(GlobalConstants.DDPAYMENTTYPE) Then
                            isValid = False
                        ElseIf BasketEntity.CAT_MODE.Trim = GlobalConstants.CATMODE_CANCEL Then
                            isValid = False
                        End If
                    End If
                Case GlobalConstants.FEECATEGORY_FINANCE
                    If _basketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
                        isValid = False
                    Else
                        If Not BasketEntity.PAYMENT_OPTIONS.Contains(GlobalConstants.CFPAYMENTTYPE) Then
                            isValid = False
                        ElseIf BasketEntity.CAT_MODE.Trim = GlobalConstants.CATMODE_CANCEL Then
                            isValid = False
                        End If
                    End If
                Case GlobalConstants.FEECATEGORY_FULFILMENT
                    If _basketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
                        isValid = False
                    Else
                        If BasketEntity.CAT_MODE.Trim = GlobalConstants.CATMODE_CANCEL Then
                            isValid = False
                        End If
                    End If
            End Select
            Return isValid
        End Function

        Private Function GetAllFeesEntities() As Boolean
            Dim talFees As New TalentFees
            talFees.Settings = Settings
            talFees.FulfilmentFeeCategory = FulfilmentFeeCategory
            Dim errObj As New ErrorObj
            errObj = talFees.FeesList()
            If Not (errObj.HasError OrElse talFees.ResultDataSet Is Nothing) Then
                If talFees.FeesCategoryList IsNot Nothing Then
                    _feesCategoryList = talFees.FeesCategoryList
                End If
                If Settings.IsAgent Then
                    _feesToProcess = talFees.Fees
                    _feeCodesToProcess = talFees.FeeCodes
                Else
                    _feesToProcess = talFees.FeesApplyToWeb
                    _feeCodesToProcess = talFees.FeeCodesApplyToWeb
                End If
                If talFees.ProductExcludeFees IsNot Nothing Then
                    _productExcludeFees = talFees.ProductExcludeFees
                End If
                If Not String.IsNullOrWhiteSpace(talFees.WebSalesDepartment) Then
                    _webSalesDepartment = talFees.WebSalesDepartment
                End If
            End If
            GetConsiderCATDetailsFlagFeeCategoryList()
            talFees = Nothing
            Return (Not errObj.HasError)
        End Function

        Private Function IsValidBasketToProcess(ByVal isBookingFeeProcess As Boolean) As Boolean
            'basketentity should not be nothing and has one count
            'basketitementity should not be nothing and has atleast one count
            Dim isValid As Boolean = False
            If GetAllFeesEntities() Then
                PopulateBasketItemsRequiresFees(isBookingFeeProcess)
                If _basketItemsRequiresFees.Count > 0 Then
                    PopulateBasketHeaderMergList()
                    isValid = True
                Else
                    'todo what happens to fees which are created in tbl_basket_fees table on previous process when the itemrequiresfees count is zero
                    'how it is working now
                    If isBookingFeeProcess Then
                        'delete booking fee in basket detail
                        TDataObjects.BasketSettings.TblBasketDetail.DeleteAllPayTypeFees(BasketEntity.Basket_Header_ID)
                    Else
                        'no item valid fees processing
                        'delete all the system fees which are not related to current basket
                        TDataObjects.BasketSettings.TblBasketDetail.DeleteAllSystemFees(BasketEntity.Basket_Header_ID)
                    End If
                End If
            End If
            Return isValid
        End Function

        Private Function IsCATBasket() As Boolean
            Dim isCAT As Boolean = False
            If BasketEntity IsNot Nothing Then
                If BasketEntity.CAT_MODE.Trim.Length > 0 Then
                    isCAT = True
                End If
            End If
            Return isCAT
        End Function

        Private Function IsProductRequiresFees(ByVal basketItem As DEBasketItem, ByVal isBookingFeeProcess As Boolean) As Boolean
            Dim isFeesRequired As Boolean = True
            If basketItem.MODULE_ = GlobalConstants.BASKETMODULETICKETING Then
                If isBookingFeeProcess Then
                    'ticketing product is it free or is it excluded from fees as it is booking fees processor, consider fees as product
                    If basketItem.IS_FREE _
                        OrElse Utilities.CheckForDBNull_Boolean_DefaultFalse(basketItem.CANNOT_APPLY_FEES) _
                        OrElse IsProductExcludedFromFees(basketItem.Product) Then
                        isFeesRequired = False
                    ElseIf basketItem.Product = Settings.EcommerceModuleDefaultsValues.CashBackFeeCode Then
                        isFeesRequired = False
                    Else
                        If IsProductItselfAFee(basketItem.Product) AndAlso basketItem.Product = GlobalConstants.BKFEE Then
                            isFeesRequired = False
                        End If
                    End If
                Else
                    'ticketing product is it free or is it excluded from fees
                    If basketItem.IS_FREE _
                        OrElse Utilities.CheckForDBNull_Boolean_DefaultFalse(basketItem.CANNOT_APPLY_FEES) _
                        OrElse IsProductExcludedFromFees(basketItem.Product) _
                        OrElse IsProductItselfAFee(basketItem.Product) Then
                        isFeesRequired = False
                    End If
                End If

            Else
                'product related to retail
                isFeesRequired = True
            End If
            Return isFeesRequired
        End Function

        Private Function IsProductExcludedFromFees(ByVal productCode As String) As Boolean
            Dim isExcluded As Boolean = False
            If _productExcludeFees IsNot Nothing And _productExcludeFees.Count > 0 Then
                If _productExcludeFees.ContainsKey(productCode) Then
                    isExcluded = True
                End If
            End If
            Return isExcluded
        End Function

        Private Function IsProductItselfAFee(ByVal productCode As String) As Boolean
            Dim isFee As Boolean = False
            If _feeCodesToProcess.ContainsKey(productCode) Then
                isFee = True
            ElseIf productCode = Settings.EcommerceModuleDefaultsValues.CashBackFeeCode Then
                isFee = True
            End If
            Return isFee
        End Function

        Private Function TryGetBasketDetailFeePdtIndex(ByVal basketItem As DEBasketItem, ByRef basketFeesItemIndex As Integer) As Boolean
            Dim isValid As Boolean = False
            If basketItem.FEE_CATEGORY = GlobalConstants.FEECATEGORY_BOOKING Then
                isValid = TryGetBasketDetailBookingFeePdtIndex(basketItem, basketFeesItemIndex)
            Else
                For itemIndex As Integer = 0 To _basketFeesEntityList.Count - 1
                    If _basketFeesEntityList(itemIndex).FeeCode = basketItem.Product And (Not basketItem.IS_EXTERNAL) Then
                        isValid = True
                        basketFeesItemIndex = itemIndex
                        Exit For
                    End If
                Next
            End If
            Return isValid
        End Function

        Private Function TryGetBasketDetailBookingFeePdtIndex(ByVal basketItem As DEBasketItem, ByRef basketFeesItemIndex As Integer) As Boolean
            Dim isValid As Boolean = False
            For itemIndex As Integer = 0 To _basketFeesEntityList.Count - 1
                If _basketFeesEntityList(itemIndex).FeeCode = basketItem.Product And (Not basketItem.IS_EXTERNAL) Then
                    If _basketFeesEntityList(itemIndex).CardType = BasketEntity.CARD_TYPE_CODE Then
                        isValid = True
                        basketFeesItemIndex = itemIndex
                        Exit For
                    End If
                End If
            Next
            Return isValid
        End Function

        Private Function IsFeeNotExcludedForBasket(ByVal feeEntity As DEBasketFees) As Boolean
            Dim isNotExcluded As Boolean = True
            If _dtExcludedFeesForBasket IsNot Nothing AndAlso _dtExcludedFeesForBasket.Rows.Count > 0 Then
                For rowIndex As Integer = 0 To _dtExcludedFeesForBasket.Rows.Count - 1
                    If _dtExcludedFeesForBasket.Rows(rowIndex)("FEE_CODE") = feeEntity.FeeCode Then
                        isNotExcluded = False
                        Exit For
                    ElseIf _dtExcludedFeesForBasket.Rows(rowIndex)("FEE_CODE") = GlobalConstants.FEESEXCLUDED_ALL AndAlso feeEntity.IsSystemFee Then
                        isNotExcluded = False
                        Exit For
                    End If
                Next
            End If
            Return isNotExcluded
        End Function

        Private Function GetConsiderCATDetailsOnCATStatus(ByVal feeCategory As String) As String
            Dim considerStatus As String = GlobalConstants.FEE_CONSIDER_CAT_STATUS_STANDARD
            If BasketEntity.CAT_MODE.Trim.Length > 0 AndAlso _considerCATDetailsStatusFeeCategoryList IsNot Nothing AndAlso _considerCATDetailsStatusFeeCategoryList.Count > 0 Then
                'basket is in cat mode
                _considerCATDetailsStatusFeeCategoryList.TryGetValue(feeCategory, considerStatus)
            End If
            Return considerStatus
        End Function

        Private Function ProcessPartPaymentFees(ByVal basketHeaderID As String, ByVal currentPaymentType As String) As Boolean
            Dim talPartPayment As New TalentPartPayment
            talPartPayment.Settings = Settings
            talPartPayment.BasketHeaderID = basketHeaderID
            talPartPayment.CardTypeFeeCategory = CardTypeFeeCategory
            talPartPayment.FulfilmentFeeCategory = FulfilmentFeeCategory
            talPartPayment.CurrentPaymentType = currentPaymentType
            If talPartPayment.CurrentPaymentType IsNot Nothing AndAlso talPartPayment.CurrentPaymentType = GlobalConstants.CCPAYMENTTYPE Then
                talPartPayment.ProcessPartPaymentBookingFees()
            End If

            Return talPartPayment.canProcessBookingFees
        End Function

        Private Function TryGetFeelistByBasketContentType(ByVal feeCategory As String, ByRef feeEntityList As List(Of DEFees)) As Boolean
            Dim isExists As Boolean = False
            Dim feeEntityListFromCategory As List(Of DEFees) = Nothing
            If _feesToProcess.TryGetValue(feeCategory, feeEntityListFromCategory) Then
                If _basketContentType = GlobalConstants.TICKETINGBASKETCONTENTTYPE Then
                    feeEntityList = (From feeEntity In feeEntityListFromCategory Where (feeEntity.ApplyFeeTo = GlobalConstants.FEEAPPLYTO_TICKETING OrElse feeEntity.ApplyFeeTo = GlobalConstants.FEEAPPLYTO_BOTH)).ToList()
                    isExists = feeEntityList.Count > 0
                ElseIf _basketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
                    feeEntityList = (From feeEntity In feeEntityListFromCategory Where (feeEntity.ApplyFeeTo = GlobalConstants.FEEAPPLYTO_MERCHANDISE OrElse feeEntity.ApplyFeeTo = GlobalConstants.FEEAPPLYTO_BOTH)).ToList()
                    isExists = feeEntityList.Count > 0
                ElseIf _basketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
                    feeEntityList = feeEntityListFromCategory
                    isExists = feeEntityList.Count > 0
                End If
            End If
            Return isExists
        End Function

#End Region

    End Class
End Namespace
