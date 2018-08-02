Imports Microsoft.VisualBasic
Imports System.Net.Http
Imports System.Net.Http.Formatting
Imports System.Collections.Generic
Imports TCUtilities = Talent.Common.Utilities

Public Class SystemDefaultsBasePage
	Inherits TalentBase01

	Private _wfr As New Talent.Common.WebFormResource
	Private _languageCode As String = TCUtilities.GetDefaultLanguage()
	Private _systemDefaultsUrl As String = String.Empty

	Sub New()
		setupWFR()
	End Sub

	Protected Function GetValues(ByVal apiName As String) As String
		Dim content As String = String.Empty
		Dim queryString As String = String.Empty
		Dim method As String = String.Empty
		Dim response As HttpResponseMessage = Nothing
		Using client As New HttpClient()
			queryString = String.Empty
			SetDefaultSystemDefaultsUrl()
			If Not String.IsNullOrWhiteSpace(ModuleDefaults.SystemDefaultsUrl) Then
				_systemDefaultsUrl = ModuleDefaults.SystemDefaultsUrl
			End If
			client.BaseAddress = New Uri(_systemDefaultsUrl)
			If Request.Url.Query <> String.Empty Then
				queryString = Request.Url.Query & "&SessionID=" + Session.SessionID
			Else
				queryString = Request.Url.Query & "?SessionID=" + Session.SessionID
			End If
			method = String.Format("api/{0}", apiName)
			response = client.GetAsync(method + queryString).Result
			content = ProcessResponse(response)
		End Using

		Return content
	End Function

	Protected Function PostValues(ByVal apiName As String, ByVal values As Object) As String
		Dim content As String = String.Empty
		Dim requestParams As ObjectContent = Nothing
		Dim queryString As String = String.Empty
		Dim method As String = String.Empty
		Dim response As HttpResponseMessage = Nothing
		Using client As New HttpClient()
			SetDefaultSystemDefaultsUrl()
			If Not String.IsNullOrWhiteSpace(ModuleDefaults.SystemDefaultsUrl) Then
				_systemDefaultsUrl = ModuleDefaults.SystemDefaultsUrl
			End If
			client.BaseAddress = New Uri(_systemDefaultsUrl)
			requestParams = New ObjectContent(values.GetType(), values, New JsonMediaTypeFormatter())
			queryString = Request.Url.Query + "&SessionID=" + Session.SessionID
			method = String.Format("api/{0}", apiName)
			response = client.PostAsync(method + queryString, requestParams).Result
			content = ProcessResponse(response)
		End Using

		Return content
	End Function

	Private Function ProcessResponse(ByVal response As HttpResponseMessage) As String

		Dim content As String = String.Empty

		If response.IsSuccessStatusCode Then
			content = response.Content.ReadAsStringAsync().Result
		ElseIf response.StatusCode = Net.HttpStatusCode.Unauthorized Then
			Dim msg As String = _wfr.Content("UnauthorizedRequestMsg", _languageCode, True)
			ShowErrorMessage(msg)
		ElseIf response.StatusCode = Net.HttpStatusCode.InternalServerError Then
			Dim msg As String = _wfr.Content("UnhandledExceptionMsg", _languageCode, True)
			ShowErrorMessage(msg)
		End If

		Return content
	End Function

	Protected Overridable Sub ShowErrorMessage(ByVal msg As String)
		Throw New NotImplementedException
	End Sub

	Protected Sub HandleException(ae As AggregateException)
		ae.Handle(Function(ex)
							Dim msg As String = String.Empty
							Dim retVal As Boolean = False

							If (TypeOf (ex) Is TimeoutException) Then
								msg = _wfr.Content("TimeoutErrorMsg", _languageCode, True)
								retVal = True
							ElseIf (TypeOf (ex) Is HttpRequestException) Then
								msg = _wfr.Content("SitedownMsg", _languageCode, True)
								retVal = True
							Else
								msg = _wfr.Content("UnknownErrorMsg", _languageCode, True)
								retVal = True
							End If

							If retVal Then
								ShowErrorMessage(msg)
							End If
							Return retVal 'Let anything else stop the application
						End Function)
	End Sub

	Private Function Stringify(ByVal values As Dictionary(Of String, String)) As String
		Dim results As New List(Of String)
		For Each key In values.Keys
			results.Add(String.Format("{0}={1}", key, values(key)))
		Next
		Return String.Join(",", results.ToArray)
	End Function

	Sub setupWFR()
		With _wfr
			.BusinessUnit = TalentCache.GetBusinessUnit()
			.PageCode = "SystemDefaultsBasePage.vb"
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
			.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
			.KeyCode = "SystemDefaultsBasePage.vb"
		End With
	End Sub

	Private Sub SetDefaultSystemDefaultsUrl()
		_systemDefaultsUrl = Request.Url.ToString().Replace(Request.Url.AbsolutePath, "")
		If Not String.IsNullOrWhiteSpace(Request.Url.Query) Then
			_systemDefaultsUrl = _systemDefaultsUrl.Replace(Request.Url.Query, "")
		End If
		_systemDefaultsUrl = _systemDefaultsUrl & "/SystemDefaults/"	'default webAPI URL
	End Sub

End Class
