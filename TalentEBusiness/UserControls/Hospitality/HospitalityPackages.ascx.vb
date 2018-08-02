Imports Talent.eCommerce
Imports Talent.Common
Imports TalentBusinessLogic.Models
Imports TalentBusinessLogic.ModelBuilders.Hospitality
Imports TalentBusinessLogic.DataTransferObjects.Hospitality

Partial Class UserControls_HospitalityPackages
    Inherits ControlBase

#Region "Class Level Fields"
    Private _viewModelPackage As HospitalityPackageListViewModel = Nothing
    'Create a view model for page text retrieval
    Private _pageViewModel As BaseViewModel = Nothing
    Private _controlViewModel As BaseViewModel = Nothing
    Private _businessUnit As String = String.Empty
    Private _partner As String = String.Empty
#End Region

#Region "Public Properties"
    ''' <summary>
    ''' Dynamic layout class
    ''' </summary>
    ''' <returns></returns>
    Public Property DynamicLayoutClass As String = String.Empty

    ''' <summary>
    ''' Product code
    ''' </summary>
    ''' <returns></returns>
    Public Property ProductCode() As String = String.Empty

    ''' <summary>
    ''' Package id
    ''' </summary>
    ''' <returns></returns>
    Public Property PackageId() As Long = 0

    ''' <summary>
    ''' View from area text
    ''' </summary>
    ''' <returns></returns>
    Public Property ViewFromAreaText As String = String.Empty

    ''' <summary>
    ''' Usage
    ''' </summary>
    ''' <returns></returns>
    Public Property Usage() As String

    ''' <summary>
    ''' Product group code
    ''' </summary>
    ''' <returns></returns>
    Public Property ProductGroupCode As String

    ''' <summary>
    ''' Is packge list by product group
    ''' </summary>
    ''' <returns></returns>
    Public Property IsPackageListByProductGroup As Boolean = False

#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        _businessUnit = TalentCache.GetBusinessUnit()
        _partner = TalentCache.GetPartner(HttpContext.Current.Profile)
        Dim inputModelPackages As HospitalityPackageListInputModel = setupInputModelPackages()
        processController(inputModelPackages)
        createView()
    End Sub

    Protected Sub UserControls_HospitalityPackages_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        plhMessages.Visible = (ltlNoPackageMessage.Text.Length > 0)
    End Sub

    Protected Sub rptPackages_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptPackages.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim packageDataItem As PackageDetails = CType(e.Item.DataItem, PackageDetails)
            Dim hplPackageHeader As HyperLink = CType(e.Item.FindControl("hplPackageHeader"), HyperLink)
            Dim navigateUrl As New StringBuilder

            If (String.IsNullOrEmpty(packageDataItem.ProductGroupCode) Or packageDataItem.ProductGroupCode Is Nothing) Then
                navigateUrl.Append(ResolveUrl("~/PagesPublic/Hospitality/HospitalityPackageDetails.aspx")).Append("?product=").Append(Request.QueryString("product"))
                navigateUrl.Append("&packageid=").Append(packageDataItem.PackageID)
                navigateUrl.Append("&availabilitycomponentid=").Append(packageDataItem.AvailabilityComponentID)
            Else
                navigateUrl.Append(ResolveUrl("~/PagesPublic/Hospitality/HospitalityBundleFixtures.aspx"))
                navigateUrl.Append("?productGroupCode=").Append(ProductGroupCode)
                navigateUrl.Append("&packageId=").Append(packageDataItem.PackageID)
                navigateUrl.Append("&availabilitycomponentid=").Append(packageDataItem.AvailabilityComponentID)
            End If
            hplPackageHeader.NavigateUrl = navigateUrl.ToString()
        End If
    End Sub

