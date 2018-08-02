Imports Talent.eCommerce
Imports Talent.Common

Partial Class UserControls_FriendsAndFamilyOptions
    Inherits ControlBase

#Region "Class Level Fields"

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _ucr As New UserControlResource
    Private _customer As New TalentCustomer
    Private _settings As New DESettings
    Private _err As New ErrorObj

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        _settings.BusinessUnit = TalentCache.GetBusinessUnit()
        _customer.Settings() = _settings
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "FriendsAndFamilyOptions.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If canMoreAssociationsCanBeAdded() Then
            plhAddMyFF.Visible = ModuleDefaults.AddCustomerToMyFriendsAndFamily
            plhAddTheirsFF.Visible = ModuleDefaults.AddMeToTheirMyFriendsAndFamily
            If Talent.eCommerce.Utilities.IsAgent Then
                plhRegisterCustomer.Visible = True
            Else
                plhRegisterCustomer.Visible = ModuleDefaults.RegisterCustomerForFriendsAndFamily
            End If
            ltlErrorLabel.Text = String.Empty
        End If
        SetVisibility()
        SetText()
    End Sub

    Protected Sub AddMyFFButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles AddMyFFButton.Click
        AddCustomerAssociation(Profile.User.Details.LoginID.ToString, CustomerNumberTextBox.Text)
    End Sub

    Protected Sub AddTheirFFButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles AddTheirFFButton.Click
        AddCustomerAssociation(CustomerNumberTextBox.Text, Profile.User.Details.LoginID.ToString)
    End Sub

    Protected Sub RegisterCustomerButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RegisterCustomerButton.Click
        Response.Redirect("~/PagesPublic/Profile/Registration.aspx?source=fandf")
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
        plhErrorMessage.Visible = (ltlErrorLabel.Text.Length > 0)
        plhSuccessMessage.Visible = (ltlAddFFSuccessLabel.Text.Length > 0)
    End Sub

#End Region

#Region "Private Methods"

    Private Sub SetText()
        With _ucr
            ltlSubHeader.Text = .Content("SubHeaderText", _languageCode, True)
            CustomerNumberLabel.Text = .Content("CustomerNumberLabel", _languageCode, True)
            CustomerSurnameLabel.Text = .Content("CustomerSurnameLabel", _languageCode, True)
            CustomerPostCodeLabel.Text = .Content("CustomerPostCodeLabel", _languageCode, True)
            AddMyFFButton.Text = .Content("AddMyFFButton", _languageCode, True)
            AddTheirFFButton.Text = .Content("AddTheirFFButton", _languageCode, True)
            RegisterCustomerButton.Text = .Content("RegisterCustomerButton", _languageCode, True)
            CustomerNumberRFV.ErrorMessage = .Content("CustomerNumberRFV", _languageCode, True)
            CustomerSurNameRFV.ErrorMessage = .Content("CustomerSurNameRFV", _languageCode, True)
            CustomerPostCodeRFV.ErrorMessage = .Content("CustomerPostCodeRFV", _languageCode, True)
            If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(.Attribute("ShowWishListLink")) Then
                hplWishList.Text = .Content("WishListLink", _languageCode, True)
                plhWishList.Visible = True
            Else
                plhWishList.Visible = False
            End If
        End With
    End Sub

    Private Sub SetVisibility()
        ' Note that this attribute now controls whether the post-code is sent to ws027r as 'no-check' and tehrefore no post code check is made.
        If Not Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_ucr.Attribute("postCodeEnableRFV")) Then
            CustomerPostCodeRFV.Enabled = False
            plhPostCode.Visible = False
        End If
    End Sub

    Private Sub AddCustomerAssociation(ByVal CustomerNumber As String, ByVal FriendsAndFamilyId As String)
        Dim deCustV11 As New DECustomerV11
        Dim deCustV1 As New DECustomer
        deCustV11.DECustomersV1.Add(deCustV1)

        'Add the friends and family
        With _customer
            .DeV11 = deCustV11
            ' Set the settings data entity. 
            .Settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .Settings.BusinessUnit = TalentCache.GetBusinessUnit()
            .Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()

            'Set the customer values
            deCustV1.FriendsAndFamilyId = FriendsAndFamilyId
            If CustomerNumber = Profile.User.Details.LoginID.ToString Then
                deCustV1.FriendsAndFamilyMode = "V1"
            Else
                deCustV1.FriendsAndFamilyMode = "V2"
            End If
            deCustV1.CustomerNumber = CustomerNumber
            deCustV1.ContactSurname = CustomerSurnameTextBox.Text
            deCustV1.PostCode = CustomerPostCodeTextBox.Text
            If Not plhPostCode.Visible Then
                deCustV1.PostCode = "no-check"
            End If
            deCustV1.Source = "W"
            .ResultDataSet = Nothing

            'Process
            _err = .AddCustomerAssociation
        End With

        'Did the call complete successfully
        If _err.HasError Or _customer.ResultDataSet Is Nothing Then
            showError("XX")
        Else
            'API error
            If _customer.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                showError(_customer.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode"))
            Else
                If FriendsAndFamilyId = Profile.User.Details.LoginID.ToString Then
                    _customer.CustomerAssociationsClearSessionFromTheirList()
                Else
                    _customer.CustomerAssociationsClearSession()
                End If
                ' Blank out option fields
                CustomerNumberTextBox.Text = String.Empty
                CustomerSurnameTextBox.Text = String.Empty
                CustomerPostCodeTextBox.Text = String.Empty
                'Show Success Message
                ltlAddFFSuccessLabel.Text = _ucr.Content("AddFFSuccess", _languageCode, True)
            End If
        End If
    End Sub

    Private Sub showError(ByVal errCode As String)
        If errCode.Trim = String.Empty Then
            ltlErrorLabel.Text = String.Empty
        Else
            Dim errMsg As New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), _ucr.FrontEndConnectionString)
            Dim message As TalentErrorMessage = errMsg.GetErrorMessage(GlobalConstants.STARALLPARTNER, _ucr.PageCode, errCode)
            ltlErrorLabel.Text = message.ERROR_MESSAGE
        End If
    End Sub

