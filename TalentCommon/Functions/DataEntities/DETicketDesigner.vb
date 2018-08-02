''' <summary>
''' Class which holds all the Ticket Designer related data entities in the form of generic list
''' </summary>
<Serializable()> _
Public Class DETicketDesigner

#Region "Class Level Fields"
    Private _labelProperties As Generic.List(Of DELabelProperties)
    Private _fieldProperties As Generic.List(Of DEFieldProperties)
    Private _fontsLaser As Generic.List(Of DEFontsLaser)
    Private _fontSimulate As Generic.List(Of DEFontSimulate)
#End Region

#Region "Properties"
    ''' <summary>
    ''' Gets or sets the label properties.
    ''' </summary>
    ''' <value>The label properties.</value>
    Public Property LabelProperties() As Generic.List(Of DELabelProperties)
        Get
            Return _labelProperties
        End Get
        Set(ByVal value As Generic.List(Of DELabelProperties))
            _labelProperties = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the field properties.
    ''' </summary>
    ''' <value>The field properties.</value>
    Public Property FieldProperties() As Generic.List(Of DEFieldProperties)
        Get
            Return _fieldProperties
        End Get
        Set(ByVal value As Generic.List(Of DEFieldProperties))
            _fieldProperties = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the fonts laser.
    ''' </summary>
    ''' <value>The fonts laser.</value>
    Public Property FontsLaser() As Generic.List(Of DEFontsLaser)
        Get
            Return _fontsLaser
        End Get
        Set(ByVal value As Generic.List(Of DEFontsLaser))
            _fontsLaser = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the font simulate.
    ''' </summary>
    ''' <value>The font simulate.</value>
    Public Property FontSimulate() As Generic.List(Of DEFontSimulate)
        Get
            Return _fontSimulate
        End Get
        Set(ByVal value As Generic.List(Of DEFontSimulate))
            _fontSimulate = value
        End Set
    End Property
#End Region
End Class
