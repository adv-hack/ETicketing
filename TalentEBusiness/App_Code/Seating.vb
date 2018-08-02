Imports System.Data
Imports System.Text
Imports System.Web
Imports Talent.Common
Imports Talent.Common.TalentDataObjects
Imports TCUtilies = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports System.Collections.Generic
Imports System.Web.HttpUtility

Namespace Talent.eCommerce

    ''' <summary>
    ''' Used to perform any functionality over seating retrieval
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Seating
        Inherits ClassBase01

#Region "Class Level Fields"

        Private _productCode As String = Nothing
        Private _standCode As String = Nothing
        Private _areaCode As String = Nothing
        Private _stadiumCode As String = Nothing
        Private _campaignCode As String = Nothing
        Private _seating As StringBuilder = Nothing
        Private _seatDetails As StringBuilder = Nothing
        Private _dtStatus As DataTable = Nothing
        Private _dtSeatAvailability As DataTable = Nothing
        Private _dtSeatNumbers As DataTable = Nothing
        Private _dtSeatRescritionCodes As DataTable = Nothing
        Private _dtSeatRestrictions As DataTable = Nothing
        Private _dtStadiumSeatColoursAndText As DataTable = Nothing
        Private _dtReservationCodes As DataTable = Nothing
        Private _dtSeatReservations As DataTable = Nothing
        Private _dtRetrievedReservedAndSoldSeats As DataTable = Nothing
        Private _dtProductPriceBreaks As DataTable = Nothing
        Private _dtPriceBreakSeatDetails As DataTable = Nothing
        Private _dtPriceBreakSummary As DataTable = Nothing
        Private _dtPriceBands As DataTable = Nothing
        Private _dtAgentReservationCodes As DataTable = Nothing
        Private _dStadiumSeatColours As New Dictionary(Of String, String)
        Private _dAgentReservationCodes As Dictionary(Of String, String)
        Private _dsProductReservationCodes As DataSet
        Private _dBasketSeats As Dictionary(Of String, String)
        Private _packageId As String
        Private _dtTransferableSeats As DataTable = Nothing
        Private _componentId As String
        Private _callId As String
        Private _pickingExceptionSeat As Boolean = False
        Private _currentExceptionSeat As New DESeatDetails
        Private _listBasketDetailExceptions As Generic.List(Of DESeatDetails)
        Private _defaultPriceBand As String = String.Empty
        Private _priceBreakId As Long = 0
        Private _priceBreakDefinitionsFound As Boolean = False
        Private _areaIsReversed As Boolean = False
        Private _includeTicketExchangeSeats As Boolean = False
        Private _selectedMinimumPrice As Decimal = 0
        Private _selectedMaximumPrice As Decimal = 0
        Private _ticketExchangeMinimumPrice As Decimal = 0
        Private _ticketExchangeMaximumPrice As Decimal = 0
        Private _changeAllSeats As Boolean = False
        Const APOSTROPHESPACE As String = "' "

#End Region

#Region "Public Properties"

        Public Property AvailabilityCaching() As Boolean
        Public Property AvailabilityCacheTime() As Integer
        Public Property SeatNumberCaching() As Boolean
        Public Property SeatNumberCacheTime() As Integer
        Public Property SeatRestrictionsCaching() As Boolean
        Public Property SeatRestrictionsCacheTime() As Integer
        Public Property SeatDetailsCaching() As Boolean
        Public Property SeatDetailsCacheTime() As Integer
        Public Property ReservedAndSoldSeatsCaching() As Boolean
        Public Property ReservedAndSoldSeatsCacheTime() As Integer
        Public Property ReservationCodesCaching() As Boolean
        Public Property ReservationCodesCacheTime() As Integer

#End Region

#Region "Constructor"

        Sub New(ByVal productCode As String, ByVal stadiumCode As String, ByVal standCode As String, ByVal areaCode As String, ByVal campaignCode As String, ByVal callId As String, ByVal currentExceptionSeat As String, ByVal priceBreakId As String,
                ByVal includeTicketExchangeSeats As String, ByVal selectedMinimumPrice As String, ByVal selectedMaximumPrice As String, ByVal ticketExchangeMin As String, ByVal ticketExchangeMax As String, ByVal packageId As String, ByVal componentId As String, ByVal changeAllSeats As Boolean)

            If String.IsNullOrEmpty(productCode) Then
                If HttpContext.Current.Request.QueryString("product") IsNot Nothing Then _productCode = HttpContext.Current.Request.QueryString("product").ToString().ToUpper()
            Else
                _productCode = productCode.ToUpper()
            End If
            If String.IsNullOrEmpty(stadiumCode) Then
                If HttpContext.Current.Request.QueryString("stadium") IsNot Nothing Then _stadiumCode = HttpContext.Current.Request.QueryString("stadium").ToString().ToUpper()
            Else
                _stadiumCode = stadiumCode.ToUpper()
            End If
            If String.IsNullOrEmpty(campaignCode) Then
                If HttpContext.Current.Request.QueryString("campaign") IsNot Nothing Then
                    _campaignCode = HttpContext.Current.Request.QueryString("campaign").ToString().ToUpper()
                Else
                    _campaignCode = String.Empty
                End If
            Else
                _campaignCode = campaignCode.ToUpper()
            End If

            _callId = callId
            If String.IsNullOrEmpty(packageId) Then
                If HttpContext.Current.Request.QueryString("packageId") IsNot Nothing Then
                    _packageId = _packageId = HttpContext.Current.Request.QueryString("packageId").ToString().ToUpper()
                Else
                    _packageId = String.Empty
                End If
            Else
                _packageId = packageId
            End If
            If String.IsNullOrEmpty(componentId) Then
                If HttpContext.Current.Request.QueryString("componentId") IsNot Nothing Then
                    _componentId = _packageId = HttpContext.Current.Request.QueryString("componentId").ToString().ToUpper()
                Else
                    _componentId = String.Empty
                End If
            Else
                _componentId = componentId
            End If

            If changeAllSeats Then
                _changeAllSeats = changeAllSeats
            Else
                If Not String.IsNullOrEmpty(HttpContext.Current.Request.QueryString("changeallseats")) Then
                    _changeAllSeats = TCUtilies.convertToBool(HttpContext.Current.Request.QueryString("changeallseats")).ToString.ToUpper
                End If
            End If

            If Not String.IsNullOrEmpty(standCode) Then _standCode = standCode.ToUpper()
            If Not String.IsNullOrEmpty(areaCode) Then _areaCode = areaCode.ToUpper()

            _pickingExceptionSeat = (currentExceptionSeat.Trim.Length > 0) OrElse _changeAllSeats
            If _pickingExceptionSeat AndAlso currentExceptionSeat.Trim.Length > 0 Then _currentExceptionSeat.FormattedSeat = currentExceptionSeat
            If Not String.IsNullOrEmpty(priceBreakId) Then _priceBreakId = TEBUtilities.CheckForDBNull_Long(priceBreakId)
            If Not String.IsNullOrEmpty(includeTicketExchangeSeats) Then _includeTicketExchangeSeats = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(includeTicketExchangeSeats)
            If Not String.IsNullOrEmpty(selectedMinimumPrice) Then _selectedMinimumPrice = TEBUtilities.CheckForDBNull_Decimal(selectedMinimumPrice)
            If Not String.IsNullOrEmpty(selectedMaximumPrice) Then _selectedMaximumPrice = TEBUtilities.CheckForDBNull_Decimal(selectedMaximumPrice)
            If Not String.IsNullOrEmpty(ticketExchangeMin) Then _ticketExchangeMinimumPrice = TEBUtilities.CheckForDBNull_Decimal(ticketExchangeMin)
            If Not String.IsNullOrEmpty(ticketExchangeMax) Then _ticketExchangeMaximumPrice = TEBUtilities.CheckForDBNull_Decimal(ticketExchangeMax)
        End Sub

#End Region

