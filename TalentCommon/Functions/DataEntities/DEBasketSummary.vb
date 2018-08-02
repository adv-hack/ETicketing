Public Class DEBasketSummary

    Private _dtBasketSummaryTotals As DataTable = Nothing
    Private _dtBasketSummaryFees As DataTable = Nothing
    Private _basketErrorCode As String = String.Empty
    Private _basketErrorRedirectURL As String = String.Empty ' "~/PagesPublic/Basket/Basket.aspx"
    'product feecategory
    Private _dicNonSystemFeeInDetail As Dictionary(Of String, String) = Nothing

    Public Sub New()
        InitialiseBasketSummaryEntity()
    End Sub

    Private Sub InitialiseBasketSummaryEntity()
        _dtBasketSummaryTotals = New DataTable
        _dtBasketSummaryTotals.Columns.Add("LABEL_CODE", GetType(String))
        _dtBasketSummaryTotals.Columns.Add("SUMMARY_PRODUCT", GetType(String))
        _dtBasketSummaryTotals.Columns.Add("FEE_CATEGORY", GetType(String))
        _dtBasketSummaryTotals.Columns.Add("GROSS_PRICE", GetType(Decimal))
        _dtBasketSummaryTotals.Columns.Add("NET_PRICE", GetType(Decimal))
        _dtBasketSummaryTotals.Columns.Add("TAX", GetType(Decimal))
        _dtBasketSummaryTotals.Columns.Add("IS_SYSTEM_FEE", GetType(Boolean))
        _dtBasketSummaryTotals.Columns.Add("IS_EXTERNAL", GetType(Boolean))
        _dtBasketSummaryTotals.Columns.Add("IS_INCLUDED", GetType(Boolean))
        _dtBasketSummaryTotals.Columns.Add("IS_DISPLAY_TYPE", GetType(Boolean))
        _dtBasketSummaryTotals.Columns.Add("SEQUENCE", GetType(Integer))

        _dtBasketSummaryFees = New DataTable
        _dtBasketSummaryFees.Columns.Add("CARD_TYPE_CODE", GetType(String))
        _dtBasketSummaryFees.Columns.Add("FEE_CODE", GetType(String))
        _dtBasketSummaryFees.Columns.Add("FEE_DESCRIPTION", GetType(String))
        _dtBasketSummaryFees.Columns.Add("FEE_CATEGORY", GetType(String))
        _dtBasketSummaryFees.Columns.Add("FEE_VALUE", GetType(String))
        _dtBasketSummaryFees.Columns.Add("IS_SYSTEM_FEE", GetType(Boolean))
        _dtBasketSummaryFees.Columns.Add("IS_TRANSACTIONAL", GetType(Boolean))
        _dtBasketSummaryFees.Columns.Add("IS_EXISTS_IN_BASKET", GetType(Boolean))

        _dicNonSystemFeeInDetail = New Dictionary(Of String, String)
    End Sub

    Public Property FeesDTCharity As DataTable = Nothing
    Public Property FeesDTAdhoc As DataTable = Nothing
    Public Property FeesDTVariable As DataTable = Nothing
    Public Property FeesDTVariableApplied As DataTable = Nothing
    Public Property FeesDTCardTypeBooking As DataTable = Nothing

    Public ReadOnly Property FeesDICNonSystemInDetail() As Dictionary(Of String, String)
        Get
            Return _dicNonSystemFeeInDetail
        End Get
    End Property

    Public ReadOnly Property SummaryTable As DataTable
        Get
            _dtBasketSummaryTotals.DefaultView.RowFilter = "IS_DISPLAY_TYPE = 1"
            _dtBasketSummaryTotals.DefaultView.Sort = "SEQUENCE ASC"
            Return _dtBasketSummaryTotals.DefaultView.ToTable
        End Get
    End Property

    Public ReadOnly Property SummaryFeesTable As DataTable
        Get
            Return _dtBasketSummaryFees
        End Get
    End Property
    'Basket Error notification to front end
    Public ReadOnly Property BasketErrorCode As String
        Get
            Return _basketErrorCode
        End Get
    End Property

    Public ReadOnly Property BasketErrorRedirectURL As String
        Get
            Return _basketErrorRedirectURL
        End Get
    End Property


    'Merchandise
    Public ReadOnly Property MerchandiseTotal As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.BSKTSMRY_TOTAL_MERCHANDISE)
        End Get
    End Property
    Public ReadOnly Property MerchandiseSubTotal As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.BSKTSMRY_TOTAL_MERCHANDISE_ITEMS_PRICE)
        End Get
    End Property
    Public ReadOnly Property MerchandisePromotionalValue As Decimal
        Get
            Return GetSummaryProductCodeValue("")
        End Get
    End Property
    Public ReadOnly Property MerchandiseDeliveryCharge As Decimal
        Get
            Return GetSummaryProductCodeValue("")
        End Get
    End Property
    Public ReadOnly Property MerchandiseVAT As Decimal
        Get
            Return GetSummaryProductCodeValue("")
        End Get
    End Property
    Public ReadOnly Property MerchandiseTotalItems As Integer
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.BSKTSMRY_TOTAL_ITEMS_MERCHANDISE)
        End Get
    End Property

    'Total Basket
    Public ReadOnly Property TotalBasket As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.BSKTSMRY_TOTAL_BASKET)
        End Get
    End Property

    'Total Basket Without Payment Fee
    Public ReadOnly Property TotalBasketWithoutPayTypeFee As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.BSKTSMRY_TOTAL_BASKET_WO_PAYFEE)
        End Get
    End Property

    'Ticketing
    Public ReadOnly Property TotalTicketing As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.BSKTSMRY_TOTAL_TICKETING)
        End Get
    End Property
    Public ReadOnly Property TotalTicketPrice As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.BSKTSMRY_TOTAL_TICKET_PRICE)
        End Get
    End Property
    Public ReadOnly Property TotalCATTicketPrice As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.BSKTSMRY_TOTAL_CAT_TICKET_PRICE)
        End Get
    End Property
    Public ReadOnly Property TotalTicketingFees As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.BSKTSMRY_TOTAL_TICKET_FEES)
        End Get
    End Property
    Public ReadOnly Property TotalItemsTicketing As Integer
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.BSKTSMRY_TOTAL_ITEMS_TICKETING)
        End Get
    End Property
    Public ReadOnly Property TotalItemsAppliedCharity As Integer
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.BSKTSMRY_TOTAL_ITEMS_APPLIED_CHRTY)
        End Get
    End Property
    Public ReadOnly Property TotalItemsAppliedAdhoc As Integer
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.BSKTSMRY_TOTAL_ITEMS_APPLIED_ADHOC)
        End Get
    End Property
    Public ReadOnly Property TotalItemsAppliedVariable As Integer
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.BSKTSMRY_TOTAL_ITEMS_APPLIED_VRBLE)
        End Get
    End Property
    Public ReadOnly Property TotalBuyBack As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.BBFEE)
        End Get
    End Property

    Public ReadOnly Property TotalOnAccount(ByVal cashBackFeeCode As String) As Decimal
        Get
            Return GetSummaryProductCodeValue(cashBackFeeCode)
        End Get
    End Property

    Public ReadOnly Property TotalPartPayments() As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.BSKTSMRY_TOTAL_PART_PAYMENTS)
        End Get
    End Property

    Public ReadOnly Property TotalPartPaymentsByCreditCard() As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.BSKTSMRY_TOTAL_PART_PAYMENTS_BY_CREDITCARD)
        End Get
    End Property

    Public ReadOnly Property TotalPartPaymentsByOthers() As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.BSKTSMRY_TOTAL_PART_PAYMENTS_BY_OTHERS)
        End Get
    End Property

    Public ReadOnly Property FeesWebSalesTotal As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.FEECATEGORY_WEBSALES)
        End Get
    End Property
    Public ReadOnly Property FeesSupplynetTotal As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.FEECATEGORY_SUPPLYNET)
        End Get
    End Property


    'Payment type
    Public ReadOnly Property FeesBookingActual As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.BSKTSMRY_BOOKING_FEE_ACTUAL)
        End Get
    End Property
    Public ReadOnly Property FeesBookingTotal As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.FEECATEGORY_BOOKING)
        End Get
    End Property
    Public ReadOnly Property FeesDirectDebitTotal As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.FEECATEGORY_DIRECTDEBIT)
        End Get
    End Property
    Public ReadOnly Property FeesFinanceTotal As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.FEECATEGORY_FINANCE)
        End Get
    End Property

    'Applied Non-System Fees Total In Basket
    Public ReadOnly Property FeesCharityTotal As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.FEECATEGORY_CHARITY)
        End Get
    End Property
    Public ReadOnly Property FeesVariableTotal As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.FEECATEGORY_VARIABLE)
        End Get
    End Property
    Public ReadOnly Property FeesAdhocTotal As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.FEECATEGORY_ADHOC)
        End Get
    End Property

    'CAT
    Public ReadOnly Property FeesCancelTotal As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.FEECATEGORY_CANCEL)
        End Get
    End Property
    Public ReadOnly Property FeesAmendTotal As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.FEECATEGORY_AMEND)
        End Get
    End Property
    Public ReadOnly Property FeesTransferTotal As Decimal
        Get
            Return GetSummaryProductCodeValue(GlobalConstants.FEECATEGORY_TRANSFER)
        End Get
    End Property

    Friend Function RemoveAllPayTypeFromBasketSummary() As Boolean
        Dim isRemoved As Boolean = False
        Try
            If _dtBasketSummaryTotals IsNot Nothing AndAlso _dtBasketSummaryTotals.Rows.Count > 0 Then
                Dim filter As String = "'" & GlobalConstants.FEECATEGORY_BOOKING & "'"
                filter = filter & ",'" & GlobalConstants.FEECATEGORY_DIRECTDEBIT & "'"
                filter = filter & ",'" & GlobalConstants.FEECATEGORY_FINANCE & "'"
                _dtBasketSummaryTotals.Delete("FEE_CATEGORY IN (" & filter & ")")
                _dtBasketSummaryTotals.AcceptChanges()
                isRemoved = True
            End If
        Catch ex As Exception
            isRemoved = False
            'log the exception
        End Try
        Return isRemoved
    End Function
    Friend Sub AddToBasketSummary(ByVal labelCode As String, ByVal summaryProductCode As String, ByVal feeCategory As String, ByVal grossPrice As Decimal, ByVal netPrice As Decimal, ByVal taxValue As Decimal, ByVal isSystemFee As Boolean, ByVal isExternal As Boolean, ByVal isIncluded As Boolean, ByVal isDisplayType As Boolean, ByVal sequence As Integer)
        Dim drBasketSummary As DataRow = _dtBasketSummaryTotals.NewRow
        drBasketSummary("LABEL_CODE") = labelCode
        drBasketSummary("SUMMARY_PRODUCT") = summaryProductCode
        drBasketSummary("FEE_CATEGORY") = feeCategory
        drBasketSummary("GROSS_PRICE") = grossPrice
        drBasketSummary("NET_PRICE") = netPrice
        drBasketSummary("TAX") = taxValue
        drBasketSummary("IS_SYSTEM_FEE") = isSystemFee
        drBasketSummary("IS_EXTERNAL") = isExternal
        drBasketSummary("IS_INCLUDED") = isIncluded
        drBasketSummary("IS_DISPLAY_TYPE") = isDisplayType
        drBasketSummary("SEQUENCE") = sequence
        _dtBasketSummaryTotals.Rows.Add(drBasketSummary)
    End Sub

    Friend Sub AddToBasketSummaryFees(ByVal cardTypeCode As String, ByVal feeCode As String, ByVal feeDescription As String, ByVal feeCategory As String, ByVal feeValue As Decimal, ByVal isSystemFee As Boolean, ByVal isTransactional As Boolean, ByVal isExistsInBasket As Boolean)
        Dim drBasketSummaryFees As DataRow = _dtBasketSummaryFees.NewRow
        drBasketSummaryFees("CARD_TYPE_CODE") = cardTypeCode
        drBasketSummaryFees("FEE_CODE") = feeCode
        drBasketSummaryFees("FEE_DESCRIPTION") = feeDescription
        drBasketSummaryFees("FEE_CATEGORY") = feeCategory
        drBasketSummaryFees("FEE_VALUE") = feeValue
        drBasketSummaryFees("IS_SYSTEM_FEE") = isSystemFee
        drBasketSummaryFees("IS_TRANSACTIONAL") = isTransactional
        drBasketSummaryFees("IS_EXISTS_IN_BASKET") = isExistsInBasket
        _dtBasketSummaryFees.Rows.Add(drBasketSummaryFees)
    End Sub

    Friend Sub AddToNonSystemFeeInDetail(ByVal product As String, ByVal feeCategory As String)
        If Not _dicNonSystemFeeInDetail.ContainsKey(product) Then
            _dicNonSystemFeeInDetail.Add(product, feeCategory)
        End If
    End Sub

    Friend Sub SetBasketErrorDetails(ByVal errorCode As String, ByVal errorRedirectURL As String)
        _basketErrorCode = errorCode
        _basketErrorRedirectURL = errorRedirectURL
    End Sub

    Private Function GetSummaryProductCodeValue(ByVal summaryProductCode As String) As Decimal
        Dim codeValue As Decimal = 0
        Try
            If _dtBasketSummaryTotals IsNot Nothing AndAlso _dtBasketSummaryTotals.Rows.Count > 0 Then
                For rowIndex As Integer = 0 To _dtBasketSummaryTotals.Rows.Count - 1
                    If summaryProductCode = Utilities.CheckForDBNull_String(_dtBasketSummaryTotals.Rows(rowIndex)("LABEL_CODE")) Then
                        codeValue = Utilities.CheckForDBNull_Decimal(_dtBasketSummaryTotals.Rows(rowIndex)("GROSS_PRICE"))
                        Exit For
                    End If
                Next
            End If
        Catch ex As Exception
            codeValue = 0
        End Try

        Return codeValue
    End Function

