<Serializable()> _
Public Class DEBasketItem

    Public Property Basket_Detail_ID() As String
    Public Property Product() As String
    Public Property Quantity() As Decimal
    Public Property Gross_Price() As Decimal
    Public Property GROUP_LEVEL_01() As String
    Public Property GROUP_LEVEL_02() As String
    Public Property GROUP_LEVEL_03() As String
    Public Property GROUP_LEVEL_04() As String
    Public Property GROUP_LEVEL_05() As String
    Public Property GROUP_LEVEL_06() As String
    Public Property GROUP_LEVEL_07() As String
    Public Property GROUP_LEVEL_08() As String
    Public Property GROUP_LEVEL_09() As String
    Public Property GROUP_LEVEL_10() As String
    Public Property STOCK_ERROR() As Boolean
    Public Property IS_FREE() As Boolean
    Public Property MODULE_() As String '"Module" is a KEYWORD in vb.net so the property name could not be called the same as the DB field.
    Public Property SEAT() As String
    Public Property RESERVED_SEAT() As String
    Public Property PRICE_BAND() As String
    Public Property PRICE_CODE() As String
    Public Property PRODUCT_TYPE() As String
    Public Property PRODUCT_DESCRIPTION1() As String
    Public Property PRODUCT_DESCRIPTION2() As String
    Public Property PRODUCT_DESCRIPTION3() As String
    Public Property PRODUCT_DESCRIPTION4() As String
    Public Property PRODUCT_DESCRIPTION5() As String
    Public Property PRODUCT_DESCRIPTION6() As String
    Public Property PRODUCT_DESCRIPTION7() As String
    Public Property LOGINID() As String
    Public Property Size() As String
    Public Property WEIGHT() As Decimal
    Public Property QUANTITY_AVAILABLE() As Decimal
    Public Property MASTER_PRODUCT() As String
    Public Property STOCK_ERROR_CODE() As String
    Public Property STOCK_LIMIT() As String
    Public Property STOCK_REQUESTED() As String
    Public Property STOCK_ERROR_DESCRIPTION() As String
    Public Property PRODUCT_SUB_TYPE() As String
    Public Property ALTERNATE_SKU() As String
    Public Property Net_Price() As Decimal
    Public Property Tax_Price() As Decimal
    Public Property RESTRICTION_CODE() As String
    Public Property Xml_Config() As String
    Public Property Cost_Centre() As String
    Public Property Account_Code() As String
    Public Property FULFIL_OPT_POST() As String
    Public Property FULFIL_OPT_COLL() As String
    Public Property FULFIL_OPT_PAH() As String
    Public Property FULFIL_OPT_PRINT() As String
    Public Property FULFIL_OPT_UPL() As String
    Public Property CURR_FULFIL_SLCTN() As String
    Public Property PACKAGE_ID() As Decimal
    Public Property ALLOW_SELECT_OPTION() As Boolean
    Public Property OPTION_SELECTED() As Boolean
    Public Property TRAVEL_PRODUCT_SELECTED() As Boolean
    Public Property CAN_SAVE_AS_FAVOURITE_SEAT() As Boolean
    Public Property ORIGINAL_LOGINID() As String
    Public Property FULFIL_OPT_REGPOST() As String
    Public Property ORIGINAL_PRICE() As String
    Public Property CAT_SEAT_DETAILS() As String
    Public Property VALID_PRICE_BANDS() As String
    Public Property LINKED_PRODUCT_ID() As Integer
    Public Property IS_SYSTEM_FEE() As Boolean
    Public Property IS_EXTERNAL() As Boolean
    Public Property IS_INCLUDED() As Boolean
    Public Property IS_TRANSACTIONAL() As Boolean
    Public Property FEE_CATEGORY() As String
    Public Property PRODUCT_TYPE_ACTUAL() As String
    Public Property CUSTOMER_ALLOCATION() As String
    Public Property VOUCHER_DEFINITION_ID() As Integer
    Public Property VOUCHER_CODE() As String
    Public Property ROVING() As String
    Public Property CANNOT_APPLY_FEES() As Boolean
    Public Property CAT_QUANTITY() As Integer
    Public Property CAT_FULFILMENT() As String
    Public Property FINANCE_CLUB_PRODUCT_ID() As String
    Public Property FINANCE_PLAN_ID() As String
    Public Property BULK_SALES_ID() As Integer
    Public Property PACKAGE_TYPE() As String
    Public Property ALLOCATED_SEAT() As String
    Public Property RESERVATION_CODE() As String
    Public Property RESTRICTED_BASKET_OPTIONS() As Boolean
    Public Property DISPLAY_IN_A_CANCEL_BASKET() As Boolean
    Public Property CALL_ID() As Long

End Class

Public Class DEBasketMergedHeader
    Public Property TotalBasket() As Decimal = 0
    Public Property TotalBasketRetailOnly() As Decimal = 0
    Public Property TotalBasketTicketsOnly() As Decimal = 0

End Class

Public Class DEBasketMergedDetail

    Public Property Product() As String = String.Empty
    Public Property Product_Type_Actual() As String = String.Empty
    Public Property Quantity() As Integer
    Public Property Quantity_After_CAT() As Integer
    Public Property Curr_Fulfil_slctn() As String = String.Empty
    Public Property CAT_Fulfilment() As String = String.Empty
    Public Property Price() As Decimal = 0
    Public Property Price_After_CAT() As Decimal = 0
    Public Property ModuleOfItem() As String = String.Empty
    Public Property IsProcessedForBookingFee() As Boolean = False
    Public Property FeeCategory() As String = String.Empty


    Public Sub New(ByVal basketItem As DEBasketItem, ByVal basket As DEBasket)
        Product = basketItem.Product
        Product_Type_Actual = basketItem.PRODUCT_TYPE_ACTUAL
        Quantity = basketItem.Quantity
        Curr_Fulfil_slctn = basketItem.CURR_FULFIL_SLCTN
        Quantity_After_CAT = basketItem.Quantity - basketItem.CAT_QUANTITY
        If Quantity_After_CAT < 0 Then Quantity_After_CAT = 0
        CAT_Fulfilment = basketItem.CAT_FULFILMENT
        Price = basketItem.Gross_Price
        Price_After_CAT = basketItem.Gross_Price - basket.CAT_PRICE
        If Price_After_CAT < 0 Then Price_After_CAT = 0
        ModuleOfItem = basketItem.MODULE_
        FeeCategory = basketItem.FEE_CATEGORY
    End Sub

End Class
