Imports System
Imports System.Data
Imports System.Xml
Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web.Script.Serialization
Imports TalentBusinessLogic.Models
Imports TalentBusinessLogic.ModelBuilders
Imports System.Exception

Partial Class PagesPublic_Profile_CustomerSelection
    Inherits TalentBase01

#Region "Constant Variables"

    Const QUERYSTRING_SUBMIT = "SUBMIT"
    Const QUERYSTRING_SEARCH = "SEARCH"

#End Region

#Region "Class Level Fields"
    'Private _wfr As New Talent.Common.WebFormResource
    Private _customerSettings As New DECustomer
    Private _errorObj As New ErrorObj
    Private _useClubDDL As Boolean = True
    Private _displayRestrictionText As Boolean = False
    Private _restrictionText As String = String.Empty
    Private _customerSearchCustomerNumber As String = String.Empty
    Private _selectedCustomer As String = String.Empty
    Private _encodedSessionID As String = String.Empty
    Private _encodedCustomerNumber As String = String.Empty
    Private _encodedRandsomString As String = String.Empty
    Private _encodedSiteHomePageString As String = String.Empty
    Private _customerSearchViewModel As CustomerSearchViewModel
#End Region

#Region "Public Properties"
    Public CustomerNumberHeader As String = String.Empty
    Public CustomerNameHeader As String = String.Empty
    Public AddressHeader As String = String.Empty
    Public PostCodeHeader As String = String.Empty
    Public DateOfBirthHeader As String = String.Empty
    Public IDHeader As String = String.Empty
    Public EMailAddressHeader As String = String.Empty
    Public UpdateHeader As String = String.Empty
    Public UpdateLinkHeader As String = String.Empty
    Public UpdateLinkItem As String = String.Empty
    Public showUpdateCol As Boolean = False
    Public ProgressLabel As String = String.Empty
    Public PrintAddressLabelHeader As String = String.Empty
    Public MembershipHeader As String = String.Empty
    Public PassportNumber As String = String.Empty
    Public CompanyNameHeader As String = String.Empty
    Public TelephoneHeader As String = String.Empty
    Public SelectHeader As String = String.Empty
    Public ContactsHeader As String = String.Empty
    Public PerformAgentWatchListCheck As String = String.Empty
    Public ClubBusinessUnit As String = String.Empty
    Public CustomerSelectionSearchHeader As String = String.Empty
    Public CustomerSearchTabTitle As String = String.Empty
    Public CompanySearchTabTitle As String = String.Empty
    Public pageSize As Integer
    Public CustomerSearchPageSize As Integer
    Public CustomerSearchPaging As String
    Public CustomerSearchChangePageSize As String
    Public CustomerSearchChangePageSizeSelection As String
    Public CustomerSearchLengthMenuText As String
    Public CustomerSearchNonSortableColumnArray As String
    Public CompanySearchPageSize As Integer
    Public CompanySearchPaging As String
    Public CompanySearchChangePageSize As String
    Public CompanySearchChangePageSizeSelection As String
    Public CompanySearchLengthMenuText As String
    Public CompanySearchNonSortableColumnArray As String

    ''' <summary>
    ''' Used client side (could be replace with JS)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property QueryStringValidation As Boolean
        Get
            If Request.QueryString("source") IsNot Nothing _
                                 AndAlso Request.QueryString("source") = "customerselect" _
                                 AndAlso Request.QueryString("basketId") IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
#End Region

