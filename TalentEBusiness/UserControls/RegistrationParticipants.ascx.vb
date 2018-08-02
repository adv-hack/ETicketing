Imports System.Data
Imports System.Collections.Generic
Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities

Partial Class UserControls_RegistrationParticipants
    Inherits ControlBase

#Region "Class Level Fields"

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _ucr As UserControlResource = Nothing
    Private _enableDefaultAddress As Boolean = False
    Private _ecomModuleDefaults As ECommerceModuleDefaults.DefaultValues = Nothing
    Private _dateListItems(31) As ListItem
    Private _monthListItems(12) As ListItem
    Private _yearListItems(101) As ListItem
    Private _sexListItems(2) As ListItem
    Private _countryListItemCollection As ListItemCollection = Nothing
    Private _fAndFListItems(1) As ListItem
    Private _rptBindActualData As Boolean = False
    Private _productCode As String = String.Empty
    Private _quantity As Integer = 0
    Private _updateMode As Boolean = False
    Private _isDuplicateMemberFound As Boolean = False
    Private _medicalInfoMaxLength As Integer = 0
    Private _medicalInfoDefaultValue As String = String.Empty
    Private _duplicateCustomerNumber As String = String.Empty

#End Region

#Region "Public Methods"

    Public Sub SetupRequiredValidator(ByVal sender As Object, ByVal e As EventArgs)
        Dim rfv As RequiredFieldValidator = CType(sender, RequiredFieldValidator)
        If CheckForDBNullOrBlank_Boolean_DefaultTrue(_ucr.Attribute(rfv.ControlToValidate & "EnableRFV")) Then
            rfv.Enabled = True
            rfv.ErrorMessage = _ucr.Content(rfv.ControlToValidate & "ErrMessRFV", _languageCode, True)
        Else
            rfv.Enabled = False
        End If
    End Sub

    Public Sub SetupRegExValidator(ByVal sender As Object, ByVal e As EventArgs)
        Dim rev As RegularExpressionValidator = CType(sender, RegularExpressionValidator)
        Dim controlRegExpression As String = _ucr.Attribute(rev.ControlToValidate & "RegExp")
        If String.IsNullOrEmpty(controlRegExpression) Then
            rev.Enabled = False
        Else
            rev.ValidationExpression = controlRegExpression
            rev.ErrorMessage = _ucr.Content(rev.ControlToValidate & "ErrMessREV", _languageCode, True)
        End If
    End Sub

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _ucr = New UserControlResource
        _ecomModuleDefaults = (New ECommerceModuleDefaults).GetDefaults
        _enableDefaultAddress = _ecomModuleDefaults.ParticipantsDefaultAddressInUse
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.Common.Utilities.GetAllString
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "RegistrationParticipants.ascx"
        End With
        phError.Visible = False
        lblError.Text = ""
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        phError.Visible = False
        btnRegister.Text = _ucr.Content("RegisterButton", _languageCode, True)
        btnBack.Text = _ucr.Content("BackButton", _languageCode, True)
        _medicalInfoMaxLength = _ucr.Attribute("MedicalInfoMaxLength")
        _medicalInfoDefaultValue = _ucr.Content("MedicalInfoDefaultValue", _languageCode, True)
        If Not Page.IsPostBack Then
            PopulateHiddenFields()
            PopulateListItems()
            If _updateMode Then
                hdfUpdateMode.Value = "Y"
                GetParticipantsToUpdate()
            Else
                hdfUpdateMode.Value = "N"
                BindRepeater()
            End If
        End If
        Dim jsContent As String = "<script language=JavaScript> function validateMedicalInfoLength(oSrc, args){ "
        jsContent = jsContent & "   args.IsValid = (args.Value.length <= " & _medicalInfoMaxLength & ");   } </script>"
        Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "MedicalInfoValidationScript", jsContent)
    End Sub

    Protected Sub MedicalInfoValidation(ByVal source As System.Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs)
        args.IsValid = (args.Value.Length <= _medicalInfoMaxLength)
    End Sub

    Protected Sub btnRegister_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRegister.Click
        If Page.IsValid Then
            Try
                Dim errorInRegister As Boolean = False
                Dim deCustV11 As DECustomerV11 = BuildParticipantsList()
                If _isDuplicateMemberFound Then
                    showError("DUP")
                Else
                    Dim deAddTicketDetails As New DEAddTicketingItems
                    setTicketingItemsDataEntity(deAddTicketDetails)
                    Dim participantCustomer As New TalentCustomer
                    Dim err As New ErrorObj
                    With participantCustomer.Settings
                        .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                        .BusinessUnit = TalentCache.GetBusinessUnit
                        .Cacheing = False
                        .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
                    End With

                    With participantCustomer
                        .DeV11 = deCustV11
                        .DeAddTicketingItems = deAddTicketDetails
                        err = .SetParticipantsAndBasket()
                    End With

                    If (Not err.HasError) Then
                        'Ticketing Error
                        Dim dtStatus As DataTable = participantCustomer.ResultDataSet.Tables(0)
                        If dtStatus.Rows(0).Item("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                            showError(dtStatus.Rows(0).Item("ReturnCode"))
                            errorInRegister = True
                        Else
                            errorInRegister = False
                        End If
                    Else
                        errorInRegister = True
                    End If

                    If (Not errorInRegister) Then
                        'redirect to basket page
                        Response.Redirect("~/Redirect/TicketingGateway.aspx?page=registrationparticipants.aspx&function=retrievebasket&product=" & Request.QueryString("ProductCode").Trim())
                    Else
                        If participantCustomer.ResultDataSet IsNot Nothing Then
                            Dim dtProcessedParticipants As DataTable = Nothing
                            If participantCustomer.ResultDataSet.Tables.Count > 1 Then
                                dtProcessedParticipants = participantCustomer.ResultDataSet.Tables(1)
                            End If
                            If dtProcessedParticipants.Rows.Count > 0 Then
                                For participantCount As Integer = 0 To dtProcessedParticipants.Rows.Count
                                    deCustV11.DECustomersV1(participantCount).CustomerNumber = dtProcessedParticipants.Rows(participantCount)("CustomerNumber").ToString
                                Next
                            End If
                        End If
                        'Convert customer list to datatable
                        Dim dtParticipantCustomers As New DataTable
                        With dtParticipantCustomers.Columns
                            .Add("CustomerNumber", GetType(String))
                            .Add("ContactForename", GetType(String))
                            .Add("ContactSurname", GetType(String))
                            .Add("DateBirth", GetType(String))
                            .Add("Gender", GetType(String))
                            .Add("EmailAddress", GetType(String))
                            .Add("EmergencyContactName", GetType(String))
                            .Add("EmergencyContactNo", GetType(String))
                            .Add("FanFlag", GetType(String))
                            .Add("MedicalInfo", GetType(String))
                            .Add("AddressLine1", GetType(String))
                            .Add("AddressLine2", GetType(String))
                            .Add("AddressLine3", GetType(String))
                            .Add("AddressLine4", GetType(String))
                            .Add("AddressLine5", GetType(String))
                            .Add("PostCode", GetType(String))
                        End With
                        Dim dRow As DataRow = Nothing
                        For deCustomerCount As Integer = 0 To (deCustV11.DECustomersV1.Count - 1)
                            dRow = dtParticipantCustomers.NewRow
                            dRow("CustomerNo") = deCustV11.DECustomersV1(deCustomerCount).CustomerNumber
                            dRow("ContactForename") = deCustV11.DECustomersV1(deCustomerCount).ContactForename
                            dRow("ContactSurname") = deCustV11.DECustomersV1(deCustomerCount).ContactSurname
                            Dim dateOfBirth As String = deCustV11.DECustomersV1(deCustomerCount).DateBirth
                            If dateOfBirth.Length > 0 Then
                                dateOfBirth = dateOfBirth.Substring(4, 4) & dateOfBirth.Substring(2, 2) & dateOfBirth.Substring(0, 2)
                            End If
                            dRow("DateBirth") = dateOfBirth
                            dRow("Gender") = deCustV11.DECustomersV1(deCustomerCount).Gender
                            dRow("EmailAddress") = deCustV11.DECustomersV1(deCustomerCount).EmailAddress
                            dRow("EmergencyName") = deCustV11.DECustomersV1(deCustomerCount).EmergencyContactName
                            dRow("EmergencyNumber") = deCustV11.DECustomersV1(deCustomerCount).EmergencyContactNumber
                            dRow("FanFlag") = deCustV11.DECustomersV1(deCustomerCount).FanFlag
                            dRow("MedicalInfo") = deCustV11.DECustomersV1(deCustomerCount).MedicalInformation
                            dRow("AddressLine1") = deCustV11.DECustomersV1(deCustomerCount).AddressLine1
                            dRow("AddressLine2") = deCustV11.DECustomersV1(deCustomerCount).AddressLine2
                            dRow("AddressLine3") = deCustV11.DECustomersV1(deCustomerCount).AddressLine3
                            dRow("AddressLine4") = deCustV11.DECustomersV1(deCustomerCount).AddressLine4
                            dRow("AddressLine5") = deCustV11.DECustomersV1(deCustomerCount).AddressLine5
                            dRow("PostCode") = deCustV11.DECustomersV1(deCustomerCount).PostCode
                            dtParticipantCustomers.Rows.Add(dRow)
                        Next
                        PopulateListItems()
                        _rptBindActualData = True
                        rptParticipantsForm.DataSource = dtParticipantCustomers
                        rptParticipantsForm.DataBind()
                    End If

                End If
            Catch ex As Exception

            End Try
        End If

    End Sub

    Protected Sub btnBack_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBack.Click
        Dim redirectURL As String = String.Empty
        If hdfUpdateMode.Value = "Y" Then
            redirectURL = "~/PagesPublic/Basket/Basket.aspx"
        Else
            Select Case hdfProductType.Value
                Case "E" 'event
                    redirectURL = "~/PagesPublic/ProductBrowse/ProductEvent.aspx?ProductSubType=" & hdfProductSubType.Value
                Case "T" 'travel
                    redirectURL = "~/PagesPublic/ProductBrowse/ProductTravel.aspx?ProductSubType=" & hdfProductSubType.Value
                Case "H" 'home
                    redirectURL = "~/PagesPublic/ProductBrowse/ProductHome.aspx?ProductSubType=" & hdfProductSubType.Value
                Case "S" 'season ticket
                    redirectURL = "~/PagesPublic/ProductBrowse/ProductSeason.aspx?ProductSubType=" & hdfProductSubType.Value
                Case "C" 'club
                    redirectURL = "~/PagesPublic/ProductBrowse/ProductMembership.aspx?ProductSubType=" & hdfProductSubType.Value
                Case "A" 'away
                    redirectURL = "~/PagesPublic/ProductBrowse/ProductAway.aspx?ProductSubType=" & hdfProductSubType.Value
                Case "CH" 'corporate hospitality
                    redirectURL = "~/PagesPublic/ProductBrowse/MatchDayHospitality.aspx?ProductSubType=" & hdfProductSubType.Value
                Case Else
                    redirectURL = "~/PagesPublic/Basket/Basket.aspx"
            End Select
        End If
        Response.Redirect(redirectURL)
    End Sub

    Protected Sub rptParticipantsForm_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptParticipantsForm.ItemDataBound
        If e.Item.ItemIndex = -1 Then
        ElseIf e.Item.ItemIndex > -1 Then
            Dim drCustomerDetails As DataRow = CType(e.Item.DataItem, DataRowView).Row
            Dim lblMember As Label = CType(e.Item.FindControl("lblMember"), Label), _
                ddlMember As DropDownList = CType(e.Item.FindControl("ddlMember"), DropDownList), _
                lblForename As Label = CType(e.Item.FindControl("lblForename"), Label), _
                txtForename As TextBox = CType(e.Item.FindControl("txtForename"), TextBox), _
                lblSurname As Label = CType(e.Item.FindControl("lblSurname"), Label), _
                txtSurname As TextBox = CType(e.Item.FindControl("txtSurname"), TextBox), _
                lblDOB As Label = CType(e.Item.FindControl("lblDOB"), Label), _
                ddlDate As DropDownList = CType(e.Item.FindControl("ddlDate"), DropDownList), _
                ddlMonth As DropDownList = CType(e.Item.FindControl("ddlMonth"), DropDownList), _
                ddlYear As DropDownList = CType(e.Item.FindControl("ddlYear"), DropDownList), _
                lblSex As Label = CType(e.Item.FindControl("lblSex"), Label), _
                ddlSex As DropDownList = CType(e.Item.FindControl("ddlSex"), DropDownList), _
                lblEmail As Label = CType(e.Item.FindControl("lblEmail"), Label), _
                txtEmail As TextBox = CType(e.Item.FindControl("txtEmail"), TextBox), _
                phAddressDetails As PlaceHolder = CType(e.Item.FindControl("phAddressDetails"), PlaceHolder), _
                lblAddressLine1 As Label = CType(e.Item.FindControl("lblAddressLine1"), Label), _
                txtAddressLine1 As TextBox = CType(e.Item.FindControl("txtAddressLine1"), TextBox), _
                lblAddressLine2 As Label = CType(e.Item.FindControl("lblAddressLine2"), Label), _
                txtAddressLine2 As TextBox = CType(e.Item.FindControl("txtAddressLine2"), TextBox), _
                lblCity As Label = CType(e.Item.FindControl("lblCity"), Label), _
                txtCity As TextBox = CType(e.Item.FindControl("txtCity"), TextBox), _
                lblCounty As Label = CType(e.Item.FindControl("lblCounty"), Label), _
                txtCounty As TextBox = CType(e.Item.FindControl("txtCounty"), TextBox), _
                lblPostcode As Label = CType(e.Item.FindControl("lblPostcode"), Label), _
                txtPostcode As TextBox = CType(e.Item.FindControl("txtPostcode"), TextBox), _
                lblCountry As Label = CType(e.Item.FindControl("lblCountry"), Label), _
                ddlCountry As DropDownList = CType(e.Item.FindControl("ddlCountry"), DropDownList), _
                lblEmergencyContactName As Label = CType(e.Item.FindControl("lblEmergencyContactName"), Label), _
                txtEmergencyContactName As TextBox = CType(e.Item.FindControl("txtEmergencyContactName"), TextBox), _
                lblEmergencyContactNo As Label = CType(e.Item.FindControl("lblEmergencyContactNo"), Label), _
                txtEmergencyContactNo As TextBox = CType(e.Item.FindControl("txtEmergencyContactNo"), TextBox), _
                lblFan As Label = CType(e.Item.FindControl("lblFan"), Label), _
                chkFan As CheckBox = CType(e.Item.FindControl("chkFan"), CheckBox), _
                lblMedicalInfo As Label = CType(e.Item.FindControl("lblMedicalInfo"), Label), _
                txtMedicalInfo As TextBox = CType(e.Item.FindControl("txtMedicalInfo"), TextBox), _
                cvMedicalInfo As CustomValidator = CType(e.Item.FindControl("cvMedicalInfo"), CustomValidator), _
                btnFetchMember As Button = CType(e.Item.FindControl("btnFetchMember"), Button)

            cvMedicalInfo.ErrorMessage = _ucr.Content(cvMedicalInfo.ControlToValidate & "ErrMessCV", _languageCode, True)

            Dim tempFAndFListItems As New ListItemCollection
            tempFAndFListItems.AddRange(_fAndFListItems)
            ddlMember.DataSource = tempFAndFListItems
            ddlMember.DataValueField = "Value"
            ddlMember.DataTextField = "Text"
            ddlMember.DataBind()
            Dim tempDateListItems As New ListItemCollection
            tempDateListItems.AddRange(_dateListItems)
            ddlDate.DataSource = tempDateListItems
            ddlDate.DataValueField = "Value"
            ddlDate.DataTextField = "Text"
            ddlDate.DataBind()
            Dim tempMonthListItems As New ListItemCollection
            tempMonthListItems.AddRange(_monthListItems)
            ddlMonth.DataSource = tempMonthListItems
            ddlMonth.DataValueField = "Value"
            ddlMonth.DataTextField = "Text"
            ddlMonth.DataBind()
            Dim tempYearListItems As New ListItemCollection
            tempYearListItems.AddRange(_yearListItems)
            ddlYear.DataSource = tempYearListItems
            ddlYear.DataValueField = "Value"
            ddlYear.DataTextField = "Text"
            ddlYear.DataBind()
            Dim tempSexListItems As New ListItemCollection
            tempSexListItems.AddRange(_sexListItems)
            ddlSex.DataSource = tempSexListItems
            ddlSex.DataValueField = "Value"
            ddlSex.DataTextField = "Text"
            ddlSex.DataBind()

            With _ucr
                'Setting MaxLength
                txtForename.MaxLength = .Attribute("ForenameMaxLength")
                txtSurname.MaxLength = .Attribute("SurnameMaxLength")
                txtEmail.MaxLength = .Attribute("EmailMaxLength")
                txtEmergencyContactName.MaxLength = .Attribute("EmergencyContactNameMaxLength")
                txtEmergencyContactNo.MaxLength = .Attribute("EmergencyContactNoMaxLength")
                txtMedicalInfo.MaxLength = _medicalInfoMaxLength

                'Setting Label Display Text
                lblMember.Text = .Content("MemberLabel", _languageCode, True)
                lblForename.Text = .Content("ForenameLabel", _languageCode, True)
                lblSurname.Text = .Content("SurnameLabel", _languageCode, True)
                lblDOB.Text = .Content("DOBLabel", _languageCode, True)
                lblSex.Text = .Content("SexLabel", _languageCode, True)
                lblEmail.Text = .Content("EmailLabel", _languageCode, True)
                lblEmergencyContactName.Text = .Content("EmergencyContactNameLabel", _languageCode, True)
                lblEmergencyContactNo.Text = .Content("EmergencyContactNoLabel", _languageCode, True)
                lblFan.Text = .Content("FanLabel", _languageCode, True)
                lblMedicalInfo.Text = .Content("MedicalInfoLabel", _languageCode, True)
                btnFetchMember.Text = .Content("FetchMemberButton", _languageCode, True)

                'Setting MaxLength and Label Display Text for Address details
                If Not _enableDefaultAddress Then
                    phAddressDetails.Visible = True
                    txtAddressLine1.MaxLength = .Attribute("AddressLine1MaxLength")
                    txtAddressLine2.MaxLength = .Attribute("AddressLine2MaxLength")
                    txtCity.MaxLength = .Attribute("CityMaxLength")
                    txtCounty.MaxLength = .Attribute("CountyMaxLength")
                    txtPostcode.MaxLength = .Attribute("PostcodeMaxLength")

                    lblAddressLine1.Text = .Content("AddressLine1Label", _languageCode, True)
                    lblAddressLine2.Text = .Content("AddressLine2Label", _languageCode, True)
                    lblCity.Text = .Content("CityLabel", _languageCode, True)
                    lblCounty.Text = .Content("CountyLabel", _languageCode, True)
                    lblPostcode.Text = .Content("PostcodeLabel", _languageCode, True)
                    lblCountry.Text = .Content("CountryLabel", _languageCode, True)
                    Dim tempCountryListItems As New ListItemCollection
                    tempCountryListItems = _countryListItemCollection
                    ddlCountry.DataSource = tempCountryListItems
                    ddlCountry.DataValueField = "Value"
                    ddlCountry.DataTextField = "Text"
                    ddlCountry.DataBind()
                Else
                    phAddressDetails.Visible = False
                End If
            End With

            'binding data
            If _rptBindActualData Then
                ddlMember.SelectedIndex = ddlMember.Items.IndexOf(ddlMember.Items.FindByValue(CheckForDBNull_String(drCustomerDetails("CustomerNo"))))
                txtForename.Text = CheckForDBNull_String(drCustomerDetails("ContactForename"))
                txtSurname.Text = CheckForDBNull_String(drCustomerDetails("ContactSurname"))
                Dim customerDOB As String = CheckForDBNull_String(drCustomerDetails("DateBirth")).Trim
                If customerDOB.Length > 0 Then
                    ddlDate.SelectedIndex = ddlDate.Items.IndexOf(ddlDate.Items.FindByValue(CInt(customerDOB.Substring(6, 2))))
                    ddlMonth.SelectedIndex = ddlMonth.Items.IndexOf(ddlMonth.Items.FindByValue(CInt(customerDOB.Substring(4, 2))))
                    ddlYear.SelectedIndex = ddlYear.Items.IndexOf(ddlYear.Items.FindByValue(CInt(customerDOB.Substring(0, 4))))
                End If
                ddlSex.SelectedIndex = ddlSex.Items.IndexOf(ddlSex.Items.FindByValue(CheckForDBNull_String(drCustomerDetails("Gender"))))
                txtEmail.Text = CheckForDBNull_String(drCustomerDetails("EmailAddress")).Trim
                txtEmergencyContactName.Text = CheckForDBNull_String(drCustomerDetails("EmergencyName"))
                txtEmergencyContactNo.Text = CheckForDBNull_String(drCustomerDetails("EmergencyNumber"))
                If CheckForDBNull_String(drCustomerDetails("FanFlag")).Trim.ToUpper() = "Y" Then
                    chkFan.Checked = True
                Else
                    chkFan.Checked = False
                End If
                txtMedicalInfo.Text = CheckForDBNull_String(drCustomerDetails("MedicalInfo"))
                If Not _enableDefaultAddress Then
                    txtAddressLine1.Text = CheckForDBNull_String(drCustomerDetails("AddressLine1"))
                    txtAddressLine2.Text = CheckForDBNull_String(drCustomerDetails("AddressLine2"))
                    txtCity.Text = CheckForDBNull_String(drCustomerDetails("AddressLine3"))
                    txtCounty.Text = CheckForDBNull_String(drCustomerDetails("AddressLine4"))
                    ddlCountry.SelectedIndex = GetCountrySelectedIndex(CheckForDBNull_String(drCustomerDetails("AddressLine5")).Trim.ToLower, ddlCountry)
                    txtPostcode.Text = CheckForDBNull_String(drCustomerDetails("PostCode"))
                End If
                If CInt(CheckForDBNull_Int(drCustomerDetails("CustomerNo"))) > 0 Then
                    'Enable / Disable Controls
                    txtForename.Enabled = False
                    txtSurname.Enabled = False
                    ddlDate.Enabled = False
                    ddlMonth.Enabled = False
                    ddlYear.Enabled = False
                    ddlSex.Enabled = False
                    txtEmail.Enabled = False
                    txtEmergencyContactName.Enabled = True
                    txtEmergencyContactNo.Enabled = True
                    chkFan.Enabled = True
                    txtMedicalInfo.Enabled = True
                    If Not _enableDefaultAddress Then
                        'Enable / Disable Controls
                        txtAddressLine1.Enabled = False
                        txtAddressLine2.Enabled = False
                        txtCity.Enabled = False
                        txtCounty.Enabled = False
                        ddlCountry.Enabled = False
                        txtPostcode.Enabled = False
                    End If
                End If
            Else
                txtMedicalInfo.Text = _medicalInfoDefaultValue
            End If
            If ddlMember.SelectedValue = "" Then
                'Enable / Disable Controls
                txtForename.Enabled = False
                txtSurname.Enabled = False
                ddlDate.Enabled = False
                ddlMonth.Enabled = False
                ddlYear.Enabled = False
                ddlSex.Enabled = False
                txtEmail.Enabled = False
                txtEmergencyContactName.Enabled = False
                txtEmergencyContactNo.Enabled = False
                chkFan.Enabled = False
                txtMedicalInfo.Enabled = False
                If Not _enableDefaultAddress Then
                    'Enable / Disable Controls
                    txtAddressLine1.Enabled = False
                    txtAddressLine2.Enabled = False
                    txtCity.Enabled = False
                    txtCounty.Enabled = False
                    ddlCountry.Enabled = False
                    txtPostcode.Enabled = False
                End If
            End If
        End If

    End Sub

    Protected Sub ddlMember_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim ddlMember As DropDownList = CType(sender, DropDownList)
        Dim rptParticipantsItem As RepeaterItem = CType(ddlMember.Parent, RepeaterItem)
        PopulateMemberDetails(rptParticipantsItem)
    End Sub

    Protected Sub rptParticipantsForm_OnItemCommand(ByVal sender As Object, ByVal e As RepeaterCommandEventArgs) Handles rptParticipantsForm.ItemCommand
        If e.CommandName = "FetchMember" Then
            PopulateMemberDetails(e.Item)
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub PopulateMemberDetails(ByVal rptParticipantsItem As RepeaterItem)
        Dim ddlMember As DropDownList = CType(rptParticipantsItem.FindControl("ddlMember"), DropDownList), _
        txtForename As TextBox = CType(rptParticipantsItem.FindControl("txtForename"), TextBox), _
        txtSurname As TextBox = CType(rptParticipantsItem.FindControl("txtSurname"), TextBox), _
        ddlDate As DropDownList = CType(rptParticipantsItem.FindControl("ddlDate"), DropDownList), _
        ddlMonth As DropDownList = CType(rptParticipantsItem.FindControl("ddlMonth"), DropDownList), _
        ddlYear As DropDownList = CType(rptParticipantsItem.FindControl("ddlYear"), DropDownList), _
        ddlSex As DropDownList = CType(rptParticipantsItem.FindControl("ddlSex"), DropDownList), _
        txtEmail As TextBox = CType(rptParticipantsItem.FindControl("txtEmail"), TextBox), _
        txtEmergencyContactName As TextBox = CType(rptParticipantsItem.FindControl("txtEmergencyContactName"), TextBox), _
        txtEmergencyContactNo As TextBox = CType(rptParticipantsItem.FindControl("txtEmergencyContactNo"), TextBox), _
        chkFan As CheckBox = CType(rptParticipantsItem.FindControl("chkFan"), CheckBox), _
        txtMedicalInfo As TextBox = CType(rptParticipantsItem.FindControl("txtMedicalInfo"), TextBox), _
        txtAddressLine1 = CType(rptParticipantsItem.FindControl("txtAddressLine1"), TextBox), _
        txtAddressLine2 = CType(rptParticipantsItem.FindControl("txtAddressLine2"), TextBox), _
        txtCity = CType(rptParticipantsItem.FindControl("txtCity"), TextBox), _
        txtCounty = CType(rptParticipantsItem.FindControl("txtCounty"), TextBox), _
        txtPostcode = CType(rptParticipantsItem.FindControl("txtPostcode"), TextBox), _
        ddlCountry = CType(rptParticipantsItem.FindControl("ddlCountry"), DropDownList)

        Dim customerNumber As String = ddlMember.SelectedValue.Trim
        If customerNumber.Length > 0 AndAlso customerNumber <> "0" Then

            Dim dtCustomer As DataTable = CustomerRetrieval(customerNumber)

            txtForename.Text = CheckForDBNull_String(dtCustomer.Rows(0)("ContactForename"))
            txtSurname.Text = CheckForDBNull_String(dtCustomer.Rows(0)("ContactSurname"))
            Dim customerDOB As String = CheckForDBNull_String(dtCustomer.Rows(0)("DateBirth"))
            If customerDOB.Length > 0 Then
                ddlDate.SelectedIndex = ddlDate.Items.IndexOf(ddlDate.Items.FindByValue(CInt(customerDOB.Substring(6, 2))))
                ddlMonth.SelectedIndex = ddlMonth.Items.IndexOf(ddlMonth.Items.FindByValue(CInt(customerDOB.Substring(4, 2))))
                ddlYear.SelectedIndex = ddlYear.Items.IndexOf(ddlYear.Items.FindByValue(CInt(customerDOB.Substring(0, 4))))
            End If
            ddlSex.SelectedIndex = ddlSex.Items.IndexOf(ddlSex.Items.FindByValue(CheckForDBNull_String(dtCustomer.Rows(0)("Gender"))))
            txtEmail.Text = CheckForDBNull_String(dtCustomer.Rows(0)("EmailAddress")).Trim
            txtEmergencyContactName.Text = CheckForDBNull_String(dtCustomer.Rows(0)("EmergencyName"))
            txtEmergencyContactNo.Text = CheckForDBNull_String(dtCustomer.Rows(0)("EmergencyNumber"))
            If CheckForDBNull_String(dtCustomer.Rows(0)("FanFlag")).Trim.ToUpper() = "Y" Then
                chkFan.Checked = True
            Else
                chkFan.Checked = False
            End If
            txtMedicalInfo.Text = CheckForDBNull_String(dtCustomer.Rows(0)("MedicalInfo"))
            txtAddressLine1.Text = CheckForDBNull_String(dtCustomer.Rows(0)("AddressLine1"))
            txtAddressLine2.Text = CheckForDBNull_String(dtCustomer.Rows(0)("AddressLine2"))
            txtCity.Text = CheckForDBNull_String(dtCustomer.Rows(0)("AddressLine3"))
            txtCounty.Text = CheckForDBNull_String(dtCustomer.Rows(0)("AddressLine4"))
            ddlCountry.SelectedIndex = GetCountrySelectedIndex(CheckForDBNull_String(dtCustomer.Rows(0)("AddressLine5")).Trim.ToLower, ddlCountry)
            txtPostcode.Text = CheckForDBNull_String(dtCustomer.Rows(0)("PostCode"))

            'Enable / Disable Controls
            txtForename.Enabled = (txtForename.Text = String.Empty)
            txtSurname.Enabled = (txtSurname.Text = String.Empty)
            ddlDate.Enabled = (ddlDate.SelectedValue = String.Empty)
            ddlMonth.Enabled = (ddlMonth.SelectedValue = String.Empty)
            ddlYear.Enabled = (ddlYear.SelectedValue = String.Empty)
            ddlSex.Enabled = (ddlSex.SelectedValue = String.Empty)
            txtEmail.Enabled = (txtEmail.Text = String.Empty)
            txtEmergencyContactName.Enabled = True
            txtEmergencyContactNo.Enabled = True
            txtMedicalInfo.Enabled = True
            chkFan.Enabled = True
            If Not _enableDefaultAddress Then
                'Enable / Disable Controls
                txtAddressLine1.Enabled = (txtAddressLine1.Text = String.Empty)
                txtAddressLine2.Enabled = (txtAddressLine2.Text = String.Empty)
                txtCity.Enabled = (txtCity.Text = String.Empty)
                txtCounty.Enabled = (txtCounty.Text = String.Empty)
                ddlCountry.Enabled = (ddlCountry.SelectedValue = String.Empty)
                txtPostcode.Enabled = (txtPostcode.Text = String.Empty)
            End If
        ElseIf (customerNumber = "0" Or customerNumber = "") Then
            txtForename.Text = ""
            txtSurname.Text = ""
            ddlDate.SelectedIndex = 0
            ddlMonth.SelectedIndex = 0
            ddlYear.SelectedIndex = 0
            ddlSex.SelectedIndex = 0
            txtEmail.Text = ""
            txtEmergencyContactName.Text = ""
            txtEmergencyContactNo.Text = ""
            chkFan.Checked = False
            txtMedicalInfo.Text = ""

            If Not _enableDefaultAddress Then
                txtAddressLine1.Text = ""
                txtAddressLine2.Text = ""
                txtCity.Text = ""
                txtCounty.Text = ""
                ddlCountry.SelectedIndex = 0
                txtPostcode.Text = ""
            End If
            If customerNumber = "" Then
                'Enable / Disable Controls
                txtForename.Enabled = False
                txtSurname.Enabled = False
                ddlDate.Enabled = False
                ddlMonth.Enabled = False
                ddlYear.Enabled = False
                ddlSex.Enabled = False
                txtEmail.Enabled = False
                txtEmergencyContactName.Enabled = False
                txtEmergencyContactNo.Enabled = False
                chkFan.Enabled = False
                txtMedicalInfo.Enabled = False
                txtAddressLine1.Enabled = False
                txtAddressLine2.Enabled = False
                txtCity.Enabled = False
                txtCounty.Enabled = False
                ddlCountry.Enabled = False
                txtPostcode.Enabled = False
            Else
                'Enable / Disable Controls
                txtForename.Enabled = True
                txtSurname.Enabled = True
                ddlDate.Enabled = True
                ddlMonth.Enabled = True
                ddlYear.Enabled = True
                ddlSex.Enabled = True
                txtEmail.Enabled = True
                txtEmergencyContactName.Enabled = True
                txtEmergencyContactNo.Enabled = True
                chkFan.Enabled = True
                txtMedicalInfo.Enabled = True
                'Enable / Disable Controls
                txtAddressLine1.Enabled = True
                txtAddressLine2.Enabled = True
                txtCity.Enabled = True
                txtCounty.Enabled = True
                ddlCountry.Enabled = True
                txtPostcode.Enabled = True
                txtMedicalInfo.Text = _medicalInfoDefaultValue
            End If

        End If
    End Sub

    Private Function BuildParticipantsList() As DECustomerV11

        'Get the default address
        Dim defaultAddressLine1 As String = String.Empty, _
        defaultAddressLine2 As String = String.Empty, _
        defaultCity As String = String.Empty, _
        defaultCounty As String = String.Empty, _
        defaultCountry As String = String.Empty, _
        defaultPostcode As String = String.Empty
        _duplicateCustomerNumber = String.Empty

        If _enableDefaultAddress Then
            defaultAddressLine1 = _ecomModuleDefaults.DefaultAddressLine1
            defaultAddressLine2 = _ecomModuleDefaults.DefaultAddressLine2
            defaultCity = _ecomModuleDefaults.DefaultCity
            defaultCounty = _ecomModuleDefaults.DefaultCounty
            defaultCountry = _ecomModuleDefaults.DefaultCountry
            defaultPostcode = _ecomModuleDefaults.DefaultPostcode
        End If

        Dim myCustomer As New TalentCustomer
        Dim DECustomersList As New DECustomerV11
        Dim DECustomerDetails As DECustomer
        Dim dicCustomerNumbers As Dictionary(Of String, String) = Nothing
        'If hdfUpdateMode.Value = "N" AndAlso Session("DicParticipantMembers") IsNot Nothing Then
        If hdfUpdateMode.Value = "N" Then
            dicCustomerNumbers = New Dictionary(Of String, String)
            For Each tbi As TalentBasketItem In CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems
                If tbi.Product = hdfProductCode.Value Then
                    dicCustomerNumbers.Add(tbi.LOGINID, tbi.LOGINID)
                End If
            Next
            'dicCustomerNumbers = CType(Session("DicParticipantMembers"), Dictionary(Of String, String))
        Else
            dicCustomerNumbers = New Dictionary(Of String, String)
        End If

        _isDuplicateMemberFound = False

        'repeater looping
        For Each rptItem As RepeaterItem In rptParticipantsForm.Items
            Dim ddlMember As DropDownList = CType(rptItem.FindControl("ddlMember"), DropDownList), _
                txtForename As TextBox = CType(rptItem.FindControl("txtForename"), TextBox), _
                txtSurname As TextBox = CType(rptItem.FindControl("txtSurname"), TextBox), _
                ddlDate As DropDownList = CType(rptItem.FindControl("ddlDate"), DropDownList), _
                ddlMonth As DropDownList = CType(rptItem.FindControl("ddlMonth"), DropDownList), _
                ddlYear As DropDownList = CType(rptItem.FindControl("ddlYear"), DropDownList), _
                ddlSex As DropDownList = CType(rptItem.FindControl("ddlSex"), DropDownList), _
                txtEmail As TextBox = CType(rptItem.FindControl("txtEmail"), TextBox), _
                txtEmergencyContactName As TextBox = CType(rptItem.FindControl("txtEmergencyContactName"), TextBox), _
                txtEmergencyContactNo As TextBox = CType(rptItem.FindControl("txtEmergencyContactNo"), TextBox), _
                chkFan As CheckBox = CType(rptItem.FindControl("chkFan"), CheckBox), _
                txtMedicalInfo As TextBox = CType(rptItem.FindControl("txtMedicalInfo"), TextBox), _
                txtAddressLine1 As TextBox = CType(rptItem.FindControl("txtAddressLine1"), TextBox), _
                txtAddressLine2 As TextBox = CType(rptItem.FindControl("txtAddressLine2"), TextBox), _
                txtCity As TextBox = CType(rptItem.FindControl("txtCity"), TextBox), _
                txtCounty As TextBox = CType(rptItem.FindControl("txtCounty"), TextBox), _
                txtPostcode As TextBox = CType(rptItem.FindControl("txtPostcode"), TextBox), _
                ddlCountry As DropDownList = CType(rptItem.FindControl("ddlCountry"), DropDownList)

            DECustomerDetails = New DECustomer

            DECustomerDetails.CustomerNumber = ddlMember.SelectedValue
            DECustomerDetails.ContactForename = txtForename.Text.Trim
            DECustomerDetails.ContactSurname = txtSurname.Text.Trim
            DECustomerDetails.DateBirth = ddlDate.SelectedValue.PadLeft(2, "0") & ddlMonth.SelectedValue.PadLeft(2, "0") & ddlYear.SelectedValue
            DECustomerDetails.Gender = ddlSex.SelectedValue
            DECustomerDetails.EmailAddress = txtEmail.Text.Trim
            DECustomerDetails.EmergencyContactName = txtEmergencyContactName.Text.Trim
            DECustomerDetails.EmergencyContactNumber = txtEmergencyContactNo.Text.Trim
            If chkFan.Checked Then
                DECustomerDetails.FanFlag = "Y"
            Else
                DECustomerDetails.FanFlag = "N"
            End If
            DECustomerDetails.MedicalInformation = txtMedicalInfo.Text.Trim
            DECustomerDetails.AddressLine1 = txtAddressLine1.Text.Trim
            DECustomerDetails.AddressLine2 = txtAddressLine2.Text.Trim
            DECustomerDetails.AddressLine3 = txtCity.Text.Trim
            DECustomerDetails.AddressLine4 = txtCounty.Text.Trim
            DECustomerDetails.AddressLine5 = ddlCountry.SelectedValue.Trim.Trim("-")
            DECustomerDetails.PostCode = txtPostcode.Text.Trim

            If DECustomerDetails.CustomerNumber = "0" Then
                If _enableDefaultAddress Then
                    DECustomerDetails.AddressLine1 = defaultAddressLine1
                    DECustomerDetails.AddressLine2 = defaultAddressLine2
                    DECustomerDetails.AddressLine3 = defaultCity
                    DECustomerDetails.AddressLine4 = defaultCounty
                    DECustomerDetails.AddressLine5 = defaultCountry
                    DECustomerDetails.PostCode = defaultPostcode
                End If
            Else
                If dicCustomerNumbers.ContainsKey(DECustomerDetails.CustomerNumber) Then
                    _isDuplicateMemberFound = True
                    _duplicateCustomerNumber = DECustomerDetails.CustomerNumber
                Else
                    dicCustomerNumbers.Add(DECustomerDetails.CustomerNumber, DECustomerDetails.CustomerNumber)
                End If
            End If

            DECustomersList.DECustomersV1.Add(DECustomerDetails)
            DECustomerDetails = Nothing
            If _isDuplicateMemberFound Then
                Exit For
            End If
        Next
        If Not _isDuplicateMemberFound Then
            Session("DicParticipantMembers") = dicCustomerNumbers
        End If
        dicCustomerNumbers = Nothing
        Return DECustomersList
    End Function

    Private Sub PopulateListItems()

        'Date, Month, Year List Items
        _dateListItems(0) = New ListItem(" -- ", "")
        For dateCount As Integer = 1 To 31
            _dateListItems(dateCount) = New ListItem(dateCount.ToString, dateCount.ToString)
        Next

        'Set Culture for Month Names ComboBox
        Dim myCulture As System.Globalization.CultureInfo
        myCulture = New System.Globalization.CultureInfo(_ecomModuleDefaults.Culture)
        System.Threading.Thread.CurrentThread.CurrentCulture = myCulture
        _monthListItems(0) = New ListItem(" -- ", "")
        For monthCount As Integer = 1 To 12
            _monthListItems(monthCount) = New ListItem(MonthName(monthCount), monthCount)
        Next

        _yearListItems(0) = New ListItem(" -- ", "")
        Dim yearCount As Integer = 1
        For yearValue As Integer = (Year(Now)) To ((Year(Now)) - 100) Step -1
            _yearListItems(yearCount) = New ListItem(yearValue, yearValue)
            yearCount = yearCount + 1
        Next

        'Country List Item Collection
        _countryListItemCollection = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, "REGISTRATION", "COUNTRY")

        'Sex List Items
        _sexListItems(0) = New ListItem(_ucr.Content("SexSelectText", _languageCode, True), "")
        _sexListItems(1) = New ListItem(_ucr.Content("MaleText", _languageCode, True), "M")
        _sexListItems(2) = New ListItem(_ucr.Content("FemaleText", _languageCode, True), "F")

        'Friends and Family List Items
        PopulateFAndFListItems()

    End Sub

    Private Sub PopulateFAndFListItems()
        Dim err As New Talent.Common.ErrorObj
        Dim dtStatus As New DataTable
        Dim dtFAndF As New DataTable
        Dim errorInRetrieve As Boolean = False
        Dim loggedInCustomerNumber As String = Profile.User.Details.LoginID.ToString()
        Dim customer As New TalentCustomer
        Dim deCustV11 As New DECustomerV11
        Dim deCustV1 As New DECustomer
        deCustV11.DECustomersV1.Add(deCustV1)

        With customer
            .DeV11 = deCustV11
            ' Set the settings data entity. 
            .Settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .Settings.BusinessUnit = TalentCache.GetBusinessUnit()
            .Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            .Settings.Cacheing = False
            .Settings.CacheTimeMinutes = CInt(_ucr.Attribute("CacheTimeMinutes"))
            .Settings.CacheDependencyPath = _ecomModuleDefaults.CacheDependencyPath

            'Set the customer values
            deCustV1.CustomerNumber = loggedInCustomerNumber
            deCustV1.Source = "W"
            .ResultDataSet = Nothing

            'Process
            err = .CustomerAssociations
        End With

        'Did the call complete successfully
        If err.HasError Then
            showError("XX")
            errorInRetrieve = True
        Else
            'Ticketing Error
            dtStatus = customer.ResultDataSet.Tables(0)
            If dtStatus.Rows(0).Item("ErrorOccurred") = GlobalConstants.ERRORFLAG AndAlso dtStatus.Rows(0).Item("ReturnCode") <> "NL" Then
                showError(dtStatus.Rows(0).Item("ReturnCode"))
                errorInRetrieve = True
            Else
                'Set the data source to the returned data set
                dtFAndF = customer.ResultDataSet.Tables(1)
                errorInRetrieve = False
            End If
        End If

        If Not errorInRetrieve Then
            Dim arraySize As Integer = dtFAndF.Rows.Count + 2
            ReDim _fAndFListItems(arraySize)
            _fAndFListItems(0) = New ListItem(_ucr.Content("MemberSelectText", _languageCode, True), "")
            _fAndFListItems(1) = New ListItem(_ucr.Content("MemberNewText", _languageCode, True), "0")
            _fAndFListItems(2) = New ListItem(loggedInCustomerNumber, loggedInCustomerNumber)
            Dim fAndFCount As Integer = 3
            For Each drFAndF As DataRow In dtFAndF.Rows
                _fAndFListItems(fAndFCount) = New ListItem(drFAndF("AssociatedCustomerNumber").ToString(), drFAndF("AssociatedCustomerNumber").ToString())
                fAndFCount = fAndFCount + 1
            Next
        End If
    End Sub

    Private Sub showError(ByVal errCode As String)
        phError.Visible = True
        If errCode = "DUP" Then
            lblError.Text = "<ul><li> " & Talent.Common.Utilities.getErrorDescription(_ucr, _languageCode, errCode, False) & " - " & _duplicateCustomerNumber & "</ul>"
        Else
            lblError.Text = Talent.Common.Utilities.getErrorDescription(_ucr, _languageCode, errCode, True)
        End If
    End Sub

    Private Function CustomerRetrieval(ByVal customerNumber As String) As DataTable

        ' Declare this first! Used for Logging function duration
        Dim timeSpan As TimeSpan = Now.TimeOfDay
        Dim dtCustomer As DataTable = Nothing

        Try

            ' Set up the calls to add items to ticketing basket
            Dim talentErrObj As New Talent.Common.ErrorObj
            Dim talentCustomerDetail As New Talent.Common.TalentCustomer
            Dim deCustomerFields As New Talent.Common.DECustomer
            Dim settings As New Talent.Common.DESettings

            ' Set the settings data entity. 
            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settings.BusinessUnit = TalentCache.GetBusinessUnit()
            settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            deCustomerFields.CustomerNumber = customerNumber
            deCustomerFields.Source = "W"

            ' Invoke.
            Dim deCustV11 As New Talent.Common.DECustomerV11
            deCustV11.DECustomersV1.Add(deCustomerFields)
            talentCustomerDetail.DeV11 = deCustV11
            talentCustomerDetail.Settings = settings
            talentErrObj = talentCustomerDetail.CustomerRetrieval

            ' Refresh the front end data on a successful return
            If Not talentErrObj.HasError _
                AndAlso Not talentCustomerDetail.ResultDataSet Is Nothing _
                AndAlso talentCustomerDetail.ResultDataSet.Tables(0).Rows.Count > 0 Then

                Dim dr As DataRow
                ' Has there been an internal error
                dr = talentCustomerDetail.ResultDataSet.Tables(0).Rows(0)
                If dr("ErrorOccurred") <> GlobalConstants.ERRORFLAG AndAlso talentCustomerDetail.ResultDataSet.Tables.Count > 1 Then
                    dtCustomer = talentCustomerDetail.ResultDataSet.Tables(1)

                End If
            End If
        Catch ex As Exception
        End Try

        Talent.eCommerce.Utilities.TalentLogging.LoadTestLog("RegistrationParticipants.ascx.vb", "CustomerRetrieval", timeSpan)
        Return dtCustomer

    End Function

    Private Sub GetParticipantsToUpdate()
        Dim basketDetails As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
        Dim basketItems As DataTable = basketDetails.GetBasketItems_ByHeaderID_Ticketing(Profile.Basket.Basket_Header_ID)
        Dim relatedRows() As DataRow = basketItems.Select("PRODUCT = '" & _productCode & "'")
        Dim customerNumber As String = String.Empty
        Dim dtParticipants As DataTable = Nothing
        If relatedRows.Length > 0 Then
            _quantity = 0
            For rowCount As Integer = 0 To (relatedRows.Length - 1)
                customerNumber = relatedRows(rowCount)("LoginId")
                Dim dtCustomer As DataTable = CustomerRetrieval(customerNumber)
                If rowCount = 0 Then
                    dtParticipants = dtCustomer.Clone()
                End If
                Dim dRow As DataRow = dtCustomer.Rows(0)
                dtParticipants.ImportRow(dRow)
                _quantity = _quantity + 1
            Next
            hdfQuantity.Value = _quantity
            _rptBindActualData = True
            rptParticipantsForm.DataSource = dtParticipants
            rptParticipantsForm.DataBind()
        End If
    End Sub

    Private Sub PopulateHiddenFields()
        If Not (String.IsNullOrEmpty(Request.QueryString("ProductCode"))) Then
            hdfProductCode.Value = Request.QueryString("ProductCode").Trim
            _productCode = Request.QueryString("ProductCode").Trim
        End If
        If Not (String.IsNullOrEmpty(Request.QueryString("productType"))) Then
            hdfProductType.Value = Request.QueryString("productType").Trim
        End If
        If Not (String.IsNullOrEmpty(Request.QueryString("productStadium"))) Then
            hdfProductStadium.Value = Request.QueryString("productStadium").Trim
        End If
        If Not (String.IsNullOrEmpty(Request.QueryString("quantity"))) Then
            hdfQuantity.Value = Request.QueryString("quantity").Trim
            _quantity = CInt(Request.QueryString("quantity").Trim)
        Else
            _updateMode = True
        End If
        If Not (String.IsNullOrEmpty(Request.QueryString("productSubType"))) Then
            hdfProductSubType.Value = Request.QueryString("productSubType").Trim
        End If
        If Not (String.IsNullOrEmpty(Request.QueryString("standCode"))) Then
            hdfStandCode.Value = Request.QueryString("standCode").Trim
        End If
        If Not (String.IsNullOrEmpty(Request.QueryString("areaCode"))) Then
            hdfAreaCode.Value = Request.QueryString("areaCode").Trim
        End If
        If Not (String.IsNullOrEmpty(Request.QueryString("campaignCode"))) Then
            hdfCampaignCode.Value = Request.QueryString("campaignCode").Trim
        End If
        If Not (String.IsNullOrEmpty(Request.QueryString("seat"))) Then
            Dim seatDetails As String = Request.QueryString("seat")
            If seatDetails.Trim.Length > 0 Then
                hdfStandCode.Value = (seatDetails.Substring(0, 3)).Trim
                hdfAreaCode.Value = (seatDetails.Substring(3, 4)).Trim
            End If
        End If
    End Sub

    Private Sub BindRepeater()

        Dim totalRows As Integer = CInt(hdfQuantity.Value)
        Dim dt As New DataTable
        dt.Columns.Add("ID", GetType(String))
        Dim dr As DataRow
        For rowCount As Integer = 1 To totalRows
            dr = Nothing
            dr = dt.NewRow
            dr("ID") = rowCount
            dt.Rows.Add(dr)
        Next

        _rptBindActualData = False
        rptParticipantsForm.DataSource = dt
        rptParticipantsForm.DataBind()

    End Sub

    Private Function GetCountrySelectedIndex(ByVal userCountry As String, ByVal countryDropDown As DropDownList) As Integer

        Try
            Dim i As Integer = 0
            For Each li As ListItem In countryDropDown.Items
                If li.Value.Trim.ToLower = userCountry OrElse li.Text.Trim.ToLower = userCountry Then
                    Return i
                    Exit For
                End If
                i += 1
            Next
        Catch
            Return -1
        End Try
        Return -1

    End Function

    Private Sub setTicketingItemsDataEntity(ByRef deAddTicketingItems As DEAddTicketingItems)
        If Session("PriceBandSelectionOptions") IsNot Nothing Then
            Dim priceBandArray As Array = CType(Session("PriceBandSelectionOptions"), Array)
            Try
                With deAddTicketingItems
                    If (hdfUpdateMode.Value = "Y") Then
                        .UpdateMode = "Y"
                    Else
                        .UpdateMode = "N"
                    End If
                    .ProductCode = hdfProductCode.Value
                    .ProductType = hdfProductType.Value
                    .StandCode = hdfStandCode.Value
                    .AreaCode = hdfAreaCode.Value
                    .Source = "W"
                    .SessionId = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
                    .SignedInCustomer = Profile.User.Details.LoginID.ToString()
                    .PriceBand01 = CStr(priceBandArray(0, 0))
                    .Quantity01 = CStr(priceBandArray(0, 1))
                    .PriceBand02 = CStr(priceBandArray(1, 0))
                    .Quantity02 = CStr(priceBandArray(1, 1))
                    .PriceBand03 = CStr(priceBandArray(2, 0))
                    .Quantity03 = CStr(priceBandArray(2, 1))
                    .PriceBand04 = CStr(priceBandArray(3, 0))
                    .Quantity04 = CStr(priceBandArray(3, 1))
                    .PriceBand05 = CStr(priceBandArray(4, 0))
                    .Quantity05 = CStr(priceBandArray(4, 1))
                    .PriceBand06 = CStr(priceBandArray(5, 0))
                    .Quantity06 = CStr(priceBandArray(5, 1))
                    .PriceBand07 = CStr(priceBandArray(6, 0))
                    .Quantity07 = CStr(priceBandArray(6, 1))
                    .PriceBand08 = CStr(priceBandArray(7, 0))
                    .Quantity08 = CStr(priceBandArray(7, 1))
                    .PriceBand09 = CStr(priceBandArray(8, 0))
                    .Quantity09 = CStr(priceBandArray(8, 1))
                    .PriceBand10 = CStr(priceBandArray(9, 0))
                    .Quantity10 = CStr(priceBandArray(9, 1))
                    .PriceBand11 = CStr(priceBandArray(10, 0))
                    .Quantity11 = CStr(priceBandArray(10, 1))
                    .PriceBand12 = CStr(priceBandArray(11, 0))
                    .Quantity12 = CStr(priceBandArray(11, 1))
                    .PriceBand13 = CStr(priceBandArray(12, 0))
                    .Quantity13 = CStr(priceBandArray(12, 1))
                    .PriceBand14 = CStr(priceBandArray(13, 0))
                    .Quantity14 = CStr(priceBandArray(13, 1))
                    .PriceBand15 = CStr(priceBandArray(14, 0))
                    .Quantity15 = CStr(priceBandArray(14, 1))
                    .PriceBand16 = CStr(priceBandArray(15, 0))
                    .Quantity16 = CStr(priceBandArray(15, 1))
                    .PriceBand17 = CStr(priceBandArray(16, 0))
                    .Quantity17 = CStr(priceBandArray(16, 1))
                    .PriceBand18 = CStr(priceBandArray(17, 0))
                    .Quantity18 = CStr(priceBandArray(17, 1))
                    .PriceBand19 = CStr(priceBandArray(18, 0))
                    .Quantity19 = CStr(priceBandArray(18, 1))
                    .PriceBand20 = CStr(priceBandArray(19, 0))
                    .Quantity20 = CStr(priceBandArray(19, 1))
                    .PriceBand21 = CStr(priceBandArray(20, 0))
                    .Quantity21 = CStr(priceBandArray(20, 1))
                    .PriceBand22 = CStr(priceBandArray(21, 0))
                    .Quantity22 = CStr(priceBandArray(21, 1))
                    .PriceBand23 = CStr(priceBandArray(22, 0))
                    .Quantity23 = CStr(priceBandArray(22, 1))
                    .PriceBand24 = CStr(priceBandArray(23, 0))
                    .Quantity24 = CStr(priceBandArray(23, 1))
                    .PriceBand25 = CStr(priceBandArray(24, 0))
                    .Quantity25 = CStr(priceBandArray(24, 1))
                    .PriceBand26 = CStr(priceBandArray(25, 0))
                    .Quantity26 = CStr(priceBandArray(25, 1))
                End With
            Catch ex As Exception
            End Try
        End If
    End Sub

#End Region



End Class
