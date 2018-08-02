Imports Talent.eCommerce
Imports TalentBusinessLogic.ModelBuilders.Setup.Templates
Imports TalentBusinessLogic.Models.Setup.Template
Imports System.Collections.Generic
Imports TalentBusinessLogic.DataTransferObjects.Setup.Template
Imports System.Linq
Imports Talent.Common
Imports System.Data

Partial Class PagesAgent_Setup_TemplateOverride
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _viewModelTemplateOverride As TemplateOverrideViewModel = Nothing
    Private _templateOverrideBuilder As TemplateOverrideBuilder = Nothing

#End Region

#Region "Public Properties"
    ''' <summary>
    ''' Template override name column heading
    ''' </summary>   
    Public TemplateNameColumnHeading As String

    ''' <summary>
    ''' Template override criteria column heading
    ''' </summary>
    Public TemplateCriteriaColumnHeading As String

    ''' <summary>
    ''' Template column heading
    ''' </summary>
    Public TemplateColumnHeading As String

    ''' <summary>
    ''' Product package specific text for "Yes"
    ''' </summary>
    Public ProductPackageSpecificYesText As String

    ''' <summary>
    ''' Product package specific text for "No"
    ''' </summary>
    Public ProductPackageSpecificNoText As String

    ''' <summary>
    ''' Product specific text for switch 
    ''' </summary>
    Public ProductSpecificText As String

    ''' <summary>
    ''' Cancel button text
    ''' </summary>
    Public CancelButtonText As String


#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Dim businessUnit As String = String.Empty
        Dim partner As String = String.Empty
        Dim defaults As ECommerceModuleDefaults.DefaultValues = Nothing
        Dim moduleDefaults As ECommerceModuleDefaults = Nothing
        Dim inputModelTemplateOverride As TemplateOverrideInputModel
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "template-override.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("template-override.js", "/Module/Setup/Template/"), False)
        If AgentProfile.IsAgent Then
            businessUnit = TalentCache.GetBusinessUnit()
            partner = TalentCache.GetPartner(HttpContext.Current.Profile)
            moduleDefaults = New ECommerceModuleDefaults
            defaults = moduleDefaults.GetDefaults()
            _templateOverrideBuilder = New TemplateOverrideBuilder
            populateBusinessUnits()
            populateOverrideCriterias()
            If Not IsPostBack Then
                If Not String.IsNullOrEmpty(HttpContext.Current.Session("SuccessMessage")) Or Not String.IsNullOrEmpty(HttpContext.Current.Session("ErrorMessage")) Then
                    ltlCreateTemplateOverrideErrors.Text = HttpContext.Current.Session("ErrorMessage")
                    ltlSuccessMessage.Text = HttpContext.Current.Session("SuccessMessage")
                    plhTemplateOverrideErrors.Visible = (ltlTemplateOverrideErrors.Text.Length > 0)
                    plhSuccessMessage.Visible = (ltlSuccessMessage.Text.Length > 0)
                    HttpContext.Current.Session("SuccessMessage") = Nothing
                    HttpContext.Current.Session("ErrorMessage") = Nothing

                    ddlBusinessUnit.SelectedValue = Session("TemplateOverrideBusinessUnit")
                    inputModelTemplateOverride = setupTemplateOverrideInputModel()
                    populateEmailConfirmationDropDownList(inputModelTemplateOverride)
                    populateQAndADropDownList(inputModelTemplateOverride)
                    populateDataCaptureDropDownList(inputModelTemplateOverride)
                    processController(inputModelTemplateOverride)
                    createView()
                ElseIf Not String.IsNullOrEmpty(Session("TemplateOverrideBusinessUnit")) Then
                    ddlBusinessUnit.SelectedValue = Session("TemplateOverrideBusinessUnit")
                    inputModelTemplateOverride = setupTemplateOverrideInputModel()
                    populateEmailConfirmationDropDownList(inputModelTemplateOverride)
                    populateQAndADropDownList(inputModelTemplateOverride)
                    populateDataCaptureDropDownList(inputModelTemplateOverride)
                    processController(inputModelTemplateOverride)
                    createView()
                Else
                    setPageLabels()
                    setHiddenFields()
                    plhCreateTemplateOverrideErrors.Visible = False
                    plhSuccessMessage.Visible = False
                End If
                If ddlBusinessUnit.SelectedItem.Text <> _viewModelTemplateOverride.GetPageText("SelectBusinessUnitText") Then
                    plhOpenCreateTemplateOverride.Visible = True
                Else
                    plhOpenCreateTemplateOverride.Visible = False
                End If
            End If
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        plhTemplateOverrideErrors.Visible = (ltlTemplateOverrideErrors.Text.Length > 0)
        plhTemplateListWarning.Visible = (ltlTemplateListWarning.Text.Length > 0)
        plhTemplateoverrides.Visible = (Not plhTemplateListWarning.Visible And Not plhTemplateOverrideErrors.Visible) And (plhOpenCreateTemplateOverride.Visible)
        plhSuccessMessage.Visible = (ltlSuccessMessage.Text.Length > 0)
    End Sub

    Protected Sub rptTemplateOverrides_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptTemplateOverrides.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim rptOverrideCriteria As Repeater = CType(e.Item.FindControl("rptOverrideCriteria"), Repeater)
            Dim templateOverride As TemplateOverrideHeader = CType(e.Item.DataItem, TemplateOverrideHeader)
            Dim overrideCriterias As List(Of TemplateOverrideCriteriaFormatted) = templateOverride.TemplateOverrideCriteriasFormatted
            Dim plhEmailTemplate As PlaceHolder = CType(e.Item.FindControl("plhEmailTemplate"), PlaceHolder)
            Dim plhQandATemplate As PlaceHolder = CType(e.Item.FindControl("plhQandATemplate"), PlaceHolder)
            Dim plhDataCaptureTemplate As PlaceHolder = CType(e.Item.FindControl("plhDataCaptureTemplate"), PlaceHolder)

            If String.IsNullOrEmpty(templateOverride.SaleConfirmationEmailDescription.Trim) Then
                plhEmailTemplate.Visible = False
            End If
            If String.IsNullOrEmpty(templateOverride.QAndATemplateDescription.Trim) Then
                plhQandATemplate.Visible = False
            End If
            If String.IsNullOrEmpty(templateOverride.DataCaptureTemplateDescription.Trim) Then
                plhDataCaptureTemplate.Visible = False
            End If
            setRowDataTitles(e)
            rptOverrideCriteria.DataSource = overrideCriterias
            rptOverrideCriteria.DataBind()
        End If
    End Sub

    Protected Sub ddlBusinessUnit_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlBusinessUnit.SelectedIndexChanged
        Session("TemplateOverrideBusinessUnit") = ddlBusinessUnit.SelectedItem.Value
        If ddlBusinessUnit.SelectedItem.Text = _viewModelTemplateOverride.GetPageText("SelectBusinessUnitText") Then
            plhOpenCreateTemplateOverride.Visible = False
            plhTemplateoverrides.Visible = False
            ltlTemplateListWarning.Text = String.Empty
            ltlTemplateOverrideErrors.Text = String.Empty
            ltlSuccessMessage.Text = String.Empty
            setPageLabels()
            rptTemplateOverrides.DataSource = New DataTable
            rptTemplateOverrides.DataBind()
        Else
            plhOpenCreateTemplateOverride.Visible = True
            plhTemplateoverrides.Visible = True
            Dim inputModelTemplateOverride As TemplateOverrideInputModel = setupTemplateOverrideInputModel()
            populateEmailConfirmationDropDownList(inputModelTemplateOverride)
            populateQAndADropDownList(inputModelTemplateOverride)
            populateDataCaptureDropDownList(inputModelTemplateOverride)
            processController(inputModelTemplateOverride)
            createView()
            clearControls()
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Dim inputModelTemplateOverride As TemplateOverrideInputModel = setupTemplateOverrideInputModel()
        If hdfTemplateOverrideType.Value = "Ticketing" Then
            inputModelTemplateOverride.TemplateOverrideCriterias = New List(Of TemplateOverrideCriteria)

            For Each item As ListItem In ddlProductSubType.Items
                If item.Selected Then
                    Dim subType As TemplateOverrideCriteria = New TemplateOverrideCriteria
                    subType.CriteriaType = GlobalConstants.TEMPLATE_OVERRIDE_CRITERIA_SUBTYPE
                    subType.CriteriaValue = item.Value
                    inputModelTemplateOverride.TemplateOverrideCriterias.Add(subType)
                End If
            Next

            For Each item As ListItem In ddlProductType.Items
                If item.Selected Then
                    Dim productType As TemplateOverrideCriteria = New TemplateOverrideCriteria
                    productType.CriteriaType = GlobalConstants.TEMPLATE_OVERRIDE_CRITERIA_PRODUCTTYPE
                    productType.CriteriaValue = item.Value
                    inputModelTemplateOverride.TemplateOverrideCriterias.Add(productType)
                End If
            Next

            For Each item As ListItem In ddlStadium.Items
                If item.Selected Then
                    Dim stadium As TemplateOverrideCriteria = New TemplateOverrideCriteria
                    stadium.CriteriaType = GlobalConstants.TEMPLATE_OVERRIDE_CRITERIA_STADIUM
                    stadium.CriteriaValue = item.Value
                    inputModelTemplateOverride.TemplateOverrideCriterias.Add(stadium)
                End If
            Next
        End If

        If hdfTemplateOverrideType.Value = "Package" Then
            Dim listOfProduct As List(Of String) = New List(Of String)
            Dim listOfPackage As List(Of String) = New List(Of String)
            inputModelTemplateOverride = New TemplateOverrideInputModel
            inputModelTemplateOverride.TemplateOverrideCriterias = New List(Of TemplateOverrideCriteria)

            For Each item As ListItem In ddlProductGroup.Items
                If item.Selected Then
                    Dim product As TemplateOverrideCriteria = New TemplateOverrideCriteria
                    product.CriteriaType = GlobalConstants.TEMPLATE_OVERRIDE_CRITERIA_PRODUCTCODE
                    product.CriteriaValue = item.Value
                    inputModelTemplateOverride.TemplateOverrideCriterias.Add(product)
                    listOfProduct.Add(item.Value)
                End If
            Next

            For Each item As ListItem In ddlPackage.Items
                If item.Selected Then
                    Dim package As TemplateOverrideCriteria = New TemplateOverrideCriteria
                    package.CriteriaType = GlobalConstants.TEMPLATE_OVERRIDE_CRITERIA_PACKAGE
                    package.CriteriaValue = item.Value
                    inputModelTemplateOverride.TemplateOverrideCriterias.Add(package)
                    listOfPackage.Add(item.Value)
                End If
            Next
        End If

        inputModelTemplateOverride.Mode = GlobalConstants.TEMPLATE_OVERRIDE_CREATE_MODE
        inputModelTemplateOverride.BusinessUnit = ddlBusinessUnit.SelectedValue
        inputModelTemplateOverride.TemplateOverrideID = 0
        inputModelTemplateOverride.Description = txtTemplateOverrideDescription.Text
        inputModelTemplateOverride.BoxOfficeUser = AgentProfile.Name

        If String.IsNullOrEmpty(ddlEmailConfirmation.SelectedValue) Then
            inputModelTemplateOverride.SaleConfirmationEmailID = 0
            inputModelTemplateOverride.SaleConfirmationEmailDescription = String.Empty
        Else
            inputModelTemplateOverride.SaleConfirmationEmailID = ddlEmailConfirmation.SelectedValue
            inputModelTemplateOverride.SaleConfirmationEmailDescription = ddlEmailConfirmation.SelectedItem.Text
        End If

        If String.IsNullOrEmpty(ddlQAndATemplate.SelectedValue) Then
            inputModelTemplateOverride.QAndATemplateID = 0
            inputModelTemplateOverride.QAndATemplateDescription = String.Empty
        Else
            inputModelTemplateOverride.QAndATemplateID = ddlQAndATemplate.SelectedValue
            inputModelTemplateOverride.QAndATemplateDescription = ddlQAndATemplate.SelectedItem.Text
        End If

        If String.IsNullOrEmpty(ddlDataCaptureTemplate.SelectedValue) Then
            inputModelTemplateOverride.DataCaptureTemplateID = 0
            inputModelTemplateOverride.DataCaptureTemplateDescription = String.Empty
        Else
            inputModelTemplateOverride.DataCaptureTemplateID = ddlDataCaptureTemplate.SelectedValue
            inputModelTemplateOverride.DataCaptureTemplateDescription = ddlDataCaptureTemplate.SelectedItem.Text
        End If
        inputModelTemplateOverride.AutoExpandQAndA = 0
        _viewModelTemplateOverride = _templateOverrideBuilder.CreateTemplateOverride(inputModelTemplateOverride)
        If _viewModelTemplateOverride.Error.HasError Then
            ltlTemplateOverrideErrors.Text = _viewModelTemplateOverride.Error.ErrorMessage
            ltlTemplateListWarning.Text = String.Empty
            ltlSuccessMessage.Text = String.Empty
        Else
            rptTemplateOverrides.DataSource = _viewModelTemplateOverride.TemplateOverrideList
            rptTemplateOverrides.DataBind()
            setPageLabels()
            clearControls()
            ltlSuccessMessage.Text = _viewModelTemplateOverride.GetPageText("CreateTemplateOverrideSuccessMessage")
        End If
        HttpContext.Current.Session("SuccessMessage") = ltlSuccessMessage.Text
        HttpContext.Current.Session("ErrorMessage") = ltlTemplateOverrideErrors.Text
        Response.Redirect(Request.Url.AbsoluteUri)
    End Sub

    Protected Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Dim inputModelTemplateOverride As TemplateOverrideInputModel = setupTemplateOverrideInputModel()
        If hdfTemplateOverrideType.Value = "Ticketing" Then
            inputModelTemplateOverride.TemplateOverrideCriterias = New List(Of TemplateOverrideCriteria)

            For Each item As ListItem In ddlProductSubTypeForEdit.Items
                If item.Selected Then
                    Dim subType As TemplateOverrideCriteria = New TemplateOverrideCriteria
                    subType.CriteriaType = GlobalConstants.TEMPLATE_OVERRIDE_CRITERIA_SUBTYPE
                    subType.CriteriaValue = item.Value
                    inputModelTemplateOverride.TemplateOverrideCriterias.Add(subType)
                End If
            Next

            For Each item As ListItem In ddlProductTypeForEdit.Items
                If item.Selected Then
                    Dim productType As TemplateOverrideCriteria = New TemplateOverrideCriteria
                    productType.CriteriaType = GlobalConstants.TEMPLATE_OVERRIDE_CRITERIA_PRODUCTTYPE
                    productType.CriteriaValue = item.Value
                    inputModelTemplateOverride.TemplateOverrideCriterias.Add(productType)
                End If
            Next

            For Each item As ListItem In ddlStadiumForEdit.Items
                If item.Selected Then
                    Dim stadium As TemplateOverrideCriteria = New TemplateOverrideCriteria
                    stadium.CriteriaType = GlobalConstants.TEMPLATE_OVERRIDE_CRITERIA_STADIUM
                    stadium.CriteriaValue = item.Value
                    inputModelTemplateOverride.TemplateOverrideCriterias.Add(stadium)
                End If
            Next
        End If

        If hdfTemplateOverrideType.Value = "Package" Then
            inputModelTemplateOverride = New TemplateOverrideInputModel
            inputModelTemplateOverride.TemplateOverrideCriterias = New List(Of TemplateOverrideCriteria)

            For Each item As ListItem In ddlProductGroupForEdit.Items
                If item.Selected Then
                    Dim product As TemplateOverrideCriteria = New TemplateOverrideCriteria
                    product.CriteriaType = GlobalConstants.TEMPLATE_OVERRIDE_CRITERIA_PRODUCTCODE
                    product.CriteriaValue = item.Value
                    inputModelTemplateOverride.TemplateOverrideCriterias.Add(product)
                End If
            Next

            For Each item As ListItem In ddlPackageForEdit.Items
                If item.Selected Then
                    Dim package As TemplateOverrideCriteria = New TemplateOverrideCriteria
                    package.CriteriaType = GlobalConstants.TEMPLATE_OVERRIDE_CRITERIA_PACKAGE
                    package.CriteriaValue = item.Value
                    inputModelTemplateOverride.TemplateOverrideCriterias.Add(package)
                End If
            Next
        End If

        inputModelTemplateOverride.Mode = GlobalConstants.TEMPLATE_OVERRIDE_UPDATE_MODE
        inputModelTemplateOverride.BusinessUnit = ddlBusinessUnit.SelectedValue
        inputModelTemplateOverride.TemplateOverrideID = Convert.ToDecimal(hdfTemplateOverrideId.Value)
        inputModelTemplateOverride.Description = txtTemplateDescriptionForEdit.Text.Trim
        inputModelTemplateOverride.BoxOfficeUser = AgentProfile.Name

        If String.IsNullOrEmpty(ddlEmailTemplateForEdit.SelectedValue) Then
            inputModelTemplateOverride.SaleConfirmationEmailID = 0
            inputModelTemplateOverride.SaleConfirmationEmailDescription = String.Empty
        Else
            inputModelTemplateOverride.SaleConfirmationEmailID = ddlEmailTemplateForEdit.SelectedValue
            inputModelTemplateOverride.SaleConfirmationEmailDescription = ddlEmailTemplateForEdit.SelectedItem.Text
        End If

        If String.IsNullOrEmpty(ddlQandATemplateForEdit.SelectedValue) Then
            inputModelTemplateOverride.QAndATemplateID = 0
            inputModelTemplateOverride.QAndATemplateDescription = String.Empty
        Else
            inputModelTemplateOverride.QAndATemplateID = ddlQandATemplateForEdit.SelectedValue
            inputModelTemplateOverride.QAndATemplateDescription = ddlQandATemplateForEdit.SelectedItem.Text
        End If

        If String.IsNullOrEmpty(ddlDataCaptureTemplateForEdit.SelectedValue) Then
            inputModelTemplateOverride.DataCaptureTemplateID = 0
            inputModelTemplateOverride.DataCaptureTemplateDescription = String.Empty
        Else
            inputModelTemplateOverride.DataCaptureTemplateID = ddlDataCaptureTemplateForEdit.SelectedValue
            inputModelTemplateOverride.DataCaptureTemplateDescription = ddlDataCaptureTemplateForEdit.SelectedItem.Text
        End If
        inputModelTemplateOverride.AutoExpandQAndA = 0
        _viewModelTemplateOverride = _templateOverrideBuilder.UpdateTemplateOverride(inputModelTemplateOverride)
        If _viewModelTemplateOverride.Error.HasError Then
            ltlTemplateOverrideErrors.Text = _viewModelTemplateOverride.Error.ErrorMessage
            ltlTemplateListWarning.Text = String.Empty
            ltlSuccessMessage.Text = String.Empty
        Else
            rptTemplateOverrides.DataSource = _viewModelTemplateOverride.TemplateOverrideList
            rptTemplateOverrides.DataBind()
            setPageLabels()
            ltlSuccessMessage.Text = _viewModelTemplateOverride.GetPageText("UpdateTemplateOverrideSuccessMessage")
        End If
        HttpContext.Current.Session("SuccessMessage") = ltlSuccessMessage.Text
        HttpContext.Current.Session("ErrorMessage") = ltlTemplateOverrideErrors.Text
        Response.Redirect(Request.Url.AbsoluteUri)
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim viewModelTemplateOverride As TemplateOverrideViewModel = Nothing
        Dim inputModelTemplateOverride As TemplateOverrideInputModel
        inputModelTemplateOverride = setupTemplateOverrideInputModel()
        inputModelTemplateOverride.TemplateOverrideID = Convert.ToDecimal(hdfTemplateOverrideId.Value)
        inputModelTemplateOverride.BoxOfficeUser = AgentProfile.Name
        viewModelTemplateOverride = _templateOverrideBuilder.DeleteTemplateOverride(inputModelTemplateOverride)

        If _viewModelTemplateOverride.Error.HasError Then
            ltlTemplateOverrideErrors.Text = _viewModelTemplateOverride.Error.ErrorMessage
            ltlTemplateListWarning.Text = String.Empty
            ltlSuccessMessage.Text = String.Empty
        Else
            rptTemplateOverrides.DataSource = _viewModelTemplateOverride.TemplateOverrideList
            rptTemplateOverrides.DataBind()
            setPageLabels()
            clearControls()
            ltlSuccessMessage.Text = _viewModelTemplateOverride.GetPageText("DeleteTemplateOverrideSuccessMessage")
        End If
        HttpContext.Current.Session("SuccessMessage") = ltlSuccessMessage.Text
        HttpContext.Current.Session("ErrorMessage") = ltlTemplateOverrideErrors.Text
        Response.Redirect(Request.Url.AbsoluteUri)
    End Sub

#End Region

#Region "Private Functions"


    ''' <summary>
    ''' Setup the template override input model and return the model for use
    ''' </summary>
    ''' <returns>The formatted input model based on form data</returns>
    ''' <remarks></remarks>
    Private Function setupTemplateOverrideInputModel() As TemplateOverrideInputModel
        Dim inputModel As New TemplateOverrideInputModel
        inputModel.BusinessUnit = ddlBusinessUnit.SelectedValue
        Return inputModel
    End Function

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Process the controller, the code actioning the input values if there are any.
    ''' </summary>
    ''' <param name="inputModelTemplateOverride">Template Override Input Model</param>
    ''' <remarks></remarks>
    Private Sub processController(ByVal inputModelTemplateOverride As TemplateOverrideInputModel)
        _viewModelTemplateOverride = _templateOverrideBuilder.RetrieveTemplateOverrideList(inputModelTemplateOverride)
    End Sub

    ''' <summary>
    ''' Create the booking page view
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub createView()
        If _viewModelTemplateOverride.Error.HasError Then
            If _viewModelTemplateOverride.Error.ReturnCode = "NT" Then
                ltlTemplateListWarning.Text = _viewModelTemplateOverride.Error.ErrorMessage & _viewModelTemplateOverride.GetPageText("NoTemplateMessageAddition")
                ltlTemplateOverrideErrors.Text = String.Empty
            Else
                ltlTemplateOverrideErrors.Text = _viewModelTemplateOverride.Error.ErrorMessage
                ltlTemplateListWarning.Text = String.Empty
            End If
        Else
            ltlTemplateListWarning.Text = String.Empty
            ltlTemplateOverrideErrors.Text = String.Empty
            setColumnHeadingsAndVisibility()
            rptTemplateOverrides.DataSource = _viewModelTemplateOverride.TemplateOverrideList
            rptTemplateOverrides.DataBind()
        End If
        setPageLabels()
        setHiddenFields()
        plhCreateTemplateOverrideErrors.Visible = False
    End Sub

    ''' <summary>
    ''' Setup the various text labels on the screen based on the view model
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setPageLabels()
        lblBusinessUnit.Text = _viewModelTemplateOverride.GetPageText("BusinessUnitLabelText")
        ltlAddTemplateOverride.Text = _viewModelTemplateOverride.GetPageText("AddTemplateOverrideButtonText")
        ltlTemplateOverrideHeader.Text = _viewModelTemplateOverride.GetPageText("TemplateOverrideHeaderText")
        lblTemplateOverrideDescription.Text = _viewModelTemplateOverride.GetPageText("TemplateOverrideNameLabelText")
        txtTemplateOverrideDescription.Attributes.Add("PlaceHolder", _viewModelTemplateOverride.GetPageText("TemplateOverrideNamePlaceholder"))
        lblProductPackageSpecific.Text = _viewModelTemplateOverride.GetPageText("ProductPackageSpecificLabelText")
        lblProductGroup.Text = _viewModelTemplateOverride.GetPageText("ProductLabelText")
        lblPackage.Text = _viewModelTemplateOverride.GetPageText("PackageLabelText")
        lblProductSubType.Text = _viewModelTemplateOverride.GetPageText("ProductSubTypeLabelText")
        lblProductType.Text = _viewModelTemplateOverride.GetPageText("ProductTypeLabelText")
        lblStadium.Text = _viewModelTemplateOverride.GetPageText("StadiumLabelText")
        lblEmailConfirmation.Text = _viewModelTemplateOverride.GetPageText("EmailConfirmationTemplateDDLLabel")
        lblQAndATemplate.Text = _viewModelTemplateOverride.GetPageText("QandATemplateDDLLabel")
        lblDataCaptureTemplateLabel.Text = _viewModelTemplateOverride.GetPageText("DataCaptureTemplateDDLLabel")
        ProductPackageSpecificYesText = _viewModelTemplateOverride.GetPageText("ProductPackageSpecificSwitchYesText")
        ProductPackageSpecificNoText = _viewModelTemplateOverride.GetPageText("ProductPackageSpecificSwitchNoText")
        btnAdd.Text = _viewModelTemplateOverride.GetPageText("AddButtonText")
        ProductSpecificText = _viewModelTemplateOverride.GetPageText("ProductSpecificText")
        CancelButtonText = _viewModelTemplateOverride.GetPageText("CancelButtonText")

        'set the labels for Edit Modal
        ltlTemplateOverrideForEdit.Text = _viewModelTemplateOverride.GetPageText("TemplateOverrideHeaderText")
        lblTemplateDescriptionForEdit.Text = _viewModelTemplateOverride.GetPageText("TemplateOverrideNameLabelText")
        txtTemplateDescriptionForEdit.Attributes.Add("PlaceHolder", _viewModelTemplateOverride.GetPageText("TemplateOverrideNamePlaceholder"))
        lblProductPackageSpecificForEdit.Text = _viewModelTemplateOverride.GetPageText("ProductPackageSpecificLabelText")
        lblProductGroupForEdit.Text = _viewModelTemplateOverride.GetPageText("ProductLabelText")
        lblPackageForEdit.Text = _viewModelTemplateOverride.GetPageText("PackageLabelText")
        lblProductSubTypeForEdit.Text = _viewModelTemplateOverride.GetPageText("ProductSubTypeLabelText")
        lblProductTypeForEdit.Text = _viewModelTemplateOverride.GetPageText("ProductTypeLabelText")
        lblStadiumForEdit.Text = _viewModelTemplateOverride.GetPageText("StadiumLabelText")
        lblEmailTemplateForEdit.Text = _viewModelTemplateOverride.GetPageText("EmailConfirmationTemplateDDLLabel")
        lblQandATemplateForEdit.Text = _viewModelTemplateOverride.GetPageText("QandATemplateDDLLabel")
        lblDataCaptureTemplateForEdit.Text = _viewModelTemplateOverride.GetPageText("DataCaptureTemplateDDLLabel")
        ProductPackageSpecificYesText = _viewModelTemplateOverride.GetPageText("ProductPackageSpecificSwitchYesText")
        ProductPackageSpecificNoText = _viewModelTemplateOverride.GetPageText("ProductPackageSpecificSwitchNoText")
        btnUpdate.Text = _viewModelTemplateOverride.GetPageText("UpdateButtonText")
        btnDelete.Text = _viewModelTemplateOverride.GetPageText("DeleteButtonText")
        ProductSpecificText = _viewModelTemplateOverride.GetPageText("ProductSpecificText")
    End Sub
    ''' <summary>
    ''' Setup hidden fields
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setHiddenFields()
        hdfCreateTemplateOverrideErrorMessage.Value = _viewModelTemplateOverride.GetPageText("CreateTemplateOverrideErrorMessage")
        hdfTemplateNameRequiredErrorMessage.Value = _viewModelTemplateOverride.GetPageText("TemplateDescriptionRequiredErrorMessage")
        hdfUpdateTemplateOverrideErrorMessage.Value = _viewModelTemplateOverride.GetPageText("UpdateTemplateOverrideErrorMessage")
    End Sub

    ''' <summary>
    ''' Set the text for the column headings
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setColumnHeadingsAndVisibility()
        TemplateNameColumnHeading = _viewModelTemplateOverride.GetPageText("TemplateNameColumnHeading")
        TemplateCriteriaColumnHeading = _viewModelTemplateOverride.GetPageText("TemplateCriteriaColumnHeading")
        TemplateColumnHeading = _viewModelTemplateOverride.GetPageText("TemplateColumnHeading")
    End Sub

    ''' <summary>
    ''' Set the data-title attribute for each row in the data-table
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setRowDataTitles(e As RepeaterItemEventArgs)
        Dim htmlLabelEmailTemplateElement As Label = CType(e.Item.FindControl("lblEmailTemplate"), Label)
        htmlLabelEmailTemplateElement.Attributes.Add("title", _viewModelTemplateOverride.GetPageText("EmailTemplateDataTitle"))
        Dim htmlLabelQandATemplateElement As Label = CType(e.Item.FindControl("lblQandATemplate"), Label)
        htmlLabelQandATemplateElement.Attributes.Add("title", _viewModelTemplateOverride.GetPageText("QandATemplateDataTitle"))
        Dim htmlLabelDataCaptureTemplateElement As Label = CType(e.Item.FindControl("lblDataCaptureTemplate"), Label)
        htmlLabelDataCaptureTemplateElement.Attributes.Add("title", _viewModelTemplateOverride.GetPageText("DataCaptureTemplateDataTitle"))
    End Sub

    ''' <summary>
    ''' Retreive the list of  businessunit and populate the drop down list
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateBusinessUnits()
        _viewModelTemplateOverride = _templateOverrideBuilder.RetrieveBusinessUnitList()
        ddlBusinessUnit.Items.Clear()
        ddlBusinessUnit.DataSource = _viewModelTemplateOverride.BusinessUnitList
        ddlBusinessUnit.DataTextField = "Value"
        ddlBusinessUnit.DataValueField = "Key"
        ddlBusinessUnit.DataBind()
        ddlBusinessUnit.Items.Insert(0, New ListItem(_viewModelTemplateOverride.GetPageText("SelectBusinessUnitText"), String.Empty))
    End Sub

    ''' <summary>
    ''' Retreive the list of ticketing and package override criterias and bind the dropdowns
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateOverrideCriterias()
        _viewModelTemplateOverride = _templateOverrideBuilder.RetriveOverrideCriterias()

        ddlStadium.Items.Clear()
        ddlStadium.DataSource = _viewModelTemplateOverride.TicketingOverrideCriterias.[Select](Function(s) New With {Key s.StadiumId, s.StadiumDescription}).Distinct().ToList().Where(Function(r) r.StadiumId.Trim() <> String.Empty)
        ddlStadium.DataTextField = "StadiumDescription"
        ddlStadium.DataValueField = "StadiumId"
        ddlStadium.DataBind()

        ddlProductType.Items.Clear()
        ddlProductType.DataSource = _viewModelTemplateOverride.TicketingOverrideCriterias.[Select](Function(s) New With {Key s.ProductType, s.ProductTypeDecsription}).Distinct().ToList()
        ddlProductType.DataTextField = "ProductTypeDecsription"
        ddlProductType.DataValueField = "ProductType"
        ddlProductType.DataBind()

        ddlProductGroup.Items.Clear()
        ddlProductGroup.DataSource = _viewModelTemplateOverride.TicketingOverrideCriterias.[Select](Function(s) New With {Key s.ProductCode, s.ProductDescription}).Distinct().ToList()
        ddlProductGroup.DataTextField = "ProductDescription"
        ddlProductGroup.DataValueField = "ProductCode"
        ddlProductGroup.DataBind()

        ddlProductSubType.Items.Clear()
        ddlProductSubType.DataSource = _viewModelTemplateOverride.TicketingOverrideCriterias.[Select](Function(s) New With {Key s.ProductSubType, s.ProductSubTypeDescription}).Distinct().ToList()
        ddlProductSubType.DataTextField = "ProductSubTypeDescription"
        ddlProductSubType.DataValueField = "ProductSubType"
        ddlProductSubType.DataBind()

        ddlPackage.Items.Clear()
        ddlPackage.DataSource = _viewModelTemplateOverride.PackageOverrideCriterias.[Select](Function(s) New With {Key s.PackageId, s.PackageName}).Distinct().ToList()
        ddlPackage.DataTextField = "PackageName"
        ddlPackage.DataValueField = "PackageName"
        ddlPackage.DataBind()

        'For Edit
        ddlStadiumForEdit.Items.Clear()
        ddlStadiumForEdit.DataSource = _viewModelTemplateOverride.TicketingOverrideCriterias.[Select](Function(s) New With {Key s.StadiumId, s.StadiumDescription}).Distinct().ToList().Where(Function(r) r.StadiumId.Trim() <> String.Empty)
        ddlStadiumForEdit.DataTextField = "StadiumDescription"
        ddlStadiumForEdit.DataValueField = "StadiumId"
        ddlStadiumForEdit.DataBind()

        ddlProductTypeForEdit.Items.Clear()
        ddlProductTypeForEdit.DataSource = _viewModelTemplateOverride.TicketingOverrideCriterias.[Select](Function(s) New With {Key s.ProductType, s.ProductTypeDecsription}).Distinct().ToList()
        ddlProductTypeForEdit.DataTextField = "ProductTypeDecsription"
        ddlProductTypeForEdit.DataValueField = "ProductType"
        ddlProductTypeForEdit.DataBind()

        ddlProductGroupForEdit.Items.Clear()
        ddlProductGroupForEdit.DataSource = _viewModelTemplateOverride.TicketingOverrideCriterias.[Select](Function(s) New With {Key s.ProductCode, s.ProductDescription}).Distinct().ToList()
        ddlProductGroupForEdit.DataTextField = "ProductDescription"
        ddlProductGroupForEdit.DataValueField = "ProductCode"
        ddlProductGroupForEdit.DataBind()

        ddlProductSubTypeForEdit.Items.Clear()
        ddlProductSubTypeForEdit.DataSource = _viewModelTemplateOverride.TicketingOverrideCriterias.[Select](Function(s) New With {Key s.ProductSubType, s.ProductSubTypeDescription}).Distinct().ToList()
        ddlProductSubTypeForEdit.DataTextField = "ProductSubTypeDescription"
        ddlProductSubTypeForEdit.DataValueField = "ProductSubType"
        ddlProductSubTypeForEdit.DataBind()

        ddlPackageForEdit.Items.Clear()
        ddlPackageForEdit.DataSource = _viewModelTemplateOverride.PackageOverrideCriterias.[Select](Function(s) New With {Key s.PackageId, s.PackageName}).Distinct().ToList()
        ddlPackageForEdit.DataTextField = "PackageName"
        ddlPackageForEdit.DataValueField = "PackageName"
        ddlPackageForEdit.DataBind()
    End Sub

    ''' <summary>
    ''' Retreive the list of email confirmation emails
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateEmailConfirmationDropDownList(ByRef inputModelTemplateOverride As TemplateOverrideInputModel)
        Dim viewModelTemplateOverride As TemplateOverrideViewModel = _templateOverrideBuilder.RetrieveEmailConfirmationList(inputModelTemplateOverride)
        ddlEmailConfirmation.Items.Clear()
        ddlEmailTemplateForEdit.Items.Clear()
        If viewModelTemplateOverride.EmailConfirmationList.Count > 0 Then
            ddlEmailConfirmation.DataSource = viewModelTemplateOverride.EmailConfirmationList
            ddlEmailConfirmation.DataTextField = "SaleConfirmationEmailDescription"
            ddlEmailConfirmation.DataValueField = "SaleConfirmationEmailId"
            ddlEmailConfirmation.DataBind()
            ddlEmailConfirmation.Attributes.Add("data-placeholder", viewModelTemplateOverride.GetPageText("EmailConfirmationDDLPromptText"))
            ddlEmailConfirmation.Attributes.Add("data-allow-clear", "true")
            ddlEmailConfirmation.Items.Insert(0, New ListItem(_viewModelTemplateOverride.GetPageText("EmailConfirmationDDLPromptText"), String.Empty))
            'For Edit

            ddlEmailTemplateForEdit.DataSource = viewModelTemplateOverride.EmailConfirmationList
            ddlEmailTemplateForEdit.DataTextField = "SaleConfirmationEmailDescription"
            ddlEmailTemplateForEdit.DataValueField = "SaleConfirmationEmailId"
            ddlEmailTemplateForEdit.DataBind()
            ddlEmailTemplateForEdit.Attributes.Add("data-placeholder", viewModelTemplateOverride.GetPageText("EmailConfirmationDDLPromptText"))
            ddlEmailTemplateForEdit.Attributes.Add("data-allow-clear", "true")
            ddlEmailTemplateForEdit.Items.Insert(0, New ListItem(_viewModelTemplateOverride.GetPageText("EmailConfirmationDDLPromptText"), String.Empty))
        Else
            ddlEmailConfirmation.Enabled = False
            ddlEmailConfirmation.Attributes.Add("title", viewModelTemplateOverride.GetPageText("EmailConfirmationDDLDisabledText"))
        End If

    End Sub

    ''' <summary>
    ''' Retreive the list of Q and A templates
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateQAndADropDownList(ByRef inputModelTemplateOverride As TemplateOverrideInputModel)
        Dim viewModelTemplateOverride As TemplateOverrideViewModel = _templateOverrideBuilder.RetrieveQandATemplateList(inputModelTemplateOverride)
        ddlQAndATemplate.Items.Clear()
        ddlQandATemplateForEdit.Items.Clear()
        If viewModelTemplateOverride.QandAList.Count > 0 Then
            ddlQAndATemplate.DataSource = viewModelTemplateOverride.QandAList
            ddlQAndATemplate.DataTextField = "QAndATemplateDescription"
            ddlQAndATemplate.DataValueField = "QAndATemplateId"
            ddlQAndATemplate.DataBind()
            ddlQAndATemplate.Attributes.Add("data-placeholder", viewModelTemplateOverride.GetPageText("QandADDLPromptText"))
            ddlQAndATemplate.Attributes.Add("data-allow-clear", "true")
            ddlQAndATemplate.ClearSelection()
            ddlQAndATemplate.Items.Insert(0, New ListItem(_viewModelTemplateOverride.GetPageText("QandADDLPromptText"), String.Empty))

            'For Edit
            ddlQandATemplateForEdit.DataSource = viewModelTemplateOverride.QandAList
            ddlQandATemplateForEdit.DataTextField = "QAndATemplateDescription"
            ddlQandATemplateForEdit.DataValueField = "QAndATemplateId"
            ddlQandATemplateForEdit.DataBind()
            ddlQandATemplateForEdit.Attributes.Add("data-placeholder", viewModelTemplateOverride.GetPageText("QandADDLPromptText"))
            ddlQandATemplateForEdit.Attributes.Add("data-allow-clear", "true")
            ddlQandATemplateForEdit.Items.Insert(0, New ListItem(_viewModelTemplateOverride.GetPageText("QandADDLPromptText"), String.Empty))
        Else
            ddlQAndATemplate.Enabled = False
            ddlQAndATemplate.Attributes.Add("title", viewModelTemplateOverride.GetPageText("QandADDLDisabledText"))
        End If
    End Sub

    ''' <summary>
    ''' Retreive the list of Data Capture templates
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateDataCaptureDropDownList(ByRef inputModelTemplateOverride As TemplateOverrideInputModel)
        Dim viewModelTemplateOverride As TemplateOverrideViewModel = _templateOverrideBuilder.RetrieveDataCaptureTemplateList(inputModelTemplateOverride)
        ddlDataCaptureTemplate.Items.Clear()
        ddlDataCaptureTemplateForEdit.Items.Clear()
        If viewModelTemplateOverride.DataCaptureList.Count > 0 Then
            ddlDataCaptureTemplate.DataSource = viewModelTemplateOverride.DataCaptureList
            ddlDataCaptureTemplate.DataTextField = "DataCaptureTemplateDescription"
            ddlDataCaptureTemplate.DataValueField = "DataCaptureTemplateId"
            ddlDataCaptureTemplate.DataBind()
            ddlDataCaptureTemplate.Attributes.Add("data-placeholder", viewModelTemplateOverride.GetPageText("DataCaptureDDLPromptText"))
            ddlDataCaptureTemplate.Attributes.Add("data-allow-clear", "true")
            ddlDataCaptureTemplate.ClearSelection()
            ddlDataCaptureTemplate.Items.Insert(0, New ListItem(_viewModelTemplateOverride.GetPageText("DataCaptureDDLPromptText"), String.Empty))

            'For Edit
            ddlDataCaptureTemplateForEdit.DataSource = viewModelTemplateOverride.DataCaptureList
            ddlDataCaptureTemplateForEdit.DataTextField = "DataCaptureTemplateDescription"
            ddlDataCaptureTemplateForEdit.DataValueField = "DataCaptureTemplateId"
            ddlDataCaptureTemplateForEdit.DataBind()
            ddlDataCaptureTemplateForEdit.Attributes.Add("data-placeholder", viewModelTemplateOverride.GetPageText("DataCaptureDDLPromptText"))
            ddlDataCaptureTemplateForEdit.Attributes.Add("data-allow-clear", "true")
            ddlDataCaptureTemplateForEdit.Items.Insert(0, New ListItem(_viewModelTemplateOverride.GetPageText("DataCaptureDDLPromptText"), String.Empty))
        Else
            ddlDataCaptureTemplate.Enabled = False
            ddlDataCaptureTemplate.Attributes.Add("title", viewModelTemplateOverride.GetPageText("DataCaptureDDLDisabledText"))
        End If

    End Sub

    ''' <summary>
    ''' Clear controls after create
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub clearControls()
        txtTemplateOverrideDescription.Text = String.Empty
        ltlSuccessMessage.Text = String.Empty
        ddlProductSubType.SelectedIndex = -1
        ddlProductGroup.SelectedIndex = -1
        ddlProductType.SelectedIndex = -1
        ddlStadium.SelectedIndex = -1
        ddlPackage.SelectedIndex = -1
        ddlEmailConfirmation.SelectedIndex = -1
        ddlQAndATemplate.SelectedIndex = -1
        ddlDataCaptureTemplate.SelectedIndex = -1
    End Sub

#End Region

End Class