#Region "Protected Page Events"
    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        SetupInputModel()
        UpdateHeader = _customerSearchViewModel.GetPageText("UpdateHeaderText")
        SelectHeader = _customerSearchViewModel.GetPageText("SelectHeaderText")
        TelephoneHeader = _customerSearchViewModel.GetPageText("TelephoneHeaderText")
        CompanyNameHeader = _customerSearchViewModel.GetPageText("CompanyNameHeaderText")
        CustomerNumberHeader = _customerSearchViewModel.GetPageText("CustomerNumberHeaderText")
        CustomerNameHeader = _customerSearchViewModel.GetPageText("CustomerNameHeaderText", )
        AddressHeader = _customerSearchViewModel.GetPageText("AddressHeaderText")
        PostCodeHeader = _customerSearchViewModel.GetPageText("PostCodeHeaderText")
        DateOfBirthHeader = _customerSearchViewModel.GetPageText("DateOfBirthHeaderText")
        IDHeader = _customerSearchViewModel.GetPageText("IDHeaderText")
        EMailAddressHeader = _customerSearchViewModel.GetPageText("EMailAddressHeaderText")
        UpdateLinkHeader = _customerSearchViewModel.GetPageText("UpdateLinkHeaderText")
        UpdateLinkItem = _customerSearchViewModel.GetPageText("UpdateLinkItemText")
        MembershipHeader = _customerSearchViewModel.GetPageText("MembershipNumberText")
        PassportNumber = _customerSearchViewModel.GetPageText("PassportNumberText")
        PrintAddressLabelHeader = _customerSearchViewModel.GetPageText("PrintAddressLabelHeaderText")
        ContactsHeader = _customerSearchViewModel.GetPageText("ContactLabelHeaderText")
        hdfPrintAddressLabelItem.Value = _customerSearchViewModel.GetPageText("PrintAddressLabelItemText")
        CustomerSearchTabTitle = _customerSearchViewModel.GetPageText("CustomerSearchTabTitleText")
        CompanySearchTabTitle = _customerSearchViewModel.GetPageText("CompanySearchTabTitleText")
        PerformAgentWatchListCheck = CType(ModuleDefaults.PerformAgentWatchListCheck, String)
        hdfCustomerSearchPageSize.Value = Talent.eCommerce.Utilities.CheckForDBNull_Int(_customerSearchViewModel.GetPageAttribute("CustomerSearchPageSize"))
        hdfCustomerSearchChangePageSize.Value = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageAttribute("CustomerSearchChangePageSize"))
        hdfCustomerSearchChangePageSizeSelection.Value = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageAttribute("CustomerSearchChangePageSizeSelection"))
        hdfCustomerSearchLengthMenuText.Value = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageText("CustomerSearchLengthMenuText"))
        hdfCustomerSearchNonSortableColumnArray.Value = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageAttribute("CustomerSearchNonSortableColumnArray"))
        hdfCompanySearchPageSize.Value = Talent.eCommerce.Utilities.CheckForDBNull_Int(_customerSearchViewModel.GetPageAttribute("CompanySearchPageSize"))
        hdfCompanySearchChangePageSize.Value = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageAttribute("CompanySearchChangePageSize"))
        hdfCompanySearchChangePageSizeSelection.Value = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageAttribute("CompanySearchChangePageSizeSelection"))
        hdfCompanySearchLengthMenuText.Value = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageText("CompanySearchLengthMenuText"))
        hdfCompanySearchNonSortableColumnArray.Value = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageAttribute("CompanySearchNonSortableColumnArray"))
        hdfCustomerSearchCustomerColumnVisibilityName.Value = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageAttribute("CustomerSearchResultsColumnVisibilityName"))
        hdfCustomerSearchCustomerColumnVisibilityAddress.Value = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageAttribute("CustomerSearchResultsColumnVisibilityAddress"))
        hdfCustomerSearchCustomerColumnVisibilityPostcode.Value = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageAttribute("CustomerSearchResultsColumnVisibilityPostcode"))
        hdfCustomerSearchCustomerColumnVisibilityPhoneNumber.Value = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageAttribute("CustomerSearchResultsColumnVisibilityPhoneNumber"))
        hdfCustomerSearchCustomerColumnVisibilityDOB.Value = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageAttribute("CustomerSearchResultsColumnVisibilityDOB"))
        hdfCustomerSearchCustomerColumnVisibilityMembershipNo.Value = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageAttribute("CustomerSearchResultsColumnVisibilityMembership"))
        hdfCustomerSearchCustomerColumnVisibilityPassport.Value = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageAttribute("CustomerSearchResultsColumnVisibilityPassport"))
        hdfCustomerSearchCustomerColumnVisibilityEmail.Value = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageAttribute("CustomerSearchResultsColumnVisibilityEmail"))
        hdfCustomerSearchLimit.Value = Talent.eCommerce.Utilities.CheckForDBNull_Int(_customerSearchViewModel.GetPageAttribute("SearchResultLimit"))
        CustomerSearchPageSize = Talent.eCommerce.Utilities.CheckForDBNull_Int(_customerSearchViewModel.GetPageAttribute("CustomerSearchPageSize"))
        CustomerSearchChangePageSize = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageAttribute("CustomerSearchChangePageSize"))
        CustomerSearchChangePageSizeSelection = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageAttribute("CustomerSearchChangePageSizeSelection"))
        CustomerSearchLengthMenuText = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageText("CustomerSearchLengthMenuText"))
        CustomerSearchNonSortableColumnArray = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageAttribute("CustomerSearchNonSortableColumnArray"))
        CompanySearchPageSize = Talent.eCommerce.Utilities.CheckForDBNull_Int(_customerSearchViewModel.GetPageAttribute("CompanySearchPageSize"))
        CompanySearchChangePageSize = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageAttribute("CompanySearchChangePageSize"))
        CompanySearchChangePageSizeSelection = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageAttribute("CompanySearchChangePageSizeSelection"))
        CompanySearchLengthMenuText = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageText("CompanySearchLengthMenuText"))
        CompanySearchNonSortableColumnArray = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageAttribute("CompanySearchNonSortableColumnArray"))
        hdfRootURL.Value = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/")
        hdfTalentAPIAddress.Value = ModuleDefaults.TalentAPIAddress
        hdfChildCompanyNumber.Value = Request.QueryString("childCompanyNumber")
        hdfNoSearchCriteria.Value = Talent.eCommerce.Utilities.CheckForDBNull_String(_customerSearchViewModel.GetPageText("NoSearchCriteriaErrorMessage"))
        hdfSearchType.Value = "ALL"

        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "error-handling.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("error-handling.js", "/Application/Status/"), False)
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "customer-search.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("customer-search.js", "/Module/Customer/"), False)
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "company-search.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("company-search.js", "/Module/CRM/"), False)

        ' Determine what should be displayed based on the displayMode query string parameter

        If Request.QueryString("displayMode") IsNot Nothing Then
            plhPageHeader.Visible = True
            If Request.QueryString("displayMode") = "ShowCompanySearch" Then
                hdfShowCompanySearch.Value = "true"
            ElseIf Request.QueryString("displayMode") = "Parent" Then
                hdfShowCompanySearch.Value = "true"
                hdfShowParentCompanySearch.Value = "true"
                pnlSingleCustomerSelection.Visible = False
                uscCustomerSearch.Visible = False
                CustomerSelectionSearchHeader = _customerSearchViewModel.GetPageText("ParentSearchHeaderText")
                hdfSearchType.Value = "Parent"
            ElseIf Request.QueryString("displayMode") = "Subsidiaries" Then
                hdfShowSubsidaries.Value = "true"
                hdfShowCompanySearch.Value = "true"
                hdfParentCompanyNumber.Value = Request.QueryString("ParentCompanyNumber")
                pnlSingleCustomerSelection.Visible = False
                uscCustomerSearch.Visible = False
                hdfSearchType.Value = "Subsidiaries"
                CustomerSelectionSearchHeader = _customerSearchViewModel.GetPageText("SubsidiariesSearchHeaderText")
            ElseIf Request.QueryString("displayMode") = "AddToNull" Then
                hdfShowCompanySearch.Value = "true"
                hdfShowParentCompanySearch.Value = "true"
                pnlSingleCustomerSelection.Visible = False
                uscCustomerSearch.Visible = False
                hdfAddCompanyToNull.Value = "true"
                CustomerSelectionSearchHeader = _customerSearchViewModel.GetPageText("ParentSearchHeaderText")
            ElseIf Request.QueryString("displayMode") = "AddSubsidiaries" Then
                hdfShowCompanySearch.Value = "true"
                hdfShowParentCompanySearch.Value = "true"
                hdfParentCompanyNumber.Value = Request.QueryString("ParentCompanyNumber")
                hdfParentCompanyName.Value = Request.QueryString("ParentCompanyName")
                pnlSingleCustomerSelection.Visible = False
                uscCustomerSearch.Visible = False
                hdfAddSubsidiaries.Value = "true"
                CustomerSelectionSearchHeader = _customerSearchViewModel.GetPageText("SubsidiariesSearchHeaderText")
            ElseIf Request.QueryString("displayMode") = "Contacts" Then
                hdfShowCompanySearch.Value = "false"
                hdfShowParentCompanySearch.Value = "false"
                pnlSingleCustomerSelection.Visible = False
                uscCustomerSearch.Visible = True
                hdfSearchType.Value = "Contacts"
                hdfCompanyNumber.Value = Request.QueryString("CompanyNumber")
                CustomerSelectionSearchHeader = _customerSearchViewModel.GetPageText("SelectContactHeaderText")
            ElseIf Request.QueryString("displayMode") = "basket" Then
                setBasketHiddenFields()
            End If
        End If
        If Request.QueryString("companyColumnMode") IsNot Nothing AndAlso Request.QueryString("companyColumnMode") = "SelectOnly" Then
            hdfColumnDisplay.Value = 1
        Else
            If AgentProfile.AgentPermissions.CanMaintainCompany Then
                hdfColumnDisplay.Value = 2
            Else
                hdfColumnDisplay.Value = 3
            End If
        End If

        If Request.QueryString("returnTo") IsNot Nothing AndAlso Not String.IsNullOrEmpty(Request.QueryString("returnTo")) Then
            hdfReturnToAddress.Value = Request.QueryString("returnTo")
        End If

        hdfSessionID.Value = Session.SessionID
        If AgentProfile.IsAgent Then
            hdfAgentType.Value = AgentProfile.Type
            hdfAgentLoginID.Value = AgentProfile.Name
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Check to see if the there is a query string to (a) get to a customer restricted page and (b) the customer is logged in
        'Return the customer to that page if they are logged in otherwise display a message asking them to login
        If Request.QueryString("RequiresLogin") IsNot Nothing AndAlso Request.QueryString("RequiresLogin").ToUpper() = "TRUE" Then
            If Profile.IsAnonymous Then
                errorList.Items.Add(_customerSearchViewModel.GetPageText("RequiresLoginMessage"))
            Else
                If Request.QueryString("ReturnUrl") IsNot Nothing Then
                    Response.Redirect(Request.QueryString("ReturnUrl"))
                End If
            End If
        End If

        If Not AgentProfile.IsAgent Then
            Response.Redirect("~/PagesPublic/Agent/AgentLogin.aspx")
        End If

        If Not Page.IsPostBack Then
            plhMembershipNumber.Visible = Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultFalse(_customerSearchViewModel.GetPageAttribute("Show_MembershipNumber"))
            plhCorporateSaleID.Visible = CheckForDBNullOrBlank_Boolean_DefaultFalse(_customerSearchViewModel.GetPageAttribute("ShowCorporateSaleIDSearch"))
            PopulateTexts()
            If _useClubDDL Then
                PopulateClubDLL()
            Else
                plhClubDDL.Visible = False
            End If
        End If
        PopulateFromQueryString()
        If CheckForDBNull_String(_customerSearchViewModel.GetPageText("FanCardBoxAutoFocusScript")).Trim.Length > 0 Then
            Dim scriptString As String = CheckForDBNull_String(_customerSearchViewModel.GetPageText("FanCardBoxAutoFocusScript")).Trim
            scriptString = scriptString.Replace("<<<TEXTBOX_ID>>>", fancardBox.ClientID)
            Page.ClientScript.RegisterStartupScript(Me.GetType(), "AutoFocusScript", scriptString)
        End If
        'SHOW LAST "N" CUSTOMERS USED 
        DisplayLastNCustomersUsed()
    End Sub
    Protected Sub Page_PreRender1(sender As Object, e As EventArgs) Handles Me.PreRender
        Dim combinedErrors As BulletedList = errorList
        If uscCustomerSearch IsNot Nothing AndAlso uscCustomerSearch.ErrorList IsNot Nothing AndAlso uscCustomerSearch.ErrorList.Items.Count > 0 Then
            Dim customerSearchErrors As ListItemCollection = uscCustomerSearch.ErrorList.Items
            combinedErrors.Items.AddRange(customerSearchErrors.Cast(Of ListItem)().ToArray())
        End If

        If uscCompanySearch IsNot Nothing AndAlso uscCompanySearch.ErrorList IsNot Nothing AndAlso uscCompanySearch.ErrorList.Items.Count > 0 Then
            Dim companySearchErrors As ListItemCollection = uscCompanySearch.ErrorList.Items
            combinedErrors.Items.AddRange(companySearchErrors.Cast(Of ListItem)().ToArray())
        End If

        If combinedErrors.Items.Count > 0 Then
            plhErrorList.Visible = True
            errorList = combinedErrors
        End If
    End Sub
