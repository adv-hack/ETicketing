Imports Microsoft.VisualBasic
Imports System.Configuration
Imports System.Data
Imports System.IO
Imports Talent.Common

Namespace Talent.TradingPortal

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Error Number Code base      TTPRQUTDD-
    ''' </remarks>
    Public Class DSUploadTDDataRequest
        Inherits DataSetRequest

#Region "Class Level Fields"
        Private Const _labelProperties As String = "LabelProperties"
        Private Const _fieldProperties As String = "FieldProperties"
        Private Const _fontSimulate As String = "FontSimulate"
        Private Const _fontsLaser As String = "FontsLaser"

        'These constants value should match web.config key name
        Private Const liveUpdateDatabase01 As String = "liveUpdateDatabase01"
        Private Const liveUpdateDatabase02 As String = "liveUpdateDatabase02"
        Private Const liveUpdateDatabase03 As String = "liveUpdateDatabase03"
        Private Const liveUpdateDatabase04 As String = "liveUpdateDatabase04"
        Private Const liveUpdateDatabase05 As String = "liveUpdateDatabase05"
        Private Const liveUpdateDatabase06 As String = "liveUpdateDatabase06"
        Private Const liveUpdateDatabase07 As String = "liveUpdateDatabase07"
        Private Const liveUpdateDatabase08 As String = "liveUpdateDatabase08"
        Private Const liveUpdateDatabase09 As String = "liveUpdateDatabase09"
        Private Const liveUpdateDatabase10 As String = "liveUpdateDatabase10"

        'This length of array has to be increased if the above livedatabase is increased
        Private _connectionStrings(10) As String

        Private _ticketDesignerData As DETicketDesigner = Nothing
        Private _connectionStringsCount As Integer = 0
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Accesses the data objects through data access layer.
        ''' </summary>
        ''' <param name="xmlResp">The XML resp.</param>
        ''' <returns></returns>
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlUploadTDDataResponse = CType(xmlResp, XmlUploadTDDataResponse)
            Dim errObj As New ErrorObj

            'Check having valid connectionstrings
            errObj = GetConnectionStrings()
            If (Not errObj.HasError) Then
                TDataObjects.Settings = Me.Settings
                TDataObjects.TicketDesignerSettings.TicketDesignerDataEntity = _ticketDesignerData
                errObj = TDataObjects.TicketDesignerSettings.UploadData(_connectionStrings, _connectionStringsCount)
            End If

            With xmlAction
                If (errObj.ErrorStatus.Length <= 0) Then
                    errObj.ErrorStatus = errObj.ErrorMessage
                End If
                .Err = errObj
                .DocumentVersion = MyBase.DocumentVersion
                '.SenderID = Settings.SenderID
                .CreateResponse()
            End With
            Return CType(xmlAction, XmlResponse)
        End Function

        ''' <summary>
        ''' Validates the data set for existence of required tables and populates the respective data entities
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ValidateDataSet() As ErrorObj
            Dim errObj As New ErrorObj
            If (Me.DataSetInput Is Nothing) Then
            End If
            If (Not errObj.HasError) Then
                errObj = IsAllTableExists()
            End If
            If (Not errObj.HasError) Then
                populateDataEntities()
            End If
            Return errObj
        End Function

#End Region

