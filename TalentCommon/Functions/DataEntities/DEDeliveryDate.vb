Public Class DEDeliveryDate


    Private _date As Date
    Public Property CurrentDate() As Date
        Get
            Return _date
        End Get
        Set(ByVal value As Date)
            _date = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the number of days from today when all items are instock
    ''' </summary>
    ''' <value>
    ''' The days until all instock
    ''' </value>
    Property StockLeadTime As Integer
    ''' <summary>
    ''' Days which home delivery is available MTWTHSS
    ''' </summary>
    ''' <value>
    ''' Home delivery Days
    ''' </value>
    Property HomeDeliveryDays As String
    ''' <summary>
    ''' Gets or sets whether the order is for Home Delivery
    ''' </summary>
    ''' <value>
    ''' Home delivery True/False
    ''' </value>
    Property HomeDelivery As Boolean


    ''' <summary>
    ''' Gets or sets whether the Carrier Code
    ''' </summary>
    ''' <value>
    ''' Carrier Code
    ''' </value>
    Property CarrierCode As String

    Private _daysWithinPreferredDate As Integer
    ''' <summary>
    ''' Gets or sets the number of days from today to get the available preferred delivery dates.
    ''' </summary>
    ''' <value>
    ''' The days within preferred delivery date.
    ''' </value>
    Public Property DaysWithinPreferredDate() As Integer
        Get
            Return _daysWithinPreferredDate
        End Get
        Set(ByVal value As Integer)
            _daysWithinPreferredDate = value
        End Set
    End Property

    Sub New(ByVal _currentDate As Date)
        MyBase.New()

        Me.CurrentDate = _currentDate
        Me.HomeDelivery = False
    End Sub


End Class