#End Region

#Region "Protected control events"
    Protected Sub rptLastNCustomerLogins_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptLastNCustomerLogins.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim navigateURL As New StringBuilder
            Dim customerNo As String = String.Empty
            Dim hplLastCustomerLogin As HyperLink = CType(e.Item.FindControl("hplLastCustomerLogin"), HyperLink)
            Dim randStr As String
            randStr = Talent.Common.Utilities.TripleDESEncode(Talent.Common.Utilities.RandomString(10), ModuleDefaults.NOISE_ENCRYPTION_KEY)
            customerNo = Talent.Common.Utilities.TripleDESEncode(e.Item.DataItem(0).ToString(), ModuleDefaults.NOISE_ENCRYPTION_KEY)

            navigateURL.Append("~/redirect/ticketinggateway.aspx?page=validatesession.aspx&function=validatesession")
            navigateURL.Append("&t=1")
            navigateURL.Append("&l=").Append(Server.UrlEncode(customerNo))
            navigateURL.Append("&p=").Append(Server.UrlEncode(randStr))
            navigateURL.Append("&y=N")
            navigateURL.Append("&ReturnUrl=").Append(Server.UrlEncode(Request.Url.AbsoluteUri))

            If e.Item.FindControl("hplLastCustomerLogin") IsNot Nothing Then
                hplLastCustomerLogin.Text = e.Item.DataItem(1).ToString()
                hplLastCustomerLogin.NavigateUrl = navigateURL.ToString()
            End If
        End If
    End Sub
    Protected Sub btnSelectCustomer_Click(sender As Object, e As EventArgs) Handles btnSelectCustomer.Click
        SubmitSearch()
    End Sub
