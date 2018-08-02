Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.eCommerce
Imports Talent.Common
'Imports TEBUtilities = Talent.eCommerce.Utilities
Imports TCUtilities = Talent.Common.Utilities

Public Class TalentSessionProvider

#Region "Class Level Fields"

    Private _overrideNotPostBack As Boolean = False
    Private _talNoise As TalentNoise = Nothing
    Private _currentSessionId As String = Nothing
    Private _currentPageName As String = Nothing
    Private _talentLogging As TalentLogging = Nothing

#End Region

#Region "Properties"

    Public WriteOnly Property CurrentPageName() As String
        Set(ByVal value As String)
            _currentPageName = value.ToLower
        End Set
    End Property

    Public Property InValidSessionPageURL() As String = String.Empty
    Public Property TrackingScript() As String = String.Empty
    Public Property PageIsPostBack As Boolean = False
    Public Property EcomModuleDefaultsValues As ECommerceModuleDefaults.DefaultValues = Nothing
    Public Property CurrentBusinessUnitURLDeviceEntity As DEBusinessUnitURLDevice = Nothing
    Public Property AllBUURLDeviceEntities As Generic.Dictionary(Of String, DEBusinessUnitURLDevice) = Nothing
    Public Property SettingsNoise As DESettings = Nothing
    Public Property AgentProfile As Agent = Nothing

#End Region

#Region "Constructor"

    Public Sub New()
        _currentSessionId = HttpContext.Current.Session.SessionID
    End Sub

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Determines whether [is invalid session].
    ''' </summary><returns>
    '''   <c>true</c> if [is invalid session]; otherwise, <c>false</c>.
    ''' </returns>
    Public Function IsInvalidSession() As Boolean

        Dim isInValid As Boolean = False

        'is log out page then no need to do anything
        If IsNotLoggedOutPage() Then

            'is session validation required
            If CanProcessSessionValidation() Then

                'is not valid session
                If IsNotValidSession() Then

                    isInValid = True

                End If 'notvalidsession if ends

            End If 'session validation required if ends

        End If 'is log out page if ends

        Return isInValid

    End Function

    ''' <summary>
    ''' Processes the in-valid session to offer valid session
    ''' </summary>
    Public Sub ProcessInValidSession()
        'Is Browser Supports Cookie or not valid due to cookieless
        If IsBrowserSupportCookie() Then

            'is session offered from queue db if direct access allowed
            If IsSessionOffered() Then

                'is session details created in active noise table
                If HaveAddedOrUpdatedNoiseSession(Now) Then
                    LogSessionDetails("ProcessInValidSession", "")
                    RedirectByJSForm("noiseform", GetSuccessActionURL())
                End If

            End If 'sessionoffered if ends

        End If 'browsersupportcookie if ends
    End Sub

#End Region

