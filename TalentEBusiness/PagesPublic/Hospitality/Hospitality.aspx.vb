Imports TalentBusinessLogic.ModelBuilders.Hospitality
Imports TalentBusinessLogic.Models
Imports System.Data
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class PagesPublic_Hospitality_Hospitality
    Inherits TalentBase01


#Region "Class Level Fields"

    Private _viewModelProduct As HospitalityProductListViewModel = Nothing
    Private _viewModelPackage As HospitalityPackageListViewModel = Nothing
    Private _viewModelProductGroup As HospitalityProductGroupViewModel = Nothing
    Private _dtCompetition As DataTable = Nothing
    Private _dtSubTypes As DataTable = Nothing
    Private _dtOpposition As DataTable = Nothing
    Public ShowSubType As Boolean = False
#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Returns Pre-Requisites warning message replaced with Pre-Requisites product description
    ''' </summary>
    ''' <param name="ProductReqdMemDesc">Description of Pre-Requisites product</param>
    ''' <returns>Returns Pre-Requisites warning message</returns>
    ''' <remarks></remarks>
    Public Function GetProductEligibilityText(ByVal ProductReqdMemDesc As String) As String
        Dim str As String = String.Empty
        If Not String.IsNullOrEmpty(ProductReqdMemDesc) Then
            If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelPackage.GetPageAttribute("RetrieveCoReqGrpDesc")) Then
                str = _viewModelPackage.GetPageText("CoReqGrpDescCaption", True).Replace("<<PRE-REQ>>", ProductReqdMemDesc)
            Else
                str = _viewModelPackage.GetPageText("EligibilityPreReqCaption", True).Replace("<<PRE-REQ>>", ProductReqdMemDesc)
            End If
        End If
        Return str
    End Function
#End Region


#Region "Protected Methods"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "error-handling.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("error-handling.js", "/Application/Status/"), False)
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "Hospitality.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("Hospitality.js", "/Module/Hospitality/"), False)
        Dim inputModelProducts As HospitalityProductListInputModel = setupInputModelProducts()
        Dim inputModelPackages As HospitalityPackageListInputModel = setupInputModelPackages()
        Dim inputModelProductGroups As HospitalityProductGroupInputModel = setupInputModelProductGroups()
        processController(inputModelProducts, inputModelPackages, inputModelProductGroups)
        ShowSubType = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_viewModelPackage.GetPageAttribute("SubTypeVisible"))

        createView()
    End Sub
    Protected Sub PagesPublic_Hospitality_Hospitality_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        plhErrorList.Visible = (blErrorMessages.Items.Count > 0)
    End Sub
    Protected Sub rptPackagesSeason_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptPackagesSeason.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim hplSeasonPackages As HyperLink = CType(e.Item.FindControl("hplSeasonPackages"), HyperLink)
            Dim navigateUrl As New StringBuilder
            Dim package As New TalentBusinessLogic.DataTransferObjects.Hospitality.PackageDetails
            package = CType(e.Item.DataItem, TalentBusinessLogic.DataTransferObjects.Hospitality.PackageDetails)
            navigateUrl.Append("~/PagesPublic/Hospitality/HospitalityPackageDetails.aspx")
            navigateUrl.Append("?packageid=").Append(package.PackageID)
            navigateUrl.Append("&producttype=S")
            navigateUrl.Append("&product=").Append(_viewModelProduct.ProductListDetailsSeason(0).ProductCode)
            navigateUrl.Append("&availabilitycomponentid=").Append(package.AvailabilityComponentID)
            hplSeasonPackages.NavigateUrl = navigateUrl.ToString()
        End If
    End Sub

    Protected Sub rptProductGroups_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptProductGroups.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim hplProductGroups As HyperLink = CType(e.Item.FindControl("hplProductGroups"), HyperLink)
            Dim ltlProductGrpFixturesAvailableMsg As Literal = CType(e.Item.FindControl("ltlProductGrpFixturesAvailableMsg"), Literal)
            Dim navigateUrl As New StringBuilder
            Dim productGroup As New TalentBusinessLogic.DataTransferObjects.Hospitality.ProductGroupDetails
            navigateUrl.Append("~/PagesPublic/Hospitality/HospitalityBundlePackages.aspx")
            navigateUrl.Append("?productgroup=").Append(DirectCast(e.Item.DataItem, TalentBusinessLogic.DataTransferObjects.Hospitality.ProductGroupDetails).GroupId)
            hplProductGroups.NavigateUrl = navigateUrl.ToString()

            ltlProductGrpFixturesAvailableMsg.Text = _viewModelProductGroup.GetPageText("ProductGrpFixturesAvailableMsg")

            Dim rptProductGroupFixtures As Repeater = CType(e.Item.FindControl("rptProductGroupFixtures"), Repeater)
            If rptProductGroupFixtures IsNot Nothing Then
                rptProductGroupFixtures.DataSource = DirectCast(e.Item.DataItem, TalentBusinessLogic.DataTransferObjects.Hospitality.ProductGroupDetails).ProductGroupFixturesList
                rptProductGroupFixtures.DataBind()
            Else
                rptProductGroupFixtures.Visible = False
            End If
        End If
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Setup the hospitality details input model and return the model for use
    ''' </summary>
    ''' <returns>The formatted input model based on form data</returns>
    ''' <remarks></remarks>
    Private Function setupInputModelProducts() As HospitalityProductListInputModel
        Dim inputModel As New HospitalityProductListInputModel
        Return inputModel
    End Function

    ''' <summary>
    ''' Setup the hospitality details input model and return the model for use
    ''' </summary>
    ''' <returns>The formatted input model based on form data</returns>
    ''' <remarks></remarks>
    Private Function setupInputModelPackages() As HospitalityPackageListInputModel
        Dim inputModel As New HospitalityPackageListInputModel
        Return inputModel
    End Function

    Private Function setupInputModelProductGroups() As HospitalityProductGroupInputModel
        Dim inputModel As New HospitalityProductGroupInputModel
        Return inputModel
    End Function

    ''Private Function dateVisable(product As HospitalityProductListViewModel ) As Boolean
    ''    Dim dateVisable As Boolean
    ''    If product.ProductListDetailsHome.
    ''End Function

