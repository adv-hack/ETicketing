<Serializable()> _
Public Class DEAddTicketingItems

#Region "Class Level Fields"

    Private _sessionId As String = ""
    Private _productCode As String = ""
    Private _standCode As String = ""
    Private _areaCode As String = ""
    Private _priceCode As String = ""
    Private _campaignCode As String = ""
    Private _bnd01 As String = ""
    Private _bnd02 As String = ""
    Private _bnd03 As String = ""
    Private _bnd04 As String = ""
    Private _bnd05 As String = ""
    Private _bnd06 As String = ""
    Private _bnd07 As String = ""
    Private _bnd08 As String = ""
    Private _bnd09 As String = ""
    Private _bnd10 As String = ""
    Private _bnd11 As String = ""
    Private _bnd12 As String = ""
    Private _bnd13 As String = ""
    Private _bnd14 As String = ""
    Private _bnd15 As String = ""
    Private _bnd16 As String = ""
    Private _bnd17 As String = ""
    Private _bnd18 As String = ""
    Private _bnd19 As String = ""
    Private _bnd20 As String = ""
    Private _bnd21 As String = ""
    Private _bnd22 As String = ""
    Private _bnd23 As String = ""
    Private _bnd24 As String = ""
    Private _bnd25 As String = ""
    Private _bnd26 As String = ""
    Private _qty01 As String = ""
    Private _qty02 As String = ""
    Private _qty03 As String = ""
    Private _qty04 As String = ""
    Private _qty05 As String = ""
    Private _qty06 As String = ""
    Private _qty07 As String = ""
    Private _qty08 As String = ""
    Private _qty09 As String = ""
    Private _qty10 As String = ""
    Private _qty11 As String = ""
    Private _qty12 As String = ""
    Private _qty13 As String = ""
    Private _qty14 As String = ""
    Private _qty15 As String = ""
    Private _qty16 As String = ""
    Private _qty17 As String = ""
    Private _qty18 As String = ""
    Private _qty19 As String = ""
    Private _qty20 As String = ""
    Private _qty21 As String = ""
    Private _qty22 As String = ""
    Private _qty23 As String = ""
    Private _qty24 As String = ""
    Private _qty25 As String = ""
    Private _qty26 As String = ""
    Private _rownsnum01 As String = ""
    Private _rownsnum02 As String = ""
    Private _rownsnum03 As String = ""
    Private _rownsnum04 As String = ""
    Private _rownsnum05 As String = ""
    Private _rownsnum06 As String = ""
    Private _rownsnum07 As String = ""
    Private _rownsnum08 As String = ""
    Private _rownsnum09 As String = ""
    Private _rownsnum10 As String = ""
    Private _rownsnum11 As String = ""
    Private _rownsnum12 As String = ""
    Private _rownsnum13 As String = ""
    Private _rownsnum14 As String = ""
    Private _rownsnum15 As String = ""
    Private _rownsnum16 As String = ""
    Private _rownsnum17 As String = ""
    Private _rownsnum18 As String = ""
    Private _rownsnum19 As String = ""
    Private _rownsnum20 As String = ""
    Private _rownsnum21 As String = ""
    Private _rownsnum22 As String = ""
    Private _rownsnum23 As String = ""
    Private _rownsnum24 As String = ""
    Private _rownsnum25 As String = ""
    Private _rownsnum26 As String = ""
    Private _rownsnum27 As String = ""
    Private _rownsnum28 As String = ""
    Private _rownsnum29 As String = ""
    Private _rownsnum30 As String = ""
    Private _rownsnum31 As String = ""
    Private _rownsnum32 As String = ""
    Private _rownsnum33 As String = ""
    Private _rownsnum34 As String = ""
    Private _rownsnum35 As String = ""
    Private _rownsnum36 As String = ""
    Private _rownsnum37 As String = ""
    Private _rownsnum38 As String = ""
    Private _rownsnum39 As String = ""
    Private _rownsnum40 As String = ""
    Private _rownsnum41 As String = ""
    Private _rownsnum42 As String = ""
    Private _rownsnum43 As String = ""
    Private _rownsnum44 As String = ""
    Private _rownsnum45 As String = ""
    Private _rownsnum46 As String = ""
    Private _rownsnum47 As String = ""
    Private _rownsnum48 As String = ""
    Private _rownsnum49 As String = ""
    Private _rownsnum50 As String = ""
    Private _failoption As String = ""
    Private _stadium1 As String = ""
    Private _stadium2 As String = ""
    Private _stadium3 As String = ""
    Private _stadium4 As String = ""
    Private _stadium5 As String = ""
    Private _stadium6 As String = ""
    Private _customer As String = ""
    Private _allocatedCustNo As String = ""
    Private _reservedSeats As String = ""
    Private _reservationCode As String = ""
    Private _src As String = ""
    Private _errorCode As String = ""
    Private _errorFlag As String = ""
    Private _excludeRestrictedSeats As String
    Private _reservationMethod As String
    Private _includeTravelProduct As String = String.Empty
    Private _row As String
    Private _seat As String
    Private _suffix As String
    Private _ignoreFriendsAndFamily As String
    Private _productType As String
    Private _signedInCustomer As String
    Private _byPassPreReqCheck As String
    Private _productDetailCode As String
    Private _packageID As Decimal = 0
    Private _seatComponentID As String = 0
    Private _updateMode As String = String.Empty
    Private _favouriteSeatSelected As Boolean = False
    Private _linkedParentProductCode As String = String.Empty