#Region "Public Functions"

        ''' <summary>
        ''' Make calls to the iSeries for the given product, stand and area.
        ''' </summary>
        ''' <returns>Return the seating string based on the data calls if they are successful.</returns>
        ''' <remarks></remarks>
        Public Function GetSeating() As String
            Dim okToCreateSeatingXml As Boolean = False
            Dim product As New TalentProduct
            Dim dsReservedAndSoldSeats As New DataSet
            Dim productReservation As New Product
            Dim err As New ErrorObj
            Dim agent As New Agent
            Dim talProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
            If String.IsNullOrEmpty(_componentId) Then _componentId = CATHelper.GetPackageComponentId(_productCode, _callId)

            'We need to reduce the cache time because a minute is too long for box office.  We still need to cache this area
            'because it save lots of multiple hits to the db.  The default cache time is now 10 seconds which should be sufficient,  
            'but this can be reduced\increased via a db setting if required.
            product.Settings = TEBUtilities.GetSettingsObject()
            product.Settings.Cacheing = AvailabilityCaching
            product.Settings.CacheTimeMinutes = 0
            product.Settings.CacheTimeSeconds = AvailabilityCacheTime
            product.De.ProductCode = _productCode
            product.De.StandCode = _standCode.ToUpper()
            product.De.AreaCode = _areaCode.ToUpper()
            product.De.CampaignCode = _campaignCode.ToUpper()
            product.De.ComponentID = _componentId
            product.De.PriceBreakId = _priceBreakId
            product.De.SelectedMinimumPrice = _selectedMinimumPrice
            product.De.SelectedMaximumPrice = _selectedMaximumPrice
            product.De.Src = GlobalConstants.SOURCE
            err = product.ProductSeatAvailability()
            product.Settings.CacheTimeSeconds = 0
            _seating = New StringBuilder

            '1. Get status table and check for errors then get availabilty table and check that there are some rows
            If Not err.HasError AndAlso product.ResultDataSet IsNot Nothing AndAlso product.ResultDataSet.Tables.Count = 2 Then
                _dtStatus = product.ResultDataSet.Tables(0)
                If _dtStatus.Rows.Count = 1 AndAlso String.IsNullOrEmpty(_dtStatus.Rows(0)("ErrorOccurred").ToString()) Then
                    If product.ResultDataSet.Tables(1).Rows.Count > 0 Then
                        _dtSeatAvailability = product.ResultDataSet.Tables(1)
                        okToCreateSeatingXml = True
                    End If
                End If
            End If
            handleErrors(Not okToCreateSeatingXml, 1)

            '2. Get the seat numbers, check for errors and check the actual rows of seat numbers matches the rows total number in the availablility table
            If okToCreateSeatingXml Then
                okToCreateSeatingXml = False
                product.ResultDataSet = Nothing
                product.Settings.Cacheing = SeatNumberCaching
                product.Settings.CacheTimeMinutes = SeatNumberCacheTime
                err = product.ProductSeatNumbers()
                If Not err.HasError AndAlso product.ResultDataSet IsNot Nothing AndAlso product.ResultDataSet.Tables.Count = 2 Then
                    If product.ResultDataSet.Tables(0).Rows.Count = 1 AndAlso String.IsNullOrEmpty(product.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred").ToString()) Then
                        _dtSeatNumbers = product.ResultDataSet.Tables(1)
                        If _dtSeatAvailability.Rows.Count = _dtSeatNumbers.Rows.Count Then
                            okToCreateSeatingXml = True
                        End If
                    End If
                End If
                handleErrors(Not okToCreateSeatingXml, 2)
            End If

            '3. Get the seat restrictions, check for errors and check the actual seat restrictions rows matches the rows in the availability table
            If okToCreateSeatingXml Then
                okToCreateSeatingXml = False
                product.ResultDataSet = Nothing
                product.Settings.Cacheing = SeatRestrictionsCaching
                product.Settings.CacheTimeMinutes = SeatRestrictionsCacheTime
                err = product.ProductSeatRestrictions()
                If Not err.HasError AndAlso product.ResultDataSet IsNot Nothing AndAlso product.ResultDataSet.Tables.Count = 3 Then
                    If product.ResultDataSet.Tables(0).Rows.Count = 1 AndAlso String.IsNullOrEmpty(product.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred").ToString()) Then
                        _dtSeatRescritionCodes = product.ResultDataSet.Tables(1)
                        _dtSeatRestrictions = product.ResultDataSet.Tables(2)
                        If _dtSeatAvailability.Rows.Count = _dtSeatRestrictions.Rows.Count Then
                            okToCreateSeatingXml = True
                        End If
                    End If
                End If
                handleErrors(Not okToCreateSeatingXml, 3)
            End If

            '4 Get the reservation codes and descriptions
            If okToCreateSeatingXml Then
                okToCreateSeatingXml = False
                product.ResultDataSet = Nothing
                product.Settings.Cacheing = ReservationCodesCaching
                product.Settings.CacheTimeMinutes = ReservationCodesCacheTime
                err = product.ProductReservationCodes()
                _dsProductReservationCodes = product.ResultDataSet
                If Not err.HasError AndAlso _dsProductReservationCodes IsNot Nothing AndAlso _dsProductReservationCodes.Tables.Count = 3 Then
                    If _dsProductReservationCodes.Tables(0).Rows.Count = 1 AndAlso String.IsNullOrEmpty(_dsProductReservationCodes.Tables(0).Rows(0)("ErrorOccurred").ToString()) Then
                        _dtReservationCodes = _dsProductReservationCodes.Tables("SeatReservations")
                        _dtSeatReservations = _dsProductReservationCodes.Tables("ReservationDesriptions")
                        If _dtReservationCodes.Rows.Count = _dtSeatRestrictions.Rows.Count Then
                            okToCreateSeatingXml = True
                        End If
                    End If
                End If
                handleErrors(Not okToCreateSeatingXml, 4)
            End If

            '5 Get reserved and sold seats for a particular customer
            If okToCreateSeatingXml AndAlso Not HttpContext.Current.Profile.IsAnonymous Then
                okToCreateSeatingXml = False
                product.ResultDataSet = Nothing
                Dim AddTicketingItemsDataEntity As New DEAddTicketingItems
                Dim talCustomer As New TalentCustomer
                talCustomer.Settings = TEBUtilities.GetSettingsObject()
                talCustomer.Settings.Cacheing = ReservedAndSoldSeatsCaching
                talCustomer.Settings.CacheTimeMinutes = ReservedAndSoldSeatsCacheTime
                AddTicketingItemsDataEntity.ProductCode = _productCode
                AddTicketingItemsDataEntity.SeatComponentID = _componentId
                AddTicketingItemsDataEntity.CustomerNumber = HttpContext.Current.Profile.UserName
                AddTicketingItemsDataEntity.Source = GlobalConstants.SOURCE
                talCustomer.DeAddTicketingItems = AddTicketingItemsDataEntity
                err = talCustomer.RetrieveReservedAndSoldSeats()
                dsReservedAndSoldSeats = talCustomer.ResultDataSet
                If dsReservedAndSoldSeats IsNot Nothing AndAlso dsReservedAndSoldSeats.Tables.Count > 1 Then
                    If dsReservedAndSoldSeats.Tables(0).Rows.Count = 1 AndAlso String.IsNullOrEmpty(dsReservedAndSoldSeats.Tables(0).Rows(0)("ErrorOccurred").ToString()) Then
                        _dtRetrievedReservedAndSoldSeats = dsReservedAndSoldSeats.Tables(1)
                        okToCreateSeatingXml = True
                    End If
                End If
                handleErrors(Not okToCreateSeatingXml, 5)
            End If

            '6. Get reservation codes
            If okToCreateSeatingXml Then
                _dAgentReservationCodes = agent.ReservationCodes
                handleErrors(Not okToCreateSeatingXml, 6)
            End If

            '7. Load the basket seats and any exception/component seats. Only do this when we're not in Bulk Sales Mode (as the 16 character seat details are never in the basket)
            If okToCreateSeatingXml AndAlso Not agent.BulkSalesMode Then
                _dBasketSeats = New Dictionary(Of String, String)
                If talProfile.Basket.BasketItems.Count > 0 Then
                    Dim seat As New DESeatDetails
                    For Each tbi As TalentBasketItem In talProfile.Basket.BasketItems
                        If _productCode.Trim.ToUpper() = tbi.Product.Trim.ToUpper Then
                            seat.UnFormattedSeat = tbi.SEAT
                            If seat.FormattedSeat.Trim().Length > 0 Then _dBasketSeats.Add(seat.FormattedSeat, seat.FormattedSeat)
                        End If
                    Next
                    If _pickingExceptionSeat Then
                        _listBasketDetailExceptions = TDataObjects.BasketSettings.TblBasketDetailExceptions.GetByBasketDetailHeaderIDProductCodeAndModule(talProfile.Basket.Basket_Header_ID, _productCode, GlobalConstants.BASKETMODULETICKETING)
                    End If
                End If
                If _callId.Length > 0 AndAlso _packageId.Length > 0 AndAlso _componentId.Length > 0 Then
                    Dim package As New TalentPackage
                    Dim seat As New DESeatDetails
                    package.Settings() = TEBUtilities.GetSettingsObject()
                    package.DePackages.PackageID = _packageId
                    package.DePackages.CallId = _callId
                    package.DePackages.ProductCode = _productCode
                    package.DePackages.BasketId = talProfile.Basket.Basket_Header_ID
                    err = package.GetCustomerPackageInformation()
                    If Not err.HasError AndAlso package.ResultDataSet IsNot Nothing AndAlso package.ResultDataSet.Tables("Seat").Rows.Count > 0 Then
                        For Each row As DataRow In package.ResultDataSet.Tables("Seat").Rows
                            If row("ComponentID").ToString() = _componentId Then
                                seat.UnFormattedSeat = row("SeatDetails").ToString()
                                _dBasketSeats.Add(seat.FormattedSeat, _componentId)
                            End If
                        Next
                    End If
                End If
                handleErrors(Not okToCreateSeatingXml, 7)
            End If

            '8. Retrieves list of Transferable seats
            If okToCreateSeatingXml AndAlso CATHelper.IsBasketInTransferMode() Then
                okToCreateSeatingXml = False
                Dim package As New Talent.Common.TalentPackage
                If _callId <> String.Empty Then package.DePackages.PackageID = _callId
                package.Settings() = TEBUtilities.GetSettingsObject()
                package.DePackages.ProductCode = _productCode
                package.DePackages.BasketId = talProfile.Basket.Basket_Header_ID
                err = package.GetComponentSeats()
                If Not err.HasError AndAlso package.ResultDataSet IsNot Nothing AndAlso package.ResultDataSet.Tables("Seat") IsNot Nothing Then
                    _dtTransferableSeats = package.ResultDataSet.Tables("Seat")
                    okToCreateSeatingXml = True
                End If
                handleErrors(Not okToCreateSeatingXml, 8)
            End If

            '9. Retrieves Product Price Break definitions for the product
            If okToCreateSeatingXml AndAlso TalentDefaults.PriceBreaksEnabled Then
                okToCreateSeatingXml = False
                product.ResultDataSet = Nothing
                err = product.ProductPriceBreaks()
                If product.ResultDataSet IsNot Nothing AndAlso product.ResultDataSet.Tables("ProductPriceBreaks") IsNot Nothing Then
                    If String.IsNullOrEmpty(product.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred").ToString()) Then
                        If product.ResultDataSet.Tables("ProductPriceBreaks").Rows.Count > 0 Then
                            _dtProductPriceBreaks = product.ResultDataSet.Tables("ProductPriceBreaks")
                            _priceBreakDefinitionsFound = True
                            okToCreateSeatingXml = True
                        End If
                    End If
                End If
                handleErrors(Not okToCreateSeatingXml, 9)
            End If

            '10. Retrieves Product Price Break details for the selected stand and area
            If okToCreateSeatingXml AndAlso TalentDefaults.PriceBreaksEnabled AndAlso _priceBreakDefinitionsFound Then
                okToCreateSeatingXml = True
                product.ResultDataSet = Nothing
                err = product.PriceBreakSeatDetails()
                If product.ResultDataSet IsNot Nothing AndAlso product.ResultDataSet.Tables("PriceBreakSeatDetails") IsNot Nothing Then
                    If String.IsNullOrEmpty(product.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred").ToString()) Then
                        If product.ResultDataSet.Tables("PriceBreakSeatDetails").Rows.Count > 0 Then
                            _dtPriceBreakSeatDetails = product.ResultDataSet.Tables("PriceBreakSeatDetails")
                            _areaIsReversed = TCUtilies.convertToBool(product.ResultDataSet.Tables("PriceBreakSeatDetails").Rows(0)("ReverseRow"))
                        End If
                    Else
                        okToCreateSeatingXml = False
                    End If
                Else
                    okToCreateSeatingXml = False
                End If
                handleErrors(Not okToCreateSeatingXml, 10)
            End If

            '11. Retrieves Product details for the selected product
            If okToCreateSeatingXml Then
                okToCreateSeatingXml = True
                product.ResultDataSet = Nothing
                err = product.ProductDetails()
                If product.ResultDataSet IsNot Nothing AndAlso product.ResultDataSet.Tables("ProductPriceBands") IsNot Nothing Then
                    If String.IsNullOrEmpty(product.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred").ToString()) Then
                        If product.ResultDataSet.Tables("ProductPriceBands").Rows.Count > 0 Then
                            _dtPriceBands = product.ResultDataSet.Tables("ProductPriceBands")
                        End If
                        _defaultPriceBand = TEBUtilities.CheckForDBNull_String(product.ResultDataSet.Tables("ProductDetails").Rows(0)("DefaultPriceBand"))
                    Else
                        okToCreateSeatingXml = False
                    End If
                Else
                    okToCreateSeatingXml = False
                End If
                handleErrors(Not okToCreateSeatingXml, 11)
            End If

            '12. If there are no errors all tables will have been populated, now begin creating the seats XML string
            If okToCreateSeatingXml Then
                Try
                    loopThroughSeating()
                Catch ex As Exception
                    handleErrors(True, 12, ex)
                    _seating.Clear()
                End Try
            End If

            Return _seating.ToString()
        End Function

        ''' <summary>
        ''' Gets the seat details from the given seat row and seat number based on product/stand/area information
        ''' </summary>
        ''' <param name="seatRow">The current seat row</param>
        ''' <param name="seatNumber">The current seat number</param>
        ''' <returns>String of formatted customer specific seat details</returns>
        ''' <remarks></remarks>
        Public Function GetSeatingDetails(ByVal seatRow As String, ByVal seatNumber As String) As String
            Dim talAgent As New Agent
            If talAgent.IsAgent Then
                Dim product As New TalentProduct
                Dim err As New ErrorObj
                product.Settings = TEBUtilities.GetSettingsObject()
                product.De.ProductCode = _productCode
                product.De.StandCode = _standCode.ToUpper()
                product.De.AreaCode = _areaCode.ToUpper()
                product.De.SeatRow = seatRow.ToUpper()
                product.De.SeatNumber = seatNumber.ToUpper()
                err = product.ProductSeatDetails()

                Dim AddTicketingItemsDataEntity As New DEAddTicketingItems
                Dim talCustomer As New TalentCustomer
                Dim dsReservedAndSoldSeats As New DataSet
                talCustomer.Settings = TEBUtilities.GetSettingsObject()
                talCustomer.Settings.Cacheing = ReservedAndSoldSeatsCaching
                talCustomer.Settings.CacheTimeMinutes = ReservationCodesCacheTime
                AddTicketingItemsDataEntity.ProductCode = _productCode
                AddTicketingItemsDataEntity.SeatComponentID = _componentId
                AddTicketingItemsDataEntity.CustomerNumber = HttpContext.Current.Profile.UserName
                AddTicketingItemsDataEntity.Source = GlobalConstants.SOURCE
                talCustomer.DeAddTicketingItems = AddTicketingItemsDataEntity
                err = talCustomer.RetrieveReservedAndSoldSeats()
                dsReservedAndSoldSeats = talCustomer.ResultDataSet

                If dsReservedAndSoldSeats IsNot Nothing AndAlso dsReservedAndSoldSeats.Tables.Count > 1 Then
                    If dsReservedAndSoldSeats.Tables(0).Rows.Count = 1 AndAlso String.IsNullOrEmpty(dsReservedAndSoldSeats.Tables(0).Rows(0)("ErrorOccurred").ToString()) Then
                        _dtRetrievedReservedAndSoldSeats = dsReservedAndSoldSeats.Tables(1)
                    End If
                End If
                IsSeatSoldOrReserved(_productCode, _standCode.ToUpper(), _areaCode.ToUpper(), seatRow.ToUpper(), seatNumber.ToUpper())
                If Not err.HasError AndAlso product.ResultDataSet IsNot Nothing _
                    AndAlso product.ResultDataSet.Tables.Contains("Status") AndAlso product.ResultDataSet.Tables.Contains("SeatDetails") Then
                    _dtStatus = product.ResultDataSet.Tables("Status")
                    If _dtStatus.Rows.Count = 1 AndAlso String.IsNullOrEmpty(_dtStatus.Rows(0)("ErrorOccurred").ToString()) Then
                        If product.ResultDataSet.Tables("SeatDetails").Rows.Count = 1 Then
                            formatSeatDetailsText(product.ResultDataSet.Tables("SeatDetails").Rows(0), seatRow, seatNumber)
                        End If
                    End If
                End If
                Return _seatDetails.ToString()
            Else
                Return String.Empty
            End If
        End Function

