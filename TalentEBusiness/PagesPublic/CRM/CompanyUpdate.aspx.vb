Imports Talent.Common
Imports System.Data
Imports System.Web.UI
Imports System.Collections.Generic
Imports TalentBusinessLogic.ModelBuilders.CRM
Imports TalentBusinessLogic.Models
Imports TalentBusinessLogic.BusinessObjects.Definitions
Imports Talent.Common.DEEcommerceModuleDefaults
Imports Talent.eCommerce.Utilities
Imports Talent.eCommerce


Partial Class PagesPublic_Company_CompanyUpdate
    Inherits TalentBase01

#Region "Class Level Fields"
    Private _pageMode As GlobalConstants.CRUDOperationMode
    Private _viewModel As CompanyUpdateViewModel
    Private _companyNumber As String = String.Empty
#End Region

#Region "Protected Page Events"
    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init

        'Determine the company number based on either session or query string
        _companyNumber = Nothing
        If Not String.IsNullOrWhiteSpace(Request.QueryString("CompanyNumber")) Then
            Session("SelectedCompanyNumber") = Request.QueryString("CompanyNumber")
            _companyNumber = Request.QueryString("CompanyNumber")
        ElseIf Not String.IsNullOrWhiteSpace(Session("LoggedInCompanyNumber")) Then
            _companyNumber = Session("LoggedInCompanyNumber")
        ElseIf Not String.IsNullOrWhiteSpace(Session("SelectedCompanyNumber")) Then
            _companyNumber = Session("SelectedCompanyNumber")
        End If

        'Set the page Mode.
        If Request.QueryString("CompanyUpdatePageMode") = "Add" Then
            _pageMode = Talent.Common.GlobalConstants.CRUDOperationMode.Create
            _companyNumber = Nothing
        ElseIf Request.QueryString("CompanyUpdatePageMode") = "Update" OrElse Not String.IsNullOrWhiteSpace(_companyNumber) Then
            _pageMode = Talent.Common.GlobalConstants.CRUDOperationMode.Update
        End If

        ' Initial processing or redirects if information is missing
        If String.IsNullOrWhiteSpace(_pageMode) Then
            Response.Redirect("~/PagesPublic/Profile/CustomerSelection.aspx?displayMode=ShowCompanySearch&companyColumnMode=SelectOnly")
        ElseIf _pageMode = Talent.Common.GlobalConstants.CRUDOperationMode.Update AndAlso String.IsNullOrWhiteSpace(_companyNumber) Then
            Response.Redirect("~/PagesPublic/Profile/CustomerSelection.aspx?displayMode=ShowCompanySearch&companyColumnMode=SelectOnly")
        End If



    End Sub
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim inputModel As CompanyUpdateInputModel = setInputModel()
        processController(inputModel)
        createView()
    End Sub

#End Region

