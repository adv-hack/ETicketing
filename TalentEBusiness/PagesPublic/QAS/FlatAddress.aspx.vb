Imports Microsoft.VisualBasic
Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Pages Public - Flat Address
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      PPCONUS- 
'                                    
'       User Controls
'           contactUsDetails(5)
'
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Imports System
Imports System.Web
Imports System.Collections
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Web.SessionState
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports com.qas.proweb       'QuickAddress services

Partial Class PagesPublic_FlatAddress
    Inherits TalentBase01

#Region "QAS Constants"
    'Page filenames
    Protected Const PAGE_BEGIN As String = "FlatCountry.aspx"
    Protected Const PAGE_INPUT As String = "FlatPrompt.aspx"
    Protected Const PAGE_SEARCH As String = "FlatSearch.aspx"
    Protected Const PAGE_REFINE As String = "FlatRefine.aspx"
    Protected Const PAGE_FORMAT As String = "FlatAddress.aspx"

    'Field names specific to the Flattened scenario
    'Which prompt set is selected - set on PAGE_INPUT, also used by PAGE_SEARCH
    Protected Const FIELD_PROMPTSET As String = "PromptSet"
    'Used to recreate the picklist - set and used by PAGE_SEARCH
    Protected Const FIELD_PICKLIST_MONIKER As String = "PicklistMoniker"
    ' The picklist item requiring refinement - set on PAGE_SEARCH, used by PAGE_REFINE
    Protected Const FIELD_REFINE_MONIKER As String = "RefineMoniker"
#End Region

    Private wfr As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    'Result arrays
    Protected m_asAddressLines As String()
    Protected m_asAddressLabels As String()
    Protected m_eRoute As com.qas.prowebintegration.Constants.Routes = com.qas.prowebintegration.Constants.Routes.Undefined


    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        SetRoute(GetRouteBase())
        If Not IsPostBack Then
            FormatAddress(m_asAddressLabels, m_asAddressLines)
            Try
                If m_asAddressLines(5) = String.Empty Then
                    m_asAddressLines(5) = Request("postCode")
                    If m_asAddressLines(5) = String.Empty Then
                        Dim postCodeIndex As Integer = Request.Url.AbsoluteUri.IndexOf("&postCode")
                        If postCodeIndex > 1 Then
                            m_asAddressLines(5) = Server.UrlDecode(Request.Url.AbsoluteUri.Substring(postCodeIndex + 10, Request.Url.AbsoluteUri.Length - postCodeIndex - 10))
                        End If
                    End If


                End If
            Catch

            End Try
            SetWelcomeMessage(m_eRoute)
        End If
        'Else leave it to the event handlers (Accept, Back)

    End Sub




    '** Operations **


    ' Retrieve the formatted address from the Moniker, or create a set of blank lines
    Protected Sub FormatAddress(ByRef asLabels As String(), ByRef asLines As String())

        If Not (m_eRoute.Equals(com.qas.prowebintegration.Constants.Routes.PreSearchFailed) Or m_eRoute.Equals(com.qas.prowebintegration.Constants.Routes.Failed)) Then

            Try
                Dim sLayout As String = System.Configuration.ConfigurationManager.AppSettings(com.qas.prowebintegration.Constants.KEY_LAYOUT)
                Dim sServerURL As String = System.Configuration.ConfigurationManager.AppSettings(com.qas.prowebintegration.Constants.KEY_SERVER_URL)
                Dim sUsername As String = System.Configuration.ConfigurationManager.AppSettings(com.qas.prowebintegration.Constants.KEY_USER_NAME)
                Dim sPassword As String = System.Configuration.ConfigurationManager.AppSettings(com.qas.prowebintegration.Constants.KEY_PASSWORD)
                sLayout = ModuleDefaults.AddressingLayout.Trim
                Dim iSize As Integer
                Dim i As Integer
                Select Case ModuleDefaults.AddressingProvider
                    Case Is = "QAS"
                        Dim searchService As QuickAddress = NewQuickAddress()

                        Dim aLines As AddressLine()
                        If m_eRoute.Equals(com.qas.prowebintegration.Constants.Routes.Okay) Then
                            'Perform address formatting
                            aLines = searchService.GetFormattedAddress(GetMoniker(), sLayout).AddressLines
                        Else
                            'Use first example address to get line labels
                            aLines = searchService.GetExampleAddresses(GetDataID(), sLayout)(0).AddressLines
                        End If
                        'Build display address arrays
                        iSize = aLines.Length
                        ReDim asLabels(iSize - 1)
                        ReDim asLines(iSize - 1)


                        For i = 0 To iSize - 1
                            asLabels(i) = aLines(i).Label
                            asLines(i) = aLines(i).Line
                        Next i

                    Case Is = "QASONDEMAND"
                        Dim searchService As com.qas.prowebondemand.QuickAddress = New com.qas.prowebondemand.QuickAddress(sServerURL, sUsername, sPassword)
                        Dim aLines As com.qas.prowebondemand.AddressLine()
                        If m_eRoute.Equals(com.qas.prowebintegration.Constants.Routes.Okay) Then
                            'Perform address formatting
                            aLines = searchService.GetFormattedAddress(GetMoniker(), sLayout).AddressLines
                        Else
                            'Use first example address to get line labels
                            aLines = searchService.GetExampleAddresses(GetDataID(), sLayout)(0).AddressLines
                        End If
                        'Build display address arrays
                        iSize = aLines.Length
                        ReDim asLabels(iSize - 1)
                        ReDim asLines(iSize - 1)


                        For i = 0 To iSize - 1
                            asLabels(i) = aLines(i).Label
                            asLines(i) = aLines(i).Line
                        Next i
                End Select


            Catch x As Exception

                m_eRoute = com.qas.prowebintegration.Constants.Routes.Failed
                SetErrorInfo(x.Message)

            End Try

        End If


        If asLabels Is Nothing Or asLines Is Nothing Then

            'Provide default (empty) address for manual entry
            ReDim asLabels(5)
            asLabels(0) = "Address Line 1"
            asLabels(1) = "Address Line 2"
            asLabels(2) = "Address Line 3"
            asLabels(3) = "City"
            asLabels(4) = "State or Province"
            asLabels(5) = "ZIP or Postal Code"
            NotFound.Value = "Y"
            ReDim asLines(5)

        End If


    End Sub




    '** Page events **


