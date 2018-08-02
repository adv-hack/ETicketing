Imports Talent.Common
Imports TalentBusinessLogic.Models
Imports TalentBusinessLogic.ModelBuilders.Hospitality
Imports TalentBusinessLogic.DataTransferObjects.Hospitality
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class UserControls_HospitalityFixtures
    Inherits ControlBase

#Region "Class Level Fields"
    Private _ucr As New Talent.Common.UserControlResource
    Private _log As Talent.Common.TalentLogging = Talent.eCommerce.Utilities.TalentLogging
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _viewModelProduct As HospitalityProductListViewModel = Nothing
    Private _viewModelProductGroup As HospitalityProductGroupViewModel = Nothing
    Private _controlViewModel As BaseViewModel = Nothing
#End Region

#Region "Public Properties"
    Public Property DynamicLayoutClass As String = String.Empty
    Public Property PackageId() As String = String.Empty
    Public Property ProductType() As String = String.Empty

    ''' <summary>
    ''' Defines which page the user control is used in so it can behave differently per page
    ''' </summary>
    ''' <returns></returns>
    Public Property Usage() As String = String.Empty

    ''' <summary>
    ''' Product Group Code
    ''' </summary>
    ''' <returns></returns>
    Public Property ProductGroupCode() As String = String.Empty

    Public DisplaySubType As Boolean = False