#Region "Private Methods"
    Private Function IsAddingNewParent() As Boolean
        If Request.QueryString("AddType") = "NewParent" OrElse Request.QueryString("AddType") = "AddToNull" Then
            Return True
        Else
            Return False
        End If
        Return False
    End Function

    Private Function SetInputModel() As CompanyUpdateInputModel
        Dim inputModel As CompanyUpdateInputModel = New CompanyUpdateInputModel()
        If _companyNumber IsNot Nothing AndAlso Not String.IsNullOrEmpty(_companyNumber) Then
            inputModel.CompanyNumber = _companyNumber
        End If

        'Set the parent id and child id dependant on mode.
        If Not String.IsNullOrEmpty(Request.QueryString("ParentCompanyNumber")) Then
            inputModel.ParentCompanyNumber = Request.QueryString("ParentCompanyNumber")
            If Request.QueryString("AddType") = "ExistingParent" Then
                inputModel.AddParent = True
            End If
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("ChildCompanyNumber")) AndAlso IsAddingNewParent() Then
            inputModel.ChildCompanyNumber = Request.QueryString("ChildCompanyNumber")
            inputModel.AddParent = True
        ElseIf Request.QueryString("AddType") = "AddToNullSubsidiaries" Then
            inputModel.ChildCompanyNumber = Request.QueryString("SubsidiaryCompanyNumber")
        End If

        ' I only need to populate the screen fields on postback.  
        If IsPostBack Then
            inputModel.CompanyName = txtCompanyName.Text.Trim
            inputModel.TelephoneNumber1 = txtTelephoneNumber1.Text.Trim
            inputModel.TelephoneNumber2 = txtTelephoneNumber2.Text.Trim
            inputModel.Telephone1Use = ckContactByTelephoneNumber1.Checked
            inputModel.Telephone2Use = ckContactByTelephoneNumber2.Checked
            inputModel.SalesLedgerAccount = txtSalesLedgerCode.Text.Trim
            inputModel.OwningAgent = ddlOwningAgent.SelectedItem.Value.ToString()
            inputModel.AddressLine1 = txtAddressLine1.Text.Trim
            inputModel.AddressLine2 = txtAddressLine2.Text.Trim
            inputModel.AddressLine3 = txtAddressLine3.Text.Trim
            inputModel.County = txtCounty.Text.Trim
            If ModuleDefaults.StoreCountryAsWholeName Then
                inputModel.Country = ddlCountry.SelectedItem.Text
            Else
                inputModel.Country = ddlCountry.SelectedValue
            End If
            inputModel.PostCode = txtPostCode.Text.Trim
            inputModel.WebAddress = txtWebAddress.Text.Trim
            inputModel.VATCodeID = ddlVATCodes.SelectedValue.ToString().PadLeft(13, "0")
        End If


        'Determine the mode of the builder
        If IsPostBack Then
            ' We need to determine the which button was been pressed to cause the postback.
            If Not String.IsNullOrWhiteSpace(Request.Params(btnAdd.UniqueID)) Then
                inputModel.CompanyOperationMode = GlobalConstants.CRUDOperationMode.Create
            ElseIf Not String.IsNullOrWhiteSpace(Request.Params(btnUpdate.UniqueID)) Then
                inputModel.CompanyOperationMode = GlobalConstants.CRUDOperationMode.Update
            ElseIf Not String.IsNullOrWhiteSpace(Request.Params(btnParentRemove.UniqueID)) Then
                inputModel.CompanyOperationMode = GlobalConstants.CRUDOperationMode.Delete
            End If
        Else
            If _pageMode = Talent.Common.GlobalConstants.CRUDOperationMode.Create Then
                'Mode 'none' - simply calls the builder to populates the DDLs for the initial page load of the 
                'create function
                inputModel.CompanyOperationMode = GlobalConstants.CRUDOperationMode.None
            Else
                inputModel.CompanyOperationMode = GlobalConstants.CRUDOperationMode.Read
            End If
        End If

        Return inputModel
    End Function

    Private Sub processController(inputModel As CompanyUpdateInputModel)

        Dim companyUpdateBuilder As New CompanyModelBuilders()
        _viewModel = New CompanyUpdateViewModel(True)
        _viewModel = companyUpdateBuilder.CompanyUpdate(inputModel)

        ' Redirect after the builder is called.  This is similar to MVC where we would chose the correct view to display.
        If _viewModel.Error IsNot Nothing AndAlso _viewModel.Error.HasError Then
        Else
            If _viewModel.CompanyOperationMode = GlobalConstants.CRUDOperationMode.Create AndAlso IsAddingNewParent() AndAlso Not String.IsNullOrWhiteSpace(Request.QueryString("ChildCompanyNumber")) Then
                Session("SendParentAddMessage") = True
                Response.Redirect("~/PagesPublic/CRM/CompanyUpdate.aspx?CompanyNumber=" & Request.QueryString("ChildCompanyNumber") & "&CompanyUpdatePageMode=Update")
            ElseIf _viewModel.CompanyOperationMode = GlobalConstants.CRUDOperationMode.Create Then
                Response.Redirect("~/PagesPublic/CRM/CompanyUpdate.aspx?CompanyNumber=" & _viewModel.Company.CompanyNumber & "&CompanyUpdatePageMode=Update")
            ElseIf Request.QueryString("AddType") = "ExistingParent" Then
                Session("SendParentAddMessage") = True
                Response.Redirect("~/PagesPublic/CRM/CompanyUpdate.aspx?CompanyNumber=" & _viewModel.Company.CompanyID & "&CompanyUpdatePageMode=Update")
            End If
        End If

    End Sub

    Private Sub createView()

        ' We should only use a viewmodel from this point.  In MVC you pass the model into the view to render the relevant HTML.
        ' Keeping to this design will make sure that we can port these pages over to MVC much easier, as the business logic layer code is transferrable.
        ' I have declared the view model as class level variable.  This makes coding easier in a webForm project, as you may need to access it from protected 
        ' methods from the html.  

        'Populate the view 
        ProcessErrors()
        If plhCompanyDetailsForm.Visible Then
            PropulateSuccessMessage()
            PopulateText()
            PopulateAttributes()
            PopulateAgentDDList()
            PopulateVATCodeDDList()
            PopulateCountriesDropDownList()
            PopulateFieldsFromViewModel()
            SetParentFields()
        End If

    End Sub

#End Region

