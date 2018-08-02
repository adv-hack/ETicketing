Imports System.Xml.Serialization
''' <summary>
''' Data Entity class for tbl_td_labelproperties
''' </summary>
''' <remarks>
''' All Element names are matched with column names 
''' </remarks>
<Serializable()> _
Public Class DELabelProperties
    Private _labellName As String
    <XmlElement(ElementName:="LABELLNAME")> _
    Public Property LabellName() As String
        Get
            Return _labellName
        End Get
        Set(ByVal value As String)
            _labellName = value
        End Set
    End Property
    Private _labelWidth As String
    <XmlElement(ElementName:="LABELWIDTH")> _
    Public Property LabelWidth() As String
        Get
            Return _labelWidth
        End Get
        Set(ByVal value As String)
            _labelWidth = value
        End Set
    End Property
    Private _labelHeight As String
    <XmlElement(ElementName:="LABELHEIGHT")> _
    Public Property LabelHeight() As String
        Get
            Return _labelHeight
        End Get
        Set(ByVal value As String)
            _labelHeight = value
        End Set
    End Property
    Private _horShift As String
    <XmlElement(ElementName:="HORSHIFT")> _
    Public Property HorShift() As String
        Get
            Return _horShift
        End Get
        Set(ByVal value As String)
            _horShift = value
        End Set
    End Property
    Private _verShift As String
    <XmlElement(ElementName:="VERSHIFT")> _
    Public Property VerShift() As String
        Get
            Return _verShift
        End Get
        Set(ByVal value As String)
            _verShift = value
        End Set
    End Property
    Private _hOffset As String
    <XmlElement(ElementName:="HOFFSET")> _
    Public Property HOffset() As String
        Get
            Return _hOffset
        End Get
        Set(ByVal value As String)
            _hOffset = value
        End Set
    End Property
    Private _vOffset As String
    <XmlElement(ElementName:="VOFFSET")> _
    Public Property VOffset() As String
        Get
            Return _vOffset
        End Get
        Set(ByVal value As String)
            _vOffset = value
        End Set
    End Property
    Private _formOffset As String
    <XmlElement(ElementName:="FORMOFFSET")> _
    Public Property FormOffset() As String
        Get
            Return _formOffset
        End Get
        Set(ByVal value As String)
            _formOffset = value
        End Set
    End Property
    Private _nrRow As String
    <XmlElement(ElementName:="NRROW")> _
    Public Property NrRow() As String
        Get
            Return _nrRow
        End Get
        Set(ByVal value As String)
            _nrRow = value
        End Set
    End Property
    Private _headVoltage As String
    <XmlElement(ElementName:="HEADVOLTAGE")> _
    Public Property HeadVoltage() As String
        Get
            Return _headVoltage
        End Get
        Set(ByVal value As String)
            _headVoltage = value
        End Set
    End Property
    Private _feedBefore As String
    <XmlElement(ElementName:="FEEDBEFORE")> _
    Public Property FeedBefore() As String
        Get
            Return _feedBefore
        End Get
        Set(ByVal value As String)
            _feedBefore = value
        End Set
    End Property
    Private _feedAfter As String
    <XmlElement(ElementName:="FEEDAFTER")> _
    Public Property FeedAfter() As String
        Get
            Return _feedAfter
        End Get
        Set(ByVal value As String)
            _feedAfter = value
        End Set
    End Property
    Private _setupName As String
    <XmlElement(ElementName:="SETUPNAME")> _
    Public Property SetupName() As String
        Get
            Return _setupName
        End Get
        Set(ByVal value As String)
            _setupName = value
        End Set
    End Property
    Private _lVersion As String
    <XmlElement(ElementName:="LVERSION")> _
    Public Property LVersion() As String
        Get
            Return _lVersion
        End Get
        Set(ByVal value As String)
            _lVersion = value
        End Set
    End Property
    Private _backgroundImage As String
    <XmlElement(ElementName:="BACKGROUNDIMAGE")> _
    Public Property BackgroundImage() As String
        Get
            Return _backgroundImage
        End Get
        Set(ByVal value As String)
            _backgroundImage = value
        End Set
    End Property
    Private _rotation As Integer
    <XmlElement(ElementName:="ROTATION")> _
    Public Property Rotation() As Integer
        Get
            Return _rotation
        End Get
        Set(ByVal value As Integer)
            _rotation = value
        End Set
    End Property

End Class
