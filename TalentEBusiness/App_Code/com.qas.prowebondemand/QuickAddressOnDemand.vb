
Imports System.Text
' StringBuilder
Imports System.Configuration
' ConfigurationSettings.AppSettings
Imports System.Net
' Proxy class
Imports com.qas.prowebondemand.soap
' QuickAddress Pro Web wrapped SOAP-layer

Namespace com.qas.prowebondemand
    ''' <summary>
    ''' This class is a facade into On Demand and provides the main functionality of the package.
    ''' It uses the com.qas.proweb.soap package in a stateless manner, but some optional settings
    ''' are maintained between construction and the main "business" call to the soap package.
    ''' An instance of this class is not intended to be preserved across different pages.
    ''' The intended usage idiom is:
    '''   construct instance - set optional settings - call main method (e.g. search) - discard instance
    ''' </summary>
    Public Class QuickAddress
        ' -- Public Constants --


        ''' <summary>
        ''' Enumeration of engine types
        ''' </summary>
        Public Enum EngineTypes
            Singleline = EngineEnumType.Singleline
            Typedown = EngineEnumType.Typedown
            Verification = EngineEnumType.Verification
            Keyfinder = EngineEnumType.Keyfinder
            Intuitive = EngineEnumType.Intuitive
        End Enum

        ''' <summary>
        ''' Enumeration of engine searching intensity levels
        ''' </summary>
        Public Enum SearchingIntesityLevels
            Exact = EngineIntensityType.Exact
            Close = EngineIntensityType.Close
            Extensive = EngineIntensityType.Extensive
        End Enum

        ''' Line separator - determined by a configuration setting on the server
        Private Const cLINE_SEPARATOR As Char = "|"c


        ' -- Private Members --


        ' QuickAddress Pro Web search service
        Private m_Service As QASOnDemandIntermediary = Nothing
        ' Engine searching configuration settings (optional to override server defaults)
        Private m_Engine As EngineType = Nothing

        ' -- Public Construction --
        ''' <summary>
        ''' Constructs the search service, using the URL, proxy server
        ''' </summary>
        ''' <param name="sEndpointURL">The URL of the QuickAddress SOAP service, e.g. http://localhost:2021/</param>
        ''' <param name="Username">The Username for the proweb ondermand service</param>
        ''' <param name="Password">The Password for the proweb ondemand service</param>
        ''' <param name="proxy">Proxy server</param> 
        Public Sub New(sEndpointURL As [String], username As [String], password As [String], proxy As IWebProxy)
            Me.New(sEndpointURL, username, password)
            If proxy IsNot Nothing Then
                m_Service.Proxy = proxy
            End If
        End Sub


        ''' <summary>
        ''' Constructs the search service, using the URL of the QuickAddress Server
        ''' </summary>
        ''' <param name="sEndpointURL">The URL of the QuickAddress SOAP service </param>
        ''' <remarks>
        ''' e.g. 
        ''' If you're integrating against the UK data centre: https://ws.ondemand.qas.com/ProOnDemand/V2/ProOnDemandService.asmx 
        ''' If you're integrating against the US data centre: https://ws2.ondemand.qas.com/ProOnDemand/V2/ProOnDemandService.asmx 		 
        ''' </remarks>
        Public Sub New(sEndpointURL As [String], Username As [String], Password As [String])
            m_Service = New QASOnDemandIntermediary()
            m_Service.Url = sEndpointURL

            Dim authentication As New QAAuthentication()
            authentication.Username = Username
            authentication.Password = Password

            Dim header As New QAQueryHeader()
            header.QAAuthentication = authentication

            m_Service.QAQueryHeaderValue = header

            m_Engine = New EngineType()
        End Sub


        ' -- Public Properties --


        ''' <summary>
        ''' Sets the current engine; if left unset, the search will use the default, SingleLine
        ''' </summary>
        Public Property Engine() As EngineTypes
            Get
                Return CType(m_Engine.Value, EngineTypes)
            End Get
            Set(value As EngineTypes)
                m_Engine.Value = DirectCast(value, EngineEnumType)
            End Set
        End Property

        ''' <summary>
        ''' Sets the engine intensity; if left unset, the search will use the server default value
        ''' </summary>
        Public WriteOnly Property SearchingIntesity() As SearchingIntesityLevels
            Set(value As SearchingIntesityLevels)
                m_Engine.Intensity = DirectCast(value, EngineIntensityType)
                m_Engine.IntensitySpecified = True
            End Set
        End Property

        ''' <summary>
        ''' Sets the picklist threshold - the maximum number of entries in a picklist,
        ''' returned by step-in and refinement; ignored by the initial search
        ''' If left unset, the server default will be used
        ''' </summary>
        Public WriteOnly Property Threshold() As Integer
            Set(value As Integer)
                m_Engine.Threshold = System.Convert.ToString(value)
            End Set
        End Property

        ''' <summary>
        ''' Sets the timeout period (milliseconds); if left unset, the server default will be used
        ''' </summary>
        Public WriteOnly Property Timeout() As Integer
            Set(value As Integer)
                m_Engine.Timeout = System.Convert.ToString(value)
            End Set
        End Property

        ''' <summary>
        ''' Sets whether operations produce a flattened or hierarchical picklist;
        ''' If left unset, the default value of false (hierarchical) will be used
        ''' </summary>
        Public WriteOnly Property Flatten() As Boolean
            Set(value As Boolean)
                m_Engine.Flatten = value
                m_Engine.FlattenSpecified = True
            End Set
        End Property


        ' -- Public Methods - Searching Operations --


        ''' <summary>
        ''' Test whether a search can be performed using a data set/layout/engine combination
        ''' </summary>
        ''' <param name="sDataID">Three-letter data identifier</param>
        ''' <param name="sLayout">Name of the layout; optional</param>
        ''' <param name="tPromptSet">The prompt set to use</param>
        ''' <returns>Is the country and layout combination available</returns>
        ''' <throws>SoapException</throws>
        Public Function CanSearch(sDataID As String, sLayout As String, tPromptSet As PromptSet.Types) As CanSearch
            Dim param As New QACanSearch()
            param.Country = sDataID
            param.Engine = m_Engine
            param.Layout = sLayout
            param.Engine.PromptSet = DirectCast(tPromptSet, PromptSetType)
            param.Engine.PromptSetSpecified = True

            Dim tResult As CanSearch = Nothing
            Try
                ' Make the call to the server
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                Dim cansearchResult As QASearchOk = SearchService.DoCanSearch(param)

                tResult = New CanSearch(cansearchResult)
            Catch x As Exception
                MapException(x)
            End Try

            Return tResult
        End Function

        ''' <summary>
        ''' Test whether a search can be performed using a data set/layout/engine combination
        ''' </summary>
        ''' <param name="sDataID">Three-letter data identifier</param>
        ''' <param name="sLayout">Name of the layout; optional</param>
        ''' <returns>Is the country and layout combination available</returns>
        ''' <throws>SoapException</throws>
        Public Function CanSearch(sDataID As String, sLayout As String) As CanSearch
            Return CanSearch(sDataID, sLayout, PromptSet.Types.[Default])
        End Function

        ''' <summary>
        ''' Method overload: provides the CanSearch function without the optional sLayout argument
        ''' <param name="sDataID">Three-letter data identifier</param>
        ''' </summary>
        Public Function CanSearch(sDataID As String) As CanSearch
            Return CanSearch(sDataID, Nothing)
        End Function


        ''' <summary>
        ''' Perform an initial search for the search terms in the specified data set
        ''' If using the verification engine, the result may include a formatted address and/or a picklist
        ''' Other engines only produce a picklist
        ''' </summary>
        ''' <param name="sDataID">Three-letter identifier of the data to search</param>
        ''' <param name="asSearch">Array of search terms</param>
        ''' <param name="sPromptSet">Name of the search prompt set applied to the search terms</param>
        ''' <param name="Layout">Name of the layout (verification engine only); optional</param>
        ''' <param name="sRequestTag">Request tag supplied by user</param>
        ''' <returns>Search result, containing a picklist and/or formatted address</returns>
        ''' <throws>SoapException</throws>
        Public Function Search(sDataID As String, asSearch As String(), tPromptSet As PromptSet.Types, sLayout As String, sRequestTag As String) As SearchResult
            System.Diagnostics.Debug.Assert(asSearch IsNot Nothing AndAlso asSearch.GetLength(0) > 0)

            ' Concatenate search terms
            Dim sSearch As New StringBuilder(asSearch(0))
            For i As Integer = 1 To asSearch.GetLength(0) - 1
                sSearch.Append(cLINE_SEPARATOR)
                sSearch.Append(asSearch(i))
            Next

            Return Search(sDataID, sSearch.ToString(), tPromptSet, sLayout, sRequestTag)
        End Function

        ''' <summary>
        ''' Method overload: provides the Search function without the optional RequestTag argument
        ''' </summary>
        ''' <param name="sDataID">Three-letter identifier of the data to search</param>
        ''' <param name="asSearch">Array of search terms</param>
        ''' <param name="sPromptSet">Name of the search prompt set applied to the search terms</param>
        ''' <param name="Layout">Name of the layout (verification engine only); optional</param>
        ''' <returns>Search result, containing a picklist and/or formatted address</returns>
        ''' <throws>SoapException</throws>
        Public Function Search(sDataID As String, asSearch As String(), tPromptSet As PromptSet.Types, sLayout As String) As SearchResult
            Return Search(sDataID, asSearch, tPromptSet, sLayout, Nothing)
        End Function

        ''' <summary>
        ''' Method overload: provides the Search function without the optional Layout
        ''' and RequestTag arguments
        ''' </summary>
        ''' <param name="sDataID">Three-letter identifier of the data to search</param>
        ''' <param name="asSearch">Array of search terms</param>
        ''' <param name="sPromptSet">Name of the search prompt set applied to the search terms</param>
        ''' <returns>Search result, containing a picklist and/or formatted address</returns>
        ''' <throws>SoapException</throws>
        Public Function Search(sDataID As String, asSearch As String(), tPromptSet As PromptSet.Types) As SearchResult
            Return Search(sDataID, asSearch, tPromptSet, Nothing)
        End Function


        ''' <summary>
        ''' Method overload: the Search function with search terms as a single string
        ''' </summary>
        ''' <param name="sDataID">Three-letter identifier of the data to search</param>
        ''' <param name="sSearch">Search terms</param>
        ''' <param name="sPromptSet">Name of the search prompt set applied to the search terms</param>
        ''' <param name="Layout">Name of the layout (verification engine only); optional</param>
        ''' <param name="sRequestTag">Request tag supplied by user</param>
        ''' <returns>Search result, containing a picklist and/or formatted address</returns>
        ''' <throws>SoapException</throws>
        Public Function Search(sDataID As String, sSearch As String, tPromptSet As PromptSet.Types, sLayout As String, sRequestTag As String) As SearchResult
            System.Diagnostics.Debug.Assert(sDataID IsNot Nothing)
            System.Diagnostics.Debug.Assert(sSearch IsNot Nothing)

            ' Set up the parameter for the SOAP call
            Dim param As New QASearch()
            param.Country = sDataID
            param.Engine = m_Engine
            param.Engine.PromptSet = DirectCast(tPromptSet, PromptSetType)
            param.Engine.PromptSetSpecified = True
            param.Layout = sLayout
            param.Search = sSearch
            param.RequestTag = sRequestTag

            Dim result As SearchResult = Nothing
            Try
                ' Make the call to the server
                Dim searchResult As QASearchResult = SearchService.DoSearch(param)

                result = New SearchResult(searchResult)
            Catch x As Exception
                MapException(x)
            End Try

            Return result
        End Function

        ''' <summary>
        ''' Method overload: provides the Search function without the optional RequestTag argument
        ''' </summary>
        ''' <param name="sDataID">Three-letter identifier of the data to search</param>
        ''' <param name="sSearch">Search terms</param>
        ''' <param name="sPromptSet">Name of the search prompt set applied to the search terms</param>
        ''' <param name="Layout">Name of the layout (verification engine only); optional</param>
        ''' <returns>Search result, containing a picklist and/or formatted address</returns>
        ''' <throws>SoapException</throws>
        Public Function Search(sDataID As String, sSearch As String, tPromptSet As PromptSet.Types, sLayout As String) As SearchResult
            Return Search(sDataID, sSearch, tPromptSet, sLayout, Nothing)
        End Function


        ''' <summary>
        ''' Method overload: the Search function with search terms as a single string, without Layout
        ''' and RequestTag arguments
        ''' </summary>
        ''' <param name="sDataID">Three-letter identifier of the data to search</param>
        ''' <param name="sSearch">Search terms</param>
        ''' <param name="sPromptSet">Name of the search prompt set applied to the search terms</param>
        ''' <returns>Search result, containing a picklist and/or formatted address</returns>
        ''' <throws>SoapException</throws>
        Public Function Search(sDataID As String, sSearch As String, tPromptSet As PromptSet.Types) As SearchResult
            Return Search(sDataID, sSearch, tPromptSet, Nothing)
        End Function


        ''' <summary>
        ''' Perform a refinement, filtering the specified picklist using the supplied text
        ''' NB: Stepin delegates to this function with blank refinement text
        ''' </summary>
        ''' <param name="sMoniker">The search point moniker of the picklist to refine</param>
        ''' <param name="sRefinementText">The refinement text</param>
        ''' <param name="sRequestTag">Request tag supplied by user</param>
        ''' <returns>Picklist result</returns>
        Public Function Refine(sMoniker As [String], sRefinementText As [String], sRequestTag As [String]) As Picklist
            System.Diagnostics.Debug.Assert(sMoniker IsNot Nothing AndAlso sRefinementText IsNot Nothing)

            ' Set up the parameter for the SOAP call
            Dim param As New QARefine()
            param.Moniker = sMoniker
            param.Refinement = sRefinementText
            param.Threshold = m_Engine.Threshold
            param.Timeout = m_Engine.Timeout
            param.RequestTag = sRequestTag

            Dim result As Picklist = Nothing
            Try
                ' Make the call to the server
                Dim picklist As QAPicklistType = SearchService.DoRefine(param).QAPicklist

                result = New Picklist(picklist)
            Catch x As Exception
                MapException(x)
            End Try

            Return result
        End Function

        ''' <summary>
        ''' Call Refine with default value for RequestTag.
        ''' </summary>
        ''' <param name="sMoniker">The search point moniker of the picklist to refine</param>
        ''' <param name="sRefinementText">The refinement text</param>
        ''' <returns>Picklist result</returns>
        Public Function Refine(sMoniker As [String], sRefinementText As [String]) As Picklist
            Return Refine(sMoniker, sRefinementText, Nothing)
        End Function


        ''' <summary>
        ''' Perform a step-in: return the picklist for a particular moniker
        ''' NB: delegates to the Refine function with blank refinement text
        ''' </summary>
        ''' <param name="sMoniker">The search point moniker of the picklist being displayed</param>
        ''' <returns>Picklist result</returns>
        Public Function StepIn(sMoniker As [String]) As Picklist
            Return Refine(sMoniker, "")
        End Function


        ''' <summary>
        ''' Retrieve the final address specifed by the moniker, formatted using the requested layout
        ''' </summary>
        ''' <param name="sMoniker">Search point moniker of the address item</param>
        ''' <param name="sLayout">Name of the layout name (specifies how the address should be formatted)</param>
        ''' <param name="sRequestTag">User supplied tag for the request</param>
        ''' <returns>Formatted address result</returns>
        Public Function GetFormattedAddress(sMoniker As String, sLayout As String, sRequestTag As String) As FormattedAddress
            System.Diagnostics.Debug.Assert(sMoniker IsNot Nothing AndAlso sLayout IsNot Nothing)

            ' Set up the parameter for the SOAP call
            Dim param As New QAGetAddress()
            param.Layout = sLayout
            param.Moniker = sMoniker
            param.RequestTag = sRequestTag

            Dim result As FormattedAddress = Nothing
            Try
                ' Make the call to the server
                Dim address As QAAddressType = SearchService.DoGetAddress(param).QAAddress

                result = New FormattedAddress(address)
            Catch x As Exception
                MapException(x)
            End Try

            Return result
        End Function

        ''' <summary>
        ''' Call GetFormattedAddress() with default value for RequestTag.
        ''' </summary>
        ''' <param name="sMoniker">Search point moniker of the address item</param>
        ''' <param name="sLayout">Name of the layout name (specifies how the address should be formatted)</param>
        ''' <returns>Formatted address result</returns>
        Public Function GetFormattedAddress(sMoniker As String, sLayout As String) As FormattedAddress
            Return GetFormattedAddress(sMoniker, sLayout, Nothing)
        End Function


        ' -- Public Methods - Status Operations --


        ''' <summary>
        ''' Retrieve all the available data sets
        ''' </summary>
        ''' <returns>Array of available data sets</returns>
        Public Function GetAllDatasets() As Dataset()
            Dim aResults As Dataset() = Nothing
            Try
                ' Make the call to the server
                Dim getData As New QAGetData()
                Dim aDatasets As QADataSet() = SearchService.DoGetData(getData)

                aResults = Dataset.CreateArray(aDatasets)
            Catch x As Exception
                MapException(x)
            End Try

            Return aResults
        End Function

        Public Function GetDataMapDetail(sID As String) As LicensedSet()
            Dim aDatasets As LicensedSet() = Nothing

            Try
                Dim tRequest As New QAGetDataMapDetail()
                tRequest.DataMap = sID

                Dim tMapDetail As QADataMapDetail = SearchService.DoGetDataMapDetail(tRequest)
                aDatasets = LicensedSet.createArray(tMapDetail)
            Catch x As Exception
                MapException(x)
            End Try

            Return aDatasets
        End Function

        ''' <summary>
        ''' Retrieve an array of all the layouts available for the specified data set
        ''' </summary>
        ''' <param name="sDataID">3-letter identifier of the data set of interest</param>
        ''' <returns>Array of layouts within this data set</returns>
        Public Function GetAllLayouts(sDataID As String) As Layout()
            System.Diagnostics.Debug.Assert(sDataID IsNot Nothing)

            ' Set up the parameter for the SOAP call
            Dim param As New QAGetLayouts()
            param.Country = sDataID

            Dim aResults As Layout() = Nothing
            Try
                ' Make the call to the server
                Dim aLayouts As QALayout() = SearchService.DoGetLayouts(param)
                aResults = Layout.CreateArray(aLayouts)
            Catch x As Exception
                MapException(x)
            End Try

            Return aResults
        End Function


        ''' <summary>
        ''' Retrieve an array of example addresses for this data set in the specified layouit
        ''' </summary>
        ''' <param name="sDataID">data set of interest, 3-letter identifier</param>
        ''' <param name="sLayout">Layout to apply</param>
        ''' <returns>Array of example addresses</returns>
        Public Function GetExampleAddresses(sDataID As [String], sLayout As [String], sRequestTag As [String]) As ExampleAddress()
            ' Set up the parameter for the SOAP call
            Dim param As New QAGetExampleAddresses()
            param.Country = sDataID
            param.Layout = sLayout
            param.RequestTag = sRequestTag

            Dim aResults As ExampleAddress() = Nothing
            Try
                ' Make the call to the server
                Dim aAddresses As QAExampleAddress() = SearchService.DoGetExampleAddresses(param)
                aResults = ExampleAddress.createArray(aAddresses)
            Catch x As Exception
                MapException(x)
            End Try

            Return aResults
        End Function

        ''' <summary>
        ''' Call GetExampleAddresses() with default value for RequestTag.
        ''' </summary>
        ''' <param name="sDataID">data set of interest, 3-letter identifier</param>
        ''' <param name="sLayout">Layout to apply</param>
        ''' <returns>Array of example addresses</returns>
        Public Function GetExampleAddresses(sDataID As [String], sLayout As [String]) As ExampleAddress()
            Return GetExampleAddresses(sDataID, sLayout, Nothing)
        End Function

        ''' <summary>
        ''' Retrieve detailed licensing information about all the data sets and DataPlus sets installed
        ''' </summary>
        ''' <returns> Array of licencing information, one per data set</returns>
        Public Function GetLicenceInfo() As LicensedSet()
            Dim aResults As LicensedSet() = Nothing
            Try
                ' Make the call to the server
                Dim getLicenseInfo As New QAGetLicenseInfo()
                Dim info As QALicenceInfo = SearchService.DoGetLicenseInfo(getLicenseInfo)
                aResults = LicensedSet.createArray(info)
            Catch x As Exception
                MapException(x)
            End Try

            Return aResults
        End Function


        ''' <summary>
        ''' Retrieve the search prompt set for a particular data set
        ''' </summary>
        ''' <param name="sDataID">data set of interest, 3-letter identifier</param>
        ''' <param name="eTemplateName">Input template of interest</param>
        ''' <returns>Input template, array of template lines</returns>
        Public Function GetPromptSet(sDataID As String, tType As PromptSet.Types) As PromptSet
            System.Diagnostics.Debug.Assert(sDataID IsNot Nothing)

            ' Set up the parameter for the SOAP call
            Dim param As New QAGetPromptSet()
            param.Country = sDataID
            param.Engine = m_Engine
            param.PromptSet = DirectCast(tType, PromptSetType)

            Dim result As PromptSet = Nothing
            Try
                ' Make the call to the server
                Dim tPromptSet As QAPromptSet = SearchService.DoGetPromptSet(param)
                result = New PromptSet(tPromptSet)
            Catch x As Exception
                MapException(x)
            End Try

            Return result
        End Function


        ''' <summary>
        ''' Retrieve system (diagnostic) information from the server
        ''' <returns>Array of strings, tab-separated key/value pairs of system info</returns>
        Public Function GetSystemInfo() As [String]()
            Dim aResults As [String]() = Nothing
            Try
                ' Make the call to the server
                Dim getSystemInfo__1 As New QAGetSystemInfo()
                aResults = SearchService.DoGetSystemInfo(getSystemInfo__1)
            Catch x As Exception
                MapException(x)
            End Try

            Return aResults
        End Function


        ' -- Private Methods - Helpers --


        ''' <summary>
        ''' Return the QuickAddress Pro Web SOAP service
        ''' </summary>
        Private ReadOnly Property SearchService() As QASOnDemandIntermediary
            Get
                Return m_Service
            End Get
        End Property


        ''' <summary>
        ''' Rethrow a remote SoapException exception, with details parsed and exposed
        ''' </summary>
        ''' <param name="e"></param>
        Private Sub MapException(x As Exception)
            System.Diagnostics.Debugger.Log(0, "Error", x.ToString() & vbLf)

            If TypeOf x Is System.Web.Services.Protocols.SoapHeaderException Then
                Dim xHeader As System.Web.Services.Protocols.SoapHeaderException = TryCast(x, System.Web.Services.Protocols.SoapHeaderException)
                Throw x
            ElseIf TypeOf x Is System.Web.Services.Protocols.SoapException Then
                ' Parse out qas:QAFault string
                Dim xSoap As System.Web.Services.Protocols.SoapException = TryCast(x, System.Web.Services.Protocols.SoapException)
                Dim xmlDetails As System.Xml.XmlNode = xSoap.Detail

                Dim sMessage As String = ""

                For Each xmlDetail As System.Xml.XmlNode In xmlDetails.ChildNodes
                    Dim asDetail As String() = xmlDetail.InnerText.Split(ControlChars.Lf)
                    If asDetail.Length = 2 Then
                        sMessage += xmlDetail.Name & ": [" & asDetail(1).Trim() & " " & asDetail(0).Trim() & "] "
                    Else
                        sMessage += xmlDetail.Name & ": [" & asDetail(0).Trim() & "] "
                    End If
                Next

                Dim xThrow As New Exception(sMessage, xSoap)
                Throw xThrow
            Else
                Throw x
            End If
        End Sub
    End Class
End Namespace