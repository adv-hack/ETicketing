﻿Imports System.Collections.Generic
Imports TalentBusinessLogic.Models
Imports TalentBusinessLogic.ModelBuilders.Hospitality
Imports Talent.Common
Imports TalentBusinessLogic.DataTransferObjects.Hospitality
Imports System.Linq

Partial Class UserControls_Hospitality_HospitalityBundlePackageHeader
    Inherits System.Web.UI.UserControl

#Region "Class Level Fields"

    Private _viewModel As HospitalityProductGroupViewModel = Nothing
    Private _ucr As Talent.Common.UserControlResource = Nothing
    Private _languageCode As String = String.Empty
    Private _settings As Talent.Common.DESettings = Nothing
#End Region

#Region "Public Properties"

    ''' <summary>
    ''' Product group code
    ''' </summary>
    ''' <returns></returns>
    Public Property ProductGroupCode() As String = String.Empty

    'Public Property CallId() As String = String.Empty

#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Me.Visible Then
            Dim inputModelProductGroups As HospitalityProductGroupInputModel = setupInputModelProductGroups()
            processController(inputModelProductGroups)
            createView()
        End If
    End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _languageCode = Utilities.GetDefaultLanguage
        _settings = New DESettings
        _ucr = New UserControlResource
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "HospitalityBundlePackageHeader.ascx"
        End With
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Setup the hospitality product group input model and return the model for use
    ''' </summary>
    ''' <returns>The formatted input model based on form data</returns>
    ''' <remarks></remarks>
    Private Function setupInputModelProductGroups() As HospitalityProductGroupInputModel
        Dim inputModel As New HospitalityProductGroupInputModel
        inputModel.ProductGroupCode = ProductGroupCode
        Return inputModel
    End Function

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Process the controller, the code actioning the input values if there are any.
    ''' </summary>
    ''' <param name="inputModelProductGroups">Product group Input Model</param>
    ''' <remarks></remarks>
    Private Sub processController(ByVal inputModelProductGroups As HospitalityProductGroupInputModel)
        Dim builder As New HospitalityListBuilder
        _viewModel = builder.GetHospitalityProductGroups(inputModelProductGroups)
    End Sub

    ''' <summary>
    ''' Create the view on the user control based on the view model
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub createView()
        'plhBookingID.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_viewModel.GetControlAttribute("BookingRefPanelVisible")) AndAlso Not String.IsNullOrEmpty(CallID)
        'If plhBookingID.Visible Then
        '    ltlBookingRef.Text = _viewModel.GetControlText("BookingRefText").Replace("<<BookingRef>>", CallID)
        'End If

        'ltlProductGroupDescription.Text = ProductGroupDescription

        Dim productGroupList As List(Of ProductGroupDetails) = New List(Of ProductGroupDetails)
        productGroupList = _viewModel.ProductGroupDetailsList

        ltlProductGroupDescription.Text = (From productGroup In productGroupList
                                           Where productGroup.GroupId = ProductGroupCode
                                           Select productGroup.GroupDescription).FirstOrDefault()

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

        imgOpposition.ImageUrl = ProductDetail.GetImageURL("PRODTICKETING", ProductGroupCode)

        If (imgOpposition.ImageUrl.Contains("NOI.gif")) Then
            container.Attributes("class") = "c-hosp-fix c-hosp-fix__noimg"
        End If

    End Sub

#End Region

End Class
