Imports Talent.Common
Imports Talent.eCommerce
Imports System.Data
Imports System.Text
Imports System.Web
Imports System.Data.SqlClient
Imports Talent.Common.TalentDataObjects
Imports TCUtilies = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class UserControls_DeliveryAddress
    Inherits ControlBase

#Region "Public Properties"

    Public Property DeliveryAddressValidated As Boolean
        Get
            If Session("deliveryAddressValidated") Is Nothing Then
                Session("deliveryAddressValidated") = False
            Else
                IsDeliveryAddressValidated()
            End If
            Return Session("deliveryAddressValidated")
        End Get
        Set(ByVal value As Boolean)
            Session("deliveryAddressValidated") = value
        End Set
    End Property

#End Region

#Region "Private Variables"
    Private _ucr As UserControlResource = Nothing
    Private _businessUnit As String = Nothing
    Private _partnerCode As String = Nothing
    Private _languageCode As String = Nothing
    Private addressLine1RowVisible As Boolean = True
    Private addressLine2RowVisible As Boolean = True
    Private addressLine3RowVisible As Boolean = True
    Private addressLine4RowVisible As Boolean = True
    Private addressLine5RowVisible As Boolean = True
    Private addressPostcodeRowVisible As Boolean = True
    Private addressCountryRowVisible As Boolean = True
    Private _noDefaultCountrySet As Boolean = True
