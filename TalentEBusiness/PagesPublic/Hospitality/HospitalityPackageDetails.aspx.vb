Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
Imports TalentBusinessLogic.ModelBuilders.Hospitality
Imports TalentBusinessLogic.Models
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class PagesPublic_Hospitality_HospitalityPackageDetails
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _viewModelHospitalityBooking As HospitalityBookingViewModel = Nothing
    Private _viewModelHospitalityDetails As HospitalityDetailsViewModel = Nothing
    Private _pageViewModel As BaseViewModel = Nothing
    Private _businessUnit As String = String.Empty
    Private _partner As String = String.Empty

#End Region

#Region "Public Properties"

    Public Property ProductCode() As String = String.Empty
    Public Property PackageId() As Long = 0
    Public Property AvailabilityComponentID() As String = String.Empty
    Public Property ProductType() As String = String.Empty
#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        _businessUnit = TalentCache.GetBusinessUnit()
        _partner = TalentCache.GetPartner(HttpContext.Current.Profile)
        _pageViewModel = New BaseViewModel(New WebFormResource, True)

        Dim inputModelHospitalityBooking As New HospitalityBookingInputModel
        Dim inputModelHospitalityDetails As New HospitalityDetailsInputModel
        If Request.QueryString("product") IsNot Nothing Then
            ProductCode = Request.QueryString("product").ToString()
        Else
            uscHospitalityPackageHeader.Visible = False
        End If
        If Request.QueryString("packageid") IsNot Nothing Then
            PackageId = TEBUtilities.CheckForDBNull_Long(Request.QueryString("packageid").ToString())
        End If
        If Request.QueryString("productType") IsNot Nothing Then
            ProductType = Request.QueryString("productType").ToString()
        End If
        If Request.QueryString("availabilitycomponentid") IsNot Nothing Then
            AvailabilityComponentID = Request.QueryString("availabilitycomponentid").ToString()
        End If

        inputModelHospitalityBooking = setupHospitalityBookingInputModel()
        inputModelHospitalityDetails = setupHospitalityDetailsInputModel()
        processController(inputModelHospitalityBooking, inputModelHospitalityDetails)
        createView()
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        plhErrorList.Visible = (blErrorMessages.Items.Count > 0)
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Setup the hospitality booking input model and return the model for use
    ''' </summary>
    ''' <returns>The formatted input model</returns>
    ''' <remarks></remarks>
    Private Function setupHospitalityBookingInputModel() As HospitalityBookingInputModel
        Dim inputModel As New HospitalityBookingInputModel
        inputModel.ProductCode = ProductCode
        inputModel.PackageID = PackageId
        inputModel.ProductType = ProductType
        inputModel.SeatComponentID = AvailabilityComponentID
        inputModel.BasketID = Profile.Basket.Basket_Header_ID
        If Profile.IsAnonymous Then
            inputModel.CustomerNumber = GlobalConstants.GENERIC_CUSTOMER_NUMBER
        Else
            inputModel.CustomerNumber = Profile.User.Details.LoginID
        End If
        If IsPostBack Then
            If Request.Params("__EVENTTARGET") = lbtnBookHospitality.UniqueID Then
                inputModel.Quantity01 = txtQuantity.Text
                If Profile.IsAnonymous Then
                    Session("PackageQuantity") = txtQuantity.Text
                End If
            End If
        End If
        Return inputModel
    End Function


    ''' <summary>
    ''' Setup the hospitality details input model and return the model for use
    ''' </summary>
    ''' <returns>The formatted input model</returns>
    ''' <remarks></remarks>
    Private Function setupHospitalityDetailsInputModel() As HospitalityDetailsInputModel
        Dim inputModel As New HospitalityDetailsInputModel
        inputModel.ProductCode = ProductCode
        inputModel.PackageID = PackageId
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
    ''' <param name="inputModelHospitalityBooking">Input Model for the booking</param>
    ''' <param name="inputModelHospitalityDetails">Input model for the details</param>
    ''' <remarks></remarks>
    Private Sub processController(ByVal inputModelHospitalityBooking As HospitalityBookingInputModel, ByVal inputModelHospitalityDetails As HospitalityDetailsInputModel)
        Dim bookingBuilder As New HospitalityBookingBuilder
        Dim detailsBuilder As New HospitalityDetailsBuilder

        blErrorMessages.Items.Clear()
        _viewModelHospitalityDetails = detailsBuilder.GetProductPackageDetails(inputModelHospitalityDetails)

        If IsPostBack Then
            If Request.Params("__EVENTTARGET") = lbtnBookHospitality.UniqueID Then
                If Not CATHelper.IsNotCATRequestOrBasketNotHasCAT(Me.Page.ToString(), "CreateHospitalityBooking", 1) Then
                    Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                End If
                If Profile.IsAnonymous Then
                    Dim redirectUrl As New StringBuilder
                    redirectUrl.Append("~/PagesPublic/Profile/CustomerSelection.aspx?RequiresLogin=True&ReturnUrl=")
                    redirectUrl.Append(Server.UrlEncode(Request.Url.AbsoluteUri))
                    Response.Redirect(redirectUrl.ToString())
                Else
                    Dim quantity As Integer = 0
                    If Integer.TryParse(txtQuantity.Text, quantity) AndAlso quantity > 0 Then
                        _viewModelHospitalityBooking = bookingBuilder.CreateHospitalityBooking(inputModelHospitalityBooking)
                        If _viewModelHospitalityBooking.Error.HasError Then
                            blErrorMessages.Items.Add(_viewModelHospitalityBooking.Error.ErrorMessage)
                        Else
                            Dim redirectUrl As New StringBuilder
                            redirectUrl.Append("~/PagesPublic/Hospitality/HospitalityBooking.aspx?product=").Append(ProductCode).Append("&packageid=").Append(PackageId)
                            Response.Redirect(redirectUrl.ToString())
                        End If
                    End If
                End If
            End If
            If Request.Params("__EVENTTARGET") = lbtnPreBooking.UniqueID Then
                Session("HospitalityPreBookingProductCode") = ProductCode
                Session("HospitalityPreBookingProductDescription") = _viewModelHospitalityDetails.ProductDescription
                Session("HospitalityPreBookingPackageCode") = _viewModelHospitalityDetails.PackageCode
                Session("HospitalityPreBookingPackageDescription") = _viewModelHospitalityDetails.PackageDescription
                Session("HospitalityPreBoookingPackageId") = PackageId
                Session("HospitalityPreBookingAvailabilityComponentId") = AvailabilityComponentID
                Dim redirectUrl As New StringBuilder
                redirectUrl.Append("~/PagesPublic/Hospitality/HospitalityPreBooking.aspx?templateid=").Append(_viewModelHospitalityDetails.DataCaptureTemplateID)
                Response.Redirect(redirectUrl.ToString())
            End If
        End If

    End Sub

    ''' <summary>
    ''' Create the view for the page based on the view models given
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub createView()
        uscHospitalityPackageHeader.ProductCode = ProductCode
        uscHospitalityPackageHeader.PackageId = PackageId
        uscHospitalityFixtures.PackageId = PackageId
        uscHospitalityFixtures.ProductType = ProductType
        uscHospitalityPackageHTMLInclude1.PackageCode = _viewModelHospitalityDetails.PackageCode
        uscHospitalityPackageHTMLInclude1.PackageID = PackageId
        uscHospitalityPackageHTMLInclude1.ProductCode = ProductCode
        uscHospitalityPackageHTMLInclude2.PackageCode = _viewModelHospitalityDetails.PackageCode
        uscHospitalityPackageHTMLInclude2.PackageID = PackageId
        uscHospitalityPackageHTMLInclude2.ProductCode = ProductCode
        uscHospitalityPackageHTMLInclude3.PackageCode = _viewModelHospitalityDetails.PackageCode
        uscHospitalityPackageHTMLInclude3.PackageID = PackageId
        uscHospitalityPackageHTMLInclude3.ProductCode = ProductCode
        uscHospitalityPackageHTMLInclude4.PackageCode = _viewModelHospitalityDetails.PackageCode
        uscHospitalityPackageHTMLInclude4.PackageID = PackageId
        uscHospitalityPackageHTMLInclude4.ProductCode = ProductCode
        If ProductCode.Length > 0 Then
            uscHospitalityPackages.ProductCode = ProductCode
            uscHospitalityPackages.PackageId = PackageId
            plhPackageBookingOptions.Visible = True
            uscHospitalityFixtures.Visible = False
        Else
            plhPackageBookingOptions.Visible = False
            uscHospitalityFixtures.Visible = True
        End If
        ltlPackageDescription.Text = _viewModelHospitalityDetails.PackageDescription
        ltlProductCompeitionDescription.Text = _viewModelHospitalityDetails.ProductCompetitionDescription
        plhProductDate.Visible = Not _viewModelHospitalityDetails.HideDate
        If plhProductDate.Visible Then
            ltlProductDate.Text = _viewModelHospitalityDetails.ProductDate
        End If
        ltlProductDescription.Text = _viewModelHospitalityDetails.ProductDescription
        lblQuantity.Text = _viewModelHospitalityDetails.FormattedPackagePrice
        ltlBookHospitality.Text = _pageViewModel.GetPageText("BookNowButtonText")
        rfvQuantity.ErrorMessage = _pageViewModel.GetPageText("BookNowRequiredErrorText")

        If Not _viewModelHospitalityDetails.AllowBooking Then
            plhBookHospitality.Visible = False
            plhQuantityBox.Visible = False
        Else
            pnlHospitalityPackageDetails.DefaultButton = lbtnBookHospitality.ID
        End If
        If Not _viewModelHospitalityDetails.AllowDataCapture Then
            plhPreBooking.Visible = False
        Else
            ltlHospitalityPreBooking.Text = _pageViewModel.GetPageText("HospitalityPreBookingButtonText")
        End If

        If Not Session("PackageQuantity") Is Nothing Then
            txtQuantity.Text = Session("PackageQuantity")
            Session("PackageQuantity") = Nothing
        End If

        '' View From Area
        Dim viewImgURLForBig As String = viewFromAreaImgURL("\default\" + _viewModelHospitalityDetails.PackageCode)
        Dim viewImgURLForSmall As String = viewFromAreaImgURL("\thumbnail\" + _viewModelHospitalityDetails.PackageCode)
        If String.IsNullOrEmpty(viewImgURLForBig) Then
            plhViewFromArea.Visible = False
        Else
            hlViewFromArea.Text = _pageViewModel.GetPageText("ViewHeaderLabel")
            imgViewAreaSmall.ImageUrl = viewImgURLForSmall
            imgViewAreaBig.ImageUrl = viewImgURLForBig
            imgViewAreaSmall.Attributes.Add("data-open", "view-area")
            hlViewFromArea.Attributes.Add("data-open", "view-area")
        End If
    End Sub

#End Region

End Class