Imports System.Xml.Serialization
''' <summary>
''' Data Entity class for tbl_td_fieldproperties
''' </summary>
''' <remarks>
''' All Element names are matched with column names 
''' </remarks>
<Serializable()> _
Public Class DEFieldProperties
    Private _labelName As String
    <XmlElement(ElementName:="LABELNAME")> _
    Public Property LabelName() As String
        Get
            Return _labelName
        End Get
        Set(ByVal value As String)
            _labelName = value
        End Set
    End Property
    Private _fieldType As String
    <XmlElement(ElementName:="FIELDTYPE")> _
    Public Property FieldType() As String
        Get
            Return _fieldType
        End Get
        Set(ByVal value As String)
            _fieldType = value
        End Set
    End Property
    Private _xLeft As String
    <XmlElement(ElementName:="XLEFT")> _
    Public Property XLeft() As String
        Get
            Return _xLeft
        End Get
        Set(ByVal value As String)
            _xLeft = value
        End Set
    End Property
    Private _yTop As String
    <XmlElement(ElementName:="YTOP")> _
    Public Property YTop() As String
        Get
            Return _yTop
        End Get
        Set(ByVal value As String)
            _yTop = value
        End Set
    End Property
    Private _xRight As String
    <XmlElement(ElementName:="XRIGHT")> _
    Public Property XRight() As String
        Get
            Return _xRight
        End Get
        Set(ByVal value As String)
            _xRight = value
        End Set
    End Property
    Private _yBottom As String
    <XmlElement(ElementName:="YBOTTOM")> _
    Public Property YBottom() As String
        Get
            Return _yBottom
        End Get
        Set(ByVal value As String)
            _yBottom = value
        End Set
    End Property
    Private _thick As String
    <XmlElement(ElementName:="THICK")> _
    Public Property Thick() As String
        Get
            Return _thick
        End Get
        Set(ByVal value As String)
            _thick = value
        End Set
    End Property
    Private _thin As String
    <XmlElement(ElementName:="THIN")> _
    Public Property Thin() As String
        Get
            Return _thin
        End Get
        Set(ByVal value As String)
            _thin = value
        End Set
    End Property
    Private _height As String
    <XmlElement(ElementName:="HEIGHT")> _
    Public Property Height() As String
        Get
            Return _height
        End Get
        Set(ByVal value As String)
            _height = value
        End Set
    End Property
    Private _slant As String
    <XmlElement(ElementName:="SLANT")> _
    Public Property Slant() As String
        Get
            Return _slant
        End Get
        Set(ByVal value As String)
            _slant = value
        End Set
    End Property
    Private _font As String
    <XmlElement(ElementName:="FONT")> _
    Public Property Font() As String
        Get
            Return _font
        End Get
        Set(ByVal value As String)
            _font = value
        End Set
    End Property
    Private _angle As String
    <XmlElement(ElementName:="ANGLE")> _
    Public Property Angle() As String
        Get
            Return _angle
        End Get
        Set(ByVal value As String)
            _angle = value
        End Set
    End Property
    Private _align As String
    <XmlElement(ElementName:="ALIGN")> _
    Public Property Align() As String
        Get
            Return _align
        End Get
        Set(ByVal value As String)
            _align = value
        End Set
    End Property
    Private _contents As String
    <XmlElement(ElementName:="CONTENTS")> _
    Public Property Contents() As String
        Get
            Return _contents
        End Get
        Set(ByVal value As String)
            _contents = value
        End Set
    End Property
    Private _length As String
    <XmlElement(ElementName:="LENGTH")> _
    Public Property Length() As String
        Get
            Return _length
        End Get
        Set(ByVal value As String)
            _length = value
        End Set
    End Property
    Private _name As String
    <XmlElement(ElementName:="NAME")> _
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property
    Private _prompt As String
    <XmlElement(ElementName:="PROMPT")> _
    Public Property Prompt() As String
        Get
            Return _prompt
        End Get
        Set(ByVal value As String)
            _prompt = value
        End Set
    End Property
    Private _inputType As String
    <XmlElement(ElementName:="INPUTTYPE")> _
    Public Property InputType() As String
        Get
            Return _inputType
        End Get
        Set(ByVal value As String)
            _inputType = value
        End Set
    End Property
    Private _start As String
    <XmlElement(ElementName:="START")> _
    Public Property Start() As String
        Get
            Return _start
        End Get
        Set(ByVal value As String)
            _start = value
        End Set
    End Property
    Private _increment As String
    <XmlElement(ElementName:="INCREMENT")> _
    Public Property Increment() As String
        Get
            Return _increment
        End Get
        Set(ByVal value As String)
            _increment = value
        End Set
    End Property
    Private _repeat As String
    <XmlElement(ElementName:="REPEAT")> _
    Public Property Repeat() As String
        Get
            Return _repeat
        End Get
        Set(ByVal value As String)
            _repeat = value
        End Set
    End Property
    Private _offset As String
    <XmlElement(ElementName:="OFFSET")> _
    Public Property Offset() As String
        Get
            Return _offset
        End Get
        Set(ByVal value As String)
            _offset = value
        End Set
    End Property
    Private _format As String
    <XmlElement(ElementName:="FORMAT")> _
    Public Property Format() As String
        Get
            Return _format
        End Get
        Set(ByVal value As String)
            _format = value
        End Set
    End Property
    Private _lineSpace As String
    <XmlElement(ElementName:="LINESPACE")> _
    Public Property LineSpace() As String
        Get
            Return _lineSpace
        End Get
        Set(ByVal value As String)
            _lineSpace = value
        End Set
    End Property
    Private _file As String
    <XmlElement(ElementName:="FILE")> _
    Public Property File() As String
        Get
            Return _file
        End Get
        Set(ByVal value As String)
            _file = value
        End Set
    End Property
    Private _inverted As String
    <XmlElement(ElementName:="INVERTED")> _
    Public Property Inverted() As String
        Get
            Return _inverted
        End Get
        Set(ByVal value As String)
            _inverted = value
        End Set
    End Property
    Private _humanReadable As String
    <XmlElement(ElementName:="HUMANREADABLE")> _
    Public Property HumanReadable() As String
        Get
            Return _humanReadable
        End Get
        Set(ByVal value As String)
            _humanReadable = value
        End Set
    End Property
    Private _useRealTimeClock As String
    <XmlElement(ElementName:="USEREALTIMECLOCK")> _
    Public Property UseRealTimeClock() As String
        Get
            Return _useRealTimeClock
        End Get
        Set(ByVal value As String)
            _useRealTimeClock = value
        End Set
    End Property
    Private _printOnLabel As String
    <XmlElement(ElementName:="PRINTONLABEL")> _
    Public Property PrintOnLabel() As String
        Get
            Return _printOnLabel
        End Get
        Set(ByVal value As String)
            _printOnLabel = value
        End Set
    End Property
    Private _mag As String
    <XmlElement(ElementName:="MAG")> _
    Public Property Mag() As String
        Get
            Return _mag
        End Get
        Set(ByVal value As String)
            _mag = value
        End Set
    End Property
    Private _ttf As String
    <XmlElement(ElementName:="TTF")> _
    Public Property Ttf() As String
        Get
            Return _ttf
        End Get
        Set(ByVal value As String)
            _ttf = value
        End Set
    End Property

End Class