#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        SetupUCR()
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

        If Not IsPostBack Then

            ' Determine if this control is in use based upon visibility set in parent control
            Dim checkOutControl As Control = Me.Parent
            Dim plhDeliveryAddress As PlaceHolder = CType(checkOutControl.FindControl("plhDeliveryAddress"), PlaceHolder)
            If plhDeliveryAddress.Visible = True Then

                SetAddressFieldsVisibility()
                PopulateLabels()
                PopulateAddressFields()
                PopulateRetailFields()
                SetRetailVisibility()
                SetupValidation()

                ' Restore Delivery Address Information following a completion/payment error
                RestoreDeliveryAddressInformation()

            End If
        End If
        lblRetailAddressInstructions.Visible = (lblRetailAddressInstructions.Text.Length > 0)
        lblTicketingAddressInstructions.Visible = (lblTicketingAddressInstructions.Text.Length > 0)
        lblUseSameAddressAddressInstructions.Visible = (lblUseSameAddressAddressInstructions.Text.Length > 0)
        lblSelectAddress.Visible = (lblSelectAddress.Text.Length > 0)
        lblSelectAddressHelpLabel.Visible = (lblSelectAddressHelpLabel.Text.Length > 0)
        hplFindAddress.Visible = (hplFindAddress.Text.Length > 0)
        DeliveryInsructionsHelpLabel.Visible = (DeliveryInsructionsHelpLabel.Text.Length > 0)
    End Sub

    Protected Sub ddlSelectAddress_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSelectAddress.SelectedIndexChanged
        PopulateCountryDDL()
        SetFieldsForNewAddress()

        If ddlSelectAddress.SelectedValue <> "New Address" Then

            'Populate the address fields using the selected address.
            PopulateAddressfromDDL()

            ' Is it the retail address that has changed
            Dim boolUpdatesRequired As Boolean = False
            If Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
                If chkUseSameAddress.Checked Then
                    boolUpdatesRequired = True
                Else
                    If rdolDisplayOption.SelectedValue = 0 Then
                        boolUpdatesRequired = True
                    End If
                End If
            ElseIf Profile.Basket.BasketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
                boolUpdatesRequired = True
            End If
            If boolUpdatesRequired Then
                ' Update Order address
                UpdateRetailOrder()
                ' Need to reset retail visability as DeliverySelection is dependant upon selected country
                SetRetailVisibility()
            End If

            ' Store the chnaged address in session again if mixed basket and using different addresses
            If Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
                If Not chkUseSameAddress.Checked Then
                    If rdolDisplayOption.SelectedValue = 0 Then
                        StoreRetailAddress()
                    Else
                        StoreTicketingAddress()
                    End If
                End If
            End If
        Else
            ' Store the new address in session again if mixed basket and using different addresses
            If Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
                If Not chkUseSameAddress.Checked Then
                    If rdolDisplayOption.SelectedValue = 0 Then
                        StoreRetailAddress(True)
                    Else
                        StoreTicketingAddress(True)
                    End If
                End If
            End If
        End If
        If Profile.Basket.BasketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
            ' Delivery Message
            PopulateRetailDeliveryMessage()
        End If
        ProcessSummaryForUpdatedBasket()
        Session("ddlSelectAddressChanged") = True
    End Sub

    Protected Sub ddlCountries_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlCountries.SelectedIndexChanged

        ' Is it the retail address that has changed
        Dim boolUpdatesRequired As Boolean = True
        If Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
            If chkUseSameAddress.Checked Then
                boolUpdatesRequired = True
            Else
                Try
                    If rdolDisplayOption.SelectedValue = 0 Then
                        boolUpdatesRequired = True
                    End If
                Catch ex As Exception

                End Try
            End If
        ElseIf Profile.Basket.BasketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
            boolUpdatesRequired = True
        End If

        If boolUpdatesRequired Then

            ' Update Order address
            UpdateRetailOrder()

            ' Delivery Message
            PopulateRetailDeliveryMessage()

            ' Need to reset retail visability as DeliverySelection is dependant upon selected country
            SetRetailVisibility()

        End If
        ProcessSummaryForUpdatedBasket()
    End Sub

    Protected Sub chkUseSameAddress_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkUseSameAddress.CheckedChanged
        If Me.chkUseSameAddress.Checked Then
            Me.rdolDisplayOption.Visible = False
            If lblUseSameAddressAddressInstructions.Text.Trim <> "" Then lblUseSameAddressAddressInstructions.Visible = True
            lblRetailAddressInstructions.Visible = False
            lblTicketingAddressInstructions.Visible = False
            '            Session("DeliveryDetails") = Session("StoredRetailDeliveryAddress")
        Else
            Me.rdolDisplayOption.Visible = True
            Me.rdolDisplayOption.SelectedIndex = 0
            StoreTicketingAddress()
            StoreRetailAddress()
            '            Session("DeliveryDetails") = Session("StoredTicketingDeliveryAddress")
            If lblRetailAddressInstructions.Text.Trim <> "" Then lblRetailAddressInstructions.Visible = True
            lblUseSameAddressAddressInstructions.Visible = False
            lblTicketingAddressInstructions.Visible = False
        End If
        SetRetailVisibility()
    End Sub

    Protected Sub rdolDisplayOption_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdolDisplayOption.SelectedIndexChanged
        Page.Validate("DeliveryAddress")
        If Page.IsValid Then
            Select Case Me.rdolDisplayOption.SelectedIndex
                Case Is = "0"
                    Me.plhRetail.Visible = True
                    StoreTicketingAddress()
                    RestoreRetailAddress()
                    If lblRetailAddressInstructions.Text.Trim <> "" Then lblRetailAddressInstructions.Visible = True
                    lblUseSameAddressAddressInstructions.Visible = False
                    lblTicketingAddressInstructions.Visible = False

                Case Is = "1"
                    Dim errorMessage As String = String.Empty
                    errorMessage = ValidateRetailFields()
                    If errorMessage = String.Empty Then
                        Me.plhRetail.Visible = False
                        StoreRetailAddress()
                        RestoreTicketingAddress()
                        If lblTicketingAddressInstructions.Text.Trim <> "" Then lblTicketingAddressInstructions.Visible = True
                        lblUseSameAddressAddressInstructions.Visible = False
                        lblRetailAddressInstructions.Visible = False

                    Else
                        Me.rdolDisplayOption.SelectedIndex = 0
                        Dim checkOutControl As Control = Me.Parent
                        Dim blErrorMessages As BulletedList = CType(checkOutControl.FindControl("blErrorMessages"), BulletedList)
                        blErrorMessages.Items.Add(errorMessage)
                    End If

            End Select
        Else
            Select Case Me.rdolDisplayOption.SelectedIndex
                Case Is = "0"
                    Me.rdolDisplayOption.SelectedIndex = 1
                Case Is = "1"
                    Me.rdolDisplayOption.SelectedIndex = 0
            End Select

        End If
        If ddlSelectAddress.SelectedValue = "New Address" Then
            SetFieldsForNewAddress(False)
        Else
            SetFieldsForNewAddress()
        End If
    End Sub

    Protected Sub chkGiftMessage_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkGiftMessage.CheckedChanged
        If chkGiftMessage.Checked Then
            plhGiftMessage.Visible = True
        Else
            plhGiftMessage.Visible = False
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub SetupUCR()
        _ucr = New UserControlResource
        _businessUnit = TalentCache.GetBusinessUnit()
        _partnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage()
        With _ucr
            .BusinessUnit = _businessUnit
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .PartnerCode = _partnerCode
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "DeliveryAddress.ascx"
        End With
    End Sub

    Private Sub SetupValidation()
        SetupUCR()

        If plhAddressContactNameRow.Visible Then
            rfvContactName.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("DeliveryContactMissingErrorText", _languageCode, True))
            regexContactName.ValidationExpression = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Attribute("TextOnlyExpression"))
            regexContactName.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("DeliveryContactNameInvalidErrorText", _languageCode, True))
            rfvContactName.Enabled = (rfvContactName.ErrorMessage.Length > 0)
            regexContactName.Enabled = (regexContactName.ValidationExpression.Length > 0)
        End If

        If plhAddressMonikerRow.Visible Then
            rfvMonikerAddress.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("AddressMonikerMissingErrorText", _languageCode, True))
            regexMonikerAddress.ValidationExpression = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Attribute("TextAndNumbersExpression"))
            regexContactName.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("AddressMonikerInvalidErrorText", _languageCode, True))
            rfvMonikerAddress.Enabled = (rfvMonikerAddress.ErrorMessage.Length > 0)
            regexMonikerAddress.Enabled = (regexMonikerAddress.ValidationExpression.Length > 0)
        End If

        If plhAddressLine1Row.Visible Then
            rfvAddress1.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("HouseNoMissingErrorText", _languageCode, True))
            regexAddress1.ValidationExpression = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Attribute("TextAndNumberExpression"))
            regexAddress1.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("HouseNoInvalidErrorText", _languageCode, True))
            rfvAddress1.Enabled = (rfvAddress1.ErrorMessage.Length > 0)
            regexAddress1.Enabled = (regexAddress1.ValidationExpression.Length > 0)
        End If

        If plhAddressLine2Row.Visible Then
            rfvAddress2.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("AddressLine2MissingErrorText", _languageCode, True))
            regexAddress2.ValidationExpression = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Attribute("TextAndNumberExpression"))
            regexAddress2.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("Address2InvalidErrorText", _languageCode, True))
            rfvAddress2.Enabled = (rfvAddress2.ErrorMessage.Length > 0)
            regexAddress2.Enabled = (regexAddress2.ValidationExpression.Length > 0)
        End If

        If plhAddressLine3Row.Visible Then
            rfvTownCity.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("AddressLine3MissingErrorText", _languageCode, True))
            regexTownCity.ValidationExpression = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Attribute("TextAndNumberExpression"))
            regexTownCity.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("Address3InvalidErrorText", _languageCode, True))
            rfvTownCity.Enabled = (rfvTownCity.ErrorMessage.Length > 0)
            regexTownCity.Enabled = (regexTownCity.ValidationExpression.Length > 0)
        End If

        If plhAddressLine4Row.Visible Then
            rfvCounty.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("AddressLine4MissingErrorText", _languageCode, True))
            rfvCounty.Enabled = (rfvCounty.ErrorMessage.Length > 0)
            regexCounty.ValidationExpression = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Attribute("TextAndNumberExpression"))
            regexCounty.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("Address4InvalidErrorText", _languageCode, True))
            regexCounty.Enabled = (regexCounty.ValidationExpression.Length > 0)
        End If

        If plhAddressLine5Row.Visible Then
            rfvAddress5.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("AddressLine5MissingErrorText", _languageCode, True))
            regexAddress5.ValidationExpression = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Attribute("TextAndNumberExpression"))
            regexAddress5.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("Address5InvalidErrorText", _languageCode, True))
            rfvAddress5.Enabled = (rfvAddress5.ErrorMessage.Length > 0)
            regexAddress5.Enabled = (regexAddress5.ValidationExpression.Length > 0)
        End If

        If plhAddressPostCodeRow.Visible Then
            rfvPostCode.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("PostcodeMissingErrorText", _languageCode, True))
            regexPostCode.ValidationExpression = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Attribute("PostcodeExpression"))
            regexPostCode.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("PostcodeInvalidErrorText", _languageCode, True))
            rfvPostCode.Enabled = (rfvPostCode.ErrorMessage.Length > 0)
            regexPostCode.Enabled = (regexPostCode.ValidationExpression.Length > 0)
        End If

        If plhAddressCountryRow.Visible Then
            rfvCountry.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("NoCountrySelectedErrorText", _languageCode, True))
            rfvCountry.Enabled = (rfvCountry.ErrorMessage.Length > 0)
        End If

        If IsRetailVisible() Then
            DeliveryInstructionsRFV.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("DeliveryInstructionsMissingErrorText", _languageCode, True))
            DeliveryInstructionsRegEx.ValidationExpression = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Attribute("TextAndPuctExpression"))
            DeliveryInstructionsRegEx.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("DeliveryInstructionsInvalidErrorText", _languageCode, True))
            DeliveryInstructionsRFV.Enabled = (DeliveryInstructionsRFV.ErrorMessage.Length > 0)
            DeliveryInstructionsRegEx.Enabled = (DeliveryInstructionsRegEx.ValidationExpression.Length > 0)

            PurchaseOrderRFV.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("PurchaseOrderMissingErrorText", _languageCode, True))
            PurchaseOrderRegEx.ValidationExpression = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Attribute("TextAndNumbersExpression"))
            PurchaseOrderRegEx.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("PurchaseOrderInvalidErrorText", _languageCode, True))
            PurchaseOrderRFV.Enabled = (PurchaseOrderRFV.ErrorMessage.Length > 0)
            PurchaseOrderRegEx.Enabled = (PurchaseOrderRegEx.ValidationExpression.Length > 0)
            PurchaseOrder.MaxLength = Talent.eCommerce.Utilities.CheckForDBNull_Int(_ucr.Attribute("PurchaseOrderMaxLength"))

            PreferredDateRegEx.ValidationExpression = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Attribute("PreferredDateExpression"))
            PreferredDateRegEx.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("PreferredDateInvalidErrorText", _languageCode, True))
            PreferredDateRegEx.Enabled = (PreferredDateRegEx.ValidationExpression.Length > 0)

            toRequired.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("toNameRequiredError", _languageCode, True))
            toRequired.Enabled = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_ucr.Attribute("toNameRequiredEnabled"))
            toRegEx.ValidationExpression = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Attribute("toNameRegularExpression"))
            toRegEx.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("toNameInvalidError", _languageCode, True))
            toRegEx.Enabled = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_ucr.Attribute("toNameRegularExpressionEnabled"))

            msgRequired.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("messageRequiredError", _languageCode, True))
            msgRequired.Enabled = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_ucr.Attribute("messageRequiredEnabled"))
            msgRegEx.ValidationExpression = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Attribute("messageRegularExpression"))
            msgRegEx.ErrorMessage = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content("messageInvalidError", _languageCode, True))
            msgRegEx.Enabled = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_ucr.Attribute("messageRegularExpressionEnabled"))
        End If
    End Sub

    Private Sub PopulateLabels()
        With _ucr
            chkUseSameAddress.Text = _ucr.Content("UseSameAddressText", _languageCode, True)
            rdolDisplayOption.Items(0).Text = _ucr.Content("RetailDeliveryAddressText", _languageCode, True)
            rdolDisplayOption.Items(1).Text = _ucr.Content("TicketsDeliveryAddressText", _languageCode, True)
            lblSelectAddress.Text = .Content("SelectAddressLabel", _languageCode, True)
            lblSelectAddressHelpLabel.Text = .Content("SelectAddressHelpLabel", _languageCode, True)
            lblMonikerAddress.Text = .Content("AddressMonikerLabel", _languageCode, True)
            lblAddress1.Text = .Content("Address1Label", _languageCode, True)
            lblAddress2.Text = .Content("Address2Label", _languageCode, True)
            lblTownCity.Text = .Content("Address3Label", _languageCode, True)
            lblCounty.Text = .Content("Address4Label", _languageCode, True)
            lblAddress5.Text = .Content("Address5Label", _languageCode, True)
            lblPostCode.Text = .Content("PostcodeLabel", _languageCode, True)
            lblCountry.Text = .Content("CountryLabel", _languageCode, True)
            lblContactName.Text = .Content("DeliveryContactLabel", _languageCode, True)
            chkSaveTheAddress.Text = .Content("SaveAddressText", _languageCode, True)
            ltlDeliveryAddressInstructions.Text = .Content("DeliveryAddressInstructionsText", _languageCode, True)
            lblUseSameAddressAddressInstructions.Text = .Content("UseSameAddressAddressInstructions", _languageCode, True)
            lblRetailAddressInstructions.Text = .Content("RetailAddressInstructionsText", _languageCode, True)
            lblTicketingAddressInstructions.Text = .Content("TicketingAddressInstructionsText", _languageCode, True)

            If IsRetailVisible() Then
                DeliveryInsructionsLabel.Text = .Content("DeliveryInsructionsLabel", _languageCode, True)
                PurchaseOrderLabel.Text = .Content("PurchaseOrderLabel", _languageCode, True)
                DeliveryDayLabel.Text = .Content("DeliveryDayLabel", _languageCode, True)
                DeliveryDateLabel.Text = .Content("DeliveryDateLabel", _languageCode, True)
                PreferredDateLabel.Text = .Content("PreferredDateLabel", _languageCode, True)
                DeliveryInsructionsHelpLabel.Text = .Content("DeliveryInsructionsHelpLabel", _languageCode, True)
                PurchaseOrderHelpLabel.Text = .Content("PurchaseOrderHelpLabel", _languageCode, True)
                DeliveryDateHelpLabel.Text = .Content("DeliveryDateHelpLabel", _languageCode, True)
                PreferredDateHelpLabel.Text = .Content("PreferredDateHelpLabel", _languageCode, True)
                lblInstallationOption.Text = .Content("InstallationOptionLabel", _languageCode, True)
                lblCollectOption.Text = .Content("CollectOptionLabel", _languageCode, True)
                ' Gift Message
                chkGiftMessage.Text = .Content("chkGiftMessageText", _languageCode, True)
                giftMessageInstructionsLabel.Text = .Content("giftMessageInstructionsLabel", _languageCode, True)
                giftMessageToLabel.Text = .Content("giftMessageToLabel", _languageCode, True)
                giftMessageLabel.Text = .Content("giftMessageLabel", _languageCode, True)

                If TEBUtilities.IsBasketHomeDelivery(Profile) Then
                    lblAddress1.Text = .Content("HomeDelivery_Address1Label", _languageCode, True)
                    lblAddress2.Text = .Content("HomeDelivery_Address2Label", _languageCode, True)
                End If

                'Set Max lengths of input fields
                If (.Attribute("txtAddress1MaxLength", True) IsNot String.Empty) Then
                    txtAddress1.MaxLength = Convert.ToInt32(.Attribute("txtAddress1MaxLength", True))
                End If
                If (.Attribute("txtAddress2MaxLength", True) IsNot String.Empty) Then
                    txtAddress2.MaxLength = Convert.ToInt32(.Attribute("txtAddress2MaxLength", True))
                End If
                If (.Attribute("txtAddress3MaxLength", True) IsNot String.Empty) Then
                    txtTownCity.MaxLength = Convert.ToInt32(.Attribute("txtAddress3MaxLength", True))
                End If
                If (.Attribute("txtAddress4MaxLength", True) IsNot String.Empty) Then
                    txtCounty.MaxLength = Convert.ToInt32(.Attribute("txtAddress4MaxLength", True))
                End If
                If (.Attribute("txtAddress5MaxLength", True) IsNot String.Empty) Then
                    txtAddress5.MaxLength = Convert.ToInt32(.Attribute("txtAddress5MaxLength", True))
                End If
                If (.Attribute("txtPostCodeMaxLength", True) IsNot String.Empty) Then
                    txtPostCode.MaxLength = Convert.ToInt32(.Attribute("txtPostCodeMaxLength", True))
                End If
            End If
        End With
    End Sub

    Private Function IsRetailVisible() As Boolean

        Dim ret As Boolean = False
        Select Case Profile.Basket.BasketContentType
            Case GlobalConstants.COMBINEDBASKETCONTENTTYPE
                ret = True
            Case GlobalConstants.MERCHANDISEBASKETCONTENTTYPE
                ret = True
        End Select
        Return ret

    End Function

    Private Sub SetRetailVisibility()

        plhRetail.Visible = IsRetailVisible()

        If plhRetail.Visible Then

            ' Only show DeliverySelection control if....
            If ModuleDefaults.DeliveryCalculationInUse Then
                Select Case UCase(ModuleDefaults.DeliveryPriceCalculationType)
                    Case "UNIT", "WEIGHT"
                        ' Only show DeliverySelection control if a country is specified on address
                        If Me.ddlCountries.SelectedIndex > 0 Then
                            DeliverySelection1.Display = True
                            Me.DeliverySelection1.Visible = True
                        Else
                            DeliverySelection1.Display = False
                            Me.DeliverySelection1.Visible = False
                        End If
                    Case Else
                        DeliverySelection1.Display = False
                        Me.DeliverySelection1.Visible = False
                End Select
            Else
                DeliverySelection1.Display = False
                Me.DeliverySelection1.Visible = False
            End If

            ' Purchase Order...
            If Not Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("usePurchaseOrder")) Then
                plhPurchaseOrderRow.Visible = False
                PurchaseOrderRFV.EnableClientScript = False
                PurchaseOrderRegEx.EnableClientScript = False
            ElseIf Not CBool(_ucr.Attribute("purchaseOrderRequired")) Then
                PurchaseOrderRFV.EnableClientScript = False
            End If

            ' Delivery Day/Date/Preferred
            plhDeliveryDayRow.Visible = False
            plhDeliveryDateRow.Visible = False
            plhPreferredDateRow.Visible = False
            If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("deliveryDayRowVisible")) Then
                plhDeliveryDayRow.Visible = True
            End If
            If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("deliveryDateRowVisible")) Then
                plhDeliveryDateRow.Visible = True
            End If
            If Profile.PartnerInfo.Details.SHOW_PREFERRED_DELIVERY_DATE Then
                plhPreferredDateRow.Visible = True
            End If

            If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowRetailHomeDeliveryOptions")) AndAlso TEBUtilities.IsBasketHomeDelivery(Profile) Then
                plhRetailHomeDeliveryOptions.Visible = True
                lblDeliveryOptions.Text = _ucr.Content("DeliveryOptionsLabel", _languageCode, True)
                Dim dtCarrier As DataTable = TDataObjects.DeliverySettings.TblCarrier.GetByCarrier(Profile.PartnerInfo.Details.CARRIER_CODE)
                If dtCarrier.Rows.Count = 1 Then
                    plhInstallationOption.Visible = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(dtCarrier.Rows(0)("INSTALLATION_AVAILABLE"))
                    plhCollectionOption.Visible = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(dtCarrier.Rows(0)("COLLECT_OLD_AVAILABLE"))
                Else
                    plhInstallationOption.Visible = False
                    plhCollectionOption.Visible = False
                End If
            Else
                plhRetailHomeDeliveryOptions.Visible = False
            End If

            ' Gift Message
            If ModuleDefaults.Show_Gift_Message_Option Then
                plhGiftMessageRow.Visible = True
            Else
                plhGiftMessageRow.Visible = False
            End If

        End If

    End Sub

    Private Sub SetAddressFieldsVisibility()
        ' Default all address lines and their regex's and rfv's to On
        plhAddressLine1Row.Visible = True
        rfvAddress1.Enabled = True
        regexAddress1.Enabled = True
        plhAddressLine2Row.Visible = True
        rfvAddress2.Enabled = True
        regexAddress2.Enabled = True
        plhAddressLine3Row.Visible = True
        rfvTownCity.Enabled = True
        regexTownCity.Enabled = True
        plhAddressLine4Row.Visible = True
        rfvCounty.Enabled = True
        regexCounty.Enabled = True
        plhAddressLine5Row.Visible = True
        rfvAddress5.Enabled = True
        regexAddress5.Enabled = True
        plhAddressPostCodeRow.Visible = True
        rfvPostCode.Enabled = True
        regexPostCode.Enabled = True
        plhAddressCountryRow.Visible = True
        rfvCountry.Enabled = True

        ' Set common address field visibility using system defaults, and then override by any control-level defaults.
        If Not ModuleDefaults.AddressLine1RowVisible Then addressLine1RowVisible = False
        If Not ModuleDefaults.AddressLine2RowVisible Then addressLine2RowVisible = False
        If Not ModuleDefaults.AddressLine3RowVisible Then addressLine3RowVisible = False
        If Not ModuleDefaults.AddressLine4RowVisible Then addressLine4RowVisible = False
        If Not ModuleDefaults.AddressPostcodeRowVisible Then addressPostcodeRowVisible = False
        If Not ModuleDefaults.AddressCountryRowVisible Then addressCountryRowVisible = False

        '
        ' Control-level overrides for visibility (these DO NOT need to exist on tbl_control_attributes)
        If Not Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("addressLine1RowVisible")) Then
            addressLine1RowVisible = False
        Else
            addressLine1RowVisible = True
        End If
        If Not Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("addressLine2RowVisible")) Then
            addressLine2RowVisible = False
        Else
            addressLine2RowVisible = True
        End If
        If Not Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("addressLine3RowVisible")) Then
            addressLine3RowVisible = False
        Else
            addressLine3RowVisible = True
        End If
        If Not Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("addressLine4RowVisible")) Then
            addressLine4RowVisible = False
        Else
            addressLine4RowVisible = True
        End If
        If Not Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("addressPostcodeRowVisible")) Then
            addressPostcodeRowVisible = False
        Else
            addressPostcodeRowVisible = True
        End If
        If Not Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("addresscountryRowVisible")) Then
            addressCountryRowVisible = False
        Else
            addressCountryRowVisible = True
        End If

        ' Set visibilty of address
        If Not addressLine1RowVisible Then
            plhAddressLine1Row.Visible = False
            rfvAddress1.Enabled = False
            regexAddress1.Enabled = False
        End If
        If Not addressLine2RowVisible Then
            plhAddressLine2Row.Visible = False
            rfvAddress2.Enabled = False
            regexAddress2.Enabled = False
        End If
        If Not addressLine3RowVisible Then
            plhAddressLine3Row.Visible = False
            rfvTownCity.Enabled = False
            regexTownCity.Enabled = False
        End If
        If Not addressLine4RowVisible Then
            plhAddressLine4Row.Visible = False
            rfvCounty.Enabled = False
            regexCounty.Enabled = False
        End If
        If Not addressLine5RowVisible Then
            plhAddressLine5Row.Visible = False
            rfvAddress5.Enabled = False
            regexAddress5.Enabled = False
        End If
        If Not addressPostcodeRowVisible Then
            plhAddressPostCodeRow.Visible = False
            rfvPostCode.Enabled = False
            regexPostCode.Enabled = False
        End If
        If Not addressCountryRowVisible Then
            plhAddressCountryRow.Visible = False
            rfvCountry.Enabled = False
        End If

        ' Address Moniker
        If Not Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("addressMonikerRowVisible")) Then
            addressCountryRowVisible = False
        Else
            addressCountryRowVisible = True
        End If

        ' Radio buttons panel
        plhRadioButtons.Visible = False
        If Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE And ModuleDefaults.AllowPublicTicketingDeliveryAddressSelection Then
            '  Cannot have two different delivery addresses when integrated with ticketing as we only have one payment reference
            If Not Talent.eCommerce.Utilities.IsTicketingDBforRetailOrders Then
                If containsPostOrRegisteredPost() Then
                    plhRadioButtons.Visible = True
                    If lblUseSameAddressAddressInstructions.Text.Trim = "" Then lblUseSameAddressAddressInstructions.Visible = False
                End If
            End If
        End If
        If Me.ltlDeliveryAddressInstructions.Text.Trim = "" Then plhDeliveryAddressInstructions.Visible = False

        ' Set the fields as enabled/disabled depending upon whether adding new address
        SetFieldsForNewAddress()
    End Sub

    Private Sub SetFieldsForNewAddress(Optional ByVal boolSetBlankValues As Boolean = True)

        ' Set the fields as enabled/disabled depending upon whether adding new address
        If ddlSelectAddress.SelectedValue = "New Address" OrElse TEBUtilities.IsBasketHomeDelivery(Profile) Then
            If TEBUtilities.IsBasketHomeDelivery(Profile) Then
                plhAddressMonikerRow.Visible = False
            Else
                plhAddressMonikerRow.Visible = True
            End If
            If addressLine1RowVisible Then txtAddress1.Enabled = True
            If addressLine2RowVisible Then txtAddress2.Enabled = True
            If addressLine3RowVisible Then txtTownCity.Enabled = True
            If addressLine4RowVisible Then txtCounty.Enabled = True
            If addressLine5RowVisible Then txtAddress5.Enabled = True
            If addressCountryRowVisible Then ddlCountries.Enabled = True
            If addressPostcodeRowVisible Then txtPostCode.Enabled = True
            If boolSetBlankValues Then
                txtAddress1.Text = ""
                txtAddress2.Text = ""
                txtTownCity.Text = ""
                txtCounty.Text = ""
                txtAddress5.Text = ""
                If _noDefaultCountrySet Then ddlCountries.SelectedIndex = 0
                If TEBUtilities.IsBasketHomeDelivery(Profile) AndAlso Session("QuickOrderPostCode") IsNot Nothing Then
                    txtPostCode.Text = Session("QuickOrderPostCode")
                Else
                    txtPostCode.Text = ""
                End If
                txtMonikerAddress.Text = ""
            End If
            lblSelectAddressHelpLabel.Visible = False
            SetupValidation()
            ' Rapid addressing (QAS or Hopewiser)
            If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("addressingOnOff")) Then
                plhFindAddressButtonRow.Visible = True
                CreateAddressingJavascript()
                CreateAddressingHiddenFields()
            Else
                plhFindAddressButtonRow.Visible = False
            End If
        Else
            plhAddressMonikerRow.Visible = False
            If TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("MakeAddressFieldsReadOnly")) Then
                If addressLine1RowVisible Then txtAddress1.Enabled = False
                If addressLine2RowVisible Then txtAddress2.Enabled = False
                If addressLine3RowVisible Then txtTownCity.Enabled = False
                If addressLine4RowVisible Then txtCounty.Enabled = False
                If addressLine5RowVisible Then txtAddress5.Enabled = False
                If addressCountryRowVisible Then ddlCountries.Enabled = False
                If addressPostcodeRowVisible Then txtPostCode.Enabled = False

                If addressLine1RowVisible Then rfvAddress1.Enabled = False
                If addressLine2RowVisible Then rfvAddress2.Enabled = False
                If addressLine3RowVisible Then rfvTownCity.Enabled = False
                If addressLine4RowVisible Then rfvCounty.Enabled = False
                If addressLine5RowVisible Then rfvAddress5.Enabled = False
                If addressCountryRowVisible Then rfvCountry.Enabled = False
                If addressPostcodeRowVisible Then rfvPostCode.Enabled = False

                If addressLine1RowVisible Then regexAddress1.Enabled = False
                If addressLine2RowVisible Then regexAddress2.Enabled = False
                If addressLine3RowVisible Then regexTownCity.Enabled = False
                If addressLine4RowVisible Then regexCounty.Enabled = False
                If addressLine5RowVisible Then regexAddress5.Enabled = False
                If addressPostcodeRowVisible Then regexPostCode.Enabled = False
            End If
            If IsRetailVisible() And rdolDisplayOption.SelectedIndex = 0 Then lblSelectAddressHelpLabel.Visible = True
        End If

    End Sub

    Private Sub PopulateAddressFields()

        ' Populate ddlCountries
        PopulateCountryDDL()

        ' Populate ddlSelectAddress and the the address field values
        ' For retail B2B/B2C home delivery there is no previous address to retrieve
        If Not TEBUtilities.IsBasketHomeDelivery(Profile) Then
            PopulateAddressDDL()
            PopulateAddressfromDDL()
        Else
            plhSelectAddressRow.Visible = False
        End If

        'Populate the default Contact Name
        If Profile.IsAnonymous Then
            txtContactName.Text = ""
            Session("StoredDeliveryAddress") = Nothing
            SetFieldsForNewAddress()
        Else
            txtContactName.Text = Profile.User.Details.Full_Name
        End If


    End Sub

    Private Sub PopulateRetailFields()

        If IsRetailVisible() Then
            Dim deliveryDate As Date = Date.MinValue
            If Session("DeliveryDate") IsNot Nothing Then
                deliveryDate = Session("DeliveryDate")
            End If

            PopulateRetailDeliveryMessage()

            Try
                Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
                Dim dt As Data.DataTable = orders.Get_UNPROCESSED_Order(TalentCache.GetBusinessUnit, Profile.UserName)

                If dt.Rows.Count > 0 Then
                    DeliveryInstructions.Text = Talent.eCommerce.Utilities.CheckForDBNull_String(dt.Rows(0)("COMMENT"))
                End If
            Catch ex As Exception
                Logging.WriteLog(Profile.UserName, "DeliveryAddress - 00110", ex.Message, "Error retrieving order header to populate Delivery Details", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "DeliverDetails.ascx")
            End Try

            If ModuleDefaults.RestrictUserPaymentType Then
                ' TODO - Restrict Payment types
            End If

            If ModuleDefaults.Show_Gift_Message_Option Then
                LoadGiftMessage()
            End If

        End If

    End Sub

    Protected Sub LoadGiftMessage()

        Const SelectStr As String = " SELECT * " & _
                                    " FROM tbl_gift_message WITH (NOLOCK)  " & _
                                    " WHERE TEMP_ORDER_ID = @TEMP_ORDER_ID " & _
                                    " AND BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                    " AND PARTNER = @PARTNER "

        Dim cmd As New SqlCommand(SelectStr, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
        Try
            cmd.Connection.Open()
            With cmd.Parameters
                .Clear()
                .Add("@TEMP_ORDER_ID", Data.SqlDbType.NVarChar).Value = Profile.Basket.TempOrderID
                .Add("@BUSINESS_UNIT", Data.SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                .Add("@PARTNER", Data.SqlDbType.NVarChar).Value = TalentCache.GetPartner(Profile)
            End With

            Dim dr As SqlDataReader = cmd.ExecuteReader

            If dr.HasRows Then
                dr.Read()
                toBox.Text = Talent.eCommerce.Utilities.CheckForDBNull_String(dr("RECIPIENT_NAME"))
                msgBox.Text = Talent.eCommerce.Utilities.CheckForDBNull_String(dr("MESSAGE"))
            End If
            dr.Close()
        Catch ex As Exception
        Finally
            cmd.Connection.Close()
        End Try

    End Sub

    Private Function ValidateRetailFields() As String

        Dim errorMessage As String = String.Empty

        ' Delivery Selection
        Dim deliverySelected As Boolean = True
        If ModuleDefaults.DeliveryCalculationInUse Then
            If UCase(ModuleDefaults.DeliveryPriceCalculationType) = "UNIT" OrElse (UCase(ModuleDefaults.DeliveryPriceCalculationType) = "WEIGHT") Then
                If String.IsNullOrEmpty(DeliverySelection1.SelectedDeliveryOption) Then
                    deliverySelected = False
                End If
            End If
        End If
        If deliverySelected Then
            Try
                Dim orderHeaderTA As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
                Dim orderDetailTA As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter
                Try
                    UpdateValues_Standard()
                Catch ex As Exception
                    Throw ex
                End Try

                orderHeaderTA.Update_Order_Status(Talent.Common.Utilities.GetOrderStatus("SUMMARY"), Now, TalentCache.GetBusinessUnit, Profile.UserName, Profile.Basket.TempOrderID)

            Catch ex As Exception
                Logging.WriteLog(Profile.UserName, "UCOSPR-010", ex.Message, "Error updating order status", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "OrderSummary.ascx")
            End Try
            Try
                Dim status As New TalentBasketDatasetTableAdapters.tbl_order_statusTableAdapter
                status.Insert_Order_Status_Flow(TalentCache.GetBusinessUnit, Profile.Basket.TempOrderID, Talent.Common.Utilities.GetOrderStatus("SUMMARY"), Now, "")
            Catch ex As Exception
                Logging.WriteLog(Profile.UserName, "UCOSPR-020", ex.Message, "Error adding order status flow", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "OrderSummary.ascx")
            End Try
        Else
            errorMessage = _ucr.Content("DeliveryTypeNotSelectedText", _languageCode, True)
        End If

        Return errorMessage

    End Function

    Protected Sub UpdateValues_Standard()
        Dim orderHeaderTA As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
        Dim orderDetailTA As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter

        'We are not using the Tax Web Service, so check the values in the standard manner
        Dim promoCode As String = ""
        'Try
        '    Dim promoBox As Control = Utilities.FindWebControl("PromotionsBox", Me.Page.Controls)
        '    promoCode = CallByName(promoBox, "PromotionCode", CallType.Get)
        'Catch ex As Exception
        'End Try


        Dim basketAdapter As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
        Dim liveBasketItems As Data.DataTable = basketAdapter.GetBasketItems_ByHeaderID_NonTicketing( _
                                                   CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)

        Dim defaults As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults
        Dim totals As Talent.Common.TalentWebPricing

        'Select Case defaults.PricingType
        '    Case 2
        '        totals = Utilities.GetPrices_Type2

        '    Case Else
        '        Dim qtyAndCodes As New Generic.Dictionary(Of String, Talent.Common.WebPriceProduct)
        '        For Each bItem As Data.DataRow In liveBasketItems.Rows
        '            If UCase(Utilities.CheckForDBNull_String(bItem("MODULE"))) <> "TICKETING" Then
        '                If Not Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(bItem("IS_FREE")) Then
        '                    If Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(bItem("MASTER_PRODUCT"))) Then
        '                        'Check to see if the multibuys are configured for this master product
        '                        Dim myPrice As Talent.Common.DEWebPrice = Talent.eCommerce.Utilities.GetWebPrices(bItem("MASTER_PRODUCT"), 0, bItem("MASTER_PRODUCT"))
        '                        If myPrice.SALE_PRICE_BREAK_QUANTITY_1 > 0 OrElse myPrice.PRICE_BREAK_QUANTITY_1 > 0 Then
        '                            'multibuys are configured
        '                            If qtyAndCodes.ContainsKey(bItem("MASTER_PRODUCT")) Then
        '                                qtyAndCodes(bItem("MASTER_PRODUCT")).Quantity += Utilities.CheckForDBNull_Decimal(bItem("QUANTITY"))
        '                            Else
        '                                ' Pass in product otherwise Promotions don't work properly
        '                                ' qtyAndCodes.Add(bItem("MASTER_PRODUCT"), New Talent.Common.WebPriceProduct(bItem("MASTER_PRODUCT"), Utilities.CheckForDBNull_Decimal(bItem("QUANTITY")), bItem("MASTER_PRODUCT")))
        '                                qtyAndCodes.Add(bItem("MASTER_PRODUCT"), New Talent.Common.WebPriceProduct(bItem("PRODUCT"), Utilities.CheckForDBNull_Decimal(bItem("QUANTITY")), bItem("MASTER_PRODUCT")))
        '                            End If
        '                        Else
        '                            If Not qtyAndCodes.ContainsKey(Utilities.CheckForDBNull_String(bItem("PRODUCT"))) Then
        '                                qtyAndCodes.Add(Utilities.CheckForDBNull_String(bItem("PRODUCT")), New Talent.Common.WebPriceProduct(Utilities.CheckForDBNull_String(bItem("PRODUCT")), Utilities.CheckForDBNull_Decimal(bItem("QUANTITY")), bItem("MASTER_PRODUCT")))
        '                            End If
        '                        End If
        '                    Else
        '                        If Not qtyAndCodes.ContainsKey(Utilities.CheckForDBNull_String(bItem("PRODUCT"))) Then
        '                            qtyAndCodes.Add(Utilities.CheckForDBNull_String(bItem("PRODUCT")), New Talent.Common.WebPriceProduct(Utilities.CheckForDBNull_String(bItem("PRODUCT")), Utilities.CheckForDBNull_Decimal(bItem("QUANTITY")), Utilities.CheckForDBNull_String(bItem("MASTER_PRODUCT"))))
        '                        End If
        '                    End If
        '                End If
        '            End If
        '        Next

        '        totals = Utilities.GetWebPrices_WithTotals(qtyAndCodes, _
        '                                                    Profile.Basket.TempOrderID, _
        '                                                    defaults.PromotionPriority, _
        '                                                    promoCode)
        'End Select

        totals = Profile.Basket.WebPrices

        If Not totals Is Nothing Then

            Dim delG As Decimal = 0, _
                delN As Decimal = 0, _
                delT As Decimal = 0, _
                delType As String = "", _
                delDesc As String = ""

            If defaults.DeliveryCalculationInUse Then
                Select Case UCase(defaults.DeliveryPriceCalculationType)
                    Case "UNIT", "WEIGHT"
                        Dim dc As New Talent.Common.DEDeliveryCharges.DEDeliveryCharge
                        If defaults.ShowPricesExVAT Then
                            dc = Me.GetSelectedDeliveryCharge(totals.Total_Items_Value_Net, totals.Qualifies_For_Free_Delivery)
                        Else
                            dc = Me.GetSelectedDeliveryCharge(totals.Total_Items_Value_Gross, totals.Qualifies_For_Free_Delivery)
                        End If
                        delG = dc.GROSS_VALUE
                        delN = dc.NET_VALUE
                        delT = dc.TAX_VALUE
                        delType = dc.DELIVERY_TYPE
                        delDesc = dc.DESCRIPTION1
                        If TEBUtilities.IsValidForTaxExemption(Profile.Basket.Temp_Order_Id) Then
                            delG = delN
                            delN = delN
                            delT = 0
                        End If
                    Case Else
                        '---------------------------------------------
                        ' If free delivery is from promotion then keep
                        ' delivery values. This is for balancing on
                        ' the backend.
                        '---------------------------------------------
                        If Not totals.Qualifies_For_Free_Delivery OrElse totals.FreeDeliveryPromotion Then
                            delG = totals.Max_Delivery_Gross
                            delN = totals.Max_Delivery_Net
                            delT = totals.Max_Delivery_Tax
                        End If
                End Select
            End If




            orderHeaderTA.UpdatePriceAndTaxValues(False, False, _
                                                   False, False, _
                                                   False, False, _
                                                   False, False, _
                                                   False, False, _
                                                   "", 0, _
                                                   "", 0, _
                                                   "", 0, _
                                                   "", 0, _
                                                   "", 0, _
                                                   Talent.eCommerce.Utilities.RoundToValue(totals.Total_Items_Value_Gross, 0.01, False), _
                                                   Talent.eCommerce.Utilities.RoundToValue(totals.Total_Items_Value_Net, 0.01, False), _
                                                   Talent.eCommerce.Utilities.RoundToValue(totals.Total_Items_Value_Tax, 0.01, False), _
                                                   Talent.eCommerce.Utilities.RoundToValue(delG, 0.01, False), _
                                                   Talent.eCommerce.Utilities.RoundToValue(delN, 0.01, False), _
                                                   Talent.eCommerce.Utilities.RoundToValue(delT, 0.01, False), _
                                                   Talent.eCommerce.Utilities.RoundToValue(totals.Total_Items_Value_Gross + delG, 0.01, False), _
                                                   delType, _
                                                   delDesc, _
                                                   Profile.Basket.TempOrderID, _
                                                   TalentCache.GetBusinessUnit, _
                                                   Profile.UserName)



            Select Case defaults.PricingType
                Case 2 'Only re-add the order lines if not using pricing type 2
                Case Else
                    Dim orderLines As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter
                    orderLines.Empty_Order(Profile.Basket.TempOrderID)

                    'create an order obj and call the Add_OrderLines_To_Order function
                    '------------------------------------------------------------
                    Dim myOrder As New Talent.eCommerce.Order
                    myOrder.Add_OrderLines_To_Order(totals, Profile.Basket.TempOrderID, TDataObjects.PaymentSettings.GetCurrencyCode(_ucr.BusinessUnit, _ucr.PartnerCode))

                    Try
                        Dim promoVal As Decimal = 0
                        Dim promoPercentage As Decimal = 0
                        If Not totals Is Nothing Then
                            promoVal = totals.Total_Promotions_Value
                            If Not totals.PromotionsResultsTable Is Nothing _
                                AndAlso totals.PromotionsResultsTable.Rows.Count > 0 Then
                                For Each promo As Data.DataRow In totals.PromotionsResultsTable.Rows
                                    If CDec(promo("PromotionPercentageValue")) > 0 Then
                                        promoPercentage = CDec(promo("PromotionPercentageValue"))
                                        Exit For
                                    End If
                                Next
                            End If
                        End If
                        orderHeaderTA.Set_Promotion_Value(Talent.eCommerce.Utilities.RoundToValue(promoVal, 0.01, False), promoPercentage, Profile.Basket.TempOrderID)
                        orderLines.Update_Promotion_Line_Values(0, promoPercentage, Profile.Basket.TempOrderID)
                    Catch ex As Exception
                    End Try
            End Select

        End If

    End Sub

    Protected Function GetSelectedDeliveryCharge(ByVal totalOrder As Decimal, ByVal freeDel As Boolean) As Talent.Common.DEDeliveryCharges.DEDeliveryCharge
        Dim dcs As Talent.Common.DEDeliveryCharges = Nothing
        Select Case ModuleDefaults.DeliveryPriceCalculationType
            Case "UNIT"
                dcs = Talent.eCommerce.Utilities.GetDeliveryOptions(totalOrder, freeDel)
            Case "WEIGHT"
                Dim countryCode As String = String.Empty
                Dim countryName As String = String.Empty
                Talent.eCommerce.Utilities.GetDeliveryCountry(Profile.Basket.Temp_Order_Id, countryCode, countryName)
                dcs = Talent.eCommerce.Utilities.GetDeliveryOptions(Talent.eCommerce.Utilities.CheckForDBNull_Int(_ucr.Attribute("CacheTimeMinutes")), totalOrder, freeDel, Talent.eCommerce.Utilities.GetBasketItemsTotalWeight(Profile.Basket.BasketItems), countryCode, countryName)
        End Select

        Return dcs.GetDeliveryCharge(DeliverySelection1.SelectedDeliveryOption)
    End Function

    Private Function UpdateRetailOrder() As Integer

        Dim sContactName As String = String.Empty
        Dim sAddress1 As String = String.Empty
        Dim sAddress2 As String = String.Empty
        Dim sTownCity As String = String.Empty
        Dim sCounty As String = String.Empty
        Dim sAddress5 As String = String.Empty
        Dim sPostCode As String = String.Empty
        Dim sCountry As String = String.Empty
        Dim sCountryCode As String = String.Empty
        Dim boolGiftMessage As Boolean = False
        Dim sDeliveryInstructions As String = String.Empty
        Dim sPurchaseOrder As String = String.Empty
        Dim setRetailHomeDeliveryOptions As Boolean = False
        If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowRetailHomeDeliveryOptions")) AndAlso TEBUtilities.IsBasketHomeDelivery(Profile) Then
            setRetailHomeDeliveryOptions = True
        End If

        ' Use on-screen value by default
        sContactName = Me.txtContactName.Text
        sAddress1 = Me.txtAddress1.Text
        sAddress2 = Me.txtAddress2.Text
        sTownCity = Me.txtTownCity.Text
        sCounty = Me.txtCounty.Text
        sAddress5 = Me.txtAddress5.Text
        sPostCode = Me.txtPostCode.Text
        sCountry = Me.ddlCountries.SelectedItem.Text
        boolGiftMessage = Me.chkGiftMessage.Checked
        sDeliveryInstructions = Me.DeliveryInstructions.Text
        sPurchaseOrder = Me.PurchaseOrder.Text
        sCountryCode = Me.ddlCountries.SelectedItem.Value

        ' Use saved version if mixed basket and not suing same address and not currently displaying retail address
        If Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
            If chkUseSameAddress.Checked Then
                ' Use on screen values
            Else
                If rdolDisplayOption.SelectedValue = 0 Then
                    ' Use on screen values 
                Else
                    ' Use value stored in session
                    Dim deliveryDetails As New DEDeliveryDetails
                    deliveryDetails = Session("StoredRetailDeliveryAddress")
                    sContactName = deliveryDetails.ContactName
                    sAddress1 = deliveryDetails.Address1
                    sAddress2 = deliveryDetails.Address2
                    sTownCity = deliveryDetails.Address3
                    sCounty = deliveryDetails.Address4
                    sAddress5 = deliveryDetails.Address5
                    sCountry = deliveryDetails.Country
                    sPostCode = deliveryDetails.Postcode
                    sCountryCode = deliveryDetails.CountryCode
                End If
            End If
        End If

        ' Save Gift Message
        If chkGiftMessage.Checked Then SaveGiftMessage()

        ' Set external ID if available
        Dim addressExternalID As String = String.Empty
        Try
            addressExternalID = Profile.User.Addresses(ddlSelectAddress.SelectedItem.Text).External_ID
        Catch ex As Exception
            addressExternalID = ""
        End Try


        Dim ret As Integer = TDataObjects.OrderSettings.TblOrderHeader.UpdateOrder(Profile.Basket.Temp_Order_Id, _
                              sContactName, sAddress1, sAddress2, sTownCity, sCounty, sAddress5, sPostCode, _
                              sCountry, boolGiftMessage, sDeliveryInstructions, sPurchaseOrder, addressExternalID, sCountryCode, _
                              setRetailHomeDeliveryOptions, chkInstallationOption.Checked, chkCollectOption.Checked)

        Try
            Dim uscDeliverySelection As UserControls_DeliverySelection = CType(TEBUtilities.FindWebControl("DeliverySelection1", Me.Page.Controls), UserControls_DeliverySelection)
            uscDeliverySelection.ReSetupDeliveryCharges(ddlCountries.SelectedItem.Value, ddlCountries.SelectedItem.Text)
            TEBUtilities.UpdateRetailBasketItems(Profile.Basket.Basket_Header_ID, Profile.Basket.WebPrices)
        Catch ex As Exception

        End Try

        Return ret
    End Function

    Private Sub SaveGiftMessage()
        Const SelectStr As String = " IF EXISTS( " & _
                                    "           SELECT * FROM tbl_gift_message WITH (NOLOCK)  " & _
                                    "           WHERE TEMP_ORDER_ID = @TEMP_ORDER_ID " & _
                                    "           AND BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                    "           AND PARTNER = @PARTNER) " & _
                                    "   BEGIN " & _
                                    "       UPDATE tbl_gift_message " & _
                                    "       SET RECIPIENT_NAME = @RECIPIENT_NAME, " & _
                                    "           MESSAGE = @MESSAGE " & _
                                    "       WHERE TEMP_ORDER_ID = @TEMP_ORDER_ID " & _
                                    "       AND BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                    "       AND PARTNER = @PARTNER " & _
                                    "   END " & _
                                    " ELSE " & _
                                    "   BEGIN " & _
                                    "       INSERT INTO tbl_gift_message " & _
                                    "       VALUES( " & _
                                    "               @BUSINESS_UNIT, " & _
                                    "               @PARTNER, " & _
                                    "               @TEMP_ORDER_ID, " & _
                                    "               @RECIPIENT_NAME, " & _
                                    "               @MESSAGE) " & _
                                    "   END "

        Dim cmd As New SqlCommand(SelectStr, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
        Try
            cmd.Connection.Open()
            With cmd.Parameters
                .Clear()
                .Add("@TEMP_ORDER_ID", Data.SqlDbType.NVarChar).Value = Profile.Basket.TempOrderID
                .Add("@BUSINESS_UNIT", Data.SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                .Add("@PARTNER", Data.SqlDbType.NVarChar).Value = TalentCache.GetPartner(Profile)
                .Add("@RECIPIENT_NAME", Data.SqlDbType.NVarChar).Value = toBox.Text
                .Add("@MESSAGE", Data.SqlDbType.NVarChar).Value = msgBox.Text
            End With

            cmd.ExecuteNonQuery()

        Catch ex As Exception
        Finally
            cmd.Connection.Close()
        End Try

    End Sub

    Private Function AddressChanged() As Boolean

        Dim ret As Boolean = False

        Dim sContactName As String = String.Empty
        Dim sAddress1 As String = String.Empty
        Dim sAddress2 As String = String.Empty
        Dim sTownCity As String = String.Empty
        Dim sCounty As String = String.Empty
        Dim sAddress5 As String = String.Empty
        Dim sPostCode As String = String.Empty
        Dim sCountry As String = String.Empty
        Dim deliveryDetails As New DEDeliveryDetails

        ' Load the saved version of address 
        If Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
            If chkUseSameAddress.Checked Then
                deliveryDetails = Session("StoredRetailDeliveryAddress")
            Else
                If rdolDisplayOption.SelectedValue = 0 Then
                    deliveryDetails = Session("StoredRetailDeliveryAddress")
                Else
                    deliveryDetails = Session("StoredTicketingDeliveryAddress")
                End If
            End If
        ElseIf Profile.Basket.BasketContentType = GlobalConstants.TICKETINGBASKETCONTENTTYPE Then
            deliveryDetails = Session("StoredTicketingDeliveryAddress")
        ElseIf Profile.Basket.BasketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
            deliveryDetails = Session("StoredRetailDeliveryAddress")
        End If


        If deliveryDetails Is Nothing Then
            ret = True
        Else
            ' Load the work vars 
            sAddress1 = deliveryDetails.Address1
            sAddress2 = deliveryDetails.Address2
            sTownCity = deliveryDetails.Address3
            sCounty = deliveryDetails.Address4
            sAddress5 = deliveryDetails.Address5
            sCountry = deliveryDetails.Country
            sPostCode = deliveryDetails.Postcode

            ' Compare
            If sAddress1 = Me.txtAddress1.Text And sAddress2 = Me.txtAddress2.Text And sTownCity = Me.txtTownCity.Text And sCounty = Me.txtCounty.Text And _
                sAddress5 = Me.txtAddress5.Text And sPostCode = Me.txtPostCode.Text And sCountry = Me.ddlCountries.SelectedItem.Text Then
                ret = False
            Else
                ret = True
            End If
        End If

        ' Return
        Return ret

    End Function

    Private Function SaveRetailAddress() As String

        Dim errorMessage As String = String.Empty

        If IsRetailVisible() AndAlso Not TEBUtilities.IsBasketHomeDelivery(Profile) Then

            ' Is it the retail address that has changed
            Dim boolUpdatesRequired As Boolean = False
            If Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
                If chkUseSameAddress.Checked Then
                    boolUpdatesRequired = True
                Else
                    If rdolDisplayOption.SelectedValue = 0 Then
                        boolUpdatesRequired = True
                    End If
                End If
            Else
                If Profile.Basket.BasketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
                    boolUpdatesRequired = True
                End If
            End If

            If boolUpdatesRequired Then

                If AddressChanged() OrElse plhPreferredDateRow.Visible Then
                    Dim addressExternalID As String = String.Empty
                    Dim deliveryDate As Date = Date.MinValue
                    If Session("DeliveryDate") IsNot Nothing Then
                        deliveryDate = Session("DeliveryDate")
                    End If

                    If Me.ddlSelectAddress.SelectedItem.Text = _ucr.Content("AddNewAddressText", _languageCode, True) AndAlso Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("AddNewAdressEnabled")) Then
                        '                If Me.chkSaveTheAddress.Checked Then
                        Try
                            Dim ta As TalentProfileAddress
                            Dim reference As String = String.Empty
                            If String.IsNullOrEmpty(txtAddress1.Text.Trim) Then
                                reference = txtAddress1.Text & " " & txtAddress2.Text
                            Else
                                reference = txtAddress1.Text & " " & txtAddress2.Text
                            End If
                            If Not Profile.User.Addresses.ContainsKey(reference) Then
                                ' Reference doesn't already exist
                                errorMessage = SaveRetailAddressToDB(False)
                                Session("NewAddressReference") = Nothing
                            Else
                                ' Reference already exists
                                ta = Profile.User.Addresses(reference)
                                If Not UCase(ta.Post_Code) = UCase(txtPostCode.Text) Then
                                    errorMessage = SaveRetailAddressToDB(True)
                                    Session("NewAddressReference") = Nothing
                                Else
                                    'The address already exists on this profile and so cannot be added
                                    errorMessage = _ucr.Content("AddressExistsErrorText", _languageCode, True)
                                End If
                            End If
                        Catch ex As Exception
                            ' SaveRetailAddressToDB(False)
                        End Try
                        '            End If
                        addressExternalID = String.Empty
                    Else
                        Dim addressKey As String = Me.ddlSelectAddress.SelectedItem.Text
                        If Profile.User.Addresses.ContainsKey(Me.ddlSelectAddress.SelectedItem.Text) Then
                            'Update an existing address if editable
                            If Not TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("MakeAddressFieldsReadOnly")) Then
                                With Profile.User.Addresses(ddlSelectAddress.SelectedItem.Text)
                                    .Address_Line_1 = txtAddress1.Text
                                    .Address_Line_2 = txtAddress2.Text
                                    .Address_Line_3 = txtTownCity.Text
                                    .Address_Line_4 = txtCounty.Text
                                    .Address_Line_5 = txtAddress5.Text
                                    .Post_Code = UCase(txtPostCode.Text)
                                    .Country = ddlCountries.SelectedValue
                                    Profile.Save()
                                End With
                            End If
                        Else
                            'add a new address
                            errorMessage = SaveRetailAddressToDB(False)
                            addressKey = Session("NewAddressReference")
                            Session("NewAddressReference") = Nothing
                        End If
                    End If

                    'decides the final delivery date
                    Dim preferredDateExists As Boolean = True
                    If plhPreferredDateRow.Visible Then
                        If PreferredDate.Text.Length > 0 Then
                            If Session("dtPreferredDeliveryDates") IsNot Nothing Then
                                Dim dtPreferredDeliveryDates As DataTable = CType(Session("dtPreferredDeliveryDates"), DataTable)
                                If dtPreferredDeliveryDates.Rows.Count > 0 Then
                                    preferredDateExists = False
                                    Dim deliveryDateFormat As String = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Attribute("DeliveryDateFormat")).Trim
                                    If deliveryDateFormat.Length <= 0 Then
                                        deliveryDateFormat = "dd/MM/yyyy"
                                    End If
                                    For rowIndex As Integer = 0 To dtPreferredDeliveryDates.Rows.Count - 1
                                        If CDate(dtPreferredDeliveryDates.Rows(rowIndex)("DeliveryDates")).ToString(deliveryDateFormat) = PreferredDate.Text Then
                                            preferredDateExists = True
                                            deliveryDate = PreferredDate.Text
                                            Exit For
                                        End If
                                    Next
                                    If Not preferredDateExists Then
                                        errorMessage = _ucr.Content("PreferredDateForZoneError", _languageCode, True)
                                        PopulateRetailDeliveryMessage()
                                    Else
                                        TDataObjects.OrderSettings.TblOrderHeader.UpdateOrderProjectedDeliveryDate(Profile.Basket.TempOrderID, deliveryDate)
                                    End If
                                End If
                            End If
                        End If
                    End If

                    ' Repopulate the SelectAddress drop down
                    PopulateAddressDDL()

                    ' Save the current retail address
                    '                StoreRetailAddress()
                    If Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
                        If Not chkUseSameAddress.Checked Then
                            If rdolDisplayOption.SelectedValue = 0 Then
                                StoreRetailAddress()
                            End If
                        End If
                    Else
                        If Profile.Basket.BasketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
                            StoreRetailAddress()
                        End If
                    End If
                End If
            Else
                ' Save the current ticketing address
                StoreTicketingAddress()

            End If
        Else
            ' Save the current ticketing address
            StoreTicketingAddress()

        End If


        Return errorMessage

    End Function

    Private Function SaveRetailAddressToDB(ByVal StorePostcode As Boolean) As String

        Dim errorMessage As String = String.Empty

        Try
            Dim ta As New TalentProfileAddress
            With ta
                .LoginID = Profile.User.Details.LoginID
                If plhAddressMonikerRow.Visible AndAlso txtMonikerAddress.Text.ToString.Trim <> "" Then
                    .Reference = txtMonikerAddress.Text
                Else
                    '                    .Reference = GenerateAddressMoniker(StorePostcode)
                    If txtAddress1.Visible Then
                        .Reference = GenerateAddressMoniker_v2(txtAddress1.Text, txtAddress2.Text, UCase(txtPostCode.Text))
                    Else
                        .Reference = GenerateAddressMoniker_v2(txtAddress2.Text, txtTownCity.Text, UCase(txtPostCode.Text))
                    End If
                End If
                .Type = ""
                .Default_Address = True
                .Address_Line_1 = txtAddress1.Text
                .Address_Line_2 = txtAddress2.Text
                .Address_Line_3 = txtTownCity.Text
                .Address_Line_4 = txtCounty.Text
                .Address_Line_5 = txtAddress5.Text
                .Post_Code = UCase(txtPostCode.Text)
                .Country = ddlCountries.SelectedValue
                .Sequence = GetNextSequenceNo()
                .Delivery_Zone_Code = ModuleDefaults.DefaultDeliveryZoneCode
            End With

            'finally, check to see if the address reference is alrady taken
            'Dim testAddress As TalentProfileAddress = Profile.User.Addresses.ContainsKey(ta.Reference)
            ' If testAddress Is Nothing Then
            If Not Profile.User.Addresses.ContainsKey(ta.Reference) Then
                Dim ret As Boolean = Profile.Provider.AddAddressToUserProfile(ta)
                If Not ret Then
                    errorMessage = "Error adding new address."
                End If
            Else
                'The address already exists on this profile and so cannot be added
                ' But now don't set error because of this
                '                errorMessage = _ucr.Content("AddressExistsErrorText", _languageCode, True)
            End If

            If errorMessage = String.Empty Then
                Try
                    Profile.Provider.UpdateDefaultAddress(ta.LoginID, Profile.PartnerInfo.Details.Partner, ta.Address_ID)
                Catch ex As Exception
                    Logging.WriteLog(Profile.UserName, "UCCASA-020", ex.Message, "Error updating the user's default address", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "DeliveryAddress.ascx")
                    errorMessage = "Error updating the user's default address"
                End Try
            End If

            If errorMessage = String.Empty Then Session("NewAddressReference") = ta.Reference

        Catch ex As Exception
            Logging.WriteLog(Profile.UserName, "UCCASA-010", ex.Message, "Error adding a new address for the user", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "DeliveryAddress.ascx")
            errorMessage = "Error adding a new address for the user"
        End Try


        Return errorMessage

    End Function

    Private Function GenerateAddressMoniker_v2(ByVal sAdd1 As String, ByVal sAdd2 As String, ByVal sPost As String) As String
        Dim addressMoniker As String = String.Empty
        addressMoniker = sAdd1.Trim & ", " & sAdd2.Trim & ", " & sPost.Trim
        Return addressMoniker
    End Function

    Private Function GenerateAddressKey(ByVal sAdd1 As String, ByVal sAdd2 As String, ByVal sAdd3 As String, ByVal sPostcode As String) As String
        Dim ret As String = String.Empty
        If String.IsNullOrEmpty(sAdd1.Trim) Then
            ret = sAdd2.Trim & " " & sAdd3.Trim
        Else
            ret = sAdd1.Trim & " " & sAdd2.Trim
        End If
        ret = ret + sPostcode.Trim
        ret = ret.Replace(" ", "")
        ret = ret.ToLower
        Return ret
    End Function

    Private Function GetNextSequenceNo() As Integer
        Dim seq As Integer = 0
        Try
            For Each tpa As TalentProfileAddress In Profile.User.Addresses.Values
                If tpa.Sequence > seq Then seq = tpa.Sequence
            Next
            seq += 1
        Catch ex As Exception
        End Try
        Return seq
    End Function

    Private Sub PopulateAddressDDL()


        ' Save the currently selected address
        Dim intSelectedIndex As Integer = 0
        If Me.ddlSelectAddress.SelectedIndex > 0 Then intSelectedIndex = Me.ddlSelectAddress.SelectedIndex

        ' Populate the Select Address drop-down
        ddlSelectAddress.Items.Clear()

        If Not Profile.IsAnonymous Then

            ' Retrieve all Ticketing-based addresses 
            Dim iSeriesAddresses As DataRowCollection = Nothing
            iSeriesAddresses = RetrieveTicketingAddresses()

            'Retrieve all Retail-based addresses 
            Dim sqlAddresses As DataRowCollection = Nothing
            sqlAddresses = RetrieveRetailAddresses()

            ' Populate drop-down
            PopulateSelectAddressDDL(ddlSelectAddress, iSeriesAddresses, sqlAddresses)

        End If

        ' Add item for 'Add New Address....' option
        If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("AddNewAdressEnabled")) Then
            ddlSelectAddress.Items.Add(New ListItem(_ucr.Content("AddNewAddressText", _languageCode, True), "New Address"))
        End If
        ddlSelectAddress.DataTextField = "Text"
        ddlSelectAddress.DataValueField = "Value"

        ' Restore previously selected index
        If intSelectedIndex > 0 Then Me.ddlSelectAddress.SelectedIndex = intSelectedIndex


    End Sub

    Private Sub PopulateCountryDDL(Optional ByVal boolRetail As Boolean = False)
        ddlCountries.Items.Clear()
        ddlCountries.DataSource = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, "DELIVERY", "COUNTRY")
        ddlCountries.DataTextField = "Text"
        ddlCountries.DataValueField = "Value"
        ddlCountries.DataBind()

        '----------------------------------------------------------------------------------------
        ' If set up to use default country on module defaults and a default country is found then
        ' set the default country and protect it
        '----------------------------------------------------------------------------------------
        If boolRetail Then ddlCountries.Enabled = False
        Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
        Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
        If def.UseDefaultCountryOnDeliveryAddress Then
            Dim defaultCountry As String = TalentCache.GetDefaultCountryForBU()
            If defaultCountry <> String.Empty Then
                ddlCountries.SelectedValue = defaultCountry
                _noDefaultCountrySet = False
            End If
        End If
    End Sub

    Private Function RetrieveTicketingAddresses() As DataRowCollection

        Dim iSeriesAddresses As DataRowCollection = Nothing

        Dim talentCustomerDetail As New Talent.Common.TalentCustomer
        Dim deSettings As New DESettings
        Dim talentErrObj As New ErrorObj
        Dim Decust = New DECustomerV11
        Dim customer As New DECustomer
        Dim agent As New DEAgent

        talentCustomerDetail.AgentDataEntity = agent
        customer.UserName = Profile.UserName
        customer.CustomerNumber = Profile.UserName
        customer.Source = GlobalConstants.SOURCE
        Decust.DECustomersV1.Add(customer)
        talentCustomerDetail.DeV11 = Decust
        deSettings = Talent.eCommerce.Utilities.GetSettingsObject()
        talentCustomerDetail.Settings() = deSettings
        talentErrObj = talentCustomerDetail.RetrieveAddressCapture()

        If Not talentErrObj.HasError _
               AndAlso Not talentCustomerDetail.ResultDataSet Is Nothing _
               AndAlso talentCustomerDetail.ResultDataSet.Tables(1).Rows.Count > 0 AndAlso ModuleDefaults.UseTicketingDeliveryAddresses Then
            iSeriesAddresses = talentCustomerDetail.ResultDataSet.Tables(1).Rows
        End If

        Return iSeriesAddresses

    End Function

    ''' <summary>
    ''' Retrieve the address records from tbl_address for the given user
    ''' </summary>
    ''' <returns>Collection of rows from tbl_address</returns>
    ''' <remarks></remarks>

    Private Function RetrieveRetailAddresses() As DataRowCollection
        Dim sqlAddresses As DataRowCollection = Nothing
        If ModuleDefaults.UseRetailDeliveryAddresses Then
            sqlAddresses = TDataObjects.ProfileSettings.tblAddress.GetByLoginId(HttpContext.Current.Profile.UserName, _partnerCode, False).Rows
        End If
        Return sqlAddresses
    End Function

    Private Sub PopulateSelectAddressDDL(ByRef ddlSelectAddress As DropDownList, ByVal drcTicketingAddresses As DataRowCollection, ByVal drcRetailAddresses As DataRowCollection)

        Dim list As New Generic.List(Of String)

        ' Ticketing Addresses
        If Not drcTicketingAddresses Is Nothing AndAlso drcTicketingAddresses.Count > 0 Then
            For Each address As DataRow In drcTicketingAddresses


                ' Generate the address key to be used to de-duplify the avaialable addresses
                ' Generate the address moniker/reference
                ' Generate the address value
                Dim addressKey As String = String.Empty
                Dim addressMoniker As String = String.Empty
                Dim addressValue As String = String.Empty

                addressKey = GenerateAddressKey(address("ADDRESS1"), address("ADDRESS2"), address("TOWN"), address("POSTCODE1") + address("POSTCODE2"))

                If list.Count = 0 Then
                    Dim sMainMonikerText = _ucr.Content("MainAddressText", _languageCode, True)
                    If sMainMonikerText.Trim = "" Then
                        addressMoniker = GenerateAddressMoniker_v2(address("ADDRESS1"), address("ADDRESS2"), address("POSTCODE1").ToString() & " " & address("POSTCODE2").ToString())
                    End If
                Else
                    If address("MONIKER").ToString.Trim <> "" Then
                        addressMoniker = address("MONIKER").ToString
                    Else
                        addressMoniker = GenerateAddressMoniker_v2(address("ADDRESS1"), address("ADDRESS2"), address("POSTCODE1").ToString() & " " & address("POSTCODE2").ToString())
                    End If
                End If

                addressValue = "" & "|" & _
                                address("ADDRESS1").ToString() & "|" & _
                                address("ADDRESS2").ToString() & "|" & _
                                address("TOWN").ToString() & "|" & _
                                address("COUNTY").ToString() & "|" & _
                                address("COUNTRY").ToString() & "|" & _
                                address("POSTCODE1").ToString() & " " & address("POSTCODE2").ToString() & "|" & _
                                addressMoniker
                ' Add to list
                If list.Count > 0 Then
                    Dim result As String = list.Find(Function(value As String) value = addressKey)
                    If result Is Nothing Then
                        ddlSelectAddress.Items.Add(New ListItem(addressMoniker, addressValue))
                        list.Add(addressKey)
                    End If
                Else
                    ddlSelectAddress.Items.Add(New ListItem(addressMoniker, addressValue))
                    list.Add(addressKey)
                End If

            Next
        End If

        ' Retail addresses
        If Not drcRetailAddresses Is Nothing AndAlso drcRetailAddresses.Count > 0 Then
            For Each address As DataRow In drcRetailAddresses

                ' Generate the address key to be used to de-duplify the avaialable addresses
                ' Generate the address moniker/reference
                ' Generate the address value
                Dim addressKey As String = String.Empty
                Dim addressMoniker As String = String.Empty
                Dim addressValue As String = String.Empty

                addressKey = GenerateAddressKey(address("ADDRESS_LINE_1"), address("ADDRESS_LINE_2"), address("ADDRESS_LINE_3"), address("POST_CODE"))

                If address("REFERENCE").ToString.Trim <> "" Then
                    addressMoniker = address("REFERENCE").ToString
                Else
                    If txtAddress1.Visible Then
                        addressMoniker = GenerateAddressMoniker_v2(address("ADDRESS_LINE_1"), address("ADDRESS_LINE_2"), address("POST_CODE"))
                    Else
                        addressMoniker = GenerateAddressMoniker_v2(address("ADDRESS_LINE_2"), address("ADDRESS_LINE_3"), address("POST_CODE"))
                    End If
                End If


                addressValue = address("ADDRESS_LINE_1").ToString() & "|" & _
                                address("ADDRESS_LINE_2").ToString() & "|" & _
                                address("ADDRESS_LINE_3").ToString() & "|" & _
                                address("ADDRESS_LINE_4").ToString() & "|" & _
                                address("ADDRESS_LINE_5").ToString() & "|" & _
                                address("COUNTRY").ToString() & "|" & _
                                address("POST_CODE").ToString() & "|" & _
                                addressMoniker

                ' Add to list
                If list.Count > 0 Then
                    Dim result As String = list.Find(Function(value As String) value = addressKey)
                    If result Is Nothing Then
                        ddlSelectAddress.Items.Add(New ListItem(addressMoniker, addressValue))
                        list.Add(addressKey)
                    End If
                Else
                    ddlSelectAddress.Items.Add(New ListItem(addressMoniker, addressValue))
                    list.Add(addressKey)
                End If
            Next

        End If
    End Sub

    Private Sub PopulateAddressfromDDL()
        Try
            txtContactName.Text = Profile.User.Details.Full_Name
        Catch ex As Exception
        End Try

        If ddlSelectAddress.SelectedItem.Value <> "New Address" Then
            Dim fullAddress As String = ddlSelectAddress.SelectedItem.Value
            Dim fullAddressTemp As String() = fullAddress.Split(New Char() {"|"c})
            txtAddress1.Text = fullAddressTemp(0)
            txtAddress2.Text = fullAddressTemp(1)
            txtTownCity.Text = fullAddressTemp(2)
            txtCounty.Text = fullAddressTemp(3)
            txtAddress5.Text = fullAddressTemp(4)
            If String.IsNullOrEmpty(fullAddressTemp(5)) Then
                ddlCountries.SelectedIndex = 0
            Else
                Dim i As Integer = 0
                For Each li As ListItem In ddlCountries.Items
                    If li.Value.ToLower = fullAddressTemp(5).ToLower OrElse li.Text.ToLower = fullAddressTemp(5).ToLower Then
                        ddlCountries.SelectedIndex = i
                        Exit For
                    End If
                    i += 1
                Next
            End If
            txtPostCode.Text = fullAddressTemp(6)
            txtMonikerAddress.Text = fullAddressTemp(7)
            UpdateRetailOrder()
        Else
            txtAddress1.Text = ""
            txtAddress2.Text = ""
            txtTownCity.Text = ""
            txtCounty.Text = ""
            txtAddress5.Text = ""
            If _noDefaultCountrySet Then ddlCountries.SelectedIndex = 0
            txtPostCode.Text = ""
            txtMonikerAddress.Text = ""
        End If
    End Sub

    Private Sub PopulateRetailDeliveryMessage()
        Try

            Dim ta As New TalentProfileAddress
            If Not TEBUtilities.IsBasketHomeDelivery(Profile) Then
                If Profile.User.Addresses.ContainsKey(ddlSelectAddress.SelectedItem.Text) Then
                    ta = Profile.User.Addresses(ddlSelectAddress.SelectedItem.Text)
                End If
            End If
            If Not ta Is Nothing Then

                'check to see if we need to show a delivery message
                'if we do, show the correct message with the calculated delivery slot
                If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("DisplayDeliveryMessage")) Then
                    Dim deliveryZoneCode As String = String.Empty
                    Dim deliveryZoneType As String = "1"
                    If Not TEBUtilities.IsBasketHomeDelivery(Profile) Then
                        deliveryZoneCode = ta.Delivery_Zone_Code
                        Session("DeliveryZoneCode") = deliveryZoneCode
                        deliveryZoneType = Talent.eCommerce.Utilities.GetDeliveryZoneType(deliveryZoneCode)
                    End If
                    Dim deliveryZoneDate As Date = Talent.eCommerce.Utilities.GetDeliveryDate(Profile, deliveryZoneCode, deliveryZoneType)
                    Dim deliveryZoneDay As String = ""
                    If deliveryZoneDate = Date.MinValue Then
                        deliveryZoneType = "2"
                    Else
                        deliveryZoneDay = deliveryZoneDate.DayOfWeek.ToString.ToUpper
                    End If
                    Dim deliveryDateFormat As String = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Attribute("DeliveryDateFormat")).Trim
                    If deliveryDateFormat.Length <= 0 Then
                        deliveryDateFormat = "dd/MM/yyyy"
                    End If
                    DeliveryDate.Text = deliveryZoneDate.ToString(deliveryDateFormat)

                    Select Case deliveryZoneType
                        Case Is = "1"
                            DeliveryDay.Text = _ucr.Content("DeliveryMessage1", _languageCode, True).Replace("<<DELIVERY_SLOT>>", deliveryZoneDay)
                        Case Is = "2"
                            DeliveryDay.Text = _ucr.Content("DeliveryMessage2", _languageCode, True)
                        Case Is = ""
                            DeliveryDay.Text = _ucr.Content("DeliveryMessage1", _languageCode, True).Replace("<<DELIVERY_SLOT>>", deliveryZoneDay)
                        Case Else
                    End Select
                    Session("DeliveryDate") = deliveryZoneDate
                    If plhPreferredDateRow.Visible Then
                        Dim dtPreferredDeliveryDates As DataTable = Talent.eCommerce.Utilities.GetPreferredDeliveryDates(Profile, deliveryZoneCode, deliveryZoneType)
                        Session("dtPreferredDeliveryDates") = dtPreferredDeliveryDates
                        If dtPreferredDeliveryDates.Rows.Count > 0 Then
                            Dim strPreferredDates As String = String.Empty
                            For rowIndex As Integer = 0 To dtPreferredDeliveryDates.Rows.Count - 1
                                If String.IsNullOrEmpty(strPreferredDates) Then
                                    strPreferredDates += CDate(dtPreferredDeliveryDates.Rows(rowIndex)("DeliveryDates")).ToString("[MM, dd, yyyy]")
                                Else
                                    strPreferredDates += CDate(dtPreferredDeliveryDates.Rows(rowIndex)("DeliveryDates")).ToString(",[MM, dd, yyyy]")
                                End If
                            Next
                            Dim script As New StringBuilder
                            script.AppendLine("$("".datepicker"").datepicker({ beforeShowDay: setAvailableDays, dateFormat: 'dd/mm/yy' });")
                            script.AppendLine("var availableDays = [").Append(strPreferredDates).AppendLine("];")
                            script.AppendLine("function setAvailableDays(date) {")
                            script.AppendLine("    var isAvailable = false;")
                            script.AppendLine("    if (availableDays != null) {")
                            script.AppendLine("        for (i = 0; i < availableDays.length; i++) {")
                            script.AppendLine("            if (date.getDate() == availableDays[i][1] && date.getMonth() == availableDays[i][0] - 1 && date.getFullYear() == availableDays[i][2]) {")
                            script.AppendLine("                isAvailable = true;")
                            script.AppendLine("            }")
                            script.AppendLine("        }")
                            script.AppendLine("    }")
                            script.AppendLine("    if (isAvailable) return [true, 'open']; else return [false, 'closed'];")
                            script.AppendLine("}")
                            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "datepicker", script.ToString(), True)
                        End If
                        PreferredDate.Text = deliveryZoneDate.ToString(deliveryDateFormat)
                    End If
                End If
            End If
        Catch ex As Exception
            Logging.WriteLog(Profile.UserName, "UCCAPA-010b", ex.Message, "Error populating address fields from profile PopulateAddressFromDDL()", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "DeliveryAddress.ascx")
        End Try
    End Sub

    Private Sub SetTicketingAddressInSession()

        '
        ' This is the ticketing address sent to WS096R
        '
        Dim deliveryDetails As New DEDeliveryDetails
        Dim boolDeliveryDetailSet As Boolean = False
        ' Use either on-screen values or saved values from Session if using differenet addresses for retauil and ticketing
        If Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
            If chkUseSameAddress.Checked Then
                ' Use on screen values
            Else
                If rdolDisplayOption.SelectedValue = 1 Then
                    ' Use on screen values 
                Else
                    ' Use value stored in session
                    deliveryDetails = Session("StoredTicketingDeliveryAddress")
                    boolDeliveryDetailSet = True
                End If
            End If
        End If

        If Not boolDeliveryDetailSet Then
            deliveryDetails.ContactName = txtContactName.Text
            If Not txtAddress1.Visible Then
                deliveryDetails.Address1 = txtAddress2.Text
                deliveryDetails.Address2 = txtTownCity.Text
                deliveryDetails.Address3 = txtCounty.Text
                deliveryDetails.Address4 = txtAddress5.Text
            Else
                deliveryDetails.Address1 = txtAddress1.Text
                If txtAddress2.Text.Trim = "" Then
                    deliveryDetails.Address2 = txtTownCity.Text
                Else
                    If txtTownCity.Text.Trim = "" Then
                        deliveryDetails.Address2 = txtAddress2.Text
                    Else
                        deliveryDetails.Address2 = txtAddress2.Text + ", " + txtTownCity.Text
                    End If
                End If
                deliveryDetails.Address3 = txtCounty.Text
                deliveryDetails.Address4 = txtAddress5.Text
            End If
            deliveryDetails.Country = ddlCountries.SelectedItem.Text
            deliveryDetails.Postcode = txtPostCode.Text
            deliveryDetails.AddressMoniker = txtMonikerAddress.Text
            deliveryDetails.CountryCode = ddlCountries.SelectedItem.Value
        End If


        Dim printOption As String = "N"
        If containsPrint() Then
            printOption = SetPrintOption(AgentProfile.PrintAddressLabelDefault, AgentProfile.PrintTransactionReceiptDefault)
        End If
        deliveryDetails.PrintOption = printOption

        Session("DeliveryDetails") = deliveryDetails


        'Set storedDeliveryAddress DE to be used purely for a validation check to determine if any address line has chnaged - see IsDeliveryAddressValidated()
        Dim storedDeliveryAddress As New DEDeliveryDetails
        storedDeliveryAddress.ContactName = txtContactName.Text
        storedDeliveryAddress.Address1 = txtAddress1.Text
        storedDeliveryAddress.Address2 = txtAddress2.Text
        storedDeliveryAddress.Address3 = txtTownCity.Text
        storedDeliveryAddress.Address4 = txtCounty.Text
        storedDeliveryAddress.Address5 = txtAddress5.Text
        storedDeliveryAddress.Country = ddlCountries.SelectedItem.Text
        storedDeliveryAddress.Postcode = txtPostCode.Text
        storedDeliveryAddress.AddressMoniker = txtMonikerAddress.Text
        storedDeliveryAddress.CountryCode = ddlCountries.SelectedValue
        Session("StoredDeliveryAddress") = storedDeliveryAddress

    End Sub

    Private Sub StoreTicketingAddress(Optional ByVal boolAsNew As Boolean = False)

        Dim deliveryDetails As New DEDeliveryDetails
        If boolAsNew Then
            deliveryDetails.ContactName = txtContactName.Text
            deliveryDetails.Address1 = ""
            deliveryDetails.Address2 = ""
            deliveryDetails.Address3 = ""
            deliveryDetails.Address4 = ""
            deliveryDetails.Address5 = ""
            deliveryDetails.Country = ""
            deliveryDetails.Postcode = ""
            deliveryDetails.AddressMoniker = ""
            deliveryDetails.CountrySelectedIndex = ""
            deliveryDetails.AddressSelectedIndex = ddlSelectAddress.SelectedIndex.ToString
            deliveryDetails.CountryCode = ""
        Else
            deliveryDetails.ContactName = txtContactName.Text
            deliveryDetails.Address1 = txtAddress1.Text
            deliveryDetails.Address2 = txtAddress2.Text
            deliveryDetails.Address3 = txtTownCity.Text
            deliveryDetails.Address4 = txtCounty.Text
            deliveryDetails.Address5 = txtAddress5.Text
            deliveryDetails.Country = ddlCountries.SelectedItem.Text
            deliveryDetails.Postcode = txtPostCode.Text
            deliveryDetails.AddressMoniker = txtMonikerAddress.Text
            deliveryDetails.CountrySelectedIndex = ddlCountries.SelectedIndex.ToString
            deliveryDetails.AddressSelectedIndex = ddlSelectAddress.SelectedIndex.ToString
            deliveryDetails.CountryCode = ddlCountries.SelectedItem.Value
        End If
        Session("StoredTicketingDeliveryAddress") = deliveryDetails

    End Sub

    Private Sub StoreRetailAddress(Optional ByVal boolAsNew As Boolean = False)

        Dim deliveryDetails As New DEDeliveryDetails
        If boolAsNew Then
            deliveryDetails.ContactName = txtContactName.Text
            deliveryDetails.Address1 = ""
            deliveryDetails.Address2 = ""
            deliveryDetails.Address3 = ""
            deliveryDetails.Address4 = ""
            deliveryDetails.Address5 = ""
            deliveryDetails.Country = ""
            deliveryDetails.Postcode = ""
            deliveryDetails.AddressMoniker = ""
            deliveryDetails.CountrySelectedIndex = ""
            deliveryDetails.AddressSelectedIndex = ddlSelectAddress.SelectedIndex.ToString
        Else
            deliveryDetails.ContactName = txtContactName.Text
            deliveryDetails.Address1 = txtAddress1.Text
            deliveryDetails.Address2 = txtAddress2.Text
            deliveryDetails.Address3 = txtTownCity.Text
            deliveryDetails.Address4 = txtCounty.Text
            deliveryDetails.Address5 = txtAddress5.Text
            deliveryDetails.Country = ddlCountries.SelectedItem.Text
            deliveryDetails.Postcode = txtPostCode.Text
            deliveryDetails.AddressMoniker = txtMonikerAddress.Text
            deliveryDetails.CountrySelectedIndex = ddlCountries.SelectedIndex.ToString
            deliveryDetails.AddressSelectedIndex = ddlSelectAddress.SelectedIndex.ToString
            deliveryDetails.CountryCode = ddlCountries.SelectedItem.Value
        End If

        Session("StoredRetailDeliveryAddress") = deliveryDetails

    End Sub

    Private Sub RestoreTicketingAddress()

        Dim deliveryDetails As New DEDeliveryDetails
        deliveryDetails = Session("StoredTicketingDeliveryAddress")
        txtContactName.Text = deliveryDetails.ContactName
        ddlSelectAddress.SelectedIndex = CType(deliveryDetails.AddressSelectedIndex, Integer)
        txtMonikerAddress.Text = deliveryDetails.AddressMoniker
        txtAddress1.Text = deliveryDetails.Address1
        txtAddress2.Text = deliveryDetails.Address2
        txtTownCity.Text = deliveryDetails.Address3
        txtCounty.Text = deliveryDetails.Address4
        txtAddress5.Text = deliveryDetails.Address5
        txtPostCode.Text = deliveryDetails.Postcode
        ddlCountries.SelectedIndex = CType(deliveryDetails.CountrySelectedIndex, Integer)

    End Sub

    Private Sub RestoreRetailAddress()
        Dim deliveryDetails As New DEDeliveryDetails
        deliveryDetails = Session("StoredRetailDeliveryAddress")
        txtContactName.Text = deliveryDetails.ContactName
        ddlSelectAddress.SelectedIndex = CType(deliveryDetails.AddressSelectedIndex, Integer)
        txtMonikerAddress.Text = deliveryDetails.AddressMoniker
        txtAddress1.Text = deliveryDetails.Address1
        txtAddress2.Text = deliveryDetails.Address2
        txtTownCity.Text = deliveryDetails.Address3
        txtCounty.Text = deliveryDetails.Address4
        txtAddress5.Text = deliveryDetails.Address5
        txtPostCode.Text = deliveryDetails.Postcode
        ddlCountries.SelectedIndex = CType(deliveryDetails.CountrySelectedIndex, Integer)

    End Sub

    Private Function containsRetailItem() As Boolean
        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            If tbi.PRODUCT_TYPE = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Function containsTicketingItem() As Boolean
        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            If tbi.PRODUCT_TYPE = GlobalConstants.TICKETINGBASKETCONTENTTYPE Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Function containsPostOrRegisteredPost() As Boolean
        Dim success As Boolean = False
        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            If (Not tbi.CURR_FULFIL_SLCTN Is Nothing AndAlso tbi.CURR_FULFIL_SLCTN.Trim = "P") Or (Not tbi.CURR_FULFIL_SLCTN Is Nothing AndAlso tbi.CURR_FULFIL_SLCTN.Trim = "R") Then
                success = True
                Exit For
            End If
        Next
        Return success
    End Function

    Private Function containsPrint() As Boolean
        Dim success As Boolean = False
        Dim printAlways As Boolean = AgentProfile.PrintAlways
        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            If Not String.IsNullOrWhiteSpace(TEBUtilities.CheckForDBNull_String(tbi.CURR_FULFIL_SLCTN)) Then
                If (Not String.IsNullOrWhiteSpace(TEBUtilities.CheckForDBNull_String(tbi.FULFIL_OPT_PRINT))) AndAlso tbi.CURR_FULFIL_SLCTN.Trim = GlobalConstants.PRINT_FULFILMENT Then
                    success = True
                Else
                    If printAlways AndAlso _
                        ( _
                            (Not String.IsNullOrWhiteSpace(TEBUtilities.CheckForDBNull_String(tbi.FULFIL_OPT_COLL)) AndAlso tbi.CURR_FULFIL_SLCTN.Trim = GlobalConstants.COLLECT_FULFILMENT) _
                            OrElse (Not String.IsNullOrWhiteSpace(TEBUtilities.CheckForDBNull_String(tbi.FULFIL_OPT_POST)) AndAlso tbi.CURR_FULFIL_SLCTN.Trim = GlobalConstants.POST_FULFILMENT) _
                            OrElse (Not String.IsNullOrWhiteSpace(TEBUtilities.CheckForDBNull_String(tbi.FULFIL_OPT_REGPOST)) AndAlso tbi.CURR_FULFIL_SLCTN.Trim = GlobalConstants.REG_POST_FULFILMENT) _
                        ) Then
                        success = True
                    End If
                End If
            End If
            If success Then
                Exit For
            End If
        Next
        Return success
    End Function

    ''' <summary>
    ''' Returns the print option based upon Print Address option and Print Receipt option
    ''' </summary>
    ''' <returns>1=Print Tickets only, 2=Print Address + Tickets, 3=Print Receipt + Tickets, 4=Print Address + Receipt + Tickets</returns>
    ''' <remarks></remarks>
    Private Function SetPrintOption(ByVal boolPrintAddress As Boolean, ByVal boolPrintReceipt As Boolean) As String
        Dim printOption As String = "N"
        Dim basketContainsTicketItems As Boolean = containsTicketItem()
        If basketContainsTicketItems Then
            If boolPrintAddress AndAlso boolPrintReceipt Then
                printOption = "4"
            ElseIf Not boolPrintAddress AndAlso boolPrintReceipt Then
                printOption = "3"
            ElseIf boolPrintAddress AndAlso Not boolPrintReceipt Then
                printOption = "2"
            Else
                printOption = "1"
            End If
        End If
        Return printOption
    End Function

    ''' <summary>
    ''' Does the basket contain any ticketing items
    ''' </summary>
    ''' <returns>True if the basket contains ticketing items</returns>
    ''' <remarks></remarks>
    Private Function containsTicketItem() As Boolean
        Dim success As Boolean = False
        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            If (Not tbi.PRODUCT_TYPE Is Nothing AndAlso tbi.PRODUCT_TYPE = GlobalConstants.TICKETINGBASKETCONTENTTYPE) Then
                success = True
                Exit For
            End If
        Next
        Return success
    End Function

    Private Sub registerAcordionMenuScript(ByVal itemToOpen As Integer)
        Dim javascriptString As New StringBuilder
        javascriptString.Append("$(function () {")
        javascriptString.Append("    $("".checkout-accordion"").accordion({ icons: false, autoHeight: false, header: ""div.header"" });")
        javascriptString.Append("    $("".checkout-accordion"").accordion(""option"", ""active"", ")
        javascriptString.Append(itemToOpen)
        javascriptString.Append(");")
        javascriptString.Append("});")
        Page.ClientScript.RegisterStartupScript(Me.GetType(), "AccordionMenu", javascriptString.ToString(), True)
    End Sub

    Private Sub IsDeliveryAddressValidated()

        Dim DEDeliveryDetails As New DEDeliveryDetails

        If Session("DeliveryDetails") Is Nothing Then
            Session("deliveryAddressValidated") = False
        Else

            ' Restore address details from last save into Session
            DEDeliveryDetails = Session("StoredDeliveryAddress")
            If Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
                Select Case Me.rdolDisplayOption.SelectedIndex
                    Case Is = "0"
                        DEDeliveryDetails = Session("StoredRetailDeliveryAddress")
                    Case Is = "1"
                        DEDeliveryDetails = Session("StoredTicketingDeliveryAddress")
                End Select
            End If

            ' Determine if any changes
            If DEDeliveryDetails.Address1 = Me.txtAddress1.Text And _
                DEDeliveryDetails.Address2 = Me.txtAddress2.Text And _
                DEDeliveryDetails.Address3 = Me.txtTownCity.Text And _
                DEDeliveryDetails.Address4 = Me.txtCounty.Text And _
                DEDeliveryDetails.Address5 = Me.txtAddress5.Text And _
                DEDeliveryDetails.Country = Me.ddlCountries.SelectedItem.Text And _
                DEDeliveryDetails.Postcode = Me.txtPostCode.Text And _
                DEDeliveryDetails.AddressMoniker = Me.txtMonikerAddress.Text Then
                Session("deliveryAddressValidated") = True
            Else
                Session("deliveryAddressValidated") = False
            End If
        End If

        ' If still OK then check all has been confirmed
        'If Session("deliveryAddressValidated") = True Then
        '    Dim errorMessage As String = String.Empty
        '    errorMessage = Confirm(False)
        '    If errorMessage = String.Empty Then
        '        Session("deliveryAddressValidated") = True
        '    Else
        '        Session("deliveryAddressValidated") = False
        '    End If
        'End If

    End Sub

    Private Sub RestoreDeliveryAddressInformation()
        If Session("StoredDeliveryAddress") IsNot Nothing Then
            Dim deliveryDetails As New DEDeliveryDetails
            deliveryDetails = Session("StoredDeliveryAddress")
            txtContactName.Text = deliveryDetails.ContactName
            txtAddress1.Text = deliveryDetails.Address1
            txtAddress2.Text = deliveryDetails.Address2
            txtTownCity.Text = deliveryDetails.Address3
            txtCounty.Text = deliveryDetails.Address4
            txtAddress5.Text = deliveryDetails.Address5
            Dim i As Integer = 0
            For Each li As ListItem In ddlCountries.Items
                If li.Value.ToLower = deliveryDetails.Country.ToLower() OrElse li.Text.ToLower = deliveryDetails.Country.ToLower Then
                    ddlCountries.SelectedIndex = i
                    Exit For
                End If
                i += 1
            Next
            txtPostCode.Text = deliveryDetails.Postcode
            txtMonikerAddress.Text = deliveryDetails.AddressMoniker
            ddlCountries.SelectedValue = deliveryDetails.Country
            Dim isSelectedAddressValueNotFound As Boolean = True
            If ddlSelectAddress.Items.Count > 0 Then
                For itemIndex As Integer = 0 To ddlSelectAddress.Items.Count - 1
                    If ddlSelectAddress.Items(itemIndex).Value.Contains(txtMonikerAddress.Text) Then
                        ddlSelectAddress.SelectedValue = ddlSelectAddress.Items(itemIndex).Value
                        isSelectedAddressValueNotFound = False
                        Exit For
                    End If
                Next
            End If
            If isSelectedAddressValueNotFound Then
                ddlSelectAddress.SelectedValue = "New Address"
            End If
            txtContactName.Enabled = True
            txtAddress1.Enabled = True
            txtAddress2.Enabled = True
            txtTownCity.Enabled = True
            txtCounty.Enabled = True
            txtAddress5.Enabled = True
            ddlCountries.Enabled = True
            txtPostCode.Enabled = True
            txtMonikerAddress.Enabled = True
            ddlCountries.Enabled = True
            ddlSelectAddress.Enabled = True
        End If
    End Sub

    Private Sub ValidateBasketForAddressConfirm()
        Basket_UpdateBasketDeliveryCountry()
        ProcessSummaryForUpdatedBasket()
        Dim isFulfilmentChanged As Boolean = False
        If Session("BasketItemsBeforeCountryChange") IsNot Nothing Then
            For Each basketItem As DEBasketItem In Session("BasketItemsBeforeCountryChange")
                For Each currentBasketItem As DEBasketItem In Profile.Basket.BasketItems
                    If currentBasketItem.MODULE_ = GlobalConstants.BASKETMODULETICKETING Then
                        If currentBasketItem.Product = basketItem.Product Then
                            If currentBasketItem.CURR_FULFIL_SLCTN <> basketItem.CURR_FULFIL_SLCTN Then
                                isFulfilmentChanged = True
                                Exit For
                            Else
                                Exit For
                            End If
                        End If
                    End If
                Next
                If isFulfilmentChanged Then
                    Exit For
                End If
            Next
            Session.Remove("BasketItemsBeforeCountryChange")
        End If
        If isFulfilmentChanged Then
            'redirect to basket page
            HttpContext.Current.Session("TicketingGatewayError") = "DEL_COUNTRY_CHANGED_FULFIL"
            HttpContext.Current.Session("TalentErrorCode") = "DEL_COUNTRY_CHANGED_FULFIL"
            HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
        End If

    End Sub

    Private Sub ProcessSummaryForUpdatedBasket()
        If Not String.IsNullOrWhiteSpace(ddlCountries.SelectedValue) Then
            Dim deliveryCountryCode As String = TEBUtilities.GetDeliveryCountryISOAlpha3Code(ddlCountries.SelectedValue.Trim)
            TDataObjects.BasketSettings.TblBasketHeader.UpdateDeliveryZoneCode(CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID, deliveryCountryCode)
        End If
        Dim talBasketProcessor As New TalentBasketProcessor
        talBasketProcessor.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        talBasketProcessor.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(talBasketProcessor.Settings.BusinessUnit)
        talBasketProcessor.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(talBasketProcessor.Settings.BusinessUnit)
        Dim errObj As ErrorObj = talBasketProcessor.ProcessSummaryForUpdatedBasket(CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)
        Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True)
    End Sub

    Private Sub Basket_UpdateBasketDeliveryCountry()
        If (Profile.Basket.DELIVERY_COUNTRY_CODE <> TEBUtilities.GetDeliveryCountryISOAlpha3Code(ddlCountries.SelectedValue.Trim)) _
            AndAlso containsTicketingItem() AndAlso containsPostOrRegisteredPost() Then
            Dim postBackControl As Object = TEBUtilities.GetPostbackControl(Me.Page)
            If postBackControl IsNot Nothing Then
                If postBackControl.GetType() IsNot Nothing AndAlso postBackControl.GetType().Name.ToUpper() = "BUTTON" Then
                    If CType(postBackControl, Button).ID = "btnConfirmDeliveryAddress" Then
                        Session("BasketItemsBeforeCountryChange") = Profile.Basket.BasketItems
                        Dim ticketGatewayFunctions As New TicketingGatewayFunctions
                        ticketGatewayFunctions.Basket_UpdateBasketDeliveryCountry()
                    End If
                End If
            End If
        End If
    End Sub

#End Region

#Region "Addressing Methods"

    Protected Function GetAddressingLinkText() As String
        Return _ucr.Content("addressingLinkButtonText", _languageCode, True)
    End Function

    Protected Sub CreateAddressingJavascript()
        Dim sString As New StringBuilder
        sString.Append(vbCrLf).Append("<script language=""javascript"" type=""text/javascript"">").Append(vbCrLf)

        Select Case ModuleDefaults.AddressingProvider.ToUpper
            Case Is = "QAS", "QASONDEMAND"
                ' Create function to open child window
                sString.Append("function addressingPopup() {").Append(vbCrLf)
                sString.Append("win1 = window.open('../../PagesPublic/QAS/FlatCountry.aspx', 'QAS', '").Append(_ucr.Attribute("addressingWindowAttributes")).Append("');").Append(vbCrLf)
                sString.Append("win1.creator=self;").Append(vbCrLf)
                sString.Append("}").Append(vbCrLf)
            Case Is = "HOPEWISER"
                ' Create function to open child window
                sString.Append("function addressingPopup() {").Append(vbCrLf)
                sString.Append("win1 = window.open('../../PagesPublic/Hopewiser/HopewiserPostcodeAndCountry.aspx', 'Hopewiser', '").Append(_ucr.Attribute("addressingWindowAttributes")).Append("');").Append(vbCrLf)
                sString.Append("win1.creator=self;").Append(vbCrLf)
                sString.Append("}").Append(vbCrLf)
        End Select

        Dim sAllFields() As String = ModuleDefaults.AddressingFields.ToString.Split(",")
        Dim count As Integer = 0

        ' Create function to populate the address fields.  This function is called from FlatAddress.aspx.
        sString.Append("function UpdateAddressFields() {").Append(vbCrLf)

        ' Create local function variables used to indicate whether an address element has already been used.
        Do While count < sAllFields.Length
            sString.Append("var usedHiddenAdr").Append(count.ToString()).Append(" = '';").Append(vbCrLf)
            count = count + 1
        Loop

        ' Clear all address fields
        If addressLine1RowVisible Then sString.Append("document.forms[0].").Append(txtAddress1.UniqueID).Append(".value = '';").Append(vbCrLf)
        If addressLine2RowVisible Then sString.Append("document.forms[0].").Append(txtAddress2.UniqueID).Append(".value = '';").Append(vbCrLf)
        If addressLine3RowVisible Then sString.Append("document.forms[0].").Append(txtTownCity.UniqueID).Append(".value = '';").Append(vbCrLf)
        If addressLine4RowVisible Then sString.Append("document.forms[0].").Append(txtCounty.UniqueID).Append(".value = '';").Append(vbCrLf)
        If addressLine5RowVisible Then sString.Append("document.forms[0].").Append(txtAddress5.UniqueID).Append(".value = '';").Append(vbCrLf)
        If addressPostcodeRowVisible Then sString.Append("document.forms[0].").Append(txtPostCode.UniqueID).Append(".value = '';").Append(vbCrLf)

        ' If an address field is in use and is defined to contain a QAS address element then create Javascript code to populate correctly.
        If addressLine1RowVisible AndAlso Not ModuleDefaults.AddressingMapAdr1.Trim = "" Then GetJavascriptString(sString, "document.forms[0]." & txtAddress1.UniqueID, ModuleDefaults.AddressingMapAdr1, ModuleDefaults.AddressingFields)
        If addressLine2RowVisible AndAlso Not ModuleDefaults.AddressingMapAdr2.Trim = "" Then GetJavascriptString(sString, "document.forms[0]." & txtAddress2.UniqueID, ModuleDefaults.AddressingMapAdr2, ModuleDefaults.AddressingFields)
        If addressLine3RowVisible AndAlso Not ModuleDefaults.AddressingMapAdr3.Trim = "" Then GetJavascriptString(sString, "document.forms[0]." & txtTownCity.UniqueID, ModuleDefaults.AddressingMapAdr3, ModuleDefaults.AddressingFields)
        If addressLine4RowVisible AndAlso Not ModuleDefaults.AddressingMapAdr4.Trim = "" Then GetJavascriptString(sString, "document.forms[0]." & txtCounty.UniqueID, ModuleDefaults.AddressingMapAdr4, ModuleDefaults.AddressingFields)
        If addressLine5RowVisible AndAlso Not ModuleDefaults.AddressingMapAdr5.Trim = "" Then GetJavascriptString(sString, "document.forms[0]." & txtAddress5.UniqueID, ModuleDefaults.AddressingMapAdr5, ModuleDefaults.AddressingFields)
        If plhAddressPostCodeRow.Visible Then If Not ModuleDefaults.AddressingMapPost.Trim = "" Then GetJavascriptString(sString, "document.forms[0]." & txtPostCode.UniqueID, ModuleDefaults.AddressingMapPost, ModuleDefaults.AddressingFields)
        If plhAddressCountryRow.Visible Then If Not ModuleDefaults.AddressingMapPost.Trim = "" Then GetJavascriptString(sString, "document.forms[0]." & ddlCountries.UniqueID, ModuleDefaults.AddressingMapCountry, ModuleDefaults.AddressingFields)

        sString.Append("}").Append(vbCrLf)
        sString.Append("function trim(s) { ").Append(vbCrLf).Append("var r=/\b(.*)\b/.exec(s); ").Append(vbCrLf).Append("return (r==null)?"""":r[1]; ").Append(vbCrLf).Append("}")
        sString.Append("</script>").Append(vbCrLf)

        ScriptManager.RegisterClientScriptBlock(Me.Page, Me.GetType(), "FindAddressjavaScript", sString.ToString, False)
    End Sub

    Protected Sub GetJavascriptString(ByRef sString As StringBuilder, ByVal sFieldString As String, ByVal sAddressingMap As String, ByVal sAddressingFields As String)
        Dim count As Integer = 0
        Dim count2 As Integer = 0
        Const sStr1 As String = "document.forms[0].hiddenAdr"
        Const sStr2 As String = ".value"
        Const sStr3 As String = "usedHiddenAdr"
        Dim sAddressingMapFields() As String = sAddressingMap.ToString.Split(",")
        Dim sAddressingAllFields() As String = sAddressingFields.ToString.Split(",")

        Do While count < sAddressingMapFields.Length
            If Not sAddressingMapFields(count).Trim = "" Then
                count2 = 0
                Do While count2 < sAddressingAllFields.Length
                    If sAddressingMapFields(count).Trim = sAddressingAllFields(count2).Trim Then
                        sString.Append(vbCrLf)
                        sString.Append("if (trim(").Append(sStr3).Append(count2.ToString()).Append(") != 'Y' && trim(").Append(sStr1).Append(count2.ToString()).Append(sStr2).Append(") != '') {").Append(vbCrLf)
                        sString.Append("if (trim(").Append(sFieldString).Append(sStr2).Append(") == '' || ").Append(sFieldString).Append(" == document.forms[0].").Append(ddlCountries.UniqueID).Append(") {").Append(vbCrLf)
                        sString.Append(sFieldString).Append(sStr2).Append(" = ").Append(sStr1).Append(count2.ToString()).Append(sStr2).Append(";").Append(vbCrLf)
                        sString.Append("}").Append(vbCrLf)
                        sString.Append("else {").Append(vbCrLf)
                        sString.Append(sFieldString).Append(sStr2).Append(" = ").Append(sFieldString).Append(sStr2).Append(" + ', ' + ").Append(sStr1).Append(count2.ToString()).Append(sStr2).Append(";").Append(vbCrLf)
                        sString.Append("}").Append(vbCrLf)
                        sString.Append(sStr3).Append(count2.ToString()).Append(" = 'Y';").Append(vbCrLf)
                        sString.Append("}")
                        Exit Do
                    End If
                    count2 = count2 + 1
                Loop
            End If
            count = count + 1
        Loop
    End Sub

    ''' <summary>
    ''' Create hidden fields for each Addressing field defined in defaults.
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub CreateAddressingHiddenFields()
        Dim qasFields() As String = Nothing
        Dim count As Integer = 0
        Dim sString As String = String.Empty
        qasFields = ModuleDefaults.AddressingFields.ToString.Split(",")
        Do While count < qasFields.Length
            If count = 0 Then
                ltlAddressingHiddenFields.Text = vbCrLf
            End If
            ltlAddressingHiddenFields.Text += "<input type=""hidden"" name=""hiddenAdr" & count.ToString & """ value="" "" />" & vbCrLf
            count = count + 1
        Loop
    End Sub


#End Region

#Region "Public Methods"

    Public Function Confirm() As String

        Dim errorMessage As String = String.Empty

        If IsRetailVisible() Then
            errorMessage = ValidateRetailFields()

            If errorMessage = String.Empty Then
                Dim intRet As Integer = UpdateRetailOrder()
                If intRet < 1 Then
                    errorMessage = "Error updating order"
                End If
            End If

            If errorMessage = String.Empty Then
                errorMessage = SaveRetailAddress()
            End If
        End If

        If errorMessage = String.Empty Then
            SetTicketingAddressInSession()
        End If

        If ddlSelectAddress.SelectedValue = "New Address" Then
            ' Store the chnaged address in session again if mixed basket and using different addresses
            If Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
                If Not chkUseSameAddress.Checked Then
                    If rdolDisplayOption.SelectedValue = 0 Then
                        StoreRetailAddress()
                    Else
                        StoreTicketingAddress()
                    End If
                End If
            End If
        End If

        If errorMessage = String.Empty Then
            Session("deliveryAddressValidated") = True
        Else
            Session("deliveryAddressValidated") = False
        End If

        ValidateBasketForAddressConfirm()

        Return errorMessage

    End Function

    Public Function GetCountryDropDownPostbackOption() As Boolean
        Dim ret As Boolean = False
        If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("DisplayDeliveryMessage")) Then
            If Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
                If chkUseSameAddress.Checked Then
                    ret = True
                Else
                    If rdolDisplayOption.SelectedValue = 0 Then
                        ret = True
                    End If
                End If
            ElseIf Profile.Basket.BasketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
                ret = True
            ElseIf Profile.Basket.BasketContentType = GlobalConstants.TICKETINGBASKETCONTENTTYPE Then
                ret = True
            End If
        End If
        Return ret
    End Function

#End Region

End Class