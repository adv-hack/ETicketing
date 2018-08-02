Imports System.Data
Imports TalentBusinessLogic.Models
Imports TalentBusinessLogic.ModelBuilders.Hospitality
Imports Talent.eCommerce
Imports Talent.Common

Partial Class UserControls_HospitalityFixturePackageHeader
    Inherits ControlBase

#Region "Class Level Fields"

    Private _viewModel As HospitalityDetailsViewModel = Nothing

#End Region

#Region "Public Properties"

    Public Property ProductCode() As String = String.Empty
    Public Property PackageId() As String = String.Empty
    Public Property CallID() As String = String.Empty

#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Me.Visible Then
            Dim inputModel As HospitalityDetailsInputModel = setupInputModel()
            processController(inputModel)
            createView()
        End If
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Setup the hospitality details input model and return the model for use
    ''' </summary>
    ''' <returns>The formatted input model based on form data</returns>
    ''' <remarks></remarks>
    Private Function setupInputModel() As HospitalityDetailsInputModel
        Dim inputModel As New HospitalityDetailsInputModel
        inputModel.ProductCode = ProductCode
        inputModel.PackageID = PackageId
        Return inputModel
    End Function

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Process the controller, the code actioning the input values if there are any.
    ''' </summary>
    ''' <param name="inputModel">The input model being worked with</param>
    ''' <remarks></remarks>
    Private Sub processController(ByVal inputModel As HospitalityDetailsInputModel)
        Dim builder As New HospitalityDetailsBuilder
        _viewModel = builder.GetProductPackageDetails(inputModel)
    End Sub

    ''' <summary>
    ''' Create the view on the user control based on the view model
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub createView()
        plhBookingID.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_viewModel.GetControlAttribute("BookingRefPanelVisible")) AndAlso Not String.IsNullOrEmpty(CallID)
        If plhBookingID.Visible Then
            ltlBookingRef.Text = _viewModel.GetControlText("BookingRefText").Replace("<<BookingRef>>", CallID)
        End If
        ltlProductDescriptionValue.Text = _viewModel.ProductDescription
        ltlProductDateValue.Text = _viewModel.ProductDate
        ltlProductDateLabel.Text = _viewModel.GetControlText("ProductDateLabel")
        plhProductTime.Visible = (_viewModel.ProductTime.Length > 0 AndAlso _viewModel.HideTime = False)
        If plhProductTime.Visible Then
            ltlProductTimeValue.Text = _viewModel.ProductTime
            ltlProductTimeLabel.Text = _viewModel.GetControlText("ProductTimeLabel")
        End If
        If _viewModel.HideDate = True Then
            plhProductDate.Visible = False
        End If
        If _viewModel.HideTime = True Then
            plhProductTime.Visible = False
        End If
        plhProductCompetitionDesc.Visible = (_viewModel.ProductCompetitionDescription.Length > 0)
        If plhProductCompetitionDesc.Visible Then
            ltlProductCompetitionDescValue.Text = _viewModel.ProductCompetitionDescription
            ltlProductCompetitionDescLabel.Text = _viewModel.GetControlText("ProductCompetitionDescriptionLabel")
        End If
        If PackageId.Length > 0 Then
            plhPackageDescription.Visible = (_viewModel.PackageDescription.Length > 0)
            If plhPackageDescription.Visible Then
                ltlPackageDescriptionValue.Text = _viewModel.PackageDescription
                ltlPackageDescriptionLabel.Text = _viewModel.GetControlText("PackageDescriptionLabel")
            End If
        Else
            plhPackageDescription.Visible = False
        End If
        If Profile.IsAnonymous Then
            plhCustomerName.Visible = False
            plhCompanyName.Visible = False
            plhCustomerAddress1.Visible = False
            plhCustomerMobile.Visible = False
            plhCustomerHome.Visible = False
            plhCustomerWork.Visible = False

        Else
            plhCustomerName.Visible = True
            ltlCustomerNameValue.Text = Profile.User.Details.Full_Name
            ltlCustomerNameLabel.Text = _viewModel.GetControlText("CustomerNameLabel")
            plhCompanyName.Visible = (Profile.User.Details.CompanyName.Length > 0)
            If plhCompanyName.Visible Then
                ltlCompanyNameValue.Text = Profile.User.Details.CompanyName
                ltlCompanyNameLabel.Text = _viewModel.GetControlText("CompanyNameLabel")
            End If

            Dim address As TalentProfileAddress = ProfileHelper.ProfileAddressEnumerator(0, Profile.User.Addresses)

            plhCustomerAddress1.Visible = (address.Address_Line_2.Length > 0)
            If plhCustomerAddress1.Visible Then
                ltlCustomerAddressLabel.Text = _viewModel.GetControlText("CustomerAddressLabel")
                ltlCustomerAddress1.Text = address.Address_Line_2
            End If

            plhCustomerAddress2.Visible = (address.Address_Line_3.Length > 0)
            If plhCustomerAddress2.Visible Then
                ltlCustomerAddress2.Text = address.Address_Line_3
            End If

            plhCustomerCity.Visible = (address.Address_Line_4.Length > 0)
            If plhCustomerCity.Visible Then
                ltlCustomerCity.Text = address.Address_Line_4
            End If

            plhCustomerState.Visible = (address.Address_Line_5.Length > 0)
            If plhCustomerState.Visible Then
                ltlCustomerState.Text = address.Address_Line_5
            End If

            plhCustomerPostCode.Visible = (address.Post_Code.Length > 0)
            If plhCustomerPostCode.Visible Then
                ltlCustomerPostCode.Text = address.Post_Code
            End If

            plhCustomerMobile.Visible = (Profile.User.Details.Mobile_Number.Length > 0)
            If plhCustomerMobile.Visible Then
                ltlCustomerMobileLabel.Text = _viewModel.GetControlText("CustomerMobileLabel")
                ltlCustomerMobile.Text = Profile.User.Details.Mobile_Number
            End If

            plhCustomerHome.Visible = (Profile.User.Details.Telephone_Number.Length > 0)
            If plhCustomerHome.Visible Then
                ltlCustomerHomeLabel.Text = _viewModel.GetControlText("CustomerHomeLabel")
                ltlCustomerHome.Text = Profile.User.Details.Telephone_Number
            End If

            plhCustomerWork.Visible = (Profile.User.Details.Work_Number.Length > 0)
            If plhCustomerWork.Visible Then
                ltlCustomerWorkLabel.Text = _viewModel.GetControlText("CustomerWorkLabel")
                ltlCustomerWork.Text = Profile.User.Details.Work_Number
            End If
        End If
        plhSubTypeDesc.Visible = (Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_viewModel.GetControlAttribute("SubTypeDescVisible")) AndAlso _viewModel.ProductSubTypeDesc.Length > 0)
        If (plhSubTypeDesc.Visible) Then
            ltlSubTypeDescLabel.Text = _viewModel.GetControlText("SubTypeDescLabel")
            ltlSubTypeDescValue.Text = _viewModel.ProductSubTypeDesc
        End If
        imgOpposition.ImageUrl = ProductDetail.GetImageURL("PRODTICKETING", _viewModel.ProductOppositionCode)

        If (imgOpposition.ImageUrl.Contains("NOI.gif")) Then
            container.Attributes("class") = "c-hosp-fix c-hosp-fix__noimg"
        End If

    End Sub

#End Region

End Class