#End Region

#Region "Public Properties"

    Public Property SessionId() As String
        Get
            Return _sessionId
        End Get
        Set(ByVal value As String)
            _sessionId = value
        End Set
    End Property
    Public Property ProductCode() As String
        Get
            Return _productCode
        End Get
        Set(ByVal value As String)
            _productCode = value
        End Set
    End Property
    Public Property StandCode() As String
        Get
            Return _standCode
        End Get
        Set(ByVal value As String)
            _standCode = value
        End Set
    End Property
    Public Property AreaCode() As String
        Get
            Return _areaCode
        End Get
        Set(ByVal value As String)
            _areaCode = value
        End Set
    End Property
    Public Property PriceCode() As String
        Get
            Return _priceCode
        End Get
        Set(ByVal value As String)
            _priceCode = value
        End Set
    End Property
    Public Property CampaignCode() As String
        Get
            Return _campaignCode
        End Get
        Set(ByVal value As String)
            _campaignCode = value
        End Set
    End Property
    Public Property PriceBand01() As String
        Get
            Return _bnd01
        End Get
        Set(ByVal value As String)
            _bnd01 = value
        End Set
    End Property
    Public Property PriceBand02() As String
        Get
            Return _bnd02
        End Get
        Set(ByVal value As String)
            _bnd02 = value
        End Set
    End Property
    Public Property PriceBand03() As String
        Get
            Return _bnd03
        End Get
        Set(ByVal value As String)
            _bnd03 = value
        End Set
    End Property
    Public Property PriceBand04() As String
        Get
            Return _bnd04
        End Get
        Set(ByVal value As String)
            _bnd04 = value
        End Set
    End Property
    Public Property PriceBand05() As String
        Get
            Return _bnd05
        End Get
        Set(ByVal value As String)
            _bnd05 = value
        End Set
    End Property
    Public Property PriceBand06() As String
        Get
            Return _bnd06
        End Get
        Set(ByVal value As String)
            _bnd06 = value
        End Set
    End Property
    Public Property PriceBand07() As String
        Get
            Return _bnd07
        End Get
        Set(ByVal value As String)
            _bnd07 = value
        End Set
    End Property
    Public Property PriceBand08() As String
        Get
            Return _bnd08
        End Get
        Set(ByVal value As String)
            _bnd08 = value
        End Set
    End Property
    Public Property PriceBand09() As String
        Get
            Return _bnd09
        End Get
        Set(ByVal value As String)
            _bnd09 = value
        End Set
    End Property
    Public Property PriceBand10() As String
        Get
            Return _bnd10
        End Get
        Set(ByVal value As String)
            _bnd10 = value
        End Set
    End Property
    Public Property PriceBand11() As String
        Get
            Return _bnd11
        End Get
        Set(ByVal value As String)
            _bnd11 = value
        End Set
    End Property
    Public Property PriceBand12() As String
        Get
            Return _bnd12
        End Get
        Set(ByVal value As String)
            _bnd12 = value
        End Set
    End Property
    Public Property PriceBand13() As String
        Get
            Return _bnd13
        End Get
        Set(ByVal value As String)
            _bnd13 = value
        End Set
    End Property
    Public Property PriceBand14() As String
        Get
            Return _bnd14
        End Get
        Set(ByVal value As String)
            _bnd14 = value
        End Set
    End Property
    Public Property PriceBand15() As String
        Get
            Return _bnd15
        End Get
        Set(ByVal value As String)
            _bnd15 = value
        End Set
    End Property
    Public Property PriceBand16() As String
        Get
            Return _bnd16
        End Get
        Set(ByVal value As String)
            _bnd16 = value
        End Set
    End Property
    Public Property PriceBand17() As String
        Get
            Return _bnd17
        End Get
        Set(ByVal value As String)
            _bnd17 = value
        End Set
    End Property
    Public Property PriceBand18() As String
        Get
            Return _bnd18
        End Get
        Set(ByVal value As String)
            _bnd18 = value
        End Set
    End Property
    Public Property PriceBand19() As String
        Get
            Return _bnd19
        End Get
        Set(ByVal value As String)
            _bnd19 = value
        End Set
    End Property
    Public Property PriceBand20() As String
        Get
            Return _bnd20
        End Get
        Set(ByVal value As String)
            _bnd20 = value
        End Set
    End Property
    Public Property PriceBand21() As String
        Get
            Return _bnd21
        End Get
        Set(ByVal value As String)
            _bnd21 = value
        End Set
    End Property
    Public Property PriceBand22() As String
        Get
            Return _bnd22
        End Get
        Set(ByVal value As String)
            _bnd22 = value
        End Set
    End Property
    Public Property PriceBand23() As String
        Get
            Return _bnd23
        End Get
        Set(ByVal value As String)
            _bnd23 = value
        End Set
    End Property
    Public Property PriceBand24() As String
        Get
            Return _bnd24
        End Get
        Set(ByVal value As String)
            _bnd24 = value
        End Set
    End Property
    Public Property PriceBand25() As String
        Get
            Return _bnd25
        End Get
        Set(ByVal value As String)
            _bnd25 = value
        End Set
    End Property
    Public Property PriceBand26() As String
        Get
            Return _bnd26
        End Get
        Set(ByVal value As String)
            _bnd26 = value
        End Set
    End Property
    Public Property Quantity01() As String
        Get
            Return _qty01
        End Get
        Set(ByVal value As String)
            _qty01 = value
        End Set
    End Property
    Public Property Quantity02() As String
        Get
            Return _qty02
        End Get
        Set(ByVal value As String)
            _qty02 = value
        End Set
    End Property
    Public Property Quantity03() As String
        Get
            Return _qty03
        End Get
        Set(ByVal value As String)
            _qty03 = value
        End Set
    End Property
    Public Property Quantity04() As String
        Get
            Return _qty04
        End Get
        Set(ByVal value As String)
            _qty04 = value
        End Set
    End Property
    Public Property Quantity05() As String
        Get
            Return _qty05
        End Get
        Set(ByVal value As String)
            _qty05 = value
        End Set
    End Property
    Public Property Quantity06() As String
        Get
            Return _qty06
        End Get
        Set(ByVal value As String)
            _qty06 = value
        End Set
    End Property
    Public Property Quantity07() As String
        Get
            Return _qty07
        End Get
        Set(ByVal value As String)
            _qty07 = value
        End Set
    End Property
    Public Property Quantity08() As String
        Get
            Return _qty08
        End Get
        Set(ByVal value As String)
            _qty08 = value
        End Set
    End Property
    Public Property Quantity09() As String
        Get
            Return _qty09
        End Get
        Set(ByVal value As String)
            _qty09 = value
        End Set
    End Property
    Public Property Quantity10() As String
        Get
            Return _qty10
        End Get
        Set(ByVal value As String)
            _qty10 = value
        End Set
    End Property
    Public Property Quantity11() As String
        Get
            Return _qty11
        End Get
        Set(ByVal value As String)
            _qty11 = value
        End Set
    End Property
    Public Property Quantity12() As String
        Get
            Return _qty12
        End Get
        Set(ByVal value As String)
            _qty12 = value
        End Set
    End Property
    Public Property Quantity13() As String
        Get
            Return _qty13
        End Get
        Set(ByVal value As String)
            _qty13 = value
        End Set
    End Property
    Public Property Quantity14() As String
        Get
            Return _qty14
        End Get
        Set(ByVal value As String)
            _qty14 = value
        End Set
    End Property
    Public Property Quantity15() As String
        Get
            Return _qty15
        End Get
        Set(ByVal value As String)
            _qty15 = value
        End Set
    End Property
    Public Property Quantity16() As String
        Get
            Return _qty16
        End Get
        Set(ByVal value As String)
            _qty16 = value
        End Set
    End Property
    Public Property Quantity17() As String
        Get
            Return _qty17
        End Get
        Set(ByVal value As String)
            _qty17 = value
        End Set
    End Property
    Public Property Quantity18() As String
        Get
            Return _qty18
        End Get
        Set(ByVal value As String)
            _qty18 = value
        End Set
    End Property
    Public Property Quantity19() As String
        Get
            Return _qty19
        End Get
        Set(ByVal value As String)
            _qty19 = value
        End Set
    End Property
    Public Property Quantity20() As String
        Get
            Return _qty20
        End Get
        Set(ByVal value As String)
            _qty20 = value
        End Set
    End Property
    Public Property Quantity21() As String
        Get
            Return _qty21
        End Get
        Set(ByVal value As String)
            _qty21 = value
        End Set
    End Property
    Public Property Quantity22() As String
        Get
            Return _qty22
        End Get
        Set(ByVal value As String)
            _qty22 = value
        End Set
    End Property
    Public Property Quantity23() As String
        Get
            Return _qty23
        End Get
        Set(ByVal value As String)
            _qty23 = value
        End Set
    End Property
    Public Property Quantity24() As String
        Get
            Return _qty24
        End Get
        Set(ByVal value As String)
            _qty24 = value
        End Set
    End Property
    Public Property Quantity25() As String
        Get
            Return _qty25
        End Get
        Set(ByVal value As String)
            _qty25 = value
        End Set
    End Property
    Public Property Quantity26() As String
        Get
            Return _qty26
        End Get
        Set(ByVal value As String)
            _qty26 = value
        End Set
    End Property
    Public Property RowSeat01() As String
        Get
            Return _rownsnum01
        End Get
        Set(ByVal value As String)
            _rownsnum01 = value
        End Set
    End Property
    Public Property RowSeat02() As String
        Get
            Return _rownsnum02
        End Get
        Set(ByVal value As String)
            _rownsnum02 = value
        End Set
    End Property
    Public Property RowSeat03() As String
        Get
            Return _rownsnum03
        End Get
        Set(ByVal value As String)
            _rownsnum03 = value
        End Set
    End Property
    Public Property RowSeat04() As String
        Get
            Return _rownsnum04
        End Get
        Set(ByVal value As String)
            _rownsnum04 = value
        End Set
    End Property
    Public Property RowSeat05() As String
        Get
            Return _rownsnum05
        End Get
        Set(ByVal value As String)
            _rownsnum05 = value
        End Set
    End Property
    Public Property RowSeat06() As String
        Get
            Return _rownsnum06
        End Get
        Set(ByVal value As String)
            _rownsnum06 = value
        End Set
    End Property
    Public Property RowSeat07() As String
        Get
            Return _rownsnum07
        End Get
        Set(ByVal value As String)
            _rownsnum07 = value
        End Set
    End Property
    Public Property RowSeat08() As String
        Get
            Return _rownsnum08
        End Get
        Set(ByVal value As String)
            _rownsnum08 = value
        End Set
    End Property
    Public Property RowSeat09() As String
        Get
            Return _rownsnum09
        End Get
        Set(ByVal value As String)
            _rownsnum09 = value
        End Set
    End Property
    Public Property RowSeat10() As String
        Get
            Return _rownsnum10
        End Get
        Set(ByVal value As String)
            _rownsnum10 = value
        End Set
    End Property
    Public Property RowSeat11() As String
        Get
            Return _rownsnum11
        End Get
        Set(ByVal value As String)
            _rownsnum11 = value
        End Set
    End Property
    Public Property RowSeat12() As String
        Get
            Return _rownsnum12
        End Get
        Set(ByVal value As String)
            _rownsnum12 = value
        End Set
    End Property
    Public Property RowSeat13() As String
        Get
            Return _rownsnum13
        End Get
        Set(ByVal value As String)
            _rownsnum13 = value
        End Set
    End Property
    Public Property RowSeat14() As String
        Get
            Return _rownsnum14
        End Get
        Set(ByVal value As String)
            _rownsnum14 = value
        End Set
    End Property
    Public Property RowSeat15() As String
        Get
            Return _rownsnum15
        End Get
        Set(ByVal value As String)
            _rownsnum15 = value
        End Set
    End Property
    Public Property RowSeat16() As String
        Get
            Return _rownsnum16
        End Get
        Set(ByVal value As String)
            _rownsnum16 = value
        End Set
    End Property
    Public Property RowSeat17() As String
        Get
            Return _rownsnum17
        End Get
        Set(ByVal value As String)
            _rownsnum17 = value
        End Set
    End Property
    Public Property RowSeat18() As String
        Get
            Return _rownsnum18
        End Get
        Set(ByVal value As String)
            _rownsnum18 = value
        End Set
    End Property
    Public Property RowSeat19() As String
        Get
            Return _rownsnum19
        End Get
        Set(ByVal value As String)
            _rownsnum19 = value
        End Set
    End Property
    Public Property RowSeat20() As String
        Get
            Return _rownsnum20
        End Get
        Set(ByVal value As String)
            _rownsnum20 = value
        End Set
    End Property
    Public Property RowSeat21() As String
        Get
            Return _rownsnum21
        End Get
        Set(ByVal value As String)
            _rownsnum21 = value
        End Set
    End Property
    Public Property RowSeat22() As String
        Get
            Return _rownsnum22
        End Get
        Set(ByVal value As String)
            _rownsnum22 = value
        End Set
    End Property
    Public Property RowSeat23() As String
        Get
            Return _rownsnum23
        End Get
        Set(ByVal value As String)
            _rownsnum23 = value
        End Set
    End Property
    Public Property RowSeat24() As String
        Get
            Return _rownsnum24
        End Get
        Set(ByVal value As String)
            _rownsnum24 = value
        End Set
    End Property
    Public Property RowSeat25() As String
        Get
            Return _rownsnum25
        End Get
        Set(ByVal value As String)
            _rownsnum25 = value
        End Set
    End Property
    Public Property RowSeat26() As String
        Get
            Return _rownsnum26
        End Get
        Set(ByVal value As String)
            _rownsnum26 = value
        End Set
    End Property
    Public Property RowSeat27() As String
        Get
            Return _rownsnum27
        End Get
        Set(ByVal value As String)
            _rownsnum27 = value
        End Set
    End Property
    Public Property RowSeat28() As String
        Get
            Return _rownsnum28
        End Get
        Set(ByVal value As String)
            _rownsnum28 = value
        End Set
    End Property
    Public Property RowSeat29() As String
        Get
            Return _rownsnum29
        End Get
        Set(ByVal value As String)
            _rownsnum29 = value
        End Set
    End Property
    Public Property RowSeat30() As String
        Get
            Return _rownsnum30
        End Get
        Set(ByVal value As String)
            _rownsnum30 = value
        End Set
    End Property
    Public Property RowSeat31() As String
        Get
            Return _rownsnum31
        End Get
        Set(ByVal value As String)
            _rownsnum31 = value
        End Set
    End Property
    Public Property RowSeat32() As String
        Get
            Return _rownsnum32
        End Get
        Set(ByVal value As String)
            _rownsnum32 = value
        End Set
    End Property
    Public Property RowSeat33() As String
        Get
            Return _rownsnum33
        End Get
        Set(ByVal value As String)
            _rownsnum33 = value
        End Set
    End Property
    Public Property RowSeat34() As String
        Get
            Return _rownsnum34
        End Get
        Set(ByVal value As String)
            _rownsnum34 = value
        End Set
    End Property
    Public Property RowSeat35() As String
        Get
            Return _rownsnum35
        End Get
        Set(ByVal value As String)
            _rownsnum35 = value
        End Set
    End Property
    Public Property RowSeat36() As String
        Get
            Return _rownsnum36
        End Get
        Set(ByVal value As String)
            _rownsnum36 = value
        End Set
    End Property
    Public Property RowSeat37() As String
        Get
            Return _rownsnum37
        End Get
        Set(ByVal value As String)
            _rownsnum37 = value
        End Set
    End Property
    Public Property RowSeat38() As String
        Get
            Return _rownsnum38
        End Get
        Set(ByVal value As String)
            _rownsnum38 = value
        End Set
    End Property
    Public Property RowSeat39() As String
        Get
            Return _rownsnum39
        End Get
        Set(ByVal value As String)
            _rownsnum39 = value
        End Set
    End Property
    Public Property RowSeat40() As String
        Get
            Return _rownsnum40
        End Get
        Set(ByVal value As String)
            _rownsnum40 = value
        End Set
    End Property
    Public Property RowSeat41() As String
        Get
            Return _rownsnum41
        End Get
        Set(ByVal value As String)
            _rownsnum41 = value
        End Set
    End Property
    Public Property RowSeat42() As String
        Get
            Return _rownsnum42
        End Get
        Set(ByVal value As String)
            _rownsnum42 = value
        End Set
    End Property
    Public Property RowSeat43() As String
        Get
            Return _rownsnum43
        End Get
        Set(ByVal value As String)
            _rownsnum43 = value
        End Set
    End Property
    Public Property RowSeat44() As String
        Get
            Return _rownsnum44
        End Get
        Set(ByVal value As String)
            _rownsnum44 = value
        End Set
    End Property
    Public Property RowSeat45() As String
        Get
            Return _rownsnum45
        End Get
        Set(ByVal value As String)
            _rownsnum45 = value
        End Set
    End Property
    Public Property RowSeat46() As String
        Get
            Return _rownsnum46
        End Get
        Set(ByVal value As String)
            _rownsnum46 = value
        End Set
    End Property
    Public Property RowSeat47() As String
        Get
            Return _rownsnum47
        End Get
        Set(ByVal value As String)
            _rownsnum47 = value
        End Set
    End Property
    Public Property RowSeat48() As String
        Get
            Return _rownsnum48
        End Get
        Set(ByVal value As String)
            _rownsnum48 = value
        End Set
    End Property
    Public Property RowSeat49() As String
        Get
            Return _rownsnum49
        End Get
        Set(ByVal value As String)
            _rownsnum49 = value
        End Set
    End Property
    Public Property RowSeat50() As String
        Get
            Return _rownsnum50
        End Get
        Set(ByVal value As String)
            _rownsnum50 = value
        End Set
    End Property
    Public Property FailOption() As String
        Get
            Return _failoption
        End Get
        Set(ByVal value As String)
            _failoption = value
        End Set
    End Property
    Public Property Stadium1() As String
        Get
            Return _stadium1
        End Get
        Set(ByVal value As String)
            _stadium1 = value
        End Set
    End Property
    Public Property Stadium2() As String
        Get
            Return _stadium2
        End Get
        Set(ByVal value As String)
            _stadium2 = value
        End Set
    End Property
    Public Property Stadium3() As String
        Get
            Return _stadium3
        End Get
        Set(ByVal value As String)
            _stadium3 = value
        End Set
    End Property
    Public Property Stadium4() As String
        Get
            Return _stadium4
        End Get
        Set(ByVal value As String)
            _stadium4 = value
        End Set
    End Property
    Public Property Stadium5() As String
        Get
            Return _stadium5
        End Get
        Set(ByVal value As String)
            _stadium5 = value
        End Set
    End Property
    Public Property Stadium6() As String
        Get
            Return _stadium6
        End Get
        Set(ByVal value As String)
            _stadium6 = value
        End Set
    End Property
    Public Property ReservedSeats() As String
        Get
            Return _reservedSeats
        End Get
        Set(ByVal value As String)
            _reservedSeats = value
        End Set
    End Property
    Public Property ReservationCode() As String
        Get
            Return _reservationCode
        End Get
        Set(ByVal value As String)
            _reservationCode = value
        End Set
    End Property
    Public Property CustomerNumber() As String
        Get
            Return _customer
        End Get
        Set(ByVal value As String)
            _customer = value
        End Set
    End Property
    Public Property AllocatedCustNo() As String
        Get
            Return _allocatedCustNo
        End Get
        Set(ByVal value As String)
            _allocatedCustNo = value
        End Set
    End Property
    Public Property ExcludeRestrictedSeats() As String
        Get
            Return _excludeRestrictedSeats
        End Get
        Set(ByVal value As String)
            _excludeRestrictedSeats = value
        End Set
    End Property
    Public Property Source() As String
        Get
            Return _src
        End Get
        Set(ByVal value As String)
            _src = value
        End Set
    End Property
    Public Property ErrorCode() As String
        Get
            Return _errorCode
        End Get
        Set(ByVal value As String)
            _errorCode = value
        End Set
    End Property
    Public Property ErrorFlag() As String
        Get
            Return _errorFlag
        End Get
        Set(ByVal value As String)
            _errorFlag = value
        End Set
    End Property
    Public Property IncludeTravelProduct() As String
        Get
            Return _includeTravelProduct
        End Get
        Set(ByVal value As String)
            _includeTravelProduct = value
        End Set
    End Property
    Public Property Row() As String
        Get
            Return _row
        End Get
        Set(ByVal value As String)
            _row = value
        End Set
    End Property
    Public Property Seat() As String
        Get
            Return _seat
        End Get
        Set(ByVal value As String)
            _seat = value
        End Set
    End Property
    Public Property Suffix() As String
        Get
            Return _suffix
        End Get
        Set(ByVal value As String)
            _suffix = value
        End Set
    End Property
    Public Property IgnoreFriendsAndFamily() As String
        Get
            Return _ignoreFriendsAndFamily
        End Get
        Set(ByVal value As String)
            _ignoreFriendsAndFamily = value
        End Set
    End Property
    Public Property ReservationMethod() As String
        Get
            Return _reservationMethod
        End Get
        Set(ByVal value As String)
            _reservationMethod = value
        End Set
    End Property
    Public Property ProductType() As String
        Get
            Return _productType
        End Get
        Set(ByVal value As String)
            _productType = value
        End Set
    End Property
    Public Property SignedInCustomer() As String
        Get
            Return _signedInCustomer
        End Get
        Set(ByVal value As String)
            _signedInCustomer = value
        End Set
    End Property
    Public Property ByPassPreReqCheck() As String
        Get
            Return _byPassPreReqCheck
        End Get
        Set(ByVal value As String)
            _byPassPreReqCheck = value
        End Set
    End Property
    Public Property ProductDetailCode() As String
        Get
            Return _productDetailCode
        End Get
        Set(ByVal value As String)
            _productDetailCode = value
        End Set
    End Property
    Public Property PackageID() As Decimal
        Get
            Return _packageID
        End Get
        Set(ByVal value As Decimal)
            _packageID = value
        End Set
    End Property
    Public Property SeatComponentID() As String
        Get
            Return _seatComponentID
        End Get
        Set(ByVal value As String)
            _seatComponentID = value
        End Set
    End Property
    Public Property UpdateMode() As String
        Get
            Return _updateMode
        End Get
        Set(ByVal value As String)
            _updateMode = value
        End Set
    End Property
    Public Property FavouriteSeatSelected() As Boolean
        Get
            Return _favouriteSeatSelected
        End Get
        Set(ByVal value As Boolean)
            _favouriteSeatSelected = value
        End Set
    End Property
    Public Property LinkedParentProductCode() As String
        Get
            Return _linkedParentProductCode
        End Get
        Set(ByVal value As String)
            _linkedParentProductCode = value
        End Set
    End Property
    Public Property CATMode() As String = String.Empty
    Public Property CATSeatDetails() As String = String.Empty
    Public Property CATSeatCustomerNo() As String = String.Empty
    Public Property CATPayRef() As String = String.Empty
    Public Property ProductHasMandtoryRelatedProducts() As Boolean = False
    Public Property ProductIsMandatory() As Boolean = False
    Public Property DefaultPrice() As Integer = 0
    Public Property SmartcardNumber() As String = String.Empty
    Public Property SmartcardAmount() As String = String.Empty
    Public Property SeatSelectionArray() As New List(Of DESeatDetails)
    Public Property TicketText() As String = String.Empty
    Public Property CallID() As Long = 0
    Public Property LinkedProductID() As Integer = 0
    Public Property LinkedMasterProduct() As String = String.Empty
    Public Property AllMandatoryLinkedProductsAdded() As Boolean = False
    Public Property BulkSalesId() As Integer = 0
    Public Property STExceptionSeasonTicketProductCode() As String = String.Empty
    Public Property STExceptionChangeRemoveMode() As String = String.Empty
    Public Property VariablePricedProductPrice As Integer = 0
    Public Property IncludeTicketExchangeSeats As Boolean = False
    Public Property SelectedPriceMinimum As Decimal = 0
    Public Property SelectedPriceMaximum As Decimal = 0
    Public Property SelectedPriceBreakId As Long = 0
    Public Property AgentCanGiveDirectDebitRefund As Boolean = False
    Public Property ProductTransactionTicketLimitExceeded As Boolean = False
    Public Property ProductTransactionTicketLimit As Integer = 0
    Public Property PickingNewComponentSeat As Boolean = False

