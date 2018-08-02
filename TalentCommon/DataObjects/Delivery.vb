Imports Talent.Common.DataObjects.TableObjects

Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access Delivery tables objects
    ''' </summary>
    <Serializable()> _
    Public Class Delivery

#Region "Class Level Fields"

        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings
        Private _tblCarrier As tbl_carrier
        Private _tblDeliveryUnavailableDates As tbl_delivery_unavailable_dates

#End Region

#Region "Constructors"

        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Delivery" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Create and Gets the tbl_carrier instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblCarrier() As tbl_carrier
            Get
                If (_tblCarrier Is Nothing) Then
                    _tblCarrier = New tbl_carrier(_settings)
                End If
                Return _tblCarrier
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_delivery_unavailable_dates instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblDeliveryUnavailableDates() As tbl_delivery_unavailable_dates
            Get
                If (_tblDeliveryUnavailableDates Is Nothing) Then
                    _tblDeliveryUnavailableDates = New tbl_delivery_unavailable_dates(_settings)
                End If
                Return _tblDeliveryUnavailableDates
            End Get
        End Property

#End Region

    End Class

End Namespace