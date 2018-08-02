Imports System.Data
Imports Talent.eCommerce
Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class UserControls_AgentPreferences
    Inherits ControlBase

#Region "Class Level Fields"

    Private _ucr As Talent.Common.UserControlResource = Nothing
    Private _languageCode As String = String.Empty
    Private _errMsg As TalentErrorMessages

#End Region

#Region "Public Properties"

    Public AgentName As String
    Public BulkSalesCheckBoxEnabled As Boolean = True
    Public Property AuthorityGroupEnabled As Boolean
        Get
            Return ddlAuthorityGroups.Enabled
        End Get
        Set(value As Boolean)
            ddlAuthorityGroups.Enabled = value
        End Set
    End Property
    Public Property OldSelectedAgent() As String
        Get
            Return If(ViewState("Message") IsNot Nothing, ViewState("Message").ToString(), String.Empty)
        End Get
        Set(ByVal value As String)
            ViewState("Message") = value
        End Set
    End Property

#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        plhAgentCopySuccessMessage.Visible = False
        If Not Session("AgentCopySuccess") Is Nothing Then
            plhAgentCopySuccessMessage.Visible = True
            ltlAgentCopySuccessMessage.Text = Session("AgentCopySuccess")
        Else
            plhAgentCopySuccessMessage.Visible = False
        End If

        InitializeProperties()

        If AgentProfile.AgentPermissions.IsAdministrator Then
            uscAgentList.Visible = True
            ddlAuthorityGroups.Enabled = True
        Else
            uscAgentList.Visible = False
            ddlAuthorityGroups.Enabled = False
            agentPreferenceHeader.Visible = False
        End If

        If Not IsPostBack Then
            uscAgentList.LoadAgents()
            LoadAuthorityGroups()
            LoadDropdownLists()
            DoLabels()
        End If


        If Session("AgentCopySuccess") Is Nothing Then
            AgentName = CType(uscAgentList.FindControl("ddlAgents"), DropDownList).SelectedValue
        Else
            CType(uscAgentList.FindControl("ddlAgents"), DropDownList).SelectedValue = Session("NewAgent")
        End If

        If OldSelectedAgent <> AgentName And Not OldSelectedAgent Is String.Empty Then
            LoadAuthorityGroups()
            LoadDropdownLists()
        End If
        Session("AgentCopySuccess") = Nothing
        Session("NewAgent") = Nothing
        OldSelectedAgent = CType(uscAgentList.FindControl("ddlAgents"), DropDownList).SelectedValue

    End Sub

    Private Sub InitializeProperties()
        If _ucr Is Nothing Then
            _ucr = New Talent.Common.UserControlResource
            With _ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "AgentPreferences.ascx"
            End With
            _languageCode = Talent.Common.Utilities.GetDefaultLanguage
            _errMsg = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), _ucr.FrontEndConnectionString)
            blErrList.Items.Clear()
            blSuccessList.Items.Clear()
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
        plhErrorList.Visible = (blErrList.Items.Count > 0)
        plhSuccessList.Visible = (blSuccessList.Items.Count > 0)
        plhDepartment.Visible = ModuleDefaults.ShowAgentDepartment
        SetBulkSalesVsPrintAlways()
    End Sub

    Protected Sub ddlPrinterGroup_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlPrinterGroup.SelectedIndexChanged
        LoadDropdownLists(Me.ddlPrinterGroup.SelectedValue)
    End Sub

    Protected Sub csvAuthorityGroup_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles csvAuthorityGroup.ServerValidate
        args.IsValid = False
        If Me.ddlAuthorityGroups.SelectedValue.Trim <> "" And Me.ddlAuthorityGroups.SelectedValue.Trim <> "0" Then args.IsValid = True
    End Sub

    Protected Sub csvPrinterGroup_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles csvPrinterGroup.ServerValidate
        args.IsValid = False
        If Me.ddlPrinterGroup.SelectedValue.Trim <> "" And Me.ddlPrinterGroup.SelectedValue.Trim <> "none" Then args.IsValid = True
    End Sub

    Protected Sub csvTicketPrinterDefault_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles csvTicketPrinterDefault.ServerValidate
        args.IsValid = False
        If Me.ddlTicketPrinterDefault.SelectedValue.Trim <> "" And Me.ddlTicketPrinterDefault.SelectedValue.Trim <> "none" Then args.IsValid = True
    End Sub

    Protected Sub csvSmartcardPrinterDefault_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles csvSmartcardPrinterDefault.ServerValidate
        args.IsValid = False
        If Me.ddlSmartcardPrinterDefault.SelectedValue.Trim <> "" And Me.ddlSmartcardPrinterDefault.SelectedValue.Trim <> "none" Then args.IsValid = True
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Page.IsValid AndAlso IsValidBulkSalesVsPrintAlways() Then
            ' Retrieve all system printer groups and all system printers
            Dim err As New ErrorObj
            Dim settings As DESettings = TEBUtilities.GetSettingsObject()
            Dim talAgent As New TalentAgent
            Dim agentDataEntity As New DEAgent
            Dim clearBasket As Boolean = (AgentProfile.BulkSalesMode <> chkBulkSalesMode.Checked)

            agentDataEntity.AgentUsername = AgentName
            agentDataEntity.Source = GlobalConstants.SOURCE
            agentDataEntity.AgentCompany = AgentProfile.GetAgentCompany(AgentName)
            agentDataEntity.PrinterGrp = ddlPrinterGroup.SelectedValue
            agentDataEntity.DftTKTPrtr = ddlTicketPrinterDefault.SelectedValue
            agentDataEntity.DftSCPrtr = ddlSmartcardPrinterDefault.SelectedValue
            agentDataEntity.Tkt1_Home = replaceDefaultPrinterName(txtOverridePrinterHome.Text)
            agentDataEntity.Tkt2_Event = replaceDefaultPrinterName(txtOverridePrinterEvent.Text)
            agentDataEntity.Tkt3_Travel = replaceDefaultPrinterName(txtOverridePrinterTravel.Text)
            agentDataEntity.Tkt4_STkt = replaceDefaultPrinterName(txtOverridePrinterSTReceipts.Text)
            agentDataEntity.Tkt5_Addr = replaceDefaultPrinterName(txtOverridePrinterAddress.Text)
            agentDataEntity.PrintAddrYN = Talent.Common.Utilities.ConvertToYN(chkPrintAddressLabelsDefault.Checked)
            agentDataEntity.PrintRcptYN = Talent.Common.Utilities.ConvertToYN(chkPrintTransactionReceiptDefault.Checked)
            agentDataEntity.Department = AgentProfile.Department(AgentName)
            agentDataEntity.AgentAuthorityGroupID = ddlAuthorityGroups.SelectedValue
            agentDataEntity.BulkSalesMode = chkBulkSalesMode.Checked
            agentDataEntity.SessionID = Profile.Basket.Basket_Header_ID
            agentDataEntity.DefaultCaptureMethod = ddlCaptureMethod.SelectedValue
            agentDataEntity.PrintAlways = chkPrintAlways.Checked
            agentDataEntity.CorporateHospitalityMode = chkCorporateHospitalityMode.Checked
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
                        blSuccessList.Items.Add(_ucr.Content("SuccessfulUpdate", _languageCode, True))
                        TDataObjects.AgentSettings.TblAgent.Update(AgentName, agentDataEntity)
                        talAgent.RetrieveAllAgentsClearCache()
                        err = TCUtilities.ClearCacheDependencyOnAllServers(_ucr.BusinessUnit, GlobalConstants.AGENT_PROFILE_CACHEKEY, _ucr.FrontEndConnectionString)
                    End If
                End If
            End If

            If clearBasket AndAlso Profile.Basket.BasketItems.Count > 0 Then
                Dim tGatewayFunctions As New TicketingGatewayFunctions
                tGatewayFunctions.Basket_ClearBasket(False)
                Profile.Basket.EmptyBasket(True)
                Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True)
            End If
        End If
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Set all the text labels for the page
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DoLabels()
        With _ucr
            ltlAgentPreferencesInstructionsLabel.Text = .Content("lblAgentPreferencesInstructionsLabel", _languageCode, True)
            lblAuthorityGroup.Text = .Content("lblAuthorityGroup", _languageCode, True)
            lblAgentPreferences.Text = .Content("lblAgentPreferences", _languageCode, True)
            lblPrinterGroup.Text = .Content("lblPrinterGroup", _languageCode, True)
            lblTicketPrinterDefault.Text = .Content("lblTicketPrinterDefault", _languageCode, True)
            lblSmartcardPrinterDefault.Text = .Content("lblSmartcardPrinterDefault", _languageCode, True)
            lblOverridePrinterHome.Text = .Content("lblOverridePrinterHome", _languageCode, True)
            lblOverridePrinterEvent.Text = .Content("lblOverridePrinterEvent", _languageCode, True)
            lblOverridePrinterTravel.Text = .Content("lblOverridePrinterTravel", _languageCode, True)
            lblOverridePrinterSTReceipts.Text = .Content("lblOverridePrinterSTReceipts", _languageCode, True)
            lblOverridePrinterAddress.Text = .Content("lblOverridePrinterAddress", _languageCode, True)
            lblDefaultCaptureMethod.Text = .Content("lblDefaultCaptureMethod", _languageCode, True)
            lblPrintAddressLabelsDefault.Text = .Content("lblPrintAddressLabelsDefault", _languageCode, True)
            lblPrintTransactionReceiptDefault.Text = .Content("lblPrintTransactionReceiptDefault", _languageCode, True)
            lblBulkSalesMode.Text = .Content("lblBulkSalesMode", _languageCode, True)
            ltlDepartment.Text = .Content("ltlDepartment", _languageCode, True)
            btnSave.Text = .Content("btnSave", _languageCode, True)
            lblPrintAlways.Text = .Content("lblPrintAlways", _languageCode, True)
            ltlBulkSalesVsPrintAlways.Text = .Content("BulkOrPrintAlwaysMessage", _languageCode, True)
            lblCorporateHospitalityMode.Text = .Content("lblCorporateHospitalityMode", _languageCode, True)
            If Profile.Basket.BasketItems.Count > 0 Then
                Dim jscript As New StringBuilder
                jscript.Append("javascript:{").Append(vbCrLf)
                If chkBulkSalesMode.Checked Then
                    jscript.Append("    if (document.getElementById(""").Append(chkBulkSalesMode.ClientID).Append(""").checked == false) {").Append(vbCrLf)
                Else
                    jscript.Append("    if (document.getElementById(""").Append(chkBulkSalesMode.ClientID).Append(""").checked == true) {").Append(vbCrLf)
                End If
                jscript.Append("        return confirm('").Append(.Content("BasketHasItemsWarningMessage", _languageCode, True)).Append("');").Append(vbCrLf)
                jscript.Append("    } else {").Append(vbCrLf)
                jscript.Append("        return true;").Append(vbCrLf)
                jscript.Append("    }").Append(vbCrLf)
                jscript.Append("}")
                btnSave.OnClientClick = jscript.ToString()
            Else
                btnSave.OnClientClick = String.Empty
            End If
            rfvAuthorityGroup.Enabled = ddlAuthorityGroups.Enabled
            csvAuthorityGroup.Enabled = ddlAuthorityGroups.Enabled
            rfvAuthorityGroup.ErrorMessage = _ucr.Content("PleaseSelectAnAuthorityGroupError", _languageCode, True)
            csvAuthorityGroup.ErrorMessage = _ucr.Content("PleaseSelectAnAuthorityGroupError", _languageCode, True)
            rfvPrinterGroup.ErrorMessage = _ucr.Content("PleaseSelectAPrinterGroupError", _languageCode, True)
            csvPrinterGroup.ErrorMessage = _ucr.Content("PleaseSelectAPrinterGroupError", _languageCode, True)
            rfvTicketPrinterDefault.ErrorMessage = _ucr.Content("PleaseSelectTicketPrinterDefaultError", _languageCode, True)
            csvTicketPrinterDefault.ErrorMessage = _ucr.Content("PleaseSelectTicketPrinterDefaultError", _languageCode, True)
            rfvSmartcardPrinterDefault.ErrorMessage = _ucr.Content("PleaseSelectSmartcardPrinterDefaultError", _languageCode, True)
            csvSmartcardPrinterDefault.ErrorMessage = _ucr.Content("PleaseSelectSmartcardPrinterDefaultError", _languageCode, True)
            rfvOverridePrinterHome.ErrorMessage = _ucr.Content("InvalidHomePrinterError", _languageCode, True)
            rfvOverridePrinterEvent.ErrorMessage = _ucr.Content("InvalidEventPrinterError", _languageCode, True)
            rfvOverridePrinterTravel.ErrorMessage = _ucr.Content("InvalidTravelPrinterError", _languageCode, True)
            rfvOverridePrinterSTReceipts.ErrorMessage = _ucr.Content("InvalidSTReceiptsPrinterError", _languageCode, True)
            rfvOverridePrinterAddress.ErrorMessage = _ucr.Content("InvalidAddressPrinterError", _languageCode, True)
            rfvDefaultCaptureMethod.ErrorMessage = _ucr.Content("PleaseSelectDefaultCaptureMethodError", _languageCode, True)
        End With
    End Sub

    ''' <summary>
    ''' Load the Authority Groups
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LoadAuthorityGroups()
        InitializeProperties()
        Dim err As New ErrorObj
        Dim SelectedGroup As String = String.Empty
        Dim dtAgentGroups As DataTable = Nothing
        dtAgentGroups = TDataObjects.TalentDefinitionsSettings.TblAgentGroupRoles.GetAllGroups(False)
        ddlAuthorityGroups.DataSource = dtAgentGroups
        ddlAuthorityGroups.DataTextField = "GROUP_NAME"
        ddlAuthorityGroups.DataValueField = "GROUP_ID"
        ddlAuthorityGroups.DataBind()
        ddlAuthorityGroups.Items.Insert(0, New ListItem(_ucr.Content("SelectAuthorityGroup", _languageCode, True), "0"))
    End Sub

    Private Sub LoadCaptureMethods(ByVal strSelectedCaptureMethod As String)
        Dim dicVanguardCaptureMethod As Generic.Dictionary(Of String, String) = TDataObjects.AppVariableSettings.TblDefaults.GetVanguardCaptureMethod(_ucr.BusinessUnit, _languageCode)
        ddlCaptureMethod.DataSource = dicVanguardCaptureMethod
        ddlCaptureMethod.DataTextField = "Value"
        ddlCaptureMethod.DataValueField = "Key"
        ddlCaptureMethod.DataBind()
        ddlCaptureMethod.SelectedIndex = -1
        ddlCaptureMethod.Items.Insert(0, New ListItem(_ucr.Content("SelectCaptureMethodDefault", _languageCode, True), "0"))
        If Not String.IsNullOrWhiteSpace(strSelectedCaptureMethod) Then
            ddlCaptureMethod.SelectedValue = strSelectedCaptureMethod
        End If
    End Sub

    Private Sub SetBulkSalesVsPrintAlways()
        Dim clientScriptMethod As String = "ValidateBulkSalesAndPrintAlways('" & chkBulkSalesMode.ClientID & "','" & chkPrintAlways.ClientID & "'," & CStr(BulkSalesCheckBoxEnabled).ToLower() & ")"
        chkBulkSalesMode.Attributes.Add("onclick", clientScriptMethod)
        chkPrintAlways.Attributes.Add("onclick", clientScriptMethod)
        Page.ClientScript.RegisterStartupScript(Page.GetType(), "BulkSalesVSPrintAlwaysJS", clientScriptMethod, True)
    End Sub

#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Load the drop down lists for printers
    ''' </summary>
    ''' <param name="sPrinterGroup">The printer group</param>
    ''' <returns>True when successful</returns>
    ''' <remarks></remarks>
    Public Function LoadDropdownLists(Optional ByVal sPrinterGroup As String = "") As Boolean
        InitializeProperties()
        ' Clear all entries
        ddlPrinterGroup.Items.Clear()
        ddlTicketPrinterDefault.Items.Clear()
        ddlSmartcardPrinterDefault.Items.Clear()
        If Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultFalse(ModuleDefaults.AllowAgentPreferencesUpdates) Then
            If AgentProfile.IsAgent Then
                Dim dtAgentDetails As DataTable = AgentProfile.GetAgentDetails(AgentName)
                If dtAgentDetails Is Nothing OrElse dtAgentDetails.Rows.Count = 0 Then
                    Me.blErrList.Items.Add(_errMsg.GetErrorMessage("Error00").ERROR_MESSAGE)
                    Return False
                End If
                LoadCaptureMethods(TEBUtilities.CheckForDBNull_String(dtAgentDetails.Rows(0)("CAPTURE_METHOD")))

                ' Printer Group to use is....
                If sPrinterGroup = "" Then
                    sPrinterGroup = dtAgentDetails.Rows(0)("PRINTER_GROUP").Trim
                End If

                ' Retrieve all system printer groups and all system printers
                Dim err As New ErrorObj
                Dim settings As DESettings = TEBUtilities.GetSettingsObject()
                Dim talAgent As New TalentAgent
                Dim agentDataEntity As New DEAgent
                agentDataEntity.AgentUsername = AgentName
                agentDataEntity.Source = GlobalConstants.SOURCE
                talAgent.AgentDataEntity = agentDataEntity
                talAgent.Settings = settings
                err = talAgent.RetrieveAgentPrinters

                If err.HasError Then
                    Me.blErrList.Items.Add(_errMsg.GetErrorMessage("Error10").ERROR_MESSAGE)
                    Return False
                Else
                    If talAgent.ResultDataSet.Tables.Count <> 3 Then
                        Me.blErrList.Items.Add(_errMsg.GetErrorMessage("Error20").ERROR_MESSAGE)
                        Return False
                    Else
                        Dim dtGroups As Data.DataTable = talAgent.ResultDataSet.Tables(1)
                        Dim dtPrinters As Data.DataTable = talAgent.ResultDataSet.Tables(2)
                        If dtGroups.Rows.Count = 0 Or dtPrinters.Rows.Count = 0 Then
                            Me.blErrList.Items.Add(_errMsg.GetErrorMessage("Error30").ERROR_MESSAGE)
                            Return False
                        Else
                            ddlAuthorityGroups.SelectedValue = TEBUtilities.CheckForDBNull_Int(dtAgentDetails.Rows(0)("GROUP_ID"))

                            'Default for Printer Group ddl
                            ddlPrinterGroup.Items.Add(New ListItem(_ucr.Content("SelectPrinterGroup", _languageCode, True), "none"))
                            ddlPrinterGroup.SelectedValue = "none"

                            ' Load Printer Group ddl
                            For Each drGroup As Data.DataRow In dtGroups.Rows
                                ddlPrinterGroup.Items.Add(New ListItem(drGroup("GROUPDESCRIPTION"), drGroup("GROUPNAME")))
                                If drGroup("GROUPNAME").ToString.Trim = sPrinterGroup Then
                                    ddlPrinterGroup.SelectedValue = drGroup("GROUPNAME")
                                End If
                            Next
                            'Defaults for Ticketing and Smartcard Printer ddl's
                            ddlTicketPrinterDefault.Items.Add(New ListItem(_ucr.Content("SelectTicketPrinterDefault", _languageCode, True), "none"))
                            ddlTicketPrinterDefault.SelectedValue = "none"
                            ddlSmartcardPrinterDefault.Items.Add(New ListItem(_ucr.Content("SelectSmartcardPrinterDefault", _languageCode, True), "none"))
                            ddlSmartcardPrinterDefault.SelectedValue = "none"

                            If ddlPrinterGroup.Items.Count > 1 Then
                                ' Load Ticketing and Smartcard Printer ddl's
                                For Each drPrinter As Data.DataRow In dtPrinters.Rows
                                    If drPrinter("PRINTERGROUP").ToString.Trim = sPrinterGroup Then
                                        If drPrinter("PRINTERTYPE") = GlobalConstants.TRAVELPRODUCTTYPE Then
                                            ddlTicketPrinterDefault.Items.Add(New ListItem(drPrinter("PRINTERDESCRIPTION"), drPrinter("PRINTERNAME")))
                                            If drPrinter("PRINTERNAME").ToString.Trim = dtAgentDetails.Rows(0)("DEFAULT_TICKET_PRINTER").ToString.Trim Then
                                                ddlTicketPrinterDefault.SelectedValue = drPrinter("PRINTERNAME")
                                            End If
                                        End If
                                        If drPrinter("PRINTERTYPE") = GlobalConstants.SEASONTICKETPRODUCTTYPE Then
                                            ddlSmartcardPrinterDefault.Items.Add(New ListItem(drPrinter("PRINTERDESCRIPTION"), drPrinter("PRINTERNAME")))
                                            If drPrinter("PRINTERNAME").ToString.Trim = dtAgentDetails.Rows(0)("DEFAULT_SMARTCARD_PRINTER").ToString.Trim Then
                                                ddlSmartcardPrinterDefault.SelectedValue = drPrinter("PRINTERNAME")
                                            End If
                                        End If
                                    End If
                                Next

                                If ddlSmartcardPrinterDefault.Items.Count <= 1 Then
                                    plhSmartPrinterDefault.Visible = False
                                Else
                                    plhSmartPrinterDefault.Visible = True
                                End If
                            Else
                                ddlSmartcardPrinterDefault.Enabled = False
                                ddlTicketPrinterDefault.Enabled = False
                                blErrList.Items.Add(_ucr.Content("NoPrinterGroup", _languageCode, True))
                            End If
                            txtOverridePrinterHome.Text = replaceDefaultPrinterName(dtAgentDetails.Rows(0)("OVERRIDE_PRINTER_HOME"))
                            txtOverridePrinterEvent.Text = replaceDefaultPrinterName(dtAgentDetails.Rows(0)("OVERRIDE_PRINTER_EVENT"))
                            txtOverridePrinterTravel.Text = replaceDefaultPrinterName(dtAgentDetails.Rows(0)("OVERRIDE_PRINTER_TRAVEL"))
                            txtOverridePrinterSTReceipts.Text = replaceDefaultPrinterName(dtAgentDetails.Rows(0)("OVERRIDE_PRINTER_SEASONTICKET"))
                            txtOverridePrinterAddress.Text = replaceDefaultPrinterName(dtAgentDetails.Rows(0)("OVERRIDE_PRINTER_ADDRESS"))
                            chkPrintAddressLabelsDefault.Checked = TCUtilities.convertToBool(dtAgentDetails.Rows(0)("PRINT_ADDRESS_LABELS_DEFAULT"))
                            chkPrintTransactionReceiptDefault.Checked = TCUtilities.convertToBool(dtAgentDetails.Rows(0)("PRINT_TRANSACTION_RECEIPTS_DEFAULT"))
                            chkBulkSalesMode.Checked = TCUtilities.convertToBool(dtAgentDetails.Rows(0)("BULK_SALES_MODE"))
                            chkBulkSalesMode.Enabled = BulkSalesCheckBoxEnabled
                            chkPrintAlways.Checked = TCUtilities.convertToBool(dtAgentDetails.Rows(0)("PRINT_ALWAYS"))
                            chkPrintAlways.Enabled = BulkSalesCheckBoxEnabled
                            ltlAgentDepartment.Text = AgentProfile.GetAgentDepartmentDescription(dtAgentDetails.Rows(0)("DEPARTMENT"))
                            chkCorporateHospitalityMode.Checked = TCUtilities.convertToBool(dtAgentDetails.Rows(0)("CORPORATE_SALES_AGENT"))
                        End If
                    End If
                End If
            Else
                blErrList.Items.Add(_ucr.Content("NoAgentLogin", _languageCode, True))
                plhAgentPreferences.Visible = False
            End If
        Else
            blErrList.Items.Add(_ucr.Content("NoAgentPreferences", _languageCode, True))
            plhAgentPreferences.Visible = False
        End If

        Return True
    End Function

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Replace the default printer name (iSeries name) with a soft coded text name
    ''' </summary>
    ''' <param name="printerName">The iSeries default printer name</param>
    ''' <returns>The friendly name string</returns>
    ''' <remarks></remarks>
    Private Function replaceDefaultPrinterName(ByRef printerName As String) As String
        Dim newPrinterName As String = printerName
        If printerName = GlobalConstants.AGENT_WRKSTN Then
            newPrinterName = _ucr.Content("DefaultPrinterText", _languageCode, True)
        ElseIf printerName = _ucr.Content("DefaultPrinterText", _languageCode, True) Then
            newPrinterName = GlobalConstants.AGENT_WRKSTN
        End If
        Return newPrinterName
    End Function

    Private Function IsValidBulkSalesVsPrintAlways() As Boolean
        Dim isValid As Boolean = True
        If chkBulkSalesMode.Checked AndAlso chkPrintAlways.Checked Then
            isValid = False
            Me.blErrList.Items.Add(_ucr.Content("BulkOrPrintAlwaysMessage", _languageCode, True))
        End If
        Return isValid
    End Function


#End Region

End Class
