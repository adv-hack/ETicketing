Imports System.Data.SqlClient
Imports System.Text
Imports Talent.Common.DataObjects.TableObjects

Namespace DataObjects

    ''' <summary>
    ''' Class provides the functionality to access basket
    ''' </summary>
    <Serializable()> _
    Public Class Basket
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings
        Private _tblBasketHeader As tbl_basket_header
        Private _tblBasketDetail As tbl_basket_detail
        Private _tblBasketDetailExceptions As tbl_basket_detail_exceptions
        Private _tblBasketStatus As tbl_basket_status
        Private _tblBasketFees As tbl_basket_fees
        Private _tblBasketPromotionItems As tbl_basket_promotion_items
        Private _tblGoogleCheckoutSerialNumberStatus As tbl_googlecheckout_serialnumber_status
        Private _basketHelperEntity As New DEGetBasketHelper
        Private _tblBasketPayment As tbl_basket_payment

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "Basket"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Basket" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"

        Public Property BasketHelperEntity() As DEGetBasketHelper
            Set(value As DEGetBasketHelper)
                _basketHelperEntity = value
            End Set
            Get
                Return _basketHelperEntity
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_basket_header instance with DESettings
        ''' </summary>
        ''' <value>tbl_basket_header instance</value>
        Public ReadOnly Property TblBasketHeader() As tbl_basket_header
            Get
                If (_tblBasketHeader Is Nothing) Then
                    _tblBasketHeader = New tbl_basket_header(_settings)
                End If
                Return _tblBasketHeader
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_basket_detail instance with DESettings
        ''' </summary>
        ''' <value>tbl_basket_detail instance</value>
        Public ReadOnly Property TblBasketDetail() As tbl_basket_detail
            Get
                If (_tblBasketDetail Is Nothing) Then
                    _tblBasketDetail = New tbl_basket_detail(_settings)
                End If
                Return _tblBasketDetail
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_basket_detail_exceptions instance with DESettings
        ''' </summary>
        ''' <value>tbl_basket_detail_exceptions instance</value>
        Public ReadOnly Property TblBasketDetailExceptions() As tbl_basket_detail_exceptions
            Get
                If (_tblBasketDetailExceptions Is Nothing) Then
                    _tblBasketDetailExceptions = New tbl_basket_detail_exceptions(_settings)
                End If
                Return _tblBasketDetailExceptions
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_basket_status instance with DESettings
        ''' </summary>
        ''' <value>tbl_basket_status instance</value>
        Public ReadOnly Property TblBasketStatus() As tbl_basket_status
            Get
                If (_tblBasketStatus Is Nothing) Then
                    _tblBasketStatus = New tbl_basket_status(_settings)
                End If
                Return _tblBasketStatus
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_basket_fees instance with DESettings
        ''' </summary>
        ''' <value>tbl_basket_fees instance</value>
        Public ReadOnly Property TblBasketFees() As tbl_basket_fees
            Get
                If (_tblBasketFees Is Nothing) Then
                    _tblBasketFees = New tbl_basket_fees(_settings)
                End If
                Return _tblBasketFees
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_basket_promotion_items instance with DESettings
        ''' </summary>
        ''' <value>tbl_basket_promotion_items instance</value>
        Public ReadOnly Property TblBasketPromotionItems() As tbl_basket_promotion_items
            Get
                If (_tblBasketPromotionItems Is Nothing) Then
                    _tblBasketPromotionItems = New tbl_basket_promotion_items(_settings)
                End If
                Return _tblBasketPromotionItems
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_basket_payment instance with DESettings
        ''' </summary>
        ''' <value>tbl_basket_payment instance</value>       
        Public ReadOnly Property TblBasketPayment() As tbl_basket_payment
            Get
                If (_tblBasketPayment Is Nothing) Then
                    _tblBasketPayment = New tbl_basket_payment(_settings)
                End If
                Return _tblBasketPayment
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_googlecheckout_serialnumber_status instance with DESettings
        ''' </summary>
        ''' <value>tbl_googlecheckout_serialnumber_status instance</value>
        Public ReadOnly Property TblGoogleCheckoutSerialNumberStatus() As tbl_googlecheckout_serialnumber_status
            Get
                If (_tblGoogleCheckoutSerialNumberStatus Is Nothing) Then
                    _tblGoogleCheckoutSerialNumberStatus = New tbl_googlecheckout_serialnumber_status(_settings)
                End If
                Return _tblGoogleCheckoutSerialNumberStatus
            End Get
        End Property



#End Region

#Region "Public Methods"

        Public Function GetBasket() As DataSet
            Dim outputDataSet As New DataSet
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandType = CommandType.StoredProcedure
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "usp_Basket_GetBasket"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_SessionID", _basketHelperEntity.SessionID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_IsAnonymous", _basketHelperEntity.IsAnonymous))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_IsAuthenticated", _basketHelperEntity.IsAuthenticated))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_LoginID", _basketHelperEntity.LoginId))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_BusinessUnit", _basketHelperEntity.BusinessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_Partner", _basketHelperEntity.Partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_Processed", _basketHelperEntity.Processed))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_StockError", _basketHelperEntity.StockError))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_MarketingCampaign", _basketHelperEntity.MarketingCampaign))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_UserSelectFulfilment", _basketHelperEntity.UserSelectFulfilment))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_PaymentOptions", _basketHelperEntity.PaymentOptions))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_RestrictPaymentOptions", _basketHelperEntity.RestrictPaymentOptions))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_CampaignCode", _basketHelperEntity.CampaignCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_PaymentAccountID", _basketHelperEntity.PaymentAccountID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_ExternalPaymentToken", _basketHelperEntity.ExternalPaymentToken))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_CatMode", _basketHelperEntity.CatMode))

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataSet = talentSqlAccessDetail.ResultDataSet
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataSet
        End Function

        Public Function GetHeaderIDBySupplyNetSessionID() As String
            Dim basketHeaderID As String = "0"
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandType = CommandType.StoredProcedure
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "usp_Basket_GetHeaderIDBySupplyNetSessionID"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_SessionID", _basketHelperEntity.SessionID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_IsAnonymous", _basketHelperEntity.IsAnonymous))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_IsAuthenticated", _basketHelperEntity.IsAuthenticated))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_LoginID", _basketHelperEntity.LoginId))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_BusinessUnit", _basketHelperEntity.BusinessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_Partner", _basketHelperEntity.Partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_Processed", _basketHelperEntity.Processed))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_StockError", _basketHelperEntity.StockError))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_MarketingCampaign", _basketHelperEntity.MarketingCampaign))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_UserSelectFulfilment", _basketHelperEntity.UserSelectFulfilment))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_PaymentOptions", _basketHelperEntity.PaymentOptions))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_RestrictPaymentOptions", _basketHelperEntity.RestrictPaymentOptions))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_CampaignCode", _basketHelperEntity.CampaignCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_PaymentAccountID", _basketHelperEntity.PaymentAccountID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_ExternalPaymentToken", _basketHelperEntity.ExternalPaymentToken))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_CatMode", _basketHelperEntity.CatMode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_CatPrice", _basketHelperEntity.CatPrice))

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    If talentSqlAccessDetail.ResultDataSet.Tables.Count > 0 AndAlso talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0 Then
                        basketHeaderID = Utilities.CheckForDBNull_String(talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0))
                    End If
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the basket header id 
            Return basketHeaderID
        End Function

        Public Function ProcessNewBasket(ByVal basketHeaderID As String, ByVal dtBasketHeader As DataTable, ByVal dtBasketDetail As DataTable, Optional ByVal dtBasketDetailExceptions As DataTable = Nothing) As ErrorObj
            Dim errObj As New ErrorObj
            Dim affectedRows As Integer = 0
            If dtBasketHeader IsNot Nothing AndAlso dtBasketDetail IsNot Nothing Then
                'always basket fees table as it will be populated again by basket fees processor
                TblBasketFees.DeleteAll(basketHeaderID)
                If dtBasketHeader.Rows.Count > 0 Then
                    affectedRows = TblBasketHeader.ProcessNewBasketUpdate(basketHeaderID, dtBasketHeader)
                End If

                If dtBasketDetail.Rows.Count > 0 Then
                    If affectedRows > 0 Then
                        'header update success
                        'delete basket detail and then insert
                        affectedRows = 0
                        affectedRows = TblBasketDetail.DeleteBasketDetail(basketHeaderID, GlobalConstants.BASKETMODULETICKETING)
                        If affectedRows < 0 Then
                            'pass back error object with error code
                            errObj.HasError = True
                            errObj.ErrorNumber = "PNBERR-002"
                            errObj.ErrorStatus = "Failed while deleting basket detail"
                        End If
                        'Delete any promotion items records for the current basket (this needs to happen before any basket detail records are created)
                        affectedRows = TblBasketPromotionItems.DeleteByBasketHeaderIdAndModule(basketHeaderID, GlobalConstants.BASKETMODULETICKETING)
                        If affectedRows < 0 Then
                            'pass back error object with error code
                            errObj.HasError = True
                            errObj.ErrorNumber = "PNBERR-003"
                            errObj.ErrorStatus = "Failed while delete basket promotion items"
                        End If
                        affectedRows = 0
                        affectedRows = ProcessNewBasketInsert(basketHeaderID, dtBasketDetail)
                        If affectedRows <= 0 Then
                            'pass back error object with error code
                            errObj.HasError = True
                            errObj.ErrorNumber = "PNBERR-004"
                            errObj.ErrorStatus = "Failed while inserting basket detail"
                        End If


                        'header update success
                        'now add the excluded fees to basket fees table
                        'delete all the previously added excluded fees
                        affectedRows = 0
                        affectedRows = TblBasketFees.Delete(basketHeaderID, True)
                        If affectedRows < 0 Then
                            'pass back error object with error code
                            errObj.HasError = True
                            errObj.ErrorNumber = "PNBERR-005"
                            errObj.ErrorStatus = "Failed while deleting exluded fees from basket fees"
                        End If
                        'deleted successfully insert now only when excluded fees exists
                        If Utilities.CheckForDBNull_String(dtBasketHeader.Rows(0)("FeesExcluded")).Trim.Length > 0 Then
                            affectedRows = 0
                            affectedRows = TblBasketFees.InsertExcludedFees(basketHeaderID, Utilities.CheckForDBNull_String(dtBasketHeader.Rows(0)("FeesExcluded")).Trim)
                            If affectedRows < 0 Then
                                'pass back error object with error code
                                errObj.HasError = True
                                errObj.ErrorNumber = "PNBERR-006"
                                errObj.ErrorStatus = "Failed while inserting exluded fees to basket fees"
                            End If
                        End If


                        'Delete any exception seats records for the current basket
                        affectedRows = TblBasketDetailExceptions.DeleteByBasketHeaderIdAndModule(basketHeaderID, GlobalConstants.BASKETMODULETICKETING)
                        If affectedRows < 0 Then
                            'pass back error object with error code
                            errObj.HasError = True
                            errObj.ErrorNumber = "PNBERR-009"
                            errObj.ErrorStatus = "Failed while delete basket exception seat items"
                        End If
                        'Insert any basket exception records
                        If dtBasketDetailExceptions IsNot Nothing AndAlso dtBasketDetailExceptions.Rows.Count > 0 Then
                            affectedRows = TblBasketDetailExceptions.InsertBasketDetailExceptionRecords(basketHeaderID, dtBasketDetailExceptions)
                            If affectedRows < 0 Then
                                'pass back error object with error code
                                errObj.HasError = True
                                errObj.ErrorNumber = "PNBERR-010"
                                errObj.ErrorStatus = "Failed while delete basket exception seat items"
                            End If
                        End If
                    Else
                        'pass back error object with error code
                        errObj.HasError = True
                        errObj.ErrorNumber = "PNBERR-001"
                        errObj.ErrorStatus = "Failed while updating basket header"
                    End If
                Else
                    affectedRows = 0
                    affectedRows = TblBasketHeader.UpdatePaymentAndCardType(basketHeaderID, "", "")
                    affectedRows = 0
                    affectedRows = TblBasketDetail.ClearBasketDetailByModule(basketHeaderID, GlobalConstants.BASKETMODULETICKETING)
                    If affectedRows < 0 Then
                        'pass back error object with error code
                        errObj.HasError = True
                        errObj.ErrorNumber = "PNBERR-007"
                        errObj.ErrorStatus = "Failed while deleting basket detail"
                    End If
                    'Delete any promotion items records for the current basket
                    affectedRows = TblBasketPromotionItems.DeleteByBasketHeaderIdAndModule(basketHeaderID, GlobalConstants.BASKETMODULETICKETING)
                    If affectedRows < 0 Then
                        'pass back error object with error code
                        errObj.HasError = True
                        errObj.ErrorNumber = "PNBERR-008"
                        errObj.ErrorStatus = "Failed while delete basket promotion items"
                    End If
                    'Delete any exception seats records for the current basket
                    affectedRows = TblBasketDetailExceptions.DeleteByBasketHeaderIdAndModule(basketHeaderID, GlobalConstants.BASKETMODULETICKETING)
                    If affectedRows < 0 Then
                        'pass back error object with error code
                        errObj.HasError = True
                        errObj.ErrorNumber = "PNBERR-009"
                        errObj.ErrorStatus = "Failed while delete basket exception seat items"
                    End If
                End If
            End If

            Return errObj
        End Function

        Public Function ProcessNewBasketInsert(ByVal basketHeaderID As String, ByVal dtBasketDetail As DataTable, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim basketDetailId As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            Dim sqlStatement As New StringBuilder
            Dim quantityFromBackend As Decimal = 0
            sqlStatement.Append(" INSERT INTO tbl_basket_detail ")
            sqlStatement.Append("([BASKET_HEADER_ID],[PRODUCT],[QUANTITY],[GROSS_PRICE],[MODULE],[SEAT],[RESERVED_SEAT],[PRICE_BAND],[LOGINID],[PRODUCT_DESCRIPTION1],[PRODUCT_DESCRIPTION2],[PRODUCT_DESCRIPTION3],[PRODUCT_DESCRIPTION4]")
            sqlStatement.Append(",[PRODUCT_DESCRIPTION5],[PRODUCT_DESCRIPTION6],[PRODUCT_DESCRIPTION7],[GROUP_LEVEL_01],[GROUP_LEVEL_02],[GROUP_LEVEL_03],[GROUP_LEVEL_04],[GROUP_LEVEL_05],[GROUP_LEVEL_06]")
            sqlStatement.Append(",[GROUP_LEVEL_07],[GROUP_LEVEL_08],[GROUP_LEVEL_09],[GROUP_LEVEL_10],[STOCK_ERROR],[IS_FREE],[STOCK_ERROR_CODE],[STOCK_LIMIT],[STOCK_REQUESTED],[PRICE_CODE],[PRODUCT_TYPE]")
            sqlStatement.Append(",[SIZE],[QUANTITY_AVAILABLE],[MASTER_PRODUCT],[STOCK_ERROR_DESCRIPTION],[PRODUCT_SUB_TYPE],[ALTERNATE_SKU],[NET_PRICE],[TAX_PRICE],[RESTRICTION_CODE],[FULFIL_OPT_POST],[FULFIL_OPT_COLL]")
            sqlStatement.Append(",[FULFIL_OPT_PAH],[FULFIL_OPT_UPL],[FULFIL_OPT_PRINT],[FULFIL_OPT_REGPOST],[CURR_FULFIL_SLCTN],[PACKAGE_ID],[TRAVEL_PRODUCT_SELECTED],[CAN_SAVE_AS_FAVOURITE_SEAT],[ORIGINAL_LOGINID]")
            sqlStatement.Append(",[ORIGINAL_PRICE],[CAT_SEAT_DETAILS],[VALID_PRICE_BANDS],[LINKED_PRODUCT_ID],[FEE_CATEGORY],[IS_SYSTEM_FEE],[IS_EXTERNAL],[IS_INCLUDED],[PRODUCT_TYPE_ACTUAL]")
            sqlStatement.Append(",[CUSTOMER_ALLOCATION],[VOUCHER_DEFINITION_ID],[VOUCHER_CODE],[ROVING],[CANNOT_APPLY_FEES],[CAT_QUANTITY],[CAT_FULFILMENT],[FINANCE_CLUB_PRODUCT_ID],[FINANCE_PLAN_ID]")
            sqlStatement.Append(",[BULK_SALES_ID], [PACKAGE_TYPE], [ALLOCATED_SEAT], [RESERVATION_CODE], [RESTRICTED_BASKET_OPTIONS], [DISPLAY_IN_A_CANCEL_BASKET], [CALL_ID]")
            sqlStatement.Append(" )VALUES( ")
            sqlStatement.Append("@BasketHeaderID,@Product,@Quantity,@Price,@Module,@Seat,@ReservedSeat,@PriceBand,@loginID,@ProductDescription1,@ProductDescription2,")
            sqlStatement.Append("@ProductDescription3,@ProductDescription4,@ProductDescription5,@ProductDescription6,@ProductDescription7,@GroupLevel01,@GroupLevel02,@GroupLevel03,@GroupLevel04,@GroupLevel05,")
            sqlStatement.Append("@GroupLevel06,@GroupLevel07,@GroupLevel08,@GroupLevel09,@GroupLevel10,@StockError,@IsFree,@StockErrorCode,@StockLimit,@StockRequested,@PriceCode,@ProductType,@Size,")
            sqlStatement.Append("@QuantityAvailable,@MasterProduct,@StockErrorDescription,@ProductSubType,@AlternateSKU,@NetPrice,@TaxPrice,@RestrictionCode,@Fulfil_Opt_Post,@Fulfil_Opt_Coll,@Fulfil_Opt_Pah,")
            sqlStatement.Append("@Fulfil_Opt_Upl,@Fulfil_Opt_Print,@Fulfil_Opt_RegPost,@Curr_Fulfil_Slctn,@Package_ID,@TravelProductSelected,@CanSaveAsFavouriteSeat,@OriginalLoginid,@OriginalPrice,")
            sqlStatement.Append("@CatSeatDetails,@ValidPriceBands,@LinkedProductId,@FeeCategory,@IsSystemFee,@IsExternal,@IsIncluded,@ProductTypeActual,@CustomerAllocation,@VoucherDefinitionId,@VoucherCode,")
            sqlStatement.Append("@Roving,@CannotApplyFees,@CATQuantity,@CATFulfilment,@FinanceClubProductId,@FinancePlanId,@BulkSalesID,@PackageType,@AllocatedSeat,@ReservationCode,@RestrictedBasketOptions, @DisplayInACancelBasket, @CallId) SELECT SCOPE_IDENTITY()")

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = String.Empty
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

            'loop it here so clear the command parameters first and then add it
            For rowIndex As Integer = 0 To dtBasketDetail.Rows.Count - 1
                talentSqlAccessDetail.CommandElements.CommandParameter.Clear()
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Product", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("ProductCode"))))
                quantityFromBackend = Utilities.CheckForDBNull_Decimal(dtBasketDetail.Rows(rowIndex)("Quantity"))
                If quantityFromBackend <= 0 Then
                    quantityFromBackend = 1
                End If

                ' For product Bundles, store the date range.
                Dim ProductDateStr As String = Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("ProductDate"))
                Dim ProductTimeStr As String = Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("ProductTime"))
                If _settings.EcommerceModuleDefaultsValues IsNot Nothing AndAlso _settings.EcommerceModuleDefaultsValues.ShowBundleDateAsRange And Not Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("BundleStartDate")) Is String.Empty Then
                    ProductDateStr = Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("BundleStartDate")) + " - " + Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("BundleEndDate"))
                    ProductTimeStr = String.Empty
                End If
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Quantity", quantityFromBackend))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Price", Utilities.CheckForDBNull_Decimal(dtBasketDetail.Rows(rowIndex)("Price"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Module", "Ticketing"))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Seat", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("Seat"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ReservedSeat", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("ReservedSeat"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PriceBand", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("PriceBand"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@loginID", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("CustomerNo"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductDescription1", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("ProductDescription"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductDescription2", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("ProductCompetitionText"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductDescription3", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("ProductOppositionCode"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductDescription4", ProductDateStr))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductDescription5", ProductTimeStr))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductDescription6", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("StandDescription"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductDescription7", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("AreaDescription"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GroupLevel01", ""))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GroupLevel02", ""))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GroupLevel03", ""))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GroupLevel04", ""))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GroupLevel05", ""))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GroupLevel06", ""))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GroupLevel07", ""))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GroupLevel08", ""))
                ' talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GroupLevel09", ""))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GroupLevel09", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("BandDescription"))))

                Dim group10Text As String = String.Empty
                If dtBasketDetail.Rows(rowIndex)("Roving").ToString = "Y" Then
                    group10Text = "ROVING"
                ElseIf dtBasketDetail.Rows(rowIndex)("Unreserved").ToString = "Y" Then
                    group10Text = "UNRESERVED"
                Else
                    group10Text = String.Empty
                End If
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GroupLevel10", group10Text))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StockError", False))
                If _settings.IsAgent AndAlso _settings.EcommerceModuleDefaultsValues IsNot Nothing AndAlso _settings.EcommerceModuleDefaultsValues.AGENT_MODE_IGNORE_FREE_PRODUCT_FLAG Then
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IsFree", False))
                Else
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IsFree", Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketDetail.Rows(rowIndex)("IsFree"))))
                End If
                If Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketDetail.Rows(rowIndex)("IsFree")) Then
                    If UCase(Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("ErrorCode"))) = "S" Then
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StockErrorCode", ""))
                    Else
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StockErrorCode", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("ErrorCode"))))
                    End If
                Else
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StockErrorCode", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("ErrorCode"))))
                End If
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StockLimit", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("ProductLimit"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StockRequested", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("UserLimit"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PriceCode", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("PriceCode"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductType", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("ProductType"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Size", ""))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@QuantityAvailable", 0))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@MasterProduct", ""))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StockErrorDescription", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("ErrorInformation"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductSubType", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("ProductSubType"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AlternateSKU", ""))
                Dim NetPrice As Decimal = 0
                If Utilities.CheckForDBNull_Decimal(dtBasketDetail.Rows(rowIndex)("Price")) > 0 AndAlso Utilities.CheckForDBNull_Decimal(dtBasketDetail.Rows(rowIndex)("VatValue")) > 0 Then
                    NetPrice = Utilities.CheckForDBNull_Decimal(dtBasketDetail.Rows(rowIndex)("Price")) - Utilities.CheckForDBNull_Decimal(dtBasketDetail.Rows(rowIndex)("VatValue"))
                    If NetPrice < 0 Then
                        NetPrice = 0
                    End If
                Else
                    If Utilities.CheckForDBNull_Decimal(dtBasketDetail.Rows(rowIndex)("Price")) > 0 Then
                        NetPrice = Utilities.CheckForDBNull_Decimal(dtBasketDetail.Rows(rowIndex)("Price"))
                    End If
                End If
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NetPrice", NetPrice, SqlDbType.Decimal))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TaxPrice", 0, SqlDbType.Decimal))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RestrictionCode", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("SeatRestriction"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Fulfil_Opt_Post", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("OptPostFul"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Fulfil_Opt_Coll", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("OptCollFul"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Fulfil_Opt_Pah", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("OptP@HFul"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Fulfil_Opt_Upl", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("OptSCUploadFul"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Fulfil_Opt_Print", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("OptPrintFul"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Fulfil_Opt_RegPost", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("OptRegPostFul"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Curr_Fulfil_Slctn", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("CurrentFulfilSlctn"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Package_ID", Utilities.CheckForDBNull_Decimal(dtBasketDetail.Rows(rowIndex)("PackageID"))))
                If Not String.IsNullOrEmpty(dtBasketDetail.Rows(rowIndex)("IncludeTravel").ToString.Trim) Then
                    If dtBasketDetail.Rows(rowIndex)("IncludeTravel").ToString.ToUpper = "Y" Then
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TravelProductSelected", True))
                    Else
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TravelProductSelected", False))
                    End If
                Else
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TravelProductSelected", False))
                End If
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CanSaveAsFavouriteSeat", Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketDetail.Rows(rowIndex)("CanSaveAsFavouriteSeat"))))
                If Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketDetail.Rows(rowIndex)("ReservedForCurrentLoginid")) Then
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OriginalLoginid", _settings.LoginId))
                Else
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OriginalLoginid", String.Empty))
                End If
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OriginalPrice", Utilities.CheckForDBNull_Decimal(dtBasketDetail.Rows(rowIndex)("OriginalPrice"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CatSeatDetails", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("CATSeatDetails"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ValidPriceBands", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("PricedBnds"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LinkedProductId", Utilities.CheckForDBNull_Int(dtBasketDetail.Rows(rowIndex)("LinkedProductId"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeCategory", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("FeeCategory"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IsSystemFee", Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketDetail.Rows(rowIndex)("IsSystemFee"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IsExternal", Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketDetail.Rows(rowIndex)("IsExternal"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IsIncluded", Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketDetail.Rows(rowIndex)("IsIncluded"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductTypeActual", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("ProductTypeActual"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CustomerAllocation", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("CustomerAllocation"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@VoucherDefinitionId", Utilities.CheckForDBNull_BigInt(dtBasketDetail.Rows(rowIndex)("VoucherDefinitionId"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@VoucherCode", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("VoucherCode"))))

                Dim roving As String = String.Empty
                If dtBasketDetail.Rows(rowIndex)("Roving").ToString = "Y" Then
                    roving = "ROVING"
                ElseIf dtBasketDetail.Rows(rowIndex)("Unreserved").ToString = "Y" Then
                    roving = "UNRESERVED"
                Else
                    roving = String.Empty
                End If
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Roving", roving))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CannotApplyFees", Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketDetail.Rows(rowIndex)("CannotApplyFees"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CATQuantity", Utilities.CheckForDBNull_Int(dtBasketDetail.Rows(rowIndex)("CATQuantity"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CATFulfilment", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("CATFulfilment"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FinanceClubProductId", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("FinanceClubProductId"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FinancePlanId", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("FinancePlanId"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BulkSalesID", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("BulkSalesID"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PackageType", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("PackageType"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AllocatedSeat", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("AllocatedSeat"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ReservationCode", Utilities.CheckForDBNull_String(dtBasketDetail.Rows(rowIndex)("ReservationCode"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RestrictedBasketOptions", Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketDetail.Rows(rowIndex)("RestrictedBasketOptions"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DisplayInACancelBasket", Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketDetail.Rows(rowIndex)("DisplayInACancelBasket"))))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CallId", Utilities.CheckForDBNull_BigInt(dtBasketDetail.Rows(rowIndex)("CallId"))))

                'Execute
                If (givenTransaction Is Nothing) Then
                    err = talentSqlAccessDetail.SQLAccess()
                Else
                    err = talentSqlAccessDetail.SQLAccess(givenTransaction)
                End If
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    basketDetailId = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                    affectedRows = 1
                Else
                    affectedRows = -1
                End If
                If affectedRows <= 0 Then
                    Exit For
                Else
                    'Insert to basket detail has succeeded, now insert any tbl_basket_promotions records here as we need the basket detail ID.
                    If basketDetailId > 0 Then
                        Dim promotionId1 As Integer = Utilities.CheckForDBNull_Int(dtBasketDetail.Rows(rowIndex)("PromotionId1"))
                        Dim promotionId2 As Integer = Utilities.CheckForDBNull_Int(dtBasketDetail.Rows(rowIndex)("PromotionId2"))
                        Dim promotionId3 As Integer = Utilities.CheckForDBNull_Int(dtBasketDetail.Rows(rowIndex)("PromotionId3"))
                        If promotionId1 > 0 Then affectedRows = TblBasketPromotionItems.InsertTicketingPromotion(basketHeaderID, basketDetailId, promotionId1)
                        If promotionId2 > 0 Then affectedRows = TblBasketPromotionItems.InsertTicketingPromotion(basketHeaderID, basketDetailId, promotionId2)
                        If promotionId3 > 0 Then affectedRows = TblBasketPromotionItems.InsertTicketingPromotion(basketHeaderID, basketDetailId, promotionId3)
                    End If
                End If
            Next

            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows
        End Function

        Public Function UpdateBookingFee(ByVal basketHeaderID As String, ByVal loginID As String, ByVal cardTypeCode As String) As Boolean
            Dim isUpdated As Boolean = False
            Dim dtBasketFees As DataTable = TblBasketFees.GetFeeByHeaderIDCodeCategory(basketHeaderID, GlobalConstants.FEECATEGORY_BOOKING, GlobalConstants.BKFEE, cardTypeCode)
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
                Dim affectedRows As Integer = -1
                'delete fee in basket detail
                affectedRows = TblBasketDetail.DeleteFee(GlobalConstants.BKFEE, basketHeaderID, GlobalConstants.FEECATEGORY_BOOKING)
                'insert the choosen one
                affectedRows = TblBasketDetail.InsertFee(basketHeaderID, loginID, basketFeesEntity, False)
                If affectedRows > 0 Then
                    'update basket header
                    affectedRows = TblBasketHeader.UpdatePaymentAndCardType(basketHeaderID, GlobalConstants.CCPAYMENTTYPE, cardTypeCode)
                    isUpdated = True
                End If
            End If
            Return isUpdated
        End Function

        Public Function HasMovedFeeInOrOutOfBasket(ByVal basketHeaderID As String, ByVal loginID As String, ByVal feeCode As String, ByVal feeCategory As String, ByVal addFeeToBasket As Boolean) As Boolean
            Dim movedSuccessfully As Boolean = False
            loginID = Utilities.PadLeadingZeros(loginID, 12)
            If addFeeToBasket Then
                movedSuccessfully = HasMovedFeeToBasketDetail(basketHeaderID, loginID, feeCategory, feeCode, Nothing)
            Else
                movedSuccessfully = HasDeletedFeeFromBasketDetail(basketHeaderID, loginID, feeCategory, feeCode)
            End If
            Return movedSuccessfully
        End Function

        Public Function HasMovedFeeInOrOutOfBasket(ByVal basketHeaderID As String, ByVal loginID As String, ByVal feeCode As String, ByVal feeCategory As String, ByVal addFeeToBasket As Boolean, ByVal feeValue As Decimal) As Boolean
            Dim movedSuccessfully As Boolean = False
            loginID = Utilities.PadLeadingZeros(loginID, 12)
            If addFeeToBasket Then
                movedSuccessfully = HasMovedFeeToBasketDetail(basketHeaderID, loginID, feeCategory, feeCode, feeValue)
            Else
                movedSuccessfully = HasDeletedFeeFromBasketDetail(basketHeaderID, loginID, feeCategory, feeCode)
            End If
            Return movedSuccessfully
        End Function

        Private Function HasMovedFeeToBasketDetail(ByVal basketHeaderID As String, ByVal loginID As String, ByVal feeCategory As String, ByVal feeCode As String, ByVal feeValue As Decimal)
            Dim movedSuccessfully As Boolean = False

            Dim dtBasketFees As DataTable = TblBasketFees.GetFeeByHeaderIDCodeCategory(basketHeaderID, feeCategory, feeCode)
            If dtBasketFees IsNot Nothing AndAlso dtBasketFees.Rows.Count > 0 Then
                'build basketfeesentity
                Dim basketFeesEntity As New DEBasketFees
                basketFeesEntity.BasketHeaderID = basketHeaderID
                basketFeesEntity.CardType = ""
                basketFeesEntity.FeeApplyType = 0
                basketFeesEntity.FeeCode = Utilities.CheckForDBNull_String(dtBasketFees.Rows(0)("FEE_CODE"))
                basketFeesEntity.FeeCategory = Utilities.CheckForDBNull_String(dtBasketFees.Rows(0)("FEE_CATEGORY"))
                basketFeesEntity.FeeDescription = Utilities.CheckForDBNull_String(dtBasketFees.Rows(0)("FEE_DESCRIPTION"))
                basketFeesEntity.FeeValue = Utilities.CheckForDBNull_Decimal(dtBasketFees.Rows(0)("FEE_VALUE"))
                basketFeesEntity.IsNegativeFee = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketFees.Rows(0)("IS_NEGATIVE_FEE"))
                If feeValue <> Nothing Then
                    If basketFeesEntity.IsNegativeFee Then
                        feeValue = feeValue * (-1)
                    End If
                    basketFeesEntity.FeeValue = feeValue
                Else
                    If basketFeesEntity.FeeValue <> Nothing Then
                        If basketFeesEntity.IsNegativeFee Then
                            basketFeesEntity.FeeValue = basketFeesEntity.FeeValue * (-1)
                        End If
                    End If
                End If
                basketFeesEntity.IsSystemFee = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketFees.Rows(0)("IS_SYSTEM_FEE"))
                basketFeesEntity.IsTransactional = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketFees.Rows(0)("IS_TRANSACTIONAL"))

                'move this to basket detail
                Dim affectedRows As Integer = -1
                affectedRows = TblBasketDetail.InsertFee(basketHeaderID, loginID, basketFeesEntity, False)
                If affectedRows > 0 Then
                    movedSuccessfully = True
                End If
            End If
            Return movedSuccessfully
        End Function

        Private Function HasDeletedFeeFromBasketDetail(ByVal basketHeaderID As String, ByVal loginID As String, ByVal feeCategory As String, ByVal feeCode As String) As Boolean
            Return ((TblBasketDetail.DeleteFee(feeCode, basketHeaderID, feeCategory)) > 0)
        End Function


        Private Function HasMovedFeeToBasketDetailOLD(ByVal basketHeaderID As String, ByVal loginID As String, ByVal feeCategory As String, ByVal feeCode As String) As Boolean
            Dim movedSuccessfully As Boolean = False

            Dim dtBasketFees As DataTable = TblBasketFees.GetFeeByHeaderIDCodeCategory(basketHeaderID, feeCategory, feeCode)
            If dtBasketFees IsNot Nothing AndAlso dtBasketFees.Rows.Count > 0 Then
                'build basketfeesentity
                Dim basketFeesEntity As New DEBasketFees
                basketFeesEntity.BasketHeaderID = basketHeaderID
                basketFeesEntity.CardType = ""
                basketFeesEntity.FeeApplyType = 0
                basketFeesEntity.FeeCode = Utilities.CheckForDBNull_String(dtBasketFees.Rows(0)("FEE_CODE"))
                basketFeesEntity.FeeCategory = Utilities.CheckForDBNull_String(dtBasketFees.Rows(0)("FEE_CATEGORY"))
                basketFeesEntity.FeeDescription = Utilities.CheckForDBNull_String(dtBasketFees.Rows(0)("FEE_DESCRIPTION"))
                basketFeesEntity.FeeValue = Utilities.CheckForDBNull_Decimal(dtBasketFees.Rows(0)("FEE_VALUE"))
                basketFeesEntity.IsSystemFee = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketFees.Rows(0)("IS_SYSTEM_FEE"))
                basketFeesEntity.IsTransactional = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketFees.Rows(0)("IS_TRANSACTIONAL"))

                'now delete fee in tbl basket fees
                Dim affectedRows As Integer = TblBasketFees.DeleteByHeaderIDCodeCategory(basketHeaderID, feeCategory, feeCode)

                If affectedRows > 0 Then
                    'deleted now move this to basket detail
                    affectedRows = -1
                    affectedRows = TblBasketDetail.InsertFee(basketHeaderID, loginID, basketFeesEntity, False)
                    If affectedRows > 0 Then
                        movedSuccessfully = True
                    End If
                End If
            End If
            Return movedSuccessfully
        End Function

        'Private Function HasMovedFeeToBasketFeesOLD(ByVal basketHeaderID As String, ByVal loginID As String, ByVal feeCategory As String, ByVal feeCode As String)
        '    Dim movedSuccessfully As Boolean = False

        '    Dim dtBasketDetailFees As DataTable = TblBasketDetail.GetDetailByHeaderIDFeeCode(basketHeaderID, feeCategory, feeCode)
        '    If dtBasketDetailFees IsNot Nothing AndAlso dtBasketDetailFees.Rows.Count > 0 Then
        '        'build basketfeesentity
        '        Dim basketFeesEntity As New DEBasketFees
        '        basketFeesEntity.BasketHeaderID = basketHeaderID
        '        basketFeesEntity.CardType = ""
        '        basketFeesEntity.FeeApplyType = 0
        '        basketFeesEntity.FeeCode = Utilities.CheckForDBNull_String(dtBasketDetailFees.Rows(0)("PRODUCT"))
        '        basketFeesEntity.FeeCategory = Utilities.CheckForDBNull_String(dtBasketDetailFees.Rows(0)("FEE_CATEGORY"))
        '        basketFeesEntity.FeeDescription = Utilities.CheckForDBNull_String(dtBasketDetailFees.Rows(0)("PRODUCT_DESCRIPTION1"))
        '        basketFeesEntity.FeeValue = Utilities.CheckForDBNull_Decimal(dtBasketDetailFees.Rows(0)("GROSS_PRICE"))
        '        basketFeesEntity.IsSystemFee = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketDetailFees.Rows(0)("IS_SYSTEM_FEE"))
        '        basketFeesEntity.IsTransactional = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtBasketDetailFees.Rows(0)("IS_TRANSACTIONAL"))

        '        'now delete fee in tbl basket detail
        '        Dim affectedRows As Integer = TblBasketDetail.DeleteFee(feeCode, basketHeaderID, feeCategory)

        '        If affectedRows > 0 Then
        '            'deleted now move this to basket detail
        '            affectedRows = -1
        '            affectedRows = TblBasketFees.Insert(basketHeaderID, basketFeesEntity)
        '            If affectedRows > 0 Then
        '                movedSuccessfully = True
        '            End If
        '        End If

        '    End If
        '    Return movedSuccessfully
        'End Function

        ''' <summary>
        ''' Get the basket detail records based on the supplied ticketing payment reference
        ''' </summary>
        ''' <param name="ticketingPaymentReference">The ticketing payment reference to get records from</param>
        ''' <returns>A dataset of tbl_basket_detail records</returns>
        ''' <remarks></remarks>
        Public Function GetBasketRecordsByTicketingPaymentReference(ByVal ticketingPaymentReference As String) As DataSet
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim outputDataSet As New DataSet
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_basket_detail] WHERE [BASKET_HEADER_ID] = (SELECT [BASKET_HEADER_ID] FROM [tbl_basket_header] WHERE [TICKETING_PAYMENT_REF] = @TicketingPaymentReference)"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TicketingPaymentReference", ticketingPaymentReference))

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataSet = talentSqlAccessDetail.ResultDataSet
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the basket header id 
            Return outputDataSet
        End Function

#End Region

        Public Class DEGetBasketHelper
            Public Property SessionID() As String = String.Empty
            Public Property IsAnonymous() As Boolean = False
            Public Property IsAuthenticated() As Boolean = False
            Public Property LoginId() As String = String.Empty
            Public Property BusinessUnit() As String = String.Empty
            Public Property Partner() As String = String.Empty
            Public Property Processed() As Boolean = False
            Public Property StockError() As Boolean = False
            Public Property MarketingCampaign() As Boolean = False
            Public Property UserSelectFulfilment() As String = String.Empty
            Public Property PaymentOptions() As String = String.Empty
            Public Property RestrictPaymentOptions() As String = String.Empty
            Public Property CampaignCode() As String = String.Empty
            Public Property PaymentAccountID() As String = String.Empty
            Public Property ExternalPaymentToken() As String = String.Empty
            Public Property CatMode() As String = String.Empty
            Public Property CatPrice() As String = String.Empty
        End Class
    End Class
End Namespace

