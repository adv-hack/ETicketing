Imports Talent.Common
Imports System.Data
Imports TalentBusinessLogic.ModelBuilders.CRM
Imports TalentBusinessLogic.Models
Imports System.Collections.Generic

Partial Class PagesAgent_Profile_AddressChange
    Inherits TalentBase01

#Region "Class Level Fields"
    Public _addressChangeInput As AddressChangeInputModel
    Public _addressChangeSyncInput As AddressChangeSyncInputModel
    Public _addressChangeViewModelList As AddressChangeViewModel
    Public _addressChangeSyncViewModelList As AddressChangeSyncViewModel
#End Region

#Region "Public Variables"
    Public OldAddressLabel As String = String.Empty
    Public NewAddressLabel As String = String.Empty
    Public SelectHeader As String = String.Empty
    Public CustomerNumberHeader As String = String.Empty
    Public ForenameHeader As String = String.Empty
    Public SurnameHeader As String = String.Empty
#End Region

#Region "Protected Page Events"
    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init

        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "table-functions.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("table-functions.js", "/Application/Elements/"), False)

        'Create the input as this is required for the builder 
        _AddressChangeInput = New AddressChangeInputModel
        _addressChangeInput.AddressLine1 = Session("oldAddress_Line_1")
        _addressChangeInput.AddressLine2 = Session("oldAddress_Line_2")
        _addressChangeInput.AddressLine3 = Session("oldAddress_Line_3")
        _addressChangeInput.AddressLine4 = Session("oldAddress_Line_4")
        _addressChangeInput.AddressLine5 = Session("oldAddress_Line_5")
        _addressChangeInput.Country = Session("oldCountry")
        _addressChangeInput.PostCode = Session("oldPost_Code")

        'Execute the builder because this creates the view model
        Dim _addressChangeBuilder As New AddressChangeBuilder()
        _addressChangeViewModelList = _addressChangeBuilder.PopulateAddressChangeList(_addressChangeInput)

        PopulateLabels()
        PopulateAddressFields()

        ' Bind the repeater with the people at the same address
        rptAddressChangeResults.DataSource = _addressChangeViewModelList.AddressChange
        rptAddressChangeResults.DataBind()
        rptAddressChangeResults.Visible = True
    End Sub
#End Region


