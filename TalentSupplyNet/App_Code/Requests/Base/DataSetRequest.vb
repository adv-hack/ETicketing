Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Xml
Imports System.Xml.Schema
Imports Talent.Common
Namespace Talent.TradingPortal
    ''' <summary>
    ''' The base class for request classes which has dataset to process
    ''' </summary>
    Public Class DataSetRequest

#Region "Class Level Fields"
        Private _settings As DESettings
        Private _datasetInput As DataSet
        Private _TDataObjects As TalentDataObjects
        Private _documentVersion As String = String.Empty
#End Region

#Region "Constructor"
        Public Sub New()
            Settings() = New DESettings
            If _TDataObjects Is Nothing Then
                _TDataObjects = New TalentDataObjects()
            End If
        End Sub
#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the Talent data objects.
        ''' </summary>
        ''' <value>The Talent data objects.</value>
        Public Property TDataObjects() As TalentDataObjects
            Get
                Return _TDataObjects
            End Get
            Set(ByVal value As Talent.Common.TalentDataObjects)
                _TDataObjects = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the settings.
        ''' </summary>
        ''' <value>The settings.</value>
        Public Property Settings() As DESettings
            Get
                Return _settings
            End Get
            Set(ByVal value As DESettings)
                _settings = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the document version.
        ''' </summary>
        ''' <value>The document version.</value>
        Public Property DocumentVersion() As String
            Get
                Return _documentVersion
            End Get
            Set(ByVal value As String)
                _documentVersion = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the data set input.
        ''' </summary>
        ''' <value>The data set input.</value>
        Public Property DataSetInput() As DataSet
            Get
                Return _datasetInput
            End Get
            Set(ByVal value As DataSet)
                _datasetInput = value
            End Set
        End Property
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Overrides this function to access the data object through data access layer and returns the respective xmlresponse
        ''' </summary>
        ''' <param name="xmlResp">The XML resp.</param>
        ''' <returns></returns>
        Public Overridable Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xr As New XmlResponse

            Return xr
        End Function

        ''' <summary>
        ''' Overrides this function and validate the received dataset for required tables, format of data etc.
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Function ValidateDataSet() As ErrorObj
            Dim errObj As New ErrorObj

            Return errObj
        End Function
#End Region

    End Class
End Namespace
