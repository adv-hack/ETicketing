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
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      UCCORI- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_WebUserControl2
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
            .KeyCode = "CatalogRequest.ascx"
            .PageCode = "CatalogRequest.aspx"
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            promoCodeLabel.Text = .Content("promoCodeLabelText", _languageCode, True)
            proceed.Text = .Content("proceedButtonText", _languageCode, True)
        End With
        'End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

    End Sub

    Protected Sub proceed_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles proceed.Click
        If Not promoCodeTextBox.Text = "" Then
            _promoCode = promoCodeTextBox.Text
            Dim result = checkPromoCode(_businessUnit, _partner, _promoCode)
            If Not result = "NOT_FOUND" Then
                'Promo Code Accepted, get campaign code
                _campaignCode = getCampaignCode(_businessUnit, _partner, _country)
                If Not _campaignCode = "NOT_FOUND" Then
                    orderCatalog()
                End If
            End If
        Else
            'Get Campaign Code
            _campaignCode = getCampaignCode(_businessUnit, _partner, _country)
            If Not _campaignCode = "NOT_FOUND" Then
                orderCatalog()
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

    Protected Sub orderCatalog()

        ' -------------------------------------------------------
        ' Submit Campaign
        ' -------------------------------------------------------
        Dim CAMPAIGNS As New Talent.Common.TalentCampaigns
        Dim settings As New Talent.Common.DESettings
        Dim DeCampaign As New Talent.Common.DECampaignDetails
        Dim err As Talent.Common.ErrorObj = Nothing
        Dim ds1 As New DataSet

        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
        settings.BusinessUnit = TalentCache.GetBusinessUnit()
        settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
        ' Caching not valid for creating a catalog
        ' settings.Cacheing = CType(ucr.Attribute("CampaignCacheing"), Boolean)
        ' settings.CacheTimeMinutes = CType(ucr.Attribute("CampaignCacheTimeMinutes"), Integer)
        settings.Cacheing = False

        CAMPAIGNS.Settings = settings
        DeCampaign.CampaignCode = _campaignCode
        CAMPAIGNS.De = DeCampaign

        Dim dtCampaign As New DataTable

        err = CAMPAIGNS.WriteCampaign
        ''  ds1 = CAMPAIGNS.ResultDataSet

        'dtCampaign = ds1.Tables(1)

        ' Dim count = 0

        'Do While count < dtCampaign.Rows.Count
        'something here
        'count = count + 1
        'Loop

        'once complete, forward the user to CatalogRequestConfirmation.aspx
        Response.Redirect("CatalogRequestConfirmation.aspx")
  
    End Sub

End Class
