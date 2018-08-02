<Serializable()> _
Public Class DEBasketPayment

    '*************add column transaction id to basket payment
    '************add column transaction type
    Public Property BASKET_PAYMENT_ID() As Long = 0
    Public Property BASKET_HEADER_ID() As String = String.Empty
    Public Property LOGIN_ID() As String = String.Empty
    Public Property STATUS() As String = String.Empty
    Public Property CHECKOUT_STAGE() As String = String.Empty
    Public Property CAPTUREMETHOD() As String = String.Empty
    Public Property CAN_SAVE_CARD() As Boolean
    Public Property AGENT_NAME() As String = String.Empty
    Public Property PAYMENT_TYPE() As String = String.Empty
    Public Property PAYMENT_AMOUNT() As Decimal
    Public Property CARD_TYPE() As String = String.Empty
    Public Property CARDNUMBER() As String = String.Empty
    Public Property STARTMONTH() As String = String.Empty
    Public Property STARTYEAR() As String = String.Empty
    Public Property EXPIRYMONTH() As String = String.Empty
    Public Property EXPIRYYEAR() As String = String.Empty
    Public Property ISSUENUMBER() As String = String.Empty
    Public Property ADDRESS_LINE_1() As String = String.Empty
    Public Property POST_CODE() As String = String.Empty
    Public Property CARD_HOLDER_NAME() As String = String.Empty
    Public Property TANDC_ACCEPTED() As String = String.Empty
    Public Property TXNType() As String = String.Empty
    ''' <summary>
    ''' Expiry Date is yymm
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property EXPIRYDATE() As String
        Get
            Return EXPIRYYEAR & EXPIRYMONTH
        End Get
    End Property
    Public Property PROCESSING_DB() As String = String.Empty
    Public Property SESSION_GUID() As String = String.Empty
    Public Property SESSION_PASSCODE() As String = String.Empty
    Public Property TRANSACTION_ID() As String = String.Empty
    Public Property TOKENID() As String = String.Empty

    Public Property SAVEDCARD_UNIQUEID() As String = String.Empty

    Public Property BASKET_AMOUNT() As Decimal
    Public Property TEMP_ORDER_ID() As String = String.Empty
    Public Property TICKETING_PAYMENT_REF() As String = String.Empty
    Public Property MERCHANT_REFERENCE() As String = String.Empty
End Class
