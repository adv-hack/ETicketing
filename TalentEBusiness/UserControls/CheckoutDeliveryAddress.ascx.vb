Imports System.Data
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Checkout Delivery Address
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      UCCASC- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_CheckoutDeliveryAddress
    Inherits ControlBase

    Dim ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Private addressLine1RowVisible As Boolean = True
    Private addressLine2RowVisible As Boolean = True
    Private addressLine3RowVisible As Boolean = True
    Private addressLine4RowVisible As Boolean = True
    Private addressLine5RowVisible As Boolean = True
    Private addressPostcodeRowVisible As Boolean = True
    Private addressCountryRowVisible As Boolean = True
    Private PurchaseOrderRowVisible As Boolean = True

    Dim eComDefs As ECommerceModuleDefaults
    Dim defs As ECommerceModuleDefaults.DefaultValues
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Utilities.DoSapOciCheckout()
        Utilities.CheckBasketFreeItemHasOptions()
        eComDefs = New ECommerceModuleDefaults
        defs = eComDefs.GetDefaults
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "CheckoutDeliveryAddress.ascx"
        End With

        If defs.Call_Tax_WebService Then
            If Not HttpContext.Current.Session.Item("DunhillWSError") Is Nothing _
                AndAlso CBool(HttpContext.Current.Session.Item("DunhillWSError")) Then
                'Dunhill WS Error
                Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
            End If
        End If


    End Sub


    Protected Function AllInStock_BackEndCheck() As Boolean

        Dim AllInStock As Boolean = True
        If defs.Perform_Back_End_Stock_Check Then
            Dim dep As New Talent.Common.DePNA
            Dim des As New Talent.Common.DESettings
            Dim tls As New Talent.Common.TalentStock
            Dim err As New Talent.Common.ErrorObj
            Dim dt As Data.DataTable = Nothing
            Dim dRow As Data.DataRow = Nothing

            Dim strResults As New StringBuilder

            Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
            Dim stockLocation As String = def.StockLocation

            Dim productcodes As String = String.Empty
            Dim alternateSKUs As String = String.Empty

            Dim locations As String = String.Empty
            For Each bi As TalentBasketItem In Profile.Basket.BasketItems
                'Only perform the check if products have been selected, this could be a tickets only basket
                '   If Not productcodes.Trim.Equals("") Then
                If bi.PRODUCT_TYPE = "" OrElse bi.PRODUCT_TYPE = "M" Then
                    If Not bi.MODULE_.ToUpper.Equals("TICKETING") Then
                        If Not String.IsNullOrEmpty(productcodes) Then productcodes += ","
                        productcodes += bi.Product
                        If Not String.IsNullOrEmpty(locations) Then locations += ","
                        locations += stockLocation
                        If Not String.IsNullOrEmpty(alternateSKUs) Then alternateSKUs += ","
                        alternateSKUs += bi.ALTERNATE_SKU
                    End If
                End If
            Next

            If productcodes.EndsWith(",") Then productcodes = productcodes.TrimEnd(",")
            If locations.EndsWith(",") Then locations = locations.TrimEnd(",")
            If alternateSKUs.EndsWith(",") Then alternateSKUs = alternateSKUs.TrimEnd(",")

            'Only perform the check if products have been selected, this could be a tickets only basket
            If Not productcodes.Trim.Equals("") Then

                dep.SKU = productcodes
                dep.Warehouse = locations
                dep.AlternateSKU = alternateSKUs

                With des
                    .BusinessUnit = TalentCache.GetBusinessUnitGroup
                    .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                    .BackOfficeConnectionString = ConfigurationManager.ConnectionStrings("SYSTEM21").ToString
                    .Cacheing = False
                    If Profile.PartnerInfo.Details.Account_No_3 Is Nothing _
                        OrElse Profile.PartnerInfo.Details.Account_No_3.Trim = String.Empty Then
                        .AccountNo3 = def.DEFAULT_COMPANY_CODE
                    Else
                        .AccountNo3 = Profile.PartnerInfo.Details.Account_No_3
                    End If
                    .AccountNo4 = Profile.PartnerInfo.Details.Account_No_4
                    .DestinationDatabase = "SYSTEM21"
                    .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
                    .RetryFailures = def.StockCheckRetry
                    .RetryAttempts = def.StockCheckRetryAttempts
                    .RetryWaitTime = def.StockCheckRetryWait
                    .RetryErrorNumbers = def.StockCheckRetryErrors
                    .IgnoreErrors = def.StockCheckIgnoreErrors
                    .LoginId = Profile.User.Details.LoginID
                    .AccountNo1 = Profile.User.Details.Account_No_1
                End With
                Try
                    With tls
                        .Settings = des
                        .Dep = dep
                        err = .GetMutlipleStock
                        If Not err.HasError Then dt = .ResultDataSet.Tables(0)
                    End With
                Catch ex As Exception
                    Logging.WriteLog(Profile.UserName, "UCCASC-010", ex.Message, "Error contacting SYSTEM 21 for stock check", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
                End Try


                If Not err.HasError Then
                    '
                    ' We could have forced no error by ignoring the error code, therefore the datatable will be empty 
                    ' - just continue.
                    If Not dt Is Nothing Then
                        If dt.Rows.Count > 0 Then
                            Dim productcode As String = String.Empty, quantity As String = String.Empty
                            Dim ItemErrorLabel As New Label
                            For Each row As Data.DataRow In dt.Rows
                                For Each bi As TalentBasketItem In Profile.Basket.BasketItems
                                    productcode = bi.Product
                                    quantity = bi.Quantity
                                    If Utilities.CheckForDBNull_String(row("ProductNumber")).Trim = productcode Then
                                        If (defs.AllowCheckoutWhenBackEndUnavailable) And ((Utilities.CheckForDBNull_String(row("ErrorCode")).Trim = "UNAVAILABLE")) Then
                                            bi.STOCK_ERROR = False
                                            Exit For
                                        Else
                                            If Utilities.CheckForDBNull_Decimal(row("Quantity")) < CDec(quantity) Then
                                                AllInStock = False
                                                bi.STOCK_ERROR = True
                                                bi.QUANTITY_AVAILABLE = Utilities.CheckForDBNull_Decimal(row("Quantity"))
                                                '-------------------
                                                ' Discontinued check
                                                '-------------------
                                                If defs.PerformDiscontinuedProductCheck Then
                                                    Dim dtProdInfo As New Data.DataTable
                                                    dtProdInfo = Utilities.GetProductInfo(productcode)
                                                    If Not dtProdInfo Is Nothing AndAlso dtProdInfo.Rows.Count > 0 Then
                                                        If Utilities.CheckForDBNull_Boolean_DefaultFalse(dtProdInfo.Rows(0)("DISCONTINUED")) Then
                                                            bi.STOCK_ERROR_CODE = "DISC"
                                                        End If
                                                    End If
                                                End If

                                                Exit For
                                            Else
                                                bi.STOCK_ERROR = False
                                                Exit For
                                            End If
                                        End If
                                    End If
                                Next
                            Next
                        Else
                        End If
                    End If
                Else
                    Logging.WriteLog(Profile.UserName, err, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
                    AllInStock = False
                End If
            End If

            Profile.Basket.STOCK_ERROR = Not AllInStock

            'Save the basket regardless of stock status
            '------------------------------------------------
            Profile.Basket.IsDirty = True
            Profile.Save()
            '------------------------------------------------
        Else
            Profile.Basket.STOCK_ERROR = False

            'Save the basket regardless of stock status
            '------------------------------------------------
            Profile.Basket.IsDirty = True
            Profile.Save()
            '------------------------------------------------
        End If
        Return AllInStock
    End Function
    '------------------------------------------------------
    ' Back end call to check if items in basket are on stop 
    ' and if so return the list of alternative products
    '------------------------------------------------------
    Protected Function RetrieveAlternativeProducts() As Data.DataSet

        'If defs.Perform_Back_End_Stock_Check Then
        'Dim dep As New Talent.Common.DePNA
        Dim productCollection As New Collection
        Dim des As New Talent.Common.DESettings
        Dim talentProd As New Talent.Common.TalentProduct
        Dim err As New Talent.Common.ErrorObj
        Dim dt As Data.DataTable = Nothing
        Dim dRow As Data.DataRow = Nothing
        Dim results As New Data.DataSet

        Dim strResults As New StringBuilder

        Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
        Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults

        Dim basketDetail As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
        Dim dtBasket As Data.DataTable = basketDetail.GetBasketItems_ByHeaderID_ALL(CType(Profile.Basket.Basket_Header_ID, Long))

        '-----------------------------------------------------------
        ' Loop through basket and check if any items have alt items. 
        ' If so put out message. (basket in profile may not be
        ' loaded yet so need to go to DB)
        '-----------------------------------------------------------
        For Each row As Data.DataRow In dtBasket.Rows
            productCollection.Add(row("PRODUCT"))
        Next

        talentProd.ProductCollection = productCollection

        With des
            .BusinessUnit = TalentCache.GetBusinessUnitGroup
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
            .BackOfficeConnectionString = ConfigurationManager.ConnectionStrings("SYSTEM21").ToString
            .Cacheing = False
            If Profile.PartnerInfo.Details.Account_No_3 Is Nothing _
                OrElse Profile.PartnerInfo.Details.Account_No_3.Trim = String.Empty Then
                .AccountNo3 = def.DEFAULT_COMPANY_CODE
            Else
                .AccountNo3 = Profile.PartnerInfo.Details.Account_No_3
            End If
            .AccountNo4 = Profile.PartnerInfo.Details.Account_No_4
            .DestinationDatabase = "SYSTEM21"
            .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()

        End With
        Try
            With talentProd
                .Settings = des
                err = .RetrieveAlternativeProducts()
                If Not err.HasError Then dt = .ResultDataSet.Tables(0)
            End With
        Catch ex As Exception
            Logging.WriteLog(Profile.UserName, "UCCASC-011", _
                                                ex.Message, _
                                                "Error contacting " & _
                                                     talentProd.Settings.DestinationDatabase & _
                                                      " for alt products check", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
        End Try

        If Not err.HasError Then
            results = talentProd.ResultDataSet
        Else
            Logging.WriteLog(Profile.UserName, err, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
        End If

        Return results
    End Function


    Protected Function UserUnderAge() As Boolean
        'Dim defs As New Talent.eCommerce.ECommerceModuleDefaults
        'Dim values As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
        'Dim UnderAge As Boolean = False
        'values = defs.GetDefaults

        'If values.Use_Age_Check Then
        '    Dim products As New TalentProductInformationTableAdapters.tbl_productTableAdapter
        '    Dim orderLines As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
        '    Dim lines As New Data.DataTable

        '    Try
        '        'lines = orderLines.Get_BasketDetailControl_Lines(Profile.Basket.Basket_Header_ID, TalentCache.GetPartner(Profile), TalentCache.GetBusinessUnit, values.PriceList)
        '        lines = orderLines.GetBasketItems_ByHeaderID_NonTicketing(Profile.Basket.Basket_Header_ID)
        '        'If lines.Rows.Count < 1 Then
        '        '    lines = orderLines.Get_BasketDetailControl_Lines(Profile.Basket.Basket_Header_ID, Talent.Common.Utilities.GetAllString, TalentCache.GetBusinessUnit, values.PriceList)
        '        'End If
        '    Catch ex As Exception
        '        Logging.WriteLog(Profile.UserName, "UCCAUA-010", ex.Message, "Error getting basket details for age check in UserUnderAge()", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
        '    End Try

        '    If lines.Rows.Count > 0 Then
        '        Dim prods As Data.DataTable
        '        Dim age As Integer = ProfileHelper.GetAge(Profile.User.Details.DOB)
        '        For Each row As Data.DataRow In lines.Rows
        '            prods = products.GetDataByProduct_Code(CheckForDBNull_String(row("PRODUCT")))
        '            If CheckForDBNull_Int(prods.Rows(0)("PRODUCT_MINIMUM_AGE")) > age Then
        '                UnderAge = True
        '                Exit For
        '            End If
        '        Next
        '    Else
        '        'No items in basket so kick back to Basket page anyway
        '        UnderAge = True
        '    End If
        'End If

        'Return UnderAge
        Return False
    End Function

    Protected Sub PopulateAddressFromDDL()
        Try
            Dim ta As New TalentProfileAddress
            ta = Profile.User.Addresses(SelectAddressDDL.SelectedItem.Text)

            With ta
                building.Text = .Address_Line_1
                postcode.Text = UCase(.Post_Code)
                Address2.Text = .Address_Line_2
                Address3.Text = .Address_Line_3
                Address4.Text = .Address_Line_4
                Address5.Text = .Address_Line_5
                Try

                    Dim i As Integer = 0
                    For Each li As ListItem In CountryDDL.Items
                        If li.Value.ToLower = .Country.ToLower OrElse li.Text.ToLower = .Country.ToLower Then
                            CountryDDL.SelectedIndex = i
                        End If
                        i += 1
                    Next
                Catch
                End Try
                DeliveryContact.Text = Profile.User.Details.Full_Name
                SaveAddress.Text = ucr.Content("UpdateAddressText", _languageCode, True)
            End With

            'check to see if we need to show a delivery message
            'if we do, show the correct message with the calculated delivery slot
            If ucr.Attribute("DisplayDeliveryMessage").ToUpper = "TRUE" Then
                plhDeliveryMessage.Visible = True
                Dim deliveryZoneCode As String = ta.Delivery_Zone_Code
                Dim deliveryZoneType As String = GetDeliveryZoneType(deliveryZoneCode)
                Dim deliveryZoneDate As Date = GetDeliveryDate(Profile, deliveryZoneCode, deliveryZoneType)
                Dim deliveryZoneDay As String = ""
                If deliveryZoneDate = Date.MinValue Then
                    deliveryZoneType = "2"
                Else
                    deliveryZoneDay = deliveryZoneDate.DayOfWeek.ToString.ToUpper
                End If
                Dim deliveryDateFormat As String = Utilities.CheckForDBNull_String(ucr.Attribute("DeliveryDateFormat")).Trim
                If deliveryDateFormat.Length <= 0 Then
                    deliveryDateFormat = "dd/MM/yyyy"
                End If
                DeliveryDate.Text = deliveryZoneDate.ToString(deliveryDateFormat)

                Select Case deliveryZoneType
                    Case Is = "1"
                        ltlDeliveryMessage.Text = ucr.Content("DeliveryMessage1", _languageCode, True).Replace("<<DELIVERY_SLOT>>", deliveryZoneDay)
                        DeliveryDay.Text = ucr.Content("DeliveryMessage1", _languageCode, True).Replace("<<DELIVERY_SLOT>>", deliveryZoneDay)
                    Case Is = "2"
                        ltlDeliveryMessage.Text = ucr.Content("DeliveryMessage2", _languageCode, True)
                        DeliveryDay.Text = ucr.Content("DeliveryMessage2", _languageCode, True)
                    Case Else
                End Select
                Session("DeliveryDate") = deliveryZoneDate
                If PreferredDateRow.Visible Then
                    Dim dtPreferredDeliveryDates As DataTable = GetPreferredDeliveryDates(Profile, deliveryZoneCode, deliveryZoneType)
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
                        ltlPreferredDatesScript.Visible = True
                        ltlPreferredDatesScript.Text = "" & _
                                "<script type=""text/javascript"">" & vbCrLf & _
                                "<!--" & vbCrLf & _
                                "var availableDays = [" & strPreferredDates & "];" & vbCrLf & _
                                "//-->" & vbCrLf & _
                                "</script>"
                    End If
                    PreferredDate.Text = deliveryZoneDate.ToString(deliveryDateFormat)
                End If
            End If
        Catch ex As Exception
            Logging.WriteLog(Profile.UserName, "UCCAPA-010", ex.Message, "Error populating address fields from profile PopulateAddressFromDDL()", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
        End Try
    End Sub

    Protected Sub PopulateAddressDDL()
        Dim count As Integer = 0
        Dim addressSelected As Boolean = False
        For i As Integer = 0 To Profile.User.Addresses.Count - 1
            Dim ta As New TalentProfileAddress
            ta = ProfileHelper.ProfileAddressEnumerator(i, Profile.User.Addresses)
            '----------------------------------------------------
            ' Don't add if we're using a default delivery country
            ' unless the address matches the delivery country
            '----------------------------------------------------
            Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
            Dim defaultCountry As String = String.Empty
            Dim countryMatch As Boolean = True
            If def.UseDefaultCountryOnDeliveryAddress Then
                defaultCountry = TalentCache.GetDefaultCountryForBU()
                If defaultCountry <> String.Empty Then
                    countryMatch = False
                    '----------------------------------------------------
                    ' Loop through address dropdown to find address code.
                    ' Then check if it matches default country.
                    '----------------------------------------------------
                    For Each li As ListItem In CountryDDL.Items
                        If UCase(li.Value) = UCase(ta.Country) OrElse UCase(li.Text) = UCase(ta.Country) Then
                            If UCase(li.Value) = UCase(defaultCountry) Then
                                countryMatch = True
                            End If
                            Exit For
                        End If
                    Next
                End If
            End If

            If countryMatch Then

                If ta.Default_Address AndAlso count = 0 Then
                    Try
                        SelectAddressDDL.Items.Insert(0, New ListItem(ta.Reference, i))
                        SelectAddressDDL.Items(0).Selected = True
                        count += 1
                        addressSelected = True
                    Catch ex As Exception
                    End Try
                Else
                    '------------------------------------
                    ' Don't add if it's a REGISTERED type
                    '------------------------------------
                    If ta.Type <> "1" Then
                        SelectAddressDDL.Items.Add(New ListItem(ta.Reference, i))
                        addressSelected = True
                    End If
                End If
            End If
        Next
        If Not addressSelected AndAlso Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("MakeAddressFieldsReadOnly")) Then
            If Profile.User.Addresses.Count > 0 Then
                Dim ta As New TalentProfileAddress
                ta = ProfileHelper.ProfileAddressEnumerator(0, Profile.User.Addresses)
                Try
                    SelectAddressDDL.Items.Insert(0, New ListItem(ta.Reference, 0))
                    SelectAddressDDL.Items(0).Selected = True
                    count += 1
                    addressSelected = True
                Catch ex As Exception
                End Try
            End If
        End If

        'check to see if the "add new address text" has been set
        'if it is blank we need to force the address fields to be readonly
        Dim addNewAddressText As String = ucr.Content("AddNewAddressText", _languageCode, True)
        If String.IsNullOrEmpty(addNewAddressText) Then
            If ucr.Attribute("MakeAddressFieldsReadOnly").ToUpper = "TRUE" Then
                building.ReadOnly = True
                Address2.ReadOnly = True
                Address3.ReadOnly = True
                Address4.ReadOnly = True
                Address5.ReadOnly = True
                postcode.ReadOnly = True
                CountryDDL.Enabled = False
                DeliveryContact.ReadOnly = True
                DeliveryInstructions.ReadOnly = True
                PurchaseOrder.ReadOnly = True
                SaveAddress.Enabled = False
                proceed.CausesValidation = False
            End If
        Else
            SelectAddressDDL.Items.Add(addNewAddressText)
        End If
    End Sub

    Protected Sub PopulateCountryDDL()
        CountryDDL.DataSource = TalentCache.GetDropDownControlText(Utilities.GetCurrentLanguageForDDLPopulation, "DELIVERY", "COUNTRY")
        CountryDDL.DataTextField = "Text"
        CountryDDL.DataValueField = "Value"
        CountryDDL.DataBind()
        '----------------------------------------------------------------------------------------
        ' If set up to use default country on module defaults and a default country is found then
        ' set the default country and protect it
        '----------------------------------------------------------------------------------------
        Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
        Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
        If def.UseDefaultCountryOnDeliveryAddress Then
            Dim defaultCountry As String = TalentCache.GetDefaultCountryForBU()
            If defaultCountry <> String.Empty Then
                CountryDDL.SelectedValue = defaultCountry
                CountryDDL.Enabled = False
            End If

        End If

    End Sub

    Protected Sub SetLabelText()
        With ucr
            TitleText.Text = .Content("TitleText", _languageCode, True)
            InstructionsText.Text = .Content("InstructionsText", _languageCode, True)
            SelectAddressLabel.Text = .Content("SelectAddressLabel", _languageCode, True)
            BuildingLabel.Text = .Content("HouseNoLabel", _languageCode, True)
            PostcodeLabel.Text = .Content("PostcodeLabel", _languageCode, True)
            AddressLabel2.Text = .Content("AddressLabel2", _languageCode, True)
            AddressLabel3.Text = .Content("AddressLabel3", _languageCode, True)
            AddressLabel4.Text = .Content("AddressLabel4", _languageCode, True)
            AddressLabel5.Text = .Content("AddressLabel5", _languageCode, True)
            countryLabel.Text = .Content("CountryLabel", _languageCode, True)
            DeliveryContactLabel.Text = .Content("DeliveryContactLabel", _languageCode, True)
            DeliveryInsructionsLabel.Text = .Content("DeliveryInsructionsLabel", _languageCode, True)
            SaveAddress.Text = .Content("SaveAddressText", _languageCode, True)
            tandc.Text = .Content("TermsAndConditionsText", _languageCode, True)
            proceed.Text = .Content("ProceedButtonText", _languageCode, True)
            PurchaseOrderLabel.Text = .Content("PurchaseOrderText", _languageCode, True)
            DeliveryDayLabel.Text = .Content("DeliveryDayLabel", _languageCode, True)
            DeliveryDateLabel.Text = .Content("DeliveryDateLabel", _languageCode, True)
            'DeliveryDay
            'DeliveryDate
            'PreferredDate
            PreferredDateLabel.Text = .Content("PreferredDateLabel", _languageCode, True)
            'HelpLabel Text
            SelectAddressHelpLabel.Text = .Content("SelectAddressHelpLabel", _languageCode, True)
            DeliveryInsructionsHelpLabel.Text = .Content("DeliveryInsructionsHelpLabel", _languageCode, True)
            PurchaseOrderHelpLabel.Text = .Content("PurchaseOrderHelpLabel", _languageCode, True)
            DeliveryDateHelpLabel.Text = .Content("DeliveryDateHelpLabel", _languageCode, True)
            PreferredDateHelpLabel.Text = .Content("PreferredDateHelpLabel", _languageCode, True)
        End With
    End Sub

    Protected Sub SetupValidation()

        With ucr

            ' Required Fields
            '-----------------------
            If .Content("HouseNoMissingErrorText", _languageCode, True) = "" Then
                BuildingRFV.Enabled = False
            Else
                BuildingRFV.ErrorMessage = .Content("HouseNoMissingErrorText", _languageCode, True)
            End If
            If .Content("PostcodeMissingErrorText", _languageCode, True) = "" Then
                postcodeRFV.Enabled = False
            Else
                postcodeRFV.ErrorMessage = .Content("PostcodeMissingErrorText", _languageCode, True)
            End If
            If .Content("AddressLine2MissingErrorText", _languageCode, True) = "" Then
                Address2RFV.Enabled = False
            Else
                Address2RFV.ErrorMessage = .Content("AddressLine2MissingErrorText", _languageCode, True)
            End If
            If .Content("AddressLine3MissingErrorText", _languageCode, True) = "" Then
                Address3RFV.Enabled = False
            Else
                Address3RFV.ErrorMessage = .Content("AddressLine3MissingErrorText", _languageCode, True)
            End If
            If .Content("AddressLine4MissingErrorText", _languageCode, True) = "" Then
                Address4RFV.Enabled = False
            Else
                Address4RFV.ErrorMessage = .Content("AddressLine4MissingErrorText", _languageCode, True)
            End If
            If .Content("AddressLine5MissingErrorText", _languageCode, True) = "" Then
                Address5RFV.Enabled = False
            Else
                Address5RFV.ErrorMessage = .Content("AddressLine5MissingErrorText", _languageCode, True)
            End If
            If .Content("PurchaseOrderMissingErrorText", _languageCode, True) = "" Then
                PurchaseOrderRFV.Enabled = False
            Else
                PurchaseOrderRFV.ErrorMessage = .Content("PurchaseOrderMissingErrorText", _languageCode, True)
            End If
            If .Content("DeliveryContactMissingErrorText", _languageCode, True) = "" Then
                DeliveryContactRFV.Enabled = False
            Else
                DeliveryContactRFV.ErrorMessage = .Content("DeliveryContactMissingErrorText", _languageCode, True)
            End If
            If .Content("DeliveryInstructionsMissingErrorText", _languageCode, True) = "" Then
                DeliveryInstructionsRFV.Enabled = False
            Else
                DeliveryInstructionsRFV.ErrorMessage = .Content("DeliveryInstructionsMissingErrorText", _languageCode, True)
            End If

            ' Regular Expressions
            '-------------------------
            BuildingRegEx.ErrorMessage = .Content("HouseNoInvalidErrorText", _languageCode, True)
            BuildingRegEx.ValidationExpression = .Attribute("TextAndNumbersExpression")
            postcodeRegEx.ErrorMessage = .Content("PostcodeInvalidErrorText", _languageCode, True)
            postcodeRegEx.ValidationExpression = .Attribute("PostcodeExpression")
            Address2RegEx.ErrorMessage = .Content("Address2InvalidErrorText", _languageCode, True)
            Address2RegEx.ValidationExpression = .Attribute("TextAndNumbersExpression")
            Address3RegEx.ErrorMessage = .Content("Address3InvalidErrorText", _languageCode, True)
            Address3RegEx.ValidationExpression = .Attribute("TextOnlyExpression")
            Address4RegEx.ErrorMessage = .Content("Address4InvalidErrorText", _languageCode, True)
            Address4RegEx.ValidationExpression = .Attribute("TextOnlyExpression")
            Address5RegEx.ErrorMessage = .Content("Address5InvalidErrorText", _languageCode, True)
            Address5RegEx.ValidationExpression = .Attribute("TextOnlyExpression")
            PurchaseOrderRegEx.ValidationExpression = .Attribute("TextAndNumbersExpression")
            PurchaseOrderRegEx.ErrorMessage = .Content("PurchaseOrderInvalidErrorText", _languageCode, True)
            CountryDDLRegEx.ErrorMessage = .Content("NoCountrySelectedErrorText", _languageCode, True)
            ' CountryDDLRegEx.ValidationExpression = "^(?! -- )?[a-zA-Z\s]+"
            CountryDDLRegEx.ValidationExpression = "^[a-zA-Z\s]{0,50}$"
            DeliveryContactRegEx.ErrorMessage = .Content("DeliveryContactNameInvalidErrorText", _languageCode, True)
            DeliveryContactRegEx.ValidationExpression = .Attribute("TextOnlyExpression")
            DeliveryInstructionsRegEx.ErrorMessage = .Content("DeliveryInstructionsInvalidErrorText", _languageCode, True)
            DeliveryInstructionsRegEx.ValidationExpression = .Attribute("TextAndPuctExpression")
            PreferredDateRegEx.ValidationExpression = .Attribute("PreferredDateExpression")
            PreferredDateRegEx.ErrorMessage = .Content("PreferredDateInvalidErrorText", _languageCode, True)

            'Readonly Fields
            If (Utilities.CheckForDBNull_Boolean_DefaultFalse(.Attribute("ReadOnlyAddressLine1"))) Then building.ReadOnly = True
            If (Utilities.CheckForDBNull_Boolean_DefaultFalse(.Attribute("ReadOnlyAddressLine2"))) Then Address2.ReadOnly = True
            If (Utilities.CheckForDBNull_Boolean_DefaultFalse(.Attribute("ReadOnlyAddressLine3"))) Then Address3.ReadOnly = True
            If (Utilities.CheckForDBNull_Boolean_DefaultFalse(.Attribute("ReadOnlyAddressLine4"))) Then Address4.ReadOnly = True
            If (Utilities.CheckForDBNull_Boolean_DefaultFalse(.Attribute("ReadOnlyAddressLine5"))) Then Address5.ReadOnly = True
            If (Utilities.CheckForDBNull_Boolean_DefaultFalse(.Attribute("ReadOnlyAddressPostcode"))) Then postcode.ReadOnly = True
            If (Utilities.CheckForDBNull_Boolean_DefaultFalse(.Attribute("ReadOnlyAddressCountry"))) Then CountryDDL.Enabled = False
            If (Utilities.CheckForDBNull_Boolean_DefaultFalse(.Attribute("ReadOnlyDeliveryContact"))) Then DeliveryContact.ReadOnly = True

            'max length attributes
            PurchaseOrder.MaxLength = Utilities.CheckForDBNull_Int(.Attribute("PurchaseOrderMaxLength"))

        End With
    End Sub

    Protected Sub proceed_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles proceed.Click
        Try
            Session.Add("CheckoutBasketState", Profile.Basket)
        Catch ex As Exception
        End Try
        Checkout.CheckBasketValidity()

        ErrorLabel.Text = String.Empty
        Dim addressExternalID As String = String.Empty
        Dim deliveryDate As Date = Date.MinValue
        If Session("DeliveryDate") IsNot Nothing Then
            deliveryDate = Session("DeliveryDate")
        End If
        If ucr.Content("TermsAndConsNotTickedErrorText", _languageCode, True).Trim.Length > 0 Then
            If (tandc.Visible) AndAlso (Not tandc.Checked) Then
                ErrorLabel.Text = ucr.Content("TermsAndConsNotTickedErrorText", _languageCode, True)
                Exit Sub
            End If
        End If
        If SelectAddressDDL.SelectedValue = ucr.Content("AddNewAddressText", _languageCode, True) Then
            If SaveAddress.Checked Then
                Try
                    Dim ta As TalentProfileAddress
                    Dim reference As String = String.Empty
                    If String.IsNullOrEmpty(building.Text.Trim) Then
                        reference = building.Text & " " & Address2.Text
                    Else
                        reference = building.Text & " " & Address2.Text
                    End If
                    If Not Profile.User.Addresses.ContainsKey(reference) Then
                        '--------------------------------
                        ' Reference doesn't already exist
                        '--------------------------------
                        SaveAddressToDB(False)
                    Else
                        '-------------------------
                        ' Reference already exists
                        '-------------------------
                        ta = Profile.User.Addresses(reference)
                        If Not UCase(ta.Post_Code) = UCase(postcode.Text) Then
                            SaveAddressToDB(True)
                        Else
                            'The address already exists on this profile and so cannot be added
                            ErrorLabel.Text = ucr.Content("AddressExistsErrorText", _languageCode, True)
                            Exit Sub
                        End If
                    End If
                    'If String.IsNullOrEmpty(building.Text.Trim) Then
                    '    ta = Profile.User.Addresses(Address2.Text & " " & Address3.Text)
                    'Else
                    '    ta = Profile.User.Addresses(building.Text & " " & Address2.Text)
                    'End If
                    'If Not UCase(ta.Post_Code) = UCase(postcode.Text) Then
                    '    SaveAddressToDB(True)
                    'Else
                    '    'The address already exists on this profile and so cannot be added
                    '    ErrorLabel.Text = ucr.Content("AddressExistsErrorText", _languageCode, True)
                    '    Exit Sub
                    'End If
                Catch ex As Exception
                    ' SaveAddressToDB(False)
                End Try
            End If
            addressExternalID = String.Empty
        Else
            If Not SelectAddressDDL.SelectedItem Is Nothing Then
                With Profile.User.Addresses(SelectAddressDDL.SelectedItem.Text)
                    If SaveAddress.Checked Then
                        .Address_Line_1 = building.Text
                        .Address_Line_2 = Address2.Text
                        .Address_Line_3 = Address3.Text
                        .Address_Line_4 = Address4.Text
                        .Address_Line_5 = Address5.Text
                        .Country = CountryDDL.SelectedItem.Text
                        .Default_Address = True
                        .Post_Code = UCase(postcode.Text)
                        .Reference = building.Text & " " & Address2.Text
                        Profile.Save()
                    End If
                    Profile.Provider.UpdateDefaultAddress(.LoginID, TalentCache.GetPartner(Profile), .Address_ID)
                End With
                addressExternalID = Profile.User.Addresses(SelectAddressDDL.SelectedItem.Text).External_ID
                'decides the final delivery date
                Dim preferredDateExists As Boolean = True
                If PreferredDateRow.Visible Then
                    If PreferredDate.Text.Length > 0 Then
                        If Session("dtPreferredDeliveryDates") IsNot Nothing Then
                            Dim dtPreferredDeliveryDates As DataTable = CType(Session("dtPreferredDeliveryDates"), DataTable)
                            If dtPreferredDeliveryDates.Rows.Count > 0 Then
                                preferredDateExists = False
                                Dim deliveryDateFormat As String = Utilities.CheckForDBNull_String(ucr.Attribute("DeliveryDateFormat")).Trim
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
                                    ErrorLabel.Text = ucr.Content("PreferredDateForZoneError", _languageCode, True)
                                    Exit Sub
                                End If
                            End If
                            Session("dtPreferredDeliveryDates") = Nothing
                            Session.Remove("dtPreferredDeliveryDates")
                        End If
                    End If
                End If
            End If
        End If
        Dim country As String = ""
        If defs.StoreCountryAsWholeName Then
            country = CountryDDL.SelectedItem.Text
        Else
            country = CountryDDL.SelectedItem.Value
        End If

        Dim order As New Order(DeliveryInstructions.Text, _
                                DeliveryContact.Text, _
                                building.Text, _
                                Address2.Text, _
                                Address3.Text, _
                                Address4.Text, _
                                Address5.Text, _
                                postcode.Text, _
                                country, _
                                PurchaseOrder.Text, _
                                addressExternalID, _
                                deliveryDate, CountryDDL.SelectedItem.Value)

        'If CreateOrder() Then
        If order.CreateOrder() Then
            Try
                Dim status As New TalentBasketDatasetTableAdapters.tbl_order_statusTableAdapter
                status.Insert_Order_Status_Flow(TalentCache.GetBusinessUnit, _
                                    Profile.Basket.TempOrderID, _
                                    Talent.Common.Utilities.GetOrderStatus("DELIVERY"), _
                                    Now, _
                                    "")
            Catch ex As Exception
                Logging.WriteLog(Profile.UserName, "UCCAPR-010", ex.Message, "Error Inserting Order Status", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
            End Try
            '            Response.Redirect("~/PagesLogin/Checkout/CheckoutOrderSummary.aspx")
        Else
            ErrorLabel.Text = ucr.Content("OrderCreationErrorText", _languageCode, True)
        End If

    End Sub

    Protected Function SaveAddressToDB(ByVal StorePostcode As Boolean) As Boolean
        Try
            Dim ta As New TalentProfileAddress
            With ta
                .LoginID = Profile.User.Details.LoginID
                If StorePostcode Then
                    If String.IsNullOrEmpty(building.Text.Trim) Then
                        .Reference = Address2.Text & " " & Address3.Text & " (" & UCase(postcode.Text) & ")"
                    Else
                        .Reference = building.Text & " " & Address2.Text & " (" & UCase(postcode.Text) & ")"
                    End If
                Else
                    If String.IsNullOrEmpty(building.Text.Trim) Then
                        .Reference = Address2.Text & " " & Address3.Text
                    Else
                        .Reference = building.Text & " " & Address2.Text
                    End If
                End If
                .Type = ""
                .Default_Address = True
                .Address_Line_1 = building.Text
                .Address_Line_2 = Address2.Text
                .Address_Line_3 = Address3.Text
                .Address_Line_4 = Address4.Text
                .Address_Line_5 = Address5.Text
                .Post_Code = UCase(postcode.Text)
                .Country = CountryDDL.SelectedValue
                .Sequence = GetNextSequenceNo()
            End With

            'finally, check to see if the address reference is alrady taken
            'Dim testAddress As TalentProfileAddress = Profile.User.Addresses.ContainsKey(ta.Reference)
            ' If testAddress Is Nothing Then
            If Not Profile.User.Addresses.ContainsKey(ta.Reference) Then
                Profile.Provider.AddAddressToUserProfile(ta)
            Else
                'address already exists
                'The address already exists on this profile and so cannot be added
                ErrorLabel.Text = ucr.Content("AddressExistsErrorText", _languageCode, True)
                Return False
            End If
            Try
                Profile.Provider.UpdateDefaultAddress(ta.LoginID, Profile.PartnerInfo.Details.Partner, ta.Address_ID)
            Catch ex As Exception
                Logging.WriteLog(Profile.UserName, "UCCASA-020", ex.Message, "Error updating the user's default address", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
            End Try
        Catch ex As Exception
            Logging.WriteLog(Profile.UserName, "UCCASA-010", ex.Message, "Error adding a new address for the user", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
        End Try

        Return True
    End Function

    Protected Function GetNextSequenceNo() As Integer
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

    Protected Sub SelectAddressDDL_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles SelectAddressDDL.SelectedIndexChanged
        If SelectAddressDDL.SelectedValue = ucr.Content("AddNewAddressText", _languageCode, True) Then
            building.Text = String.Empty
            postcode.Text = String.Empty
            Address2.Text = String.Empty
            Address3.Text = String.Empty
            Address4.Text = String.Empty
            Address5.Text = String.Empty
            CountryDDL.SelectedIndex = 0
            '----------------------------------------------------------------------------------------
            ' If set up to use default country on module defaults and a default country is found then
            ' set the default country and protect it
            '----------------------------------------------------------------------------------------
            Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
            If def.UseDefaultCountryOnDeliveryAddress Then
                Dim defaultCountry As String = TalentCache.GetDefaultCountryForBU()
                If defaultCountry <> String.Empty Then
                    CountryDDL.SelectedValue = defaultCountry
                    CountryDDL.Enabled = False
                End If

            End If

            DeliveryContact.Text = Profile.User.Details.Full_Name
            SaveAddress.Text = ucr.Content("SaveAddressText", _languageCode, True)
            SaveAddress.Visible = True
        Else
            PopulateAddressFromDDL()
            'SaveAddress.Text = ucr.Content("UpdateAddressText", _languageCode, True)
            SaveAddress.Visible = False
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Not Page.IsPostBack Then
            If ucr.Content("TermsAndConsNotTickedErrorText", _languageCode, True).Trim.Length <= 0 Then
                tandc.Visible = False
            End If
            If UserUnderAge() Then
                Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
            Else
                If Not defs.AllowCheckoutWhenNoStock AndAlso _
                     Not AllInStock_BackEndCheck() Then
                    Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                End If

                '-------------------------------------------------------
                ' Check for discontinued products which are out of stock
                '-------------------------------------------------------
                If defs.PerformDiscontinuedProductCheck Then
                    For Each bi As TalentBasketItem In Profile.Basket.BasketItems
                        If bi.STOCK_ERROR AndAlso bi.STOCK_ERROR_CODE = "DISC" Then
                            Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                        End If
                    Next
                End If

                '-------------------------------------------------------
                ' Check for mandatory account codes
                '-------------------------------------------------------
                If Not Profile.PartnerInfo.Details.COST_CENTRE Is Nothing And Not Profile.PartnerInfo.Details.COST_CENTRE Is String.Empty Then
                    For Each bi As TalentBasketItem In Profile.Basket.BasketItems
                        If bi.Cost_Centre = Nothing Or bi.Cost_Centre = String.Empty Or bi.Account_Code = Nothing Or bi.Account_Code = String.Empty Then
                            Session("TalentErrorCode") = "CC"
                            Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                        End If
                    Next
                End If

                '-------------------------------------
                ' Check for alt products
                ' If any exist then redirect to basket
                ' allowing user to select alt products
                '-------------------------------------
                If defs.RetrieveAlternativeProductsAtCheckout Then
                    Dim ds As New Data.DataSet
                    ds = RetrieveAlternativeProducts()

                    If Not ds Is Nothing AndAlso ds.Tables.Count > 0 AndAlso ds.Tables.Item("ALTPRODUCTRESULTS").Rows.Count > 0 Then
                        ' Save to session - this will be checked and cleared immediately in basket
                        Session("AlternativeProducts") = ds
                        Session.Remove("CheckoutBreadCrumbTrail")
                        Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                    End If

                End If
                If defs.DISPLAY_PAGE_BEFORE_CHECKOUT Then
                    If Session.Item("CheckoutPromotionsShown") Is Nothing _
                        OrElse Not CBool(Session.Item("CheckoutPromotionsShown")) Then
                        Session.Item("CheckoutPromotionsShown") = True
                        Response.Redirect(defs.PAGE_BEFORE_CHECKOUT)
                    End If
                End If

                SetLabelText()
                SetupValidation()
                PopulateCountryDDL()
                PopulateAddressDDL()

                SetAddressVisibilityProperties()
                SetAddressVisibility()

                '-------------------------------------
                ' Check if need to show 'Save Address'
                '-------------------------------------
                If SelectAddressDDL.SelectedValue <> ucr.Content("AddNewAddressText", _languageCode, True) Then
                    SaveAddress.Visible = False
                    PopulateAddressFromDDL()
                Else
                    SaveAddress.Text = ucr.Content("SaveAddressText", _languageCode, True)
                    SaveAddress.Visible = True
                End If

                '   SaveAddress.Visible = False
            End If
            'Redirect to payment if basket content type is only ticketing
            'Select Case Profile.Basket.BasketContentType
            '    Case "T"
            '        Session.Remove("CheckoutBreadCrumbTrail")
            '        Me.CreateOrderHeader()
            '        Session.Add("CheckoutBasketState", Profile.Basket)
            '        Response.Redirect("~/PagesLogin/Checkout/CheckoutPaymentDetails.aspx")
            '    Case Else
            'End Select

            '---------------------------------------
            ' Check what stage checkout should start 
            ' (i.e. skip the delivery address page?)
            '---------------------------------------

            'Try
            '    'Add the Basket Session Variable at the start of the Checkout
            '    Session.Add("CheckoutBasketState", Profile.Basket)
            'Catch ex As Exception
            'End Try

            'If Not String.IsNullOrEmpty(defs.FirstCheckoutPage) Then

            '    If defs.FirstCheckoutPage.ToUpper <> "CHECKOUTDELIVERYDETAILS.ASPX" Then
            '        If Me.CreateOrderHeader Then
            '            Select Case defs.FirstCheckoutPage.ToUpper
            '                Case "CHECKOUTORDERSUMMARY.ASPX"
            '                    Select Case Profile.Basket.BasketContentType
            '                        Case "M", "C"
            '                            Checkout.CheckBasketValidity()
            '                            Session.Remove("CheckoutBreadCrumbTrail")
            '                            Response.Redirect("~/PagesLogin/Checkout/CheckoutOrderSummary.aspx")
            '                        Case "T"
            '                            Session.Remove("CheckoutBreadCrumbTrail")
            '                            Response.Redirect("~/PagesLogin/Checkout/CheckoutPaymentDetails.aspx")
            '                        Case Else
            '                            Session.Remove("CheckoutBreadCrumbTrail")
            '                            Session.Remove("CheckoutBasketState")
            '                            Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
            '                    End Select
            '                Case "CHECKOUTPAYMENTDETAILS.ASPX"
            '                    Select Case Profile.Basket.BasketContentType
            '                        Case "T", "M", "C"
            '                            Session.Remove("CheckoutBreadCrumbTrail")
            '                            Response.Redirect("~/PagesLogin/Checkout/CheckoutPaymentDetails.aspx")
            '                        Case Else
            '                            Session.Remove("CheckoutBreadCrumbTrail")
            '                            Session.Remove("CheckoutBasketState")
            '                            Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
            '                    End Select
            '            End Select
            '        End If
            '    Else
            '        Select Case Profile.Basket.BasketContentType
            '            Case "M", "C"
            '                Checkout.CheckBasketValidity()
            '            Case "T"
            '                Session.Remove("CheckoutBreadCrumbTrail")
            '                Me.CreateOrderHeader()
            '                Response.Redirect("~/PagesLogin/Checkout/CheckoutPaymentDetails.aspx")
            '            Case Else
            '                Session.Remove("CheckoutBreadCrumbTrail")
            '                Session.Remove("CheckoutBasketState")
            '                Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
            '        End Select
            '    End If

            'Else
            '    Select Case Profile.Basket.BasketContentType
            '        Case "M", "C"
            '            Checkout.CheckBasketValidity()

            '        Case "T"
            '            Me.CreateOrderHeader()
            '            Session.Remove("CheckoutBreadCrumbTrail")
            '            Response.Redirect("~/PagesLogin/Checkout/CheckoutPaymentDetails.aspx")
            '        Case Else
            '            Session.Remove("CheckoutBreadCrumbTrail")
            '            Session.Remove("CheckoutBasketState")
            '            Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
            '    End Select
            'End If


            If Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("MakeAddressFieldsReadOnly")) Then
                building.ReadOnly = True
                postcode.ReadOnly = True
                Address2.ReadOnly = True
                Address3.ReadOnly = True
                Address4.ReadOnly = True
                Address5.ReadOnly = True
                CountryDDL.Enabled = False
            End If

            'check to see if we need to show a delivery message
            If ucr.Attribute("DisplayDeliveryMessage").ToUpper = "TRUE" Then
                plhDeliveryMessage.Visible = True
            End If

        End If

        If defs.UseEPOSOptions Then Response.Redirect("~/PagesLogin/Checkout/CheckoutOrderSummary.aspx")
    End Sub

    Protected Function CreateOrderHeader() As Boolean
        Dim createOrderOK As Boolean = False
        '-------------
        ' Create order
        '-------------
        Try
            Dim ta As New TalentProfileAddress
            ta = Profile.User.Addresses(SelectAddressDDL.SelectedItem.Text)

            Dim order As New Order(DeliveryInstructions.Text, _
                                     DeliveryContact.Text, _
                                     ta.Address_Line_1, _
                                     ta.Address_Line_2, _
                                     ta.Address_Line_3, _
                                     ta.Address_Line_4, _
                                     ta.Address_Line_5, _
                                     ta.Post_Code, _
                                     ta.Country, _
                                     PurchaseOrder.Text, _
                                     ta.External_ID)

            If order.CreateOrder() Then
                createOrderOK = True
                Try
                    Dim status As New TalentBasketDatasetTableAdapters.tbl_order_statusTableAdapter
                    status.Insert_Order_Status_Flow(TalentCache.GetBusinessUnit, _
                                        Profile.Basket.TempOrderID, _
                                        Talent.Common.Utilities.GetOrderStatus("DELIVERY"), _
                                        Now, _
                                        "")
                Catch ex As Exception
                    Logging.WriteLog(Profile.UserName, "UCCAPR-010", ex.Message, "Error Inserting Order Status", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
                End Try

            Else
                createOrderOK = False
                ErrorLabel.Text = ucr.Content("OrderCreationErrorText", _languageCode, True)
            End If
        Catch ex As Exception
            Logging.WriteLog(Profile.UserName, "UCCAPR-010", ex.Message, "Error Inserting Order Status", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
            createOrderOK = False
            ErrorLabel.Text = ucr.Content("OrderCreationErrorText", _languageCode, True)
        End Try

        Return createOrderOK
    End Function



    Protected Sub SetAddressVisibility()

        Dim eComDefs As New ECommerceModuleDefaults
        Dim defs As ECommerceModuleDefaults.DefaultValues = eComDefs.GetDefaults

        If Not addressLine1RowVisible Then
            AddressLine1Row.Visible = False
            BuildingRFV.Enabled = False
            BuildingRegEx.Enabled = False
        End If
        If Not addressLine2RowVisible Then
            AddressLine2Row.Visible = False
            Address2RFV.Enabled = False
            Address2RegEx.Enabled = False
        End If
        If Not addressLine3RowVisible Then
            AddressLine3Row.Visible = False
            Address3RegEx.Enabled = False
        End If
        If Not addressLine4RowVisible Then
            AddressLine4Row.Visible = False
            Address4RegEx.Enabled = False
        End If
        If Not addressLine5RowVisible Then
            AddressLine5Row.Visible = False
            Address5RFV.Enabled = False
            Address5RegEx.Enabled = False
        End If
        If Not addressPostcodeRowVisible Then
            AddressPostcodeRow.Visible = False
            postcodeRFV.Enabled = False
            postcodeRegEx.Enabled = False
        End If
        If Not addressCountryRowVisible Then
            AddressCountryRow.Visible = False
            CountryDDLRegEx.Enabled = False
        End If
        If Not ucr.Attribute("addressingOnOff").Trim = "" Then
            If Not CType(ucr.Attribute("addressingOnOff"), Boolean) Then
                FindAddressButtonRow.Visible = False
            End If
        End If

    End Sub
    Protected Sub SetAddressVisibilityProperties()

        Dim eComDefs As New ECommerceModuleDefaults
        Dim defs As ECommerceModuleDefaults.DefaultValues = eComDefs.GetDefaults

        ' 
        ' Set common address field visibility using system defaults, and then override by any control-level defaults.
        If Not defs.AddressLine1RowVisible Then
            addressLine1RowVisible = False
        End If
        If Not defs.AddressLine2RowVisible Then
            addressLine2RowVisible = False
        End If
        If Not defs.AddressLine3RowVisible Then
            addressLine3RowVisible = False
        End If
        If Not defs.AddressLine4RowVisible Then
            addressLine4RowVisible = False
        End If
        If Not defs.AddressLine5RowVisible Then
            addressLine5RowVisible = False
        End If
        If Not defs.AddressPostcodeRowVisible Then
            addressPostcodeRowVisible = False
        End If
        If Not defs.AddressCountryRowVisible Then
            addressCountryRowVisible = False
        End If

        '
        ' Control-level overrides for visibility (these DO NOT need to exist on tbl_control_attributes)
        If Not ucr.Attribute("addressLine1RowVisible").Trim = "" Then
            If Not CType(ucr.Attribute("addressLine1RowVisible"), Boolean) Then
                addressLine1RowVisible = False
            Else
                addressLine1RowVisible = True
            End If
        End If
        If Not ucr.Attribute("addressLine2RowVisible").Trim = "" Then
            If Not CType(ucr.Attribute("addressLine2RowVisible"), Boolean) Then
                addressLine2RowVisible = False
            Else
                addressLine2RowVisible = True
            End If
        End If
        If Not ucr.Attribute("addressLine3RowVisible").Trim = "" Then
            If Not CType(ucr.Attribute("addressLine3RowVisible"), Boolean) Then
                addressLine3RowVisible = False
            Else
                addressLine3RowVisible = True
            End If
        End If
        If Not ucr.Attribute("addressLine4RowVisible").Trim = "" Then
            If Not CType(ucr.Attribute("addressLine4RowVisible"), Boolean) Then
                addressLine4RowVisible = False
            Else
                addressLine4RowVisible = True
            End If
        End If
        If Not ucr.Attribute("addressLine5RowVisible").Trim = "" Then
            If Not CType(ucr.Attribute("addressLine5RowVisible"), Boolean) Then
                addressLine5RowVisible = False
            Else
                addressLine5RowVisible = True
            End If
        End If
        If Not ucr.Attribute("addressPostcodeRowVisible").Trim = "" Then
            If Not CType(ucr.Attribute("addressPostcodeRowVisible"), Boolean) Then
                addressPostcodeRowVisible = False
            Else
                addressPostcodeRowVisible = True
            End If
        End If
        If Not ucr.Attribute("addresscountryRowVisible").Trim = "" Then
            If Not CType(ucr.Attribute("addressCountryRowVisible"), Boolean) Then
                addressCountryRowVisible = False
            Else
                addressCountryRowVisible = True
            End If
        End If

        '
        ' Now the control-specific fields (these DO need to exist on tbl_control_attributes)
        If Not Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("addressingOnOff")) Then
            FindAddressButtonRow.Visible = False
        End If

        ' New to control PurchaseOrder this is required in tbl_control_attributes

        '  If ucr.Attribute("usePurchaseOrder") = "False" Then
        If Not CBool(ucr.Attribute("usePurchaseOrder")) Then
            PurchaseOrderRow.Visible = False
            PurchaseOrderRFV.EnableClientScript = False
            PurchaseOrderRegEx.EnableClientScript = False
        ElseIf Not CBool(ucr.Attribute("purchaseOrderRequired")) Then
            PurchaseOrderRFV.EnableClientScript = False
        End If
        DeliveryDayRow.Visible = False
        DeliveryDateRow.Visible = False
        PreferredDateRow.Visible = False
        If Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("deliveryDayRowVisible")) Then
            DeliveryDayRow.Visible = True
        End If
        If Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("deliveryDateRowVisible")) Then
            DeliveryDateRow.Visible = True
        End If
        If Profile.PartnerInfo.Details.SHOW_PREFERRED_DELIVERY_DATE Then
            PreferredDateRow.Visible = True
        End If
    End Sub

    Protected Function GetAddressingLinkText() As String
        Return ucr.Content("addressingLinkButtonText", _languageCode, True)
    End Function
    Protected Sub CreateAddressingJavascript()

        If Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("addressingOnOff")) Then

            Dim defaults As ECommerceModuleDefaults.DefaultValues
            Dim defs As New ECommerceModuleDefaults
            Dim sString As String = String.Empty
            defaults = defs.GetDefaults


            Response.Write(vbCrLf & "<script language=""javascript"" type=""text/javascript"">" & vbCrLf)

            Select Case defaults.AddressingProvider.ToUpper

                Case Is = "QAS"
                    ' Create function to open child window
                    Response.Write("function addressingPopup() {" & vbCrLf)
                    Response.Write("win1 = window.open('../../PagesPublic/QAS/FlatCountry.aspx', 'QAS', '" & ucr.Attribute("addressingWindowAttributes") & "');" & vbCrLf)
                    Response.Write("win1.creator=self;" & vbCrLf)
                    Response.Write("}" & vbCrLf)
                Case Is = "HOPEWISER"
                    ' Create function to open child window
                    Response.Write("function addressingPopup() {" & vbCrLf)
                    Response.Write("win1 = window.open('../../PagesPublic/Hopewiser/HopewiserPostcodeAndCountry.aspx', 'Hopewiser', '" & ucr.Attribute("addressingWindowAttributes") & "');" & vbCrLf)
                    Response.Write("win1.creator=self;" & vbCrLf)
                    Response.Write("}" & vbCrLf)
            End Select

            Dim sAllFields() As String = defaults.AddressingFields.ToString.Split(",")
            Dim count As Integer = 0

            '
            ' Create function to populate the address fields.  This function is called from FlatAddress.aspx.
            Response.Write("function UpdateAddressFields() {" & vbCrLf)

            '
            ' Create local function variables used to indicate whether an address element has already been used.
            Do While count < sAllFields.Length
                Response.Write("var usedHiddenAdr" & count.ToString & " = '';" & vbCrLf)
                count = count + 1
            Loop

            '
            ' Clear all address fields
            If addressLine1RowVisible Then Response.Write("document.forms[0]." & building.UniqueID & ".value = '';" & vbCrLf)
            If addressLine2RowVisible Then Response.Write("document.forms[0]." & Address2.UniqueID & ".value = '';" & vbCrLf)
            If addressLine3RowVisible Then Response.Write("document.forms[0]." & Address3.UniqueID & ".value = '';" & vbCrLf)
            If addressLine4RowVisible Then Response.Write("document.forms[0]." & Address4.UniqueID & ".value = '';" & vbCrLf)
            If addressLine5RowVisible Then Response.Write("document.forms[0]." & Address5.UniqueID & ".value = '';" & vbCrLf)
            If addressPostcodeRowVisible Then Response.Write("document.forms[0]." & postcode.UniqueID & ".value = '';" & vbCrLf)
            If addressCountryRowVisible Then Response.Write("document.forms[0]." & CountryDDL.UniqueID & ".value = '';" & vbCrLf)

            '
            ' If an address field is in use and is defined to contain a QAS address element then create Javascript code to populate correctly.
            If addressLine1RowVisible And Not defaults.AddressingMapAdr1.Trim = "" Then
                sString = GetJavascriptString("document.forms[0]." & building.UniqueID & ".value", defaults.AddressingMapAdr1, defaults.AddressingFields)
                Response.Write(sString)
            End If
            If addressLine2RowVisible And Not defaults.AddressingMapAdr2.Trim = "" Then
                sString = GetJavascriptString("document.forms[0]." & Address2.UniqueID & ".value", defaults.AddressingMapAdr2, defaults.AddressingFields)
                Response.Write(sString)
            End If
            If addressLine3RowVisible And Not defaults.AddressingMapAdr3.Trim = "" Then
                sString = GetJavascriptString("document.forms[0]." & Address3.UniqueID & ".value", defaults.AddressingMapAdr3, defaults.AddressingFields)
                Response.Write(sString)
            End If
            If addressLine4RowVisible And Not defaults.AddressingMapAdr4.Trim = "" Then
                sString = GetJavascriptString("document.forms[0]." & Address4.UniqueID & ".value", defaults.AddressingMapAdr4, defaults.AddressingFields)
                Response.Write(sString)
            End If
            If addressLine5RowVisible And Not defaults.AddressingMapAdr5.Trim = "" Then
                sString = GetJavascriptString("document.forms[0]." & Address5.UniqueID & ".value", defaults.AddressingMapAdr5, defaults.AddressingFields)
                Response.Write(sString)
            End If
            If Not defaults.AddressingMapPost.Trim = "" Then
                sString = GetJavascriptString("document.forms[0]." & postcode.UniqueID & ".value", defaults.AddressingMapPost, defaults.AddressingFields)
                Response.Write(sString)
            End If
            If Not defaults.AddressingMapCountry.Trim = "" Then
                sString = GetJavascriptString("document.forms[0]." & CountryDDL.UniqueID & ".value", defaults.AddressingMapCountry, defaults.AddressingFields)
                Response.Write(sString)
            End If

            Response.Write("}" & vbCrLf)

            Response.Write("function trim(s) { " & vbCrLf & "var r=/\b(.*)\b/.exec(s); " & vbCrLf & "return (r==null)?"""":r[1]; " & vbCrLf & "}")
            Response.Write("</script>" & vbCrLf)
        End If

    End Sub
    Protected Function GetJavascriptString(ByVal sFieldString, ByVal sAddressingMap, ByVal sAddressingFields) As String

        Dim sString As String = String.Empty
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
                        sString = sString & vbCrLf & _
                                "if (trim(" & sStr3 & count2.ToString & ") != 'Y' && trim(" & sStr1 & count2.ToString & sStr2 & ") != '') {" & vbCrLf & _
                                "if (trim(" & sFieldString & ") == '') {" & vbCrLf & _
                                sFieldString & " = " & sStr1 & count2.ToString & sStr2 & ";" & vbCrLf & _
                                "}" & vbCrLf & _
                                "else {" & vbCrLf & _
                                sFieldString & " = " & sFieldString & " + ', ' + " & sStr1 & count2.ToString & sStr2 & ";" & vbCrLf & _
                                "}" & vbCrLf & _
                                sStr3 & count2.ToString & " = 'Y';" & vbCrLf & _
                                "}"
                        Exit Do
                    End If
                    count2 = count2 + 1
                Loop
            End If
            count = count + 1
        Loop

        Return sString

    End Function
    Protected Sub CreateAddressingHiddenFields()

        '
        ' Create hidden fields for each Addressing field defined in defaults.
        Dim defaults As ECommerceModuleDefaults.DefaultValues
        Dim defs As New ECommerceModuleDefaults
        Dim qasFields() As String = Nothing
        Dim count As Integer = 0
        Dim sString As String = String.Empty

        defaults = defs.GetDefaults

        If Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("addressingOnOff")) Then
            qasFields = defaults.AddressingFields.ToString.Split(",")
            Do While count < qasFields.Length
                If count = 0 Then
                    Response.Write(vbCrLf)
                End If
                sString = "<input type=""hidden"" name=""hiddenAdr" & count.ToString & """ value="" "" />"
                Response.Write(sString & vbCrLf)
                count = count + 1
            Loop
        End If

    End Sub



End Class
