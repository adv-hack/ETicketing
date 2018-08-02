Imports Microsoft.VisualBasic
Imports Talent.eCommerce
Imports Talent.Common
Imports System.Data

'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Registration Form
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved. 
'
'       Error Number Code base      UCREGFM- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'       25/05/07    /001    Ben     Add attribute processing for sending to CRM (newletter prefs)
'
'       31/07/09    /002    ARC     Session to clear up duplicate registration
'
'   todo currently DOB controls allow [31 February]  etc. so need date validation bit 
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_AddressFormat1
    Inherits ControlBase

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private ucr As New Talent.Common.UserControlResource
    Private _customerNumber As String = ""
    Private addressLine1RowVisible As Boolean = True
    Private addressLine2RowVisible As Boolean = True
    Private addressLine3RowVisible As Boolean = True
    Private addressLine4RowVisible As Boolean = True
    Private addressLine5RowVisible As Boolean = True
    Private addressPostcodeRowVisible As Boolean = True
    Private addressCountryRowVisible As Boolean = True
    Private addressTypeRowVisible As Boolean = True
    Private addressReferenceRowVisible As Boolean = True
    Private addressSequenceRowVisible As Boolean = True
    Private mothersnameRowVisible As Boolean = True
    Private fathersnameRowVisible As Boolean = True
    Private mailOptionTrueValue As Boolean = True
    Private opt1TrueValue As Boolean = True
    Private opt2TrueValue As Boolean = True
    Private opt3TrueValue As Boolean = True
    Private opt4TrueValue As Boolean = True
    Private opt5TrueValue As Boolean = True
    Private errMsg As Talent.Common.TalentErrorMessages
    Private _crmBranch As String = String.Empty
    Dim myDefs As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults

    Public Property ADF1_Street() As String
        Get
            Return street.Text
        End Get
        Set(ByVal value As String)
            street.Text = value
        End Set
    End Property
    Public Property ADF1_Town() As String
        Get
            Return town.Text
        End Get
        Set(ByVal value As String)
            town.Text = value
        End Set
    End Property
    Public Property ADF1_City() As String
        Get
            Return cityDDL.SelectedValue
        End Get
        Set(ByVal value As String)
            cityDDL.SelectedValue = value
        End Set
    End Property
    Public Property ADF1_County() As String
        Get
            Return countyDDL.SelectedValue
        End Get
        Set(ByVal value As String)
            countyDDL.SelectedValue = value
        End Set
    End Property
    Public Property ADF1_CityText() As String
        Get
            Return city.Text
        End Get
        Set(ByVal value As String)
            city.Text = value
        End Set
    End Property
    Public Property ADF1_CountyText() As String
        Get
            Return county.Text
        End Get
        Set(ByVal value As String)
            county.Text = value
        End Set
    End Property

    Public Property ADF1_Postcode() As String
        Get
            Return postcode.Text
        End Get
        Set(ByVal value As String)
            postcode.Text = value
        End Set
    End Property
    Public Property ADF1_Country() As String
        Get
            Return country.SelectedValue
        End Get
        Set(ByVal value As String)
            street.Text = value
        End Set
    End Property
    Public Property ADF1_CountryText() As String
        Get
            Return country.SelectedItem.Text
        End Get
        Set(ByVal value As String)
            country.SelectedItem.Text = value
        End Set
    End Property


    'Need to default to true because the other controls that use this one may not set the display property
    Public Display As Boolean = True

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.Display Then
            aceAddressSearch1.Enabled = True
            If country.Items.Count = 0 Then
                PopulateCountriesDropDownList()
            End If
        Else
            Me.Visible = False
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Me.Display Then
            SetUpUCR()

            If Me.Visible Then
                If country.Items.Count = 0 Then
                    PopulateCountriesDropDownList()
                End If
                SetLabelText()
            End If
            ' default country in from other format if changing format
            If Not Session("AddressFormatCountry") Is Nothing Then
                country.SelectedValue = Session("AddressFormatCountry")
                Session("AddressFormatCountry") = Nothing
            End If
        End If
        If Not Page.IsPostBack AndAlso Talent.eCommerce.Utilities.GetCurrentPageName.ToUpper <> "UPDATEPROFILE.ASPX" Then
            cityDDL.Enabled = False
            countyDDL.Enabled = False
        End If

    End Sub


    Protected Sub SetUpUCR()
        If Me.Display Then
            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .PageCode = Talent.Common.Utilities.GetAllString 'GetCurrentPageName()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "RegistrationForm.ascx"
            End With
        End If
    End Sub


    Private Sub SetLabelText()
        If Me.Display Then
            Try

                btnGetAreas.Text = ucr.Content("btnGetAreas", _languageCode, True)

                RegistrationHeaderLabel.Text = ucr.Content("registrationHeaderLabel1", _languageCode, True)             ' CSG Registration"

                With ucr
                    ' Personal Details"
                    AddressInfoLabel.Text = .Content("addressInfoLabel", _languageCode, True)                                ' Address Info"
                    '
                    streetLabel.Text = .Content("streetLabel", _languageCode, True)                                          ' Street"
                    townLabel.Text = .Content("townLabel", _languageCode, True)                                              ' Town"
                    cityLabel.Text = .Content("cityLabel", _languageCode, True)                                              ' City"
                    countyLabel.Text = .Content("countyLabel", _languageCode, True)                                          ' County"
                    postcodeLabel.Text = .Content("postcodeLabel", _languageCode, True)                                      ' Postcode"
                    countryLabel.Text = .Content("countryLabel", _languageCode, True)                                        ' Country"

                End With

                'Setup Max Lengths for textboxes
                With ucr

                    postcode.MaxLength = .Attribute("postcodeMaxLength")
                    street.MaxLength = .Attribute("streetMaxLength")
                    town.MaxLength = .Attribute("townMaxLength")
                    ' city.MaxLength = .Attribute("cityMaxLength")
                    county.MaxLength = .Attribute("countyMaxLength")

                End With

            Catch ex As Exception
            End Try
        End If

    End Sub

    Protected Function getError(ByVal errCode As String) As String
        errMsg = New Talent.Common.TalentErrorMessages(_languageCode, _
                                                TalentCache.GetBusinessUnitGroup, _
                                                TalentCache.GetPartner(Profile), _
                                                ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)

        Return errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
                            Talent.eCommerce.Utilities.GetCurrentPageName, _
                            errCode).ERROR_MESSAGE
    End Function


    Public Sub SetupRequiredValidator(ByVal sender As Object, ByVal e As EventArgs)
        If Me.Display Then
            Dim rfv As RequiredFieldValidator = CType(sender, RequiredFieldValidator)
            Try
                SetUpUCR()
                If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute(rfv.ControlToValidate & "EnableRFV")) Then
                    Try
                        rfv.ErrorMessage = ucr.Content(rfv.ControlToValidate & "RequiredFieldValidator", _languageCode, True)
                    Catch ex As Exception
                    End Try
                ElseIf rfv.ControlToValidate = "emailConfirm" Then
                    If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("emailEnableRFV")) Then
                        Try
                            rfv.ErrorMessage = ucr.Content("CompareEmailErrorText", _languageCode, True)
                        Catch ex As Exception
                        End Try
                    Else
                        rfv.Enabled = False
                    End If
                Else
                    rfv.Enabled = False
                End If
            Catch ex As Exception
            End Try
        End If
    End Sub

    Protected Sub SetupRegExValidator(ByVal sender As Object, ByVal e As EventArgs)
        If Me.Display Then
            SetUpUCR()

            Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults

            Dim rev As RegularExpressionValidator = CType(sender, RegularExpressionValidator)

            rev.ErrorMessage = ucr.Content(rev.ControlToValidate & "ErrorText", _languageCode, True)

            Select Case (rev.ControlToValidate)


                Case Is = "postcode", "txtRegPostcode"
                    rev.ValidationExpression = ucr.Attribute("PostcodeExpression")

                Case Is = "country", "ddlRegCountry"
                    'rev.ValidationExpression = "^(?! -- )?[a-zA-Z\s]+"
                    rev.ValidationExpression = "^[a-zA-Z\s]{0,50}$"

                Case Is = "sex"
                    rev.ValidationExpression = "^(?! -- )?[a-zA-Z\s]+"
                    rev.Enabled = def.ProfileSexMandatory

                Case Is = "building", _
                            "street", _
                            "town", _
                            "city", _
                            "county", _
                            "txtRegAddress1", _
                            "txtRegAddress2", _
                            "txtRegAddress3", _
                            "txtRegAddress4", _
                            "txtRegAddress5"
                    rev.ValidationExpression = ucr.Attribute("AddressExpression")

            End Select
        End If


    End Sub

    Protected Sub PopulateCountriesDropDownList()
        If Me.Display Then
            country.DataSource = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, "REGISTRATION", "COUNTRY")
            country.DataTextField = "Text"
            country.DataValueField = "Value"
            country.DataBind()
            Dim defaultCountry As String = TalentCache.GetDefaultCountryForBU()
            'Select the default country
            If myDefs.UseDefaultCountryOnRegistration Then
                If defaultCountry <> String.Empty Then
                    country.SelectedValue = defaultCountry
                End If
            End If



        End If
    End Sub


    Protected Sub btnGetAreas_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGetAreas.Click

        cityDDL.Enabled = True
        Dim strTArray As String() = town.Text.Split(",")
        Dim strTown As String = String.Empty
        If strTArray.Length > 0 Then
            strTown = strTArray(0)
        End If
        Dim strCity As String = String.Empty
        If strTArray.Length > 1 Then
            strCity = strTArray(1)
        End If
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As DESettings = New DESettings()

        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = Talent.Common.GlobalConstants.SQL2005DESTINATIONDATABASE
        settings.BusinessUnit = TalentCache.GetBusinessUnit
        tDataObjects.Settings = settings
        Dim productOptionMaster As Boolean = False

        Dim dtAreas As DataTable = tDataObjects.ProfileSettings.TblAddressFormat1Data.GetAreaByTownCity(strTown, strCity)

        Dim plsSelect As New ListItem
        plsSelect.Text = ucr.Content("pleaseSelectDropdown", _languageCode, True)

        cityDDL.DataSource = dtAreas
        cityDDL.DataTextField = "AREA"
        cityDDL.DataValueField = "AREA"
        cityDDL.DataBind()

        cityDDL.Items.Insert(0, plsSelect)

    End Sub

    Protected Sub cityDDL_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cityDDL.SelectedIndexChanged

        countyDDL.Enabled = True

        Dim strTArray As String() = town.Text.Split(",")
        Dim strTown As String = String.Empty
        If strTArray.Length > 0 Then
            strTown = strTArray(0)
        End If
        Dim strCity As String = String.Empty
        If strTArray.Length > 1 Then
            strCity = strTArray(1)
        End If
        Dim strArea As String = cityDDL.SelectedValue
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As DESettings = New DESettings()

        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = Talent.Common.GlobalConstants.SQL2005DESTINATIONDATABASE
        settings.BusinessUnit = TalentCache.GetBusinessUnit
        tDataObjects.Settings = settings
        Dim productOptionMaster As Boolean = False

        Dim dtDistricts As DataTable = tDataObjects.ProfileSettings.TblAddressFormat1Data.GetDistrictByAreaTownCity(strArea, strTown, strCity)

        Dim plsSelect As New ListItem
        plsSelect.Text = ucr.Content("pleaseSelectDropdown", _languageCode, True)

        countyDDL.DataSource = dtDistricts
        countyDDL.DataTextField = "DISTRICT"
        countyDDL.DataValueField = "DISTRICT"
        countyDDL.DataBind()

        countyDDL.Items.Insert(0, plsSelect)


    End Sub

    Protected Sub countyDDL_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles countyDDL.SelectedIndexChanged
        ' Get postcode..
        Dim strTArray As String() = town.Text.Split(",")
        Dim strTown As String = String.Empty
        If strTArray.Length > 0 Then
            strTown = strTArray(0)
        End If
        Dim strCity As String = String.Empty
        If strTArray.Length > 1 Then
            strCity = strTArray(1)
        End If
        Dim strArea As String = cityDDL.SelectedValue
        Dim strDistrict As String = countyDDL.SelectedValue
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As DESettings = New DESettings()

        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = Talent.Common.GlobalConstants.SQL2005DESTINATIONDATABASE
        settings.BusinessUnit = TalentCache.GetBusinessUnit
        tDataObjects.Settings = settings
        Dim productOptionMaster As Boolean = False

        Dim dtDistricts As DataTable = tDataObjects.ProfileSettings.TblAddressFormat1Data.GetPostcodeByDistrict(strDistrict, strArea, strTown, strCity)

        If dtDistricts.Rows.Count > 0 Then
            postcode.Text = dtDistricts.Rows(0)("POSTCODE").ToString
        End If


    End Sub

    Protected Sub country_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles country.SelectedIndexChanged
        If country.SelectedValue.Trim <> "TR" Then
            Session("AddressFormat") = "0"
            Session("AddressFormatCountry") = country.SelectedValue
        Else
            Session("AddressFormat") = "1"
            Session("AddressFormatCountry") = "TR"

        End If
    End Sub

    Public Function ValidateAddress() As Boolean
        Dim validAddress As Boolean = False
        ' check street is populated
        ' Check details on AddressFormat1 are OK (Galatasaray)
        Dim valid As Boolean = True

        If street.Text = String.Empty OrElse
            town.Text = String.Empty OrElse
            cityDDL.SelectedIndex <= 0 OrElse
            countyDDL.SelectedIndex <= 0 OrElse
            country.SelectedValue = String.Empty Then

            valid = False
        Else
            ' check town/city is a valid value (this isa free text with an auto complete and it must match one of the autocompletes
            Dim tDataObjects = New TalentDataObjects()
            Dim settings As DESettings = New DESettings()

            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settings.DestinationDatabase = Talent.Common.GlobalConstants.SQL2005DESTINATIONDATABASE
            settings.BusinessUnit = TalentCache.GetBusinessUnit
            tDataObjects.Settings = settings

            Dim strTArray As String() = town.Text.Split(",")
            Dim strTown As String = String.Empty
            If strTArray.Length > 0 Then
                strTown = strTArray(0)
            End If
            Dim strCity As String = String.Empty
            If strTArray.Length > 1 Then
                strCity = strTArray(1)
            End If

            Dim dtTownAndCities As DataTable = tDataObjects.ProfileSettings.TblAddressFormat1Data.GetDistinctTownCity()

            If dtTownAndCities IsNot Nothing AndAlso dtTownAndCities.Rows.Count > 0 Then
                Dim matchedRows() As DataRow = Nothing
                matchedRows = dtTownAndCities.Select("TOWN LIKE '%" & strTown & "%'" & " and CITY LIKE '%" & strCity & "%'")
                If matchedRows.Length > 0 Then

                Else
                    valid = False
                End If
            End If
        End If
        Return valid

    End Function

    Public Function SetDropdownsForUpdateMode() As Boolean
        Dim setOK As Boolean = True
        Dim sender As Object = New Object
        Dim e As EventArgs = New EventArgs

        Try
            btnGetAreas_Click(sender, e)

            cityDDL.SelectedValue = ADF1_CityText.ToUpper

            cityDDL_SelectedIndexChanged(sender, e)

            countyDDL.SelectedValue = ADF1_CountyText.ToUpper
        Catch ex As Exception
            setOK = False
        End Try
        Return setOK
    End Function

    Protected Sub btnValidate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnValidate.Click
        Dim a As Boolean = ValidateAddress()
    End Sub
End Class