#End Region

#Region "Protected methods"
    Protected Function ShowMembershipLabel(ByVal membershipNumber As String) As Boolean
        If membershipNumber.Length > 0 Then
            If membershipNumber.Equals("MANY") Then
                Return False
            Else
                Return True
            End If
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Set visibility of columns based on page attribute
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub CanDisplayThisColumn(ByVal sender As Object, ByVal e As EventArgs)
        Select Case sender.id
            Case "plhMembership"
                sender.Visible = CheckForDBNullOrBlank_Boolean_DefaultFalse(_customerSearchViewModel.GetPageAttribute("ShowMembershipColumn"))
        End Select
    End Sub
#End Region

#Region "Private Methods"
    Private Sub PopulateFromQueryString()
        If Request.QueryString("Type") IsNot Nothing AndAlso Not String.IsNullOrEmpty(Request.QueryString("Type")) Then
            If String.IsNullOrEmpty(fancardBox.Text) AndAlso String.IsNullOrEmpty(txtMembershipNumber.Text) Then
                fancardBox.Text = Request.QueryString("FancardBox")
                txtMembershipNumber.Text = Request.QueryString("MembershipNumber")
                clubDDL.SelectedValue = "BOXOFFICE"
                If Request.QueryString("Type") = "Submit" Then
                    SubmitSearch()
                End If
            End If
        End If
    End Sub

    Private Sub SubmitSearch()
        errorList.Items.Clear()
        ClearDeliveryAddressCntrlSessions()

        Dim validationError As Boolean = False

        Dim entryCount As Integer
        If Not String.IsNullOrEmpty(fancardBox.Text.Trim) Then entryCount += 1
        If Not String.IsNullOrEmpty(txtMembershipNumber.Text.Trim) Then entryCount += 1
        If Not String.IsNullOrEmpty(txtPaymentReference.Text.Trim) Then entryCount += 1
        If Not String.IsNullOrEmpty(txtCorporateSaleID.Text.Trim) Then entryCount += 1

        If entryCount = 0 Then
            errorList.Items.Add(_customerSearchViewModel.GetPageText("ErrorText_NoCustomerDataEntered"))
            validationError = True
        ElseIf entryCount > 1 Then
            errorList.Items.Add(_customerSearchViewModel.GetPageText("ErrorText_MoreThan1Entry"))
            validationError = True
        End If


        'validation for club dropdown
        If clubDDL.SelectedValue.Trim().Length <= 0 Then
            errorList.Items.Add(_customerSearchViewModel.GetPageText("ErrorText_NoClubSelection"))
            validationError = True
        End If

        If Not validationError AndAlso Request.QueryString("source") IsNot Nothing Then
            If (Request.QueryString("source") = "customerselect" AndAlso
                Request.QueryString("basketId") IsNot Nothing) _
                OrElse Request.QueryString("source") = "customerselecthospitalitybooking" Then
                ProcessSelectedCustomer(validationError)
            End If
        End If

        If Not validationError Then
            ProcessCustomerRedirection(validationError)
        End If
    End Sub
    ''' <summary>
    ''' Sets up input model to return text and attribute for page
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub SetupInputModel()
        _customerSearchViewModel = New CustomerSearchViewModel(True)
    End Sub

    ''' <summary>
    ''' Display Last N customers used to navigate for reuse
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DisplayLastNCustomersUsed()

        Dim LastNCustomersUsed As New List(Of String())
        Dim SortedLastNCustomersUsed As New List(Of String())
        LastNCustomersUsed = CType(Session("LastNCustomerLogins"), List(Of String()))
        If Not IsNothing(Session("LastNCustomerLogins")) AndAlso LastNCustomersUsed.Count > 0 Then
            For Each item In LastNCustomersUsed
                SortedLastNCustomersUsed.Add(item.Clone())
            Next
            SortedLastNCustomersUsed.Reverse()
            rptLastNCustomerLogins.DataSource = SortedLastNCustomersUsed
            rptLastNCustomerLogins.DataBind()
        Else
            plhLastNCustomerLogins.Visible = False
        End If
    End Sub
    Private Sub PopulateClubDLL()
        plhClubDDL.Visible = True
        Dim dt As Data.DataTable = Talent.eCommerce.Utilities.GetClubs
        If dt.Rows.Count > 0 Then
            clubDDL.DataSource = dt
            clubDDL.DataTextField = "CLUB_DESCRIPTION"
            clubDDL.DataValueField = "CLUB_CODE"
            clubDDL.DataBind()

            clubDDL.Items.Add(New ListItem(_customerSearchViewModel.GetPageText("SelectListItemText"), ""))
            If Not String.IsNullOrEmpty(Session.Item("AgentPortalDefaultClub")) Then
                clubDDL.SelectedValue = Session.Item("AgentPortalDefaultClub")

            ElseIf Not String.IsNullOrEmpty(AgentProfile.Name) Then
                For Each li As ListItem In clubDDL.Items

                    If li.Value.ToLower = AgentProfile.Name.ToLower Then
                        clubDDL.SelectedValue = li.Value
                        Exit For
                    End If
                Next
            Else
                Dim drc As Data.DataRow() = dt.Select("IS_DEFAULT = 'True'")
                If drc.Length > 0 Then
                    clubDDL.SelectedValue = drc(0)("CLUB_CODE")
                Else
                    clubDDL.SelectedValue = ""
                End If
            End If

            If (AgentProfile.Type.Equals("1")) Then
                plhClubDDL.Visible = False
            ElseIf (clubDDL.Items.Count <= 2) AndAlso (clubDDL.SelectedValue <> "") Then
                plhClubDDL.Visible = False
            End If
            ClubBusinessUnit = clubDDL.SelectedValue
        Else
            plhClubDDL.Visible = False
        End If

    End Sub
    Private Sub PopulateTexts()
        clubLabel.Text = _customerSearchViewModel.GetPageText("ClubSelectionLabelText")
        fancardLabel.Text = _customerSearchViewModel.GetPageText("FancardLabelText")
        btnSelectCustomer.Text = _customerSearchViewModel.GetPageText("SubmitButtonText")
        lblMembershipNumber.Text = _customerSearchViewModel.GetPageText("MembershipNumberText")
        lblPaymentReference.Text = _customerSearchViewModel.GetPageText("PaymentReferenceText")
        lblCorporateSaleID.Text = _customerSearchViewModel.GetPageText("CorporateSaleIDText")
        fancardBox.Attributes.Add("placeholder", fancardLabel.Text)
        txtMembershipNumber.Attributes.Add("placeholder", lblMembershipNumber.Text)
        txtPaymentReference.Attributes.Add("placeholder", lblPaymentReference.Text)
        txtCorporateSaleID.Attributes.Add("placeholder", lblCorporateSaleID.Text)

        If Not IsNothing(Session("LastNCustomerLogins")) Then
            Dim lastNCustomerLoginsList As List(Of String()) = Session("LastNCustomerLogins")
            lblLastNCustomersUsed.Text = String.Format(_customerSearchViewModel.GetPageText("lblLastNCustomersUsed"), lastNCustomerLoginsList.Count.ToString())
        End If
    End Sub

    ''' <summary>
    ''' Format the current URL if it is a localhost URL
    ''' </summary>
    ''' <param name="url">The given URL</param>
    ''' <returns>The formatted URL</returns>
    ''' <remarks></remarks>
    Private Function formatUrlForLocalHost(ByVal url As String) As String
        Dim formattedUrl As String = url
        If Request.Url.AbsoluteUri.Contains("localhost") AndAlso url.ToUpper().Contains("TALENTEBUSINESS") AndAlso url.ToUpper().IndexOf("/TALENTEBUSINESS") > 0 Then
            formattedUrl = url.Substring(url.ToUpper().IndexOf("/TALENTEBUSINESS"), url.Length - url.ToUpper().IndexOf("/TALENTEBUSINESS"))
            Dim port As String = Request.Url.Port
        Else
            formattedUrl = "~" & formattedUrl
        End If
        Return formattedUrl
    End Function

    Private Sub ProcessCustomerRedirection(ByVal validationError As Boolean)
        Dim validCustUrl As String = String.Empty
        Dim invalidCustUrl As String = String.Empty
        Dim standardCustForwardUrl As String = String.Empty
        Dim noiseEncKey As String = String.Empty
        Dim clubs As Data.DataTable = Talent.eCommerce.Utilities.GetClubs
        If clubs.Rows.Count > 0 Then
            Dim drc As Data.DataRow() = clubs.Select("CLUB_CODE = '" & clubDDL.SelectedValue & "'")
            If drc.Length > 0 Then
                Dim club As Data.DataRow = drc(0)
                If Not String.IsNullOrEmpty(CheckForDBNull_String(club("VALID_CUSTOMER_FORWARD_URL"))) AndAlso
                    Not String.IsNullOrEmpty(CheckForDBNull_String(club("INVALID_CUSTOMER_FORWARD_URL"))) Then

                    validCustUrl = Convert.ToString(club("VALID_CUSTOMER_FORWARD_URL"))
                    invalidCustUrl = Convert.ToString(club("INVALID_CUSTOMER_FORWARD_URL"))
                    If ModuleDefaults.CustomerSearchMode = 1 And AgentProfile.Type.Equals("2") Then
                        standardCustForwardUrl = Convert.ToString(CheckForDBNull_String(club("STANDARD_CUSTOMER_FORWARD_URL"))).ToLower()
                        If standardCustForwardUrl.Length > 0 AndAlso standardCustForwardUrl.IndexOf("&returnurl") > 0 Then
                            standardCustForwardUrl = standardCustForwardUrl.Substring(standardCustForwardUrl.IndexOf("&returnurl"), standardCustForwardUrl.Length - standardCustForwardUrl.IndexOf("&returnurl"))
                        End If
                    End If
                    noiseEncKey = Convert.ToString(club("NOISE_ENCRYPTION_KEY"))

                    Dim customer As VerifyAndRetrieveCustomerViewModel = GetAndValidateCustomer()
                    SetupInputModel()

                    Dim sessionID As String = Talent.Common.Utilities.TripleDESEncode(Now.ToString("dd/MM/yyyy HH:mm:ss") &
                        Talent.Common.Utilities.FixStringLength(AgentProfile.Name, 10) &
                        Talent.Common.Utilities.FixStringLength(AgentProfile.Type, 10), noiseEncKey)

                    If customer.Valid Then
                        '--------------------------------------------------------
                        ' If customer is web ready then redirect to UpdateDetails
                        ' otherwise error.
                        ' If in agent mode doesn't need to be web ready
                        '--------------------------------------------------------
                        If Not String.IsNullOrEmpty(customer.CompanyNumber) Then
                            Session("LoggedInCompanyNumber") = customer.CompanyNumber
                        End If

                        If Not String.IsNullOrEmpty(customer.CustomerNumber) Then
                            If customer.WebReady = "1" OrElse customer.WebReady = "Y" OrElse Talent.eCommerce.Utilities.IsAgent Then
                                Dim randStr As String = Talent.Common.Utilities.TripleDESEncode(Talent.Common.Utilities.RandomString(10), noiseEncKey)
                                customer.CustomerNumber = Talent.Common.Utilities.TripleDESEncode(customer.CustomerNumber, noiseEncKey)
                                Dim navigateURL As New StringBuilder

                                navigateURL.Append("/redirect/ticketinggateway.aspx?page=validatesession.aspx&function=validatesession")
                                If Not Request.QueryString("posip") Is Nothing Then navigateURL.Append("&posip=").Append(Request.QueryString("posip"))
                                If Not Request.QueryString("posport") Is Nothing Then navigateURL.Append("&posport=").Append(Request.QueryString("posport"))
                                navigateURL.Append("&t=1")
                                navigateURL.Append("&l=").Append(Server.UrlEncode(customer.CustomerNumber))
                                navigateURL.Append("&p=").Append(Server.UrlEncode(randStr))
                                navigateURL.Append("&y=N")
                                If Request.QueryString("returnurl") Is Nothing Then
                                    navigateURL.Append(standardCustForwardUrl)
                                Else
                                    'Remove previous logon querystrings before added for the new customer.
                                    Dim returnUrl As String
                                    returnUrl = Request.QueryString("returnurl")
                                    returnUrl = Talent.eCommerce.Utilities.RemoveQueryStringByKey(returnUrl, "t")
                                    returnUrl = Talent.eCommerce.Utilities.RemoveQueryStringByKey(returnUrl, "l")
                                    returnUrl = Talent.eCommerce.Utilities.RemoveQueryStringByKey(returnUrl, "p")
                                    returnUrl = Talent.eCommerce.Utilities.RemoveQueryStringByKey(returnUrl, "y")
                                    navigateURL.Append("&ReturnUrl=").Append(returnUrl)
                                End If


                                Me.ViewState.Remove("CustSelc")

                                ' If search on payref or Corporate ID go to purchase details 
                                If Trim(txtPaymentReference.Text) <> String.Empty OrElse Trim(txtCorporateSaleID.Text) <> String.Empty Then
                                    CType(Page, TalentBase01).doAutoLogin(True, "1", customer.CustomerNumber, " ", "N")
                                    Session("ProductDetailsPath") = "CustomerSelection"
                                    Dim purchDetailsUrl As String = "/PagesLogin/Orders/PurchaseDetails.aspx?payref=" + Trim(customer.PaymentReference)
                                    Response.Redirect(formatUrlForLocalHost(purchDetailsUrl))
                                End If


                                Response.Redirect(formatUrlForLocalHost(navigateURL.ToString()))

                            Else
                                errorList.Items.Add(_customerSearchViewModel.GetPageText("ErrorText_CustomerIsNotWebReady"))
                                validationError = True
                            End If

                        End If
                    Else
                        '------------------------------------------------
                        ' If invalid and a fancard was entered then error
                        ' otherwise redirect to Registration
                        '------------------------------------------------
                        If Not String.IsNullOrEmpty(fancardBox.Text) Then
                            errorList.Items.Add(_customerSearchViewModel.GetPageText("ErrorText_FancardDoesNotExist"))
                            validationError = True
                        ElseIf Not String.IsNullOrEmpty(txtMembershipNumber.Text) Then
                            errorList.Items.Add(_customerSearchViewModel.GetPageText("ErrorText_MembershipDoesNotExist"))
                            validationError = True
                        ElseIf Not String.IsNullOrEmpty(txtPaymentReference.Text) Then
                            errorList.Items.Add(_customerSearchViewModel.GetPageText("ErrorText_PayrefDoesNotExist"))
                            validationError = True
                        ElseIf Not String.IsNullOrEmpty(txtCorporateSaleID.Text) Then
                            errorList.Items.Add(_customerSearchViewModel.GetPageText("ErrorText_CorporateSaleDoesNotExist"))
                            validationError = True
                        End If
                    End If

                Else
                    errorList.Items.Add(_customerSearchViewModel.GetPageText("ErrorText_NoCustomerValidationUrl"))
                End If
            End If
        End If
    End Sub

    Private Sub setBasketHiddenFields()

        hdfSearchType.Value = "Basket"
        hdfBasketDetailID.Value = Request.QueryString("basketId")

        If Request.QueryString("source").ToString() = "customerselect" Then
            For Each item As DEBasketItem In Profile.Basket.BasketItems
                If item.Basket_Detail_ID = Request.QueryString("basketId").ToString() Then
                    hdfProductCode.Value = item.Product
                    hdfPackageId.Value = item.PACKAGE_ID
                    hdfProductSubType.Value = item.PRODUCT_SUB_TYPE
                    hdfProductType.Value = item.PRODUCT_TYPE
                    hdfPriceCode.Value = item.PRICE_CODE
                    hdfPriceBand.Value = item.PRICE_BAND
                    hdfSeat.Value = item.SEAT
                    hdfFulfilmentMethod.Value = item.CURR_FULFIL_SLCTN
                    hdfBulkSalesId.Value = item.BULK_SALES_ID
                    hdfOriginalUser.Value = item.LOGINID
                    If item.BULK_SALES_ID > 0 Then hdfBulkSalesQuantity.Value = item.Quantity
                    Exit For
                End If
            Next
        End If

        ' Hospitality Seat Details are not on the basket detail record, passewd in a query string instead.
        If Request.QueryString("source").ToString() = "customerselecthospitalitybooking" Then
            hdfProductType.Value = Request.QueryString("productType").ToString()
            hdfProductCode.Value = Request.QueryString("productCode").ToString()
            hdfProductSubType.Value = Request.QueryString("productSubType").ToString()
            hdfPackageId.Value = Request.QueryString("packageId").ToString()
            hdfPriceCode.Value = Request.QueryString("priceCode").ToString()
            hdfPriceBand.Value = Request.QueryString("priceBand").ToString()
            hdfSeat.Value = Request.QueryString("seat").ToString()
            hdfFulfilmentMethod.Value = Request.QueryString("fulfilmentMethod").ToString()
            hdfBulkSalesId.Value = Request.QueryString("bulkId").ToString()
            hdfBulkSalesQuantity.Value = Request.QueryString("bulkQuantity").ToString()
            hdfOriginalUser.Value = Request.QueryString("originalCustomer").ToString()
            hdfReturnURL.Value = Server.UrlEncode(Request.QueryString("ReturnUrl").ToString())
        End If
    End Sub
    Private Sub ProcessSelectedCustomer(ByVal validationError As Boolean)

        Dim customer As VerifyAndRetrieveCustomerViewModel = GetAndValidateCustomer()

        If customer.Valid Then
            If Not String.IsNullOrEmpty(customer.CompanyNumber) Then
                Session("LoggedInCompanyNumber") = customer.CompanyNumber
            End If
            If Not String.IsNullOrEmpty(customer.CustomerNumber) Then
                Dim basketId As String = hdfBasketDetailID.Value
                Dim productCode As String = hdfProductCode.Value
                Dim packageId As String = hdfPackageId.Value
                Dim productSubType As String = hdfProductSubType.Value
                Dim productType As String = hdfProductType.Value
                Dim priceCode As String = hdfPriceCode.Value
                Dim priceBand As String = hdfPriceBand.Value
                Dim fulfilmentMethod As String = hdfFulfilmentMethod.Value
                Dim seat As String = hdfSeat.Value
                Dim bulkSalesId As Integer = hdfBulkSalesId.Value
                Dim bulkSalesQuantity As String = hdfBulkSalesQuantity.Value
                Dim returnURL As String = hdfReturnURL.Value

                Dim redirectUrl As String
                redirectUrl = UpdateCustomerBasket(Session.SessionID, "", productType, productCode, productSubType, packageId, priceCode, priceBand, fulfilmentMethod, seat, bulkSalesId, bulkSalesQuantity, returnURL, customer.CustomerNumber, Profile.User.Details.LoginID)
                Response.Redirect(formatUrlForLocalHost(redirectUrl))
            End If
        End If
    End Sub
    ''' <summary>
    ''' Retrieves and validates customer for customer selection
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetAndValidateCustomer() As VerifyAndRetrieveCustomerViewModel
        Dim setting As New DESettings
        Dim inputModel As New VerifyAndRetrieveCustomerInputModel
        Dim builder As New VerifyAndRetrieveCustomerBuilder

        'inputModel.IsAgent = AgentProfile.IsAgent
        If (CheckForDBNull_String(AgentProfile.Type).Trim().Equals("2")) Or AgentProfile.IsAgent() Then
            inputModel.PerformWatchListCheck = ModuleDefaults.PerformAgentWatchListCheck
        Else
            inputModel.PerformWatchListCheck = False
        End If

        inputModel.BasketID = Profile.Basket.Basket_Header_ID
        inputModel.CustomerNumber = fancardBox.Text.Trim
        'inputModel.Source = "W"
        inputModel.MembershipNumber = txtMembershipNumber.Text.Trim.ToUpper
        inputModel.CorporateSaleID = txtCorporateSaleID.Text.Trim
        inputModel.PaymentReference = txtPaymentReference.Text.Trim

        Return builder.VerifyAndRetrieveCustomer(inputModel)
    End Function

    Public Shared Function ResolveSharedUrl(originalUrl As String) As String
        If originalUrl Is Nothing Then
            Return Nothing
        End If

        ' *** Absolute path - just return
        If originalUrl.IndexOf("://") <> -1 Then
            Return originalUrl
        End If

        ' *** Fix up image path for ~ root app dir directory
        If originalUrl.StartsWith("~") Then
            Dim newUrl As String = ""
            If HttpContext.Current IsNot Nothing Then ' AndAlso HttpContext.Current.Request.ApplicationPath.Contains("BoxOffice") Then
                newUrl = HttpContext.Current.Request.ApplicationPath + originalUrl.Substring(1).Replace("//", "/")
            Else
                ' *** Not context: assume current directory is the base directory
                Throw New ArgumentException("Invalid URL: Relative URL not allowed.")
            End If

            ' *** Just to be sure fix up any double slashes
            Return newUrl
        End If

        Return originalUrl
    End Function