#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region



    '' 'Back' button clicked
    'Private Sub ButtonBack_ServerClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonBack.ServerClick
    '    Select Case GetRoute()
    '        Case com.qas.prowebintegration.Constants.Routes.NoMatches, com.qas.prowebintegration.Constants.Routes.Timeout, com.qas.prowebintegration.Constants.Routes.TooManyMatches
    '            GoInputPage()
    '        Case com.qas.prowebintegration.Constants.Routes.Okay
    '            If Not GetRefineMoniker() = "" Then
    '                GoRefinementPage(Nothing)
    '            Else
    '                GoSearchPage()
    '            End If
    '        Case Else
    '            GoFirstPage()
    '    End Select
    'End Sub

    '' 'Accept' button clicked: move out of this scenario
    'Private Sub ButtonAccept_ServerClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonAccept.ServerClick
    '    '        GoFinalPage()
    'End Sub

    Protected Sub ButtonBack_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonBack.Click
        Select Case GetRoute()
            Case com.qas.prowebintegration.Constants.Routes.NoMatches, com.qas.prowebintegration.Constants.Routes.Timeout, com.qas.prowebintegration.Constants.Routes.TooManyMatches
                GoInputPage()
            Case com.qas.prowebintegration.Constants.Routes.Okay
                If Not GetRefineMoniker() = "" Then
                    GoRefinementPage(Nothing)
                Else
                    GoSearchPage()
                End If
            Case Else
                GoFirstPage()
        End Select
    End Sub

    Protected Sub ButtonAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonAccept.Click
        ' do nothing - javascript will do window.close();
    End Sub


    '** Page controls **


    ' Update the page welcome depending on the route we took to get here
    Private Sub SetWelcomeMessage(ByVal eRoute As com.qas.prowebintegration.Constants.Routes)

        Select Case eRoute

            Case com.qas.prowebintegration.Constants.Routes.Okay
                LiteralMessage.Text = "Please confirm your address below."

            Case com.qas.prowebintegration.Constants.Routes.NoMatches, com.qas.prowebintegration.Constants.Routes.Timeout, com.qas.prowebintegration.Constants.Routes.TooManyMatches
                LiteralMessage.Text = "Automatic address capture did not succeed.<br /><br />Please search again or enter your address below."

            Case Else
                LiteralMessage.Text = "Automatic address capture is not available.<br /><br />Please enter your address below."

        End Select

    End Sub


    ' Current search state, how we arrived on the address format page (i.e. too many matches)
    Protected Shadows Function GetRoute() As com.qas.prowebintegration.Constants.Routes
        Return m_eRoute
    End Function

    Protected Sub SetRoute(ByVal eRoute As com.qas.prowebintegration.Constants.Routes)
        m_eRoute = eRoute
    End Sub