#Region "Presentation"

    Private Sub ProcessErrors()

        blErrorList.Items.Clear()
        If _viewModel.Error IsNot Nothing AndAlso _viewModel.Error.HasError Then
            'TODO - Add error message from the stored procedures
            blErrorList.Items.Add(_viewModel.Error.ErrorMessage)

            ' Is this a crtical error that we cannot continue from
            If String.IsNullOrWhiteSpace(_viewModel.Error.ReturnCode) Then
                plhCompanyDetailsForm.Visible = False
            End If

            ' We need to have a company when the mode is not 'none' or 'delete'
            If _viewModel.CompanyOperationMode <> Talent.Common.GlobalConstants.CRUDOperationMode.None AndAlso
                _viewModel.CompanyOperationMode <> Talent.Common.GlobalConstants.CRUDOperationMode.Delete Then
                plhCompanyDetailsForm.Visible = False
            End If
        End If
        plhErrorList.Visible = (blErrorList.Items.Count > 0)
    End Sub

    Private Sub PropulateSuccessMessage()
        ltlSuccess.Text = String.Empty
        If _viewModel.CompanyOperationMode = Talent.Common.GlobalConstants.CRUDOperationMode.Delete Then
            ltlSuccess.Text = _viewModel.GetPageText("ParentDeleteSuccessMessage")
        ElseIf Not String.IsNullOrWhiteSpace(Session("SendParentAddMessage")) AndAlso Session("SendParentAddMessage") = True Then
            ltlSuccess.Text = _viewModel.GetPageText("ParentAddSuccessMessage")
            ltlSuccess.Text = ltlSuccess.Text.Replace("<<ParentCompany>>", _viewModel.Company.ParentCompanyName)
            ltlSuccess.Text = ltlSuccess.Text.Replace("<<ChildCompany>>", _viewModel.Company.CompanyName)
            Session("SendParentAddMessage") = Nothing
        ElseIf _viewModel.CompanyOperationMode = Talent.Common.GlobalConstants.CRUDOperationMode.Update Then
            ltlSuccess.Text = _viewModel.GetPageText("CompanyUpdateSuccessMessage")
            ltlSuccess.Text = ltlSuccess.Text.Replace("<<Company>>", _viewModel.Company.CompanyName)
        ElseIf _viewModel.CompanyOperationMode = Talent.Common.GlobalConstants.CRUDOperationMode.Create Then
            ltlSuccess.Text = _viewModel.GetPageText("CompanyCreateSuccessMessage")
            ltlSuccess.Text = ltlSuccess.Text.Replace("<<Company>>", _viewModel.Company.CompanyName)
        End If

        plhSuccess.Visible = IIf(String.IsNullOrEmpty(ltlSuccess.Text), False, True)
    End Sub



    Private Sub PopulateAttributes()

        plhFindAddressButtonRow.Visible = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModel.GetPageAttribute("RapidAddressingEnabled"))
        lblContactByTelephoneNumber1.Text = Talent.eCommerce.Utilities.CheckForDBNull_String(_viewModel.GetPageText(lblContactByTelephoneNumber1.AssociatedControlID + "Text"))
        lblContactByTelephoneNumber2.Text = Talent.eCommerce.Utilities.CheckForDBNull_String(_viewModel.GetPageText(lblContactByTelephoneNumber2.AssociatedControlID + "Text"))
        lblContactByTelephoneNumber3.Text = Talent.eCommerce.Utilities.CheckForDBNull_String(_viewModel.GetPageText(lblContactByTelephoneNumber3.AssociatedControlID + "Text"))
        lblContactByWebAddress.Text = Talent.eCommerce.Utilities.CheckForDBNull_String(_viewModel.GetPageText(lblContactByWebAddress.AssociatedControlID + "Text"))

        ckContactByWebAddress.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_viewModel.GetPageAttribute("ContactByWebAddressEnabled"))
        ckContactByTelephoneNumber1.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_viewModel.GetPageAttribute("ContactByTelephoneNumber1Enabled"))
        ckContactByTelephoneNumber2.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_viewModel.GetPageAttribute("ContactByTelephoneNumber2Enabled"))
        ckContactByTelephoneNumber3.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_viewModel.GetPageAttribute("ContactByTelephoneNumber3Enabled"))
        lblContactByTelephoneNumber1.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_viewModel.GetPageAttribute("ContactByTelephoneNumber1Enabled"))
        lblContactByTelephoneNumber2.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_viewModel.GetPageAttribute("ContactByTelephoneNumber2Enabled"))
        lblContactByTelephoneNumber3.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_viewModel.GetPageAttribute("ContactByTelephoneNumber3Enabled"))
        lblContactByWebAddress.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_viewModel.GetPageAttribute("ContactByWebAddressEnabled"))

        If _companyNumber Is Nothing AndAlso _pageMode = Talent.Common.GlobalConstants.CRUDOperationMode.Create Then
            btnAdd.Visible = True
            If Not AgentProfile.AgentPermissions.CanAddCompany Then
                btnAdd.Visible = False
                displayOnlyForm()
                plhWarningList.Visible = True
                lblWarning.Text = _viewModel.GetPageText("AgentNotAuthorisedCompanyAdd")
            End If
        Else
            btnUpdate.Visible = True
            If Not AgentProfile.AgentPermissions.CanMaintainCompany Then
                btnUpdate.Visible = False
                displayOnlyForm()
                plhWarningList.Visible = True
                lblWarning.Text = _viewModel.GetPageText("AgentNotAuthorisedCompanyUpdate")
            End If
        End If

        hplSearch.Visible = True
        hplSearch.NavigateUrl = "~/PagesPublic/Profile/CustomerSelection.aspx?displayMode=ShowCompanySearch"
        hplSearch.Text = _viewModel.GetPageText("CompanySearchButtonText")
        hplBack.Visible = False
        If Not String.IsNullOrWhiteSpace(Request.QueryString("source")) Then
            If Request.QueryString("Source") = "Parent" Then
                hplBack.Visible = True
                hplBack.NavigateUrl = "~/PagesPublic/CRM/CompanyUpdate.aspx?CompanyUpdatePageMode=Update&CompanyNumber=" & Request.QueryString("ChildCompanyNumber")
                hplBack.Text = _viewModel.GetPageText("BackButtonText")
            End If
        End If

        hplContacts.Visible = False
        hplSubsidiary.Visible = False
        hplAddSubsidiary.Visible = False
        hplAdd.Visible = False
        If _pageMode = Talent.Common.GlobalConstants.CRUDOperationMode.Update Then
            hplAdd.NavigateUrl = "~/PagesPublic/CRM/CompanyUpdate.aspx?CompanyUpdatePageMode=Add&source=companysearch"
            hplAdd.Visible = True
            hplAdd.Text = _viewModel.GetPageText("AddCompanyButtonText")
            hplContacts.Visible = True
            hplContacts.Text = _viewModel.GetPageText("ContactsButtonText")
            hplContacts.NavigateUrl = "~/PagesPublic/CRM/CompanyContacts.aspx?CompanyNumber=" & _companyNumber & "&source=companysearch"
            If _viewModel.Company IsNot Nothing AndAlso _viewModel.Company.ParentFlag = "P" Then
                hplSubsidiary.Visible = True
                hplSubsidiary.Text = _viewModel.GetPageText("SubsidiaryButtonText")
                hplSubsidiary.NavigateUrl = "~/PagesPublic/Profile/CustomerSelection.aspx?displayMode=Subsidiaries&ParentCompanyNumber=" + _companyNumber + "&ParentCompanyName=" + _viewModel.Company.CompanyName.Trim + "&ChildCompanyNumber=" + _companyNumber
            Else
                hplAddSubsidiary.Visible = True
                hplAddSubsidiary.Text = _viewModel.GetPageText("AddSubsidiaryButtonText")
                hplAddSubsidiary.NavigateUrl = "~/PagesPublic/Profile/CustomerSelection.aspx?displayMode=AddSubsidiaries&companyColumnMode=SelectOnly&ParentCompanyNumber=" & _companyNumber
            End If
        End If

    End Sub

    Private Sub PopulateText()
        ltlCompanyAddressFormHeader.Text = _viewModel.GetPageText("CompanyAddressHeaderText")
        ltlCompanyDetailsFormHeader.Text = _viewModel.GetPageText("CompanyDetailsFormHeaderText")
        ltlCompanyName.Text = _viewModel.GetPageText("CompanyNameText")
        ltlWebAddress.Text = _viewModel.GetPageText("WebAddressText")
        ltlTelephoneNumber1.Text = _viewModel.GetPageText("TelephoneNumber1Text")
        ltlTelephoneNumber2.Text = _viewModel.GetPageText("TelephoneNumber2Text")
        ltlTelephoneNumber3.Text = _viewModel.GetPageText("TelephoneNumber3Text")
        ltlVATCode.Text = _viewModel.GetPageText("VATCodeText")
        ltlSalesLedgerCode.Text = _viewModel.GetPageText("SalesLedgerCodeText")
        ltlOwningAgent.Text = _viewModel.GetPageText("OwningAgentText")

        ltlAddressLine1.Text = _viewModel.GetPageText("AddressLine1Text")
        ltlAddressLine2.Text = _viewModel.GetPageText("AddressLine2Text")
        ltlAddressLine3.Text = _viewModel.GetPageText("AddressLine3Text")
        ltlCounty.Text = _viewModel.GetPageText("CountyText")
        ltlCountry.Text = _viewModel.GetPageText("CountryText")
        ltlPostCode.Text = _viewModel.GetPageText("PostCodeText")

        ltlParentCompanyHeader.Text = _viewModel.GetPageText("ParentCompanyHeaderText")
        hplParentAdd.Text = _viewModel.GetPageText("AddParentCompanyButtonText")
        hplParentSearch.Text = _viewModel.GetPageText("SearchParentCompanyButtonText")
        hplParentSubsidiaries.Text = _viewModel.GetPageText("SubsidiariesParentCompanyButtonText")
        btnParentRemove.Text = _viewModel.GetPageText("RemoveParentCompanyButtonText")
        hplParentChange.Text = _viewModel.GetPageText("ChangeParentCompanyButtonText")
        hplParentDetails.Text = _viewModel.GetPageText("DetailsParentCompanyButtonText")

        btnAdd.Text = _viewModel.GetPageText("AddButtonText")
        btnUpdate.Text = _viewModel.GetPageText("UpdateButtonText")

    End Sub


    Private Sub SetParentFields()

        'Set everything is off
        plhParentCompany.Visible = False
        hplParentChange.Visible = False
        btnParentRemove.Visible = False
        hplParentSubsidiaries.Visible = False
        hplParentAdd.Visible = False
        hplParentSearch.Visible = False
        ltlParentCompanyName.Text = String.Empty

        Dim parentCompanyNumber As String = String.Empty
        Dim parentCompanyName As String = String.Empty
        Dim parentFlag As String = "S"
        Dim companyNumber As String = String.Empty

        'Populate local variables from either the view model or the query string values
        If _viewModel.Company IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(_viewModel.Company.ParentCompanyNumber) Then
            parentCompanyNumber = _viewModel.Company.ParentCompanyNumber
            parentCompanyName = _viewModel.Company.ParentCompanyName
            parentFlag = _viewModel.Company.ParentFlag
            companyNumber = _viewModel.Company.CompanyNumber
        Else
            If Not String.IsNullOrWhiteSpace(Request.QueryString("ParentCompanyNumber")) Then
                parentCompanyNumber = Request.QueryString("ParentCompanyNumber")
            End If
            If Not String.IsNullOrWhiteSpace(Request.QueryString("ParentCompanyName")) Then
                parentCompanyName = Request.QueryString("ParentCompanyName")
            End If
            companyNumber = _companyNumber
        End If

        'Process the functionality of the parent control
        If parentFlag <> "P" AndAlso Request.QueryString("AddType") <> "NewParent" Then
            'Parent add to null functionality hacked to add subsidiaries in a similar way
            If Request.QueryString("AddType") = "AddToNullSubsidiaries" Then
                plhAddSubsidiary.Visible = True
                ltlAddSubsidiaryHeader.Text = "Subsidiary: "
                ltlAddSubsidiaryName.Text = Request.QueryString("SubsidiaryCompanyName")
                hplAddSubsidiary.Text = "Change Subsidiary"
            Else
                plhParentCompany.Visible = True
                ltlParentCompanyName.Text = parentCompanyName

                Dim searchUrl As String
                If String.IsNullOrEmpty(_companyNumber) Then
                    searchUrl = "~/PagesPublic/Profile/CustomerSelection.aspx?displayMode=AddToNull&companyColumnMode=SelectOnly"
                Else
                    searchUrl = "~/PagesPublic/Profile/CustomerSelection.aspx?displayMode=Parent&companyColumnMode=SelectOnly&childCompanyNumber=" + companyNumber
                End If

                hplParentSearch.NavigateUrl = searchUrl
                hplParentChange.NavigateUrl = searchUrl

                hplParentAdd.NavigateUrl = "~/PagesPublic/CRM/CompanyUpdate.aspx?CompanyUpdatePageMode=Add&source=Parent&AddType=NewParent&ChildCompanyNumber=" + companyNumber
                hplParentDetails.NavigateUrl = "~/PagesPublic/CRM/CompanyUpdate.aspx?CompanyNumber=" + parentCompanyNumber + "&CompanyUpdatePageMode=Update" + "&ChildCompanyNumber=" + companyNumber + "&source=Parent"

                ' Different functionality when the parent exists or not.
                If (String.IsNullOrEmpty(parentCompanyNumber) OrElse parentCompanyNumber = "0") OrElse
                        _viewModel.CompanyOperationMode = Talent.Common.GlobalConstants.CRUDOperationMode.Delete OrElse
                        _viewModel.CompanyOperationMode = Talent.Common.GlobalConstants.CRUDOperationMode.None Then
                    hplParentSearch.Visible = AgentProfile.AgentPermissions.CanMaintainCompany

                    'Add new parent is not available when we are in add mode ourselves
                    If _viewModel.CompanyOperationMode <> Talent.Common.GlobalConstants.CRUDOperationMode.None Then
                        hplParentAdd.Visible = AgentProfile.AgentPermissions.CanMaintainCompany
                    End If

                Else

                    hplParentSubsidiaries.NavigateUrl = "~/PagesPublic/Profile/CustomerSelection.aspx?displayMode=Subsidiaries&ParentCompanyNumber=" + parentCompanyNumber + "&ChildCompanyNumber=" + companyNumber
                    hplParentChange.Visible = AgentProfile.AgentPermissions.CanMaintainCompany
                    btnParentRemove.Visible = AgentProfile.AgentPermissions.CanMaintainCompany
                    hplParentSubsidiaries.Visible = True
                    hplParentDetails.Visible = True
                End If
            End If
        End If


    End Sub

    Private Sub PopulateSuccessMessage()
        If _viewModel.CompanyOperationMode = Talent.Common.GlobalConstants.CRUDOperationMode.Read Then

        End If
    End Sub

    Private Sub displayOnlyForm()
        For Each controlItem As Control In Page.Controls
            If TypeOf controlItem Is TextBox Then
                CType(controlItem, TextBox).Enabled = False
            End If
            If TypeOf controlItem Is DropDownList Then
                CType(controlItem, DropDownList).Enabled = False
            End If
            If TypeOf controlItem Is CheckBox Then
                CType(controlItem, CheckBox).Enabled = False
            End If
        Next
        plhFindAddressButtonRow.Visible = False
    End Sub

    Protected Sub SetupRequiredValidator(ByVal sender As Object, ByVal e As EventArgs)
        Dim genericRequiredFieldValidator As RequiredFieldValidator = CType(sender, RequiredFieldValidator)
        If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_viewModel.GetPageAttribute(genericRequiredFieldValidator.ControlToValidate & "RFVEnable")) Then
            genericRequiredFieldValidator.ErrorMessage = _viewModel.GetPageText(genericRequiredFieldValidator.ControlToValidate & "RequiredFieldValidator")

        Else
            genericRequiredFieldValidator.Enabled = False
            genericRequiredFieldValidator.Visible = False
        End If
    End Sub
    Protected Sub ShowPlaceHolder(ByVal sender As Object, ByVal e As EventArgs)
        Dim genericPlaceholoder As PlaceHolder = CType(sender, PlaceHolder)
        genericPlaceholoder.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_viewModel.GetPageAttribute(genericPlaceholoder.ClientID + "Enable"))
    End Sub
    Protected Sub SetupRegExValidator(ByVal sender As Object, ByVal e As EventArgs)
        Dim genericRegularExpressionValidator As RegularExpressionValidator = CType(sender, RegularExpressionValidator)
        If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse((_viewModel.GetPageAttribute(genericRegularExpressionValidator.ControlToValidate & "RegexEnable"))) Then
            genericRegularExpressionValidator.ValidationExpression = _viewModel.GetPageAttribute(genericRegularExpressionValidator.ControlToValidate & "RegexExpression")
            genericRegularExpressionValidator.ErrorMessage = _viewModel.GetPageText(genericRegularExpressionValidator.ControlToValidate & "RegexErrorText")
        Else
            genericRegularExpressionValidator.Enabled = False
            genericRegularExpressionValidator.Visible = False
        End If
    End Sub

    Private Sub PopulateFieldsFromViewModel()
        If _viewModel.Company IsNot Nothing Then
            txtCompanyName.Text = TrimWhenNotNull(_viewModel.Company.CompanyName)
            txtTelephoneNumber1.Text = TrimWhenNotNull(_viewModel.Company.TelephoneNumber1)
            txtTelephoneNumber2.Text = TrimWhenNotNull(_viewModel.Company.TelephoneNumber2)
            txtSalesLedgerCode.Text = TrimWhenNotNull(_viewModel.Company.SalesLedgerAccount)
            txtWebAddress.Text = TrimWhenNotNull(_viewModel.Company.WebAddress)
            txtAddressLine1.Text = TrimWhenNotNull(_viewModel.Company.AddressLine1)
            txtAddressLine2.Text = TrimWhenNotNull(_viewModel.Company.AddressLine2)
            txtAddressLine3.Text = TrimWhenNotNull(_viewModel.Company.AddressLine3)
            txtCounty.Text = TrimWhenNotNull(_viewModel.Company.County)
            ddlCountry.SelectedValue = FindByTextCaseInsensitive(ddlCountry, TrimWhenNotNull(_viewModel.Company.Country)).Value
            txtPostCode.Text = TrimWhenNotNull(_viewModel.Company.PostCode)
            ckContactByTelephoneNumber1.Checked = _viewModel.Company.Telephone1Use
            ckContactByTelephoneNumber2.Checked = _viewModel.Company.Telephone2Use
        End If
    End Sub

    Protected Sub CreateAddressingHiddenFields()
        '
        ' Create hidden fields for each Addressing field defined in defaults.
        Dim defaults As DefaultValues
        Dim defs As New ECommerceModuleDefaults
        Dim qasFields() As String = Nothing
        Dim count As Integer = 0
        Dim sString As String = String.Empty

        defaults = defs.GetDefaults

        If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModel.GetPageAttribute("RapidAddressingEnabled")) Then
            qasFields = defaults.AddressingFields.ToString.Split(",")
            Do While count < qasFields.Length
                If count = 0 Then
                    Response.Write(vbCrLf)
                End If
                sString = "<input type=""hidden"" name=""hiddenAdr" & count.ToString & """ value="" "" />"
                Response.Write(sString & vbCrLf)
                count = count + 1
            Loop
        End If
    End Sub

    Protected Function GetJavascriptString(ByVal sFieldString As String, ByVal sAddressingMap As String, ByVal sAddressingFields As String) As String
        Dim sString As String = String.Empty
        Dim count As Integer = 0
        Dim count2 As Integer = 0
        Const sStr1 As String = "document.forms[0].hiddenAdr"
        Const sStr2 As String = ".value"
        Const sStr3 As String = "usedHiddenAdr"
        Dim sAddressingMapFields() As String = sAddressingMap.ToString.Split(",")
        Dim sAddressingAllFields() As String = sAddressingFields.ToString.Split(",")

        Do While count < sAddressingMapFields.Length
            If Not sAddressingMapFields(count).Trim = "" Then
                count2 = 0
                Do While count2 < sAddressingAllFields.Length
                    If sAddressingMapFields(count).Trim = sAddressingAllFields(count2).Trim Then
                        sString = sString & vbCrLf & _
                                "if (trim(" & sStr3 & count2.ToString & ") != 'Y' && trim(" & sStr1 & count2.ToString & sStr2 & ") != '') {" & vbCrLf & _
                                "if (trim(" & sFieldString & sStr2 & ") == '' || " & sFieldString & " == document.forms[0]." & txtCounty.UniqueID & ") {" & vbCrLf & _
                                sFieldString & sStr2 & " = " & sStr1 & count2.ToString & sStr2 & ";" & vbCrLf & _
                                "}" & vbCrLf & _
                                "else {" & vbCrLf & _
                                sFieldString & sStr2 & " = " & sFieldString & sStr2 & " + ', ' + " & sStr1 & count2.ToString & sStr2 & ";" & vbCrLf & _
                                "}" & vbCrLf & _
                                sStr3 & count2.ToString & " = 'Y';" & vbCrLf & _
                                "}"
                        Exit Do
                    End If
                    count2 = count2 + 1
                Loop
            End If
            count = count + 1
        Loop
        Return sString
    End Function

    Protected Sub CreateAddressingJavascript()

        If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModel.GetPageAttribute("RapidAddressingEnabled")) Then
            Dim sString As String = String.Empty
            Dim sAllFields() As String = ModuleDefaults.AddressingFields.ToString.Split(",")
            Dim count As Integer = 0

            Response.Write(vbCrLf & "<script language=""javascript"" type=""text/javascript"">" & vbCrLf)
            Select Case ModuleDefaults.AddressingProvider.ToUpper
                Case Is = "QAS"
                    ' Create function to open child window
                    Response.Write("function addressingPopup() {" & vbCrLf)
                    Response.Write("win1 = window.open('../../PagesPublic/QAS/FlatCountry.aspx', 'QAS', '" & _viewModel.GetPageAttribute("addressingWindowAttributes") & "');" & vbCrLf)
                    Response.Write("win1.creator=self;" & vbCrLf)
                    Response.Write("}" & vbCrLf)
                Case Is = "HOPEWISER"
                    ' Create function to open child window
                    Response.Write("function addressingPopup() {" & vbCrLf)
                    Response.Write("win1 = window.open('../../PagesPublic/Hopewiser/HopewiserPostcodeAndCountry.aspx', 'Hopewiser', '" & _viewModel.GetPageAttribute("addressingWindowAttributes") & "');" & vbCrLf)
                    Response.Write("win1.creator=self;" & vbCrLf)
                    Response.Write("}" & vbCrLf)
            End Select

            ' Create function to populate the address fields.  This function is called from FlatAddress.aspx.
            Response.Write("function UpdateAddressFields() {" & vbCrLf)

            ' Create local function variables used to indicate whether an address element has already been used.
            Do While count < sAllFields.Length
                Response.Write("var usedHiddenAdr" & count.ToString & " = '';" & vbCrLf)
                count = count + 1
            Loop

            ' Clear all address fields
            If plhAddressLine1.Visible Then Response.Write("document.forms[0]." & txtAddressLine1.UniqueID & ".value = '';" & vbCrLf)
            If plhAddressLine2.Visible Then Response.Write("document.forms[0]." & txtAddressLine2.UniqueID & ".value = '';" & vbCrLf)

            If plhAddressLine3.Visible Then Response.Write("document.forms[0]." & txtAddressLine3.UniqueID & ".value = '';" & vbCrLf)
            If plhCounty.Visible Then Response.Write("document.forms[0]." & txtCounty.UniqueID & ".value = '';" & vbCrLf)
            If plhPostCode.Visible Then Response.Write("document.forms[0]." & txtPostCode.UniqueID & ".value = '';" & vbCrLf)

            ' If an address field is in use and is defined to contain a QAS address element then create Javascript code to populate correctly.
            If plhAddressLine1.Visible And Not ModuleDefaults.AddressingMapAdr2.Trim = "" Then
                sString = GetJavascriptString("document.forms[0]." & txtAddressLine1.UniqueID, ModuleDefaults.AddressingMapAdr2, ModuleDefaults.AddressingFields)
                Response.Write(sString)
            End If
            If plhAddressLine2.Visible And Not ModuleDefaults.AddressingMapAdr3.Trim = "" Then
                sString = GetJavascriptString("document.forms[0]." & txtAddressLine2.UniqueID, ModuleDefaults.AddressingMapAdr3, ModuleDefaults.AddressingFields)
                Response.Write(sString)
            End If
            If plhAddressLine3.Visible And Not ModuleDefaults.AddressingMapAdr4.Trim = "" Then
                sString = GetJavascriptString("document.forms[0]." & txtAddressLine3.UniqueID, ModuleDefaults.AddressingMapAdr4, ModuleDefaults.AddressingFields)
                Response.Write(sString)
            End If
            If plhCounty.Visible And Not ModuleDefaults.AddressingMapAdr5.Trim = "" Then
                sString = GetJavascriptString("document.forms[0]." & txtCounty.UniqueID, ModuleDefaults.AddressingMapAdr5, ModuleDefaults.AddressingFields)
                Response.Write(sString)
            End If
            If plhPostCode.Visible And Not ModuleDefaults.AddressingMapPost.Trim = "" Then
                sString = GetJavascriptString("document.forms[0]." & txtPostCode.UniqueID, ModuleDefaults.AddressingMapPost, ModuleDefaults.AddressingFields)
                Response.Write(sString)
            End If

            Response.Write("}" & vbCrLf)
            Response.Write("function trim(s) { " & vbCrLf & "var r=/\b(.*)\b/.exec(s); " & vbCrLf & "return (r==null)?"""":r[1]; " & vbCrLf & "}")
            Response.Write("</script>" & vbCrLf)
        End If
    End Sub

    Protected Function GetAddressingLinkText() As String
        Return _viewModel.GetPageText("RapidAddressingLinkButtonText")
    End Function

    Protected Sub PopulateCountriesDropDownList()
        ddlCountry.DataSource = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, "REGISTRATION", "COUNTRY")
        ddlCountry.DataTextField = "Text"
        ddlCountry.DataValueField = "Value"
        ddlCountry.DataBind()
        Dim defaultCountry As String = String.Empty
        'Select the default country
        If ModuleDefaults.UseDefaultCountryOnRegistration Then
            defaultCountry = TalentCache.GetDefaultCountryForBU()
            ddlCountry.SelectedValue = defaultCountry
        End If
    End Sub

    Private Sub PopulateVATCodeDDList()
        ddlVATCodes.DataSource = _viewModel.VatCodeList
        ddlVATCodes.DataTextField = "VATCode"
        ddlVATCodes.DataValueField = "VATUniqueID"
        ddlVATCodes.DataBind()

        ' Select the item in the drop down lists to match the returning company information
        If ddlVATCodes.Items.Count > 0 AndAlso _viewModel.Company IsNot Nothing Then
            If ddlVATCodes.Items.FindByValue(_viewModel.Company.VatCodeId) IsNot Nothing Then
                ddlVATCodes.Items.FindByValue(_viewModel.Company.VatCodeId).Selected = True
                ddlVATCodes.Items.FindByValue(0).Selected = False
            End If
        End If

    End Sub

    Private Sub PopulateAgentDDList()
        ddlOwningAgent.DataSource = _viewModel.AgentList
        ddlOwningAgent.DataTextField = "Username"
        ddlOwningAgent.DataValueField = "UserCode"
        ddlOwningAgent.DataBind()

        'Default 'No Owning Agent' Value is blank
        ddlOwningAgent.Items(0).Value = ""

        ' Select the item in the drop down lists to match the returning company information.
        ' If in create mode, default the owner to the logged in agent
        If ddlOwningAgent.Items.Count > 0 Then
            If _viewModel.CompanyOperationMode = Talent.Common.GlobalConstants.CRUDOperationMode.None Then
                If ddlOwningAgent.Items.FindByValue(AgentProfile.Name) IsNot Nothing Then
                    ddlOwningAgent.SelectedValue = ddlOwningAgent.Items.FindByValue(AgentProfile.Name).Value
                    ddlOwningAgent.Items(0).Selected = False
                End If
            ElseIf _viewModel.Company IsNot Nothing Then
                If ddlOwningAgent.Items.FindByValue(_viewModel.Company.OwningAgent) IsNot Nothing Then
                    ddlOwningAgent.SelectedValue = ddlOwningAgent.Items.FindByValue(_viewModel.Company.OwningAgent).Value
                    ddlOwningAgent.Items(0).Selected = False
                End If
            End If
        End If

    End Sub

#End Region

#Region "Private Function"
    Private Function FindByTextCaseInsensitive(ByVal ctl As ListControl, ByVal text As String) As ListItem
        If ctl Is Nothing Then Return Nothing
        If String.IsNullOrEmpty(text) Then
            text = " -- "
            Return ctl.Items.FindByText(text)
        Else ''If Not ctl.Items.Contains(New ListItem(text.ToString(), text.ToUpper().ToString())) Then
            Dim found As Boolean = False
            For Each li As ListItem In ctl.Items
                If String.Compare(li.Text.Trim(), text, True) = 0 Then
                    found = True
                    Return li
                End If
            Next

            If Not found Then
                text = " -- "
                Return ctl.Items.FindByText(text)
            End If
        End If
        'For Each li As ListItem In ctl.Items
        '    If String.Compare(li.Text.Trim(), text, True) = 0 Then Return li
        'Next
        Return Nothing
    End Function
#End Region


End Class
