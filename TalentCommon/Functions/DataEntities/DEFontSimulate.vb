Imports System.Xml.Serialization
''' <summary>
''' Data Entity class for tbl_td_fontsimulate
''' </summary>
''' <remarks>
''' All Element names are matched with column names 
''' </remarks>
<Serializable()> _
Public Class DEFontSimulate
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
    Private _simulate As String
    <XmlElement(ElementName:="SIMULATE")> _
    Public Property Simulate() As String
        Get
            Return _simulate
        End Get
        Set(ByVal value As String)
            _simulate = value
        End Set
    End Property

End Class