#Region "Private Functions"

    Private Sub PopulateLabels()

        CustomerNumberHeader = _addressChangeViewModelList.GetPageText("CustomerNumberHeader")
        ForenameHeader = _addressChangeViewModelList.GetPageText("ForenameHeader")
        SurnameHeader = _addressChangeViewModelList.GetPageText("SurnameHeader")
        SelectHeader = _addressChangeViewModelList.GetPageText("SelectHeader")

        AddressLine1Label.Text = _addressChangeViewModelList.GetPageText("AddressLine1Label")
        AddressLine2Label.Text = _addressChangeViewModelList.GetPageText("AddressLine2Label")
        AddressLine3Label.Text = _addressChangeViewModelList.GetPageText("AddressLine3Label")
        AddressLine4Label.Text = _addressChangeViewModelList.GetPageText("AddressLine4Label")
        AddressLine5Label.Text = _addressChangeViewModelList.GetPageText("AddressLine5Label")
        CountryLabel.Text = _addressChangeViewModelList.GetPageText("CountryLabel")
        PostcodeLabel.Text = _addressChangeViewModelList.GetPageText("PostCodeLabel")
        btnUpdate.Text = _addressChangeViewModelList.GetPageText("btnUpdate")
        ltlAddressChangeInstructions.Text = _addressChangeViewModelList.GetPageText("ltlAddressChangeInstructions")

    End Sub

    Private Sub PopulateAddressFields()
        newAddressLine1Label.Text = AddressLine1Label.Text
        newAddressLine2Label.Text = AddressLine2Label.Text
        newAddressLine3Label.Text = AddressLine3Label.Text
        newAddressLine4Label.Text = AddressLine4Label.Text
        newAddressLine5Label.Text = AddressLine5Label.Text
        newCountryLabel.Text = CountryLabel.Text
        newPostCodeLabel.Text = PostcodeLabel.Text

        oldAddressLine1.Text = Session("oldAddress_Line_1")
        oldAddressLine2.Text = Session("oldAddress_Line_2")
        oldAddressLine3.Text = Session("oldAddress_Line_3")
        oldAddressLine4.Text = Session("oldAddress_Line_4")
        oldAddressLine5.Text = Session("oldAddress_Line_5")
        oldCountry.Text = Session("oldCountry")
        oldPostcode.Text = Session("oldPost_Code")
        newAddressLine1.Text = Session("newAddress_Line_1")
        newAddressLine2.Text = Session("newAddress_Line_2")
        newAddressLine3.Text = Session("newAddress_Line_3")
        newAddressLine4.Text = Session("newAddress_Line_4")
        newAddressLine5.Text = Session("newAddress_Line_5")
        newPostcode.Text = Session("newPost_Code")
        newCountry.Text = Session("newCountry")


        plholdAddressLine1.Visible = Utilities.CheckForDBNull_Boolean_DefaultTrue(_addressChangeViewModelList.GetPageAttribute("showAddressLine1"))
        plhnewAddressLine1.Visible = Utilities.CheckForDBNull_Boolean_DefaultTrue(_addressChangeViewModelList.GetPageAttribute("showAddressLine1"))
        plholdAddressLine2.Visible = Utilities.CheckForDBNull_Boolean_DefaultTrue(_addressChangeViewModelList.GetPageAttribute("showAddressLine2"))
        plhnewAddressLine2.Visible = Utilities.CheckForDBNull_Boolean_DefaultTrue(_addressChangeViewModelList.GetPageAttribute("showAddressLine2"))
        plholdAddressLine3.Visible = Utilities.CheckForDBNull_Boolean_DefaultTrue(_addressChangeViewModelList.GetPageAttribute("showAddressLine3"))
        plhnewAddressLine3.Visible = Utilities.CheckForDBNull_Boolean_DefaultTrue(_addressChangeViewModelList.GetPageAttribute("showAddressLine3"))
        plholdAddressLine4.Visible = Utilities.CheckForDBNull_Boolean_DefaultTrue(_addressChangeViewModelList.GetPageAttribute("showAddressLine4"))
        plhnewAddressLine4.Visible = Utilities.CheckForDBNull_Boolean_DefaultTrue(_addressChangeViewModelList.GetPageAttribute("showAddressLine4"))
        plholdAddressLine5.Visible = Utilities.CheckForDBNull_Boolean_DefaultTrue(_addressChangeViewModelList.GetPageAttribute("showAddressLine5"))
        plhnewAddressLine5.Visible = Utilities.CheckForDBNull_Boolean_DefaultTrue(_addressChangeViewModelList.GetPageAttribute("showAddressLine5"))
        plhnewCountry.Visible = Utilities.CheckForDBNull_Boolean_DefaultTrue(_addressChangeViewModelList.GetPageAttribute("showCountry"))
        plhnewCountry.Visible = Utilities.CheckForDBNull_Boolean_DefaultTrue(_addressChangeViewModelList.GetPageAttribute("showCountry"))
        plholdPostcode.Visible = Utilities.CheckForDBNull_Boolean_DefaultTrue(_addressChangeViewModelList.GetPageAttribute("showPostcode"))
        plhnewPostcode.Visible = Utilities.CheckForDBNull_Boolean_DefaultTrue(_addressChangeViewModelList.GetPageAttribute("showPostcode"))


    End Sub


    Protected Sub btnUpdate_Click(sender As Object, e As System.EventArgs) Handles btnUpdate.Click
        ErrorList.Items.Clear()
        'Create the input as this is required for the builder 
        _addressChangeSyncInput = New AddressChangeSyncInputModel()

        If Page.IsValid Then
            For Each item As RepeaterItem In rptAddressChangeResults.Items
                Dim cb As CheckBox = item.FindControl("chkSelectedItem")
                If cb.Checked = True Then
                    Dim tb As Label = item.FindControl("lblCustomerNumber")
                    _addressChangeSyncInput.CustomerList += tb.Text.Trim
                End If
            Next
            Dim newAddress As New StringBuilder
            newAddress.Append(Utilities.FixStringLength(newAddressLine2.Text, 30))
            newAddress.Append(Utilities.FixStringLength(newAddressLine3.Text, 30))
            newAddress.Append(Utilities.FixStringLength(newAddressLine4.Text, 25))
            newAddress.Append(Utilities.FixStringLength(newAddressLine5.Text, 25))
            newAddress.Append(Utilities.FixStringLength(newCountry.Text, 20))
            newAddress.Append(Utilities.FixStringLength(newPostcode.Text, 8))
            _addressChangeSyncInput.NewAddress = newAddress.ToString
            _addressChangeSyncInput.CustomerNo = Profile.User.Details.LoginID.ToString

            'Execute the builder because this creates the view model
            Dim _addressChangeBuilder As New AddressChangeBuilder()
            _addressChangeSyncViewModelList = _addressChangeBuilder.SyncAddress(_addressChangeSyncInput)

            If _addressChangeSyncViewModelList.Error.HasError Then
                ErrorList.Items.Add(_addressChangeSyncViewModelList.Error.ErrorMessage)
            Else
                Session("oldAddress_Line_1") = Nothing
                Session("oldAddress_Line_2") = Nothing
                Session("oldAddress_Line_3") = Nothing
                Session("oldAddress_Line_4") = Nothing
                Session("oldAddress_Line_5") = Nothing
                Session("oldCountry") = Nothing
                Session("oldPost_Code") = Nothing
                Session("newAddress_Line_1") = Nothing
                Session("oldAddress_Line_2") = Nothing
                Session("oldAddress_Line_3") = Nothing
                Session("oldAddress_Line_4") = Nothing
                Session("oldAddress_Line_5") = Nothing
                Session("oldCountry") = Nothing
                Session("oldPost_Code") = Nothing
                Response.Redirect("~/PagesLogin/Profile/myAccount.aspx")
            End If



        End If
    End Sub

#End Region
End Class
