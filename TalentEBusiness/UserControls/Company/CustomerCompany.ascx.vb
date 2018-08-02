Imports Talent.Common
Imports System.Data
Imports TalentBusinessLogic.ModelBuilders
Imports TalentBusinessLogic.Models
Imports System.Collections.Generic
Imports TalentBusinessLogic.ModelBuilders.CRM

Partial Class UserControls_CustomerCompany
    Inherits ControlBase
    Private _customerCompanyViewModel As CustomerCompanyViewModel
    Private _companyNumber As String = String.Empty
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If AgentProfile.IsAgent Then
            If Session("SelectedCompanyNumber") IsNot Nothing AndAlso Not String.IsNullOrEmpty(Session("SelectedCompanyNumber")) Then
                _companyNumber = Session("SelectedCompanyNumber")
            ElseIf Session("LoggedInCompanyNumber") IsNot Nothing AndAlso Not String.IsNullOrEmpty(Session("LoggedInCompanyNumber")) Then
                _companyNumber = Session("LoggedInCompanyNumber")
            End If

            Dim company As KeyValuePair(Of String, String) = Nothing
            Dim customerCompanyInputModel As New CustomerCompanyInputModel
            customerCompanyInputModel.CompanyOperationMode = GlobalConstants.CRUDOperationMode.Read
            customerCompanyInputModel.CompanyNumber = _companyNumber
            customerCompanyInputModel.ControlCode = "CustomerCompany.ascx"
            Dim companyOperationsBuilder As New CompanyModelBuilders()
            _customerCompanyViewModel = companyOperationsBuilder.ReadCompanyOperations(customerCompanyInputModel)

            If Session("Company") IsNot Nothing Then
                company = CType(Session("Company"), KeyValuePair(Of String, String))
            End If

            ltlCompanyHeader.Text = _customerCompanyViewModel.GetControlText("CustomerCompanyHeaderText")
            ltlCompanyName.Text = _customerCompanyViewModel.GetControlText("CompanyNameText")
            hplAdd.Text = _customerCompanyViewModel.GetControlText("AddButtonText")
            hplDetails.Text = _customerCompanyViewModel.GetControlText("DetailsButtonText")
            If Talent.eCommerce.Utilities.GetCurrentPageName.ToUpper.Contains("COMPANYCONTACTS") AndAlso Session("Company") IsNot Nothing Then
                hplDetails.NavigateUrl = "~/PagesPublic/CRM/CompanyUpdate.aspx?Source=companycontacts&CompanyUpdatePageMode=Update&CompanyNumber=" & company.Key
                hplDetails.Visible = True
                ltlCompanyName.Visible = True
                SetPageElements(company)
            Else
                hplChange.Visible = True
                btnRemove.Visible = True
                hplChange.Text = _customerCompanyViewModel.GetControlText("ChangeButtonText")
                btnRemove.Text = _customerCompanyViewModel.GetControlText("RemoveButtonText")
                If company.Key Is Nothing AndAlso company.Value Is Nothing Then
                    hplChange.Visible = False
                    btnRemove.Visible = False
                    hplAdd.Visible = True
                    ltlCompanyReference.Visible = False
                    ltlCompanyName.Visible = False
                Else
                    SetPageElements(company)
                End If

            End If
        Else
            Me.Visible = False
        End If     
    End Sub
    Protected Sub btnRemove_Click(sender As Object, e As EventArgs) Handles btnRemove.Click
        _companyNumber = Nothing
        hplChange.Visible = False
        btnRemove.Visible = False
        hplAdd.Visible = True
        ltlCompanyReference.Visible = False
        ltlCompanyName.Visible = False
        Session("LoggedInCompanyNumber") = Nothing
        Session("SelectedCompanyNumber") = Nothing
    End Sub
    Private Sub SetPageElements(ByVal company As KeyValuePair(Of String, String))
        If Not String.IsNullOrEmpty(company.Key) Then
            ltlCompanyReference.Text = company.Key
        Else
            ltlCompanyReference.Visible = False
        End If

        If Not String.IsNullOrEmpty(company.Value) Then
            ltlCompanyName.Text = company.Value
        Else
            ltlCompanyName.Visible = False
        End If
        hdfCustomerNumber.Value = CType(HttpContext.Current.Profile, TalentProfile).UserName
        hdfCompanyID.Value = company.Key
        hplChange.NavigateUrl = "~/PagesPublic/Profile/CustomerSelection.aspx?displayMode=ShowCompanySearch&companyColumnMode=SelectOnly&returnTo=/PagesPublic/Profile/Registration.aspx"
    End Sub
End Class
