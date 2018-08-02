Imports System.Data
Imports System.Web
Imports System.Text
Imports Talent.Common.Utilities

<Serializable()> _
Public Class TalentPartPayment
    Inherits TalentBase

    Private _isExternalCall As Boolean = False

    Public Property BasketHeaderID() As String = String.Empty

    Public Property BasketTotalWithoutPayTypeFee() As Decimal = 0

    Public Property CardTypeCode() As String = String.Empty

    Public Property canProcessBookingFees() As Boolean = False

    Public Property CurrentPaymentType() As String = String.Empty

    Public Sub ProcessPartPaymentBookingFees()
        If String.IsNullOrWhiteSpace(CardTypeCode) Then
            CardTypeCode = TDataObjects.PaymentSettings.TblCreditCardBu.GetDefaultCardTypeCode()
        End If
        If IsBookingFeesPercentageBased(Settings, CardTypeFeeCategory, FulfilmentFeeCategory) Then
            canProcessBookingFees = True
        Else
            ProcessPartPaymentFeesFixed()
        End If
    End Sub

    Public Function GetFeeValueForCurrentPartPayment(ByVal feeValue As Decimal) As Decimal
        If Not String.IsNullOrWhiteSpace(BasketHeaderID) Then
            Dim bookingFeeFlagCode As String = TDataObjects.FeesSettings.TblFeesRelations.GetPartPaymentFlag(GlobalConstants.FEECATEGORY_BOOKING, Settings.BusinessUnit)
            If bookingFeeFlagCode = GlobalConstants.FEE_PARTPAYMENTFLAG_FIRST_ONLY Then
                feeValue = GetFeeValueForFirstOnly(feeValue)
            ElseIf bookingFeeFlagCode = GlobalConstants.FEE_PARTPAYMENTFLAG_HIGHEST_ONLY Then
                feeValue = GetFeeValueForHighestOnly(feeValue)
            Else
                feeValue = GetFeeValueForChargeAll(feeValue)
            End If
        End If
        Return feeValue
    End Function

    Public Function GetPartPaymentFlag() As String
        Dim bookingFeeFlagCode As String = TDataObjects.FeesSettings.TblFeesRelations.GetPartPaymentFlag(GlobalConstants.FEECATEGORY_BOOKING, Settings.BusinessUnit)
        If IsBookingFeesPercentageBased(Settings, CardTypeFeeCategory, FulfilmentFeeCategory) Then
            bookingFeeFlagCode = GlobalConstants.FEE_PARTPAYMENTFLAG_CHARGE_ALL
        End If
        Return bookingFeeFlagCode
    End Function

    Private Sub ProcessPartPaymentFeesFixed()
        If Not String.IsNullOrWhiteSpace(BasketHeaderID) Then
            Dim bookingFeeFlagCode As String = TDataObjects.FeesSettings.TblFeesRelations.GetPartPaymentFlag(GlobalConstants.FEECATEGORY_BOOKING, Settings.BusinessUnit)
            If Not String.IsNullOrWhiteSpace(bookingFeeFlagCode) Then
                If bookingFeeFlagCode = GlobalConstants.FEE_PARTPAYMENTFLAG_CHARGE_ALL Then
                    ProcessPartPaymentChargeAll()
                ElseIf bookingFeeFlagCode = GlobalConstants.FEE_PARTPAYMENTFLAG_HIGHEST_ONLY Then
                    ProcessPartPaymentHighestOnly()
                ElseIf bookingFeeFlagCode = GlobalConstants.FEE_PARTPAYMENTFLAG_FIRST_ONLY Then
                    ProcessPartPaymentFirstOnly()
                End If
            End If
        End If
    End Sub

    Private Sub ProcessPartPaymentChargeAll()

        Dim dtPartPayments As DataTable = RetrievePartPayments()

        Dim chargedBookingFeeValue As Decimal = 0
        If dtPartPayments.Rows.Count > 0 Then
            For rowIndex As Integer = 0 To dtPartPayments.Rows.Count - 1
                If IsValidPartPaymentForFee(Utilities.CheckForDBNull_String(dtPartPayments.Rows(rowIndex)("PaymentMethod"))) Then
                    chargedBookingFeeValue += Utilities.CheckForDBNull_Decimal(dtPartPayments.Rows(rowIndex)("FeeValue"))
                End If
            Next
            If chargedBookingFeeValue <> 0 Then
                Dim bookingFeeInBasketDetail As Decimal = 0
                Dim basketFeeEntity As New DEBasketFees
                basketFeeEntity.BasketHeaderID = BasketHeaderID
                basketFeeEntity.FeeCategory = GlobalConstants.FEECATEGORY_BOOKING
                basketFeeEntity.FeeCode = GlobalConstants.BKFEE
                basketFeeEntity.IsSystemFee = True
                If TDataObjects.BasketSettings.TblBasketDetail.TryGetFeeValue(bookingFeeInBasketDetail, BasketHeaderID, GlobalConstants.BKFEE, GlobalConstants.FEECATEGORY_BOOKING) Then
                    'update the bkfee in basket detail
                    bookingFeeInBasketDetail += chargedBookingFeeValue
                    basketFeeEntity.FeeValue = bookingFeeInBasketDetail
                    TDataObjects.BasketSettings.TblBasketDetail.UpdateFeeValue(BasketHeaderID, basketFeeEntity)
                Else
                    'insert the bkfee in basket detail
                    basketFeeEntity.FeeValue = chargedBookingFeeValue
                    TDataObjects.BasketSettings.TblBasketDetail.InsertFee(BasketHeaderID, Settings.LoginId, basketFeeEntity, False)
                End If
            End If
        End If
    End Sub

    Private Sub ProcessPartPaymentHighestOnly()
        Dim dtPartPayments As DataTable = RetrievePartPayments()
        Dim highestFeeValue As Decimal = 0
        Dim chargedBookingFeeValue As Decimal = 0
        If dtPartPayments.Rows.Count > 0 Then
            For rowIndex As Integer = 0 To dtPartPayments.Rows.Count - 1
                If IsValidPartPaymentForFee(Utilities.CheckForDBNull_String(dtPartPayments.Rows(rowIndex)("PaymentMethod"))) Then
                    If Utilities.CheckForDBNull_Decimal(dtPartPayments.Rows(rowIndex)("FeeValueActual")) > highestFeeValue Then
                        highestFeeValue = Utilities.CheckForDBNull_Decimal(dtPartPayments.Rows(rowIndex)("FeeValueActual"))
                    End If
                    chargedBookingFeeValue += Utilities.CheckForDBNull_Decimal(dtPartPayments.Rows(rowIndex)("FeeValue"))
                End If
            Next
            If chargedBookingFeeValue <> 0 Then
                Dim bookingFeeInBasketDetail As Decimal = 0
                Dim basketFeeEntity As New DEBasketFees
                basketFeeEntity.BasketHeaderID = BasketHeaderID
                basketFeeEntity.FeeCategory = GlobalConstants.FEECATEGORY_BOOKING
                basketFeeEntity.FeeCode = GlobalConstants.BKFEE
                basketFeeEntity.IsSystemFee = True
                If TDataObjects.BasketSettings.TblBasketDetail.TryGetFeeValue(bookingFeeInBasketDetail, BasketHeaderID, GlobalConstants.BKFEE, GlobalConstants.FEECATEGORY_BOOKING) Then
                    If bookingFeeInBasketDetail < highestFeeValue Then
                        basketFeeEntity.FeeValue = highestFeeValue
                        TDataObjects.BasketSettings.TblBasketDetail.UpdateFeeValue(BasketHeaderID, basketFeeEntity)
                    End If
                Else
                    'insert the bkfee in basket detail
                    basketFeeEntity.FeeValue = chargedBookingFeeValue
                    TDataObjects.BasketSettings.TblBasketDetail.InsertFee(BasketHeaderID, Settings.LoginId, basketFeeEntity, False)
                End If
            End If
        End If
    End Sub

    Private Sub ProcessPartPaymentFirstOnly()
        Dim dtPartPayments As DataTable = RetrievePartPayments()
        Dim chargedBookingFeeValue As Decimal = 0
        If dtPartPayments.Rows.Count > 0 Then
            For rowIndex As Integer = 0 To dtPartPayments.Rows.Count - 1
                If IsValidPartPaymentForFee(Utilities.CheckForDBNull_String(dtPartPayments.Rows(rowIndex)("PaymentMethod"))) Then
                    chargedBookingFeeValue += Utilities.CheckForDBNull_Decimal(dtPartPayments.Rows(rowIndex)("FeeValue"))
                End If
            Next
            If chargedBookingFeeValue <> 0 Then
                Dim bookingFeeInBasketDetail As Decimal = 0
                Dim basketFeeEntity As New DEBasketFees
                basketFeeEntity.BasketHeaderID = BasketHeaderID
                basketFeeEntity.FeeCategory = GlobalConstants.FEECATEGORY_BOOKING
                basketFeeEntity.FeeCode = GlobalConstants.BKFEE
                basketFeeEntity.IsSystemFee = True
                If TDataObjects.BasketSettings.TblBasketDetail.TryGetFeeValue(bookingFeeInBasketDetail, BasketHeaderID, GlobalConstants.BKFEE, GlobalConstants.FEECATEGORY_BOOKING) Then
                    'update the bkfee in basket detail
                    basketFeeEntity.FeeValue = chargedBookingFeeValue
                    TDataObjects.BasketSettings.TblBasketDetail.UpdateFeeValue(BasketHeaderID, basketFeeEntity)
                Else
                    'insert the bkfee in basket detail
                    basketFeeEntity.FeeValue = chargedBookingFeeValue
                    TDataObjects.BasketSettings.TblBasketDetail.InsertFee(BasketHeaderID, Settings.LoginId, basketFeeEntity, False)
                End If
            End If
        End If
    End Sub

    Private Function GetFeeValueForFirstOnly(ByVal feeValue As Decimal) As Decimal
        If Not String.IsNullOrWhiteSpace(BasketHeaderID) Then
            Dim dtPartPayments As DataTable = RetrievePartPayments()
            Dim chargedBookingFeeValue As Decimal = 0
            If dtPartPayments.Rows.Count > 0 Then
                For rowIndex As Integer = 0 To dtPartPayments.Rows.Count - 1
                    If IsValidPartPaymentForFee(Utilities.CheckForDBNull_String(dtPartPayments.Rows(rowIndex)("PaymentMethod"))) Then
                        chargedBookingFeeValue += Utilities.CheckForDBNull_Decimal(dtPartPayments.Rows(rowIndex)("FeeValue"))
                    End If
                Next
                If chargedBookingFeeValue <> 0 Then
                    Dim bookingFeeInBasketDetail As Decimal = 0
                    Dim basketFeeEntity As New DEBasketFees
                    basketFeeEntity.BasketHeaderID = BasketHeaderID
                    basketFeeEntity.FeeCategory = GlobalConstants.FEECATEGORY_BOOKING
                    basketFeeEntity.FeeCode = GlobalConstants.BKFEE
                    basketFeeEntity.IsSystemFee = True
                    If TDataObjects.BasketSettings.TblBasketDetail.TryGetFeeValue(bookingFeeInBasketDetail, BasketHeaderID, GlobalConstants.BKFEE, GlobalConstants.FEECATEGORY_BOOKING) Then
                        If bookingFeeInBasketDetail = feeValue Then
                            feeValue = 0
                        End If
                    End If
                End If
            End If
        End If
        Return feeValue
    End Function

    Private Function GetFeeValueForChargeAll(ByVal feeValue As Decimal) As Decimal
        If Not String.IsNullOrWhiteSpace(BasketHeaderID) Then
            Dim dtPartPayments As DataTable = RetrievePartPayments()
            Dim chargedBookingFeeValue As Decimal = 0
            If dtPartPayments.Rows.Count > 0 Then
                For rowIndex As Integer = 0 To dtPartPayments.Rows.Count - 1
                    If IsValidPartPaymentForFee(Utilities.CheckForDBNull_String(dtPartPayments.Rows(rowIndex)("PaymentMethod"))) Then
                        chargedBookingFeeValue += Utilities.CheckForDBNull_Decimal(dtPartPayments.Rows(rowIndex)("FeeValue"))
                    End If
                Next
                feeValue = (feeValue - chargedBookingFeeValue)
            End If
        End If
        Return feeValue
    End Function

    Private Function GetFeeValueForHighestOnly(ByVal feeValue As Decimal) As Decimal
        If Not String.IsNullOrWhiteSpace(BasketHeaderID) Then
            Dim dtPartPayments As DataTable = RetrievePartPayments()
            Dim chargedBookingFeeValue As Decimal = 0
            Dim highestFeeValue As Decimal = 0
            If dtPartPayments.Rows.Count > 0 Then
                For rowIndex As Integer = 0 To dtPartPayments.Rows.Count - 1
                    If IsValidPartPaymentForFee(Utilities.CheckForDBNull_String(dtPartPayments.Rows(rowIndex)("PaymentMethod"))) Then
                        If Utilities.CheckForDBNull_Decimal(dtPartPayments.Rows(rowIndex)("FeeValueActual")) > highestFeeValue Then
                            highestFeeValue = Utilities.CheckForDBNull_Decimal(dtPartPayments.Rows(rowIndex)("FeeValueActual"))
                        End If
                        chargedBookingFeeValue += Utilities.CheckForDBNull_Decimal(dtPartPayments.Rows(rowIndex)("FeeValue"))
                    End If
                Next
                If feeValue > highestFeeValue Then
                    feeValue = (feeValue - chargedBookingFeeValue)
                Else
                    feeValue = 0
                End If
            End If
        End If
        Return feeValue
    End Function

    Private Function RetrievePartPayments() As DataTable
        Dim dtPartPayments As New DataTable
        Dim err As New Talent.Common.ErrorObj
        Dim talPayment As New Talent.Common.TalentPayment
        Dim paymentEntity As New Talent.Common.DEPayments
        talPayment.Settings = Settings
        With paymentEntity
            .SessionId = BasketHeaderID
            .CustomerNumber = Settings.LoginId
        End With
        talPayment.De = paymentEntity
        err = talPayment.RetrievePartPayments

        If Not err.HasError AndAlso _
            Not talPayment.ResultDataSet Is Nothing AndAlso _
            talPayment.ResultDataSet.Tables.Count = 2 AndAlso _
            talPayment.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") <> GlobalConstants.ERRORFLAG Then
            dtPartPayments = talPayment.ResultDataSet.Tables("PartPayments")
        End If
        Return dtPartPayments
    End Function

    Private Function IsValidPartPaymentForFee(ByVal paymentType As String) As Boolean
        Dim isValid As Boolean = False
        If Not String.IsNullOrWhiteSpace(paymentType) AndAlso
            (paymentType = GlobalConstants.CCPAYMENTTYPE OrElse
            paymentType = GlobalConstants.SAVEDCARDPAYMENTTYPE OrElse
            paymentType = GlobalConstants.CHIPANDPINPAYMENTTYPE) Then
            isValid = True
        End If
        Return isValid
    End Function

End Class