#End Region

#Region "Private Methods"
    ''' <summary>
    ''' Process the controller, the code actioning the input values if there are any.
    ''' </summary>
    ''' <param name="inputModelProducts">Input Model for Products</param>
    ''' <param name="inputModelPackages">Input Model for Packages</param>
    ''' <param name="inputModelProductGroups">Input Model for Product Groups</param>
    ''' <remarks></remarks>
    Private Sub processController(ByVal inputModelProducts As HospitalityProductListInputModel, ByVal inputModelPackages As HospitalityPackageListInputModel, ByVal inputModelProductGroups As HospitalityProductGroupInputModel)
        Dim builder As New HospitalityListBuilder
        blErrorMessages.Items.Clear()
        _viewModelProduct = builder.GetHospitalityProductList(inputModelProducts)
        _viewModelPackage = builder.GetHospitalityPackageList(inputModelPackages)
        _viewModelProductGroup = builder.GetHospitalityProductGroups(inputModelProductGroups)
    End Sub

    ''' <summary>
    ''' Control the visibility of Competition Filter and bind the repeaters if they are visible
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setCompetition()
        plhCompetition.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_viewModelPackage.GetPageAttribute("CompetitionFilterVisible")) AndAlso _dtCompetition.Rows.Count > 0
        If (plhCompetition.Visible) Then
            rptCompetitionA.DataSource = _dtCompetition
            rptCompetitionA.DataBind()
            rptCompetitionB.DataSource = _dtCompetition
            rptCompetitionB.DataBind()
        End If
    End Sub

    ''' <summary>
    ''' Control the visibility of Opposition Filter and bind the repeaters if they are visible
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setOpposition()
        plhOpposition.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_viewModelPackage.GetPageAttribute("OppositionFilterVisible")) AndAlso _dtOpposition.Rows.Count > 0
        If (plhOpposition.Visible) Then
            rptOppositionFilterA.DataSource = _dtOpposition
            rptOppositionFilterA.DataBind()
            rptOppositionFilterB.DataSource = _dtOpposition
            rptOppositionFilterB.DataBind()
        End If
    End Sub

    ''' <summary>
    ''' Control the visibility of Subtype Filter and bind the repeaters if they are visible
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setSubtype()
        plhSubtype.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_viewModelPackage.GetPageAttribute("SubtypeFilterVisible")) AndAlso _dtSubTypes.Rows.Count > 0
        If (plhSubtype.Visible) Then
            rptSubTypeA.DataSource = _dtSubTypes
            rptSubTypeA.DataBind()
            rptSubTypeB.DataSource = _dtSubTypes
            rptSubTypeB.DataBind()
        End If
    End Sub

    ''' <summary>
    ''' Set the hidden field values
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setHiddenFields()
        hdfSeasonProductCount.Value = _viewModelProduct.ProductListDetailsSeason.Count
        hdfMinDate.Value = _viewModelProduct.MinDateHome
        hdfMaxDate.Value = _viewModelProduct.MaxDateHome
        hdfMinGroupSizeHome.Value = _viewModelPackage.MinGroupSizeHome
        hdfMaxGroupSizeHome.Value = _viewModelPackage.MaxGroupSizeHome
        hdfMinBudgetHome.Value = _viewModelPackage.MinBudgetHome
        hdfMaxBudgetHome.Value = _viewModelPackage.MaxBudgetHome
        hdfMinGroupSizeSeason.Value = _viewModelPackage.MinGroupSizeSeason
        hdfMaxGroupSizeSeason.Value = _viewModelPackage.MaxGroupSizeSeason
        hdfMinBudgetSeason.Value = _viewModelPackage.MinBudgetSeason
        hdfMaxBudgetSeason.Value = _viewModelPackage.MaxBudgetSeason
        hdfCurrencySymbol.Value = TDataObjects.PaymentSettings.GetCurrencySymbol(TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))

        'default filters
        hdfFilterGroupType.Value = Request.QueryString("type")
        hdfFilterGroupsubType.Value = Request.QueryString("subtype")
        hdfFilterGroupCompetition.Value = Request.QueryString("competition")
        hdfFilterGroupOpposition.Value = Request.QueryString("opposition")
        hdfRangeGroupMin.Value = Request.QueryString("groupMin")
        hdfRangeGroupMax.Value = Request.QueryString("groupMax")
        hdfRangeBudgetMin.Value = Request.QueryString("budgetMin")
        hdfRangeBudgetMax.Value = Request.QueryString("budgetMax")
        hdfRangeDateMin.Value = Request.QueryString("dateMin")
        hdfRangeDateMax.Value = Request.QueryString("dateMax")
    End Sub


    ''' <summary>
    ''' Set the page label text 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setPageLabels()
        ltlFixtureTab.Text = _viewModelPackage.GetPageText("fixturesTabLabel")
        ltlPackagesTab.Text = _viewModelPackage.GetPageText("packagesTabLabel")
        ltlFixSeasonTab.Text = _viewModelPackage.GetPageText("fixtureSeasonTabLabel")
        ltlPakSeasonTab.Text = _viewModelPackage.GetPageText("packageSeasonTabLabel")
        ltlSubType.Text = _viewModelPackage.GetPageText("subTypeText")
        ltlCompetition.Text = _viewModelPackage.GetPageText("competitionText")
        ltlNoResults.Text = _viewModelPackage.GetPageText("FilterGivesNoResults")
        ltlProductGroupTab.Text = _viewModelPackage.GetPageText("productGroupTabLabel")

        Dim anyString As String = _viewModelPackage.GetPageText("anyText")
        ltlAnyCompetitionTabA.Text = anyString
        ltlAnyCompetitionTabB.Text = anyString
        ltlAnyOpposition.Text = anyString
        ltlAnyOppositionTab.Text = anyString
        ltlAnySubTypeTabA.Text = anyString
        ltlAnySubTypeTabB.Text = anyString

        ltlGroupSliderLabel.Text = _viewModelPackage.GetPageText("groupSliderLabel")
        ltlBudgetSliderLabel.Text = _viewModelPackage.GetPageText("budgetSliderLabel")
        ltlDateSliderLabel.Text = _viewModelPackage.GetPageText("dateSliderLabel")
        ltlResetOptionText.Text = _viewModelPackage.GetPageText("resetOptionLabel")
    End Sub

    ''' <summary>
    ''' Show the status message on basis of the availability
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setStatusMessage()
        If _viewModelProduct.ProductListDetailsHome.Count = 0 Then
            ltlNoFixtures.Text = _viewModelPackage.GetPageText("msgNoHomeFixturesAvailable")
        End If
        If _viewModelPackage.PackageListDetailsHome.Count = 0 Then
            ltlNoPackages.Text = _viewModelPackage.GetPageText("msgNoHomePackagesAvailable")
        End If
        If _viewModelProduct.ProductListDetailsSeason.Count = 0 Then
            ltlNoSeason.Text = _viewModelPackage.GetPageText("msgNoSeasonFixturesAvailable")
        End If
        If _viewModelPackage.PackageListDetailsSeason.Count = 0 Then
            ltlNoPackagesSeason.Text = _viewModelPackage.GetPageText("msgNoSeasonPackagesAvailable")
        End If
        If _viewModelProductGroup.ProductGroupDetailsList.Count = 0 Then
            ltlNoProductGroup.Text = _viewModelPackage.GetPageText("msgNoProductGroupAvailable")
        End If
    End Sub

    ''' <summary>
    ''' Create the view for the page based on the view models given
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub createView()

        Dim hasResults As Boolean = _viewModelProduct.ProductListDetailsHome.Count > 0 _
                                    OrElse _viewModelProduct.ProductListDetailsSeason.Count > 0 _
                                    OrElse _viewModelPackage.PackageListDetailsHome.Count > 0 _
                                    OrElse _viewModelPackage.PackageListDetailsSeason.Count > 0 _
                                    OrElse _viewModelProductGroup.ProductGroupDetailsList.Count > 0

        If hasResults Then
            plhResults.Visible = True
            setStatusMessage()
            setHiddenFields()
            setPageLabels()
            buildTablesForFilteringOptions()
            setCompetition()
            setOpposition()
            setSubtype()
            plhDate.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_viewModelPackage.GetPageAttribute("DateFilterVisible")) AndAlso _viewModelProduct.ProductListDetailsHome.Count > 0
            plhGroupsize.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_viewModelPackage.GetPageAttribute("GroupSizeFilterVisible"))
            plhBudget.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_viewModelPackage.GetPageAttribute("BudgetFilterVisible"))
            rptFixturesHome.DataSource = _viewModelProduct.ProductListDetailsHome
            rptFixturesHome.DataBind()
            rptFixturesSeason.DataSource = _viewModelProduct.ProductListDetailsSeason
            rptFixturesSeason.DataBind()
            rptPackages.DataSource = _viewModelPackage.PackageListDetailsHome
            rptPackages.DataBind()
            rptPackagesSeason.DataSource = _viewModelPackage.PackageListDetailsSeason
            rptPackagesSeason.DataBind()
            rptProductGroups.DataSource = _viewModelProductGroup.ProductGroupDetailsList
            rptProductGroups.DataBind()
        Else
            plhResults.Visible = False
            blErrorMessages.Items.Add(_viewModelPackage.GetPageText("msgNoResultsAvailable"))
        End If

        If _viewModelProductGroup.Error.HasError Then
            blErrorMessages.Items.Add(_viewModelProductGroup.Error.ErrorMessage)
        End If

    End Sub

    ''' <summary>
    ''' Build the local data tables for filter options
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub buildTablesForFilteringOptions()
        Dim dtProductDetails As DataTable = New DataTable
        Dim dvProductDetailsView As DataView = Nothing

        dtProductDetails.Columns.Add("ProductCode", GetType(String))
        dtProductDetails.Columns.Add("ProductDescription", GetType(String))
        dtProductDetails.Columns.Add("ProductDateSearch", GetType(String))
        dtProductDetails.Columns.Add("ProductType", GetType(String))
        dtProductDetails.Columns.Add("ProductMDTE08", GetType(Integer))
        dtProductDetails.Columns.Add("ProductCompetitionCode", GetType(String))
        dtProductDetails.Columns.Add("ProductCompetitionDesc", GetType(String))
        dtProductDetails.Columns.Add("ProductSubType", GetType(String))
        dtProductDetails.Columns.Add("ProductSubTypeDesc", GetType(String))
        dtProductDetails.Columns.Add("ProductOppositionCode", GetType(String))
        dtProductDetails.Columns.Add("ProductOppositionDesc", GetType(String))

        For Each item In _viewModelProduct.ProductListDetailsHome()
            dtProductDetails.Rows.Add(item.ProductCode, item.ProductDescription, item.ProductDateSearch, item.ProductType, item.ProductMDTE08, item.ProductCompetitionCode, item.ProductCompetitionDesc, item.ProductSubType, item.ProductSubTypeDesc, item.ProductOppositionCode, item.ProductOppositionDesc)
        Next
        dvProductDetailsView = New DataView(dtProductDetails)

        'get distinct list of competion codes for filtering
        dvProductDetailsView.RowFilter = "ProductCompetitionCode <> ''"
        _dtCompetition = dvProductDetailsView.ToTable(True, "ProductCompetitionCode", "ProductCompetitionDesc")

        'get distinct list of product subtypes for filtering
        dvProductDetailsView.RowFilter = "ProductSubType <> ''"
        _dtSubTypes = dvProductDetailsView.ToTable(True, "ProductSubType", "ProductSubTypeDesc")

        'get distinct list of product opposition for filtering
        dvProductDetailsView.RowFilter = "ProductOppositionCode <> ''"
        _dtOpposition = dvProductDetailsView.ToTable(True, "ProductOppositionCode", "ProductOppositionDesc")
    End Sub
#End Region

End Class