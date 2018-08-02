<Serializable()> _
Public Class DEProductDetails

#Region "Public Properties"

    Public Property ProductDate() As String = String.Empty
    Public Property CATMode As String = String.Empty
    Public Property GetLowPrices As Boolean = False
    Public Property PPSType() As String = String.Empty
    Public Property SessionId() As String = String.Empty
    Public Property ProductCode() As String = String.Empty
    Public Property CampaignCode() As String = String.Empty
    Public Property StadiumCode() As String = String.Empty
    Public Property ProductType() As String = String.Empty
    Public Property StandCode() As String = String.Empty
    Public Property AreaCode() As String = String.Empty
    Public Property Src() As String = String.Empty
    Public Property CapacityByStadium() As Boolean = False
    Public Property CustomerNumber() As String = String.Empty
    Public Property WaitList() As Boolean = False
    Public Property PriceCode() As String = String.Empty
    Public Property ProductHomeAsAway() As String = String.Empty
    Public Property AllowPriceException() As Boolean = False
    Public Property SeatRow() As String
    Public Property SeatNumber() As String
    Public Property PriceAndAreaSelection() As Boolean
    Public Property AvailableToSellAvailableTickets() As Boolean
    Public Property AvailableToSell03() As Boolean
    Public Property FullSeatDetails() As DESeatDetails
    Public Property PaymentReference() As String
    Public Property TemplateID() As Integer
    Public Property TemplateDescription() As String
    Public Property TemplateMode() As String
    Public Property TemplateIsActive() As Boolean
    Public Property ComponentID() As Long
    Public Property ProductSupertype() As String
    Public Property ProductSubtype() As String
    Public Property ProductRelationsID() As Integer
    Public Property LinkedProductPackageMode() As String = String.Empty
    Public Property MasterPackageProduct() As String = String.Empty
    Public Property RelatedProduct() As String = String.Empty
    Public Property PriceBand() As String
    Public Property Quantity() As Integer
    Public Property ProductDetailID() As String = String.Empty
    Public Property MasterPriceCode() As String = String.Empty
    Public Property MasterCampaignCode() As String = String.Empty
    Public Property NumOfMasterProducts() As Integer
    Public Property PackageComponentValue1() As Decimal
    Public Property PackageComponentValue2() As Decimal
    Public Property PackageComponentValue3() As Decimal
    Public Property PackageComponentValue4() As Decimal
    Public Property PackageComponentValue5() As Decimal
    Public Property PackageComponentPriceBands() As String
    Public Property PriceBreakId() As Long = 0
    Public Property PriceBreakReverseRow() As Boolean
    Public Property IncludeTicketExchangeSeats() As Boolean = False
    Public Property ProductDescription() As String
    Public Property SelectedMinimumPrice() As Decimal
    Public Property SelectedMaximumPrice() As Decimal
    Public Property YearsOfPastProductsToShow As Integer
    Public Property SVGStadiumDescriptionAvailable As Boolean
    Public Property TemplateTypeId As String = String.Empty
    Public Property BusinessUnitFlag As String = String.Empty
    Public Property PackageID As String = String.Empty
    Public Property BusinessUnit As String = String.Empty
    Public Property ProductGroupCode As String = String.Empty
#End Region

#Region "Public Functions"

    Public Function LogString() As String
        Dim sb As New System.Text.StringBuilder
        With sb
            .Append(SessionId & ",")
            .Append(ProductCode & ",")
            .Append(CampaignCode & ",")
            .Append(StadiumCode & ",")
            .Append(ProductType & ",")
            .Append(StandCode & ",")
            .Append(AreaCode & ",")
            .Append(Src & ",")
            .Append(CapacityByStadium.ToString & ",")
            .Append(CustomerNumber)
        End With
        Return sb.ToString.Trim
    End Function

#End Region

End Class