#Region " QAS Helper Functions "

    ' Create a new QuickAddress service, connected to the configured server
    Protected Function NewQuickAddress() As QuickAddress

        'Retrieve server URL from web.config
        Dim sServerURL As String = System.Configuration.ConfigurationManager.AppSettings(com.qas.prowebintegration.Constants.KEY_SERVER_URL)
        'Create QuickAddress search object
        Return New QuickAddress(sServerURL)

    End Function


    ' Transfer to the initial page, to select the country
    Protected Sub GoFirstPage()

        Server.Transfer(PAGE_BEGIN)

    End Sub


    ' Transfer to the input page, which prompts for address terms
    Protected Sub GoInputPage()

        Server.Transfer(PAGE_INPUT)

    End Sub


    ' Transfer to the address searching and picklist display page
    Protected Sub GoSearchPage()

        Server.Transfer(PAGE_SEARCH)

    End Sub


    ' Transfer to the address refinement page, when additional information is required
    Protected Sub GoRefinementPage(ByVal sMoniker As String)

        If Not sMoniker Is Nothing Then
            SetRefineMoniker(sMoniker)
        End If

        Server.Transfer(PAGE_REFINE)

    End Sub


    ' Transfer to the address confirmation page to retrieve the found address
    Protected Sub GoFormatPage(ByVal sMoniker As String)

        If Not sMoniker Is Nothing Then
            SetMoniker(sMoniker)
        End If

        SetRoute(com.qas.prowebintegration.Constants.Routes.Okay)
        Server.Transfer(PAGE_FORMAT)

    End Sub


    ' Transfer to the address confirmation page for manual address entry, after capture failed
    Protected Sub GoErrorPage(ByVal route As com.qas.prowebintegration.Constants.Routes)

        SetRoute(route)
        Server.Transfer(PAGE_FORMAT)

    End Sub


    ' Transfer to the address confirmation page for manual address entry, after exception thrown
    Protected Sub GoErrorPage(ByVal x As Exception)

        SetRoute(com.qas.prowebintegration.Constants.Routes.Failed)
        SetErrorInfo(x.Message)
        Server.Transfer(PAGE_FORMAT)

    End Sub


    ' Transfer out of the scenario to the final (summary) page
    Protected Sub GoFinalPage()

        Server.Transfer(com.qas.prowebintegration.Constants.PAGE_FINAL_ADDRESS)

    End Sub


    ' Propagate a value through, from the Request into a hidden field on our page
    Protected Sub RenderRequestString(ByVal sKey As String)

        Dim sValue As String = Request(sKey)
        RenderHiddenField(sKey, sValue)

    End Sub


    ' Propagate values through, from the Request to hidden fields on our page
    Protected Sub RenderRequestArray(ByVal sKey As String)
        Dim asValues As String() = Request.Params.GetValues(sKey)
        If Not asValues Is Nothing Then
            Dim sValue As String
            For Each sValue In asValues
                RenderHiddenField(sKey, sValue)
            Next

            'Add dummy entry to 1-sized arrays to allow array subscripting in JavaScript
            If asValues.Length = 1 Then
                RenderHiddenField(sKey, "")
            End If
        End If

    End Sub


    ' Render a hidden field directly into the page
    Protected Sub RenderHiddenField(ByVal sKey As String, ByVal sValue As String)

        Response.Write("<input type=""hidden"" name=""")
        Response.Write(sKey)
        If Not sValue Is Nothing Then
            If sValue <> "" Then
                Response.Write(""" value=""")
                Response.Write(HttpUtility.HtmlEncode(sValue))
            End If
        End If

        Response.Write(""" />" & vbNewLine)

    End Sub


    ' Render a boolean hidden field directly into the page
    Protected Sub RenderHiddenField(ByVal sKey As String, ByVal bValue As Boolean)

        Response.Write("<input type=""hidden"" name=""")
        Response.Write(sKey)
        If bValue Then
            Response.Write(""" value=""")
            Response.Write(True.ToString())
        End If

        Response.Write(""" />" & vbNewLine)

    End Sub



    '** Stored parameters **


    ' Country data identifier (i.e. AUS)
    Protected Function GetDataID() As String
        Return Request(com.qas.prowebintegration.Constants.FIELD_DATA_ID)
    End Function


    ' Country display name (i.e. Australia)
    Protected Function GetCountryName() As String

        Return Request(com.qas.prowebintegration.Constants.FIELD_COUNTRY_NAME)
    End Function


    ' Prompt set selected
    Protected Function GetPromptSet() As PromptSet.Types
        Dim sValue As String = Request(FIELD_PROMPTSET)
        If Not sValue Is Nothing Then
            Return System.Enum.Parse(GetType(PromptSet.Types), sValue)
        Else
            Return PromptSet.Types.Optimal
        End If
    End Function

    Protected Sub SetPromptSet(ByVal ePromptSet As PromptSet.Types)
        Request.Cookies.Set(New HttpCookie(FIELD_PROMPTSET, ePromptSet.ToString()))
    End Sub


    ' Initial user search (i.e. "14 main street", "boston")
    Protected Function GetInputLines() As String()
        Dim asValues As String() = Request.Params.GetValues(com.qas.prowebintegration.Constants.FIELD_INPUT_LINES)
        If Not asValues Is Nothing Then
            Return asValues
        Else
            Dim asBlank As String()
            ReDim asBlank(0)
            Return asBlank
        End If
    End Function

    '
    ' Note that the GetRoute and SetRoute functions have had name change as they are override here (FlatAddress.aspx)
    '
    ' Current search state, how we arrived on the address format page (i.e. too many matches)
    Protected Function GetRouteBase() As com.qas.prowebintegration.Constants.Routes
        Dim sValue As String = Request(com.qas.prowebintegration.Constants.FIELD_ROUTE)
        If Not sValue Is Nothing Then
            Return System.Enum.Parse(GetType(com.qas.prowebintegration.Constants.Routes), sValue)
        Else
            Return com.qas.prowebintegration.Constants.Routes.Undefined
        End If
    End Function

    Private Sub SetRouteBase(ByVal eRoute As com.qas.prowebintegration.Constants.Routes)
        Request.Cookies.Set(New HttpCookie(com.qas.prowebintegration.Constants.FIELD_ROUTE, eRoute.ToString()))
    End Sub

    ' Error information returned through the exception
    Protected Function GetErrorInfo() As String
        Return Request(com.qas.prowebintegration.Constants.FIELD_ERROR_INFO)
    End Function

    Protected Sub SetErrorInfo(ByVal sErrorInfo As String)
        Request.Cookies.Set(New HttpCookie(com.qas.prowebintegration.Constants.FIELD_ERROR_INFO, sErrorInfo))
    End Sub


    ' Moniker of the final address
    Protected Function GetMoniker() As String
        Return Request(com.qas.prowebintegration.Constants.FIELD_MONIKER)
    End Function

    Private Sub SetMoniker(ByVal sMoniker As String)
        Request.Cookies.Set(New HttpCookie(com.qas.prowebintegration.Constants.FIELD_MONIKER, sMoniker))
    End Sub


    ' Moniker of the initial flattened picklist
    Protected Function GetPicklistMoniker() As String
        Return Request(FIELD_PICKLIST_MONIKER)
    End Function

    Protected Sub SetPicklistMoniker(ByVal sMoniker As String)
        Request.Cookies.Set(New HttpCookie(FIELD_PICKLIST_MONIKER, sMoniker))
    End Sub


    ' Moniker of the picklist to refine
    Protected Function GetRefineMoniker() As String
        Return Request(FIELD_REFINE_MONIKER)
    End Function

    Private Sub SetRefineMoniker(ByVal sMoniker As String)
        Request.Cookies.Set(New HttpCookie(FIELD_REFINE_MONIKER, sMoniker))
    End Sub
#End Region

    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
        With wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = "FlatAddress.aspx"
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "FlatAddress.aspx"
        End With
    End Sub

    Protected Function GetPageText(ByVal sKey As String) As String
        Dim strString As String = wfr.Content(sKey, _languageCode, True)
        Return strString.Trim
    End Function

    Protected Function GetNotFoundID() As String
        Dim strString As String = String.Empty
        strString = NotFound.ID
        Return strString
    End Function

    'Protected Sub SetCountryCode()

    '    Dim countries As New TalentApplicationVariablesTableAdapters.tbl_countryTableAdapter
    '    Dim dtCountries As Data.DataTable = countries.GetAll
    '    Dim dr As Data.DataRow = Nothing
    '    Dim sCountryName As String = GetCountryName().ToString.Trim
    '    If dtCountries.Rows.Count > 0 Then
    '        For Each dr In dtCountries.Rows
    '            If dr("COUNTRY_DESCRIPTION").ToString.Trim = sCountryName Then
    '                CountryCode.Value = dr("COUNTRY_CODE").ToString.Trim
    '            End If
    '        Next
    '    End If

    'End Sub
    Protected Function GetCountryCode() As String

        Dim countries As New TalentApplicationVariablesTableAdapters.tbl_countryTableAdapter
        Dim dtCountries As Data.DataTable = countries.GetAll
        Dim dr As Data.DataRow = Nothing
        Dim sCountryName As String = GetCountryName().ToString.Trim
        If dtCountries.Rows.Count > 0 Then
            For Each dr In dtCountries.Rows
                If dr("COUNTRY_DESCRIPTION").ToString.Trim = sCountryName Then
                    Return dr("COUNTRY_CODE").ToString.Trim
                End If
            Next
        End If
        Return ""
    End Function

    Protected Sub CreateQASJavascript()

        Dim i As Integer = m_asAddressLines.Length
        Dim defaults As ECommerceModuleDefaults.DefaultValues
        Dim defs As New ECommerceModuleDefaults
        Dim sString As String = String.Empty
        defaults = defs.GetDefaults
        Dim sAllFields() As String = defaults.AddressingFields.ToString.Split(",")
        Dim count As Integer = 0

        Response.Write(vbCrLf & "<script  type=""text/javascript"">")
        Response.Write(vbCrLf & "function init() {" & vbCrLf & "}")
        Response.Write(vbCrLf & "function DoOpener() {" & vbCrLf & "var NotFound = document.forms[0].ctl00$ContentPlaceHolder1$NotFound.value;")
        Response.Write(vbCrLf & "var AdrLine0 = '', AdrLine1 = '', AdrLine2 = '', AdrLine3 = '', AdrLine4 = '', AdrLine5= '';")
        Response.Write(vbCrLf & "var AdrLine6 = '',  AdrLine7 = '',  AdrLine8 = '',  AdrLine9 = '',  AdrLine10 = '',  AdrLine11= '';")
        Response.Write(vbCrLf & "var AdrLine12 = '';")
        count = 0
        ' BF- as AdrLine fields above 5 don't exist, the javascript drops out (without an error message)
        ' and the page posts back leaving the window
        ' Do While count < sAllFields.Length
        Do While count < m_asAddressLines.Length
            If sAllFields(count).ToString.Trim = defaults.AddressingMapCountry.ToString.Trim Then
                Response.Write(vbCrLf & " AdrLine" & count.ToString & " = document.forms[0].CountryCode.value;")
            Else
                Response.Write(vbCrLf & " AdrLine" & count.ToString & " = document.forms[0].AdrLine" & count.ToString & ".value;")
            End If
            count = count + 1
        Loop
        Response.Write(vbCrLf & "if (window.opener && !window.opener.closed) {")
        count = 0
        Do While count < sAllFields.Length
            Response.Write(vbCrLf & "window.opener.document.forms[0].hiddenAdr" & count.ToString & ".value = AdrLine" & count.ToString & ";")
            count = count + 1
        Loop
        '   Response.Write("window.alert(AdrLine1 + AdrLine2 + AdrLine3);")
        Response.Write(vbCrLf & "window.opener.UpdateAddressFields();")
        Response.Write(vbCrLf & "}" & vbCrLf & "window.close();" & vbCrLf & "}" & vbCrLf)
        '  Response.Write(vbCrLf & "}" & vbCrLf & ";" & vbCrLf & "}" & vbCrLf)
        Response.Write("DoOpener();" & vbCrLf)
        Response.Write("</script>")

    End Sub
End Class






