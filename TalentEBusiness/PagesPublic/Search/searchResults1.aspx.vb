Imports Microsoft.VisualBasic
Imports Talent.eCommerce
Imports Talent.Common
Imports System.Data
Imports System.Data.sqlclient
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Pages Public - Search Results Ocelluz
'
'       Date                        050907
'
'       Author                      Ben Ford
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      PPSEARO- 
'         
'       User Controls
'           searchResults
'
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class PagesPublic_searchResults1
    Inherits TalentBase01

    Private wfr As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim moduleDefaults As New ECommerceModuleDefaults
        Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults

        If Not Page.IsPostBack Then

            With wfr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = ProfileHelper.GetPageName
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "searchResults1.aspx"
            End With
        End If

        iframeOcelluz.Visible = True

        Dim searchCriteria As String = ""

        ' Set Search Products List Control Properties from request

        ' Take search text from posted form 
        'If Not Request.Form("txtProductSearch") Is Nothing Then
        '    searchCriteria = Request.Form("txtProductSearch")
        '    ' Save the search criteria for paging 
        '    If Not IsPostBack Then
        '        Session("saveSearchCriteria") = searchCriteria
        '    End If
        'Else
        '    If Not Session("saveSearchCriteria") Is Nothing Then
        '        searchCriteria = Session("saveSearchCriteria").ToString
        '    End If

        'End If
        Dim queryString As NameValueCollection = HttpContext.Current.Request.QueryString
        Dim i As Integer = 1
        Dim searchString As New StringBuilder
        Dim profile As String = String.Empty
        Dim priceList As String = String.Empty
        '-------------------------------------------------------
        ' Testing - if running off local host then force profile
        '-------------------------------------------------------
        If Request.Url.ToString.Substring(0, 16) = "http://localhost" Then
            profile = "ade_eng"
            priceList = "001"
        Else
            profile = TalentCache.GetBusinessUnit().Trim & "_" & Talent.Common.Utilities.GetDefaultLanguage()
            priceList = def.PriceList
        End If

        With searchString
            .Append("?profile=").Append(profile)
            .Append("&country=").Append("UK")
            .Append("&businessunit=").Append(TalentCache.GetBusinessUnit)
            .Append("&pricelist=").Append("001")
            .Append("&query=")
            ' pricelist=001&amp;query=leather%20bag"

        End With

        While Not queryString.Item("keyword" & i.ToString) Is Nothing
            searchString.Append(queryString.Item("keyword" & i.ToString).Trim).Append("%20")
            i += 1
        End While


        Dim strIFrameUrl As String = String.Empty
        'Dim sbIFrameUrl As New StringBuilder
        'Dim strProfile As String = Session("appCode") & "_" & Session("language").ToString
        ''     strProfile = strProfile.ToLowre
        'With sbIFrameUrl
        '    .Append(ConfigurationManager.AppSettings("nonSecureURL").ToString)
        '    .Append("/").Append(strProfile).Append("/default.aspx")
        '    .Append("?profile=").Append(strProfile)
        '    .Append("&country=").Append(Session("country").ToString.Trim).Append("&pricelist=")
        '    .Append(Session("priceList").ToString.Trim).Append("&query=")
        '    .Append(searchCriteria.Trim.Replace(" ", "%20"))
        'End With


        strIFrameUrl = "http://store.dunhill.com/ade_eng/default.aspx" & searchString.ToString
        '--------------------------------------------------------------------------
        ' Check Virtual Path maps to real path exists otherwist go to normal search
        '--------------------------------------------------------------------------
        'Dim vp As String = "/" & strProfile & "/default.aspx"
        'If Not File.Exists(Server.MapPath(vp)) Then
        '    Session("saveSearchCriteria") = String.Empty
        '    Response.Redirect("../pagesPublic/search.aspx")
        'End If

        iframeOcelluz.Attributes("src") = strIFrameUrl

        'iframeOcelluz.Attributes("src") = "http://ocelluzdemo.dnsalias.com/dunhill/default.aspx?profile=prod&query=" & _
        '                                        searchCriteria.Trim.Replace(" ", "%20")


    End Sub


End Class