End Class


Public Class DEBasketSummaryDisplaySequence
    Public Property SEQ_DEFAULT() As Integer = 10000
    ' 0 - 7000 ticketing summary
    Public Property SEQ_TOTAL_TICKET_PRICE() As Integer = 10000
    Public Property SEQ_TOTAL_TICKET_DISCOUNT() As Integer = 10000
    Public Property SEQ_TOTAL_CAT_TICKET_PRICE() As Integer = 10000
    Public Property SEQ_TOTAL_PART_PAYMENTS() As Integer = 10000
    Public Property SEQ_ONACCOUNT() As Integer = 10000
    Public Property SEQ_BUYBACK() As Integer = 10000
    Public Property SEQ_FEES_DEFAULT() As Integer = 10000
    Public Property SEQ_BOOKING() As Integer = 10000
    Public Property SEQ_DIRECTDEBIT() As Integer = 10000
    Public Property SEQ_FINANCE() As Integer = 10000
    Public Property SEQ_TOTAL_CHARITY() As Integer = 10000
    Public Property SEQ_TOTAL_ADHOC() As Integer = 10000
    Public Property SEQ_TOTAL_VARIABLE() As Integer = 10000
    Public Property SEQ_WEBSALES() As Integer = 10000
    ' 7020 - 9000 merchandise summary
    Public Property SEQ_TOTAL_MERCHANDISE_ITEMS() As Integer = 10000
    Public Property SEQ_TOTAL_MERCHANDISE_PROMOTIONS() As Integer = 10000
    Public Property SEQ_TOTAL_MERCHANDISE_DELIVERY() As Integer = 10000
    Public Property SEQ_TOTAL_MERCHANDISE_VAT() As Integer = 10000
    Public Property SEQ_TOTAL_MERCHANDISE() As Integer = 10000
End Class
'todo
'buybackreward
'onaccount
'cashback
'do we need the show properties
