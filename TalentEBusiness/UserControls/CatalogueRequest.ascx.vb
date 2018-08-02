Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.eCommerce
Imports Talent.Common
Imports System.Xml
Imports System.Globalization
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Copyright
'
'       Date                        Feb 2007
'
'       Author                       
'
'       CS Group 2007               All rights reserved.
'
'       Error Number Code base      UCCORI- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_CatalogueRequest
    Inherits ControlBase

    Private _usage As String = Talent.Common.Utilities.GetAllString
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private ucr As New Talent.Common.UserControlResource
    Private _businessUnit As String = TalentCache.GetBusinessUnit()
    Private _partner As String = TalentCache.GetPartner(HttpContext.Current.Profile)
    Private _country As String = TalentCache.GetDefaultCountryForBU
    Private _promoCode As String
    Private _campaignCode As String

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        'If Not Page.IsPostBack Then
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
            .KeyCode = "CatalogueRequest.ascx"
            .PageCode = "CatalogueRequest.aspx"
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            promoCodeLabel.Text = .Content("promoCodeLabelText", _languageCode, True)
            proceed.Text = .Content("proceedButtonText", _languageCode, True)
        End With
        'End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

    End Sub

    Protected Sub proceed_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles proceed.Click

        Dim defaultCampaignResult As String = ucr.Attribute("defaultCampaignResult")
        Dim defaultCampaignEventCode As String = ucr.Attribute("defaultCampaignEventCode")

        Dim address As TalentProfileAddress = ProfileHelper.ProfileAddressEnumerator(0, Profile.User.Addresses)

        If Not address.Country Is Nothing AndAlso address.Country <> String.Empty Then
            _country = address.Country
        End If

        If Not promoCodeTextBox.Text = "" Then
            '-----------------------
            ' Promotion code entered
            '-----------------------
            _promoCode = promoCodeTextBox.Text
            Dim result = checkPromoCode(_businessUnit, _partner, _promoCode)
            If result = "NOT_FOUND" Then

                _campaignCode = getCampaignCode(_businessUnit, _partner, _country)
                If Not _campaignCode = "NOT_FOUND" Then
                    orderCatalog(_campaignCode, defaultCampaignEventCode, defaultCampaignResult)
                End If

            Else
                'Promo Code Accepted, get campaign code

                ' Use character portion as campaign code
                ' Use numeric portion of promo code as event code
                Dim i As Integer = 0
                Do While (i < _promoCode.Trim.Length AndAlso Not IsNumeric(_promoCode.Substring(i, 1)))
                    i = i + 1
                Loop
                orderCatalog(_promoCode.Substring(0, i), _promoCode.Substring(i, _promoCode.Length - i), defaultCampaignResult)

            End If
        Else
            '--------------------------
            ' No promotion code entered
            '--------------------------
            'Get Campaign Code
            _campaignCode = getCampaignCode(_businessUnit, _partner, _country)
            If Not _campaignCode = "NOT_FOUND" Then
                orderCatalog(_campaignCode, defaultCampaignEventCode, defaultCampaignResult)
            End If
        End If
    End Sub

    Private Function checkPromoCode(ByVal BUSINESS_UNIT As String, ByVal PARTNER_CODE As String, ByVal PROMO_CODE As String) As String

        Dim myData As New TalentCatalogPromoCodesTableAdapters.tbl_catalog_promo_codesTableAdapter 

        Dim dt As Data.DataTable = myData.CheckPromoCode(BUSINESS_UNIT, PARTNER_CODE, PROMO_CODE)

        If (dt.Rows.Count > 0) Then
            Return dt.Rows(0)("PROMO_CODE").ToString.Trim
        Else
            dt = New Data.DataTable
            dt = myData.CheckPromoCode(BUSINESS_UNIT, "*ALL", PROMO_CODE)
            If (dt.Rows.Count > 0) Then
                Return dt.Rows(0)("PROMO_CODE").ToString.Trim
            Else
                dt = New Data.DataTable
                dt = myData.CheckPromoCode("*ALL", "*ALL", PROMO_CODE)
                If (dt.Rows.Count > 0) Then
                    Return dt.Rows(0)("PROMO_CODE").ToString.Trim
                Else
                    Return "NOT_FOUND"
                End If
            End If
        End If
    End Function

    Private Function getCampaignCode(ByVal BUSINESS_UNIT As String, ByVal PARTNER_CODE As String, ByVal COUNTRY_CODE As String) As String

        Dim myData As New TalentCampaignCodesTableAdapters.tbl_campaign_codesTableAdapter 

        Dim dt As Data.DataTable = myData.getCampaignCode(BUSINESS_UNIT, PARTNER_CODE, COUNTRY_CODE)

        If (dt.Rows.Count > 0) Then
            Return dt.Rows(0)("CAMPAIGN_CODE").ToString.Trim
        Else
            dt = New Data.DataTable
            dt = myData.getCampaignCode(BUSINESS_UNIT, "*ALL", COUNTRY_CODE)
            If (dt.Rows.Count > 0) Then
                Return dt.Rows(0)("CAMPAIGN_CODE").ToString.Trim
            Else
                dt = New Data.DataTable
                dt = myData.getCampaignCode("*ALL", "*ALL", COUNTRY_CODE)
                If (dt.Rows.Count > 0) Then
                    Return dt.Rows(0)("CAMPAIGN_CODE").ToString.Trim
                Else
                    Return "NOT_FOUND"
                End If
            End If
        End If
    End Function

    Protected Sub orderCatalog(ByVal parmCampaignCode As String, ByVal parmCampaignEventCode As String, ByVal parmCampaignResult As String)

        ' -------------------------------------------------------
        ' Submit Campaign
        ' -------------------------------------------------------
        'Dim CAMPAIGNS As New Talent.Common.TalentCampaigns
        'Dim settings As New Talent.Common.DESettings
        'Dim DeCampaign As New Talent.Common.DECampaignDetails
        'Dim err As Talent.Common.ErrorObj = Nothing
        'Dim ds1 As New DataSet

        'settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
        'settings.BusinessUnit = TalentCache.GetBusinessUnit()
        'settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
        '' Caching not valid for creating a catalog
        '' settings.Cacheing = CType(ucr.Attribute("CampaignCacheing"), Boolean)
        '' settings.CacheTimeMinutes = CType(ucr.Attribute("CampaignCacheTimeMinutes"), Integer)
        'settings.Cacheing = False

        'CAMPAIGNS.Settings = settings
        'DeCampaign.CampaignCode = _campaignCode
        'CAMPAIGNS.De = DeCampaign

        'Dim dtCampaign As New DataTable

        '      err = CAMPAIGNS.WriteCampaign

        '----------------
        ' Try as customer
        '----------------
        Dim deCustV11 As New DECustomerV11
        Dim deCustV1 As New DECustomer
        Dim myCustomer As New TalentCustomer
        deCustV11.DECustomersV1.Add(deCustV1)

        Dim myErrorObj As New ErrorObj
        Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
        Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults

        With myCustomer
            ' Reset the settings entity to a customer specific settings entity 
            .Settings = CType(New DECustomerSettings, DESettings)

            .DeV11 = deCustV11
            With deCustV1
                .Action = ""
                .ThirdPartyContactRef = Profile.User.Details.User_Number
                .ThirdPartyCompanyRef2 = ""
                .DateFormat = "1"

                If CType(def.LoginidIsCustomerNumber, Boolean) Then
                    .CustomerNumber = Profile.User.Details.LoginID
                End If
                Dim defAddress As TalentProfileAddress = ProfileHelper.ProfileAddressEnumerator(0, Profile.User.Addresses)
                .CompanySLNumber1 = Profile.PartnerInfo.Details.Account_No_1
                .CompanySLNumber2 = Profile.PartnerInfo.Details.Account_No_2
                .CompanyName = defAddress.Address_Line_1
                .AddressLine1 = defAddress.Address_Line_2
                .AddressLine2 = defAddress.Address_Line_3
                .AddressLine3 = defAddress.Address_Line_4
                .AddressLine4 = defAddress.Address_Line_5
                .AddressLine5 = defAddress.Country
                '    .PostCode = defAddress.Country
                .PostCode = defAddress.Post_Code
                .ProcessCompanySLNumber1 = "1"
                .ProcessCompanySLNumber2 = "1"
                .ProcessCompanyName = "1"
                .ProcessAddressLine1 = "1"
                .ProcessAddressLine2 = "1"
                .ProcessAddressLine3 = "1"
                .ProcessAddressLine4 = "1"
                .ProcessAddressLine5 = "1"
                .ProcessPostCode = "1"

                .CampaignCode = parmCampaignCode
                .CampaignResult = parmCampaignResult
                .CampaignEventCode = parmCampaignEventCode


                .ContactSurname = Profile.User.Details.Surname

                .ProcessCampaignCode = "1"
                .ProcessCampaignEventCode = "1"
                .ProcessCampaignResult = "1"
                .ProcessContactSurname = "1"
            End With

            Dim decs As New DECustomerSettings()
            decs = CType(.Settings, DECustomerSettings)
            .Settings = CType(decs, DESettings)

            With .Settings
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .BusinessUnit = TalentCache.GetBusinessUnit
                '.Company = ucr.Content("crmExternalKeyName", _languageCode, True)
                .Company = "EBUSINESS"
                .Cacheing = False

                .DestinationDatabase = Talent.eCommerce.Utilities.GetCustomerDestinationDatabase()
                .BackOfficeConnectionString = ConfigurationManager.ConnectionStrings("TALENTCRM").ToString
                .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
                .RetryFailures = def.RegistrationRetry
                .RetryAttempts = def.RegistrationRetryAttempts
                .RetryWaitTime = def.RegistrationRetryWait
                .RetryErrorNumbers = def.RegistrationRetryErrors
            End With
            myErrorObj = .SetCustomer()
        End With
        'once complete, forward the user to CatalogRequestConfirmation.aspx
        If myErrorObj.HasError Then
            Response.Redirect("CatalogueRequestConfirmation.aspx?CatalogueRequest=failed")
        Else
            Response.Redirect("CatalogueRequestConfirmation.aspx?CatalogueRequest=success")
        End If

    End Sub

End Class
