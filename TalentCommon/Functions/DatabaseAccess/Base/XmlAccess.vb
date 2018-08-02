Imports Microsoft.VisualBasic
Imports System.Xml
Imports System.IO

'       Error Number Code base      TACXMAC-  

<Serializable()> _
Public Class XmlAccess

    Private _deSettings As DESettings
    Public Property Settings() As DESettings
        Get
            Return _deSettings
        End Get
        Set(ByVal value As DESettings)
            _deSettings = value
        End Set
    End Property

    Private _InputXmlDocument As New XmlDocument
    Public Property InputXmlDocument() As XmlDocument
        Get
            Return _InputXmlDocument
        End Get
        Set(ByVal value As XmlDocument)
            _InputXmlDocument = value
        End Set
    End Property
    Private _resultDataSet As New DataSet
    Public Property ResultDataSet() As DataSet
        Get
            Return _resultDataSet
        End Get
        Set(ByVal value As DataSet)
            _resultDataSet = value
        End Set
    End Property

    Public Function ConvertXmlToString(ByVal xmlDoc As XmlDocument) As String
        Dim sw As StringWriter = New StringWriter
        Dim xw As XmlTextWriter = New XmlTextWriter(sw)
        xmlDoc.WriteTo(xw)
        Return sw.ToString
    End Function

    Public Function LoadInputXml() As ErrorObj
        Dim myerr As New ErrorObj

        If Not String.IsNullOrEmpty(Settings.XmlSettings.InputXmlLocation) Then
            If IO.File.Exists(Settings.XmlSettings.InputXmlLocation) Then
                Me.InputXmlDocument.Load(Settings.XmlSettings.InputXmlLocation)
            Else
                myerr.HasError = True
                myerr.ErrorNumber = "TACXMAC-01"
                myerr.ErrorMessage = "The input file '" & Settings.XmlSettings.InputXmlLocation & "' could not be found."
            End If
        End If

        Return myerr
    End Function




End Class