#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Return text with placeholders replaced   
    ''' </summary>
    ''' <param name="id">The control text ID</param>
    ''' <param name="package">The package object being worked with</param>
    ''' <returns>The formatted text value</returns>
    ''' <remarks></remarks>
    Public Function GetText(ByVal id As String, ByVal package As PackageDetails) As String
        Dim textValue As String = _controlViewModel.GetControlText(id)
        If Not IsPackageListByProductGroup Then
            If textValue.Contains("<<") Then
                textValue = textValue.Replace("<<AvailableUnits>>", package.AvailabilityDetail.AvailableUnits)
                textValue = textValue.Replace("<<OriginalUnits>>", package.AvailabilityDetail.OriginalUnits)
                textValue = textValue.Replace("<<PercentageRemaining>>", package.AvailabilityDetail.PercentageRemaining)
            End If
        End If
        Return textValue
    End Function

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
        inputModel.ProductCode = ProductCode
        inputModel.PackageId = PackageId
        inputModel.ProductGroupCode = ProductGroupCode
        Return inputModel
    End Function

    '' <summary>
    '' Get ImageUrl of the playing area from a perspective of the package seat
    '' </summary>
    '' <param name="packageCode">The packageCode</param>
    '' <returns>Image path</returns>
    Private Function viewFromAreaImgURL(ByVal packageCode As String) As String
        Dim imgURL As String = String.Empty
        Dim viewImageName As String = packageCode
        imgURL = ImagePath.getImagePath("PRODCORPORATE", viewImageName, _businessUnit, _partner)
        If imgURL.Contains(ModuleDefaults.MissingImagePath) Then
            imgURL = String.Empty
        End If
        Return imgURL
    End Function
#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Process the controller, the code actioning the input values if there are any.
    ''' </summary>
    ''' <param name="inputModelPackages">Input Model for Packages</param>
    ''' <remarks></remarks>
    Private Sub processController(ByVal inputModelPackages As HospitalityPackageListInputModel)
        Dim builder As New HospitalityListBuilder
        _viewModelPackage = builder.GetHospitalityPackageList(inputModelPackages)
    End Sub

    ''' <summary>
    ''' Bind html controls
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub createView()
        _pageViewModel = New BaseViewModel(New WebFormResource, True)
        _controlViewModel = New BaseViewModel(New UserControlResource, True, "HospitalityPackages.ascx")
        ViewFromAreaText = _pageViewModel.GetPageText("ViewHeaderLabel")

        If Not _viewModelPackage.PackageListDetailsByProductCode Is Nothing Then
            For Each package As PackageDetails In _viewModelPackage.PackageListDetailsByProductCode
                Dim viewImgURL As String = viewFromAreaImgURL(package.PackageCode)
                If String.IsNullOrEmpty(viewImgURL) Then
                    package.ShowViewFromArea = False
                Else
                    package.ShowViewFromArea = True
                    package.ViewAreaImageUrl = viewImgURL
                End If
            Next

            If Me.Usage.ToUpper = "FILTEREDPACKAGES" Then
                rptPackages.DataSource = _viewModelPackage.PackageListDetailsByProductCode.FindAll(Function(p) p.AvailabilityDetail.HaveSoldOutComponents = False)
            ElseIf Me.Usage.ToUpper = "FULLPACKAGELIST" Then
                rptPackages.DataSource = _viewModelPackage.PackageListDetailsByProductCode
            End If

            rptPackages.DataBind()
        ElseIf Not _viewModelPackage.PackageListDetailsByProductGroup Is Nothing Then
            IsPackageListByProductGroup = True
            For Each package As PackageDetails In _viewModelPackage.PackageListDetailsByProductGroup
                Dim viewImgURL As String = viewFromAreaImgURL(package.PackageCode)
                If String.IsNullOrEmpty(viewImgURL) Then
                    package.ShowViewFromArea = False
                Else
                    package.ShowViewFromArea = True
                    package.ViewAreaImageUrl = viewImgURL
                End If
            Next
            If Me.Usage.ToUpper = "BUNDLEPACKAGELIST" Then
                rptPackages.DataSource = _viewModelPackage.PackageListDetailsByProductGroup
                rptPackages.DataBind()

                If _viewModelPackage.PackageListDetailsByProductGroup.Count = 0 Or _viewModelPackage.PackageListDetailsByProductGroup Is Nothing Then
                    ltlNoPackageMessage.Text = _controlViewModel.GetControlText("NoPackageMessage")
                Else
                    ltlNoPackageMessage.Text = String.Empty
                End If

            End If
            End If
    End Sub

#End Region

End Class
