Imports Microsoft.VisualBasic
Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Pages Public - Flat Prompt
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

Partial Class PagesPublic_FlatPrompt
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
    Protected m_atPromptLines As PromptLine()
    Protected m_atPromptLinesOnDemand As com.qas.prowebondemand.PromptLine()

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim asValues As String() = Request.Params.GetValues(com.qas.prowebintegration.Constants.FIELD_INPUT_LINES)

        If Not IsPostBack Then

            'Populate hidden field
            PagePromptSet = GetPromptSet()
            'Get the search prompts for the page
            Select Case ModuleDefaults.AddressingProvider
                Case Is = "QAS"
                    plcqas.visible = True
                    plcqasOnDemand.visible = False
                    RetrieveSearchPrompts()
                Case Is = "QASONDEMAND"
                    plcqas.visible = False
                    plcqasOnDemand.visible = True

                    RetrieveSearchPromptsOnDemand()

            End Select
        End If

        'Else leave it to the event handlers (Next, Back, Alternate Prompts)

    End Sub


    '** Operations **


    ' Check if searching is available for this country/layout then retrieve the appropriate prompts
    Sub RetrieveSearchPrompts()

        Dim bPreSearchFailed As Boolean = False

        Try

            Dim searchService As QuickAddress = NewQuickAddress()
            searchService.Engine = QuickAddress.EngineTypes.SingleLine

            'Retrieve layout from web.config
            Dim sLayout As String = System.Configuration.ConfigurationManager.AppSettings(com.qas.prowebintegration.Constants.KEY_LAYOUT)

            ' Layout now stored in module defaults
            Dim defaults As ECommerceModuleDefaults.DefaultValues
            Dim defs As New ECommerceModuleDefaults
            defaults = defs.GetDefaults
            sLayout = defaults.AddressingLayout.Trim


            'Is automatic address capture available for this country & layout
            If (IsAlternate() Or searchService.CanSearch(GetDataID(), sLayout).IsOk) Then

                m_atPromptLines = searchService.GetPromptSet(GetDataID(), PagePromptSet).Lines

            Else

                'QuickAddress is not available for this country
                bPreSearchFailed = True
            End If
            lnkAlternate.Visible = Not IsAlternate()

        Catch x As Exception

            GoErrorPage(x)

        End Try


        If (bPreSearchFailed) Then

            GoErrorPage(com.qas.prowebintegration.Constants.Routes.PreSearchFailed)

        Else

            HyperlinkAlternate.Visible = Not IsAlternate()
            'And let page render prompts

        End If

    End Sub

    Sub RetrieveSearchPromptsOnDemand()

        Dim bPreSearchFailed As Boolean = False
        Dim sServerURL As String = System.Configuration.ConfigurationManager.AppSettings(com.qas.prowebintegration.Constants.KEY_SERVER_URL)
        Dim sUsername As String = System.Configuration.ConfigurationManager.AppSettings(com.qas.prowebintegration.Constants.KEY_USER_NAME)
        Dim sPassword As String = System.Configuration.ConfigurationManager.AppSettings(com.qas.prowebintegration.Constants.KEY_PASSWORD)

        Try

            Dim searchService As com.qas.prowebondemand.QuickAddress = New com.qas.prowebondemand.QuickAddress(sServerURL, sUsername, sPassword)

            searchService.Engine = QuickAddress.EngineTypes.SingleLine

            'Retrieve layout from web.config
            Dim sLayout As String = System.Configuration.ConfigurationManager.AppSettings(com.qas.prowebintegration.Constants.KEY_LAYOUT)

            ' Layout now stored in module defaults
            Dim defaults As ECommerceModuleDefaults.DefaultValues
            Dim defs As New ECommerceModuleDefaults
            defaults = defs.GetDefaults
            sLayout = defaults.AddressingLayout.Trim

            'Is automatic address capture available for this country & layout
            If (IsAlternate() Or searchService.CanSearch(GetDataID(), sLayout).IsOk) Then
                m_atPromptLinesOnDemand = searchService.GetPromptSet(GetDataID(), PagePromptSet).Lines
            Else
                'QuickAddress is not available for this country
                bPreSearchFailed = True
            End If
            lnkAlternate.Visible = Not IsAlternate()
        Catch x As Exception
            GoErrorPage(x)
        End Try

        If (bPreSearchFailed) Then
            GoErrorPage(com.qas.prowebintegration.Constants.Routes.PreSearchFailed)
        Else
            HyperlinkAlternate.Visible = Not IsAlternate()
            'And let page render prompt
        End If

    End Sub


    '** Page events **