#Region "Private Methods"
    ''' <summary>
    ''' Processes the session provider. if required in future change this to public
    ''' </summary>
    Private Sub ProcessSessionProvider()

        'is log out page then no need to do anything
        If IsNotLoggedOutPage() Then

            'is session validation required
            If CanProcessSessionValidation() Then

                'is not valid session
                If IsNotValidSession() Then

                    'Is Browser Supports Cookie or not valid due to cookieless
                    If IsBrowserSupportCookie() Then

                        'is session offered from queue db if direct access allowed
                        If IsSessionOffered() Then

                            'is session details created in active noise table
                            If HaveAddedOrUpdatedNoiseSession(Now) Then

                                RedirectByJSForm("noiseform", GetSuccessActionURL())

                            End If

                        End If 'sessionoffered if ends

                    End If 'browsersupportcookie if ends

                End If 'notvalidsession if ends

            End If 'session validation required if ends

        End If 'is log out page if ends
    End Sub

    ''' <summary>
    ''' Redirects the page by using html form with JS submit.
    ''' </summary>
    ''' <param name="formName">Name of the form.</param>
    ''' <param name="actionPageURL">The action page URL.</param>
    Private Sub RedirectByJSForm(ByVal formName As String, ByVal actionPageURL As String)
        'Direct the customer to the noise pages if it is not postback
        Dim isRedirected As Boolean = False
        Dim sbHtmlRedirectForm As New StringBuilder
        sbHtmlRedirectForm.AppendLine("<html><body>[[[TRACKINGSCRIPT]]]<form id='" & formName & "' name='" & formName & "' action='" & actionPageURL & "' method='post'>")
        sbHtmlRedirectForm.AppendLine("<input type=""hidden"" id=""hidJSSupport"" name=""hidJSSupport"" value=""supportsjs"" />")
        If HttpContext.Current.Request.Form("promotion-code") IsNot Nothing Then
            sbHtmlRedirectForm.Append("<input type=""hidden"" id=""promotion-code"" name=""promotion-code"" value=""")
            sbHtmlRedirectForm.Append(TCUtilities.TripleDESEncode(HttpContext.Current.Request.Form("promotion-code"), EcomModuleDefaultsValues.NOISE_ENCRYPTION_KEY))
            sbHtmlRedirectForm.AppendLine(""" />")
        ElseIf HttpContext.Current.Request.QueryString("xcvs") IsNot Nothing Then
            sbHtmlRedirectForm.Append("<input type=""hidden"" id=""promotion-code"" name=""promotion-code"" value=""")
            sbHtmlRedirectForm.Append(HttpContext.Current.Request.QueryString("xcvs"))
            sbHtmlRedirectForm.AppendLine(""" />")
        End If
        sbHtmlRedirectForm.AppendLine("</form>")
        sbHtmlRedirectForm.AppendLine("<script type='text/javascript'>")
        sbHtmlRedirectForm.AppendLine("document.getElementById('" & formName & "').submit();")
        sbHtmlRedirectForm.AppendLine("</script>")
        sbHtmlRedirectForm.AppendLine("</body>")
        sbHtmlRedirectForm.AppendLine("</html>")
        sbHtmlRedirectForm.AppendLine("Automatically redirect to site. If not, Please <a href='" & actionPageURL & "'>click here for site</a>")

        If EcomModuleDefaultsValues.TrackingCodeInUse Then
            'if ega querystring exists then tracking code already executed so don't execute now
            If HttpContext.Current.Request.QueryString("ega") Is Nothing Then
                If TrackingScript.Length > 0 Then
                    isRedirected = True
                    HttpContext.Current.Response.Buffer = True
                    'HttpContext.Current.Response.Write(TrackingScript)
                    'HttpContext.Current.Response.Flush()
                    sbHtmlRedirectForm.Replace("[[[TRACKINGSCRIPT]]]", TrackingScript)
                    HttpContext.Current.Response.Write(sbHtmlRedirectForm.ToString)
                    HttpContext.Current.Response.Flush()
                    HttpContext.Current.Response.End()
                End If
            End If
        End If
        If Not isRedirected Then
            sbHtmlRedirectForm.Replace("[[[TRACKINGSCRIPT]]]", "")
            HttpContext.Current.Response.Write(sbHtmlRedirectForm.ToString)
        End If
    End Sub

    ''' <summary>
    ''' Logs the session details to text file
    ''' </summary>
    ''' <param name="methodName">Name of the method.</param>
    ''' <param name="logMessage">The log message.</param>
    Private Sub LogSessionDetails(ByVal methodName As String, ByVal logMessage As String)
        If _talentLogging Is Nothing Then
            _talentLogging = New TalentLogging
            _talentLogging.FrontEndConnectionString = SettingsNoise.FrontEndConnectionString
        End If
        Dim sbLogMessage As New StringBuilder
        sbLogMessage.Append(logMessage)
        sbLogMessage.Append("$;$ UserAgent:" & GetNullAsEmptyString(HttpContext.Current.Request.UserAgent))
        If HttpContext.Current.Request.UrlReferrer IsNot Nothing Then
            sbLogMessage.Append("$;$ Referrer:" & GetNullAsEmptyString(HttpContext.Current.Request.UrlReferrer.AbsoluteUri))
        End If
        sbLogMessage.Append("$;$ AbsoluteUri:" & HttpContext.Current.Request.Url.AbsoluteUri)
        sbLogMessage.Append("$;$ HTTP_ACCEPT:" & GetNullAsEmptyString(HttpContext.Current.Request.ServerVariables("HTTP_ACCEPT")))
        sbLogMessage.Append("$;$ HTTP_ACCEPT_LANGUAGE:" & GetNullAsEmptyString(HttpContext.Current.Request.ServerVariables("HTTP_ACCEPT_LANGUAGE")))
        sbLogMessage.Append("$;$ HTTP_COOKIE:" & GetNullAsEmptyString(HttpContext.Current.Request.ServerVariables("HTTP_COOKIE")))
        sbLogMessage.Append("$;$ QUERY_STRING:" & GetNullAsEmptyString(HttpContext.Current.Request.ServerVariables("QUERY_STRING")))
        sbLogMessage.Append("$;$ REMOTE_ADDR:" & GetNullAsEmptyString(HttpContext.Current.Request.ServerVariables("REMOTE_ADDR")))
        sbLogMessage.Append("$;$ REMOTE_HOST:" & GetNullAsEmptyString(HttpContext.Current.Request.ServerVariables("REMOTE_HOST")))
        sbLogMessage.Append("$;$ REMOTE_USER:" & GetNullAsEmptyString(HttpContext.Current.Request.ServerVariables("REMOTE_USER")))
        sbLogMessage.Append("$;$ REQUEST_METHOD:" & GetNullAsEmptyString(HttpContext.Current.Request.ServerVariables("REQUEST_METHOD")))
        _talentLogging.GeneralLog("TalentSessionProvider.vb", methodName, sbLogMessage.ToString, "SessionProviderLogging")
    End Sub

    ''' <summary>
    ''' Gets the null string as empty string.
    ''' </summary>
    ''' <param name="valueToValidate">The value to validate.</param><returns></returns>
    Private Function GetNullAsEmptyString(ByVal valueToValidate As String) As String
        If String.IsNullOrWhiteSpace(valueToValidate) Then
            valueToValidate = ""
        End If
        Return valueToValidate
    End Function

    ''' <summary>
    ''' Determines whether [is not logged out page].
    ''' </summary><returns>
    '''   <c>true</c> if [is not logged out page]; otherwise, <c>false</c>.
    ''' </returns>
    Private Function IsNotLoggedOutPage() As Boolean
        Dim isNotLogOutPage As Boolean = True
        If EcomModuleDefaultsValues.NOISE_CLEAR_SESSION_ON_LOGOUT AndAlso _currentPageName = LCase("loggedout.aspx") Then
            isNotLogOutPage = False
        End If
        Return isNotLogOutPage
    End Function

    ''' <summary>
    ''' Determines whether this instance [can process session validation].
    ''' </summary><returns>
    '''   <c>true</c> if this instance [can process session validation]; otherwise, <c>false</c>.
    ''' </returns>
    Private Function CanProcessSessionValidation() As Boolean
        Dim canProcess As Boolean = False
        If EcomModuleDefaultsValues.NOISE_IN_USE Then
            If IsSingleProductFilterNotExists() AndAlso IsThisPageRequireSessionValidation() Then
                canProcess = True
            End If
        Else
            If EcomModuleDefaultsValues.WholeSiteIsInAgentMode Then
                'Check if this is an agent, if so populate the agent object
                Dim talNoise As New TalentNoise(SettingsNoise, _currentSessionId, Now, Now.AddMinutes(-EcomModuleDefaultsValues.NOISE_THRESHOLD_MINUTES), _
                                                EcomModuleDefaultsValues.NOISE_MAX_SESSION_KEEP_ALIVE_MINUTES + EcomModuleDefaultsValues.NOISE_MAX_SESSION_CHECKOUT_ADD_MINUTES)
                talNoise.CheckAndUpdateAgentNoiseSession()
                If talNoise.De_Noise.IsAgent Then
                    AgentProfile.Populate(True, talNoise.De_Noise.AgentName, talNoise.De_Noise.AgentType)
                Else
                    AgentProfile.Clear()
                End If
            Else
                AgentProfile.Clear()
            End If
        End If
        Return canProcess
    End Function

    ''' <summary>
    ''' Determines whether [is single product filter not exists].
    ''' </summary><returns>
    '''   <c>true</c> if [is single product filter not exists]; otherwise, <c>false</c>.
    ''' </returns>
    Private Function IsSingleProductFilterNotExists() As Boolean
        Dim isNotExists As Boolean = True
        If (Not String.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString("IsSingleProduct"))) AndAlso (HttpContext.Current.Request.QueryString("IsSingleProduct") = "TRUE") Then
            If (PageIsPostBack) Then
                _overrideNotPostBack = True
            Else
                isNotExists = False
            End If
        End If
        Return isNotExists
    End Function

    ''' <summary>
    ''' Determines whether [is this page require session validation].
    ''' </summary><returns>
    '''   <c>true</c> if [is this page require session validation]; otherwise, <c>false</c>.
    ''' </returns>
    Private Function IsThisPageRequireSessionValidation() As Boolean
        Dim isRequired As Boolean = True
        If _currentPageName = LCase("TicketingGateway.aspx") _
            OrElse _currentPageName = LCase("MicroBasket.aspx") _
            OrElse _currentPageName = LCase("AgentLogin.aspx") _
            OrElse _currentPageName = LCase("clearcache.aspx") _
            OrElse _currentPageName = LCase("PrerequisiteMissing.aspx") _
            OrElse _currentPageName = LCase("SessionError.aspx") Then
            isRequired = False
        End If
        Return isRequired
    End Function

    ''' <summary>
    ''' Determines whether [is this page require extra session time].
    ''' If the user has entered the checkout pages we will grant them extra time on top of the
    ''' max session time to allow them to complete checkout. This will only take effect on the payment and confirmation pages.
    ''' </summary><returns>
    '''   <c>true</c> if [is this page require extra session time]; otherwise, <c>false</c>.
    ''' </returns>
    Private Function IsThisPageRequireExtraSessionTime() As Boolean
        Dim isRequired As Boolean = False
        If _currentPageName = LCase("checkout.aspx") _
                OrElse _currentPageName = LCase("checkoutorderconfirmation.aspx") Then
            isRequired = True
        End If
        Return isRequired
    End Function

    ''' <summary>
    ''' Determines whether [is browser support cookie].
    ''' </summary><returns>
    '''   <c>true</c> if [is browser support cookie]; otherwise, <c>false</c>.
    ''' </returns>
    Private Function IsBrowserSupportCookie() As Boolean
        Dim isSupportCookie As Boolean = True
        Dim errorParameters As String = String.Empty
        If HttpContext.Current.Request("soalready") IsNot Nothing AndAlso HttpContext.Current.Request("soalready").Length > 0 Then
            'in ticketing gateway encrypt the session id once created the row in tbl_active_session table
            'here decrypt the value from the querystring and compare the current session id if both are different
            'then browser doesn't support cookie redirect the user to session error page with querystring 
            'to show the cookieless error message

            Dim previousSessionID As String = TCUtilities.TripleDESDecode(HttpContext.Current.Request("soalready").ToString.Replace(" ", "+"), EcomModuleDefaultsValues.NOISE_ENCRYPTION_KEY)
            If previousSessionID <> _currentSessionId Then
                errorParameters = "cookieless=true"
                isSupportCookie = False
                If Not (HttpContext.Current.Request("hidJSSupport") IsNot Nothing AndAlso HttpContext.Current.Request("hidJSSupport") = "supportsjs") Then
                    errorParameters = errorParameters & "&clientscriptless=true"
                End If
            End If
            LogSessionDetails("IsBrowserSupportCookie", "PreviousSessionId:" & previousSessionID & ";CurrentSessionId:" & _currentSessionId & ";Parameters:" & errorParameters)
        End If
        If Not isSupportCookie Then
            Dim actionURL As String = InValidSessionPageURL
            If actionURL.Contains("?") Then
                actionURL = actionURL & "&" & errorParameters
            Else
                actionURL = actionURL & "?" & errorParameters
            End If
            HttpContext.Current.Response.Redirect(actionURL)
        End If
        Return isSupportCookie
    End Function

    ''' <summary>
    ''' Determines whether [is not valid session].
    ''' </summary><returns>
    '''   <c>true</c> if [is not valid session]; otherwise, <c>false</c>.
    ''' </returns>
    Private Function IsNotValidSession() As Boolean
        Dim isNotValid As Boolean = True
        Dim checkoutAddMinutes As Integer = 0

        If IsThisPageRequireExtraSessionTime() Then
            checkoutAddMinutes = EcomModuleDefaultsValues.NOISE_MAX_SESSION_CHECKOUT_ADD_MINUTES
        End If

        'validate the session against the DB
        _talNoise = New TalentNoise(SettingsNoise, _currentSessionId, Now, Now.AddMinutes(-EcomModuleDefaultsValues.NOISE_THRESHOLD_MINUTES), _
                                        EcomModuleDefaultsValues.NOISE_MAX_SESSION_KEEP_ALIVE_MINUTES + checkoutAddMinutes)
        _talNoise.CheckAndUpdateNoiseSession()

        'if the call failed or there was no record forward to the invalid session page
        If Not _talNoise.SuccessfullCall OrElse _talNoise.RowsAffected <= 0 Then
            isNotValid = True
        Else
            isNotValid = False
            'Check if this is an agent, if so populate the agent object
            If _talNoise.RowsAffected >= 0 AndAlso _talNoise.De_Noise.IsAgent Then
                AgentProfile.Populate(True, _talNoise.De_Noise.AgentName, _talNoise.De_Noise.AgentType)
            Else
                AgentProfile.Clear()
            End If
        End If
        Return isNotValid
    End Function

    ''' <summary>
    ''' Determines whether [is session offered].
    ''' </summary><returns>
    '''   <c>true</c> if [is session offered]; otherwise, <c>false</c>.
    ''' </returns>
    Private Function IsSessionOffered() As Boolean
        Dim isOffered As Boolean = False
        'check can connect queue db directly from ecom module defaults
        'if yes connect and excute the stored proc to get valid session
        If EcomModuleDefaultsValues.QueueDBDirectAccessAllowed Then
            'call talent queue to get resultset
            Try
                Dim talQueue As New TalentQueue
                talQueue.Settings = SettingsNoise
                talQueue.Settings.Cacheing = False
                Dim errObj As ErrorObj = talQueue.UpdateSiteActivityTotal()
                If (Not errObj.HasError) AndAlso (talQueue.ResultDataSet IsNot Nothing) Then
                    If (talQueue.ResultDataSet.Tables.Count > 0) AndAlso (talQueue.ResultDataSet.Tables(0).Rows.Count > 0) Then
                        Dim updatedTotal As Integer = TCUtilities.CheckForDBNull_Int(talQueue.ResultDataSet.Tables(0).Rows(0)(0))
                        If updatedTotal > 0 Then
                            isOffered = True
                        End If
                    End If
                End If
            Catch ex As Exception
                isOffered = False
            End Try
        End If
        If Not isOffered Then
            'session not offered
            Dim actionURL As String = InValidSessionPageURL
            If ((Not PageIsPostBack) OrElse (_overrideNotPostBack)) Then
                'redirect the user to queue url after executed google analytics
                '                actionURL = EcomModuleDefaultsValues.NOISE_URL & "?returnUrl=" & HttpContext.Current.Server.UrlEncode(HttpContext.Current.Request.Url.AbsoluteUri)
                actionURL = EcomModuleDefaultsValues.NOISE_URL & "?returnUrl=" & HttpContext.Current.Server.UrlEncode(FormatURL())
            End If
            RedirectByJSForm("noiseform", actionURL)
        End If
        Return isOffered
    End Function
    Private Function FormatURL() As String
        Dim retURI As String = String.Empty
        Dim SSLOffloaded As String = ConfigurationManager.AppSettings("SSLOffloaded")
        If String.IsNullOrWhiteSpace(SSLOffloaded) Then
            SSLOffloaded = ""
        End If
        ' Reformat the URI to be an https request if scheme is http and port is 443 - this is an ssl page when ssl is off-loaded.
        If SSLOffloaded.ToLower = "true" Then
            If HttpContext.Current.Request.Url.Scheme = "http" And HttpContext.Current.Request.Url.Port = "443" Then
                retURI = "https://" + HttpContext.Current.Request.Url.Host.ToString.Trim + HttpContext.Current.Request.Url.PathAndQuery
            End If
        End If

        ' Return HttpContext.Current.Request.Url.AbsoluteUri by default
        If retURI = String.Empty Then
            retURI = HttpContext.Current.Request.Url.AbsoluteUri
        End If
        If HttpContext.Current.Request.Form("promotion-code") IsNot Nothing Then
            'don't expose "promotion code" as part of the URL hence meaningless querystring key
            If HttpContext.Current.Request.QueryString().Count > 0 Then
                retURI = retURI & "&xcvs=" & TCUtilities.TripleDESEncode(HttpContext.Current.Request.Form("promotion-code"), EcomModuleDefaultsValues.NOISE_ENCRYPTION_KEY)
            Else
                retURI = "?xcvs=" & TCUtilities.TripleDESEncode(HttpContext.Current.Request.Form("promotion-code"), EcomModuleDefaultsValues.NOISE_ENCRYPTION_KEY)
            End If
        End If

        Return retURI
    End Function

    ''' <summary>
    ''' Haves the added or updated noise session.
    ''' </summary>
    ''' <param name="sessionDateTime">The session date time.</param><returns></returns>
    Private Function HaveAddedOrUpdatedNoiseSession(ByVal sessionDateTime As DateTime) As Boolean
        'create a record tbl_active_sessions and redirect/allow the user to the requested page
        Dim addOrUpdated As Boolean = False
        If sessionDateTime >= Now.AddMinutes(-EcomModuleDefaultsValues.NOISE_THRESHOLD_MINUTES) _
                        AndAlso sessionDateTime <= Now.AddMinutes(EcomModuleDefaultsValues.NOISE_THRESHOLD_MINUTES) Then
            'if the session is valid, add it to the DB and forward on
            Dim sessionStartTime As DateTime = Now
            Dim talNoise As New TalentNoise(SettingsNoise, _
                                            HttpContext.Current.Session.SessionID, _
                                            sessionStartTime, _
                                            sessionStartTime.AddMinutes(-EcomModuleDefaultsValues.NOISE_THRESHOLD_MINUTES), _
                                            EcomModuleDefaultsValues.NOISE_MAX_SESSION_KEEP_ALIVE_MINUTES)
            talNoise.De_Noise.ServerIP = HttpContext.Current.Request.ServerVariables("LOCAL_ADDR").ToString
            talNoise.De_Noise.ClientIP = HttpContext.Current.Request.UserHostAddress()
            talNoise.AddOrUpdateNoiseSession(EcomModuleDefaultsValues.NOISE_MAX_CONCURRENT_USERS)
            If talNoise.SuccessfullCall AndAlso talNoise.RowsAffected > 0 Then
                addOrUpdated = True
                'Add a session var so we can tell the 
                'user how long their session has left
                If HttpContext.Current.Session.Item("NoiseSessionStartTime") Is Nothing Then
                    HttpContext.Current.Session.Add("NoiseSessionStartTime", sessionStartTime)
                Else
                    HttpContext.Current.Session.Item("NoiseSessionStartTime") = sessionStartTime
                End If
            End If
        End If
        If Not addOrUpdated Then
            RedirectByJSForm("noiseform", InValidSessionPageURL)
        End If
        Return addOrUpdated
    End Function

    ''' <summary>
    ''' Gets the success action URL.
    ''' </summary><returns></returns>
    Private Function GetSuccessActionURL() As String
        Dim actionURL As String = HttpContext.Current.Request.Url.AbsoluteUri
        If HttpContext.Current.Request.QueryString.Count > 0 Then
            actionURL = actionURL & "&"
        Else
            actionURL = actionURL.TrimEnd("?") & "?"
        End If
        actionURL = actionURL & "soalready=" & TCUtilities.TripleDESEncode(_currentSessionId, EcomModuleDefaultsValues.NOISE_ENCRYPTION_KEY)
        Return actionURL
    End Function

    ''' <summary>
    ''' Gets the formatted device type URL.
    ''' </summary>
    ''' <param name="actionURL">The action URL.</param><returns></returns>
    Private Function GetFormattedDeviceTypeURL(ByVal actionURL As String) As String
        If Not (actionURL.ToLower.StartsWith("http://") OrElse actionURL.ToLower.StartsWith("https://")) Then
            actionURL = "http://" & actionURL
        End If
        If actionURL.Contains("?") Then
            actionURL = actionURL & "&ega=false"
        Else
            actionURL = actionURL & "?ega=false"
        End If
        Return actionURL
    End Function

#End Region

    Private Sub CheckForValidSession_Ori()
        'Dim invalidSessionPageName As String = "SessionError.aspx"
        'Dim invalidSessionURL As String = "~/PagesPublic/Error/" & invalidSessionPageName
        'Dim currentPageName As String = Talent.eCommerce.Utilities.GetCurrentPageName
        'Dim noiseSettings As DESettings = Talent.eCommerce.Utilities.GetSettingsObject

        'Dim s As String = Session.SessionID
        ''check that we are using noise and that we are not already on the invalid session id page
        'If ModuleDefaults.NOISE_IN_USE AndAlso CanProcessSessionValidation() AndAlso IsThisPageRequireSessionValidation(currentPageName) Then

        '    If ModuleDefaults.NOISE_CLEAR_SESSION_ON_LOGOUT AndAlso LCase(currentPageName) = LCase("loggedout.aspx") Then Exit Sub

        '    'If the user has entered the checkout pages we will grant them extra time on top of the
        '    'max session time to allow them to complete checkout. This will only take effect on the payment and confirmation pages.
        '    Dim checkoutAddMinutes As Integer = 0
        '    If LCase(currentPageName) = LCase("checkout.aspx") _
        '        OrElse LCase(currentPageName) = LCase("checkoutorderconfirmation.aspx") Then
        '        checkoutAddMinutes = ModuleDefaults.NOISE_MAX_SESSION_CHECKOUT_ADD_MINUTES
        '    End If

        '    'validate the session against the DB
        '    Dim myNoise As New TalentNoise(noiseSettings, Session.SessionID, Now, Now.AddMinutes(-ModuleDefaults.NOISE_THRESHOLD_MINUTES), _
        '                                    ModuleDefaults.NOISE_MAX_SESSION_KEEP_ALIVE_MINUTES + checkoutAddMinutes)
        '    myNoise.CheckAndUpdateNoiseSession()

        '    'if the call failed or there was no record forward to the invalid session page
        '    If Not myNoise.SuccessfullCall OrElse myNoise.RowsAffected <= 0 Then
        '        'Direct the customer to the noise pages if it is not postback
        '        Dim isRedirected As Boolean = False
        '        If ((Not Page.IsPostBack) OrElse (_overrideNotPostBack)) Then
        '            invalidSessionURL = ModuleDefaults.NOISE_URL & "?returnUrl=" & HttpContext.Current.Server.UrlEncode(HttpContext.Current.Request.Url.AbsoluteUri)
        '            If Not ModuleDefaults Is Nothing AndAlso ModuleDefaults.TrackingCodeInUse Then
        '                Dim trackingScript As String = TrackingCodes.DoInsertTrackingCodes("BODY")
        '                If trackingScript.Length > 0 Then
        '                    isRedirected = True
        '                    Response.Buffer = True
        '                    Response.Write(trackingScript)
        '                    Response.Flush()
        '                    Response.Write("<html><body><form id='noiseform' name='noiseform' action='" & invalidSessionURL & "' method='post'></form>")
        '                    Response.Write("<script type='text/javascript'>")
        '                    Response.Write("document.getElementById('noiseform').submit();")
        '                    Response.Write("</script>")
        '                    Response.Write("</body>")
        '                    Response.Write("</html>")
        '                    Response.Write("Automatically redirect to site. If not, Please <a href='" & invalidSessionURL & "'>click here for site</a>")
        '                    Response.End()
        '                End If
        '            End If
        '        End If
        '        If Not isRedirected Then
        '            Response.Redirect(invalidSessionURL)
        '        End If
        '    Else
        '        'Check if this is an agent, if so populate the agent object
        '        If myNoise.RowsAffected <= 0 AndAlso myNoise.De_Noise.IsAgent Then AgentProfile.Populate(True, myNoise.De_Noise.AgentName, myNoise.De_Noise.AgentType)
        '    End If

        'Else
        '    'Check if this is an agent, if so populate the agent object
        '    Dim myNoise As New TalentNoise(noiseSettings, Session.SessionID, Now, Now.AddMinutes(-ModuleDefaults.NOISE_THRESHOLD_MINUTES), _
        '                                    ModuleDefaults.NOISE_MAX_SESSION_KEEP_ALIVE_MINUTES + ModuleDefaults.NOISE_MAX_SESSION_CHECKOUT_ADD_MINUTES)
        '    myNoise.CheckAndUpdateAgentNoiseSession()
        '    If myNoise.De_Noise.IsAgent Then AgentProfile.Populate(True, myNoise.De_Noise.AgentName, myNoise.De_Noise.AgentType)
        'End If
    End Sub

End Class
