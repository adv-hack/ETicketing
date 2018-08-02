Imports Talent.Common
Imports System.Data
Imports System.Collections.Generic
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports TCUtilities = Talent.Common.Utilities

Partial Class UserControls_AgentPersonalisation
    Inherits ControlBase

    Private _ucr As UserControlResource = Nothing
    Private _languageCode As String = Nothing
    Private Const RESTRICTEDPAGENAME As String = "CHECKOUTORDERCONFIRMATION.ASPX"
    Private _errMsg As TalentErrorMessages
    Private _overridePrinterHome As String
    Private _overridePrinterEvent As String
    Private _overridePrinterTravel As String
    Private _overridePrinterSTReceipts As String
    Private _overridePrinterAddress As String
    Private _printAddressLabelsDefault As Boolean
    Private _printTransactionReceiptDefault As Boolean
    Private _bulkSalesMode As Boolean
    Private _printAlways As Boolean
    Private _agentDepartment As String
    Private _corporateHospitalityMode As Boolean
    Private _smartcardPrinterDefault As String
    Private _ticketPrinterDefault As String
    Private _printerGroup As String
    Private _authorityGroups As String
    Private _captureMethod As String

#Region "Protected Methods"

    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        _ucr = New UserControlResource
        _languageCode = TEBUtilities.GetCurrentLanguage()
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ConnectionString
            .KeyCode = "AgentPersonalisation.ascx"
            .PageCode = TEBUtilities.GetCurrentPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
        End With

        SetPrinterSelectionWindow()
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
        If AgentProfile.IsAgent Then
            plhLoggedIn.Visible = True
            plhNotLoggedIn.Visible = False
            ltlLoggedInAgent.Text = _ucr.Content("AgentDetailsLabel", _languageCode, True)
            ltlLoggedInAgent.Text = ltlLoggedInAgent.Text.Replace("<<AgentName>>", AgentProfile.Name)
            plhCustomerDetails.Visible = Not Profile.IsAnonymous
            hplBasket.ToolTip = _ucr.Content("BasketLabel", _languageCode, True)

            ltlSelectPrinter.Text = _ucr.Content("ltlSelectPrinter", _languageCode, True)
            lblPrinterListLabel.Text = _ucr.Content("lblPrinterListLabel", _languageCode, True)
            btnOkPrinterSelection.Text = _ucr.Content("btnOkPrinterSelectionText", _languageCode, True)
            btnCancelPrinterSelection.Text = _ucr.Content("btnCancelPrinterSelection", _languageCode, True)
            plhErrorList.Visible = (blErrList.Items.Count > 0)

            If plhCustomerDetails.Visible Then
                ltlCustomerDetails.Text = _ucr.Content("AgentAndCustomerDetailsLabel", _languageCode, True)
                ltlStopCodeDetails.Text = _ucr.Content("StopCodeLabel", _languageCode, True)
                If ModuleDefaults.LoginidType.Equals("1") Then
                    If Profile.User.Details.LoginID IsNot Nothing Then ltlCustomerDetails.Text = ltlCustomerDetails.Text.Replace("<<UserName>>", Profile.User.Details.LoginID.TrimStart(GlobalConstants.LEADING_ZEROS))
                Else
                    If Profile.User.Details.LoginID IsNot Nothing Then ltlCustomerDetails.Text = ltlCustomerDetails.Text.Replace("<<UserName>>", Profile.User.Details.LoginID)
                End If
                ltlCustomerDetails.Text = ltlCustomerDetails.Text.Replace("<<FullName>>", Profile.User.Details.Full_Name)
                ltlCustomerDetails.Text = ltlCustomerDetails.Text.Replace("<<Forename>>", Profile.User.Details.Forename)
                ltlCustomerDetails.Text = ltlCustomerDetails.Text.Replace("<<Surname>>", Profile.User.Details.Surname)
                ltlCustomerDetails.Text = ltlCustomerDetails.Text.Replace("<<Salutation>>", Profile.User.Details.Salutation)

                If Profile.User.Details.STOP_CODE IsNot String.Empty And Profile.User.Details.STOP_CODE IsNot Nothing Then
                    If ltlStopCodeDetails.Text.Contains("<<StopCode>>") Then
                        ltlStopCodeDetails.Text = ltlStopCodeDetails.Text.Replace("<<StopCode>>", Profile.User.Details.STOP_CODE)
                    End If
                    If ltlStopCodeDetails.Text.Contains("<<StopCodeDescription>>") Then
                        Dim stopCodeDescription As New String(String.Empty)
                        If Profile.User.Details.STOP_CODE IsNot String.Empty Then
                            stopCodeDescription = RetrieveStopCodeDescription(Profile.User.Details.STOP_CODE)
                        End If
                        ltlStopCodeDetails.Text = ltlStopCodeDetails.Text.Replace("<<StopCodeDescription>>", stopCodeDescription)
                    End If
                Else
                    ltlStopCodeDetails.Visible = False
                End If
            End If

            Dim totalItems As Decimal = Profile.Basket.BasketSummary.MerchandiseTotalItems + Profile.Basket.BasketSummary.TotalItemsTicketing
            If Not _ucr.PageCode.ToUpper.Equals(RESTRICTEDPAGENAME) Then
                ltlBasketItems.Text = CType(totalItems, String)
            Else
                ltlBasketItems.Text = "0"
            End If

            If (Session("LoggedInCompanyName") IsNot Nothing AndAlso Not String.IsNullOrEmpty(Session("LoggedInCompanyName"))) _
                AndAlso (Session("LoggedInCompanyNumber") IsNot Nothing AndAlso Not String.IsNullOrEmpty(Session("LoggedInCompanyNumber"))) _
                    AndAlso AgentProfile.AgentPermissions.CanMaintainCompany Then
                hplUserCompany.Visible = True
                ltlCompanyName.Text = Session("LoggedInCompanyName")
                hplUserCompany.NavigateUrl = "~/PagesPublic/CRM/CompanyUpdate.aspx?Source=companycontacts&CompanyUpdatePageMode=Update&CompanyNumber=" & Session("LoggedInCompanyNumber")
            End If

            'Bind the Saved Search Repeater only if SAVED_SEARCH_ENABLED module default is true.
            If ModuleDefaults.SavedSearchEnabled Then
                RetrieveSavedSearches()
                SetLastSearchAndLink()
                plhSavedAgentSearch.Visible = True
            Else
                plhSavedAgentSearch.Visible = False
            End If
        Else
            plhLoggedIn.Visible = False
            plhNotLoggedIn.Visible = True
            ltlLogin.Text = _ucr.Content("LoginLabel", _languageCode, True)
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Request.QueryString("DeleteSearchID") IsNot Nothing Then
            Dim DeleteSearchID As Integer = Request.QueryString("DeleteSearchID")
            DeleteSavedSearch(DeleteSearchID)
        End If
    End Sub

    Protected Sub rptSavedSearches_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.Header Then
            Dim ltlSavedSearchLabel As Literal = CType(e.Item.FindControl("ltlSavedSearchLabel"), Literal)
            ltlSavedSearchLabel.Text = _ucr.Content("SavedSearchLabel", _languageCode, True)
        ElseIf e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim plhSearch As PlaceHolder = CType(e.Item.FindControl("plhSearch"), PlaceHolder)
            Dim lblSearch As Label = CType(e.Item.FindControl("lblSearch"), Label)
            Dim hlkSavedSearch As HyperLink = CType(e.Item.FindControl("hlkSavedSearch"), HyperLink)

            Dim searchTypeValue As String = e.Item.DataItem("SEARCHTYPE").ToString.Trim()
            Dim productTypeValue As String = e.Item.DataItem("PRODUCTTYPE").ToString.Trim()
            Dim stadiumValue As String = e.Item.DataItem("STADIUMCODE").ToString.Trim()
            Dim keywordValue As String = e.Item.DataItem("KEYWORDVALUE").ToString.Trim()
            Dim categoryValue As String = e.Item.DataItem("CATEGORYVALUE").ToString.Trim()
            Dim locationValue As String = e.Item.DataItem("LOCATIONVALUE").ToString.Trim()
            Dim dateValue As String = e.Item.DataItem("DATEVALUE").ToString.Trim()

            Dim searchid As String = CType(e.Item.FindControl("hdfSearchID"), HiddenField).Value
            Dim hlkDeleteSavedSearch As HyperLink = CType(e.Item.FindControl("hlkDeleteSavedSearch"), HyperLink)

            Dim sbUrl As New StringBuilder("~/PagesPublic/ProductBrowse/ProductSubtypes.aspx?")
            sbUrl.Append("SearchType=").Append(searchTypeValue)
            sbUrl.Append("&SearchKeyword=").Append(keywordValue)
            sbUrl.Append("&SearchDate=").Append(dateValue)
            sbUrl.Append("&SearchLocation=").Append(locationValue)
            sbUrl.Append("&SearchCategoryId=").Append(categoryValue)
            sbUrl.Append("&SearchStadium=").Append(stadiumValue)
            sbUrl.Append("&SearchProductType=").Append(productTypeValue)

            hlkSavedSearch.NavigateUrl = sbUrl.ToString
            hlkDeleteSavedSearch.NavigateUrl = Request.CurrentExecutionFilePath + "?DeleteSearchID=" + searchid

            Dim searchMask As String = _ucr.Content("SavedSearchMask-" & searchTypeValue, _languageCode, True)
            If searchMask.Contains("<<") Then
                searchMask = searchMask.Replace("<<Keyword>>", keywordValue)
                searchMask = searchMask.Replace("<<Date>>", dateValue)
                searchMask = searchMask.Replace("<<Location>>", GetLocationValues(locationValue))
                searchMask = searchMask.Replace("<<Category>>", GetCategoryDescription(categoryValue))
                searchMask = searchMask.Replace("<<Stadium>>", stadiumValue)
                searchMask = searchMask.Replace("<<ProductType>>", productTypeValue)
            End If

            lblSearch.Text = searchMask
            plhSearch.Visible = lblSearch.Text.Length > 0
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub RetrieveSavedSearches()
        Dim err As New ErrorObj
        Dim deAgent As New DEAgent
        Dim settings As DESettings = TEBUtilities.GetSettingsObjectForAgent()
        Dim ta As New TalentAgent
        deAgent.SavedSearchLimit = ModuleDefaults.SavedSearchLimit
        deAgent.Source = GlobalConstants.SOURCE
        deAgent.AgentUsername = AgentProfile.Name
        settings.Cacheing = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_ucr.Attribute("SavedAgentSearchCaching"))
        settings.CacheTimeMinutes = TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("SavedAgentSearchCachingTimeInMins"))
        settings.Company = AgentProfile.GetAgentCompany
        ta.AgentDataEntity = deAgent
        ta.Settings = settings
        err = ta.RetrieveSavedSearch()

        If Not err.HasError Then
            If ta.ResultDataSet.Tables(1).Rows.Count > 0 Then BindSavedSearchRepeater(ta.ResultDataSet.Tables(1))
        End If
    End Sub

    Private Sub BindSavedSearchRepeater(ByVal dtSavedSearches As DataTable)
        rptSavedSearches.DataSource = dtSavedSearches
        rptSavedSearches.DataBind()
    End Sub

    Private Sub DeleteSavedSearch(ByVal SavedSearchUniqueID As Integer)
        Dim err As New ErrorObj
        Dim deAgent As New DEAgent
        Dim settings As DESettings = TEBUtilities.GetSettingsObjectForAgent()
        Dim ta As New TalentAgent
        deAgent.SavedSearchLimit = ModuleDefaults.SavedSearchLimit
        deAgent.Source = GlobalConstants.SOURCE
        deAgent.AgentUsername = AgentProfile.Name
        deAgent.SavedSearchUniqueID = SavedSearchUniqueID
        settings.Company = AgentProfile.GetAgentCompany
        settings.Cacheing = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_ucr.Attribute("SavedAgentSearchCaching"))
        settings.CacheTimeMinutes = TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("SavedAgentSearchCachingTimeInMins"))
        ta.AgentDataEntity = deAgent
        ta.Settings = settings
        err = ta.DeleteSavedSearch()

        If Not err.HasError Then
            If ta.ResultDataSet.Tables(1).Rows.Count > 0 Then BindSavedSearchRepeater(ta.ResultDataSet.Tables(1))
        End If
    End Sub

    Private Sub SetLastSearchAndLink()
        Dim strSearchType As String = String.Empty
        Dim strSearchKeyword As String = String.Empty
        Dim strSearchDate As String = String.Empty
        Dim strSearchLocation As String = String.Empty
        Dim strSearchCategory As String = String.Empty
        Dim strSearchStadium As String = String.Empty
        Dim strSearchProductType As String = String.Empty

        If Not Session("SearchType") Is Nothing Then strSearchLocation = Session("SearchType")
        If Not Session("SearchStadium") Is Nothing Then strSearchLocation = Session("SearchStadium")
        If Not Session("SearchProductType") Is Nothing Then strSearchLocation = Session("SearchProductType")
        If Not Session("SearchLocation") Is Nothing Then strSearchLocation = Session("SearchLocation")
        If Not Session("SearchDate") Is Nothing Then strSearchDate = Session("SearchDate")
        If Not Session("SearchKeyword") Is Nothing Then strSearchKeyword = Session("SearchKeyword")
        If Not Session("SearchCategoryId") Is Nothing Then strSearchCategory = Session("SearchCategoryId")

        If String.IsNullOrEmpty(strSearchType) AndAlso String.IsNullOrEmpty(strSearchLocation) AndAlso String.IsNullOrEmpty(strSearchDate) AndAlso String.IsNullOrEmpty(strSearchCategory) AndAlso String.IsNullOrEmpty(strSearchKeyword) AndAlso String.IsNullOrEmpty(strSearchStadium) AndAlso String.IsNullOrEmpty(strSearchProductType) Then
            plhLastSearches.Visible = False
        Else
            plhLastSearches.Visible = True
            Dim searchMask As String = _ucr.Content("SavedSearchMask-" & strSearchType, _languageCode, True)
            If searchMask.Contains("<<") Then
                searchMask = searchMask.Replace("<<Keyword>>", strSearchKeyword)
                searchMask = searchMask.Replace("<<Date>>", strSearchDate)
                searchMask = searchMask.Replace("<<Location>>", GetLocationValues(strSearchLocation))
                searchMask = searchMask.Replace("<<Category>>", GetCategoryDescription(strSearchCategory))
                searchMask = searchMask.Replace("<<Stadium>>", strSearchStadium)
                searchMask = searchMask.Replace("<<ProductType>>", strSearchProductType)
            End If

            lblSearch.Text = searchMask
            plhSearch.Visible = lblSearch.Text.Length > 0

            Dim sbUrl As New StringBuilder("~/PagesPublic/ProductBrowse/ProductSubtypes.aspx?")
            sbUrl.Append("SearchType=").Append(strSearchType)
            sbUrl.Append("&SearchKeyword=").Append(strSearchKeyword)
            sbUrl.Append("&SearchCategoryId=").Append(strSearchCategory)
            sbUrl.Append("&SearchLocation=").Append(strSearchLocation)
            sbUrl.Append("&DateSearch=").Append(strSearchDate)
            sbUrl.Append("&SearchStadium=").Append(strSearchStadium)
            sbUrl.Append("&SearchProductType=").Append(strSearchProductType)
            hlkLastSavedSearch.NavigateUrl = sbUrl.ToString
            ltlLastSearchLabel.Text = _ucr.Content("LastSearchLabel", _languageCode, True)
        End If
    End Sub

    Private Sub btnOkPrinterSelection_Click(sender As Object, e As EventArgs) Handles btnOkPrinterSelection.Click
        Dim err As New ErrorObj
        Dim agentDataEntity As New DEAgent
        Dim settings As DESettings = TEBUtilities.GetSettingsObject()
        Dim talAgent As New TalentAgent
        agentDataEntity.AgentUsername = AgentProfile.Name
        agentDataEntity.Source = GlobalConstants.SOURCE
        agentDataEntity.AgentCompany = AgentProfile.GetAgentCompany(AgentProfile.Name)
        agentDataEntity.PrinterGrp = _printerGroup
        agentDataEntity.DftTKTPrtr = ddlPrinterList.SelectedValue
        agentDataEntity.DftSCPrtr = _smartcardPrinterDefault
        agentDataEntity.Tkt1_Home = _overridePrinterHome
        agentDataEntity.Tkt2_Event = _overridePrinterEvent
        agentDataEntity.Tkt3_Travel = _overridePrinterTravel
        agentDataEntity.Tkt4_STkt = _overridePrinterSTReceipts
        agentDataEntity.Tkt5_Addr = _overridePrinterAddress
        agentDataEntity.PrintAddrYN = _printAddressLabelsDefault
        agentDataEntity.PrintRcptYN = _printTransactionReceiptDefault
        agentDataEntity.Department = AgentProfile.Department(AgentProfile.Name)
        agentDataEntity.AgentAuthorityGroupID = _authorityGroups
        agentDataEntity.BulkSalesMode = _bulkSalesMode
        agentDataEntity.SessionID = Profile.Basket.Basket_Header_ID
        agentDataEntity.DefaultCaptureMethod = _captureMethod
        agentDataEntity.PrintAlways = _printAlways
        agentDataEntity.CorporateHospitalityMode = _corporateHospitalityMode
        talAgent.AgentDataEntity = agentDataEntity
        talAgent.Settings = settings
        err = talAgent.UpdateAgentPrinters

        If err.HasError Then
            Me.blErrList.Items.Add(_errMsg.GetErrorMessage("Error50").ERROR_MESSAGE)
        Else
            If talAgent.ResultDataSet.Tables("Status") IsNot Nothing AndAlso talAgent.ResultDataSet.Tables("Status").Rows.Count > 0 Then
                If talAgent.ResultDataSet.Tables("Status").Rows(0)(0) = GlobalConstants.ERRORFLAG Then
                    Dim errText As TalentErrorMessage = _errMsg.GetErrorMessage(GlobalConstants.STARALLPARTNER, _ucr.PageCode, talAgent.ResultDataSet.Tables("Status").Rows(0)("ReturnCode"))
                    blErrList.Items.Add(errText.ERROR_MESSAGE)
                Else
                    TDataObjects.AgentSettings.TblAgent.Update(AgentProfile.Name, agentDataEntity)
                    talAgent.RetrieveAllAgentsClearCache()
                    err = TCUtilities.ClearCacheDependencyOnAllServers(_ucr.BusinessUnit, GlobalConstants.AGENT_PROFILE_CACHEKEY, _ucr.FrontEndConnectionString)
                    Response.Redirect(Request.Url.PathAndQuery)  'This is required to resume the regular page load life-cycle without post back
                End If
            End If
        End If
    End Sub
    ''' <summary>
    ''' Set dropdown list & other details for printer selection window
    ''' </summary>
    Private Sub SetPrinterSelectionWindow()
        If AgentProfile.IsAgent Then
            Dim dtAgentDetails As DataTable = AgentProfile.GetAgentDetails(AgentProfile.Name)
            If (Not dtAgentDetails Is Nothing) Then
                Dim sPrinterGroup As String = dtAgentDetails.Rows(0)("PRINTER_GROUP").Trim
                ' Retrieve all system printer groups and all system printers
                Dim err As New ErrorObj
                Dim settings As DESettings = TEBUtilities.GetSettingsObject()
                Dim talAgent As New TalentAgent
                Dim agentDataEntity As New DEAgent
                agentDataEntity.AgentUsername = AgentProfile.Name
                agentDataEntity.Source = GlobalConstants.SOURCE
                talAgent.AgentDataEntity = agentDataEntity
                talAgent.Settings = settings
                err = talAgent.RetrieveAgentPrinters

                If err.HasError Then
                    Me.blErrList.Items.Add(_errMsg.GetErrorMessage("Error10").ERROR_MESSAGE)
                Else
                    If talAgent.ResultDataSet.Tables.Count <> 3 Then
                        Me.blErrList.Items.Add(_errMsg.GetErrorMessage("Error20").ERROR_MESSAGE)
                    Else

                        Dim dtGroups As Data.DataTable = talAgent.ResultDataSet.Tables(1)
                        Dim dtPrinters As Data.DataTable = talAgent.ResultDataSet.Tables(2)
                        If dtGroups.Rows.Count = 0 Or dtPrinters.Rows.Count = 0 Then
                            Me.blErrList.Items.Add(_errMsg.GetErrorMessage("Error30").ERROR_MESSAGE)
                        Else
                            _authorityGroups = TEBUtilities.CheckForDBNull_Int(dtAgentDetails.Rows(0)("GROUP_ID"))
                            _captureMethod = TEBUtilities.CheckForDBNull_String(dtAgentDetails.Rows(0)("CAPTURE_METHOD"))
                            ' Load Printer Group ddl
                            For Each drGroup As Data.DataRow In dtGroups.Rows
                                If drGroup("GROUPNAME").ToString.Trim = sPrinterGroup Then
                                    _printerGroup = drGroup("GROUPNAME")
                                End If
                            Next

                            ' Load Ticketing and Smartcard Printer ddl's
                            For Each drPrinter As Data.DataRow In dtPrinters.Rows
                                If drPrinter("PRINTERGROUP").ToString.Trim = sPrinterGroup Then
                                    If drPrinter("PRINTERTYPE") = GlobalConstants.TICKETINGPRODUCTTYPE Then
                                        ddlPrinterList.Items.Add(New ListItem(drPrinter("PRINTERDESCRIPTION"), drPrinter("PRINTERNAME")))
                                        If drPrinter("PRINTERNAME").ToString.Trim = dtAgentDetails.Rows(0)("DEFAULT_TICKET_PRINTER").ToString.Trim Then
                                            _ticketPrinterDefault = drPrinter("PRINTERNAME")
                                            ddlPrinterList.SelectedValue = drPrinter("PRINTERNAME")
                                            Me.hdfDefaultSelectedPrinter.Value = ddlPrinterList.SelectedIndex
                                        End If
                                    End If
                                    If drPrinter("PRINTERTYPE") = GlobalConstants.SEASONTICKETPRODUCTTYPE Then
                                        If drPrinter("PRINTERNAME").ToString.Trim = dtAgentDetails.Rows(0)("DEFAULT_SMARTCARD_PRINTER").ToString.Trim Then
                                            _smartcardPrinterDefault = drPrinter("PRINTERNAME")
                                        End If
                                    End If
                                End If
                            Next
                            _overridePrinterHome = dtAgentDetails.Rows(0)("OVERRIDE_PRINTER_HOME")
                            _overridePrinterEvent = dtAgentDetails.Rows(0)("OVERRIDE_PRINTER_EVENT")
                            _overridePrinterTravel = dtAgentDetails.Rows(0)("OVERRIDE_PRINTER_TRAVEL")
                            _overridePrinterSTReceipts = dtAgentDetails.Rows(0)("OVERRIDE_PRINTER_SEASONTICKET")
                            _overridePrinterAddress = dtAgentDetails.Rows(0)("OVERRIDE_PRINTER_ADDRESS")
                            _printAddressLabelsDefault = TCUtilities.convertToBool(dtAgentDetails.Rows(0)("PRINT_ADDRESS_LABELS_DEFAULT"))
                            _printTransactionReceiptDefault = TCUtilities.convertToBool(dtAgentDetails.Rows(0)("PRINT_TRANSACTION_RECEIPTS_DEFAULT"))
                            _bulkSalesMode = TCUtilities.convertToBool(dtAgentDetails.Rows(0)("BULK_SALES_MODE"))
                            _printAlways = TCUtilities.convertToBool(dtAgentDetails.Rows(0)("PRINT_ALWAYS"))
                            _agentDepartment = AgentProfile.GetAgentDepartmentDescription(dtAgentDetails.Rows(0)("DEPARTMENT"))
                            _corporateHospitalityMode = TCUtilities.convertToBool(dtAgentDetails.Rows(0)("CORPORATE_SALES_AGENT"))
                        End If
                    End If
                End If
            End If
        End If
    End Sub

#End Region

#Region "Private Functions"

    Private Function GetCategoryDescription(ByVal categoryId As String) As String
        Dim categoryDescription As String = String.Empty
        If categoryId.Length > 0 Then
            Dim dtCategories As DataTable = TDataObjects.ProductsSettings.TblEventCategory.GetAllEventCategoriesByBUAndPartner(_ucr.BusinessUnit, _ucr.PartnerCode)
            If dtCategories IsNot Nothing AndAlso dtCategories.Rows.Count > 0 Then
                Dim rows() As DataRow = dtCategories.Select("CATEGORY_NUMBER= " + categoryId)
                If rows.Length > 0 Then
                    categoryDescription = rows(0).Item("CATEGORY_DESCRIPTION")
                End If
            End If
        End If
        Return categoryDescription
    End Function

    Private Function GetLocationValues(ByVal locationId As String) As String
        Dim locationText As String = String.Empty
        If locationId.Length > 0 Then
            Dim dtLocationDetails As DataTable = GetLocationTable()
            If dtLocationDetails IsNot Nothing AndAlso dtLocationDetails.Rows.Count > 0 Then
                Dim rows() As DataRow = dtLocationDetails.Select("LocationId= " + locationId)
                If rows.Length > 0 Then
                    locationText = rows(0).Item("Location")
                End If
            End If
        End If
        Return locationText
    End Function

    Private Function GetLocationTable() As DataTable
        Dim talentProduct As New TalentProduct
        Dim settings As New DESettings
        Dim err As New ErrorObj
        Dim dtLocations As New DataTable("Details")
        With dtLocations.Columns
            .Add("LocationId", GetType(String))
            .Add("Location", GetType(String))
        End With

        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.BusinessUnit = TalentCache.GetBusinessUnit()
        settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
        settings.Cacheing = True
        talentProduct.Settings() = settings
        talentProduct.Settings.OriginatingSourceCode = "W"

        err = talentProduct.ProductSubTypesList
        Dim dtLocationDetails As DataTable = talentProduct.ResultDataSet.Tables("LocationDetails")
        Return dtLocationDetails
    End Function

    Private Function RetrieveStopCodeDescription(ByVal stopCode As String) As String
        Dim err As New Talent.Common.ErrorObj
        Dim pb As New TalentStopcodes
        Dim Settings As New DESettings
        Dim stopCodeDescription As New String(String.Empty)

        Settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        Settings.BusinessUnit = TalentCache.GetBusinessUnit()
        Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
        Settings.Cacheing = True
        Settings.Company = AgentProfile.GetAgentCompany
        pb.Settings = Settings
        err = pb.RetrieveTalentStopcodes()

        If Not err.HasError AndAlso pb.ResultDataSet IsNot Nothing AndAlso pb.ResultDataSet.Tables.Count() > 0 Then
            Dim rows() As DataRow = pb.ResultDataSet.Tables(0).Select(("Code = '" & stopCode & "'"))
            If rows.Length > 0 Then
                stopCodeDescription = rows(0)("Description")
            End If
        End If
        Return stopCodeDescription
    End Function

#End Region

End Class