#End Region

#Region "Public Methods"

    Public Sub ValidateCustomer(ByVal EnterredCustomerNumber As String, ByVal EnterredCustomerSurname As String, ByVal EnterredCustomersPostCode As String)
        Dim deCustV11 As New DECustomerV11
        Dim deCustV1 As New DECustomer
        deCustV11.DECustomersV1.Add(deCustV1)
        'Validate the customer detaiuls are correct
        With _customer
            .DeV11 = deCustV11
            deCustV1.CustomerNumber = EnterredCustomerNumber
            deCustV1.ContactSurname = EnterredCustomerSurname
            deCustV1.PostCode = EnterredCustomersPostCode
            _err = .ValidateCustomer
        End With
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Checks the backend WS026R program to see if the customer can add more associated customers to their list
    ''' It hides the Add Customer and Register Customer buttons and displays an error message
    ''' </summary>
    ''' <returns>true/false</returns>
    Private Function canMoreAssociationsCanBeAdded() As Boolean
        Dim moreAssociationsBoolean As Boolean = False
        Dim deCustV11 As New DECustomerV11
        Dim deCustV1 As New DECustomer
        deCustV11.DECustomersV1.Add(deCustV1)

        If Not Profile.IsAnonymous Then
            'Add the friends and family
            With _customer
                .DeV11 = deCustV11
                ' Set the settings data entity. 
                .Settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .Settings.BusinessUnit = TalentCache.GetBusinessUnit()
                .Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
                deCustV1.CustomerNumber = Profile.User.Details.LoginID.ToString()
                deCustV1.Source = "W"
                .ResultDataSet = Nothing

                'Process
                _err = .CanMoreCustomerAssociationsCanBeAdded
            End With

            'Did the call complete successfully
            If _err.HasError Or _customer.ResultDataSet Is Nothing Then
                showError("XX")
            Else
                If _customer.ResultDataSet.Tables.Count = 1 Then
                    If _customer.ResultDataSet.Tables(0).Rows.Count > 0 Then
                        moreAssociationsBoolean = _customer.ResultDataSet.Tables(0).Rows(0).Item("MoreAssociationsAdded")
                    End If
                End If
            End If

            'Hide the buttons if no more assocations can be added
            If Not moreAssociationsBoolean Then
                plhRegisterCustomer.Visible = False
                plhAddMyFF.Visible = False
                Dim errMsg As Talent.Common.TalentErrorMessages
                errMsg = New Talent.Common.TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), _
                                                    ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)
                ltlErrorLabel.Text = errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, Talent.eCommerce.Utilities.GetCurrentPageName, "LT").ERROR_MESSAGE
            End If
        End If

        Return moreAssociationsBoolean
    End Function

#End Region

End Class