#Region "Private Methods"

        ''' <summary>
        ''' Determines whether the required tables are exists in the dataset or not
        ''' </summary>
        ''' <returns></returns>
        Private Function IsAllTableExists() As ErrorObj
            Dim errObj As New ErrorObj
            Dim tableNames As String = String.Empty
            If (DataSetInput.Tables(_labelProperties) Is Nothing) Then
                tableNames += _labelProperties
            End If
            If (DataSetInput.Tables(_fieldProperties) Is Nothing) Then
                tableNames += _fieldProperties
            End If
            If (DataSetInput.Tables(_fontSimulate) Is Nothing) Then
                tableNames += _fontSimulate
            End If
            If (DataSetInput.Tables(_fontsLaser) Is Nothing) Then
                tableNames += _fontsLaser
            End If
            If (tableNames.Length > 0) Then
                errObj.HasError = True
                errObj.ErrorMessage = tableNames & " tables missing in the dataset. Please check"
                errObj.ErrorNumber = "TTPRQUTDD-01"
            Else
                errObj.HasError = False
            End If
            Return errObj
        End Function

        ''' <summary>
        ''' Populates the data entities from the dataset
        ''' </summary>
        ''' <returns></returns>
        Private Function populateDataEntities() As ErrorObj
            Dim errObj As New ErrorObj
            errObj.HasError = False
            Try
                Dim properties As ArrayList
                Dim tempDataTable As New DataTable

                _ticketDesignerData = New DETicketDesigner()
                Dim listLabelProperties As New Generic.List(Of DELabelProperties)
                Dim listFieldProperties As New Generic.List(Of DEFieldProperties)
                Dim listFontSimulate As New Generic.List(Of DEFontSimulate)

                'LabelProperties
                tempDataTable = DataSetInput.Tables(_labelProperties).Copy()
                properties = Utilities.GetPropertyNames(New DELabelProperties())
                For Each labelRow As DataRow In tempDataTable.Rows
                    Dim tempLabelProperties As New DELabelProperties
                    tempLabelProperties = Utilities.PopulateProperties(properties, labelRow, tempLabelProperties)
                    listLabelProperties.Add(tempLabelProperties)
                    tempLabelProperties = Nothing
                Next
                _ticketDesignerData.LabelProperties = listLabelProperties

                'FieldProperties
                tempDataTable.Rows.Clear()
                tempDataTable.Columns.Clear()
                properties.Clear()
                tempDataTable = DataSetInput.Tables(_fieldProperties).Copy()
                properties = Utilities.GetPropertyNames(New DEFieldProperties())
                For Each fieldRow As DataRow In tempDataTable.Rows
                    Dim tempFieldProperties As New DEFieldProperties
                    tempFieldProperties = Utilities.PopulateProperties(properties, fieldRow, tempFieldProperties)
                    listFieldProperties.Add(tempFieldProperties)
                    tempFieldProperties = Nothing
                Next
                _ticketDesignerData.FieldProperties = listFieldProperties

                'FontSimulate
                tempDataTable.Rows.Clear()
                tempDataTable.Columns.Clear()
                properties.Clear()
                tempDataTable = DataSetInput.Tables(_fontSimulate).Copy()
                properties = Utilities.GetPropertyNames(New DEFontSimulate())
                For Each fontSimulateRow As DataRow In tempDataTable.Rows
                    Dim tempFontSimulate As New DEFontSimulate
                    tempFontSimulate = Utilities.PopulateProperties(properties, fontSimulateRow, tempFontSimulate)
                    listFontSimulate.Add(tempFontSimulate)
                    tempFontSimulate = Nothing
                Next
                _ticketDesignerData.FontSimulate = listFontSimulate

                'FontsLaser
                Dim listFontsLaser As New Generic.List(Of DEFontsLaser)
                tempDataTable.Rows.Clear()
                tempDataTable.Columns.Clear()
                properties.Clear()
                tempDataTable = DataSetInput.Tables(_fontsLaser).Copy()
                properties = Utilities.GetPropertyNames(New DEFontsLaser())
                For Each fontsLaserRow As DataRow In tempDataTable.Rows
                    Dim tempFontsLaser As New DEFontsLaser
                    tempFontsLaser = Utilities.PopulateProperties(properties, fontsLaserRow, tempFontsLaser)
                    listFontsLaser.Add(tempFontsLaser)
                    tempFontsLaser = Nothing
                Next
                _ticketDesignerData.FontsLaser = listFontsLaser

            Catch ex As Exception
                errObj.HasError = True
                errObj.ErrorMessage = ex.Message
                errObj.ErrorNumber = "TTPRQUTDD-02"
            End Try
            Return errObj
        End Function

        ''' <summary>
        ''' Gets the connection strings which are listed in the web.config key-liveUpdateDatabase
        ''' </summary>
        ''' <returns></returns>
        Private Function GetConnectionStrings() As ErrorObj
            Dim errObj As New ErrorObj
            errObj.HasError = False
            ReadConnectionString(liveUpdateDatabase01)
            ReadConnectionString(liveUpdateDatabase02)
            ReadConnectionString(liveUpdateDatabase03)
            ReadConnectionString(liveUpdateDatabase04)
            ReadConnectionString(liveUpdateDatabase05)
            ReadConnectionString(liveUpdateDatabase06)
            ReadConnectionString(liveUpdateDatabase07)
            ReadConnectionString(liveUpdateDatabase08)
            ReadConnectionString(liveUpdateDatabase09)
            ReadConnectionString(liveUpdateDatabase10)
            If (_connectionStringsCount <= 0) Then
                errObj.HasError = True
                errObj.ErrorMessage = "Connection Strings are missing in the configuration file. Please contact administrator"
                errObj.ErrorNumber = "TTPRQUTDD-03"
            End If
            Return errObj
        End Function

        ''' <summary>
        ''' Reads the connection string and populates the connection string array
        ''' </summary>
        ''' <param name="appSetConnVariableName">key name of the connection string variable in web.config</param>
        Private Sub ReadConnectionString(ByVal appSetConnVariableName As String)
            Dim connectionString As String = ConfigurationManager.AppSettings(appSetConnVariableName)
            If connectionString.Length > 0 Then
                _connectionStrings(_connectionStringsCount) = connectionString
                _connectionStringsCount += 1
            End If
        End Sub

#End Region

    End Class
End Namespace