#End Region

#Region "WebMethod Classes and Methods"
    <System.Web.Services.WebMethod()>
    Public Shared Sub PrintAddressLabel(ByVal customerNumber As String, ByVal SessionID As String)
        If CheckAgentLogin(SessionID) Then
            Dim TalentAddressing As New TalentAddressing
            Dim err As New ErrorObj
            TalentAddressing.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            TalentAddressing.De.CustomerNumber = Talent.Common.Utilities.PadLeadingZeros(customerNumber, 12)
            TalentAddressing.PrintAddressLabel()
        End If
    End Sub
    ''' <summary>
    ''' Ecodes parameters for client-side
    ''' </summary>
    ''' <param name="SessionID"></param>
    ''' <param name="CustomerNo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <System.Web.Services.WebMethod()>
    Public Shared Function GetEncodedQueryStringParameters(ByVal SessionID As String, ByVal CustomerNo As String, ByVal Update As Boolean, ByVal ReturnUrl As String) As String
        Dim customerForwardUrl As New StringBuilder
        If Not CheckAgentLogin(SessionID) Then
            Throw New System.Exception("Invalid session.")
        End If
        Dim clubsTable As Data.DataTable = Talent.eCommerce.Utilities.GetClubs()
        Dim clubs As Data.DataRow() = clubsTable.Select("CLUB_CODE = '" & TalentCache.GetBusinessUnit & "'")
        If clubs.Length > 0 Then
            Dim club As Data.DataRow = clubs(0)
            Dim RandomString = Talent.Common.Utilities.TripleDESEncode(Talent.Common.Utilities.RandomString(10), club("NOISE_ENCRYPTION_KEY"))
            CustomerNo = Talent.Common.Utilities.TripleDESEncode(CustomerNo, club("NOISE_ENCRYPTION_KEY"))
            SessionID = Talent.Common.Utilities.TripleDESEncode(SessionID, club("NOISE_ENCRYPTION_KEY"))
            If Update Then
                customerForwardUrl.Append(club("VALID_CUSTOMER_FORWARD_URL"))
            ElseIf Not String.IsNullOrEmpty(ReturnUrl) Then
                customerForwardUrl.Append("redirect/ticketinggateway.aspx?page=validatesession.aspx&function=validatesession")
                customerForwardUrl.Append("&ReturnUrl=").Append(ReturnUrl)
            Else
                customerForwardUrl.Append(club("STANDARD_CUSTOMER_FORWARD_URL"))
            End If

            customerForwardUrl.Append("&session_id=").Append(HttpUtility.UrlEncode(SessionID))
            customerForwardUrl.Append("&t=1")
            customerForwardUrl.Append("&l=").Append(HttpUtility.UrlEncode(CustomerNo))
            customerForwardUrl.Append("&p=").Append(HttpUtility.UrlEncode(RandomString))
            customerForwardUrl.Append("&y=N")
        End If
        Return customerForwardUrl.ToString
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function UpdateCustomerBasket(ByVal SessionID As String,
                                                ByVal BasketID As String,
                                                ByVal ProductType As String,
                                                ByVal ProductCode As String,
                                                ByVal ProductSubType As String,
                                                ByVal PackageId As String,
                                                ByVal PriceCode As String,
                                                ByVal PriceBand As String,
                                                ByVal FulfilmentMethod As String,
                                                ByVal Seat As String,
                                                ByVal BulkSalesId As String,
                                                ByVal BulkSalesQuantity As String,
                                                ByVal ReturnURL As String,
                                                ByVal NewUser As String,
                                                ByVal OriginalUser As String) As String
        If Not CheckAgentLogin(SessionID) Then
            Throw New System.Exception("Invalid session.")
        End If

        Dim talPackage As New TalentPackage
        talPackage.RemoveCustomerPackageSession(PackageId, ProductCode, 0)

        Dim redirectUrl As New StringBuilder
        redirectUrl.Append("/Redirect/TicketingGateway.aspx?page=basket.aspx&function=updatebasket")
        redirectUrl.Append("&product1=").Append(ProductCode)
        redirectUrl.Append("&customer1=").Append(NewUser)
        redirectUrl.Append("&concession1=").Append(PriceBand)
        redirectUrl.Append("&priceCode1=").Append(PriceCode)
        redirectUrl.Append("&originalCust1=").Append(OriginalUser)
        redirectUrl.Append("&productSubtype1=").Append(ProductSubType)
        redirectUrl.Append("&productType1=").Append(ProductType)
        redirectUrl.Append("&fulfilmentMethod1=").Append(FulfilmentMethod)
        If BulkSalesId > 0 Then
            redirectUrl.Append("&seat1=").Append(Seat.Trim())
        Else
            redirectUrl.Append("&seat1=").Append(Seat)
        End If
        redirectUrl.Append("&packageId1=").Append(PackageId)
        redirectUrl.Append("&customerSelection=").Append("Y")
        redirectUrl.Append("&bulkSalesID1=").Append(BulkSalesId)
        redirectUrl.Append("&bulkSalesQuantity1=").Append(BulkSalesQuantity)
        redirectUrl.Append("&returnurl=").Append(ReturnURL)
        Return redirectUrl.ToString
    End Function

    Private Shared Function RetrieveAgentDetailsFromSessionID(ByVal sessionID As String) As DataTable
        Dim dataObjects As New TalentDataObjects
        dataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        Return dataObjects.AgentSettings.TblAgent.RetrieveAgentDetailsFromSessionID(sessionID)
    End Function

    Private Shared Function CheckAgentLogin(ByVal sessionID As String) As Boolean
        Dim agentDetails As New DataTable()
        Dim retVal As Boolean = True
        agentDetails = RetrieveAgentDetailsFromSessionID(sessionID)
        If agentDetails.Rows.Count = 0 Then
            retVal = False
        Else
            retVal = True
        End If
        Return retVal
    End Function
#End Region


End Class

