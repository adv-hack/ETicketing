Imports System
Imports System.Collections.Generic
Imports System.Dynamic
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Text

<Serializable()> _
Public Class DEPackages

    Public Property ProductCode As String
    Public Property TicketingProductCode As String = ""
    Public Property ComponentID As Int64
    Public Property ComponentGroupID As Int64
    Public Property Mode As OperationMode
    Public Property ComponentGroupDescription As String
    Public Property ComponentType As String
    Public Property DisplayType As String
    Public Property AreaInComponents As New List(Of AreaInComponent)
    Public Property ComponentDefaults As New List(Of ComponentDefault)
    Public Property PackageID As Long
    Public Property BasketId As String
    Public Property SessionID As String
    Public Property BoxOfficeUser As String
    Public Property AmendComponents As New List(Of AmendComponent)
    Public Property SeatAllocations As New List(Of SeatAllocation)
    Public Property Fromdate As Date
    Public Property ToDate As Date
    Public Property StandCode As String
    Public Property OtherTAComponentAreas As New List(Of OtherTAComponentArea)
    Public Property TAComponentArea2 As New TAComponentArea2
    Public Property ComponentCode() As String
    Public Property QuantityToSelect() As Integer
    Public Property ComponentSequence() As Integer
    Public Property MarkAsCompleted() As Boolean = False
    Public Property Sequence As SequenceMode
    Public Property Proceed() As Boolean = False
    Public Property Discount As Decimal = 0.0
    Public Property UpdateDiscount As Boolean = False
    Public Property RemoveAllDiscounts As Boolean = False
    Public Property AllAvailableDates() As Boolean = False
    Public Property StartDayAdjustment() As New DateAdjustment
    Public Property EndDayAdjustment() As New DateAdjustment
    Public Property ExtraDaysChargeable() As Boolean = False
    Public Property DiscountOnDailyRate() As Integer = 0
    Public Property AvailableToSell03() As Boolean = False
    Public Property AvailableToSellAvailableTickets() As Boolean = False
    Public Property QuantityArea() As String = String.Empty
    Public Property IsCATBasket() As Boolean = False
    Public Property NoneReqOpt() As String
    Public Property LeadSourceID() As String
    Public Property PackageDiscountedByValue() As Decimal
    Public Property PackageComponentLevelDiscountValue() As Decimal
    Public Property CallId As Long
    Public Property HospitalityBookingFilters() As HospitalityBookingFilters
    Public Property Source As String
    Public Property Status As String
    Public Property MarkOrderFor() As String
    Public Property SeatToBePrinted() As String
    Public Property ProductCodeToBePrinted() As String
    Public Property CustomerNumber() As String
    Public Property BusinessUnit() As String = String.Empty
End Class

Public Enum OperationMode
    Add
    Edit
    Delete
    Amend
    Proceed
    StartAgain
    Sequence
    Cancel
    Extra
End Enum

Public Enum SequenceMode
    MovingUp
    MovingDown
End Enum

Public Enum NoneReqOptMode
    first
    last
    notDisplayed
End Enum

#Region "WS081R and WS080R Classes"
Public Class AreaInComponent
    Public Property Area As String
    'Public Property IsAreaActive As Boolean
End Class

Public Class ComponentDefault
    Public Property IsDefault As Boolean
    Public Property DefaultFrom As Integer
    Public Property DefaultTo As Integer
    Public Property Group As DefaultType
End Class
#End Region

#Region "WS072R Classes"
Public Class AmendComponent
    Public Property ComponentId As Int64
    Public Property ComponentGroupId As Int64
    Public Property Quantity As Integer
    Public Property Discount As Decimal
End Class

Public Class SeatAllocation
    Public Property ComponentId As Int64
    Public Property ProductCode As String
    Public Property Seat As String
    Public Property AlphaSuffix As String
    Public Property PriceBand As String
    Public Property PriceCode As String
    Public Property CustomerNumber As String
    Public Property Action As OperationMode
    Public Property BulkId As Long
    Public Property Quantity As Integer
End Class
#End Region

#Region "WS073R Classes"
Public Class PriceBandAndQuantity
    Public Property PriceBand As String
    Public Property Quantity As Integer
End Class

Public Class TAComponentArea2
    Public Property Area As String
    Public Property PriceBandAndQuantities As New List(Of PriceBandAndQuantity)
End Class

Public Class OtherTAComponentArea
    Public Property Area As String
    Public Property PriceBandAndQuantity As New PriceBandAndQuantity
End Class

#End Region

Public Class ComponentGroups
    Public Const TA As String = "Travel & Accomodation"
    Public Const CG As String = "Component Group"
End Class

Public Enum DefaultType
    PerGroup
    PerTicket
End Enum

Public Class DateAdjustment
    Public Property DayAdjustment() As String
    Public Property Days() As Integer = 0
End Class

Public Class HospitalityBookingFilters
    Public Property Agent As String
    Public Property CallId As Decimal
    Public Property Fromdate As Decimal
    Public Property ToDate As Decimal
    Public Property Status As String
    Public Property Customer As String
    Public Property PackageDescription As String
    Public Property ProductDescription As String
    Public Property MarkOrderFor As String
    Public Property MaxRecords As Decimal
    Public Property QandAStatus As String
    Public Property PrintStatus As String
End Class