#Region "Web Form Designer generated code"

    Protected Overrides Sub OnInit(ByVal e As EventArgs)

        'CODEGEN: This call is required by the ASP.NET Web Form Designer.
        InitializeComponent()
        MyBase.OnInit(e)

    End Sub


    ' Required method for Designer support - do not modify
    ' the contents of this method with the code editor.
    Private Sub InitializeComponent()

    End Sub

#End Region


    ' Hyperlink for alternate prompt set clicked: update and redisplay
    Private Sub HyperlinkAlternate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HyperlinkAlternate.Click

        PagePromptSet = PromptSet.Types.Alternate
        RetrieveSearchPrompts()

    End Sub


    ''Back button clicked: either redisplay with standard prompt set, or go back to the first page
    'Private Sub ButtonBack_ServerClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonBack.ServerClick
    '    If IsAlternate() Then
    '        PagePromptSet = PromptSet.Types.Optimal
    '        RetrieveSearchPrompts()
    '    Else
    '        GoFirstPage()
    '    End If
    'End Sub

    ''Next button clicked: move on to the search page
    'Private Sub ButtonNext_ServerClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonNext.ServerClick
    '    SetPromptSet(PagePromptSet)
    '    GoSearchPage()
    'End Sub

    Protected Sub ButtonBack_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonBack.Click
        If IsAlternate() Then
            PagePromptSet = PromptSet.Types.Optimal
            RetrieveSearchPrompts()
        Else
            GoFirstPage()
        End If
    End Sub

    Protected Sub ButtonNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonNext.Click
        Dim asValues As String() = Request.Params.GetValues(com.qas.prowebintegration.Constants.FIELD_INPUT_LINES)
        '  Request.Params.Remove("InputLine")
        '    Request.Params.Item(0).Remove()

        SetPromptSet(PagePromptSet)
        GoSearchPage()
    End Sub

    '** Page properties **


    ' Get/set the value of the Prompt Set field on the form
    Private Property PagePromptSet() As PromptSet.Types

        Get
            Dim sValue As String = HiddenPromptSet.Value
            If Not sValue Is Nothing Then
                Return System.Enum.Parse(GetType(PromptSet.Types), sValue)
            Else
                Return PromptSet.Types.Optimal
            End If
        End Get

        Set(ByVal Value As PromptSet.Types)
            HiddenPromptSet.Value = Value.ToString()
        End Set

    End Property


    ' Are we currently using the standard or alternate prompt set?
    Private Function IsAlternate() As Boolean

        Return Not (PagePromptSet.Equals(PromptSet.Types.Optimal))

    End Function


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
        Dim asValues As String() = Request.Params.GetValues(com.qas.prowebintegration.Constants.FIELD_INPUT_LINES)
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
            ' Check for postcode 
            If Not Request("postcode") Is Nothing Then
                ReDim asBlank(1)
                asBlank(1) = Request("postcode")
            Else
                ReDim asBlank(0)
            End If

            Return asBlank
        End If
    End Function


    ' Current search state, how we arrived on the address format page (i.e. too many matches)
    Protected Function GetRoute() As com.qas.prowebintegration.Constants.Routes
        Dim sValue As String = Request(com.qas.prowebintegration.Constants.FIELD_ROUTE)
        If Not sValue Is Nothing Then
            Return System.Enum.Parse(GetType(com.qas.prowebintegration.Constants.Routes), sValue)
        Else
            Return com.qas.prowebintegration.Constants.Routes.Undefined
        End If
    End Function

    Private Sub SetRoute(ByVal eRoute As com.qas.prowebintegration.Constants.Routes)
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
            .PageCode = "FlatPrompt.aspx"
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "FlatPrompt.aspx"
        End With
    End Sub

    Protected Function GetPageText(ByVal sKey As String) As String
        Dim strString As String = wfr.Content(sKey, _languageCode, True)
        Return strString.Trim
    End Function

    Protected Sub lnkAlternate_Click(sender As Object, e As System.EventArgs) Handles lnkAlternate.Click
        PagePromptSet = PromptSet.Types.Alternate
        If ModuleDefaults.AddressingProvider = "QASONDEMAND" Then
            RetrieveSearchPromptsOnDemand()
        Else
            RetrieveSearchPrompts()
        End If

    End Sub
End Class






