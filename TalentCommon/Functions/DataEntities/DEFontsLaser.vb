Imports System.Xml.Serialization
''' <summary>
''' Data Entity class for tbl_td_fontslaser
''' </summary>
''' <remarks>
''' All Element names are matched with column names 
''' </remarks>
<Serializable()> _
Public Class DEFontsLaser
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
    Private _bitmapSize As String
    <XmlElement(ElementName:="BITMAPSIZE")> _
    Public Property BitmapSize() As String
        Get
            Return _bitmapSize
        End Get
        Set(ByVal value As String)
            _bitmapSize = value
        End Set
    End Property
End Class