#End Region

#Region "Private Methods and Functions"

        ''' <summary>
        ''' Loop through all the seats in the area as a matrix left to right along each row using the data already retrieved from TALENT
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub loopThroughSeating()
            Dim rowIndex As Integer = 0
            Dim columnIndex As Integer = 0
            Dim pBreakIndex As Integer = 0
            Dim pBreakView As New DataView(_dtPriceBreakSeatDetails)
            Dim xPosition As Integer = 25
            Dim yPosition As Integer = 25
            Dim firstSeat As Boolean = True
            Dim totalColumnsInArea As Integer = CInt(_dtStatus.Rows(0)("ColTotal"))
            Dim agent As New Agent
            _seating.Append("<seats>")

            'Loop through each row (including blank rows), increment the row index and
            Do While rowIndex < _dtSeatAvailability.Rows.Count
                Dim cIndex As Integer = 0
                Dim currentRowOfSeatAvailability As String = _dtSeatAvailability.Rows(rowIndex)("RowDetail").ToString()
                If Not String.IsNullOrWhiteSpace(currentRowOfSeatAvailability) Then
                    Dim currentRowOfSeatNumbers As String = _dtSeatNumbers.Rows(rowIndex)("RowSeatNumbers").ToString()
                    Dim currentRowNumber As String = _dtSeatNumbers.Rows(rowIndex)("RowName").ToString()
                    Dim rowReservations As String = _dtReservationCodes.Rows(rowIndex)("ReservationCodes").ToString()
                    Dim priceArray As New List(Of String)
                    Dim numberOfColumnsInThisRow As Integer = 0

                    'Add a space on the end of the seat numbers in this row if need be and get the number of seats to loop through
                    Do While Not currentRowOfSeatNumbers.Length Mod 5 = 0
                        currentRowOfSeatNumbers += " "
                    Loop
                    numberOfColumnsInThisRow = currentRowOfSeatNumbers.Length / 5

                    'Loop through each column (individual seat element or space). Spaces are not drawn but they must be catered for as they affect the X coordinate
                    columnIndex = 0
                    xPosition = 25

                    Dim isRowSeatOnSVG As Boolean = True
                    If isRowSeatOnSVG AndAlso agent.IsAgent Then
                        'xPosition = 50
                        'yPosition = 50
                        _seating.Append("<t v='").Append(currentRowNumber).Append(APOSTROPHESPACE)
                        _seating.Append("x='").Append(25).Append(APOSTROPHESPACE)
                        _seating.Append("t='").Append("r").Append(APOSTROPHESPACE)
                        _seating.Append("y='").Append(yPosition + 25).Append(APOSTROPHESPACE).Append("/>")
                    End If

                    If TalentDefaults.PriceBreaksEnabled AndAlso _priceBreakDefinitionsFound Then
                        If _dtPriceBreakSeatDetails IsNot Nothing AndAlso _dtPriceBreakSeatDetails.Rows.Count > 0 Then
                            priceArray = getPriceArray(_dtPriceBreakSeatDetails.Select("RowNumber = '" & currentRowNumber & "'"))
                        End If
                    End If

                    Do While columnIndex < numberOfColumnsInThisRow
                        Dim currentSeatNumber As String = currentRowOfSeatNumbers.Substring(columnIndex * 5, 5)
                        Dim currentSeatAvailabilty As String = currentRowOfSeatAvailability.Substring(columnIndex, 1)
                        If currentSeatNumber <> "     " Then
                            Dim restrictionCode As String = String.Empty
                            Dim restrictionDescription As String = String.Empty
                            Dim seatReservationDescription As String = String.Empty
                            Dim currentSeatReservation As String = String.Empty

                            'This code crashes if the reservations row does not match the seats.  This should never 
                            'happen but I have added some code to stop it crashing.  I started to debug the backend
                            'to see why it occurred but the records corrected themselves at that point.  I therefore
                            'presume the seat has no reservation code.
                            If (columnIndex * 2) < rowReservations.Length Then
                                currentSeatReservation = rowReservations.Substring(columnIndex * 2, 2)
                            Else
                                currentSeatReservation = ".@"
                            End If

                            getRestriction(restrictionCode, restrictionDescription, rowIndex, columnIndex)
                            getReservationDesc(seatReservationDescription, currentSeatReservation)
                            _seating.Append("<s v='").Append(restrictionCode).Append(APOSTROPHESPACE)
                            _seating.Append("vDesc='").Append(HtmlEncode(restrictionDescription)).Append(APOSTROPHESPACE)
                            If agent.IsAgent Then
                                _seating.Append("rs='").Append(currentSeatReservation.Trim()).Append(APOSTROPHESPACE)
                                _seating.Append("rsDesc='").Append(HtmlEncode(seatReservationDescription.Trim())).Append(APOSTROPHESPACE)
                            Else
                                _seating.Append("rs='' rsDesc='' ")
                            End If
                            _seating.Append("r='").Append(currentRowNumber.Trim()).Append(APOSTROPHESPACE)
                            _seating.Append("n='").Append(currentSeatNumber.Trim()).Append(APOSTROPHESPACE)

                            Dim seatPriceDecimal As Decimal = 0
                            If priceArray.Count > 0 Then seatPriceDecimal = TEBUtilities.CheckForDBNull_Decimal(priceArray(columnIndex))
                            'Add the price information to the seat if enabled and definitions have been found
                            If TalentDefaults.PriceBreaksEnabled AndAlso _priceBreakDefinitionsFound Then
                                If priceArray.Count > 0 Then
                                    Dim seatPriceString As String = String.Empty
                                    If seatPriceDecimal > 0 Then
                                        seatPriceString = TDataObjects.PaymentSettings.FormatCurrency(seatPriceDecimal, TalentCache.GetBusinessUnit(), TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)))
                                    End If
                                    _seating.Append("p='").Append(HttpContext.Current.Server.HtmlDecode(seatPriceString)).Append(APOSTROPHESPACE) 'replace "&pound;" with "£"
                                End If
                            End If

                            Dim seatAvailability As String = currentSeatAvailabilty
                            'Set the seat availability to in the basket when we have already added it.
                            Dim seat As New DESeatDetails
                            seat.Stand = Talent.Common.Utilities.FixStringLength(_standCode.ToUpper.Trim, 3)
                            seat.Area = Talent.Common.Utilities.FixStringLength(_areaCode.ToUpper.Trim, 4)
                            seat.Row = Talent.Common.Utilities.FixStringLength(currentRowNumber.ToUpper.Trim, 4)
                            seat.Seat = currentSeatNumber.ToUpper
                            If Not agent.BulkSalesMode AndAlso _dBasketSeats.ContainsKey(seat.FormattedSeat) Then
                                If _dBasketSeats.ContainsValue(_componentId) Then
                                    If _changeAllSeats Then
                                        seatAvailability = "T"
                                    ElseIf seat.FormattedSeat = _currentExceptionSeat.FormattedSeat Then
                                        seatAvailability = "EX" 'changing a component seat, therefore use the "exception seat" colour key on the seating colours chart.
                                    Else
                                        seatAvailability = "B" 'when basketSeats has componentId but it is not current formattedSeat
                                    End If
                                Else
                                    seatAvailability = "B"
                                End If
                            End If
                            If _pickingExceptionSeat AndAlso _listBasketDetailExceptions.Count > 0 Then
                                seatAvailability = getSeatAvailabilityForExceptionSeat(seatAvailability, currentRowNumber, currentSeatNumber)
                            End If

                            Select Case seatAvailability
                                Case "." 'Unavailable
                                    If CATHelper.IsBasketInTransferMode() Then
                                        If IsSeatTransferable(_productCode.ToUpper(), _standCode.ToUpper(), _areaCode.ToUpper(), currentRowNumber.Trim(), currentSeatNumber.Trim()) Then
                                            seatAvailability = "T"
                                        End If
                                    Else
                                        If IsSeatSoldOrReserved(_productCode.ToUpper(), _standCode.ToUpper(), _areaCode.ToUpper(), currentRowNumber.Trim(), currentSeatNumber.Trim()) Then
                                            seatAvailability = ".2"
                                        End If
                                    End If
                                Case "A" 'Available
                                    If (agent.IsAgent AndAlso agent.SellAvailableTickets) OrElse (Not agent.IsAgent AndAlso _callId <> String.Empty) Then
                                        If restrictionCode <> String.Empty Then
                                            seatAvailability = "X"
                                        Else
                                            seatAvailability = "A"
                                        End If
                                    Else
                                        seatAvailability = "."
                                    End If
                                Case "3" 'Web Sales
                                    If (agent.IsAgent AndAlso IsReservationAuthorised("03")) OrElse Not agent.IsAgent Then
                                        If restrictionCode <> String.Empty Then
                                            seatAvailability = "X"
                                        Else
                                            seatAvailability = "A"
                                        End If
                                    Else
                                        seatAvailability = "."
                                    End If
                                Case "9" ' Ticket Exchange
                                    If (agent.IsAgent AndAlso IsReservationAuthorised("09")) OrElse Not agent.IsAgent Then
                                        If _includeTicketExchangeSeats AndAlso seatPriceDecimal >= _ticketExchangeMinimumPrice AndAlso seatPriceDecimal <= _ticketExchangeMaximumPrice Then
                                            seatAvailability = "TX"
                                        Else
                                            seatAvailability = "."
                                        End If
                                    Else
                                        seatAvailability = "."
                                    End If
                                Case "7"
                                    If (agent.IsAgent AndAlso IsReservationAuthorised("07")) OrElse Not agent.IsAgent Then
                                        If restrictionCode <> String.Empty Then
                                            seatAvailability = "X"
                                        Else
                                            seatAvailability = "A"
                                        End If
                                    Else
                                        seatAvailability = "."
                                    End If
                                Case "R" 'Reserved
                                    If currentSeatReservation = GlobalConstants.RES_CODE_ST_RESERVED OrElse currentSeatReservation = GlobalConstants.RES_CODE_CUSTOMER_RESERVED Then
                                        If (agent.IsAgent AndAlso IsReservationAuthorised(currentSeatReservation)) OrElse Not agent.IsAgent Then
                                            If IsSeatSoldOrReserved(_productCode.ToUpper(), _standCode.ToUpper(), _areaCode.ToUpper(), currentRowNumber.Trim(), currentSeatNumber.Trim()) Then
                                                seatAvailability = "C"
                                            Else
                                                seatAvailability = "."
                                            End If
                                        Else
                                            seatAvailability = "."
                                        End If
                                    ElseIf (agent.IsAgent AndAlso IsReservationAuthorised(currentSeatReservation)) Then
                                        seatAvailability = "R"
                                    Else
                                        seatAvailability = "."
                                    End If
                                Case "D" 'Disabled Seat
                                    If IsReservationAuthorised(currentSeatReservation) Then
                                        seatAvailability = "D"
                                    Else
                                        seatAvailability = "."
                                    End If
                                Case "S" 'Partial Season Ticket
                                    If agent.IsAgent Then
                                        seatAvailability = "S"
                                    ElseIf String.IsNullOrEmpty(_callID)
                                        seatAvailability = "A"
                                    Else
                                        seatAvailability = "."
                                    End If
                                Case "E"
                                    seatAvailability = "."
                            End Select

                            _seating.Append("a='").Append(seatAvailability).Append(APOSTROPHESPACE)
                            If firstSeat Then
                                firstSeat = False
                                _seating.Append("cn='").Append(totalColumnsInArea).Append(APOSTROPHESPACE)
                                _seating.Append("rn='").Append(_dtSeatAvailability.Rows.Count).Append(APOSTROPHESPACE)
                                _seating.Append(getSeatColours(_stadiumCode))
                            End If
                            If isRowSeatOnSVG Then
                                _seating.Append("x='").Append(xPosition + 25).Append(APOSTROPHESPACE)
                                _seating.Append("y='").Append(yPosition + 25).Append(APOSTROPHESPACE)
                            Else
                                _seating.Append("x='").Append(xPosition).Append(APOSTROPHESPACE)
                                _seating.Append("y='").Append(yPosition).Append(APOSTROPHESPACE)
                            End If

                            pBreakView.RowFilter = "STAND = '" & _standCode.ToUpper() & "' AND AREA = '" & _areaCode.ToUpper() & "' AND ROWNUMBER = '" & currentRowNumber.Trim() & "'"
                            If pBreakView.Count > 0 Then
                                pBreakIndex = columnIndex + 1
                                'This adds the correct price break id for seat to the XML string
                                _seating.Append("pb='").Append(pBreakView.Item(0).Item("PBID" & pBreakIndex)).Append("'")
                            End If
                            _seating.Append("/>")
                            currentSeatReservation = ""
                        End If
                        columnIndex += 1
                        xPosition += 25
                        cIndex += 1
                    Loop
                End If
                rowIndex += 1
                yPosition += 25
            Loop

            'Create summary table of PriceBreaks for selected Stand / Area
            _dtPriceBreakSummary = priceBreakSummaryDT(_dtProductPriceBreaks, _dtPriceBreakSeatDetails)

            'Expand XML seating to include price break information
            If _dtPriceBreakSummary.Rows.Count > 0 Then
                Dim pBreakString As New StringBuilder
                Dim price As String = String.Empty
                Dim formattedPrice As String = String.Empty
                Dim defaultPrice As String = String.Empty
                Dim pBreakID As String = String.Empty
                Dim pBreakDescription As String = String.Empty
                Dim availablePriceBandsCharged As String = String.Empty
                Dim availablePriceBandsFree As String = String.Empty
                Dim n As Integer = 0

                pBreakString.Append("<pricing><pband>")
                If _dtPriceBands.Rows.Count > 0 Then
                    For Each row As DataRow In _dtPriceBands.Rows
                        Dim priceBand As String = row.Item("PriceBand").ToString()
                        If priceBand = "Z" OrElse Integer.TryParse(priceBand, n) Then
                            availablePriceBandsFree &= priceBand
                        Else
                            availablePriceBandsCharged &= priceBand
                        End If
                        pBreakString.Append("<d b='").Append(row.Item("PriceBand"))
                        pBreakString.Append("' desc='").Append(row.Item("PriceBandDescription"))
                        pBreakString.Append("'/>")
                    Next
                End If
                pBreakString.Append("</pband>")
                For Each row As DataRow In _dtPriceBreakSummary.Rows
                    Dim pBreakHeaderDefinition As New StringBuilder
                    Dim pBreakDetailDefinition As New StringBuilder
                    pBreakID = row.Item("PriceBreakID")
                    pBreakDescription = row.Item("PriceBreakDescription")
                    defaultPrice = getDefaultPrice(row)
                    formattedPrice = TDataObjects.PaymentSettings.FormatCurrency(defaultPrice, TalentCache.GetBusinessUnit(), TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)))
                    pBreakHeaderDefinition.Append("<pbreak").Append(" id='").Append(pBreakID).Append("' d='").Append(pBreakDescription).Append("' b='").Append(_defaultPriceBand)
                    pBreakHeaderDefinition.Append("' p='").Append(defaultPrice).Append("' f='").Append(HttpContext.Current.Server.HtmlDecode(formattedPrice)).Append("'>")
                    For Each c In availablePriceBandsCharged
                        If row.Item("PriceBand" & c) > 0 Then
                            price = row.Item("PriceBand" & c)
                            formattedPrice = TDataObjects.PaymentSettings.FormatCurrency(CDec(price), TalentCache.GetBusinessUnit(), TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)))
                            pBreakDetailDefinition.Append("<d b='").Append(c).Append("' p='").Append(price).Append("' f='").Append(HttpContext.Current.Server.HtmlDecode(formattedPrice)).Append("'/>")
                        End If
                    Next
                    For Each c In availablePriceBandsFree
                        If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(row.Item("PriceBandAvailable" & c)) Then
                            formattedPrice = TDataObjects.PaymentSettings.FormatCurrency(0, TalentCache.GetBusinessUnit(), TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)))
                            pBreakDetailDefinition.Append("<d b='").Append(c).Append("' p='0'").Append(" f='").Append(HttpContext.Current.Server.HtmlDecode(formattedPrice)).Append("'/>")
                        End If
                    Next
                    If pBreakDetailDefinition.Length > 0 Then
                        pBreakHeaderDefinition.Append(pBreakDetailDefinition)
                        pBreakHeaderDefinition.Append("</pbreak>")
                        pBreakString.Append(pBreakHeaderDefinition)
                    End If
                Next
                pBreakString.Append("</pricing>")
                _seating.Append(pBreakString)
            End If

            _seating.Append("</seats>")
        End Sub

        ''' <summary>
        ''' Set the restriction description and code based on the given seat that is currently being worked with
        ''' </summary>
        ''' <param name="restrictionCode">The restriction code for the seat</param>
        ''' <param name="restrictionDescription">The description of the seat restriction</param>
        ''' <param name="rowIndex">The current row thats being worked with</param>
        ''' <param name="columnIndex">The current column (seat) within the row thats being worked with</param>
        ''' <remarks></remarks>
        Private Sub getRestriction(ByRef restrictionCode As String, ByRef restrictionDescription As String, ByRef rowIndex As Integer, ByRef columnIndex As Integer)
            Try
                Dim seatRestrictionRow As String = _dtSeatRestrictions.Rows(rowIndex)("SeatRestrictions").ToString()
                If Not String.IsNullOrWhiteSpace(seatRestrictionRow) Then
                    Dim thisSeat As String = seatRestrictionRow.Substring(columnIndex * 2, 2)
                    If Not String.IsNullOrWhiteSpace(thisSeat) AndAlso thisSeat <> ".@" Then
                        restrictionCode = thisSeat
                        For Each row As DataRow In _dtSeatRescritionCodes.Rows
                            If row("Restriction").ToString() = thisSeat Then
                                restrictionDescription = row("Description").ToString().Trim()
                                Exit For
                            End If
                        Next
                    End If
                End If
            Catch ex As Exception
            End Try
        End Sub

        ''' <summary>
        ''' Get he reservation description based on the code
        ''' </summary>
        ''' <param name="reservationDesc">The reservation description</param>
        ''' <param name="reservationCode">The reservation code</param>
        ''' <remarks></remarks>
        Private Sub getReservationDesc(ByRef reservationDesc As String, ByVal reservationCode As String)
            For Each description In _dtSeatReservations.Rows
                If reservationCode = description("ReservationCode").Trim() Then
                    reservationDesc = description("Description")
                    Exit For
                End If
            Next
        End Sub

        ''' <summary>
        ''' Has the current seat for the given product been sold or reserved
        ''' </summary>
        ''' <param name="productCode">The given product code</param>
        ''' <param name="standCode">The stand code</param>
        ''' <param name="areaCode">The area code</param>
        ''' <param name="rowNumber">The row number</param>
        ''' <param name="seatNumber">The seat number</param>
        ''' <returns>True if the seat is sold or reserved</returns>
        ''' <remarks></remarks>
        Private Function IsSeatSoldOrReserved(ByVal productCode As String, ByVal standCode As String, ByVal areaCode As String, ByVal rowNumber As String, ByVal seatNumber As String) As Boolean
            If _dtRetrievedReservedAndSoldSeats IsNot Nothing Then
                For Each seat As DataRow In _dtRetrievedReservedAndSoldSeats.Rows
                    If seat("ProductCode").Equals(productCode) AndAlso seat("Stand").Equals(standCode) AndAlso seat("Area").Equals(areaCode) AndAlso seat("Row").Equals(rowNumber) AndAlso seat("Seat").Equals(seatNumber) Then
                        Return True
                    End If
                Next
            End If
            Return False
        End Function

        ''' <summary>
        ''' Is the current seat a transfer seat
        ''' </summary>
        ''' <param name="productCode">The given product code</param>
        ''' <param name="standCode">The stand code</param>
        ''' <param name="areaCode">The area code</param>
        ''' <param name="rowNumber">The row number</param>
        ''' <param name="seatNumber">The seat number</param>
        ''' <returns>True if the seat is transferable</returns>
        ''' <remarks></remarks>
        Private Function IsSeatTransferable(ByVal productCode As String, ByVal standCode As String, ByVal areaCode As String, ByVal rowNumber As String, ByVal seatNumber As String) As Boolean
            If _dtTransferableSeats IsNot Nothing Then
                For Each seat As DataRow In _dtTransferableSeats.Rows
                    If seat("ProductCode").Equals(productCode) AndAlso seat("Stand").Equals(standCode) AndAlso seat("Area").Equals(areaCode) AndAlso seat("Row").Equals(rowNumber) AndAlso seat("Seat").Equals(seatNumber) Then
                        Return True
                    End If
                Next
            End If
            Return False
        End Function

        ''' <summary>
        ''' Returns the reserved seat
        ''' </summary>
        ''' <param name="productCode">The given product code</param>
        ''' <param name="standCode">The stand code</param>
        ''' <param name="areaCode">The area code</param>
        ''' <param name="rowNumber">The row number</param>
        ''' <param name="seatNumber">The seat number</param>
        ''' <returns>True if the seat is sold or reserved</returns>
        ''' <remarks></remarks>
        Private Function returnReservedSeat(ByVal productCode As String, ByVal standCode As String, ByVal areaCode As String, ByVal rowNumber As String, ByVal seatNumber As String) As DataRow
            If _dtRetrievedReservedAndSoldSeats IsNot Nothing Then
                For Each seat As DataRow In _dtRetrievedReservedAndSoldSeats.Rows
                    If seat("ProductCode").Equals(productCode) AndAlso seat("Stand").Equals(standCode) AndAlso seat("Area").Equals(areaCode) AndAlso seat("Row").Equals(rowNumber) AndAlso seat("Seat").Equals(seatNumber) AndAlso seat("ReservationCode") = "04" Then
                        Return seat
                        Exit For
                    End If
                Next
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Is the current agent authorized to sell the current reservation code
        ''' </summary>
        ''' <param name="seatReservation">The given seat reservation</param>
        ''' <returns>True if the agent can sell the current reservation</returns>
        ''' <remarks></remarks>
        Private Function IsReservationAuthorised(ByVal seatReservation As String) As Boolean
            If _dAgentReservationCodes.ContainsKey(seatReservation) Then
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' Get the seat colours based on the given stadium code and seat type
        ''' </summary>
        ''' <param name="stadiumCode">The stadium code</param>
        ''' <remarks></remarks>
        Private Function getSeatColours(ByRef stadiumCode As String) As String
            Dim _businessUnit As String = TalentCache.GetBusinessUnit()
            _dtStadiumSeatColoursAndText = New DataTable
            _dtStadiumSeatColoursAndText = TDataObjects.StadiumSettings.TblStadiumSeatColours.GetStadiumSeatColoursAndText(_businessUnit, stadiumCode, False)
            Dim _seating As New StringBuilder

            If _dtStadiumSeatColoursAndText.Rows.Count > 0 Then
                For Each dr As DataRow In _dtStadiumSeatColoursAndText.Rows
                    Select Case dr("SEAT_TYPE")
                        Case Is = "A" 'Available
                            _seating.Append("ac='").Append(dr("OUTLINE_COLOUR")).Append(APOSTROPHESPACE)
                            _seating.Append("ao='").Append(dr("FILL_COLOUR")).Append(APOSTROPHESPACE)
                        Case Is = "C" 'Customer Reserved
                            _seating.Append("cc='").Append(dr("OUTLINE_COLOUR")).Append(APOSTROPHESPACE)
                            _seating.Append("co='").Append(dr("FILL_COLOUR")).Append(APOSTROPHESPACE)
                        Case Is = "X" 'Restricted Seat
                            _seating.Append("rc='").Append(dr("OUTLINE_COLOUR")).Append(APOSTROPHESPACE)
                            _seating.Append("ro='").Append(dr("FILL_COLOUR")).Append(APOSTROPHESPACE)
                        Case Is = "." 'Unavailable
                            _seating.Append("uc='").Append(dr("OUTLINE_COLOUR")).Append(APOSTROPHESPACE)
                            _seating.Append("uo='").Append(dr("FILL_COLOUR")).Append(APOSTROPHESPACE)
                        Case Is = ".2" 'Sold/Reserved (to you or F&F)
                            _seating.Append("u2c='").Append(dr("OUTLINE_COLOUR")).Append(APOSTROPHESPACE)
                            _seating.Append("u2o='").Append(dr("FILL_COLOUR")).Append(APOSTROPHESPACE)
                        Case Is = "D" 'Disabled Seat
                            _seating.Append("dc='").Append(dr("OUTLINE_COLOUR")).Append(APOSTROPHESPACE)
                            _seating.Append("do='").Append(dr("FILL_COLOUR")).Append(APOSTROPHESPACE)
                        Case Is = "R" 'Reserved
                            _seating.Append("vc='").Append(dr("OUTLINE_COLOUR")).Append(APOSTROPHESPACE)
                            _seating.Append("vo='").Append(dr("FILL_COLOUR")).Append(APOSTROPHESPACE)
                        Case Is = "HOVER" 'Hovered
                            _seating.Append("hc='").Append(dr("OUTLINE_COLOUR")).Append(APOSTROPHESPACE)
                            _seating.Append("ho='").Append(dr("FILL_COLOUR")).Append(APOSTROPHESPACE)
                        Case Is = "SELECTED" 'Selected
                            _seating.Append("selc='").Append(dr("OUTLINE_COLOUR")).Append(APOSTROPHESPACE)
                            _seating.Append("selo='").Append(dr("FILL_COLOUR")).Append(APOSTROPHESPACE)
                        Case Is = "B" 'In basket
                            _seating.Append("bc='").Append(dr("OUTLINE_COLOUR")).Append(APOSTROPHESPACE)
                            _seating.Append("bo='").Append(dr("FILL_COLOUR")).Append(APOSTROPHESPACE)
                        Case Is = "T" 'Transferable Seat
                            _seating.Append("tc='").Append(dr("OUTLINE_COLOUR")).Append(APOSTROPHESPACE)
                            _seating.Append("to='").Append(dr("FILL_COLOUR")).Append(APOSTROPHESPACE)
                        Case Is = "EXCEPTION" 'Season Ticket Exception Seat
                            _seating.Append("ec='").Append(dr("OUTLINE_COLOUR")).Append(APOSTROPHESPACE)
                            _seating.Append("eo='").Append(dr("FILL_COLOUR")).Append(APOSTROPHESPACE)
                        Case Is = "TX" 'Ticket Exchange Listed Seat
                            _seating.Append("txc='").Append(dr("OUTLINE_COLOUR")).Append(APOSTROPHESPACE)
                            _seating.Append("txo='").Append(dr("FILL_COLOUR")).Append(APOSTROPHESPACE)
                        Case Is = "S" 'Partial Season Ticket Seat
                            _seating.Append("psc='").Append(dr("OUTLINE_COLOUR")).Append(APOSTROPHESPACE)
                            _seating.Append("pso='").Append(dr("FILL_COLOUR")).Append(APOSTROPHESPACE)
                    End Select
                Next
            End If
            Return _seating.ToString
        End Function

        ''' <summary>
        ''' Format the seat details based on the data row given
        ''' </summary>
        ''' <param name="row">The seat details data row</param>
        ''' <remarks></remarks>
        Private Sub formatSeatDetailsText(ByRef row As DataRow, ByVal seatRow As String, ByVal seatNumber As String)
            Const APOSTROPHESPACE As String = "' "
            _seatDetails = New StringBuilder
            _seatDetails.Append("<sd>")
            _seatDetails.Append("<s a='").Append(row("Available").ToString()).Append(APOSTROPHESPACE)
            _seatDetails.Append("cat='").Append(row("Category").ToString()).Append(APOSTROPHESPACE)
            _seatDetails.Append("cd='").Append(row("CategoryDescription").ToString()).Append(APOSTROPHESPACE)
            _seatDetails.Append("rd='").Append(row("TextDescription").ToString()).Append(APOSTROPHESPACE)
            _seatDetails.Append("rc='").Append(row("RestrictionCode").ToString()).Append(APOSTROPHESPACE)
            _seatDetails.Append("rds='").Append(row("RestrictionDescription").ToString()).Append(APOSTROPHESPACE)
            _seatDetails.Append("c='").Append(row("Customer").ToString()).Append(APOSTROPHESPACE)
            _seatDetails.Append("fn='").Append(row("Forename").ToString()).Append(APOSTROPHESPACE)
            _seatDetails.Append("sn='").Append(row("Surname").ToString()).Append(APOSTROPHESPACE)
            _seatDetails.Append("ad1='").Append(row("AddressLine1").ToString()).Append(APOSTROPHESPACE)
            _seatDetails.Append("ad2='").Append(row("AddressLine2").ToString()).Append(APOSTROPHESPACE)
            _seatDetails.Append("ad3='").Append(row("AddressLine3").ToString()).Append(APOSTROPHESPACE)
            _seatDetails.Append("sts='").Append(row("SeasonTicketSeat").ToString()).Append(APOSTROPHESPACE)
            Dim seat As DataRow = returnReservedSeat(_productCode, _standCode.ToUpper(), _areaCode.ToUpper(), seatRow.ToUpper(), seatNumber.ToUpper())
            If Not seat Is Nothing Then
                _seatDetails.Append("resd='").Append(seat("ReservedDate").ToString()).Append(APOSTROPHESPACE)
                _seatDetails.Append("rest='").Append(seat("ReservedTime").ToString()).Append(APOSTROPHESPACE)
            End If
            _seatDetails.Append("stn='").Append(row("SeasonTicketBookNumber").ToString()).Append("'/>")
            _seatDetails.Append("</sd>")
        End Sub

        ''' <summary>
        ''' Get the seat availability value for a season ticket exception seat if this current seat is an exception seat. There maybe a number of seats in exception therefore only 1 seat is changed at a time.
        ''' Show the current seat being changed as an exception seat with the other exception seats as in the basket.
        ''' </summary>
        ''' <param name="seatAvailability">The current seat availablity value</param>
        ''' <param name="seatRow">The current seat row</param>
        ''' <param name="seatNumber">The current seat number</param>
        ''' <returns>The seat availability value</returns>
        ''' <remarks></remarks>
        Private Function getSeatAvailabilityForExceptionSeat(ByRef seatAvailability As String, ByVal seatRow As String, ByVal seatNumber As String) As String
            If _currentExceptionSeat.Stand = _standCode AndAlso _currentExceptionSeat.Area = _areaCode AndAlso _currentExceptionSeat.Row = seatRow AndAlso
                    (_currentExceptionSeat.Seat.Trim() & _currentExceptionSeat.AlphaSuffix.Trim()) = seatNumber.Trim() Then
                seatAvailability = "EX" 'current exception seat being changed
            End If
            If _listBasketDetailExceptions.Count > 1 Then 'There is only 1 exception seat for this product, no need to mark any other seats as in basket
                For Each exceptionSeat As DESeatDetails In _listBasketDetailExceptions
                    If seatAvailability <> "EX" Then
                        If exceptionSeat.Stand = _standCode AndAlso exceptionSeat.Area = _areaCode AndAlso exceptionSeat.Row = seatRow AndAlso exceptionSeat.Seat = seatNumber Then
                            seatAvailability = "B" 'Any other seat given as exception but not currently being changed (mark as in the basket)
                            Exit For
                        End If
                    End If
                Next
            End If
            Return seatAvailability
        End Function

        ''' <summary>
        ''' Populate the price break array based on the row of given price break data. Handle the reversing of the array if the area is reversed.
        ''' </summary>
        ''' <param name="priceBreakRow">The price break row of data</param>
        ''' <returns>The price break array in the correct order, reversed or not.</returns>
        ''' <remarks></remarks>
        Private Function getPriceArray(ByRef priceBreakRow As DataRow()) As List(Of String)
            Dim priceArray As New List(Of String)
            If priceBreakRow.Length > 0 Then
                Dim columnPosition As Integer = 1
                Dim priceArrayColumnName As String = "DefaultPrice"
                Dim priceTypeArrayColumnName As String = "PriceType"
                Dim priceTypeArray As New List(Of String)
                Dim reversedPriceArray As New List(Of String)
                Dim loopCounter As Integer = 149
                Dim reverseCounter As Integer = 0
                Dim offset As Integer = 0
                Dim firstSeatFound As Boolean = False

                Do While columnPosition <= 150
                    priceArray.Add(priceBreakRow(0)(priceArrayColumnName & columnPosition))
                    priceTypeArray.Add(priceBreakRow(0)(priceTypeArrayColumnName & columnPosition))
                    columnPosition += 1
                Loop

                If _areaIsReversed Then
                    columnPosition = 1
                    Do While columnPosition <= 150
                        reversedPriceArray.Add(0.0) 'Load the reversed array with 150 elements
                        columnPosition += 1
                    Loop

                    Do While loopCounter > -1
                        offset = 0
                        If Not String.IsNullOrWhiteSpace(priceTypeArray(loopCounter)) Or firstSeatFound Then 'Does this seat have a type? This validation is only relevant when we have found the first seat

                            'Offset processing is not applicable when the seat is the last one in the row
                            firstSeatFound = True
                            If loopCounter > 0 Then

                                'You need to apply the offsets (aisles) before the seat when reversing the row.
                                'If the seat before has an offset then load that into the reversed array first
                                Do While offset < loopCounter
                                    If String.IsNullOrWhiteSpace(priceTypeArray(loopCounter - (offset + 1))) Then
                                        offset += 1
                                        reversedPriceArray(reverseCounter) = 0.0
                                        reverseCounter += 1
                                    Else
                                        Exit Do
                                    End If
                                Loop
                            End If

                            'load the pricing information in the reversed arrays
                            reversedPriceArray(reverseCounter) = priceArray(loopCounter)
                            reverseCounter += 1
                        End If
                        loopCounter = loopCounter - 1 - offset 'Decrement the loop counter by 1 and by the number of offsets processed
                    Loop
                    priceArray = New List(Of String)
                    priceArray = reversedPriceArray
                End If
            End If
            Return priceArray
        End Function

        ''' <summary>
        ''' This Function creates a summary table joining MD141TBL data and MD143TBL data
        ''' </summary>
        ''' <param name="dtProductPriceBreaks">MD141TBL data</param>
        ''' <param name="dtPriceBreakDetails">MD143TBL data</param>
        ''' <returns>Datatable of Summary</returns>
        ''' <remarks></remarks>
        Private Function priceBreakSummaryDT(ByRef dtProductPriceBreaks As DataTable, ByRef dtPriceBreakDetails As DataTable) As DataTable
            Dim dtPriceBreakSummary As New DataTable
            If dtPriceBreakDetails IsNot Nothing Then
                Dim PriceBreaks As New List(Of String)
                Dim pbreakid As String = ""
                For Each row In dtPriceBreakDetails.Rows
                    For i = 1 To 150
                        pbreakid = row.item("PBID" & i)
                        If PriceBreaks.Contains(pbreakid) = False Then
                            PriceBreaks.Add(pbreakid)
                        End If
                    Next
                Next
                Dim InPriceBreaks As String = String.Join(",", PriceBreaks)
                dtProductPriceBreaks.DefaultView.RowFilter = "PriceBreakid IN (" & InPriceBreaks & ")"
                dtPriceBreakSummary = dtProductPriceBreaks.DefaultView.ToTable
            End If
            Return dtPriceBreakSummary
        End Function

        ''' <summary>
        ''' Get the default price for the default price band for the specific price break definition
        ''' </summary>
        ''' <param name="priceBreakRow">The price break definition row</param>
        ''' <returns>The default price band price</returns>
        ''' <remarks></remarks>
        Private Function getDefaultPrice(ByRef priceBreakRow As DataRow) As String
            Dim defaultPrice As String = String.Empty
            Dim isFOCPriceBand As Boolean = True
            For Each c In "ABCDEFGHIJKLMNOPQRSTUVWXY"
                If _defaultPriceBand = c.ToString() Then
                    isFOCPriceBand = False
                    Exit For
                End If
            Next
            If isFOCPriceBand Then
                defaultPrice = "0.00"
            Else
                defaultPrice = priceBreakRow.Item("PriceBand" & _defaultPriceBand)
            End If
            Return defaultPrice
        End Function

        ''' <summary>
        ''' Handle any errors and based on the property values that are present
        ''' </summary>
        ''' <param name="functionNumber">The function number that has caused the error</param>
        ''' <param name="hasError">Has an error occurred</param>
        ''' <param name="ex">Exception object if available</param>
        ''' <remarks></remarks>
        Private Sub handleErrors(ByVal hasError As Boolean, ByVal functionNumber As Integer, Optional ByVal ex As Exception = Nothing)
            If hasError Then
                Dim log As New TalentLogging
                log.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString()
                Dim seatingvb As String = "Seating.vb"
                Dim seatSelectionLog As String = "SeatSelectionLog"
                Dim productCode As String = _productCode
                Dim standAndArea As String = _standCode & "/" & _areaCode
                If String.IsNullOrEmpty(_productCode) Then productCode = HttpContext.Current.Request("product")
                Select Case functionNumber
                    Case Is = 1 : log.GeneralLog(seatingvb, "Product: " & _productCode & ", Stand/Area: " & standAndArea & ", seating-class-01", "No availabilty returned from WS151R", seatSelectionLog)
                    Case Is = 2 : log.GeneralLog(seatingvb, "Product: " & _productCode & ", Stand/Area: " & standAndArea & ", seating-class-02", "No seat numbers returned from WS152R or seat numbers do not match availabilty", seatSelectionLog)
                    Case Is = 3 : log.GeneralLog(seatingvb, "Product: " & _productCode & ", Stand/Area: " & standAndArea & ", seating-class-03", "No seat restrictions/descriptions returned from WS153R or restrictions do not match availability", seatSelectionLog)
                    Case Is = 4 : log.GeneralLog(seatingvb, "Product: " & _productCode & ", Stand/Area: " & standAndArea & ", seating-class-04", "No seat reservation codes or descriptions returned from WS153R or the number of reservations does not match the number of restrictions", seatSelectionLog)
                    Case Is = 5 : log.GeneralLog(seatingvb, "Product: " & _productCode & ", Stand/Area: " & standAndArea & ", seating-class-05", "Error returned from WS133R when retrieving the reserved and sold seats", seatSelectionLog)
                    Case Is = 6 : log.GeneralLog(seatingvb, "Product: " & _productCode & ", Stand/Area: " & standAndArea & ", seating-class-06", "Error returned from WS129R when retrieving the agent reservation codes", seatSelectionLog)
                    Case Is = 7 : log.GeneralLog(seatingvb, "Product: " & _productCode & ", Stand/Area: " & standAndArea & ", seating-class-07", "Error occurred during basket retrieval", seatSelectionLog)
                    Case Is = 8 : log.GeneralLog(seatingvb, "Product: " & _productCode & ", Stand/Area: " & standAndArea & ", seating-class-08", "Error occurred during retrieval of transferrable seats", seatSelectionLog)
                    Case Is = 9 : log.GeneralLog(seatingvb, "Product: " & _productCode & ", Stand/Area: " & standAndArea & ", seating-class-09", "Error occurred during retrieval of price breaks definitions - MD141S", seatSelectionLog)
                    Case Is = 10 : log.GeneralLog(seatingvb, "Product: " & _productCode & ", Stand/Area: " & standAndArea & ", seating-class-10", "Error occurred during retrieval of price breaks seating availablity - MD143S", seatSelectionLog)
                    Case Is = 11 : log.GeneralLog(seatingvb, "Product: " & _productCode & ", Stand/Area: " & standAndArea & ", seating-class-11", "Error occurred during retrieval of product details WS007R", seatSelectionLog)
                    Case Is = 12 : log.GeneralLog(seatingvb, "Product: " & _productCode & ", Stand/Area: " & standAndArea & ", seating-class-12", "Exception: " & ex.Message & " | Stack Trace: " & ex.StackTrace & " | seatingXML: " & _seating.ToString(), seatSelectionLog)
                End Select
            End If
        End Sub

#End Region

    End Class

End Namespace