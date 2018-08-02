Imports Microsoft.VisualBasic
Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Pages Public - Flat Country
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

Partial Class PagesPublic_FlatSearch
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
    Protected FIELD_MUST_REFINE As String = "MustRefine"
    Protected m_Picklist As Picklist
    Protected m_PicklistOnDemand As com.qas.prowebondemand.Picklist


    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then
            Select Case ModuleDefaults.AddressingProvider
                Case Is = "QAS"
                    plcQAS.Visible = True
                    plcQASOnDemand.Visible = False
                Case Is = "QASONDEMAND"
                    plcQAS.Visible = False
                    plcQASOnDemand.Visible = True
            End Select
            DoSearch()
        End If
        'Else leave it to the event handlers (Next, Back)
    End Sub



    '** Search operations **


    ' Perform search using DataID and InputLines
    Private Sub DoSearch()

        Dim eRoute As com.qas.prowebintegration.Constants.Routes = com.qas.prowebintegration.Constants.Routes.Undefined

        ' If the picklist moniker is not present then we haven't been here before, and can auto-skip forwards
        Dim bAutoSkip As Boolean = (GetPicklistMoniker() Is Nothing)
        Dim sServerURL As String = System.Configuration.ConfigurationManager.AppSettings(com.qas.prowebintegration.Constants.KEY_SERVER_URL)
        Dim sUsername As String = System.Configuration.ConfigurationManager.AppSettings(com.qas.prowebintegration.Constants.KEY_USER_NAME)
        Dim sPassword As String = System.Configuration.ConfigurationManager.AppSettings(com.qas.prowebintegration.Constants.KEY_PASSWORD)

        Try

            Select Case ModuleDefaults.AddressingProvider
                Case Is = "QAS"
                    Dim searchService As QuickAddress = NewQuickAddress()
                    searchService.Engine = QuickAddress.EngineTypes.SingleLine
                    searchService.Flatten = True

                    If bAutoSkip Then
                        m_Picklist = searchService.Search(GetDataID(), GetInputLines(), GetPromptSet()).Picklist
                        SetPicklistMoniker(m_Picklist.Moniker)
                    Else
                        ' Recreate picklist from moniker
                        m_Picklist = searchService.StepIn(GetPicklistMoniker())
                    End If

                    'Handle 'failure' cases
                    If m_Picklist.IsTimeout Then
                        eRoute = com.qas.prowebintegration.Constants.Routes.Timeout
                    ElseIf m_Picklist.IsMaxMatches Then
                        eRoute = com.qas.prowebintegration.Constants.Routes.TooManyMatches
                    ElseIf m_Picklist.Items Is Nothing Then
                        eRoute = com.qas.prowebintegration.Constants.Routes.NoMatches
                    End If
                Case Is = "QASONDEMAND"
                    Dim searchService As com.qas.prowebondemand.QuickAddress = New com.qas.prowebondemand.QuickAddress(sServerURL, sUsername, sPassword)
                    searchService.Engine = com.qas.prowebondemand.QuickAddress.EngineTypes.Singleline
                    searchService.Flatten = True

                    If bAutoSkip Then
                        m_PicklistOnDemand = searchService.Search(GetDataID(), GetInputLines(), GetPromptSet()).Picklist
                    Else
                        ' Recreate picklist from moniker
                        m_PicklistOnDemand = searchService.StepIn(GetPicklistMoniker())
                    End If



            End Select

        Catch x As Exception
            GoErrorPage(x)
        End Try


        If Not eRoute.Equals(com.qas.prowebintegration.Constants.Routes.Undefined) Then
            GoErrorPage(eRoute)
        ElseIf bAutoSkip Then
            Select Case ModuleDefaults.AddressingProvider
                Case Is = "QAS"

                    If m_Picklist.IsAutoFormatSafe Or m_Picklist.IsAutoFormatPastClose Then
                        'Auto-step past picklist to format page
                        GoFormatPage(m_Picklist.Items(0).Moniker)
                    ElseIf m_Picklist.Length = 1 And MustRefine(m_Picklist.Items(0)) Then
                        'Auto-step past picklist to refinement page
                        GoRefinementPage(m_Picklist.Items(0).Moniker)
                    End If

                Case Is = "QASONDEMAND"

                    If m_PicklistOnDemand.IsAutoFormatSafe Or m_PicklistOnDemand.IsAutoFormatPastClose Then
                        'Auto-step past picklist to format page
                        GoFormatPage(m_PicklistOnDemand.Items(0).Moniker)
                    ElseIf m_PicklistOnDemand.Length = 1 And MustRefine(m_PicklistOnDemand.Items(0)) Then
                        'Auto-step past picklist to refinement page
                        GoRefinementPage(m_PicklistOnDemand.Items(0).Moniker)
                    End If
            End Select

        End If

        'Else let page render the picklist itself


    End Sub



    ' Helper function: must the picklist item be refined (extra text entered)
    Protected Function MustRefine(ByVal item As PicklistItem) As Boolean

        Return (item.IsIncompleteAddress Or item.IsUnresolvableRange Or item.IsPhantomPrimaryPoint)

    End Function
    ' Helper function: must the picklist item be refined (extra text entered)
    Protected Function MustRefine(ByVal item As com.qas.prowebondemand.PicklistItem) As Boolean

        Return (item.IsIncompleteAddress Or item.IsUnresolvableRange Or item.IsPhantomPrimaryPoint)

    End Function



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

    ' 'Back' button clicked: return to the search terms Input page
    'Private Sub ButtonBack_ServerClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonBack.ServerClick

    '    GoInputPage()

    'End Sub

    ' 'Next' button clicked: go forward to the refinement page if required, or format address page
    Private Sub ButtonNext1_ServerClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonNext1.ServerClick

        'Pick up hidden field, set by client JavaScript when they selected an item
        Dim bMustRefine As Boolean = Not (Request(FIELD_MUST_REFINE) = "")
        If bMustRefine Then
            GoRefinementPage(GetMoniker())
        Else
            GoFormatPage(Nothing)                'Moniker already set on page
        End If

    End Sub

    Protected Sub ButtonBack_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonBack.Click
        GoInputPage()
    End Sub

    Protected Sub ButtonNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonNext.Click
        DoNextButton()
    End Sub

    Protected Sub DoNextButton()
        'Pick up hidden field, set by client JavaScript when they selected an item
        Dim bMustRefine As Boolean = Not (Request(FIELD_MUST_REFINE) = "")
        If bMustRefine Then
            GoRefinementPage(GetMoniker())
        Else
            GoFormatPage(Nothing)                'Moniker already set on page
        End If
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
            .PageCode = "FlatSearch.aspx"
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "FlatSearch.aspx"
        End With
    End Sub

    Protected Function GetPageText(ByVal sKey As String) As String
        Dim strString As String = String.Empty
        strString = wfr.Content(sKey, _languageCode, True)
        Return strString.Trim
    End Function

    Protected Function GetUniqID() As String
        Return ButtonNext1.Name
    End Function
End Class






