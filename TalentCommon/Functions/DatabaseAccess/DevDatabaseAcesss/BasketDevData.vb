Public Class BasketDevData

    Public Shared Sub AddDevDataToBasketHeaderTable(ByRef dt As DataTable)
        Dim row As DataRow = dt.NewRow
        row.Item("MarketingCampaign") = ""
        row.Item("PrintAtHome") = ""
        row.Item("ReservedQuantity") = "5"
        row.Item("ApplyCarriageFee") = "Y"
        row.Item("ApplyBookingFee") = "Y"
        row.Item("ApplyWebSalesFee") = "Y"
        row.Item("ApplyWebServicesFee") = "Y"
        row.Item("CarriageFee") = "1.00"
        row.Item("BookingFee") = "1.00"
        row.Item("WebSalesFee") = "1.00"
        row.Item("WebServicesFee") = "1.00"
        row.Item("TotalPrice") = "10.00"
        dt.Rows.Add(row)
    End Sub

    Public Shared Sub AddDevDataToBasketDetailsTable(ByRef Dt As DataTable)
        Dim row As DataRow = Dt.NewRow
        row.Item("CustomerNo") = ""
        row.Item("ProductCode") = "1234"
        row.Item("PriceCode") = ""
        row.Item("PriceBand") = ""
        row.Item("Seat") = ""
        row.Item("ProductType") = ""
        row.Item("Price") = ""
        row.Item("ProductDescription") = "This is a product description"
        row.Item("StandDescription") = "This is a stand description"
        row.Item("AreaDescription") = "This is an Area description"
        row.Item("BandDescription") = "This is a band description"
        row.Item("ReservedSeat") = ""
        row.Item("PostCollect") = ""
        row.Item("ForceCollect") = ""
        row.Item("ErrorCode") = ""
        row.Item("ProductLimit") = ""
        row.Item("UserLimit") = ""
        row.Item("ErrorInformation") = ""
        row.Item("ProductTime") = ""
        row.Item("ProductOppositionCode") = ""
        row.Item("ProductCompetitionCode") = ""
        row.Item("ProductCompetitionText") = "Product Competition Text"
        row.Item("ProductDate") = ""
        row.Item("SeatRestriction") = ""
        row.Item("IsFree") = "N"
        row.Item("ProductSubType") = ""
        Dt.Rows.Add(row)
    End Sub

End Class
