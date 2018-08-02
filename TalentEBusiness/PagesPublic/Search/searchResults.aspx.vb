Imports Microsoft.VisualBasic
Imports Talent.eCommerce
Imports Talent.Common
Imports System.Data
Imports System.Data.sqlclient
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Pages Public - Search Results
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      PPSEARE- 
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
Partial Class PagesPublic_searchResults
    Inherits TalentBase01

    Private wfr As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

            Dim queryString As NameValueCollection = HttpContext.Current.Request.QueryString
            Dim i As Integer = 0

            With wfr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = ProfileHelper.GetPageName
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "searchResults.aspx"
                btnSearchAgain.Text = .Content("SearchAgainButtonText", _languageCode, True)
            End With

            ' Hide search again button if not coming from advanced search
            btnSearchAgain.Visible = False
            If Not queryString.Item("source") Is Nothing AndAlso queryString.Item("source").Equals("advanced") Then
                btnSearchAgain.Visible = True
            End If

            Dim moduleDefaults As ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues
            moduleDefaults = New ECommerceModuleDefaults
            def = moduleDefaults.GetDefaults

            Select Case def.Product_List_Graphical_Template_Type
                Case Is = 1
                    ProductSearchResultsList.Visible = True
                    ProductSearchResultsList2.Visible = False

                Case Is = 2, 3
                    ProductSearchResultsList.Visible = False
                    ProductSearchResultsList2.Visible = True

                Case Else
                    ProductSearchResultsList.Visible = True
                    ProductSearchResultsList2.Visible = False

            End Select

        End If

    End Sub

    Protected Sub btnSearchAgain_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearchAgain.Click

        Dim redirectString As New StringBuilder
        Dim queryString As NameValueCollection = HttpContext.Current.Request.QueryString

        redirectString.Append("~/PagesPublic/Search/advancedSearch.aspx")

        Dim i As Integer = 0
        For Each s As String In queryString.AllKeys
            If Not s.Equals("source") Then
                i += 1
                If i = 1 Then
                    redirectString.Append("?")
                Else
                    redirectString.Append("&")
                End If
                redirectString.Append(s & "=" & queryString.Item(s))
            End If
        Next

        Response.Redirect(redirectString.ToString)

    End Sub

End Class