#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Return text with placeholders replaced   
    ''' </summary>
    ''' <param name="id">The control text ID</param>
    ''' <returns>The formatted text value</returns>
    ''' <remarks></remarks>
    Public Function GetText(ByVal id As String, ByVal product As ProductDetails) As String
        Dim textValue As String = _controlViewModel.GetControlText(id)

        If textValue.Contains("<<") Then
            textValue = textValue.Replace("<<AvailableUnits>>", product.AvailabilityDetail.AvailableUnits)
            textValue = textValue.Replace("<<OriginalUnits>>", product.AvailabilityDetail.OriginalUnits)
            textValue = textValue.Replace("<<PercentageRemaining>>", product.AvailabilityDetail.PercentageRemaining)
        End If
        Return textValue
    End Function
    ''' <summary>
    ''' Returns Pre-Requisites warning message replaced with Pre-Requisites product description
    ''' </summary>
    ''' <param name="ProductReqdMemDesc">Description of Pre-Requisites product</param>
    ''' <returns>Returns Pre-Requisites warning message</returns>
    ''' <remarks></remarks>
    Public Function GetProductEligibilityText(ByVal ProductReqdMemDesc As String) As String
        Dim str As String = String.Empty
        If Not String.IsNullOrEmpty(ProductReqdMemDesc) Then
            If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("RetrieveCoReqGrpDesc")) Then
                str = _ucr.Content("CoReqGrpDescCaption", _languageCode, True).Replace("<<PRE-REQ>>", ProductReqdMemDesc)
            Else
                str = _ucr.Content("EligibilityPreReqCaption", _languageCode, True).Replace("<<PRE-REQ>>", ProductReqdMemDesc)
            End If
        End If
        Return str
    End Function

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "HospitalityFixtures.ascx"
        End With

    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        blErrorMessages.Items.Clear()

        Dim inputModelProducts As HospitalityProductListInputModel = setupInputModelProducts()
        If Usage.ToUpper = "HOSPITALITYBUNDLEFIXTURES.ASPX" Then
            Dim inputModelProductGroup As HospitalityProductGroupInputModel = setupInputModelProductGroup()
            processController(inputModelProducts, inputModelProductGroup)
        Else
            processController(inputModelProducts)
        End If
        createView()
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        plhErrorMessages.Visible = (blErrorMessages.Items.Count > 0)
    End Sub

    Protected Sub rptFixturesHome_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        Dim rptFixturesHome As Repeater = CType(sender, Repeater)

        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim hlkFixturePackageDetails As HyperLink = CType(e.Item.FindControl("hlkFixturePackageDetails"), HyperLink)
            Dim plhHospitalityBundleFixtureItem As PlaceHolder = CType(e.Item.FindControl("plhHospitalityBundleFixtureItem"), PlaceHolder)
            Dim divFixtureItem As HtmlGenericControl = CType(e.Item.FindControl("divFixtureItem"), HtmlGenericControl)
            Dim fixture As ProductDetails = CType(e.Item.DataItem, ProductDetails)

            If ProductDetail.GetImageURL("PRODTICKETING", fixture.ProductOppositionCode).Contains("NOI.gif") Then
                divFixtureItem.Attributes.Add("class", "c-hosp-item__card c-hosp-item__noimg")
            Else
                divFixtureItem.Attributes.Add("class", "c-hosp-item__card")
            End If

            If Usage.ToUpper = "HOSPITALITYBUNDLEFIXTURES.ASPX" Then
                plhHospitalityBundleFixtureItem.Visible = True
                hlkFixturePackageDetails.Visible = False

                Dim iSeriesProductDate As Integer = Talent.Common.Utilities.CheckForDBNull_Int(fixture.ProductMDTE08.Trim())
                Dim iSeriesTodayDate As Integer = Talent.Common.Utilities.CheckForDBNull_Int(Talent.Common.Utilities.DateToIseriesFormat(Today))

                divFixtureItem.Attributes.Item("class") = divFixtureItem.Attributes.Item("class") & " c-hosp-item__card--select"

                If (fixture.IsSoldOut OrElse (iSeriesProductDate <= iSeriesTodayDate)) Then
                    divFixtureItem.Attributes.Item("class") = divFixtureItem.Attributes.Item("class") & " c-hosp-item__card--unavailable"
                    divFixtureItem.Attributes.Add("title", _ucr.Content("FixtureUnavailableMessage", _languageCode, True))
                End If

            Else
                plhHospitalityBundleFixtureItem.Visible = True
                hlkFixturePackageDetails.Visible = False
                Dim navigateUrl As New StringBuilder

                navigateUrl.Append("../../PagesPublic/Hospitality/HospitalityPackageDetails.aspx?product=")
                navigateUrl.Append(fixture.ProductCode)
                navigateUrl.Append("&packageid=").Append(Request.QueryString("PackageId"))
                navigateUrl.Append("&availabilitycomponentid=").Append(Request.QueryString("availabilitycomponentid"))
                hlkFixturePackageDetails.NavigateUrl = navigateUrl.ToString()
            End If
        End If

    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Setup the hospitality productlist input model and return the model for use
    ''' </summary>
    ''' <returns>The formatted input model based on form data</returns>
    ''' <remarks></remarks>
    Private Function setupInputModelProducts() As HospitalityProductListInputModel
        Dim inputModel As New HospitalityProductListInputModel
        Return inputModel
    End Function

    ''' <summary>
    ''' Create an instance of HospitalityProductGroupInputModel and initialize it
    ''' </summary>
    ''' <returns>Instance of HospitalityProductGroupInputModel</returns>
    ''' <remarks></remarks>
    Private Function setupInputModelProductGroup() As HospitalityProductGroupInputModel
        Dim inputModel As New HospitalityProductGroupInputModel
        Return inputModel
    End Function

    ''' <summary>
    ''' Process the controller, the code actioning the input values if there are any.
    ''' </summary>
    ''' <param name="inputModelProducts">Input Model for Products</param>
    ''' <remarks></remarks>
    Private Sub processController(inputModelProducts As HospitalityProductListInputModel)
        Dim builder As New HospitalityListBuilder
        inputModelProducts.PackageId = PackageId
        inputModelProducts.ProductType = ProductType
        _viewModelProduct = builder.GetHospitalityProductList(inputModelProducts)
    End Sub

    ''' <summary>
    ''' Process the controller, the code actioning the input values if there are any.
    ''' </summary>
    ''' <param name="inputModelProducts">Input Model for Products</param>
    ''' <param name="inputModelProductGroup">Input Model for Products</param>
    ''' <remarks></remarks>
    Private Sub processController(inputModelProducts As HospitalityProductListInputModel, inputModelProductGroup As HospitalityProductGroupInputModel)
        Dim builder As New HospitalityListBuilder
        inputModelProducts.PackageID = PackageId
        inputModelProducts.ProductType = ProductType
        inputModelProductGroup.ProductGroupCode = ProductGroupCode
        _viewModelProductGroup = builder.GetHospitalityProductListByProductGroup(inputModelProducts, inputModelProductGroup)
    End Sub

    ''' <summary>
    ''' Create the view for the page based on the view models given
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub createView()
        _controlViewModel = New BaseViewModel(New UserControlResource, True, "HospitalityFixtures.ascx")
        DisplaySubType = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("SubTypeOnUpcomingFixturesVisible"))
        If Usage.ToUpper = "HOSPITALITYBUNDLEFIXTURES.ASPX" Then
            If _viewModelProductGroup.Error.HasError Then
                blErrorMessages.Items.Add(_controlViewModel.GetControlText("GenericUserControlErrorMessage"))
            Else
                ltlFixturesHeader.Text = _controlViewModel.GetControlText("FixturesAvailableMessageText")
                ltlAddNFixturesToProductGroupMessage.Text = _controlViewModel.GetControlText("AddNFixturesToProductGroupMessage").Replace("<<NumberOfFixtures>>", _viewModelProductGroup.ProductGroupDetailsList(0).MinimumFixturesRequired)
                ltlAddNFixturesToProductGroupMessageFooter.Text = ltlAddNFixturesToProductGroupMessage.Text

                If ProductType = GlobalConstants.HOMEPRODUCTTYPE Then
                    'bind home fixtures
                    rptFixturesHome.DataSource = _viewModelProductGroup.ProductGroupFixturesList
                    rptFixturesHome.DataBind()
                End If
            End If
        Else
            ltlFixturesHeader.Text = _controlViewModel.GetControlText("FixturesHeaderText")

            If ProductType = GlobalConstants.HOMEPRODUCTTYPE Then
                'bind home fixtures
                rptFixturesHome.DataSource = _viewModelProduct.ProductListDetailByPackageId
                rptFixturesHome.DataBind()
            Else
                'bind season fixtures
                rptFixturesSeason.DataSource = _viewModelProduct.ProductListDetailByPackageId
                rptFixturesSeason.DataBind()
            End If
        End If
    End Sub

#End Region

End Class
