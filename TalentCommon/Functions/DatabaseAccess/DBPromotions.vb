Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Text
Imports System.Web
Imports IBM.Data.DB2.iSeries

''' <summary>
''' Class which holds the functionalities for promotions
''' </summary>
<Serializable()> _
Public Class DBPromotions
    Inherits DBAccess

#Region "Constants"

    Const GetYFree As String = "GetYFree"
    Const GetYDiscount As String = "GetYDiscount"
    Const GetDiscountForY As String = "GetDiscountForY"
    Const GetSuppliedDiscount As String = "GetSuppliedDiscount"
    Const GetGroupDeal As String = "GetGroupDeal"
    Const ProcessPromotions As String = "ProcessPromotions"
    Const PromotionDetails As String = "PromotionDetails"
    Const RetrievePromotionHistory As String = "RetrievePromotionHistory"
    Const RetrievePromotionHistoryDetail As String = "RetrievePromotionHistoryDetail"
    Const GetPartnerPromotions As String = "GetPartnerPromotions"

    'Constant Promotion types
    Const BUY_X_GET_Y_FREE As String = "BUY_X_GET_Y_FREE"
    Const BUY_X_GET_Y_DISCOUNT As String = "BUY_X_GET_Y_DISCOUNT"
    Const BUY_X_OR_MORE_GET_SUPPLIED_DISCOUNT As String = "BUY_X_OR_MORE_GET_SUPPLIED_DISCOUNT"
    Const BUY_X_OR_MORE_GET_Y_FREE As String = "BUY_X_OR_MORE_GET_Y_FREE"
    Const BUY_X_OR_MORE_GET_Y_DISCOUNT As String = "BUY_X_OR_MORE_GET_Y_DISCOUNT"
    Const BUY_X_OR_MORE_GET_DISCOUNT_FOR_Y As String = "BUY_X_OR_MORE_GET_DISCOUNT_FOR_Y"
    Const GROUP_DEAL As String = "GROUP_DEAL"
    Const SPEND_X_GET_Y_FREE As String = "SPEND_X_GET_Y_FREE"
    Const SPEND_X_GET_Y_DISCOUNT As String = "SPEND_X_GET_Y_DISCOUNT"
    Const SPEND_X_OR_MORE_GET_SUPPLIED_DISCOUNT As String = "SPEND_X_OR_MORE_GET_SUPPLIED_DISCOUNT"
    Const SPEND_X_OR_MORE_GET_Y_FREE As String = "SPEND_X_OR_MORE_GET_Y_FREE"
    Const SPEND_X_OR_MORE_GET_Y_DISCOUNT As String = "SPEND_X_OR_MORE_GET_Y_DISCOUNT"
    Const SPEND_X_OR_MORE_GET_DISCOUNT_FOR_Y As String = "SPEND_X_OR_MORE_GET_DISCOUNT_FOR_Y"

#End Region

#Region "Class level fields"

    Private _basketFunctions As New DBBasketFunctions
    Private _deAlertsCollections As New Generic.List(Of DEAlerts)
    Private _allowAdditionalPromotionWithFreeDel As Boolean = False
    Private _UsePrePromotionValueForFreeDelCal As Boolean = False
    Private _spendTallyInitialValue As Decimal = 0
    Private _spendTallyBalanceForFreeDelCal As Decimal = 0
    Private _currentCodeCount As Integer = 0
    Private _freeDelCodeCount As Integer = 0
    Private _generalCodeCount As Integer = 0

    'Values gets manipulated in AssignFreeDelieveryDefaults() method
    Private _maxNumberOfCodesAllowed As Integer = 0
    Private _maxGeneralCodesAllowed As Integer = 1
    Private _maxFreeDelCodesAllowed As Integer = 0
    Private _dep As DEPromotions
    Private _promoResults As DataTable
    Private _productTally As DataTable
    Private _spendTally As Decimal
    Private _dtTicketingPromoDetails As DataTable

#End Region

#Region "Public Properties"

    Public Shared auto As String = "AUTO"
    Public Shared code As String = "CODE"
    Public Property Dep() As DEPromotions
        Get
            Return _dep
        End Get
        Set(ByVal value As DEPromotions)
            _dep = value
        End Set
    End Property
    Public Property PromotionResults() As DataTable
        Get
            Return _promoResults
        End Get
        Set(ByVal value As DataTable)
            _promoResults = value
        End Set
    End Property
    Public Property PromoProductsTallyTable() As DataTable
        Get
            Return _productTally
        End Get
        Set(ByVal value As DataTable)
            _productTally = value
        End Set
    End Property
    Public Property SpendTally() As Decimal
        Get
            Return _spendTally
        End Get
        Set(ByVal value As Decimal)
            _spendTally = value
        End Set
    End Property
    Public Property dtTicketingPromoDetails() As DataTable
        Get
            Return _dtTicketingPromoDetails
        End Get
        Set(ByVal value As DataTable)
            _dtTicketingPromoDetails = value
        End Set
    End Property
   
#End Region

#Region "Protected Functions"

    Protected Overrides Function AccessDataBaseSystem21() As ErrorObj
        Dim err As New ErrorObj
        Return err
    End Function

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj

        'Set up the basket functions object
        _basketFunctions.conTALENTTKT = conTALENTTKT
        _basketFunctions.StoredProcedureGroup = Settings.StoredProcedureGroup
        _basketFunctions.OriginatingSource = Settings.OriginatingSource
        _basketFunctions.Source = Settings.OriginatingSourceCode
        _basketFunctions.CustomerNo = Settings.LoginId
        _basketFunctions.SessionId = Dep.BasketHeaderID.ToString
        If Settings.AgentEntity IsNot Nothing Then
            _basketFunctions.BulkSalesMode = Settings.AgentEntity.BulkSalesMode
        End If

        Select Case _settings.ModuleName
            Case Is = ProcessPromotions : err = AccessDatabaseWS613R()
            Case Is = PromotionDetails : err = ReadPromotionDetailsTALENTTKT()
            Case Is = RetrievePromotionHistory : err = DBRetrievePromotionHistory()
            Case Is = RetrievePromotionHistoryDetail : err = DBRetrievePromotionHistoryDetail()
            Case Is = GetPartnerPromotions : err = AccessDatabasePM001S()
        End Select

        Return err
    End Function

    Protected Overrides Function AccessDataBaseSql2005() As ErrorObj
        Dim err As New ErrorObj
        GetUsersAttributesList()
        NewPromotionResultsTable()
        NewPromoProductsTallyTable()
        AssignFreeDelieveryDefaults()
        SpendTally = Dep.BasketTotal
        _spendTallyInitialValue = SpendTally
        _spendTallyBalanceForFreeDelCal = SpendTally
        err = ProcessPromotions_Sql2005()
        Me.ResultDataSet = New DataSet
        Me.ResultDataSet.Tables.Add(PromotionResults)
        If Not String.IsNullOrEmpty(Dep.TempOrderID) Then
            If Not PromotionResults Is Nothing AndAlso PromotionResults.Rows.Count > 0 Then
                StorePromotionResults()
            End If
        End If
        Return err
    End Function

    Protected Function ProcessPromotions_Sql2005() As ErrorObj
        Dim err As New ErrorObj

        'DeleteFreeItemsFromBasket()

        Select Case UCase(Dep.ExecutionPrecedence)
            Case Is = auto
                err = ProcessAutoPromo_Sql2005()
                If Not err.HasError Then
                    err = ProcessCodePromo_Sql2005()
                End If
            Case Is = code
                err = ProcessCodePromo_Sql2005()
                If Not err.HasError Then
                    err = ProcessAutoPromo_Sql2005()
                End If
        End Select

        ModifyFreeItemsInBasket()
        Return err
    End Function

    Protected Function ProcessAutoPromo_Sql2005() As ErrorObj
        Dim err As New ErrorObj

        Dim promos, users, promoLang As DataTable
        Dim maxRedeemExceeded As Boolean = False

        promos = GetPromotions_Sql2005(auto)

        If promos.Rows.Count > 0 Then
            For Each promo As DataRow In promos.Rows
                maxRedeemExceeded = False

                promoLang = GetPromotionsLanguage_Sql2005(promo("PROMOTION_CODE"))

                Dim userAttributesNotMet As Boolean = False
                If IsAutoCodeAllowedToProcess(promo("PROMOTION_CODE"), promo) Then
                    If Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(promo("REQUIRED_USER_ATTRIBUTE"))) Then
                        If Not Me.Dep.UsersAttributeList = Nothing Then
                            Dim userAttributes() As String = Me.Dep.UsersAttributeList.Split(",")
                            If userAttributes.Length > 0 Then
                                Dim found As Boolean = False
                                For i As Integer = 0 To userAttributes.Length - 1
                                    If userAttributes(i).Trim.ToUpper = CStr(promo("REQUIRED_USER_ATTRIBUTE")).Trim.ToUpper Then
                                        found = True
                                        Exit For
                                    End If
                                Next
                                If Not found Then
                                    userAttributesNotMet = True
                                End If
                            Else
                                userAttributesNotMet = True
                            End If
                        Else
                            userAttributesNotMet = True
                        End If
                    End If
                    If Not userAttributesNotMet Then

                        users = GetPromotionsUsers_Sql2005(Dep.Username, _
                                                            Utilities.CheckForDBNull_DateTime(promo("START_DATE")), _
                                                            Utilities.CheckForDBNull_DateTime(promo("END_DATE")), _
                                                            Utilities.CheckForDBNull_String(promo("PROMOTION_CODE")))
                        If users.Rows.Count > 0 Then
                            If Utilities.CheckForDBNull_Int(promo("USER_REDEEM_MAX")) > 0 _
                                    AndAlso Utilities.CheckForDBNull_Int(users.Rows(0)("USAGE_COUNT")) >= Utilities.CheckForDBNull_Int(promo("USER_REDEEM_MAX")) Then
                                maxRedeemExceeded = True
                            End If
                        End If

                        If (Not maxRedeemExceeded) AndAlso Utilities.CheckForDBNull_Int(promo("REDEEM_MAX")) <= Utilities.CheckForDBNull_Int(promo("REDEEM_COUNT")) Then
                            maxRedeemExceeded = True
                        End If

                        If Not maxRedeemExceeded Then
                            ProcessPromotion_Sql2005(promo)
                        Else
                            ResetCodeCount(Utilities.CheckForDBNull_String(UCase(promo("PROMOTION_TYPE"))), Utilities.CheckForDBNull_String(promo("ACTIVATION_MECHANISM")))
                            'Add the error to the PromotionResults data table
                            Dim promoResult As DataRow = PromotionResults.NewRow
                            promoResult("PromotionCode") = Utilities.CheckForDBNull_String(promo("PROMOTION_CODE"))
                            promoResult("BusinessUnit") = Utilities.CheckForDBNull_String(promo("BUSINESS_UNIT"))
                            promoResult("Partner") = Utilities.CheckForDBNull_String(promo("PARTNER"))
                            promoResult("Success") = False
                            promoResult("PromotionDisplayName") = Utilities.CheckForDBNull_String(promoLang.Rows(0)("DISPLAY_NAME"))
                            promoResult("PromotionValue") = 0
                            promoResult("ErrorMessage") = Utilities.CheckForDBNull_String(promoLang.Rows(0)("USER_REDEEM_MAX_EXCEEDED_ERROR"))
                            promoResult("ActivationMechanism") = Utilities.CheckForDBNull_String(promo("ACTIVATION_MECHANISM"))
                            promoResult("ActiveFrom") = Utilities.CheckForDBNull_DateTime(promo("START_DATE"))
                            promoResult("ActiveTo") = Utilities.CheckForDBNull_DateTime(promo("END_DATE"))
                            promoResult("ApplicationCount") = 0
                            promoResult("PromotionPercentageValue") = 0
                            PromotionResults.Rows.Add(promoResult)
                        End If

                    Else
                        ResetCodeCount(Utilities.CheckForDBNull_String(UCase(promo("PROMOTION_TYPE"))), Utilities.CheckForDBNull_String(promo("ACTIVATION_MECHANISM")))
                        'Add the error to the PromotionResults data table
                        Dim promoResult As DataRow = PromotionResults.NewRow
                        promoResult("PromotionCode") = Utilities.CheckForDBNull_String(promo("PROMOTION_CODE"))
                        promoResult("BusinessUnit") = Utilities.CheckForDBNull_String(promo("BUSINESS_UNIT"))
                        promoResult("Partner") = Utilities.CheckForDBNull_String(promo("PARTNER"))
                        promoResult("Success") = False
                        promoResult("PromotionDisplayName") = Utilities.CheckForDBNull_String(promoLang.Rows(0)("DISPLAY_NAME"))
                        promoResult("PromotionValue") = 0
                        promoResult("ErrorMessage") = Utilities.CheckForDBNull_String(promoLang.Rows(0)("RULES_NOT_MET_ERROR"))
                        promoResult("ActivationMechanism") = Utilities.CheckForDBNull_String(promo("ACTIVATION_MECHANISM"))
                        promoResult("ActiveFrom") = Utilities.CheckForDBNull_DateTime(promo("START_DATE"))
                        promoResult("ActiveTo") = Utilities.CheckForDBNull_DateTime(promo("END_DATE"))
                        promoResult("ApplicationCount") = 0
                        promoResult("PromotionPercentageValue") = 0
                        PromotionResults.Rows.Add(promoResult)

                    End If
                Else
                    'Add the error to the PromotionResults data table
                    Dim promoResult As DataRow = PromotionResults.NewRow
                    promoResult("PromotionCode") = Utilities.CheckForDBNull_String(promo("PROMOTION_CODE"))
                    promoResult("BusinessUnit") = Utilities.CheckForDBNull_String(promo("BUSINESS_UNIT"))
                    promoResult("Partner") = Utilities.CheckForDBNull_String(promo("PARTNER"))
                    promoResult("Success") = False
                    promoResult("PromotionDisplayName") = Utilities.CheckForDBNull_String(promoLang.Rows(0)("DISPLAY_NAME"))
                    promoResult("PromotionValue") = 0
                    promoResult("ErrorMessage") = Utilities.CheckForDBNull_String(promoLang.Rows(0)("RULES_NOT_MET_ERROR"))
                    promoResult("ActivationMechanism") = Utilities.CheckForDBNull_String(promo("ACTIVATION_MECHANISM"))
                    promoResult("ActiveFrom") = Utilities.CheckForDBNull_DateTime(promo("START_DATE"))
                    promoResult("ActiveTo") = Utilities.CheckForDBNull_DateTime(promo("END_DATE"))
                    promoResult("ApplicationCount") = 0
                    promoResult("PromotionPercentageValue") = 0
                    PromotionResults.Rows.Add(promoResult)
                End If
            Next
        Else
            'No automatic promotions exist
        End If
        Return err
    End Function

    Protected Function ProcessCodePromo_Sql2005() As ErrorObj
        Dim err As New ErrorObj

        Dim promos, users, promoLang As DataTable
        Dim maxRedeemExceeded As Boolean = False

        Dim dvUserEntPromoCodes As DataView = GetUserEnteredPromoCodesByPriority()
        If dvUserEntPromoCodes.Count > 0 Then

            For Each userPromoCode As DataRowView In dvUserEntPromoCodes
                If Not String.IsNullOrEmpty(userPromoCode("PROMOTION_CODE")) Then
                    promos = GetPromotions_Sql2005(code, userPromoCode("PROMOTION_CODE"))
                    If promos.Rows.Count > 0 Then
                        For Each promo As DataRow In promos.Rows
                            maxRedeemExceeded = False
                            promoLang = GetPromotionsLanguage_Sql2005(promo("PROMOTION_CODE"))
                            If IsCodeAllowedToProcess(userPromoCode("PROMOTION_CODE").ToString(), promo) Then

                                Dim userAttributesNotMet As Boolean = False

                                If Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(promo("REQUIRED_USER_ATTRIBUTE"))) Then
                                    Dim userAttributes() As String = Me.Dep.UsersAttributeList.Split(",")
                                    If userAttributes.Length > 0 Then
                                        Dim found As Boolean = False
                                        For i As Integer = 0 To userAttributes.Length - 1
                                            If userAttributes(i).Trim.ToUpper = CStr(promo("REQUIRED_USER_ATTRIBUTE")).Trim.ToUpper Then
                                                found = True
                                                Exit For
                                            End If
                                        Next
                                        If Not found Then
                                            userAttributesNotMet = True
                                        End If
                                    Else
                                        userAttributesNotMet = True
                                    End If
                                End If

                                If Not userAttributesNotMet Then
                                    users = GetPromotionsUsers_Sql2005(Dep.Username, _
                                                                        Utilities.CheckForDBNull_DateTime(promo("START_DATE")), _
                                                                        Utilities.CheckForDBNull_DateTime(promo("END_DATE")), _
                                                                        Utilities.CheckForDBNull_String(promo("PROMOTION_CODE")))
                                    If users.Rows.Count > 0 Then
                                        If Utilities.CheckForDBNull_Int(users.Rows(0)("USAGE_COUNT")) >= Utilities.CheckForDBNull_Int(promo("USER_REDEEM_MAX")) Then
                                            maxRedeemExceeded = True
                                        End If
                                    End If

                                    If (Not maxRedeemExceeded) AndAlso Utilities.CheckForDBNull_Int(promo("REDEEM_MAX")) <= Utilities.CheckForDBNull_Int(promo("REDEEM_COUNT")) Then
                                        maxRedeemExceeded = True
                                    End If

                                    If Not maxRedeemExceeded Then
                                        ProcessPromotion_Sql2005(promo)
                                    Else
                                        'Add the error to the PromotionResults data table
                                        Dim promoResult As DataRow = PromotionResults.NewRow
                                        promoResult("PromotionCode") = Utilities.CheckForDBNull_String(promo("PROMOTION_CODE"))
                                        promoResult("BusinessUnit") = Utilities.CheckForDBNull_String(promo("BUSINESS_UNIT"))
                                        promoResult("Partner") = Utilities.CheckForDBNull_String(promo("PARTNER"))
                                        promoResult("Success") = False
                                        promoResult("PromotionDisplayName") = Utilities.CheckForDBNull_String(promoLang.Rows(0)("DISPLAY_NAME"))
                                        promoResult("PromotionValue") = 0
                                        promoResult("ErrorMessage") = Utilities.CheckForDBNull_String(promoLang.Rows(0)("USER_REDEEM_MAX_EXCEEDED_ERROR"))
                                        promoResult("ActivationMechanism") = Utilities.CheckForDBNull_String(promo("ACTIVATION_MECHANISM"))
                                        promoResult("ActiveFrom") = Utilities.CheckForDBNull_DateTime(promo("START_DATE"))
                                        promoResult("ActiveTo") = Utilities.CheckForDBNull_DateTime(promo("END_DATE"))
                                        promoResult("ApplicationCount") = 0
                                        promoResult("PromotionPercentageValue") = 0
                                        PromotionResults.Rows.Add(promoResult)
                                    End If
                                Else

                                    'Add the error to the PromotionResults data table
                                    Dim promoResult As DataRow = PromotionResults.NewRow
                                    promoResult("PromotionCode") = Utilities.CheckForDBNull_String(promo("PROMOTION_CODE"))
                                    promoResult("BusinessUnit") = Utilities.CheckForDBNull_String(promo("BUSINESS_UNIT"))
                                    promoResult("Partner") = Utilities.CheckForDBNull_String(promo("PARTNER"))
                                    promoResult("Success") = False
                                    promoResult("PromotionDisplayName") = Utilities.CheckForDBNull_String(promoLang.Rows(0)("DISPLAY_NAME"))
                                    promoResult("PromotionValue") = 0
                                    promoResult("ErrorMessage") = Utilities.CheckForDBNull_String(promoLang.Rows(0)("RULES_NOT_MET_ERROR"))
                                    promoResult("ActivationMechanism") = Utilities.CheckForDBNull_String(promo("ACTIVATION_MECHANISM"))
                                    promoResult("ActiveFrom") = Utilities.CheckForDBNull_DateTime(promo("START_DATE"))
                                    promoResult("ActiveTo") = Utilities.CheckForDBNull_DateTime(promo("END_DATE"))
                                    promoResult("ApplicationCount") = 0
                                    promoResult("PromotionPercentageValue") = 0
                                    PromotionResults.Rows.Add(promoResult)
                                End If
                            Else
                                'Add the error to the PromotionResults data table
                                Dim promoResult As DataRow = PromotionResults.NewRow
                                promoResult("PromotionCode") = Utilities.CheckForDBNull_String(promo("PROMOTION_CODE"))
                                promoResult("BusinessUnit") = Utilities.CheckForDBNull_String(promo("BUSINESS_UNIT"))
                                promoResult("Partner") = Utilities.CheckForDBNull_String(promo("PARTNER"))
                                promoResult("Success") = False
                                promoResult("PromotionDisplayName") = Utilities.CheckForDBNull_String(promoLang.Rows(0)("DISPLAY_NAME"))
                                promoResult("PromotionValue") = 0
                                promoResult("ErrorMessage") = Utilities.CheckForDBNull_String(promoLang.Rows(0)("RULES_NOT_MET_ERROR"))
                                promoResult("ActivationMechanism") = Utilities.CheckForDBNull_String(promo("ACTIVATION_MECHANISM"))
                                promoResult("ActiveFrom") = Utilities.CheckForDBNull_DateTime(promo("START_DATE"))
                                promoResult("ActiveTo") = Utilities.CheckForDBNull_DateTime(promo("END_DATE"))
                                promoResult("ApplicationCount") = 0
                                promoResult("PromotionPercentageValue") = 0
                                PromotionResults.Rows.Add(promoResult)
                            End If
                        Next
                    Else
                        'Add the error to the PromotionResults data table
                        Dim promoResult As DataRow = PromotionResults.NewRow
                        promoResult("PromotionCode") = userPromoCode("PROMOTION_CODE")
                        promoResult("BusinessUnit") = Dep.BusinessUnit
                        promoResult("Partner") = Dep.Partner
                        promoResult("Success") = False
                        promoResult("PromotionDisplayName") = ""
                        promoResult("PromotionValue") = 0
                        promoResult("ErrorMessage") = Dep.InvalidCodeError
                        promoResult("ActivationMechanism") = code
                        promoResult("ApplicationCount") = 0
                        promoResult("ActiveFrom") = Now
                        promoResult("ActiveTo") = Now
                        promoResult("PromotionPercentageValue") = 0
                        PromotionResults.Rows.Add(promoResult)
                    End If
                End If
            Next
        End If

        Return err
    End Function

    Protected Function ProcessPromotion_Sql2005(ByVal promo As DataRow) As ErrorObj
        Dim err As New ErrorObj
        Select Case UCase(Utilities.CheckForDBNull_String(promo("PROMOTION_TYPE")))
            Case Is = BUY_X_GET_Y_FREE
                err = BuyXGetYFree_Sql2005(promo)
            Case Is = BUY_X_GET_Y_DISCOUNT
                err = BuyXGetYDiscount_Sql2005(promo)
            Case Is = BUY_X_OR_MORE_GET_SUPPLIED_DISCOUNT
                err = BuyXGetOrMoreSuppliedDiscount_Sql2005(promo)
            Case Is = BUY_X_OR_MORE_GET_Y_FREE
                err = BuyXOrMoreGetYFree_Sql2005(promo)
            Case Is = BUY_X_OR_MORE_GET_Y_DISCOUNT
                err = BuyXOrMoreGetYDiscount_Sql2005(promo)
            Case Is = BUY_X_OR_MORE_GET_DISCOUNT_FOR_Y
                err = BuyXOrMoreGetDiscountForY_Sql2005(promo)
            Case Is = GROUP_DEAL
                err = GroupDeal_Sql2005(promo)
            Case Is = SPEND_X_GET_Y_FREE
                err = SpendXGetYFree_Sql2005(promo)
            Case Is = SPEND_X_GET_Y_DISCOUNT
                err = SpendXGetYDiscount_Sql2005(promo)
            Case Is = SPEND_X_OR_MORE_GET_SUPPLIED_DISCOUNT
                err = SpendXOrMoreGetSuppliedDiscount_Sql2005(promo)
            Case Is = SPEND_X_OR_MORE_GET_Y_FREE
                err = SpendXOrMoreGetYFree_Sql2005(promo)
            Case Is = SPEND_X_OR_MORE_GET_Y_DISCOUNT
                err = SpendXOrMoreGetYDiscount_Sql2005(promo)
            Case Is = SPEND_X_OR_MORE_GET_DISCOUNT_FOR_Y
                err = SpendXOrMoreGetDiscountForY_Sql2005(promo)
        End Select
        Return err
    End Function

    Protected Function BuyXGetYFree_Sql2005(ByVal promo As DataRow) As ErrorObj
        Dim err As New ErrorObj
        err = Do_BuyX_Check_Sql2005(promo, False, GetYFree)
        Return err
    End Function

    Protected Function BuyXGetYDiscount_Sql2005(ByVal promo As DataRow) As ErrorObj
        Dim err As New ErrorObj
        err = Do_BuyX_Check_Sql2005(promo, False, GetYDiscount)
        Return err
    End Function

    Protected Function BuyXGetOrMoreSuppliedDiscount_Sql2005(ByVal promo As DataRow) As ErrorObj
        Dim err As New ErrorObj
        err = Do_BuyX_Check_Sql2005(promo, True, GetSuppliedDiscount)
        Return err
    End Function

    Protected Function BuyXOrMoreGetYFree_Sql2005(ByVal promo As DataRow) As ErrorObj
        Dim err As New ErrorObj
        err = Do_BuyX_Check_Sql2005(promo, True, GetYFree)
        Return err
    End Function

    Protected Function BuyXOrMoreGetYDiscount_Sql2005(ByVal promo As DataRow) As ErrorObj
        Dim err As New ErrorObj
        err = Do_BuyX_Check_Sql2005(promo, True, GetYDiscount)
        Return err
    End Function

    Protected Function BuyXGetDiscountForY_Sql2005(ByVal promo As DataRow) As ErrorObj
        Dim err As New ErrorObj
        err = Do_BuyX_Check_Sql2005(promo, False, GetDiscountForY)
        Return err
    End Function

    Protected Function BuyXOrMoreGetDiscountForY_Sql2005(ByVal promo As DataRow) As ErrorObj
        Dim err As New ErrorObj
        err = Do_BuyX_Check_Sql2005(promo, True, GetDiscountForY)
        Return err
    End Function

    Protected Function GroupDeal_Sql2005(ByVal promo As DataRow) As ErrorObj
        Dim err As New ErrorObj
        err = Do_BuyX_Check_Sql2005(promo, False, GetGroupDeal)
        Return err
    End Function

    Protected Function SpendXGetYFree_Sql2005(ByVal promo As DataRow) As ErrorObj
        Dim err As New ErrorObj
        err = Do_SpendX_Check_Sql2005(promo, False, GetYFree)
        Return err
    End Function

    Protected Function SpendXGetYDiscount_Sql2005(ByVal promo As DataRow) As ErrorObj
        Dim err As New ErrorObj
        err = Do_SpendX_Check_Sql2005(promo, False, GetYDiscount)
        Return err
    End Function

    Protected Function SpendXGetDiscountForY_Sql2005(ByVal promo As DataRow) As ErrorObj
        Dim err As New ErrorObj
        err = Do_SpendX_Check_Sql2005(promo, False, GetDiscountForY)
        Return err
    End Function

    Protected Function SpendXOrMoreGetSuppliedDiscount_Sql2005(ByVal promo As DataRow) As ErrorObj
        Dim err As New ErrorObj
        err = Do_SpendX_Check_Sql2005(promo, True, GetSuppliedDiscount)
        Return err
    End Function

    Protected Function SpendXOrMoreGetYFree_Sql2005(ByVal promo As DataRow) As ErrorObj
        Dim err As New ErrorObj
        err = Do_SpendX_Check_Sql2005(promo, True, GetYFree)
        Return err
    End Function

    Protected Function SpendXOrMoreGetYDiscount_Sql2005(ByVal promo As DataRow) As ErrorObj
        Dim err As New ErrorObj
        err = Do_SpendX_Check_Sql2005(promo, True, GetYDiscount)
        Return err
    End Function

    Protected Function SpendXOrMoreGetDiscountForY_Sql2005(ByVal promo As DataRow) As ErrorObj
        Dim err As New ErrorObj
        err = Do_SpendX_Check_Sql2005(promo, True, GetDiscountForY)
        Return err
    End Function

    Protected Function Do_BuyX_Check_Sql2005(ByVal promo As DataRow, _
                                            ByVal IsOrMorePromo As Boolean, _
                                            ByVal promoActionType As String) As ErrorObj
        Dim err As New ErrorObj

        Dim promoResult As DataRow = PromotionResults.NewRow
        Dim requiredProducts As DataTable = GetRequiredProducts_Sql2005(Utilities.CheckForDBNull_String(promo("PROMOTION_CODE")))
        Dim promoLang As DataTable = GetPromotionsLanguage_Sql2005(Utilities.CheckForDBNull_String(promo("PROMOTION_CODE")))
        Dim minItems As Integer = Utilities.CheckForDBNull_Int(promo("MIN_ITEMS"))
        Dim promoIsValid As Boolean = True
        Dim alreadyApplied As Boolean = False
        Dim matchingProducts As New Collections.Generic.Dictionary(Of String, Decimal)

        Dim count As Integer = 0
        If requiredProducts.Rows.Count > 0 Then
            If promoLang.Rows.Count > 0 Then

                While promoIsValid 'Loop through until the current promotion
                    '               can no longer be applied

                    'Reset the variables for each itteration
                    promoIsValid = False
                    matchingProducts = New Collections.Generic.Dictionary(Of String, Decimal)
                    count = 0

                    'Loop through the products in the tally table
                    For Each basketItem As DataRow In PromoProductsTallyTable.Rows

                        'Loop through the required products
                        For Each requiredProduct As DataRow In requiredProducts.Rows

                            'Check to see if the product codes match
                            If UCase(Utilities.CheckForDBNull_String(requiredProduct("PRODUCT_CODE"))) = UCase(basketItem("ProductCode")) Then

                                Dim remainingQuantity As Decimal = 0
                                If promoActionType = GetSuppliedDiscount Then
                                    If _allowAdditionalPromotionWithFreeDel Then
                                        If _UsePrePromotionValueForFreeDelCal Then
                                            remainingQuantity = CDec(basketItem("InitialQuantity"))
                                        Else
                                            remainingQuantity = CDec(basketItem("RemainingQuantity"))
                                        End If
                                    Else
                                        remainingQuantity = CDec(basketItem("RemainingQuantity"))
                                    End If
                                Else
                                    remainingQuantity = CDec(basketItem("RemainingQuantity"))
                                End If
                                'Check to see if the tally table contains sufficient remaining quantity
                                If Utilities.CheckForDBNull_Decimal(requiredProduct("QUANTITY")) <= remainingQuantity Then
                                    'if promoActionType is GetDiscountForY then check product y from tbl_promotions_discount 
                                    'is exists in basket otherwise override the promoIsValid to false
                                    Dim tempPromoValid As Boolean = True
                                    If promoActionType = GetDiscountForY Then
                                        If IsProductYInBasket(Utilities.CheckForDBNull_String(promo("PROMOTION_CODE"))) Then
                                            tempPromoValid = True
                                        Else
                                            tempPromoValid = False
                                        End If
                                    End If
                                    If tempPromoValid Then
                                        Dim available As Decimal = remainingQuantity
                                        Dim reqPerEntry As Decimal = Utilities.CheckForDBNull_Decimal(requiredProduct("QUANTITY"))
                                        Dim totalToUse As Integer = Math.Floor(available / reqPerEntry)

                                        'count += 1
                                        Dim remainingRequired As Integer = minItems - count

                                        If totalToUse > remainingRequired Then
                                            count += remainingRequired
                                            matchingProducts.Add(UCase(basketItem("ProductCode")), reqPerEntry * remainingRequired)
                                        Else
                                            count += totalToUse
                                            matchingProducts.Add(UCase(basketItem("ProductCode")), reqPerEntry * totalToUse)
                                        End If

                                        'Add the item to the matched products collection, along with the
                                        'quantity used for this match
                                        If count >= minItems Then
                                            promoIsValid = True
                                        End If
                                    End If
                                End If
                                'If we have a match, exit the loop
                                If promoIsValid Then Exit For
                            Else
                                ' Check if it has a master and see if this matches...
                                If UCase(Utilities.CheckForDBNull_String(requiredProduct("PRODUCT_CODE"))) = UCase(basketItem("MasterProductCode")) Then
                                    Dim remainingQuantity As Decimal = 0
                                    If promoActionType = GetSuppliedDiscount Then
                                        If _allowAdditionalPromotionWithFreeDel Then
                                            If _UsePrePromotionValueForFreeDelCal Then
                                                remainingQuantity = CDec(basketItem("InitialQuantity"))
                                            Else
                                                remainingQuantity = CDec(basketItem("RemainingQuantity"))
                                            End If
                                        Else
                                            remainingQuantity = CDec(basketItem("RemainingQuantity"))
                                        End If
                                    Else
                                        remainingQuantity = CDec(basketItem("RemainingQuantity"))
                                    End If
                                    'if promoActionType is GetDiscountForY then check product y from tbl_promotions_discount 
                                    'is exists in basket otherwise override the promoIsValid to false
                                    Dim tempPromoValid As Boolean = True
                                    If promoActionType = GetDiscountForY Then
                                        If IsProductYInBasket(Utilities.CheckForDBNull_String(promo("PROMOTION_CODE"))) Then
                                            tempPromoValid = True
                                        Else
                                            tempPromoValid = False
                                        End If
                                    End If
                                    If tempPromoValid Then
                                        Dim available As Decimal = remainingQuantity
                                        Dim reqPerEntry As Decimal = Utilities.CheckForDBNull_Decimal(requiredProduct("QUANTITY"))
                                        Dim totalToUse As Integer = Math.Floor(available / reqPerEntry)

                                        'count += 1
                                        Dim remainingRequired As Integer = minItems - count

                                        If totalToUse > remainingRequired Then
                                            count += remainingRequired
                                            matchingProducts.Add(UCase(basketItem("ProductCode")), reqPerEntry * remainingRequired)
                                        Else
                                            count += totalToUse
                                            matchingProducts.Add(UCase(basketItem("ProductCode")), reqPerEntry * totalToUse)
                                        End If

                                        'Add the item to the matched products collection, along with the
                                        'quantity used for this match
                                        If count >= minItems Then
                                            promoIsValid = True
                                        End If
                                    End If
                                End If
                                'If we have a match, exit the loop
                                If promoIsValid Then Exit For
                            End If
                        Next
                        'If we have a match, exit the loop
                        If promoIsValid Then Exit For
                    Next

                    If promoIsValid Then
                        Dim matchingIndex As Integer = 0
                        If (promoActionType = GetSuppliedDiscount) AndAlso _allowAdditionalPromotionWithFreeDel Then
                            promoIsValid = False
                        Else
                            While matchingIndex < matchingProducts.Count
                                Dim productCode As String = ""
                                Dim pcIndex As Integer = 0
                                For Each pc As String In matchingProducts.Keys
                                    If pcIndex = matchingIndex Then
                                        productCode = pc
                                        Exit For
                                    End If
                                    pcIndex += 1
                                Next
                                Dim productsTallyIndex As Integer = 0
                                While productsTallyIndex < PromoProductsTallyTable.Rows.Count
                                    If UCase(PromoProductsTallyTable.Rows(productsTallyIndex)("ProductCode")) = UCase(productCode) Then
                                        If IsOrMorePromo Then
                                            'The remaining products cannot be used for any other promotions
                                            'so update the tallies to ensure this does not happen.
                                            matchingProducts(UCase(productCode)) = PromoProductsTallyTable.Rows(productsTallyIndex)("RemainingQuantity")
                                            PromoProductsTallyTable.Rows(productsTallyIndex)("RemainingQuantity") = 0
                                            PromoProductsTallyTable.Rows(productsTallyIndex)("UsedQuantity") = PromoProductsTallyTable.Rows(productsTallyIndex)("InitialQuantity")
                                        Else
                                            PromoProductsTallyTable.Rows(productsTallyIndex)("RemainingQuantity") -= matchingProducts(UCase(productCode))
                                            PromoProductsTallyTable.Rows(productsTallyIndex)("UsedQuantity") += matchingProducts(UCase(productCode))
                                        End If
                                    End If
                                    productsTallyIndex += 1
                                End While
                                matchingIndex += 1
                            End While
                        End If
                        '------------------------------------------------------------

                        'Select the action to take based on the promotion 
                        'action type passed to the function
                        Select Case promoActionType
                            Case Is = GetYFree
                                err = Do_GetYFree_Action_Sql2005(promo, alreadyApplied, promoLang, matchingProducts)
                            Case Is = GetYDiscount
                                err = Do_GetYDiscount_Action_Sql2005(promo, alreadyApplied, promoLang, IsOrMorePromo, matchingProducts)
                            Case Is = GetDiscountForY
                                err = Do_GetDiscountForY_Action_Sql2005(promo, alreadyApplied, promoLang, IsOrMorePromo, matchingProducts)
                            Case Is = GetSuppliedDiscount
                                err = Do_GetSuppliedDiscount_Action_Sql2005(promo, alreadyApplied, promoLang)
                            Case Is = GetGroupDeal
                                err = Do_GroupDeal_Action_Sql2005(promo, alreadyApplied, promoLang, matchingProducts)
                        End Select

                        If Not err.HasError Then
                            'If the promotions was successfully added, set the flag
                            'to indicate it has been applied on a previous itteration
                            'so that a new results record is not added to the results table
                            alreadyApplied = True
                        Else
                            'Handle error adding free items
                        End If

                    Else 'Promotion not applicable

                        'Check to see if the promo has been applied on a previous itteration 
                        'and only notify of failure if it has NOT.
                        If Not alreadyApplied Then
                            ResetCodeCount(Utilities.CheckForDBNull_String(UCase(promo("PROMOTION_TYPE"))), Utilities.CheckForDBNull_String(promo("ACTIVATION_MECHANISM")))
                            promoResult("PromotionCode") = Utilities.CheckForDBNull_String(promo("PROMOTION_CODE"))
                            promoResult("BusinessUnit") = Utilities.CheckForDBNull_String(promo("BUSINESS_UNIT"))
                            promoResult("Partner") = Utilities.CheckForDBNull_String(promo("PARTNER"))
                            promoResult("Success") = False
                            promoResult("PromotionDisplayName") = Utilities.CheckForDBNull_String(promoLang.Rows(0)("DISPLAY_NAME"))
                            promoResult("PromotionValue") = 0
                            promoResult("ErrorMessage") = Utilities.CheckForDBNull_String(promoLang.Rows(0)("RULES_NOT_MET_ERROR"))
                            promoResult("ActivationMechanism") = Utilities.CheckForDBNull_String(promo("ACTIVATION_MECHANISM"))
                            promoResult("ActiveFrom") = Utilities.CheckForDBNull_DateTime(promo("START_DATE"))
                            promoResult("ActiveTo") = Utilities.CheckForDBNull_DateTime(promo("END_DATE"))
                            promoResult("ApplicationCount") = 0
                            promoResult("ProductCodes") = ""
                            promoResult("FreeDelivery") = False
                            promoResult("PromotionPercentageValue") = 0
                            PromotionResults.Rows.Add(promoResult)
                        End If

                    End If

                End While


            Else
                'Promo Language entry does not exist
            End If
            'Promo not configured correctly
        End If

        Return err
    End Function

    Protected Function Do_SpendX_Check_Sql2005(ByVal promo As DataRow, _
                                                ByVal IsOrMorePromo As Boolean, _
                                                ByVal promoActionType As String) As ErrorObj
        Dim err As New ErrorObj

        Dim promoResult As DataRow = PromotionResults.NewRow
        Dim promoLang As DataTable = GetPromotionsLanguage_Sql2005(Utilities.CheckForDBNull_String(promo("PROMOTION_CODE")))
        Dim minSpend As Decimal = Utilities.CheckForDBNull_Decimal(promo("MIN_SPEND"))
        Dim promoIsValid As Boolean = True
        Dim alreadyApplied As Boolean = False

        While promoIsValid 'Loop through until the current promotion
            '               can no longer be applied

            'Reset the variables for each itteration
            promoIsValid = False

            If SpendTally >= minSpend AndAlso SpendTally <> 0 Then
                promoIsValid = True
            Else
                'Code does not qualify
            End If

            'override promoIsValid for FREEDEL based on defaults
            Dim updateSpendTally As Boolean = True
            If promoActionType = GetSuppliedDiscount Then
                If _allowAdditionalPromotionWithFreeDel Then
                    updateSpendTally = False
                    If _UsePrePromotionValueForFreeDelCal Then
                        If _spendTallyInitialValue >= minSpend AndAlso _spendTallyInitialValue <> 0 Then
                            promoIsValid = True
                        Else
                            promoIsValid = False
                        End If
                    Else
                        If _spendTallyBalanceForFreeDelCal >= minSpend AndAlso _spendTallyBalanceForFreeDelCal <> 0 Then
                            promoIsValid = True
                        Else
                            promoIsValid = False
                        End If
                    End If
                End If
            End If

            'if promoActionType is GetDiscountForY then check product y from tbl_promotions_discount 
            'is exists in basket otherwise override the promoIsValid to false
            If promoIsValid AndAlso promoActionType = GetDiscountForY Then
                If IsProductYInBasket(Utilities.CheckForDBNull_String(promo("PROMOTION_CODE"))) Then
                    promoIsValid = True
                Else
                    promoIsValid = False
                End If
            End If

            If promoIsValid Then

                'Select the action to take based on the promotion 
                'action type passed to the function

                Dim matchingProducts As New Generic.Dictionary(Of String, Decimal)
                Select Case promoActionType
                    Case Is = GetYFree
                        err = Do_GetYFree_Action_Sql2005(promo, alreadyApplied, promoLang, matchingProducts)
                    Case Is = GetYDiscount
                        err = Do_GetYDiscount_Action_Sql2005(promo, alreadyApplied, promoLang, IsOrMorePromo)
                    Case Is = GetDiscountForY
                        err = Do_GetDiscountForY_Action_Sql2005(promo, alreadyApplied, promoLang, IsOrMorePromo)
                    Case Is = GetSuppliedDiscount
                        err = Do_GetSuppliedDiscount_Action_Sql2005(promo, alreadyApplied, promoLang)
                End Select

                If promoActionType = GetSuppliedDiscount Then
                    If _allowAdditionalPromotionWithFreeDel Then
                        promoIsValid = False
                    Else
                        If IsOrMorePromo Then
                            'The remaining products cannot be used for any other promotions
                            'so update the tallies to ensure this does not happen.
                            SpendTally = 0
                        Else
                            SpendTally -= minSpend
                        End If
                    End If
                Else
                    If IsOrMorePromo Then
                        'The remaining products cannot be used for any other promotions
                        'so update the tallies to ensure this does not happen.
                        SpendTally = 0
                    Else
                        SpendTally -= minSpend
                    End If
                End If


                If Not err.HasError Then
                    'If the promotions was successfully added, set the flag
                    'to indicate it has been applied on a previous itteration
                    'so that a new results record is not added to the results table
                    alreadyApplied = True
                Else
                    'Handle error adding free items
                End If

            Else 'Promotion not applicable
                'Check to see if the promo has been applied on a previous itteration 
                'and only notify of failure if it has NOT.
                If Not alreadyApplied Then
                    ResetCodeCount(Utilities.CheckForDBNull_String(UCase(promo("PROMOTION_TYPE"))), Utilities.CheckForDBNull_String(promo("ACTIVATION_MECHANISM")))
                    promoResult("PromotionCode") = Utilities.CheckForDBNull_String(promo("PROMOTION_CODE"))
                    promoResult("BusinessUnit") = Utilities.CheckForDBNull_String(promo("BUSINESS_UNIT"))
                    promoResult("Partner") = Utilities.CheckForDBNull_String(promo("PARTNER"))
                    promoResult("Success") = False
                    promoResult("PromotionDisplayName") = Utilities.CheckForDBNull_String(promoLang.Rows(0)("DISPLAY_NAME"))
                    promoResult("PromotionValue") = 0
                    promoResult("ErrorMessage") = Utilities.CheckForDBNull_String(promoLang.Rows(0)("RULES_NOT_MET_ERROR"))
                    promoResult("ActivationMechanism") = Utilities.CheckForDBNull_String(promo("ACTIVATION_MECHANISM"))
                    promoResult("ActiveFrom") = Utilities.CheckForDBNull_DateTime(promo("START_DATE"))
                    promoResult("ActiveTo") = Utilities.CheckForDBNull_DateTime(promo("END_DATE"))
                    promoResult("ApplicationCount") = 0
                    promoResult("ProductCodes") = ""
                    promoResult("FreeDelivery") = False
                    promoResult("PromotionPercentageValue") = 0
                    PromotionResults.Rows.Add(promoResult)
                End If

            End If

        End While

        Return err
    End Function

    Protected Function Do_GetYFree_Action_Sql2005(ByVal promo As DataRow, _
                                                    ByVal promoHasBeenAppliedBefore As Boolean, _
                                                    ByVal promoLang As DataTable, _
                                                    ByVal matchingProducts As Collections.Generic.Dictionary(Of String, Decimal)) As ErrorObj

        Dim err As New ErrorObj
        Dim freeProducts As DataTable = GetFreeProducts_Sql2005(Utilities.CheckForDBNull_String(UCase(promo("PROMOTION_CODE"))))
        Dim requiredProducts As DataTable = GetRequiredProducts_Sql2005(Utilities.CheckForDBNull_String(promo("PROMOTION_CODE")))

        Dim isBogof As Boolean = False
        If requiredProducts.Rows.Count = 1 AndAlso freeProducts.Rows.Count = 1 AndAlso _
                requiredProducts.Rows(0)("PRODUCT_CODE").ToString = freeProducts.Rows(0)("PRODUCT_CODE").ToString Then
            isBogof = True
        End If

        If freeProducts.Rows.Count > 0 Then
            Try

                Dim deaItem As Talent.Common.DEAlerts
                Dim productCode As String = String.Empty
                Dim masterProduct As String = String.Empty
                For Each prod As Data.DataRow In freeProducts.Rows
                    If isBogof Then
                        For Each key As String In matchingProducts.Keys
                            productCode = UCase(prod("PRODUCT_CODE"))
                            masterProduct = UCase(prod("PRODUCT_CODE"))
                        Next
                    Else
                        productCode = UCase(prod("PRODUCT_CODE"))
                        masterProduct = UCase(prod("PRODUCT_CODE"))
                    End If
                    Dim itemIndex As Integer = _deAlertsCollections.FindIndex(Function(deAlertsItem As DEAlerts) deAlertsItem.ProductCode = productCode)

                    If itemIndex < 0 Then
                        Dim tblGroupProduct As DataTable = GetGroupProductDetails_Sql2005(UCase(prod("PRODUCT_CODE")))
                        deaItem = New Talent.Common.DEAlerts
                        deaItem.ProductCode = productCode
                        deaItem.MasterProduct = masterProduct
                        deaItem.Quantity = prod("QUANTITY")
                        deaItem.Price = 0
                        deaItem.GroupL01Group = Utilities.CheckForDBNull_String(tblGroupProduct.Rows(0)("GROUP_L01_GROUP"))
                        deaItem.GroupL02Group = Utilities.CheckForDBNull_String(tblGroupProduct.Rows(0)("GROUP_L02_GROUP"))
                        deaItem.GroupL03Group = Utilities.CheckForDBNull_String(tblGroupProduct.Rows(0)("GROUP_L03_GROUP"))
                        deaItem.GroupL04Group = Utilities.CheckForDBNull_String(tblGroupProduct.Rows(0)("GROUP_L04_GROUP"))
                        deaItem.GroupL05Group = Utilities.CheckForDBNull_String(tblGroupProduct.Rows(0)("GROUP_L05_GROUP"))
                        deaItem.GroupL06Group = Utilities.CheckForDBNull_String(tblGroupProduct.Rows(0)("GROUP_L06_GROUP"))
                        deaItem.GroupL07Group = Utilities.CheckForDBNull_String(tblGroupProduct.Rows(0)("GROUP_L07_GROUP"))
                        deaItem.GroupL08Group = Utilities.CheckForDBNull_String(tblGroupProduct.Rows(0)("GROUP_L08_GROUP"))
                        deaItem.GroupL09Group = Utilities.CheckForDBNull_String(tblGroupProduct.Rows(0)("GROUP_L09_GROUP"))
                        deaItem.GroupL10Group = Utilities.CheckForDBNull_String(tblGroupProduct.Rows(0)("GROUP_L10_GROUP"))
                        deaItem.AllowSelectOption = Utilities.CheckForDBNull_Boolean_DefaultFalse(prod("ALLOW_SELECT_OPTION"))
                        _deAlertsCollections.Add(deaItem)
                        tblGroupProduct = Nothing
                    Else
                        _deAlertsCollections.Item(itemIndex).Quantity += prod("QUANTITY")
                    End If
                Next

                'The promotion has been successfully applied, but we only
                'add the details to the results table if they have not
                'already been added
                Dim currentResultsRow As DataRow
                If promoHasBeenAppliedBefore Then
                    currentResultsRow = PromotionResults.Select("PromotionCode = '" & promo("PROMOTION_CODE") & "'")(0)
                    currentResultsRow("ApplicationCount") += 1
                    If Not matchingProducts Is Nothing Then
                        currentResultsRow("ProductCodes") = ProcessMatchedProductCodes(matchingProducts, currentResultsRow("ProductCodes"))
                    End If
                Else
                    currentResultsRow = PromotionResults.NewRow
                    currentResultsRow("PromotionCode") = Utilities.CheckForDBNull_String(promo("PROMOTION_CODE"))
                    currentResultsRow("BusinessUnit") = Utilities.CheckForDBNull_String(promo("BUSINESS_UNIT"))
                    currentResultsRow("Partner") = Utilities.CheckForDBNull_String(promo("PARTNER"))
                    currentResultsRow("Success") = True
                    currentResultsRow("PromotionDisplayName") = Utilities.CheckForDBNull_String(promoLang.Rows(0)("DISPLAY_NAME"))
                    currentResultsRow("PromotionValue") = 0
                    currentResultsRow("ErrorMessage") = String.Empty
                    currentResultsRow("ActivationMechanism") = Utilities.CheckForDBNull_String(promo("ACTIVATION_MECHANISM"))
                    currentResultsRow("ApplicationCount") = 1
                    currentResultsRow("ActiveFrom") = Utilities.CheckForDBNull_DateTime(promo("START_DATE"))
                    currentResultsRow("ActiveTo") = Utilities.CheckForDBNull_DateTime(promo("END_DATE"))
                    currentResultsRow("FreeDelivery") = False
                    currentResultsRow("ProductCodes") = ""
                    If Not matchingProducts Is Nothing Then
                        currentResultsRow("ProductCodes") = ProcessMatchedProductCodes(matchingProducts, currentResultsRow("ProductCodes"))
                    End If
                    currentResultsRow("PromotionPercentageValue") = 0
                    PromotionResults.Rows.Add(currentResultsRow)
                End If

            Catch ex As Exception
                err.HasError = True
                err.ErrorNumber = "Add Free Items Error"
                err.ErrorMessage = ex.Message
            End Try
        End If

        Return err
    End Function

    Protected Function Do_GetYDiscount_Action_Sql2005(ByVal promo As DataRow, _
                                                        ByVal promoHasBeenAppliedBefore As Boolean, _
                                                        ByVal promoLang As DataTable, _
                                                        ByVal isOrMorePromo As Boolean, _
                                                        Optional ByVal matchingProducts As Generic.Dictionary(Of String, Decimal) = Nothing) As ErrorObj
        Dim err As New ErrorObj

        'Get the details of the discount
        Dim discounts As DataTable = GetPromotionsDiscounts_Sql2005(Utilities.CheckForDBNull_String(promo("PROMOTION_CODE")))

        If discounts.Rows.Count > 0 Then
            Dim discount As DataRow = discounts.Rows(0)
            Dim isPercentage As Boolean = Utilities.CheckForDBNull_Boolean_DefaultFalse(discount("IS_PERCENTAGE"))
            Dim value As Decimal = Utilities.CheckForDBNull_Decimal(discount("VALUE"))

            Dim discountValue As Decimal = 0

            'If the discount is a percentage, get the total for the items included in the discount
            'and calculate the discount as a percentage of their total value
            If isPercentage Then
                Dim products As New Generic.Dictionary(Of String, WebPriceProduct)

                'If we have not passed a matchingProducts dictionary into the
                'function then we are using a SpendX promotion
                If matchingProducts Is Nothing Then
                    If isOrMorePromo Then
                        discountValue = Utilities.RoundToValue((SpendTally / 100) * value, 0.01)
                    Else
                        Dim minSpend As Decimal = Utilities.CheckForDBNull_Decimal(promo("MIN_SPEND"))
                        discountValue = Utilities.RoundToValue((minSpend / 100) * value, 0.01)
                    End If
                Else
                    'Otherwise we are using a BuyX... promotion

                    'Add each product code to an arraylist
                    For Each productCode As String In matchingProducts.Keys
                        For Each rw As DataRow In Me.PromoProductsTallyTable.Rows
                            If UCase(rw("ProductCode")) = productCode Then
                                products.Add(productCode, New WebPriceProduct(productCode, CDec(rw("InitialQuantity")), CStr(rw("MasterProductCode"))))
                            End If
                        Next

                    Next

                    Dim pricingsettings As New Talent.Common.DEWebPriceSetting(Settings.FrontEndConnectionString, _
                                                                                Settings.DestinationDatabase, _
                                                                                "", _
                                                                                Settings.BusinessUnit, _
                                                                                Settings.Partner, _
                                                                                Dep.PriceList, _
                                                                                False, _
                                                                                Talent.Common.Utilities.GetDefaultLanguage, _
                                                                                Dep.PartnerGroup, _
                                                                                Dep.SecondaryPriceList, _
                                                                                Dep.UsersAttributeList)

                    Dim productPricing As New Talent.Common.TalentWebPricing(pricingsettings, products, True)
                    productPricing.GetWebPrices()

                    'Get the total value of the products being used for this promotion
                    Dim machingProductsTotalValue As Decimal = 0
                    If Not err.HasError Then
                        For Each productPrice As DEWebPrice In productPricing.RetrievedPrices.Values
                            machingProductsTotalValue += productPrice.Purchase_Price_Gross * matchingProducts(productPrice.PRODUCT_CODE)
                        Next
                    End If

                    discountValue = (machingProductsTotalValue / 100) * value
                End If


            Else 'Otherwise, use the value from the database as the discount value
                discountValue = value
            End If

            'record the balance spend tally for free delivery cal if allowed additional promotion
            'use pre promotion value for free del cal is false
            _spendTallyBalanceForFreeDelCal = _spendTallyBalanceForFreeDelCal - discountValue

            'The promotion has been successfully applied, but we only
            'add the details to the results table if they have not
            'already been added
            Dim currentResultsRow As DataRow
            If promoHasBeenAppliedBefore Then
                currentResultsRow = PromotionResults.Select("PromotionCode = '" & promo("PROMOTION_CODE") & "'")(0)
                currentResultsRow("ApplicationCount") += 1
                currentResultsRow("PromotionValue") += discountValue
                If Not matchingProducts Is Nothing Then
                    currentResultsRow("ProductCodes") = ProcessMatchedProductCodes(matchingProducts, currentResultsRow("ProductCodes"))
                End If
            Else
                currentResultsRow = PromotionResults.NewRow
                currentResultsRow("PromotionCode") = Utilities.CheckForDBNull_String(promo("PROMOTION_CODE"))
                currentResultsRow("BusinessUnit") = Utilities.CheckForDBNull_String(promo("BUSINESS_UNIT"))
                currentResultsRow("Partner") = Utilities.CheckForDBNull_String(promo("PARTNER"))
                currentResultsRow("Success") = True
                currentResultsRow("PromotionDisplayName") = Utilities.CheckForDBNull_String(promoLang.Rows(0)("DISPLAY_NAME"))
                currentResultsRow("PromotionValue") = discountValue
                currentResultsRow("ErrorMessage") = String.Empty
                currentResultsRow("ActivationMechanism") = Utilities.CheckForDBNull_String(promo("ACTIVATION_MECHANISM"))
                currentResultsRow("ApplicationCount") = 1
                currentResultsRow("ActiveFrom") = Utilities.CheckForDBNull_DateTime(promo("START_DATE"))
                currentResultsRow("ActiveTo") = Utilities.CheckForDBNull_DateTime(promo("END_DATE"))
                currentResultsRow("FreeDelivery") = False
                currentResultsRow("ProductCodes") = ""
                If Not matchingProducts Is Nothing Then
                    currentResultsRow("ProductCodes") = ProcessMatchedProductCodes(matchingProducts, currentResultsRow("ProductCodes"))
                End If
                currentResultsRow("PromotionPercentageValue") = 0
                If isPercentage Then
                    currentResultsRow("PromotionPercentageValue") = value
                End If

                PromotionResults.Rows.Add(currentResultsRow)
            End If
        End If

        Return err
    End Function

    Protected Function Do_GetDiscountForY_Action_Sql2005(ByVal promo As DataRow, _
                                                        ByVal promoHasBeenAppliedBefore As Boolean, _
                                                        ByVal promoLang As DataTable, _
                                                        ByVal isOrMorePromo As Boolean, _
                                                        Optional ByVal matchingProducts As Generic.Dictionary(Of String, Decimal) = Nothing) As ErrorObj
        Dim err As New ErrorObj

        'Get the details of the discount
        Dim discounts As DataTable = GetPromotionsDiscounts_Sql2005(Utilities.CheckForDBNull_String(promo("PROMOTION_CODE")))

        If discounts.Rows.Count > 0 Then
            Dim discount As DataRow = discounts.Rows(0)
            Dim isPercentage As Boolean = Utilities.CheckForDBNull_Boolean_DefaultFalse(discount("IS_PERCENTAGE"))
            Dim value As Decimal = Utilities.CheckForDBNull_Decimal(discount("VALUE"))
            Dim productCode_Y As String = Utilities.CheckForDBNull_String(discount("PRODUCT_CODE")).Trim.ToUpper()

            Dim discountValue As Decimal = 0


            'check the product Y is available in the basket
            Dim totalQuantityOfProductYInBasket As Integer = 0
            Dim totalPriceOfProductYInBasket As Decimal = 0
            Dim priceOfProductYInBasket As Decimal = 0
            Dim masterProductOfProductYInBasket As String = String.Empty
            For Each key As String In Dep.BasketItems.Keys
                If ((productCode_Y = Dep.BasketItems(key).ProductCode) OrElse (productCode_Y = Dep.BasketItems(key).MasterProductCode)) Then
                    totalQuantityOfProductYInBasket += Dep.BasketItems(key).Quantity
                    masterProductOfProductYInBasket = Dep.BasketItems(key).MasterProductCode
                End If
            Next

            'Process promotion only when the product Y found in basket
            If totalQuantityOfProductYInBasket > 0 Then

                'Get the price of product Y in basket
                Dim YProductsInBasket As New Generic.Dictionary(Of String, WebPriceProduct)
                YProductsInBasket.Add(productCode_Y, New WebPriceProduct(productCode_Y, totalQuantityOfProductYInBasket, masterProductOfProductYInBasket))
                Dim YPricingSettings As New Talent.Common.DEWebPriceSetting(Settings.FrontEndConnectionString, _
                                                                                    Settings.DestinationDatabase, _
                                                                                    "", _
                                                                                    Settings.BusinessUnit, _
                                                                                    Settings.Partner, _
                                                                                    Dep.PriceList, _
                                                                                    False, _
                                                                                    Talent.Common.Utilities.GetDefaultLanguage, _
                                                                                    Dep.PartnerGroup, _
                                                                                    Dep.SecondaryPriceList, _
                                                                                    Dep.UsersAttributeList)

                Dim YProductPricing As New Talent.Common.TalentWebPricing(YPricingSettings, YProductsInBasket, True)
                err = YProductPricing.GetWebPrices()
                If Not err.HasError Then
                    For Each productPrice As DEWebPrice In YProductPricing.RetrievedPrices.Values
                        priceOfProductYInBasket = productPrice.Purchase_Price_Gross
                    Next
                End If

                'total price of product Y in basket
                totalPriceOfProductYInBasket = Utilities.RoundToValue(priceOfProductYInBasket * totalQuantityOfProductYInBasket, 0.01)

                'If the discount is a percentage, get the total for the items included in the discount
                'and calculate the discount as a percentage of their total value
                If isPercentage Then
                    Dim products As New Generic.Dictionary(Of String, WebPriceProduct)

                    'If we have not passed a matchingProducts dictionary into the
                    'function then we are using a SpendX promotion
                    If matchingProducts Is Nothing Then
                        If isOrMorePromo Then
                            discountValue = Utilities.RoundToValue((totalPriceOfProductYInBasket / 100) * value, 0.01)
                        Else
                            Dim minSpend As Decimal = Utilities.CheckForDBNull_Decimal(promo("MIN_SPEND"))
                            discountValue = Utilities.RoundToValue((minSpend / 100) * value, 0.01)
                        End If
                    Else
                        'Otherwise we are using a BuyX... promotion
                        discountValue = Utilities.RoundToValue((totalPriceOfProductYInBasket / 100) * value, 0.01)
                    End If
                Else 'Otherwise, use the value from the database as the discount value
                    discountValue = value
                End If
            End If

            'record the balance spend tally for free delivery cal if allowed additional promotion
            'use pre promotion value for free del cal is false
            _spendTallyBalanceForFreeDelCal = _spendTallyBalanceForFreeDelCal - discountValue

            'The promotion has been successfully applied, but we only
            'add the details to the results table if they have not
            'already been added
            Dim currentResultsRow As DataRow
            If promoHasBeenAppliedBefore Then
                currentResultsRow = PromotionResults.Select("PromotionCode = '" & promo("PROMOTION_CODE") & "'")(0)
                currentResultsRow("ApplicationCount") += 1
                currentResultsRow("PromotionValue") += discountValue
                If Not matchingProducts Is Nothing Then
                    currentResultsRow("ProductCodes") = ProcessMatchedProductCodes(matchingProducts, currentResultsRow("ProductCodes"))
                End If
            Else
                currentResultsRow = PromotionResults.NewRow
                currentResultsRow("PromotionCode") = Utilities.CheckForDBNull_String(promo("PROMOTION_CODE"))
                currentResultsRow("BusinessUnit") = Utilities.CheckForDBNull_String(promo("BUSINESS_UNIT"))
                currentResultsRow("Partner") = Utilities.CheckForDBNull_String(promo("PARTNER"))
                currentResultsRow("Success") = True
                currentResultsRow("PromotionDisplayName") = Utilities.CheckForDBNull_String(promoLang.Rows(0)("DISPLAY_NAME"))
                currentResultsRow("PromotionValue") = discountValue
                currentResultsRow("ErrorMessage") = String.Empty
                currentResultsRow("ActivationMechanism") = Utilities.CheckForDBNull_String(promo("ACTIVATION_MECHANISM"))
                currentResultsRow("ApplicationCount") = 1
                currentResultsRow("ActiveFrom") = Utilities.CheckForDBNull_DateTime(promo("START_DATE"))
                currentResultsRow("ActiveTo") = Utilities.CheckForDBNull_DateTime(promo("END_DATE"))
                currentResultsRow("FreeDelivery") = False
                currentResultsRow("ProductCodes") = ""
                If Not matchingProducts Is Nothing Then
                    currentResultsRow("ProductCodes") = ProcessMatchedProductCodes(matchingProducts, currentResultsRow("ProductCodes"))
                End If
                currentResultsRow("PromotionPercentageValue") = 0
                If isPercentage Then
                    currentResultsRow("PromotionPercentageValue") = value
                End If

                PromotionResults.Rows.Add(currentResultsRow)
            End If
        End If

        Return err
    End Function

    Protected Function ProcessMatchedProductCodes(ByVal matchingProducts As Generic.Dictionary(Of String, Decimal), ByVal currentString As String) As String


        If currentString.Trim.Length > 0 Then
            Dim codes() As String = currentString.Split(",")
            Dim exists As Boolean = False
            For Each matchingCode As String In matchingProducts.Keys
                exists = False
                For Each code As String In codes
                    If UCase(code) = UCase(matchingCode) Then
                        exists = True
                        Exit For
                    End If
                Next
                If Not exists Then
                    currentString += "," & matchingCode
                End If
            Next
        Else
            For Each key As String In matchingProducts.Keys
                If currentString.Length > 0 Then currentString += ","
                currentString += key
            Next
        End If


        Return currentString
    End Function

    Protected Function Do_GetSuppliedDiscount_Action_Sql2005(ByVal promo As DataRow, _
                                                                ByVal promoHasBeenAppliedBefore As Boolean, _
                                                                ByVal promoLang As DataTable) As ErrorObj

        Dim err As New ErrorObj

        'The promotion has been successfully applied, but we only
        'add the details to the results table if they have not
        'already been added
        Dim currentResultsRow As DataRow
        If promoHasBeenAppliedBefore Then
            'Do nothing as this is currently set up to pass down the delivery value
            'and the free delivery cannot be redeemed more than once
            '----------------------------------------------
            'currentResultsRow = PromotionResults.Select("PromotionCode = '" & promo("PROMOTION_CODE") & "'")(0)
            'currentResultsRow("ApplicationCount") += 1
            'currentResultsRow("PromotionValue") += Dep.DiscountValue
        Else
            currentResultsRow = PromotionResults.NewRow
            currentResultsRow("PromotionCode") = Utilities.CheckForDBNull_String(promo("PROMOTION_CODE"))
            currentResultsRow("BusinessUnit") = Utilities.CheckForDBNull_String(promo("BUSINESS_UNIT"))
            currentResultsRow("Partner") = Utilities.CheckForDBNull_String(promo("PARTNER"))
            currentResultsRow("Success") = True
            currentResultsRow("PromotionDisplayName") = Utilities.CheckForDBNull_String(promoLang.Rows(0)("DISPLAY_NAME"))
            currentResultsRow("PromotionValue") = Dep.DiscountValue
            currentResultsRow("ErrorMessage") = String.Empty
            currentResultsRow("ActivationMechanism") = Utilities.CheckForDBNull_String(promo("ACTIVATION_MECHANISM"))
            currentResultsRow("ApplicationCount") = 1
            currentResultsRow("ActiveFrom") = Utilities.CheckForDBNull_DateTime(promo("START_DATE"))
            currentResultsRow("ActiveTo") = Utilities.CheckForDBNull_DateTime(promo("END_DATE"))
            currentResultsRow("FreeDelivery") = True
            currentResultsRow("PromotionPercentageValue") = 0
            PromotionResults.Rows.Add(currentResultsRow)
        End If

        Return err
    End Function

    Protected Function Do_GroupDeal_Action_Sql2005(ByVal promo As DataRow, _
                                                   ByVal promoHasBeenAppliedBefore As Boolean, _
                                                   ByVal promoLang As DataTable, _
                                                   ByVal matchingProducts As Generic.Dictionary(Of String, Decimal)) As ErrorObj

        Dim err As New ErrorObj

        Dim discountValue As Decimal = 0


        Dim products As New Generic.Dictionary(Of String, WebPriceProduct)
        'Add each product code to an arraylist
        For Each productCode As String In matchingProducts.Keys
            For Each rw As DataRow In Me.PromoProductsTallyTable.Rows
                If UCase(rw("ProductCode")) = productCode Then
                    products.Add(productCode, New WebPriceProduct(productCode, CDec(rw("InitialQuantity")), CStr(rw("MasterProductCode"))))
                End If
            Next

        Next

        Dim pricingsettings As New Talent.Common.DEWebPriceSetting(Settings.FrontEndConnectionString, _
                                                                    Settings.DestinationDatabase, _
                                                                    "", _
                                                                    Settings.BusinessUnit, _
                                                                    Settings.Partner, _
                                                                    Dep.PriceList, _
                                                                    False, _
                                                                    Talent.Common.Utilities.GetDefaultLanguage, _
                                                                    Dep.PartnerGroup, _
                                                                    Dep.SecondaryPriceList, _
                                                                    Dep.UsersAttributeList)

        Dim productPricing As New Talent.Common.TalentWebPricing(pricingsettings, products, True)
        productPricing.GetWebPrices()

        'Get the total value of the products being used for this promotion
        Dim machingProductsTotalValue As Decimal = 0
        If Not err.HasError Then
            For Each productPrice As DEWebPrice In productPricing.RetrievedPrices.Values
                machingProductsTotalValue += productPrice.Purchase_Price_Gross * productPrice.RequestedQuantity
            Next
        End If

        discountValue = Utilities.RoundToValue((machingProductsTotalValue - Utilities.CheckForDBNull_Decimal(promo("NEW_PRICE"))), 0.01)

        'The promotion has been successfully applied, but we only
        'add the details to the results table if they have not
        'already been added
        Dim currentResultsRow As DataRow
        If promoHasBeenAppliedBefore Then
            currentResultsRow = PromotionResults.Select("PromotionCode = '" & promo("PROMOTION_CODE") & "'")(0)
            currentResultsRow("ApplicationCount") += 1
            currentResultsRow("PromotionValue") += discountValue 'Utilities.CheckForDBNull_Decimal(promo("NEW_PRICE"))
            'If the promo has already been applied, we need to ensure any newly used product codes
            'are added to the list
            If Not matchingProducts Is Nothing Then
                currentResultsRow("ProductCodes") = ProcessMatchedProductCodes(matchingProducts, currentResultsRow("ProductCodes"))
            End If
        Else
            currentResultsRow = PromotionResults.NewRow
            currentResultsRow("PromotionCode") = Utilities.CheckForDBNull_String(promo("PROMOTION_CODE"))
            currentResultsRow("BusinessUnit") = Utilities.CheckForDBNull_String(promo("BUSINESS_UNIT"))
            currentResultsRow("Partner") = Utilities.CheckForDBNull_String(promo("PARTNER"))
            currentResultsRow("Success") = True
            currentResultsRow("PromotionDisplayName") = Utilities.CheckForDBNull_String(promoLang.Rows(0)("DISPLAY_NAME"))
            currentResultsRow("PromotionValue") = discountValue 'Utilities.CheckForDBNull_Decimal(promo("NEW_PRICE"))
            currentResultsRow("ErrorMessage") = String.Empty
            currentResultsRow("ActivationMechanism") = Utilities.CheckForDBNull_String(promo("ACTIVATION_MECHANISM"))
            currentResultsRow("ApplicationCount") = 1
            currentResultsRow("ActiveFrom") = Utilities.CheckForDBNull_DateTime(promo("START_DATE"))
            currentResultsRow("ActiveTo") = Utilities.CheckForDBNull_DateTime(promo("END_DATE"))
            currentResultsRow("ProductCodes") = ""
            If Not matchingProducts Is Nothing Then
                currentResultsRow("ProductCodes") = ProcessMatchedProductCodes(matchingProducts, currentResultsRow("ProductCodes"))
            End If
            currentResultsRow("FreeDelivery") = False
            currentResultsRow("PromotionPercentageValue") = 0
            PromotionResults.Rows.Add(currentResultsRow)
        End If

        Return err
    End Function

    Protected Function CheckRequiredProducts_Sql2005(ByVal webPricesEntityList As Generic.Dictionary(Of String, DEWebPrice)) As DataTable
        Dim dt As New DataTable
        Dim sbProductsList As New System.Text.StringBuilder
        Dim count As Integer = 0
        For Each wp As DEWebPrice In webPricesEntityList.Values
            If count > 0 Then
                sbProductsList.Append(",'" & wp.PRODUCT_CODE & "'")
            Else
                sbProductsList.Append("'" & wp.PRODUCT_CODE & "'")
            End If
            count += 1
        Next

        Dim cmd As New SqlCommand("", New SqlConnection(Settings.FrontEndConnectionString))
        Dim SelectStr As String = " SELECT    * " & _
                                    " FROM  tbl_promotions WITH (NOLOCK)  INNER JOIN " & _
                                    "           tbl_promotions_required_products WITH (NOLOCK)  " & _
                                    "           ON tbl_promotions.BUSINESS_UNIT = tbl_promotions_required_products.BUSINESS_UNIT " & _
                                    "           AND tbl_promotions.PARTNER_GROUP = tbl_promotions_required_products.PARTNER_GROUP " & _
                                    "           AND tbl_promotions.PARTNER = tbl_promotions_required_products.PARTNER " & _
                                    "           AND tbl_promotions.PROMOTION_CODE = tbl_promotions_required_products.PROMOTION_CODE  " & _
                                    "           INNER JOIN tbl_promotions_lang WITH (NOLOCK)   " & _
                                    "           ON tbl_promotions.BUSINESS_UNIT = tbl_promotions_lang.BUSINESS_UNIT  " & _
                                    "           AND tbl_promotions.PARTNER_GROUP = tbl_promotions_lang.PARTNER_GROUP  " & _
                                    "           AND tbl_promotions.PARTNER = tbl_promotions_lang.PARTNER  " & _
                                    "           AND tbl_promotions.PROMOTION_CODE = tbl_promotions_lang.PROMOTION_CODE " & _
                                    " WHERE (tbl_promotions.BUSINESS_UNIT IN (@BusinessUnit,'*ALL'))   " & _
                                    "   AND (tbl_promotions.PARTNER_GROUP IN (@PartnerGroup,'*ALL'))   " & _
                                    "   AND (tbl_promotions.PARTNER IN (@Partner,'*ALL'))   " & _
                                    "   AND (tbl_promotions.START_DATE <= @CurrentDate)   " & _
                                    "   AND (tbl_promotions.END_DATE >= @CurrentDate)   " & _
                                    "   AND (tbl_promotions.ACTIVE = 'True')   " & _
                                    "   AND (tbl_promotions_required_products.PRODUCT_CODE IN ({0}))   " & _
                                    "   AND (tbl_promotions_lang.LANGUAGE_CODE = @LanguageCode) " & _
                                    "   ORDER BY tbl_promotions.PRIORITY_SEQUENCE "

        SelectStr = String.Format(SelectStr, sbProductsList.ToString())

        cmd.CommandText = SelectStr

        With cmd.Parameters
            .Add("@BusinessUnit", SqlDbType.NVarChar).Value = Dep.BusinessUnit
            .Add("@PartnerGroup", SqlDbType.NVarChar).Value = Dep.PartnerGroup
            .Add("@Partner", SqlDbType.NVarChar).Value = Dep.Partner
            .Add("@CurrentDate", SqlDbType.NVarChar).Value = Now.ToString("dd MMM yyyy")
            .Add("@LanguageCode", SqlDbType.NVarChar).Value = Dep.LanguageCode
        End With
        Try
            cmd.Connection.Open()
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)
            da.Dispose()
        Catch ex As Exception
        Finally
            cmd.Connection.Close()
        End Try
        Return dt
    End Function

    Protected Function CheckRequiredProducts_Sql2005(ByVal productCode As String) As DataTable
        Dim dt As New DataTable

        Dim cmd As New SqlCommand("", New SqlConnection(Settings.FrontEndConnectionString))
        Dim SelectStr As String = " SELECT    * " & _
                                    " FROM  tbl_promotions WITH (NOLOCK)  INNER JOIN " & _
                                    "           tbl_promotions_required_products WITH (NOLOCK)  " & _
                                    "           ON tbl_promotions.BUSINESS_UNIT = tbl_promotions_required_products.BUSINESS_UNIT " & _
                                    "           AND tbl_promotions.PARTNER_GROUP = tbl_promotions_required_products.PARTNER_GROUP " & _
                                    "           AND tbl_promotions.PARTNER = tbl_promotions_required_products.PARTNER " & _
                                    "           AND tbl_promotions.PROMOTION_CODE = tbl_promotions_required_products.PROMOTION_CODE  " & _
                                    "           INNER JOIN tbl_promotions_lang WITH (NOLOCK)   " & _
                                    "           ON tbl_promotions.BUSINESS_UNIT = tbl_promotions_lang.BUSINESS_UNIT  " & _
                                    "           AND tbl_promotions.PARTNER_GROUP = tbl_promotions_lang.PARTNER_GROUP  " & _
                                    "           AND tbl_promotions.PARTNER = tbl_promotions_lang.PARTNER  " & _
                                    "           AND tbl_promotions.PROMOTION_CODE = tbl_promotions_lang.PROMOTION_CODE " & _
                                    " WHERE (tbl_promotions.BUSINESS_UNIT = @BusinessUnit)   " & _
                                    "   AND (tbl_promotions.PARTNER_GROUP = @PartnerGroup)   " & _
                                    "   AND (tbl_promotions.PARTNER = @Partner)   " & _
                                    "   AND (tbl_promotions.START_DATE <= @CurrentDate)   " & _
                                    "   AND (tbl_promotions.END_DATE >= @CurrentDate)   " & _
                                    "   AND (tbl_promotions.ACTIVE = 'True')   " & _
                                    "   AND (tbl_promotions_required_products.PRODUCT_CODE = @ProductCode)   " & _
                                    "   AND (tbl_promotions_lang.LANGUAGE_CODE = @LanguageCode) " & _
                                    "   ORDER BY tbl_promotions.PRIORITY_SEQUENCE "
        cmd.CommandText = SelectStr

        With cmd.Parameters
            .Add("@BusinessUnit", SqlDbType.NVarChar).Value = Dep.BusinessUnit
            .Add("@PartnerGroup", SqlDbType.NVarChar).Value = Dep.PartnerGroup
            .Add("@Partner", SqlDbType.NVarChar).Value = Dep.Partner
            .Add("@CurrentDate", SqlDbType.NVarChar).Value = Now.ToString("dd MMM yyyy")
            .Add("@ProductCode", SqlDbType.NVarChar).Value = productCode
            .Add("@LanguageCode", SqlDbType.NVarChar).Value = Dep.LanguageCode
        End With
        Try
            cmd.Connection.Open()

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            If dt.Rows.Count = 0 Then
                cmd.Parameters("@Partner").Value = Utilities.GetAllString
                da.Fill(dt)
                If dt.Rows.Count = 0 Then
                    cmd.Parameters("@PartnerGroup").Value = Utilities.GetAllString
                    da.Fill(dt)
                    If dt.Rows.Count = 0 Then
                        cmd.Parameters("@BusinessUnit").Value = Utilities.GetAllString
                        da.Fill(dt)
                    End If
                End If
            End If

            da.Dispose()
        Catch ex As Exception
        Finally
            cmd.Connection.Close()
        End Try

        Return dt
    End Function

    Protected Function GetPromotions_Sql2005(ByVal activationMechanism As String, Optional ByVal promotionCode As String = "") As DataTable
        Dim dt As New DataTable

        Dim cmd As New SqlCommand("", New SqlConnection(Settings.FrontEndConnectionString))
        Dim SelectStr As String = " SELECT * FROM tbl_promotions WITH (NOLOCK)  " & _
                                  " WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                  " AND PARTNER_GROUP = @PARTNER_GROUP " & _
                                  " AND PARTNER = @PARTNER " & _
                                  " AND START_DATE <= @CURRENT_DATE " & _
                                  " AND END_DATE >= @CURRENT_DATE " & _
                                  " AND REDEEM_COUNT <= REDEEM_MAX " & _
                                  " AND ACTIVATION_MECHANISM = @ACTIVATION_MECHANISM " & _
                                  " AND ACTIVE = 'True' "

        'If a promo code has been supplied, include it in the statement
        If Not String.IsNullOrEmpty(promotionCode) Then
            SelectStr += " AND PROMOTION_CODE = @PROMOTION_CODE "
        End If
        SelectStr += " ORDER BY PRIORITY_SEQUENCE ASC "

        cmd.CommandText = SelectStr

        With cmd.Parameters
            .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = Dep.BusinessUnit
            .Add("@PARTNER_GROUP", SqlDbType.NVarChar).Value = Dep.PartnerGroup
            .Add("@PARTNER", SqlDbType.NVarChar).Value = Dep.Partner
            .Add("@CURRENT_DATE", SqlDbType.DateTime).Value = Dep.CurrentDate
            .Add("@ACTIVATION_MECHANISM", SqlDbType.NVarChar).Value = activationMechanism
            If Not String.IsNullOrEmpty(promotionCode) Then
                .Add("@PROMOTION_CODE", SqlDbType.NVarChar).Value = promotionCode
            End If
        End With

        Try
            cmd.Connection.Open()

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            If dt.Rows.Count = 0 Then
                cmd.Parameters("@PARTNER").Value = Utilities.GetAllString
                da.Fill(dt)
                If dt.Rows.Count = 0 Then
                    cmd.Parameters("@PARTNER_GROUP").Value = Utilities.GetAllString
                    da.Fill(dt)
                    If dt.Rows.Count = 0 Then
                        cmd.Parameters("@BUSINESS_UNIT").Value = Utilities.GetAllString
                        da.Fill(dt)
                    End If
                End If
            End If

            da.Dispose()
        Catch ex As Exception
        Finally
            cmd.Connection.Close()
        End Try

        Return dt
    End Function

    Protected Function GetRequiredProducts_Sql2005(ByVal promoCode As String) As DataTable
        Dim dt As New DataTable

        Dim cmd As New SqlCommand("", New SqlConnection(Settings.FrontEndConnectionString))
        Dim SelectStr As String = " SELECT * FROM tbl_promotions_required_products WITH (NOLOCK)  " & _
                                  " WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                  " AND PARTNER_GROUP = @PARTNER_GROUP " & _
                                  " AND PARTNER = @PARTNER " & _
                                  " AND PROMOTION_CODE = @PROMOTION_CODE "

        cmd.CommandText = SelectStr

        With cmd.Parameters
            .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = Dep.BusinessUnit
            .Add("@PARTNER_GROUP", SqlDbType.NVarChar).Value = Dep.PartnerGroup
            .Add("@PARTNER", SqlDbType.NVarChar).Value = Dep.Partner
            .Add("@PROMOTION_CODE", SqlDbType.NVarChar).Value = promoCode
        End With

        Try
            cmd.Connection.Open()

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            If dt.Rows.Count = 0 Then
                cmd.Parameters("@PARTNER").Value = Utilities.GetAllString
                da.Fill(dt)
                If dt.Rows.Count = 0 Then
                    cmd.Parameters("@PARTNER_GROUP").Value = Utilities.GetAllString
                    da.Fill(dt)
                    If dt.Rows.Count = 0 Then
                        cmd.Parameters("@BUSINESS_UNIT").Value = Utilities.GetAllString
                        da.Fill(dt)
                    End If
                End If
            End If


            da.Dispose()
        Catch ex As Exception
        Finally
            cmd.Connection.Close()
        End Try

        Return dt
    End Function

    Protected Function GetFreeProducts_Sql2005(ByVal promoCode As String) As DataTable
        Dim dt As New DataTable

        Dim cmd As New SqlCommand("", New SqlConnection(Settings.FrontEndConnectionString))
        Dim SelectStr As String = " SELECT * FROM tbl_promotions_free_products WITH (NOLOCK)  " & _
                                  " WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                  " AND PARTNER_GROUP = @PARTNER_GROUP " & _
                                  " AND PARTNER = @PARTNER " & _
                                  " AND PROMOTION_CODE = @PROMOTION_CODE "

        cmd.CommandText = SelectStr

        With cmd.Parameters
            .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = Dep.BusinessUnit
            .Add("@PARTNER_GROUP", SqlDbType.NVarChar).Value = Dep.PartnerGroup
            .Add("@PARTNER", SqlDbType.NVarChar).Value = Dep.Partner
            .Add("@PROMOTION_CODE", SqlDbType.NVarChar).Value = promoCode
        End With

        Try
            cmd.Connection.Open()

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)


            If dt.Rows.Count = 0 Then
                cmd.Parameters("@PARTNER").Value = Utilities.GetAllString
                da.Fill(dt)
                If dt.Rows.Count = 0 Then
                    cmd.Parameters("@PARTNER_GROUP").Value = Utilities.GetAllString
                    da.Fill(dt)
                    If dt.Rows.Count = 0 Then
                        cmd.Parameters("@BUSINESS_UNIT").Value = Utilities.GetAllString
                        da.Fill(dt)
                    End If
                End If
            End If


            da.Dispose()
        Catch ex As Exception
        Finally
            cmd.Connection.Close()
        End Try

        Return dt
    End Function

    Protected Function GetGroupProductDetails_Sql2005(ByVal productCode As String) As DataTable
        Dim dt As New DataTable

        Dim cmd As New SqlCommand("", New SqlConnection(Settings.FrontEndConnectionString))
        Dim SelectStr As String = " SELECT TOP 1 * FROM tbl_group_product WITH (NOLOCK)  " & _
                                  " WHERE PRODUCT = @PRODUCT "
        cmd.CommandText = SelectStr

        With cmd.Parameters
            .Add("@PRODUCT", SqlDbType.NVarChar).Value = productCode
        End With

        Try
            cmd.Connection.Open()
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)
            da.Dispose()
        Catch ex As Exception
        Finally
            cmd.Connection.Close()
        End Try

        Return dt
    End Function

    Protected Function GetPromotionsDiscounts_Sql2005(ByVal promoCode As String) As DataTable
        Dim dt As New DataTable

        Dim cmd As New SqlCommand("", New SqlConnection(Settings.FrontEndConnectionString))
        Dim SelectStr As String = " SELECT * FROM tbl_promotions_discounts WITH (NOLOCK)  " & _
                                  " WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                  " AND PARTNER_GROUP = @PARTNER_GROUP " & _
                                  " AND PARTNER = @PARTNER " & _
                                  " AND PROMOTION_CODE = @PROMOTION_CODE "

        cmd.CommandText = SelectStr

        With cmd.Parameters
            .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = Dep.BusinessUnit
            .Add("@PARTNER_GROUP", SqlDbType.NVarChar).Value = Dep.PartnerGroup
            .Add("@PARTNER", SqlDbType.NVarChar).Value = Dep.Partner
            .Add("@PROMOTION_CODE", SqlDbType.NVarChar).Value = promoCode
        End With

        Try
            cmd.Connection.Open()

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)
            If dt.Rows.Count = 0 Then
                cmd.Parameters("@PARTNER").Value = Utilities.GetAllString
                da.Fill(dt)
                If dt.Rows.Count = 0 Then
                    cmd.Parameters("@PARTNER_GROUP").Value = Utilities.GetAllString
                    da.Fill(dt)
                    If dt.Rows.Count = 0 Then
                        cmd.Parameters("@BUSINESS_UNIT").Value = Utilities.GetAllString
                        da.Fill(dt)
                    End If
                End If
            End If


            da.Dispose()
        Catch ex As Exception
        Finally
            cmd.Connection.Close()
        End Try

        Return dt
    End Function

    Protected Function GetPromotionsLanguage_Sql2005(ByVal promoCode As String) As DataTable
        Dim dt As New DataTable

        Dim cmd As New SqlCommand("", New SqlConnection(Settings.FrontEndConnectionString))
        Dim SelectStr As String = " SELECT * FROM tbl_promotions_lang WITH (NOLOCK)  " & _
                                  " WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                  " AND PARTNER_GROUP = @PARTNER_GROUP " & _
                                  " AND PARTNER = @PARTNER " & _
                                  " AND PROMOTION_CODE = @PROMOTION_CODE " & _
                                  " AND LANGUAGE_CODE = @LANGUAGE_CODE "

        cmd.CommandText = SelectStr

        With cmd.Parameters
            .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = Dep.BusinessUnit
            .Add("@PARTNER_GROUP", SqlDbType.NVarChar).Value = Dep.PartnerGroup
            .Add("@PARTNER", SqlDbType.NVarChar).Value = Dep.Partner
            .Add("@PROMOTION_CODE", SqlDbType.NVarChar).Value = promoCode
            .Add("@LANGUAGE_CODE", SqlDbType.NVarChar).Value = Dep.LanguageCode
        End With

        Try
            cmd.Connection.Open()

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            If dt.Rows.Count = 0 Then
                cmd.Parameters("@PARTNER").Value = Utilities.GetAllString
                da.Fill(dt)
                If dt.Rows.Count = 0 Then
                    cmd.Parameters("@PARTNER_GROUP").Value = Utilities.GetAllString
                    da.Fill(dt)
                    If dt.Rows.Count = 0 Then
                        cmd.Parameters("@BUSINESS_UNIT").Value = Utilities.GetAllString
                        da.Fill(dt)
                    End If
                End If
            End If


            da.Dispose()
        Catch ex As Exception
        Finally
            cmd.Connection.Close()
        End Try

        Return dt
    End Function

    Protected Function GetPromotionsUsers_Sql2005(ByVal username As String, _
                                                    ByVal start_date As DateTime, _
                                                    ByVal end_date As DateTime, _
                                                    ByVal promo_code As String) As DataTable
        Dim dt As New DataTable

        Dim cmd As New SqlCommand("", New SqlConnection(Settings.FrontEndConnectionString))
        Dim SelectStr As String = " SELECT * FROM tbl_promotions_users WITH (NOLOCK)  " & _
                                  " WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                  " AND PARTNER_GROUP = @PARTNER_GROUP " & _
                                  " AND PARTNER = @PARTNER " & _
                                  " AND ACTIVE_FROM <= @START_DATE " & _
                                  " AND ACTIVE_TO >= @END_DATE " & _
                                  " AND PROMOTION_CODE = @PROMOTION_CODE " & _
                                  " AND USERNAME = @USERNAME "

        cmd.CommandText = SelectStr

        With cmd.Parameters
            .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = Dep.BusinessUnit
            .Add("@PARTNER_GROUP", SqlDbType.NVarChar).Value = Dep.PartnerGroup
            .Add("@PARTNER", SqlDbType.NVarChar).Value = Dep.Partner
            .Add("@START_DATE", SqlDbType.DateTime).Value = start_date
            .Add("@END_DATE", SqlDbType.DateTime).Value = end_date
            .Add("@PROMOTION_CODE", SqlDbType.NVarChar).Value = promo_code
            .Add("@USERNAME", SqlDbType.NVarChar).Value = username
        End With

        Try
            cmd.Connection.Open()

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            da.Dispose()
        Catch ex As Exception
        Finally
            cmd.Connection.Close()
        End Try

        Return dt
    End Function

    Protected Sub StorePromotionResults()

        ' change the status of success to column to false and 
        ' update that row if it found in promotionresults table
        ' delete the row where success is false

        Const UpdateSuccessStr As String = " UPDATE tbl_promotions_redeemed SET SUCCESS = 0   " & _
                                    "   WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                    "   AND PARTNER_GROUP = @PARTNER_GROUP " & _
                                    "   AND PARTNER = @PARTNER " & _
                                    "   AND TEMP_ORDER_ID = @TEMP_ORDER_ID " & _
                                     "   AND LOGINID = @LOGINID  " & _
                                    "    "
        Const DeleteFalseSuccessStr As String = " DELETE FROM  tbl_promotions_redeemed   " & _
                                    "   WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                    "   AND PARTNER_GROUP = @PARTNER_GROUP " & _
                                    "   AND PARTNER = @PARTNER " & _
                                    "   AND TEMP_ORDER_ID = @TEMP_ORDER_ID " & _
                                    "   AND LOGINID = @LOGINID  " & _
                                    "   AND SUCCESS = 0 "

        Const UpdateInsertStr As String = " IF EXISTS (" & _
                                     "SELECT ID FROM tbl_promotions_redeemed " & _
                                     "   WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                     "   AND PARTNER_GROUP = @PARTNER_GROUP " & _
                                     "   AND PARTNER = @PARTNER " & _
                                     "   AND TEMP_ORDER_ID = @TEMP_ORDER_ID " & _
                                     "   AND ACTIVATION_MECHANISM = @ACTIVATION_MECHANISM  " & _
                                     "   AND PROMOTION_CODE = @PROMOTION_CODE  " & _
                                     "   AND LOGINID = @LOGINID" & _
                                     ")" & _
                                     " BEGIN " & _
                                        " UPDATE tbl_promotions_redeemed  SET " & _
                                        " ACTIVE_FROM_DATE = @ACTIVE_FROM_DATE, ACTIVE_TO_DATE = @ACTIVE_TO_DATE, " & _
                                        " SUCCESS = @SUCCESS, PROMOTION_DISPLAY_NAME = @PROMOTION_DISPLAY_NAME, " & _
                                        " PROMOTION_VALUE = @PROMOTION_VALUE, ERROR_MESSAGE = @ERROR_MESSAGE, " & _
                                        " APPLICATION_COUNT = @APPLICATION_COUNT" & _
                                        "   WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                        "   AND PARTNER_GROUP = @PARTNER_GROUP " & _
                                        "   AND PARTNER = @PARTNER " & _
                                        "   AND TEMP_ORDER_ID = @TEMP_ORDER_ID " & _
                                        "   AND ACTIVATION_MECHANISM = @ACTIVATION_MECHANISM  " & _
                                        "   AND PROMOTION_CODE = @PROMOTION_CODE  " & _
                                        "   AND LOGINID = @LOGINID  " & _
                                     " END " & _
                                     " ELSE " & _
                                     " BEGIN " & _
                                            " INSERT INTO tbl_promotions_redeemed  " & _
                                            "(TEMP_ORDER_ID, LOGINID, BUSINESS_UNIT, PARTNER_GROUP," & _
                                            "PARTNER, PROMOTION_CODE, ACTIVE_FROM_DATE," & _
                                            "ACTIVE_TO_DATE, SUCCESS, PROMOTION_DISPLAY_NAME," & _
                                            "PROMOTION_VALUE, ERROR_MESSAGE, ACTIVATION_MECHANISM," & _
                                            "APPLICATION_COUNT)" & _
                                            "    VALUES(@TEMP_ORDER_ID, @LOGINID, @BUSINESS_UNIT, @PARTNER_GROUP, @PARTNER, " & _
                                            "           @PROMOTION_CODE, @ACTIVE_FROM_DATE, @ACTIVE_TO_DATE, " & _
                                            "           @SUCCESS, @PROMOTION_DISPLAY_NAME, @PROMOTION_VALUE, " & _
                                            "           @ERROR_MESSAGE, @ACTIVATION_MECHANISM, @APPLICATION_COUNT ) " & _
                                     " END "


        Dim cmd As New SqlCommand("", New SqlConnection(Settings.FrontEndConnectionString))

        cmd.CommandText = UpdateSuccessStr

        Try
            cmd.Connection.Open()

            With cmd.Parameters
                .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = Dep.BusinessUnit
                .Add("@PARTNER_GROUP", SqlDbType.NVarChar).Value = Dep.PartnerGroup
                .Add("@PARTNER", SqlDbType.NVarChar).Value = Dep.Partner
                .Add("@TEMP_ORDER_ID", SqlDbType.NVarChar).Value = Dep.TempOrderID
                .Add("@LOGINID", SqlDbType.NVarChar).Value = Dep.Username
            End With

            cmd.ExecuteNonQuery()
            cmd.CommandText = UpdateInsertStr

            For Each promo As DataRow In PromotionResults.Rows
                If CBool(promo("SUCCESS")) Then
                    With cmd.Parameters
                        .Clear()
                        .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = Dep.BusinessUnit
                        .Add("@PARTNER_GROUP", SqlDbType.NVarChar).Value = Dep.PartnerGroup
                        .Add("@PARTNER", SqlDbType.NVarChar).Value = Dep.Partner
                        .Add("@TEMP_ORDER_ID", SqlDbType.NVarChar).Value = Dep.TempOrderID
                        .Add("@LOGINID", SqlDbType.NVarChar).Value = Dep.Username
                        .Add("@PROMOTION_CODE", SqlDbType.NVarChar).Value = promo("PromotionCode")
                        .Add("@ACTIVE_FROM_DATE", SqlDbType.DateTime).Value = promo("ActiveFrom")
                        .Add("@ACTIVE_TO_DATE", SqlDbType.DateTime).Value = promo("ActiveTo")
                        .Add("@SUCCESS", SqlDbType.Bit).Value = promo("Success")
                        .Add("@PROMOTION_DISPLAY_NAME", SqlDbType.NVarChar).Value = promo("PromotionDisplayName")
                        .Add("@PROMOTION_VALUE", SqlDbType.NVarChar).Value = promo("PromotionValue")
                        .Add("@ERROR_MESSAGE", SqlDbType.NVarChar).Value = promo("ErrorMessage")
                        .Add("@ACTIVATION_MECHANISM", SqlDbType.NVarChar).Value = promo("ActivationMechanism")
                        .Add("@APPLICATION_COUNT", SqlDbType.NVarChar).Value = promo("ApplicationCount")
                    End With
                    cmd.ExecuteNonQuery()
                End If
            Next

            cmd.CommandText = DeleteFalseSuccessStr
            With cmd.Parameters
                .Clear()
                .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = Dep.BusinessUnit
                .Add("@PARTNER_GROUP", SqlDbType.NVarChar).Value = Dep.PartnerGroup
                .Add("@PARTNER", SqlDbType.NVarChar).Value = Dep.Partner
                .Add("@TEMP_ORDER_ID", SqlDbType.NVarChar).Value = Dep.TempOrderID
                .Add("@LOGINID", SqlDbType.NVarChar).Value = Dep.Username
            End With
            cmd.ExecuteNonQuery()

        Catch ex As Exception
        Finally
            cmd.Connection.Close()
        End Try
    End Sub

    Protected Sub NewPromotionResultsTable()
        PromotionResults = New DataTable("PromotionResults")
        With PromotionResults.Columns
            .Add("PromotionCode", GetType(String))
            .Add("BusinessUnit", GetType(String))
            .Add("Partner", GetType(String))
            .Add("Success", GetType(Boolean))
            .Add("PromotionDisplayName", GetType(String))
            .Add("PromotionValue", GetType(Decimal))
            .Add("ErrorMessage", GetType(String))
            .Add("ActivationMechanism", GetType(String))
            .Add("ApplicationCount", GetType(Integer))
            .Add("ActiveFrom", GetType(DateTime))
            .Add("ActiveTo", GetType(DateTime))
            .Add("ProductCodes", GetType(String))
            .Add("FreeDelivery", GetType(Boolean))
            .Add("PromotionPercentageValue", GetType(Decimal))
        End With
    End Sub

    Protected Sub NewPromoProductsTallyTable()
        PromoProductsTallyTable = New DataTable
        With PromoProductsTallyTable.Columns
            .Add("ProductCode", GetType(String))
            .Add("InitialQuantity", GetType(String))
            .Add("UsedQuantity", GetType(String))
            .Add("RemainingQuantity", GetType(String))
            .Add("MasterProductCode", GetType(String))
        End With

        'Put the basket items into the tally table
        Dim prod As DataRow
        For Each key As String In Dep.BasketItems.Keys
            prod = PromoProductsTallyTable.NewRow
            prod("ProductCode") = Dep.BasketItems(key).ProductCode
            prod("InitialQuantity") = Dep.BasketItems(key).Quantity
            prod("UsedQuantity") = 0
            prod("RemainingQuantity") = Dep.BasketItems(key).Quantity
            prod("MasterProductCode") = Dep.BasketItems(key).MasterProductCode
            PromoProductsTallyTable.Rows.Add(prod)
        Next
    End Sub

    Protected Function DeleteFreeItemsFromBasket() As ErrorObj
        Dim err As New ErrorObj

        Try

            Dim amendBasket As New Talent.Common.DEAmendBasket
            With amendBasket
                .DeleteFreeItems = True
                .BasketId = Dep.BasketHeaderID
                .BusinessUnit = Dep.BusinessUnit
                .PartnerCode = Dep.Partner
                .UserID = Dep.Username
                .IsFreeItem = True
            End With

            Dim DBAmend As New DBAmendBasket
            With DBAmend
                .Dep = amendBasket
                .Settings = Settings
                .AccessDatabase()
            End With

        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "Delete Free Items Error"
            err.ErrorMessage = ex.Message
        End Try

        Return err
    End Function

    Protected Sub GetUsersAttributesList()
        Dim atts As String = String.Empty
        If HttpContext.Current.User.Identity.IsAuthenticated Then
            Dim SelectStr As String = " SELECT ATTRIBUTES_LIST  " & _
                                              " FROM tbl_partner_user " & _
                                              " Where LOGINID = @USERNAME "
            Dim conn As New SqlConnection(Settings.FrontEndConnectionString)
            Dim cmd As New SqlCommand(SelectStr, conn)
            With cmd.Parameters
                .Add("@USERNAME", SqlDbType.NVarChar).Value = Dep.Username
            End With

            Try
                conn.Open()
                atts = cmd.ExecuteScalar()
                conn.Close()
            Catch ex As Exception
            Finally
                cmd.Dispose()
                conn.Dispose()
            End Try
            Me.Dep.UsersAttributeList = atts
        Else
            Me.Dep.UsersAttributeList = ""
        End If

    End Sub

    Protected Function AccessDatabaseWS613R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim sLastRecord As String = "000"
        Dim sRecordTotal As String = "000"
        Dim bMoreRecords As Boolean = True

        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUTBasket As String = String.Empty

        'Create the basket data tables
        _basketFunctions.CreateBasketTables(ResultDataSet)

        Dim DtPromotionStatus As New DataTable("PromotionStatus")
        ResultDataSet.Tables.Add(DtPromotionStatus)
        With DtPromotionStatus.Columns
            .Add("SuccessCode", GetType(String))
        End With

        Try
            '
            'Call WS613R
            PARAMOUT = CallWS613R()
            PARAMOUTBasket = PARAMOUT.Substring("1024")
            PARAMOUT = PARAMOUT.Substring("0", "1024")

            '
            ' Is there an error code from AddTicketingItems
            If PARAMOUT.ToString.Trim.Length >= 1023 Then
                dRow = Nothing
                dRow = ResultDataSet.Tables("BasketStatus").NewRow
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
                ResultDataSet.Tables("BasketStatus").Rows.Add(dRow)
                err.ErrorNumber = PARAMOUT.Substring(1021, 2)
                err.HasError = True
            Else
                dRow = Nothing
                dRow = ResultDataSet.Tables("PromotionStatus").NewRow
                dRow("SuccessCode") = PARAMOUT.Substring(985, 1)
                ResultDataSet.Tables("PromotionStatus").Rows.Add(dRow)
                ' Retrieve the ticketing shopping basket
                _basketFunctions.RetrieveTicketingShoppingBasketValues(PARAMOUTBasket, ResultDataSet, err, "WS613R")
            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDABPM-WS613R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Protected Function AccessDatabasePM001S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Customer Activities data table
        Dim dtPartnerPromotions As New DataTable("PartnerPromotions")
        With dtPartnerPromotions.Columns
            .Add("ProductCode", GetType(String))
            .Add("ProductDescription", GetType(String))
            .Add("StadiumCode", GetType(String))
            .Add("ProductType", GetType(String))
            .Add("PriceCode", GetType(String))
        End With
        ResultDataSet.Tables.Add(dtPartnerPromotions)

        Try
            CallPM001S()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBPromotions-PM001S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

#End Region

#Region "Public Functions"

    Public Function GetPromotionStatus(ByVal productcode As String) As Generic.Dictionary(Of String, String)
        Dim results As New Generic.Dictionary(Of String, String)

        Dim promos As DataTable = Me.CheckRequiredProducts_Sql2005(productcode)

        If Not promos Is Nothing AndAlso promos.Rows.Count > 0 Then
            Dim promo As DataRow = promos.Rows(0)
            results.Add("PromotionCode", Utilities.CheckForDBNull_String(promo("PROMOTION_CODE")))
            results.Add("PromotionType", Utilities.CheckForDBNull_String(promo("PROMOTION_TYPE")))
            results.Add("PromotionActivationMechanism", Utilities.CheckForDBNull_String(promo("ACTIVATION_MECHANISM")))
            results.Add("DisplayName", Utilities.CheckForDBNull_String(promo("DISPLAY_NAME")))
        End If

        Return results
    End Function

    Public Function GetPromotionStatusForReqProducts(ByVal webPricesEntityList As Generic.Dictionary(Of String, DEWebPrice)) As DataTable

        Dim dtPromotions As DataTable = Me.CheckRequiredProducts_Sql2005(webPricesEntityList)
        Return dtPromotions

    End Function

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Modifies the free items in basket. This method will perform update and delete in tbl_basket_detail based on the free items in the collection
    ''' </summary><returns></returns>
    Private Function ModifyFreeItemsInBasket() As Talent.Common.ErrorObj
        Dim err As New ErrorObj
        Try
            If _deAlertsCollections.Count > 0 Then
                Dim amendBasket As New Talent.Common.DEAmendBasket
                With amendBasket
                    '.AddToBasket = True
                    .AddFreeItems = True
                    .BasketId = Dep.BasketHeaderID
                    .BusinessUnit = Dep.BusinessUnit
                    .PartnerCode = Dep.Partner
                    .UserID = Dep.Username
                    .IsFreeItem = True
                End With
                For itemIndex As Integer = 0 To (_deAlertsCollections.Count - 1)
                    amendBasket.CollDEAlerts.Add(_deAlertsCollections.Item(itemIndex))
                Next

                Dim DBAmend As New DBAmendBasket
                With DBAmend
                    .Dep = amendBasket
                    .Settings = Settings
                    .AccessDatabase()
                End With
            Else
                DeleteFreeItemsFromBasket()
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "Modify Free Items In Basket Error"
            err.ErrorMessage = ex.Message
        End Try

        Return err
    End Function

    Private Sub AssignFreeDelieveryDefaults()
        Dim dt As New DataTable

        Dim cmd As New SqlCommand("", New SqlConnection(Settings.FrontEndConnectionString))
        Dim SelectStr As String = " SELECT [DEFAULT_NAME], [VALUE] FROM tbl_ecommerce_module_defaults_bu WITH (NOLOCK)  " & _
                                   " WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                   " AND PARTNER = @PARTNER " & _
                                   " AND (DEFAULT_NAME = 'ALLOW_ADDITIONAL_PROMOTION_WITH_FREE_DEL' OR DEFAULT_NAME = 'USE_PRE_PROMOTION_VALUE_FOR_FREE_DEL_CAL') "

        'Todo get the full product details from tbl_group_product
        ' then DEAlerts should have needed properties to build url in basket
        cmd.CommandText = SelectStr

        With cmd.Parameters
            .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = Dep.BusinessUnit
            .Add("@PARTNER", SqlDbType.NVarChar).Value = Dep.Partner
        End With

        Try
            cmd.Connection.Open()

            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)

            If dt.Rows.Count = 0 Then
                cmd.Parameters("@PARTNER").Value = Utilities.GetAllString
                da.Fill(dt)
                If dt.Rows.Count = 0 Then
                    cmd.Parameters("@BUSINESS_UNIT").Value = Utilities.GetAllString
                    da.Fill(dt)
                End If
            End If
            da.Dispose()
        Catch ex As Exception
        Finally
            cmd.Connection.Close()
        End Try
        If dt.Rows.Count > 0 Then
            Dim tempDefaultName As String = String.Empty
            For rowIndex As Integer = 0 To dt.Rows.Count - 1
                tempDefaultName = UCase((dt.Rows(rowIndex)("DEFAULT_NAME")).ToString())
                If tempDefaultName = "ALLOW_ADDITIONAL_PROMOTION_WITH_FREE_DEL" Then
                    _allowAdditionalPromotionWithFreeDel = Utilities.CheckForDBNull_Boolean_DefaultFalse(dt.Rows(rowIndex)("VALUE"))
                ElseIf tempDefaultName = "USE_PRE_PROMOTION_VALUE_FOR_FREE_DEL_CAL" Then
                    _UsePrePromotionValueForFreeDelCal = Utilities.CheckForDBNull_Boolean_DefaultFalse(dt.Rows(rowIndex)("VALUE"))
                End If
            Next
        End If
        'Decide the maximum codes allowed with free delivery
        If _allowAdditionalPromotionWithFreeDel Then
            _maxFreeDelCodesAllowed = 1
        End If
        _maxNumberOfCodesAllowed = _maxGeneralCodesAllowed + _maxFreeDelCodesAllowed
    End Sub

    Private Function IsCodeAllowedToProcess(ByVal promotionCode As String, ByVal promo As DataRow) As Boolean
        Dim isCodeAllowed As Boolean = False
        Dim promotionType As String = Utilities.CheckForDBNull_String(UCase(promo("PROMOTION_TYPE")))
        If promotionType.Length > 0 Then
            If _currentCodeCount < _maxNumberOfCodesAllowed Then
                If (promotionType = SPEND_X_OR_MORE_GET_SUPPLIED_DISCOUNT) OrElse (promotionType = BUY_X_OR_MORE_GET_SUPPLIED_DISCOUNT) Then
                    If _freeDelCodeCount < _maxFreeDelCodesAllowed Then
                        _freeDelCodeCount = _freeDelCodeCount + 1
                        isCodeAllowed = True
                        _currentCodeCount = _currentCodeCount + 1
                    End If
                Else
                    If _generalCodeCount < _maxGeneralCodesAllowed Then
                        _generalCodeCount = _generalCodeCount + 1
                        _currentCodeCount = _currentCodeCount + 1
                        isCodeAllowed = True
                    End If
                End If
            Else
                isCodeAllowed = False
            End If
        End If
        Return isCodeAllowed
    End Function

    Private Function IsAutoCodeAllowedToProcess(ByVal promotionCode As String, ByVal promo As DataRow) As Boolean
        Dim isAutoCodeAllowed As Boolean = False
        Dim promotionType As String = Utilities.CheckForDBNull_String(UCase(promo("PROMOTION_TYPE")))
        If promotionType.Length > 0 Then
            If (promotionType = SPEND_X_OR_MORE_GET_SUPPLIED_DISCOUNT) OrElse (promotionType = BUY_X_OR_MORE_GET_SUPPLIED_DISCOUNT) Then
                If _freeDelCodeCount < _maxFreeDelCodesAllowed Then
                    _freeDelCodeCount = _freeDelCodeCount + 1
                    isAutoCodeAllowed = True
                    _currentCodeCount = _currentCodeCount + 1
                End If
            Else
                isAutoCodeAllowed = True
            End If
        End If
        Return isAutoCodeAllowed
    End Function

    Private Sub ResetCodeCount(ByVal promotionType As String, ByVal activationMechanism As String)
        If activationMechanism = "CODE" Then
            If (promotionType = SPEND_X_OR_MORE_GET_SUPPLIED_DISCOUNT) OrElse (promotionType = BUY_X_OR_MORE_GET_SUPPLIED_DISCOUNT) Then
                If _freeDelCodeCount > 0 AndAlso _currentCodeCount > 0 Then
                    _freeDelCodeCount = _freeDelCodeCount - 1
                    _currentCodeCount = _currentCodeCount - 1
                End If
            Else
                If _generalCodeCount > 0 AndAlso _currentCodeCount > 0 Then
                    _generalCodeCount = _generalCodeCount - 1
                    _currentCodeCount = _currentCodeCount - 1
                End If
            End If
        ElseIf activationMechanism = "AUTO" Then
            If (promotionType = SPEND_X_OR_MORE_GET_SUPPLIED_DISCOUNT) OrElse (promotionType = BUY_X_OR_MORE_GET_SUPPLIED_DISCOUNT) Then
                If _freeDelCodeCount > 0 AndAlso _currentCodeCount > 0 Then
                    _freeDelCodeCount = _freeDelCodeCount - 1
                    _currentCodeCount = _currentCodeCount - 1
                End If
            End If
        End If

    End Sub

    Private Function GetUserEnteredPromoCodesByPriority() As DataView
        Dim dtUserPromoCodes As New DataTable
        Dim dvPromocode As DataView
        With dtUserPromoCodes.Columns
            .Add("PROMOTION_CODE", GetType(String))
            .Add("PRIORITY", GetType(Integer))
            .Add("USER_ENTERED_ORDER", GetType(Integer))
            .Add("MOVED", GetType(Integer))
            .Add("PROMOTYPE", GetType(String))
            .Add("NEW_PRIORITY", GetType(Integer))
        End With
        Dim PromotionCodes As String()
        Dim charSeparators() As Char = {";"c}
        PromotionCodes = (Dep.PromotionCode).Split(charSeparators, System.StringSplitOptions.RemoveEmptyEntries)
        Dim drPromoCode As DataRow
        Dim totalCodes As Integer = PromotionCodes.Length
        'build user entered promo code table
        If totalCodes > 0 Then
            If totalCodes = 1 Then
                drPromoCode = dtUserPromoCodes.NewRow
                drPromoCode("PROMOTION_CODE") = PromotionCodes(0)
                drPromoCode("PRIORITY") = 1
                drPromoCode("USER_ENTERED_ORDER") = 1
                drPromoCode("MOVED") = 0
                drPromoCode("PROMOTYPE") = ""
                drPromoCode("NEW_PRIORITY") = 0
                dtUserPromoCodes.Rows.Add(drPromoCode)
            Else
                Dim dtCodePromotions As DataTable = GetPromotions_Sql2005(code)
                Dim promotionType As String = ""
                For promoIndex As Integer = 0 To totalCodes - 1
                    Dim drPromoCodeDetails() As DataRow = dtCodePromotions.Select("PROMOTION_CODE='" & PromotionCodes(promoIndex) & "'")
                    If drPromoCodeDetails.Length > 0 Then
                        For promoCodeDetailsIndex As Integer = 0 To drPromoCodeDetails.Length - 1
                            drPromoCode = dtUserPromoCodes.NewRow
                            drPromoCode("PROMOTION_CODE") = PromotionCodes(promoIndex)
                            drPromoCode("PRIORITY") = drPromoCodeDetails(promoCodeDetailsIndex)("PRIORITY_SEQUENCE")
                            drPromoCode("USER_ENTERED_ORDER") = promoIndex + 1
                            drPromoCode("MOVED") = 0
                            drPromoCode("NEW_PRIORITY") = 0
                            promotionType = Utilities.CheckForDBNull_String(UCase(drPromoCodeDetails(promoCodeDetailsIndex)("PROMOTION_TYPE")))
                            If (promotionType = SPEND_X_OR_MORE_GET_SUPPLIED_DISCOUNT) OrElse (promotionType = BUY_X_OR_MORE_GET_SUPPLIED_DISCOUNT) Then
                                drPromoCode("PROMOTYPE") = "F" 'Free
                            Else
                                drPromoCode("PROMOTYPE") = "G" 'General
                            End If
                            dtUserPromoCodes.Rows.Add(drPromoCode)
                        Next
                    Else
                        drPromoCode = dtUserPromoCodes.NewRow
                        drPromoCode("PROMOTION_CODE") = PromotionCodes(promoIndex)
                        drPromoCode("PRIORITY") = 0
                        drPromoCode("USER_ENTERED_ORDER") = promoIndex + 1
                        drPromoCode("MOVED") = 0
                        drPromoCode("NEW_PRIORITY") = 0
                        drPromoCode("PROMOTYPE") = ""
                        dtUserPromoCodes.Rows.Add(drPromoCode)
                    End If
                Next
                dtCodePromotions = Nothing
            End If

            drPromoCode = Nothing
            PromotionCodes = Nothing

            'decide the allow additional free del status and decide order by column
            dvPromocode = dtUserPromoCodes.DefaultView
            If _allowAdditionalPromotionWithFreeDel Then
                If _UsePrePromotionValueForFreeDelCal Then
                    'order by user entered order desc
                    dvPromocode.Sort = "USER_ENTERED_ORDER DESC"
                Else
                    'order by general user entered order and free with priority and make own priority
                    If dtUserPromoCodes.Rows.Count > 1 Then
                        Dim tempDvPromoCodes As DataView = dtUserPromoCodes.DefaultView
                        tempDvPromoCodes.Sort = "USER_ENTERED_ORDER DESC"
                        tempDvPromoCodes.RowFilter = "PROMOTYPE='G'"
                        Dim newPriority As Integer = 0
                        If tempDvPromoCodes.Count > 0 Then
                            For Each drvGeneralPromoCode As DataRowView In tempDvPromoCodes
                                Dim drFreePromoCodes() As DataRow = dtUserPromoCodes.Select("PRIORITY < " & drvGeneralPromoCode("PRIORITY") & " AND PROMOTYPE='F' AND MOVED=0")
                                If drFreePromoCodes.Length > 0 Then
                                    For freePromoCodeIndex As Integer = 0 To drFreePromoCodes.Length - 1
                                        newPriority = newPriority + 1
                                        drFreePromoCodes(freePromoCodeIndex)("NEW_PRIORITY") = newPriority
                                        drFreePromoCodes(freePromoCodeIndex)("MOVED") = 1
                                    Next
                                End If
                                newPriority = newPriority + 1
                                drvGeneralPromoCode("NEW_PRIORITY") = newPriority
                                drvGeneralPromoCode("MOVED") = 1
                            Next
                            'move any unmoved promotions
                            Dim drRemainPromoCodes() As DataRow = dtUserPromoCodes.Select("MOVED=0")
                            If drRemainPromoCodes.Length > 0 Then
                                For freePromoCodeIndex As Integer = 0 To drRemainPromoCodes.Length - 1
                                    newPriority = newPriority + 1
                                    drRemainPromoCodes(freePromoCodeIndex)("NEW_PRIORITY") = newPriority
                                    drRemainPromoCodes(freePromoCodeIndex)("MOVED") = 1
                                Next
                            End If
                            drRemainPromoCodes = Nothing
                            dtUserPromoCodes.DefaultView.RowFilter = ""
                            dvPromocode = dtUserPromoCodes.DefaultView
                            dvPromocode.Sort = "NEW_PRIORITY ASC"
                        Else
                            dvPromocode.Sort = "USER_ENTERED_ORDER DESC"
                        End If
                    Else
                        dvPromocode.Sort = "USER_ENTERED_ORDER DESC"
                    End If
                End If
            Else
                'order by user entered order desc
                dvPromocode.Sort = "USER_ENTERED_ORDER DESC"
            End If
        Else
            dvPromocode = dtUserPromoCodes.DefaultView
        End If
        Return dvPromocode
    End Function

    Private Function IsProductYInBasket(ByVal promoCode As String) As Boolean
        Dim isYExists As Boolean = False
        Dim discounts As DataTable = GetPromotionsDiscounts_Sql2005(promoCode)

        If discounts.Rows.Count > 0 Then
            Dim discount As DataRow = discounts.Rows(0)
            Dim productCode_Y As String = Utilities.CheckForDBNull_String(discount("PRODUCT_CODE")).Trim.ToUpper()
            For Each key As String In Dep.BasketItems.Keys
                If ((productCode_Y = Dep.BasketItems(key).ProductCode) OrElse (productCode_Y = Dep.BasketItems(key).MasterProductCode)) Then
                    isYExists = True
                    Exit For
                End If
            Next
        End If
        Return isYExists
    End Function

    Private Function ReadPromotionDetailsTALENTTKT() As ErrorObj
        Dim err As New ErrorObj
        Dim intPosition As Integer = 0
        Dim moreResults As Boolean = True
        Dim firstCall As Boolean = True
        Dim nextStartingNumber As Integer = 0
        Dim totalToReturn As Integer = 0
        Dim lastDetailReturned As Integer = 0

        dtTicketingPromoDetails = New DataTable("PromotionDetails")
        ResultDataSet = New DataSet
        ResultDataSet.Tables.Add(dtTicketingPromoDetails)

        If dtTicketingPromoDetails.Columns.Count = 0 Then
            AddColumnsToDataTable()
        End If

        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/WS008R(@PARAM1, @PARAM2)"
        Dim parmInput, parmInput2 As iDB2Parameter
        Dim PARMOUT As String = String.Empty
        Dim ws008Param As New StringBuilder
        Dim promotionSettings As New DEPromotionSettings
        promotionSettings = CType(Settings, DEPromotionSettings)

        If Not err.HasError Then
            While moreResults And nextStartingNumber < 1000

                Try
                    ws008Param.Clear()
                    ws008Param.Append(Utilities.FixStringLength(String.Empty, 5090))
                    ws008Param.Append(nextStartingNumber.ToString.PadLeft(5, "0"))
                    ws008Param.Append(Utilities.PadLeadingZeros(_dep.PromotionId, 13))
                    ws008Param.Append(Utilities.FixStringLength(promotionSettings.IncludeProductPurchasers, 1))
                    ws008Param.Append(Utilities.FixStringLength(String.Empty, 1))                   
                    ws008Param.Append(Utilities.FixStringLength(_dep.Source, 1))
                    ws008Param.Append(Utilities.FixStringLength(String.Empty, 3))

                    cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)
                    parmInput = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 5120)
                    parmInput.Value = ws008Param.ToString()
                    parmInput.Direction = ParameterDirection.InputOutput

                    parmInput2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 5120)
                    parmInput2.Value = WS008Parm2()
                    parmInput2.Direction = ParameterDirection.InputOutput

                    cmdSELECT.ExecuteNonQuery()
                    PARMOUT = cmdSELECT.Parameters(Param1).Value.ToString

                Catch ex As Exception
                    Const strError2 As String = "Error during database access"
                    moreResults = False
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError2
                        .ErrorNumber = "TACDBPROMO1"
                        .HasError = True
                    End With
                End Try

                If Not err.HasError Then
                    '----------------
                    ' Process Results
                    '----------------
                    Try
                        Dim iCounter As Integer
                        Dim iPosition As Integer = 0
                        Dim sWork As String = PARMOUT.Substring(0, 5119)
                        Dim dr As DataRow = Nothing
                        Dim strQty As String = String.Empty
                        Dim year, month, day As Integer

                        For iCounter = 0 To 15
                            iPosition = iCounter * 300
                            If sWork.Substring(iPosition, 50).Trim > String.Empty Then
                                dr = Nothing
                                dr = dtTicketingPromoDetails.NewRow()
                                dr("PromotionID") = sWork.Substring(iPosition, 13).Trim
                                dr("ShortDescription") = sWork.Substring(iPosition + 13, 20).Trim
                                dr("Description1") = sWork.Substring(iPosition + 33, 40).Trim
                                dr("Description2") = sWork.Substring(iPosition + 73, 40).Trim
                                dr("CompetitionDescription") = sWork.Substring(iPosition + 113, 50).Trim
                                dr("MaxPerProduct") = CDec(sWork.Substring(iPosition + 163, 5))
                                dr("MaxPerPromotion") = CDec(sWork.Substring(iPosition + 168, 5).Trim)
                                dr("PromotionType") = sWork.Substring(iPosition + 187, 1).Trim
                                dr("Priority") = sWork.Substring(iPosition + 267, 5).Trim
                                dr("MatchType") = sWork.Substring(iPosition + 190, 1).Trim
                                dr("ProductCode") = sWork.Substring(iPosition + 191, 6).Trim
                                dr("Stand") = sWork.Substring(iPosition + 197, 3).Trim
                                dr("Area") = sWork.Substring(iPosition + 200, 4).Trim
                                dr("PreReq") = sWork.Substring(iPosition + 204, 6).Trim
                                For i As Integer = 1 To 10
                                    dr("PriceCode" & i) = sWork.Substring(iPosition + 210 + (2 * (i - 1)), 2).Trim
                                Next
                                dr("PriceBand") = sWork.Substring(iPosition + 230, 1).Trim
                                dr("CompetitionCode") = sWork.Substring(iPosition + 231, 6).Trim
                                If sWork.Substring(iPosition + 173, 1) = "1" Then
                                    year = CInt(sWork.Substring(iPosition + 174, 2)) + 2000
                                Else
                                    year = CInt(sWork.Substring(iPosition + 174, 2)) + 1900
                                End If
                                month = CInt(sWork.Substring(iPosition + 176, 2))
                                day = CInt(sWork.Substring(iPosition + 178, 2))
                                Try
                                    dr("StartDate") = New Date(year, month, day)
                                Catch
                                End Try
                                If sWork.Substring(iPosition + 180, 1) = "1" Then
                                    year = CInt(sWork.Substring(iPosition + 181, 2)) + 2000
                                Else
                                    year = CInt(sWork.Substring(iPosition + 181, 2)) + 1900
                                End If
                                month = CInt(sWork.Substring(iPosition + 183, 2))
                                day = CInt(sWork.Substring(iPosition + 185, 2))
                                Try
                                    dr("EndDate") = New Date(year, month, day)
                                Catch
                                End Try
                                dr("PackageId") = sWork.Substring(iPosition + 235, 13).Trim
                                dr("DiscountValue") = Utilities.CheckForDBNull_BigInt(sWork.Substring(iPosition + 248, 3).Trim)
                                dr("DiscountSalePrice") = Utilities.CheckForDBNull_Decimal(Utilities.FormatPrice(sWork.Substring(iPosition + 251, 9).Trim))
                                dr("FeesRemoved") = sWork.Substring(iPosition + 260, 6).Trim
                                dtTicketingPromoDetails.Rows.Add(dr)
                            Else
                                Exit For
                            End If
                        Next
                        '------------------------
                        ' Check if more to return
                        '------------------------ 
                        lastDetailReturned = CInt(PARMOUT.Substring(5090, 5))
                        If firstCall Then
                            totalToReturn = CInt(PARMOUT.Substring(5085, 5))
                            firstCall = False
                        End If

                        If lastDetailReturned >= totalToReturn Then
                            moreResults = False
                        Else
                            nextStartingNumber = lastDetailReturned
                        End If
                    Catch ex As Exception
                        Const strError2 As String = "Error processing results"
                        moreResults = False
                        With err
                            .ErrorMessage = ex.Message
                            .ErrorStatus = strError2
                            .ErrorNumber = "TACDBPROMO2"
                            .HasError = True
                        End With
                    End Try
                Else
                    moreResults = False
                End If

            End While

        End If

        Return err
    End Function

    ''' <summary>
    ''' Sets the parameter 2 value for WS008R.
    ''' </summary>
    ''' <returns>parameter 2 value</returns>
    Private Function WS008Parm2() As String
        Dim parameter2Value As String
        'Construct the parameter
        parameter2Value = Utilities.FixStringLength(Settings.OriginatingSource, 10) & _
                        Utilities.FixStringLength(" ", 5110)
        Return parameter2Value
    End Function

    Private Sub AddColumnsToDataTable()
        With dtTicketingPromoDetails.Columns
            .Add("PromotionID", GetType(String))
            .Add("PromotionType", GetType(String))
            .Add("Priority", GetType(String))
            .Add("MatchType", GetType(String))
            .Add("ProductCode", GetType(String))
            .Add("Stand", GetType(String))
            .Add("Area", GetType(String))
            .Add("PreReq", GetType(String))
            For i As Integer = 1 To 10
                .Add("PriceCode" & i, GetType(String))
            Next
            .Add("PriceBand", GetType(String))
            .Add("CompetitionCode", GetType(String))
            .Add("ShortDescription", GetType(String))
            .Add("Description1", GetType(String))
            .Add("Description2", GetType(String))
            .Add("CompetitionDescription", GetType(String))
            .Add("MaxPerProduct", GetType(Decimal))
            .Add("MaxPerPromotion", GetType(Decimal))
            .Add("StartDate", GetType(Date))
            .Add("EndDate", GetType(Date))
            .Add("PackageId", GetType(String))
            .Add("DiscountValue", GetType(Integer))
            .Add("DiscountSalePrice", GetType(Decimal))
            .Add("FeesRemoved", GetType(String))
        End With
    End Sub

    Private Function CallWS613R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS613R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1, @PARAM2)"
        Dim parmIO As iDB2Parameter
        Dim parmIOBasket As iDB2Parameter
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUTBasket As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS613Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        parmIOBasket = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 5120)
        parmIOBasket.Value = Utilities.FixStringLength("", 5120)
        parmIOBasket.Direction = ParameterDirection.InputOutput

        'Execute
        Utilities.TalentCommonLog("CallWS613R", Settings.LoginId, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()

        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUTBasket = cmdSELECT.Parameters(Param2).Value.ToString
        PARAMOUT = Utilities.FixStringLength(PARAMOUT, "1024") & PARAMOUTBasket

        Utilities.TalentCommonLog("CallWS613R", Settings.LoginId, "Backend Response: PARAMOUT=" & PARAMOUT & ", PARAMOUTBasket=" & PARAMOUTBasket)

        Return PARAMOUT

    End Function

    Private Function WS613Parm() As String

        Dim myString As String
        Dim validPromotionCode As String = ""
        'split promotion codes and take only the last one
        Dim PromotionCodes As String()
        Dim charSeparators() As Char = {";"c}
        PromotionCodes = (Dep.PromotionCode).Split(charSeparators, System.StringSplitOptions.RemoveEmptyEntries)
        If PromotionCodes.Length > 0 Then
            validPromotionCode = PromotionCodes(PromotionCodes.Length - 1)
        End If
        'Construct the parameter
        myString = Utilities.FixStringLength(Dep.BasketHeaderID.ToString, 36) & _
                    Utilities.FixStringLength(validPromotionCode, 30) & _
                    Utilities.FixStringLength("", 910) & _
                    Utilities.PadLeadingZerosDec(Dep.BasketTotal, 9) & _
                    Utilities.FixStringLength("", 7) & _
                    Utilities.FixStringLength(Settings.OriginatingSource, 10) & _
                    Utilities.PadLeadingZeros(Settings.LoginId, 12) & _
                    Utilities.FixStringLength("", 6) & _
                    Utilities.FixStringLength(Settings.OriginatingSourceCode, 1) & _
                    Utilities.FixStringLength("", 3)
        Return myString

    End Function

    Private Function DBRetrievePromotionHistory() As Talent.Common.ErrorObj

        Dim cmdSELECT As iDB2Command = Nothing
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim bMoreRecords As Boolean = True
        Dim bError As Boolean = False
        Dim dRow As DataRow = Nothing
        Dim strProgram As String = "PM001R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO1 As iDB2Parameter
        Dim PARAMOUT1 As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameters
        parmIO1 = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 5120)
        parmIO1.Value = buildPM001RParm()
        parmIO1.Direction = ParameterDirection.InputOutput



        'Create the Status data table
        Dim DtStatusResults As New DataTable("Status")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the PromotionHistory table 
        Dim dtPromotionHistory As New DataTable("PromotionHistory")
        dtPromotionHistory.Columns.Add("ProductType", GetType(String))
        dtPromotionHistory.Columns.Add("Priority", GetType(Decimal))
        dtPromotionHistory.Columns.Add("PromotionDescription", GetType(String))
        dtPromotionHistory.Columns.Add("PreRequisiteProduct", GetType(String))
        dtPromotionHistory.Columns.Add("MaxDiscountPromotions", GetType(Decimal))
        dtPromotionHistory.Columns.Add("MaxProductPromotions", GetType(Decimal))
        dtPromotionHistory.Columns.Add("CustomerAllocation", GetType(String))
        dtPromotionHistory.Columns.Add("PromotionID", GetType(String))
        ResultDataSet.Tables.Add(dtPromotionHistory)


        ' Call PM001R 1st time
        Try
            cmdSELECT.ExecuteNonQuery()
            PARAMOUT1 = cmdSELECT.Parameters(Param1).Value.ToString
            bMoreRecords = Utilities.convertToBool(PARAMOUT1.Substring(5110, 1))
            'Set the response data on the first call
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT1.Substring(5119, 1) = "E" Or PARAMOUT1.Substring(5117, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT1.Substring(5117, 2)
                bMoreRecords = False
                bError = True
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
                bMoreRecords = True
            End If

            'No errors 
            If bError = False Then
                Do While bMoreRecords = True
                    'Extract the data from the parameter
                    Dim iPosition As Integer = 99
                    Dim iCounter As Integer = 1
                    Do While iCounter <= 10
                        'Create a new row
                        dRow = Nothing
                        dRow = dtPromotionHistory.NewRow
                        If Trim(PARAMOUT1.Substring(iPosition, 10)) <> String.Empty Then
                            dRow("ProductType") = PARAMOUT1.Substring(iPosition, 6)
                            dRow("Priority") = PARAMOUT1.Substring(iPosition + 6, 3)
                            dRow("PromotionDescription") = PARAMOUT1.Substring(iPosition + 9, 20)
                            dRow("PreRequisiteProduct") = PARAMOUT1.Substring(iPosition + 29, 6)
                            dRow("MaxDiscountPromotions") = PARAMOUT1.Substring(iPosition + 35, 5)
                            dRow("MaxProductPromotions") = PARAMOUT1.Substring(iPosition + 40, 5)
                            dRow("CustomerAllocation") = PARAMOUT1.Substring(iPosition + 45, 11)
                            dRow("PromotionID") = PARAMOUT1.Substring(iPosition + 56, 13)
                            dtPromotionHistory.Rows.Add(dRow)
                        End If
                        iPosition = iPosition + 200
                        iCounter = iCounter + 1
                    Loop
                    ' Call PM001R again if more records
                    bMoreRecords = Utilities.convertToBool(PARAMOUT1.Substring(5110, 1))
                    If bMoreRecords = True Then
                        cmdSELECT.ExecuteNonQuery()
                        PARAMOUT1 = cmdSELECT.Parameters(Param1).Value.ToString
                    End If
                Loop
            End If

        Catch ex As Exception
            Const strError As String = "Error Retrieving Promotion History"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "RetrievePromotionHistory-01"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function DBRetrievePromotionHistoryDetail() As Talent.Common.ErrorObj
        Dim cmdSELECT As iDB2Command = Nothing
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim bMoreRecords As Boolean = True
        Dim bError As Boolean = False
        Dim dRow As DataRow = Nothing
        Dim strProgram As String = "PM003R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1, @PARAM2)"
        Dim parmIO1 As iDB2Parameter
        Dim PARAMOUT1 As String = String.Empty
        Dim parmIO2 As iDB2Parameter
        Dim PARAMOUT2 As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameters
        parmIO1 = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 5120)
        '  Dep.Agent = Session("agent")

        parmIO1.Value = " **Not Used for WEB call** "
        parmIO1.Direction = ParameterDirection.InputOutput
        parmIO2.Value = buildPM003RParm()
        parmIO2.Direction = ParameterDirection.InputOutput

        'Create the Status data table
        Dim DtStatusResults As New DataTable("Status")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the PromotionHistorydetaiHeader table  
        Dim dtPromotionHistoryDetailHeader As New DataTable("PromotionHistoryDetailHeader")
        dtPromotionHistoryDetailHeader.Columns.Add("ProductType", GetType(String))
        dtPromotionHistoryDetailHeader.Columns.Add("Priority", GetType(String))
        dtPromotionHistoryDetailHeader.Columns.Add("PriceCodes", GetType(String))
        dtPromotionHistoryDetailHeader.Columns.Add("PriceBand", GetType(String))
        ResultDataSet.Tables.Add(dtPromotionHistoryDetailHeader)

        'Create the PromotionHistorydetaiLine table  
        Dim dtPromotionHistoryDetailLine As New DataTable("PromotionHistoryDetailLine")
        dtPromotionHistoryDetailLine.Columns.Add("ProductType", GetType(String))
        dtPromotionHistoryDetailLine.Columns.Add("ProductDescription", GetType(String))
        dtPromotionHistoryDetailLine.Columns.Add("Seat", GetType(String))
        dtPromotionHistoryDetailLine.Columns.Add("PriceCode", GetType(String))
        dtPromotionHistoryDetailLine.Columns.Add("PriceBand", GetType(String))
        dtPromotionHistoryDetailLine.Columns.Add("SoldDate", GetType(String))
        ResultDataSet.Tables.Add(dtPromotionHistoryDetailLine)


        ' Call PM003R 1st time
        Try
            Dim Iseriesjob As String = cmdSELECT.Connection.JobName
            cmdSELECT.ExecuteNonQuery()
            PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString
            bMoreRecords = Utilities.convertToBool(PARAMOUT2.Substring(5110, 1))
            'Set the response data on the first call 
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT2.Substring(5119, 1) = "E" Or PARAMOUT2.Substring(5117, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT2.Substring(5117, 2)
                bMoreRecords = False
                bError = True
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
                bMoreRecords = True
            End If

            'No errors 
            If bError = False Then

                ' Retrieve header level information 
                Dim iPosition As Integer = 199
                dRow = Nothing
                dRow = dtPromotionHistoryDetailHeader.NewRow
                If Trim(PARAMOUT2.Substring(iPosition, 10)) <> String.Empty Then
                    dRow("ProductType") = PARAMOUT2.Substring(iPosition, 6)
                    dRow("Priority") = PARAMOUT2.Substring(iPosition + 6, 3)
                    dRow("PriceCodes") = PARAMOUT2.Substring(iPosition + 9, 20)
                    dRow("PriceBand") = PARAMOUT2.Substring(iPosition + 29, 6)
                    dtPromotionHistoryDetailHeader.Rows.Add(dRow)
                End If
                ' Line level information
                Do While bMoreRecords = True
                    iPosition = 499
                    Dim iCounter As Integer = 1
                    Do While iCounter <= 10
                        'Create a new row
                        dRow = Nothing
                        dRow = dtPromotionHistoryDetailLine.NewRow
                        If Trim(PARAMOUT2.Substring(iPosition, 10)) <> String.Empty Then
                            dRow("ProductType") = PARAMOUT2.Substring(iPosition, 6)
                            dRow("ProductDescription") = PARAMOUT2.Substring(iPosition + 6, 40)
                            dRow("Seat") = PARAMOUT2.Substring(iPosition + 46, 20)
                            dRow("PriceCode") = PARAMOUT2.Substring(iPosition + 66, 2)
                            dRow("PriceBand") = PARAMOUT2.Substring(iPosition + 68, 1)
                            dRow("SoldDate") = Utilities.ISeriesDate(PARAMOUT2.Substring(iPosition + 69, 7)).ToString("dd/MM/yy")
                            dtPromotionHistoryDetailLine.Rows.Add(dRow)
                        End If
                        iPosition = iPosition + 200
                        iCounter = iCounter + 1
                    Loop
                    ' Call PM003R again if more records
                    bMoreRecords = Utilities.convertToBool(PARAMOUT2.Substring(5110, 1))
                    If bMoreRecords = True Then
                        cmdSELECT.ExecuteNonQuery()
                        PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString
                    End If

                Loop

            End If

        Catch ex As Exception
            Const strError As String = "Error Retrieving Promotion Detail History"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "RetrievePromotionDetailHistory-01"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Private Function buildPM001RParm() As String
        Dim mystring As String
        mystring = Utilities.FixStringLength(Dep.CustomerNumber, 12) & _
                   Utilities.FixStringLength(Dep.Agent, 10) & _
                   Utilities.FixStringLength("", 5094) & _
                     Utilities.FixStringLength("W  ", 3)
        Return mystring
    End Function
    Private Function buildPM003RParm() As String
        Dim mystring As String
        mystring = Utilities.FixStringLength(Dep.CustomerNumber, 12) & _
                   Utilities.FixStringLength(Dep.PromotionId, 13) & _
                      Utilities.FixStringLength(Dep.Company, 13) & _
                   Utilities.FixStringLength("", 5091) & _
                     Utilities.FixStringLength("W  ", 3) & _
        Utilities.FixStringLength("", 3)

        Return mystring
    End Function

    ''' <summary>
    ''' Build the parameters and call the stored procedure
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CallPM001S()
        Dim cmd As iDB2Command = Nothing
        cmd = conTALENTTKT.CreateCommand()
        cmd.CommandText = "CALL PM001S(@PARAM0, @PARAM1, @PARAM2)"
        cmd.CommandType = CommandType.Text

        Dim pPromotionCode As New iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pErrorCode As iDB2Parameter

        pPromotionCode = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 30)
        pSource = cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)
        pErrorCode = cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput

        pPromotionCode.Value = Dep.PromotionCode
        pSource.Value = Dep.Source
        pErrorCode.Value = String.Empty

        Dim cmdAdapter As iDB2DataAdapter = Nothing
        cmdAdapter = New IBM.Data.DB2.iSeries.iDB2DataAdapter
        cmdAdapter.SelectCommand = cmd
        cmdAdapter.Fill(ResultDataSet, "PartnerPromotions")

        Dim drStatus As DataRow = ResultDataSet.Tables("ErrorStatus").NewRow
        If CStr(cmd.Parameters(Param2).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(cmd.Parameters(Param2).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables("ErrorStatus").Rows.Add(drStatus)
    End Sub

#End Region

#Region "OLD Methods"
    ' ------------- NEW BY SANJAY ------
    'Protected Sub StorePromotionResults()
    '    Dim cmd As New SqlCommand("", New SqlConnection(Settings.FrontEndConnectionString))

    '    Const DeleteStrCode1 As String = " IF EXISTS (    " & _
    '                                " SELECT p.PROMOTION_CODE FROM  tbl_promotions p  " & _
    '                                " where p.PROMOTION_CODE = @PROMOTION_CODE  " & _
    '                                " )   " & _
    '                                " BEGIN   " & _
    '                                " 	DELETE FROM  tbl_promotions_redeemed  " & _
    '                                "   WHERE  " & _
    '                                "   ( " & _
    '                                "   BUSINESS_UNIT = @BUSINESS_UNIT " & _
    '                                "   AND PARTNER_GROUP = @PARTNER_GROUP " & _
    '                                "   AND PARTNER = @PARTNER " & _
    '                                "   AND TEMP_ORDER_ID = @TEMP_ORDER_ID " & _
    '                                "   AND LOGINID = @LOGINID " & _
    '                                "   AND " & _
    '                                "   ( " & _
    '                                "       PROMOTION_CODE <> @PROMOTION_CODE " & _
    '                                "       AND " & _
    '                                "       ( " & _
    '                                "       SELECT ACTIVATION_MECHANISM FROM  tbl_promotions t " & _
    '                                "       WHERE PROMOTION_CODE = @PROMOTION_CODE " & _
    '                                "       ) = @ACTIVATION_MECHANISM " & _
    '                                "   ) " & _
    '                                "   AND ACTIVATION_MECHANISM = @ACTIVATION_MECHANISM " & _
    '                                "   ) " & _
    '                                "   OR " & _
    '                                "   ID IN ( " & _
    '                                "   SELECT r.ID FROM  tbl_promotions t " & _
    '                                "   INNER JOIN  tbl_promotions_redeemed r ON t.PROMOTION_CODE = r.PROMOTION_CODE " & _
    '                                "   WHERE r.BUSINESS_UNIT = @BUSINESS_UNIT " & _
    '                                "   AND r.PARTNER_GROUP = @PARTNER_GROUP " & _
    '                                "   AND r.PARTNER = @PARTNER " & _
    '                                "   AND r.TEMP_ORDER_ID = @TEMP_ORDER_ID " & _
    '                                "   AND r.LOGINID = @LOGINID " & _
    '                                "   AND " & _
    '                                "   ( " & _
    '                                "   t.ACTIVE = 0 " & _
    '                                "   OR " & _
    '                                "   NOT (t.START_DATE <= GETDATE() AND t.END_DATE >= GETDATE()) " & _
    '                                "   ) " & _
    '                                "   ) " & _
    '                                " END " & _
    '                                " ELSE " & _
    '                                " BEGIN " & _
    '                                "   DELETE FROM  tbl_promotions_redeemed " & _
    '                                "   WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
    '                                "   AND PARTNER_GROUP = @PARTNER_GROUP " & _
    '                                "   AND PARTNER = @PARTNER " & _
    '                                "   AND TEMP_ORDER_ID = @TEMP_ORDER_ID " & _
    '                                "   AND LOGINID = @LOGINID " & _
    '                                "   AND PROMOTION_CODE = @PROMOTION_CODE " & _
    '                                " END " & _
    '                                "   " & _
    '                                ""

    '    Const InsertStr1 As String = " IF NOT EXISTS ( " & _
    '                                "    SELECT ID FROM tbl_promotions_redeemed " & _
    '                                "    WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
    '                                "    AND PARTNER_GROUP = @PARTNER_GROUP " & _
    '                                "    AND PARTNER = @PARTNER " & _
    '                                "    AND TEMP_ORDER_ID = @TEMP_ORDER_ID " & _
    '                                "    AND LOGINID = @LOGINID " & _
    '                                "    AND PROMOTION_CODE = @PROMOTION_CODE " & _
    '                                ") " & _
    '                                " BEGIN " & _
    '                                " " & _
    '                                " INSERT INTO tbl_promotions_redeemed  " & _
    '                                " ([TEMP_ORDER_ID], [LOGINID], [BUSINESS_UNIT], [PARTNER_GROUP], [PARTNER], [PROMOTION_CODE], " & _
    '                                " [ACTIVE_FROM_DATE], [ACTIVE_TO_DATE], [SUCCESS], [PROMOTION_DISPLAY_NAME], " & _
    '                                " [PROMOTION_VALUE], [ERROR_MESSAGE], [ACTIVATION_MECHANISM], [APPLICATION_COUNT]) " & _
    '                                "    VALUES(@TEMP_ORDER_ID, @LOGINID, @BUSINESS_UNIT, @PARTNER_GROUP, @PARTNER, " & _
    '                                "           @PROMOTION_CODE, @ACTIVE_FROM_DATE, @ACTIVE_TO_DATE, " & _
    '                                "           @SUCCESS, @PROMOTION_DISPLAY_NAME, @PROMOTION_VALUE, " & _
    '                                "           @ERROR_MESSAGE, @ACTIVATION_MECHANISM, @APPLICATION_COUNT ) " & _
    '                                " " & _
    '                                "  END " & _
    '                                " "


    '    Const DeleteStrAutos As String = " DELETE FROM  tbl_promotions_redeemed   " & _
    '                                "   WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
    '                                "   AND PARTNER_GROUP = @PARTNER_GROUP " & _
    '                                "   AND PARTNER = @PARTNER " & _
    '                                "   AND TEMP_ORDER_ID = @TEMP_ORDER_ID " & _
    '                                "   AND LOGINID = @LOGINID  " & _
    '                                "   AND ACTIVATION_MECHANISM = @ACTIVATION_MECHANISM "



    '    Try
    '        Dim insDel As Integer = -1
    '        cmd.Connection.Open()
    '        '
    '        ' 1. delete all AUTO promotion records - these will be refreshed everytime
    '        '
    '        cmd.CommandText = DeleteStrAutos
    '        With cmd.Parameters
    '            .Clear()
    '            .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = Dep.BusinessUnit
    '            .Add("@PARTNER_GROUP", SqlDbType.NVarChar).Value = Dep.PartnerGroup
    '            .Add("@PARTNER", SqlDbType.NVarChar).Value = Dep.Partner
    '            .Add("@TEMP_ORDER_ID", SqlDbType.NVarChar).Value = Dep.TempOrderID
    '            .Add("@LOGINID", SqlDbType.NVarChar).Value = Dep.Username
    '            .Add("@ACTIVATION_MECHANISM", SqlDbType.NVarChar).Value = auto
    '        End With
    '        insDel = cmd.ExecuteNonQuery()
    '        '
    '        ' 2. Only one user entered CODE is allowed per basket. Delete the CODE promo record only if:
    '        ' - a new code has been entered or
    '        ' - the existing code in the redeemed table has expired, been deleted or is inactive
    '        '
    '        cmd.CommandText = DeleteStrCode1
    '        With cmd.Parameters
    '            .Clear()
    '            .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = Dep.BusinessUnit
    '            .Add("@PARTNER_GROUP", SqlDbType.NVarChar).Value = Dep.PartnerGroup
    '            .Add("@PARTNER", SqlDbType.NVarChar).Value = Dep.Partner
    '            .Add("@TEMP_ORDER_ID", SqlDbType.NVarChar).Value = Dep.TempOrderID
    '            .Add("@LOGINID", SqlDbType.NVarChar).Value = Dep.Username
    '            .Add("@PROMOTION_CODE", SqlDbType.NVarChar).Value = Dep.PromotionCode
    '            .Add("@ACTIVATION_MECHANISM", SqlDbType.NVarChar).Value = code
    '        End With
    '        insDel = cmd.ExecuteNonQuery()

    '        '
    '        ' 3. insert only successful promo records: because the AUTO promos are always deleted
    '        ' this will re-insert promos
    '        '
    '        For Each promo As DataRow In PromotionResults.Rows
    '            If CBool(promo("SUCCESS")) Then
    '                cmd.CommandText = InsertStr1
    '                With cmd.Parameters
    '                    .Clear()
    '                    .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = Dep.BusinessUnit
    '                    .Add("@PARTNER_GROUP", SqlDbType.NVarChar).Value = Dep.PartnerGroup
    '                    .Add("@PARTNER", SqlDbType.NVarChar).Value = Dep.Partner
    '                    .Add("@TEMP_ORDER_ID", SqlDbType.NVarChar).Value = Dep.TempOrderID
    '                    .Add("@LOGINID", SqlDbType.NVarChar).Value = Dep.Username
    '                    .Add("@PROMOTION_CODE", SqlDbType.NVarChar).Value = promo("PromotionCode")
    '                    .Add("@ACTIVE_FROM_DATE", SqlDbType.DateTime).Value = promo("ActiveFrom")
    '                    .Add("@ACTIVE_TO_DATE", SqlDbType.DateTime).Value = promo("ActiveTo")
    '                    .Add("@SUCCESS", SqlDbType.Bit).Value = promo("Success")
    '                    .Add("@PROMOTION_DISPLAY_NAME", SqlDbType.NVarChar).Value = promo("PromotionDisplayName")
    '                    .Add("@PROMOTION_VALUE", SqlDbType.NVarChar).Value = promo("PromotionValue")
    '                    .Add("@ERROR_MESSAGE", SqlDbType.NVarChar).Value = promo("ErrorMessage")
    '                    .Add("@ACTIVATION_MECHANISM", SqlDbType.NVarChar).Value = promo("ActivationMechanism")
    '                    .Add("@APPLICATION_COUNT", SqlDbType.NVarChar).Value = promo("ApplicationCount")
    '                End With
    '                insDel = cmd.ExecuteNonQuery()
    '            End If
    '        Next

    '    Catch ex As Exception
    '        Throw ex
    '    Finally
    '        cmd.Connection.Close()
    '    End Try

    'End Sub

    ' ------- OLD  -------
    '--- Old StorePromotion Results ---
    'Protected Sub StorePromotionResults()

    '    'Todo : instead delete and insert change the status of success to column to false and 
    '    ' update that row if it found in promotionresults table
    '    ' delete the row where success is false

    '    Dim cmd As New SqlCommand("", New SqlConnection(Settings.FrontEndConnectionString))
    '    Const DeleteStr As String = " DELETE FROM  tbl_promotions_redeemed   " & _
    '                                "   WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
    '                                "   AND PARTNER_GROUP = @PARTNER_GROUP " & _
    '                                "   AND PARTNER = @PARTNER " & _
    '                                "   AND TEMP_ORDER_ID = @TEMP_ORDER_ID " & _
    '                                "   AND LOGINID = @LOGINID  " & _
    '                                "    "

    '    Const InsertStr As String = " INSERT INTO tbl_promotions_redeemed  " & _
    '                                        "(TEMP_ORDER_ID, LOGINID, BUSINESS_UNIT, PARTNER_GROUP," & _
    '                                        "PARTNER, PROMOTION_CODE, ACTIVE_FROM_DATE," & _
    '                                        "ACTIVE_TO_DATE, SUCCESS, PROMOTION_DISPLAY_NAME," & _
    '                                        "PROMOTION_VALUE, ERROR_MESSAGE, ACTIVATION_MECHANISM," & _
    '                                        "APPLICATION_COUNT)" & _
    '                                        "    VALUES(@TEMP_ORDER_ID, @LOGINID, @BUSINESS_UNIT, @PARTNER_GROUP, @PARTNER, " & _
    '                                        "           @PROMOTION_CODE, @ACTIVE_FROM_DATE, @ACTIVE_TO_DATE, " & _
    '                                        "           @SUCCESS, @PROMOTION_DISPLAY_NAME, @PROMOTION_VALUE, " & _
    '                                        "           @ERROR_MESSAGE, @ACTIVATION_MECHANISM, @APPLICATION_COUNT ) " & _
    '                                        ""

    '    cmd.CommandText = DeleteStr

    '    Try
    '        cmd.Connection.Open()

    '        With cmd.Parameters
    '            .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = Dep.BusinessUnit
    '            .Add("@PARTNER_GROUP", SqlDbType.NVarChar).Value = Dep.PartnerGroup
    '            .Add("@PARTNER", SqlDbType.NVarChar).Value = Dep.Partner
    '            .Add("@TEMP_ORDER_ID", SqlDbType.NVarChar).Value = Dep.TempOrderID
    '            .Add("@LOGINID", SqlDbType.NVarChar).Value = Dep.Username
    '        End With

    '        cmd.ExecuteNonQuery()
    '        cmd.CommandText = InsertStr

    '        For Each promo As DataRow In PromotionResults.Rows
    '            If CBool(promo("SUCCESS")) Then
    '                With cmd.Parameters
    '                    .Clear()
    '                    .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = Dep.BusinessUnit
    '                    .Add("@PARTNER_GROUP", SqlDbType.NVarChar).Value = Dep.PartnerGroup
    '                    .Add("@PARTNER", SqlDbType.NVarChar).Value = Dep.Partner
    '                    .Add("@TEMP_ORDER_ID", SqlDbType.NVarChar).Value = Dep.TempOrderID
    '                    .Add("@LOGINID", SqlDbType.NVarChar).Value = Dep.Username
    '                    .Add("@PROMOTION_CODE", SqlDbType.NVarChar).Value = promo("PromotionCode")
    '                    .Add("@ACTIVE_FROM_DATE", SqlDbType.DateTime).Value = promo("ActiveFrom")
    '                    .Add("@ACTIVE_TO_DATE", SqlDbType.DateTime).Value = promo("ActiveTo")
    '                    .Add("@SUCCESS", SqlDbType.Bit).Value = promo("Success")
    '                    .Add("@PROMOTION_DISPLAY_NAME", SqlDbType.NVarChar).Value = promo("PromotionDisplayName")
    '                    .Add("@PROMOTION_VALUE", SqlDbType.NVarChar).Value = promo("PromotionValue")
    '                    .Add("@ERROR_MESSAGE", SqlDbType.NVarChar).Value = promo("ErrorMessage")
    '                    .Add("@ACTIVATION_MECHANISM", SqlDbType.NVarChar).Value = promo("ActivationMechanism")
    '                    .Add("@APPLICATION_COUNT", SqlDbType.NVarChar).Value = promo("ApplicationCount")
    '                End With
    '                cmd.ExecuteNonQuery()
    '            End If
    '        Next

    '    Catch ex As Exception
    '    Finally
    '        cmd.Connection.Close()
    '    End Try
    'End Sub
    'Protected Function Do_GetYFree_Action_Sql2005(ByVal promo As DataRow, _
    '                                                ByVal promoHasBeenAppliedBefore As Boolean, _
    '                                                ByVal promoLang As DataTable, _
    '                                                ByVal matchingProducts As Collections.Generic.Dictionary(Of String, Decimal)) As ErrorObj

    '    Dim err As New ErrorObj
    '    Dim allowSelectOption As Boolean = Utilities.CheckForDBNull_Boolean_DefaultFalse(promo("ALLOW_SELECT_OPTION"))
    '    Dim freeProducts As DataTable = GetFreeProducts_Sql2005(Utilities.CheckForDBNull_String(UCase(promo("PROMOTION_CODE"))))
    '    Dim requiredProducts As DataTable = GetRequiredProducts_Sql2005(Utilities.CheckForDBNull_String(promo("PROMOTION_CODE")))

    '    '--------------------------------------------------------------
    '    ' Check special BOGOF case. If there's one required product and 
    '    ' one free product and they are the same, then add the matching
    '    ' product as it may be an OPTION, not a master
    '    '--------------------------------------------------------------
    '    Dim isBogof As Boolean = False
    '    If requiredProducts.Rows.Count = 1 AndAlso freeProducts.Rows.Count = 1 AndAlso _
    '            requiredProducts.Rows(0)("PRODUCT_CODE").ToString = freeProducts.Rows(0)("PRODUCT_CODE").ToString Then
    '        isBogof = True
    '    End If

    '    If freeProducts.Rows.Count > 0 Then
    '        Try

    '            Dim amendBasket As New Talent.Common.DEAmendBasket
    '            With amendBasket
    '                '.AddToBasket = True
    '                .AddFreeItems = True
    '                .BasketId = Dep.BasketHeaderID
    '                .BusinessUnit = Dep.BusinessUnit
    '                .PartnerCode = Dep.Partner
    '                .UserID = Dep.Username
    '                .IsFreeItem = True
    '            End With
    '            Dim deaItem As Talent.Common.DEAlerts

    '            For Each prod As Data.DataRow In freeProducts.Rows
    '                deaItem = New Talent.Common.DEAlerts
    '                If isBogof Then
    '                    For Each key As String In matchingProducts.Keys
    '                        deaItem.ProductCode = key
    '                        deaItem.MasterProduct = key
    '                    Next
    '                Else
    '                    deaItem.ProductCode = UCase(prod("PRODUCT_CODE"))
    '                    deaItem.MasterProduct = UCase(prod("PRODUCT_CODE"))
    '                End If

    '                deaItem.Quantity = prod("QUANTITY")
    '                deaItem.Price = 0
    '                deaItem.Group_L01_Group = Utilities.CheckForDBNull_String(prod("GROUP_L01_GROUP"))
    '                deaItem.Group_L02_Group = Utilities.CheckForDBNull_String(prod("GROUP_L02_GROUP"))
    '                deaItem.Group_L03_Group = Utilities.CheckForDBNull_String(prod("GROUP_L03_GROUP"))
    '                deaItem.Group_L04_Group = Utilities.CheckForDBNull_String(prod("GROUP_L04_GROUP"))
    '                deaItem.Group_L05_Group = Utilities.CheckForDBNull_String(prod("GROUP_L05_GROUP"))
    '                deaItem.Group_L06_Group = Utilities.CheckForDBNull_String(prod("GROUP_L06_GROUP"))
    '                deaItem.Group_L07_Group = Utilities.CheckForDBNull_String(prod("GROUP_L07_GROUP"))
    '                deaItem.Group_L08_Group = Utilities.CheckForDBNull_String(prod("GROUP_L08_GROUP"))
    '                deaItem.Group_L09_Group = Utilities.CheckForDBNull_String(prod("GROUP_L09_GROUP"))
    '                deaItem.Group_L10_Group = Utilities.CheckForDBNull_String(prod("GROUP_L10_GROUP"))
    '                deaItem.Allow_Select_Option = allowSelectOption
    '                amendBasket.CollDEAlerts.Add(deaItem)
    '            Next

    '            Dim DBAmend As New DBAmendBasket
    '            With DBAmend
    '                .Dep = amendBasket
    '                .Settings = Settings
    '                .AccessDatabase()
    '            End With

    '            'The promotion has been successfully applied, but we only
    '            'add the details to the results table if they have not
    '            'already been added
    '            Dim currentResultsRow As DataRow
    '            If promoHasBeenAppliedBefore Then
    '                currentResultsRow = PromotionResults.Select("PromotionCode = '" & promo("PROMOTION_CODE") & "'")(0)
    '                currentResultsRow("ApplicationCount") += 1
    '            Else
    '                currentResultsRow = PromotionResults.NewRow
    '                currentResultsRow("PromotionCode") = Utilities.CheckForDBNull_String(promo("PROMOTION_CODE"))
    '                currentResultsRow("BusinessUnit") = Utilities.CheckForDBNull_String(promo("BUSINESS_UNIT"))
    '                currentResultsRow("Partner") = Utilities.CheckForDBNull_String(promo("PARTNER"))
    '                currentResultsRow("Success") = True
    '                currentResultsRow("PromotionDisplayName") = Utilities.CheckForDBNull_String(promoLang.Rows(0)("DISPLAY_NAME"))
    '                currentResultsRow("PromotionValue") = 0
    '                currentResultsRow("ErrorMessage") = String.Empty
    '                currentResultsRow("ActivationMechanism") = Utilities.CheckForDBNull_String(promo("ACTIVATION_MECHANISM"))
    '                currentResultsRow("ApplicationCount") = 1
    '                currentResultsRow("ActiveFrom") = Utilities.CheckForDBNull_DateTime(promo("START_DATE"))
    '                currentResultsRow("ActiveTo") = Utilities.CheckForDBNull_DateTime(promo("END_DATE"))
    '                currentResultsRow("FreeDelivery") = False
    '                currentResultsRow("PromotionPercentageValue") = 0
    '                PromotionResults.Rows.Add(currentResultsRow)
    '            End If

    '        Catch ex As Exception
    '            err.HasError = True
    '            err.ErrorNumber = "Add Free Items Error"
    '            err.ErrorMessage = ex.Message
    '        End Try
    '    End If

    '    Return err
    'End Function
#End Region

End Class