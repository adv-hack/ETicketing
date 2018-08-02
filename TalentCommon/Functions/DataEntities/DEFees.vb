
<Serializable()> _
Public MustInherit Class DEFeesBase
    Public Property FeeCode() As String
    Public Property FeeDescription() As String
    Public Property FeeDepartment() As String
    Public Property FeeType() As String
    Public Property FeeCategory() As String
    Public Property FeeValue() As Decimal
    Public Property CardType() As String
    Public Property IsNegativeFee() As Boolean
    Public Property IsSystemFee() As Boolean
    Public Property IsTransactional() As Boolean
    Public Property IsChargeable() As Boolean = False
    Public Property FeeFunction() As String
    Public Property ApplyFeeTo() As String
    Public Property ChargeType() As String
    Public Property GeographicalZone() As String
End Class

<Serializable()> _
Public Class DEFeesPartPayment
    Inherits DEFeesBase

    Public Property ProductCode() As String
    Public Property ProductType() As String
    Public Property ApplyWebSales() As Boolean
    Public Property FeeValueActual() As Decimal
End Class

<Serializable()> _
Public Class DEFeesPayment
    Inherits DEFeesBase
    Public Property IsExternal() As Boolean = False
End Class

<Serializable()> _
Public Class DEFeesRelations
    Inherits DEFeesBase
    Public Property FeeApplyType As Integer = 0
    Public Property ApplyRelatedCode As String = ""
    Public Sub New(ByVal feeCode As String, ByVal feeCategory As String, ByVal feeApplyType As Integer, ByVal applyRelatedCode As String)
        Me.FeeCode = feeCode
        Me.FeeCategory = feeCategory
        Me.FeeApplyType = feeApplyType
        Me.ApplyRelatedCode = applyRelatedCode
    End Sub
End Class

<Serializable()> _
Public Class DEBasketFees
    Inherits DEFeesBase
    Public Property BasketHeaderID As String = String.Empty
    Public Property FeeApplyType As Integer = 0
    Public Property ExistsInBasket() As Boolean = False
End Class

<Serializable()> _
Public Class DEFees
    Inherits DEFeesBase

    Public Property ProductCode() As String
    Public Property ProductType() As String
    Public Property ApplyWebSales() As Boolean

    ' ''' <summary>
    ' ''' Gets or sets a value indicating whether [process helper flag]. helper flag for fees calculation in talentbasketsummary
    ' ''' </summary>
    ' ''' <value>
    ' '''   <c>true</c> if [process helper flag]; otherwise, <c>false</c>.
    ' ''' </value>
    'Public Property ProcessHelperFlag() As Boolean = True

    'Public Sub New()
    '    ProcessHelperFlag = True
    'End Sub

    Public Function Equal(ByVal feeEntityToCompare As DEFees) As Boolean
        Dim isEqual As Boolean = False
        Select Case Me.FeeCategory
            Case GlobalConstants.FEECATEGORY_CHARITY
                isEqual = IsEqualFeeCharity(feeEntityToCompare)
            Case GlobalConstants.FEECATEGORY_VARIABLE
                isEqual = IsEqualFeeVariable(feeEntityToCompare)
            Case Else
                isEqual = IsEqualFeeOthers(feeEntityToCompare)
        End Select
        Return isEqual
    End Function

    Private Function IsEqualFeeCharity(ByVal feeEntityToCompare As DEFees)
        Dim isEqual As Boolean = False
        isEqual = (Me.FeeCode = feeEntityToCompare.FeeCode)
        Return isEqual
    End Function

    Private Function IsEqualFeeVariable(ByVal feeEntityToCompare As DEFees)
        Dim isEqual As Boolean = False
        isEqual = (Me.FeeCode = feeEntityToCompare.FeeCode)
        Return isEqual
    End Function

    Private Function IsEqualFeeOthers(ByVal feeEntityToCompare As DEFees)
        Dim isEqual As Boolean = False
        If Me.FeeCode = feeEntityToCompare.FeeCode _
            AndAlso Me.FeeCategory = feeEntityToCompare.FeeCategory _
            AndAlso Me.FeeDescription = feeEntityToCompare.FeeDescription _
            AndAlso Me.FeeDepartment = feeEntityToCompare.FeeDepartment _
            AndAlso Me.FeeType = feeEntityToCompare.FeeType _
            AndAlso Me.FeeValue = feeEntityToCompare.FeeValue _
            AndAlso Me.CardType = feeEntityToCompare.CardType _
            AndAlso Me.ProductCode = feeEntityToCompare.ProductCode _
            AndAlso Me.ProductType = feeEntityToCompare.ProductType _
            AndAlso Me.ApplyWebSales = feeEntityToCompare.ApplyWebSales _
            AndAlso Me.IsNegativeFee = feeEntityToCompare.IsNegativeFee _
            AndAlso Me.IsSystemFee = feeEntityToCompare.IsSystemFee _
            AndAlso Me.FeeFunction = feeEntityToCompare.FeeFunction _
            AndAlso Me.ApplyFeeTo = feeEntityToCompare.ApplyFeeTo _
            AndAlso Me.ChargeType = feeEntityToCompare.ChargeType _
            AndAlso Me.GeographicalZone = feeEntityToCompare.GeographicalZone Then
            isEqual = True
        End If
        Return isEqual
    End Function


End Class