#End Region

#Region "Public Functions"

    Public Function LogString() As String
        Dim sb As New System.Text.StringBuilder
        With sb
            .Append(_sessionId & ",")
            .Append(_productCode & ",")
            .Append(_standCode & ",")
            .Append(_areaCode & ",")
            .Append(_priceCode & ",")
            .Append(_campaignCode & ",")
            .Append(_bnd01 & ",")
            .Append(_bnd02 & ",")
            .Append(_bnd03 & ",")
            .Append(_bnd04 & ",")
            .Append(_bnd05 & ",")
            .Append(_bnd06 & ",")
            .Append(_bnd07 & ",")
            .Append(_bnd08 & ",")
            .Append(_bnd09 & ",")
            .Append(_bnd10 & ",")
            .Append(_bnd11 & ",")
            .Append(_bnd12 & ",")
            .Append(_bnd13 & ",")
            .Append(_bnd14 & ",")
            .Append(_bnd15 & ",")
            .Append(_bnd16 & ",")
            .Append(_bnd17 & ",")
            .Append(_bnd18 & ",")
            .Append(_bnd19 & ",")
            .Append(_bnd20 & ",")
            .Append(_bnd21 & ",")
            .Append(_bnd22 & ",")
            .Append(_bnd23 & ",")
            .Append(_bnd24 & ",")
            .Append(_bnd25 & ",")
            .Append(_bnd26 & ",")
            .Append(_qty01 & ",")
            .Append(_qty02 & ",")
            .Append(_qty03 & ",")
            .Append(_qty04 & ",")
            .Append(_qty05 & ",")
            .Append(_qty06 & ",")
            .Append(_qty07 & ",")
            .Append(_qty08 & ",")
            .Append(_qty09 & ",")
            .Append(_qty10 & ",")
            .Append(_qty11 & ",")
            .Append(_qty12 & ",")
            .Append(_qty13 & ",")
            .Append(_qty14 & ",")
            .Append(_qty15 & ",")
            .Append(_qty16 & ",")
            .Append(_qty17 & ",")
            .Append(_qty18 & ",")
            .Append(_qty19 & ",")
            .Append(_qty20 & ",")
            .Append(_qty21 & ",")
            .Append(_qty22 & ",")
            .Append(_qty23 & ",")
            .Append(_qty24 & ",")
            .Append(_qty25 & ",")
            .Append(_qty26 & ",")
            .Append(_rownsnum01 & ",")
            .Append(_rownsnum02 & ",")
            .Append(_rownsnum03 & ",")
            .Append(_rownsnum04 & ",")
            .Append(_rownsnum05 & ",")
            .Append(_rownsnum06 & ",")
            .Append(_rownsnum07 & ",")
            .Append(_rownsnum08 & ",")
            .Append(_rownsnum09 & ",")
            .Append(_rownsnum10 & ",")
            .Append(_rownsnum11 & ",")
            .Append(_rownsnum12 & ",")
            .Append(_rownsnum13 & ",")
            .Append(_rownsnum14 & ",")
            .Append(_rownsnum15 & ",")
            .Append(_rownsnum16 & ",")
            .Append(_rownsnum17 & ",")
            .Append(_rownsnum18 & ",")
            .Append(_rownsnum19 & ",")
            .Append(_rownsnum20 & ",")
            .Append(_rownsnum21 & ",")
            .Append(_rownsnum22 & ",")
            .Append(_rownsnum23 & ",")
            .Append(_rownsnum24 & ",")
            .Append(_rownsnum25 & ",")
            .Append(_rownsnum26 & ",")
            .Append(_rownsnum27 & ",")
            .Append(_rownsnum28 & ",")
            .Append(_rownsnum29 & ",")
            .Append(_rownsnum30 & ",")
            .Append(_rownsnum31 & ",")
            .Append(_rownsnum32 & ",")
            .Append(_rownsnum33 & ",")
            .Append(_rownsnum34 & ",")
            .Append(_rownsnum35 & ",")
            .Append(_rownsnum36 & ",")
            .Append(_rownsnum37 & ",")
            .Append(_rownsnum38 & ",")
            .Append(_rownsnum39 & ",")
            .Append(_rownsnum40 & ",")
            .Append(_rownsnum41 & ",")
            .Append(_rownsnum42 & ",")
            .Append(_rownsnum43 & ",")
            .Append(_rownsnum44 & ",")
            .Append(_rownsnum45 & ",")
            .Append(_rownsnum46 & ",")
            .Append(_rownsnum47 & ",")
            .Append(_rownsnum48 & ",")
            .Append(_rownsnum49 & ",")
            .Append(_rownsnum50 & ",")
            .Append(_failoption & ",")
            .Append(_stadium1 & ",")
            .Append(_stadium2 & ",")
            .Append(_stadium3 & ",")
            .Append(_stadium4 & ",")
            .Append(_stadium5 & ",")
            .Append(_stadium6 & ",")
            .Append(_customer & ",")
            .Append(_excludeRestrictedSeats & ",")
            .Append(_reservedSeats & ",")
            .Append(_reservationMethod & ",")
            .Append(_updateMode & ",")
            .Append(CATMode & ",")
            .Append(CATSeatDetails & ",")
            .Append(CATSeatCustomerNo & ",")
            .Append(_src & ",")
            .Append(_errorCode & ",")
            .Append(_errorFlag)
        End With
        Return sb.ToString.Trim
    End Function

#End Region

End Class