''' <summary>
''' Holds the voucher details
''' </summary>
<Serializable()> _
Public Class DEVouchers

    Public Property ShowActiveAndInActiveRecords() As Boolean
    Public Property Mode() As Boolean
    Public Property VoucherDefinitionId() As Integer
    Public Property VoucherDescription() As String
    Public Property VoucherType() As String
    Public Property ExpiryDate() As Date
    Public Property ExpiryMonths() As Integer
    Public Property ExpiryDays() As Integer
    Public Property Source() As Boolean

    Public Property VoucherCode() As String
    Public Property CustomerNumber() As String
    Public Property RedeemMode() As RedeemMode
    Public Property ExternalCompany() As String
    Public Property VoucherPrice As Decimal
    Public Property GiftVouchrPrice As Decimal
    Public Property OnAccountTotal As Decimal
    Public Property BoxOfficeUser As String
    Public Property ExternalVoucherCodeFlag() As Boolean
    Public Property AgreementCode() As String
    Public Property UniqueVoucherId() As String
    Public Property VoucherSource() As String
    Public Property RetrieveUsedVoucher As Boolean

End Class

Public Enum RedeemMode
    Redeem
    Convert
    External
    Delete
End Enum