
Imports System
Imports System.Text				' StringBuilder
Imports System.Configuration	' ConfigurationSettings.AppSettings
Imports com.qas.proweb.soap		' QuickAddress Pro Web wrapped SOAP-layer


Namespace com.qas.proweb


	' This class is a facade into Pro Web and provides the main functionality of the package.
	' It uses the com.qas.proweb.soap package in a stateless manner, but some optional settings
	' are maintained between construction and the main "business" call to the soap package.
	' An instance of this class is not intended to be preserved across different pages.
	' The intended usage idiom is:
	' construct instance - set optional settings - call main method (e.g. search) - discard instance
	Public Class QuickAddress



		' -- Public Constants --



		' Enumeration of engine types
		Public Enum EngineTypes
			SingleLine = EngineEnumType.Singleline
			Typedown = EngineEnumType.Typedown
			Verification = EngineEnumType.Verification
		End Enum

		' Enumeration of engine searching intensity levels
		Public Enum SearchingIntesityLevels
			Exact = EngineIntensityType.Exact
			Close = EngineIntensityType.Close
			Extensive = EngineIntensityType.Extensive
		End Enum

		' Line separator - determined by a configuration setting on the server
		Private Const cLINE_SEPARATOR As Char = "|"



		' -- Private Members --



		'QuickAddress Pro Web search service
		Dim m_Server As com.qas.proweb.soap.ProWeb

		'Engine searching configuration settings (optional to override server defaults)
		Private m_Engine As EngineType

		' Configuration file settings (optional to override defaults)
		Private m_Config As QAConfigType



		' -- Public Construction --



		' Constructs the search service, using the URL of the QuickAddress Server
		'    sEndpointURL = The URL of the QuickAddress SOAP service, e.g. http://localhost:2021/
		Public Sub New(ByVal sEndpointURL As String)

			m_Server = New com.qas.proweb.soap.ProWeb()
			m_Server.Url = sEndpointURL
			m_Engine = New EngineType()
			m_Config = New QAConfigType()

		End Sub



		' -- Public Properties --



		' Sets the current engine if left unset, the search will use the default, SingleLine
		Property Engine() As EngineTypes
			Get
				Return m_Engine.Value
			End Get
			Set(ByVal Value As EngineTypes)
				m_Engine.Value = Value
			End Set
		End Property


		' Sets the engine intensity if left unset, the search will use the server default value
		WriteOnly Property SearchingIntesity() As SearchingIntesityLevels
			Set(ByVal Value As SearchingIntesityLevels)
				m_Engine.Intensity = Value
				m_Engine.IntensitySpecified = True
			End Set
		End Property


		' Sets the picklist threshold - the maximum number of entries in a picklist,
		' returned by step-in and refinement ignored by the initial search
		' If left unset, the server default will be used
		WriteOnly Property Threshold() As Integer
			Set(ByVal Value As Integer)
				m_Engine.Threshold = System.Convert.ToString(Value)
			End Set
		End Property


		' Sets the timeout period (milliseconds) if left unset, the server default will be used
		WriteOnly Property Timeout() As Integer
			Set(ByVal Value As Integer)
				m_Engine.Timeout = System.Convert.ToString(Value)
			End Set
		End Property


		' Sets whether operations produce a flattened or hierarchical picklist
		' If left unset, the default value of false will be used
		WriteOnly Property Flatten() As Boolean
			Set(ByVal Value As Boolean)
				m_Engine.Flatten = Value
				m_Engine.FlattenSpecified = True
			End Set
		End Property


		' Sets the config file to read settings from; if left unset, the default "qawserve.ini" file will be used
		WriteOnly Property ConfigFile() As String
			Set(ByVal Value As String)
				m_Config.IniFile = Value
			End Set
		End Property


		' Sets the config section to read settings from; if left unset, the default "[QADefault]" section will be used
		WriteOnly Property ConfigSection() As String
			Set(ByVal Value As String)
				m_Config.IniSection = Value
			End Set
		End Property



		' -- Public Methods - Searching Operations --



		' Test whether a search can be performed using a data set/layout combination
		'    sDataID = Three-letter data identifier
		'    sLayout = Name of the layout
		' Returns: Is the country and layout combination available
		' Throws QasException
		Public Function CanSearch(ByVal sDataID As String, Optional ByVal sLayout As String = Nothing) As CanSearch

			Dim param As QACanSearch = New QACanSearch()
			param.Country = sDataID
			param.Engine = m_Engine
			param.Layout = sLayout
			param.QAConfig = m_Config

            Dim tResult As CanSearch = Nothing
			Try

                'Make the call to the server
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12
                Dim cansearchResult As QASearchOk = ProServer.DoCanSearch(param)
				tResult = New CanSearch(cansearchResult)

			Catch x As Exception

				MapException(x)

			End Try

			Return tResult

		End Function



		' Perform an initial search for the search terms in the specified data set
		' If using the verification engine, the result may include a formatted address and/or a picklist
		' Other engines only produce a picklist
		'    sDataID = Three-letter identifier of the data to search
		'    asSearch = Array of search terms
		'    sPromptSet = Name of the search prompt set applied to the search terms
		'    Layout = Name of the layout (for verification engine); optional
		' Returns Search result, containing a picklist and/or formatted address
		' ThrowsQasException
		Public Function Search(ByVal sDataID As String, _
		  ByVal asSearch() As String, _
		  ByVal tPromptSet As PromptSet.Types, _
		  Optional ByVal sLayout As String = Nothing) As SearchResult

			System.Diagnostics.Debug.Assert(Not sDataID Is Nothing)
			System.Diagnostics.Debug.Assert(Not asSearch Is Nothing And asSearch.GetLength(0) > 0)

			' Concatenate search terms
			Dim sSearch As StringBuilder = New StringBuilder(asSearch(0))
			Dim i As Integer
			For i = 1 To asSearch.GetLength(0) - 1
				sSearch.Append(cLINE_SEPARATOR)
				sSearch.Append(asSearch(i))
			Next i

			Return Search(sDataID, sSearch.ToString(), tPromptSet, sLayout)

		End Function



		' Method overload: the Search function with search terms as a single string
		'    sDataID = Three-letter identifier of the data to search
		'    sSearch = Search terms
		'    sPromptSet = Name of the search prompt set applied to the search terms
		'    Layout = Name of the layout (for verification engine); optional
		' Returns Search result, containing a picklist and/or formatted address
		' ThrowsQasException
		Public Function Search(ByVal sDataID As String, _
		  ByVal sSearch As String, _
		  ByVal tPromptSet As PromptSet.Types, _
		  Optional ByVal sLayout As String = Nothing) As SearchResult

			System.Diagnostics.Debug.Assert(Not sDataID Is Nothing)
			System.Diagnostics.Debug.Assert(Not sSearch Is Nothing)

			'Set up the parameter for the SOAP call
			Dim param As QASearch = New QASearch()
			param.Country = sDataID
			param.Engine = m_Engine
			param.Engine.PromptSet = tPromptSet
			param.Engine.PromptSetSpecified = True
			param.Layout = sLayout
			param.QAConfig = m_Config
			param.Search = sSearch.ToString()

            Dim result As SearchResult = Nothing
			Try

				'Make the call to the server
				Dim SR As QASearchResult
				SR = ProServer.DoSearch(param)
				result = New SearchResult(SR)

			Catch x As Exception

				MapException(x)

			End Try

			Return result

		End Function


		' Perform an initial Singleline search, returning a picklist
		'     sDataID = data set to search, 3-letter identifier
		'     asSearch = Array of search terms
		'     sPromptSet = Name of the search prompt set applied to the search terms
		' Returns Picklist result
		Public Function SearchSingleline(ByVal sDataID As String, _
		   ByVal asSearch() As String, _
		   ByVal tPromptSet As PromptSet.Types) As Picklist

			System.Diagnostics.Debug.Assert(Not sDataID Is Nothing)
			System.Diagnostics.Debug.Assert(Not asSearch Is Nothing And asSearch.GetLength(0) > 0)

			' Change the engine to Singleline; remember the old setting for restoration
			Dim engineOld As EngineTypes = Engine
			Engine = EngineTypes.SingleLine

			' Perform the search operation
			Dim SR As SearchResult = Search(sDataID, asSearch, tPromptSet, Nothing)
			Engine = engineOld
			Return SR.Picklist

		End Function



		' Perform a refinement, filtering the specified picklist using the supplied text
		' NB: Stepin delegates to this function with blank refinement text
		'     sMoniker = the search point moniker of the picklist to refine
		'     sRefinementText = the refinement text
		' Returns Picklist result
		Public Function Refine(ByVal sMoniker As String, ByVal sRefinementText As String) As Picklist

			System.Diagnostics.Debug.Assert(Not sMoniker Is Nothing And Not sRefinementText Is Nothing)

			'Set up the parameter for the SOAP call
			Dim param As QARefine = New QARefine()
			param.Moniker = sMoniker
			param.Refinement = sRefinementText
			param.QAConfig = m_Config
			param.Threshold = m_Engine.Threshold
			param.Timeout = m_Engine.Timeout

            Dim result As Picklist = Nothing
			Try

				'Make the call to the server
				Dim pl As QAPicklistType = ProServer.DoRefine(param).QAPicklist
				result = New Picklist(pl)

			Catch x As Exception

				MapException(x)

			End Try

			Return result

		End Function



		' Perform a step-in: return the picklist for a particular moniker
		' NB: delegates to the Refine function with blank refinement text
		'     sMoniker = The search point moniker of the picklist being displayed
		' Returns Picklist result
		Public Function StepIn(ByVal sMoniker As String) As Picklist

			Return Refine(sMoniker, "")

		End Function



		' Retrieve the final address specifed by the moniker, formatted using the requested layout
		'     sMoniker = Search point moniker of the address item
		'     sLayout = Name of the layout name (specifies how the address should be formatted)
		' Returns Formatted address result
		Public Function GetFormattedAddress(ByVal sMoniker As String, ByVal sLayout As String) As FormattedAddress

			System.Diagnostics.Debug.Assert(Not sMoniker Is Nothing And Not sLayout Is Nothing)

			'Set up the parameter for the SOAP call
			Dim param As QAGetAddress = New QAGetAddress()
			param.Layout = sLayout
			param.Moniker = sMoniker
			param.QAConfig = m_Config

			Dim result As FormattedAddress = Nothing
			Try

				'Make the call to the server
				Dim Address As QAAddressType = ProServer.DoGetAddress(param).QAAddress
				result = New FormattedAddress(Address)

			Catch x As Exception

				MapException(x)

			End Try

			Return result

		End Function



		' -- Public Methods - Status Operations --



		' Retrieve all the available data sets
		' Returns Array of available data sets
		Public Function GetAllDatasets() As Dataset()

            Dim aResults() As Dataset = Nothing
			Try

				'Make the call to the server
				Dim aDatasets As QADataSet() = ProServer.DoGetData()
				aResults = Dataset.CreateArray(aDatasets)

			Catch x As Exception

				MapException(x)

			End Try

			Return aResults

		End Function



		' Retrieve an array of all the layouts available for the specified data set
		'     sDataID = 3-letter identifier of the data set of interest
		' Returns Array of layouts within this data set
		Public Function GetAllLayouts(ByVal sDataID As String) As Layout()

			System.Diagnostics.Debug.Assert(Not sDataID Is Nothing)

			'Set up the parameter for the SOAP call
			Dim param As QAGetLayouts = New QAGetLayouts()
			param.Country = sDataID
			param.QAConfig = m_Config

            Dim aResults() As Layout = Nothing
			Try

				'Make the call to the server
				Dim aLayouts As QALayout() = ProServer.DoGetLayouts(param)
				aResults = Layout.CreateArray(aLayouts)

			Catch x As Exception

				MapException(x)

			End Try

			Return aResults

		End Function



		' Retrieve an array of example addresses for this data set in the specified layouit
		'     sDataID = data set of interest, 3-letter identifier
		'     sLayout = Layout to apply
		' Returns Array of example addresses
		Public Function GetExampleAddresses(ByVal sDataID As String, ByVal sLayout As String) As ExampleAddress()

			'Set up the parameter for the SOAP call
			Dim param As QAGetExampleAddresses = New QAGetExampleAddresses()
			param.Country = sDataID
			param.Layout = sLayout
			param.QAConfig = m_Config

            Dim aResults() As ExampleAddress = Nothing
			Try

				'Make the call to the server
				Dim aAddresses() As QAExampleAddress = ProServer.DoGetExampleAddresses(param)
				aResults = ExampleAddress.createArray(aAddresses)

			Catch x As Exception

				MapException(x)

			End Try

			Return aResults

		End Function



		' Retrieve detailed licensing information about all the data resources installed
		' Returns Array of licencing information, one per data resource
		Public Function GetLicenceInfo() As LicensedSet()

            Dim aResults As LicensedSet() = Nothing
			Try

				'Make the call to the server
				Dim info As QALicenceInfo = ProServer.DoGetLicenseInfo()
				aResults = LicensedSet.CreateArray(info)

			Catch x As Exception

				MapException(x)

			End Try

			Return aResults

		End Function



		' Retrieve the search prompt set for a particular data set
		'     sDataID = data set of interest, 3-letter identifier
		'     eTemplateName = Input template of interest
		' Returns Input template, array of template lines
		Public Function GetPromptSet(ByVal sDataID As String, ByVal tType As PromptSet.Types) As PromptSet

			System.Diagnostics.Debug.Assert(Not sDataID Is Nothing)

			'Set up the parameter for the SOAP call
			Dim param As QAGetPromptSet = New QAGetPromptSet()
			param.Country = sDataID
			param.Engine = m_Engine
			param.PromptSet = tType
			param.QAConfig = m_Config

			Dim result As PromptSet = Nothing
			Try

				'Make the call to the server
				Dim tPromptSet As QAPromptSet = ProServer.DoGetPromptSet(param)
				result = New PromptSet(tPromptSet)

			Catch x As Exception

				MapException(x)

			End Try

			Return result

		End Function



		' Retrieve system (diagnostic) information from the server
		' Returns Array of strings, tab-separated key/value pairs of system info
		Public Function GetSystemInfo() As String()

            Dim aResults As String() = Nothing
			Try

				'Make the call to the server
				aResults = ProServer.DoGetSystemInfo()

			Catch x As Exception

				MapException(x)

			End Try

			Return aResults

		End Function



		' -- Private Methods - Helpers --



		' Return the QuickAddress Pro Web SOAP service
		Private ReadOnly Property ProServer() As com.qas.proweb.soap.ProWeb
			Get
				Return m_Server
			End Get
		End Property



		' Rethrow a remote SoapException exception, with details parsed and exposed
		Private Sub MapException(ByVal x As Exception)

			System.Diagnostics.Debugger.Log(0, "Error", x.ToString() + ControlChars.NewLine)

			If TypeOf x Is System.Web.Services.Protocols.SoapHeaderException Then

				Throw x

			ElseIf TypeOf x Is System.Web.Services.Protocols.SoapException Then

				Dim xSoap As System.Web.Services.Protocols.SoapException = x
				Dim xmlDetail As System.Xml.XmlNode = xSoap.Detail
				Dim sDetail As String = xmlDetail.InnerText.Trim()
				Dim asDetail As String() = sDetail.Split(ControlChars.Lf)

				Dim sMessage As String = asDetail(1).Trim() + " [" + asDetail(0).Trim() + "]"
				Dim xThrow As Exception = New Exception(sMessage, xSoap)
				Throw xThrow

			Else

				Throw x

			End If

		End Sub


	End Class


End Namespace
